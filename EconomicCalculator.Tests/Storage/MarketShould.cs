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

        private Mock<IPopulationGroup> popMock1;
        private Mock<IPopulationGroup> popMock2;
        private Mock<IPopulationGroup> MerchantsMock;

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
            MerchantsMock = new Mock<IPopulationGroup>();

            testPops = new Mock<IPopulations>();

            testPops.Setup(x => x.Merchants)
                .Returns(MerchantsMock.Object);

            popMock1 = new Mock<IPopulationGroup>();
            popMock2 = new Mock<IPopulationGroup>();

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
                BarterLegal = false
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

        #region BuyGoodsFromMarket

        [Test]
        public void ThrowArgumentNullFromBuyGoodsFromMarketWhenBuyerIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => sutMarket.BuyGoodsFromMarket(null, productMock1.Object, 100));
        }

        [Test]
        public void ThrowArgumentNullFromBuyGoodsFromMarketWhenGoodIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => sutMarket.BuyGoodsFromMarket(popMock1.Object, null, 100));
        }

        [Test]
        [TestCase(0)]
        [TestCase(-1)]
        public void ThrowArgumentOutOfRangeFromBuyGoodsFromMarketWhenAmountIsNotPositive(double val)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => sutMarket.BuyGoodsFromMarket(popMock1.Object, productMock1.Object, val));
        }

        [Test]
        public void ReturnTotalTransactionAfterBuyingFromMarketFully()
        {
            // preexisting conditions.
            double BuyAmount = 3;
            double MerchantSells = 1;
            double ProducersSell = 1;
            double TravellersSell = 1;

            // Setup Selling Pops
            testPops.Setup(x => x.GetPopsSellingProduct(productMock1.Object))
                .Returns(new List<IPopulationGroup> { popMock2.Object });

            // Setup Buyer
            var buyerCash = new ProductAmountCollection();
            buyerCash.AddProducts(currencyMock1.Object, 1);
            buyerCash.AddProducts(currencyMock2.Object, 1);
            buyerCash.AddProducts(currencyMock3.Object, 1);
            popMock1.Setup(x => x.GetCash(sutMarket.AcceptedCurrencies))
                .Returns(buyerCash);
            var ForSale = new ProductAmountCollection();
            popMock1.Setup(x => x.ForSale)
                .Returns(ForSale);

            // Setup Merchants
            var merchantResult = new ProductAmountCollection();
            merchantResult.AddProducts(productMock1.Object, 1);

            MerchantsMock.Setup(x => x.BuyGood(buyerCash, productMock1.Object, BuyAmount, sutMarket))
                .Returns(merchantResult);

            // Setup Sellers
            var sellerResults = new ProductAmountCollection();
            sellerResults.AddProducts(productMock1.Object, 1);
            popMock2.Setup(x => x.BuyGood(buyerCash, productMock1.Object, BuyAmount - MerchantSells, sutMarket))
                .Returns(sellerResults);

            // Setup Travelling Merchants
            var travellerGoods = new ProductAmountCollection();
            travellerGoods.AddProducts(productMock1.Object, 1);
            TravellerMock1.Setup(x => x.ForSale)
                .Returns(travellerGoods);

            var travellerResults = new ProductAmountCollection();
            travellerResults.AddProducts(productMock1.Object, 1);

            TravellerMock1.Setup(x => x.BuyGood(buyerCash, productMock1.Object, BuyAmount - MerchantSells - ProducersSell, sutMarket))
                .Returns(travellerResults);

            sutMarket.TravellingMerchants = new List<IPopulationGroup> { TravellerMock1.Object };

            // Buy from the market.
            var result = sutMarket.BuyGoodsFromMarket(popMock1.Object, productMock1.Object, BuyAmount);

            // Ensure that the result has the expected good
            AssertProductAmountIsEqual(result, productMock1, BuyAmount);

            // ensure merchants bought from
            MerchantsMock.Verify(x => x.BuyGood(buyerCash, productMock1.Object, BuyAmount, sutMarket),
                Times.Once);

            // Ensure Sellers Bought From
            popMock2.Verify(x => x.BuyGood(buyerCash, productMock1.Object, BuyAmount - MerchantSells, sutMarket),
                Times.Once);

            // Ensure Travelling Merchants bought from
            TravellerMock1.Verify(x => x.BuyGood(buyerCash, productMock1.Object, BuyAmount - MerchantSells - ProducersSell, sutMarket),
                Times.Once);
        }

        [Test]
        public void ReturnTotalTransactionAfterBuyingFromOnlyMerchants()
        {
            // preexisting conditions.
            double BuyAmount = 3;
            double MerchantSells = 1;
            double ProducersSell = 1;
            double TravellersSell = 1;

            // Setup Selling Pops
            testPops.Setup(x => x.GetPopsSellingProduct(productMock1.Object))
                .Returns(new List<IPopulationGroup>());

            // Setup Buyer
            var buyerCash = new ProductAmountCollection();
            buyerCash.AddProducts(currencyMock1.Object, 1);
            buyerCash.AddProducts(currencyMock2.Object, 1);
            buyerCash.AddProducts(currencyMock3.Object, 1);
            popMock1.Setup(x => x.GetCash(sutMarket.AcceptedCurrencies))
                .Returns(buyerCash);
            var ForSale = new ProductAmountCollection();
            popMock1.Setup(x => x.ForSale)
                .Returns(ForSale);

            // Setup Merchants
            var merchantResult = new ProductAmountCollection();
            merchantResult.AddProducts(productMock1.Object, 1);

            MerchantsMock.Setup(x => x.BuyGood(buyerCash, productMock1.Object, BuyAmount, sutMarket))
                .Returns(merchantResult);

            // Setup Sellers
            var sellerResults = new ProductAmountCollection();
            // sellerResults.AddProducts(productMock1.Object, 1);
            popMock2.Setup(x => x.BuyGood(buyerCash, productMock1.Object, BuyAmount - MerchantSells, sutMarket))
                .Returns(sellerResults);

            // Setup Travelling Merchants
            var travellerGoods = new ProductAmountCollection();
            // travellerGoods.AddProducts(productMock1.Object, 1);
            TravellerMock1.Setup(x => x.ForSale)
                .Returns(travellerGoods);

            var travellerResults = new ProductAmountCollection();
            travellerResults.AddProducts(productMock1.Object, 1);

            TravellerMock1.Setup(x => x.BuyGood(buyerCash, productMock1.Object, BuyAmount - MerchantSells - ProducersSell, sutMarket))
                .Returns(travellerResults);

            sutMarket.TravellingMerchants = new List<IPopulationGroup> { TravellerMock1.Object };

            // Buy from the market.
            var result = sutMarket.BuyGoodsFromMarket(popMock1.Object, productMock1.Object, BuyAmount);

            // Ensure that the result has the expected good
            AssertProductAmountIsEqual(result, productMock1, MerchantSells);

            // ensure merchants bought from
            MerchantsMock.Verify(x => x.BuyGood(buyerCash, productMock1.Object, BuyAmount, sutMarket),
                Times.Once);

            // Ensure Sellers Bought From
            popMock2.Verify(x => x.BuyGood(buyerCash, productMock1.Object, BuyAmount - MerchantSells, sutMarket),
                Times.Never);

            // Ensure Travelling Merchants bought from
            TravellerMock1.Verify(x => x.BuyGood(buyerCash, productMock1.Object, BuyAmount - MerchantSells - ProducersSell, sutMarket),
                Times.Never);
        }

        [Test]
        public void ReturnTotalTransactionAfterProducersSatisfyEverything()
        {
            // preexisting conditions.
            double BuyAmount = 3;
            double MerchantSells = 1;
            double ProducersSell = 2;
            double TravellersSell = 1;

            // Setup Selling Pops
            testPops.Setup(x => x.GetPopsSellingProduct(productMock1.Object))
                .Returns(new List<IPopulationGroup> { popMock2.Object });

            // Setup Buyer
            var buyerCash = new ProductAmountCollection();
            buyerCash.AddProducts(currencyMock1.Object, 1);
            buyerCash.AddProducts(currencyMock2.Object, 1);
            buyerCash.AddProducts(currencyMock3.Object, 1);
            popMock1.Setup(x => x.GetCash(sutMarket.AcceptedCurrencies))
                .Returns(buyerCash);
            var ForSale = new ProductAmountCollection();
            popMock1.Setup(x => x.ForSale)
                .Returns(ForSale);

            // Setup Merchants
            var merchantResult = new ProductAmountCollection();
            merchantResult.AddProducts(productMock1.Object, MerchantSells);

            MerchantsMock.Setup(x => x.BuyGood(buyerCash, productMock1.Object, BuyAmount, sutMarket))
                .Returns(merchantResult);

            // Setup Sellers
            var sellerResults = new ProductAmountCollection();
            sellerResults.AddProducts(productMock1.Object, ProducersSell);
            popMock2.Setup(x => x.BuyGood(buyerCash, productMock1.Object, 
                                          BuyAmount - MerchantSells, sutMarket))
                .Returns(sellerResults);

            // Setup Travelling Merchants
            var travellerGoods = new ProductAmountCollection();
            travellerGoods.AddProducts(productMock1.Object, TravellersSell);
            TravellerMock1.Setup(x => x.ForSale)
                .Returns(travellerGoods);

            var travellerResults = new ProductAmountCollection();
            travellerResults.AddProducts(productMock1.Object, TravellersSell);

            TravellerMock1.Setup(x => x.BuyGood(buyerCash, productMock1.Object, BuyAmount - MerchantSells - ProducersSell, sutMarket))
                .Returns(travellerResults);

            sutMarket.TravellingMerchants = new List<IPopulationGroup> { TravellerMock1.Object };

            // Buy from the market.
            var result = sutMarket.BuyGoodsFromMarket(popMock1.Object, productMock1.Object, BuyAmount);

            // Ensure that the result has the expected good
            AssertProductAmountIsEqual(result, productMock1, BuyAmount);

            // ensure merchants bought from
            MerchantsMock.Verify(x => x.BuyGood(buyerCash, productMock1.Object, BuyAmount, sutMarket),
                Times.Once);

            // Ensure Sellers Bought From
            popMock2.Verify(x => x.BuyGood(buyerCash, productMock1.Object, BuyAmount - MerchantSells, sutMarket),
                Times.Once);

            // Ensure Travelling Merchants bought from
            TravellerMock1.Verify(x => x.BuyGood(buyerCash, productMock1.Object, BuyAmount - MerchantSells - ProducersSell, sutMarket),
                Times.Never);
        }

        [Test]
        public void ReturnTotalTransactionAfterSkippingProducersAndBuyingFromMerchants()
        {
            // preexisting conditions.
            double BuyAmount = 3;
            double MerchantSells = 1;
            double ProducersSell = 0;
            double TravellersSell = 2;

            // Setup Selling Pops
            testPops.Setup(x => x.GetPopsSellingProduct(productMock1.Object))
                .Returns(new List<IPopulationGroup>());

            // Setup Buyer
            var buyerCash = new ProductAmountCollection();
            buyerCash.AddProducts(currencyMock1.Object, 1);
            buyerCash.AddProducts(currencyMock2.Object, 1);
            buyerCash.AddProducts(currencyMock3.Object, 1);
            popMock1.Setup(x => x.GetCash(sutMarket.AcceptedCurrencies))
                .Returns(buyerCash);
            var ForSale = new ProductAmountCollection();
            popMock1.Setup(x => x.ForSale)
                .Returns(ForSale);

            // Setup Merchants
            var merchantResult = new ProductAmountCollection();
            merchantResult.AddProducts(productMock1.Object, MerchantSells);

            MerchantsMock.Setup(x => x.BuyGood(buyerCash, productMock1.Object, BuyAmount, sutMarket))
                .Returns(merchantResult);

            // Setup Sellers
            var sellerResults = new ProductAmountCollection();
            sellerResults.AddProducts(productMock1.Object, ProducersSell);
            popMock2.Setup(x => x.BuyGood(buyerCash, productMock1.Object,
                                          BuyAmount - MerchantSells, sutMarket))
                .Returns(sellerResults);

            // Setup Travelling Merchants
            var travellerGoods = new ProductAmountCollection();
            travellerGoods.AddProducts(productMock1.Object, TravellersSell);
            TravellerMock1.Setup(x => x.ForSale)
                .Returns(travellerGoods);

            var travellerResults = new ProductAmountCollection();
            travellerResults.AddProducts(productMock1.Object, TravellersSell);

            TravellerMock1.Setup(x => x.BuyGood(buyerCash, productMock1.Object, BuyAmount - MerchantSells, sutMarket))
                .Returns(travellerResults);

            sutMarket.TravellingMerchants = new List<IPopulationGroup> { TravellerMock1.Object };

            // Buy from the market.
            var result = sutMarket.BuyGoodsFromMarket(popMock1.Object, productMock1.Object, BuyAmount);

            // Ensure that the result has the expected good
            AssertProductAmountIsEqual(result, productMock1, BuyAmount);

            // ensure merchants bought from
            MerchantsMock.Verify(x => x.BuyGood(buyerCash, productMock1.Object, BuyAmount, sutMarket),
                Times.Once);

            // Ensure Sellers Bought From
            popMock2.Verify(x => x.BuyGood(buyerCash, productMock1.Object, BuyAmount - MerchantSells, sutMarket),
                Times.Never);

            // Ensure Travelling Merchants bought from
            TravellerMock1.Verify(x => x.BuyGood(buyerCash, productMock1.Object, BuyAmount - MerchantSells - ProducersSell, sutMarket),
                Times.Once);
        }

        #endregion BuyGoodsFromMarket

        #region TravellingMerchantsSelling

        [Test]
        public void ThrowArgumentNullFromTravellingMerchantsSellingWhenGoodIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => sutMarket.TravellingMerchantsSelling(null));
        }

        #endregion TravellingMerchantsSelling

        #region BuyGoods

        [Test]
        public void ThrowsArgumentNullFromBuyGoodsWhenBuyerIsNull()
        {
            Assert.Throws<ArgumentNullException>(
                () => sutMarket.BuyGoods(null, productMock1.Object, 100, popMock2.Object));
        }

        [Test]
        public void ThrowsArgumentNullFromBuyGoodsWhenGoodIsNull()
        {
            Assert.Throws<ArgumentNullException>(
                () => sutMarket.BuyGoods(popMock1.Object, null, 100, popMock2.Object));
        }

        [Test]
        [TestCase(0)]
        [TestCase(-1)]
        public void ThrowsArgumentOutOfRangeFromBuyGoodsWhenValueIsZeroOrLess(double val)
        {
            Assert.Throws<ArgumentOutOfRangeException>(
                () => sutMarket.BuyGoods(popMock1.Object, productMock1.Object, val, popMock2.Object));
        }

        [Test]
        public void ThrowsArgumentNullFromBuyGoodsWhenSellerIsNull()
        {
            Assert.Throws<ArgumentNullException>(
                () => sutMarket.BuyGoods(popMock1.Object, productMock1.Object, 100, null));
        }

        [Test]
        public void CreateAndReturnTransactionFromBuyGoodsOnlyBuyNoBarter()
        {
            // setup good to buy
            var buyAmount = 1;
            var recievedAmount = 1;
            var goodPrice = 100;

            // setup buyer
            var buyerCash = new ProductAmountCollection();
            buyerCash.AddProducts(currencyMock1.Object, 10000);
            buyerCash.AddProducts(currencyMock2.Object, 10000);
            buyerCash.AddProducts(currencyMock3.Object, 10000);

            popMock1.Setup(x => x.GetCash(sutMarket.AcceptedCurrencies))
                .Returns(buyerCash);

            // setup seller
            var reciept = new ProductAmountCollection();
            reciept.AddProducts(productMock1.Object, recievedAmount);
            reciept.AddProducts(currencyMock1.Object, -1);

            popMock2
                .Setup(x => x.BuyGood(buyerCash, productMock1.Object, buyAmount, sutMarket))
                .Returns(reciept);

            var result = sutMarket.BuyGoods(popMock1.Object, productMock1.Object,
                buyAmount, popMock2.Object);

            // ensure transaction is correct
            AssertProductAmountIsEqual(result, productMock1, recievedAmount);
            AssertProductAmountIsEqual(result, currencyMock1, -1);

            // Ensure buying ran
            popMock2.Verify(x => x.BuyGood(buyerCash, productMock1.Object, buyAmount, sutMarket),
                Times.Once);

            // Ensure Barter Did not run
            popMock2.Verify(x => x.BarterGood(popMock1.Object.ForSale, productMock1.Object, buyAmount, sutMarket),
                Times.Never);

            // Ensure Transaction Completed on the buyer's side the correct number of times.
            popMock1.Verify(x => x.CompleteTransaction(It.IsAny<ProductAmountCollection>()),
                Times.Once);
        }

        [Test]
        public void CreateAndReturnTransactionFromBuyGoodsNoBuyOnlyBarter()
        {
            // Set Barter to legal
            sutMarket.BarterLegal = true;

            // setup good to buy
            var buyAmount = 1;
            var boughtAmount = 1;
            var barteredAmount = 1;
            var goodPrice = 100;

            // setup buyer
            var buyerCash = new ProductAmountCollection();
            buyerCash.AddProducts(currencyMock1.Object, 0);
            buyerCash.AddProducts(currencyMock2.Object, 0);
            buyerCash.AddProducts(currencyMock3.Object, 0);

            popMock1.Setup(x => x.GetCash(sutMarket.AcceptedCurrencies))
                .Returns(buyerCash);

            var buyerGoods = new ProductAmountCollection();
            buyerGoods.AddProducts(productMock2.Object, 1000);

            popMock1.Setup(x => x.ForSale)
                .Returns(buyerGoods);

            // setup seller
            var reciept = new ProductAmountCollection();
            reciept.AddProducts(productMock1.Object, boughtAmount);
            reciept.AddProducts(currencyMock1.Object, -1);

            popMock2
                .Setup(x => x.BuyGood(buyerCash, productMock1.Object, buyAmount, sutMarket))
                .Returns(reciept);

            var barterTransaction = new ProductAmountCollection();
            barterTransaction.AddProducts(productMock1.Object, barteredAmount);

            popMock2
                .Setup(x => x.BarterGood(popMock1.Object.ForSale, productMock1.Object, buyAmount, sutMarket))
                .Returns(barterTransaction);

            var result = sutMarket.BuyGoods(popMock1.Object, productMock1.Object,
                buyAmount, popMock2.Object);

            // ensure transaction is correct
            AssertProductAmountIsEqual(result, productMock1, boughtAmount);

            // Ensure buying ran
            popMock2.Verify(x => x.BuyGood(buyerCash, productMock1.Object, buyAmount, sutMarket),
                Times.Never);

            // Ensure Barter Did not run
            popMock2.Verify(x => x.BarterGood(popMock1.Object.ForSale, productMock1.Object, buyAmount, sutMarket),
                Times.Once);

            // Ensure Transaction Completed on the buyer's side the correct number of times.
            popMock1.Verify(x => x.CompleteTransaction(It.IsAny<ProductAmountCollection>()),
                Times.Once);
        }

        [Test]
        public void CreateAndReturnTransactionFromBuyGoodsBothBuyAndBarter()
        {
            // Set Barter to legal
            sutMarket.BarterLegal = true;

            // setup good to buy
            var buyAmount = 2;
            var boughtAmount = 1;
            var barteredAmount = 1;
            var goodPrice = 100;

            // setup buyer
            var buyerCash = new ProductAmountCollection();
            buyerCash.AddProducts(currencyMock1.Object, 1000);
            buyerCash.AddProducts(currencyMock2.Object, 1000);
            buyerCash.AddProducts(currencyMock3.Object, 1000);

            popMock1.Setup(x => x.GetCash(sutMarket.AcceptedCurrencies))
                .Returns(buyerCash);

            var buyerGoods = new ProductAmountCollection();
            buyerGoods.AddProducts(productMock2.Object, 1000);

            popMock1.Setup(x => x.ForSale)
                .Returns(buyerGoods);

            // setup seller
            var reciept = new ProductAmountCollection();
            reciept.AddProducts(productMock1.Object, boughtAmount);
            reciept.AddProducts(currencyMock1.Object, -1);

            popMock2
                .Setup(x => x.BuyGood(buyerCash, productMock1.Object, buyAmount, sutMarket))
                .Returns(reciept);

            var barterTransaction = new ProductAmountCollection();
            barterTransaction.AddProducts(productMock1.Object, barteredAmount);

            popMock2
                .Setup(x => x.BarterGood(popMock1.Object.ForSale, productMock1.Object, barteredAmount, sutMarket))
                .Returns(barterTransaction);

            var result = sutMarket.BuyGoods(popMock1.Object, productMock1.Object,
                buyAmount, popMock2.Object);

            // ensure transaction is correct
            AssertProductAmountIsEqual(result, productMock1, boughtAmount+barteredAmount);

            // Ensure buying ran
            popMock2.Verify(x => x.BuyGood(buyerCash, productMock1.Object, buyAmount, sutMarket),
                Times.Once);

            // Ensure Barter Did not run
            popMock2.Verify(x => x.BarterGood(popMock1.Object.ForSale, productMock1.Object, barteredAmount, sutMarket),
                Times.Once);

            // Ensure Transaction Completed on the buyer's side the correct number of times.
            popMock1.Verify(x => x.CompleteTransaction(It.IsAny<ProductAmountCollection>()),
                Times.Exactly(2));
        }

        [Test]
        public void CreateAndReturnTransactionFromBuyGoodsSkipBarterWhenIllegal()
        {
            // Set Barter to legal
            sutMarket.BarterLegal = false;

            // setup good to buy
            var buyAmount = 2;
            var boughtAmount = 1;
            var barteredAmount = 1;
            var goodPrice = 100;

            // setup buyer
            var buyerCash = new ProductAmountCollection();
            buyerCash.AddProducts(currencyMock1.Object, 1000);
            buyerCash.AddProducts(currencyMock2.Object, 1000);
            buyerCash.AddProducts(currencyMock3.Object, 1000);

            popMock1.Setup(x => x.GetCash(sutMarket.AcceptedCurrencies))
                .Returns(buyerCash);

            var buyerGoods = new ProductAmountCollection();
            buyerGoods.AddProducts(productMock2.Object, 1000);

            popMock1.Setup(x => x.ForSale)
                .Returns(buyerGoods);

            // setup seller
            var reciept = new ProductAmountCollection();
            reciept.AddProducts(productMock1.Object, boughtAmount);
            reciept.AddProducts(currencyMock1.Object, -1);

            popMock2
                .Setup(x => x.BuyGood(buyerCash, productMock1.Object, buyAmount, sutMarket))
                .Returns(reciept);

            var barterTransaction = new ProductAmountCollection();
            barterTransaction.AddProducts(productMock1.Object, barteredAmount);

            popMock2
                .Setup(x => x.BarterGood(popMock1.Object.ForSale, productMock1.Object, barteredAmount, sutMarket))
                .Returns(barterTransaction);

            var result = sutMarket.BuyGoods(popMock1.Object, productMock1.Object,
                buyAmount, popMock2.Object);

            // ensure transaction is correct
            AssertProductAmountIsEqual(result, productMock1, boughtAmount);

            // Ensure buying ran
            popMock2.Verify(x => x.BuyGood(buyerCash, productMock1.Object, buyAmount, sutMarket),
                Times.Once);

            // Ensure Barter Did not run 
            popMock2.Verify(x => x.BarterGood(popMock1.Object.ForSale, productMock1.Object, barteredAmount, sutMarket),
                Times.Never);

            // Ensure Transaction Completed on the buyer's side the correct number of times.
            popMock1.Verify(x => x.CompleteTransaction(It.IsAny<ProductAmountCollection>()),
                Times.Once);
        }

        #endregion BuyGoods

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
