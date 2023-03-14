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

using NodaTime;
using RDFSharp.Model;
using System;

namespace RDFSharp.Semantics.Extensions.TIME
{
    /// <summary>
    /// TIMEInterval represents a temporal entity with an extent or duration
    /// </summary>
    public class TIMEInterval : RDFResource
    {
        #region Properties
        /// <summary>
        /// Value of the time interval
        /// </summary>
        internal Interval Value { get; set; }
        #endregion

        #region Ctors
        /// <summary>
        /// Default-ctor to build a time interval with the given Uri
        /// </summary>
        public TIMEInterval(RDFResource timeIntervalUri, DateTime timeIntervalStart, DateTime timeIntervalEnd) 
            : base(timeIntervalUri?.ToString())
        {
            if (timeIntervalEnd < timeIntervalStart)
                throw new OWLSemanticsException("Cannot represent time interval because given \"timeIntervalEnd\" parameter must be greater or equal than given \"timeIntervalStart\" parameter ");

            Value = new Interval(Instant.FromDateTimeUtc(timeIntervalStart), Instant.FromDateTimeUtc(timeIntervalEnd));
        }
        #endregion
    }
}