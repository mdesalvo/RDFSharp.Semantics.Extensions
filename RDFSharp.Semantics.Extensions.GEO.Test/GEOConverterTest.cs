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
        public void ShouldGetLAZGeometryFromWGS84()
        {
            Geometry milanWGS84 = new Point(9.188540, 45.464664);
            Geometry milanLAZ = GEOConverter.GetLambertAzimuthalGeometryFromWGS84(milanWGS84);

            Assert.IsNotNull(milanLAZ);
            Assert.IsTrue(string.Equals(milanLAZ.ToText(), "POINT (-899167.1609069 4413535.43188373)"));
            Assert.IsTrue(milanLAZ.SRID == 42106);
        }

        [TestMethod]
        public void ShouldGetWGS84GeometryFromLAZ()
        {
            Geometry milanLAZ = GEOConverter.GetLambertAzimuthalGeometryFromWGS84(new Point(9.188540, 45.464664));
            Geometry milanWGS84 = GEOConverter.GetWGS84GeometryFromLambertAzimuthal(milanLAZ);

            Assert.IsNotNull(milanWGS84);
            Assert.IsTrue(string.Equals(milanWGS84.ToText(), "POINT (9.18854 45.464664)"));
            Assert.IsTrue(milanWGS84.SRID == 4326);
        }
        #endregion
    }
}