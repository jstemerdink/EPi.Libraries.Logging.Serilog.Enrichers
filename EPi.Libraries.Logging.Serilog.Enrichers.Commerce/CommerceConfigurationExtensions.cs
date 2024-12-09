// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CommerceConfigurationExtensions.cs" company="Jeroen Stemerdink">
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

namespace EPi.Libraries.Logging.Serilog.Enrichers.Commerce
{
    using System;

    using global::Serilog;
    using global::Serilog.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    /// <summary>
    /// Class CommerceConfigurationExtensions.
    /// </summary>
    public static class CommerceConfigurationExtensions
    {
        /// <summary>
        /// Enrich the logging with customer data.
        /// </summary>
        /// <param name="enrichmentConfiguration">The enrichment configuration.</param>
        /// <param name="serviceProvider">The service provider.</param>
        /// <returns>The LoggerConfiguration.</returns>
        /// <exception cref="System.ArgumentNullException">enrichmentConfiguration is null</exception>
        public static LoggerConfiguration WithCustomerData(this LoggerEnrichmentConfiguration enrichmentConfiguration, IServiceProvider serviceProvider)
        {
            if (enrichmentConfiguration == null)
            {
                throw new ArgumentNullException(nameof(enrichmentConfiguration));
            }

            ContactDataEnricher enricher = serviceProvider.GetService<ContactDataEnricher>();

            return enrichmentConfiguration.With(enricher);
        }

        /// <summary>
        /// Enrich the logging with cart data.
        /// </summary>
        /// <param name="enrichmentConfiguration">The enrichment configuration.</param>
        /// <returns>The LoggerConfiguration.</returns>
        /// <exception cref="ArgumentNullException">enrichmentConfiguration is null</exception>
        public static LoggerConfiguration WithDefaultCartData(
            this LoggerEnrichmentConfiguration enrichmentConfiguration)
        {
            if (enrichmentConfiguration == null)
            {
                throw new ArgumentNullException(nameof(enrichmentConfiguration));
            }

            return enrichmentConfiguration.With<DefaultCartDataEnricher>();
        }

        /// <summary>
        /// Enrich the logging with market data.
        /// </summary>
        /// <param name="enrichmentConfiguration">The enrichment configuration.</param>
        /// <returns>The LoggerConfiguration.</returns>
        /// <exception cref="ArgumentNullException">enrichmentConfiguration is null</exception>
        public static LoggerConfiguration WithMarketData(this LoggerEnrichmentConfiguration enrichmentConfiguration)
        {
            if (enrichmentConfiguration == null)
            {
                throw new ArgumentNullException(nameof(enrichmentConfiguration));
            }

            return enrichmentConfiguration.With<MarketDataEnricher>();
        }
    }
}