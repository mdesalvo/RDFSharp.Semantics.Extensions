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
    public class GEOBufferExpressionTest
    {
        #region Tests
        [TestMethod]
        public void ShouldCreateGEOBufferExpressionWithExpression()
        {
            GEOBufferExpression expression = new GEOBufferExpression(
                new RDFVariableExpression(new RDFVariable("?V")), 150); //150mt buffering

            Assert.IsNotNull(expression);
            Assert.IsNotNull(expression.LeftArgument);
            Assert.IsNull(expression.RightArgument);
            Assert.IsTrue(expression.ToString().Equals($"(<{RDFVocabulary.GEOSPARQL.GEOF.BUFFER}>(?V, 150))"));
            Assert.IsTrue(expression.ToString(new List<RDFNamespace>() { RDFNamespaceRegister.GetByPrefix("geof") }).Equals("(geof:buffer(?V, 150))"));
        }

        [TestMethod]
        public void ShouldCreateGEOBufferExpressionWithVariable()
        {
            GEOBufferExpression expression = new GEOBufferExpression(
                new RDFVariable("?V"), 150); //150mt buffering

            Assert.IsNotNull(expression);
            Assert.IsNotNull(expression.LeftArgument);
            Assert.IsNull(expression.RightArgument);
            Assert.IsTrue(expression.ToString().Equals($"(<{RDFVocabulary.GEOSPARQL.GEOF.BUFFER}>(?V, 150))"));
            Assert.IsTrue(expression.ToString(new List<RDFNamespace>() { RDFNamespaceRegister.GetByPrefix("geof") }).Equals("(geof:buffer(?V, 150))"));
        }

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingGEOBufferExpressionWithEXPBecauseNullLeftArgument()
            => Assert.ThrowsException<RDFQueryException>(() => new GEOBufferExpression(null as RDFVariableExpression, 150));

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingGEOBufferExpressionWithVARBecauseNullLeftArgument()
            => Assert.ThrowsException<RDFQueryException>(() => new GEOBufferExpression(null as RDFVariable, 150));

        [TestMethod]
        public void ShouldApplyExpressionWithEXPAndCalculateResultPoint()
        {
            DataTable table = new DataTable();
            table.Columns.Add("?MILAN", typeof(string));
            table.Columns.Add("?ROME", typeof(string));
            DataRow row = table.NewRow();
            row["?MILAN"] = new RDFTypedLiteral("POINT (9.18854 45.464664)", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT).ToString();
            row["?ROME"] = new RDFTypedLiteral("<gml:Point xmlns:gml=\"http://www.opengis.net/gml/3.2\"><gml:pos>12.496365 41.902782</gml:pos></gml:Point>", RDFModelEnums.RDFDatatypes.GEOSPARQL_GML).ToString();
            table.Rows.Add(row);
            table.AcceptChanges();

            GEOBufferExpression expression = new GEOBufferExpression(
                new RDFVariableExpression(new RDFVariable("?MILAN")), 1000); //1000mt buffering
            RDFPatternMember expressionResult = expression.ApplyExpression(table.Rows[0]);

            Assert.IsNotNull(expressionResult);
            Assert.IsTrue(expressionResult.Equals(new RDFTypedLiteral("POLYGON ((9.20133163 45.46464217, 9.2010796 45.46288657, 9.20034572 45.4611993, 9.19915824 45.45964518, 9.19756282 45.45828394, 9.19562077 45.45716787, 9.19340671 45.45633984, 9.19100571 45.45583166, 9.18851 45.45566285, 9.18601544 45.4558399, 9.18361786 45.45635599, 9.18140934 45.45719132, 9.17947475 45.4583138, 9.17788841 45.45968029, 9.1767113 45.4612383, 9.17598868 45.46292798, 9.17574836 45.4646844, 9.17599961 45.46644006, 9.17673283 45.46812748, 9.17791987 45.46968183, 9.17951515 45.47104334, 9.18145736 45.47215968, 9.18367186 45.47298794, 9.18607352 45.47349627, 9.18857001 45.47366514, 9.19106535 45.47348803, 9.19346359 45.47297178, 9.19567254 45.47213622, 9.19760729 45.47101347, 9.19919346 45.46964671, 9.20037013 45.46808847, 9.20109209 45.46639864, 9.20133163 45.46464217))", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT)));
        }

        [TestMethod]
        public void ShouldApplyExpressionWithVARAndCalculateResult()
        {
            DataTable table = new DataTable();
            table.Columns.Add("?MILAN", typeof(string));
            table.Columns.Add("?ROME", typeof(string));
            DataRow row = table.NewRow();
            row["?ROME"] = new RDFTypedLiteral("POINT (12.496365 41.902782)", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT).ToString();
            row["?MILAN"] = new RDFTypedLiteral("<gml:Point xmlns:gml=\"http://www.opengis.net/gml/3.2\"><gml:pos>9.18854 45.464664</gml:pos></gml:Point>", RDFModelEnums.RDFDatatypes.GEOSPARQL_GML).ToString();
            table.Rows.Add(row);
            table.AcceptChanges();

            GEOBufferExpression expression = new GEOBufferExpression(
                new RDFVariable("?MILAN"), 150); //150mt buffering
            RDFPatternMember expressionResult = expression.ApplyExpression(table.Rows[0]);

            Assert.IsNotNull(expressionResult);
            Assert.IsTrue(expressionResult.Equals(new RDFTypedLiteral("POLYGON ((9.19045875 45.46466082, 9.19042099 45.46439747, 9.19031095 45.46414437, 9.19013286 45.46391124, 9.18989355 45.46370704, 9.18960223 45.46353961, 9.1892701 45.46341539, 9.18890991 45.46333915, 9.1885355 45.46331383, 9.18816127 45.46334039, 9.18780159 45.46341781, 9.18747028 45.46354313, 9.18718008 45.46371152, 9.18694214 45.46391651, 9.1867656 45.46415022, 9.18665725 45.46440369, 9.18662125 45.46466715, 9.18665899 45.4649305, 9.18676902 45.4651836, 9.1869471 45.46541674, 9.1871864 45.46562095, 9.18747772 45.46578838, 9.18780987 45.46591261, 9.18817008 45.46598885, 9.1885445 45.46601417, 9.18891875 45.46598761, 9.18927845 45.46591018, 9.18960976 45.46578486, 9.18989996 45.46561647, 9.1901379 45.46541147, 9.19031443 45.46517775, 9.19042276 45.46492428, 9.19045875 45.46466082))", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT)));
        }

        [TestMethod]
        public void ShouldApplyExpressionWithEXPAndCalculateResultLineString()
        {
            DataTable table = new DataTable();
            table.Columns.Add("?MILANROME", typeof(string));
            table.Columns.Add("?ROMENAPLES", typeof(string));
            DataRow row = table.NewRow();
            row["?MILANROME"] = new RDFTypedLiteral("LINESTRING (9.18854 45.464664, 12.496365 41.902782)", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT).ToString();
            row["?ROMENAPLES"] = new RDFTypedLiteral("<gml:LineString xmlns:gml=\"http://www.opengis.net/gml/3.2\"><gml:posList>12.496365 41.902782 14.2681244 40.8517746</gml:posList></gml:LineString>", RDFModelEnums.RDFDatatypes.GEOSPARQL_GML).ToString();
            table.Rows.Add(row);
            table.AcceptChanges();

            GEOBufferExpression expression = new GEOBufferExpression(
                new RDFVariableExpression(new RDFVariable("?MILANROME")), 150); //150mt buffering
            RDFPatternMember expressionResult = expression.ApplyExpression(table.Rows[0]);

            Assert.IsNotNull(expressionResult);
            Assert.IsTrue(expressionResult.Equals(new RDFTypedLiteral("POLYGON ((12.49788185 41.90351509, 12.49804414 41.90327993, 12.49814191 41.90302564, 12.49817139 41.90276199, 12.49813145 41.9024991, 12.49802363 41.90224709, 12.49785207 41.90201563, 12.49762336 41.90181363, 12.4973463 41.90164884, 12.49703153 41.90152759, 12.49669115 41.90145455, 12.49633823 41.90143252, 12.49598634 41.90146235, 12.49564901 41.9015429, 12.49533918 41.90167106, 12.49506878 41.90184191, 12.49484819 41.90204889, 9.18697026 45.46388757, 9.18678516 45.46411798, 9.18666749 45.46436938, 9.18662179 45.4646321, 9.18664979 45.46489604, 9.18675044 45.46515107, 9.18691987 45.46538738, 9.18715155 45.4655959, 9.1874366 45.4657686, 9.18776405 45.46589885, 9.18812132 45.46598164, 9.18849468 45.4660138, 9.18886979 45.46599408, 9.18923222 45.46592325, 9.18956805 45.46580403, 9.18986436 45.46564099, 9.19010978 45.46544041, 12.49788185 41.90351509))", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT)));
        }

        [TestMethod]
        public void ShouldApplyExpressionWithEXPAndCalculateResultPolygon()
        {
            DataTable table = new DataTable();
            table.Columns.Add("?MILANROMENAPLES", typeof(string));
            table.Columns.Add("?ROMENAPLESMILAN", typeof(string));
            DataRow row = table.NewRow();
            row["?MILANROMENAPLES"] = new RDFTypedLiteral("POLYGON ((9.18854 45.464664, 12.496365 41.902782, 14.2681244 40.8517746, 9.18854 45.464664))", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT).ToString();
            row["?ROMENAPLESMILAN"] = new RDFTypedLiteral("<gml:Polygon xmlns:gml=\"http://www.opengis.net/gml/3.2\"><gml:exterior><gml:LinearRing><gml:posList>12.496365 41.902782 14.2681244 40.8517746 9.18854 45.464664 12.496365 41.902782</gml:posList></gml:LinearRing></gml:exterior></gml:Polygon>", RDFModelEnums.RDFDatatypes.GEOSPARQL_GML).ToString();
            table.Rows.Add(row);
            table.AcceptChanges();

            GEOBufferExpression expression = new GEOBufferExpression(
                new RDFVariableExpression(new RDFVariable("?ROMENAPLESMILAN")), 150); //150mt buffering
            RDFPatternMember expressionResult = expression.ApplyExpression(table.Rows[0]);

            Assert.IsNotNull(expressionResult);
            Assert.IsTrue(expressionResult.Equals(new RDFTypedLiteral("POLYGON ((12.49484506 41.90205125, 9.18697686 45.46388691, 9.18679617 45.46410903, 9.18667845 45.46435119, 9.18662797 45.46460465, 9.18664654 45.46486026, 9.18673349 45.46510878, 9.18688569 45.46534123, 9.18709763 45.46554923, 9.18736168 45.46572525, 9.18766828 45.46586295, 9.18800636 45.46595735, 9.18836372 45.46600503, 9.18872745 45.46600428, 9.1890844 45.46595512, 9.1894217 45.46585934, 9.18972715 45.46572038, 9.18998972 45.46554327, 14.26954393 40.85258944, 14.26972343 40.85236752, 14.26984352 40.85212358, 14.26989976 40.85186668, 14.26989004 40.85160635, 14.26981474 40.85135228, 14.26967664 40.8511139, 14.26948089 40.85090005, 14.26923474 40.8507187, 14.26894735 40.85057657, 14.26862939 40.85047894, 14.26829267 40.85042945, 14.26794971 40.85042992, 14.26761323 40.85048034, 14.26729574 40.85057884, 14.26700903 40.85072176, 12.49526167 41.90171246, 12.49503391 41.9018685, 12.49484506 41.90205125))", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT)));
        }

        [TestMethod]
        public void ShouldApplyExpressionAndNotCalculateResultBecauseUnknownLeftExpression()
        {
            DataTable table = new DataTable();
            table.Columns.Add("?MILAN", typeof(string));
            table.Columns.Add("?ROME", typeof(string));
            DataRow row = table.NewRow();
            row["?MILAN"] = new RDFTypedLiteral("POINT (9.18854 45.464664)", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT).ToString();
            row["?ROME"] = new RDFTypedLiteral("<gml:Point xmlns:gml=\"http://www.opengis.net/gml/3.2\"><gml:pos>12.496365 41.902782</gml:pos></gml:Point>", RDFModelEnums.RDFDatatypes.GEOSPARQL_GML).ToString();
            table.Rows.Add(row);
            table.AcceptChanges();

            GEOBufferExpression expression = new GEOBufferExpression(
                new RDFVariable("?NAPLES"), 150);
            RDFPatternMember expressionResult = expression.ApplyExpression(table.Rows[0]);

            Assert.IsNull(expressionResult);
        }

        [TestMethod]
        public void ShouldApplyExpressionAndNotCalculateResultBecauseNotGeographicLeftExpression()
        {
            DataTable table = new DataTable();
            table.Columns.Add("?MILAN", typeof(string));
            table.Columns.Add("?ROME", typeof(string));
            DataRow row = table.NewRow();
            row["?MILAN"] = new RDFTypedLiteral("POINT (9.18854 45.464664)", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT).ToString();
            row["?ROME"] = new RDFTypedLiteral("hello", RDFModelEnums.RDFDatatypes.RDFS_LITERAL).ToString();
            table.Rows.Add(row);
            table.AcceptChanges();

            GEOBufferExpression expression = new GEOBufferExpression(
                new RDFVariableExpression(new RDFVariable("?ROME")), 150);
            RDFPatternMember expressionResult = expression.ApplyExpression(table.Rows[0]);

            Assert.IsNull(expressionResult);
        }

        [TestMethod]
        public void ShouldApplyExpressionAndNotCalculateResultBecauseUnboundLeftExpression()
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

            GEOBufferExpression expression = new GEOBufferExpression(
                new RDFVariableExpression(new RDFVariable("?NAPLES")), 150);
            RDFPatternMember expressionResult = expression.ApplyExpression(table.Rows[0]);

            Assert.IsNull(expressionResult);
        }
        #endregion
    }
}