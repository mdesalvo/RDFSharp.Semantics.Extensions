﻿/*
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
using NetTopologySuite.IO;
using NetTopologySuite.IO.GML2;
using RDFSharp.Model;
using RDFSharp.Query;
using System.Linq;

namespace RDFSharp.Semantics.Extensions.GEO
{
    /// <summary>
    /// GEOOntologyLoader is responsible for loading spatial ontologies from remote sources or alternative representations
    /// </summary>
    internal static class GEOOntologyLoader
    {
        #region Methods
        /// <summary>
        /// Gets a spatial ontology representation of the given graph
        /// </summary>
        internal static GEOOntology FromRDFGraph(RDFGraph graph, OWLOntologyLoaderOptions loaderOptions)
        {
            if (graph == null)
                throw new OWLSemanticsException("Cannot get GEO ontology from RDFGraph because given \"graph\" parameter is null");

            //Get OWL ontology with GEO extension points
            OWLOntology ontology = OWLOntologyLoader.FromRDFGraph(graph, loaderOptions,
               classModelExtensionPoint: GEOClassModelExtensionPoint,
               propertyModelExtensionPoint: GEOPropertyModelExtensionPoint);

            //Build GEO ontology from OWL ontology
            GEOOntology geoOntology = new GEOOntology(ontology.ToString()) { Model = ontology.Model, Data = ontology.Data, OBoxGraph = ontology.OBoxGraph };

            //Detect spatial entities
            WKTReader wktReader = new WKTReader();
            GMLReader gmlReader = new GMLReader();
            foreach (RDFResource spatialObject in geoOntology.Data.GetIndividualsOf(geoOntology.Model, RDFVocabulary.GEOSPARQL.SPATIAL_OBJECT))
            {
                Geometry spatialObjectGeometry = null;

                //Try detect by WKT representation
                RDFPatternMember asWKTObject = geoOntology.Data.ABoxGraph[spatialObject, RDFVocabulary.GEOSPARQL.AS_WKT, null, null].FirstOrDefault()?.Object;
                if (asWKTObject is RDFTypedLiteral asWKTTypedLiteral && asWKTTypedLiteral.Datatype.Equals(RDFModelEnums.RDFDatatypes.GEOSPARQL_WKT))
                    spatialObjectGeometry = wktReader.Read(asWKTTypedLiteral.Value);
                if (spatialObject == null)
                {
                    //Try detect by GML representation
                    RDFPatternMember asGMLObject = geoOntology.Data.ABoxGraph[spatialObject, RDFVocabulary.GEOSPARQL.AS_GML, null, null].FirstOrDefault()?.Object;
                    if (asGMLObject is RDFTypedLiteral asGMLTypedLiteral && asGMLTypedLiteral.Datatype.Equals(RDFModelEnums.RDFDatatypes.GEOSPARQL_GML))
                        spatialObjectGeometry = gmlReader.Read(asGMLTypedLiteral.Value);
                }

                //Declare spatial entity
                if (spatialObjectGeometry != null)
                {
                    //sf:Point
                    if (spatialObjectGeometry is Point spatialObjectGeometryPoint)
                        geoOntology.DeclarePoint(spatialObject, spatialObjectGeometryPoint.Coordinate.X, spatialObjectGeometryPoint.Coordinate.Y);
                    //sf:LineString
                    else if (spatialObjectGeometry is LineString spatialObjectGeometryLineString)
                        geoOntology.DeclareLineString(spatialObject, spatialObjectGeometryLineString.Coordinates.Select(c => (c.X, c.Y)).ToList());
                    //sf:Polygon
                    else if (spatialObjectGeometry is Polygon spatialObjectGeometryPolygon)
                        geoOntology.DeclarePolygon(spatialObject, spatialObjectGeometryPolygon.Coordinates.Select(c => (c.X, c.Y)).ToList());
                }
            }

            return geoOntology;
        }
        #endregion

        #region Utilities
        /// <summary>
        /// Extends OWL class model loading with support for spatial entities
        /// </summary>
        internal static void GEOClassModelExtensionPoint(OWLOntology ontology, RDFGraph graph)
            => BuildGEOClassModel(ontology.Model.ClassModel);

        /// <summary>
        /// Extends OWL property model loading with support for spatial entities
        /// </summary>
        internal static void GEOPropertyModelExtensionPoint(OWLOntology ontology, RDFGraph graph)
            => BuildGEOPropertyModel(ontology.Model.PropertyModel);

        /// <summary>
        /// Builds a reference spatial model
        /// </summary>
        internal static OWLOntologyModel BuildGEOModel()
            => new OWLOntologyModel() { ClassModel = BuildGEOClassModel(), PropertyModel = BuildGEOPropertyModel() };

        /// <summary>
        /// Builds a reference spatial class model
        /// </summary>
        internal static OWLOntologyClassModel BuildGEOClassModel(OWLOntologyClassModel existingClassModel = null)
        {
            OWLOntologyClassModel classModel = existingClassModel ?? new OWLOntologyClassModel();

            //GeoSPARQL
            classModel.DeclareClass(RDFVocabulary.GEOSPARQL.SPATIAL_OBJECT);
            classModel.DeclareClass(RDFVocabulary.GEOSPARQL.GEOMETRY);
            classModel.DeclareClass(RDFVocabulary.GEOSPARQL.FEATURE);
            classModel.DeclareSubClasses(RDFVocabulary.GEOSPARQL.GEOMETRY, RDFVocabulary.GEOSPARQL.SPATIAL_OBJECT);
            classModel.DeclareSubClasses(RDFVocabulary.GEOSPARQL.FEATURE, RDFVocabulary.GEOSPARQL.SPATIAL_OBJECT);
            classModel.DeclareDisjointClasses(RDFVocabulary.GEOSPARQL.GEOMETRY, RDFVocabulary.GEOSPARQL.FEATURE);

            //Simple Features (Geometry)
            classModel.DeclareClass(RDFVocabulary.GEOSPARQL.SF.POINT);
            classModel.DeclareClass(RDFVocabulary.GEOSPARQL.SF.CURVE);
            classModel.DeclareClass(RDFVocabulary.GEOSPARQL.SF.SURFACE);
            classModel.DeclareClass(RDFVocabulary.GEOSPARQL.SF.POLYGON);
            classModel.DeclareClass(RDFVocabulary.GEOSPARQL.SF.TRIANGLE);
            classModel.DeclareClass(RDFVocabulary.GEOSPARQL.SF.LINESTRING);
            classModel.DeclareClass(RDFVocabulary.GEOSPARQL.SF.LINEAR_RING);
            classModel.DeclareClass(RDFVocabulary.GEOSPARQL.SF.LINE);
            classModel.DeclareClass(RDFVocabulary.GEOSPARQL.SF.GEOMETRY_COLLECTION);
            classModel.DeclareClass(RDFVocabulary.GEOSPARQL.SF.MULTI_POINT);
            classModel.DeclareClass(RDFVocabulary.GEOSPARQL.SF.MULTI_CURVE);
            classModel.DeclareClass(RDFVocabulary.GEOSPARQL.SF.MULTI_SURFACE);
            classModel.DeclareClass(RDFVocabulary.GEOSPARQL.SF.MULTI_POLYGON);
            classModel.DeclareClass(RDFVocabulary.GEOSPARQL.SF.MULTI_LINESTRING);
            classModel.DeclareClass(RDFVocabulary.GEOSPARQL.SF.POLYHEDRAL_SURFACE);
            classModel.DeclareClass(RDFVocabulary.GEOSPARQL.SF.TIN);
            classModel.DeclareSubClasses(RDFVocabulary.GEOSPARQL.SF.POINT, RDFVocabulary.GEOSPARQL.GEOMETRY);
            classModel.DeclareSubClasses(RDFVocabulary.GEOSPARQL.SF.CURVE, RDFVocabulary.GEOSPARQL.GEOMETRY);
            classModel.DeclareSubClasses(RDFVocabulary.GEOSPARQL.SF.SURFACE, RDFVocabulary.GEOSPARQL.GEOMETRY);
            classModel.DeclareSubClasses(RDFVocabulary.GEOSPARQL.SF.POLYGON, RDFVocabulary.GEOSPARQL.GEOMETRY);
            classModel.DeclareSubClasses(RDFVocabulary.GEOSPARQL.SF.POLYGON, RDFVocabulary.GEOSPARQL.SF.SURFACE);
            classModel.DeclareSubClasses(RDFVocabulary.GEOSPARQL.SF.TRIANGLE, RDFVocabulary.GEOSPARQL.GEOMETRY);
            classModel.DeclareSubClasses(RDFVocabulary.GEOSPARQL.SF.TRIANGLE, RDFVocabulary.GEOSPARQL.SF.POLYGON);
            classModel.DeclareSubClasses(RDFVocabulary.GEOSPARQL.SF.LINESTRING, RDFVocabulary.GEOSPARQL.GEOMETRY);
            classModel.DeclareSubClasses(RDFVocabulary.GEOSPARQL.SF.LINESTRING, RDFVocabulary.GEOSPARQL.SF.CURVE);
            classModel.DeclareSubClasses(RDFVocabulary.GEOSPARQL.SF.LINEAR_RING, RDFVocabulary.GEOSPARQL.GEOMETRY);
            classModel.DeclareSubClasses(RDFVocabulary.GEOSPARQL.SF.LINEAR_RING, RDFVocabulary.GEOSPARQL.SF.LINESTRING);
            classModel.DeclareSubClasses(RDFVocabulary.GEOSPARQL.SF.LINE, RDFVocabulary.GEOSPARQL.GEOMETRY);
            classModel.DeclareSubClasses(RDFVocabulary.GEOSPARQL.SF.LINE, RDFVocabulary.GEOSPARQL.SF.LINESTRING);
            classModel.DeclareSubClasses(RDFVocabulary.GEOSPARQL.SF.GEOMETRY_COLLECTION, RDFVocabulary.GEOSPARQL.GEOMETRY);
            classModel.DeclareSubClasses(RDFVocabulary.GEOSPARQL.SF.MULTI_POINT, RDFVocabulary.GEOSPARQL.GEOMETRY);
            classModel.DeclareSubClasses(RDFVocabulary.GEOSPARQL.SF.MULTI_POINT, RDFVocabulary.GEOSPARQL.SF.GEOMETRY_COLLECTION);
            classModel.DeclareSubClasses(RDFVocabulary.GEOSPARQL.SF.MULTI_CURVE, RDFVocabulary.GEOSPARQL.GEOMETRY);
            classModel.DeclareSubClasses(RDFVocabulary.GEOSPARQL.SF.MULTI_CURVE, RDFVocabulary.GEOSPARQL.SF.GEOMETRY_COLLECTION);
            classModel.DeclareSubClasses(RDFVocabulary.GEOSPARQL.SF.MULTI_SURFACE, RDFVocabulary.GEOSPARQL.GEOMETRY);
            classModel.DeclareSubClasses(RDFVocabulary.GEOSPARQL.SF.MULTI_SURFACE, RDFVocabulary.GEOSPARQL.SF.GEOMETRY_COLLECTION);
            classModel.DeclareSubClasses(RDFVocabulary.GEOSPARQL.SF.MULTI_POLYGON, RDFVocabulary.GEOSPARQL.GEOMETRY);
            classModel.DeclareSubClasses(RDFVocabulary.GEOSPARQL.SF.MULTI_POLYGON, RDFVocabulary.GEOSPARQL.SF.MULTI_SURFACE);
            classModel.DeclareSubClasses(RDFVocabulary.GEOSPARQL.SF.MULTI_LINESTRING, RDFVocabulary.GEOSPARQL.GEOMETRY);
            classModel.DeclareSubClasses(RDFVocabulary.GEOSPARQL.SF.MULTI_LINESTRING, RDFVocabulary.GEOSPARQL.SF.MULTI_CURVE);
            classModel.DeclareSubClasses(RDFVocabulary.GEOSPARQL.SF.POLYHEDRAL_SURFACE, RDFVocabulary.GEOSPARQL.GEOMETRY);
            classModel.DeclareSubClasses(RDFVocabulary.GEOSPARQL.SF.POLYHEDRAL_SURFACE, RDFVocabulary.GEOSPARQL.SF.SURFACE);
            classModel.DeclareSubClasses(RDFVocabulary.GEOSPARQL.SF.TIN, RDFVocabulary.GEOSPARQL.GEOMETRY);
            classModel.DeclareSubClasses(RDFVocabulary.GEOSPARQL.SF.TIN, RDFVocabulary.GEOSPARQL.SF.SURFACE);

            return classModel;
        }

        /// <summary>
        /// Builds a reference spatial property model
        /// </summary>
        internal static OWLOntologyPropertyModel BuildGEOPropertyModel(OWLOntologyPropertyModel existingPropertyModel = null)
        {
            OWLOntologyPropertyModel propertyModel = existingPropertyModel ?? new OWLOntologyPropertyModel();

            //GeoSPARQL
            propertyModel.DeclareObjectProperty(RDFVocabulary.GEOSPARQL.HAS_GEOMETRY, new OWLOntologyObjectPropertyBehavior() { Domain = RDFVocabulary.GEOSPARQL.FEATURE, Range = RDFVocabulary.GEOSPARQL.GEOMETRY });
            propertyModel.DeclareObjectProperty(RDFVocabulary.GEOSPARQL.DEFAULT_GEOMETRY, new OWLOntologyObjectPropertyBehavior() { Domain = RDFVocabulary.GEOSPARQL.FEATURE, Range = RDFVocabulary.GEOSPARQL.GEOMETRY });
            propertyModel.DeclareObjectProperty(RDFVocabulary.GEOSPARQL.SF_CONTAINS, new OWLOntologyObjectPropertyBehavior() { Domain = RDFVocabulary.GEOSPARQL.SPATIAL_OBJECT, Range = RDFVocabulary.GEOSPARQL.SPATIAL_OBJECT });
            propertyModel.DeclareObjectProperty(RDFVocabulary.GEOSPARQL.SF_CROSSES, new OWLOntologyObjectPropertyBehavior() { Domain = RDFVocabulary.GEOSPARQL.SPATIAL_OBJECT, Range = RDFVocabulary.GEOSPARQL.SPATIAL_OBJECT });
            propertyModel.DeclareObjectProperty(RDFVocabulary.GEOSPARQL.SF_DISJOINT, new OWLOntologyObjectPropertyBehavior() { Domain = RDFVocabulary.GEOSPARQL.SPATIAL_OBJECT, Range = RDFVocabulary.GEOSPARQL.SPATIAL_OBJECT });
            propertyModel.DeclareObjectProperty(RDFVocabulary.GEOSPARQL.SF_EQUALS, new OWLOntologyObjectPropertyBehavior() { Domain = RDFVocabulary.GEOSPARQL.SPATIAL_OBJECT, Range = RDFVocabulary.GEOSPARQL.SPATIAL_OBJECT });
            propertyModel.DeclareObjectProperty(RDFVocabulary.GEOSPARQL.SF_INTERSECTS, new OWLOntologyObjectPropertyBehavior() { Domain = RDFVocabulary.GEOSPARQL.SPATIAL_OBJECT, Range = RDFVocabulary.GEOSPARQL.SPATIAL_OBJECT });
            propertyModel.DeclareObjectProperty(RDFVocabulary.GEOSPARQL.SF_OVERLAPS, new OWLOntologyObjectPropertyBehavior() { Domain = RDFVocabulary.GEOSPARQL.SPATIAL_OBJECT, Range = RDFVocabulary.GEOSPARQL.SPATIAL_OBJECT });
            propertyModel.DeclareObjectProperty(RDFVocabulary.GEOSPARQL.SF_TOUCHES, new OWLOntologyObjectPropertyBehavior() { Domain = RDFVocabulary.GEOSPARQL.SPATIAL_OBJECT, Range = RDFVocabulary.GEOSPARQL.SPATIAL_OBJECT });
            propertyModel.DeclareObjectProperty(RDFVocabulary.GEOSPARQL.SF_WITHIN, new OWLOntologyObjectPropertyBehavior() { Domain = RDFVocabulary.GEOSPARQL.SPATIAL_OBJECT, Range = RDFVocabulary.GEOSPARQL.SPATIAL_OBJECT });
            propertyModel.DeclareObjectProperty(RDFVocabulary.GEOSPARQL.EH_CONTAINS, new OWLOntologyObjectPropertyBehavior() { Domain = RDFVocabulary.GEOSPARQL.SPATIAL_OBJECT, Range = RDFVocabulary.GEOSPARQL.SPATIAL_OBJECT });
            propertyModel.DeclareObjectProperty(RDFVocabulary.GEOSPARQL.EH_COVERS, new OWLOntologyObjectPropertyBehavior() { Domain = RDFVocabulary.GEOSPARQL.SPATIAL_OBJECT, Range = RDFVocabulary.GEOSPARQL.SPATIAL_OBJECT });
            propertyModel.DeclareObjectProperty(RDFVocabulary.GEOSPARQL.EH_COVERED_BY, new OWLOntologyObjectPropertyBehavior() { Domain = RDFVocabulary.GEOSPARQL.SPATIAL_OBJECT, Range = RDFVocabulary.GEOSPARQL.SPATIAL_OBJECT });
            propertyModel.DeclareObjectProperty(RDFVocabulary.GEOSPARQL.EH_DISJOINT, new OWLOntologyObjectPropertyBehavior() { Domain = RDFVocabulary.GEOSPARQL.SPATIAL_OBJECT, Range = RDFVocabulary.GEOSPARQL.SPATIAL_OBJECT });
            propertyModel.DeclareObjectProperty(RDFVocabulary.GEOSPARQL.EH_EQUALS, new OWLOntologyObjectPropertyBehavior() { Domain = RDFVocabulary.GEOSPARQL.SPATIAL_OBJECT, Range = RDFVocabulary.GEOSPARQL.SPATIAL_OBJECT });
            propertyModel.DeclareObjectProperty(RDFVocabulary.GEOSPARQL.EH_INSIDE, new OWLOntologyObjectPropertyBehavior() { Domain = RDFVocabulary.GEOSPARQL.SPATIAL_OBJECT, Range = RDFVocabulary.GEOSPARQL.SPATIAL_OBJECT });
            propertyModel.DeclareObjectProperty(RDFVocabulary.GEOSPARQL.EH_MEET, new OWLOntologyObjectPropertyBehavior() { Domain = RDFVocabulary.GEOSPARQL.SPATIAL_OBJECT, Range = RDFVocabulary.GEOSPARQL.SPATIAL_OBJECT });
            propertyModel.DeclareObjectProperty(RDFVocabulary.GEOSPARQL.EH_OVERLAP, new OWLOntologyObjectPropertyBehavior() { Domain = RDFVocabulary.GEOSPARQL.SPATIAL_OBJECT, Range = RDFVocabulary.GEOSPARQL.SPATIAL_OBJECT });
            propertyModel.DeclareObjectProperty(RDFVocabulary.GEOSPARQL.RCC8DC, new OWLOntologyObjectPropertyBehavior() { Domain = RDFVocabulary.GEOSPARQL.SPATIAL_OBJECT, Range = RDFVocabulary.GEOSPARQL.SPATIAL_OBJECT });
            propertyModel.DeclareObjectProperty(RDFVocabulary.GEOSPARQL.RCC8EC, new OWLOntologyObjectPropertyBehavior() { Domain = RDFVocabulary.GEOSPARQL.SPATIAL_OBJECT, Range = RDFVocabulary.GEOSPARQL.SPATIAL_OBJECT });
            propertyModel.DeclareObjectProperty(RDFVocabulary.GEOSPARQL.RCC8EQ, new OWLOntologyObjectPropertyBehavior() { Domain = RDFVocabulary.GEOSPARQL.SPATIAL_OBJECT, Range = RDFVocabulary.GEOSPARQL.SPATIAL_OBJECT });
            propertyModel.DeclareObjectProperty(RDFVocabulary.GEOSPARQL.RCC8NTPP, new OWLOntologyObjectPropertyBehavior() { Domain = RDFVocabulary.GEOSPARQL.SPATIAL_OBJECT, Range = RDFVocabulary.GEOSPARQL.SPATIAL_OBJECT });
            propertyModel.DeclareObjectProperty(RDFVocabulary.GEOSPARQL.RCC8NTPPI, new OWLOntologyObjectPropertyBehavior() { Domain = RDFVocabulary.GEOSPARQL.SPATIAL_OBJECT, Range = RDFVocabulary.GEOSPARQL.SPATIAL_OBJECT });
            propertyModel.DeclareObjectProperty(RDFVocabulary.GEOSPARQL.RCC8PO, new OWLOntologyObjectPropertyBehavior() { Domain = RDFVocabulary.GEOSPARQL.SPATIAL_OBJECT, Range = RDFVocabulary.GEOSPARQL.SPATIAL_OBJECT });
            propertyModel.DeclareObjectProperty(RDFVocabulary.GEOSPARQL.RCC8TPP, new OWLOntologyObjectPropertyBehavior() { Domain = RDFVocabulary.GEOSPARQL.SPATIAL_OBJECT, Range = RDFVocabulary.GEOSPARQL.SPATIAL_OBJECT });
            propertyModel.DeclareObjectProperty(RDFVocabulary.GEOSPARQL.RCC8TPPI, new OWLOntologyObjectPropertyBehavior() { Domain = RDFVocabulary.GEOSPARQL.SPATIAL_OBJECT, Range = RDFVocabulary.GEOSPARQL.SPATIAL_OBJECT });
            propertyModel.DeclareDatatypeProperty(RDFVocabulary.GEOSPARQL.DIMENSION, new OWLOntologyDatatypePropertyBehavior() { Domain = RDFVocabulary.GEOSPARQL.GEOMETRY, Range = RDFVocabulary.XSD.INTEGER });
            propertyModel.DeclareDatatypeProperty(RDFVocabulary.GEOSPARQL.COORDINATE_DIMENSION, new OWLOntologyDatatypePropertyBehavior() { Domain = RDFVocabulary.GEOSPARQL.GEOMETRY, Range = RDFVocabulary.XSD.INTEGER });
            propertyModel.DeclareDatatypeProperty(RDFVocabulary.GEOSPARQL.SPATIAL_DIMENSION, new OWLOntologyDatatypePropertyBehavior() { Domain = RDFVocabulary.GEOSPARQL.GEOMETRY, Range = RDFVocabulary.XSD.INTEGER });
            propertyModel.DeclareDatatypeProperty(RDFVocabulary.GEOSPARQL.HAS_SERIALIZATION, new OWLOntologyObjectPropertyBehavior() { Domain = RDFVocabulary.GEOSPARQL.GEOMETRY, Range = RDFVocabulary.RDFS.LITERAL });
            propertyModel.DeclareDatatypeProperty(RDFVocabulary.GEOSPARQL.AS_WKT, new OWLOntologyObjectPropertyBehavior() { Domain = RDFVocabulary.GEOSPARQL.GEOMETRY, Range = RDFVocabulary.GEOSPARQL.WKT_LITERAL });
            propertyModel.DeclareDatatypeProperty(RDFVocabulary.GEOSPARQL.AS_GML, new OWLOntologyObjectPropertyBehavior() { Domain = RDFVocabulary.GEOSPARQL.GEOMETRY, Range = RDFVocabulary.GEOSPARQL.GML_LITERAL });
            propertyModel.DeclareDatatypeProperty(RDFVocabulary.GEOSPARQL.IS_EMPTY, new OWLOntologyObjectPropertyBehavior() { Domain = RDFVocabulary.GEOSPARQL.GEOMETRY, Range = RDFVocabulary.XSD.BOOLEAN });
            propertyModel.DeclareDatatypeProperty(RDFVocabulary.GEOSPARQL.IS_SIMPLE, new OWLOntologyObjectPropertyBehavior() { Domain = RDFVocabulary.GEOSPARQL.GEOMETRY, Range = RDFVocabulary.XSD.BOOLEAN });
            propertyModel.DeclareSubProperties(RDFVocabulary.GEOSPARQL.DEFAULT_GEOMETRY, RDFVocabulary.GEOSPARQL.HAS_GEOMETRY);
            propertyModel.DeclareSubProperties(RDFVocabulary.GEOSPARQL.AS_WKT, RDFVocabulary.GEOSPARQL.HAS_SERIALIZATION);
            propertyModel.DeclareSubProperties(RDFVocabulary.GEOSPARQL.AS_GML, RDFVocabulary.GEOSPARQL.HAS_SERIALIZATION);
            propertyModel.DeclareInverseProperties(RDFVocabulary.GEOSPARQL.EH_COVERS, RDFVocabulary.GEOSPARQL.EH_COVERED_BY);

            return propertyModel;
        }
        #endregion
    }
}