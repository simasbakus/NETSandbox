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
        }

        [Test]
        public void VATIsNotAdded_WhenProviderIsNotVATPayer()
        {
            // arrange
            var invoice = new Invoice(_customer, _serviceProvider, 100);

            _serviceProvider.VATPayer.Returns(false);

            // act
            invoice.RecalculateTotalPriceWithVAT();

            // assert
            Assert.AreEqual(invoice.TotalPrice, invoice.TotalPriceWithVAT);
        }

        [Test]
        public void VATIsNotAdded_WhenCustomerIsNotInEU()
        {
            // arrange
            var invoice = new Invoice(_customer, _serviceProvider, 100);

            _serviceProvider.VATPayer.Returns(true);
            _customer.CountryCode.Returns("USA");

            // act
            invoice.RecalculateTotalPriceWithVAT();

            // assert
            Assert.AreEqual(invoice.TotalPrice, invoice.TotalPriceWithVAT);
        }

        [Test]
        public void VATIsAdded_WhenCustomerAndProviderLiveInSameEUCountry()
        {
            // arrange
            var invoice = new Invoice(_customer, _serviceProvider, 100);

            _serviceProvider.VATPayer.Returns(true);
            _serviceProvider.Country.Returns("LT");
            _customer.CountryCode.Returns("LT");

            var customerCountry = _euCountriesWithVAT.Find(x => x.Code == _customer.CountryCode);

            // act
            invoice.RecalculateTotalPriceWithVAT();

            // assert
            Assert.AreNotEqual(invoice.TotalPrice, invoice.TotalPriceWithVAT);
            Assert.AreEqual(customerCountry.VAT, invoice.VATApplied);
        }

        [Test]
        public void VATIsAdded_WhenCustomerIsNotIndividualAndNotInSameCountryAsProvider()
        {
            // arrange
            var invoice = new Invoice(_customer, _serviceProvider, 100);

            _serviceProvider.VATPayer.Returns(true);
            _serviceProvider.Country.Returns("LT");
            _customer.CountryCode.Returns("AT");
            _customer.IsIndividual.Returns(false);

            var customerCountry = _euCountriesWithVAT.Find(x => x.Code == _customer.CountryCode);

            // act
            invoice.RecalculateTotalPriceWithVAT();

            // assert
            Assert.AreNotEqual(invoice.TotalPrice, invoice.TotalPriceWithVAT);
            Assert.AreEqual(customerCountry.VAT, invoice.VATApplied);
        }

        [Test]
        public void VATIsAdded_WhenCustomerIsIndividualAndNotInSameCountryAsProvider()
        {
            // arrange
            var invoice = new Invoice(_customer, _serviceProvider, 100);

            _serviceProvider.VATPayer.Returns(true);
            _serviceProvider.Country.Returns("LT");
            _customer.CountryCode.Returns("AT");
            _customer.IsIndividual.Returns(true);

            var customerCountry = _euCountriesWithVAT.Find(x => x.Code == _customer.CountryCode);

            // act
            invoice.RecalculateTotalPriceWithVAT();

            // assert
            Assert.AreNotEqual(invoice.TotalPrice, invoice.TotalPriceWithVAT);
            Assert.AreEqual(customerCountry.VAT, invoice.VATApplied);
        }
    }
}