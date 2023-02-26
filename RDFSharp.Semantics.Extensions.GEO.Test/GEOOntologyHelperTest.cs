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
using System.Collections.Generic;
using System.Linq;

namespace RDFSharp.Semantics.Extensions.GEO.Test
{
    [TestClass]
    public class GEOOntologyHelperTest
    {
        #region Tests (Declarer)
        //geosparql:Feature
        [TestMethod]
        public void ShouldDeclareFeature()
        {
            GEOOntology geoOnt = new GEOOntology("ex:geoOnt");
            geoOnt.DeclareFeature(new RDFResource("ex:MilanFeature"));

            //Test evolution of GEO knowledge
            Assert.IsTrue(geoOnt.URI.Equals(geoOnt.URI));
            Assert.IsTrue(geoOnt.Model.ClassModel.ClassesCount == 19);
            Assert.IsTrue(geoOnt.Model.PropertyModel.PropertiesCount == 34);
            Assert.IsTrue(geoOnt.Data.IndividualsCount == 1);
            Assert.IsTrue(geoOnt.Data.CheckHasIndividual(new RDFResource("ex:MilanFeature")));
            Assert.IsTrue(geoOnt.Data.CheckIsIndividualOf(geoOnt.Model, new RDFResource("ex:MilanFeature"), RDFVocabulary.GEOSPARQL.FEATURE));
            Assert.IsTrue(geoOnt.Data.CheckIsIndividualOf(geoOnt.Model, new RDFResource("ex:MilanFeature"), RDFVocabulary.GEOSPARQL.SPATIAL_OBJECT));
        }

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringFeatureBecauseNullUri()
            => Assert.ThrowsException<OWLSemanticsException>(() => new GEOOntology("ex:geoOnt").DeclareFeature(null));

        [TestMethod]
        public void ShouldDeclareFeatureHasGeometry()
        {
            GEOOntology geoOnt = new GEOOntology("ex:geoOnt");
            geoOnt.DeclareSecondaryGeometry(new RDFResource("ex:MilanFeature"), new RDFResource("ex:MilanGeometry"));

            //Test evolution of GEO knowledge
            Assert.IsTrue(geoOnt.URI.Equals(geoOnt.URI));
            Assert.IsTrue(geoOnt.Model.ClassModel.ClassesCount == 19);
            Assert.IsTrue(geoOnt.Model.PropertyModel.PropertiesCount == 34);
            Assert.IsTrue(geoOnt.Data.IndividualsCount == 2);
            Assert.IsTrue(geoOnt.Data.CheckHasIndividual(new RDFResource("ex:MilanFeature")));
            Assert.IsTrue(geoOnt.Data.CheckIsIndividualOf(geoOnt.Model, new RDFResource("ex:MilanFeature"), RDFVocabulary.GEOSPARQL.FEATURE));
            Assert.IsTrue(geoOnt.Data.CheckIsIndividualOf(geoOnt.Model, new RDFResource("ex:MilanFeature"), RDFVocabulary.GEOSPARQL.SPATIAL_OBJECT));
            Assert.IsTrue(geoOnt.Data.CheckHasIndividual(new RDFResource("ex:MilanGeometry")));
            Assert.IsTrue(geoOnt.Data.CheckIsIndividualOf(geoOnt.Model, new RDFResource("ex:MilanGeometry"), RDFVocabulary.GEOSPARQL.GEOMETRY));
            Assert.IsTrue(geoOnt.Data.CheckIsIndividualOf(geoOnt.Model, new RDFResource("ex:MilanGeometry"), RDFVocabulary.GEOSPARQL.SPATIAL_OBJECT));
            Assert.IsTrue(geoOnt.Data.CheckHasObjectAssertion(new RDFResource("ex:MilanFeature"), RDFVocabulary.GEOSPARQL.HAS_GEOMETRY , new RDFResource("ex:MilanGeometry")));
        }

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringFeatureHasGeometryBecauseNullFeatureUri()
            => Assert.ThrowsException<OWLSemanticsException>(() => new GEOOntology("ex:geoOnt").DeclareSecondaryGeometry(null, new RDFResource("ex:MilanGeometry")));

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringFeatureHasGeometryBecauseNullGeometryUri()
            => Assert.ThrowsException<OWLSemanticsException>(() => new GEOOntology("ex:geoOnt").DeclareSecondaryGeometry(new RDFResource("ex:MilanFeature"), null));

        [TestMethod]
        public void ShouldDeclareFeatureHasDefaultGeometry()
        {
            GEOOntology geoOnt = new GEOOntology("ex:geoOnt");
            geoOnt.DeclareDefaultGeometry(new RDFResource("ex:MilanFeature"), new RDFResource("ex:MilanGeometry"));

            //Test evolution of GEO knowledge
            Assert.IsTrue(geoOnt.URI.Equals(geoOnt.URI));
            Assert.IsTrue(geoOnt.Model.ClassModel.ClassesCount == 19);
            Assert.IsTrue(geoOnt.Model.PropertyModel.PropertiesCount == 34);
            Assert.IsTrue(geoOnt.Data.IndividualsCount == 2);
            Assert.IsTrue(geoOnt.Data.CheckHasIndividual(new RDFResource("ex:MilanFeature")));
            Assert.IsTrue(geoOnt.Data.CheckIsIndividualOf(geoOnt.Model, new RDFResource("ex:MilanFeature"), RDFVocabulary.GEOSPARQL.FEATURE));
            Assert.IsTrue(geoOnt.Data.CheckIsIndividualOf(geoOnt.Model, new RDFResource("ex:MilanFeature"), RDFVocabulary.GEOSPARQL.SPATIAL_OBJECT));
            Assert.IsTrue(geoOnt.Data.CheckHasIndividual(new RDFResource("ex:MilanGeometry")));
            Assert.IsTrue(geoOnt.Data.CheckIsIndividualOf(geoOnt.Model, new RDFResource("ex:MilanGeometry"), RDFVocabulary.GEOSPARQL.GEOMETRY));
            Assert.IsTrue(geoOnt.Data.CheckIsIndividualOf(geoOnt.Model, new RDFResource("ex:MilanGeometry"), RDFVocabulary.GEOSPARQL.SPATIAL_OBJECT));
            Assert.IsTrue(geoOnt.Data.CheckHasObjectAssertion(new RDFResource("ex:MilanFeature"), RDFVocabulary.GEOSPARQL.DEFAULT_GEOMETRY, new RDFResource("ex:MilanGeometry")));
        }

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringFeatureHasDefaultGeometryBecauseNullFeatureUri()
            => Assert.ThrowsException<OWLSemanticsException>(() => new GEOOntology("ex:geoOnt").DeclareDefaultGeometry(null, new RDFResource("ex:MilanGeometry")));

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringFeatureHasDefaultGeometryBecauseNullGeometryUri()
            => Assert.ThrowsException<OWLSemanticsException>(() => new GEOOntology("ex:geoOnt").DeclareDefaultGeometry(new RDFResource("ex:MilanFeature"), null));

        //sf:Point
        [TestMethod]
        public void ShouldDeclarePoint()
        {
            GEOOntology geoOnt = new GEOOntology("ex:geoOnt");
            geoOnt.DeclarePoint(new RDFResource("ex:Milan"), 9.188540, 45.464664);
            geoOnt.DeclarePoint(new RDFResource("ex:Rome"), 12.496365, 41.902782);

            //Test evolution of GEO knowledge
            Assert.IsTrue(geoOnt.URI.Equals(geoOnt.URI));
            Assert.IsTrue(geoOnt.Model.ClassModel.ClassesCount == 19);
            Assert.IsTrue(geoOnt.Model.PropertyModel.PropertiesCount == 34);
            Assert.IsTrue(geoOnt.Data.IndividualsCount == 2);
            Assert.IsTrue(geoOnt.Data.CheckHasIndividual(new RDFResource("ex:Milan")));
            Assert.IsTrue(geoOnt.Data.CheckIsIndividualOf(geoOnt.Model, new RDFResource("ex:Milan"), RDFVocabulary.GEOSPARQL.SPATIAL_OBJECT));
            Assert.IsTrue(geoOnt.Data.CheckIsIndividualOf(geoOnt.Model, new RDFResource("ex:Milan"), RDFVocabulary.GEOSPARQL.GEOMETRY));
            Assert.IsTrue(geoOnt.Data.CheckIsIndividualOf(geoOnt.Model, new RDFResource("ex:Milan"), RDFVocabulary.GEOSPARQL.SF.POINT));
            Assert.IsTrue(geoOnt.Data.CheckHasDatatypeAssertion(new RDFResource("ex:Milan"), RDFVocabulary.GEOSPARQL.AS_WKT, new RDFTypedLiteral("POINT (9.18854 45.464664)", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT)));
            Assert.IsTrue(geoOnt.Data.CheckHasDatatypeAssertion(new RDFResource("ex:Milan"), RDFVocabulary.GEOSPARQL.AS_GML, new RDFTypedLiteral("<gml:Point xmlns:gml=\"http://www.opengis.net/gml/3.2\"><gml:pos>9.18854 45.464664</gml:pos></gml:Point>", RDFModelEnums.RDFDatatypes.GEOSPARQL_GML)));
            Assert.IsTrue(geoOnt.Data.CheckIsIndividualOf(geoOnt.Model, new RDFResource("ex:Rome"), RDFVocabulary.GEOSPARQL.SPATIAL_OBJECT));
            Assert.IsTrue(geoOnt.Data.CheckIsIndividualOf(geoOnt.Model, new RDFResource("ex:Rome"), RDFVocabulary.GEOSPARQL.GEOMETRY));
            Assert.IsTrue(geoOnt.Data.CheckIsIndividualOf(geoOnt.Model, new RDFResource("ex:Rome"), RDFVocabulary.GEOSPARQL.SF.POINT));
            Assert.IsTrue(geoOnt.Data.CheckHasDatatypeAssertion(new RDFResource("ex:Rome"), RDFVocabulary.GEOSPARQL.AS_WKT, new RDFTypedLiteral("POINT (12.496365 41.902782)", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT)));
            Assert.IsTrue(geoOnt.Data.CheckHasDatatypeAssertion(new RDFResource("ex:Rome"), RDFVocabulary.GEOSPARQL.AS_GML, new RDFTypedLiteral("<gml:Point xmlns:gml=\"http://www.opengis.net/gml/3.2\"><gml:pos>12.496365 41.902782</gml:pos></gml:Point>", RDFModelEnums.RDFDatatypes.GEOSPARQL_GML)));
        }

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringPointBecauseNullUri()
            => Assert.ThrowsException<OWLSemanticsException>(() => new GEOOntology("ex:geoOnt").DeclarePoint(null, 0, 0));

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringPointBecauseInvalideLatitude()
            => Assert.ThrowsException<OWLSemanticsException>(() => new GEOOntology("ex:geoOnt").DeclarePoint(new RDFResource("ex:Milan"), 0, 91));

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringPointBecauseInvalideLongitude()
            => Assert.ThrowsException<OWLSemanticsException>(() => new GEOOntology("ex:geoOnt").DeclarePoint(new RDFResource("ex:Milan"), 181, 0));

        //sf:LineString
        [TestMethod]
        public void ShouldDeclareLineString()
        {
            GEOOntology geoOnt = new GEOOntology("ex:geoOnt");
            geoOnt.DeclareLineString(new RDFResource("ex:MilanRome"), new List<(double, double)>() {
                (9.188540, 45.464664), (12.496365, 41.902782) });

            //Test evolution of GEO knowledge
            Assert.IsTrue(geoOnt.URI.Equals(geoOnt.URI));
            Assert.IsTrue(geoOnt.Model.ClassModel.ClassesCount == 19);
            Assert.IsTrue(geoOnt.Model.PropertyModel.PropertiesCount == 34);
            Assert.IsTrue(geoOnt.Data.IndividualsCount == 1);
            Assert.IsTrue(geoOnt.Data.CheckHasIndividual(new RDFResource("ex:MilanRome")));
            Assert.IsTrue(geoOnt.Data.CheckIsIndividualOf(geoOnt.Model, new RDFResource("ex:MilanRome"), RDFVocabulary.GEOSPARQL.SPATIAL_OBJECT));
            Assert.IsTrue(geoOnt.Data.CheckIsIndividualOf(geoOnt.Model, new RDFResource("ex:MilanRome"), RDFVocabulary.GEOSPARQL.GEOMETRY));
            Assert.IsTrue(geoOnt.Data.CheckIsIndividualOf(geoOnt.Model, new RDFResource("ex:MilanRome"), RDFVocabulary.GEOSPARQL.SF.LINESTRING));
            Assert.IsTrue(geoOnt.Data.CheckHasDatatypeAssertion(new RDFResource("ex:MilanRome"), RDFVocabulary.GEOSPARQL.AS_WKT, new RDFTypedLiteral("LINESTRING (9.18854 45.464664, 12.496365 41.902782)", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT)));
            Assert.IsTrue(geoOnt.Data.CheckHasDatatypeAssertion(new RDFResource("ex:MilanRome"), RDFVocabulary.GEOSPARQL.AS_GML, new RDFTypedLiteral("<gml:LineString xmlns:gml=\"http://www.opengis.net/gml/3.2\"><gml:posList>9.18854 45.464664 12.496365 41.902782</gml:posList></gml:LineString>", RDFModelEnums.RDFDatatypes.GEOSPARQL_GML)));
        }

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringLineStringBecauseNullUri()
            => Assert.ThrowsException<OWLSemanticsException>(() => new GEOOntology("ex:geoOnt").DeclareLineString(null, new List<(double, double)>() { (0, 0), (1, 1) }));

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringLineStringBecauseNullPoints()
            => Assert.ThrowsException<OWLSemanticsException>(() => new GEOOntology("ex:geoOnt").DeclareLineString(new RDFResource("ex:Milan"), null));

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringLineStringBecauseLessThan2Points()
            => Assert.ThrowsException<OWLSemanticsException>(() => new GEOOntology("ex:geoOnt").DeclareLineString(new RDFResource("ex:Milan"), new List<(double, double)>() { (181, 0) }));

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringLineStringBecauseInvalideLatitude()
            => Assert.ThrowsException<OWLSemanticsException>(() => new GEOOntology("ex:geoOnt").DeclareLineString(new RDFResource("ex:Milan"), new List<(double, double)>() { (0, 91), (1, 1) }));

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringLineStringBecauseInvalideLongitude()
            => Assert.ThrowsException<OWLSemanticsException>(() => new GEOOntology("ex:geoOnt").DeclareLineString(new RDFResource("ex:Milan"), new List<(double, double)>() { (181, 0), (1, 1) }));

        //sf:Polygon
        [TestMethod]
        public void ShouldDeclarePolygon()
        {
            GEOOntology geoOnt = new GEOOntology("ex:geoOnt");
            geoOnt.DeclarePolygon(new RDFResource("ex:MilanRomeNaples"), new List<(double, double)>() {
                (9.188540, 45.464664), (12.496365, 41.902782), (14.2681244, 40.8517746) }); //This will be closed automatically with 4th point being the 1st

            //Test evolution of GEO knowledge
            Assert.IsTrue(geoOnt.URI.Equals(geoOnt.URI));
            Assert.IsTrue(geoOnt.Model.ClassModel.ClassesCount == 19);
            Assert.IsTrue(geoOnt.Model.PropertyModel.PropertiesCount == 34);
            Assert.IsTrue(geoOnt.Data.IndividualsCount == 1);
            Assert.IsTrue(geoOnt.Data.CheckHasIndividual(new RDFResource("ex:MilanRomeNaples")));
            Assert.IsTrue(geoOnt.Data.CheckIsIndividualOf(geoOnt.Model, new RDFResource("ex:MilanRomeNaples"), RDFVocabulary.GEOSPARQL.SPATIAL_OBJECT));
            Assert.IsTrue(geoOnt.Data.CheckIsIndividualOf(geoOnt.Model, new RDFResource("ex:MilanRomeNaples"), RDFVocabulary.GEOSPARQL.GEOMETRY));
            Assert.IsTrue(geoOnt.Data.CheckIsIndividualOf(geoOnt.Model, new RDFResource("ex:MilanRomeNaples"), RDFVocabulary.GEOSPARQL.SF.POLYGON));
            Assert.IsTrue(geoOnt.Data.CheckHasDatatypeAssertion(new RDFResource("ex:MilanRomeNaples"), RDFVocabulary.GEOSPARQL.AS_WKT, new RDFTypedLiteral("POLYGON ((9.18854 45.464664, 12.496365 41.902782, 14.2681244 40.8517746, 9.18854 45.464664))", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT)));
            Assert.IsTrue(geoOnt.Data.CheckHasDatatypeAssertion(new RDFResource("ex:MilanRomeNaples"), RDFVocabulary.GEOSPARQL.AS_GML, new RDFTypedLiteral("<gml:Polygon xmlns:gml=\"http://www.opengis.net/gml/3.2\"><gml:exterior><gml:LinearRing><gml:posList>9.18854 45.464664 12.496365 41.902782 14.2681244 40.8517746 9.18854 45.464664</gml:posList></gml:LinearRing></gml:exterior></gml:Polygon>", RDFModelEnums.RDFDatatypes.GEOSPARQL_GML)));
        }

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringPolygonBecauseNullUri()
            => Assert.ThrowsException<OWLSemanticsException>(() => new GEOOntology("ex:geoOnt").DeclarePolygon(null, new List<(double, double)>() { (0, 0), (1, 1), (2, 2) }));

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringPolygonBecauseNullPoints()
            => Assert.ThrowsException<OWLSemanticsException>(() => new GEOOntology("ex:geoOnt").DeclarePolygon(new RDFResource("ex:MilanRomeNaples"), null));

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringPolygonBecauseLessThan3Points()
            => Assert.ThrowsException<OWLSemanticsException>(() => new GEOOntology("ex:geoOnt").DeclarePolygon(new RDFResource("ex:MilanRomeNaples"), new List<(double, double)>() { (181, 0), (1, 1) }));

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringPolygonBecauseInvalideLatitude()
            => Assert.ThrowsException<OWLSemanticsException>(() => new GEOOntology("ex:geoOnt").DeclarePolygon(new RDFResource("ex:MilanRomeNaples"), new List<(double, double)>() { (0, 91), (1, 1), (2, 2) }));

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringPolygonBecauseInvalideLongitude()
            => Assert.ThrowsException<OWLSemanticsException>(() => new GEOOntology("ex:geoOnt").DeclarePolygon(new RDFResource("ex:MilanRomeNaples"), new List<(double, double)>() { (181, 0), (1, 1), (2, 2) }));

        //sf:MultiPoint
        [TestMethod]
        public void ShouldDeclareMultiPoint()
        {
            GEOOntology geoOnt = new GEOOntology("ex:geoOnt");
            geoOnt.DeclareMultiPoint(new RDFResource("ex:MilanRome"), new List<(double, double)>() {
                (9.188540, 45.464664), (12.496365, 41.902782) });

            //Test evolution of GEO knowledge
            Assert.IsTrue(geoOnt.URI.Equals(geoOnt.URI));
            Assert.IsTrue(geoOnt.Model.ClassModel.ClassesCount == 19);
            Assert.IsTrue(geoOnt.Model.PropertyModel.PropertiesCount == 34);
            Assert.IsTrue(geoOnt.Data.IndividualsCount == 1);
            Assert.IsTrue(geoOnt.Data.CheckHasIndividual(new RDFResource("ex:MilanRome")));
            Assert.IsTrue(geoOnt.Data.CheckIsIndividualOf(geoOnt.Model, new RDFResource("ex:MilanRome"), RDFVocabulary.GEOSPARQL.SPATIAL_OBJECT));
            Assert.IsTrue(geoOnt.Data.CheckIsIndividualOf(geoOnt.Model, new RDFResource("ex:MilanRome"), RDFVocabulary.GEOSPARQL.GEOMETRY));
            Assert.IsTrue(geoOnt.Data.CheckIsIndividualOf(geoOnt.Model, new RDFResource("ex:MilanRome"), RDFVocabulary.GEOSPARQL.SF.MULTI_POINT));
            Assert.IsTrue(geoOnt.Data.CheckHasDatatypeAssertion(new RDFResource("ex:MilanRome"), RDFVocabulary.GEOSPARQL.AS_WKT, new RDFTypedLiteral("MULTIPOINT ((9.18854 45.464664), (12.496365 41.902782))", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT)));
            Assert.IsTrue(geoOnt.Data.CheckHasDatatypeAssertion(new RDFResource("ex:MilanRome"), RDFVocabulary.GEOSPARQL.AS_GML, new RDFTypedLiteral("<gml:MultiPoint xmlns:gml=\"http://www.opengis.net/gml/3.2\"><gml:pointMember><gml:Point><gml:pos>9.18854 45.464664</gml:pos></gml:Point></gml:pointMember><gml:pointMember><gml:Point><gml:pos>12.496365 41.902782</gml:pos></gml:Point></gml:pointMember></gml:MultiPoint>", RDFModelEnums.RDFDatatypes.GEOSPARQL_GML)));
        }

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringMultiPointBecauseNullUri()
            => Assert.ThrowsException<OWLSemanticsException>(() => new GEOOntology("ex:geoOnt").DeclareMultiPoint(null, new List<(double, double)>() { (0, 0), (1, 1) }));

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringMultiPointBecauseNullPoints()
            => Assert.ThrowsException<OWLSemanticsException>(() => new GEOOntology("ex:geoOnt").DeclareMultiPoint(new RDFResource("ex:Milan"), null));

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringMultiPointBecauseLessThan2Points()
            => Assert.ThrowsException<OWLSemanticsException>(() => new GEOOntology("ex:geoOnt").DeclareMultiPoint(new RDFResource("ex:Milan"), new List<(double, double)>() { (181, 0) }));

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringMultiPointBecauseInvalideLatitude()
            => Assert.ThrowsException<OWLSemanticsException>(() => new GEOOntology("ex:geoOnt").DeclareMultiPoint(new RDFResource("ex:Milan"), new List<(double, double)>() { (0, 91), (1, 1) }));

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringMultiPointBecauseInvalideLongitude()
            => Assert.ThrowsException<OWLSemanticsException>(() => new GEOOntology("ex:geoOnt").DeclareMultiPoint(new RDFResource("ex:Milan"), new List<(double, double)>() { (181, 0), (1, 1) }));

        //sf:MultiLineString
        [TestMethod]
        public void ShouldDeclareMultiLineString()
        {
            GEOOntology geoOnt = new GEOOntology("ex:geoOnt");
            geoOnt.DeclareMultiLineString(new RDFResource("ex:MilanRomeAndRomeNaples"), new List<List<(double, double)>>() {
                new List<(double, double)>() { (9.188540, 45.464664), (12.496365, 41.902782) },
                new List<(double, double)>() { (12.496365, 41.902782), (14.2681244, 40.8517746) } });

            //Test evolution of GEO knowledge
            Assert.IsTrue(geoOnt.URI.Equals(geoOnt.URI));
            Assert.IsTrue(geoOnt.Model.ClassModel.ClassesCount == 19);
            Assert.IsTrue(geoOnt.Model.PropertyModel.PropertiesCount == 34);
            Assert.IsTrue(geoOnt.Data.IndividualsCount == 1);
            Assert.IsTrue(geoOnt.Data.CheckHasIndividual(new RDFResource("ex:MilanRomeAndRomeNaples")));
            Assert.IsTrue(geoOnt.Data.CheckIsIndividualOf(geoOnt.Model, new RDFResource("ex:MilanRomeAndRomeNaples"), RDFVocabulary.GEOSPARQL.SPATIAL_OBJECT));
            Assert.IsTrue(geoOnt.Data.CheckIsIndividualOf(geoOnt.Model, new RDFResource("ex:MilanRomeAndRomeNaples"), RDFVocabulary.GEOSPARQL.GEOMETRY));
            Assert.IsTrue(geoOnt.Data.CheckIsIndividualOf(geoOnt.Model, new RDFResource("ex:MilanRomeAndRomeNaples"), RDFVocabulary.GEOSPARQL.SF.MULTI_LINESTRING));
            Assert.IsTrue(geoOnt.Data.CheckHasDatatypeAssertion(new RDFResource("ex:MilanRomeAndRomeNaples"), RDFVocabulary.GEOSPARQL.AS_WKT, new RDFTypedLiteral("MULTILINESTRING ((9.18854 45.464664, 12.496365 41.902782), (12.496365 41.902782, 14.2681244 40.8517746))", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT)));
            Assert.IsTrue(geoOnt.Data.CheckHasDatatypeAssertion(new RDFResource("ex:MilanRomeAndRomeNaples"), RDFVocabulary.GEOSPARQL.AS_GML, new RDFTypedLiteral("<gml:MultiCurve xmlns:gml=\"http://www.opengis.net/gml/3.2\"><gml:curveMember><gml:LineString><gml:posList>9.18854 45.464664 12.496365 41.902782</gml:posList></gml:LineString></gml:curveMember><gml:curveMember><gml:LineString><gml:posList>12.496365 41.902782 14.2681244 40.8517746</gml:posList></gml:LineString></gml:curveMember></gml:MultiCurve>", RDFModelEnums.RDFDatatypes.GEOSPARQL_GML)));
        }

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringMultiLineStringBecauseNullUri()
            => Assert.ThrowsException<OWLSemanticsException>(() => new GEOOntology("ex:geoOnt").DeclareMultiLineString(null, new List<List<(double, double)>>() {
                new List<(double,double)>() { (0, 0), (1, 1) }, new List<(double,double)>() { (1, 1), (2, 2) } }));

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringMultiLineStringBecauseNullLineStrings()
            => Assert.ThrowsException<OWLSemanticsException>(() => new GEOOntology("ex:geoOnt").DeclareMultiLineString(new RDFResource("ex:MLS"), null));

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringMultiLineStringBecauseLessThan2LineStrings()
            => Assert.ThrowsException<OWLSemanticsException>(() => new GEOOntology("ex:geoOnt").DeclareMultiLineString(new RDFResource("ex:MLS"), new List<List<(double, double)>>() {
                new List<(double,double)>() { (0, 0), (1, 1) } }));

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringMultiLineStringBecauseHavingNullLineString()
            => Assert.ThrowsException<OWLSemanticsException>(() => new GEOOntology("ex:geoOnt").DeclareMultiLineString(new RDFResource("ex:MLS"), new List<List<(double, double)>>() {
                null, new List<(double,double)>() { (1, 1), (2, 2) } }));

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringMultiLineStringBecauseHavingLineStringWithLessThan2Points()
            => Assert.ThrowsException<OWLSemanticsException>(() => new GEOOntology("ex:geoOnt").DeclareMultiLineString(null, new List<List<(double, double)>>() {
                new List<(double,double)>() { (0, 0) }, new List<(double,double)>() { (1, 1), (2, 2) } }));

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringMultiLineStringBecauseInvalideLatitude()
            => Assert.ThrowsException<OWLSemanticsException>(() => new GEOOntology("ex:geoOnt").DeclareMultiLineString(new RDFResource("ex:MLS"), new List<List<(double, double)>>() {
                new List<(double,double)>() { (0, 91), (1, 1) }, new List<(double,double)>() { (1, 1), (2, 2) } }));

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringMultiLineStringBecauseInvalideLongitude()
            => Assert.ThrowsException<OWLSemanticsException>(() => new GEOOntology("ex:geoOnt").DeclareMultiLineString(new RDFResource("ex:MLS"), new List<List<(double, double)>>() {
                new List<(double,double)>() { (181, 0), (1, 1) }, new List<(double,double)>() { (1, 1), (2, 2) } }));

        //sf:MultiPolygon
        [TestMethod]
        public void ShouldDeclareMultiPolygon()
        {
            GEOOntology geoOnt = new GEOOntology("ex:geoOnt");
            geoOnt.DeclareMultiPolygon(new RDFResource("ex:MilanRomeNaples"), new List<List<(double, double)>>() {
                new List<(double, double)>() { (9.188540, 45.464664), (12.496365, 41.902782), (14.2681244, 40.8517746) }, //These polygons will be automatically closed
                new List<(double, double)>() { (12.496365, 41.902782), (14.2681244, 40.8517746), (9.188540, 45.464664) } });

            //Test evolution of GEO knowledge
            Assert.IsTrue(geoOnt.URI.Equals(geoOnt.URI));
            Assert.IsTrue(geoOnt.Model.ClassModel.ClassesCount == 19);
            Assert.IsTrue(geoOnt.Model.PropertyModel.PropertiesCount == 34);
            Assert.IsTrue(geoOnt.Data.IndividualsCount == 1);
            Assert.IsTrue(geoOnt.Data.CheckHasIndividual(new RDFResource("ex:MilanRomeNaples")));
            Assert.IsTrue(geoOnt.Data.CheckIsIndividualOf(geoOnt.Model, new RDFResource("ex:MilanRomeNaples"), RDFVocabulary.GEOSPARQL.SPATIAL_OBJECT));
            Assert.IsTrue(geoOnt.Data.CheckIsIndividualOf(geoOnt.Model, new RDFResource("ex:MilanRomeNaples"), RDFVocabulary.GEOSPARQL.GEOMETRY));
            Assert.IsTrue(geoOnt.Data.CheckIsIndividualOf(geoOnt.Model, new RDFResource("ex:MilanRomeNaples"), RDFVocabulary.GEOSPARQL.SF.MULTI_POLYGON));
            Assert.IsTrue(geoOnt.Data.CheckHasDatatypeAssertion(new RDFResource("ex:MilanRomeNaples"), RDFVocabulary.GEOSPARQL.AS_WKT, new RDFTypedLiteral("MULTIPOLYGON (((9.18854 45.464664, 12.496365 41.902782, 14.2681244 40.8517746, 9.18854 45.464664)), ((12.496365 41.902782, 14.2681244 40.8517746, 9.18854 45.464664, 12.496365 41.902782)))", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT)));
            Assert.IsTrue(geoOnt.Data.CheckHasDatatypeAssertion(new RDFResource("ex:MilanRomeNaples"), RDFVocabulary.GEOSPARQL.AS_GML, new RDFTypedLiteral("<gml:MultiSurface xmlns:gml=\"http://www.opengis.net/gml/3.2\"><gml:surfaceMember><gml:Polygon><gml:exterior><gml:LinearRing><gml:posList>9.18854 45.464664 12.496365 41.902782 14.2681244 40.8517746 9.18854 45.464664</gml:posList></gml:LinearRing></gml:exterior></gml:Polygon></gml:surfaceMember><gml:surfaceMember><gml:Polygon><gml:exterior><gml:LinearRing><gml:posList>12.496365 41.902782 14.2681244 40.8517746 9.18854 45.464664 12.496365 41.902782</gml:posList></gml:LinearRing></gml:exterior></gml:Polygon></gml:surfaceMember></gml:MultiSurface>", RDFModelEnums.RDFDatatypes.GEOSPARQL_GML)));
        }

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringMultiPolygonBecauseNullUri()
            => Assert.ThrowsException<OWLSemanticsException>(() => new GEOOntology("ex:geoOnt").DeclareMultiPolygon(null, new List<List<(double, double)>>() {
                new List<(double,double)>() { (0, 0), (1, 1), (2, 2) }, new List<(double,double)>() { (1, 1), (2, 2), (3, 3) } }));

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringMultiPolygonBecauseNullPolygons()
            => Assert.ThrowsException<OWLSemanticsException>(() => new GEOOntology("ex:geoOnt").DeclareMultiPolygon(new RDFResource("ex:MPL"), null));

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringMultiPolygonBecauseLessThan2Polygons()
            => Assert.ThrowsException<OWLSemanticsException>(() => new GEOOntology("ex:geoOnt").DeclareMultiPolygon(new RDFResource("ex:MPL"), new List<List<(double, double)>>() {
                new List<(double,double)>() { (0, 0), (1, 1), (2, 2) } }));

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringMultiPolygonBecauseHavingNullPolygon()
            => Assert.ThrowsException<OWLSemanticsException>(() => new GEOOntology("ex:geoOnt").DeclareMultiPolygon(new RDFResource("ex:MPL"), new List<List<(double, double)>>() {
                null, new List<(double,double)>() { (1, 1), (2, 2), (3, 3) } }));

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringMultiPolygonBecauseHavingPolygonWithLessThan3Points()
            => Assert.ThrowsException<OWLSemanticsException>(() => new GEOOntology("ex:geoOnt").DeclareMultiPolygon(new RDFResource("ex:MPL"), new List<List<(double, double)>>() {
                new List<(double,double)>() { (0, 0), (1, 1) }, new List<(double,double)>() { (1, 1), (2, 2), (3, 3) } }));

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringMultiPolygonBecauseInvalideLatitude()
            => Assert.ThrowsException<OWLSemanticsException>(() => new GEOOntology("ex:geoOnt").DeclareMultiPolygon(new RDFResource("ex:MPL"), new List<List<(double, double)>>() {
                new List<(double,double)>() { (0, 91), (1, 1), (2, 2) }, new List<(double,double)>() { (1, 1), (2, 2), (3, 3) } }));

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringMultiPolygonBecauseInvalideLongitude()
            => Assert.ThrowsException<OWLSemanticsException>(() => new GEOOntology("ex:geoOnt").DeclareMultiPolygon(new RDFResource("ex:MPL"), new List<List<(double, double)>>() {
                new List<(double,double)>() { (181, 0), (1, 1), (2, 2) }, new List<(double,double)>() { (1, 1), (2, 2), (2, 2) } }));

        //sf:GeometryCollection
        [TestMethod]
        public void ShouldDeclareGeometryCollection()
        {
            GEOOntology geoOnt = new GEOOntology("ex:geoOnt");
            geoOnt.DeclareGeometryCollection(new RDFResource("ex:GC"),
                new List<(double, double)>() {
                    { (9.188540, 45.464664) },
                    { (12.496365, 41.902782) },
                    { (14.2681244, 40.8517746) }
                },
                new List<List<(double, double)>>() {
                    new List<(double, double)>() { (9.188540, 45.464664), (12.496365, 41.902782) },
                    new List<(double, double)>() { (14.2681244, 40.8517746), (9.188540, 45.464664) }
                },
                new List<List<(double, double)>>() {
                    new List<(double, double)>() { (9.188540, 45.464664), (12.496365, 41.902782), (14.2681244, 40.8517746) }, //These polygons will be automatically closed
					new List<(double, double)>() { (12.496365, 41.902782), (14.2681244, 40.8517746), (9.188540, 45.464664) }
                });

            //Test evolution of GEO knowledge
            Assert.IsTrue(geoOnt.URI.Equals(geoOnt.URI));
            Assert.IsTrue(geoOnt.Model.ClassModel.ClassesCount == 19);
            Assert.IsTrue(geoOnt.Model.PropertyModel.PropertiesCount == 34);
            Assert.IsTrue(geoOnt.Data.IndividualsCount == 1);
            Assert.IsTrue(geoOnt.Data.CheckHasIndividual(new RDFResource("ex:GC")));
            Assert.IsTrue(geoOnt.Data.CheckIsIndividualOf(geoOnt.Model, new RDFResource("ex:GC"), RDFVocabulary.GEOSPARQL.SPATIAL_OBJECT));
            Assert.IsTrue(geoOnt.Data.CheckIsIndividualOf(geoOnt.Model, new RDFResource("ex:GC"), RDFVocabulary.GEOSPARQL.GEOMETRY));
            Assert.IsTrue(geoOnt.Data.CheckIsIndividualOf(geoOnt.Model, new RDFResource("ex:GC"), RDFVocabulary.GEOSPARQL.SF.GEOMETRY_COLLECTION));
            Assert.IsTrue(geoOnt.Data.CheckHasDatatypeAssertion(new RDFResource("ex:GC"), RDFVocabulary.GEOSPARQL.AS_WKT, new RDFTypedLiteral("GEOMETRYCOLLECTION (POINT (9.18854 45.464664), POINT (12.496365 41.902782), POINT (14.2681244 40.8517746), LINESTRING (9.18854 45.464664, 12.496365 41.902782), LINESTRING (14.2681244 40.8517746, 9.18854 45.464664), POLYGON ((9.18854 45.464664, 12.496365 41.902782, 14.2681244 40.8517746, 9.18854 45.464664)), POLYGON ((12.496365 41.902782, 14.2681244 40.8517746, 9.18854 45.464664, 12.496365 41.902782)))", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT)));
            Assert.IsTrue(geoOnt.Data.CheckHasDatatypeAssertion(new RDFResource("ex:GC"), RDFVocabulary.GEOSPARQL.AS_GML, new RDFTypedLiteral("<gml:MultiGeometry xmlns:gml=\"http://www.opengis.net/gml/3.2\"><gml:geometryMember><gml:Point><gml:pos>9.18854 45.464664</gml:pos></gml:Point></gml:geometryMember><gml:geometryMember><gml:Point><gml:pos>12.496365 41.902782</gml:pos></gml:Point></gml:geometryMember><gml:geometryMember><gml:Point><gml:pos>14.2681244 40.8517746</gml:pos></gml:Point></gml:geometryMember><gml:geometryMember><gml:LineString><gml:posList>9.18854 45.464664 12.496365 41.902782</gml:posList></gml:LineString></gml:geometryMember><gml:geometryMember><gml:LineString><gml:posList>14.2681244 40.8517746 9.18854 45.464664</gml:posList></gml:LineString></gml:geometryMember><gml:geometryMember><gml:Polygon><gml:exterior><gml:LinearRing><gml:posList>9.18854 45.464664 12.496365 41.902782 14.2681244 40.8517746 9.18854 45.464664</gml:posList></gml:LinearRing></gml:exterior></gml:Polygon></gml:geometryMember><gml:geometryMember><gml:Polygon><gml:exterior><gml:LinearRing><gml:posList>12.496365 41.902782 14.2681244 40.8517746 9.18854 45.464664 12.496365 41.902782</gml:posList></gml:LinearRing></gml:exterior></gml:Polygon></gml:geometryMember></gml:MultiGeometry>", RDFModelEnums.RDFDatatypes.GEOSPARQL_GML)));
        }

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringGeometryCollectionBecauseNullUri()
            => Assert.ThrowsException<OWLSemanticsException>(() => new GEOOntology("ex:geoOnt").DeclareGeometryCollection(null,
                new List<(double, double)>() {
                    { (0, 0) }, { (1, 1) }, { (2, 2) }
                },
                new List<List<(double, double)>>() {
                    new List<(double,double)>() { (0, 0), (1, 1) }, new List<(double,double)>() { (1, 1), (2, 2) }
                },
                new List<List<(double, double)>>() {
                    new List<(double,double)>() { (0, 0), (1, 1), (2, 2) }, new List<(double,double)>() { (1, 1), (2, 2), (3, 3) }
                }));

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringGeometryCollectionBecauseHavingPointWithInvalidLatitude()
            => Assert.ThrowsException<OWLSemanticsException>(() => new GEOOntology("ex:geoOnt").DeclareGeometryCollection(new RDFResource("ex:GC"),
               new List<(double, double)>() {
                    { (0, 91) }, { (1, 1) }, { (2, 2) }
                },
                new List<List<(double, double)>>() {
                    new List<(double,double)>() { (0, 0), (1, 1) }, new List<(double,double)>() { (1, 1), (2, 2) }
                },
                new List<List<(double, double)>>() {
                    new List<(double,double)>() { (0, 0), (1, 1), (2, 2) }, new List<(double,double)>() { (1, 1), (2, 2), (3, 3) }
                }));

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringGeometryCollectionBecauseHavingPointWithInvalidLongitude()
            => Assert.ThrowsException<OWLSemanticsException>(() => new GEOOntology("ex:geoOnt").DeclareGeometryCollection(new RDFResource("ex:GC"),
                new List<(double, double)>() {
                    { (181, 0) }, { (1, 1) }, { (2, 2) }
                },
                new List<List<(double, double)>>() {
                    new List<(double,double)>() { (0, 0), (1, 1) }, new List<(double,double)>() { (1, 1), (2, 2) }
                },
                new List<List<(double, double)>>() {
                    new List<(double,double)>() { (0, 0), (1, 1), (2, 2) }, new List<(double,double)>() { (1, 1), (2, 2), (3, 3) }
                }));

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringGeometryCollectionBecauseHavingNullLineString()
            => Assert.ThrowsException<OWLSemanticsException>(() => new GEOOntology("ex:geoOnt").DeclareGeometryCollection(new RDFResource("ex:GC"),
                new List<(double, double)>() {
                    { (0, 0) }, { (1, 1) }, { (2, 2) }
                },
                new List<List<(double, double)>>() {
                    null, new List<(double,double)>() { (1, 1), (2, 2) }
                },
                new List<List<(double, double)>>() {
                    new List<(double,double)>() { (0, 0), (1, 1), (2, 2) }, new List<(double,double)>() { (1, 1), (2, 2), (3, 3) }
                }));

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringGeometryCollectionBecauseHavingLineStringWithLessThan2Points()
            => Assert.ThrowsException<OWLSemanticsException>(() => new GEOOntology("ex:geoOnt").DeclareGeometryCollection(new RDFResource("ex:GC"),
                new List<(double, double)>() {
                    { (0, 0) }, { (1, 1) }, { (2, 2) }
                },
                new List<List<(double, double)>>() {
                    new List<(double,double)>() { (0, 0) }, new List<(double,double)>() { (1, 1), (2, 2) }
                },
                new List<List<(double, double)>>() {
                    new List<(double,double)>() { (0, 0), (1, 1), (2, 2) }, new List<(double,double)>() { (1, 1), (2, 2), (3, 3) }
                }));

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringGeometryCollectionBecauseHavingLineStringWithInvalidLatitude()
            => Assert.ThrowsException<OWLSemanticsException>(() => new GEOOntology("ex:geoOnt").DeclareGeometryCollection(new RDFResource("ex:GC"),
                new List<(double, double)>() {
                    { (0, 0) }, { (1, 1) }, { (2, 2) }
                },
                new List<List<(double, double)>>() {
                    new List<(double,double)>() { (0, 91), (1, 1) }, new List<(double,double)>() { (1, 1), (2, 2) }
                },
                new List<List<(double, double)>>() {
                    new List<(double,double)>() { (0, 0), (1, 1), (2, 2) }, new List<(double,double)>() { (1, 1), (2, 2), (3, 3) }
                }));

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringGeometryCollectionBecauseHavingLineStringWithInvalidLongitude()
            => Assert.ThrowsException<OWLSemanticsException>(() => new GEOOntology("ex:geoOnt").DeclareGeometryCollection(new RDFResource("ex:GC"),
                new List<(double, double)>() {
                    { (0, 0) }, { (1, 1) }, { (2, 2) }
                },
                new List<List<(double, double)>>() {
                    new List<(double,double)>() { (181, 0), (1, 1) }, new List<(double,double)>() { (1, 1), (2, 2) }
                },
                new List<List<(double, double)>>() {
                    new List<(double,double)>() { (0, 0), (1, 1), (2, 2) }, new List<(double,double)>() { (1, 1), (2, 2), (3, 3) }
                }));

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringGeometryCollectionBecauseHavingNullPolygon()
            => Assert.ThrowsException<OWLSemanticsException>(() => new GEOOntology("ex:geoOnt").DeclareGeometryCollection(new RDFResource("ex:GC"),
                new List<(double, double)>() {
                    { (0, 0) }, { (1, 1) }, { (2, 2) }
                },
                new List<List<(double, double)>>() {
                    new List<(double,double)>() { (0, 0), (1, 1) }, new List<(double,double)>() { (1, 1), (2, 2) }
                },
                new List<List<(double, double)>>() {
                    null, new List<(double,double)>() { (1, 1), (2, 2), (3, 3) }
                }));

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringGeometryCollectionBecauseHavingPolygonWithLessThan3Points()
            => Assert.ThrowsException<OWLSemanticsException>(() => new GEOOntology("ex:geoOnt").DeclareGeometryCollection(new RDFResource("ex:GC"),
                new List<(double, double)>() {
                    { (0, 0) }, { (1, 1) }, { (2, 2) }
                },
                new List<List<(double, double)>>() {
                    new List<(double,double)>() { (0, 0), (1, 1) }, new List<(double,double)>() { (1, 1), (2, 2) }
                },
                new List<List<(double, double)>>() {
                    new List<(double,double)>() { (0, 0), (1, 1) }, new List<(double,double)>() { (1, 1), (2, 2), (3, 3) }
                }));

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringGeometryCollectionBecauseHavingPolygonWithInvalidLatitude()
            => Assert.ThrowsException<OWLSemanticsException>(() => new GEOOntology("ex:geoOnt").DeclareGeometryCollection(new RDFResource("ex:GC"),
                new List<(double, double)>() {
                    { (0, 0) }, { (1, 1) }, { (2, 2) }
                },
                new List<List<(double, double)>>() {
                    new List<(double,double)>() { (0, 0), (1, 1) }, new List<(double,double)>() { (1, 1), (2, 2) }
                },
                new List<List<(double, double)>>() {
                    new List<(double,double)>() { (0, 91), (1, 1), (2, 2) }, new List<(double,double)>() { (1, 1), (2, 2), (3, 3) }
                }));

        [TestMethod]
        public void ShouldThrowExceptionOnDeclaringGeometryCollectionBecauseHavingPolygonWithInvalidLongitude()
            => Assert.ThrowsException<OWLSemanticsException>(() => new GEOOntology("ex:geoOnt").DeclareGeometryCollection(new RDFResource("ex:GC"),
                new List<(double, double)>() {
                    { (0, 0) }, { (1, 1) }, { (2, 2) }
                },
                new List<List<(double, double)>>() {
                    new List<(double,double)>() { (0, 0), (1, 1) }, new List<(double,double)>() { (1, 1), (2, 2) }
                },
                new List<List<(double, double)>>() {
                    new List<(double,double)>() { (181, 0), (1, 1), (2, 2) }, new List<(double,double)>() { (1, 1), (2, 2), (3, 3) }
                }));
        #endregion

        #region Tests (Analyzer)
        [TestMethod]
        public void ShouldGetDefaultGeometryFromWKT()
        {
            GEOOntology geoOntology = new GEOOntology("ex:geoOnt");
            geoOntology.DeclareDefaultGeometry(new RDFResource("ex:milanFeat"), new RDFResource("ex:milanGeom"));
            geoOntology.DeclarePoint(new RDFResource("ex:milanGeom"), 9.188540, 45.464664);
            (Geometry, Geometry) milanDefaultGeometry = geoOntology.GetDefaultGeometryOfFeature(new RDFResource("ex:milanFeat"));

            Assert.IsTrue(milanDefaultGeometry.Item1.SRID == 4326  && milanDefaultGeometry.Item1.EqualsTopologically(new Point(9.188540, 45.464664)));
            Assert.IsTrue(milanDefaultGeometry.Item2.SRID == 32632 && milanDefaultGeometry.Item2.EqualsTopologically(new Point(514739.23764243, 5034588.07621425)));
        }

        [TestMethod]
        public void ShouldGetDefaultGeometryFromGML()
        {
            GEOOntology geoOntology = new GEOOntology("ex:geoOnt");
            geoOntology.DeclareDefaultGeometry(new RDFResource("ex:milanFeat"), new RDFResource("ex:milanGeom"));
            geoOntology.DeclarePoint(new RDFResource("ex:milanGeom"), 9.188540, 45.464664);
            geoOntology.Data.ABoxGraph.RemoveTriplesByPredicate(RDFVocabulary.GEOSPARQL.AS_WKT);
            (Geometry, Geometry) milanDefaultGeometry = geoOntology.GetDefaultGeometryOfFeature(new RDFResource("ex:milanFeat"));

            Assert.IsTrue(milanDefaultGeometry.Item1.SRID == 4326 && milanDefaultGeometry.Item1.EqualsTopologically(new Point(9.188540, 45.464664)));
            Assert.IsTrue(milanDefaultGeometry.Item2.SRID == 32632 && milanDefaultGeometry.Item2.EqualsTopologically(new Point(514739.23764243, 5034588.07621425)));
        }

        [TestMethod]
        public void ShouldNotGetDefaultGeometry()
        {
            GEOOntology geoOntology = new GEOOntology("ex:geoOnt");
            geoOntology.DeclareDefaultGeometry(new RDFResource("ex:milanFeat"), new RDFResource("ex:milanGeom"));
            (Geometry, Geometry) milanDefaultGeometry = geoOntology.GetDefaultGeometryOfFeature(new RDFResource("ex:milanFeat"));

            Assert.IsNull(milanDefaultGeometry.Item1);
            Assert.IsNull(milanDefaultGeometry.Item2);
        }

        [TestMethod]
        public void ShouldGetSecondaryGeometriesFromWKT()
        {
            GEOOntology geoOntology = new GEOOntology("ex:geoOnt");
            geoOntology.DeclareSecondaryGeometry(new RDFResource("ex:milanFeat"), new RDFResource("ex:milanGeomA"));
            geoOntology.DeclarePoint(new RDFResource("ex:milanGeomA"), 9.188540, 45.464664);
            geoOntology.DeclareSecondaryGeometry(new RDFResource("ex:milanFeat"), new RDFResource("ex:milanGeomB"));
            geoOntology.DeclarePoint(new RDFResource("ex:milanGeomB"), 9.588540, 45.864664);
            List<(Geometry, Geometry)> milanSecondaryGeometries = geoOntology.GetSecondaryGeometriesOfFeature(new RDFResource("ex:milanFeat"));

            Assert.IsNotNull(milanSecondaryGeometries);
            Assert.IsTrue(milanSecondaryGeometries.Count == 2);
            Assert.IsTrue(milanSecondaryGeometries[0].Item1.SRID == 4326  && milanSecondaryGeometries[0].Item1.EqualsTopologically(new Point(9.188540, 45.464664)));
            Assert.IsTrue(milanSecondaryGeometries[0].Item2.SRID == 32632 && milanSecondaryGeometries[0].Item2.EqualsTopologically(new Point(514739.23764243, 5034588.07621425)));
            Assert.IsTrue(milanSecondaryGeometries[1].Item1.SRID == 4326  && milanSecondaryGeometries[1].Item1.EqualsTopologically(new Point(9.588540, 45.864664)));
            Assert.IsTrue(milanSecondaryGeometries[1].Item2.SRID == 32632 && milanSecondaryGeometries[1].Item2.EqualsTopologically(new Point(545682.9698859, 5079179.30709953)));
        }

        [TestMethod]
        public void ShouldGetSecondaryGeometriesFromGML()
        {
            GEOOntology geoOntology = new GEOOntology("ex:geoOnt");
            geoOntology.DeclareSecondaryGeometry(new RDFResource("ex:milanFeat"), new RDFResource("ex:milanGeomA"));
            geoOntology.DeclarePoint(new RDFResource("ex:milanGeomA"), 9.188540, 45.464664);
            geoOntology.DeclareSecondaryGeometry(new RDFResource("ex:milanFeat"), new RDFResource("ex:milanGeomB"));
            geoOntology.DeclarePoint(new RDFResource("ex:milanGeomB"), 9.588540, 45.864664);
            geoOntology.Data.ABoxGraph.RemoveTriplesByPredicate(RDFVocabulary.GEOSPARQL.AS_WKT);
            List<(Geometry, Geometry)> milanSecondaryGeometries = geoOntology.GetSecondaryGeometriesOfFeature(new RDFResource("ex:milanFeat"));

            Assert.IsNotNull(milanSecondaryGeometries);
            Assert.IsTrue(milanSecondaryGeometries.Count == 2);
            Assert.IsTrue(milanSecondaryGeometries[0].Item1.SRID == 4326 && milanSecondaryGeometries[0].Item1.EqualsTopologically(new Point(9.188540, 45.464664)));
            Assert.IsTrue(milanSecondaryGeometries[0].Item2.SRID == 32632 && milanSecondaryGeometries[0].Item2.EqualsTopologically(new Point(514739.23764243, 5034588.07621425)));
            Assert.IsTrue(milanSecondaryGeometries[1].Item1.SRID == 4326 && milanSecondaryGeometries[1].Item1.EqualsTopologically(new Point(9.588540, 45.864664)));
            Assert.IsTrue(milanSecondaryGeometries[1].Item2.SRID == 32632 && milanSecondaryGeometries[1].Item2.EqualsTopologically(new Point(545682.9698859, 5079179.30709953)));
        }

        [TestMethod]
        public void ShouldNotGetSecondaryGeometries()
        {
            GEOOntology geoOntology = new GEOOntology("ex:geoOnt");
            geoOntology.DeclareSecondaryGeometry(new RDFResource("ex:milanFeat"), new RDFResource("ex:milanGeom"));
            List<(Geometry, Geometry)> milanSecondaryGeometries = geoOntology.GetSecondaryGeometriesOfFeature(new RDFResource("ex:milanFeat"));

            Assert.IsNotNull(milanSecondaryGeometries);
            Assert.IsTrue(milanSecondaryGeometries.Count == 0);
        }

        //SPATIAL ANALYSIS

        [TestMethod]
        public void ShouldGetDistanceBetweenFeatures()
        {
            GEOOntology geoOntology = new GEOOntology("ex:geoOnt");
            geoOntology.DeclareDefaultGeometry(new RDFResource("ex:milanFeat"), new RDFResource("ex:milanDefGeom"));
            geoOntology.DeclarePoint(new RDFResource("ex:milanDefGeom"), 9.188540, 45.464664);
            geoOntology.DeclareSecondaryGeometry(new RDFResource("ex:milanFeat"), new RDFResource("ex:milanSecGeom"));
            geoOntology.DeclarePoint(new RDFResource("ex:milanSecGeom"), 9.191934556314395, 45.46420722396936);
            geoOntology.DeclareDefaultGeometry(new RDFResource("ex:romeFeat"), new RDFResource("ex:romeDefGeom"));
            geoOntology.DeclarePoint(new RDFResource("ex:romeDefGeom"), 12.496365, 41.902782);
            geoOntology.DeclareSecondaryGeometry(new RDFResource("ex:romeFeat"), new RDFResource("ex:romeSecGeom"));
            geoOntology.DeclarePoint(new RDFResource("ex:romeSecGeom"), 12.492218708798534, 41.8903301420294);
            double? milanRomeDistance = geoOntology.GetDistanceBetweenFeatures(new RDFResource("ex:milanFeat"), new RDFResource("ex:romeFeat"));

            Assert.IsTrue(milanRomeDistance >= 450000 && milanRomeDistance <= 4800000); //milan-rome should be between 450km and 480km
         }

        [TestMethod]
        public void ShouldNotGetDistanceBetweenFeaturesBecauseMissingFromGeometries()
        {
            GEOOntology geoOntology = new GEOOntology("ex:geoOnt");
            geoOntology.DeclareDefaultGeometry(new RDFResource("ex:milanFeat"), new RDFResource("ex:milanGeom"));
            geoOntology.DeclareDefaultGeometry(new RDFResource("ex:romeFeat"), new RDFResource("ex:romeGeom"));
            geoOntology.DeclarePoint(new RDFResource("ex:romeGeom"), 12.496365, 41.902782);
            double? milanRomeDistance = geoOntology.GetDistanceBetweenFeatures(new RDFResource("ex:milanFeat"), new RDFResource("ex:romeFeat"));

            Assert.IsNull(milanRomeDistance);
        }

        [TestMethod]
        public void ShouldNotGetDistanceBetweenFeaturesBecauseMissingToGeometries()
        {
            GEOOntology geoOntology = new GEOOntology("ex:geoOnt");
            geoOntology.DeclareDefaultGeometry(new RDFResource("ex:milanFeat"), new RDFResource("ex:milanGeom"));
            geoOntology.DeclarePoint(new RDFResource("ex:milanGeom"), 9.188540, 45.464664);
            geoOntology.DeclareDefaultGeometry(new RDFResource("ex:romeFeat"), new RDFResource("ex:romeGeom"));
            double? milanRomeDistance = geoOntology.GetDistanceBetweenFeatures(new RDFResource("ex:milanFeat"), new RDFResource("ex:romeFeat"));

            Assert.IsNull(milanRomeDistance);
        }

        [TestMethod]
        public void ShouldThrowExceptionOnGettingDistanceBetweenFeaturesBecauseNullFrom()
            => Assert.ThrowsException<OWLSemanticsException>(() => new GEOOntology("ex:geoOnt").GetDistanceBetweenFeatures(null, new RDFResource("ex:to")));

        [TestMethod]
        public void ShouldThrowExceptionOnGettingDistanceBetweenFeaturesBecauseNullTo()
            => Assert.ThrowsException<OWLSemanticsException>(() => new GEOOntology("ex:geoOnt").GetDistanceBetweenFeatures(new RDFResource("ex:from"), null));

        [TestMethod]
        public void ShouldGetFeaturesNearPointFromWKT()
        {
            GEOOntology geoOntology = new GEOOntology("ex:geoOnt");
            geoOntology.DeclareDefaultGeometry(new RDFResource("ex:milanFeat"), new RDFResource("ex:milanGeom"));
            geoOntology.DeclarePoint(new RDFResource("ex:milanGeom"), 9.188540, 45.464664);
            geoOntology.DeclareDefaultGeometry(new RDFResource("ex:romeFeat"), new RDFResource("ex:romeDefGeom"));
            geoOntology.DeclarePoint(new RDFResource("ex:romeDefGeom"), 12.496365, 41.902782);
            geoOntology.DeclareSecondaryGeometry(new RDFResource("ex:romeFeat"), new RDFResource("ex:romeSecGeom"));
            geoOntology.DeclarePoint(new RDFResource("ex:romeSecGeom"), 12.492218708798534, 41.8903301420294);
            geoOntology.DeclareDefaultGeometry(new RDFResource("ex:tivoliFeat"), new RDFResource("ex:tivoliDefGeom"));
            geoOntology.DeclarePoint(new RDFResource("ex:tivoliDefGeom"), 12.799386614751448, 41.9621771776109);
            List<RDFResource> featuresWithin100KmFromRome = geoOntology.GetFeaturesNearPoint(12.496365, 41.902782, 100000);

            Assert.IsNotNull(featuresWithin100KmFromRome);
            Assert.IsTrue(featuresWithin100KmFromRome.Count == 2);
            Assert.IsTrue(featuresWithin100KmFromRome.Any(ft => ft.Equals(new RDFResource("ex:romeFeat"))));
            Assert.IsTrue(featuresWithin100KmFromRome.Any(ft => ft.Equals(new RDFResource("ex:tivoliFeat"))));
        }

        [TestMethod]
        public void ShouldGetFeaturesNearPointFromGML()
        {
            GEOOntology geoOntology = new GEOOntology("ex:geoOnt");
            geoOntology.DeclareDefaultGeometry(new RDFResource("ex:milanFeat"), new RDFResource("ex:milanGeom"));
            geoOntology.DeclarePoint(new RDFResource("ex:milanGeom"), 9.188540, 45.464664);
            geoOntology.DeclareDefaultGeometry(new RDFResource("ex:romeFeat"), new RDFResource("ex:romeDefGeom"));
            geoOntology.DeclarePoint(new RDFResource("ex:romeDefGeom"), 12.496365, 41.902782);
            geoOntology.DeclareSecondaryGeometry(new RDFResource("ex:romeFeat"), new RDFResource("ex:romeSecGeom"));
            geoOntology.DeclarePoint(new RDFResource("ex:romeSecGeom"), 12.492218708798534, 41.8903301420294);
            geoOntology.DeclareDefaultGeometry(new RDFResource("ex:tivoliFeat"), new RDFResource("ex:tivoliDefGeom"));
            geoOntology.DeclarePoint(new RDFResource("ex:tivoliDefGeom"), 12.799386614751448, 41.9621771776109);
            geoOntology.Data.ABoxGraph.RemoveTriplesByPredicate(RDFVocabulary.GEOSPARQL.AS_WKT);
            List<RDFResource> featuresWithin100KmFromRome = geoOntology.GetFeaturesNearPoint(9.064627221414433, 45.475514057674644, 10000); //10Km around Settimo Milanese

            Assert.IsNotNull(featuresWithin100KmFromRome);
            Assert.IsTrue(featuresWithin100KmFromRome.Count == 1);
            Assert.IsTrue(featuresWithin100KmFromRome.Any(ft => ft.Equals(new RDFResource("ex:milanFeat"))));
        }

        [TestMethod]
        public void ShouldNotGetFeaturesNearPoint()
        {
            GEOOntology geoOntology = new GEOOntology("ex:geoOnt");
            geoOntology.DeclareDefaultGeometry(new RDFResource("ex:milanFeat"), new RDFResource("ex:milanGeom"));
            geoOntology.DeclarePoint(new RDFResource("ex:milanGeom"), 9.188540, 45.464664);
            geoOntology.DeclareDefaultGeometry(new RDFResource("ex:romeFeat"), new RDFResource("ex:romeDefGeom"));
            geoOntology.DeclarePoint(new RDFResource("ex:romeDefGeom"), 12.496365, 41.902782);
            geoOntology.DeclareSecondaryGeometry(new RDFResource("ex:romeFeat"), new RDFResource("ex:romeSecGeom"));
            geoOntology.DeclarePoint(new RDFResource("ex:romeSecGeom"), 12.492218708798534, 41.8903301420294);
            geoOntology.DeclareDefaultGeometry(new RDFResource("ex:tivoliFeat"), new RDFResource("ex:tivoliDefGeom"));
            geoOntology.DeclarePoint(new RDFResource("ex:tivoliDefGeom"), 12.799386614751448, 41.9621771776109);
            List<RDFResource> featuresWithin100KmFromRome = geoOntology.GetFeaturesNearPoint(11.538600883689174, 45.54896859401364, 20000); //20km around Vicenza

            Assert.IsNotNull(featuresWithin100KmFromRome);
            Assert.IsTrue(featuresWithin100KmFromRome.Count == 0);
        }

        [TestMethod]
        public void ShouldNotGetFeaturesNearPointBecauseMissingGeometries()
        {
            GEOOntology geoOntology = new GEOOntology("ex:geoOnt");
            geoOntology.DeclareDefaultGeometry(new RDFResource("ex:milanFeat"), new RDFResource("ex:milanGeom"));
            geoOntology.DeclareDefaultGeometry(new RDFResource("ex:romeFeat"), new RDFResource("ex:romeDefGeom"));
            geoOntology.DeclareSecondaryGeometry(new RDFResource("ex:romeFeat"), new RDFResource("ex:romeSecGeom"));
            geoOntology.DeclareDefaultGeometry(new RDFResource("ex:tivoliFeat"), new RDFResource("ex:tivoliDefGeom"));
            List<RDFResource> featuresWithin100KmFromRome = geoOntology.GetFeaturesNearPoint(11.538600883689174, 45.54896859401364, 20000); //20km around Vicenza

            Assert.IsNotNull(featuresWithin100KmFromRome);
            Assert.IsTrue(featuresWithin100KmFromRome.Count == 0);
        }

        [TestMethod]
        public void ShouldThrowExceptionOnGettingFeaturesNearPointBecauseInvalidLongitude()
            => Assert.ThrowsException<OWLSemanticsException>(() => new GEOOntology("ex:geoOnt").GetFeaturesNearPoint(-181, 45, 1000));

        [TestMethod]
        public void ShouldThrowExceptionOnGettingFeaturesNearPointBecauseInvalidLatitude()
            => Assert.ThrowsException<OWLSemanticsException>(() => new GEOOntology("ex:geoOnt").GetFeaturesNearPoint(9, 91, 1000));

        [TestMethod]
        public void ShouldGetFeaturesWithinBoxFromWKT()
        {
            GEOOntology geoOntology = new GEOOntology("ex:geoOnt");
            geoOntology.DeclareDefaultGeometry(new RDFResource("ex:milanFeat"), new RDFResource("ex:milanGeom"));
            geoOntology.DeclarePoint(new RDFResource("ex:milanGeom"), 9.188540, 45.464664);
            geoOntology.DeclareDefaultGeometry(new RDFResource("ex:romeFeat"), new RDFResource("ex:romeDefGeom"));
            geoOntology.DeclarePoint(new RDFResource("ex:romeDefGeom"), 12.496365, 41.902782);
            geoOntology.DeclareSecondaryGeometry(new RDFResource("ex:romeFeat"), new RDFResource("ex:romeSecGeom"));
            geoOntology.DeclarePoint(new RDFResource("ex:romeSecGeom"), 12.492218708798534, 41.8903301420294);
            geoOntology.DeclareDefaultGeometry(new RDFResource("ex:tivoliFeat"), new RDFResource("ex:tivoliDefGeom"));
            geoOntology.DeclarePoint(new RDFResource("ex:tivoliDefGeom"), 12.799386614751448, 41.9621771776109);
            List<RDFResource> featuresWithinSearchBox = geoOntology.GetFeaturesWithinBox(12.42447817, 41.84821607, 12.82959902, 41.98310753);

            Assert.IsNotNull(featuresWithinSearchBox);
            Assert.IsTrue(featuresWithinSearchBox.Count == 2);
            Assert.IsTrue(featuresWithinSearchBox.Any(ft => ft.Equals(new RDFResource("ex:romeFeat"))));
            Assert.IsTrue(featuresWithinSearchBox.Any(ft => ft.Equals(new RDFResource("ex:tivoliFeat"))));
        }

        [TestMethod]
        public void ShouldGetFeaturesWithinBoxFromGML()
        {
            GEOOntology geoOntology = new GEOOntology("ex:geoOnt");
            geoOntology.DeclareDefaultGeometry(new RDFResource("ex:milanFeat"), new RDFResource("ex:milanGeom"));
            geoOntology.DeclarePoint(new RDFResource("ex:milanGeom"), 9.188540, 45.464664);
            geoOntology.DeclareDefaultGeometry(new RDFResource("ex:romeFeat"), new RDFResource("ex:romeDefGeom"));
            geoOntology.DeclarePoint(new RDFResource("ex:romeDefGeom"), 12.496365, 41.902782);
            geoOntology.DeclareSecondaryGeometry(new RDFResource("ex:romeFeat"), new RDFResource("ex:romeSecGeom"));
            geoOntology.DeclarePoint(new RDFResource("ex:romeSecGeom"), 12.492218708798534, 41.8903301420294);
            geoOntology.DeclareDefaultGeometry(new RDFResource("ex:tivoliFeat"), new RDFResource("ex:tivoliDefGeom"));
            geoOntology.DeclarePoint(new RDFResource("ex:tivoliDefGeom"), 12.799386614751448, 41.9621771776109);
            geoOntology.Data.ABoxGraph.RemoveTriplesByPredicate(RDFVocabulary.GEOSPARQL.AS_WKT);
            List<RDFResource> featuresWithinSearchBox = geoOntology.GetFeaturesWithinBox(12.42447817, 41.84821607, 12.82959902, 41.98310753);

            Assert.IsNotNull(featuresWithinSearchBox);
            Assert.IsTrue(featuresWithinSearchBox.Count == 2);
            Assert.IsTrue(featuresWithinSearchBox.Any(ft => ft.Equals(new RDFResource("ex:romeFeat"))));
            Assert.IsTrue(featuresWithinSearchBox.Any(ft => ft.Equals(new RDFResource("ex:tivoliFeat"))));
        }

        [TestMethod]
        public void ShouldNotGetFeaturesWithinBox()
        {
            GEOOntology geoOntology = new GEOOntology("ex:geoOnt");
            geoOntology.DeclareDefaultGeometry(new RDFResource("ex:milanFeat"), new RDFResource("ex:milanGeom"));
            geoOntology.DeclarePoint(new RDFResource("ex:milanGeom"), 9.188540, 45.464664);
            geoOntology.DeclareDefaultGeometry(new RDFResource("ex:romeFeat"), new RDFResource("ex:romeDefGeom"));
            geoOntology.DeclarePoint(new RDFResource("ex:romeDefGeom"), 12.496365, 41.902782);
            geoOntology.DeclareSecondaryGeometry(new RDFResource("ex:romeFeat"), new RDFResource("ex:romeSecGeom"));
            geoOntology.DeclarePoint(new RDFResource("ex:romeSecGeom"), 12.492218708798534, 41.8903301420294);
            geoOntology.DeclareDefaultGeometry(new RDFResource("ex:tivoliFeat"), new RDFResource("ex:tivoliDefGeom"));
            geoOntology.DeclarePoint(new RDFResource("ex:tivoliDefGeom"), 12.799386614751448, 41.9621771776109);
            List<RDFResource> featuresWithinSearchBox = geoOntology.GetFeaturesWithinBox(12.506875630937513, 41.67714954342952, 12.678537007890638, 41.807283590331984); //Pomezia-Frascati

            Assert.IsNotNull(featuresWithinSearchBox);
            Assert.IsTrue(featuresWithinSearchBox.Count == 0);
        }

        [TestMethod]
        public void ShouldNotGetFeaturesWithinBoxBecauseMissingGeometries()
        {
            GEOOntology geoOntology = new GEOOntology("ex:geoOnt");
            geoOntology.DeclareDefaultGeometry(new RDFResource("ex:milanFeat"), new RDFResource("ex:milanGeom"));
            geoOntology.DeclareDefaultGeometry(new RDFResource("ex:romeFeat"), new RDFResource("ex:romeDefGeom"));
            geoOntology.DeclareSecondaryGeometry(new RDFResource("ex:romeFeat"), new RDFResource("ex:romeSecGeom"));
            geoOntology.DeclareDefaultGeometry(new RDFResource("ex:tivoliFeat"), new RDFResource("ex:tivoliDefGeom"));
            List<RDFResource> featuresWithin100KmFromRome = geoOntology.GetFeaturesWithinBox(12.42447817, 41.84821607, 12.82959902, 41.98310753);

            Assert.IsNotNull(featuresWithin100KmFromRome);
            Assert.IsTrue(featuresWithin100KmFromRome.Count == 0);
        }

        [TestMethod]
        public void ShouldThrowExceptionOnGettingFeaturesWithinBoxBecauseInvalidLowerLeftLongitude()
            => Assert.ThrowsException<OWLSemanticsException>(() => new GEOOntology("ex:geoOnt").GetFeaturesWithinBox(-181, 45, 76, 58));

        [TestMethod]
        public void ShouldThrowExceptionOnGettingFeaturesWithinBoxBecauseInvalidLowerLeftLatitude()
            => Assert.ThrowsException<OWLSemanticsException>(() => new GEOOntology("ex:geoOnt").GetFeaturesWithinBox(9, 91, 76, 58));

        [TestMethod]
        public void ShouldThrowExceptionOnGettingFeaturesWithinBoxBecauseInvalidUpperRightLongitude()
            => Assert.ThrowsException<OWLSemanticsException>(() => new GEOOntology("ex:geoOnt").GetFeaturesWithinBox(32, 45, 181, 58));

        [TestMethod]
        public void ShouldThrowExceptionOnGettingFeaturesWithinBoxBecauseInvalidUpperRightLatitude()
            => Assert.ThrowsException<OWLSemanticsException>(() => new GEOOntology("ex:geoOnt").GetFeaturesWithinBox(9, 45, 76, 91));

        [TestMethod]
        public void ShouldThrowExceptionOnGettingFeaturesWithinBoxBecauseExceedingLowerLeftLongitude()
            => Assert.ThrowsException<OWLSemanticsException>(() => new GEOOntology("ex:geoOnt").GetFeaturesWithinBox(81, 45, 76, 58));

        [TestMethod]
        public void ShouldThrowExceptionOnGettingFeaturesWithinBoxBecauseExceedingLowerLeftLatitude()
            => Assert.ThrowsException<OWLSemanticsException>(() => new GEOOntology("ex:geoOnt").GetFeaturesWithinBox(9, 84, 76, 58));
        #endregion
    }
}