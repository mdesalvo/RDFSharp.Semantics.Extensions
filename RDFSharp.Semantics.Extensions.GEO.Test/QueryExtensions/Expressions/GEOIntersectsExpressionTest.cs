﻿/*
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
using RDFSharp.Model;
using RDFSharp.Query;
using System.Collections.Generic;
using System.Data;

namespace RDFSharp.Semantics.Extensions.GEO.Test
{
    [TestClass]
    public class GEOIntersectsExpressionTest
    {
        #region Tests
        [TestMethod]
        public void ShouldCreateGEOIntersectsExpressionWithExpressions()
        {
            GEOIntersectsExpression expression = new GEOIntersectsExpression(
                new RDFVariableExpression(new RDFVariable("?V")),
                new RDFConstantExpression(new RDFTypedLiteral("POINT (1 1)", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT)));

            Assert.IsNotNull(expression);
            Assert.IsNotNull(expression.LeftArgument);
            Assert.IsNotNull(expression.RightArgument);
            Assert.IsTrue(expression.ToString().Equals($"(<{RDFVocabulary.GEOSPARQL.GEOF.SF_INTERSECTS}>(?V, \"POINT (1 1)\"^^<{RDFVocabulary.GEOSPARQL.WKT_LITERAL}>))"));
            Assert.IsTrue(expression.ToString(new List<RDFNamespace>() { RDFNamespaceRegister.GetByPrefix("geof"), RDFNamespaceRegister.GetByPrefix("geosparql") }).Equals("(geof:sfIntersects(?V, \"POINT (1 1)\"^^geosparql:wktLiteral))"));
        }

        [TestMethod]
        public void ShouldCreateGEOIntersectsExpressionWithExpressionAndVariable()
        {
            GEOIntersectsExpression expression = new GEOIntersectsExpression(
                new RDFConstantExpression(new RDFTypedLiteral("POINT (1 1)", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT)),
                new RDFVariable("?V"));

            Assert.IsNotNull(expression);
            Assert.IsNotNull(expression.LeftArgument);
            Assert.IsNotNull(expression.RightArgument);
            Assert.IsTrue(expression.ToString().Equals($"(<{RDFVocabulary.GEOSPARQL.GEOF.SF_INTERSECTS}>(\"POINT (1 1)\"^^<{RDFVocabulary.GEOSPARQL.WKT_LITERAL}>, ?V))"));
            Assert.IsTrue(expression.ToString(new List<RDFNamespace>() { RDFNamespaceRegister.GetByPrefix("geof"), RDFNamespaceRegister.GetByPrefix("geosparql") }).Equals("(geof:sfIntersects(\"POINT (1 1)\"^^geosparql:wktLiteral, ?V))"));
        }

        [TestMethod]
        public void ShouldCreateGEOIntersectsExpressionWithExpressionAndTypedLiteral()
        {
            GEOIntersectsExpression expression = new GEOIntersectsExpression(
                new RDFConstantExpression(new RDFTypedLiteral("POINT (2 2)", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT)),
                new RDFTypedLiteral("POINT (1 1)", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT));

            Assert.IsNotNull(expression);
            Assert.IsNotNull(expression.LeftArgument);
            Assert.IsNotNull(expression.RightArgument);
            Assert.IsTrue(expression.ToString().Equals($"(<{RDFVocabulary.GEOSPARQL.GEOF.SF_INTERSECTS}>(\"POINT (2 2)\"^^<{RDFVocabulary.GEOSPARQL.WKT_LITERAL}>, \"POINT (1 1)\"^^<{RDFVocabulary.GEOSPARQL.WKT_LITERAL}>))"));
            Assert.IsTrue(expression.ToString(new List<RDFNamespace>() { RDFNamespaceRegister.GetByPrefix("geof"), RDFNamespaceRegister.GetByPrefix("geosparql") }).Equals("(geof:sfIntersects(\"POINT (2 2)\"^^geosparql:wktLiteral, \"POINT (1 1)\"^^geosparql:wktLiteral))"));
        }

        [TestMethod]
        public void ShouldCreateGEOIntersectsExpressionWithVariableAndExpression()
        {
            GEOIntersectsExpression expression = new GEOIntersectsExpression(
                new RDFVariable("?V"),
                new RDFConstantExpression(new RDFTypedLiteral("POINT (1 1)", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT)));

            Assert.IsNotNull(expression);
            Assert.IsNotNull(expression.LeftArgument);
            Assert.IsNotNull(expression.RightArgument);
            Assert.IsTrue(expression.ToString().Equals($"(<{RDFVocabulary.GEOSPARQL.GEOF.SF_INTERSECTS}>(?V, \"POINT (1 1)\"^^<{RDFVocabulary.GEOSPARQL.WKT_LITERAL}>))"));
            Assert.IsTrue(expression.ToString(new List<RDFNamespace>() { RDFNamespaceRegister.GetByPrefix("geof"), RDFNamespaceRegister.GetByPrefix("geosparql") }).Equals("(geof:sfIntersects(?V, \"POINT (1 1)\"^^geosparql:wktLiteral))"));
        }

        [TestMethod]
        public void ShouldCreateGEOIntersectsExpressionWithVariables()
        {
            GEOIntersectsExpression expression = new GEOIntersectsExpression(
                new RDFVariable("?V1"),
                new RDFVariable("?V2"));

            Assert.IsNotNull(expression);
            Assert.IsNotNull(expression.LeftArgument);
            Assert.IsNotNull(expression.RightArgument);
            Assert.IsTrue(expression.ToString().Equals($"(<{RDFVocabulary.GEOSPARQL.GEOF.SF_INTERSECTS}>(?V1, ?V2))"));
            Assert.IsTrue(expression.ToString(new List<RDFNamespace>() { RDFNamespaceRegister.GetByPrefix("geof"), RDFNamespaceRegister.GetByPrefix("geosparql") }).Equals("(geof:sfIntersects(?V1, ?V2))"));
        }

        [TestMethod]
        public void ShouldCreateGEOIntersectsExpressionWithVariableAndTypedLiteral()
        {
            GEOIntersectsExpression expression = new GEOIntersectsExpression(
                new RDFVariable("?V"),
                new RDFTypedLiteral("POINT (1 1)", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT));

            Assert.IsNotNull(expression);
            Assert.IsNotNull(expression.LeftArgument);
            Assert.IsNotNull(expression.RightArgument);
            Assert.IsTrue(expression.ToString().Equals($"(<{RDFVocabulary.GEOSPARQL.GEOF.SF_INTERSECTS}>(?V, \"POINT (1 1)\"^^<{RDFVocabulary.GEOSPARQL.WKT_LITERAL}>))"));
            Assert.IsTrue(expression.ToString(new List<RDFNamespace>() { RDFNamespaceRegister.GetByPrefix("geof"), RDFNamespaceRegister.GetByPrefix("geosparql") }).Equals("(geof:sfIntersects(?V, \"POINT (1 1)\"^^geosparql:wktLiteral))"));
        }

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingGEOIntersectsExpressionWithEEBecauseNullLeftArgument()
            => Assert.ThrowsException<RDFQueryException>(() => new GEOIntersectsExpression(null as RDFVariableExpression, new RDFVariableExpression(new RDFVariable("?V"))));

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingGEOIntersectsExpressionWithEEBecauseNullRightArgument()
            => Assert.ThrowsException<RDFQueryException>(() => new GEOIntersectsExpression(new RDFVariableExpression(new RDFVariable("?V")), null as RDFVariableExpression));

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingGEOIntersectsExpressionWithEVBecauseNullLeftArgument()
            => Assert.ThrowsException<RDFQueryException>(() => new GEOIntersectsExpression(null as RDFVariableExpression, new RDFVariable("?V")));

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingGEOIntersectsExpressionWithEVBecauseNullRightArgument()
            => Assert.ThrowsException<RDFQueryException>(() => new GEOIntersectsExpression(new RDFVariableExpression(new RDFVariable("?V")), null as RDFVariable));

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingGEOIntersectsExpressionWithETBecauseNullLeftArgument()
            => Assert.ThrowsException<RDFQueryException>(() => new GEOIntersectsExpression(null as RDFVariableExpression, new RDFTypedLiteral("POINT (1 1)", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT)));

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingGEOIntersectsExpressionWithETBecauseNullRightArgument()
            => Assert.ThrowsException<RDFQueryException>(() => new GEOIntersectsExpression(new RDFVariableExpression(new RDFVariable("?V")), null as RDFTypedLiteral));

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingGEOIntersectsExpressionWithETBecauseNotGeographicRightArgument()
            => Assert.ThrowsException<RDFQueryException>(() => new GEOIntersectsExpression(new RDFVariableExpression(new RDFVariable("?V")), new RDFTypedLiteral("Hello", RDFModelEnums.RDFDatatypes.XSD_STRING)));

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingGEOIntersectsExpressionWithVEBecauseNullLeftArgument()
            => Assert.ThrowsException<RDFQueryException>(() => new GEOIntersectsExpression(null as RDFVariable, new RDFVariableExpression(new RDFVariable("?V"))));

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingGEOIntersectsExpressionWithVEBecauseNullRightArgument()
            => Assert.ThrowsException<RDFQueryException>(() => new GEOIntersectsExpression(new RDFVariable("?V"), null as RDFVariableExpression));

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingGEOIntersectsExpressionWithVVBecauseNullLeftArgument()
            => Assert.ThrowsException<RDFQueryException>(() => new GEOIntersectsExpression(null as RDFVariable, new RDFVariable("?V")));

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingGEOIntersectsExpressionWithVVBecauseNullRightArgument()
            => Assert.ThrowsException<RDFQueryException>(() => new GEOIntersectsExpression(new RDFVariable("?V"), null as RDFVariable));

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingGEOIntersectsExpressionWithVTBecauseNullLeftArgument()
            => Assert.ThrowsException<RDFQueryException>(() => new GEOIntersectsExpression(null as RDFVariable, new RDFTypedLiteral("POINT (1 1)", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT)));

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingGEOIntersectsExpressionWithVTBecauseNullRightArgument()
            => Assert.ThrowsException<RDFQueryException>(() => new GEOIntersectsExpression(new RDFVariable("?V"), null as RDFTypedLiteral));

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingGEOIntersectsExpressionWithVTBecauseNotGeographicRightArgument()
            => Assert.ThrowsException<RDFQueryException>(() => new GEOIntersectsExpression(new RDFVariable("?V"), new RDFTypedLiteral("Hello", RDFModelEnums.RDFDatatypes.XSD_STRING)));

        [TestMethod]
        public void ShouldApplyExpressionWithEEAndCalculateResult()
        {
            DataTable table = new DataTable();
            table.Columns.Add("?MILAN", typeof(string));
            DataRow row = table.NewRow();
            row["?MILAN"] = new RDFTypedLiteral("POINT (9.18854 45)", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT).ToString();
            table.Rows.Add(row);
            table.AcceptChanges();

            GEOIntersectsExpression expression = new GEOIntersectsExpression(
                new RDFVariableExpression(new RDFVariable("?MILAN")),
                new RDFVariableExpression(new RDFVariable("?MILAN")));
            RDFPatternMember expressionResult = expression.ApplyExpression(table.Rows[0]);

            Assert.IsNotNull(expressionResult);
            Assert.IsTrue(expressionResult.Equals(RDFTypedLiteral.True));
        }

        [TestMethod]
        public void ShouldApplyExpressionWithEVAndCalculateResult()
        {
            DataTable table = new DataTable();
            table.Columns.Add("?MILAN", typeof(string));
            table.Columns.Add("?MILANNAPLES", typeof(string));
            DataRow row = table.NewRow();
            row["?MILAN"] = new RDFTypedLiteral("POINT (9.18854 45)", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT).ToString();
            row["?MILANNAPLES"] = new RDFTypedLiteral("<gml:LineString xmlns:gml=\"http://www.opengis.net/gml/3.2\"><gml:posList>9.18854 45 14.2681244 40.8517746</gml:posList></gml:LineString>", RDFModelEnums.RDFDatatypes.GEOSPARQL_GML).ToString();
            table.Rows.Add(row);
            table.AcceptChanges();

            GEOIntersectsExpression expression = new GEOIntersectsExpression(
                new RDFVariableExpression(new RDFVariable("?MILAN")),
                new RDFVariable("?MILANNAPLES"));
            RDFPatternMember expressionResult = expression.ApplyExpression(table.Rows[0]);

            Assert.IsNotNull(expressionResult);
            Assert.IsTrue(expressionResult.Equals(RDFTypedLiteral.True));
        }

        [TestMethod]
        public void ShouldApplyExpressionWithETAndCalculateResult()
        {
            DataTable table = new DataTable();
            table.Columns.Add("?MILAN", typeof(string));
            DataRow row = table.NewRow();
            row["?MILAN"] = new RDFTypedLiteral("POINT (9.18854 45)", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT).ToString();
            table.Rows.Add(row);
            table.AcceptChanges();

            GEOIntersectsExpression expression = new GEOIntersectsExpression(
                new RDFVariableExpression(new RDFVariable("?MILAN")),
                new RDFTypedLiteral("POLYGON ((9.18854 45, 12.496365 41.902782, 14.268124 40.851774, 9.18854 45))", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT));
            RDFPatternMember expressionResult = expression.ApplyExpression(table.Rows[0]);

            Assert.IsNotNull(expressionResult);
            Assert.IsTrue(expressionResult.Equals(RDFTypedLiteral.True));
        }

        [TestMethod]
        public void ShouldApplyExpressionWithVEAndCalculateResult()
        {
            DataTable table = new DataTable();
            table.Columns.Add("?PL", typeof(string));
            table.Columns.Add("?MPT", typeof(string));
            DataRow row = table.NewRow();
            row["?PL"] = new RDFTypedLiteral("POLYGON ((9.55854 46, 12.496365 41.902782, 14.268124 40.851774, 9.55854 46))", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT);
            row["?MPT"] = new RDFTypedLiteral("<gml:MultiPoint xmlns:gml=\"http://www.opengis.net/gml/3.2\"><gml:pointMember><gml:Point><gml:pos>9.18854 45</gml:pos></gml:Point></gml:pointMember><gml:pointMember><gml:Point><gml:pos>9.18854 39</gml:pos></gml:Point></gml:pointMember></gml:MultiPoint>", RDFModelEnums.RDFDatatypes.GEOSPARQL_GML).ToString();
            table.Rows.Add(row);
            table.AcceptChanges();

            GEOIntersectsExpression expression = new GEOIntersectsExpression(
                new RDFVariable("?MPT"),
                new RDFVariableExpression(new RDFVariable("?PL")));
            RDFPatternMember expressionResult = expression.ApplyExpression(table.Rows[0]);

            Assert.IsNotNull(expressionResult);
            Assert.IsTrue(expressionResult.Equals(RDFTypedLiteral.False));
        }

        [TestMethod]
        public void ShouldApplyExpressionWithVVAndCalculateResult()
        {
            DataTable table = new DataTable();
            table.Columns.Add("?MILAN", typeof(string));
            DataRow row = table.NewRow();
            row["?MILAN"] = new RDFTypedLiteral("POINT (9.18854 45)", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT).ToString();
            table.Rows.Add(row);
            table.AcceptChanges();

            GEOIntersectsExpression expression = new GEOIntersectsExpression(
                new RDFVariable("?MILAN"),
                new RDFVariable("?MILAN"));
            RDFPatternMember expressionResult = expression.ApplyExpression(table.Rows[0]);

            Assert.IsNotNull(expressionResult);
            Assert.IsTrue(expressionResult.Equals(RDFTypedLiteral.True));
        }

        [TestMethod]
        public void ShouldApplyExpressionWithVTAndCalculateResult()
        {
            DataTable table = new DataTable();
            table.Columns.Add("?MILAN", typeof(string));
            DataRow row = table.NewRow();
            row["?MILAN"] = new RDFTypedLiteral("POINT (9.18854 45)", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT).ToString();
            table.Rows.Add(row);
            table.AcceptChanges();

            GEOIntersectsExpression expression = new GEOIntersectsExpression(
                new RDFVariable("?MILAN"),
                new RDFTypedLiteral("MULTILINESTRING ((9.18854 49, 12.496365 41.902782), (12.496365 41.902782, 14.2681244 40.8517746))", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT));
            RDFPatternMember expressionResult = expression.ApplyExpression(table.Rows[0]);

            Assert.IsNotNull(expressionResult);
            Assert.IsTrue(expressionResult.Equals(RDFTypedLiteral.False));
        }

        [TestMethod]
        public void ShouldApplyExpressionWithEEAndNotCalculateResultBecauseUnknownLeftExpression()
        {
            DataTable table = new DataTable();
            table.Columns.Add("?MILAN", typeof(string));
            table.Columns.Add("?ROME", typeof(string));
            DataRow row = table.NewRow();
            row["?MILAN"] = new RDFTypedLiteral("POINT (9.18854 45)", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT).ToString();
            row["?ROME"] = new RDFTypedLiteral("<gml:Point xmlns:gml=\"http://www.opengis.net/gml/3.2\"><gml:pos>12.496365 41.902782</gml:pos></gml:Point>", RDFModelEnums.RDFDatatypes.GEOSPARQL_GML).ToString();
            table.Rows.Add(row);
            table.AcceptChanges();

            GEOIntersectsExpression expression = new GEOIntersectsExpression(
                new RDFVariableExpression(new RDFVariable("?NAPLES")),
                new RDFConstantExpression(new RDFTypedLiteral("<gml:Point xmlns:gml=\"http://www.opengis.net/gml/3.2\"><gml:pos>12.496365 41.902782</gml:pos></gml:Point>", RDFModelEnums.RDFDatatypes.GEOSPARQL_GML)));
            RDFPatternMember expressionResult = expression.ApplyExpression(table.Rows[0]);

            Assert.IsNull(expressionResult);
        }

        [TestMethod]
        public void ShouldApplyExpressionWithEEAndNotCalculateResultBecauseUnknownRightExpression()
        {
            DataTable table = new DataTable();
            table.Columns.Add("?MILAN", typeof(string));
            table.Columns.Add("?ROME", typeof(string));
            DataRow row = table.NewRow();
            row["?MILAN"] = new RDFTypedLiteral("POINT (9.18854 45)", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT).ToString();
            row["?ROME"] = new RDFTypedLiteral("<gml:Point xmlns:gml=\"http://www.opengis.net/gml/3.2\"><gml:pos>12.496365 41.902782</gml:pos></gml:Point>", RDFModelEnums.RDFDatatypes.GEOSPARQL_GML).ToString();
            table.Rows.Add(row);
            table.AcceptChanges();

            GEOIntersectsExpression expression = new GEOIntersectsExpression(
                new RDFConstantExpression(new RDFTypedLiteral("<gml:Point xmlns:gml=\"http://www.opengis.net/gml/3.2\"><gml:pos>12.496365 41.902782</gml:pos></gml:Point>", RDFModelEnums.RDFDatatypes.GEOSPARQL_GML)),
                new RDFVariableExpression(new RDFVariable("?NAPLES")));
            RDFPatternMember expressionResult = expression.ApplyExpression(table.Rows[0]);

            Assert.IsNull(expressionResult);
        }

        [TestMethod]
        public void ShouldApplyExpressionWithEEAndNotCalculateResultBecauseNotGeographicLeftExpression()
        {
            DataTable table = new DataTable();
            table.Columns.Add("?MILAN", typeof(string));
            table.Columns.Add("?ROME", typeof(string));
            DataRow row = table.NewRow();
            row["?MILAN"] = new RDFTypedLiteral("POINT (9.18854 45)", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT).ToString();
            row["?ROME"] = new RDFTypedLiteral("hello", RDFModelEnums.RDFDatatypes.RDFS_LITERAL).ToString();
            table.Rows.Add(row);
            table.AcceptChanges();

            GEOIntersectsExpression expression = new GEOIntersectsExpression(
                new RDFVariableExpression(new RDFVariable("?ROME")),
                new RDFConstantExpression(new RDFTypedLiteral("<gml:Point xmlns:gml=\"http://www.opengis.net/gml/3.2\"><gml:pos>12.496365 41.902782</gml:pos></gml:Point>", RDFModelEnums.RDFDatatypes.GEOSPARQL_GML)));
            RDFPatternMember expressionResult = expression.ApplyExpression(table.Rows[0]);

            Assert.IsNull(expressionResult);
        }

        [TestMethod]
        public void ShouldApplyExpressionWithEEAndNotCalculateResultBecauseNotGeographicRightExpression()
        {
            DataTable table = new DataTable();
            table.Columns.Add("?MILAN", typeof(string));
            table.Columns.Add("?ROME", typeof(string));
            DataRow row = table.NewRow();
            row["?MILAN"] = new RDFTypedLiteral("POINT (9.18854 45)", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT).ToString();
            row["?ROME"] = new RDFTypedLiteral("hello", RDFModelEnums.RDFDatatypes.RDFS_LITERAL).ToString();
            table.Rows.Add(row);
            table.AcceptChanges();

            GEOIntersectsExpression expression = new GEOIntersectsExpression(
                new RDFConstantExpression(new RDFTypedLiteral("<gml:Point xmlns:gml=\"http://www.opengis.net/gml/3.2\"><gml:pos>12.496365 41.902782</gml:pos></gml:Point>", RDFModelEnums.RDFDatatypes.GEOSPARQL_GML)),
                new RDFVariableExpression(new RDFVariable("?ROME")));
            RDFPatternMember expressionResult = expression.ApplyExpression(table.Rows[0]);

            Assert.IsNull(expressionResult);
        }

        [TestMethod]
        public void ShouldApplyExpressionWithEEAndNotCalculateResultBecauseUnboundLeftExpression()
        {
            DataTable table = new DataTable();
            table.Columns.Add("?MILAN", typeof(string));
            table.Columns.Add("?ROME", typeof(string));
            table.Columns.Add("?NAPLES", typeof(string));
            DataRow row = table.NewRow();
            row["?MILAN"] = new RDFTypedLiteral("POINT (9.18854 45)", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT).ToString();
            row["?ROME"] = new RDFTypedLiteral("<gml:Point xmlns:gml=\"http://www.opengis.net/gml/3.2\"><gml:pos>12.496365 41.902782</gml:pos></gml:Point>", RDFModelEnums.RDFDatatypes.GEOSPARQL_GML).ToString();
            row["?NAPLES"] = null;
            table.Rows.Add(row);
            table.AcceptChanges();

            GEOIntersectsExpression expression = new GEOIntersectsExpression(
                new RDFVariableExpression(new RDFVariable("?NAPLES")),
                new RDFConstantExpression(new RDFTypedLiteral("<gml:Point xmlns:gml=\"http://www.opengis.net/gml/3.2\"><gml:pos>12.496365 41.902782</gml:pos></gml:Point>", RDFModelEnums.RDFDatatypes.GEOSPARQL_GML)));
            RDFPatternMember expressionResult = expression.ApplyExpression(table.Rows[0]);

            Assert.IsNull(expressionResult);
        }

        [TestMethod]
        public void ShouldApplyExpressionWithEEAndNotCalculateResultBecauseUnboundRightExpression()
        {
            DataTable table = new DataTable();
            table.Columns.Add("?MILAN", typeof(string));
            table.Columns.Add("?ROME", typeof(string));
            table.Columns.Add("?NAPLES", typeof(string));
            DataRow row = table.NewRow();
            row["?MILAN"] = new RDFTypedLiteral("POINT (9.18854 45)", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT).ToString();
            row["?ROME"] = new RDFTypedLiteral("<gml:Point xmlns:gml=\"http://www.opengis.net/gml/3.2\"><gml:pos>12.496365 41.902782</gml:pos></gml:Point>", RDFModelEnums.RDFDatatypes.GEOSPARQL_GML).ToString();
            row["?NAPLES"] = null;
            table.Rows.Add(row);
            table.AcceptChanges();

            GEOIntersectsExpression expression = new GEOIntersectsExpression(
                new RDFConstantExpression(new RDFTypedLiteral("<gml:Point xmlns:gml=\"http://www.opengis.net/gml/3.2\"><gml:pos>12.496365 41.902782</gml:pos></gml:Point>", RDFModelEnums.RDFDatatypes.GEOSPARQL_GML)),
                new RDFVariableExpression(new RDFVariable("?NAPLES")));
            RDFPatternMember expressionResult = expression.ApplyExpression(table.Rows[0]);

            Assert.IsNull(expressionResult);
        }
        #endregion
    }
}