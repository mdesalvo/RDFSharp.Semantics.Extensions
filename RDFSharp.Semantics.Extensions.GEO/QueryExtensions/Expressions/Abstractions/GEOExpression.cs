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
using NetTopologySuite.IO;
using NetTopologySuite.IO.GML2;
using NetTopologySuite.IO.GML3;
using RDFSharp.Model;
using RDFSharp.Query;
using System;
using System.Data;
using System.Globalization;

namespace RDFSharp.Semantics.Extensions.GEO
{
    /// <summary>
    /// GEOExpression represents a geographic expression to be applied on a query results table.
    /// </summary>
    public abstract class GEOExpression : RDFExpression
    {
        #region Properties
        /// <summary>
        /// Reader for WKT spatial representation
        /// </summary>
        protected WKTReader WKTReader { get; set; }

        /// <summary>
        /// Writer for WKT spatial representation
        /// </summary>
        protected WKTWriter WKTWriter { get; set; }

        /// <summary>
        /// Reader for GML spatial representation
        /// </summary>
        protected GMLReader GMLReader { get; set; }

        /// <summary>
        /// Writer for GML spatial representation
        /// </summary>
        protected GMLWriter GMLWriter { get; set; }

        /// <summary>
        /// Answers if this is a geographic expression using right argument
        /// </summary>
        protected bool HasRightArgument => RightArgument != null;
        #endregion

        #region Ctors
        /// <summary>
        /// Default-ctor to build a geographic expression with given arguments
        /// </summary>
        public GEOExpression(RDFExpressionArgument leftArgument, RDFExpressionArgument rightArgument)
            : base(leftArgument, rightArgument)
        {
            WKTReader = new WKTReader();
            WKTWriter = new WKTWriter();
            GMLReader = new GMLReader();
            GMLWriter = new GML3Writer(); 
        }
        #endregion

        #region Methods
        /// <summary>
        /// Applies the geographic expression on the given datarow
        /// </summary>
        internal override RDFPatternMember ApplyExpression(DataRow row)
        {
            RDFTypedLiteral expressionResult = null;

            #region Guards
            if (LeftArgument is RDFVariable && !row.Table.Columns.Contains(LeftArgument.ToString()))
                return expressionResult;
            if (RightArgument is RDFVariable && !row.Table.Columns.Contains(RightArgument.ToString()))
                return expressionResult;
            #endregion

            try
            {
                #region Evaluate Arguments
                //Evaluate left argument (Expression VS Variable)
                RDFPatternMember leftArgumentPMember = null;
                if (LeftArgument is RDFExpression leftArgumentExpression)
                    leftArgumentPMember = leftArgumentExpression.ApplyExpression(row);
                else
                    leftArgumentPMember = RDFQueryUtilities.ParseRDFPatternMember(row[LeftArgument.ToString()].ToString());

                //Evaluate right argument (Expression VS Variable VS TypedLiteral)
                RDFPatternMember rightArgumentPMember = null;
                if (RightArgument is RDFExpression rightArgumentExpression)
                    rightArgumentPMember = rightArgumentExpression.ApplyExpression(row);
                else if (RightArgument is RDFVariable)
                    rightArgumentPMember = RDFQueryUtilities.ParseRDFPatternMember(row[RightArgument.ToString()].ToString());
                else
                    rightArgumentPMember = (RDFTypedLiteral)RightArgument;
                #endregion

                #region Calculate Result
                Geometry leftGeometry = null;
                Geometry leftGeometryUTM = null;
                Geometry rightGeometry = null;
                Geometry rightGeometryUTM = null;
                if (leftArgumentPMember is RDFTypedLiteral leftArgumentTypedLiteral
                     && (leftArgumentTypedLiteral.Datatype.Equals(RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT) || 
                          leftArgumentTypedLiteral.Datatype.Equals(RDFModelEnums.RDFDatatypes.GEOSPARQL_GML)))
                {
                    //Parse WGS84 WKT/GML left geometry
                    leftGeometry = leftArgumentTypedLiteral.Datatype.Equals(RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT) ?
                        WKTReader.Read(leftArgumentTypedLiteral.Value) : GMLReader.Read(leftArgumentTypedLiteral.Value);
                    leftGeometry.SRID = 4326;

                    //Short-circuit empty geometry evaluation
                    if (this is GEOIsEmptyExpression)
                        return leftGeometry.IsEmpty ? RDFTypedLiteral.True : RDFTypedLiteral.False;

                    //Project left geometry from WGS84 to UTM
                    (int, bool) utmZoneOfLeftGeometry = GEOConverter.GetUTMZoneFromWGS84Coordinates(leftGeometry.Coordinates[0].X, leftGeometry.Coordinates[0].Y);
                    leftGeometryUTM = GEOConverter.GetUTMGeometryFromWGS84(leftGeometry, utmZoneOfLeftGeometry);

                    //Determine if requested GEO function needs right geometry
                    if (HasRightArgument)
                    {
                        //If so, it must be a well-formed GEO literal (WKT/GML)
                        if (rightArgumentPMember is RDFTypedLiteral rightArgumentTypedLiteral
                             && (rightArgumentTypedLiteral.Datatype.Equals(RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT) 
                                  || rightArgumentTypedLiteral.Datatype.Equals(RDFModelEnums.RDFDatatypes.GEOSPARQL_GML)))
                        {
                            //Parse WGS84 WKT/GML right geometry
                            rightGeometry = rightArgumentTypedLiteral.Datatype.Equals(RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT) ?
                                WKTReader.Read(rightArgumentTypedLiteral.Value) : GMLReader.Read(rightArgumentTypedLiteral.Value);
                            rightGeometry.SRID = 4326;

                            //Project right geometry from WGS84 to UTM
                            (int, bool) utmZoneOfRightGeometry = GEOConverter.GetUTMZoneFromWGS84Coordinates(rightGeometry.Coordinates[0].X, rightGeometry.Coordinates[0].Y);
                            rightGeometryUTM = GEOConverter.GetUTMGeometryFromWGS84(rightGeometry, utmZoneOfRightGeometry);
                        }

                        //Otherwise, return null to signal binding error
                        else
                            return expressionResult;
                    }

                    //Execute GEO functions on UTM geometries
                    if (this is GEOBoundaryExpression)
                    {
                        Geometry boundaryGeometryUTM = leftGeometryUTM.Boundary;
                        Geometry boundaryGeometryWGS84 = GEOConverter.GetWGS84GeometryFromUTM(boundaryGeometryUTM, utmZoneOfLeftGeometry);
                        expressionResult = new RDFTypedLiteral(WKTWriter.Write(boundaryGeometryWGS84), RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT);
                    }
                    else if (this is GEOBufferExpression geoBufferExpression)
                    {
                        Geometry bufferGeometryUTM = leftGeometryUTM.Buffer(geoBufferExpression.BufferMeters);
                        Geometry bufferGeometryWGS84 = GEOConverter.GetWGS84GeometryFromUTM(bufferGeometryUTM, utmZoneOfLeftGeometry);
                        expressionResult = new RDFTypedLiteral(WKTWriter.Write(bufferGeometryWGS84), RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT);
                    }
                    else if (this is GEOContainsExpression)
                    {
                        bool sfContains = leftGeometryUTM.Contains(rightGeometryUTM);
                        expressionResult = sfContains ? RDFTypedLiteral.True : RDFTypedLiteral.False;
                    }
                    else if (this is GEOConvexHullExpression)
                    {
                        Geometry convexHullGeometryUTM = leftGeometryUTM.ConvexHull();
                        Geometry convexHullGeometryWGS84 = GEOConverter.GetWGS84GeometryFromUTM(convexHullGeometryUTM, utmZoneOfLeftGeometry);
                        expressionResult = new RDFTypedLiteral(WKTWriter.Write(convexHullGeometryWGS84), RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT);
                    }
                    else if (this is GEOCrossesExpression)
                    {
                        bool sfCrosses = leftGeometryUTM.Crosses(rightGeometryUTM);
                        expressionResult = sfCrosses ? RDFTypedLiteral.True : RDFTypedLiteral.False;
                    }
                    else if (this is GEODifferenceExpression)
                    {
                        Geometry differenceGeometryUTM = leftGeometryUTM.Difference(rightGeometryUTM);
                        Geometry differenceGeometryWGS84 = GEOConverter.GetWGS84GeometryFromUTM(differenceGeometryUTM, utmZoneOfLeftGeometry);
                        expressionResult = new RDFTypedLiteral(WKTWriter.Write(differenceGeometryWGS84), RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT);
                    }
                    else if (this is GEODimensionExpression)
                    {
                        int geosparqlDimension = (int)leftGeometryUTM.Dimension;
                        expressionResult = new RDFTypedLiteral(Convert.ToString(geosparqlDimension, CultureInfo.InvariantCulture), RDFModelEnums.RDFDatatypes.XSD_INTEGER);
                    }
                    else if (this is GEODisjointExpression)
                    {
                        bool sfDisjoint = leftGeometryUTM.Disjoint(rightGeometryUTM);
                        expressionResult = sfDisjoint ? RDFTypedLiteral.True : RDFTypedLiteral.False;
                    }
                    else if (this is GEODistanceExpression)
                    {
                        double distanceMeters = leftGeometryUTM.Distance(rightGeometryUTM);
                        expressionResult = new RDFTypedLiteral(Convert.ToString(distanceMeters, CultureInfo.InvariantCulture), RDFModelEnums.RDFDatatypes.XSD_DOUBLE);
                    }
                    else if (this is GEOEgenhoferExpression geoEgenhoferExpression)
                    {
                        bool sfEgenhoferRelate = false;
                        switch (geoEgenhoferExpression.EgenhoferRelation)
                        {
                            case GEOEnums.GEOEgenhoferRelations.Contains:
                                sfEgenhoferRelate = leftGeometryUTM.Relate(rightGeometryUTM, "T*TFF*FF*");
                                break;
                            case GEOEnums.GEOEgenhoferRelations.CoveredBy:
                                sfEgenhoferRelate = leftGeometryUTM.Relate(rightGeometryUTM, "TFF*TFT**");
                                break;
                            case GEOEnums.GEOEgenhoferRelations.Covers:
                                sfEgenhoferRelate = leftGeometryUTM.Relate(rightGeometryUTM, "T*TFT*FF*");
                                break;
                            case GEOEnums.GEOEgenhoferRelations.Disjoint:
                                sfEgenhoferRelate = leftGeometryUTM.Relate(rightGeometryUTM, "FF*FF****");
                                break;
                            case GEOEnums.GEOEgenhoferRelations.Equals:
                                sfEgenhoferRelate = leftGeometryUTM.Relate(rightGeometryUTM, "TFFFTFFFT");
                                break;
                            case GEOEnums.GEOEgenhoferRelations.Inside:
                                sfEgenhoferRelate = leftGeometryUTM.Relate(rightGeometryUTM, "TFF*FFT**");
                                break;
                            case GEOEnums.GEOEgenhoferRelations.Meet:
                                sfEgenhoferRelate = leftGeometryUTM.Relate(rightGeometryUTM, "FT*******")
                                                     || leftGeometryUTM.Relate(rightGeometryUTM, "F**T*****")
                                                      || leftGeometryUTM.Relate(rightGeometryUTM, "F***T****");
                                break;
                            case GEOEnums.GEOEgenhoferRelations.Overlap:
                                sfEgenhoferRelate = leftGeometryUTM.Relate(rightGeometryUTM, "T*T***T**");
                                break;
                        }
                        expressionResult = sfEgenhoferRelate ? RDFTypedLiteral.True : RDFTypedLiteral.False;
                    }
                    else if (this is GEOEnvelopeExpression)
                    {
                        Geometry envelopeGeometryUTM = leftGeometryUTM.Envelope;
                        Geometry envelopeGeometryWGS84 = GEOConverter.GetWGS84GeometryFromUTM(envelopeGeometryUTM, utmZoneOfLeftGeometry);
                        expressionResult = new RDFTypedLiteral(WKTWriter.Write(envelopeGeometryWGS84), RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT);
                    }
                    else if (this is GEOEqualsExpression)
                    {
                        bool sfEquals = leftGeometryUTM.EqualsTopologically(rightGeometryUTM);
                        expressionResult = sfEquals ? RDFTypedLiteral.True : RDFTypedLiteral.False;
                    }
                    else if (this is GEOGetSRIDExpression)
                    {
                        expressionResult = new RDFTypedLiteral($"http://www.opengis.net/def/crs/EPSG/0/{leftGeometry.SRID}", RDFModelEnums.RDFDatatypes.XSD_ANYURI);
                    }
                    else if (this is GEOIntersectionExpression)
                    {
                        Geometry intersectionGeometryUTM = leftGeometryUTM.Intersection(rightGeometryUTM);
                        Geometry intersectionGeometryWGS84 = GEOConverter.GetWGS84GeometryFromUTM(intersectionGeometryUTM, utmZoneOfLeftGeometry);
                        expressionResult = new RDFTypedLiteral(WKTWriter.Write(intersectionGeometryWGS84), RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT);
                    }
                    else if (this is GEOIntersectsExpression)
                    {
                        bool sfIntersects = leftGeometryUTM.Intersects(rightGeometryUTM);
                        expressionResult = sfIntersects ? RDFTypedLiteral.True : RDFTypedLiteral.False;
                    }
                    else if (this is GEOOverlapsExpression)
                    {
                        bool sfOverlaps = leftGeometryUTM.Overlaps(rightGeometryUTM);
                        expressionResult = sfOverlaps ? RDFTypedLiteral.True : RDFTypedLiteral.False;
                    }
                    else if (this is GEORCC8Expression geoRCC8Expression)
                    {
                        bool sfRCSS8Relate = false;
                        switch (geoRCC8Expression.RCC8Relation)
                        {
                            case GEOEnums.GEORCC8Relations.RCC8DC:
                                sfRCSS8Relate = leftGeometryUTM.Relate(rightGeometryUTM, "FFTFFTTTT");
                                break;
                            case GEOEnums.GEORCC8Relations.RCC8EC:
                                sfRCSS8Relate = leftGeometryUTM.Relate(rightGeometryUTM, "FFTFTTTTT");
                                break;
                            case GEOEnums.GEORCC8Relations.RCC8EQ:
                                sfRCSS8Relate = leftGeometryUTM.Relate(rightGeometryUTM, "TFFFTFFFT");
                                break;
                            case GEOEnums.GEORCC8Relations.RCC8NTPP:
                                sfRCSS8Relate = leftGeometryUTM.Relate(rightGeometryUTM, "TFFTFFTTT");
                                break;
                            case GEOEnums.GEORCC8Relations.RCC8NTPPI:
                                sfRCSS8Relate = leftGeometryUTM.Relate(rightGeometryUTM, "TTTFFTFFT");
                                break;
                            case GEOEnums.GEORCC8Relations.RCC8PO:
                                sfRCSS8Relate = leftGeometryUTM.Relate(rightGeometryUTM, "TTTTTTTTT");
                                break;
                            case GEOEnums.GEORCC8Relations.RCC8TPP:
                                sfRCSS8Relate = leftGeometryUTM.Relate(rightGeometryUTM, "TFFTTFTTT");
                                break;
                            case GEOEnums.GEORCC8Relations.RCC8TPPI:
                                sfRCSS8Relate = leftGeometryUTM.Relate(rightGeometryUTM, "TTTFTTFFT");
                                break;
                        }
                        expressionResult = sfRCSS8Relate ? RDFTypedLiteral.True : RDFTypedLiteral.False;
                    }
                    else if (this is GEORelateExpression geoRelateExpression)
                    {
                        bool sfRelate = leftGeometryUTM.Relate(rightGeometryUTM, geoRelateExpression.DE9IMRelation);
                        expressionResult = sfRelate ? RDFTypedLiteral.True : RDFTypedLiteral.False;
                    }
                    else if (this is GEOSymDifferenceExpression)
                    {
                        Geometry symDifferenceGeometryUTM = leftGeometryUTM.SymmetricDifference(rightGeometryUTM);
                        Geometry symDifferenceGeometryWGS84 = GEOConverter.GetWGS84GeometryFromUTM(symDifferenceGeometryUTM, utmZoneOfLeftGeometry);
                        expressionResult = new RDFTypedLiteral(WKTWriter.Write(symDifferenceGeometryWGS84), RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT);
                    }
                    else if (this is GEOTouchesExpression)
                    {
                        bool sfTouches = leftGeometryUTM.Touches(rightGeometryUTM);
                        expressionResult = sfTouches ? RDFTypedLiteral.True : RDFTypedLiteral.False;
                    }
                    else if (this is GEOUnionExpression)
                    {
                        Geometry unionGeometryUTM = leftGeometryUTM.Union(rightGeometryUTM);
                        Geometry unionGeometryWGS84 = GEOConverter.GetWGS84GeometryFromUTM(unionGeometryUTM, utmZoneOfLeftGeometry);
                        expressionResult = new RDFTypedLiteral(WKTWriter.Write(unionGeometryWGS84), RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT);
                    }
                    else if (this is GEOWithinExpression)
                    {
                        bool sfWithin = leftGeometryUTM.Within(rightGeometryUTM);
                        expressionResult = sfWithin ? RDFTypedLiteral.True : RDFTypedLiteral.False;
                    }
                }
                #endregion
            }
            catch { /* Just a no-op, since type errors are normal when trying to face variable's bindings */ }

            return expressionResult;
        }
        #endregion
    }
}