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
                Geometry leftGeometryLAZ = null;
                Geometry rightGeometry = null;
                Geometry rightGeometryLAZ = null;
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

                    //Project left geometry from WGS84 to Lambert Azimuthal
                    leftGeometryLAZ = GEOConverter.GetLambertAzimuthalGeometryFromWGS84(leftGeometry);

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

                            //Project right geometry from WGS84 to Lambert Azimuthal
                            rightGeometryLAZ = GEOConverter.GetLambertAzimuthalGeometryFromWGS84(rightGeometry);
                        }

                        //Otherwise, return null to signal binding error
                        else
                            return expressionResult;
                    }

                    //Execute GEO functions on LAZ geometries
                    if (this is GEOBoundaryExpression)
                    {
                        Geometry boundaryGeometryLAZ = leftGeometryLAZ.Boundary;
                        Geometry boundaryGeometryWGS84 = GEOConverter.GetWGS84GeometryFromLambertAzimuthal(boundaryGeometryLAZ);
                        expressionResult = new RDFTypedLiteral(WKTWriter.Write(boundaryGeometryWGS84), RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT);
                    }
                    else if (this is GEOBufferExpression geoBufferExpression)
                    {
                        Geometry bufferGeometryLAZ = leftGeometryLAZ.Buffer(geoBufferExpression.BufferMeters);
                        Geometry bufferGeometryWGS84 = GEOConverter.GetWGS84GeometryFromLambertAzimuthal(bufferGeometryLAZ);
                        expressionResult = new RDFTypedLiteral(WKTWriter.Write(bufferGeometryWGS84), RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT);
                    }
                    else if (this is GEOContainsExpression)
                    {
                        expressionResult = leftGeometryLAZ.Contains(rightGeometryLAZ) ? RDFTypedLiteral.True : RDFTypedLiteral.False;
                    }
                    else if (this is GEOConvexHullExpression)
                    {
                        Geometry convexHullGeometryLAZ = leftGeometryLAZ.ConvexHull();
                        Geometry convexHullGeometryWGS84 = GEOConverter.GetWGS84GeometryFromLambertAzimuthal(convexHullGeometryLAZ);
                        expressionResult = new RDFTypedLiteral(WKTWriter.Write(convexHullGeometryWGS84), RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT);
                    }
                    else if (this is GEOCrossesExpression)
                    {
                        expressionResult = leftGeometryLAZ.Crosses(rightGeometryLAZ) ? RDFTypedLiteral.True : RDFTypedLiteral.False;
                    }   
                    else if (this is GEODifferenceExpression)
                    {
                        Geometry differenceGeometryLAZ = leftGeometryLAZ.Difference(rightGeometryLAZ);
                        Geometry differenceGeometryWGS84 = GEOConverter.GetWGS84GeometryFromLambertAzimuthal(differenceGeometryLAZ);
                        expressionResult = new RDFTypedLiteral(WKTWriter.Write(differenceGeometryWGS84), RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT);
                    }
                    else if (this is GEODimensionExpression)
                    {
                        expressionResult = new RDFTypedLiteral(Convert.ToString((int)leftGeometryLAZ.Dimension, CultureInfo.InvariantCulture), RDFModelEnums.RDFDatatypes.XSD_INTEGER);
                    }
                    else if (this is GEODisjointExpression)
                    {
                        expressionResult = leftGeometryLAZ.Disjoint(rightGeometryLAZ) ? RDFTypedLiteral.True : RDFTypedLiteral.False;
                    }
                    else if (this is GEODistanceExpression)
                    {
                        expressionResult = new RDFTypedLiteral(Convert.ToString(leftGeometryLAZ.Distance(rightGeometryLAZ), CultureInfo.InvariantCulture), RDFModelEnums.RDFDatatypes.XSD_DOUBLE);
                    }
                    else if (this is GEOEgenhoferExpression geoEgenhoferExpression)
                    {
                        bool sfEgenhoferRelate = false;
                        switch (geoEgenhoferExpression.EgenhoferRelation)
                        {
                            case GEOEnums.GEOEgenhoferRelations.Contains:
                                sfEgenhoferRelate = leftGeometryLAZ.Relate(rightGeometryLAZ, "T*TFF*FF*");
                                break;
                            case GEOEnums.GEOEgenhoferRelations.CoveredBy:
                                sfEgenhoferRelate = leftGeometryLAZ.Relate(rightGeometryLAZ, "TFF*TFT**");
                                break;
                            case GEOEnums.GEOEgenhoferRelations.Covers:
                                sfEgenhoferRelate = leftGeometryLAZ.Relate(rightGeometryLAZ, "T*TFT*FF*");
                                break;
                            case GEOEnums.GEOEgenhoferRelations.Disjoint:
                                sfEgenhoferRelate = leftGeometryLAZ.Relate(rightGeometryLAZ, "FF*FF****");
                                break;
                            case GEOEnums.GEOEgenhoferRelations.Equals:
                                sfEgenhoferRelate = leftGeometryLAZ.Relate(rightGeometryLAZ, "TFFFTFFFT");
                                break;
                            case GEOEnums.GEOEgenhoferRelations.Inside:
                                sfEgenhoferRelate = leftGeometryLAZ.Relate(rightGeometryLAZ, "TFF*FFT**");
                                break;
                            case GEOEnums.GEOEgenhoferRelations.Meet:
                                sfEgenhoferRelate = leftGeometryLAZ.Relate(rightGeometryLAZ, "FT*******")
                                                     || leftGeometryLAZ.Relate(rightGeometryLAZ, "F**T*****")
                                                      || leftGeometryLAZ.Relate(rightGeometryLAZ, "F***T****");
                                break;
                            case GEOEnums.GEOEgenhoferRelations.Overlap:
                                sfEgenhoferRelate = leftGeometryLAZ.Relate(rightGeometryLAZ, "T*T***T**");
                                break;
                        }
                        expressionResult = sfEgenhoferRelate ? RDFTypedLiteral.True : RDFTypedLiteral.False;
                    }
                    else if (this is GEOEnvelopeExpression)
                    {
                        Geometry envelopeGeometryLAZ = leftGeometryLAZ.Envelope;
                        Geometry envelopeGeometryWGS84 = GEOConverter.GetWGS84GeometryFromLambertAzimuthal(envelopeGeometryLAZ);
                        expressionResult = new RDFTypedLiteral(WKTWriter.Write(envelopeGeometryWGS84), RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT);
                    }
                    else if (this is GEOEqualsExpression)
                    {
                        expressionResult = leftGeometryLAZ.EqualsTopologically(rightGeometryLAZ) ? RDFTypedLiteral.True : RDFTypedLiteral.False;
                    }
                    else if (this is GEOGetSRIDExpression)
                    {
                        expressionResult = new RDFTypedLiteral($"http://www.opengis.net/def/crs/EPSG/0/{leftGeometry.SRID}", RDFModelEnums.RDFDatatypes.XSD_ANYURI);
                    }
                    else if (this is GEOIntersectionExpression)
                    {
                        Geometry intersectionGeometryLAZ = leftGeometryLAZ.Intersection(rightGeometryLAZ);
                        Geometry intersectionGeometryWGS84 = GEOConverter.GetWGS84GeometryFromLambertAzimuthal(intersectionGeometryLAZ);
                        expressionResult = new RDFTypedLiteral(WKTWriter.Write(intersectionGeometryWGS84), RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT);
                    }
                    else if (this is GEOIntersectsExpression)
                    {
                        expressionResult = leftGeometryLAZ.Intersects(rightGeometryLAZ) ? RDFTypedLiteral.True : RDFTypedLiteral.False;
                    }
                    else if (this is GEOIsSimpleExpression)
                    {
                        expressionResult = leftGeometryLAZ.IsSimple ? RDFTypedLiteral.True : RDFTypedLiteral.False;
                    }
                    else if (this is GEOOverlapsExpression)
                    {
                        expressionResult = leftGeometryLAZ.Overlaps(rightGeometryLAZ) ? RDFTypedLiteral.True : RDFTypedLiteral.False;
                    }
                    else if (this is GEORCC8Expression geoRCC8Expression)
                    {
                        bool sfRCSS8Relate = false;
                        switch (geoRCC8Expression.RCC8Relation)
                        {
                            case GEOEnums.GEORCC8Relations.RCC8DC:
                                sfRCSS8Relate = leftGeometryLAZ.Relate(rightGeometryLAZ, "FFTFFTTTT");
                                break;
                            case GEOEnums.GEORCC8Relations.RCC8EC:
                                sfRCSS8Relate = leftGeometryLAZ.Relate(rightGeometryLAZ, "FFTFTTTTT");
                                break;
                            case GEOEnums.GEORCC8Relations.RCC8EQ:
                                sfRCSS8Relate = leftGeometryLAZ.Relate(rightGeometryLAZ, "TFFFTFFFT");
                                break;
                            case GEOEnums.GEORCC8Relations.RCC8NTPP:
                                sfRCSS8Relate = leftGeometryLAZ.Relate(rightGeometryLAZ, "TFFTFFTTT");
                                break;
                            case GEOEnums.GEORCC8Relations.RCC8NTPPI:
                                sfRCSS8Relate = leftGeometryLAZ.Relate(rightGeometryLAZ, "TTTFFTFFT");
                                break;
                            case GEOEnums.GEORCC8Relations.RCC8PO:
                                sfRCSS8Relate = leftGeometryLAZ.Relate(rightGeometryLAZ, "TTTTTTTTT");
                                break;
                            case GEOEnums.GEORCC8Relations.RCC8TPP:
                                sfRCSS8Relate = leftGeometryLAZ.Relate(rightGeometryLAZ, "TFFTTFTTT");
                                break;
                            case GEOEnums.GEORCC8Relations.RCC8TPPI:
                                sfRCSS8Relate = leftGeometryLAZ.Relate(rightGeometryLAZ, "TTTFTTFFT");
                                break;
                        }
                        expressionResult = sfRCSS8Relate ? RDFTypedLiteral.True : RDFTypedLiteral.False;
                    }
                    else if (this is GEORelateExpression geoRelateExpression)
                    {
                        expressionResult = leftGeometryLAZ.Relate(rightGeometryLAZ, geoRelateExpression.DE9IMRelation) ? RDFTypedLiteral.True : RDFTypedLiteral.False;
                    }
                    else if (this is GEOSymDifferenceExpression)
                    {
                        Geometry symDifferenceGeometryLAZ = leftGeometryLAZ.SymmetricDifference(rightGeometryLAZ);
                        Geometry symDifferenceGeometryWGS84 = GEOConverter.GetWGS84GeometryFromLambertAzimuthal(symDifferenceGeometryLAZ);
                        expressionResult = new RDFTypedLiteral(WKTWriter.Write(symDifferenceGeometryWGS84), RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT);
                    }
                    else if (this is GEOTouchesExpression)
                    {
                        expressionResult = leftGeometryLAZ.Touches(rightGeometryLAZ) ? RDFTypedLiteral.True : RDFTypedLiteral.False;
                    }
                    else if (this is GEOUnionExpression)
                    {
                        Geometry unionGeometryLAZ = leftGeometryLAZ.Union(rightGeometryLAZ);
                        Geometry unionGeometryWGS84 = GEOConverter.GetWGS84GeometryFromLambertAzimuthal(unionGeometryLAZ);
                        expressionResult = new RDFTypedLiteral(WKTWriter.Write(unionGeometryWGS84), RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT);
                    }
                    else if (this is GEOWithinExpression)
                    {
                        expressionResult = leftGeometryLAZ.Within(rightGeometryLAZ) ? RDFTypedLiteral.True : RDFTypedLiteral.False;
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