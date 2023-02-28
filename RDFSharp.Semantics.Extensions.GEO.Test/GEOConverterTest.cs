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

namespace RDFSharp.Semantics.Extensions.GEO.Test
{
    [TestClass]
    public class GEOConverterTest
    {
        #region Tests
        [TestMethod]
        public void CheckWGS84GeometryFitsSingleUTMZone()
        {
            Geometry milanWGS84 = new Point(9.188540, 45.464664);
            bool milanFitsUTM = GEOConverter.CheckWGS84GeometryFitsSingleUTMZone(milanWGS84);

            Assert.IsTrue(milanFitsUTM);
        }

        [TestMethod]
        public void ShouldCheckWGS84GeometryDoesNotFitSingleUTMZone()
        {
            Geometry northItalyLineWGS84 = new LineString(new Coordinate[] {
                new Coordinate(9.18801422,  45.47725289),
                new Coordinate(10.01198883, 45.14502743),
                new Coordinate(10.95681305, 45.43872208),
                new Coordinate(11.90163727, 45.41559096),
                new Coordinate(13.19802399, 46.09016569)
            });
            bool northItalyLineWGS84FitsUTM = GEOConverter.CheckWGS84GeometryFitsSingleUTMZone(northItalyLineWGS84);

            Assert.IsFalse(northItalyLineWGS84FitsUTM);
        }

        [TestMethod]
        public void ShouldGetUTMGeometryFromWGS84()
        {
            Geometry milanWGS84 = new Point(9.188540, 45.464664);
            Geometry milanUTM32N = GEOConverter.GetUTMGeometryFromWGS84(milanWGS84, (32, true));

            Assert.IsNotNull(milanUTM32N);
            Assert.IsTrue(string.Equals(milanUTM32N.ToText(), "POINT (514739.23764243 5034588.07621425)"));
            Assert.IsTrue(milanUTM32N.SRID == 32632);
        }

        [TestMethod]
        public void ShouldGetWGS84GeometryFromUTM()
        {
            Geometry milanUTM32N = GEOConverter.GetUTMGeometryFromWGS84(new Point(9.188540, 45.464664), (32, true));
            Geometry milanWGS84 = GEOConverter.GetWGS84GeometryFromUTM(milanUTM32N, (32, true));

            Assert.IsNotNull(milanWGS84);
            Assert.IsTrue(string.Equals(milanWGS84.ToText(), "POINT (9.18854 45.464664)"));
            Assert.IsTrue(milanWGS84.SRID == 4326);
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