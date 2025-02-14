// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Helpers.cs" company="Jeroen Stemerdink">
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

using System.Collections.Generic;
using System.Linq;
using EPiServer.Commerce.Order;
using EPiServer.ServiceLocation;
using Mediachase.Commerce;
using Mediachase.Commerce.Customers;
using Mediachase.Commerce.Orders;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace EPi.Libraries.Logging.Serilog.Enrichers.Commerce
{
    public static class Helpers
    {
        public static string GetCartData()
        {
            return GetCartData(Cart.DefaultName);
        }

        public static string GetCartData(string cartName)
        {
            IOrderRepository orderRepository;
            CustomerContext customerContext = CustomerContext.Current;

            try
            {
                orderRepository = ServiceLocator.Current.GetInstance<IOrderRepository>();
            }
            catch (ActivationException)
            {
                return null;
            }

            ICart cart = orderRepository.Load<ICart>(
                customerId: customerContext.CurrentContactId,
                name: cartName).FirstOrDefault();

            return GetCartData(cart);
        }

        public static string GetCartData(ICart cart)
        {
            string serializedCart = string.Empty;

            if (cart == null || !cart.GetAllLineItems().Any())
            {
                return serializedCart;
            }

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
                return null;
            }

            return serializedCart;
        }

        public static Dictionary<string, object> GetMarketData()
        {
            return GetMarketData(true);
        }

        public static Dictionary<string, object> GetMarketData(bool limitData)
        {
            Dictionary<string, object> marketData = new Dictionary<string, object>();

            ICurrentMarket currentMarket;

            try
            {
                currentMarket = ServiceLocator.Current.GetInstance<ICurrentMarket>();
            }
            catch (ActivationException)
            {
                return null;
            }

            var market = currentMarket.GetCurrentMarket();
            marketData.TryAdd("MarketName", market.MarketName);
            marketData.TryAdd("MarketDescription", market.MarketDescription);
            marketData.TryAdd("MarketId", market.MarketId);

            if (limitData)
            {
                return marketData;
            }

            marketData.TryAdd("MarketDefaultCurrency", market.DefaultCurrency);
            marketData.TryAdd("MarketDefaultLanguage", market.DefaultLanguage);
            marketData.TryAdd("MarketPricesIncludeTax", market.PricesIncludeTax);
            marketData.TryAdd("MarketCountries", market.Countries);
            marketData.TryAdd("MarketCurrencies", market.Currencies.Select(c => c.CurrencyCode));
            marketData.TryAdd("MarketLanguages", market.Languages.Select(l => l.Name));

            return marketData;
        }

        public static Dictionary<string, object> GetContactData()
        {
            return GetContactData(true);
        }

        public static Dictionary<string, object> GetContactData(bool limitData)
        {
            Dictionary<string, object> customerData = new Dictionary<string, object>();
            CustomerContext customerContext = CustomerContext.Current;

            if (customerContext == null)
            {
                return customerData;
            }
            
            customerData.TryAdd("ContactId", customerContext.CurrentContactId);
            customerData.TryAdd("ContactName", customerContext.CurrentContactName);
            
            if (limitData)
            {
                return customerData;
            }

            var customerContact = customerContext.CurrentContact;

            if (customerContact == null)
            {
                return customerData;
            }

            customerData.TryAdd("ContactEmail", customerContact.Email);
            customerData.TryAdd("ContactRegistrationSource", customerContact.RegistrationSource);
            customerData.TryAdd("ContactCustomerGroup", customerContact.CustomerGroup);
            customerData.TryAdd("ContactOrganization", customerContact.ContactOrganization?.Name);

            return customerData;
        }
    }
}
