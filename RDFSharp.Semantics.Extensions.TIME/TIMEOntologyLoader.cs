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
            
            return classModel;
        }

        /// <summary>
        /// Builds a reference temporal property model
        /// </summary>
        internal static OWLOntologyPropertyModel BuildTIMEPropertyModel(OWLOntologyPropertyModel existingPropertyModel = null)
        {
            OWLOntologyPropertyModel propertyModel = existingPropertyModel ?? new OWLOntologyPropertyModel();

            //W3C TIME            

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