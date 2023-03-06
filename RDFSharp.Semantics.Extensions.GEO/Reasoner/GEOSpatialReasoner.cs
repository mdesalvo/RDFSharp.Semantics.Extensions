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

using System.Collections.Generic;
using System.Threading.Tasks;

namespace RDFSharp.Semantics.Extensions.GEO
{
    /// <summary>
    /// GEOSpatialReasoner analyzes a geospatial ontology in order to infer knowledge from its model and data
    /// </summary>
    public class GEOSpatialReasoner
    {
        #region Properties
        /// <summary>
        /// List of geospatial rules applied by the reasoner
        /// </summary>
        internal List<GEOEnums.GEOSpatialReasonerRules> GeoSpatialRules { get; set; }
        #endregion

        #region Ctors
        /// <summary>
        /// Default-ctor to build an empty geospatial reasoner
        /// </summary>
        public GEOSpatialReasoner() : base()
            => GeoSpatialRules = new List<GEOEnums.GEOSpatialReasonerRules>();
        #endregion

        #region Methods
        /// <summary>
        /// Adds the given rule to the geospatial reasoner
        /// </summary>
        public GEOSpatialReasoner AddGEOSpatialRule(GEOEnums.GEOSpatialReasonerRules geoSpatialRule)
        {
            if (!GeoSpatialRules.Contains(geoSpatialRule))
                GeoSpatialRules.Add(geoSpatialRule);
            return this;
        }

        /// <summary>
        /// Applies the geospatial reasoner on the given geospatial ontology
        /// </summary>
        public OWLReasonerReport ApplyToOntology(GEOOntology geoOntology, OWLOntologyLoaderOptions loaderOptions=null)
        {
            OWLReasonerReport reasonerReport = new OWLReasonerReport();

            if (geoOntology != null)
            {
                OWLSemanticsEvents.RaiseSemanticsInfo($"GEOSpatialReasoner is going to be applied on geospatial ontology '{geoOntology.URI}': this may require intensive processing, depending on size and complexity of domain knowledge and rules");

                //Initialize inference registry
                Dictionary<string, OWLReasonerReport> inferenceRegistry = new Dictionary<string, OWLReasonerReport>();
                foreach (GEOEnums.GEOSpatialReasonerRules geoSpatialRule in GeoSpatialRules)
                    inferenceRegistry.Add(geoSpatialRule.ToString(), null);
                
                //Execute geospatial rules
                Parallel.ForEach(GeoSpatialRules,
                    geoSpatialRule =>
                    {
                        OWLSemanticsEvents.RaiseSemanticsInfo($"Launching geospatial reasoner rule '{geoSpatialRule}'");

                        switch (geoSpatialRule)
                        {
                            
                        }

                        OWLSemanticsEvents.RaiseSemanticsInfo($"Completed geospatial reasoner rule '{geoSpatialRule}': found {inferenceRegistry[geoSpatialRule.ToString()].EvidencesCount} evidences");
                    });

                //Process inference registry
                foreach (OWLReasonerReport inferenceRegistryReport in inferenceRegistry.Values)
                    reasonerReport.MergeEvidences(inferenceRegistryReport);

                OWLSemanticsEvents.RaiseSemanticsInfo($"GEOSpatialReasoner has been applied on geospatial ontology '{geoOntology.URI}': found {reasonerReport.EvidencesCount} evidences");
            }

            return reasonerReport;
        }

        /// <summary>
        /// Asynchronously applies the geospatial reasoner on the given geospatial ontology
        /// </summary>
        public Task<OWLReasonerReport> ApplyToOntologyAsync(GEOOntology geoOntology, OWLOntologyLoaderOptions loaderOptions=null)
            => Task.Run(() => ApplyToOntology(geoOntology, loaderOptions));
        #endregion
    }
}