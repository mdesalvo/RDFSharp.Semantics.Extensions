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

namespace RDFSharp.Semantics.Extensions.TIME
{
    /// <summary>
    /// TIMEOntologyHelper contains methods for declaring and analyzing relations describing W3C TIME entities
    /// </summary>
    public static class TIMEOntologyHelper
    {
        /// <summary>
        /// Represents the Gregorian Time Reference System
        /// </summary>
        public static RDFResource GregorianTRS = new RDFResource("http://www.opengis.net/def/uom/ISO-8601/0/Gregorian");

        #region Declarer
        /// <summary>
        /// Declares the given instant feature to the temporal ontology (value of the instant is expressed as xsd:dateTimeStamp).
        /// </summary>
        public static TIMEOntology DeclareInstantFeature(this TIMEOntology timeOntology, RDFResource featureUri,
            RDFResource instantUri, RDFTypedLiteral instantValue)
        {
            if (featureUri == null)
                throw new OWLSemanticsException("Cannot declare instant to the temporal ontology because given \"featureUri\" parameter is null");
            if (instantUri == null)
                throw new OWLSemanticsException("Cannot declare instant to the temporal ontology because given \"instantUri\" parameter is null");
            if (instantValue == null)
                throw new OWLSemanticsException("Cannot declare instant to the temporal ontology because given \"instantValue\" parameter is null");
            if (!instantValue.Datatype.Equals(RDFModelEnums.RDFDatatypes.XSD_DATETIMESTAMP))
                throw new OWLSemanticsException("Cannot declare instant to the temporal ontology because given \"instantValue\" parameter must have \"xsd:dateTimeStamp\" datatype");

            //Add knowledge to the A-BOX
            timeOntology.Data.DeclareIndividual(featureUri);
            timeOntology.Data.DeclareObjectAssertion(featureUri, RDFVocabulary.TIME.HAS_TIME, instantUri);
            timeOntology.Data.DeclareIndividual(instantUri);
            timeOntology.Data.DeclareIndividualType(instantUri, RDFVocabulary.TIME.INSTANT);
            timeOntology.Data.DeclareDatatypeAssertion(instantUri, RDFVocabulary.TIME.IN_XSD_DATETIMESTAMP, instantValue);

            return timeOntology;
        }

        /// <summary>
        /// Declares the given instant feature to the temporal ontology (value of the instant is expressed as numeric coordinate in the given TRS).<br/>
        /// If temporal reference system is not provided, Gregorian (http://www.opengis.net/def/uom/ISO-8601/0/Gregorian) will be used.
        /// </summary>
        public static TIMEOntology DeclareInstantFeature(this TIMEOntology timeOntology, RDFResource featureUri,
            RDFResource instantUri, RDFResource timePositionUri, RDFTypedLiteral numericPositionValue, RDFResource trsUri=null)
        {
            if (featureUri == null)
                throw new OWLSemanticsException("Cannot declare instant to the temporal ontology because given \"featureUri\" parameter is null");
            if (instantUri == null)
                throw new OWLSemanticsException("Cannot declare instant to the temporal ontology because given \"instantUri\" parameter is null");
            if (timePositionUri == null)
                throw new OWLSemanticsException("Cannot declare instant to the temporal ontology because given \"timePositionUri\" parameter is null");
            if (!numericPositionValue.HasDecimalDatatype())
                throw new OWLSemanticsException("Cannot declare instant to the temporal ontology because given \"numericPositionValue\" parameter must have a numeric datatype");

            //Add knowledge to the A-BOX
            timeOntology.Data.DeclareIndividual(featureUri);
            timeOntology.Data.DeclareObjectAssertion(featureUri, RDFVocabulary.TIME.HAS_TIME, instantUri);
            timeOntology.Data.DeclareIndividual(instantUri);
            timeOntology.Data.DeclareIndividualType(instantUri, RDFVocabulary.TIME.INSTANT);
            timeOntology.Data.DeclareObjectAssertion(instantUri, RDFVocabulary.TIME.IN_TIME_POSITION, timePositionUri);
            timeOntology.Data.DeclareIndividual(timePositionUri);
            timeOntology.Data.DeclareIndividualType(timePositionUri, RDFVocabulary.TIME.TIME_POSITION);
            timeOntology.Data.DeclareObjectAssertion(timePositionUri, RDFVocabulary.TIME.HAS_TRS, trsUri ?? GregorianTRS);
            timeOntology.Data.DeclareDatatypeAssertion(timePositionUri, RDFVocabulary.TIME.NUMERIC_POSITION, numericPositionValue);

            return timeOntology;
        }
        #endregion
    }
}