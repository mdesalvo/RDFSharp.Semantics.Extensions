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
using System.Threading.Tasks;

namespace RDFSharp.Semantics.Extensions.TIME
{
    /// <summary>
    /// TIMEOntology represents an OWL ontology specialized in describing relations between temporal entities.<br/>
    /// It implements "W3C Time Ontology" classes, properties and individuals.
    /// </summary>
    public class TIMEOntology : OWLOntology
    {
        #region Ctors
        /// <summary>
        /// Builds a temporal ontology with the given URI (internal T-BOX/A-BOX is initialized with W3C Time ontology)
        /// </summary>
        public TIMEOntology(string timeOntologyURI) : base(timeOntologyURI)
        {
            Model = TIMEOntologyLoader.BuildTIMEModel();
            Data = TIMEOntologyLoader.BuildTIMEData();
        }
        #endregion

        #region Methods
        /// <summary>
        /// Gets a temporal ontology representation from the given graph
        /// </summary>
        public static new TIMEOntology FromRDFGraph(RDFGraph graph)
            => FromRDFGraph(graph, OWLOntologyLoaderOptions.DefaultOptions);

        /// <summary>
        /// Gets a temporal ontology representation from the given graph (applying the given loader options)
        /// </summary>
        public static new TIMEOntology FromRDFGraph(RDFGraph graph, OWLOntologyLoaderOptions loaderOptions)
            => TIMEOntologyLoader.FromRDFGraph(graph, loaderOptions);

        /// <summary>
        /// Asynchronously gets a temporal ontology representation from the given graph
        /// </summary>
        public static new Task<TIMEOntology> FromRDFGraphAsync(RDFGraph graph)
            => Task.Run(() => FromRDFGraph(graph));

        /// <summary>
        /// Asynchronously gets a temporal ontology representation from the given graph (applying the given loader options)
        /// </summary>
        public static new Task<TIMEOntology> FromRDFGraphAsync(RDFGraph graph, OWLOntologyLoaderOptions loaderOptions)
            => Task.Run(() => FromRDFGraph(graph, loaderOptions));
        #endregion
    }
}