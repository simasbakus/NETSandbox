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

        [Test]
        public void VATIsNotAdded_WhenProviderIsNotVATPayer()
        {
            // arrange
            _serviceProvider.VATPayer.Returns(false);

            // act
            _invoice.RecalculateTotalPriceWithVAT();

            // assert
            Assert.AreEqual(_invoice.TotalPrice, _invoice.TotalPriceWithVAT);
        }

        [Test]
        public void VATIsNotAdded_WhenCustomerIsNotInEU()
        {
            // arrange
            _serviceProvider.VATPayer.Returns(true);
            _customer.CountryCode.Returns("USA");

            // act
            _invoice.RecalculateTotalPriceWithVAT();

            // assert
            Assert.AreEqual(_invoice.TotalPrice, _invoice.TotalPriceWithVAT);
        }

        [Test]
        public void VATIsAdded_WhenCustomerAndProviderLiveInSameEUCountry()
        {
            // arrange
            _serviceProvider.VATPayer.Returns(true);
            _serviceProvider.Country.Returns("LT");
            _customer.CountryCode.Returns("LT");

            var customerCountry = _euCountriesWithVAT.Find(x => x.Code == _customer.CountryCode);

            // act
            _invoice.RecalculateTotalPriceWithVAT();

            // assert
            Assert.AreNotEqual(_invoice.TotalPrice, _invoice.TotalPriceWithVAT);
            Assert.AreEqual(customerCountry.VAT, _invoice.VATApplied);
        }

        [Test]
        public void VATIsAdded_WhenCustomerIsNotIndividualAndNotInSameCountryAsProvider()
        {
            // arrange
            _serviceProvider.VATPayer.Returns(true);
            _serviceProvider.Country.Returns("LT");
            _customer.CountryCode.Returns("AT");
            _customer.IsIndividual.Returns(false);

            var customerCountry = _euCountriesWithVAT.Find(x => x.Code == _customer.CountryCode);

            // act
            _invoice.RecalculateTotalPriceWithVAT();

            // assert
            Assert.AreNotEqual(_invoice.TotalPrice, _invoice.TotalPriceWithVAT);
            Assert.AreEqual(customerCountry.VAT, _invoice.VATApplied);
        }

        [Test]
        public void VATIsAdded_WhenCustomerIsIndividualAndNotInSameCountryAsProvider()
        {
            // arrange
            _serviceProvider.VATPayer.Returns(true);
            _serviceProvider.Country.Returns("LT");
            _customer.CountryCode.Returns("AT");
            _customer.IsIndividual.Returns(true);

            var customerCountry = _euCountriesWithVAT.Find(x => x.Code == _customer.CountryCode);

            // act
            _invoice.RecalculateTotalPriceWithVAT();

            // assert
            Assert.AreNotEqual(_invoice.TotalPrice, _invoice.TotalPriceWithVAT);
            Assert.AreEqual(customerCountry.VAT, _invoice.VATApplied);
        }
    }
}