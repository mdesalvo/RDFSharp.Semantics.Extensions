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
                new RDFVariableExpression(new RDFVariable("?MILANROME")), 150); //150mt buffering
            RDFPatternMember expressionResult = expression.ApplyExpression(table.Rows[0]);

            Assert.IsNotNull(expressionResult);
            Assert.IsTrue(expressionResult.Equals(new RDFTypedLiteral("POLYGON ((12.49788184790704 41.90351508992105, 12.498044143334553 41.90327993291395, 12.498141908820765 41.90302564098356, 12.498171388033143 41.90276198656525, 12.498131448943951 41.90249910177084, 12.49802362720531 41.90224708901409, 12.497852067010797 41.90201563279605, 12.497623361735261 41.90181362756685, 12.497346300492278 41.9016488359599, 12.497031530358537 41.901527590526065, 12.496691147247896 41.901454550422784, 12.496338231152247 41.90143252240119, 12.495986343597448 41.90146235296538, 12.495649006609618 41.90154289584493, 12.495339183195364 41.90167105602998, 12.49506877928189 41.90184190867898, 12.494848186241745 41.90204888833471, 9.18697025944553 45.46388756793846, 9.186785158235066 45.464117980929174, 9.186667492685425 45.46436937742787, 9.186621785436962 45.46463209661727, 9.186649793981102 45.46489604243018, 9.186750442970583 45.4651510715228, 9.186919865391387 45.46538738308607, 9.187151551037442 45.46559589551323, 9.187436596602417 45.465768595444445, 9.187764047791786 45.46589884576812, 9.188121320313538 45.46598164073461, 9.188494683565615 45.466013798370575, 9.188869788420012 45.465994082792236, 9.18923221880208 45.465923251712454, 9.189568045845288 45.46580402731401, 9.189864363302235 45.4656409916096, 9.190109783616233 45.465440410314145, 12.49788184790704 41.90351508992105))", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT)));
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
            Assert.IsTrue(expressionResult.Equals(new RDFTypedLiteral("POLYGON ((12.494845060807727 41.90205125322031, 9.186976859924293 45.46388690764068, 9.186796167110636 45.464109028735024, 9.186678452135538 45.46435119307258, 9.186627967051622 45.46460465495307, 9.18664653603718 45.46486026055565, 9.18673348938959 45.4651087785166, 9.186885687579048 45.465341233324665, 9.187097634512973 45.46554922949385, 9.18736167593789 45.46572525480336, 9.187668275826786 45.46586295164868, 9.188006360777331 45.4659573466977, 9.188363719983087 45.46600503055107, 9.188727446325695 45.46600428091167, 9.189084402644731 45.46595512480961, 9.189421696328344 45.46585933763237, 9.189727145065508 45.46572037899384, 9.189989716920765 45.46554326776038, 14.269543925770376 40.85258943735444, 14.26972342571296 40.852367522424046, 14.269843523809964 40.85212358192707, 14.269899759253745 40.85186667790543, 14.269890043741738 40.85160635387317, 14.26981473894633 40.851352280281674, 14.26967664296726 40.8511138952851, 14.269480886286347 40.85090005415118, 14.269234741104597 40.850718700338035, 14.26894735115574 40.85057657045148, 14.268629392038893 40.85047894403732, 14.26829267468801 40.85042944749678, 14.26794970670175 40.85042991940372, 14.267613227817662 40.85048034222241, 14.267295736771528 40.85057884295964, 14.267009027101423 40.85072176272721, 12.495261666389668 41.90171245787068, 12.495033908409827 41.90186850344531, 12.494845060807727 41.90205125322031))", RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT)));
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