/*
   Copyright 2012-2022 Marco De Salvo

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

     http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.
*/

using NetTopologySuite.Geometries;
using RDFSharp.Model;
using RDFSharp.Query;
using System.Collections.Generic;
using System.Linq;

namespace RDFSharp.Semantics.Extensions.GEO
{
    /// <summary>
    /// SpatialHelper represents an helper for common spatial analysis on features
    /// </summary>
    public class GEOSpatialHelper
    {
        #region Properties
        /// <summary>
        /// The wrapped ontology on which performing spatial analysis
        /// </summary>
        internal GEOOntology Ontology { get; set; }
        #endregion

        #region Ctors
        /// <summary>
        /// Default-ctor to build a geospatial helper on the given ontology
        /// </summary>
        internal GEOSpatialHelper(GEOOntology ontology)
            => Ontology = ontology;
        #endregion

        #region Methods
        /// <summary>
        /// Gets the distance, expressed in meters, between the given features
        /// </summary>
        public double? GetDistanceBetweenFeatures(RDFResource fromFeatureUri, RDFResource toFeatureUri)
        {
            if (fromFeatureUri == null)
                throw new OWLSemanticsException("Cannot get distance between features because given \"fromFeatureUri\" parameter is null");
            if (toFeatureUri == null)
                throw new OWLSemanticsException("Cannot get distance between features because given \"toFeatureUri\" parameter is null");

            //Collect geometries of "From" feature
            (Geometry, Geometry) defaultGeometryOfFromFeature = Ontology.GetDefaultGeometryOfFeature(fromFeatureUri);
            List<(Geometry, Geometry)> secondaryGeometriesOfFromFeature = Ontology.GetSecondaryGeometriesOfFeature(fromFeatureUri);
            if (defaultGeometryOfFromFeature.Item1 != null && defaultGeometryOfFromFeature.Item2 != null)
                secondaryGeometriesOfFromFeature.Add(defaultGeometryOfFromFeature);

            //Collect geometries of "To" feature
            (Geometry, Geometry) defaultGeometryOfToFeature = Ontology.GetDefaultGeometryOfFeature(toFeatureUri);
            List<(Geometry, Geometry)> secondaryGeometriesOfToFeature = Ontology.GetSecondaryGeometriesOfFeature(toFeatureUri);
            if (defaultGeometryOfToFeature.Item1 != null && defaultGeometryOfToFeature.Item2 != null)
                secondaryGeometriesOfToFeature.Add(defaultGeometryOfToFeature);

            //Perform spatial analysis between collected geometries:
            //iterate from/to geometries and calibrate minimal distance
            double? featuresDistance = double.MaxValue;
            secondaryGeometriesOfFromFeature.ForEach(fromGeom => {
                secondaryGeometriesOfToFeature.ForEach(toGeom => {
                    double tempDistance = fromGeom.Item2.Distance(toGeom.Item2);
                    if (tempDistance < featuresDistance)
                        featuresDistance = tempDistance;
                });
            });

            //Give null in case distance could not be calculated (no available geometries from any sides)
            return featuresDistance == double.MaxValue ? null : featuresDistance;
        }

        /// <summary>
        /// Gets the features around the given WGS84 Lon/Lat point in a radius of given meters 
        /// </summary>
        public List<RDFResource> GetFeaturesNearPoint((double,double) wgs84LonLat, double radiusMeters)
        {
            if (wgs84LonLat.Item1 < -180 || wgs84LonLat.Item1 > 180)
                throw new OWLSemanticsException("Cannot get features near point because given \"wgs84LonLat\" parameter has not a valid longitude for WGS84");
            if (wgs84LonLat.Item2 < -90 || wgs84LonLat.Item2 > 90)
                throw new OWLSemanticsException("Cannot get features near point because given \"wgs84LonLat\" parameter has not a valid latitude for WGS84");

            //Create WGS84 geometry from given center of search
            Geometry wgs84SearchPoint = new Point(wgs84LonLat.Item1, wgs84LonLat.Item2) { SRID = 4326 };

            //Create UTM geometry from given center of search
            (int, bool) utmZoneSearchPoint = GEOConverter.GetUTMZoneFromWGS84Coordinates(wgs84LonLat.Item1, wgs84LonLat.Item2);
            Geometry utmSearchPoint = GEOConverter.GetUTMGeometryFromWGS84(wgs84SearchPoint, utmZoneSearchPoint);

            //Execute SPARQL query to retrieve WKT/GML serialization of features having geometries
            List<(RDFResource, Geometry, Geometry)> featuresWithGeometry = Ontology.GetFeaturesWithGeometries();

            //Perform spatial analysis between collected geometries: iterate geometries and collect those within given radius
            List<RDFResource> featuresNearPoint = new List<RDFResource>();
            featuresWithGeometry.ForEach(featureWithGeometry => {
                if (featureWithGeometry.Item3.IsWithinDistance(utmSearchPoint, radiusMeters))
                    featuresNearPoint.Add(featureWithGeometry.Item1);
            });

            return RDFQueryUtilities.RemoveDuplicates(featuresNearPoint);
        }

        /// <summary>
        /// Gets the features located north of the given WGS84 Lon/Lat point
        /// </summary>
        public List<RDFResource> GetFeaturesNorthOfPoint((double,double) wgs84LonLat)
        {
            if (wgs84LonLat.Item1 < -180 || wgs84LonLat.Item1 > 180)
                throw new OWLSemanticsException("Cannot get features north of point because given \"wgs84LonLat\" parameter has not a valid longitude for WGS84");
            if (wgs84LonLat.Item2 < -90 || wgs84LonLat.Item2 > 90)
                throw new OWLSemanticsException("Cannot get features north of point because given \"wgs84LonLat\" parameter has not a valid latitude for WGS84");

            //Create WGS84 geometry from given point
            Geometry wgs84SearchPoint = new Point(wgs84LonLat.Item1, wgs84LonLat.Item2) { SRID = 4326 };

            //Create UTM geometry from given point
            (int, bool) utmZoneSearchPoint = GEOConverter.GetUTMZoneFromWGS84Coordinates(wgs84LonLat.Item1, wgs84LonLat.Item2);
            Geometry utmSearchPoint = GEOConverter.GetUTMGeometryFromWGS84(wgs84SearchPoint, utmZoneSearchPoint);

            //Execute SPARQL query to retrieve WKT/GML serialization of features having geometries
            List<(RDFResource, Geometry, Geometry)> featuresWithGeometry = Ontology.GetFeaturesWithGeometries();

            //Perform spatial analysis between collected geometries: iterate geometries and collect those having latitudes higher than given point
            List<RDFResource> featuresNorthOfPoint = new List<RDFResource>();
            featuresWithGeometry.ForEach(featureWithGeometry => {
                if (featureWithGeometry.Item3.Coordinates.Any(coordinate => coordinate.Y > utmSearchPoint.Coordinate.Y))
                    featuresNorthOfPoint.Add(featureWithGeometry.Item1);
            });

            return RDFQueryUtilities.RemoveDuplicates(featuresNorthOfPoint);
        }

        /// <summary>
        /// Gets the features located south of the given WGS84 Lon/Lat point
        /// </summary>
        public List<RDFResource> GetFeaturesSouthOfPoint((double,double) wgs84LonLat)
        {
            if (wgs84LonLat.Item1 < -180 || wgs84LonLat.Item1 > 180)
                throw new OWLSemanticsException("Cannot get features south of point because given \"wgs84LonLat\" parameter has not a valid longitude for WGS84");
            if (wgs84LonLat.Item2 < -90 || wgs84LonLat.Item2 > 90)
                throw new OWLSemanticsException("Cannot get features south of point because given \"wgs84LonLat\" parameter has not a valid latitude for WGS84");

            //Create WGS84 geometry from given point
            Geometry wgs84SearchPoint = new Point(wgs84LonLat.Item1, wgs84LonLat.Item2) { SRID = 4326 };

            //Create UTM geometry from given point
            (int, bool) utmZoneSearchPoint = GEOConverter.GetUTMZoneFromWGS84Coordinates(wgs84LonLat.Item1, wgs84LonLat.Item2);
            Geometry utmSearchPoint = GEOConverter.GetUTMGeometryFromWGS84(wgs84SearchPoint, utmZoneSearchPoint);

            //Execute SPARQL query to retrieve WKT/GML serialization of features having geometries
            List<(RDFResource, Geometry, Geometry)> featuresWithGeometry = Ontology.GetFeaturesWithGeometries();

            //Perform spatial analysis between collected geometries: iterate geometries and collect those having latitudes lower than given point
            List<RDFResource> featuresNorthOfPoint = new List<RDFResource>();
            featuresWithGeometry.ForEach(featureWithGeometry => {
                if (featureWithGeometry.Item3.Coordinates.Any(coordinate => coordinate.Y < utmSearchPoint.Coordinate.Y))
                    featuresNorthOfPoint.Add(featureWithGeometry.Item1);
            });

            return RDFQueryUtilities.RemoveDuplicates(featuresNorthOfPoint);
        }

        /// <summary>
        /// Gets the features within the given box represented by WGS84 Lon/Lat (lower-left, upper-right) corner points
        /// </summary>
        public List<RDFResource> GetFeaturesWithinBox((double,double) wgs84LonLat_LowerLeft, (double,double) wgs84LonLat_UpperRight)
        {
            if (wgs84LonLat_LowerLeft.Item1 < -180 || wgs84LonLat_LowerLeft.Item1 > 180)
                throw new OWLSemanticsException("Cannot get features within box because given \"wgs84LonMin\" parameter is not a valid longitude for WGS84");
            if (wgs84LonLat_LowerLeft.Item2 < -90 || wgs84LonLat_LowerLeft.Item2 > 90)
                throw new OWLSemanticsException("Cannot get features within box because given \"wgs84LatMin\" parameter is not a valid latitude for WGS84");
            if (wgs84LonLat_UpperRight.Item1 < -180 || wgs84LonLat_UpperRight.Item1 > 180)
                throw new OWLSemanticsException("Cannot get features within box because given \"wgs84LonMax\" parameter is not a valid longitude for WGS84");
            if (wgs84LonLat_UpperRight.Item2 < -90 || wgs84LonLat_UpperRight.Item2 > 90)
                throw new OWLSemanticsException("Cannot get features within box because given \"wgs84LatMax\" parameter is not a valid latitude for WGS84");
            if (wgs84LonLat_LowerLeft.Item1 >= wgs84LonLat_UpperRight.Item1)
                throw new OWLSemanticsException("Cannot get features within box because given \"wgs84LonMin\" parameter must be lower than given \"wgs84LonMax\" parameter");
            if (wgs84LonLat_LowerLeft.Item2 >= wgs84LonLat_UpperRight.Item2)
                throw new OWLSemanticsException("Cannot get features within box because given \"wgs84LatMin\" parameter must be lower than given \"wgs84LatMax\" parameter");

            //Create WGS84 geometry from given box corners
            Geometry wgs84SearchBox = new Polygon(new LinearRing(new Coordinate[] {
                new Coordinate(wgs84LonLat_LowerLeft.Item1, wgs84LonLat_LowerLeft.Item2),
                new Coordinate(wgs84LonLat_UpperRight.Item1, wgs84LonLat_LowerLeft.Item2),
                new Coordinate(wgs84LonLat_UpperRight.Item1, wgs84LonLat_UpperRight.Item2),
                new Coordinate(wgs84LonLat_LowerLeft.Item1, wgs84LonLat_UpperRight.Item2),
                new Coordinate(wgs84LonLat_LowerLeft.Item1, wgs84LonLat_LowerLeft.Item2)
            })) { SRID = 4326 };

            //Create UTM geometry from given box corners
            (int, bool) utmZoneSearchBox = GEOConverter.GetUTMZoneFromWGS84Coordinates(wgs84LonLat_LowerLeft.Item1, wgs84LonLat_LowerLeft.Item2);
            Geometry utmSearchBox = GEOConverter.GetUTMGeometryFromWGS84(wgs84SearchBox, utmZoneSearchBox);

            //Execute SPARQL query to retrieve WKT/GML serialization of features having geometries
            List<(RDFResource, Geometry, Geometry)> featuresWithGeometry = Ontology.GetFeaturesWithGeometries();

            //Perform spatial analysis between collected geometries:
            //iterate geometries and collect those within given box
            List<RDFResource> featuresWithinBox = new List<RDFResource>();
            featuresWithGeometry.ForEach(featureWithGeometry => {
                if (utmSearchBox.Contains(featureWithGeometry.Item3))
                    featuresWithinBox.Add(featureWithGeometry.Item1);
            });

            return RDFQueryUtilities.RemoveDuplicates(featuresWithinBox);
        }
        #endregion
    }
}