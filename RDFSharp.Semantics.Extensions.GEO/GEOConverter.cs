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

using NetTopologySuite.Geometries;
using ProjNet.CoordinateSystems;
using ProjNet.CoordinateSystems.Transformations;
using System;

namespace RDFSharp.Semantics.Extensions.GEO
{
    /// <summary>
    /// GEOConverter is an helper for converting geometries between different coordinate systems
    /// </summary>
    internal static class GEOConverter
    {
        #region Properties
        /// <summary>
        /// Projected system used when coordinates of geometries span across multiple UTM zones
        /// </summary>
        internal static CoordinateSystem LambertAzimutalWGS84 = new CoordinateSystemFactory().CreateFromWkt(
@"PROJCS[""WGS84 / Lambert Azim Mozambique"",
    GEOGCS[""WGS 84"",
        DATUM[""WGS_1984"",
            SPHEROID[""WGS_1984"",6378137.0,298.257223563]],
        PRIMEM[""Greenwich"",0.0],
        UNIT[""degree"",0.017453292519943295],
        AXIS[""Longitude"",EAST],
        AXIS[""Latitude"",NORTH]],
    PROJECTION[""Lambert_Azimuthal_Equal_Area""],
    PARAMETER[""latitude_of_center"",5.0],
    PARAMETER[""longitude_of_center"",20.0],
    PARAMETER[""false_easting"",0.0],
    PARAMETER[""false_northing"",0.0],
    UNIT[""m"",1.0],
    AXIS[""x"",EAST],
    AXIS[""y"",NORTH],
    AUTHORITY[""EPSG"",""42106""]]");
        #endregion

        #region Methods
        /// <summary>
        /// Projects the given WGS84 geometry to an equivalent Lambert Azimuthal geometry
        /// </summary>
        internal static Geometry GetLambertAzimuthalGeometryFromWGS84(Geometry wgs84Geometry)
        {
            ICoordinateTransformation coordinateTransformation = new CoordinateTransformationFactory().CreateFromCoordinateSystems(
                GeographicCoordinateSystem.WGS84, LambertAzimutalWGS84);
            Geometry lazGeometry = TransformGeometry(wgs84Geometry, coordinateTransformation.MathTransform);
            lazGeometry.SRID = Convert.ToInt32(coordinateTransformation.TargetCS.AuthorityCode);
            return lazGeometry;
        }

        /// <summary>
        /// Projects the given Lambert Azimuthal geometry to an equivalent WGS84 geometry
        /// </summary>
        internal static Geometry GetWGS84GeometryFromLambertAzimuthal(Geometry projectedGeometry)
        {
            ICoordinateTransformation coordinateTransformation = new CoordinateTransformationFactory().CreateFromCoordinateSystems(
                LambertAzimutalWGS84, GeographicCoordinateSystem.WGS84);
            Geometry wgs84Geometry = TransformGeometry(projectedGeometry, coordinateTransformation.MathTransform);
            wgs84Geometry.SRID = Convert.ToInt32(coordinateTransformation.TargetCS.AuthorityCode);
            return wgs84Geometry;
        }
        #endregion

        #region Utilities
        private static Geometry TransformGeometry(Geometry geometry, MathTransform mathTransform)
        {
            geometry = geometry.Copy();
            geometry.Apply(new MathTransformFilter(mathTransform));
            return geometry;
        }

        private sealed class MathTransformFilter : ICoordinateSequenceFilter
        {
            private readonly MathTransform _mathTransform;
            public MathTransformFilter(MathTransform mathTransform) => _mathTransform = mathTransform;
            public bool Done => false;
            public bool GeometryChanged => true;

            public void Filter(CoordinateSequence coordinateSequence, int index)
            {
                double x = coordinateSequence.GetX(index);
                double y = coordinateSequence.GetY(index);
                double z = coordinateSequence.GetZ(index);
                _mathTransform.Transform(ref x, ref y, ref z);
                coordinateSequence.SetX(index, Math.Round(x, 8));
                coordinateSequence.SetY(index, Math.Round(y, 8));
                coordinateSequence.SetZ(index, Math.Round(z, 8));
            }
        }
        #endregion
    }
}