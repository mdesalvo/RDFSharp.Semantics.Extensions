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
using NetTopologySuite.IO.GML3;
using RDFSharp.Model;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace RDFSharp.Semantics.Extensions.GEO
{
    /// <summary>
    /// GEOOntologyHelper contains methods for declaring and analyzing relations describing GeoSPARQL geometries
    /// </summary>
    public static class GEOOntologyHelper
    {
        // WGS84 uses LON/LAT coordinates
        // LON => X (West/East, -180->180)
        // LAT => Y (North/South, -90->90)

        #region Declarer
        /// <summary>
        /// Declares the given sf:Point instance to the spatial ontology
        /// </summary>
        public static GEOOntology DeclarePoint(this GEOOntology geoOntology, RDFResource pointUri, double wgs84Lon, double wgs84Lat)
        {
            if (pointUri == null)
                throw new OWLSemanticsException("Cannot declare sf:Point instance to the spatial ontology because given \"pointUri\" parameter is null");
            if (wgs84Lon < -180 || wgs84Lon > 180)
                throw new OWLSemanticsException("Cannot declare sf:Point instance to the spatial ontology because given \"wgs84Lon\" parameter is not a valid longitude for WGS84");
            if (wgs84Lat < -90 || wgs84Lat > 90)
                throw new OWLSemanticsException("Cannot declare sf:Point instance to the spatial ontology because given \"wgs84Lat\" parameter is not a valid latitude for WGS84");
                        
            //Build sf:Point instance
            Point wgs84Point = new Point(wgs84Lon, wgs84Lat);
            
            //Build sf:Point serializations
            string wgs84PointWKT = new WKTWriter().Write(wgs84Point);
            string wgs84PointGML = null;
            using (XmlReader gmlReader = new GML3Writer().Write(wgs84Point))
            {
                XmlDocument gmlDocument = new XmlDocument();
                gmlDocument.Load(gmlReader);
                wgs84PointGML = gmlDocument.OuterXml;
            }

            //Add knowledge to the A-BOX
            geoOntology.Data.DeclareIndividual(pointUri);
            geoOntology.Data.DeclareIndividualType(pointUri, RDFVocabulary.GEOSPARQL.SF.POINT);
            geoOntology.Data.DeclareDatatypeAssertion(pointUri, RDFVocabulary.GEOSPARQL.AS_WKT, new RDFTypedLiteral(wgs84PointWKT, RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT));
            geoOntology.Data.DeclareDatatypeAssertion(pointUri, RDFVocabulary.GEOSPARQL.AS_GML, new RDFTypedLiteral(wgs84PointGML, RDFModelEnums.RDFDatatypes.GEOSPARQL_GML));

            //Add geometry to the spatial ontology (along with its UTM projection)
            (int,bool) utmFromWGS84 = GEOConverter.GetUTMZoneFromWGS84Coordinates(wgs84Lon, wgs84Lat);
            Point utmPoint = (Point)GEOConverter.GetUTMGeometryFromWGS84(wgs84Point, utmFromWGS84);
            if (!geoOntology.Geometries.ContainsKey(pointUri.ToString()))
                geoOntology.Geometries.Add(pointUri.ToString(), (wgs84Point, utmPoint));

            return geoOntology;
        }

        /// <summary>
        /// Declares the given sf:LineString instance to the spatial ontology (points must be WGS84 Lon/Lat)
        /// </summary>
        public static GEOOntology DeclareLineString(this GEOOntology geoOntology, RDFResource lineStringUri, List<(double,double)> wgs84LonLatPoints)
        {
            if (lineStringUri == null)
                throw new OWLSemanticsException("Cannot declare sf:LineString instance to the spatial ontology because given \"multiPointUri\" parameter is null");
            if (wgs84LonLatPoints == null)
                throw new OWLSemanticsException("Cannot declare sf:LineString instance to the spatial ontology because given \"wgs84LatLonPoints\" parameter is null");
            if (wgs84LonLatPoints.Count < 2)
                throw new OWLSemanticsException("Cannot declare sf:LineString instance to the spatial ontology because given \"wgs84LatLonPoints\" parameter contains less than 2 points");
            if (wgs84LonLatPoints.Any(pt => pt.Item1 < -180 || pt.Item1 > 180))
                throw new OWLSemanticsException("Cannot declare sf:LineString instance to the spatial ontology because given \"wgs84LatLonPoints\" parameter contains a point with invalid longitude for WGS84");
            if (wgs84LonLatPoints.Any(pt => pt.Item2 < -90 || pt.Item2 > 90))
                throw new OWLSemanticsException("Cannot declare sf:LineString instance to the spatial ontology because given \"wgs84LatLonPoints\" parameter contains a point with invalid latitude for WGS84");
            
            //Build sf:LineString instance
            LineString wgs84LineString = new LineString(wgs84LonLatPoints.Select(wgs84Point => new Coordinate(wgs84Point.Item1, wgs84Point.Item2)).ToArray());

            //Build sf:LineString serializations
            string wgs84LineStringWKT = new WKTWriter().Write(wgs84LineString);
            string wgs84LineStringGML = null;
            using (XmlReader gmlReader = new GML3Writer().Write(wgs84LineString))
            {
                XmlDocument gmlDocument = new XmlDocument();
                gmlDocument.Load(gmlReader);
                wgs84LineStringGML = gmlDocument.OuterXml;
            }

            //Add knowledge to the A-BOX
            geoOntology.Data.DeclareIndividual(lineStringUri);
            geoOntology.Data.DeclareIndividualType(lineStringUri, RDFVocabulary.GEOSPARQL.SF.LINESTRING);
            geoOntology.Data.DeclareDatatypeAssertion(lineStringUri, RDFVocabulary.GEOSPARQL.AS_WKT, new RDFTypedLiteral(wgs84LineStringWKT, RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT));
            geoOntology.Data.DeclareDatatypeAssertion(lineStringUri, RDFVocabulary.GEOSPARQL.AS_GML, new RDFTypedLiteral(wgs84LineStringGML, RDFModelEnums.RDFDatatypes.GEOSPARQL_GML));

            //Add geometry to the spatial ontology (along with its UTM projection)
            (int,bool) utmFromWGS84 = GEOConverter.GetUTMZoneFromWGS84Coordinates(wgs84LonLatPoints[0].Item1, wgs84LonLatPoints[0].Item2);
            LineString utmLineString = (LineString)GEOConverter.GetUTMGeometryFromWGS84(wgs84LineString, utmFromWGS84);
            if (!geoOntology.Geometries.ContainsKey(lineStringUri.ToString()))
                geoOntology.Geometries.Add(lineStringUri.ToString(), (wgs84LineString, utmLineString));

            return geoOntology;
        }

        /// <summary>
        /// Declares the given sf:Polygon instance to the spatial ontology (points must be WGS84 Lon/Lat)
        /// </summary>
        public static GEOOntology DeclarePolygon(this GEOOntology geoOntology, RDFResource polygonUri, List<(double,double)> wgs84LonLatPoints)
        {
            if (polygonUri == null)
                throw new OWLSemanticsException("Cannot declare sf:Polygon instance to the spatial ontology because given \"polygonUri\" parameter is null");
            if (wgs84LonLatPoints == null)
                throw new OWLSemanticsException("Cannot declare sf:Polygon instance to the spatial ontology because given \"wgs84LatLonPoints\" parameter is null");
            if (wgs84LonLatPoints.Count < 3)
                throw new OWLSemanticsException("Cannot declare sf:Polygon instance to the spatial ontology because given \"wgs84LatLonPoints\" parameter contains less than 3 points");
            if (wgs84LonLatPoints.Any(pt => pt.Item1 < -180 || pt.Item1 > 180))
                throw new OWLSemanticsException("Cannot declare sf:Polygon instance to the spatial ontology because given \"wgs84LatLonPoints\" parameter contains a point with invalid longitude for WGS84");
            if (wgs84LonLatPoints.Any(pt => pt.Item2 < -90 || pt.Item2 > 90))
                throw new OWLSemanticsException("Cannot declare sf:Polygon instance to the spatial ontology because given \"wgs84LatLonPoints\" parameter contains a point with invalid latitude for WGS84");

            //Automatically close polygon (if needed)
            if (wgs84LonLatPoints[0].Item1 != wgs84LonLatPoints[wgs84LonLatPoints.Count-1].Item1
                 && wgs84LonLatPoints[0].Item2 != wgs84LonLatPoints[wgs84LonLatPoints.Count-1].Item2)
                wgs84LonLatPoints.Add(wgs84LonLatPoints[0]);

            //Build sf:Polygon instance
            Polygon wgs84Polygon = new Polygon(new LinearRing(wgs84LonLatPoints.Select(wgs84Point => new Coordinate(wgs84Point.Item1, wgs84Point.Item2)).ToArray()));

            //Build sf:Polygon serializations
            string wgs84PolygonWKT = new WKTWriter().Write(wgs84Polygon);
            string wgs84PolygonGML = null;
            using (XmlReader gmlReader = new GML3Writer().Write(wgs84Polygon))
            {
                XmlDocument gmlDocument = new XmlDocument();
                gmlDocument.Load(gmlReader);
                wgs84PolygonGML = gmlDocument.OuterXml;
            }

            //Add knowledge to the A-BOX
            geoOntology.Data.DeclareIndividual(polygonUri);
            geoOntology.Data.DeclareIndividualType(polygonUri, RDFVocabulary.GEOSPARQL.SF.POLYGON);
            geoOntology.Data.DeclareDatatypeAssertion(polygonUri, RDFVocabulary.GEOSPARQL.AS_WKT, new RDFTypedLiteral(wgs84PolygonWKT, RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT));
            geoOntology.Data.DeclareDatatypeAssertion(polygonUri, RDFVocabulary.GEOSPARQL.AS_GML, new RDFTypedLiteral(wgs84PolygonGML, RDFModelEnums.RDFDatatypes.GEOSPARQL_GML));

            //Add geometry to the spatial ontology (along with its UTM projection)
            (int, bool) utmFromWGS84 = GEOConverter.GetUTMZoneFromWGS84Coordinates(wgs84LonLatPoints[0].Item1, wgs84LonLatPoints[0].Item2);
            Polygon utmPolygon = (Polygon)GEOConverter.GetUTMGeometryFromWGS84(wgs84Polygon, utmFromWGS84);
            if (!geoOntology.Geometries.ContainsKey(polygonUri.ToString()))
                geoOntology.Geometries.Add(polygonUri.ToString(), (wgs84Polygon, utmPolygon));

            return geoOntology;
        }

        /// <summary>
        /// Declares the given sf:MultiPoint instance to the spatial ontology (points must be WGS84 Lon/Lat)
        /// </summary>
        public static GEOOntology DeclareMultiPoint(this GEOOntology geoOntology, RDFResource multiPointUri, List<(double,double)> wgs84LonLatPoints)
        {
            if (multiPointUri == null)
                throw new OWLSemanticsException("Cannot declare sf:MultiPoint instance to the spatial ontology because given \"multiPointUri\" parameter is null");
            if (wgs84LonLatPoints == null)
                throw new OWLSemanticsException("Cannot declare sf:MultiPoint instance to the spatial ontology because given \"wgs84LatLonPoints\" parameter is null");
            if (wgs84LonLatPoints.Count < 2)
                throw new OWLSemanticsException("Cannot declare sf:MultiPoint instance to the spatial ontology because given \"wgs84LatLonPoints\" parameter contains less than 2 points");
            if (wgs84LonLatPoints.Any(pt => pt.Item1 < -180 || pt.Item1 > 180))
                throw new OWLSemanticsException("Cannot declare sf:MultiPoint instance to the spatial ontology because given \"wgs84LatLonPoints\" parameter contains a point with invalid longitude for WGS84");
            if (wgs84LonLatPoints.Any(pt => pt.Item2 < -90 || pt.Item2 > 90))
                throw new OWLSemanticsException("Cannot declare sf:MultiPoint instance to the spatial ontology because given \"wgs84LatLonPoints\" parameter contains a point with invalid latitude for WGS84");

            //Build sf:MultiPoint instance
            MultiPoint wgs84MultiPoint = new MultiPoint(wgs84LonLatPoints.Select(wgs84Point => new Point(wgs84Point.Item1, wgs84Point.Item2)).ToArray());

            //Build sf:MultiPoint serializations
            string wgs84MultiPointWKT = new WKTWriter().Write(wgs84MultiPoint);
            string wgs84MultiPointGML = null;
            using (XmlReader gmlReader = new GML3Writer().Write(wgs84MultiPoint))
            {
                XmlDocument gmlDocument = new XmlDocument();
                gmlDocument.Load(gmlReader);
                wgs84MultiPointGML = gmlDocument.OuterXml;
            }

            //Add knowledge to the A-BOX
            geoOntology.Data.DeclareIndividual(multiPointUri);
            geoOntology.Data.DeclareIndividualType(multiPointUri, RDFVocabulary.GEOSPARQL.SF.MULTI_POINT);
            geoOntology.Data.DeclareDatatypeAssertion(multiPointUri, RDFVocabulary.GEOSPARQL.AS_WKT, new RDFTypedLiteral(wgs84MultiPointWKT, RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT));
            geoOntology.Data.DeclareDatatypeAssertion(multiPointUri, RDFVocabulary.GEOSPARQL.AS_GML, new RDFTypedLiteral(wgs84MultiPointGML, RDFModelEnums.RDFDatatypes.GEOSPARQL_GML));

            //Add geometry to the spatial ontology (along with its UTM projection)
            (int, bool) utmFromWGS84 = GEOConverter.GetUTMZoneFromWGS84Coordinates(wgs84LonLatPoints[0].Item1, wgs84LonLatPoints[0].Item2);
            MultiPoint utmMultiPoint = (MultiPoint)GEOConverter.GetUTMGeometryFromWGS84(wgs84MultiPoint, utmFromWGS84);
            if (!geoOntology.Geometries.ContainsKey(multiPointUri.ToString()))
                geoOntology.Geometries.Add(multiPointUri.ToString(), (wgs84MultiPoint, utmMultiPoint));

            return geoOntology;
        }
        #endregion
    }
}