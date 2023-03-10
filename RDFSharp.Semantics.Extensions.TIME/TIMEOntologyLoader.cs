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

using RDFSharp.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RDFSharp.Semantics.Extensions.TIME
{
    /// <summary>
    /// TIMEOntologyLoader is responsible for loading temporal ontologies from remote sources or alternative representations
    /// </summary>
    internal static class TIMEOntologyLoader
    {
        #region Methods
        /// <summary>
        /// Gets a temporal ontology representation of the given graph
        /// </summary>
        internal static TIMEOntology FromRDFGraph(RDFGraph graph, OWLOntologyLoaderOptions loaderOptions)
        {
            if (graph == null)
                throw new OWLSemanticsException("Cannot get TIME ontology from RDFGraph because given \"graph\" parameter is null");

            //Get OWL ontology with TIME extension points
            OWLOntology ontology = OWLOntologyLoader.FromRDFGraph(graph, loaderOptions,
               classModelExtensionPoint: TIMEClassModelExtensionPoint,
               propertyModelExtensionPoint: TIMEPropertyModelExtensionPoint,
               dataExtensionPoint: TIMEDataExtensionPoint);

            //Build GEO ontology from OWL ontology
            TIMEOntology geoOntology = new TIMEOntology(ontology.ToString()) { 
                Model = ontology.Model, 
                Data = ontology.Data, 
                OBoxGraph = ontology.OBoxGraph
            };

            return geoOntology;
        }
        #endregion

        #region Utilities
        /// <summary>
        /// Extends OWL class model loading with support for temporal entities
        /// </summary>
        internal static void TIMEClassModelExtensionPoint(OWLOntology ontology, RDFGraph graph)
            => BuildTIMEClassModel(ontology.Model.ClassModel);

        /// <summary>
        /// Extends OWL property model loading with support for temporal entities
        /// </summary>
        internal static void TIMEPropertyModelExtensionPoint(OWLOntology ontology, RDFGraph graph)
            => BuildTIMEPropertyModel(ontology.Model.PropertyModel);

        /// <summary>
        /// Extends OWL data loading with support for temporal entities
        /// </summary>
        internal static void TIMEDataExtensionPoint(OWLOntology ontology, RDFGraph graph)
            => BuildTIMEData(ontology.Data);

        /// <summary>
        /// Builds a reference temporal model
        /// </summary>
        internal static OWLOntologyModel BuildTIMEModel()
            => new OWLOntologyModel() { ClassModel = BuildTIMEClassModel(), PropertyModel = BuildTIMEPropertyModel() };

        /// <summary>
        /// Builds a reference temporal class model
        /// </summary>
        internal static OWLOntologyClassModel BuildTIMEClassModel(OWLOntologyClassModel existingClassModel = null)
        {
            OWLOntologyClassModel classModel = existingClassModel ?? new OWLOntologyClassModel();

            //W3C TIME
            classModel.DeclareClass(RDFVocabulary.TIME.DATETIME_DESCRIPTION);
            classModel.DeclareClass(RDFVocabulary.TIME.DATETIME_INTERVAL);
            classModel.DeclareClass(RDFVocabulary.TIME.DAY_OF_WEEK_CLASS);
            classModel.DeclareClass(RDFVocabulary.TIME.DURATION);
            classModel.DeclareClass(RDFVocabulary.TIME.DURATION_DESCRIPTION);
            classModel.DeclareClass(RDFVocabulary.TIME.GENERAL_DATETIME_DESCRIPTION);
            classModel.DeclareClass(RDFVocabulary.TIME.GENERAL_DURATION_DESCRIPTION);
            classModel.DeclareClass(RDFVocabulary.TIME.INSTANT);
            classModel.DeclareClass(RDFVocabulary.TIME.INTERVAL);
            classModel.DeclareClass(RDFVocabulary.TIME.MONTH_OF_YEAR_CLASS);
            classModel.DeclareClass(RDFVocabulary.TIME.PROPER_INTERVAL);
            classModel.DeclareClass(RDFVocabulary.TIME.TEMPORAL_DURATION);
            classModel.DeclareClass(RDFVocabulary.TIME.TEMPORAL_ENTITY);
            classModel.DeclareClass(RDFVocabulary.TIME.TEMPORAL_POSITION);
            classModel.DeclareClass(RDFVocabulary.TIME.TEMPORAL_UNIT);
            classModel.DeclareClass(RDFVocabulary.TIME.TIMEZONE_CLASS);
            classModel.DeclareClass(RDFVocabulary.TIME.TIME_POSITION);
            classModel.DeclareClass(RDFVocabulary.TIME.TRS);
            classModel.DeclareUnionClass(RDFVocabulary.TIME.TEMPORAL_ENTITY, new List<RDFResource>() { 
                RDFVocabulary.TIME.INSTANT, RDFVocabulary.TIME.INTERVAL });
            classModel.DeclareUnionClass(new RDFResource("bnode:HasTRSDomain"), new List<RDFResource>() {
                RDFVocabulary.TIME.TEMPORAL_POSITION, RDFVocabulary.TIME.GENERAL_DURATION_DESCRIPTION });
            classModel.DeclareSubClasses(RDFVocabulary.TIME.DATETIME_DESCRIPTION, RDFVocabulary.TIME.GENERAL_DATETIME_DESCRIPTION);
            classModel.DeclareSubClasses(RDFVocabulary.TIME.DATETIME_INTERVAL, RDFVocabulary.TIME.PROPER_INTERVAL);
            classModel.DeclareSubClasses(RDFVocabulary.TIME.DURATION, RDFVocabulary.TIME.TEMPORAL_DURATION);
            classModel.DeclareSubClasses(RDFVocabulary.TIME.DURATION_DESCRIPTION, RDFVocabulary.TIME.GENERAL_DURATION_DESCRIPTION);
            classModel.DeclareSubClasses(RDFVocabulary.TIME.GENERAL_DURATION_DESCRIPTION, RDFVocabulary.TIME.TEMPORAL_DURATION);
            classModel.DeclareSubClasses(RDFVocabulary.TIME.INSTANT, RDFVocabulary.TIME.TEMPORAL_ENTITY);
            classModel.DeclareSubClasses(RDFVocabulary.TIME.INTERVAL, RDFVocabulary.TIME.TEMPORAL_ENTITY);
            classModel.DeclareSubClasses(RDFVocabulary.TIME.MONTH_OF_YEAR_CLASS, RDFVocabulary.TIME.DATETIME_DESCRIPTION);
            classModel.DeclareSubClasses(RDFVocabulary.TIME.PROPER_INTERVAL, RDFVocabulary.TIME.INTERVAL);
            classModel.DeclareSubClasses(RDFVocabulary.TIME.TEMPORAL_UNIT, RDFVocabulary.TIME.TEMPORAL_DURATION);
            classModel.DeclareSubClasses(RDFVocabulary.TIME.TIME_POSITION, RDFVocabulary.TIME.TEMPORAL_POSITION);
            classModel.DeclareSubClasses(RDFVocabulary.TIME.GENERAL_DATETIME_DESCRIPTION, RDFVocabulary.TIME.TEMPORAL_POSITION);
            classModel.DeclareDisjointClasses(RDFVocabulary.TIME.PROPER_INTERVAL, RDFVocabulary.TIME.INSTANT);

            return classModel;
        }

        /// <summary>
        /// Builds a reference temporal property model
        /// </summary>
        internal static OWLOntologyPropertyModel BuildTIMEPropertyModel(OWLOntologyPropertyModel existingPropertyModel = null)
        {
            OWLOntologyPropertyModel propertyModel = existingPropertyModel ?? new OWLOntologyPropertyModel();

            //W3C TIME            
            propertyModel.DeclareObjectProperty(RDFVocabulary.TIME.AFTER, new OWLOntologyObjectPropertyBehavior() { Domain = RDFVocabulary.TIME.TEMPORAL_ENTITY, Range = RDFVocabulary.TIME.TEMPORAL_ENTITY });
            propertyModel.DeclareObjectProperty(RDFVocabulary.TIME.BEFORE, new OWLOntologyObjectPropertyBehavior() { Domain = RDFVocabulary.TIME.TEMPORAL_ENTITY, Range = RDFVocabulary.TIME.TEMPORAL_ENTITY });
            propertyModel.DeclareObjectProperty(RDFVocabulary.TIME.DAY_OF_WEEK, new OWLOntologyObjectPropertyBehavior() { Domain = RDFVocabulary.TIME.GENERAL_DATETIME_DESCRIPTION, Range = RDFVocabulary.TIME.DAY_OF_WEEK_CLASS });
            propertyModel.DeclareObjectProperty(RDFVocabulary.TIME.HAS_BEGINNING, new OWLOntologyObjectPropertyBehavior() { Domain = RDFVocabulary.TIME.TEMPORAL_ENTITY, Range = RDFVocabulary.TIME.INSTANT });
            propertyModel.DeclareObjectProperty(RDFVocabulary.TIME.HAS_DATETIME_DESCRIPTION, new OWLOntologyObjectPropertyBehavior() { Domain = RDFVocabulary.TIME.DATETIME_INTERVAL, Range = RDFVocabulary.TIME.GENERAL_DATETIME_DESCRIPTION });
            propertyModel.DeclareObjectProperty(RDFVocabulary.TIME.HAS_DURATION, new OWLOntologyObjectPropertyBehavior() { Range = RDFVocabulary.TIME.DURATION });
            propertyModel.DeclareObjectProperty(RDFVocabulary.TIME.HAS_DURATION_DESCRIPTION, new OWLOntologyObjectPropertyBehavior() { Range = RDFVocabulary.TIME.DURATION_DESCRIPTION });
            propertyModel.DeclareObjectProperty(RDFVocabulary.TIME.HAS_END, new OWLOntologyObjectPropertyBehavior() { Domain = RDFVocabulary.TIME.TEMPORAL_ENTITY, Range = RDFVocabulary.TIME.INSTANT });
            propertyModel.DeclareObjectProperty(RDFVocabulary.TIME.HAS_TEMPORAL_DURATION, new OWLOntologyObjectPropertyBehavior() { Domain = RDFVocabulary.TIME.TEMPORAL_ENTITY, Range = RDFVocabulary.TIME.TEMPORAL_DURATION });
            propertyModel.DeclareObjectProperty(RDFVocabulary.TIME.HAS_TIME, new OWLOntologyObjectPropertyBehavior() { Range = RDFVocabulary.TIME.TEMPORAL_ENTITY });
            propertyModel.DeclareObjectProperty(RDFVocabulary.TIME.HAS_TRS, new OWLOntologyObjectPropertyBehavior() { Domain = new RDFResource("bnode:HasTRSDomain"), Range = RDFVocabulary.TIME.TRS, Functional = true });

            propertyModel.DeclareDatatypeProperty(RDFVocabulary.TIME.DAY, new OWLOntologyDatatypePropertyBehavior() { Domain = RDFVocabulary.TIME.GENERAL_DATETIME_DESCRIPTION });
            propertyModel.DeclareDatatypeProperty(RDFVocabulary.TIME.DAY_OF_YEAR, new OWLOntologyDatatypePropertyBehavior() { Domain = RDFVocabulary.TIME.GENERAL_DATETIME_DESCRIPTION, Range = RDFVocabulary.XSD.NON_NEGATIVE_INTEGER });
            propertyModel.DeclareDatatypeProperty(RDFVocabulary.TIME.DAYS, new OWLOntologyDatatypePropertyBehavior() { Domain = RDFVocabulary.TIME.GENERAL_DATETIME_DESCRIPTION, Range = RDFVocabulary.XSD.DECIMAL });
            propertyModel.DeclareDatatypeProperty(RDFVocabulary.TIME.HAS_XSD_DURATION, new OWLOntologyDatatypePropertyBehavior() { Domain = RDFVocabulary.TIME.TEMPORAL_ENTITY, Range = RDFVocabulary.XSD.DURATION });
            propertyModel.DeclareDatatypeProperty(RDFVocabulary.TIME.HOUR, new OWLOntologyDatatypePropertyBehavior() { Domain = RDFVocabulary.TIME.GENERAL_DATETIME_DESCRIPTION, Range = RDFVocabulary.XSD.NON_NEGATIVE_INTEGER });
            propertyModel.DeclareDatatypeProperty(RDFVocabulary.TIME.HOURS, new OWLOntologyDatatypePropertyBehavior() { Domain = RDFVocabulary.TIME.GENERAL_DURATION_DESCRIPTION, Range = RDFVocabulary.XSD.DECIMAL });

            propertyModel.DeclareSubProperties(RDFVocabulary.TIME.HAS_DURATION, RDFVocabulary.TIME.HAS_TEMPORAL_DURATION);
            propertyModel.DeclareSubProperties(RDFVocabulary.TIME.HAS_DURATION_DESCRIPTION, RDFVocabulary.TIME.HAS_TEMPORAL_DURATION);
            propertyModel.DeclareInverseProperties(RDFVocabulary.TIME.AFTER, RDFVocabulary.TIME.BEFORE);

            return propertyModel;
        }

        /// <summary>
        /// Builds a reference temporal data
        /// </summary>
        internal static OWLOntologyData BuildTIMEData(OWLOntologyData existingData = null)
        {
            OWLOntologyData data = existingData ?? new OWLOntologyData();

            //W3C TIME            

            return data;
        }
        #endregion
    }
}