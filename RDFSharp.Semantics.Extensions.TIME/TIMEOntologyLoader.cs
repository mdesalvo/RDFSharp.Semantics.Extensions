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
            classModel.DeclareHasValueRestriction(new RDFResource("bnode:HasGregorianTRSValue"), 
                RDFVocabulary.TIME.HAS_TRS, new RDFResource("http://www.opengis.net/def/uom/ISO-8601/0/Gregorian"));
            classModel.DeclareCardinalityRestriction(new RDFResource("bnode:HasExactlyOneTRSValue"),
                RDFVocabulary.TIME.HAS_TRS, 1);
            classModel.DeclareAllValuesFromRestriction(new RDFResource("bnode:HasAllYearValuesFromGYear"),
                RDFVocabulary.TIME.YEAR, RDFVocabulary.XSD.G_YEAR);
            classModel.DeclareAllValuesFromRestriction(new RDFResource("bnode:HasAllMonthValuesFromGMonth"),
                RDFVocabulary.TIME.MONTH, RDFVocabulary.XSD.G_MONTH);
            classModel.DeclareAllValuesFromRestriction(new RDFResource("bnode:HasAllDayValuesFromGDay"),
                RDFVocabulary.TIME.DAY, RDFVocabulary.XSD.G_DAY);
            classModel.DeclareCardinalityRestriction(new RDFResource("bnode:HasExactlyOneNumericDurationValue"),
                RDFVocabulary.TIME.NUMERIC_DURATION, 1);
            classModel.DeclareCardinalityRestriction(new RDFResource("bnode:HasExactlyOneNumericPositionValue"),
                RDFVocabulary.TIME.NUMERIC_POSITION, 1);
            classModel.DeclareCardinalityRestriction(new RDFResource("bnode:HasExactlyOneNominalPositionValue"),
                RDFVocabulary.TIME.NOMINAL_POSITION, 1);
            classModel.DeclareCardinalityRestriction(new RDFResource("bnode:HasExactlyOneUnitTypeValue"),
                RDFVocabulary.TIME.UNIT_TYPE, 1);
            classModel.DeclareAllValuesFromRestriction(new RDFResource("bnode:HasAllYearsValuesFromDecimal"),
                RDFVocabulary.TIME.YEARS, RDFVocabulary.XSD.DECIMAL);
            classModel.DeclareAllValuesFromRestriction(new RDFResource("bnode:HasAllMonthsValuesFromDecimal"),
                RDFVocabulary.TIME.MONTHS, RDFVocabulary.XSD.DECIMAL);
            classModel.DeclareAllValuesFromRestriction(new RDFResource("bnode:HasAllWeeksValuesFromDecimal"),
                RDFVocabulary.TIME.WEEKS, RDFVocabulary.XSD.DECIMAL);
            classModel.DeclareAllValuesFromRestriction(new RDFResource("bnode:HasAllDaysValuesFromDecimal"),
                RDFVocabulary.TIME.DAYS, RDFVocabulary.XSD.DECIMAL);
            classModel.DeclareAllValuesFromRestriction(new RDFResource("bnode:HasAllHoursValuesFromDecimal"),
                RDFVocabulary.TIME.HOURS, RDFVocabulary.XSD.DECIMAL);
            classModel.DeclareAllValuesFromRestriction(new RDFResource("bnode:HasAllMinutesValuesFromDecimal"),
                RDFVocabulary.TIME.MINUTES, RDFVocabulary.XSD.DECIMAL);
            classModel.DeclareAllValuesFromRestriction(new RDFResource("bnode:HasAllSecondsValuesFromDecimal"),
                RDFVocabulary.TIME.SECONDS, RDFVocabulary.XSD.DECIMAL);
            classModel.DeclareMaxCardinalityRestriction(new RDFResource("bnode:HasMaximumOneTimeZoneValue"),
                RDFVocabulary.TIME.TIMEZONE, 1);
            classModel.DeclareMaxCardinalityRestriction(new RDFResource("bnode:HasMaximumOneYearValue"),
                RDFVocabulary.TIME.YEAR, 1);
            classModel.DeclareMaxCardinalityRestriction(new RDFResource("bnode:HasMaximumOneMonthValue"),
                RDFVocabulary.TIME.MONTH, 1);
            classModel.DeclareMaxCardinalityRestriction(new RDFResource("bnode:HasMaximumOneDayValue"),
                RDFVocabulary.TIME.DAY, 1);
            classModel.DeclareMaxCardinalityRestriction(new RDFResource("bnode:HasMaximumOneHourValue"),
                RDFVocabulary.TIME.HOUR, 1);
            classModel.DeclareMaxCardinalityRestriction(new RDFResource("bnode:HasMaximumOneMinuteValue"),
                RDFVocabulary.TIME.MINUTE, 1);
            classModel.DeclareMaxCardinalityRestriction(new RDFResource("bnode:HasMaximumOneSecondValue"),
                RDFVocabulary.TIME.SECOND, 1);
            classModel.DeclareMaxCardinalityRestriction(new RDFResource("bnode:HasMaximumOneWeekValue"),
                RDFVocabulary.TIME.WEEK, 1);
            classModel.DeclareMaxCardinalityRestriction(new RDFResource("bnode:HasMaximumOneDayOfYearValue"),
                RDFVocabulary.TIME.DAY_OF_YEAR, 1);
            classModel.DeclareMaxCardinalityRestriction(new RDFResource("bnode:HasMaximumOneDayOfWeekValue"),
                RDFVocabulary.TIME.DAY_OF_WEEK, 1);
            classModel.DeclareMaxCardinalityRestriction(new RDFResource("bnode:HasMaximumOneMonthOfYearValue"),
                RDFVocabulary.TIME.MONTH_OF_YEAR, 1);
            classModel.DeclareMaxCardinalityRestriction(new RDFResource("bnode:HasMaximumOneYearsValue"),
                RDFVocabulary.TIME.YEARS, 1);
            classModel.DeclareMaxCardinalityRestriction(new RDFResource("bnode:HasMaximumOneMonthsValue"),
                RDFVocabulary.TIME.MONTHS, 1);
            classModel.DeclareMaxCardinalityRestriction(new RDFResource("bnode:HasMaximumOneDaysValue"),
                RDFVocabulary.TIME.DAYS, 1);
            classModel.DeclareMaxCardinalityRestriction(new RDFResource("bnode:HasMaximumOneHoursValue"),
                RDFVocabulary.TIME.HOURS, 1);
            classModel.DeclareMaxCardinalityRestriction(new RDFResource("bnode:HasMaximumOneMinutesValue"),
                RDFVocabulary.TIME.MINUTES, 1);
            classModel.DeclareMaxCardinalityRestriction(new RDFResource("bnode:HasMaximumOneSecondsValue"),
                RDFVocabulary.TIME.SECONDS, 1);
            classModel.DeclareMaxCardinalityRestriction(new RDFResource("bnode:HasMaximumOneWeeksValue"),
                RDFVocabulary.TIME.WEEKS, 1);
            classModel.DeclareCardinalityRestriction(new RDFResource("bnode:HasNoYearValues"),
                RDFVocabulary.TIME.YEAR, 0);
            classModel.DeclareCardinalityRestriction(new RDFResource("bnode:HasNoMonthValues"),
                RDFVocabulary.TIME.MONTH, 0);
            classModel.DeclareCardinalityRestriction(new RDFResource("bnode:HasExactlyOneMonthValues"),
                RDFVocabulary.TIME.MONTH, 1);
            classModel.DeclareCardinalityRestriction(new RDFResource("bnode:HasNoWeekValues"),
                RDFVocabulary.TIME.WEEK, 0);
            classModel.DeclareCardinalityRestriction(new RDFResource("bnode:HasNoDayValues"),
                RDFVocabulary.TIME.DAY, 0);
            classModel.DeclareCardinalityRestriction(new RDFResource("bnode:HasNoHourValues"),
                RDFVocabulary.TIME.HOUR, 0);
            classModel.DeclareCardinalityRestriction(new RDFResource("bnode:HasNoMinuteValues"),
                RDFVocabulary.TIME.MINUTE, 0);
            classModel.DeclareCardinalityRestriction(new RDFResource("bnode:HasNoSecondValues"),
                RDFVocabulary.TIME.SECOND, 0);
            classModel.DeclareHasValueRestriction(new RDFResource("bnode:HasUnitTypeMonthValue"),
                RDFVocabulary.TIME.UNIT_TYPE, RDFVocabulary.TIME.UNIT_MONTH);
            classModel.DeclareUnionClass(RDFVocabulary.TIME.TEMPORAL_ENTITY, new List<RDFResource>() {
                RDFVocabulary.TIME.INSTANT, RDFVocabulary.TIME.INTERVAL });
            classModel.DeclareUnionClass(new RDFResource("bnode:HasTRSDomain"), new List<RDFResource>() {
                RDFVocabulary.TIME.TEMPORAL_POSITION, RDFVocabulary.TIME.GENERAL_DURATION_DESCRIPTION });
            classModel.DeclareUnionClass(new RDFResource("bnode:HasExactlyOneNumericPositionValueOrNominalPositionValue"), new List<RDFResource>() {
                new RDFResource("bnode:HasExactlyOneNumericPositionValue"), new RDFResource("bnode:HasExactlyOneNominalPositionValue") });
            classModel.DeclareSubClasses(RDFVocabulary.TIME.DATETIME_DESCRIPTION, RDFVocabulary.TIME.GENERAL_DATETIME_DESCRIPTION);
            classModel.DeclareSubClasses(RDFVocabulary.TIME.DATETIME_DESCRIPTION, new RDFResource("bnode:HasGregorianTRSValue"));
            classModel.DeclareSubClasses(RDFVocabulary.TIME.DATETIME_DESCRIPTION, new RDFResource("bnode:HasAllYearValuesFromGYear"));
            classModel.DeclareSubClasses(RDFVocabulary.TIME.DATETIME_DESCRIPTION, new RDFResource("bnode:HasAllMonthValuesFromGMonth"));
            classModel.DeclareSubClasses(RDFVocabulary.TIME.DATETIME_DESCRIPTION, new RDFResource("bnode:HasAllDayValuesFromGDay"));
            classModel.DeclareSubClasses(RDFVocabulary.TIME.DATETIME_INTERVAL, RDFVocabulary.TIME.PROPER_INTERVAL);
            classModel.DeclareSubClasses(RDFVocabulary.TIME.DURATION, RDFVocabulary.TIME.TEMPORAL_DURATION);
            classModel.DeclareSubClasses(RDFVocabulary.TIME.DURATION, new RDFResource("bnode:HasExactlyOneNumericDurationValue"));
            classModel.DeclareSubClasses(RDFVocabulary.TIME.DURATION, new RDFResource("bnode:HasExactlyOneUnitTypeValue"));
            classModel.DeclareSubClasses(RDFVocabulary.TIME.DURATION_DESCRIPTION, RDFVocabulary.TIME.GENERAL_DURATION_DESCRIPTION);
            classModel.DeclareSubClasses(RDFVocabulary.TIME.DURATION_DESCRIPTION, new RDFResource("bnode:HasGregorianTRSValue"));
            classModel.DeclareSubClasses(RDFVocabulary.TIME.DURATION_DESCRIPTION, new RDFResource("bnode:HasAllYearsValuesFromDecimal"));
            classModel.DeclareSubClasses(RDFVocabulary.TIME.DURATION_DESCRIPTION, new RDFResource("bnode:HasAllMonthsValuesFromDecimal"));
            classModel.DeclareSubClasses(RDFVocabulary.TIME.DURATION_DESCRIPTION, new RDFResource("bnode:HasAllWeeksValuesFromDecimal"));
            classModel.DeclareSubClasses(RDFVocabulary.TIME.DURATION_DESCRIPTION, new RDFResource("bnode:HasAllDaysValuesFromDecimal"));
            classModel.DeclareSubClasses(RDFVocabulary.TIME.DURATION_DESCRIPTION, new RDFResource("bnode:HasAllHoursValuesFromDecimal"));
            classModel.DeclareSubClasses(RDFVocabulary.TIME.DURATION_DESCRIPTION, new RDFResource("bnode:HasAllMinutesValuesFromDecimal"));
            classModel.DeclareSubClasses(RDFVocabulary.TIME.DURATION_DESCRIPTION, new RDFResource("bnode:HasAllSecondsValuesFromDecimal"));
            classModel.DeclareSubClasses(RDFVocabulary.TIME.GENERAL_DURATION_DESCRIPTION, RDFVocabulary.TIME.TEMPORAL_DURATION);
            classModel.DeclareSubClasses(RDFVocabulary.TIME.GENERAL_DURATION_DESCRIPTION, new RDFResource("bnode:HasExactlyOneTRSValue"));
            classModel.DeclareSubClasses(RDFVocabulary.TIME.GENERAL_DURATION_DESCRIPTION, new RDFResource("bnode:HasMaximumOneYearsValue"));
            classModel.DeclareSubClasses(RDFVocabulary.TIME.GENERAL_DURATION_DESCRIPTION, new RDFResource("bnode:HasMaximumOneMonthsValue"));
            classModel.DeclareSubClasses(RDFVocabulary.TIME.GENERAL_DURATION_DESCRIPTION, new RDFResource("bnode:HasMaximumOneDaysValue"));
            classModel.DeclareSubClasses(RDFVocabulary.TIME.GENERAL_DURATION_DESCRIPTION, new RDFResource("bnode:HasMaximumOneHoursValue"));
            classModel.DeclareSubClasses(RDFVocabulary.TIME.GENERAL_DURATION_DESCRIPTION, new RDFResource("bnode:HasMaximumOneMinutesValue"));
            classModel.DeclareSubClasses(RDFVocabulary.TIME.GENERAL_DURATION_DESCRIPTION, new RDFResource("bnode:HasMaximumOneSecondsValue"));
            classModel.DeclareSubClasses(RDFVocabulary.TIME.GENERAL_DURATION_DESCRIPTION, new RDFResource("bnode:HasMaximumOneWeeksValue"));
            classModel.DeclareSubClasses(RDFVocabulary.TIME.INSTANT, RDFVocabulary.TIME.TEMPORAL_ENTITY);
            classModel.DeclareSubClasses(RDFVocabulary.TIME.INTERVAL, RDFVocabulary.TIME.TEMPORAL_ENTITY);
            classModel.DeclareSubClasses(RDFVocabulary.TIME.MONTH_OF_YEAR_CLASS, RDFVocabulary.TIME.DATETIME_DESCRIPTION);
            classModel.DeclareSubClasses(RDFVocabulary.TIME.MONTH_OF_YEAR_CLASS, new RDFResource("bnode:HasNoYearValues"));
            classModel.DeclareSubClasses(RDFVocabulary.TIME.MONTH_OF_YEAR_CLASS, new RDFResource("bnode:HasExactlyOneMonthValues"));
            classModel.DeclareSubClasses(RDFVocabulary.TIME.MONTH_OF_YEAR_CLASS, new RDFResource("bnode:HasNoWeekValues"));
            classModel.DeclareSubClasses(RDFVocabulary.TIME.MONTH_OF_YEAR_CLASS, new RDFResource("bnode:HasNoDayValues"));
            classModel.DeclareSubClasses(RDFVocabulary.TIME.MONTH_OF_YEAR_CLASS, new RDFResource("bnode:HasNoHourValues"));
            classModel.DeclareSubClasses(RDFVocabulary.TIME.MONTH_OF_YEAR_CLASS, new RDFResource("bnode:HasNoMinuteValues"));
            classModel.DeclareSubClasses(RDFVocabulary.TIME.MONTH_OF_YEAR_CLASS, new RDFResource("bnode:HasNoSecondValues"));
            classModel.DeclareSubClasses(RDFVocabulary.TIME.MONTH_OF_YEAR_CLASS, new RDFResource("bnode:HasUnitTypeMonthValue"));
            classModel.DeclareSubClasses(RDFVocabulary.TIME.PROPER_INTERVAL, RDFVocabulary.TIME.INTERVAL);
            classModel.DeclareSubClasses(RDFVocabulary.TIME.TEMPORAL_UNIT, RDFVocabulary.TIME.TEMPORAL_DURATION);
            classModel.DeclareSubClasses(RDFVocabulary.TIME.TIME_POSITION, RDFVocabulary.TIME.TEMPORAL_POSITION);
            classModel.DeclareSubClasses(RDFVocabulary.TIME.TIME_POSITION, new RDFResource("bnode:HasExactlyOneNumericPositionValueOrNominalPositionValue"));
            classModel.DeclareSubClasses(RDFVocabulary.TIME.TEMPORAL_POSITION, new RDFResource("bnode:HasExactlyOneTRSValue"));
            classModel.DeclareSubClasses(RDFVocabulary.TIME.GENERAL_DATETIME_DESCRIPTION, RDFVocabulary.TIME.TEMPORAL_POSITION);
            classModel.DeclareSubClasses(RDFVocabulary.TIME.GENERAL_DATETIME_DESCRIPTION, new RDFResource("bnode:HasMaximumOneTimeZoneValue"));
            classModel.DeclareSubClasses(RDFVocabulary.TIME.GENERAL_DATETIME_DESCRIPTION, new RDFResource("bnode:HasExactlyOneUnitTypeValue"));
            classModel.DeclareSubClasses(RDFVocabulary.TIME.GENERAL_DATETIME_DESCRIPTION, new RDFResource("bnode:HasMaximumOneYearValue"));
            classModel.DeclareSubClasses(RDFVocabulary.TIME.GENERAL_DATETIME_DESCRIPTION, new RDFResource("bnode:HasMaximumOneMonthValue"));
            classModel.DeclareSubClasses(RDFVocabulary.TIME.GENERAL_DATETIME_DESCRIPTION, new RDFResource("bnode:HasMaximumOneDayValue"));
            classModel.DeclareSubClasses(RDFVocabulary.TIME.GENERAL_DATETIME_DESCRIPTION, new RDFResource("bnode:HasMaximumOneHourValue"));
            classModel.DeclareSubClasses(RDFVocabulary.TIME.GENERAL_DATETIME_DESCRIPTION, new RDFResource("bnode:HasMaximumOneMinuteValue"));
            classModel.DeclareSubClasses(RDFVocabulary.TIME.GENERAL_DATETIME_DESCRIPTION, new RDFResource("bnode:HasMaximumOneSecondValue"));
            classModel.DeclareSubClasses(RDFVocabulary.TIME.GENERAL_DATETIME_DESCRIPTION, new RDFResource("bnode:HasMaximumOneWeekValue"));
            classModel.DeclareSubClasses(RDFVocabulary.TIME.GENERAL_DATETIME_DESCRIPTION, new RDFResource("bnode:HasMaximumOneDayOfYearValue"));
            classModel.DeclareSubClasses(RDFVocabulary.TIME.GENERAL_DATETIME_DESCRIPTION, new RDFResource("bnode:HasMaximumOneDayOfWeekValue"));
            classModel.DeclareSubClasses(RDFVocabulary.TIME.GENERAL_DATETIME_DESCRIPTION, new RDFResource("bnode:HasMaximumOneMonthOfYearValue"));            
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
            propertyModel.DeclareObjectProperty(RDFVocabulary.TIME.AFTER, new OWLOntologyObjectPropertyBehavior() { 
                Domain = RDFVocabulary.TIME.TEMPORAL_ENTITY, Range = RDFVocabulary.TIME.TEMPORAL_ENTITY });
            propertyModel.DeclareObjectProperty(RDFVocabulary.TIME.BEFORE, new OWLOntologyObjectPropertyBehavior() { 
                Domain = RDFVocabulary.TIME.TEMPORAL_ENTITY, Range = RDFVocabulary.TIME.TEMPORAL_ENTITY });
            propertyModel.DeclareObjectProperty(RDFVocabulary.TIME.DAY_OF_WEEK, new OWLOntologyObjectPropertyBehavior() { 
                Domain = RDFVocabulary.TIME.GENERAL_DATETIME_DESCRIPTION, Range = RDFVocabulary.TIME.DAY_OF_WEEK_CLASS });
            propertyModel.DeclareObjectProperty(RDFVocabulary.TIME.HAS_BEGINNING, new OWLOntologyObjectPropertyBehavior() { 
                Domain = RDFVocabulary.TIME.TEMPORAL_ENTITY, Range = RDFVocabulary.TIME.INSTANT });
            propertyModel.DeclareObjectProperty(RDFVocabulary.TIME.HAS_DATETIME_DESCRIPTION, new OWLOntologyObjectPropertyBehavior() { 
                Domain = RDFVocabulary.TIME.DATETIME_INTERVAL, Range = RDFVocabulary.TIME.GENERAL_DATETIME_DESCRIPTION });
            propertyModel.DeclareObjectProperty(RDFVocabulary.TIME.HAS_DURATION, new OWLOntologyObjectPropertyBehavior() { 
                Range = RDFVocabulary.TIME.DURATION });
            propertyModel.DeclareObjectProperty(RDFVocabulary.TIME.HAS_DURATION_DESCRIPTION, new OWLOntologyObjectPropertyBehavior() { 
                Range = RDFVocabulary.TIME.DURATION_DESCRIPTION });
            propertyModel.DeclareObjectProperty(RDFVocabulary.TIME.HAS_END, new OWLOntologyObjectPropertyBehavior() { 
                Domain = RDFVocabulary.TIME.TEMPORAL_ENTITY, Range = RDFVocabulary.TIME.INSTANT });
            propertyModel.DeclareObjectProperty(RDFVocabulary.TIME.HAS_TEMPORAL_DURATION, new OWLOntologyObjectPropertyBehavior() { 
                Domain = RDFVocabulary.TIME.TEMPORAL_ENTITY, Range = RDFVocabulary.TIME.TEMPORAL_DURATION });
            propertyModel.DeclareObjectProperty(RDFVocabulary.TIME.HAS_TIME, new OWLOntologyObjectPropertyBehavior() { 
                Range = RDFVocabulary.TIME.TEMPORAL_ENTITY });
            propertyModel.DeclareObjectProperty(RDFVocabulary.TIME.HAS_TRS, new OWLOntologyObjectPropertyBehavior() { 
                Domain = new RDFResource("bnode:HasTRSDomain"), Range = RDFVocabulary.TIME.TRS, Functional = true });

            propertyModel.DeclareDatatypeProperty(RDFVocabulary.TIME.DAY, new OWLOntologyDatatypePropertyBehavior() { 
                Domain = RDFVocabulary.TIME.GENERAL_DATETIME_DESCRIPTION });
            propertyModel.DeclareDatatypeProperty(RDFVocabulary.TIME.DAY_OF_YEAR, new OWLOntologyDatatypePropertyBehavior() { 
                Domain = RDFVocabulary.TIME.GENERAL_DATETIME_DESCRIPTION, Range = RDFVocabulary.XSD.NON_NEGATIVE_INTEGER });
            propertyModel.DeclareDatatypeProperty(RDFVocabulary.TIME.DAYS, new OWLOntologyDatatypePropertyBehavior() { 
                Domain = RDFVocabulary.TIME.GENERAL_DATETIME_DESCRIPTION, Range = RDFVocabulary.XSD.DECIMAL });
            propertyModel.DeclareDatatypeProperty(RDFVocabulary.TIME.HAS_XSD_DURATION, new OWLOntologyDatatypePropertyBehavior() { 
                Domain = RDFVocabulary.TIME.TEMPORAL_ENTITY, Range = RDFVocabulary.XSD.DURATION });
            propertyModel.DeclareDatatypeProperty(RDFVocabulary.TIME.HOUR, new OWLOntologyDatatypePropertyBehavior() { 
                Domain = RDFVocabulary.TIME.GENERAL_DATETIME_DESCRIPTION, Range = RDFVocabulary.XSD.NON_NEGATIVE_INTEGER });
            propertyModel.DeclareDatatypeProperty(RDFVocabulary.TIME.HOURS, new OWLOntologyDatatypePropertyBehavior() { 
                Domain = RDFVocabulary.TIME.GENERAL_DURATION_DESCRIPTION, Range = RDFVocabulary.XSD.DECIMAL });

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