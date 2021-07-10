using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NETSandbox.Entities
{
    public class Invoice
    {
        private readonly ICustomer _customer;
        private readonly IServiceProvider _serviceProvider;
        private readonly List<VATRate> _euCountriesWithVAT;

        public double TotalPrice { get; set; }
        public int VATApplied { get; set; }
        public double TotalPriceWithVAT { get; set; }

        public Invoice(ICustomer customer, IServiceProvider serviceProvider, int totalPrice)
        {
            _customer = customer;
            _serviceProvider = serviceProvider;
            TotalPrice = totalPrice;

            _euCountriesWithVAT = JsonConvert.DeserializeObject<List<VATRate>>(File.ReadAllText("EuVats.json"));
        }

        public void RecalculateTotalPriceWithVAT()
        {
            if (!_serviceProvider.VATPayer 
                || (!string.IsNullOrEmpty(_customer.CountryCode) && !_euCountriesWithVAT.Exists(x => x.Code == _customer.CountryCode)))
            {
                /* Service provider is not a VAT payer or Customer is not in EU */

                TotalPriceWithVAT = TotalPrice;
                VATApplied = 0;
            }
            else
            {
                if (_customer.CountryCode == _serviceProvider.Country || _customer.IsIndividual)
                {
                    /* Customer and service provider live in the same EU country or they  */
                    /* live in different countries but the customer is not a VAT payer    */

                    VATApplied = _euCountriesWithVAT.Find(x => x.Code == _customer.CountryCode).VAT;
                    TotalPriceWithVAT = TotalPrice * (1 + VATApplied * 0.01);
                }
                else
                {

                }
            }
        }
    }
}
