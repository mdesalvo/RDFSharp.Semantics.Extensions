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
    /// TIMEInstant represents a temporal entity with zero extent or duration
    /// </summary>
    public class TIMEInstant : RDFResource
    {
        #region Properties
        /// <summary>
        /// Value of the time instant
        /// </summary>
        internal Instant Value { get; set; }
        #endregion

        #region Ctors
        /// <summary>
        /// Default-ctor to build a time instant with the given Uri and value
        /// </summary>
        public TIMEInstant(RDFResource timeInstantUri, DateTime timeInstantValue)
            : base(timeInstantUri?.ToString())
        {
            Value = Instant.FromDateTimeUtc(timeInstantValue);
        }
        #endregion
    }
}