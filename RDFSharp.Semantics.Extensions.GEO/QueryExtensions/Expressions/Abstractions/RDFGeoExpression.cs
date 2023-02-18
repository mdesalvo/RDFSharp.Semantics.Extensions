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
    /// RDFGeoExpression represents a geographic expression to be applied on a query results table.
    /// </summary>
    public abstract class RDFGeoExpression : RDFExpression
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
        public RDFGeoExpression(RDFExpressionArgument leftArgument, RDFExpressionArgument rightArgument)
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

                    //Project left geometry from WGS84 to UTM
                    (int, bool) utmFromWGS84LeftGeometry = GEOConverter.GetUTMZoneFromWGS84Coordinates(leftGeometry.Coordinates[0].X, leftGeometry.Coordinates[0].Y);
                    leftGeometryUTM = GEOConverter.GetUTMGeometryFromWGS84(leftGeometry, utmFromWGS84LeftGeometry);

                    //Determine if GeoSPARQL function requires right geometry
                    if (HasRightArgument
                         && rightArgumentPMember is RDFTypedLiteral rightArgumentTypedLiteral
                          && (rightArgumentTypedLiteral.Datatype.Equals(RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT) ||
                               rightArgumentTypedLiteral.Datatype.Equals(RDFModelEnums.RDFDatatypes.GEOSPARQL_GML)))
                    {
                        //Parse WGS84 WKT/GML right geometry
                        rightGeometry = rightArgumentTypedLiteral.Datatype.Equals(RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT) ?
                        WKTReader.Read(rightArgumentTypedLiteral.Value) : GMLReader.Read(rightArgumentTypedLiteral.Value);

                        //Project right geometry from WGS84 to UTM
                        (int, bool) utmFromWGS84RightGeometry = GEOConverter.GetUTMZoneFromWGS84Coordinates(rightGeometry.Coordinates[0].X, rightGeometry.Coordinates[0].Y);
                        rightGeometryUTM = GEOConverter.GetUTMGeometryFromWGS84(rightGeometry, utmFromWGS84RightGeometry);
                    }

                    //Execute GeoSPARQL functions on UTM geometries
                    if (this is RDFGeoDistanceExpression)
                        expressionResult = new RDFTypedLiteral(Convert.ToString(leftGeometryUTM.Distance(rightGeometryUTM), CultureInfo.InvariantCulture), RDFModelEnums.RDFDatatypes.XSD_DOUBLE);
                    else if (this is RDFGeoBufferExpression geobufexp)
                    {
                        Geometry bufferGeometryUTM = leftGeometryUTM.Buffer(geobufexp.BufferMeters);
                        Geometry bufferGeometryWGS84 = GEOConverter.GetWGS84GeometryFromUTM(bufferGeometryUTM, utmFromWGS84LeftGeometry);
                        expressionResult = new RDFTypedLiteral(WKTWriter.Write(bufferGeometryWGS84), RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT);
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