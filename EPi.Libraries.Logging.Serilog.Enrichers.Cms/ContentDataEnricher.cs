// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ContentDataEnricher.cs" company="Jeroen Stemerdink">
//      Copyright © 2019 Jeroen Stemerdink.
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

    using EPiServer.Globalization;
    using EPiServer.ServiceLocation;
    using EPiServer.Web.Routing;

    using Newtonsoft.Json;

    using global::Serilog.Core;
    using global::Serilog.Events;

    /// <summary>
    ///     Class CmsDataEnricher.
    /// </summary>
    /// <seealso cref="ILogEventEnricher" />
    public class ContentDataEnricher : ILogEventEnricher
    {
        /// <summary>
        ///     The content data property name
        /// </summary>
        public const string ContentDataPropertyName = "ContentData";

        /// <summary>
        ///     The content identifier property name
        /// </summary>
        public const string ContentIdPropertyName = "ContentId";

        /// <summary>
        ///     The preferred culture property name
        /// </summary>
        public const string PreferredCulturePropertyName = "PreferredCulture";

        /// <summary>
        ///     Enrich the log event.
        /// </summary>
        /// <param name="logEvent">The log event to enrich.</param>
        /// <param name="propertyFactory">Factory for creating new properties to add to the event.</param>
        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            if (logEvent == null)
            {
                return;
            }

            IContentRouteHelper contentRouteRouteHelper;

            try
            {
                contentRouteRouteHelper = ServiceLocator.Current.GetInstance<IContentRouteHelper>();
            }
            catch (ActivationException)
            {
                return;
            }

            logEvent.AddPropertyIfAbsent(
                new LogEventProperty(
                    name: PreferredCulturePropertyName,
                    value: new ScalarValue(value: ContentLanguage.PreferredCulture.Name)));

            if (contentRouteRouteHelper?.Content == null || contentRouteRouteHelper?.Content?.ContentLink == null)
            {
                return;
            }

            logEvent.AddPropertyIfAbsent(
                new LogEventProperty(
                    name: ContentIdPropertyName,
                    value: new ScalarValue(value: contentRouteRouteHelper.Content.ContentLink.ID)));

            string serializedContent = string.Empty;

            try
            {
                serializedContent = JsonConvert.SerializeObject(
                    value: contentRouteRouteHelper.Content,
                    formatting: Formatting.Indented,
                    settings: new JsonSerializerSettings
                                  {
                                      ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                                      NullValueHandling = NullValueHandling.Ignore,
                                      DefaultValueHandling = DefaultValueHandling.Ignore
                                  });
            }
            catch
            {
            }

            if (!string.IsNullOrWhiteSpace(serializedContent))
            {
                logEvent.AddPropertyIfAbsent(
                    new LogEventProperty(
                        name: ContentDataPropertyName,
                        value: new ScalarValue(serializedContent)));
            }
        }
    }
}