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
        #region Methods
        /// <summary>
        /// Projects the given WGS84 geometry to an equivalent UTM geometry
        /// </summary>
        internal static Geometry GetUTMGeometryFromWGS84(Geometry wgs84Geometry, (int,bool) utmZone)
        {
            ICoordinateTransformation coordinateTransformation = new CoordinateTransformationFactory().CreateFromCoordinateSystems(
                GeographicCoordinateSystem.WGS84, ProjectedCoordinateSystem.WGS84_UTM(utmZone.Item1, utmZone.Item2));
            Geometry utmGeometry = Transform(wgs84Geometry, coordinateTransformation.MathTransform);
            utmGeometry.SRID = Convert.ToInt32(coordinateTransformation.TargetCS.AuthorityCode);
            return utmGeometry;
        }

        /// <summary>
        /// Projects the given UTM geometry to an equivalent WGS84 geometry
        /// </summary>
        internal static Geometry GetWGS84GeometryFromUTM(Geometry utmGeometry, (int, bool) utmZone)
        {
            ICoordinateTransformation coordinateTransformation = new CoordinateTransformationFactory().CreateFromCoordinateSystems(
                ProjectedCoordinateSystem.WGS84_UTM(utmZone.Item1, utmZone.Item2), GeographicCoordinateSystem.WGS84);
            Geometry wgs84Geometry = Transform(utmGeometry, coordinateTransformation.MathTransform);
            wgs84Geometry.SRID = Convert.ToInt32(coordinateTransformation.TargetCS.AuthorityCode);
            return wgs84Geometry;
        }

        /// <summary>
        /// Determines the UTM zone to which the given WGS84 lon/lat coordinates belong
        /// </summary>
        internal static (int,bool) GetUTMZoneFromWGS84Coordinates(double wgs84Longitude, double wgs84Latitude)
        {
            bool isNorth = wgs84Latitude >= 0;

            // Special Cases for Norway & Svalbard
            if (wgs84Latitude > 55 && wgs84Latitude < 64 && wgs84Longitude > 2 && wgs84Longitude < 6)
                return (32, isNorth);
            if (wgs84Latitude > 71 && wgs84Longitude >= 6 && wgs84Longitude < 9)
                return (31, isNorth);
            if (wgs84Latitude > 71 && ((wgs84Longitude >= 9 && wgs84Longitude < 12) || (wgs84Longitude >= 18 && wgs84Longitude < 21)))
                return (33, isNorth);
            if (wgs84Latitude > 71 && ((wgs84Longitude >= 21 && wgs84Longitude < 24) || (wgs84Longitude >= 30 && wgs84Longitude < 33)))
                return (35, isNorth);

            // Rest of the world
            int utmZone = (int)(Math.Floor((wgs84Longitude + 180) / 6) % 60) + 1;
            return (utmZone, isNorth);
        }
        #endregion

        #region Utilities
        private static Geometry Transform(Geometry geometry, MathTransform mathTransform)
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
                coordinateSequence.SetX(index, Math.Round(x, 15));
                coordinateSequence.SetY(index, Math.Round(y, 15));
                coordinateSequence.SetZ(index, Math.Round(z, 15));
            }
        }
        #endregion
    }
}