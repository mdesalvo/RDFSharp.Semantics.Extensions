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
            Assert.IsTrue(expressionResult.Equals(new RDFTypedLiteral("POLYGON ((9.201331629406859 45.4646421703495, 9.201079599158819 45.46288656848276, 9.200345724263068 45.46119929554034, 9.19915824450492 45.459645183600294, 9.19756281888693 45.45828394313989, 9.195620767205316 45.457167870400895, 9.193406712277415 45.45633984002811, 9.19100571362246 45.45583165976439, 9.188510002533777 45.455662850074326, 9.18601544342394 45.45583989529432, 9.183617856558405 45.45635599488805, 9.181409342467 45.45719132429876, 9.17947474826111 45.458313795448205, 9.177888410788967 45.459680287861474, 9.176711301212148 45.46123830341017, 9.17598868053686 45.462927981441155, 9.175748356405366 45.46468439720692, 9.175999608723036 45.4664400555719, 9.176732826281208 45.46812748436393, 9.177919869368251 45.46968182777819, 9.179515145460835 45.471043340097616, 9.18145735752664 45.47215968369233, 9.183671858331309 45.47298794268226, 9.186073520474917 45.47349627451958, 9.188570011653997 45.47366513566404, 9.19106534869427 45.47348803394578, 9.19346359289741 45.47297177850244, 9.19567254365989 45.47213621761748, 9.19760728739112 45.47101347460901, 9.199193464479006 45.46964671134004, 9.20037012817903 45.468088467174226, 9.201092085365696 45.46639863756963, 9.201331629406859 45.4646421703495))", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT)));
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

            GEOBufferExpression expression = new GEOBufferExpression(
                new RDFVariable("?ROME"), 150); //150mt buffering
            RDFPatternMember expressionResult = expression.ApplyExpression(table.Rows[0]);

            Assert.IsNotNull(expressionResult);
            Assert.IsTrue(expressionResult.Equals(new RDFTypedLiteral("POLYGON ((12.49817173063777 41.902821401089035, 12.498147300551285 41.90255732448988, 12.49805437829104 41.90230188254452, 12.497896535552954 41.90206489160506, 12.49767983867993 41.901855458864574, 12.49741261545427 41.901681632408774, 12.497105135028683 41.90155009197772, 12.496769213301608 41.90146589231487, 12.49641775890005 41.90143226895897, 12.49606427720823 41.90145051393566, 12.49572235148675 41.90151992612099, 12.495405121003762 41.901637838182154, 12.495124776213766 41.901799719060534, 12.494892090366733 41.901999348061835, 12.494716005535153 41.90222905386862, 12.494603288962162 41.90248000929714, 12.494558272939003 41.90274257047812, 12.494582688217577 41.90300664743375, 12.494675597375087 41.90326209181446, 12.49483343071039 41.90349908689738, 12.495050123312065 41.90370852485864, 12.495317348048093 41.90388235681768, 12.495624835535821 41.90401390219665, 12.495960768801657 41.90409810549884, 12.496312237461938 41.90413173063137, 12.496665733961668 41.90411348529821, 12.497007672785884 41.90404407067753, 12.497324912671658 41.903926154472906, 12.497605261733 41.90376426837376, 12.497837946069668 41.903564633869195, 12.498014023839104 41.903334923114485, 12.498126728873332 41.90308396404666, 12.49817173063777 41.902821401089035))", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT)));
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
                new RDFVariableExpression(new RDFVariable("?MILANROME")), 1000); //1000mt buffering
            RDFPatternMember expressionResult = expression.ApplyExpression(table.Rows[0]);

            Assert.IsNotNull(expressionResult);
            Assert.IsTrue(expressionResult.Equals(new RDFTypedLiteral("POLYGON ((12.506477960801005 41.907668861299705, 12.50755975569211 41.9061010693807, 12.508211279847721 41.904405743698696, 12.508407528474216 41.90264804084563, 12.50814099726765 41.90089550925427, 12.50742196510212 41.89921549321465, 12.506278093504282 41.89767254555982, 12.50475335914505 41.89632594834453, 12.502906361054816 41.89522743658098, 12.500808068037694 41.89441921221637, 12.498539092949423 41.89393232435055, 12.496186598347403 41.89378547763489, 12.493840951885758 41.89398431440214, 12.491592259225971 41.894521197972, 12.48952690683665 41.895375505439155, 12.487724246703175 41.896514418796215, 12.486253549668737 41.897894184202315, 9.178075876317909 45.459487375556385, 9.17684169359804 45.46102336168162, 9.176056984705198 45.4626992688727, 9.175751941954976 45.464450702259214, 9.175938331849403 45.466210358974514, 9.176609036218348 45.46791061399663, 9.177738318962149 45.46948611930374, 9.179282809181101 45.4708763164053, 9.181183163809242 45.4720277655049, 9.183366346482085 45.47289620149181, 9.185748435315949 45.47344823739594, 9.188237851541134 45.473662649481696, 9.190738884372063 45.473531194282266, 9.19315537580975 45.47305892596012, 9.19539442275144 45.47226400171053, 9.1973699530988 45.471176982742925, 9.199006037538265 45.469839657893, 12.506477960801005 41.907668861299705))", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT)));
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
                new RDFVariableExpression(new RDFVariable("?ROMENAPLESMILAN")), 1000); //1000mt buffering
            RDFPatternMember expressionResult = expression.ApplyExpression(table.Rows[0]);

            Assert.IsNotNull(expressionResult);
            Assert.IsTrue(expressionResult.Equals(new RDFTypedLiteral("POLYGON ((12.486232738357756 41.89790999076356, 9.178119895089319 45.45948303579323, 9.17691513032443 45.46096373610627, 9.176130119091432 45.46257808210041, 9.175793243430116 45.46426778097997, 9.175916708677354 45.465971814003574, 9.176496097105138 45.467628639369856, 9.177510521556114 45.46917841476279, 9.178923374374836 45.47056515929989, 9.1806836453637 45.47173877670286, 9.182727761760914 45.47265686642496, 9.184981884117702 45.473286257057744, 9.187364575172545 45.47360420633875, 9.189789745032183 45.47359922413452, 9.19216976571659 45.47327148844236, 9.194418641817109 45.4726328392394, 9.19645512190946 45.47170635036127, 9.198205637543836 45.47052549494596, 14.277588564305137 40.857206522419794, 14.27878511466525 40.855727000836424, 14.279585572753934 40.85410066468277, 14.27996022870166 40.852387937311626, 14.279895196859892 40.8506524471365, 14.279392926911166 40.84895866333262, 14.27847210787957 40.84736950101606, 14.277166969341788 40.84594398485324, 14.275526006463744 40.844735057779005, 14.273610176725946 40.84378761602553, 14.271490635586963 40.843136843199645, 14.269246095184897 40.842806905019046, 14.26695990390506 40.842810052940465, 14.264716954779512 40.84314616978276, 14.26260053688723 40.8438027741158, 14.26068924598528 40.84475548324644, 12.48901013224861 41.89565153432839, 12.487491775806484 41.89669174837175, 12.486232738357756 41.89790999076356))", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT)));
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