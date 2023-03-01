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
using NetTopologySuite.IO;
using NetTopologySuite.IO.GML2;
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
        /// Reader for WKT spatial representation
        /// </summary>
        internal static WKTReader WKTReader = new WKTReader();

        /// <summary>
        /// Writer for WKT spatial representation
        /// </summary>
        internal static WKTWriter WKTWriter = new WKTWriter();

        /// <summary>
        /// Reader for GML spatial representation
        /// </summary>
        internal static GMLReader GMLReader = new GMLReader();

        /// <summary>
        /// Writer for GML spatial representation
        /// </summary>
        internal static GMLWriter GMLWriter = new GMLWriter();

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
            (Geometry,Geometry) defaultGeometryOfFromFeature = Ontology.GetDefaultGeometryOfFeature(fromFeatureUri);
            List<(Geometry,Geometry)> secondaryGeometriesOfFromFeature = Ontology.GetSecondaryGeometriesOfFeature(fromFeatureUri);
            if (defaultGeometryOfFromFeature.Item1 != null && defaultGeometryOfFromFeature.Item2 != null)
                secondaryGeometriesOfFromFeature.Add(defaultGeometryOfFromFeature);

            //Collect geometries of "To" feature
            (Geometry,Geometry) defaultGeometryOfToFeature = Ontology.GetDefaultGeometryOfFeature(toFeatureUri);
            List<(Geometry,Geometry)> secondaryGeometriesOfToFeature = Ontology.GetSecondaryGeometriesOfFeature(toFeatureUri);
            if (defaultGeometryOfToFeature.Item1 != null && defaultGeometryOfToFeature.Item2 != null)
                secondaryGeometriesOfToFeature.Add(defaultGeometryOfToFeature);

            //Perform spatial analysis between collected geometries (calibrate minimum distance)
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
        /// Gets the length, expressed in meters, of the given feature (the perimeter in case of area)
        /// </summary>
        public double? GetLengthOfFeature(RDFResource featureUri)
        {
            if (featureUri == null)
                throw new OWLSemanticsException("Cannot get length of feature because given \"featureUri\" parameter is null");

            //Collect geometries of feature
            (Geometry,Geometry) defaultGeometryOfFeature = Ontology.GetDefaultGeometryOfFeature(featureUri);
            List<(Geometry,Geometry)> secondaryGeometriesOfFeature = Ontology.GetSecondaryGeometriesOfFeature(featureUri);
            if (defaultGeometryOfFeature.Item1 != null && defaultGeometryOfFeature.Item2 != null)
                secondaryGeometriesOfFeature.Add(defaultGeometryOfFeature);

            //Perform spatial analysis between collected geometries (calibrate maximum length)
            double? featureLength = double.MinValue;
            secondaryGeometriesOfFeature.ForEach(geom => {
                double tempLength = geom.Item2.Length;
                if (tempLength > featureLength)
                    featureLength = tempLength;
            });

            //Give null in case length could not be calculated (no available geometries)
            return featureLength == double.MinValue ? null : featureLength;
        }

        /// <summary>
        /// Gets the area, expressed in square meters, of the given feature
        /// </summary>
        public double? GetAreaOfFeature(RDFResource featureUri)
        {
            if (featureUri == null)
                throw new OWLSemanticsException("Cannot get area of feature because given \"featureUri\" parameter is null");

            //Collect geometries of feature
            (Geometry, Geometry) defaultGeometryOfFeature = Ontology.GetDefaultGeometryOfFeature(featureUri);
            List<(Geometry, Geometry)> secondaryGeometriesOfFeature = Ontology.GetSecondaryGeometriesOfFeature(featureUri);
            if (defaultGeometryOfFeature.Item1 != null && defaultGeometryOfFeature.Item2 != null)
                secondaryGeometriesOfFeature.Add(defaultGeometryOfFeature);

            //Perform spatial analysis between collected geometries (calibrate maximum area)
            double? featureArea = double.MinValue;
            secondaryGeometriesOfFeature.ForEach(geom => {
                double tempArea = geom.Item2.Area;
                if (tempArea > featureArea)
                    featureArea = tempArea;
            });

            //Give null in case area could not be calculated (no available geometries)
            return featureArea == double.MinValue ? null : featureArea;
        }

        /// <summary>
        /// Calculates geof:boundary of the given feature, giving a WGS84 Lon/Lat geometry expressed as WKT typed literal
        /// </summary>
        public RDFTypedLiteral GetBoundaryOfFeature(RDFResource featureUri)
        {
            if (featureUri == null)
                throw new OWLSemanticsException("Cannot get geof:boundary of feature because given \"featureUri\" parameter is null");

            //Analyze default geometry of feature
            (Geometry,Geometry) defaultGeometryOfFeature = Ontology.GetDefaultGeometryOfFeature(featureUri);
            if (defaultGeometryOfFeature.Item1 != null && defaultGeometryOfFeature.Item2 != null)
            {
                Geometry boundaryGeometryAZ = defaultGeometryOfFeature.Item2.Boundary;
                Geometry boundaryGeometryWGS84 = GEOConverter.GetWGS84GeometryFromLambertAzimuthal(boundaryGeometryAZ);
                string wktBoundaryGeometryWGS84 = WKTWriter.Write(boundaryGeometryWGS84)
                                                    .Replace("LINEARRING", "LINESTRING");
                return new RDFTypedLiteral(wktBoundaryGeometryWGS84, RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT);
            }

            //Analyze secondary geometries of feature: if any, just work on the first available
            List<(Geometry,Geometry)> secondaryGeometriesOfFeature = Ontology.GetSecondaryGeometriesOfFeature(featureUri);
            if (secondaryGeometriesOfFeature.Any())
            {
                Geometry boundaryGeometryAZ = secondaryGeometriesOfFeature.First().Item2.Boundary;
                Geometry boundaryGeometryWGS84 = GEOConverter.GetWGS84GeometryFromLambertAzimuthal(boundaryGeometryAZ);
                string wktBoundaryGeometryWGS84 = WKTWriter.Write(boundaryGeometryWGS84)
                                                    .Replace("LINEARRING", "LINESTRING");
                return new RDFTypedLiteral(wktBoundaryGeometryWGS84, RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT);
            }

            return null;
        }

        /// <summary>
        /// Gets the features near the given WGS84 Lon/Lat point within a radius of given meters 
        /// </summary>
        public List<RDFResource> GetFeaturesNearPoint((double,double) wgs84LonLat, double radiusMeters)
        {
            if (wgs84LonLat.Item1 < -180 || wgs84LonLat.Item1 > 180)
                throw new OWLSemanticsException("Cannot get features near point because given \"wgs84LonLat\" parameter has not a valid longitude for WGS84");
            if (wgs84LonLat.Item2 < -90 || wgs84LonLat.Item2 > 90)
                throw new OWLSemanticsException("Cannot get features near point because given \"wgs84LonLat\" parameter has not a valid latitude for WGS84");

            //Create WGS84 geometry from given center of search
            Geometry wgs84SearchPoint = new Point(wgs84LonLat.Item1, wgs84LonLat.Item2) { SRID = 4326 };

            //Create Lambert Azimuthal geometry from given center of search
            Geometry lazSearchPoint = GEOConverter.GetLambertAzimuthalGeometryFromWGS84(wgs84SearchPoint);

            //Execute SPARQL query to retrieve WKT/GML serialization of features having geometries
            List<(RDFResource,Geometry,Geometry)> featuresWithGeometry = Ontology.GetFeaturesWithGeometries();

            //Perform spatial analysis between collected geometries:
            //iterate geometries and collect those within given radius
            List<RDFResource> featuresNearPoint = new List<RDFResource>();
            featuresWithGeometry.ForEach(featureWithGeometry => {
                if (featureWithGeometry.Item3.IsWithinDistance(lazSearchPoint, radiusMeters))
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

            //Execute SPARQL query to retrieve WKT/GML serialization of features having geometries
            List<(RDFResource,Geometry,Geometry)> featuresWithGeometry = Ontology.GetFeaturesWithGeometries();

            //Perform spatial analysis between collected geometries:
            //iterate geometries and collect those having latitudes higher than given point
            List<RDFResource> featuresNorthOfPoint = new List<RDFResource>();
            featuresWithGeometry.ForEach(featureWithGeometry => {
                if (featureWithGeometry.Item2.Coordinates.Any(c => c.Y > wgs84SearchPoint.Coordinate.Y))
                    featuresNorthOfPoint.Add(featureWithGeometry.Item1);
            });

            return RDFQueryUtilities.RemoveDuplicates(featuresNorthOfPoint);
        }

        /// <summary>
        /// Gets the features located east of the given WGS84 Lon/Lat point
        /// </summary>
        public List<RDFResource> GetFeaturesEastOfPoint((double,double) wgs84LonLat)
        {
            if (wgs84LonLat.Item1 < -180 || wgs84LonLat.Item1 > 180)
                throw new OWLSemanticsException("Cannot get features east of point because given \"wgs84LonLat\" parameter has not a valid longitude for WGS84");
            if (wgs84LonLat.Item2 < -90 || wgs84LonLat.Item2 > 90)
                throw new OWLSemanticsException("Cannot get features east of point because given \"wgs84LonLat\" parameter has not a valid latitude for WGS84");

            //Create WGS84 geometry from given point
            Geometry wgs84SearchPoint = new Point(wgs84LonLat.Item1, wgs84LonLat.Item2) { SRID = 4326 };

            //Execute SPARQL query to retrieve WKT/GML serialization of features having geometries
            List<(RDFResource,Geometry,Geometry)> featuresWithGeometry = Ontology.GetFeaturesWithGeometries();

            //Perform spatial analysis between collected geometries:
            //iterate geometries and collect those having longitudes greater than given point
            List<RDFResource> featuresEastOfPoint = new List<RDFResource>();
            featuresWithGeometry.ForEach(featureWithGeometry => {
                if (featureWithGeometry.Item2.Coordinates.Any(c => c.X > wgs84SearchPoint.Coordinate.X))
                    featuresEastOfPoint.Add(featureWithGeometry.Item1);
            });

            return RDFQueryUtilities.RemoveDuplicates(featuresEastOfPoint);
        }

        /// <summary>
        /// Gets the features located west of the given WGS84 Lon/Lat point
        /// </summary>
        public List<RDFResource> GetFeaturesWestOfPoint((double,double) wgs84LonLat)
        {
            if (wgs84LonLat.Item1 < -180 || wgs84LonLat.Item1 > 180)
                throw new OWLSemanticsException("Cannot get features west of point because given \"wgs84LonLat\" parameter has not a valid longitude for WGS84");
            if (wgs84LonLat.Item2 < -90 || wgs84LonLat.Item2 > 90)
                throw new OWLSemanticsException("Cannot get features west of point because given \"wgs84LonLat\" parameter has not a valid latitude for WGS84");

            //Create WGS84 geometry from given point
            Geometry wgs84SearchPoint = new Point(wgs84LonLat.Item1, wgs84LonLat.Item2) { SRID = 4326 };

            //Execute SPARQL query to retrieve WKT/GML serialization of features having geometries
            List<(RDFResource,Geometry,Geometry)> featuresWithGeometry = Ontology.GetFeaturesWithGeometries();

            //Perform spatial analysis between collected geometries:
            //iterate geometries and collect those having longitudes lower than given point
            List<RDFResource> featuresWestOfPoint = new List<RDFResource>();
            featuresWithGeometry.ForEach(featureWithGeometry => {
                if (featureWithGeometry.Item2.Coordinates.Any(c => c.X < wgs84SearchPoint.Coordinate.X))
                    featuresWestOfPoint.Add(featureWithGeometry.Item1);
            });

            return RDFQueryUtilities.RemoveDuplicates(featuresWestOfPoint);
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

            //Execute SPARQL query to retrieve WKT/GML serialization of features having geometries
            List<(RDFResource,Geometry,Geometry)> featuresWithGeometry = Ontology.GetFeaturesWithGeometries();

            //Perform spatial analysis between collected geometries:
            //iterate geometries and collect those having latitudes lower than given point
            List<RDFResource> featuresSouthOfPoint = new List<RDFResource>();
            featuresWithGeometry.ForEach(featureWithGeometry => {
                if (featureWithGeometry.Item2.Coordinates.Any(c => c.Y < wgs84SearchPoint.Coordinate.Y))
                    featuresSouthOfPoint.Add(featureWithGeometry.Item1);
            });

            return RDFQueryUtilities.RemoveDuplicates(featuresSouthOfPoint);
        }

        /// <summary>
        /// Gets the features inside the given box represented by WGS84 Lon/Lat (lower-left, upper-right) corner points
        /// </summary>
        public List<RDFResource> GetFeaturesInsideBox((double,double) wgs84LonLat_LowerLeft, (double,double) wgs84LonLat_UpperRight)
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

            //Create Lambert Azimuthal geometry from given box corners
            Geometry lazSearchBox = GEOConverter.GetLambertAzimuthalGeometryFromWGS84(wgs84SearchBox);

            //Execute SPARQL query to retrieve WKT/GML serialization of features having geometries
            List<(RDFResource,Geometry,Geometry)> featuresWithGeometry = Ontology.GetFeaturesWithGeometries();

            //Perform spatial analysis between collected geometries:
            //iterate geometries and collect those inside given box
            List<RDFResource> featuresInsideBox = new List<RDFResource>();
            featuresWithGeometry.ForEach(featureWithGeometry => {
                if (lazSearchBox.Contains(featureWithGeometry.Item3))
                    featuresInsideBox.Add(featureWithGeometry.Item1);
            });

            return RDFQueryUtilities.RemoveDuplicates(featuresInsideBox);
        }

        /// <summary>
        /// Gets the features outside the given box represented by WGS84 Lon/Lat (lower-left, upper-right) corner points
        /// </summary>
        public List<RDFResource> GetFeaturesOutsideBox((double, double) wgs84LonLat_LowerLeft, (double, double) wgs84LonLat_UpperRight)
        {
            if (wgs84LonLat_LowerLeft.Item1 < -180 || wgs84LonLat_LowerLeft.Item1 > 180)
                throw new OWLSemanticsException("Cannot get features outside box because given \"wgs84LonMin\" parameter is not a valid longitude for WGS84");
            if (wgs84LonLat_LowerLeft.Item2 < -90 || wgs84LonLat_LowerLeft.Item2 > 90)
                throw new OWLSemanticsException("Cannot get features outside box because given \"wgs84LatMin\" parameter is not a valid latitude for WGS84");
            if (wgs84LonLat_UpperRight.Item1 < -180 || wgs84LonLat_UpperRight.Item1 > 180)
                throw new OWLSemanticsException("Cannot get features outside box because given \"wgs84LonMax\" parameter is not a valid longitude for WGS84");
            if (wgs84LonLat_UpperRight.Item2 < -90 || wgs84LonLat_UpperRight.Item2 > 90)
                throw new OWLSemanticsException("Cannot get features outside box because given \"wgs84LatMax\" parameter is not a valid latitude for WGS84");
            if (wgs84LonLat_LowerLeft.Item1 >= wgs84LonLat_UpperRight.Item1)
                throw new OWLSemanticsException("Cannot get features outside box because given \"wgs84LonMin\" parameter must be lower than given \"wgs84LonMax\" parameter");
            if (wgs84LonLat_LowerLeft.Item2 >= wgs84LonLat_UpperRight.Item2)
                throw new OWLSemanticsException("Cannot get features outside box because given \"wgs84LatMin\" parameter must be lower than given \"wgs84LatMax\" parameter");

            //Create WGS84 geometry from given box corners
            Geometry wgs84SearchBox = new Polygon(new LinearRing(new Coordinate[] {
                new Coordinate(wgs84LonLat_LowerLeft.Item1, wgs84LonLat_LowerLeft.Item2),
                new Coordinate(wgs84LonLat_UpperRight.Item1, wgs84LonLat_LowerLeft.Item2),
                new Coordinate(wgs84LonLat_UpperRight.Item1, wgs84LonLat_UpperRight.Item2),
                new Coordinate(wgs84LonLat_LowerLeft.Item1, wgs84LonLat_UpperRight.Item2),
                new Coordinate(wgs84LonLat_LowerLeft.Item1, wgs84LonLat_LowerLeft.Item2)
            })) { SRID = 4326 };

            //Create Lambert Azimuthal geometry from given box corners
            Geometry lazSearchBox = GEOConverter.GetLambertAzimuthalGeometryFromWGS84(wgs84SearchBox);

            //Execute SPARQL query to retrieve WKT/GML serialization of features having geometries
            List<(RDFResource,Geometry,Geometry)> featuresWithGeometry = Ontology.GetFeaturesWithGeometries();

            //Perform spatial analysis between collected geometries:
            //iterate geometries and collect those outside given box
            List<RDFResource> featuresOutsideBox = new List<RDFResource>();
            featuresWithGeometry.ForEach(featureWithGeometry => {
                if (!lazSearchBox.Contains(featureWithGeometry.Item3))
                    featuresOutsideBox.Add(featureWithGeometry.Item1);
            });

            return RDFQueryUtilities.RemoveDuplicates(featuresOutsideBox);
        }
        #endregion
    }
}