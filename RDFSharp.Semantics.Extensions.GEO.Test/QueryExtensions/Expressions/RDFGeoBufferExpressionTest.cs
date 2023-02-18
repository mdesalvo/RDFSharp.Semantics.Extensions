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
using NetTopologySuite.Geometries;
using RDFSharp.Model;
using RDFSharp.Query;
using System.Collections.Generic;
using System.Data;

namespace RDFSharp.Semantics.Extensions.GEO.Test
{
    [TestClass]
    public class RDFGeoBufferExpressionTest
    {
        #region Tests
        [TestMethod]
        public void ShouldCreateGeoBufferExpressionWithExpression()
        {
            RDFGeoBufferExpression expression = new RDFGeoBufferExpression(
                new RDFVariableExpression(new RDFVariable("?V")), 150); //150mt buffering

            Assert.IsNotNull(expression);
            Assert.IsNotNull(expression.LeftArgument);
            Assert.IsNull(expression.RightArgument);
            Assert.IsTrue(expression.ToString().Equals($"(<{RDFVocabulary.GEOSPARQL.GEOF.BUFFER}>(?V, 150))"));
            Assert.IsTrue(expression.ToString(new List<RDFNamespace>() { RDFNamespaceRegister.GetByPrefix("geof") }).Equals("(geof:buffer(?V, 150))"));
        }

        [TestMethod]
        public void ShouldCreateGeoBufferExpressionWithVariable()
        {
            RDFGeoBufferExpression expression = new RDFGeoBufferExpression(
                new RDFVariable("?V"), 150); //150mt buffering

            Assert.IsNotNull(expression);
            Assert.IsNotNull(expression.LeftArgument);
            Assert.IsNull(expression.RightArgument);
            Assert.IsTrue(expression.ToString().Equals($"(<{RDFVocabulary.GEOSPARQL.GEOF.BUFFER}>(?V, 150))"));
            Assert.IsTrue(expression.ToString(new List<RDFNamespace>() { RDFNamespaceRegister.GetByPrefix("geof") }).Equals("(geof:buffer(?V, 150))"));
        }

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingBufferExpressionWithEXPBecauseNullLeftArgument()
            => Assert.ThrowsException<RDFQueryException>(() => new RDFGeoBufferExpression(null as RDFVariableExpression, 150));

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingBufferExpressionWithVARBecauseNullLeftArgument()
            => Assert.ThrowsException<RDFQueryException>(() => new RDFGeoBufferExpression(null as RDFVariable, 150));

        [TestMethod]
        public void ShouldApplyExpressionWithEXPAndCalculateResult()
        {
            DataTable table = new DataTable();
            table.Columns.Add("?MILAN", typeof(string));
            table.Columns.Add("?ROME", typeof(string));
            DataRow row = table.NewRow();
            row["?MILAN"] = new RDFTypedLiteral("POINT (9.18854 45.464664)", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT).ToString();
            row["?ROME"] = new RDFTypedLiteral("<gml:Point xmlns:gml=\"http://www.opengis.net/gml/3.2\"><gml:pos>12.496365 41.902782</gml:pos></gml:Point>", RDFModelEnums.RDFDatatypes.GEOSPARQL_GML).ToString();
            table.Rows.Add(row);
            table.AcceptChanges();

            RDFGeoBufferExpression expression = new RDFGeoBufferExpression(
                new RDFVariableExpression(new RDFVariable("?MILAN")), 1000); //1000mt buffering
            RDFPatternMember expressionResult = expression.ApplyExpression(table.Rows[0]);

            Assert.IsNotNull(expressionResult);
            Assert.IsTrue(expressionResult.Equals(new RDFTypedLiteral("POLYGON ((9.201331629406859 45.46464217034949, 9.201079599158819 45.46288656848276, 9.200345724263068 45.46119929554033, 9.19915824450492 45.459645183600294, 9.19756281888693 45.45828394313989, 9.195620767205316 45.457167870400895, 9.193406712277415 45.45633984002811, 9.19100571362246 45.45583165976439, 9.188510002533777 45.455662850074326, 9.18601544342394 45.45583989529432, 9.183617856558405 45.45635599488805, 9.181409342467001 45.45719132429876, 9.17947474826111 45.458313795448205, 9.177888410788967 45.459680287861474, 9.176711301212148 45.46123830341017, 9.17598868053686 45.462927981441155, 9.175748356405366 45.46468439720692, 9.175999608723036 45.4664400555719, 9.176732826281208 45.46812748436393, 9.177919869368251 45.469681827778196, 9.179515145460833 45.471043340097616, 9.18145735752664 45.47215968369233, 9.183671858331309 45.47298794268225, 9.186073520474915 45.47349627451958, 9.188570011653997 45.47366513566404, 9.19106534869427 45.47348803394578, 9.19346359289741 45.47297177850244, 9.19567254365989 45.47213621761748, 9.19760728739112 45.47101347460901, 9.199193464479006 45.46964671134004, 9.20037012817903 45.468088467174226, 9.201092085365696 45.46639863756963, 9.201331629406859 45.46464217034949))", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT)));
        }

        [TestMethod]
        public void ShouldApplyExpressionWithVARAndCalculateResult()
        {
            DataTable table = new DataTable();
            table.Columns.Add("?MILAN", typeof(string));
            table.Columns.Add("?ROME", typeof(string));
            DataRow row = table.NewRow();
            row["?MILAN"] = new RDFTypedLiteral("POINT (9.18854 45.464664)", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT).ToString();
            row["?ROME"] = new RDFTypedLiteral("<gml:Point xmlns:gml=\"http://www.opengis.net/gml/3.2\"><gml:pos>12.496365 41.902782</gml:pos></gml:Point>", RDFModelEnums.RDFDatatypes.GEOSPARQL_GML).ToString();
            table.Rows.Add(row);
            table.AcceptChanges();

            RDFGeoBufferExpression expression = new RDFGeoBufferExpression(
                new RDFVariable("?ROME"), 150);
            RDFPatternMember expressionResult = expression.ApplyExpression(table.Rows[0]);

            Assert.IsNotNull(expressionResult);
            Assert.IsTrue(expressionResult.Equals(new RDFTypedLiteral("POLYGON ((12.49817173063777 41.90282140108903, 12.498147300551285 41.902557324489884, 12.498054378291041 41.90230188254452, 12.497896535552954 41.90206489160506, 12.49767983867993 41.901855458864574, 12.49741261545427 41.901681632408774, 12.497105135028681 41.90155009197772, 12.496769213301608 41.90146589231486, 12.49641775890005 41.90143226895897, 12.49606427720823 41.90145051393566, 12.49572235148675 41.90151992612099, 12.495405121003762 41.901637838182154, 12.495124776213766 41.901799719060534, 12.494892090366731 41.901999348061835, 12.494716005535153 41.90222905386862, 12.494603288962162 41.90248000929714, 12.494558272939003 41.90274257047812, 12.494582688217577 41.90300664743375, 12.494675597375087 41.90326209181446, 12.49483343071039 41.90349908689738, 12.495050123312065 41.90370852485864, 12.495317348048093 41.90388235681768, 12.495624835535821 41.90401390219666, 12.495960768801659 41.90409810549884, 12.496312237461938 41.90413173063137, 12.496665733961668 41.90411348529821, 12.497007672785884 41.90404407067753, 12.497324912671658 41.903926154472906, 12.497605261733 41.90376426837376, 12.497837946069668 41.903564633869195, 12.498014023839104 41.903334923114485, 12.498126728873332 41.90308396404666, 12.49817173063777 41.90282140108903))", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT)));
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

            RDFGeoBufferExpression expression = new RDFGeoBufferExpression(
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

            RDFGeoBufferExpression expression = new RDFGeoBufferExpression(
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

            RDFGeoBufferExpression expression = new RDFGeoBufferExpression(
                new RDFVariableExpression(new RDFVariable("?NAPLES")), 150);
            RDFPatternMember expressionResult = expression.ApplyExpression(table.Rows[0]);

            Assert.IsNull(expressionResult);
        }
        #endregion
    }
}