using EconomicCalculator.Storage;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EconomicCalculator.Tests.Storage
{
    [TestFixture]
    public class MarketShould
    {
        private Market sutMarket;

        private readonly Guid testGuid = Guid.NewGuid();
        private const string testName = "TestName";
        private const double testPopTotal = 100;
        private const double testTerritory = 100;

        private Mock<IPopulations> testPops;

        private Mock<IProduct> currencyMock1;
        private Mock<IProduct> currencyMock2;
        private Mock<IProduct> currencyMock3;

        private const double currencyVal1 = 100;
        private const double currencyVal2 = 25;
        private const double currencyVal3 = 1;

        private Mock<IProduct> productMock1;
        private Mock<IProduct> productMock2;
        private Mock<IProduct> productMock3;

        public const double productVal1 = 100;
        public const double productVal2 = 150;
        public const double productVal3 = 123.45;

        private Mock<IPopulationGroup> TravellerMock1;
        private Mock<IPopulationGroup> TravellerMock2;

        private IProductAmountCollection testCash;

        [SetUp]
        public void Setup()
        {
            testPops = new Mock<IPopulations>();

            #region CurrencySetup

            currencyMock1 = new Mock<IProduct>();
            currencyMock1.Setup(x => x.Id).Returns(Guid.NewGuid());
            currencyMock1.Setup(x => x.Name).Returns("Dollar");
            currencyMock2 = new Mock<IProduct>();
            currencyMock2.Setup(x => x.Id).Returns(Guid.NewGuid());
            currencyMock2.Setup(x => x.Name).Returns("Quarter");
            currencyMock3 = new Mock<IProduct>();
            currencyMock3.Setup(x => x.Id).Returns(Guid.NewGuid());
            currencyMock3.Setup(x => x.Name).Returns("Cent");

            var currencies = new List<IProduct>
            {
                currencyMock1.Object,
                currencyMock2.Object,
                currencyMock3.Object
            };

            #endregion CurrencySetup

            #region TestCash

            // get cash for tests of an arbitrarily high amount.
            testCash = new ProductAmountCollection();
            testCash.AddProducts(currencyMock1.Object, 100000);
            testCash.AddProducts(currencyMock2.Object, 100000);
            testCash.AddProducts(currencyMock3.Object, 100000);

            #endregion TestCash

            #region Products

            // start the mocks
            productMock1 = new Mock<IProduct>();
            productMock2 = new Mock<IProduct>();
            productMock3 = new Mock<IProduct>();

            // Give Ids
            productMock1.Setup(x => x.Id).Returns(Guid.NewGuid());
            productMock2.Setup(x => x.Id).Returns(Guid.NewGuid());
            productMock3.Setup(x => x.Id).Returns(Guid.NewGuid());

            // give names
            productMock1.Setup(x => x.Name).Returns("Ro");
            productMock1.Setup(x => x.Name).Returns("Sham");
            productMock1.Setup(x => x.Name).Returns("Bo");

            #endregion Products

            TravellerMock1 = new Mock<IPopulationGroup>();
            TravellerMock2 = new Mock<IPopulationGroup>();

            var travMerchants = new List<IPopulationGroup>
            {
                TravellerMock1.Object,
                TravellerMock2.Object
            };

            sutMarket = new Market
            {
                Id = testGuid,
                Name = testName,
                Populations = testPops.Object,
                Territory = testTerritory,
                TotalPopulation = testPopTotal,
                AcceptedCurrencies = currencies,
                TravellingMerchants = travMerchants,
            };

            // Add products to prices
            sutMarket.ProductPrices.AddProducts(currencyMock1.Object, currencyVal1);
            sutMarket.ProductPrices.AddProducts(currencyMock2.Object, currencyVal2);
            sutMarket.ProductPrices.AddProducts(currencyMock3.Object, currencyVal3);
            sutMarket.ProductPrices.AddProducts(productMock1.Object, productVal1);
            sutMarket.ProductPrices.AddProducts(productMock2.Object, productVal2);
            sutMarket.ProductPrices.AddProducts(productMock3.Object, productVal3);
        }

        // A helper function for avsserting products in the collection are correct.
        private void AssertProductAmountIsEqual(IProductAmountCollection collection,
            Mock<IProduct> product, double value)
        {
            Assert.That(collection.GetProductValue(product.Object), Is.EqualTo(value));
        }

        #region BuyPhase



        #endregion BuyPhase

        #region SellPhase

        [Test]
        public void CallsPopulationsSellPhaseInSellPhase()
        {
            // setup the sell phase with an arbitrary return
            testPops.Setup(x => x.SellPhase()).Returns(new ProductAmountCollection());

            // Test it's not been run.
            testPops.Verify(x => x.SellPhase(), Times.Never);

            // run sell phase.
            sutMarket.SellPhase();

            // Check the function was run.
            testPops.Verify(x => x.SellPhase(), Times.Once);
        }

        [Test]
        public void UpdateProductSupplyInSellPhase()
        {
            // setup the sell phase
            var products = new ProductAmountCollection();
            testPops.Setup(x => x.SellPhase()).Returns(products);

            // run the sell phase
            sutMarket.SellPhase();

            // check that the product supply has been copied over.
            Assert.That(sutMarket.ProductSupply, Is.EqualTo(products));
        }

        #endregion SellPhase

        #region ProductPhase

        [Test]
        public void CallPopulationProductionPhase()
        {
            // set up test
            testPops.Setup(x => x.ProductionPhase());

            // check it hasn't run yet.
            testPops.Verify(x => x.ProductionPhase(), Times.Never);

            // run production phase
            sutMarket.ProductionPhase();

            // check that it ran the function call.
            testPops.Verify(x => x.ProductionPhase(), Times.Once);
        }

        #endregion ProductPhase

        #region ConsumptionPhase

        [Test]
        public void CallPopulationsConsumptionPhaseFromConsumptionPhase()
        {
            // set up test
            testPops.Setup(x => x.Consume());

            // check it hasn't run yet.
            testPops.Verify(x => x.Consume(), Times.Never);

            // run production phase
            sutMarket.ConsumptionPhase();

            // check that it ran the function call.
            testPops.Verify(x => x.Consume(), Times.Once);
        }

        #endregion

        #region LossPhase

        [Test]
        public void CallsPopulationLossPhaseInLossPhase()
        {
            // set up test
            testPops.Setup(x => x.LossPhase());

            // check it hasn't run yet.
            testPops.Verify(x => x.LossPhase(), Times.Never);

            // run production phase
            sutMarket.LossPhase();

            // check that it ran the function call.
            testPops.Verify(x => x.LossPhase(), Times.Once);
        }

        #endregion LossPhase

        #region GetPrice

        [Test]
        public void ThrowArgumentNullFromGetPriceWithAmount()
        {
            Assert.Throws<ArgumentNullException>(() => sutMarket.GetPrice(null, 100));
        }

        [Test]
        public void ThrowArgumentNullFromGetPriceWithoutAmount()
        {
            Assert.Throws<ArgumentNullException>(() => sutMarket.GetPrice(null));
        }

        [Test]
        public void ReturnProductMarketPriceWithoutAmount()
        {
            var result = sutMarket.GetPrice(productMock1.Object);

            Assert.That(result, Is.EqualTo(productVal1));
        }

        [Test]
        [TestCase(150)]
        [TestCase(300)]
        public void ReturnProductMarketPriceWithoutAmount(double val)
        {
            var result = sutMarket.GetPrice(productMock1.Object, val);

            Assert.That(result, Is.EqualTo(productVal1 * val));
        }

        #endregion GetPrice

        #region ChangeForPrice

        [Test]
        public void ThrowArgumentNullIfAvailableCashIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => sutMarket.ChangeForPrice(null, 100));
        }

        [Test]
        [TestCase(-100)]
        [TestCase(0)]
        public void ThrowArgumentOutOfRangeIfPriceIsNegativeOrZero(double val)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => sutMarket.ChangeForPrice(testCash, val));
        }

        [Test]
        public void ReturnChangeInDescendingValueOrder()
        {
            // Get change
            var result = sutMarket.ChangeForPrice(testCash, 153);

            // check everything is in order of most valuable to least.
            Assert.That(result.Products[0].Id, Is.EqualTo(currencyMock1.Object.Id));
            Assert.That(result.Products[1].Id, Is.EqualTo(currencyMock2.Object.Id));
            Assert.That(result.Products[2].Id, Is.EqualTo(currencyMock3.Object.Id));
        }

        [Test]
        public void ReturnCorrectChangeOnExactValue()
        {
            // Get change
            var result = sutMarket.ChangeForPrice(testCash, 153);

            // Check that there is the correct number of each.
            AssertProductAmountIsEqual(result, currencyMock1, 1);
            AssertProductAmountIsEqual(result, currencyMock2, 2);
            AssertProductAmountIsEqual(result, currencyMock3, 3);
        }

        [Test]
        public void ReturnEverythingWhenNotEnoughCash()
        {
            // change cash to be less than required
            testCash.SetProductAmount(currencyMock1.Object, 1);
            testCash.SetProductAmount(currencyMock2.Object, 5);
            testCash.SetProductAmount(currencyMock3.Object, 10);

            // Get change
            var result = sutMarket.ChangeForPrice(testCash, 1000);

            // Check that there is the correct number of each.
            AssertProductAmountIsEqual(result, currencyMock1, 1);
            AssertProductAmountIsEqual(result, currencyMock2, 5);
            AssertProductAmountIsEqual(result, currencyMock3, 10);
        }

        [Test]
        public void RoundUpWhenExactValueCannotBeDoneOnChange()
        {
            // Get change
            var result = sutMarket.ChangeForPrice(testCash, 153.4);

            // Check that there is the correct number of each.
            AssertProductAmountIsEqual(result, currencyMock1, 1);
            AssertProductAmountIsEqual(result, currencyMock2, 2);
            AssertProductAmountIsEqual(result, currencyMock3, 4);

            // double check that the summed value is greater than the price
            Assert.That(result.Sum(x => x.Item2 * sutMarket.ProductPrices.GetProductValue(x.Item1)), Is.GreaterThan(153.4));
            Assert.That(result.Sum(x => x.Item2 * sutMarket.ProductPrices.GetProductValue(x.Item1)), Is.EqualTo(154));
        }

        [Test]
        public void RoundUpOnSmallestAvailableCoinOnly()
        {
            // Change Cash to ensure quarters are our rounding option
            testCash.SetProductAmount(currencyMock3.Object, 3);

            // Get change
            var result = sutMarket.ChangeForPrice(testCash, 153.4);

            // Check that there is the correct number of each.
            AssertProductAmountIsEqual(result, currencyMock1, 1);
            AssertProductAmountIsEqual(result, currencyMock2, 3);
            AssertProductAmountIsEqual(result, currencyMock3, 3);

            // double check that the summed value is greater than the price
            Assert.That(result.Sum(x => x.Item2 * sutMarket.ProductPrices.GetProductValue(x.Item1)), Is.GreaterThan(153.4));
            Assert.That(result.Sum(x => x.Item2 * sutMarket.ProductPrices.GetProductValue(x.Item1)), Is.EqualTo(178));
        }

        [Test]
        public void SkipCoinsWhichWeDoNotHaveInAvailableCash()
        {
            // change cash to have a missing set of coins
            testCash.SetProductAmount(currencyMock1.Object, 0);

            // get change
            var result = sutMarket.ChangeForPrice(testCash, 153);

            // Ensure change is correct.
            AssertProductAmountIsEqual(result, currencyMock1, 0);
            AssertProductAmountIsEqual(result, currencyMock2, 6);
            AssertProductAmountIsEqual(result, currencyMock3, 3);
        }

        #endregion ChangeForPrice
    }
}
