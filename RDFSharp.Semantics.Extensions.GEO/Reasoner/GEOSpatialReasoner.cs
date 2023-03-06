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
    /// GEOSpatialReasoner analyzes an ontology in order to infer knowledge from its geospatial model and data
    /// </summary>
    public class GEOSpatialReasoner : OWLReasoner
    {
        #region Properties
        /// <summary>
        /// List of geospatial rules applied by the reasoner
        /// </summary>
        internal List<GEOEnums.GEOSpatialReasonerRules> GeoSpatialRules { get; set; }
        #endregion

        #region Ctors
        /// <summary>
        /// Default-ctor to build an empty reasoner
        /// </summary>
        public GEOSpatialReasoner() : base()
            => GeoSpatialRules = new List<GEOEnums.GEOSpatialReasonerRules>();
        #endregion

        #region Methods
        /// <summary>
        /// Adds the given standard rule to the reasoner
        /// </summary>
        public GEOSpatialReasoner AddGEOSPatialRule(GEOEnums.GEOSpatialReasonerRules geoSpatialRule)
        {
            if (!GeoSpatialRules.Contains(geoSpatialRule))
                GeoSpatialRules.Add(geoSpatialRule);
            return this;
        }

        /// <summary>
        /// Applies the geospatial reasoner on the given ontology
        /// </summary>
        public new OWLReasonerReport ApplyToOntology(OWLOntology ontology, OWLOntologyLoaderOptions loaderOptions=null)
        {
            OWLReasonerReport reasonerReport = new OWLReasonerReport();

            if (ontology != null)
            {
                OWLSemanticsEvents.RaiseSemanticsInfo($"Reasoner is going to be applied on Ontology '{ontology.URI}': this may require intensive processing, depending on size and complexity of domain knowledge and rules");

                //Initialize inference registry
                Dictionary<string, OWLReasonerReport> inferenceRegistry = new Dictionary<string, OWLReasonerReport>();
                foreach (OWLSemanticsEnums.OWLReasonerStandardRules standardRule in StandardRules)
                    inferenceRegistry.Add(standardRule.ToString(), null);
                foreach (GEOEnums.GEOSpatialReasonerRules geoSpatialRule in GeoSpatialRules)
                    inferenceRegistry.Add(geoSpatialRule.ToString(), null);
                foreach (OWLReasonerRule customRule in CustomRules)
                    inferenceRegistry.Add(customRule.RuleName, null);

                //Execute standard rules
                Parallel.ForEach(StandardRules, 
                    standardRule =>
                    {
                        OWLSemanticsEvents.RaiseSemanticsInfo($"Launching standard reasoner rule '{standardRule}'");

                        switch (standardRule)
                        {
                            case OWLSemanticsEnums.OWLReasonerStandardRules.SubClassTransitivity:
                                inferenceRegistry[OWLSemanticsEnums.OWLReasonerStandardRules.SubClassTransitivity.ToString()] = OWLSubClassTransitivityRule.ExecuteRule(ontology);
                                break;
                            case OWLSemanticsEnums.OWLReasonerStandardRules.SubPropertyTransitivity:
                                inferenceRegistry[OWLSemanticsEnums.OWLReasonerStandardRules.SubPropertyTransitivity.ToString()] = OWLSubPropertyTransitivityRule.ExecuteRule(ontology);
                                break;
                            case OWLSemanticsEnums.OWLReasonerStandardRules.EquivalentClassTransitivity:
                                inferenceRegistry[OWLSemanticsEnums.OWLReasonerStandardRules.EquivalentClassTransitivity.ToString()] = OWLEquivalentClassTransitivityRule.ExecuteRule(ontology);
                                break;
                            case OWLSemanticsEnums.OWLReasonerStandardRules.EquivalentPropertyTransitivity:
                                inferenceRegistry[OWLSemanticsEnums.OWLReasonerStandardRules.EquivalentPropertyTransitivity.ToString()] = OWLEquivalentPropertyTransitivityRule.ExecuteRule(ontology);
                                break;
                            case OWLSemanticsEnums.OWLReasonerStandardRules.DisjointClassEntailment:
                                inferenceRegistry[OWLSemanticsEnums.OWLReasonerStandardRules.DisjointClassEntailment.ToString()] = OWLDisjointClassEntailmentRule.ExecuteRule(ontology);
                                break;
                            case OWLSemanticsEnums.OWLReasonerStandardRules.DisjointPropertyEntailment:
                                inferenceRegistry[OWLSemanticsEnums.OWLReasonerStandardRules.DisjointPropertyEntailment.ToString()] = OWLDisjointPropertyEntailmentRule.ExecuteRule(ontology);
                                break;
                            case OWLSemanticsEnums.OWLReasonerStandardRules.DomainEntailment:
                                inferenceRegistry[OWLSemanticsEnums.OWLReasonerStandardRules.DomainEntailment.ToString()] = OWLDomainEntailmentRule.ExecuteRule(ontology);
                                break;
                            case OWLSemanticsEnums.OWLReasonerStandardRules.RangeEntailment:
                                inferenceRegistry[OWLSemanticsEnums.OWLReasonerStandardRules.RangeEntailment.ToString()] = OWLRangeEntailmentRule.ExecuteRule(ontology);
                                break;
                            case OWLSemanticsEnums.OWLReasonerStandardRules.SameAsTransitivity:
                                inferenceRegistry[OWLSemanticsEnums.OWLReasonerStandardRules.SameAsTransitivity.ToString()] = OWLSameAsTransitivityRule.ExecuteRule(ontology);
                                break;
                            case OWLSemanticsEnums.OWLReasonerStandardRules.DifferentFromEntailment:
                                inferenceRegistry[OWLSemanticsEnums.OWLReasonerStandardRules.DifferentFromEntailment.ToString()] = OWLDifferentFromEntailmentRule.ExecuteRule(ontology);
                                break;
                            case OWLSemanticsEnums.OWLReasonerStandardRules.IndividualTypeEntailment:
                                inferenceRegistry[OWLSemanticsEnums.OWLReasonerStandardRules.IndividualTypeEntailment.ToString()] = OWLIndividualTypeEntailmentRule.ExecuteRule(ontology);
                                break;
                            case OWLSemanticsEnums.OWLReasonerStandardRules.SymmetricPropertyEntailment:
                                inferenceRegistry[OWLSemanticsEnums.OWLReasonerStandardRules.SymmetricPropertyEntailment.ToString()] = OWLSymmetricPropertyEntailmentRule.ExecuteRule(ontology);
                                break;
                            case OWLSemanticsEnums.OWLReasonerStandardRules.TransitivePropertyEntailment:
                                inferenceRegistry[OWLSemanticsEnums.OWLReasonerStandardRules.TransitivePropertyEntailment.ToString()] = OWLTransitivePropertyEntailmentRule.ExecuteRule(ontology);
                                break;
                            case OWLSemanticsEnums.OWLReasonerStandardRules.ReflexivePropertyEntailment:
                                inferenceRegistry[OWLSemanticsEnums.OWLReasonerStandardRules.ReflexivePropertyEntailment.ToString()] = OWLReflexivePropertyEntailmentRule.ExecuteRule(ontology);
                                break;
                            case OWLSemanticsEnums.OWLReasonerStandardRules.InverseOfEntailment:
                                inferenceRegistry[OWLSemanticsEnums.OWLReasonerStandardRules.InverseOfEntailment.ToString()] = OWLInverseOfEntailmentRule.ExecuteRule(ontology);
                                break;
                            case OWLSemanticsEnums.OWLReasonerStandardRules.PropertyEntailment:
                                inferenceRegistry[OWLSemanticsEnums.OWLReasonerStandardRules.PropertyEntailment.ToString()] = OWLPropertyEntailmentRule.ExecuteRule(ontology);
                                break;
                            case OWLSemanticsEnums.OWLReasonerStandardRules.SameAsEntailment:
                                inferenceRegistry[OWLSemanticsEnums.OWLReasonerStandardRules.SameAsEntailment.ToString()] = OWLSameAsEntailmentRule.ExecuteRule(ontology);
                                break;
                            case OWLSemanticsEnums.OWLReasonerStandardRules.HasValueEntailment:
                                inferenceRegistry[OWLSemanticsEnums.OWLReasonerStandardRules.HasValueEntailment.ToString()] = OWLHasValueEntailmentRule.ExecuteRule(ontology);
                                break;
                            case OWLSemanticsEnums.OWLReasonerStandardRules.HasSelfEntailment:
                                inferenceRegistry[OWLSemanticsEnums.OWLReasonerStandardRules.HasSelfEntailment.ToString()] = OWLHasSelfEntailmentRule.ExecuteRule(ontology);
                                break;
                            case OWLSemanticsEnums.OWLReasonerStandardRules.HasKeyEntailment:
                                inferenceRegistry[OWLSemanticsEnums.OWLReasonerStandardRules.HasKeyEntailment.ToString()] = OWLHasKeyEntailmentRule.ExecuteRule(ontology);
                                break;
                            case OWLSemanticsEnums.OWLReasonerStandardRules.PropertyChainEntailment:
                                inferenceRegistry[OWLSemanticsEnums.OWLReasonerStandardRules.PropertyChainEntailment.ToString()] = OWLPropertyChainEntailmentRule.ExecuteRule(ontology);
                                break;
                        }

                        OWLSemanticsEvents.RaiseSemanticsInfo($"Completed standard reasoner rule '{standardRule}': found {inferenceRegistry[standardRule.ToString()].EvidencesCount} evidences");
                    });

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

                //Execute custom rules
                Parallel.ForEach(CustomRules, 
                    customRule =>
                    {
                        OWLSemanticsEvents.RaiseSemanticsInfo($"Launching custom (SWRL) reasoner rule '{customRule.RuleName}'");

                        inferenceRegistry[customRule.RuleName] = customRule.ApplyToOntology(ontology);

                        OWLSemanticsEvents.RaiseSemanticsInfo($"Completed custom (SWRL) reasoner rule '{customRule.RuleName}': found {inferenceRegistry[customRule.RuleName].EvidencesCount} evidences");
                    });

                //Process inference registry
                foreach (OWLReasonerReport inferenceRegistryReport in inferenceRegistry.Values)
                    reasonerReport.MergeEvidences(inferenceRegistryReport);

                OWLSemanticsEvents.RaiseSemanticsInfo($"Reasoner has been applied on Ontology '{ontology.URI}': found {reasonerReport.EvidencesCount} evidences");
            }

            return reasonerReport;
        }

        /// <summary>
        /// Asynchronously applies the reasoner on the given ontology
        /// </summary>
        public Task<OWLReasonerReport> ApplyToOntologyAsync(OWLOntology ontology, OWLOntologyLoaderOptions loaderOptions=null)
            => Task.Run(() => ApplyToOntology(ontology, loaderOptions));
        #endregion
    }
}