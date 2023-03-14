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
    /// TIMEDuration represents a time extent
    /// </summary>
    public class TIMEDuration : RDFResource
    {
        #region Properties
        /// <summary>
        /// Value of the time duration
        /// </summary>
        internal Duration Value { get; set; }

        /// <summary>
        /// Description of the time duration
        /// </summary>
        public TIMEDurationDescription Description { get; internal set; }
        #endregion

        #region Ctors
        /// <summary>
        /// Default-ctor to build a time duration with the given Uri and value
        /// </summary>
        public TIMEDuration(RDFResource timeDurationUri, TimeSpan timeDurationValue)
            : this(timeDurationUri, timeDurationValue, TIMEOntologyHelper.GregorianTRS) { }

        /// <summary>
        /// Default-ctor to build a time duration with the given Uri, value and time reference system
        /// </summary>
        public TIMEDuration(RDFResource timeDurationUri, TimeSpan timeDurationValue, RDFResource timeReferenceSystemUri)
            : base(timeDurationUri?.ToString())
        {
            Value = Duration.FromTimeSpan(timeDurationValue);

            //In order to build the descritpon of the duration, we need to represent it as NodaTime.Period
            //which gives us all the data structures we need for OWL modeling (Years, Months, Weeks)
            Period period = Period.FromTicks(timeDurationValue.Ticks);
            Description = new TIMEDurationDescription()
            {
                TRSUri = timeReferenceSystemUri ?? TIMEOntologyHelper.GregorianTRS,
                Years = period.Years,
                Months = period.Months,
                Weeks = period.Weeks,
                Days = period.Days,
                Hours = period.Hours,
                Minutes = period.Minutes,
                Seconds = period.Seconds
            };
        }

        #endregion
    }

    /// <summary>
    /// TIMEDurationDescription represents a granular description of a time extent
    /// </summary>
    public class TIMEDurationDescription
    {
        #region Properties
        /// <summary>
        /// Represents the URI of the Time Reference System in which the time duration is expressed<br/>
        /// (if not available, it defaults to "http://www.opengis.net/def/uom/ISO-8601/0/Gregorian")
        /// </summary>
        public RDFResource TRSUri { get; set; }

        /// <summary>
        /// Represents the "years" component of the time duration
        /// </summary>
        public decimal Years { get; set; }

        /// <summary>
        /// Represents the "months" component of the time duration
        /// </summary>
        public decimal Months { get; set; }

        /// <summary>
        /// Represents the "weeks" component of the time duration
        /// </summary>
        public decimal Weeks { get; set; }

        /// <summary>
        /// Represents the "days" component of the time duration
        /// </summary>
        public decimal Days { get; set; }

        /// <summary>
        /// Represents the "hours" component of the time duration
        /// </summary>
        public decimal Hours { get; set; }

        /// <summary>
        /// Represents the "minutes" component of the time duration
        /// </summary>
        public decimal Minutes { get; set; }

        /// <summary>
        /// Represents the "seconds" component of the time duration
        /// </summary>
        public decimal Seconds { get; set; }
        #endregion
    }
}