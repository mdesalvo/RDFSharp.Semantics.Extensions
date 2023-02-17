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
using RDFSharp.Model;
using RDFSharp.Query;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Text;

namespace RDFSharp.Semantics.Extensions.GEO
{
    /// <summary>
    /// RDFGeographyExpression represents a geographic expression to be applied on a query results table.
    /// </summary>
    public abstract class RDFGeographyExpression : RDFExpression
    {
        #region Properties
        /// <summary>
        /// Reader for WKT spatial representation
        /// </summary>
        protected WKTReader WKTReader { get; set; }

        /// <summary>
        /// Reader for GML spatial representation
        /// </summary>
        protected GMLReader GMLReader { get; set; }
        #endregion

        #region Ctors
        /// <summary>
        /// Default-ctor to build a geographic expression with given arguments
        /// </summary>
        public RDFGeographyExpression(RDFExpression leftArgument, RDFExpression rightArgument)
            : base(leftArgument, rightArgument)
        {
            if (rightArgument == null)
                throw new RDFQueryException("Cannot create expression because given \"rightArgument\" parameter is null");

            WKTReader = new WKTReader();
            GMLReader = new GMLReader();
        }

        /// <summary>
        /// Default-ctor to build a geographic expression with given arguments
        /// </summary>
        public RDFGeographyExpression(RDFExpression leftArgument, RDFVariable rightArgument)
            : base(leftArgument, rightArgument)
        {
            if (rightArgument == null)
                throw new RDFQueryException("Cannot create expression because given \"rightArgument\" parameter is null");

            WKTReader = new WKTReader();
            GMLReader = new GMLReader();
        }

        /// <summary>
        /// Default-ctor to build a geographic expression with given arguments
        /// </summary>
        public RDFGeographyExpression(RDFExpression leftArgument, RDFTypedLiteral rightArgument)
            : base(leftArgument, rightArgument)
        {
            if (rightArgument == null)
                throw new RDFQueryException("Cannot create expression because given \"rightArgument\" parameter is null");
            if (!rightArgument.Datatype.Equals(RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT)
                 && !rightArgument.Datatype.Equals(RDFModelEnums.RDFDatatypes.GEOSPARQL_GML))
                throw new RDFQueryException("Cannot create expression because given \"rightArgument\" parameter is not a geographic typed literal");

            WKTReader = new WKTReader();
            GMLReader = new GMLReader();
        }

        /// <summary>
        /// Default-ctor to build a geographic expression with given arguments
        /// </summary>
        public RDFGeographyExpression(RDFVariable leftArgument, RDFExpression rightArgument)
            : base(leftArgument, rightArgument)
        {
            if (rightArgument == null)
                throw new RDFQueryException("Cannot create expression because given \"rightArgument\" parameter is null");

            WKTReader = new WKTReader();
            GMLReader = new GMLReader();
        }

        /// <summary>
        /// Default-ctor to build a geographic expression with given arguments
        /// </summary>
        public RDFGeographyExpression(RDFVariable leftArgument, RDFVariable rightArgument)
            : base(leftArgument, rightArgument)
        {
            if (rightArgument == null)
                throw new RDFQueryException("Cannot create expression because given \"rightArgument\" parameter is null");

            WKTReader = new WKTReader();
            GMLReader = new GMLReader();
        }

        /// <summary>
        /// Default-ctor to build a geographic expression with given arguments
        /// </summary>
        public RDFGeographyExpression(RDFVariable leftArgument, RDFTypedLiteral rightArgument)
            : base(leftArgument, rightArgument)
        {
            if (rightArgument == null)
                throw new RDFQueryException("Cannot create expression because given \"rightArgument\" parameter is null");
            if (!rightArgument.Datatype.Equals(RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT)
                 && !rightArgument.Datatype.Equals(RDFModelEnums.RDFDatatypes.GEOSPARQL_GML))
                throw new RDFQueryException("Cannot create expression because given \"rightArgument\" parameter is not a geographic typed literal");

            WKTReader = new WKTReader();
            GMLReader = new GMLReader();
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
                if (leftArgumentPMember is RDFTypedLiteral leftArgumentTypedLiteral
                     && (leftArgumentTypedLiteral.Datatype.Equals(RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT) || 
                          leftArgumentTypedLiteral.Datatype.Equals(RDFModelEnums.RDFDatatypes.GEOSPARQL_GML))
                      && rightArgumentPMember is RDFTypedLiteral rightArgumentTypedLiteral
                       && (rightArgumentTypedLiteral.Datatype.Equals(RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT) || 
                            rightArgumentTypedLiteral.Datatype.Equals(RDFModelEnums.RDFDatatypes.GEOSPARQL_GML)))
                {
                    //Parse WKT/GML spatial representations
                    Geometry leftGeometry = leftArgumentTypedLiteral.Datatype.Equals(RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT) ?
                        WKTReader.Read(leftArgumentTypedLiteral.Value) : GMLReader.Read(leftArgumentTypedLiteral.Value);
                    Geometry rightGeometry = rightArgumentTypedLiteral.Datatype.Equals(RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT) ?
                        WKTReader.Read(rightArgumentTypedLiteral.Value) : GMLReader.Read(rightArgumentTypedLiteral.Value);

                    //Project geometries from WGS84 to UTM
                    (int, bool) utmFromWGS84LeftGeometry = GEOConverter.GetUTMZoneFromWGS84Coordinates(leftGeometry.Coordinates[0].X, leftGeometry.Coordinates[0].Y);
                    Geometry leftGeometryUTM = GEOConverter.GetUTMGeometryFromWGS84(leftGeometry, utmFromWGS84LeftGeometry);
                    (int, bool) utmFromWGS84RightGeometry = GEOConverter.GetUTMZoneFromWGS84Coordinates(rightGeometry.Coordinates[0].X, rightGeometry.Coordinates[0].Y);
                    Geometry rightGeometryUTM = GEOConverter.GetUTMGeometryFromWGS84(rightGeometry, utmFromWGS84RightGeometry);

                    //Execute GeoSPARQL functions on UTM geometries
                    if (this is RDFGeographyDistanceExpression)
                        expressionResult = new RDFTypedLiteral(Convert.ToString(leftGeometryUTM.Distance(rightGeometryUTM), CultureInfo.InvariantCulture), RDFModelEnums.RDFDatatypes.XSD_DOUBLE);
                }
                #endregion
            }
            catch { /* Just a no-op, since type errors are normal when trying to face variable's bindings */ }

            return expressionResult;
        }
        #endregion
    }
}