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
using System.Threading.Tasks;

namespace RDFSharp.Semantics.Extensions.GEO.Test
{
    [TestClass]
    public class GEOOntologyTest
    {
        #region Tests
        [TestMethod]
        public void ShouldCreateGEOOntology()
        {
            GEOOntology geoOnt = new GEOOntology("ex:geoOnt");

            Assert.IsNotNull(geoOnt);
            Assert.IsNotNull(geoOnt.SpatialHelper);

            //Test initialization of GEO knowledge
            Assert.IsTrue(geoOnt.URI.Equals(geoOnt.URI));
            Assert.IsTrue(geoOnt.Model.ClassModel.ClassesCount == 19);
            Assert.IsTrue(geoOnt.Model.PropertyModel.PropertiesCount == 34);
            Assert.IsTrue(geoOnt.Data.IndividualsCount == 0);
        }

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
            graph.AddTriple(new RDFTriple(new RDFResource("ex:milan"), new RDFResource("ex:hasName"), new RDFPlainLiteral("Milano", "it-IT")));
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


            //LOAD OWL+GEO
            GEOOntology geoOntology = GEOOntology.FromRDFGraph(graph);

            //Test persistence of OWL+GEO knowledge
            Assert.IsNotNull(geoOntology);
            Assert.IsTrue(geoOntology.URI.Equals(RDFNamespaceRegister.DefaultNamespace.NamespaceUri));
            Assert.IsTrue(geoOntology.Model.ClassModel.ClassesCount == 20);
            Assert.IsTrue(geoOntology.Model.PropertyModel.PropertiesCount == 36);
            Assert.IsTrue(geoOntology.Data.IndividualsCount == 5);
            Assert.IsTrue(geoOntology.Model.PropertyModel.CheckHasAnnotation(new RDFResource("ex:connectedToCity"), RDFVocabulary.RDFS.COMMENT, new RDFPlainLiteral("two cities are connected each other")));
            Assert.IsTrue(geoOntology.Data.CheckHasObjectAssertion(new RDFResource("ex:milan"), new RDFResource("ex:connectedToCity"), new RDFResource("ex:rome")));
            Assert.IsTrue(geoOntology.Data.CheckHasDatatypeAssertion(new RDFResource("ex:milan"), new RDFResource("ex:hasName"), new RDFPlainLiteral("Milano", "it-IT")));
            Assert.IsTrue(geoOntology.Data.CheckHasAnnotation(new RDFResource("ex:milan"), RDFVocabulary.DC.DESCRIPTION, new RDFPlainLiteral("this is the city of Milan,IT")));
        }

        [TestMethod]
        public async Task ShouldLoadFromGraphAsync()
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


            //LOAD OWL+GEO
            GEOOntology geoOntology = await GEOOntology.FromRDFGraphAsync(graph);

            //Test persistence of OWL+GEO knowledge
            Assert.IsNotNull(geoOntology);
            Assert.IsTrue(geoOntology.URI.Equals(RDFNamespaceRegister.DefaultNamespace.NamespaceUri));
            Assert.IsTrue(geoOntology.Model.ClassModel.ClassesCount == 20);
            Assert.IsTrue(geoOntology.Model.PropertyModel.PropertiesCount == 36);
            Assert.IsTrue(geoOntology.Data.IndividualsCount == 5);
            Assert.IsTrue(geoOntology.Model.PropertyModel.CheckHasAnnotation(new RDFResource("ex:connectedToCity"), RDFVocabulary.RDFS.COMMENT, new RDFPlainLiteral("two cities are connected each other")));
            Assert.IsTrue(geoOntology.Data.CheckHasObjectAssertion(new RDFResource("ex:milan"), new RDFResource("ex:connectedToCity"), new RDFResource("ex:rome")));
            Assert.IsTrue(geoOntology.Data.CheckHasDatatypeAssertion(new RDFResource("ex:milan"), new RDFResource("ex:hasName"), new RDFPlainLiteral("Milano", "it-IT")));
            Assert.IsTrue(geoOntology.Data.CheckHasAnnotation(new RDFResource("ex:milan"), RDFVocabulary.DC.DESCRIPTION, new RDFPlainLiteral("this is the city of Milan,IT")));
        }

        [TestMethod]
        public async Task ShouldLoadFromGraphWithOptionsAsync()
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


            //LOAD OWL+GEO
            GEOOntology geoOntology = await GEOOntology.FromRDFGraphAsync(graph, new OWLOntologyLoaderOptions() { EnableAutomaticEntityDeclaration=true });

            //Test persistence of OWL+GEO knowledge
            Assert.IsNotNull(geoOntology);
            Assert.IsTrue(geoOntology.URI.Equals(RDFNamespaceRegister.DefaultNamespace.NamespaceUri));
            Assert.IsTrue(geoOntology.Model.ClassModel.ClassesCount == 20);
            Assert.IsTrue(geoOntology.Model.PropertyModel.PropertiesCount == 36);
            Assert.IsTrue(geoOntology.Data.IndividualsCount == 5);
            Assert.IsTrue(geoOntology.Model.PropertyModel.CheckHasAnnotation(new RDFResource("ex:connectedToCity"), RDFVocabulary.RDFS.COMMENT, new RDFPlainLiteral("two cities are connected each other")));
            Assert.IsTrue(geoOntology.Data.CheckHasObjectAssertion(new RDFResource("ex:milan"), new RDFResource("ex:connectedToCity"), new RDFResource("ex:rome")));
            Assert.IsTrue(geoOntology.Data.CheckHasDatatypeAssertion(new RDFResource("ex:milan"), new RDFResource("ex:hasName"), new RDFPlainLiteral("Milano", "it-IT")));
            Assert.IsTrue(geoOntology.Data.CheckHasAnnotation(new RDFResource("ex:milan"), RDFVocabulary.DC.DESCRIPTION, new RDFPlainLiteral("this is the city of Milan,IT")));
        }
        #endregion
    }
}