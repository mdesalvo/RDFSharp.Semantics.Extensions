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
using System.Globalization;

namespace RDFSharp.Semantics.Extensions.TIME
{
    /// <summary>
    /// Represents a temporal entity with zero extent or duration
    /// </summary>
    public class TIMEInstant : RDFResource
    {
        #region Properties
        /// <summary>
        /// Position of the time instant, expressed using xsd:dateTimeStamp
        /// </summary>
        public DateTime InXSDDateTimeStamp { get; internal set; }
        #endregion

        #region Ctors
        /// <summary>
        /// Default-ctor to build a time instant with given Uri and given xsd:xsdDateTimeStamp
        /// </summary>
        public TIMEInstant(string timeInstantUri, RDFTypedLiteral xsdDateTimeStamp) : base(timeInstantUri)
        {
            if (xsdDateTimeStamp == null)
                throw new OWLSemanticsException("Cannot build time instant since given \"xsdDateTimeStamp\" parameter is null");
            if (!xsdDateTimeStamp.Datatype.Equals(RDFModelEnums.RDFDatatypes.XSD_DATETIMESTAMP))
                throw new OWLSemanticsException("Cannot build time instant since given \"xsdDateTimeStamp\" parameter has not required \"xsd:dateTimeStamp\" datatype");

            InXSDDateTimeStamp = DateTime.Parse(xsdDateTimeStamp.Value, CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal);
        }
        #endregion

        #region Methods
        
        #endregion
    }
}