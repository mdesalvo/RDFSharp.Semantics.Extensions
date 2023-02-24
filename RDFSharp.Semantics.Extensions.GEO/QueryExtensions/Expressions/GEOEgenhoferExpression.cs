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

using RDFSharp.Model;
using RDFSharp.Query;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace RDFSharp.Semantics.Extensions.GEO
{
    /// <summary>
    /// GEOEgenhoferExpression represents "geof:eh*" geographic function to be applied on a query results table.<br/>
    /// The result of this function is a boolean typed literal.
    /// </summary>
    public class GEOEgenhoferExpression : GEOExpression
    {
        #region Properties
        /// <summary>
        /// Egenhofer relation checked by this expression
        /// </summary>
        internal GEOEnums.GEOEgenhoferRelations EgenhoferRelation { get; set; }
        #endregion

        #region Ctors
        /// <summary>
        /// Default-ctor to build a geof:eh* function with given arguments
        /// </summary>
        public GEOEgenhoferExpression(RDFExpression leftArgument, RDFExpression rightArgument, GEOEnums.GEOEgenhoferRelations egenoferRelation)
            : base(leftArgument, rightArgument)
        {
            if (rightArgument == null)
                throw new RDFQueryException("Cannot create expression because given \"rightArgument\" parameter is null");

            EgenhoferRelation = egenoferRelation;
        }

        /// <summary>
        /// Default-ctor to build a geof:eh* function with given arguments
        /// </summary>
        public GEOEgenhoferExpression(RDFExpression leftArgument, RDFVariable rightArgument, GEOEnums.GEOEgenhoferRelations egenoferRelation)
            : base(leftArgument, rightArgument)
        {
            if (rightArgument == null)
                throw new RDFQueryException("Cannot create expression because given \"rightArgument\" parameter is null");

            EgenhoferRelation = egenoferRelation;
        }

        /// <summary>
        /// Default-ctor to build a geof:eh* function with given arguments
        /// </summary>
        public GEOEgenhoferExpression(RDFExpression leftArgument, RDFTypedLiteral rightArgument, GEOEnums.GEOEgenhoferRelations egenoferRelation)
            : base(leftArgument, rightArgument)
        {
            if (rightArgument == null)
                throw new RDFQueryException("Cannot create expression because given \"rightArgument\" parameter is null");
            if (!rightArgument.Datatype.Equals(RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT)
                 && !rightArgument.Datatype.Equals(RDFModelEnums.RDFDatatypes.GEOSPARQL_GML))
                throw new RDFQueryException("Cannot create expression because given \"rightArgument\" parameter is not a geographic typed literal");

            EgenhoferRelation = egenoferRelation;
        }

        /// <summary>
        /// Default-ctor to build a geof:eh* function with given arguments
        /// </summary>
        public GEOEgenhoferExpression(RDFVariable leftArgument, RDFExpression rightArgument, GEOEnums.GEOEgenhoferRelations egenoferRelation)
            : base(leftArgument, rightArgument)
        {
            if (rightArgument == null)
                throw new RDFQueryException("Cannot create expression because given \"rightArgument\" parameter is null");

            EgenhoferRelation = egenoferRelation;
        }

        /// <summary>
        /// Default-ctor to build a geof:eh* function with given arguments
        /// </summary>
        public GEOEgenhoferExpression(RDFVariable leftArgument, RDFVariable rightArgument, GEOEnums.GEOEgenhoferRelations egenoferRelation)
            : base(leftArgument, rightArgument)
        {
            if (rightArgument == null)
                throw new RDFQueryException("Cannot create expression because given \"rightArgument\" parameter is null");

            EgenhoferRelation = egenoferRelation;
        }

        /// <summary>
        /// Default-ctor to build a geof:eh* function with given arguments
        /// </summary>
        public GEOEgenhoferExpression(RDFVariable leftArgument, RDFTypedLiteral rightArgument, GEOEnums.GEOEgenhoferRelations egenoferRelation)
            : base(leftArgument, rightArgument)
        {
            if (rightArgument == null)
                throw new RDFQueryException("Cannot create expression because given \"rightArgument\" parameter is null");
            if (!rightArgument.Datatype.Equals(RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT)
                 && !rightArgument.Datatype.Equals(RDFModelEnums.RDFDatatypes.GEOSPARQL_GML))
                throw new RDFQueryException("Cannot create expression because given \"rightArgument\" parameter is not a geographic typed literal");

            EgenhoferRelation = egenoferRelation;
        }
        #endregion

        #region Interfaces
        /// <summary>
        /// Gives the string representation of the geof:eh* function
        /// </summary>
        public override string ToString()
            => ToString(new List<RDFNamespace>());
        internal override string ToString(List<RDFNamespace> prefixes)
        {
            StringBuilder sb = new StringBuilder();

            //(geof:eh*(L,R))
            sb.Append($"({RDFQueryPrinter.PrintPatternMember(GetEgenhoferFunction(), prefixes)}(");
            if (LeftArgument is RDFExpression expLeftArgument)
                sb.Append(expLeftArgument.ToString(prefixes));
            else
                sb.Append(RDFQueryPrinter.PrintPatternMember((RDFPatternMember)LeftArgument, prefixes));
            sb.Append(", ");
            if (RightArgument is RDFExpression expRightArgument)
                sb.Append(expRightArgument.ToString(prefixes));
            else
                sb.Append(RDFQueryPrinter.PrintPatternMember((RDFPatternMember)RightArgument, prefixes));
            sb.Append("))");

            return sb.ToString();
        }
        #endregion

        #region Utilities
        /// <summary>
        /// Gets the Egenhofer function corresponding to this expression
        /// </summary>
        internal RDFResource GetEgenhoferFunction()
        {
            switch (EgenhoferRelation)
            {
                case GEOEnums.GEOEgenhoferRelations.Contains:
                    return RDFVocabulary.GEOSPARQL.GEOF.EH_CONTAINS;
                case GEOEnums.GEOEgenhoferRelations.CoveredBy:
                    return RDFVocabulary.GEOSPARQL.GEOF.EH_COVERED_BY;
                case GEOEnums.GEOEgenhoferRelations.Covers:
                    return RDFVocabulary.GEOSPARQL.GEOF.EH_COVERS;
                case GEOEnums.GEOEgenhoferRelations.Disjoint:
                    return RDFVocabulary.GEOSPARQL.GEOF.EH_DISJOINT;
                case GEOEnums.GEOEgenhoferRelations.Equals:
                    return RDFVocabulary.GEOSPARQL.GEOF.EH_EQUALS;
                case GEOEnums.GEOEgenhoferRelations.Inside:
                    return RDFVocabulary.GEOSPARQL.GEOF.EH_INSIDE;
                case GEOEnums.GEOEgenhoferRelations.Meet:
                    return RDFVocabulary.GEOSPARQL.GEOF.EH_MEET;
                case GEOEnums.GEOEgenhoferRelations.Overlap:
                    return RDFVocabulary.GEOSPARQL.GEOF.EH_OVERLAP;
                default: return null;
            }
        }
        #endregion
    }
}