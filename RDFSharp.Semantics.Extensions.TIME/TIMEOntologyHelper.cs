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
    }
}