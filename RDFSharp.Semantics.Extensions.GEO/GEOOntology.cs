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

using NetTopologySuite.Geometries;
using RDFSharp.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RDFSharp.Semantics.Extensions.GEO
{
    /// <summary>
    /// GEOOntology represents an OWL ontology specialized in describing relations between spatial entities
    /// </summary>
    public class GEOOntology : OWLOntology
    {
        #region Properties
        /// <summary>
        /// Dictionary of geometries formally serialized into the ontology data (WGS84,UTM)
        /// </summary>
        internal Dictionary<string, (Geometry, Geometry)> SerializedGeometries { get; set; } 
        #endregion

        #region Ctors
        /// <summary>
        /// Builds a spatial ontology with the given URI (internal T-BOX is initialized with GeoSPARQL ontology)
        /// </summary>
        public GEOOntology(string geoOntologyURI) : base(geoOntologyURI)
        {
            SerializedGeometries = new Dictionary<string, (Geometry, Geometry)>();
            Model = GEOOntologyLoader.BuildGEOModel();
        }
        #endregion

        #region Methods
        /// <summary>
        /// Gets a spatial ontology representation from the given graph
        /// </summary>
        public static new GEOOntology FromRDFGraph(RDFGraph graph)
            => FromRDFGraph(graph, OWLOntologyLoaderOptions.DefaultOptions);

        /// <summary>
        /// Gets a spatial ontology representation from the given graph (applying the given loader options)
        /// </summary>
        public static new GEOOntology FromRDFGraph(RDFGraph graph, OWLOntologyLoaderOptions loaderOptions)
            => GEOOntologyLoader.FromRDFGraph(graph, loaderOptions);

        /// <summary>
        /// Asynchronously gets a spatial ontology representation from the given graph
        /// </summary>
        public static new Task<GEOOntology> FromRDFGraphAsync(RDFGraph graph)
            => Task.Run(() => FromRDFGraph(graph));

        /// <summary>
        /// Asynchronously gets a spatial ontology representation from the given graph (applying the given loader options)
        /// </summary>
        public static new Task<GEOOntology> FromRDFGraphAsync(RDFGraph graph, OWLOntologyLoaderOptions loaderOptions)
            => Task.Run(() => FromRDFGraph(graph, loaderOptions));
        #endregion
    }
}