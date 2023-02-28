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
using System.Collections.Generic;

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
        internal static CoordinateSystem LambertAzimutalEAWGS84 = new CoordinateSystemFactory().CreateFromWkt(
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
        /// Determines if the coordinates of the given WGS84 geometry fits single UTM zones
        /// </summary>
        internal static bool CheckWGS84GeometryFitsSingleUTMZone(Geometry wgs84Geometry)
        {
            HashSet<(int, bool)> wgs84CoordinateUTMZones = new HashSet<(int,bool)>();
            foreach (Coordinate wgs84Coordinate in wgs84Geometry.Coordinates)
                wgs84CoordinateUTMZones.Add(GetUTMZoneFromWGS84Coordinates(wgs84Coordinate.X, wgs84Coordinate.Y));
            return wgs84CoordinateUTMZones.Count == 1;
        }

        /// <summary>
        /// Projects the given WGS84 geometry to an equivalent UTM/Lambert geometry
        /// </summary>
        internal static Geometry GetUTMGeometryFromWGS84(Geometry wgs84Geometry, (int,bool) utmZone)
        {
            //Depending if the given WGS84 geometry fits within one UTM zone or spans over multiple ones,
            //we may decide to project to Lambert Azimuthal EA WGS84 (which handles these scenarios better)
            bool wgs84GeometryFitsSingleUTMZone = CheckWGS84GeometryFitsSingleUTMZone(wgs84Geometry);
            ICoordinateTransformation coordinateTransformation = new CoordinateTransformationFactory().CreateFromCoordinateSystems(
                GeographicCoordinateSystem.WGS84, 
                wgs84GeometryFitsSingleUTMZone ? ProjectedCoordinateSystem.WGS84_UTM(utmZone.Item1, utmZone.Item2) 
                                               : LambertAzimutalEAWGS84);
            Geometry utmGeometry = Transform(wgs84Geometry, coordinateTransformation.MathTransform);
            utmGeometry.SRID = Convert.ToInt32(coordinateTransformation.TargetCS.AuthorityCode);
            return utmGeometry;
        }

        /// <summary>
        /// Projects the given UTM/Lambert geometry to an equivalent WGS84 geometry
        /// </summary>
        internal static Geometry GetWGS84GeometryFromUTM(Geometry projectedGeometry, (int,bool) utmZone)
        {
            ICoordinateTransformation coordinateTransformation = new CoordinateTransformationFactory().CreateFromCoordinateSystems(
                projectedGeometry.SRID == 42106 ? LambertAzimutalEAWGS84 
                                                : ProjectedCoordinateSystem.WGS84_UTM(utmZone.Item1, utmZone.Item2), GeographicCoordinateSystem.WGS84);
            Geometry wgs84Geometry = Transform(projectedGeometry, coordinateTransformation.MathTransform);
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
                _mathTransform.Transform(ref x, ref y);
                coordinateSequence.SetX(index, Math.Round(x, 8));
                coordinateSequence.SetY(index, Math.Round(y, 8));
            }
        }
        #endregion
    }
}