// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SiteDataEnricher.cs" company="Jeroen Stemerdink">
//      Copyright © 2024 Jeroen Stemerdink.
//      Permission is hereby granted, free of charge, to any person obtaining a copy
//      of this software and associated documentation files (the "Software"), to deal
//      in the Software without restriction, including without limitation the rights
//      to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//      copies of the Software, and to permit persons to whom the Software is
//      furnished to do so, subject to the following conditions:
//
//      The above copyright notice and this permission notice shall be included in all
//      copies or substantial portions of the Software.
//
//      THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//      IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//      FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//      AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//      LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//      OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
//      SOFTWARE.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace EPi.Libraries.Logging.Serilog.Enrichers.Cms
{
    using System;

    using EPiServer.Web;

    using global::Serilog.Core;
    using global::Serilog.Events;

    /// <summary>
    ///     Class SiteDataEnricher.
    /// </summary>
    /// <seealso cref="ILogEventEnricher" />
    public class SiteDataEnricher : ILogEventEnricher
    {
        /// <summary>
        ///     The site identifier property name
        /// </summary>
        public const string SiteIdPropertyName = "SiteId";

        /// <summary>
        ///     The site name property name
        /// </summary>
        public const string SiteNamePropertyName = "SiteName";

        /// <summary>
        ///     The site URL property name
        /// </summary>
        public const string SiteUrlPropertyName = "SiteUrl";

        /// <summary>
        ///     Enrich the log event.
        /// </summary>
        /// <param name="logEvent">The log event to enrich.</param>
        /// <param name="propertyFactory">Factory for creating new properties to add to the event.</param>
        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            SiteDefinition siteDefinition = SiteDefinition.Current;

            if (!string.IsNullOrWhiteSpace(value: siteDefinition.Name))
            {
                logEvent.AddPropertyIfAbsent(
                    new LogEventProperty(
                        name: SiteNamePropertyName,
                        value: new ScalarValue(value: SiteDefinition.Current.Name)));
            }

            if (siteDefinition.Id != Guid.Empty)
            {
                logEvent.AddPropertyIfAbsent(
                    new LogEventProperty(
                        name: SiteIdPropertyName,
                        value: new ScalarValue(value: SiteDefinition.Current.Id)));
            }

            if (siteDefinition.SiteUrl != null)
            {
                logEvent.AddPropertyIfAbsent(
                    new LogEventProperty(
                        name: SiteUrlPropertyName,
                        value: new ScalarValue(value: SiteDefinition.Current.SiteUrl)));
            }
        }
    }
}