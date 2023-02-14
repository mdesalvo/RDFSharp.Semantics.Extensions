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
using System.Collections.Generic;
using System.Linq;

namespace RDFSharp.Semantics.Extensions.SKOS
{
    /// <summary>
    /// SKOSConceptSchemeLoader is responsible for loading SKOS concept schemes from remote sources or alternative representations
    /// </summary>
    internal static class SKOSConceptSchemeLoader
    {
        #region Methods
        /// <summary>
        /// Gets a concept scheme representation of the given graph
        /// </summary>
        internal static SKOSConceptScheme FromRDFGraph(RDFGraph graph, OWLOntologyLoaderOptions loaderOptions)
        {
            if (graph == null)
                throw new OWLSemanticsException("Cannot get concept scheme from RDFGraph because given \"graph\" parameter is null");

            //Get OWL ontology with SKOS extension points
            OWLOntology ontology = OWLOntologyLoader.FromRDFGraph(graph, loaderOptions,
               classModelExtensionPoint: SKOSClassModelExtensionPoint,
               propertyModelExtensionPoint: SKOSPropertyModelExtensionPoint,
               dataExtensionPoint: SKOSDataExtensionPoint);

            //Build SKOS concept scheme from OWL ontology
            RDFResource conceptSchemeURI = graph[null, RDFVocabulary.RDF.TYPE, RDFVocabulary.SKOS.CONCEPT_SCHEME, null]
                                             .FirstOrDefault()?.Subject as RDFResource ?? ontology;
            SKOSConceptScheme conceptScheme = new SKOSConceptScheme(conceptSchemeURI.ToString()) { Ontology = ontology };
            conceptScheme.Ontology.Data.DeclareIndividual(conceptScheme);
            conceptScheme.Ontology.Data.DeclareIndividualType(conceptScheme, RDFVocabulary.SKOS.CONCEPT_SCHEME);

            return conceptScheme;
        }
        #endregion

        #region Utilities
        /// <summary>
        /// Extends OWL class model loading with support for SKOS artifacts
        /// </summary>
        internal static void SKOSClassModelExtensionPoint(OWLOntology ontology, RDFGraph graph)
            => ontology.Model.ClassModel = BuildSKOSClassModel(ontology.Model.ClassModel);

        /// <summary>
        /// Extends OWL property model loading with support for SKOS artifacts
        /// </summary>
        internal static void SKOSPropertyModelExtensionPoint(OWLOntology ontology, RDFGraph graph)
            => ontology.Model.PropertyModel = BuildSKOSPropertyModel(ontology.Model.PropertyModel);

        /// <summary>
        /// Extends OWL data loading with support for SKOS artifacts
        /// </summary>
        internal static void SKOSDataExtensionPoint(OWLOntology ontology, RDFGraph graph)
        {
            //skos:Collection
            foreach (RDFTriple typeCollection in graph[null, RDFVocabulary.RDF.TYPE, RDFVocabulary.SKOS.COLLECTION, null])
                foreach (RDFTriple memberRelation in graph[(RDFResource)typeCollection.Subject, RDFVocabulary.SKOS.MEMBER, null, null])
                    ontology.Data.DeclareObjectAssertion((RDFResource)typeCollection.Subject, RDFVocabulary.SKOS.MEMBER, (RDFResource)memberRelation.Object);

            //skos:OrderedCollection
            foreach (RDFTriple typeOrderedCollection in graph[null, RDFVocabulary.RDF.TYPE, RDFVocabulary.SKOS.ORDERED_COLLECTION, null])
                foreach (RDFTriple memberListRelation in graph[(RDFResource)typeOrderedCollection.Subject, RDFVocabulary.SKOS.MEMBER_LIST, null, null])
                {
                    RDFCollection skosOrderedCollection = RDFModelUtilities.DeserializeCollectionFromGraph(graph, (RDFResource)memberListRelation.Object, RDFModelEnums.RDFTripleFlavors.SPO);
                    if (skosOrderedCollection.ItemsCount > 0)
                    {
                        ontology.Data.ABoxGraph.AddCollection(skosOrderedCollection);
                        ontology.Data.DeclareObjectAssertion((RDFResource)typeOrderedCollection.Subject, RDFVocabulary.SKOS.MEMBER_LIST, skosOrderedCollection.ReificationSubject);
                    }
                }
        }

        /// <summary>
        /// Builds a reference SKOS model
        /// </summary>
        internal static OWLOntologyModel BuildSKOSModel()
            => new OWLOntologyModel() { ClassModel = BuildSKOSClassModel(), PropertyModel = BuildSKOSPropertyModel() };

        /// <summary>
        /// Builds a reference SKOS class model
        /// </summary>
        internal static OWLOntologyClassModel BuildSKOSClassModel(OWLOntologyClassModel existingClassModel=null)
        {
            OWLOntologyClassModel classModel = existingClassModel ?? new OWLOntologyClassModel();

            //SKOS
            classModel.DeclareClass(RDFVocabulary.SKOS.COLLECTION);
            classModel.DeclareClass(RDFVocabulary.SKOS.CONCEPT);
            classModel.DeclareClass(RDFVocabulary.SKOS.CONCEPT_SCHEME);
            classModel.DeclareClass(RDFVocabulary.SKOS.ORDERED_COLLECTION);
            classModel.DeclareUnionClass(new RDFResource("bnode:ConceptCollection"), new List<RDFResource>() { RDFVocabulary.SKOS.CONCEPT, RDFVocabulary.SKOS.COLLECTION });
            classModel.DeclareCardinalityRestriction(new RDFResource("bnode:ExactlyOneLiteralForm"), RDFVocabulary.SKOS.SKOSXL.LITERAL_FORM, 1);
            classModel.DeclareAllDisjointClasses(new RDFResource("bnode:AllDisjointSKOSClasses"), new List<RDFResource>() { RDFVocabulary.SKOS.COLLECTION, RDFVocabulary.SKOS.CONCEPT, RDFVocabulary.SKOS.CONCEPT_SCHEME, RDFVocabulary.SKOS.SKOSXL.LABEL });
            classModel.DeclareSubClasses(RDFVocabulary.SKOS.ORDERED_COLLECTION, RDFVocabulary.SKOS.COLLECTION);

            //SKOS-XL
            classModel.DeclareClass(RDFVocabulary.SKOS.SKOSXL.LABEL);
            classModel.DeclareSubClasses(RDFVocabulary.SKOS.SKOSXL.LABEL, new RDFResource("bnode:ExactlyOneLiteralForm"));

            return classModel;
        }

        /// <summary>
        /// Builds a reference SKOS property model
        /// </summary>
        internal static OWLOntologyPropertyModel BuildSKOSPropertyModel(OWLOntologyPropertyModel existingPropertyModel=null)
        {
            OWLOntologyPropertyModel propertyModel = existingPropertyModel ?? new OWLOntologyPropertyModel();

            //SKOS
            propertyModel.DeclareAnnotationProperty(RDFVocabulary.SKOS.ALT_LABEL);
            propertyModel.DeclareObjectProperty(RDFVocabulary.SKOS.BROAD_MATCH);
            propertyModel.DeclareObjectProperty(RDFVocabulary.SKOS.BROADER);
            propertyModel.DeclareObjectProperty(RDFVocabulary.SKOS.BROADER_TRANSITIVE, new OWLOntologyObjectPropertyBehavior() { Transitive = true });
            propertyModel.DeclareAnnotationProperty(RDFVocabulary.SKOS.CHANGE_NOTE);
            propertyModel.DeclareObjectProperty(RDFVocabulary.SKOS.CLOSE_MATCH, new OWLOntologyObjectPropertyBehavior() { Symmetric = true });
            propertyModel.DeclareAnnotationProperty(RDFVocabulary.SKOS.DEFINITION);
            propertyModel.DeclareAnnotationProperty(RDFVocabulary.SKOS.EDITORIAL_NOTE);
            propertyModel.DeclareObjectProperty(RDFVocabulary.SKOS.EXACT_MATCH, new OWLOntologyObjectPropertyBehavior() { Symmetric = true, Transitive = true });
            propertyModel.DeclareAnnotationProperty(RDFVocabulary.SKOS.EXAMPLE);
            propertyModel.DeclareObjectProperty(RDFVocabulary.SKOS.HAS_TOP_CONCEPT, new OWLOntologyObjectPropertyBehavior() { Domain = RDFVocabulary.SKOS.CONCEPT_SCHEME, Range = RDFVocabulary.SKOS.CONCEPT });
            propertyModel.DeclareAnnotationProperty(RDFVocabulary.SKOS.HIDDEN_LABEL);
            propertyModel.DeclareAnnotationProperty(RDFVocabulary.SKOS.HISTORY_NOTE);
            propertyModel.DeclareObjectProperty(RDFVocabulary.SKOS.NARROW_MATCH);
            propertyModel.DeclareObjectProperty(RDFVocabulary.SKOS.NARROWER);
            propertyModel.DeclareObjectProperty(RDFVocabulary.SKOS.NARROWER_TRANSITIVE, new OWLOntologyObjectPropertyBehavior() { Transitive = true });
            propertyModel.DeclareObjectProperty(RDFVocabulary.SKOS.IN_SCHEME, new OWLOntologyObjectPropertyBehavior() { Range = RDFVocabulary.SKOS.CONCEPT_SCHEME });
            propertyModel.DeclareObjectProperty(RDFVocabulary.SKOS.MAPPING_RELATION);
            propertyModel.DeclareObjectProperty(RDFVocabulary.SKOS.MEMBER, new OWLOntologyObjectPropertyBehavior() { Domain = RDFVocabulary.SKOS.COLLECTION, Range = new RDFResource("bnode:ConceptCollection") });
            propertyModel.DeclareObjectProperty(RDFVocabulary.SKOS.MEMBER_LIST, new OWLOntologyObjectPropertyBehavior() { Functional = true, Domain = RDFVocabulary.SKOS.ORDERED_COLLECTION });
            propertyModel.DeclareDatatypeProperty(RDFVocabulary.SKOS.NOTATION);
            propertyModel.DeclareAnnotationProperty(RDFVocabulary.SKOS.NOTE);
            propertyModel.DeclareAnnotationProperty(RDFVocabulary.SKOS.PREF_LABEL);
            propertyModel.DeclareObjectProperty(RDFVocabulary.SKOS.RELATED_MATCH, new OWLOntologyObjectPropertyBehavior() { Symmetric = true });
            propertyModel.DeclareObjectProperty(RDFVocabulary.SKOS.RELATED, new OWLOntologyObjectPropertyBehavior() { Symmetric = true });
            propertyModel.DeclareAnnotationProperty(RDFVocabulary.SKOS.SCOPE_NOTE);
            propertyModel.DeclareObjectProperty(RDFVocabulary.SKOS.SEMANTIC_RELATION, new OWLOntologyObjectPropertyBehavior() { Domain = RDFVocabulary.SKOS.CONCEPT, Range = RDFVocabulary.SKOS.CONCEPT });
            propertyModel.DeclareObjectProperty(RDFVocabulary.SKOS.TOP_CONCEPT_OF, new OWLOntologyObjectPropertyBehavior() { Domain = RDFVocabulary.SKOS.CONCEPT, Range = RDFVocabulary.SKOS.CONCEPT_SCHEME });
            propertyModel.DeclareSubProperties(RDFVocabulary.SKOS.BROAD_MATCH, RDFVocabulary.SKOS.BROADER);
            propertyModel.DeclareSubProperties(RDFVocabulary.SKOS.BROAD_MATCH, RDFVocabulary.SKOS.MAPPING_RELATION);
            propertyModel.DeclareSubProperties(RDFVocabulary.SKOS.BROADER, RDFVocabulary.SKOS.BROADER_TRANSITIVE);
            propertyModel.DeclareSubProperties(RDFVocabulary.SKOS.BROADER_TRANSITIVE, RDFVocabulary.SKOS.SEMANTIC_RELATION);
            propertyModel.DeclareSubProperties(RDFVocabulary.SKOS.CLOSE_MATCH, RDFVocabulary.SKOS.MAPPING_RELATION);
            propertyModel.DeclareSubProperties(RDFVocabulary.SKOS.EXACT_MATCH, RDFVocabulary.SKOS.CLOSE_MATCH);
            propertyModel.DeclareSubProperties(RDFVocabulary.SKOS.MAPPING_RELATION, RDFVocabulary.SKOS.SEMANTIC_RELATION);
            propertyModel.DeclareSubProperties(RDFVocabulary.SKOS.NARROW_MATCH, RDFVocabulary.SKOS.NARROWER);
            propertyModel.DeclareSubProperties(RDFVocabulary.SKOS.NARROW_MATCH, RDFVocabulary.SKOS.MAPPING_RELATION);
            propertyModel.DeclareSubProperties(RDFVocabulary.SKOS.NARROWER, RDFVocabulary.SKOS.NARROWER_TRANSITIVE);
            propertyModel.DeclareSubProperties(RDFVocabulary.SKOS.NARROWER_TRANSITIVE, RDFVocabulary.SKOS.SEMANTIC_RELATION);
            propertyModel.DeclareSubProperties(RDFVocabulary.SKOS.TOP_CONCEPT_OF, RDFVocabulary.SKOS.IN_SCHEME);
            propertyModel.DeclareSubProperties(RDFVocabulary.SKOS.RELATED_MATCH, RDFVocabulary.SKOS.RELATED);
            propertyModel.DeclareSubProperties(RDFVocabulary.SKOS.RELATED_MATCH, RDFVocabulary.SKOS.MAPPING_RELATION);
            propertyModel.DeclareSubProperties(RDFVocabulary.SKOS.RELATED, RDFVocabulary.SKOS.SEMANTIC_RELATION);
            propertyModel.DeclareInverseProperties(RDFVocabulary.SKOS.BROAD_MATCH, RDFVocabulary.SKOS.NARROW_MATCH);
            propertyModel.DeclareInverseProperties(RDFVocabulary.SKOS.BROADER, RDFVocabulary.SKOS.NARROWER);
            propertyModel.DeclareInverseProperties(RDFVocabulary.SKOS.BROADER_TRANSITIVE, RDFVocabulary.SKOS.NARROWER_TRANSITIVE);
            propertyModel.DeclareInverseProperties(RDFVocabulary.SKOS.HAS_TOP_CONCEPT, RDFVocabulary.SKOS.TOP_CONCEPT_OF);
            propertyModel.DeclareDisjointProperties(RDFVocabulary.SKOS.RELATED, RDFVocabulary.SKOS.BROADER_TRANSITIVE);
            propertyModel.DeclareDisjointProperties(RDFVocabulary.SKOS.RELATED, RDFVocabulary.SKOS.NARROWER_TRANSITIVE);
            propertyModel.DeclareDisjointProperties(RDFVocabulary.SKOS.EXACT_MATCH, RDFVocabulary.SKOS.BROAD_MATCH);
            propertyModel.DeclareDisjointProperties(RDFVocabulary.SKOS.EXACT_MATCH, RDFVocabulary.SKOS.NARROW_MATCH);
            propertyModel.DeclareDisjointProperties(RDFVocabulary.SKOS.EXACT_MATCH, RDFVocabulary.SKOS.RELATED_MATCH);
            propertyModel.DeclareAllDisjointProperties(new RDFResource("bnode:AllDisjointSKOSLabelingProperties"), new List<RDFResource>() { RDFVocabulary.SKOS.PREF_LABEL, RDFVocabulary.SKOS.ALT_LABEL, RDFVocabulary.SKOS.HIDDEN_LABEL });

            //SKOS-XL
            propertyModel.DeclareDatatypeProperty(RDFVocabulary.SKOS.SKOSXL.LITERAL_FORM, new OWLOntologyDatatypePropertyBehavior() { Domain = RDFVocabulary.SKOS.SKOSXL.LABEL });
            propertyModel.DeclareObjectProperty(RDFVocabulary.SKOS.SKOSXL.PREF_LABEL, new OWLOntologyObjectPropertyBehavior() { Range = RDFVocabulary.SKOS.SKOSXL.LABEL });
            propertyModel.DeclareObjectProperty(RDFVocabulary.SKOS.SKOSXL.ALT_LABEL, new OWLOntologyObjectPropertyBehavior() { Range = RDFVocabulary.SKOS.SKOSXL.LABEL });
            propertyModel.DeclareObjectProperty(RDFVocabulary.SKOS.SKOSXL.HIDDEN_LABEL, new OWLOntologyObjectPropertyBehavior() { Range = RDFVocabulary.SKOS.SKOSXL.LABEL });
            propertyModel.DeclareObjectProperty(RDFVocabulary.SKOS.SKOSXL.LABEL_RELATION, new OWLOntologyObjectPropertyBehavior() { Symmetric = true, Domain = RDFVocabulary.SKOS.SKOSXL.LABEL, Range = RDFVocabulary.SKOS.SKOSXL.LABEL });

            return propertyModel;
        }
        #endregion
    }
}