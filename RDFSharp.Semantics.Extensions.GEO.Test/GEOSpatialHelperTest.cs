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
        public void ShouldGetFeaturesNearPointFromWKT()
        {
            GEOOntology geoOntology = new GEOOntology("ex:geoOnt");
            geoOntology.DeclarePointFeature(new RDFResource("ex:milanFeat"), new RDFResource("ex:milanGeom"), (9.188540, 45.464664), true);
            geoOntology.DeclarePointFeature(new RDFResource("ex:romeFeat"), new RDFResource("ex:romeDefGeom"), (12.496365, 41.902782), true);
            geoOntology.DeclarePointFeature(new RDFResource("ex:romeFeat"), new RDFResource("ex:romeSecGeom"), (12.49221871, 41.89033014), false);
            geoOntology.DeclarePointFeature(new RDFResource("ex:tivoliFeat"), new RDFResource("ex:tivoliDefGeom"), (12.79938661, 41.96217718), true);
            List<RDFResource> featuresWithin100KmFromRome = geoOntology.SpatialHelper.GetFeaturesNearPoint((12.496365, 41.902782), 100000); //100km around Rome (DefGeom)

            Assert.IsNotNull(featuresWithin100KmFromRome);
            Assert.IsTrue(featuresWithin100KmFromRome.Count == 2);
            Assert.IsTrue(featuresWithin100KmFromRome.Any(ft => ft.Equals(new RDFResource("ex:romeFeat"))));
            Assert.IsTrue(featuresWithin100KmFromRome.Any(ft => ft.Equals(new RDFResource("ex:tivoliFeat"))));
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
            List<RDFResource> featuresWithin100KmFromRome = geoOntology.SpatialHelper.GetFeaturesNearPoint((9.06462722, 45.47551406), 10000); //10Km around Settimo Milanese

            Assert.IsNotNull(featuresWithin100KmFromRome);
            Assert.IsTrue(featuresWithin100KmFromRome.Count == 1);
            Assert.IsTrue(featuresWithin100KmFromRome.Any(ft => ft.Equals(new RDFResource("ex:milanFeat"))));
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
            List<RDFResource> featuresWithin100KmFromRome = geoOntology.SpatialHelper.GetFeaturesNorthOfPoint((9.03879405, 44.45787556)); //Genoa

            Assert.IsNotNull(featuresWithin100KmFromRome);
            Assert.IsTrue(featuresWithin100KmFromRome.Count == 1);
            Assert.IsTrue(featuresWithin100KmFromRome.Any(ft => ft.Equals(new RDFResource("ex:milanFeat"))));
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
            List<RDFResource> featuresWithin100KmFromRome = geoOntology.SpatialHelper.GetFeaturesNorthOfPoint((9.03879405, 44.45787556)); //Genoa

            Assert.IsNotNull(featuresWithin100KmFromRome);
            Assert.IsTrue(featuresWithin100KmFromRome.Count == 1);
            Assert.IsTrue(featuresWithin100KmFromRome.Any(ft => ft.Equals(new RDFResource("ex:milanFeat"))));
        }

        [TestMethod]
        public void ShouldNotGetFeaturesNorthOfPointFromWKT()
        {
            GEOOntology geoOntology = new GEOOntology("ex:geoOnt");
            geoOntology.DeclarePointFeature(new RDFResource("ex:milanFeat"), new RDFResource("ex:milanGeom"), (9.188540, 45.464664), true);
            geoOntology.DeclarePointFeature(new RDFResource("ex:romeFeat"), new RDFResource("ex:romeDefGeom"), (12.496365, 41.902782), true);
            geoOntology.DeclarePointFeature(new RDFResource("ex:romeFeat"), new RDFResource("ex:romeSecGeom"), (12.49221871, 41.89033014), false);
            geoOntology.DeclarePointFeature(new RDFResource("ex:tivoliFeat"), new RDFResource("ex:tivoliDefGeom"), (12.79938661, 41.96217718), true);
            List<RDFResource> featuresWithin100KmFromRome = geoOntology.SpatialHelper.GetFeaturesNorthOfPoint((10.20883090, 45.56077293)); //Brescia

            Assert.IsNotNull(featuresWithin100KmFromRome);
            Assert.IsTrue(featuresWithin100KmFromRome.Count == 0);
        }

        [TestMethod]
        public void ShouldNotGetFeaturesNorthOfPointBecauseMissingGeometries()
        {
            GEOOntology geoOntology = new GEOOntology("ex:geoOnt");
            geoOntology.Data.DeclareIndividual(new RDFResource("ex:milanFeat"));
            geoOntology.Data.DeclareIndividualType(new RDFResource("ex:milanFeat"), RDFVocabulary.GEOSPARQL.FEATURE);
            List<RDFResource> featuresWithin100KmFromRome = geoOntology.SpatialHelper.GetFeaturesNorthOfPoint((11.53860090, 45.54896859));

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
        public void ShouldGetFeaturesSouthOfPointFromWKT()
        {
            GEOOntology geoOntology = new GEOOntology("ex:geoOnt");
            geoOntology.DeclarePointFeature(new RDFResource("ex:milanFeat"), new RDFResource("ex:milanGeom"), (9.188540, 45.464664), true);
            geoOntology.DeclarePointFeature(new RDFResource("ex:romeFeat"), new RDFResource("ex:romeDefGeom"), (12.496365, 41.902782), true);
            geoOntology.DeclarePointFeature(new RDFResource("ex:romeFeat"), new RDFResource("ex:romeSecGeom"), (12.49221871, 41.89033014), false);
            geoOntology.DeclarePointFeature(new RDFResource("ex:tivoliFeat"), new RDFResource("ex:tivoliDefGeom"), (12.79938661, 41.96217718), true);
            List<RDFResource> featuresWithin100KmFromRome = geoOntology.SpatialHelper.GetFeaturesSouthOfPoint((9.03879405, 44.45787556)); //Genoa

            Assert.IsNotNull(featuresWithin100KmFromRome);
            Assert.IsTrue(featuresWithin100KmFromRome.Count == 2);
            Assert.IsTrue(featuresWithin100KmFromRome.Any(ft => ft.Equals(new RDFResource("ex:romeFeat"))));
            Assert.IsTrue(featuresWithin100KmFromRome.Any(ft => ft.Equals(new RDFResource("ex:tivoliFeat"))));
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
            List<RDFResource> featuresWithin100KmFromRome = geoOntology.SpatialHelper.GetFeaturesSouthOfPoint((9.03879405, 44.45787556)); //Genoa

            Assert.IsNotNull(featuresWithin100KmFromRome);
            Assert.IsTrue(featuresWithin100KmFromRome.Count == 2);
            Assert.IsTrue(featuresWithin100KmFromRome.Any(ft => ft.Equals(new RDFResource("ex:romeFeat"))));
            Assert.IsTrue(featuresWithin100KmFromRome.Any(ft => ft.Equals(new RDFResource("ex:tivoliFeat"))));
        }

        [TestMethod]
        public void ShouldNotGetFeaturesSouthOfPointFromWKT()
        {
            GEOOntology geoOntology = new GEOOntology("ex:geoOnt");
            geoOntology.DeclarePointFeature(new RDFResource("ex:milanFeat"), new RDFResource("ex:milanGeom"), (9.188540, 45.464664), true);
            geoOntology.DeclarePointFeature(new RDFResource("ex:romeFeat"), new RDFResource("ex:romeDefGeom"), (12.496365, 41.902782), true);
            geoOntology.DeclarePointFeature(new RDFResource("ex:romeFeat"), new RDFResource("ex:romeSecGeom"), (12.49221871, 41.89033014), false);
            geoOntology.DeclarePointFeature(new RDFResource("ex:tivoliFeat"), new RDFResource("ex:tivoliDefGeom"), (12.79938661, 41.96217718), true);
            List<RDFResource> featuresWithin100KmFromRome = geoOntology.SpatialHelper.GetFeaturesSouthOfPoint((15.80512362, 40.64259592)); //Potenza

            Assert.IsNotNull(featuresWithin100KmFromRome);
            Assert.IsTrue(featuresWithin100KmFromRome.Count == 0);
        }

        [TestMethod]
        public void ShouldNotGetFeaturesSouthOfPointBecauseMissingGeometries()
        {
            GEOOntology geoOntology = new GEOOntology("ex:geoOnt");
            geoOntology.Data.DeclareIndividual(new RDFResource("ex:milanFeat"));
            geoOntology.Data.DeclareIndividualType(new RDFResource("ex:milanFeat"), RDFVocabulary.GEOSPARQL.FEATURE);
            List<RDFResource> featuresWithin100KmFromRome = geoOntology.SpatialHelper.GetFeaturesSouthOfPoint((11.53860090, 45.54896859));

            Assert.IsNotNull(featuresWithin100KmFromRome);
            Assert.IsTrue(featuresWithin100KmFromRome.Count == 0);
        }

        [TestMethod]
        public void ShouldThrowExceptionOnGettingFeaturesSouthOfPointBecauseInvalidLongitude()
            => Assert.ThrowsException<OWLSemanticsException>(() => new GEOOntology("ex:geoOnt").SpatialHelper.GetFeaturesSouthOfPoint((-181, 45)));

        [TestMethod]
        public void ShouldThrowExceptionOnGettingFeaturesSouthOfPointBecauseInvalidLatitude()
            => Assert.ThrowsException<OWLSemanticsException>(() => new GEOOntology("ex:geoOnt").SpatialHelper.GetFeaturesSouthOfPoint((9, 91)));

        [TestMethod]
        public void ShouldGetFeaturesWithinBoxFromWKT()
        {
            GEOOntology geoOntology = new GEOOntology("ex:geoOnt");
            geoOntology.DeclarePointFeature(new RDFResource("ex:milanFeat"), new RDFResource("ex:milanGeom"), (9.188540, 45.464664), true);
            geoOntology.DeclarePointFeature(new RDFResource("ex:romeFeat"), new RDFResource("ex:romeDefGeom"), (12.496365, 41.902782), true);
            geoOntology.DeclarePointFeature(new RDFResource("ex:romeFeat"), new RDFResource("ex:romeSecGeom"), (12.49221871, 41.89033014), false);
            geoOntology.DeclarePointFeature(new RDFResource("ex:tivoliFeat"), new RDFResource("ex:tivoliDefGeom"), (12.79938661, 41.96217718), true);
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
            geoOntology.DeclarePointFeature(new RDFResource("ex:milanFeat"), new RDFResource("ex:milanGeom"), (9.188540, 45.464664), true);
            geoOntology.DeclarePointFeature(new RDFResource("ex:romeFeat"), new RDFResource("ex:romeDefGeom"), (12.496365, 41.902782), true);
            geoOntology.DeclarePointFeature(new RDFResource("ex:romeFeat"), new RDFResource("ex:romeSecGeom"), (12.49221871, 41.89033014), false);
            geoOntology.DeclarePointFeature(new RDFResource("ex:tivoliFeat"), new RDFResource("ex:tivoliDefGeom"), (12.79938661, 41.96217718), true);
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
            geoOntology.DeclarePointFeature(new RDFResource("ex:milanFeat"), new RDFResource("ex:milanGeom"), (9.188540, 45.464664), true);
            geoOntology.DeclarePointFeature(new RDFResource("ex:romeFeat"), new RDFResource("ex:romeDefGeom"), (12.496365, 41.902782), true);
            geoOntology.DeclarePointFeature(new RDFResource("ex:romeFeat"), new RDFResource("ex:romeSecGeom"), (12.49221871, 41.89033014), false);
            geoOntology.DeclarePointFeature(new RDFResource("ex:tivoliFeat"), new RDFResource("ex:tivoliDefGeom"), (12.79938661, 41.96217718), true);
            List<RDFResource> featuresWithinSearchBox = geoOntology.SpatialHelper.GetFeaturesWithinBox((12.50687563, 41.67714954), (12.67853701, 41.80728360)); //Pomezia-Frascati

            Assert.IsNotNull(featuresWithinSearchBox);
            Assert.IsTrue(featuresWithinSearchBox.Count == 0);
        }

        [TestMethod]
        public void ShouldNotGetFeaturesWithinBoxBecauseMissingGeometries()
        {
            GEOOntology geoOntology = new GEOOntology("ex:geoOnt");
            geoOntology.Data.DeclareIndividual(new RDFResource("ex:milanFeat"));
            geoOntology.Data.DeclareIndividualType(new RDFResource("ex:milanFeat"), RDFVocabulary.GEOSPARQL.FEATURE);
            List<RDFResource> featuresWithin100KmFromRome = geoOntology.SpatialHelper.GetFeaturesWithinBox((12.50687563, 41.67714954), (12.67853701, 41.80728360)); //Pomezia-Frascati

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