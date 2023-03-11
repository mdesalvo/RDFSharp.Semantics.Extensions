/*
   Copyright 2012-2023 Marco De Salvo

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

namespace RDFSharp.Semantics.Extensions.TIME.Test
{
    [TestClass]
    public class TIMEOntologyLoaderTest
    {
        #region Tests
        [TestMethod]
        public void ShouldLoadFromGraph()
        {
            RDFGraph graph = new RDFGraph();

            //OWL knowledge
            graph.AddTriple(new RDFTriple(new RDFResource("ex:City"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.CLASS));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:hasName"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.DATATYPE_PROPERTY));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:connectedToCity"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.OBJECT_PROPERTY));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:connectedToCity"), RDFVocabulary.RDF.TYPE, RDFVocabulary.OWL.REFLEXIVE_PROPERTY));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:connectedToCity"), RDFVocabulary.RDFS.COMMENT, new RDFPlainLiteral("two cities are connected each other")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:milan"), RDFVocabulary.RDF.TYPE, new RDFResource("ex:City")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:milan"), new RDFResource("ex:hasName"), new RDFPlainLiteral("Milano", "it-IT")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:milan"), RDFVocabulary.DC.DESCRIPTION, new RDFPlainLiteral("this is the city of Milan,IT")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:rome"), RDFVocabulary.RDF.TYPE, new RDFResource("ex:City")));
            graph.AddTriple(new RDFTriple(new RDFResource("ex:milan"), new RDFResource("ex:connectedToCity"), new RDFResource("ex:rome")));

            //TIME knowledge
            graph.AddTriple(new RDFTriple(new RDFResource("ex:milanFoundation"), RDFVocabulary.RDF.TYPE, RDFVocabulary.TIME.PROPER_INTERVAL));

            //LOAD OWL+TIME
            TIMEOntology timeOntology = TIMEOntologyLoader.FromRDFGraph(graph, null);

            //Test persistence of OWL+GEO knowledge
            Assert.IsNotNull(timeOntology);
            Assert.IsTrue(timeOntology.URI.Equals(RDFNamespaceRegister.DefaultNamespace.NamespaceUri));
            Assert.IsTrue(timeOntology.Model.ClassModel.ClassesCount == 68);
            Assert.IsTrue(timeOntology.Model.PropertyModel.PropertiesCount == 60);
            Assert.IsTrue(timeOntology.Data.IndividualsCount == 32);
            Assert.IsTrue(timeOntology.Model.PropertyModel.CheckHasAnnotation(new RDFResource("ex:connectedToCity"), RDFVocabulary.RDFS.COMMENT, new RDFPlainLiteral("two cities are connected each other")));
            Assert.IsTrue(timeOntology.Data.CheckHasObjectAssertion(new RDFResource("ex:milan"), new RDFResource("ex:connectedToCity"), new RDFResource("ex:rome")));
            Assert.IsTrue(timeOntology.Data.CheckHasDatatypeAssertion(new RDFResource("ex:milan"), new RDFResource("ex:hasName"), new RDFPlainLiteral("Milano", "it-IT")));
            Assert.IsTrue(timeOntology.Data.CheckHasAnnotation(new RDFResource("ex:milan"), RDFVocabulary.DC.DESCRIPTION, new RDFPlainLiteral("this is the city of Milan,IT")));
        }
        #endregion
    }
}