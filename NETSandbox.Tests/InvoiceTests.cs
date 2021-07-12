using NETSandbox.Entities;
using Newtonsoft.Json;
using NSubstitute;
using NUnit.Framework;
using System.Collections.Generic;
using System.IO;

namespace NETSandbox.Tests
{
    public class InvoiceTests
    {
        private ICustomer _customer;
        private IServiceProvider _serviceProvider;
        private Invoice _invoice;
        private readonly List<VATRate> _euCountriesWithVAT;

        public InvoiceTests()
        {
            _euCountriesWithVAT = JsonConvert.DeserializeObject<List<VATRate>>(File.ReadAllText("EuVats.json"));
        }

        [SetUp]
        public void Setup()
        {
            _customer = Substitute.For<ICustomer>();
            _serviceProvider = Substitute.For<IServiceProvider>();

            _invoice = new Invoice(_customer, _serviceProvider, _euCountriesWithVAT, 100);
        }

        [TestCase(false)] // service provider is not a VAT payer
        [TestCase(true, "USA")] // service provider is VAT payer but customer is not from EU
        [TestCase(true, "LT", "AT")] // service provider and customer are VAT payers and from different countries
        public void VATIsNotAdded(bool providerVATPayer, string customerCountry = "", string providerCountry = "")
        {
            // arrange
            _serviceProvider.VATPayer.Returns(providerVATPayer);
            _customer.CountryCode.Returns(customerCountry);
            _serviceProvider.Country.Returns(providerCountry);

            // act
            _invoice.RecalculateTotalPriceWithVAT();

            // assert
            Assert.AreEqual(_invoice.TotalPrice, _invoice.TotalPriceWithVAT);
        }

        [TestCase(true, "LT", "AT", true)] // different countries, provider is a VAT payer, but customer is not
        [TestCase(true, "LT", "LT", false)] // same countries, provider and customer both VAT payers
        public void VATIsAdded(bool providerVATPayer, string providerCountry, string customerCountry, bool customerIsIndividual)
        {
            // arrange
            _serviceProvider.VATPayer.Returns(providerVATPayer);
            _serviceProvider.Country.Returns(providerCountry);
            _customer.CountryCode.Returns(customerCountry);
            _customer.IsIndividual.Returns(customerIsIndividual);

            var customerCountryWithVAT = _euCountriesWithVAT.Find(x => x.Code == _customer.CountryCode);

            // act
            _invoice.RecalculateTotalPriceWithVAT();

            // assert
            Assert.AreNotEqual(_invoice.TotalPrice, _invoice.TotalPriceWithVAT);
            Assert.AreEqual(customerCountryWithVAT.VAT, _invoice.VATApplied);
        }


    }
}