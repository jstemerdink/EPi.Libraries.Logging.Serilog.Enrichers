// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultCartDataEnricher.cs" company="Jeroen Stemerdink">
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

namespace EPi.Libraries.Logging.Serilog.Enrichers.Commerce
{
    using System;
    using System.Linq;

    using EPiServer.Commerce.Order;
    using EPiServer.ServiceLocation;

    using Mediachase.Commerce.Customers;
    using Mediachase.Commerce.Orders;

    using Newtonsoft.Json;

    using global::Serilog.Core;
    using global::Serilog.Events;

    /// <summary>
    /// Class DefaultCartDataEnricher.
    /// </summary>
    /// <seealso cref="ILogEventEnricher" />
    public class DefaultCartDataEnricher : ILogEventEnricher
    {
        /// <summary>
        /// The current cart property name
        /// </summary>
        public const string CurrentCartPropertyName = "CurrentCart";

        /// <summary>
        /// Enrich the log event.
        /// </summary>
        /// <param name="logEvent">The log event to enrich.</param>
        /// <param name="propertyFactory">Factory for creating new properties to add to the event.</param>
        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            if (logEvent == null)
            {
                return;
            }

            IOrderRepository orderRepository;
            CustomerContext customerContext = CustomerContext.Current;

            if (customerContext == null || customerContext.CurrentContactId == Guid.Empty)
            {
                return;
            }

            try
            {
                orderRepository = ServiceLocator.Current.GetInstance<IOrderRepository>();
            }
            catch (ActivationException)
            {
                return;
            }

            ICart cart = orderRepository.Load<ICart>(
                customerId: customerContext.CurrentContactId,
                name: Cart.DefaultName).FirstOrDefault();

            string serializedCart = string.Empty;

            if (cart != null && cart.GetAllLineItems().Any())
            {
                try
                {
                    serializedCart = JsonConvert.SerializeObject(
                        value: cart,
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
            }

            if (!string.IsNullOrWhiteSpace(serializedCart))
            {
                logEvent.AddPropertyIfAbsent(
                    new LogEventProperty(
                        name: CurrentCartPropertyName,
                        value: new ScalarValue(serializedCart)));
            }
        }
    }
}