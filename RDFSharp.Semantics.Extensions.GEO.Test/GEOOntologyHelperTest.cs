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
using RDFSharp.Model;
using System.Collections.Generic;

namespace RDFSharp.Semantics.Extensions.GEO.Test
{
    [TestClass]
    public class GEOOntologyHelperTest
    {
        #region Tests
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

            //Test geometries
            Assert.IsTrue(geoOnt.Geometries.Count == 2);
            Assert.IsTrue(geoOnt.Geometries["ex:Milan"].Item2.Distance(geoOnt.Geometries["ex:Rome"].Item2)/1000 >= 450 
                           && geoOnt.Geometries["ex:Milan"].Item2.Distance(geoOnt.Geometries["ex:Rome"].Item2)/1000 <= 480); //Between 450km and 480km
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

            //Test geometries
            Assert.IsTrue(geoOnt.Geometries.Count == 1);
            Assert.IsTrue(geoOnt.Geometries["ex:MilanRome"].Item2.Length / 1000 >= 450 
                           && geoOnt.Geometries["ex:MilanRome"].Item2.Length / 1000 <= 480); //Between 450km and 480km
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

            //Test geometries
            Assert.IsTrue(geoOnt.Geometries.Count == 1);
            Assert.IsTrue(geoOnt.Geometries["ex:MilanRomeNaples"].Item2.Length / 1000 >= 1300
                           && geoOnt.Geometries["ex:MilanRomeNaples"].Item2.Length / 1000 <= 1400); //Between 1300km and 1400km
            Assert.IsTrue(geoOnt.Geometries["ex:MilanRomeNaples"].Item2.Area / 1000000 >= 14000
                           && geoOnt.Geometries["ex:MilanRomeNaples"].Item2.Area / 1000000 <= 15000); //Between 14000km2 and 15000km2
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

            //Test geometries
            Assert.IsTrue(geoOnt.Geometries.Count == 1);
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
        #endregion
    }
}