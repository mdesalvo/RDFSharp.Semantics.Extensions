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
            geoOntology.DeclareDefaultGeometry(new RDFResource("ex:milanFeat"), new RDFResource("ex:milanDefGeom"));
            geoOntology.DeclarePoint(new RDFResource("ex:milanDefGeom"), 9.188540, 45.464664);
            geoOntology.DeclareSecondaryGeometry(new RDFResource("ex:milanFeat"), new RDFResource("ex:milanSecGeom"));
            geoOntology.DeclarePoint(new RDFResource("ex:milanSecGeom"), 9.191934556314395, 45.46420722396936);
            geoOntology.DeclareDefaultGeometry(new RDFResource("ex:romeFeat"), new RDFResource("ex:romeDefGeom"));
            geoOntology.DeclarePoint(new RDFResource("ex:romeDefGeom"), 12.496365, 41.902782);
            geoOntology.DeclareSecondaryGeometry(new RDFResource("ex:romeFeat"), new RDFResource("ex:romeSecGeom"));
            geoOntology.DeclarePoint(new RDFResource("ex:romeSecGeom"), 12.492218708798534, 41.8903301420294);
            double? milanRomeDistance = geoOntology.SpatialHelper.GetDistanceBetweenFeatures(new RDFResource("ex:milanFeat"), new RDFResource("ex:romeFeat"));

            Assert.IsTrue(milanRomeDistance >= 450000 && milanRomeDistance <= 4800000); //milan-rome should be between 450km and 480km
         }

        [TestMethod]
        public void ShouldNotGetDistanceBetweenFeaturesBecauseMissingFromGeometries()
        {
            GEOOntology geoOntology = new GEOOntology("ex:geoOnt");
            geoOntology.DeclareDefaultGeometry(new RDFResource("ex:milanFeat"), new RDFResource("ex:milanGeom"));
            geoOntology.DeclareDefaultGeometry(new RDFResource("ex:romeFeat"), new RDFResource("ex:romeGeom"));
            geoOntology.DeclarePoint(new RDFResource("ex:romeGeom"), 12.496365, 41.902782);
            double? milanRomeDistance = geoOntology.SpatialHelper.GetDistanceBetweenFeatures(new RDFResource("ex:milanFeat"), new RDFResource("ex:romeFeat"));

            Assert.IsNull(milanRomeDistance);
        }

        [TestMethod]
        public void ShouldNotGetDistanceBetweenFeaturesBecauseMissingToGeometries()
        {
            GEOOntology geoOntology = new GEOOntology("ex:geoOnt");
            geoOntology.DeclareDefaultGeometry(new RDFResource("ex:milanFeat"), new RDFResource("ex:milanGeom"));
            geoOntology.DeclarePoint(new RDFResource("ex:milanGeom"), 9.188540, 45.464664);
            geoOntology.DeclareDefaultGeometry(new RDFResource("ex:romeFeat"), new RDFResource("ex:romeGeom"));
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
            List<RDFResource> featuresWithin100KmFromRome = geoOntology.SpatialHelper.GetFeaturesNearPoint((12.496365, 41.902782), 100000);

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
            List<RDFResource> featuresWithin100KmFromRome = geoOntology.SpatialHelper.GetFeaturesNearPoint((9.064627221414433, 45.475514057674644), 10000); //10Km around Settimo Milanese

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
            List<RDFResource> featuresWithin100KmFromRome = geoOntology.SpatialHelper.GetFeaturesNearPoint((11.538600883689174, 45.54896859401364), 20000); //20km around Vicenza

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
            List<RDFResource> featuresWithin100KmFromRome = geoOntology.SpatialHelper.GetFeaturesNearPoint((11.538600883689174, 45.54896859401364), 20000); //20km around Vicenza

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
            geoOntology.DeclareDefaultGeometry(new RDFResource("ex:milanFeat"), new RDFResource("ex:milanGeom"));
            geoOntology.DeclarePoint(new RDFResource("ex:milanGeom"), 9.188540, 45.464664);
            geoOntology.DeclareDefaultGeometry(new RDFResource("ex:romeFeat"), new RDFResource("ex:romeDefGeom"));
            geoOntology.DeclarePoint(new RDFResource("ex:romeDefGeom"), 12.496365, 41.902782);
            geoOntology.DeclareSecondaryGeometry(new RDFResource("ex:romeFeat"), new RDFResource("ex:romeSecGeom"));
            geoOntology.DeclarePoint(new RDFResource("ex:romeSecGeom"), 12.492218708798534, 41.8903301420294);
            geoOntology.DeclareDefaultGeometry(new RDFResource("ex:tivoliFeat"), new RDFResource("ex:tivoliDefGeom"));
            geoOntology.DeclarePoint(new RDFResource("ex:tivoliDefGeom"), 12.799386614751448, 41.9621771776109);
            List<RDFResource> featuresWithin100KmFromRome = geoOntology.SpatialHelper.GetFeaturesNorthOfPoint((9.03879405213778, 44.457875560621204)); //Genoa

            Assert.IsNotNull(featuresWithin100KmFromRome);
            Assert.IsTrue(featuresWithin100KmFromRome.Count == 1);
            Assert.IsTrue(featuresWithin100KmFromRome.Any(ft => ft.Equals(new RDFResource("ex:milanFeat"))));
        }

        [TestMethod]
        public void ShouldGetFeaturesNorthOfPointFromGML()
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
            List<RDFResource> featuresWithin100KmFromRome = geoOntology.SpatialHelper.GetFeaturesNorthOfPoint((9.03879405213778, 44.457875560621204)); //Genoa

            Assert.IsNotNull(featuresWithin100KmFromRome);
            Assert.IsTrue(featuresWithin100KmFromRome.Count == 1);
            Assert.IsTrue(featuresWithin100KmFromRome.Any(ft => ft.Equals(new RDFResource("ex:milanFeat"))));
        }

        [TestMethod]
        public void ShouldNotGetFeaturesNorthOfPointFromWKT()
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
            List<RDFResource> featuresWithin100KmFromRome = geoOntology.SpatialHelper.GetFeaturesNorthOfPoint((10.20883799745028, 45.560772926104875)); //Brescia

            Assert.IsNotNull(featuresWithin100KmFromRome);
            Assert.IsTrue(featuresWithin100KmFromRome.Count == 0);
        }

        [TestMethod]
        public void ShouldNotGetFeaturesNorthOfPointBecauseMissingGeometries()
        {
            GEOOntology geoOntology = new GEOOntology("ex:geoOnt");
            geoOntology.DeclareDefaultGeometry(new RDFResource("ex:milanFeat"), new RDFResource("ex:milanGeom"));
            geoOntology.DeclareDefaultGeometry(new RDFResource("ex:romeFeat"), new RDFResource("ex:romeDefGeom"));
            geoOntology.DeclareSecondaryGeometry(new RDFResource("ex:romeFeat"), new RDFResource("ex:romeSecGeom"));
            geoOntology.DeclareDefaultGeometry(new RDFResource("ex:tivoliFeat"), new RDFResource("ex:tivoliDefGeom"));
            List<RDFResource> featuresWithin100KmFromRome = geoOntology.SpatialHelper.GetFeaturesNorthOfPoint((11.538600883689174, 45.54896859401364));

            Assert.IsNotNull(featuresWithin100KmFromRome);
            Assert.IsTrue(featuresWithin100KmFromRome.Count == 0);
        }

        [TestMethod]
        public void ShouldThrowExceptionOnGettingFeaturesNorthOfPointBecauseInvalidLongitude()
            => Assert.ThrowsException<OWLSemanticsException>(() => new GEOOntology("ex:geoOnt").SpatialHelper.GetFeaturesNorthOfPoint((-181, 45)));

        [TestMethod]
        public void ShouldThrowExceptionOnGettingFeaturesNorthOfPointBecauseInvalidLatitude()
            => Assert.ThrowsException<OWLSemanticsException>(() => new GEOOntology("ex:geoOnt").SpatialHelper.GetFeaturesNorthOfPoint((9, 91)));

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
            List<RDFResource> featuresWithinSearchBox = geoOntology.SpatialHelper.GetFeaturesWithinBox((12.42447817, 41.84821607), (12.82959902, 41.98310753));

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
            List<RDFResource> featuresWithinSearchBox = geoOntology.SpatialHelper.GetFeaturesWithinBox((12.42447817, 41.84821607), (12.82959902, 41.98310753));

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
            List<RDFResource> featuresWithinSearchBox = geoOntology.SpatialHelper.GetFeaturesWithinBox((12.506875630937513, 41.67714954342952), (12.678537007890638, 41.807283590331984)); //Pomezia-Frascati

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
            List<RDFResource> featuresWithin100KmFromRome = geoOntology.SpatialHelper.GetFeaturesWithinBox((12.42447817, 41.84821607), (12.82959902, 41.98310753));

            Assert.IsNotNull(featuresWithin100KmFromRome);
            Assert.IsTrue(featuresWithin100KmFromRome.Count == 0);
        }

        [TestMethod]
        public void ShouldThrowExceptionOnGettingFeaturesWithinBoxBecauseInvalidLowerLeftLongitude()
            => Assert.ThrowsException<OWLSemanticsException>(() => new GEOOntology("ex:geoOnt").SpatialHelper.GetFeaturesWithinBox((-181, 45), (76, 58)));

        [TestMethod]
        public void ShouldThrowExceptionOnGettingFeaturesWithinBoxBecauseInvalidLowerLeftLatitude()
            => Assert.ThrowsException<OWLSemanticsException>(() => new GEOOntology("ex:geoOnt").SpatialHelper.GetFeaturesWithinBox((9, 91), (76, 58)));

        [TestMethod]
        public void ShouldThrowExceptionOnGettingFeaturesWithinBoxBecauseInvalidUpperRightLongitude()
            => Assert.ThrowsException<OWLSemanticsException>(() => new GEOOntology("ex:geoOnt").SpatialHelper.GetFeaturesWithinBox((32, 45), (181, 58)));

        [TestMethod]
        public void ShouldThrowExceptionOnGettingFeaturesWithinBoxBecauseInvalidUpperRightLatitude()
            => Assert.ThrowsException<OWLSemanticsException>(() => new GEOOntology("ex:geoOnt").SpatialHelper.GetFeaturesWithinBox((9, 45), (76, 91)));

        [TestMethod]
        public void ShouldThrowExceptionOnGettingFeaturesWithinBoxBecauseExceedingLowerLeftLongitude()
            => Assert.ThrowsException<OWLSemanticsException>(() => new GEOOntology("ex:geoOnt").SpatialHelper.GetFeaturesWithinBox((81, 45), (76, 58)));

        [TestMethod]
        public void ShouldThrowExceptionOnGettingFeaturesWithinBoxBecauseExceedingLowerLeftLatitude()
            => Assert.ThrowsException<OWLSemanticsException>(() => new GEOOntology("ex:geoOnt").SpatialHelper.GetFeaturesWithinBox((9, 84), (76, 58)));
        #endregion
    }
}