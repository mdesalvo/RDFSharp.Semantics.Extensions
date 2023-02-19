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
            row["?ROME"] = new RDFTypedLiteral("POINT (12.496365 41.902782)", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT).ToString();
            row["?MILAN"] = new RDFTypedLiteral("<gml:Point xmlns:gml=\"http://www.opengis.net/gml/3.2\"><gml:pos>9.18854 45.464664</gml:pos></gml:Point>", RDFModelEnums.RDFDatatypes.GEOSPARQL_GML).ToString();
            table.Rows.Add(row);
            table.AcceptChanges();

            GEOBufferExpression expression = new GEOBufferExpression(
                new RDFVariable("?MILAN"), 150); //150mt buffering
            RDFPatternMember expressionResult = expression.ApplyExpression(table.Rows[0]);

            Assert.IsNotNull(expressionResult);
            Assert.IsTrue(expressionResult.Equals(new RDFTypedLiteral("POLYGON ((9.190458745339022 45.4646608168706, 9.190420990390798 45.46439747298269, 9.190310951051076 45.46414437210139, 9.190132856911536 45.46391124055206, 9.189893552589094 45.46370703715302, 9.189602234606026 45.46353960897832, 9.18927009794365 45.463415389852045, 9.18890990585676 45.463339153152845, 9.188535499475796 45.46331382842033, 9.188161266029288 45.46334038880383, 9.18780158610276 45.46341781367509, 9.18747028115249 45.4635431278392, 9.187180082482826 45.46371151583948, 9.186942142074404 45.46391650696756, 9.186765604048947 45.46415022387612, 9.18665725323457 45.464403685248165, 9.186621254341752 45.46466715089942, 9.18665899178654 45.464930496058535, 9.186769016336168 45.46518360044646, 9.186947100650608 45.46541673720395, 9.18718640160873 45.46562094671981, 9.187477723200441 45.465788380988776, 9.187809869895045 45.465912605258936, 9.18817007691043 45.46598884536853, 9.188544500843427 45.46601417125878, 9.188918751793366 45.465987609604056, 9.189278446509991 45.46591018122619, 9.189609761285363 45.465784861853905, 9.189899963319347 45.4656164677368, 9.190137900119124 45.465411470514475, 9.190314428112355 45.465177748462025, 9.190422763998235 45.464924283679565, 9.190458745339022 45.4646608168706))", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT)));
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