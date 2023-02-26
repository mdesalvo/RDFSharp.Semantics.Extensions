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
using NetTopologySuite.IO.GML3;
using RDFSharp.Model;
using RDFSharp.Query;
using System.Collections.Generic;
using System.Data;
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
        /// Declares the given geosparql:Feature instance to the spatial ontology
        /// </summary>
        public static GEOOntology DeclareFeature(this GEOOntology geoOntology, RDFResource featureUri)
        {
            if (featureUri == null)
                throw new OWLSemanticsException("Cannot declare geosparql:Feature instance to the spatial ontology because given \"featureUri\" parameter is null");

            //Add knowledge to the A-BOX
            geoOntology.Data.DeclareIndividual(featureUri);
            geoOntology.Data.DeclareIndividualType(featureUri, RDFVocabulary.GEOSPARQL.FEATURE);

            return geoOntology;
        }

        /// <summary>
        /// Declares the existence of the given "DefaultGeometry(featureUri, geometryUri)" relation to the spatial ontology
        /// </summary>
        public static GEOOntology DeclareDefaultGeometry(this GEOOntology geoOntology, RDFResource featureUri, RDFResource geometryUri)
        {
            if (geometryUri == null)
                throw new OWLSemanticsException("Cannot declare geosparql:Feature instance to the spatial ontology because given \"geometryUri\" parameter is null");

            //Add knowledge to the A-BOX
            DeclareFeature(geoOntology, featureUri);
            geoOntology.Data.DeclareIndividual(geometryUri);
            geoOntology.Data.DeclareIndividualType(geometryUri, RDFVocabulary.GEOSPARQL.GEOMETRY);
            geoOntology.Data.DeclareObjectAssertion(featureUri, RDFVocabulary.GEOSPARQL.DEFAULT_GEOMETRY, geometryUri);

            return geoOntology;
        }

        /// <summary>
        /// Declares the existence of the given "HasGeometry(featureUri, geometryUri)" relation to the spatial ontology
        /// </summary>
        public static GEOOntology DeclareSecondaryGeometry(this GEOOntology geoOntology, RDFResource featureUri, RDFResource geometryUri)
        {
            if (geometryUri == null)
                throw new OWLSemanticsException("Cannot declare geosparql:Feature instance to the spatial ontology because given \"geometryUri\" parameter is null");

            //Add knowledge to the A-BOX
            DeclareFeature(geoOntology, featureUri);
            geoOntology.Data.DeclareIndividual(geometryUri);
            geoOntology.Data.DeclareIndividualType(geometryUri, RDFVocabulary.GEOSPARQL.GEOMETRY);
            geoOntology.Data.DeclareObjectAssertion(featureUri, RDFVocabulary.GEOSPARQL.HAS_GEOMETRY, geometryUri);

            return geoOntology;
        }

        /// <summary>
        /// Declares the given sf:Point instance to the spatial ontology (coordinates must be WGS84 Lon/Lat)
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
            return DeclarePointInternal(geoOntology, pointUri, new Point(wgs84Lon, wgs84Lat) { SRID = 4326 });
        }
        internal static GEOOntology DeclarePointInternal(this GEOOntology geoOntology, RDFResource pointUri, Point wgs84Point)
        {
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

            return geoOntology;
        }

        /// <summary>
        /// Declares the given sf:LineString instance to the spatial ontology (points must be WGS84 Lon/Lat)
        /// </summary>
        public static GEOOntology DeclareLineString(this GEOOntology geoOntology, RDFResource lineStringUri, List<(double,double)> wgs84LineString)
        {
            if (lineStringUri == null)
                throw new OWLSemanticsException("Cannot declare sf:LineString instance to the spatial ontology because given \"geometryCollectionUri\" parameter is null");
            if (wgs84LineString == null)
                throw new OWLSemanticsException("Cannot declare sf:LineString instance to the spatial ontology because given \"wgs84Polygon\" parameter is null");
            if (wgs84LineString.Count < 2)
                throw new OWLSemanticsException("Cannot declare sf:LineString instance to the spatial ontology because given \"wgs84Polygon\" parameter contains less than 2 points");
            if (wgs84LineString.Any(pt => pt.Item1 < -180 || pt.Item1 > 180))
                throw new OWLSemanticsException("Cannot declare sf:LineString instance to the spatial ontology because given \"wgs84Polygon\" parameter contains a point with invalid longitude for WGS84");
            if (wgs84LineString.Any(pt => pt.Item2 < -90 || pt.Item2 > 90))
                throw new OWLSemanticsException("Cannot declare sf:LineString instance to the spatial ontology because given \"wgs84Polygon\" parameter contains a point with invalid latitude for WGS84");

            //Build sf:LineString instance
            return DeclareLineStringInternal(geoOntology, lineStringUri, new LineString(wgs84LineString.Select(wgs84Point => new Coordinate(wgs84Point.Item1, wgs84Point.Item2)).ToArray()) { SRID = 4326 });
        }
        internal static GEOOntology DeclareLineStringInternal(this GEOOntology geoOntology, RDFResource lineStringUri, LineString wgs84LineString)
        {
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

            return geoOntology;
        }

        /// <summary>
        /// Declares the given sf:Polygon instance to the spatial ontology (points must be WGS84 Lon/Lat)
        /// </summary>
        public static GEOOntology DeclarePolygon(this GEOOntology geoOntology, RDFResource polygonUri, List<(double,double)> wgs84Polygon)
        {
            if (polygonUri == null)
                throw new OWLSemanticsException("Cannot declare sf:Polygon instance to the spatial ontology because given \"polygonUri\" parameter is null");
            if (wgs84Polygon == null)
                throw new OWLSemanticsException("Cannot declare sf:Polygon instance to the spatial ontology because given \"wgs84Polygon\" parameter is null");
            if (wgs84Polygon.Count < 3)
                throw new OWLSemanticsException("Cannot declare sf:Polygon instance to the spatial ontology because given \"wgs84Polygon\" parameter contains less than 3 points");
            if (wgs84Polygon.Any(pt => pt.Item1 < -180 || pt.Item1 > 180))
                throw new OWLSemanticsException("Cannot declare sf:Polygon instance to the spatial ontology because given \"wgs84Polygon\" parameter contains a point with invalid longitude for WGS84");
            if (wgs84Polygon.Any(pt => pt.Item2 < -90 || pt.Item2 > 90))
                throw new OWLSemanticsException("Cannot declare sf:Polygon instance to the spatial ontology because given \"wgs84Polygon\" parameter contains a point with invalid latitude for WGS84");

            //Automatically close polygon (if needed)
            if (wgs84Polygon[0].Item1 != wgs84Polygon[wgs84Polygon.Count-1].Item1
                 && wgs84Polygon[0].Item2 != wgs84Polygon[wgs84Polygon.Count-1].Item2)
                wgs84Polygon.Add(wgs84Polygon[0]);

            //Build sf:Polygon instance
            return DeclarePolygonInternal(geoOntology, polygonUri, new Polygon(new LinearRing(wgs84Polygon.Select(wgs84Point => new Coordinate(wgs84Point.Item1, wgs84Point.Item2)).ToArray())) { SRID = 4326 });
        }
        internal static GEOOntology DeclarePolygonInternal(this GEOOntology geoOntology, RDFResource polygonUri, Polygon wgs84Polygon)
        {
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

            return geoOntology;
        }

        /// <summary>
        /// Declares the given sf:MultiPoint instance to the spatial ontology (points must be WGS84 Lon/Lat)
        /// </summary>
        public static GEOOntology DeclareMultiPoint(this GEOOntology geoOntology, RDFResource multiPointUri, List<(double,double)> wgs84MultiPoint)
        {
            if (multiPointUri == null)
                throw new OWLSemanticsException("Cannot declare sf:MultiPoint instance to the spatial ontology because given \"geometryCollectionUri\" parameter is null");
            if (wgs84MultiPoint == null)
                throw new OWLSemanticsException("Cannot declare sf:MultiPoint instance to the spatial ontology because given \"wgs84MultiPoint\" parameter is null");
            if (wgs84MultiPoint.Count < 2)
                throw new OWLSemanticsException("Cannot declare sf:MultiPoint instance to the spatial ontology because given \"wgs84MultiPoint\" parameter contains less than 2 points");
            if (wgs84MultiPoint.Any(pt => pt.Item1 < -180 || pt.Item1 > 180))
                throw new OWLSemanticsException("Cannot declare sf:MultiPoint instance to the spatial ontology because given \"wgs84MultiPoint\" parameter contains a point with invalid longitude for WGS84");
            if (wgs84MultiPoint.Any(pt => pt.Item2 < -90 || pt.Item2 > 90))
                throw new OWLSemanticsException("Cannot declare sf:MultiPoint instance to the spatial ontology because given \"wgs84MultiPoint\" parameter contains a point with invalid latitude for WGS84");

            //Build sf:MultiPoint instance
            return DeclareMultiPointInternal(geoOntology, multiPointUri, new MultiPoint(wgs84MultiPoint.Select(wgs84Point => new Point(wgs84Point.Item1, wgs84Point.Item2) { SRID = 4326 }).ToArray()) { SRID = 4326 });
        }
        internal static GEOOntology DeclareMultiPointInternal(this GEOOntology geoOntology, RDFResource multiPointUri, MultiPoint wgs84MultiPoint)
        {
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

            return geoOntology;
        }

        /// <summary>
        /// Declares the given sf:MultiLineString instance to the spatial ontology (linestrings must be WGS84 Lon/Lat)
        /// </summary>
        public static GEOOntology DeclareMultiLineString(this GEOOntology geoOntology, RDFResource multiLineStringUri, List<List<(double,double)>> wgs84MultiLineString)
        {
            if (multiLineStringUri == null)
                throw new OWLSemanticsException("Cannot declare sf:MultiLineString instance to the spatial ontology because given \"geometryCollectionUri\" parameter is null");
            if (wgs84MultiLineString == null)
                throw new OWLSemanticsException("Cannot declare sf:MultiLineString instance to the spatial ontology because given \"wgs84MultiPolygon\" parameter is null");
            if (wgs84MultiLineString.Any(ls => ls == null))
                throw new OWLSemanticsException("Cannot declare sf:MultiLineString instance to the spatial ontology because given \"wgs84MultiPolygon\" parameter contains a null linestring");
            if (wgs84MultiLineString.Count < 2)
                throw new OWLSemanticsException("Cannot declare sf:MultiLineString instance to the spatial ontology because given \"wgs84MultiPolygon\" parameter contains less than 2 linestrings");
            if (wgs84MultiLineString.Any(ls => ls.Count < 2))
                throw new OWLSemanticsException("Cannot declare sf:MultiLineString instance to the spatial ontology because given \"wgs84MultiPolygon\" parameter contains a linestring with less than 2 points");
            if (wgs84MultiLineString.Any(ls => ls.Any(pt => pt.Item1 < -180 || pt.Item1 > 180)))
                throw new OWLSemanticsException("Cannot declare sf:MultiLineString instance to the spatial ontology because given \"wgs84MultiPolygon\" parameter contains a linestring having point with invalid longitude for WGS84");
            if (wgs84MultiLineString.Any(ls => ls.Any(pt => pt.Item2 < -90 || pt.Item2 > 90)))
                throw new OWLSemanticsException("Cannot declare sf:MultiLineString instance to the spatial ontology because given \"wgs84MultiPolygon\" parameter contains a linestring having point with invalid latitude for WGS84");

            //Reconstruct sf:MultiLineString
            List<LineString> wgs84LineStrings = new List<LineString>();
            foreach (List<(double, double)> wgs84LineString in wgs84MultiLineString)
                wgs84LineStrings.Add(new LineString(wgs84LineString.Select(wgs84Point => new Coordinate(wgs84Point.Item1, wgs84Point.Item2)).ToArray()) { SRID = 4326 });

            //Build sf:MultiLineString instance
            return DeclareMultiLineStringInternal(geoOntology, multiLineStringUri, new MultiLineString(wgs84LineStrings.ToArray()));
        }
        internal static GEOOntology DeclareMultiLineStringInternal(this GEOOntology geoOntology, RDFResource multiLineStringUri, MultiLineString wgs84MultiLineString)
        {
            //Build sf:MultiLineString serializations
            string wgs84MultiLineStringWKT = new WKTWriter().Write(wgs84MultiLineString);
            string wgs84MultiLineStringGML = null;
            using (XmlReader gmlReader = new GML3Writer().Write(wgs84MultiLineString))
            {
                XmlDocument gmlDocument = new XmlDocument();
                gmlDocument.Load(gmlReader);
                wgs84MultiLineStringGML = gmlDocument.OuterXml;
            }

            //Add knowledge to the A-BOX
            geoOntology.Data.DeclareIndividual(multiLineStringUri);
            geoOntology.Data.DeclareIndividualType(multiLineStringUri, RDFVocabulary.GEOSPARQL.SF.MULTI_LINESTRING);
            geoOntology.Data.DeclareDatatypeAssertion(multiLineStringUri, RDFVocabulary.GEOSPARQL.AS_WKT, new RDFTypedLiteral(wgs84MultiLineStringWKT, RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT));
            geoOntology.Data.DeclareDatatypeAssertion(multiLineStringUri, RDFVocabulary.GEOSPARQL.AS_GML, new RDFTypedLiteral(wgs84MultiLineStringGML, RDFModelEnums.RDFDatatypes.GEOSPARQL_GML));

            return geoOntology;
        }

        /// <summary>
        /// Declares the given sf:MultiPolygon instance to the spatial ontology (polygons must be WGS84 Lon/Lat)
        /// </summary>
        public static GEOOntology DeclareMultiPolygon(this GEOOntology geoOntology, RDFResource multiPolygonUri, List<List<(double,double)>> wgs84MultiPolygon)
        {
            if (multiPolygonUri == null)
                throw new OWLSemanticsException("Cannot declare sf:MultiPolygon instance to the spatial ontology because given \"geometryCollectionUri\" parameter is null");
            if (wgs84MultiPolygon == null)
                throw new OWLSemanticsException("Cannot declare sf:MultiPolygon instance to the spatial ontology because given \"wgs84MultiPolygon\" parameter is null");
            if (wgs84MultiPolygon.Any(pl => pl == null))
                throw new OWLSemanticsException("Cannot declare sf:MultiPolygon instance to the spatial ontology because given \"wgs84MultiPolygon\" parameter contains a null polygon");
            if (wgs84MultiPolygon.Count < 2)
                throw new OWLSemanticsException("Cannot declare sf:MultiPolygon instance to the spatial ontology because given \"wgs84MultiPolygon\" parameter contains less than 2 polygons");
            if (wgs84MultiPolygon.Any(pl => pl.Count < 3))
                throw new OWLSemanticsException("Cannot declare sf:MultiPolygon instance to the spatial ontology because given \"wgs84MultiPolygon\" parameter contains a polygon with less than 3 points");
            if (wgs84MultiPolygon.Any(pl => pl.Any(pt => pt.Item1 < -180 || pt.Item1 > 180)))
                throw new OWLSemanticsException("Cannot declare sf:MultiPolygon instance to the spatial ontology because given \"wgs84MultiPolygon\" parameter contains a polygon having point with invalid longitude for WGS84");
            if (wgs84MultiPolygon.Any(pl => pl.Any(pt => pt.Item2 < -90 || pt.Item2 > 90)))
                throw new OWLSemanticsException("Cannot declare sf:MultiPolygon instance to the spatial ontology because given \"wgs84MultiPolygon\" parameter contains a polygon having point with invalid latitude for WGS84");

            //Reconstruct sf:MultiPolygon
            List<Polygon> wgs84Polygons = new List<Polygon>();
            foreach (List<(double, double)> wgs84Polygon in wgs84MultiPolygon)
            {
                //Automatically close polygon (if needed)
                if (wgs84Polygon[0].Item1 != wgs84Polygon[wgs84Polygon.Count - 1].Item1
                     && wgs84Polygon[0].Item2 != wgs84Polygon[wgs84Polygon.Count - 1].Item2)
                    wgs84Polygon.Add(wgs84Polygon[0]);
                wgs84Polygons.Add(new Polygon(new LinearRing(wgs84Polygon.Select(wgs84Point => new Coordinate(wgs84Point.Item1, wgs84Point.Item2)).ToArray())) { SRID = 4326 });
            }

            //Build sf:MultiPolygon instance
            return DeclareMultiPolygonInternal(geoOntology, multiPolygonUri, new MultiPolygon(wgs84Polygons.ToArray()));
        }
        internal static GEOOntology DeclareMultiPolygonInternal(this GEOOntology geoOntology, RDFResource multiPolygonUri, MultiPolygon wgs84MultiPolygon)
        {
            //Build sf:MultiPolygon serializations
            string wgs84MultiPolygonWKT = new WKTWriter().Write(wgs84MultiPolygon);
            string wgs84MultiPolygonGML = null;
            using (XmlReader gmlReader = new GML3Writer().Write(wgs84MultiPolygon))
            {
                XmlDocument gmlDocument = new XmlDocument();
                gmlDocument.Load(gmlReader);
                wgs84MultiPolygonGML = gmlDocument.OuterXml;
            }

            //Add knowledge to the A-BOX
            geoOntology.Data.DeclareIndividual(multiPolygonUri);
            geoOntology.Data.DeclareIndividualType(multiPolygonUri, RDFVocabulary.GEOSPARQL.SF.MULTI_POLYGON);
            geoOntology.Data.DeclareDatatypeAssertion(multiPolygonUri, RDFVocabulary.GEOSPARQL.AS_WKT, new RDFTypedLiteral(wgs84MultiPolygonWKT, RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT));
            geoOntology.Data.DeclareDatatypeAssertion(multiPolygonUri, RDFVocabulary.GEOSPARQL.AS_GML, new RDFTypedLiteral(wgs84MultiPolygonGML, RDFModelEnums.RDFDatatypes.GEOSPARQL_GML));

            return geoOntology;
        }

        /// <summary>
        /// Declares the given sf:GeometryCollection instance to the spatial ontology (points, linestrings and polygons must be WGS84 Lon/Lat)
        /// </summary>
        public static GEOOntology DeclareGeometryCollection(this GEOOntology geoOntology, RDFResource geometryCollectionUri, List<(double,double)> wgs84Points, 
            List<List<(double,double)>> wgs84LineStrings, List<List<(double,double)>> wgs84Polygons)
        {
            if (geometryCollectionUri == null)
                throw new OWLSemanticsException("Cannot declare sf:GeometryCollection instance to the spatial ontology because given \"geometryCollectionUri\" parameter is null");
            if (wgs84Points?.Any(pt => pt.Item1 < -180 || pt.Item1 > 180) ?? false)
                throw new OWLSemanticsException("Cannot declare sf:GeometryCollection instance to the spatial ontology because given \"wgs84Points\" parameter contains a point with invalid longitude for WGS84");
            if (wgs84Points?.Any(pt => pt.Item2 < -90 || pt.Item2 > 90) ?? false)
                throw new OWLSemanticsException("Cannot declare sf:GeometryCollection instance to the spatial ontology because given \"wgs84Points\" parameter contains a point with invalid latitude for WGS84");
            if (wgs84LineStrings?.Any(ls => ls == null) ?? false)
                throw new OWLSemanticsException("Cannot declare sf:GeometryCollection instance to the spatial ontology because given \"wgs84LineStrings\" parameter contains a null linestring");
            if (wgs84LineStrings.Any(ls => ls.Count < 2))
                throw new OWLSemanticsException("Cannot declare sf:GeometryCollection instance to the spatial ontology because given \"wgs84LineStrings\" parameter contains a linestring with less than 2 points");
            if (wgs84LineStrings?.Any(ls => ls.Any(pt => pt.Item1 < -180 || pt.Item1 > 180)) ?? false)
                throw new OWLSemanticsException("Cannot declare sf:GeometryCollection instance to the spatial ontology because given \"wgs84LineStrings\" parameter contains a linestring having a point with invalid longitude for WGS84");
            if (wgs84LineStrings?.Any(ls => ls.Any(pt => pt.Item2 < -90 || pt.Item2 > 90)) ?? false)
                throw new OWLSemanticsException("Cannot declare sf:GeometryCollection instance to the spatial ontology because given \"wgs84LineStrings\" parameter contains a linestring having a point with invalid latitude for WGS84");
            if (wgs84Polygons?.Any(pl => pl == null) ?? false)
                throw new OWLSemanticsException("Cannot declare sf:GeometryCollection instance to the spatial ontology because given \"wgs84Polygons\" parameter contains a null polygon");
            if (wgs84Polygons.Any(pl => pl.Count < 3))
                throw new OWLSemanticsException("Cannot declare sf:GeometryCollection instance to the spatial ontology because given \"wgs84Polygons\" parameter contains a polygon with less than 3 points");
            if (wgs84Polygons?.Any(pl => pl.Any(pt => pt.Item1 < -180 || pt.Item1 > 180)) ?? false)
                throw new OWLSemanticsException("Cannot declare sf:GeometryCollection instance to the spatial ontology because given \"wgs84Polygons\" parameter contains a polygon having a point with invalid longitude for WGS84");
            if (wgs84Polygons?.Any(pl => pl.Any(pt => pt.Item2 < -90 || pt.Item2 > 90)) ?? false)
                throw new OWLSemanticsException("Cannot declare sf:GeometryCollection instance to the spatial ontology because given \"wgs84Polygons\" parameter contains a polygon having a point with invalid latitude for WGS84");

            //Reconstruct sf:Point(s)
            List<Point> wgs84CollectionPoints = new List<Point>();
            foreach ((double,double) wgs84Point in wgs84Points)
                wgs84CollectionPoints.Add(new Point(wgs84Point.Item1, wgs84Point.Item2) { SRID = 4326 });

            //Reconstruct sf:LineString(s)
            List<LineString> wgs84CollectionLineStrings = new List<LineString>();
            foreach (List<(double,double)> wgs84LineString in wgs84LineStrings)
                wgs84CollectionLineStrings.Add(new LineString(wgs84LineString.Select(wgs84Point => new Coordinate(wgs84Point.Item1, wgs84Point.Item2)).ToArray()) { SRID = 4326 });

            //Reconstruct sf:Polygon(s)
            List<Polygon> wgs84CollectionPolygons = new List<Polygon>();
            foreach (List<(double,double)> wgs84Polygon in wgs84Polygons)
            {
                //Automatically close polygon (if needed)
                if (wgs84Polygon[0].Item1 != wgs84Polygon[wgs84Polygon.Count - 1].Item1
                     && wgs84Polygon[0].Item2 != wgs84Polygon[wgs84Polygon.Count - 1].Item2)
                    wgs84Polygon.Add(wgs84Polygon[0]);
                wgs84CollectionPolygons.Add(new Polygon(new LinearRing(wgs84Polygon.Select(wgs84Point => new Coordinate(wgs84Point.Item1, wgs84Point.Item2)).ToArray())) { SRID = 4326 });
            }

            //Build sf:GeometryCollection instance
            GeometryCollection wgs84GeometryCollection = 
                new GeometryCollection(wgs84CollectionPoints.OfType<Geometry>()
                                        .Union(wgs84CollectionLineStrings.OfType<Geometry>())
                                         .Union(wgs84CollectionPolygons.OfType<Geometry>())
                                          .ToArray()) { SRID = 4326 };
            return DeclareGeometryCollectionInternal(geoOntology, geometryCollectionUri, wgs84GeometryCollection);
        }
        internal static GEOOntology DeclareGeometryCollectionInternal(this GEOOntology geoOntology, RDFResource geometryCollectionUri, GeometryCollection wgs84GeometryCollection)
        {
            //Build sf:GeometryCollection serializations
            string wgs84GeometryCollectionWKT = new WKTWriter().Write(wgs84GeometryCollection);
            string wgs84GeometryCollectionGML = null;
            using (XmlReader gmlReader = new GML3Writer().Write(wgs84GeometryCollection))
            {
                XmlDocument gmlDocument = new XmlDocument();
                gmlDocument.Load(gmlReader);
                wgs84GeometryCollectionGML = gmlDocument.OuterXml;
            }

            //Add knowledge to the A-BOX
            geoOntology.Data.DeclareIndividual(geometryCollectionUri);
            geoOntology.Data.DeclareIndividualType(geometryCollectionUri, RDFVocabulary.GEOSPARQL.SF.GEOMETRY_COLLECTION);
            geoOntology.Data.DeclareDatatypeAssertion(geometryCollectionUri, RDFVocabulary.GEOSPARQL.AS_WKT, new RDFTypedLiteral(wgs84GeometryCollectionWKT, RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT));
            geoOntology.Data.DeclareDatatypeAssertion(geometryCollectionUri, RDFVocabulary.GEOSPARQL.AS_GML, new RDFTypedLiteral(wgs84GeometryCollectionGML, RDFModelEnums.RDFDatatypes.GEOSPARQL_GML));

            return geoOntology;
        }
        #endregion

        #region Analyzer
        /// <summary>
        /// Gets the default geometry (WGS84,UTM) assigned to the given geosparql:Feature instance
        /// </summary>
        internal static (Geometry,Geometry) GetDefaultGeometryOfFeature(this GEOOntology geoOntology, RDFResource featureUri)
        {
            //Execute SPARQL query to retrieve WKT/GML serialization of the given feature's default geometry
            RDFSelectQuery selectQuery = new RDFSelectQuery()
                .AddPatternGroup(new RDFPatternGroup()
                    .AddPattern(new RDFPattern(featureUri, RDFVocabulary.GEOSPARQL.DEFAULT_GEOMETRY, new RDFVariable("?GEOM")))
                    .AddPattern(new RDFPattern(new RDFVariable("?GEOM"), RDFVocabulary.GEOSPARQL.AS_WKT, new RDFVariable("?GEOMWKT")).Optional())
                    .AddPattern(new RDFPattern(new RDFVariable("?GEOM"), RDFVocabulary.GEOSPARQL.AS_GML, new RDFVariable("?GEOMGML")).Optional())
                    .AddFilter(new RDFBooleanOrFilter(
                        new RDFDatatypeFilter(new RDFVariable("?GEOMWKT"), RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT),
                        new RDFDatatypeFilter(new RDFVariable("?GEOMGML"), RDFModelEnums.RDFDatatypes.GEOSPARQL_GML))))
                .AddProjectionVariable(new RDFVariable("?GEOMWKT"))
                .AddProjectionVariable(new RDFVariable("?GEOMGML"))
                .AddModifier(new RDFLimitModifier(1));
            RDFSelectQueryResult selectQueryResult = selectQuery.ApplyToGraph(geoOntology.Data.ABoxGraph);

            //Parse retrieved WKT/GML serialization into (WGS84,UTM) result geometry
            if (selectQueryResult.SelectResultsCount > 0)
            {
                //WKT
                if (!selectQueryResult.SelectResults.Rows[0].IsNull("?GEOMWKT"))
                {
                    try
                    {
                        //Parse default geometry from WKT
                        RDFTypedLiteral wktGeometryLiteral = (RDFTypedLiteral)RDFQueryUtilities.ParseRDFPatternMember(selectQueryResult.SelectResults.Rows[0]["?GEOMWKT"].ToString());
                        Geometry wgs84Geometry = new WKTReader().Read(wktGeometryLiteral.Value);
                        wgs84Geometry.SRID = 4326;

                        //Project default geometry from WGS84 to UTM
                        (int, bool) utmZone = GEOConverter.GetUTMZoneFromWGS84Coordinates(wgs84Geometry.Coordinates[0].X, wgs84Geometry.Coordinates[0].Y);
                        Geometry utmGeometry = GEOConverter.GetUTMGeometryFromWGS84(wgs84Geometry, utmZone);

                        return (wgs84Geometry, utmGeometry);
                    }
                    catch { /* Just a no-op, since type errors are normal when trying to face variable's bindings */ }
                }

                //GML
                if (!selectQueryResult.SelectResults.Rows[0].IsNull("?GEOMGML"))
                {
                    try
                    {
                        //Parse default geometry from GML
                        RDFTypedLiteral gmlGeometryLiteral = (RDFTypedLiteral)RDFQueryUtilities.ParseRDFPatternMember(selectQueryResult.SelectResults.Rows[0]["?GEOMGML"].ToString());
                        Geometry wgs84Geometry = new GMLReader().Read(gmlGeometryLiteral.Value);
                        wgs84Geometry.SRID = 4326;

                        //Project default geometry from WGS84 to UTM
                        (int, bool) utmZone = GEOConverter.GetUTMZoneFromWGS84Coordinates(wgs84Geometry.Coordinates[0].X, wgs84Geometry.Coordinates[0].Y);
                        Geometry utmGeometry = GEOConverter.GetUTMGeometryFromWGS84(wgs84Geometry, utmZone);

                        return (wgs84Geometry, utmGeometry);
                    }
                    catch { /* Just a no-op, since type errors are normal when trying to face variable's bindings */ }
                }
            }

            return (null,null);
        }

        /// <summary>
        /// Gets the list of secondary geometries (WGS84,UTM) assigned to the given geosparql:Feature instance
        /// </summary>
        internal static List<(Geometry,Geometry)> GetSecondaryGeometriesOfFeature(this GEOOntology geoOntology, RDFResource featureUri)
        {
            List<(Geometry,Geometry)> secondaryGeometries = new List<(Geometry,Geometry)>();

            //Execute SPARQL query to retrieve WKT/GML serialization of the given feature's not default geometries
            RDFSelectQuery selectQuery = new RDFSelectQuery()
                .AddPatternGroup(new RDFPatternGroup()
                    .AddPattern(new RDFPattern(featureUri, RDFVocabulary.GEOSPARQL.HAS_GEOMETRY, new RDFVariable("?GEOM")))
                    .AddPattern(new RDFPattern(new RDFVariable("?GEOM"), RDFVocabulary.GEOSPARQL.AS_WKT, new RDFVariable("?GEOMWKT")).Optional())
                    .AddPattern(new RDFPattern(new RDFVariable("?GEOM"), RDFVocabulary.GEOSPARQL.AS_GML, new RDFVariable("?GEOMGML")).Optional())
                    .AddFilter(new RDFBooleanOrFilter(
                        new RDFDatatypeFilter(new RDFVariable("?GEOMWKT"), RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT),
                        new RDFDatatypeFilter(new RDFVariable("?GEOMGML"), RDFModelEnums.RDFDatatypes.GEOSPARQL_GML))))
                .AddProjectionVariable(new RDFVariable("?GEOMWKT"))
                .AddProjectionVariable(new RDFVariable("?GEOMGML"));
            RDFSelectQueryResult selectQueryResult = selectQuery.ApplyToGraph(geoOntology.Data.ABoxGraph);

            //Parse retrieved WKT/GML serialization into (WGS84,UTM) result geometries
            foreach (DataRow selectResultsRow in selectQueryResult.SelectResults.Rows)
            {
                bool geometryCollected = false;

                //WKT
                if (!selectResultsRow.IsNull("?GEOMWKT"))
                {
                    try
                    {
                        //Parse geometry from WKT
                        RDFTypedLiteral wktGeometryLiteral = (RDFTypedLiteral)RDFQueryUtilities.ParseRDFPatternMember(selectResultsRow["?GEOMWKT"].ToString());
                        Geometry wgs84Geometry = new WKTReader().Read(wktGeometryLiteral.Value);
                        wgs84Geometry.SRID = 4326;

                        //Project geometry from WGS84 to UTM
                        (int, bool) utmZone = GEOConverter.GetUTMZoneFromWGS84Coordinates(wgs84Geometry.Coordinates[0].X, wgs84Geometry.Coordinates[0].Y);
                        Geometry utmGeometry = GEOConverter.GetUTMGeometryFromWGS84(wgs84Geometry, utmZone);

                        geometryCollected = true;
                        secondaryGeometries.Add((wgs84Geometry, utmGeometry));
                    }
                    catch { /* Just a no-op, since type errors are normal when trying to face variable's bindings */ }
                }

                //GML
                if (!geometryCollected && !selectResultsRow.IsNull("?GEOMGML"))
                {
                    try
                    {
                        //Parse default geometry from GML
                        RDFTypedLiteral gmlGeometryLiteral = (RDFTypedLiteral)RDFQueryUtilities.ParseRDFPatternMember(selectResultsRow["?GEOMGML"].ToString());
                        Geometry wgs84Geometry = new GMLReader().Read(gmlGeometryLiteral.Value);
                        wgs84Geometry.SRID = 4326;

                        //Project default geometry from WGS84 to UTM
                        (int, bool) utmZone = GEOConverter.GetUTMZoneFromWGS84Coordinates(wgs84Geometry.Coordinates[0].X, wgs84Geometry.Coordinates[0].Y);
                        Geometry utmGeometry = GEOConverter.GetUTMGeometryFromWGS84(wgs84Geometry, utmZone);

                        secondaryGeometries.Add((wgs84Geometry, utmGeometry));
                    }
                    catch { /* Just a no-op, since type errors are normal when trying to face variable's bindings */ }
                }
            }

            return secondaryGeometries;
        }

        /// <summary>
        /// Gets the features having at least one serialized geometry (WGS84,UTM)
        /// </summary>
        internal static List<(RDFResource,Geometry,Geometry)> GetFeaturesWithGeometries(this GEOOntology geoOntology)
        {
            List<(RDFResource,Geometry,Geometry)> featuresWithGeometry = new List<(RDFResource,Geometry,Geometry)>();

            //Execute SPARQL query to retrieve WKT/GML serialization of features having geometries
            RDFSelectQuery selectQuery = new RDFSelectQuery()
                .AddPatternGroup(new RDFPatternGroup()
                    .AddPattern(new RDFPattern(new RDFVariable("?FEATURE"), RDFVocabulary.GEOSPARQL.DEFAULT_GEOMETRY, new RDFVariable("?GEOM")))
                    .AddPattern(new RDFPattern(new RDFVariable("?GEOM"), RDFVocabulary.GEOSPARQL.AS_WKT, new RDFVariable("?GEOMWKT")).Optional())
                    .AddPattern(new RDFPattern(new RDFVariable("?GEOM"), RDFVocabulary.GEOSPARQL.AS_GML, new RDFVariable("?GEOMGML")).Optional())
                    .AddFilter(new RDFIsUriFilter(new RDFVariable("?FEATURE")))
                    .AddFilter(new RDFBooleanOrFilter(
                        new RDFDatatypeFilter(new RDFVariable("?GEOMWKT"), RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT),
                        new RDFDatatypeFilter(new RDFVariable("?GEOMGML"), RDFModelEnums.RDFDatatypes.GEOSPARQL_GML)))
                    .UnionWithNext())
                .AddPatternGroup(new RDFPatternGroup()
                    .AddPattern(new RDFPattern(new RDFVariable("?FEATURE"), RDFVocabulary.GEOSPARQL.HAS_GEOMETRY, new RDFVariable("?GEOM")))
                    .AddPattern(new RDFPattern(new RDFVariable("?GEOM"), RDFVocabulary.GEOSPARQL.AS_WKT, new RDFVariable("?GEOMWKT")).Optional())
                    .AddPattern(new RDFPattern(new RDFVariable("?GEOM"), RDFVocabulary.GEOSPARQL.AS_GML, new RDFVariable("?GEOMGML")).Optional())
                    .AddFilter(new RDFIsUriFilter(new RDFVariable("?FEATURE")))
                    .AddFilter(new RDFBooleanOrFilter(
                        new RDFDatatypeFilter(new RDFVariable("?GEOMWKT"), RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT),
                        new RDFDatatypeFilter(new RDFVariable("?GEOMGML"), RDFModelEnums.RDFDatatypes.GEOSPARQL_GML))))
                .AddProjectionVariable(new RDFVariable("?FEATURE"))
                .AddProjectionVariable(new RDFVariable("?GEOMWKT"))
                .AddProjectionVariable(new RDFVariable("?GEOMGML"));
            RDFSelectQueryResult selectQueryResult = selectQuery.ApplyToGraph(geoOntology.Data.ABoxGraph);

            //Parse retrieved WKT/GML serialization into (WGS84,UTM) result geometries
            foreach (DataRow selectResultsRow in selectQueryResult.SelectResults.Rows)
            {
                bool geometryCollected = false;

                //WKT
                if (!selectResultsRow.IsNull("?GEOMWKT"))
                {
                    try
                    {
                        //Parse feature URI
                        RDFResource featureUri = (RDFResource)RDFQueryUtilities.ParseRDFPatternMember(selectResultsRow["?FEATURE"].ToString());

                        //Parse geometry from WKT
                        RDFTypedLiteral wktGeometryLiteral = (RDFTypedLiteral)RDFQueryUtilities.ParseRDFPatternMember(selectResultsRow["?GEOMWKT"].ToString());
                        Geometry wgs84Geometry = new WKTReader().Read(wktGeometryLiteral.Value);
                        wgs84Geometry.SRID = 4326;

                        //Project geometry from WGS84 to UTM
                        (int, bool) utmZone = GEOConverter.GetUTMZoneFromWGS84Coordinates(wgs84Geometry.Coordinates[0].X, wgs84Geometry.Coordinates[0].Y);
                        Geometry utmGeometry = GEOConverter.GetUTMGeometryFromWGS84(wgs84Geometry, utmZone);

                        geometryCollected = true;
                        featuresWithGeometry.Add((featureUri, wgs84Geometry, utmGeometry));
                    }
                    catch { /* Just a no-op, since type errors are normal when trying to face variable's bindings */ }
                }

                //GML
                if (!geometryCollected && !selectResultsRow.IsNull("?GEOMGML"))
                {
                    try
                    {
                        //Parse feature URI
                        RDFResource featureUri = (RDFResource)RDFQueryUtilities.ParseRDFPatternMember(selectResultsRow["?FEATURE"].ToString());

                        //Parse default geometry from GML
                        RDFTypedLiteral gmlGeometryLiteral = (RDFTypedLiteral)RDFQueryUtilities.ParseRDFPatternMember(selectResultsRow["?GEOMGML"].ToString());
                        Geometry wgs84Geometry = new GMLReader().Read(gmlGeometryLiteral.Value);
                        wgs84Geometry.SRID = 4326;

                        //Project default geometry from WGS84 to UTM
                        (int, bool) utmZone = GEOConverter.GetUTMZoneFromWGS84Coordinates(wgs84Geometry.Coordinates[0].X, wgs84Geometry.Coordinates[0].Y);
                        Geometry utmGeometry = GEOConverter.GetUTMGeometryFromWGS84(wgs84Geometry, utmZone);

                        featuresWithGeometry.Add((featureUri, wgs84Geometry, utmGeometry));
                    }
                    catch { /* Just a no-op, since type errors are normal when trying to face variable's bindings */ }
                }
            }

            return featuresWithGeometry;
        }

        //SPATIAL ANALYSIS

        /// <summary>
        /// Gets the distance, expressed in meters, between the given geosparql:Feature instances
        /// </summary>
        public static double? GetDistanceBetweenFeatures(this GEOOntology geoOntology, RDFResource fromFeatureUri, RDFResource toFeatureUri)
        {
            if (fromFeatureUri == null)
                throw new OWLSemanticsException("Cannot get distance between features because given \"fromFeatureUri\" parameter is null");
            if (toFeatureUri == null)
                throw new OWLSemanticsException("Cannot get distance between features because given \"toFeatureUri\" parameter is null");

            //Collect geometries of "From" feature
            (Geometry,Geometry) defaultGeometryOfFromFeature = GetDefaultGeometryOfFeature(geoOntology, fromFeatureUri);
            List<(Geometry,Geometry)> secondaryGeometriesOfFromFeature = GetSecondaryGeometriesOfFeature(geoOntology, fromFeatureUri);
            if (defaultGeometryOfFromFeature.Item1 != null && defaultGeometryOfFromFeature.Item2 != null)
                secondaryGeometriesOfFromFeature.Add(defaultGeometryOfFromFeature);

            //Collect geometries of "To" feature
            (Geometry,Geometry) defaultGeometryOfToFeature = GetDefaultGeometryOfFeature(geoOntology, toFeatureUri);
            List<(Geometry,Geometry)> secondaryGeometriesOfToFeature = GetSecondaryGeometriesOfFeature(geoOntology, toFeatureUri);
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
        /// Gets the features around the given WGS84 Lon/Lat point in a radius of given search meters 
        /// </summary>
        public static List<RDFResource> GetFeaturesNearPoint(this GEOOntology geoOntology, (double,double) wgs84LonLat, double radiusMeters)
        {
            if (wgs84LonLat.Item1 < -180 || wgs84LonLat.Item1 > 180)
                throw new OWLSemanticsException("Cannot get features near point because given \"wgs84LonLat\" parameter has not a valid longitude for WGS84");
            if (wgs84LonLat.Item2 < -90 || wgs84LonLat.Item2 > 90)
                throw new OWLSemanticsException("Cannot get features near point because given \"wgs84LonLat\" parameter has not a valid latitude for WGS84");

            //Create WGS84 geometry from given center of search
            Geometry wgs84SearchPoint = new Point(wgs84LonLat.Item1, wgs84LonLat.Item2) { SRID = 4326 };

            //Create UTM geometry from given center of search
            (int,bool) utmZoneSearchPoint = GEOConverter.GetUTMZoneFromWGS84Coordinates(wgs84LonLat.Item1, wgs84LonLat.Item2);
            Geometry utmSearchPoint = GEOConverter.GetUTMGeometryFromWGS84(wgs84SearchPoint, utmZoneSearchPoint);

            //Execute SPARQL query to retrieve WKT/GML serialization of features having geometries
            List<(RDFResource,Geometry,Geometry)> featuresWithGeometry = GetFeaturesWithGeometries(geoOntology);

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
        public static List<RDFResource> GetFeaturesNorthOfPoint(this GEOOntology geoOntology, (double, double) wgs84LonLat)
        {
            if (wgs84LonLat.Item1 < -180 || wgs84LonLat.Item1 > 180)
                throw new OWLSemanticsException("Cannot get features near point because given \"wgs84LonLat\" parameter has not a valid longitude for WGS84");
            if (wgs84LonLat.Item2 < -90 || wgs84LonLat.Item2 > 90)
                throw new OWLSemanticsException("Cannot get features near point because given \"wgs84LonLat\" parameter has not a valid latitude for WGS84");

            //Create WGS84 geometry from given point
            Geometry wgs84SearchPoint = new Point(wgs84LonLat.Item1, wgs84LonLat.Item2) { SRID = 4326 };

            //Create UTM geometry from given point
            (int, bool) utmZoneSearchPoint = GEOConverter.GetUTMZoneFromWGS84Coordinates(wgs84LonLat.Item1, wgs84LonLat.Item2);
            Geometry utmSearchPoint = GEOConverter.GetUTMGeometryFromWGS84(wgs84SearchPoint, utmZoneSearchPoint);

            //Execute SPARQL query to retrieve WKT/GML serialization of features having geometries
            List<(RDFResource, Geometry, Geometry)> featuresWithGeometry = GetFeaturesWithGeometries(geoOntology);

            //Perform spatial analysis between collected geometries: iterate geometries and collect those having latitudes higher than given point
            List<RDFResource> featuresNorthOfPoint = new List<RDFResource>();
            featuresWithGeometry.ForEach(featureWithGeometry => {
                if (featureWithGeometry.Item3.Coordinates.Any(coordinate => coordinate.Y > utmSearchPoint.Coordinate.Y))
                    featuresNorthOfPoint.Add(featureWithGeometry.Item1);
            });

            return RDFQueryUtilities.RemoveDuplicates(featuresNorthOfPoint);
        }

        /// <summary>
        /// Gets the features within the given box represented by WGS84 Lon/Lat (lower-left, upper-right) corner points
        /// </summary>
        public static List<RDFResource> GetFeaturesWithinBox(this GEOOntology geoOntology, (double,double) wgs84LonLat_LowerLeft, (double,double) wgs84LonLat_UpperRight)
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
            List<(RDFResource,Geometry,Geometry)> featuresWithGeometry = GetFeaturesWithGeometries(geoOntology);

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