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
using System.Linq;

namespace RDFSharp.Semantics.Extensions.GEO.Test
{
    [TestClass]
    public class GEOSpatialHelperTest
    {
        #region Tests
        [TestMethod]
        public void ShouldGetDistanceBetweenFeatures()
        {
            GEOOntology geoOntology = new GEOOntology("ex:geoOnt");
            geoOntology.DeclarePointFeature(new RDFResource("ex:milanFeat"), new RDFResource("ex:milanDefGeom"), (9.188540, 45.464664), true);
            geoOntology.DeclarePointFeature(new RDFResource("ex:milanFeat"), new RDFResource("ex:milanSecGeom"), (9.19193456, 45.46420722), false);
            geoOntology.DeclarePointFeature(new RDFResource("ex:romeFeat"), new RDFResource("ex:romeDefGeom"), (12.496365, 41.902782), true);
            geoOntology.DeclarePointFeature(new RDFResource("ex:romeFeat"), new RDFResource("ex:romeSecGeom"), (12.49221871, 41.89033014), false);
            double? milanRomeDistance = geoOntology.SpatialHelper.GetDistanceBetweenFeatures(new RDFResource("ex:milanFeat"), new RDFResource("ex:romeFeat"));

            Assert.IsTrue(milanRomeDistance >= 450000 && milanRomeDistance <= 4800000); //milan-rome should be between 450km and 480km
        }

        [TestMethod]
        public void ShouldNotGetDistanceBetweenFeaturesBecauseMissingFromGeometries()
        {
            GEOOntology geoOntology = new GEOOntology("ex:geoOnt");
            geoOntology.Data.DeclareIndividual(new RDFResource("ex:milanFeat"));
            geoOntology.Data.DeclareIndividualType(new RDFResource("ex:milanFeat"), RDFVocabulary.GEOSPARQL.FEATURE);
            geoOntology.DeclarePointFeature(new RDFResource("ex:romeFeat"), new RDFResource("ex:romeDefGeom"), (12.496365, 41.902782), true);
            geoOntology.DeclarePointFeature(new RDFResource("ex:romeFeat"), new RDFResource("ex:romeSecGeom"), (12.49221871, 41.89033014), false);
            double? milanRomeDistance = geoOntology.SpatialHelper.GetDistanceBetweenFeatures(new RDFResource("ex:milanFeat"), new RDFResource("ex:romeFeat"));

            Assert.IsNull(milanRomeDistance);
        }

        [TestMethod]
        public void ShouldNotGetDistanceBetweenFeaturesBecauseMissingToGeometries()
        {
            GEOOntology geoOntology = new GEOOntology("ex:geoOnt");
            geoOntology.DeclarePointFeature(new RDFResource("ex:milanFeat"), new RDFResource("ex:milanDefGeom"), (9.188540, 45.464664), true);
            geoOntology.DeclarePointFeature(new RDFResource("ex:milanFeat"), new RDFResource("ex:milanSecGeom"), (9.19193456, 45.46420722), false);
            geoOntology.Data.DeclareIndividual(new RDFResource("ex:romeFeat"));
            geoOntology.Data.DeclareIndividualType(new RDFResource("ex:romeFeat"), RDFVocabulary.GEOSPARQL.FEATURE);
            double? milanRomeDistance = geoOntology.SpatialHelper.GetDistanceBetweenFeatures(new RDFResource("ex:milanFeat"), new RDFResource("ex:romeFeat"));

            Assert.IsNull(milanRomeDistance);
        }

        [TestMethod]
        public void ShouldThrowExceptionOnGettingDistanceBetweenFeaturesBecauseNullFrom()
            => Assert.ThrowsException<OWLSemanticsException>(() => new GEOOntology("ex:geoOnt").SpatialHelper.GetDistanceBetweenFeatures(null, new RDFResource("ex:to")));

        [TestMethod]
        public void ShouldThrowExceptionOnGettingDistanceBetweenFeaturesBecauseNullTo()
            => Assert.ThrowsException<OWLSemanticsException>(() => new GEOOntology("ex:geoOnt").SpatialHelper.GetDistanceBetweenFeatures(new RDFResource("ex:from"), null));

        [TestMethod]
        public void ShouldGetLengthOfFeature()
        {
            GEOOntology geoOntology = new GEOOntology("ex:geoOnt");
            geoOntology.DeclareAreaFeature(new RDFResource("ex:milanCentreFeat"), new RDFResource("ex:milanCentreGeom"), new List<(double, double)>() {
                (9.18217536, 45.46819347), (9.19054385, 45.46819347), (9.19054385, 45.46003666), (9.18217536, 45.46003666), (9.18217536, 45.46819347) }, true);
            geoOntology.DeclareLineFeature(new RDFResource("ex:brebemiFeat"), new RDFResource("ex:brebemiGeom1"), new List<(double, double)>() {
                (9.16778508, 45.46481222), (9.6118352, 45.68014585), (10.21423284, 45.54758259) }, true);
            geoOntology.DeclareLineFeature(new RDFResource("ex:brebemiFeat"), new RDFResource("ex:brebemiGeom2"), new List<(double, double)>() {
                (9.16778508, 45.46481222), (9.62118352, 45.65014585), (10.26423284, 45.59758259) }, true);
            geoOntology.DeclarePointFeature(new RDFResource("ex:milanFeat"), new RDFResource("ex:milanGeom"), (9.16778508, 45.46481222), false);
            double? milanCentreLength = geoOntology.SpatialHelper.GetLengthOfFeature(new RDFResource("ex:milanCentreFeat"));
            double? brebemiLength = geoOntology.SpatialHelper.GetLengthOfFeature(new RDFResource("ex:brebemiFeat"));
            double? milanLength = geoOntology.SpatialHelper.GetLengthOfFeature(new RDFResource("ex:milanFeat"));

            Assert.IsTrue(milanCentreLength >= 3000 && milanCentreLength <= 3300); //Perimeter of milan centre is about 3KM lineair
            Assert.IsTrue(brebemiLength >= 95000 && brebemiLength <= 100000); //BreBeMi is about 95-100KM lineair
            Assert.IsTrue(milanLength == 0); //points have no length
        }

        [TestMethod]
        public void ShouldNotGetLengthOfFeatureBecauseMissingGeometries()
        {
            GEOOntology geoOntology = new GEOOntology("ex:geoOnt");
            geoOntology.Data.DeclareIndividual(new RDFResource("ex:milanFeat"));
            geoOntology.Data.DeclareIndividualType(new RDFResource("ex:milanFeat"), RDFVocabulary.GEOSPARQL.FEATURE);
            double? milanLength = geoOntology.SpatialHelper.GetLengthOfFeature(new RDFResource("ex:milanFeat"));

            Assert.IsNull(milanLength);
        }

        [TestMethod]
        public void ShouldThrowExceptionOnGettingLengthOfFeatureBecauseNullUri()
            => Assert.ThrowsException<OWLSemanticsException>(() => new GEOOntology("ex:geoOnt").SpatialHelper.GetLengthOfFeature(null));

        [TestMethod]
        public void ShouldGetAreaOfFeature()
        {
            GEOOntology geoOntology = new GEOOntology("ex:geoOnt");
            geoOntology.DeclareAreaFeature(new RDFResource("ex:milanCentreFeat"), new RDFResource("ex:milanCentreGeom"), new List<(double, double)>() {
                (9.18217536, 45.46819347), (9.19054385, 45.46819347), (9.19054385, 45.46003666), (9.18217536, 45.46003666), (9.18217536, 45.46819347) }, true);
            geoOntology.DeclareLineFeature(new RDFResource("ex:brebemiFeat"), new RDFResource("ex:brebemiGeom"), new List<(double, double)>() {
                (9.16778508, 45.46481222), (9.6118352, 45.68014585), (10.21423284, 45.54758259) }, true);
            geoOntology.DeclarePointFeature(new RDFResource("ex:milanFeat"), new RDFResource("ex:milanGeom"), (9.16778508, 45.46481222), false);
            double? brebemiArea = geoOntology.SpatialHelper.GetAreaOfFeature(new RDFResource("ex:brebemiFeat"));
            double? milanArea = geoOntology.SpatialHelper.GetAreaOfFeature(new RDFResource("ex:milanFeat"));
            double? milanCentreArea = geoOntology.SpatialHelper.GetAreaOfFeature(new RDFResource("ex:milanCentreFeat"));

            Assert.IsTrue(milanCentreArea >= 590000 && milanCentreArea <= 600000);
            Assert.IsTrue(brebemiArea == 0); //lines have no area
            Assert.IsTrue(milanArea == 0); //points have no area
        }

        [TestMethod]
        public void ShouldNotGetAreaOfFeatureBecauseMissingGeometries()
        {
            GEOOntology geoOntology = new GEOOntology("ex:geoOnt");
            geoOntology.Data.DeclareIndividual(new RDFResource("ex:milanFeat"));
            geoOntology.Data.DeclareIndividualType(new RDFResource("ex:milanFeat"), RDFVocabulary.GEOSPARQL.FEATURE);
            double? milanArea = geoOntology.SpatialHelper.GetAreaOfFeature(new RDFResource("ex:milanFeat"));

            Assert.IsNull(milanArea);
        }

        [TestMethod]
        public void ShouldThrowExceptionOnGettingAreaOfFeatureBecauseNullUri()
            => Assert.ThrowsException<OWLSemanticsException>(() => new GEOOntology("ex:geoOnt").SpatialHelper.GetAreaOfFeature(null));

        [TestMethod]
        public void ShouldGetBoundaryOfFeature()
        {
            GEOOntology geoOntology = new GEOOntology("ex:geoOnt");
            geoOntology.DeclareAreaFeature(new RDFResource("ex:milanCentreFeat"), new RDFResource("ex:milanCentreGeom"), new List<(double, double)>() {
                (9.18217536, 45.46819347), (9.19054385, 45.46819347), (9.19054385, 45.46003666), (9.18217536, 45.46003666), (9.18217536, 45.46819347) }, true);
            geoOntology.DeclareLineFeature(new RDFResource("ex:brebemiFeat"), new RDFResource("ex:brebemiGeom"), new List<(double, double)>() {
                (9.16778508, 45.46481222), (9.6118352, 45.68014585), (10.21423284, 45.54758259) }, false);
            geoOntology.DeclarePointFeature(new RDFResource("ex:milanFeat"), new RDFResource("ex:milanGeom"), (9.16778508, 45.46481222), false);
            RDFTypedLiteral milanCentreBoundary = geoOntology.SpatialHelper.GetBoundaryOfFeature(new RDFResource("ex:milanCentreFeat"));
            RDFTypedLiteral brebemiBoundary = geoOntology.SpatialHelper.GetBoundaryOfFeature(new RDFResource("ex:brebemiFeat"));
            RDFTypedLiteral milanBoundary = geoOntology.SpatialHelper.GetBoundaryOfFeature(new RDFResource("ex:milanFeat"));
            
            Assert.IsNotNull(milanCentreBoundary);
            Assert.IsTrue(milanCentreBoundary.Equals(new RDFTypedLiteral("LINESTRING (9.18217536 45.46819347, 9.19054385 45.46819347, 9.19054385 45.46003666, 9.18217536 45.46003666, 9.18217536 45.46819347)", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT)));
            Assert.IsNotNull(brebemiBoundary);
            Assert.IsTrue(brebemiBoundary.Equals(new RDFTypedLiteral("MULTIPOINT ((9.16778508 45.46481222), (10.21423284 45.54758259))", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT)));
            Assert.IsNotNull(milanBoundary);
            Assert.IsTrue(milanBoundary.Equals(new RDFTypedLiteral("GEOMETRYCOLLECTION EMPTY", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT)));
        }

        [TestMethod]
        public void ShouldNotGetBoundaryOfFeatureBecauseMissingGeometries()
        {
            GEOOntology geoOntology = new GEOOntology("ex:geoOnt");
            geoOntology.Data.DeclareIndividual(new RDFResource("ex:milanFeat"));
            geoOntology.Data.DeclareIndividualType(new RDFResource("ex:milanFeat"), RDFVocabulary.GEOSPARQL.FEATURE);
            RDFTypedLiteral milanBoundary = geoOntology.SpatialHelper.GetBoundaryOfFeature(new RDFResource("ex:milanFeat"));

            Assert.IsNull(milanBoundary);
        }

        [TestMethod]
        public void ShouldThrowExceptionOnGettingBoundaryOfFeatureBecauseNullUri()
            => Assert.ThrowsException<OWLSemanticsException>(() => new GEOOntology("ex:geoOnt").SpatialHelper.GetBoundaryOfFeature(null as RDFResource));

        [TestMethod]
        public void ShouldGetBoundaryOfWKTFeature()
        {
            GEOSpatialHelper spatialHelper = new GEOSpatialHelper(null);
            RDFTypedLiteral milanCentreBoundary = spatialHelper.GetBoundaryOfFeature(new RDFTypedLiteral("POLYGON((9.18217536 45.46819347, 9.19054385 45.46819347, 9.19054385 45.46003666, 9.18217536 45.46003666, 9.18217536 45.46819347))", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT));
            RDFTypedLiteral brebemiBoundary = spatialHelper.GetBoundaryOfFeature(new RDFTypedLiteral("LINESTRING(9.16778508 45.46481222, 9.6118352 45.68014585, 10.21423284 45.54758259)", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT));
            RDFTypedLiteral milanBoundary = spatialHelper.GetBoundaryOfFeature(new RDFTypedLiteral("POINT(9.16778508 45.46481222)", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT));

            Assert.IsNotNull(milanCentreBoundary);
            Assert.IsTrue(milanCentreBoundary.Equals(new RDFTypedLiteral("LINESTRING (9.18217536 45.46819347, 9.19054385 45.46819347, 9.19054385 45.46003666, 9.18217536 45.46003666, 9.18217536 45.46819347)", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT)));
            Assert.IsNotNull(brebemiBoundary);
            Assert.IsTrue(brebemiBoundary.Equals(new RDFTypedLiteral("MULTIPOINT ((9.16778508 45.46481222), (10.21423284 45.54758259))", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT)));
            Assert.IsNotNull(milanBoundary);
            Assert.IsTrue(milanBoundary.Equals(new RDFTypedLiteral("GEOMETRYCOLLECTION EMPTY", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT)));
        }

        [TestMethod]
        public void ShouldThrowExceptionOnGettingBoundaryOfWKTFeatureBecauseNullLiteral()
            => Assert.ThrowsException<OWLSemanticsException>(() => new GEOOntology("ex:geoOnt").SpatialHelper.GetBoundaryOfFeature(null as RDFTypedLiteral));

        [TestMethod]
        public void ShouldThrowExceptionOnGettingBoundaryOfWKTFeatureBecauseNotGeographicLiteral()
            => Assert.ThrowsException<OWLSemanticsException>(() => new GEOOntology("ex:geoOnt").SpatialHelper.GetBoundaryOfFeature(new RDFTypedLiteral("hello", RDFModelEnums.RDFDatatypes.RDFS_LITERAL)));

        [TestMethod]
        public void ShouldGetBufferAroundFeature()
        {
            GEOOntology geoOntology = new GEOOntology("ex:geoOnt");
            geoOntology.DeclareAreaFeature(new RDFResource("ex:milanCentreFeat"), new RDFResource("ex:milanCentreGeom"), new List<(double, double)>() {
                (9.18217536, 45.46819347), (9.19054385, 45.46819347), (9.19054385, 45.46003666), (9.18217536, 45.46003666), (9.18217536, 45.46819347) }, true);
            geoOntology.DeclareLineFeature(new RDFResource("ex:brebemiFeat"), new RDFResource("ex:brebemiGeom"), new List<(double, double)>() {
                (9.16778508, 45.46481222), (9.6118352, 45.68014585), (10.21423284, 45.54758259) }, false);
            geoOntology.DeclarePointFeature(new RDFResource("ex:milanFeat"), new RDFResource("ex:milanGeom"), (9.16778508, 45.46481222), false);
            RDFTypedLiteral milanCentreBuffer = geoOntology.SpatialHelper.GetBufferAroundFeature(new RDFResource("ex:milanCentreFeat"), 5000);
            RDFTypedLiteral brebemiBuffer = geoOntology.SpatialHelper.GetBufferAroundFeature(new RDFResource("ex:brebemiFeat"), 5000);
            RDFTypedLiteral milanBuffer = geoOntology.SpatialHelper.GetBufferAroundFeature(new RDFResource("ex:milanFeat"), 5000);

            Assert.IsNotNull(milanCentreBuffer);
            Assert.IsTrue(milanCentreBuffer.Equals(new RDFTypedLiteral("POLYGON ((9.12167581 45.47166215, 9.12272511 45.48045166, 9.12585291 45.48881321, 9.13095057 45.49645448, 9.13784041 45.50310824, 9.14628183 45.50854175, 9.15597971 45.5125649, 9.16659471 45.51503687, 9.17775522 45.51587112, 9.18612951 45.51587156, 9.19854003 45.51486882, 9.21061331 45.51190188, 9.22184051 45.50709563, 9.23174866 45.50065245, 9.23992067 45.49284362, 9.24601291 45.48399789, 9.24976966 45.47448762, 9.25103377 45.46471308, 9.25102652 45.45655599, 9.24996149 45.44776617, 9.24682014 45.43940575, 9.24171298 45.43176688, 9.234819 45.42511643, 9.22637941 45.41968668, 9.2166891 45.41566723, 9.20608644 45.41319837, 9.19494141 45.41236626, 9.18657871 45.4123667, 9.17418592 45.41336954, 9.16212839 45.41633501, 9.15091245 45.42113841, 9.14100927 45.42757784, 9.1328352 45.43538264, 9.12673428 45.44422471, 9.12296373 45.4537323, 9.12168305 45.46350562, 9.12167581 45.47166215))", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT)));
            Assert.IsNotNull(brebemiBuffer);
            Assert.IsTrue(brebemiBuffer.Equals(new RDFTypedLiteral("POLYGON ((9.57750259 45.7213073, 9.58820757 45.72531047, 9.59986996 45.72748624, 9.61201717 45.72774644, 9.62415683 45.72608045, 10.22695044 45.5934877, 10.23824673 45.59004725, 10.24861827 45.58497436, 10.25766599 45.57846416, 10.26504204 45.57076707, 10.27046316 45.56217912, 10.27372157 45.55303054, 10.27469286 45.54367306, 10.27334062 45.53446636, 10.26971773 45.52576421, 10.26396419 45.51790094, 10.25630159 45.51117854, 10.24702456 45.50585511, 10.2364894 45.50213501, 10.22510042 45.50016099, 10.21329451 45.50000875, 10.20152443 45.50168408, 9.62481345 45.62869051, 9.20220303 45.4237427, 9.19184348 45.41981076, 9.18056095 45.41760779, 9.16878828 45.41721828, 9.15697698 45.41865711, 9.14558001 45.42186899, 9.13503456 45.42673058, 9.12574534 45.43305522, 9.11806912 45.44060008, 9.11230103 45.44907545, 9.10866322 45.45815585, 9.10729626 45.46749248, 9.1082536 45.47672665, 9.11149935 45.48550347, 9.11690961 45.49348557, 9.12427699 45.50036604, 9.1333186 45.50588024, 9.57750259 45.7213073))", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT)));
            Assert.IsNotNull(milanBuffer);
            Assert.IsTrue(milanBuffer.Equals(new RDFTypedLiteral("POLYGON ((9.22781127 45.46725521, 9.22811497 45.45791943, 9.22610039 45.44884908, 9.22184584 45.44039267, 9.21551552 45.43287506, 9.2073531 45.42658493, 9.1976723 45.4217638, 9.18684485 45.41859669, 9.17528622 45.41720513, 9.16343974 45.41764244, 9.15175974 45.41989179, 9.14069419 45.42386676, 9.13066764 45.42941474, 9.122065 45.43632272, 9.1152168 45.44432546, 9.11038653 45.45311564, 9.10776044 45.46235567, 9.10744029 45.4716906, 9.10943931 45.48076173, 9.11368159 45.48922043, 9.12000477 45.49674149, 9.12816629 45.5030357, 9.13785255 45.50786094, 9.14869099 45.51103154, 9.16026449 45.51242547, 9.17212743 45.511989, 9.18382298 45.50973888, 9.1949008 45.50576162, 9.20493449 45.50021018, 9.21353803 45.49329812, 9.22038077 45.48529128, 9.22520004 45.4764976, 9.22781127 45.46725521))", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT)));
        }

        [TestMethod]
        public void ShouldNotGetBufferAroundFeatureBecauseMissingGeometries()
        {
            GEOOntology geoOntology = new GEOOntology("ex:geoOnt");
            geoOntology.Data.DeclareIndividual(new RDFResource("ex:milanFeat"));
            geoOntology.Data.DeclareIndividualType(new RDFResource("ex:milanFeat"), RDFVocabulary.GEOSPARQL.FEATURE);
            RDFTypedLiteral milanBuffer = geoOntology.SpatialHelper.GetBufferAroundFeature(new RDFResource("ex:milanFeat"), 2500);

            Assert.IsNull(milanBuffer);
        }

        [TestMethod]
        public void ShouldThrowExceptionOnGettingBufferAroundFeatureBecauseNullUri()
            => Assert.ThrowsException<OWLSemanticsException>(() => new GEOOntology("ex:geoOnt").SpatialHelper.GetBufferAroundFeature(null as RDFResource, 650));

        [TestMethod]
        public void ShouldGetBufferAroundWKTFeature()
        {
            GEOSpatialHelper spatialHelper = new GEOSpatialHelper(null);
            RDFTypedLiteral milanCentreBuffer = spatialHelper.GetBufferAroundFeature(new RDFTypedLiteral("POLYGON((9.18217536 45.46819347, 9.19054385 45.46819347, 9.19054385 45.46003666, 9.18217536 45.46003666, 9.18217536 45.46819347))", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT), 5000);
            RDFTypedLiteral brebemiBuffer = spatialHelper.GetBufferAroundFeature(new RDFTypedLiteral("LINESTRING(9.16778508 45.46481222, 9.6118352 45.68014585, 10.21423284 45.54758259)", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT), 5000);
            RDFTypedLiteral milanBuffer = spatialHelper.GetBufferAroundFeature(new RDFTypedLiteral("POINT(9.16778508 45.46481222)", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT), 5000);

            Assert.IsNotNull(milanCentreBuffer);
            Assert.IsTrue(milanCentreBuffer.Equals(new RDFTypedLiteral("POLYGON ((9.12167581 45.47166215, 9.12272511 45.48045166, 9.12585291 45.48881321, 9.13095057 45.49645448, 9.13784041 45.50310824, 9.14628183 45.50854175, 9.15597971 45.5125649, 9.16659471 45.51503687, 9.17775522 45.51587112, 9.18612951 45.51587156, 9.19854003 45.51486882, 9.21061331 45.51190188, 9.22184051 45.50709563, 9.23174866 45.50065245, 9.23992067 45.49284362, 9.24601291 45.48399789, 9.24976966 45.47448762, 9.25103377 45.46471308, 9.25102652 45.45655599, 9.24996149 45.44776617, 9.24682014 45.43940575, 9.24171298 45.43176688, 9.234819 45.42511643, 9.22637941 45.41968668, 9.2166891 45.41566723, 9.20608644 45.41319837, 9.19494141 45.41236626, 9.18657871 45.4123667, 9.17418592 45.41336954, 9.16212839 45.41633501, 9.15091245 45.42113841, 9.14100927 45.42757784, 9.1328352 45.43538264, 9.12673428 45.44422471, 9.12296373 45.4537323, 9.12168305 45.46350562, 9.12167581 45.47166215))", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT)));
            Assert.IsNotNull(brebemiBuffer);
            Assert.IsTrue(brebemiBuffer.Equals(new RDFTypedLiteral("POLYGON ((9.57750259 45.7213073, 9.58820757 45.72531047, 9.59986996 45.72748624, 9.61201717 45.72774644, 9.62415683 45.72608045, 10.22695044 45.5934877, 10.23824673 45.59004725, 10.24861827 45.58497436, 10.25766599 45.57846416, 10.26504204 45.57076707, 10.27046316 45.56217912, 10.27372157 45.55303054, 10.27469286 45.54367306, 10.27334062 45.53446636, 10.26971773 45.52576421, 10.26396419 45.51790094, 10.25630159 45.51117854, 10.24702456 45.50585511, 10.2364894 45.50213501, 10.22510042 45.50016099, 10.21329451 45.50000875, 10.20152443 45.50168408, 9.62481345 45.62869051, 9.20220303 45.4237427, 9.19184348 45.41981076, 9.18056095 45.41760779, 9.16878828 45.41721828, 9.15697698 45.41865711, 9.14558001 45.42186899, 9.13503456 45.42673058, 9.12574534 45.43305522, 9.11806912 45.44060008, 9.11230103 45.44907545, 9.10866322 45.45815585, 9.10729626 45.46749248, 9.1082536 45.47672665, 9.11149935 45.48550347, 9.11690961 45.49348557, 9.12427699 45.50036604, 9.1333186 45.50588024, 9.57750259 45.7213073))", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT)));
            Assert.IsNotNull(milanBuffer);
            Assert.IsTrue(milanBuffer.Equals(new RDFTypedLiteral("POLYGON ((9.22781127 45.46725521, 9.22811497 45.45791943, 9.22610039 45.44884908, 9.22184584 45.44039267, 9.21551552 45.43287506, 9.2073531 45.42658493, 9.1976723 45.4217638, 9.18684485 45.41859669, 9.17528622 45.41720513, 9.16343974 45.41764244, 9.15175974 45.41989179, 9.14069419 45.42386676, 9.13066764 45.42941474, 9.122065 45.43632272, 9.1152168 45.44432546, 9.11038653 45.45311564, 9.10776044 45.46235567, 9.10744029 45.4716906, 9.10943931 45.48076173, 9.11368159 45.48922043, 9.12000477 45.49674149, 9.12816629 45.5030357, 9.13785255 45.50786094, 9.14869099 45.51103154, 9.16026449 45.51242547, 9.17212743 45.511989, 9.18382298 45.50973888, 9.1949008 45.50576162, 9.20493449 45.50021018, 9.21353803 45.49329812, 9.22038077 45.48529128, 9.22520004 45.4764976, 9.22781127 45.46725521))", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT)));
        }

        [TestMethod]
        public void ShouldThrowExceptionOnGettingBufferAroundWKTFeatureBecauseNullLiteral()
            => Assert.ThrowsException<OWLSemanticsException>(() => new GEOOntology("ex:geoOnt").SpatialHelper.GetBufferAroundFeature(null as RDFTypedLiteral, 20000));

        [TestMethod]
        public void ShouldThrowExceptionOnGettingBufferAroundWKTFeatureBecauseNotGeographicLiteral()
            => Assert.ThrowsException<OWLSemanticsException>(() => new GEOOntology("ex:geoOnt").SpatialHelper.GetBufferAroundFeature(new RDFTypedLiteral("hello", RDFModelEnums.RDFDatatypes.RDFS_LITERAL), 12000));

        [TestMethod]
        public void ShouldGetFeaturesNearPointFromWKT()
        {
            GEOOntology geoOntology = new GEOOntology("ex:geoOnt");
            geoOntology.DeclarePointFeature(new RDFResource("ex:milanFeat"), new RDFResource("ex:milanGeom"), (9.188540, 45.464664), true);
            geoOntology.DeclarePointFeature(new RDFResource("ex:romeFeat"), new RDFResource("ex:romeDefGeom"), (12.496365, 41.902782), true);
            geoOntology.DeclarePointFeature(new RDFResource("ex:romeFeat"), new RDFResource("ex:romeSecGeom"), (12.49221871, 41.89033014), false);
            geoOntology.DeclarePointFeature(new RDFResource("ex:tivoliFeat"), new RDFResource("ex:tivoliDefGeom"), (12.79938661, 41.96217718), true);
            List<RDFResource> featuresNearPoint = geoOntology.SpatialHelper.GetFeaturesNearPoint((12.496365, 41.902782), 100000); //100km around Rome (DefGeom)

            Assert.IsNotNull(featuresNearPoint);
            Assert.IsTrue(featuresNearPoint.Count == 2);
            Assert.IsTrue(featuresNearPoint.Any(ft => ft.Equals(new RDFResource("ex:romeFeat"))));
            Assert.IsTrue(featuresNearPoint.Any(ft => ft.Equals(new RDFResource("ex:tivoliFeat"))));
        }

        [TestMethod]
        public void ShouldGetFeaturesNearPointFromGML()
        {
            GEOOntology geoOntology = new GEOOntology("ex:geoOnt");
            geoOntology.DeclarePointFeature(new RDFResource("ex:milanFeat"), new RDFResource("ex:milanGeom"), (9.188540, 45.464664), true);
            geoOntology.DeclarePointFeature(new RDFResource("ex:romeFeat"), new RDFResource("ex:romeDefGeom"), (12.496365, 41.902782), true);
            geoOntology.DeclarePointFeature(new RDFResource("ex:romeFeat"), new RDFResource("ex:romeSecGeom"), (12.49221871, 41.89033014), false);
            geoOntology.DeclarePointFeature(new RDFResource("ex:tivoliFeat"), new RDFResource("ex:tivoliDefGeom"), (12.79938661, 41.96217718), true);
            geoOntology.Data.ABoxGraph.RemoveTriplesByPredicate(RDFVocabulary.GEOSPARQL.AS_WKT);
            List<RDFResource> featuresNearPoint = geoOntology.SpatialHelper.GetFeaturesNearPoint((9.15513558, 45.46777408), 10000); //10Km around Milan De Angeli

            Assert.IsNotNull(featuresNearPoint);
            Assert.IsTrue(featuresNearPoint.Count == 1);
            Assert.IsTrue(featuresNearPoint.Any(ft => ft.Equals(new RDFResource("ex:milanFeat"))));
        }

        [TestMethod]
        public void ShouldNotGetFeaturesNearPoint()
        {
            GEOOntology geoOntology = new GEOOntology("ex:geoOnt");
            geoOntology.DeclarePointFeature(new RDFResource("ex:milanFeat"), new RDFResource("ex:milanGeom"), (9.188540, 45.464664), true);
            geoOntology.DeclarePointFeature(new RDFResource("ex:romeFeat"), new RDFResource("ex:romeDefGeom"), (12.496365, 41.902782), true);
            geoOntology.DeclarePointFeature(new RDFResource("ex:romeFeat"), new RDFResource("ex:romeSecGeom"), (12.49221871, 41.89033014), false);
            geoOntology.DeclarePointFeature(new RDFResource("ex:tivoliFeat"), new RDFResource("ex:tivoliDefGeom"), (12.79938661, 41.96217718), true);
            List<RDFResource> featuresWithin100KmFromRome = geoOntology.SpatialHelper.GetFeaturesNearPoint((11.53860088, 45.54896859), 20000); //20km around Vicenza

            Assert.IsNotNull(featuresWithin100KmFromRome);
            Assert.IsTrue(featuresWithin100KmFromRome.Count == 0);
        }

        [TestMethod]
        public void ShouldNotGetFeaturesNearPointBecauseMissingGeometries()
        {
            GEOOntology geoOntology = new GEOOntology("ex:geoOnt");
            geoOntology.Data.DeclareIndividual(new RDFResource("ex:milanFeat"));
            geoOntology.Data.DeclareIndividualType(new RDFResource("ex:milanFeat"), RDFVocabulary.GEOSPARQL.FEATURE);
            List<RDFResource> featuresWithin100KmFromRome = geoOntology.SpatialHelper.GetFeaturesNearPoint((11.53860088, 45.54896859), 20000); //20km around Vicenza

            Assert.IsNotNull(featuresWithin100KmFromRome);
            Assert.IsTrue(featuresWithin100KmFromRome.Count == 0);
        }

        [TestMethod]
        public void ShouldThrowExceptionOnGettingFeaturesNearPointBecauseInvalidLongitude()
            => Assert.ThrowsException<OWLSemanticsException>(() => new GEOOntology("ex:geoOnt").SpatialHelper.GetFeaturesNearPoint((-181, 45), 1000));

        [TestMethod]
        public void ShouldThrowExceptionOnGettingFeaturesNearPointBecauseInvalidLatitude()
            => Assert.ThrowsException<OWLSemanticsException>(() => new GEOOntology("ex:geoOnt").SpatialHelper.GetFeaturesNearPoint((9, 91), 1000));

        [TestMethod]
        public void ShouldGetFeaturesNorthOfPointFromWKT()
        {
            GEOOntology geoOntology = new GEOOntology("ex:geoOnt");
            geoOntology.DeclarePointFeature(new RDFResource("ex:milanFeat"), new RDFResource("ex:milanGeom"), (9.188540, 45.464664), true);
            geoOntology.DeclarePointFeature(new RDFResource("ex:romeFeat"), new RDFResource("ex:romeDefGeom"), (12.496365, 41.902782), true);
            geoOntology.DeclarePointFeature(new RDFResource("ex:romeFeat"), new RDFResource("ex:romeSecGeom"), (12.49221871, 41.89033014), false);
            geoOntology.DeclarePointFeature(new RDFResource("ex:tivoliFeat"), new RDFResource("ex:tivoliDefGeom"), (12.79938661, 41.96217718), true);
            List<RDFResource> featuresNorthOfPoint = geoOntology.SpatialHelper.GetFeaturesNorthOfPoint((9.03879405, 44.45787556)); //Genoa

            Assert.IsNotNull(featuresNorthOfPoint);
            Assert.IsTrue(featuresNorthOfPoint.Count == 1);
            Assert.IsTrue(featuresNorthOfPoint.Any(ft => ft.Equals(new RDFResource("ex:milanFeat"))));
        }

        [TestMethod]
        public void ShouldGetFeaturesNorthOfPointFromGML()
        {
            GEOOntology geoOntology = new GEOOntology("ex:geoOnt");
            geoOntology.DeclarePointFeature(new RDFResource("ex:milanFeat"), new RDFResource("ex:milanGeom"), (9.188540, 45.464664), true);
            geoOntology.DeclarePointFeature(new RDFResource("ex:romeFeat"), new RDFResource("ex:romeDefGeom"), (12.496365, 41.902782), true);
            geoOntology.DeclarePointFeature(new RDFResource("ex:romeFeat"), new RDFResource("ex:romeSecGeom"), (12.49221871, 41.89033014), false);
            geoOntology.DeclarePointFeature(new RDFResource("ex:tivoliFeat"), new RDFResource("ex:tivoliDefGeom"), (12.79938661, 41.96217718), true);
            geoOntology.Data.ABoxGraph.RemoveTriplesByPredicate(RDFVocabulary.GEOSPARQL.AS_WKT);
            List<RDFResource> featuresNorthOfPoint = geoOntology.SpatialHelper.GetFeaturesNorthOfPoint((9.03879405, 44.45787556)); //Genoa

            Assert.IsNotNull(featuresNorthOfPoint);
            Assert.IsTrue(featuresNorthOfPoint.Count == 1);
            Assert.IsTrue(featuresNorthOfPoint.Any(ft => ft.Equals(new RDFResource("ex:milanFeat"))));
        }

        [TestMethod]
        public void ShouldNotGetFeaturesNorthOfPointFromWKT()
        {
            GEOOntology geoOntology = new GEOOntology("ex:geoOnt");
            geoOntology.DeclarePointFeature(new RDFResource("ex:milanFeat"), new RDFResource("ex:milanGeom"), (9.188540, 45.464664), true);
            geoOntology.DeclarePointFeature(new RDFResource("ex:romeFeat"), new RDFResource("ex:romeDefGeom"), (12.496365, 41.902782), true);
            geoOntology.DeclarePointFeature(new RDFResource("ex:romeFeat"), new RDFResource("ex:romeSecGeom"), (12.49221871, 41.89033014), false);
            geoOntology.DeclarePointFeature(new RDFResource("ex:tivoliFeat"), new RDFResource("ex:tivoliDefGeom"), (12.79938661, 41.96217718), true);
            List<RDFResource> featuresNorthOfPoint = geoOntology.SpatialHelper.GetFeaturesNorthOfPoint((10.20883090, 45.56077293)); //Brescia

            Assert.IsNotNull(featuresNorthOfPoint);
            Assert.IsTrue(featuresNorthOfPoint.Count == 0);
        }

        [TestMethod]
        public void ShouldNotGetFeaturesNorthOfPointBecauseMissingGeometries()
        {
            GEOOntology geoOntology = new GEOOntology("ex:geoOnt");
            geoOntology.Data.DeclareIndividual(new RDFResource("ex:milanFeat"));
            geoOntology.Data.DeclareIndividualType(new RDFResource("ex:milanFeat"), RDFVocabulary.GEOSPARQL.FEATURE);
            List<RDFResource> featuresNorthOfPoint = geoOntology.SpatialHelper.GetFeaturesNorthOfPoint((11.53860090, 45.54896859));

            Assert.IsNotNull(featuresNorthOfPoint);
            Assert.IsTrue(featuresNorthOfPoint.Count == 0);
        }

        [TestMethod]
        public void ShouldThrowExceptionOnGettingFeaturesNorthOfPointBecauseInvalidLongitude()
            => Assert.ThrowsException<OWLSemanticsException>(() => new GEOOntology("ex:geoOnt").SpatialHelper.GetFeaturesNorthOfPoint((-181, 45)));

        [TestMethod]
        public void ShouldThrowExceptionOnGettingFeaturesNorthOfPointBecauseInvalidLatitude()
            => Assert.ThrowsException<OWLSemanticsException>(() => new GEOOntology("ex:geoOnt").SpatialHelper.GetFeaturesNorthOfPoint((9, 91)));

        [TestMethod]
        public void ShouldGetFeaturesEastOfPointFromWKT()
        {
            GEOOntology geoOntology = new GEOOntology("ex:geoOnt");
            geoOntology.DeclarePointFeature(new RDFResource("ex:milanFeat"), new RDFResource("ex:milanGeom"), (9.188540, 45.464664), true);
            geoOntology.DeclarePointFeature(new RDFResource("ex:romeFeat"), new RDFResource("ex:romeDefGeom"), (12.496365, 41.902782), true);
            geoOntology.DeclarePointFeature(new RDFResource("ex:romeFeat"), new RDFResource("ex:romeSecGeom"), (12.49221871, 41.89033014), false);
            geoOntology.DeclarePointFeature(new RDFResource("ex:tivoliFeat"), new RDFResource("ex:tivoliDefGeom"), (12.79938661, 41.96217718), true);
            List<RDFResource> featuresEastOfPoint = geoOntology.SpatialHelper.GetFeaturesEastOfPoint((12.396365, 42.902782));

            Assert.IsNotNull(featuresEastOfPoint);
            Assert.IsTrue(featuresEastOfPoint.Count == 2);
            Assert.IsTrue(featuresEastOfPoint.Any(ft => ft.Equals(new RDFResource("ex:romeFeat"))));
            Assert.IsTrue(featuresEastOfPoint.Any(ft => ft.Equals(new RDFResource("ex:tivoliFeat"))));
        }

        [TestMethod]
        public void ShouldGetFeaturesEastOfPointFromGML()
        {
            GEOOntology geoOntology = new GEOOntology("ex:geoOnt");
            geoOntology.DeclarePointFeature(new RDFResource("ex:milanFeat"), new RDFResource("ex:milanGeom"), (9.188540, 45.464664), true);
            geoOntology.DeclarePointFeature(new RDFResource("ex:romeFeat"), new RDFResource("ex:romeDefGeom"), (12.496365, 41.902782), true);
            geoOntology.DeclarePointFeature(new RDFResource("ex:romeFeat"), new RDFResource("ex:romeSecGeom"), (12.49221871, 41.89033014), false);
            geoOntology.DeclarePointFeature(new RDFResource("ex:tivoliFeat"), new RDFResource("ex:tivoliDefGeom"), (12.79938661, 41.96217718), true);
            geoOntology.Data.ABoxGraph.RemoveTriplesByPredicate(RDFVocabulary.GEOSPARQL.AS_WKT);
            List<RDFResource> featuresEastOfPoint = geoOntology.SpatialHelper.GetFeaturesEastOfPoint((12.396365, 42.902782));

            Assert.IsNotNull(featuresEastOfPoint);
            Assert.IsTrue(featuresEastOfPoint.Count == 2);
            Assert.IsTrue(featuresEastOfPoint.Any(ft => ft.Equals(new RDFResource("ex:romeFeat"))));
            Assert.IsTrue(featuresEastOfPoint.Any(ft => ft.Equals(new RDFResource("ex:tivoliFeat"))));
        }

        [TestMethod]
        public void ShouldNotGetFeaturesEastOfPointFromWKT()
        {
            GEOOntology geoOntology = new GEOOntology("ex:geoOnt");
            geoOntology.DeclarePointFeature(new RDFResource("ex:milanFeat"), new RDFResource("ex:milanGeom"), (9.188540, 45.464664), true);
            geoOntology.DeclarePointFeature(new RDFResource("ex:romeFeat"), new RDFResource("ex:romeDefGeom"), (12.496365, 41.902782), true);
            geoOntology.DeclarePointFeature(new RDFResource("ex:romeFeat"), new RDFResource("ex:romeSecGeom"), (12.49221871, 41.89033014), false);
            geoOntology.DeclarePointFeature(new RDFResource("ex:tivoliFeat"), new RDFResource("ex:tivoliDefGeom"), (12.79938661, 41.96217718), true);
            List<RDFResource> featuresEastOfPoint = geoOntology.SpatialHelper.GetFeaturesEastOfPoint((15.80512362, 40.64259592)); //Potenza

            Assert.IsNotNull(featuresEastOfPoint);
            Assert.IsTrue(featuresEastOfPoint.Count == 0);
        }

        [TestMethod]
        public void ShouldNotGetFeaturesEastOfPointBecauseMissingGeometries()
        {
            GEOOntology geoOntology = new GEOOntology("ex:geoOnt");
            geoOntology.Data.DeclareIndividual(new RDFResource("ex:milanFeat"));
            geoOntology.Data.DeclareIndividualType(new RDFResource("ex:milanFeat"), RDFVocabulary.GEOSPARQL.FEATURE);
            List<RDFResource> featuresEastOfPoint = geoOntology.SpatialHelper.GetFeaturesEastOfPoint((11.53860090, 45.54896859));

            Assert.IsNotNull(featuresEastOfPoint);
            Assert.IsTrue(featuresEastOfPoint.Count == 0);
        }

        [TestMethod]
        public void ShouldThrowExceptionOnGettingFeaturesEastOfPointBecauseInvalidLongitude()
            => Assert.ThrowsException<OWLSemanticsException>(() => new GEOOntology("ex:geoOnt").SpatialHelper.GetFeaturesEastOfPoint((-181, 45)));

        [TestMethod]
        public void ShouldThrowExceptionOnGettingFeaturesEastOfPointBecauseInvalidLatitude()
            => Assert.ThrowsException<OWLSemanticsException>(() => new GEOOntology("ex:geoOnt").SpatialHelper.GetFeaturesEastOfPoint((9, 91)));

        [TestMethod]
        public void ShouldGetFeaturesWestOfPointFromWKT()
        {
            GEOOntology geoOntology = new GEOOntology("ex:geoOnt");
            geoOntology.DeclarePointFeature(new RDFResource("ex:milanFeat"), new RDFResource("ex:milanGeom"), (9.188540, 45.464664), true);
            geoOntology.DeclarePointFeature(new RDFResource("ex:romeFeat"), new RDFResource("ex:romeDefGeom"), (12.496365, 41.902782), true);
            geoOntology.DeclarePointFeature(new RDFResource("ex:romeFeat"), new RDFResource("ex:romeSecGeom"), (12.49221871, 41.89033014), false);
            geoOntology.DeclarePointFeature(new RDFResource("ex:tivoliFeat"), new RDFResource("ex:tivoliDefGeom"), (12.79938661, 41.96217718), true);
            List<RDFResource> featuresWestOfPoint = geoOntology.SpatialHelper.GetFeaturesWestOfPoint((12.396365, 42.902782));

            Assert.IsNotNull(featuresWestOfPoint);
            Assert.IsTrue(featuresWestOfPoint.Count == 1);
            Assert.IsTrue(featuresWestOfPoint.Any(ft => ft.Equals(new RDFResource("ex:milanFeat"))));
        }

        [TestMethod]
        public void ShouldGetFeaturesWestOfPointFromGML()
        {
            GEOOntology geoOntology = new GEOOntology("ex:geoOnt");
            geoOntology.DeclarePointFeature(new RDFResource("ex:milanFeat"), new RDFResource("ex:milanGeom"), (9.188540, 45.464664), true);
            geoOntology.DeclarePointFeature(new RDFResource("ex:romeFeat"), new RDFResource("ex:romeDefGeom"), (12.496365, 41.902782), true);
            geoOntology.DeclarePointFeature(new RDFResource("ex:romeFeat"), new RDFResource("ex:romeSecGeom"), (12.49221871, 41.89033014), false);
            geoOntology.DeclarePointFeature(new RDFResource("ex:tivoliFeat"), new RDFResource("ex:tivoliDefGeom"), (12.79938661, 41.96217718), true);
            geoOntology.Data.ABoxGraph.RemoveTriplesByPredicate(RDFVocabulary.GEOSPARQL.AS_WKT);
            List<RDFResource> featuresWestOfPoint = geoOntology.SpatialHelper.GetFeaturesWestOfPoint((12.396365, 42.902782));

            Assert.IsNotNull(featuresWestOfPoint);
            Assert.IsTrue(featuresWestOfPoint.Count == 1);
            Assert.IsTrue(featuresWestOfPoint.Any(ft => ft.Equals(new RDFResource("ex:milanFeat"))));
        }

        [TestMethod]
        public void ShouldNotGetFeaturesWestOfPointFromWKT()
        {
            GEOOntology geoOntology = new GEOOntology("ex:geoOnt");
            geoOntology.DeclarePointFeature(new RDFResource("ex:milanFeat"), new RDFResource("ex:milanGeom"), (9.188540, 45.464664), true);
            geoOntology.DeclarePointFeature(new RDFResource("ex:romeFeat"), new RDFResource("ex:romeDefGeom"), (12.496365, 41.902782), true);
            geoOntology.DeclarePointFeature(new RDFResource("ex:romeFeat"), new RDFResource("ex:romeSecGeom"), (12.49221871, 41.89033014), false);
            geoOntology.DeclarePointFeature(new RDFResource("ex:tivoliFeat"), new RDFResource("ex:tivoliDefGeom"), (12.79938661, 41.96217718), true);
            List<RDFResource> featuresWestOfPoint = geoOntology.SpatialHelper.GetFeaturesWestOfPoint((8.20958180, 44.90095240)); //Asti

            Assert.IsNotNull(featuresWestOfPoint);
            Assert.IsTrue(featuresWestOfPoint.Count == 0);
        }

        [TestMethod]
        public void ShouldNotGetFeaturesWestOfPointBecauseMissingGeometries()
        {
            GEOOntology geoOntology = new GEOOntology("ex:geoOnt");
            geoOntology.Data.DeclareIndividual(new RDFResource("ex:milanFeat"));
            geoOntology.Data.DeclareIndividualType(new RDFResource("ex:milanFeat"), RDFVocabulary.GEOSPARQL.FEATURE);
            List<RDFResource> featuresWestOfPoint = geoOntology.SpatialHelper.GetFeaturesWestOfPoint((11.53860090, 45.54896859));

            Assert.IsNotNull(featuresWestOfPoint);
            Assert.IsTrue(featuresWestOfPoint.Count == 0);
        }

        [TestMethod]
        public void ShouldThrowExceptionOnGettingFeaturesWestOfPointBecauseInvalidLongitude()
            => Assert.ThrowsException<OWLSemanticsException>(() => new GEOOntology("ex:geoOnt").SpatialHelper.GetFeaturesWestOfPoint((-181, 45)));

        [TestMethod]
        public void ShouldThrowExceptionOnGettingFeaturesWestOfPointBecauseInvalidLatitude()
            => Assert.ThrowsException<OWLSemanticsException>(() => new GEOOntology("ex:geoOnt").SpatialHelper.GetFeaturesWestOfPoint((9, 91)));

        [TestMethod]
        public void ShouldGetFeaturesSouthOfPointFromWKT()
        {
            GEOOntology geoOntology = new GEOOntology("ex:geoOnt");
            geoOntology.DeclarePointFeature(new RDFResource("ex:milanFeat"), new RDFResource("ex:milanGeom"), (9.188540, 45.464664), true);
            geoOntology.DeclarePointFeature(new RDFResource("ex:romeFeat"), new RDFResource("ex:romeDefGeom"), (12.496365, 41.902782), true);
            geoOntology.DeclarePointFeature(new RDFResource("ex:romeFeat"), new RDFResource("ex:romeSecGeom"), (12.49221871, 41.89033014), false);
            geoOntology.DeclarePointFeature(new RDFResource("ex:tivoliFeat"), new RDFResource("ex:tivoliDefGeom"), (12.79938661, 41.96217718), true);
            List<RDFResource> featuresSouthOfPoint = geoOntology.SpatialHelper.GetFeaturesSouthOfPoint((9.03879405, 44.45787556)); //Genoa

            Assert.IsNotNull(featuresSouthOfPoint);
            Assert.IsTrue(featuresSouthOfPoint.Count == 2);
            Assert.IsTrue(featuresSouthOfPoint.Any(ft => ft.Equals(new RDFResource("ex:romeFeat"))));
            Assert.IsTrue(featuresSouthOfPoint.Any(ft => ft.Equals(new RDFResource("ex:tivoliFeat"))));
        }

        [TestMethod]
        public void ShouldGetFeaturesSouthOfPointFromGML()
        {
            GEOOntology geoOntology = new GEOOntology("ex:geoOnt");
            geoOntology.DeclarePointFeature(new RDFResource("ex:milanFeat"), new RDFResource("ex:milanGeom"), (9.188540, 45.464664), true);
            geoOntology.DeclarePointFeature(new RDFResource("ex:romeFeat"), new RDFResource("ex:romeDefGeom"), (12.496365, 41.902782), true);
            geoOntology.DeclarePointFeature(new RDFResource("ex:romeFeat"), new RDFResource("ex:romeSecGeom"), (12.49221871, 41.89033014), false);
            geoOntology.DeclarePointFeature(new RDFResource("ex:tivoliFeat"), new RDFResource("ex:tivoliDefGeom"), (12.79938661, 41.96217718), true);
            geoOntology.Data.ABoxGraph.RemoveTriplesByPredicate(RDFVocabulary.GEOSPARQL.AS_WKT);
            List<RDFResource> featuresSouthOfPoint = geoOntology.SpatialHelper.GetFeaturesSouthOfPoint((9.03879405, 44.45787556)); //Genoa

            Assert.IsNotNull(featuresSouthOfPoint);
            Assert.IsTrue(featuresSouthOfPoint.Count == 2);
            Assert.IsTrue(featuresSouthOfPoint.Any(ft => ft.Equals(new RDFResource("ex:romeFeat"))));
            Assert.IsTrue(featuresSouthOfPoint.Any(ft => ft.Equals(new RDFResource("ex:tivoliFeat"))));
        }

        [TestMethod]
        public void ShouldNotGetFeaturesSouthOfPointFromWKT()
        {
            GEOOntology geoOntology = new GEOOntology("ex:geoOnt");
            geoOntology.DeclarePointFeature(new RDFResource("ex:milanFeat"), new RDFResource("ex:milanGeom"), (9.188540, 45.464664), true);
            geoOntology.DeclarePointFeature(new RDFResource("ex:romeFeat"), new RDFResource("ex:romeDefGeom"), (12.496365, 41.902782), true);
            geoOntology.DeclarePointFeature(new RDFResource("ex:romeFeat"), new RDFResource("ex:romeSecGeom"), (12.49221871, 41.89033014), false);
            geoOntology.DeclarePointFeature(new RDFResource("ex:tivoliFeat"), new RDFResource("ex:tivoliDefGeom"), (12.79938661, 41.96217718), true);
            List<RDFResource> featuresSouthOfPoint = geoOntology.SpatialHelper.GetFeaturesSouthOfPoint((15.80512362, 40.64259592)); //Potenza

            Assert.IsNotNull(featuresSouthOfPoint);
            Assert.IsTrue(featuresSouthOfPoint.Count == 0);
        }

        [TestMethod]
        public void ShouldNotGetFeaturesSouthOfPointBecauseMissingGeometries()
        {
            GEOOntology geoOntology = new GEOOntology("ex:geoOnt");
            geoOntology.Data.DeclareIndividual(new RDFResource("ex:milanFeat"));
            geoOntology.Data.DeclareIndividualType(new RDFResource("ex:milanFeat"), RDFVocabulary.GEOSPARQL.FEATURE);
            List<RDFResource> featuresSouthOfPoint = geoOntology.SpatialHelper.GetFeaturesSouthOfPoint((11.53860090, 45.54896859));

            Assert.IsNotNull(featuresSouthOfPoint);
            Assert.IsTrue(featuresSouthOfPoint.Count == 0);
        }

        [TestMethod]
        public void ShouldThrowExceptionOnGettingFeaturesSouthOfPointBecauseInvalidLongitude()
            => Assert.ThrowsException<OWLSemanticsException>(() => new GEOOntology("ex:geoOnt").SpatialHelper.GetFeaturesSouthOfPoint((-181, 45)));

        [TestMethod]
        public void ShouldThrowExceptionOnGettingFeaturesSouthOfPointBecauseInvalidLatitude()
            => Assert.ThrowsException<OWLSemanticsException>(() => new GEOOntology("ex:geoOnt").SpatialHelper.GetFeaturesSouthOfPoint((9, 91)));

        [TestMethod]
        public void ShouldGetFeaturesInsideBoxFromWKT()
        {
            GEOOntology geoOntology = new GEOOntology("ex:geoOnt");
            geoOntology.DeclarePointFeature(new RDFResource("ex:milanFeat"), new RDFResource("ex:milanGeom"), (9.188540, 45.464664), true);
            geoOntology.DeclarePointFeature(new RDFResource("ex:romeFeat"), new RDFResource("ex:romeDefGeom"), (12.496365, 41.902782), true);
            geoOntology.DeclarePointFeature(new RDFResource("ex:romeFeat"), new RDFResource("ex:romeSecGeom"), (12.49221871, 41.89033014), false);
            geoOntology.DeclarePointFeature(new RDFResource("ex:tivoliFeat"), new RDFResource("ex:tivoliDefGeom"), (12.79938661, 41.96217718), true);
            List<RDFResource> featuresWithinSearchBox = geoOntology.SpatialHelper.GetFeaturesInsideBox((12.42447817, 41.84821607), (12.82959902, 41.98310753));

            Assert.IsNotNull(featuresWithinSearchBox);
            Assert.IsTrue(featuresWithinSearchBox.Count == 2);
            Assert.IsTrue(featuresWithinSearchBox.Any(ft => ft.Equals(new RDFResource("ex:romeFeat"))));
            Assert.IsTrue(featuresWithinSearchBox.Any(ft => ft.Equals(new RDFResource("ex:tivoliFeat"))));
        }

        [TestMethod]
        public void ShouldGetFeaturesInsideBoxFromGML()
        {
            GEOOntology geoOntology = new GEOOntology("ex:geoOnt");
            geoOntology.DeclarePointFeature(new RDFResource("ex:milanFeat"), new RDFResource("ex:milanGeom"), (9.188540, 45.464664), true);
            geoOntology.DeclarePointFeature(new RDFResource("ex:romeFeat"), new RDFResource("ex:romeDefGeom"), (12.496365, 41.902782), true);
            geoOntology.DeclarePointFeature(new RDFResource("ex:romeFeat"), new RDFResource("ex:romeSecGeom"), (12.49221871, 41.89033014), false);
            geoOntology.DeclarePointFeature(new RDFResource("ex:tivoliFeat"), new RDFResource("ex:tivoliDefGeom"), (12.79938661, 41.96217718), true);
            geoOntology.Data.ABoxGraph.RemoveTriplesByPredicate(RDFVocabulary.GEOSPARQL.AS_WKT);
            List<RDFResource> featuresWithinSearchBox = geoOntology.SpatialHelper.GetFeaturesInsideBox((12.42447817, 41.84821607), (12.82959902, 41.98310753));

            Assert.IsNotNull(featuresWithinSearchBox);
            Assert.IsTrue(featuresWithinSearchBox.Count == 2);
            Assert.IsTrue(featuresWithinSearchBox.Any(ft => ft.Equals(new RDFResource("ex:romeFeat"))));
            Assert.IsTrue(featuresWithinSearchBox.Any(ft => ft.Equals(new RDFResource("ex:tivoliFeat"))));
        }

        [TestMethod]
        public void ShouldNotGetFeaturesInsideBox()
        {
            GEOOntology geoOntology = new GEOOntology("ex:geoOnt");
            geoOntology.DeclarePointFeature(new RDFResource("ex:milanFeat"), new RDFResource("ex:milanGeom"), (9.188540, 45.464664), true);
            geoOntology.DeclarePointFeature(new RDFResource("ex:romeFeat"), new RDFResource("ex:romeDefGeom"), (12.496365, 41.902782), true);
            geoOntology.DeclarePointFeature(new RDFResource("ex:romeFeat"), new RDFResource("ex:romeSecGeom"), (12.49221871, 41.89033014), false);
            geoOntology.DeclarePointFeature(new RDFResource("ex:tivoliFeat"), new RDFResource("ex:tivoliDefGeom"), (12.79938661, 41.96217718), true);
            List<RDFResource> featuresWithinSearchBox = geoOntology.SpatialHelper.GetFeaturesInsideBox((12.50687563, 41.67714954), (12.67853701, 41.80728360)); //Pomezia-Frascati

            Assert.IsNotNull(featuresWithinSearchBox);
            Assert.IsTrue(featuresWithinSearchBox.Count == 0);
        }

        [TestMethod]
        public void ShouldNotGetFeaturesInsideBoxBecauseMissingGeometries()
        {
            GEOOntology geoOntology = new GEOOntology("ex:geoOnt");
            geoOntology.Data.DeclareIndividual(new RDFResource("ex:milanFeat"));
            geoOntology.Data.DeclareIndividualType(new RDFResource("ex:milanFeat"), RDFVocabulary.GEOSPARQL.FEATURE);
            List<RDFResource> featuresWithin100KmFromRome = geoOntology.SpatialHelper.GetFeaturesInsideBox((12.50687563, 41.67714954), (12.67853701, 41.80728360)); //Pomezia-Frascati

            Assert.IsNotNull(featuresWithin100KmFromRome);
            Assert.IsTrue(featuresWithin100KmFromRome.Count == 0);
        }

        [TestMethod]
        public void ShouldThrowExceptionOnGettingFeaturesInsideBoxBecauseInvalidLowerLeftLongitude()
            => Assert.ThrowsException<OWLSemanticsException>(() => new GEOOntology("ex:geoOnt").SpatialHelper.GetFeaturesInsideBox((-181, 45), (76, 58)));

        [TestMethod]
        public void ShouldThrowExceptionOnGettingFeaturesInsideBoxBecauseInvalidLowerLeftLatitude()
            => Assert.ThrowsException<OWLSemanticsException>(() => new GEOOntology("ex:geoOnt").SpatialHelper.GetFeaturesInsideBox((9, 91), (76, 58)));

        [TestMethod]
        public void ShouldThrowExceptionOnGettingFeaturesInsideBoxBecauseInvalidUpperRightLongitude()
            => Assert.ThrowsException<OWLSemanticsException>(() => new GEOOntology("ex:geoOnt").SpatialHelper.GetFeaturesInsideBox((32, 45), (181, 58)));

        [TestMethod]
        public void ShouldThrowExceptionOnGettingFeaturesInsideBoxBecauseInvalidUpperRightLatitude()
            => Assert.ThrowsException<OWLSemanticsException>(() => new GEOOntology("ex:geoOnt").SpatialHelper.GetFeaturesInsideBox((9, 45), (76, 91)));

        [TestMethod]
        public void ShouldThrowExceptionOnGettingFeaturesInsideBoxBecauseExceedingLowerLeftLongitude()
            => Assert.ThrowsException<OWLSemanticsException>(() => new GEOOntology("ex:geoOnt").SpatialHelper.GetFeaturesInsideBox((81, 45), (76, 58)));

        [TestMethod]
        public void ShouldThrowExceptionOnGettingFeaturesInsideBoxBecauseExceedingLowerLeftLatitude()
            => Assert.ThrowsException<OWLSemanticsException>(() => new GEOOntology("ex:geoOnt").SpatialHelper.GetFeaturesInsideBox((9, 84), (76, 58)));

        [TestMethod]
        public void ShouldGetFeaturesOutsideBoxFromWKT()
        {
            GEOOntology geoOntology = new GEOOntology("ex:geoOnt");
            geoOntology.DeclarePointFeature(new RDFResource("ex:milanFeat"), new RDFResource("ex:milanGeom"), (9.188540, 45.464664), true);
            geoOntology.DeclarePointFeature(new RDFResource("ex:romeFeat"), new RDFResource("ex:romeDefGeom"), (12.496365, 41.902782), true);
            geoOntology.DeclarePointFeature(new RDFResource("ex:romeFeat"), new RDFResource("ex:romeSecGeom"), (12.49221871, 41.89033014), false);
            geoOntology.DeclarePointFeature(new RDFResource("ex:tivoliFeat"), new RDFResource("ex:tivoliDefGeom"), (12.79938661, 41.96217718), true);
            List<RDFResource> featuresOutsideSearchBox = geoOntology.SpatialHelper.GetFeaturesOutsideBox((12.42447817, 41.84821607), (12.82959902, 41.98310753));

            Assert.IsNotNull(featuresOutsideSearchBox);
            Assert.IsTrue(featuresOutsideSearchBox.Count == 1);
            Assert.IsTrue(featuresOutsideSearchBox.Any(ft => ft.Equals(new RDFResource("ex:milanFeat"))));
        }

        [TestMethod]
        public void ShouldGetFeaturesOutsideBoxFromGML()
        {
            GEOOntology geoOntology = new GEOOntology("ex:geoOnt");
            geoOntology.DeclarePointFeature(new RDFResource("ex:milanFeat"), new RDFResource("ex:milanGeom"), (9.188540, 45.464664), true);
            geoOntology.DeclarePointFeature(new RDFResource("ex:romeFeat"), new RDFResource("ex:romeDefGeom"), (12.496365, 41.902782), true);
            geoOntology.DeclarePointFeature(new RDFResource("ex:romeFeat"), new RDFResource("ex:romeSecGeom"), (12.49221871, 41.89033014), false);
            geoOntology.DeclarePointFeature(new RDFResource("ex:tivoliFeat"), new RDFResource("ex:tivoliDefGeom"), (12.79938661, 41.96217718), true);
            geoOntology.Data.ABoxGraph.RemoveTriplesByPredicate(RDFVocabulary.GEOSPARQL.AS_WKT);
            List<RDFResource> featuresOutsideSearchBox = geoOntology.SpatialHelper.GetFeaturesOutsideBox((12.42447817, 41.84821607), (12.82959902, 41.98310753));

            Assert.IsNotNull(featuresOutsideSearchBox);
            Assert.IsTrue(featuresOutsideSearchBox.Count == 1);
            Assert.IsTrue(featuresOutsideSearchBox.Any(ft => ft.Equals(new RDFResource("ex:milanFeat"))));
        }

        [TestMethod]
        public void ShouldNotGetFeaturesOutsideBox()
        {
            GEOOntology geoOntology = new GEOOntology("ex:geoOnt");
            geoOntology.DeclarePointFeature(new RDFResource("ex:milanFeat"), new RDFResource("ex:milanGeom"), (9.188540, 45.464664), true);
            List<RDFResource> featuresOutsideSearchBox = geoOntology.SpatialHelper.GetFeaturesOutsideBox((9.12149722, 45.18770380), (9.82530581, 45.77780892));

            Assert.IsNotNull(featuresOutsideSearchBox);
            Assert.IsTrue(featuresOutsideSearchBox.Count == 0);
        }

        [TestMethod]
        public void ShouldNotGetFeaturesOutsideBoxBecauseMissingGeometries()
        {
            GEOOntology geoOntology = new GEOOntology("ex:geoOnt");
            geoOntology.Data.DeclareIndividual(new RDFResource("ex:milanFeat"));
            geoOntology.Data.DeclareIndividualType(new RDFResource("ex:milanFeat"), RDFVocabulary.GEOSPARQL.FEATURE);
            List<RDFResource> featuresOutside100KmFromRome = geoOntology.SpatialHelper.GetFeaturesOutsideBox((12.50687563, 41.67714954), (12.67853701, 41.80728360));

            Assert.IsNotNull(featuresOutside100KmFromRome);
            Assert.IsTrue(featuresOutside100KmFromRome.Count == 0);
        }

        [TestMethod]
        public void ShouldThrowExceptionOnGettingFeaturesOutsideBoxBecauseInvalidLowerLeftLongitude()
            => Assert.ThrowsException<OWLSemanticsException>(() => new GEOOntology("ex:geoOnt").SpatialHelper.GetFeaturesOutsideBox((-181, 45), (76, 58)));

        [TestMethod]
        public void ShouldThrowExceptionOnGettingFeaturesOutsideBoxBecauseInvalidLowerLeftLatitude()
            => Assert.ThrowsException<OWLSemanticsException>(() => new GEOOntology("ex:geoOnt").SpatialHelper.GetFeaturesOutsideBox((9, 91), (76, 58)));

        [TestMethod]
        public void ShouldThrowExceptionOnGettingFeaturesOutsideBoxBecauseInvalidUpperRightLongitude()
            => Assert.ThrowsException<OWLSemanticsException>(() => new GEOOntology("ex:geoOnt").SpatialHelper.GetFeaturesOutsideBox((32, 45), (181, 58)));

        [TestMethod]
        public void ShouldThrowExceptionOnGettingFeaturesOutsideBoxBecauseInvalidUpperRightLatitude()
            => Assert.ThrowsException<OWLSemanticsException>(() => new GEOOntology("ex:geoOnt").SpatialHelper.GetFeaturesOutsideBox((9, 45), (76, 91)));

        [TestMethod]
        public void ShouldThrowExceptionOnGettingFeaturesOutsideBoxBecauseExceedingLowerLeftLongitude()
            => Assert.ThrowsException<OWLSemanticsException>(() => new GEOOntology("ex:geoOnt").SpatialHelper.GetFeaturesOutsideBox((81, 45), (76, 58)));

        [TestMethod]
        public void ShouldThrowExceptionOnGettingFeaturesOutsideBoxBecauseExceedingLowerLeftLatitude()
            => Assert.ThrowsException<OWLSemanticsException>(() => new GEOOntology("ex:geoOnt").SpatialHelper.GetFeaturesOutsideBox((9, 84), (76, 58)));
        #endregion
    }
}