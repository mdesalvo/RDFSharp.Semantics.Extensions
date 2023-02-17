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

using Microsoft.VisualStudio.TestTools.UnitTesting;
using NetTopologySuite.Geometries;
using RDFSharp.Model;

namespace RDFSharp.Semantics.Extensions.GEO.Test
{
    [TestClass]
    public class GEOOntologyLoaderTest
    {
        #region Tests
        [TestMethod]
        public void ShouldLoadFromGraph()
        {
            RDFGraph graph = new RDFGraph();

            //OWL knowledge
            graph.AddTriple(new RDFTriple(new RDFResource("ex:City"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.CLASS));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:hasName"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.DATATYPE_PROPERTY));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:connectedToCity"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.OBJECT_PROPERTY));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:connectedToCity"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.REFLEXIVE_PROPERTY));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:connectedToCity"), RDFVocabulary.RDFS.COMMENT, new RDFPlainLiteral("two cities are connected each other")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:milan"), RDFVocabulary.RDF.TYPE, new RDFResource("ex:City")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:rome"), RDFVocabulary.RDF.TYPE, new RDFResource("ex:City")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:naples"), RDFVocabulary.RDF.TYPE, new RDFResource("ex:City")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:milan"), RDFVocabulary.DC.DESCRIPTION, new RDFPlainLiteral("this is the city of Milan,IT")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:milan"), new RDFResource("ex:hasName"), new RDFPlainLiteral("Milano","it-IT")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:rome"), RDFVocabulary.DC.DESCRIPTION, new RDFPlainLiteral("this is the city of Rome,IT")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:rome"), new RDFResource("ex:hasName"), new RDFPlainLiteral("Roma", "it-IT")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:naples"), RDFVocabulary.DC.DESCRIPTION, new RDFPlainLiteral("this is the city of Naples,IT")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:naples"), new RDFResource("ex:hasName"), new RDFPlainLiteral("Napoli", "it-IT")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:milan"), new RDFResource("ex:connectedToCity"), new RDFResource("ex:rome")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:rome"), new RDFResource("ex:connectedToCity"), new RDFResource("ex:naples")));

            //GEO knowledge
            graph.AddTriple(new RDFTriple(new RDFResource("ex:milan"), RDFVocabulary.RDF.TYPE, RDFVocabulary.GEOSPARQL.SF.POINT));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:milan"), RDFVocabulary.GEOSPARQL.AS_WKT, new RDFTypedLiteral("POINT (9.18854 45.464664)", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT)));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:milan"), RDFVocabulary.GEOSPARQL.AS_GML, new RDFTypedLiteral("<gml:Point xmlns:gml=\"http://www.opengis.net/gml/3.2\"><gml:pos>9.18854 45.464664</gml:pos></gml:Point>", RDFModelEnums.RDFDatatypes.GEOSPARQL_GML)));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:rome"), RDFVocabulary.RDF.TYPE, RDFVocabulary.GEOSPARQL.SF.POINT));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:rome"), RDFVocabulary.GEOSPARQL.AS_WKT, new RDFTypedLiteral("POINT (12.496365 41.902782)", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT)));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:rome"), RDFVocabulary.GEOSPARQL.AS_GML, new RDFTypedLiteral("<gml:Point xmlns:gml=\"http://www.opengis.net/gml/3.2\"><gml:pos>12.496365 41.902782</gml:pos></gml:Point>", RDFModelEnums.RDFDatatypes.GEOSPARQL_GML)));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:naples"), RDFVocabulary.RDF.TYPE, RDFVocabulary.GEOSPARQL.SF.POINT));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:naples"), RDFVocabulary.GEOSPARQL.AS_WKT, new RDFTypedLiteral("POINT (14.2681244 40.8517746)", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT)));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:naples"), RDFVocabulary.GEOSPARQL.AS_GML, new RDFTypedLiteral("<gml:Point xmlns:gml=\"http://www.opengis.net/gml/3.2\"><gml:pos>14.2681244 40.8517746</gml:pos></gml:Point>", RDFModelEnums.RDFDatatypes.GEOSPARQL_GML)));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:milanrome"), RDFVocabulary.RDF.TYPE, RDFVocabulary.GEOSPARQL.SF.LINESTRING));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:milanrome"), RDFVocabulary.GEOSPARQL.AS_WKT, new RDFTypedLiteral("LINESTRING (9.18854 45.464664, 12.496365 41.902782)", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT)));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:milanrome"), RDFVocabulary.GEOSPARQL.AS_GML, new RDFTypedLiteral("<gml:LineString xmlns:gml=\"http://www.opengis.net/gml/3.2\"><gml:posList>9.18854 45.464664 12.496365 41.902782</gml:posList></gml:LineString>", RDFModelEnums.RDFDatatypes.GEOSPARQL_GML)));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:milanromenaples"), RDFVocabulary.RDF.TYPE, RDFVocabulary.GEOSPARQL.SF.POLYGON));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:milanromenaples"), RDFVocabulary.GEOSPARQL.AS_WKT, new RDFTypedLiteral("POLYGON ((9.18854 45.464664, 12.496365 41.902782, 14.2681244 40.8517746, 9.18854 45.464664))", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT)));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:milanromenaples"), RDFVocabulary.GEOSPARQL.AS_GML, new RDFTypedLiteral("<gml:Polygon xmlns:gml=\"http://www.opengis.net/gml/3.2\"><gml:exterior><gml:LinearRing><gml:posList>9.18854 45.464664 12.496365 41.902782 14.2681244 40.8517746 9.18854 45.464664</gml:posList></gml:LinearRing></gml:exterior></gml:Polygon>", RDFModelEnums.RDFDatatypes.GEOSPARQL_GML)));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:milanrome_mpt"), RDFVocabulary.RDF.TYPE, RDFVocabulary.GEOSPARQL.SF.MULTI_POINT));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:milanrome_mpt"), RDFVocabulary.GEOSPARQL.AS_WKT, new RDFTypedLiteral("MULTIPOINT ((9.18854 45.464664), (12.496365 41.902782))", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT)));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:milanrome_mpt"), RDFVocabulary.GEOSPARQL.AS_GML, new RDFTypedLiteral("<gml:MultiPoint xmlns:gml=\"http://www.opengis.net/gml/3.2\"><gml:pointMember><gml:Point><gml:pos>9.18854 45.464664</gml:pos></gml:Point></gml:pointMember><gml:pointMember><gml:Point><gml:pos>12.496365 41.902782</gml:pos></gml:Point></gml:pointMember></gml:MultiPoint>", RDFModelEnums.RDFDatatypes.GEOSPARQL_GML)));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:milanromenaples_mls"), RDFVocabulary.RDF.TYPE, RDFVocabulary.GEOSPARQL.SF.MULTI_LINESTRING));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:milanromenaples_mls"), RDFVocabulary.GEOSPARQL.AS_WKT, new RDFTypedLiteral("MULTILINESTRING ((9.18854 45.464664, 12.496365 41.902782), (12.496365 41.902782, 14.2681244 40.8517746))", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT)));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:milanromenaples_mls"), RDFVocabulary.GEOSPARQL.AS_GML, new RDFTypedLiteral("<gml:MultiCurve xmlns:gml=\"http://www.opengis.net/gml/3.2\"><gml:curveMember><gml:LineString><gml:posList>9.18854 45.464664 12.496365 41.902782</gml:posList></gml:LineString></gml:curveMember><gml:curveMember><gml:LineString><gml:posList>12.496365 41.902782 14.2681244 40.8517746</gml:posList></gml:LineString></gml:curveMember></gml:MultiCurve>", RDFModelEnums.RDFDatatypes.GEOSPARQL_GML)));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:milanromenaples_mpl"), RDFVocabulary.RDF.TYPE, RDFVocabulary.GEOSPARQL.SF.MULTI_POLYGON));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:milanromenaples_mpl"), RDFVocabulary.GEOSPARQL.AS_WKT, new RDFTypedLiteral("MULTIPOLYGON (((9.18854 45.464664, 12.496365 41.902782, 14.2681244 40.8517746, 9.18854 45.464664)), ((12.496365 41.902782, 14.2681244 40.8517746, 9.18854 45.464664, 12.496365 41.902782)))", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT)));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:milanromenaples_mpl"), RDFVocabulary.GEOSPARQL.AS_GML, new RDFTypedLiteral("<gml:MultiSurface xmlns:gml=\"http://www.opengis.net/gml/3.2\"><gml:surfaceMember><gml:Polygon><gml:exterior><gml:LinearRing><gml:posList>9.18854 45.464664 12.496365 41.902782 14.2681244 40.8517746 9.18854 45.464664</gml:posList></gml:LinearRing></gml:exterior></gml:Polygon></gml:surfaceMember><gml:surfaceMember><gml:Polygon><gml:exterior><gml:LinearRing><gml:posList>12.496365 41.902782 14.2681244 40.8517746 9.18854 45.464664 12.496365 41.902782</gml:posList></gml:LinearRing></gml:exterior></gml:Polygon></gml:surfaceMember></gml:MultiSurface>", RDFModelEnums.RDFDatatypes.GEOSPARQL_GML)));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:geometrycollection"), RDFVocabulary.RDF.TYPE, RDFVocabulary.GEOSPARQL.SF.GEOMETRY_COLLECTION));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:geometrycollection"), RDFVocabulary.GEOSPARQL.AS_WKT, new RDFTypedLiteral("GEOMETRYCOLLECTION (POINT (9.18854 45.464664), POINT (12.496365 41.902782), POINT (14.2681244 40.8517746), LINESTRING (9.18854 45.464664, 12.496365 41.902782), LINESTRING (14.2681244 40.8517746, 9.18854 45.464664), POLYGON ((9.18854 45.464664, 12.496365 41.902782, 14.2681244 40.8517746, 9.18854 45.464664)), POLYGON ((12.496365 41.902782, 14.2681244 40.8517746, 9.18854 45.464664, 12.496365 41.902782)))", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT)));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:geometrycollection"), RDFVocabulary.GEOSPARQL.AS_GML, new RDFTypedLiteral("<gml:MultiGeometry xmlns:gml=\"http://www.opengis.net/gml/3.2\"><gml:geometryMember><gml:Point><gml:pos>9.18854 45.464664</gml:pos></gml:Point></gml:geometryMember><gml:geometryMember><gml:Point><gml:pos>12.496365 41.902782</gml:pos></gml:Point></gml:geometryMember><gml:geometryMember><gml:Point><gml:pos>14.2681244 40.8517746</gml:pos></gml:Point></gml:geometryMember><gml:geometryMember><gml:LineString><gml:posList>9.18854 45.464664 12.496365 41.902782</gml:posList></gml:LineString></gml:geometryMember><gml:geometryMember><gml:LineString><gml:posList>14.2681244 40.8517746 9.18854 45.464664</gml:posList></gml:LineString></gml:geometryMember><gml:geometryMember><gml:Polygon><gml:exterior><gml:LinearRing><gml:posList>9.18854 45.464664 12.496365 41.902782 14.2681244 40.8517746 9.18854 45.464664</gml:posList></gml:LinearRing></gml:exterior></gml:Polygon></gml:geometryMember><gml:geometryMember><gml:Polygon><gml:exterior><gml:LinearRing><gml:posList>12.496365 41.902782 14.2681244 40.8517746 9.18854 45.464664 12.496365 41.902782</gml:posList></gml:LinearRing></gml:exterior></gml:Polygon></gml:geometryMember></gml:MultiGeometry>", RDFModelEnums.RDFDatatypes.GEOSPARQL_GML)));


            //LOAD OWL+GEO
            GEOOntology geoOntology = GEOOntologyLoader.FromRDFGraph(graph, null);

            //Test persistence of OWL+GEO knowledge
            Assert.IsNotNull(geoOntology);
            Assert.IsTrue(geoOntology.URI.Equals(RDFNamespaceRegister.DefaultNamespace.NamespaceUri));
            Assert.IsTrue(geoOntology.Model.ClassModel.ClassesCount == 20);
            Assert.IsTrue(geoOntology.Model.PropertyModel.PropertiesCount == 36);
            Assert.IsTrue(geoOntology.Data.IndividualsCount == 9);
            Assert.IsTrue(geoOntology.Geometries.Count == 9);
            Assert.IsTrue(geoOntology.Geometries.ContainsKey("ex:milan"));
            Assert.IsTrue(geoOntology.Geometries["ex:milan"].Item1.EqualsTopologically(new Point(9.18854, 45.464664)));
            Assert.IsTrue(geoOntology.Geometries["ex:milan"].Item2.EqualsTopologically(new Point(514739.23764243146, 5034588.076214247)));
            Assert.IsTrue(geoOntology.Geometries.ContainsKey("ex:rome"));
            Assert.IsTrue(geoOntology.Geometries["ex:rome"].Item1.EqualsTopologically(new Point(12.496365, 41.902782)));
            Assert.IsTrue(geoOntology.Geometries["ex:rome"].Item2.EqualsTopologically(new Point(292332.3514461573, 4642013.586099927)));
            Assert.IsTrue(geoOntology.Geometries.ContainsKey("ex:naples"));
            Assert.IsTrue(geoOntology.Geometries["ex:naples"].Item1.EqualsTopologically(new Point(14.2681244, 40.8517746)));
            Assert.IsTrue(geoOntology.Geometries["ex:naples"].Item2.EqualsTopologically(new Point(438310.2105054362, 4522560.57431237)));
            Assert.IsTrue(geoOntology.Geometries.ContainsKey("ex:milanrome"));
            Assert.IsTrue(geoOntology.Geometries["ex:milanrome"].Item1.EqualsTopologically(new LineString(new Coordinate[] { new Coordinate(9.18854, 45.464664), new Coordinate(12.496365, 41.902782) })));
            Assert.IsTrue(geoOntology.Geometries["ex:milanrome"].Item2.EqualsTopologically(new LineString(new Coordinate[] { new Coordinate(514739.23764243146, 5034588.076214247), new Coordinate(790020.659168055, 4644896.163254826) })));
            Assert.IsTrue(geoOntology.Geometries.ContainsKey("ex:milanromenaples"));
            Assert.IsTrue(geoOntology.Geometries["ex:milanromenaples"].Item1.EqualsTopologically(new Polygon(new LinearRing(new Coordinate[] { new Coordinate(9.18854, 45.464664), new Coordinate(12.496365, 41.902782), new Coordinate(14.2681244, 40.8517746), new Coordinate(9.18854, 45.464664) }))));
            Assert.IsTrue(geoOntology.Geometries["ex:milanromenaples"].Item2.EqualsTopologically(new Polygon(new LinearRing(new Coordinate[] { new Coordinate(514739.23764243146, 5034588.076214247), new Coordinate(790020.659168055, 4644896.163254826), new Coordinate(944139.3677008688, 4535678.997426257), new Coordinate(514739.23764243146, 5034588.076214247) }))));
            Assert.IsTrue(geoOntology.Geometries.ContainsKey("ex:milanrome_mpt"));
            Assert.IsTrue(geoOntology.Geometries["ex:milanrome_mpt"].Item1.EqualsTopologically(new MultiPoint(new Point[] { 
                new Point(9.18854, 45.464664), new Point(12.496365, 41.902782) })));
            Assert.IsTrue(geoOntology.Geometries["ex:milanrome_mpt"].Item2.EqualsTopologically(new MultiPoint(new Point[] { 
                new Point(514739.23764243146, 5034588.076214247), new Point(790020.659168055, 4644896.163254826) })));
            Assert.IsTrue(geoOntology.Geometries.ContainsKey("ex:milanromenaples_mls"));
            Assert.IsTrue(geoOntology.Geometries["ex:milanromenaples_mls"].Item1.EqualsTopologically(new MultiLineString(new LineString[] {
                new LineString(new Coordinate[] { new Coordinate(9.18854, 45.464664), new Coordinate(12.496365, 41.902782) }),
                new LineString(new Coordinate[] { new Coordinate(12.496365, 41.902782), new Coordinate(14.2681244, 40.8517746) })
            })));
            Assert.IsTrue(geoOntology.Geometries["ex:milanromenaples_mls"].Item2.EqualsTopologically(new MultiLineString(new LineString[] {
                new LineString(new Coordinate[] { new Coordinate(514739.23764243146, 5034588.076214247), new Coordinate(790020.659168055, 4644896.163254826) }),
                new LineString(new Coordinate[] { new Coordinate(790020.659168055, 4644896.163254826), new Coordinate(944139.3677008688, 4535678.997426257) })
            })));
            Assert.IsTrue(geoOntology.Geometries.ContainsKey("ex:milanromenaples_mpl"));
            Assert.IsTrue(geoOntology.Geometries["ex:milanromenaples_mpl"].Item1.EqualsTopologically(new MultiPolygon(new Polygon[] { 
                new Polygon(new LinearRing(new Coordinate[] { new Coordinate(9.18854, 45.464664), new Coordinate(12.496365, 41.902782), new Coordinate(14.2681244, 40.8517746), new Coordinate(9.18854, 45.464664) })),
                new Polygon(new LinearRing(new Coordinate[] { new Coordinate(12.496365, 41.902782), new Coordinate(14.2681244, 40.8517746), new Coordinate(9.18854, 45.464664), new Coordinate(12.496365, 41.902782) }))
            })));
            Assert.IsTrue(geoOntology.Geometries["ex:milanromenaples_mpl"].Item2.EqualsTopologically(new MultiPolygon(new Polygon[] { 
                new Polygon(new LinearRing(new Coordinate[] { new Coordinate(514739.23764243146, 5034588.076214247), new Coordinate(790020.659168055, 4644896.163254826), new Coordinate(944139.3677008688, 4535678.997426257), new Coordinate(514739.23764243146, 5034588.076214247) })),
                new Polygon(new LinearRing(new Coordinate[] { new Coordinate(790020.659168055, 4644896.163254826), new Coordinate(944139.3677008688, 4535678.997426257), new Coordinate(514739.23764243146, 5034588.076214247), new Coordinate(790020.659168055, 4644896.163254826) }))
            })));
            Assert.IsTrue(geoOntology.Geometries.ContainsKey("ex:geometrycollection"));
            Assert.IsTrue(geoOntology.Geometries["ex:geometrycollection"].Item1.OgcGeometryType == OgcGeometryType.GeometryCollection);
            Assert.IsTrue(geoOntology.Geometries["ex:geometrycollection"].Item2.OgcGeometryType == OgcGeometryType.GeometryCollection);
            Assert.IsTrue(geoOntology.Model.PropertyModel.CheckHasAnnotation(new RDFResource("ex:connectedToCity"), RDFVocabulary.RDFS.COMMENT, new RDFPlainLiteral("two cities are connected each other")));
            Assert.IsTrue(geoOntology.Data.CheckHasObjectAssertion(new RDFResource("ex:milan"), new RDFResource("ex:connectedToCity"), new RDFResource("ex:rome")));
            Assert.IsTrue(geoOntology.Data.CheckHasDatatypeAssertion(new RDFResource("ex:milan"), new RDFResource("ex:hasName"), new RDFPlainLiteral("Milano", "it-IT")));
            Assert.IsTrue(geoOntology.Data.CheckHasAnnotation(new RDFResource("ex:milan"), RDFVocabulary.DC.DESCRIPTION, new RDFPlainLiteral("this is the city of Milan,IT")));
        }

        [TestMethod]
        public void ShouldLoadFromGraphWithoutSpatialRepresentations()
        {
            RDFGraph graph = new RDFGraph();

            //OWL knowledge
            graph.AddTriple(new RDFTriple(new RDFResource("ex:City"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.CLASS));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:hasName"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.DATATYPE_PROPERTY));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:connectedToCity"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.OBJECT_PROPERTY));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:connectedToCity"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.REFLEXIVE_PROPERTY));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:connectedToCity"), RDFVocabulary.RDFS.COMMENT, new RDFPlainLiteral("two cities are connected each other")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:milan"), RDFVocabulary.RDF.TYPE, new RDFResource("ex:City")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:rome"), RDFVocabulary.RDF.TYPE, new RDFResource("ex:City")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:naples"), RDFVocabulary.RDF.TYPE, new RDFResource("ex:City")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:milan"), RDFVocabulary.DC.DESCRIPTION, new RDFPlainLiteral("this is the city of Milan,IT")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:milan"), new RDFResource("ex:hasName"), new RDFPlainLiteral("Milano", "it-IT")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:rome"), RDFVocabulary.DC.DESCRIPTION, new RDFPlainLiteral("this is the city of Rome,IT")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:rome"), new RDFResource("ex:hasName"), new RDFPlainLiteral("Roma", "it-IT")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:naples"), RDFVocabulary.DC.DESCRIPTION, new RDFPlainLiteral("this is the city of Naples,IT")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:naples"), new RDFResource("ex:hasName"), new RDFPlainLiteral("Napoli", "it-IT")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:milan"), new RDFResource("ex:connectedToCity"), new RDFResource("ex:rome")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:rome"), new RDFResource("ex:connectedToCity"), new RDFResource("ex:naples")));

            //GEO knowledge
            graph.AddTriple(new RDFTriple(new RDFResource("ex:milan"), RDFVocabulary.RDF.TYPE, RDFVocabulary.GEOSPARQL.SF.POINT));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:rome"), RDFVocabulary.RDF.TYPE, RDFVocabulary.GEOSPARQL.SF.POINT));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:naples"), RDFVocabulary.RDF.TYPE, RDFVocabulary.GEOSPARQL.SF.POINT));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:milanrome"), RDFVocabulary.RDF.TYPE, RDFVocabulary.GEOSPARQL.SF.LINESTRING));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:milanromenaples"), RDFVocabulary.RDF.TYPE, RDFVocabulary.GEOSPARQL.SF.POLYGON));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:milanrome_mpt"), RDFVocabulary.RDF.TYPE, RDFVocabulary.GEOSPARQL.SF.MULTI_POINT));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:milanromenaples_mls"), RDFVocabulary.RDF.TYPE, RDFVocabulary.GEOSPARQL.SF.MULTI_LINESTRING));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:milanromenaples_mpl"), RDFVocabulary.RDF.TYPE, RDFVocabulary.GEOSPARQL.SF.MULTI_POLYGON));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:geometrycollection"), RDFVocabulary.RDF.TYPE, RDFVocabulary.GEOSPARQL.SF.GEOMETRY_COLLECTION));

            //LOAD OWL+GEO
            GEOOntology geoOntology = GEOOntologyLoader.FromRDFGraph(graph, null);

            //Test persistence of OWL+GEO knowledge
            Assert.IsNotNull(geoOntology);
            Assert.IsTrue(geoOntology.URI.Equals(RDFNamespaceRegister.DefaultNamespace.NamespaceUri));
            Assert.IsTrue(geoOntology.Model.ClassModel.ClassesCount == 20);
            Assert.IsTrue(geoOntology.Model.PropertyModel.PropertiesCount == 36);
            Assert.IsTrue(geoOntology.Data.IndividualsCount == 9);
            Assert.IsTrue(geoOntology.Geometries.Count == 0);
            Assert.IsTrue(geoOntology.Model.PropertyModel.CheckHasAnnotation(new RDFResource("ex:connectedToCity"), RDFVocabulary.RDFS.COMMENT, new RDFPlainLiteral("two cities are connected each other")));
            Assert.IsTrue(geoOntology.Data.CheckHasObjectAssertion(new RDFResource("ex:milan"), new RDFResource("ex:connectedToCity"), new RDFResource("ex:rome")));
            Assert.IsTrue(geoOntology.Data.CheckHasDatatypeAssertion(new RDFResource("ex:milan"), new RDFResource("ex:hasName"), new RDFPlainLiteral("Milano", "it-IT")));
            Assert.IsTrue(geoOntology.Data.CheckHasAnnotation(new RDFResource("ex:milan"), RDFVocabulary.DC.DESCRIPTION, new RDFPlainLiteral("this is the city of Milan,IT")));
        }

        [TestMethod]
        public void ShouldLoadFromGraphAndCrashBecauseBadFormedGeometry()
        {
            RDFGraph graph = new RDFGraph();

            graph.AddTriple(new RDFTriple(new RDFResource("ex:milanromenaples"), RDFVocabulary.RDF.TYPE, RDFVocabulary.GEOSPARQL.SF.POLYGON));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:milanromenaples"), RDFVocabulary.GEOSPARQL.AS_WKT, new RDFTypedLiteral("POLYGON ((9.18854 45.464664, 12.496365 41.902782, 14.2681244 40.8517746))", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT)));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:milanromenaples"), RDFVocabulary.GEOSPARQL.AS_GML, new RDFTypedLiteral("<gml:Polygon xmlns:gml=\"http://www.opengis.net/gml/3.2\"><gml:exterior><gml:LinearRing><gml:posList>9.18854 45.464664 12.496365 41.902782 14.2681244 40.8517746</gml:posList></gml:LinearRing></gml:exterior></gml:Polygon>", RDFModelEnums.RDFDatatypes.GEOSPARQL_GML)));

            Assert.ThrowsException<OWLSemanticsException>(() => GEOOntologyLoader.FromRDFGraph(graph, null), "Cannot read spatial entity 'ex:milanromenaples' from graph because it's not a well-formed geometry.");
        }
        #endregion
    }
}