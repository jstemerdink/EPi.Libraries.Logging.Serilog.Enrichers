// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultCartDataEnricher.cs" company="Jeroen Stemerdink">
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

        readonly string _propertyName;
        readonly string _cartName;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultCartDataEnricher"/> class.
        /// </summary>
        public DefaultCartDataEnricher()
        {
            _propertyName = CurrentCartPropertyName;
            _cartName = Cart.DefaultName;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultCartDataEnricher" /> class.
        /// </summary>
        /// <param name="propertyName">The name.</param>
        /// <param name="cartName">Name of the cart.</param>
        public DefaultCartDataEnricher(string propertyName, string cartName)
        {
            _propertyName = propertyName;
            _cartName = cartName;
        }

        /// <summary>
        /// Enrich the log event.
        /// </summary>
        /// <param name="logEvent">The log event to enrich.</param>
        /// <param name="propertyFactory">Factory for creating new properties to add to the event.</param>
        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            IOrderRepository orderRepository;
            CustomerContext customerContext = CustomerContext.Current;

            if (customerContext == null || customerContext.CurrentContactId == Guid.Empty)
            {
                return;
            }

            string serializedCart = Helpers.GetCartData(_cartName);
            
            if (!string.IsNullOrWhiteSpace(serializedCart))
            {
                logEvent.AddPropertyIfAbsent(
                    new LogEventProperty(
                        name: _propertyName,
                        value: new ScalarValue(serializedCart)));
            }
        }

    }
}