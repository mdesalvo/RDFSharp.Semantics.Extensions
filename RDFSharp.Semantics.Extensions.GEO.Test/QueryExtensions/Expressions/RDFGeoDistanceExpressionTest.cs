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
using RDFSharp.Model;
using RDFSharp.Query;
using System.Collections.Generic;
using System.Data;

namespace RDFSharp.Semantics.Extensions.GEO.Test
{
    [TestClass]
    public class RDFGeoDistanceExpressionTest
    {
        #region Tests
        [TestMethod]
        public void ShouldCreateGeoDistanceExpressionWithExpressions()
        {
            RDFGeoDistanceExpression expression = new RDFGeoDistanceExpression(
                new RDFVariableExpression(new RDFVariable("?V")),
                new RDFConstantExpression(new RDFTypedLiteral("POINT (1 1)", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT)));

            Assert.IsNotNull(expression);
            Assert.IsNotNull(expression.LeftArgument);
            Assert.IsNotNull(expression.RightArgument);
            Assert.IsTrue(expression.ToString().Equals($"(<{RDFVocabulary.GEOSPARQL.GEOF.DISTANCE}>(?V, \"POINT (1 1)\"^^<{RDFVocabulary.GEOSPARQL.WKT_LITERAL}>))"));
            Assert.IsTrue(expression.ToString(new List<RDFNamespace>() { RDFNamespaceRegister.GetByPrefix("geof"), RDFNamespaceRegister.GetByPrefix("geosparql") }).Equals("(geof:distance(?V, \"POINT (1 1)\"^^geosparql:wktLiteral))"));
        }

        [TestMethod]
        public void ShouldCreateGeoDistanceExpressionWithExpressionAndVariable()
        {
            RDFGeoDistanceExpression expression = new RDFGeoDistanceExpression(
                new RDFConstantExpression(new RDFTypedLiteral("POINT (1 1)", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT)),
                new RDFVariable("?V"));

            Assert.IsNotNull(expression);
            Assert.IsNotNull(expression.LeftArgument);
            Assert.IsNotNull(expression.RightArgument);
            Assert.IsTrue(expression.ToString().Equals($"(<{RDFVocabulary.GEOSPARQL.GEOF.DISTANCE}>(\"POINT (1 1)\"^^<{RDFVocabulary.GEOSPARQL.WKT_LITERAL}>, ?V))"));
            Assert.IsTrue(expression.ToString(new List<RDFNamespace>() { RDFNamespaceRegister.GetByPrefix("geof"), RDFNamespaceRegister.GetByPrefix("geosparql") }).Equals("(geof:distance(\"POINT (1 1)\"^^geosparql:wktLiteral, ?V))"));
        }

        [TestMethod]
        public void ShouldCreateGeoDistanceExpressionWithExpressionAndTypedLiteral()
        {
            RDFGeoDistanceExpression expression = new RDFGeoDistanceExpression(
                new RDFConstantExpression(new RDFTypedLiteral("POINT (2 2)", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT)),
                new RDFTypedLiteral("POINT (1 1)", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT));

            Assert.IsNotNull(expression);
            Assert.IsNotNull(expression.LeftArgument);
            Assert.IsNotNull(expression.RightArgument);
            Assert.IsTrue(expression.ToString().Equals($"(<{RDFVocabulary.GEOSPARQL.GEOF.DISTANCE}>(\"POINT (2 2)\"^^<{RDFVocabulary.GEOSPARQL.WKT_LITERAL}>, \"POINT (1 1)\"^^<{RDFVocabulary.GEOSPARQL.WKT_LITERAL}>))"));
            Assert.IsTrue(expression.ToString(new List<RDFNamespace>() { RDFNamespaceRegister.GetByPrefix("geof"), RDFNamespaceRegister.GetByPrefix("geosparql") }).Equals("(geof:distance(\"POINT (2 2)\"^^geosparql:wktLiteral, \"POINT (1 1)\"^^geosparql:wktLiteral))"));
        }

        [TestMethod]
        public void ShouldCreateGeoDistanceExpressionWithVariableAndExpression()
        {
            RDFGeoDistanceExpression expression = new RDFGeoDistanceExpression(
                new RDFVariable("?V"),
                new RDFConstantExpression(new RDFTypedLiteral("POINT (1 1)", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT)));

            Assert.IsNotNull(expression);
            Assert.IsNotNull(expression.LeftArgument);
            Assert.IsNotNull(expression.RightArgument);
            Assert.IsTrue(expression.ToString().Equals($"(<{RDFVocabulary.GEOSPARQL.GEOF.DISTANCE}>(?V, \"POINT (1 1)\"^^<{RDFVocabulary.GEOSPARQL.WKT_LITERAL}>))"));
            Assert.IsTrue(expression.ToString(new List<RDFNamespace>() { RDFNamespaceRegister.GetByPrefix("geof"), RDFNamespaceRegister.GetByPrefix("geosparql") }).Equals("(geof:distance(?V, \"POINT (1 1)\"^^geosparql:wktLiteral))"));
        }

        [TestMethod]
        public void ShouldCreateGeoDistanceExpressionWithVariables()
        {
            RDFGeoDistanceExpression expression = new RDFGeoDistanceExpression(
                new RDFVariable("?V1"),
                new RDFVariable("?V2"));

            Assert.IsNotNull(expression);
            Assert.IsNotNull(expression.LeftArgument);
            Assert.IsNotNull(expression.RightArgument);
            Assert.IsTrue(expression.ToString().Equals($"(<{RDFVocabulary.GEOSPARQL.GEOF.DISTANCE}>(?V1, ?V2))"));
            Assert.IsTrue(expression.ToString(new List<RDFNamespace>() { RDFNamespaceRegister.GetByPrefix("geof"), RDFNamespaceRegister.GetByPrefix("geosparql") }).Equals("(geof:distance(?V1, ?V2))"));
        }

        [TestMethod]
        public void ShouldCreateGeoDistanceExpressionWithVariableAndTypedLiteral()
        {
            RDFGeoDistanceExpression expression = new RDFGeoDistanceExpression(
                new RDFVariable("?V"),
                new RDFTypedLiteral("POINT (1 1)", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT));

            Assert.IsNotNull(expression);
            Assert.IsNotNull(expression.LeftArgument);
            Assert.IsNotNull(expression.RightArgument);
            Assert.IsTrue(expression.ToString().Equals($"(<{RDFVocabulary.GEOSPARQL.GEOF.DISTANCE}>(?V, \"POINT (1 1)\"^^<{RDFVocabulary.GEOSPARQL.WKT_LITERAL}>))"));
            Assert.IsTrue(expression.ToString(new List<RDFNamespace>() { RDFNamespaceRegister.GetByPrefix("geof"), RDFNamespaceRegister.GetByPrefix("geosparql") }).Equals("(geof:distance(?V, \"POINT (1 1)\"^^geosparql:wktLiteral))"));
        }

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingDistanceExpressionWithEEBecauseNullLeftArgument()
            => Assert.ThrowsException<RDFQueryException>(() => new RDFGeoDistanceExpression(null as RDFVariableExpression, new RDFVariableExpression(new RDFVariable("?V"))));

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingDistanceExpressionWithEEBecauseNullRightArgument()
            => Assert.ThrowsException<RDFQueryException>(() => new RDFGeoDistanceExpression(new RDFVariableExpression(new RDFVariable("?V")), null as RDFVariableExpression));

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingDistanceExpressionWithEVBecauseNullLeftArgument()
            => Assert.ThrowsException<RDFQueryException>(() => new RDFGeoDistanceExpression(null as RDFVariableExpression, new RDFVariable("?V")));

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingDistanceExpressionWithEVBecauseNullRightArgument()
            => Assert.ThrowsException<RDFQueryException>(() => new RDFGeoDistanceExpression(new RDFVariableExpression(new RDFVariable("?V")), null as RDFVariable));

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingDistanceExpressionWithETBecauseNullLeftArgument()
            => Assert.ThrowsException<RDFQueryException>(() => new RDFGeoDistanceExpression(null as RDFVariableExpression, new RDFTypedLiteral("POINT (1 1)", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT)));

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingDistanceExpressionWithETBecauseNullRightArgument()
            => Assert.ThrowsException<RDFQueryException>(() => new RDFGeoDistanceExpression(new RDFVariableExpression(new RDFVariable("?V")), null as RDFTypedLiteral));

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingDistanceExpressionWithETBecauseNotGeographicRightArgument()
            => Assert.ThrowsException<RDFQueryException>(() => new RDFGeoDistanceExpression(new RDFVariableExpression(new RDFVariable("?V")), new RDFTypedLiteral("Hello", RDFModelEnums.RDFDatatypes.XSD_STRING)));

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingDistanceExpressionWithVEBecauseNullLeftArgument()
            => Assert.ThrowsException<RDFQueryException>(() => new RDFGeoDistanceExpression(null as RDFVariable, new RDFVariableExpression(new RDFVariable("?V"))));

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingDistanceExpressionWithVEBecauseNullRightArgument()
            => Assert.ThrowsException<RDFQueryException>(() => new RDFGeoDistanceExpression(new RDFVariable("?V"), null as RDFVariableExpression));

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingDistanceExpressionWithVVBecauseNullLeftArgument()
            => Assert.ThrowsException<RDFQueryException>(() => new RDFGeoDistanceExpression(null as RDFVariable, new RDFVariable("?V")));

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingDistanceExpressionWithVVBecauseNullRightArgument()
            => Assert.ThrowsException<RDFQueryException>(() => new RDFGeoDistanceExpression(new RDFVariable("?V"), null as RDFVariable));

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingDistanceExpressionWithVTBecauseNullLeftArgument()
            => Assert.ThrowsException<RDFQueryException>(() => new RDFGeoDistanceExpression(null as RDFVariable, new RDFTypedLiteral("POINT (1 1)", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT)));

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingDistanceExpressionWithVTBecauseNullRightArgument()
            => Assert.ThrowsException<RDFQueryException>(() => new RDFGeoDistanceExpression(new RDFVariable("?V"), null as RDFTypedLiteral));

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingDistanceExpressionWithVTBecauseNotGeographicRightArgument()
            => Assert.ThrowsException<RDFQueryException>(() => new RDFGeoDistanceExpression(new RDFVariable("?V"), new RDFTypedLiteral("Hello", RDFModelEnums.RDFDatatypes.XSD_STRING)));

        [TestMethod]
        public void ShouldApplyExpressionWithEEAndCalculateResult()
        {
            DataTable table = new DataTable();
            table.Columns.Add("?MILAN", typeof(string));
            table.Columns.Add("?ROME", typeof(string));
            DataRow row = table.NewRow();
            row["?MILAN"] = new RDFTypedLiteral("POINT (9.18854 45.464664)", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT).ToString();
            row["?ROME"] = new RDFTypedLiteral("<gml:Point xmlns:gml=\"http://www.opengis.net/gml/3.2\"><gml:pos>12.496365 41.902782</gml:pos></gml:Point>", RDFModelEnums.RDFDatatypes.GEOSPARQL_GML).ToString();
            table.Rows.Add(row);
            table.AcceptChanges();

            RDFGeoDistanceExpression expression = new RDFGeoDistanceExpression(
                new RDFVariableExpression(new RDFVariable("?MILAN")),
                new RDFVariableExpression(new RDFVariable("?ROME")));
            RDFPatternMember expressionResult = expression.ApplyExpression(table.Rows[0]);

            Assert.IsNotNull(expressionResult);
            Assert.IsTrue(expressionResult.Equals(new RDFTypedLiteral("451197.909254953", RDFModelEnums.RDFDatatypes.XSD_DOUBLE)));
        }

        [TestMethod]
        public void ShouldApplyExpressionWithEVAndCalculateResult()
        {
            DataTable table = new DataTable();
            table.Columns.Add("?MILAN", typeof(string));
            table.Columns.Add("?ROME", typeof(string));
            DataRow row = table.NewRow();
            row["?MILAN"] = new RDFTypedLiteral("POINT (9.18854 45.464664)", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT).ToString();
            row["?ROME"] = new RDFTypedLiteral("<gml:Point xmlns:gml=\"http://www.opengis.net/gml/3.2\"><gml:pos>12.496365 41.902782</gml:pos></gml:Point>", RDFModelEnums.RDFDatatypes.GEOSPARQL_GML).ToString();
            table.Rows.Add(row);
            table.AcceptChanges();

            RDFGeoDistanceExpression expression = new RDFGeoDistanceExpression(
                new RDFVariableExpression(new RDFVariable("?MILAN")),
                new RDFVariable("?ROME"));
            RDFPatternMember expressionResult = expression.ApplyExpression(table.Rows[0]);

            Assert.IsNotNull(expressionResult);
            Assert.IsTrue(expressionResult.Equals(new RDFTypedLiteral("451197.909254953", RDFModelEnums.RDFDatatypes.XSD_DOUBLE)));
        }

        [TestMethod]
        public void ShouldApplyExpressionWithETAndCalculateResult()
        {
            DataTable table = new DataTable();
            table.Columns.Add("?MILAN", typeof(string));
            table.Columns.Add("?ROME", typeof(string));
            DataRow row = table.NewRow();
            row["?MILAN"] = new RDFTypedLiteral("POINT (9.18854 45.464664)", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT).ToString();
            row["?ROME"] = new RDFTypedLiteral("<gml:Point xmlns:gml=\"http://www.opengis.net/gml/3.2\"><gml:pos>12.496365 41.902782</gml:pos></gml:Point>", RDFModelEnums.RDFDatatypes.GEOSPARQL_GML).ToString();
            table.Rows.Add(row);
            table.AcceptChanges();

            RDFGeoDistanceExpression expression = new RDFGeoDistanceExpression(
                new RDFVariableExpression(new RDFVariable("?MILAN")),
                new RDFTypedLiteral("POINT (12.496365 41.902782)", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT));
            RDFPatternMember expressionResult = expression.ApplyExpression(table.Rows[0]);

            Assert.IsNotNull(expressionResult);
            Assert.IsTrue(expressionResult.Equals(new RDFTypedLiteral("451197.909254953", RDFModelEnums.RDFDatatypes.XSD_DOUBLE)));
        }

        [TestMethod]
        public void ShouldApplyExpressionWithVEAndCalculateResult()
        {
            DataTable table = new DataTable();
            table.Columns.Add("?MILAN", typeof(string));
            table.Columns.Add("?ROME", typeof(string));
            DataRow row = table.NewRow();
            row["?MILAN"] = new RDFTypedLiteral("POINT (9.18854 45.464664)", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT).ToString();
            row["?ROME"] = new RDFTypedLiteral("<gml:Point xmlns:gml=\"http://www.opengis.net/gml/3.2\"><gml:pos>12.496365 41.902782</gml:pos></gml:Point>", RDFModelEnums.RDFDatatypes.GEOSPARQL_GML).ToString();
            table.Rows.Add(row);
            table.AcceptChanges();

            RDFGeoDistanceExpression expression = new RDFGeoDistanceExpression(
                new RDFVariable("?MILAN"),
                new RDFVariableExpression(new RDFVariable("?ROME")));
            RDFPatternMember expressionResult = expression.ApplyExpression(table.Rows[0]);

            Assert.IsNotNull(expressionResult);
            Assert.IsTrue(expressionResult.Equals(new RDFTypedLiteral("451197.909254953", RDFModelEnums.RDFDatatypes.XSD_DOUBLE)));
        }

        [TestMethod]
        public void ShouldApplyExpressionWithVVAndCalculateResult()
        {
            DataTable table = new DataTable();
            table.Columns.Add("?MILAN", typeof(string));
            table.Columns.Add("?ROME", typeof(string));
            DataRow row = table.NewRow();
            row["?MILAN"] = new RDFTypedLiteral("POINT (9.18854 45.464664)", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT).ToString();
            row["?ROME"] = new RDFTypedLiteral("<gml:Point xmlns:gml=\"http://www.opengis.net/gml/3.2\"><gml:pos>12.496365 41.902782</gml:pos></gml:Point>", RDFModelEnums.RDFDatatypes.GEOSPARQL_GML).ToString();
            table.Rows.Add(row);
            table.AcceptChanges();

            RDFGeoDistanceExpression expression = new RDFGeoDistanceExpression(
                new RDFVariable("?MILAN"),
                new RDFVariable("?ROME"));
            RDFPatternMember expressionResult = expression.ApplyExpression(table.Rows[0]);

            Assert.IsNotNull(expressionResult);
            Assert.IsTrue(expressionResult.Equals(new RDFTypedLiteral("451197.909254953", RDFModelEnums.RDFDatatypes.XSD_DOUBLE)));
        }

        [TestMethod]
        public void ShouldApplyExpressionWithVTAndCalculateResult()
        {
            DataTable table = new DataTable();
            table.Columns.Add("?MILAN", typeof(string));
            table.Columns.Add("?ROME", typeof(string));
            DataRow row = table.NewRow();
            row["?MILAN"] = new RDFTypedLiteral("POINT (9.18854 45.464664)", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT).ToString();
            row["?ROME"] = new RDFTypedLiteral("<gml:Point xmlns:gml=\"http://www.opengis.net/gml/3.2\"><gml:pos>12.496365 41.902782</gml:pos></gml:Point>", RDFModelEnums.RDFDatatypes.GEOSPARQL_GML).ToString();
            table.Rows.Add(row);
            table.AcceptChanges();

            RDFGeoDistanceExpression expression = new RDFGeoDistanceExpression(
                new RDFVariable("?MILAN"),
                new RDFTypedLiteral("<gml:Point xmlns:gml=\"http://www.opengis.net/gml/3.2\"><gml:pos>12.496365 41.902782</gml:pos></gml:Point>", RDFModelEnums.RDFDatatypes.GEOSPARQL_GML));
            RDFPatternMember expressionResult = expression.ApplyExpression(table.Rows[0]);

            Assert.IsNotNull(expressionResult);
            Assert.IsTrue(expressionResult.Equals(new RDFTypedLiteral("451197.909254953", RDFModelEnums.RDFDatatypes.XSD_DOUBLE)));
        }

        [TestMethod]
        public void ShouldApplyExpressionWithEEAndNotCalculateResultBecauseUnknownLeftExpression()
        {
            DataTable table = new DataTable();
            table.Columns.Add("?MILAN", typeof(string));
            table.Columns.Add("?ROME", typeof(string));
            DataRow row = table.NewRow();
            row["?MILAN"] = new RDFTypedLiteral("POINT (9.18854 45.464664)", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT).ToString();
            row["?ROME"] = new RDFTypedLiteral("<gml:Point xmlns:gml=\"http://www.opengis.net/gml/3.2\"><gml:pos>12.496365 41.902782</gml:pos></gml:Point>", RDFModelEnums.RDFDatatypes.GEOSPARQL_GML).ToString();
            table.Rows.Add(row);
            table.AcceptChanges();

            RDFGeoDistanceExpression expression = new RDFGeoDistanceExpression(
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
            row["?MILAN"] = new RDFTypedLiteral("POINT (9.18854 45.464664)", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT).ToString();
            row["?ROME"] = new RDFTypedLiteral("<gml:Point xmlns:gml=\"http://www.opengis.net/gml/3.2\"><gml:pos>12.496365 41.902782</gml:pos></gml:Point>", RDFModelEnums.RDFDatatypes.GEOSPARQL_GML).ToString();
            table.Rows.Add(row);
            table.AcceptChanges();

            RDFGeoDistanceExpression expression = new RDFGeoDistanceExpression(
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
            row["?MILAN"] = new RDFTypedLiteral("POINT (9.18854 45.464664)", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT).ToString();
            row["?ROME"] = new RDFTypedLiteral("hello", RDFModelEnums.RDFDatatypes.RDFS_LITERAL).ToString();
            table.Rows.Add(row);
            table.AcceptChanges();

            RDFGeoDistanceExpression expression = new RDFGeoDistanceExpression(
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
            row["?MILAN"] = new RDFTypedLiteral("POINT (9.18854 45.464664)", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT).ToString();
            row["?ROME"] = new RDFTypedLiteral("hello", RDFModelEnums.RDFDatatypes.RDFS_LITERAL).ToString();
            table.Rows.Add(row);
            table.AcceptChanges();

            RDFGeoDistanceExpression expression = new RDFGeoDistanceExpression(
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
            row["?MILAN"] = new RDFTypedLiteral("POINT (9.18854 45.464664)", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT).ToString();
            row["?ROME"] = new RDFTypedLiteral("<gml:Point xmlns:gml=\"http://www.opengis.net/gml/3.2\"><gml:pos>12.496365 41.902782</gml:pos></gml:Point>", RDFModelEnums.RDFDatatypes.GEOSPARQL_GML).ToString();
            row["?NAPLES"] = null;
            table.Rows.Add(row);
            table.AcceptChanges();

            RDFGeoDistanceExpression expression = new RDFGeoDistanceExpression(
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
            row["?MILAN"] = new RDFTypedLiteral("POINT (9.18854 45.464664)", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT).ToString();
            row["?ROME"] = new RDFTypedLiteral("<gml:Point xmlns:gml=\"http://www.opengis.net/gml/3.2\"><gml:pos>12.496365 41.902782</gml:pos></gml:Point>", RDFModelEnums.RDFDatatypes.GEOSPARQL_GML).ToString();
            row["?NAPLES"] = null;
            table.Rows.Add(row);
            table.AcceptChanges();

            RDFGeoDistanceExpression expression = new RDFGeoDistanceExpression(
                new RDFConstantExpression(new RDFTypedLiteral("<gml:Point xmlns:gml=\"http://www.opengis.net/gml/3.2\"><gml:pos>12.496365 41.902782</gml:pos></gml:Point>", RDFModelEnums.RDFDatatypes.GEOSPARQL_GML)),
                new RDFVariableExpression(new RDFVariable("?NAPLES")));
            RDFPatternMember expressionResult = expression.ApplyExpression(table.Rows[0]);

            Assert.IsNull(expressionResult);
        }
        #endregion
    }
}