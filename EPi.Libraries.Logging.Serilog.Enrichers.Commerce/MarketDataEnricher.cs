// Copyright © 2019 Jeroen Stemerdink.
// Permission is hereby granted, free of charge, to any person
// obtaining a copy of this software and associated documentation
// files (the "Software"), to deal in the Software without
// restriction, including without limitation the rights to use,
// copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following
// conditions:
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.
namespace EPi.Libraries.Logging.Serilog.Enrichers.Commerce
{
    using EPiServer.ServiceLocation;

    using global::Serilog.Core;
    using global::Serilog.Events;

    using Mediachase.Commerce;

    /// <summary>
    /// Class CommerceDataEnricher.
    /// </summary>
    /// <seealso cref="ILogEventEnricher" />
    public class MarketDataEnricher : ILogEventEnricher 
    {
        /// <summary>
        /// The current market property name
        /// </summary>
        public const string CurrentMarketPropertyName = "CurrentMarket";

        /// <summary>
        /// Enrich the log event.
        /// </summary>
        /// <param name="logEvent">The log event to enrich.</param>
        /// <param name="propertyFactory">Factory for creating new properties to add to the event.</param>
        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
           if (logEvent == null || logEvent.Level != LogEventLevel.Error || logEvent.Level != LogEventLevel.Fatal)
            {
                return;
            }

            ICurrentMarket currentMarket;

            try
            {
                currentMarket = ServiceLocator.Current.GetInstance<ICurrentMarket>();
            }
            catch (ActivationException)
            {
                return;
            }

            if (currentMarket != null)
            {
                logEvent.AddPropertyIfAbsent(new LogEventProperty(CurrentMarketPropertyName, new ScalarValue(currentMarket.GetCurrentMarket().MarketName)));
            }
        }
    }
}
