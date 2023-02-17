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
    public class GEOConverterTest
    {
        #region Tests
        [TestMethod]
        public void ShouldGetUTMGeometryFromWGS84()
        {
            GEOOntology geoOnt = new GEOOntology("ex:geoOnt");
            geoOnt.DeclarePoint(new RDFResource("ex:Milan"), 9.188540, 45.464664);
            Geometry utmGeometry = GEOConverter.GetUTMGeometryFromWGS84(geoOnt.Geometries["ex:Milan"].Item1, (32, true));

            Assert.IsNotNull(utmGeometry);
            Assert.IsTrue(utmGeometry.EqualsTopologically(geoOnt.Geometries["ex:Milan"].Item2));
        }

        [TestMethod]
        public void ShouldGetWGS84GeometryFromUTM()
        {
            GEOOntology geoOnt = new GEOOntology("ex:geoOnt");
            geoOnt.DeclarePoint(new RDFResource("ex:Milan"), 9.188540, 45.464664);
            Geometry wgs84Geometry = GEOConverter.GetWGS84GeometryFromUTM(geoOnt.Geometries["ex:Milan"].Item2, (32, true));

            Assert.IsNotNull(wgs84Geometry);
            //Must test this way since IEEE754 floating-point errors makes EqualsTopologically strictly unfeasible
            Assert.IsTrue(wgs84Geometry.Coordinate.X >= 9.188540 && wgs84Geometry.Coordinate.X <= 9.188541);
            Assert.IsTrue(wgs84Geometry.Coordinate.Y >= 45.464664 && wgs84Geometry.Coordinate.Y <= 45.464665);
        }

        [TestMethod]
        public void ShouldGetUTMZoneFromWGS84Coordinates()
        {
            (int,bool) utmZone = GEOConverter.GetUTMZoneFromWGS84Coordinates(9.188540, 45.464664);

            Assert.IsTrue(utmZone.Equals((32, true)));
        }

        [TestMethod]
        public void ShouldGetUTMZoneFromWGS84CoordinatesSouth()
        {
            (int, bool) utmZone = GEOConverter.GetUTMZoneFromWGS84Coordinates(9.188540, -45.464664);

            Assert.IsTrue(utmZone.Equals((32, false)));
        }

        [TestMethod]
        public void ShouldGetUTMZoneFromWGS84CoordinatesNorwaySvalbardCase1()
        {
            (int, bool) utmZone = GEOConverter.GetUTMZoneFromWGS84Coordinates(4.188540, 58.464664);

            Assert.IsTrue(utmZone.Equals((32, true)));
        }

        [TestMethod]
        public void ShouldGetUTMZoneFromWGS84CoordinatesNorwaySvalbardCase2()
        {
            (int, bool) utmZone = GEOConverter.GetUTMZoneFromWGS84Coordinates(8.188540, 73.464664);

            Assert.IsTrue(utmZone.Equals((31, true)));
        }

        [TestMethod]
        public void ShouldGetUTMZoneFromWGS84CoordinatesNorwaySvalbardCase3()
        {
            (int, bool) utmZone = GEOConverter.GetUTMZoneFromWGS84Coordinates(11.188540, 73.464664);

            Assert.IsTrue(utmZone.Equals((33, true)));
        }

        [TestMethod]
        public void ShouldGetUTMZoneFromWGS84CoordinatesNorwaySvalbardCase4()
        {
            (int, bool) utmZone = GEOConverter.GetUTMZoneFromWGS84Coordinates(22.188540, 73.464664);

            Assert.IsTrue(utmZone.Equals((35, true)));
        }
        #endregion
    }
}