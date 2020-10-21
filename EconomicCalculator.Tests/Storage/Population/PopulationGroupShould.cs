using EconomicCalculator.Storage;
using EconomicCalculator.Storage.Jobs;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace EconomicCalculator.Tests.Storage
{
    [TestFixture]
    class PopulationGroupShould
    {
        private PopulationGroup sut;

        private readonly Guid TestId = Guid.NewGuid();
        private const string TestName = "TestName";
        private const double PopCount = 1;
        private Mock<IProduct> LaborMock;
        private readonly Guid LaborId = Guid.NewGuid();
        private const int Priority1 = 1;
        private const int Priority2 = 2;
        private const string SkillName = "TestName";
        private const int SkillLevel = 0;
        private IProductAmountCollection LifeNeedsMock;
        private IProductAmountCollection DailyNeedsMock;
        private IProductAmountCollection LuxuryNeedsMock;
        private IProductAmountCollection StorageMock;
        private IProductAmountCollection LifeSat;
        private IProductAmountCollection DailySat;
        private IProductAmountCollection LuxurySat;

        private Mock<IMarket> MarketMock;

        private Mock<IJob> JobMock;
        private IProductAmountCollection JobInputs;
        private Mock<IProduct> JobInput;
        private readonly Guid JobInputId = Guid.NewGuid();
        private IProductAmountCollection JobOutputs;
        private Mock<IProduct> JobOutput;
        private readonly Guid JobOutputId = Guid.NewGuid();
        private IProductAmountCollection JobCapitals;
        private Mock<IProduct> JobCapital;
        private readonly Guid JobCapitalId = Guid.NewGuid();
        private IProductAmountCollection ProducedGoods;
        private Mock<IProduct> ProducedGood;
        private readonly Guid ProducedGoodId = Guid.NewGuid();

        private IProductAmountCollection currencyValues;
        private Mock<IProduct> CurrencyMock1;
        private readonly Guid Currency1Id = Guid.NewGuid();
        private Mock<IProduct> CurrencyMock2;
        private readonly Guid Currency2Id = Guid.NewGuid();

        #region Needs
        private Mock<IProduct> LifeNeed;
        private readonly Guid LifeNeedId = Guid.NewGuid();
        private Mock<IProduct> DailyNeed;
        private readonly Guid DailyNeedId = Guid.NewGuid();
        private Mock<IProduct> LuxNeed;
        private readonly Guid LuxNeedId = Guid.NewGuid();
        #endregion Needs

        [SetUp]
        public void Setup()
        {
            #region Needs

            LifeNeed = new Mock<IProduct>();
            LifeNeed.Setup(x => x.Id).Returns(LifeNeedId);
            LifeNeed.Setup(x => x.Name).Returns("LifeNeed");
            DailyNeed = new Mock<IProduct>();
            DailyNeed.Setup(x => x.Id).Returns(DailyNeedId);
            DailyNeed.Setup(x => x.Name).Returns("DailyNeed");
            LuxNeed = new Mock<IProduct>();
            LuxNeed.Setup(x => x.Id).Returns(LuxNeedId);
            LuxNeed.Setup(x => x.Name).Returns("LuxNeed");

            LifeNeedsMock = new ProductAmountCollection();
            LifeNeedsMock.AddProducts(LifeNeed.Object, 1);
            DailyNeedsMock = new ProductAmountCollection();
            DailyNeedsMock.AddProducts(DailyNeed.Object, 1);
            LuxuryNeedsMock = new ProductAmountCollection();
            LuxuryNeedsMock.AddProducts(LuxNeed.Object, 1);

            #endregion Needs

            #region JobGoods

            JobInput = new Mock<IProduct>();
            JobInput.Setup(x => x.Id).Returns(JobInputId);
            JobInput.Setup(x => x.Name).Returns("JobInput");
            JobOutput = new Mock<IProduct>();
            JobOutput.Setup(x => x.Id).Returns(JobOutputId);
            JobOutput.Setup(x => x.Name).Returns("JobOutput");
            JobCapital = new Mock<IProduct>();
            JobCapital.Setup(x => x.Id).Returns(JobCapitalId);
            JobCapital.Setup(x => x.Name).Returns("JobCapital");
            ProducedGood = new Mock<IProduct>();
            ProducedGood.Setup(x => x.Id).Returns(ProducedGoodId);

            JobInputs = new ProductAmountCollection();
            JobInputs.AddProducts(JobInput.Object, 1);
            JobOutputs = new ProductAmountCollection();
            JobOutputs.AddProducts(JobOutput.Object, 1);
            JobCapitals = new ProductAmountCollection();
            JobCapitals.AddProducts(JobCapital.Object, 1);
            ProducedGoods = new ProductAmountCollection();
            ProducedGoods.AddProducts(ProducedGood.Object, 1);

            #endregion JobGoods

            #region JobSetup
            JobMock = new Mock<IJob>();
            JobMock.Setup(x => x.Inputs)
                .Returns(JobInputs);
            JobMock.Setup(x => x.Outputs)
                .Returns(JobOutputs);
            JobMock.Setup(x => x.Capital)
                .Returns(JobCapitals);
            JobMock.Setup(x => x.LaborRequirements)
                .Returns(1);
            JobMock.Setup(x => x.DailyInputNeedsForPops(It.IsAny<double>()))
                .Returns((double value) => JobInputs.Multiply(value));
            JobMock.Setup(x => x.CapitalNeedsForPops(It.IsAny<double>()))
                .Returns((double value) => JobCapitals.Multiply(value));
            #endregion JobSetup

            LaborMock = new Mock<IProduct>();
            LaborMock.Setup(x => x.Id).Returns(LaborId);

            currencyValues = new ProductAmountCollection();

            CurrencyMock1 = new Mock<IProduct>();
            CurrencyMock1.Setup(x => x.Id).Returns(Currency1Id);
            CurrencyMock1.Setup(x => x.Name).Returns("Currency1");
            CurrencyMock2 = new Mock<IProduct>();
            CurrencyMock2.Setup(x => x.Id).Returns(Currency2Id);
            CurrencyMock2.Setup(x => x.Name).Returns("Currency2");

            MarketMock = new Mock<IMarket>();
            MarketMock.Setup(x => x.GetPrice(LifeNeed.Object))
                .Returns(100);
            MarketMock.Setup(x => x.GetPrice(DailyNeed.Object))
                .Returns(100);
            MarketMock.Setup(x => x.GetPrice(LuxNeed.Object))
                .Returns(100);
            MarketMock.Setup(x => x.GetPrice(JobInput.Object))
                .Returns(100);
            MarketMock.Setup(x => x.GetPrice(JobOutput.Object))
                .Returns(100);
            MarketMock.Setup(x => x.GetPrice(JobCapital.Object))
                .Returns(100);
            MarketMock.Setup(x => x.GetPrice(ProducedGood.Object))
                .Returns(100);
            MarketMock.Setup(x => x.GetPrice(CurrencyMock1.Object))
                .Returns(100);
            MarketMock.Setup(x => x.GetPrice(CurrencyMock2.Object))
                .Returns(100);

            MarketMock.Setup(x => x.AcceptedCurrencies)
                .Returns(new List<IProduct> { CurrencyMock1.Object, CurrencyMock2.Object });

            sut = new PopulationGroup
            {
                Id = TestId,
                Name = TestName,
                Count = PopCount,
                JobLabor = LaborMock.Object,
                DailyNeeds = DailyNeedsMock,
                LifeNeeds = LifeNeedsMock,
                LuxuryNeeds = LuxuryNeedsMock,
                PrimaryJob = JobMock.Object,
                Priority = Priority1,
                Storage = new ProductAmountCollection(),
                ForSale = new ProductAmountCollection(),
                SecondaryJobs = new List<IJob>(),
                SkillLevel = SkillLevel,
                SkillName = SkillName,
            };

            sut.InitializeStorage();

        }

        // A helper function for avsserting products in the collection are correct.
        private void AssertProductAmountIsEqual(IProductAmountCollection collection, Mock<IProduct> product, double value)
        {
            Assert.That(collection.GetProductValue(product.Object), Is.EqualTo(value));
        }

        #region Success

        [Test]
        [TestCase(1, 1, 1)]
        public void ReturnSumOfSatisfactionMinus1(double lifeSat, double dailySat, double luxSat)
        {
            sut.LifeSatisfaction.AddProducts(LifeNeed.Object, lifeSat);
            sut.DailySatisfaction.AddProducts(DailyNeed.Object, dailySat);
            sut.LuxurySatisfaction.AddProducts(LuxNeed.Object, luxSat);

            var result = sut.Success();

            Assert.That(result, Is.EqualTo(lifeSat + dailySat + luxSat));
        }

        #endregion Success

        #region InitializeStorage

        // I'm lazy, this is a placeholder for InitilazeStorage().

        #endregion InitializeStorage

        #region ProductionPhase

        [Test]
        public void ReturnsEmptyProductionAmountWhenStorageDoesNotContainNeededInputsOrCapital()
        {
            var result = sut.ProductionPhase();

            Assert.That(result.Products.Count, Is.EqualTo(0));
        }

        [Test]
        public void ReturnsEmptyProductionAmountWhenRequirementSatisfactionIsZero()
        {
            sut.Storage.IncludeProduct(JobInput.Object);

            var result = sut.ProductionPhase();

            Assert.That(result.Products.Count, Is.EqualTo(0));
        }

        [Test]
        public void ReturnsFullySatisfiedCollection()
        {
            // Add all inputs and Capital to Storage
            sut.Storage.AddProducts(JobInput.Object, 2);
            sut.Storage.AddProducts(JobCapital.Object, 2);

            // Run production phase
            var result = sut.ProductionPhase();

            // Check that the result is correct
            Assert.That(result.Products.Count, Is.EqualTo(2));
            Assert.That(result.ProductDict.Count, Is.EqualTo(2));
            AssertProductAmountIsEqual(result, JobInput, -1);
            AssertProductAmountIsEqual(result, JobOutput, 1);

            // ensure that capital was not eaten.
            AssertProductAmountIsEqual(sut.Storage, JobCapital, 2);

            // check that storage has changed appropriately.
            AssertProductAmountIsEqual(sut.Storage, JobInput, 1);
            AssertProductAmountIsEqual(sut.Storage, JobOutput, 1);
        }

        [Test]
        public void ReturnPartiallySatisfiedCollection()
        {
            sut.Count = 100;

            // Add all inputs and Capital to Storage
            sut.Storage.AddProducts(JobInput.Object, 50);
            sut.Storage.AddProducts(JobCapital.Object, 50);

            // Run production phase
            var result = sut.ProductionPhase();

            // Check that the result is correct
            Assert.That(result.Products.Count, Is.EqualTo(3));
            Assert.That(result.ProductDict.Count, Is.EqualTo(3));
            AssertProductAmountIsEqual(result, JobInput, -50);
            AssertProductAmountIsEqual(result, JobOutput, 50);
            AssertProductAmountIsEqual(result, LaborMock, 50);

            // ensure that capital was not eaten.
            AssertProductAmountIsEqual(sut.Storage, JobCapital, 50);

            // check that storage has changed appropriately.
            AssertProductAmountIsEqual(sut.Storage, JobInput, 0);
            AssertProductAmountIsEqual(sut.Storage, JobOutput, 50);
            AssertProductAmountIsEqual(sut.Storage, LaborMock, 50);
        }

        #endregion ProductionPhase

        #region ConsumptionPhase

        [Test]
        public void ConsumeGoodsFromStorage()
        {
            // Add needs to storage
            sut.Storage.AddProducts(LifeNeed.Object, 1);
            sut.Storage.AddProducts(DailyNeed.Object, 1);
            sut.Storage.AddProducts(LuxNeed.Object, 1);

            // run consumption phase
            var result = sut.ConsumptionPhase();

            // assuming nothing broke
            // Ensure consumption occured correctly
            AssertProductAmountIsEqual(sut.Storage, LifeNeed, 0);
            AssertProductAmountIsEqual(sut.Storage, DailyNeed, 0);
            AssertProductAmountIsEqual(sut.Storage, LuxNeed, 0);

            // Ensure the return is correct
            AssertProductAmountIsEqual(result, LifeNeed, -1);
            AssertProductAmountIsEqual(result, DailyNeed, -1);
            AssertProductAmountIsEqual(result, LuxNeed, -1);

            // Ensure Satisfaction is recorded properly.
            AssertProductAmountIsEqual(sut.LifeSatisfaction, LifeNeed, 1);
            AssertProductAmountIsEqual(sut.DailySatisfaction, DailyNeed, 1);
            AssertProductAmountIsEqual(sut.LuxurySatisfaction, LuxNeed, 1);
        }

        [Test]
        public void ConsumeGoodsFromStorageWhenNotFullyStocked()
        {
            // set to 100 pops.
            sut.Count = 100;

            // Add needs to storage
            sut.Storage.AddProducts(LifeNeed.Object, 50);
            sut.Storage.AddProducts(DailyNeed.Object, 50);
            sut.Storage.AddProducts(LuxNeed.Object, 50);

            // run consumption phase
            var result = sut.ConsumptionPhase();

            // assuming nothing broke
            // Ensure consumption occured correctly
            AssertProductAmountIsEqual(sut.Storage, LifeNeed, 0);
            AssertProductAmountIsEqual(sut.Storage, DailyNeed, 0);
            AssertProductAmountIsEqual(sut.Storage, LuxNeed, 0);

            // Ensure the return is correct
            AssertProductAmountIsEqual(result, LifeNeed, -50);
            AssertProductAmountIsEqual(result, DailyNeed, -50);
            AssertProductAmountIsEqual(result, LuxNeed, -50);

            // Ensure Satisfaction is recorded properly.
            AssertProductAmountIsEqual(sut.LifeSatisfaction, LifeNeed, 0.5);
            AssertProductAmountIsEqual(sut.DailySatisfaction, DailyNeed, 0.5);
            AssertProductAmountIsEqual(sut.LuxurySatisfaction, LuxNeed, 0.5);
        }

        [Test]
        public void ConsumeGoodsFromStorageWhenOverStocked()
        {
            // set to 100 pops.
            sut.Count = 100;

            // Add needs to storage
            sut.Storage.AddProducts(LifeNeed.Object, 150);
            sut.Storage.AddProducts(DailyNeed.Object, 150);
            sut.Storage.AddProducts(LuxNeed.Object, 150);

            // run consumption phase
            var result = sut.ConsumptionPhase();

            // assuming nothing broke
            // Ensure consumption occured correctly
            AssertProductAmountIsEqual(sut.Storage, LifeNeed, 50);
            AssertProductAmountIsEqual(sut.Storage, DailyNeed, 50);
            AssertProductAmountIsEqual(sut.Storage, LuxNeed, 50);

            // Ensure the return is correct
            AssertProductAmountIsEqual(result, LifeNeed, -100);
            AssertProductAmountIsEqual(result, DailyNeed, -100);
            AssertProductAmountIsEqual(result, LuxNeed, -100);

            // Ensure Satisfaction is recorded properly.
            AssertProductAmountIsEqual(sut.LifeSatisfaction, LifeNeed, 1);
            AssertProductAmountIsEqual(sut.DailySatisfaction, DailyNeed, 1);
            AssertProductAmountIsEqual(sut.LuxurySatisfaction, LuxNeed, 1);
        }

        #endregion ConsumptionPhase

        #region LossPhase

        [Test]
        public void RemoveAppropriateGoodsFromStorage()
        {
            // Add Failed Products to Need Mocks
            LifeNeed.Setup(x => x.FailedProducts(100)).Returns(50);
            DailyNeed.Setup(x => x.FailedProducts(100)).Returns(25);
            LuxNeed.Setup(x => x.FailedProducts(100)).Returns(10);
            JobCapital.Setup(x => x.FailedProducts(100)).Returns(0);

            // Add goods to storage.
            sut.Storage.AddProducts(LifeNeed.Object, 100);
            sut.Storage.AddProducts(DailyNeed.Object, 100);
            sut.Storage.AddProducts(LuxNeed.Object, 100);
            sut.Storage.AddProducts(JobCapital.Object, 100);

            // Run loss phase
            var result = sut.LossPhase();

            // Check that the correct good amounts were lost.
            AssertProductAmountIsEqual(result, LifeNeed, -50);
            AssertProductAmountIsEqual(result, DailyNeed, -25);
            AssertProductAmountIsEqual(result, LuxNeed, -10);
            // Ensure that good which didn't decay, doesn't get added.
            Assert.That(result.Contains(JobCapital.Object), Is.False);

            // check that the expected goods were lost in storage.
            AssertProductAmountIsEqual(sut.Storage, LifeNeed, 50);
            AssertProductAmountIsEqual(sut.Storage, DailyNeed, 75);
            AssertProductAmountIsEqual(sut.Storage, LuxNeed, 90);
            AssertProductAmountIsEqual(sut.Storage, JobCapital, 100);
        }

        #endregion LossPhase

        #region UpForSale

        [Test]
        public void ReturnListOfGoodsAvailableForSale()
        {
            // Adjust pops for easier testing.
            sut.Count = 50;

            // Add goods to storage
            sut.Storage.AddProducts(LifeNeed.Object, 100);
            sut.Storage.AddProducts(DailyNeed.Object, 100);
            sut.Storage.AddProducts(LuxNeed.Object, 100);
            sut.Storage.AddProducts(JobCapital.Object, 100);
            sut.Storage.AddProducts(JobInput.Object, 100);
            sut.Storage.AddProducts(JobOutput.Object, 100);

            // Get back what can be sold.
            var result = sut.UpForSale();

            // check it's all there for sale.
            AssertProductAmountIsEqual(result, LifeNeed, 50);
            AssertProductAmountIsEqual(result, DailyNeed, 50);
            AssertProductAmountIsEqual(result, LuxNeed, 50);
            AssertProductAmountIsEqual(result, JobCapital, 50);
            AssertProductAmountIsEqual(result, JobInput, 50);
            AssertProductAmountIsEqual(result, JobOutput, 100);

            // check the goods haven't been removed. We only remove from storage upon loss or purchase.
            AssertProductAmountIsEqual(sut.Storage, LifeNeed, 100);
            AssertProductAmountIsEqual(sut.Storage, DailyNeed, 100);
            AssertProductAmountIsEqual(sut.Storage, LuxNeed, 100);
            AssertProductAmountIsEqual(sut.Storage, JobCapital, 100);
            AssertProductAmountIsEqual(sut.Storage, JobInput, 100);
            AssertProductAmountIsEqual(sut.Storage, JobOutput, 100);
        }

        #endregion UpForSale

        #region TotalNeeds

        [Test]
        public void GetAllNeedsFromTotalNeeds()
        {
            // Get the total needs
            var result = sut.TotalNeeds;

            // Ensure they are all there.
            AssertProductAmountIsEqual(result, LifeNeed, 1);
            AssertProductAmountIsEqual(result, DailyNeed, 1);
            AssertProductAmountIsEqual(result, LuxNeed, 1);
            AssertProductAmountIsEqual(result, JobInput, 1);
            AssertProductAmountIsEqual(result, JobCapital, 1);

            // ENsure they are in priority order
            Assert.That(result.Products[0].Id, Is.EqualTo(LifeNeed.Object.Id));
            Assert.That(result.Products[1].Id, Is.EqualTo(JobCapital.Object.Id));
            Assert.That(result.Products[2].Id, Is.EqualTo(JobInput.Object.Id));
            Assert.That(result.Products[3].Id, Is.EqualTo(DailyNeed.Object.Id));
            Assert.That(result.Products[4].Id, Is.EqualTo(LuxNeed.Object.Id));
        }

        #endregion TotalNeeds

        #region GetCash

        [Test]
        public void ReturnCashFromGetCash()
        {
            // setup ForSale
            sut.ForSale.AddProducts(LuxNeed.Object, 1);
            sut.ForSale.AddProducts(DailyNeed.Object, 1);
            sut.ForSale.AddProducts(CurrencyMock1.Object, 1);
            sut.ForSale.AddProducts(CurrencyMock2.Object, 1);

            // get list of currencies we want back.
            var currenices = new List<IProduct>
            {
                CurrencyMock1.Object,
                CurrencyMock2.Object
            };

            // Get our Cash
            var cash = sut.GetCash(currenices)
;

            // check our desired cash is in there.
            AssertProductAmountIsEqual(cash, CurrencyMock1, 1);
            AssertProductAmountIsEqual(cash, CurrencyMock2, 1);

            // Ensure the others aren't there.
            Assert.That(cash.Contains(LuxNeed.Object), Is.False);
            Assert.That(cash.Contains(DailyNeed.Object), Is.False);
        }

        #endregion GetCash

        #region GetPrice

        [Test]
        public void ReturnPriceFromGetPrice()
        {
            var price = 100;
            // TODO test this more later, it doesn't do anything right now anyway.
            var result = sut.GetPrice(LifeNeed.Object, price);

            Assert.That(result, Is.EqualTo(price));
        }

        #endregion GetPrice

        #region BuyGood

        [Test]
        public void ThrowArgumentNullFromBuyGoodWhenCashIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => sut.BuyGood(null, LifeNeed.Object, 100, MarketMock.Object));
        }

        [Test]
        public void ThrowArgumentNullFromBuyGoodWhenGoodIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => sut.BuyGood(JobInputs, null, 100, MarketMock.Object));
        }

        [Test]
        public void ThrowArgumentNullFromBuyGoodWhenMarketIsNull()
        {
            Assert.Throws<ArgumentNullException>(
                () => sut.BuyGood(JobInputs, LifeNeed.Object, 100, null));
        }

        [Test]
        [TestCase(0)]
        [TestCase(-1)]
        public void ThrowArgumentNullFromBuyGoodWhenAmountIsLessThanOrEqualTo0(double val)
        {
            Assert.Throws<ArgumentOutOfRangeException>(
                () => sut.BuyGood(JobInputs, LifeNeed.Object, val, MarketMock.Object));
        }

        [Test]
        public void ReturnSuccessfulPurchaseFromBuyGood()
        {
            // set up currency.
            MarketMock.Setup(x => x.GetPrice(CurrencyMock1.Object, It.IsAny<double>()))
                .Returns((IProduct prod, double x) => x * 1);
            MarketMock.Setup(x => x.GetPrice(CurrencyMock2.Object, It.IsAny<double>()))
                .Returns((IProduct prod, double x) => x * 100);

            // setup immediate cash.
            var cash = new ProductAmountCollection();
            cash.AddProducts(CurrencyMock1.Object, 10000);
            cash.AddProducts(CurrencyMock2.Object, 10000);

            // Setup Storage and ForSale
            sut.Storage.AddProducts(LifeNeed.Object, 100);
            sut.Storage.AddProducts(CurrencyMock1.Object, 10000);
            sut.Storage.AddProducts(CurrencyMock2.Object, 10000);
            sut.ForSale.AddProducts(LifeNeed.Object, 100);
            sut.ForSale.AddProducts(CurrencyMock1.Object, 10000);
            sut.ForSale.AddProducts(CurrencyMock2.Object, 10000);

            // setup Product
            MarketMock.Setup(x => x.GetPrice(LifeNeed.Object))
                .Returns(123);

            // setup change return
            var change = new ProductAmountCollection();
            change.AddProducts(CurrencyMock2.Object, 1);
            change.AddProducts(CurrencyMock1.Object, 23);
            MarketMock.Setup(x => x.ChangeForPrice(cash, 123))
                .Returns(change);

            // test it out
            var result = sut.BuyGood(cash, LifeNeed.Object, 1, MarketMock.Object);

            // Ensure that the seller has appropriately changed his goods.
            AssertProductAmountIsEqual(sut.Storage, LifeNeed, 99);
            AssertProductAmountIsEqual(sut.Storage, CurrencyMock2, 10001);
            AssertProductAmountIsEqual(sut.Storage, CurrencyMock1, 10023);

            // ensure result is correct transaction reciept
            AssertProductAmountIsEqual(result, LifeNeed, 1);
            AssertProductAmountIsEqual(result, CurrencyMock2, -1);
            AssertProductAmountIsEqual(result, CurrencyMock1, -23);
        }

        [Test]
        public void ReturnSuccessfulPurchaseFromBuyGoodWithChange()
        {
            // set up currency.
            MarketMock.Setup(x => x.GetPrice(CurrencyMock1.Object, It.IsAny<double>()))
                .Returns((IProduct prod, double x) => x * 1);
            MarketMock.Setup(x => x.GetPrice(CurrencyMock2.Object, It.IsAny<double>()))
                .Returns((IProduct prod, double x) => x * 100);

            // setup immediate cash.
            var cash = new ProductAmountCollection();
            cash.AddProducts(CurrencyMock1.Object, 0);
            cash.AddProducts(CurrencyMock2.Object, 10000);

            // Setup Storage and ForSale
            sut.Storage.AddProducts(LifeNeed.Object, 100);
            sut.Storage.AddProducts(CurrencyMock1.Object, 10000);
            sut.Storage.AddProducts(CurrencyMock2.Object, 10000);
            sut.ForSale.AddProducts(LifeNeed.Object, 100);
            sut.ForSale.AddProducts(CurrencyMock1.Object, 10000);
            sut.ForSale.AddProducts(CurrencyMock2.Object, 10000);

            // setup Product
            MarketMock.Setup(x => x.GetPrice(LifeNeed.Object))
                .Returns(123);

            // setup change return
            var change = new ProductAmountCollection();
            change.AddProducts(CurrencyMock1.Object, 0);
            change.AddProducts(CurrencyMock2.Object, 2);

            // Setup seller's change
            var sellerChange = new ProductAmountCollection();
            sellerChange.AddProducts(CurrencyMock1.Object, 77);
            sellerChange.AddProducts(CurrencyMock2.Object, 0);

            // Setup changes, general to specific
            MarketMock
                .Setup(x => x.ChangeForPrice(It.IsAny<IProductAmountCollection>(), 77))
                .Returns(sellerChange);
            MarketMock.Setup(x => x.ChangeForPrice(cash, 123)).Returns(change);

            // test it out
            var result = sut.BuyGood(cash, LifeNeed.Object, 1, MarketMock.Object);

            // Ensure that the seller has appropriately changed his goods.
            AssertProductAmountIsEqual(sut.Storage, LifeNeed, 99);
            AssertProductAmountIsEqual(sut.Storage, CurrencyMock2, 10002);
            AssertProductAmountIsEqual(sut.Storage, CurrencyMock1,  9923);

            // ensure result is correct transaction reciept
            AssertProductAmountIsEqual(result, LifeNeed, 1);
            AssertProductAmountIsEqual(result, CurrencyMock2, -2);
            AssertProductAmountIsEqual(result, CurrencyMock1, 77);
        }

        [Test]
        public void ReturnUnsuccessfulPurchaseWhenBuyerCannotAffordAnything()
        {
            // set up currency.
            MarketMock.Setup(x => x.GetPrice(CurrencyMock1.Object, It.IsAny<double>()))
                .Returns((IProduct prod, double x) => x * 1);
            MarketMock.Setup(x => x.GetPrice(CurrencyMock2.Object, It.IsAny<double>()))
                .Returns((IProduct prod, double x) => x * 100);

            // setup immediate cash.
            var cash = new ProductAmountCollection();
            cash.AddProducts(CurrencyMock1.Object, 0);
            cash.AddProducts(CurrencyMock2.Object, 0);

            // Setup Storage and ForSale
            sut.Storage.AddProducts(LifeNeed.Object, 100);
            sut.Storage.AddProducts(CurrencyMock1.Object, 10000);
            sut.Storage.AddProducts(CurrencyMock2.Object, 10000);
            sut.ForSale.AddProducts(LifeNeed.Object, 100);
            sut.ForSale.AddProducts(CurrencyMock1.Object, 10000);
            sut.ForSale.AddProducts(CurrencyMock2.Object, 10000);

            // setup Product
            MarketMock.Setup(x => x.GetPrice(LifeNeed.Object))
                .Returns(123);

            // setup change return
            var change = new ProductAmountCollection();
            change.AddProducts(CurrencyMock1.Object, 0);
            change.AddProducts(CurrencyMock2.Object, 0);

            // Setup seller's change
            var sellerChange = new ProductAmountCollection();
            sellerChange.AddProducts(CurrencyMock1.Object, 0);
            sellerChange.AddProducts(CurrencyMock2.Object, 0);

            // Setup changes, general to specific
            MarketMock
                .Setup(x => x.ChangeForPrice(It.IsAny<IProductAmountCollection>(), 77))
                .Returns(sellerChange);
            MarketMock.Setup(x => x.ChangeForPrice(cash, 123)).Returns(change);

            // test it out
            var result = sut.BuyGood(cash, LifeNeed.Object, 1, MarketMock.Object);

            // Ensure that the seller has appropriately changed his goods.
            AssertProductAmountIsEqual(sut.Storage, LifeNeed, 100);
            AssertProductAmountIsEqual(sut.Storage, CurrencyMock2, 10000);
            AssertProductAmountIsEqual(sut.Storage, CurrencyMock1, 10000);

            // ensure result is correct transaction reciept
            Assert.That(result.Contains(LifeNeed.Object), Is.False);
            Assert.That(result.Contains(CurrencyMock1.Object), Is.False);
            Assert.That(result.Contains(CurrencyMock2.Object), Is.False);
        }

        [Test]
        public void ReturnPartialSuccessWhenGoodIsFractionalAndSomeCanBeBought()
        {
            // test varablies
            var goodPrice = 100;
            var BuyAmount = 1;

            // make good fractional
            LifeNeed.Setup(x => x.Fractional).Returns(true);

            // set up currency.
            MarketMock.Setup(x => x.GetPrice(CurrencyMock1.Object, It.IsAny<double>()))
                .Returns((IProduct prod, double x) => x * 1);
            MarketMock.Setup(x => x.GetPrice(CurrencyMock2.Object, It.IsAny<double>()))
                .Returns((IProduct prod, double x) => x * 100);

            // setup immediate cash.
            var cash = new ProductAmountCollection();
            cash.AddProducts(CurrencyMock1.Object, 50);
            cash.AddProducts(CurrencyMock2.Object, 0);

            // Setup Storage and ForSale
            sut.Storage.AddProducts(LifeNeed.Object, 100);
            sut.Storage.AddProducts(CurrencyMock1.Object, 10000);
            sut.Storage.AddProducts(CurrencyMock2.Object, 10000);
            sut.ForSale.AddProducts(LifeNeed.Object, 100);
            sut.ForSale.AddProducts(CurrencyMock1.Object, 10000);
            sut.ForSale.AddProducts(CurrencyMock2.Object, 10000);

            // setup Product
            MarketMock.Setup(x => x.GetPrice(LifeNeed.Object))
                .Returns(goodPrice);

            // setup change return
            var change = new ProductAmountCollection();
            change.AddProducts(CurrencyMock1.Object, 50);
            change.AddProducts(CurrencyMock2.Object, 0);

            // Setup seller's change
            var sellerChange = new ProductAmountCollection();
            sellerChange.AddProducts(CurrencyMock1.Object, 0);
            sellerChange.AddProducts(CurrencyMock2.Object, 0);

            // Setup changes, general to specific
            MarketMock
                .Setup(x => x.ChangeForPrice(It.IsAny<IProductAmountCollection>(), 77))
                .Returns(sellerChange);
            MarketMock.Setup(x => x.ChangeForPrice(cash, goodPrice)).Returns(change);

            // test it out
            var result = sut.BuyGood(cash, LifeNeed.Object, BuyAmount, MarketMock.Object);

            // Ensure that the seller has appropriately changed his goods.
            AssertProductAmountIsEqual(sut.Storage, LifeNeed, 99.5);
            AssertProductAmountIsEqual(sut.Storage, CurrencyMock2, 10000);
            AssertProductAmountIsEqual(sut.Storage, CurrencyMock1, 10050);

            // ensure result is correct transaction reciept
            AssertProductAmountIsEqual(result, LifeNeed, 0.5);
            AssertProductAmountIsEqual(result, CurrencyMock2, 0);
            AssertProductAmountIsEqual(result, CurrencyMock1, -50);
        }

        [Test]
        public void ReturnSuccessWhenSellerIsShortOnGoods()
        {
            // test varablies
            var goodPrice = 100;
            var BuyAmount = 2;

            // set up currency.
            MarketMock.Setup(x => x.GetPrice(CurrencyMock1.Object, It.IsAny<double>()))
                .Returns((IProduct prod, double x) => x * 1);
            MarketMock.Setup(x => x.GetPrice(CurrencyMock2.Object, It.IsAny<double>()))
                .Returns((IProduct prod, double x) => x * 100);

            // setup immediate cash.
            var cash = new ProductAmountCollection();
            cash.AddProducts(CurrencyMock1.Object, 10000);
            cash.AddProducts(CurrencyMock2.Object, 10000);

            // Setup Storage and ForSale
            sut.Storage.AddProducts(LifeNeed.Object, 1);
            sut.Storage.AddProducts(CurrencyMock1.Object, 10000);
            sut.Storage.AddProducts(CurrencyMock2.Object, 10000);
            sut.ForSale.AddProducts(LifeNeed.Object, 1);
            sut.ForSale.AddProducts(CurrencyMock1.Object, 10000);
            sut.ForSale.AddProducts(CurrencyMock2.Object, 10000);

            // setup Product
            MarketMock.Setup(x => x.GetPrice(LifeNeed.Object))
                .Returns(goodPrice);

            // setup change return
            var change = new ProductAmountCollection();
            change.AddProducts(CurrencyMock1.Object, 0);
            change.AddProducts(CurrencyMock2.Object, 1);

            // Setup seller's change
            var sellerChange = new ProductAmountCollection();
            sellerChange.AddProducts(CurrencyMock1.Object, 0);
            sellerChange.AddProducts(CurrencyMock2.Object, 0);

            // Setup changes, general to specific
            MarketMock
                .Setup(x => x.ChangeForPrice(It.IsAny<IProductAmountCollection>(), 0))
                .Returns(sellerChange);
            MarketMock.Setup(x => x.ChangeForPrice(cash, goodPrice)).Returns(change);

            // test it out
            var result = sut.BuyGood(cash, LifeNeed.Object, BuyAmount, MarketMock.Object);

            // Ensure that the seller has appropriately changed his goods.
            AssertProductAmountIsEqual(sut.Storage, LifeNeed, 0);
            AssertProductAmountIsEqual(sut.Storage, CurrencyMock2, 10001);
            AssertProductAmountIsEqual(sut.Storage, CurrencyMock1, 10000);

            // ensure result is correct transaction reciept
            AssertProductAmountIsEqual(result, LifeNeed, 1);
            AssertProductAmountIsEqual(result, CurrencyMock2, -1);
            AssertProductAmountIsEqual(result, CurrencyMock1, 0);
        }

        [Test]
        public void ReturnSuccessWhenSellerIsShortOnChange()
        {
            // test varablies
            var goodPrice = 123;
            var BuyAmount = 1;
            var sellAmount = 100;
            var expectedBuy = 1;

            // Change good to produce change.
            MarketMock.Setup(x => x.GetPrice(LifeNeed.Object)).Returns(goodPrice);

            // set up currency.
            MarketMock.Setup(x => x.GetPrice(CurrencyMock1.Object, It.IsAny<double>()))
                .Returns((IProduct prod, double x) => x * 1);
            MarketMock.Setup(x => x.GetPrice(CurrencyMock2.Object, It.IsAny<double>()))
                .Returns((IProduct prod, double x) => x * 100);

            // setup immediate cash.
            var cash = new ProductAmountCollection();
            cash.AddProducts(CurrencyMock1.Object, 0);
            cash.AddProducts(CurrencyMock2.Object, 10000);

            // Setup Storage and ForSale
            sut.Storage.AddProducts(LifeNeed.Object, sellAmount);
            sut.Storage.AddProducts(CurrencyMock1.Object, 50);
            sut.Storage.AddProducts(CurrencyMock2.Object, 0);
            sut.ForSale.AddProducts(LifeNeed.Object, sellAmount);
            sut.ForSale.AddProducts(CurrencyMock1.Object, 50);
            sut.ForSale.AddProducts(CurrencyMock2.Object, 0);

            // setup Product
            MarketMock.Setup(x => x.GetPrice(LifeNeed.Object))
                .Returns(goodPrice);

            // setup change return
            var change = new ProductAmountCollection();
            change.AddProducts(CurrencyMock1.Object, 0);
            change.AddProducts(CurrencyMock2.Object, 2);

            // Setup seller's change
            var sellerChange = new ProductAmountCollection();
            sellerChange.AddProducts(CurrencyMock1.Object, 50);
            sellerChange.AddProducts(CurrencyMock2.Object, 0);

            // Setup changes, general to specific
            MarketMock
                .Setup(x => x.ChangeForPrice(It.IsAny<IProductAmountCollection>(), 77))
                .Returns(sellerChange);
            MarketMock.Setup(x => x.ChangeForPrice(cash, goodPrice)).Returns(change);

            // test it out
            var result = sut.BuyGood(cash, LifeNeed.Object, BuyAmount, MarketMock.Object);

            // Ensure that the seller has appropriately changed his goods.
            AssertProductAmountIsEqual(sut.Storage, LifeNeed, sellAmount-expectedBuy);
            AssertProductAmountIsEqual(sut.Storage, CurrencyMock2, 2);
            AssertProductAmountIsEqual(sut.Storage, CurrencyMock1, 0);

            // ensure result is correct transaction reciept
            AssertProductAmountIsEqual(result, LifeNeed, expectedBuy);
            AssertProductAmountIsEqual(result, CurrencyMock2, -2);
            AssertProductAmountIsEqual(result, CurrencyMock1, 50);
        }

        #endregion BuyGood

        #region BarterGood

        [Test]
        public void ThrowArgumentNullFromBarterGoodWhenBuyerStockIsNull()
        {
            Assert.Throws<ArgumentNullException>(
                () => sut.BarterGood(null, LifeNeed.Object, 100, MarketMock.Object));
        }

        [Test]
        public void ThrowArgumentNullFromBarterGoodWhenGoodIsNull()
        {
            Assert.Throws<ArgumentNullException>(
                () => sut.BarterGood(JobInputs, null, 100, MarketMock.Object));
        }

        [Test]
        public void ThrowArgumentNullFromBarterGoodWhenMarketIsNull()
        {
            Assert.Throws<ArgumentNullException>(
                () => sut.BarterGood(JobInputs, LifeNeed.Object, 100, null));
        }

        [Test]
        [TestCase(0)]
        [TestCase(-1)]
        public void ThrowArgumentNullFromBarterGoodWhenAmountIs0OrLess(double val)
        {
            Assert.Throws<ArgumentOutOfRangeException>(
                () => sut.BarterGood(JobInputs, LifeNeed.Object, val, MarketMock.Object));
        }

        [Test]
        public void SuccessfullyBarterGoodWithSufficentBarter()
        {
            // Setup Market Change for price.
            MarketMock.Setup(x => x.ChangeForPrice(JobInputs, 100))
                .Returns(JobInputs.Copy());

            // Add Storage and ForSale
            sut.Storage.AddProducts(LifeNeed.Object, 1);
            sut.ForSale.AddProducts(LifeNeed.Object, 1);

            // Barter 1 LifeNeed for 1 JobInput.
            var result = sut.BarterGood(JobInputs, LifeNeed.Object, 1, MarketMock.Object);

            // check transaction
            AssertProductAmountIsEqual(result, LifeNeed, 1);
            AssertProductAmountIsEqual(result, JobInput, -1);

            // Ensure no change in storage or for sale
            AssertProductAmountIsEqual(sut.Storage, LifeNeed, 1);
            AssertProductAmountIsEqual(sut.ForSale, LifeNeed, 1);

            // Ensure that receipt doesn't go above storage or ForSale
            Assert.That(sut.Storage.GetProductValue(LifeNeed.Object),
                Is.GreaterThanOrEqualTo(result.GetProductValue(LifeNeed.Object)));
            Assert.That(sut.ForSale.GetProductValue(LifeNeed.Object),
                Is.GreaterThanOrEqualTo(result.GetProductValue(LifeNeed.Object)));
        }

        [Test]
        public void SuccessfullyBarterGoodWithSufficentBarterButInsufficientStorage()
        {
            // Setup Market Change for price.
            MarketMock.Setup(x => x.ChangeForPrice(JobInputs, 100))
                .Returns(JobInputs.Copy());

            // Add Storage and ForSale
            sut.Storage.AddProducts(LifeNeed.Object, 1);
            sut.ForSale.AddProducts(LifeNeed.Object, 1);

            // Barter 1 LifeNeed for 1 JobInput.
            var result = sut.BarterGood(JobInputs, LifeNeed.Object, 2, MarketMock.Object);

            // check transaction
            AssertProductAmountIsEqual(result, LifeNeed, 1);
            AssertProductAmountIsEqual(result, JobInput, -1);

            // Ensure no change in storage or for sale
            AssertProductAmountIsEqual(sut.Storage, LifeNeed, 1);
            AssertProductAmountIsEqual(sut.ForSale, LifeNeed, 1);

            // Ensure that receipt doesn't go above storage or ForSale
            Assert.That(sut.Storage.GetProductValue(LifeNeed.Object),
                Is.GreaterThanOrEqualTo(result.GetProductValue(LifeNeed.Object)));
            Assert.That(sut.ForSale.GetProductValue(LifeNeed.Object),
                Is.GreaterThanOrEqualTo(result.GetProductValue(LifeNeed.Object)));
        }

        [Test]
        public void SuccessfullyBarterWithInsufficientBarter()
        {
            // Update Good Price
            MarketMock.Setup(x => x.GetPrice(JobInput.Object))
                .Returns(20);

            // Add Storage and ForSale
            sut.Storage.AddProducts(LifeNeed.Object, 1);
            sut.ForSale.AddProducts(LifeNeed.Object, 1);

            // Setup Market Change for price.
            MarketMock.Setup(x => x.ChangeForPrice(JobInputs, 100))
                .Returns(JobInputs.Copy());

            // Barter 1 LifeNeed for 1 JobInput.
            var result = sut.BarterGood(JobInputs, LifeNeed.Object, 1, MarketMock.Object);

            // check transaction
            Assert.That(result.Contains(JobInput.Object), Is.False);
            Assert.That(result.Contains(LifeNeed.Object), Is.False);

            // Ensure no change in storage or for sale
            AssertProductAmountIsEqual(sut.Storage, LifeNeed, 1);
            AssertProductAmountIsEqual(sut.ForSale, LifeNeed, 1);
        }

        [Test]
        public void SucessfullyBarterWithInsufficientBarterButEnoughToGetSomething()
        {
            MarketMock.Setup(x => x.GetPrice(JobInput.Object))
                .Returns(20);

            JobInputs.SetProductAmount(JobInput.Object, 6);

            var changeRet = new ProductAmountCollection();
            changeRet.SetProductAmount(JobInput.Object, 5);

            // Setup Market Change for price.
            MarketMock.Setup(x => x.ChangeForPrice(JobInputs, 100))
                .Returns(changeRet);

            // Add Storage and ForSale
            sut.Storage.AddProducts(LifeNeed.Object, 1);
            sut.ForSale.AddProducts(LifeNeed.Object, 1);

            // Barter 1 LifeNeed for 1 JobInput.
            var result = sut.BarterGood(JobInputs, LifeNeed.Object, 2, MarketMock.Object);

            // check transaction
            AssertProductAmountIsEqual(result, LifeNeed, 1);
            AssertProductAmountIsEqual(result, JobInput, -5);

            // Ensure no change in storage or for sale
            AssertProductAmountIsEqual(sut.Storage, LifeNeed, 1);
            AssertProductAmountIsEqual(sut.ForSale, LifeNeed, 1);

            // Ensure that receipt doesn't go above storage or ForSale
            Assert.That(sut.Storage.GetProductValue(LifeNeed.Object),
                Is.GreaterThanOrEqualTo(result.GetProductValue(LifeNeed.Object)));
            Assert.That(sut.ForSale.GetProductValue(LifeNeed.Object),
                Is.GreaterThanOrEqualTo(result.GetProductValue(LifeNeed.Object)));
        }

        [Test]
        public void SucessfullyBarterForFractionalGood()
        {
            MarketMock.Setup(x => x.GetPrice(JobInput.Object))
                .Returns(20);

            LifeNeed.Setup(x => x.Fractional)
                .Returns(true);

            JobInputs.SetProductAmount(JobInput.Object, 6);

            var changeRet = new ProductAmountCollection();
            changeRet.SetProductAmount(JobInput.Object, 5);

            // Add Storage and ForSale
            sut.Storage.AddProducts(LifeNeed.Object, 2);
            sut.ForSale.AddProducts(LifeNeed.Object, 2);

            // Setup Market Change for price.
            MarketMock.Setup(x => x.ChangeForPrice(JobInputs, 100))
                .Returns(changeRet);

            // Barter 1 LifeNeed for 1 JobInput.
            var result = sut.BarterGood(JobInputs, LifeNeed.Object, 2, MarketMock.Object);

            // check transaction
            AssertProductAmountIsEqual(result, LifeNeed, 1.2);
            AssertProductAmountIsEqual(result, JobInput, -6);

            // Ensure no change in storage or for sale
            AssertProductAmountIsEqual(sut.Storage, LifeNeed, 2);
            AssertProductAmountIsEqual(sut.ForSale, LifeNeed, 2);

            // Ensure that receipt doesn't go above storage or ForSale
            Assert.That(sut.Storage.GetProductValue(LifeNeed.Object),
                Is.GreaterThanOrEqualTo(result.GetProductValue(LifeNeed.Object)));
            Assert.That(sut.ForSale.GetProductValue(LifeNeed.Object),
                Is.GreaterThanOrEqualTo(result.GetProductValue(LifeNeed.Object)));
        }

        #endregion BarterGood

        #region CompleteTransaction

        [Test]
        public void ThrowArgumentNullFromCompleteTransaction()
        {
            Assert.Throws<ArgumentNullException>(() => sut.CompleteTransaction(null));
        }

        [Test]
        public void CompleteATransactionsCorrectly()
        {
            // create transaction
            var transaction = new ProductAmountCollection();
            transaction.AddProducts(LifeNeed.Object, 1);
            transaction.AddProducts(DailyNeed.Object, 2);
            transaction.AddProducts(LuxNeed.Object, 3);
            transaction.AddProducts(CurrencyMock1.Object, 4);
            transaction.AddProducts(CurrencyMock2.Object, 5);

            // complete transaction
            sut.CompleteTransaction(transaction);

            // check it's all there in Storage
            AssertProductAmountIsEqual(sut.Storage, LifeNeed, 1);
            AssertProductAmountIsEqual(sut.Storage, DailyNeed, 2);
            AssertProductAmountIsEqual(sut.Storage, LuxNeed, 3);
            AssertProductAmountIsEqual(sut.Storage, CurrencyMock1, 4);
            AssertProductAmountIsEqual(sut.Storage, CurrencyMock2, 5);

            // Check it's in For Sale
            AssertProductAmountIsEqual(sut.ForSale, LifeNeed, 1);
            AssertProductAmountIsEqual(sut.ForSale, DailyNeed, 2);
            AssertProductAmountIsEqual(sut.ForSale, LuxNeed, 3);
            AssertProductAmountIsEqual(sut.ForSale, CurrencyMock1, 4);
            AssertProductAmountIsEqual(sut.ForSale, CurrencyMock2, 5);

            // Add it again to double check
            sut.CompleteTransaction(transaction);

            // check it's all there in Storage
            AssertProductAmountIsEqual(sut.Storage, LifeNeed, 2);
            AssertProductAmountIsEqual(sut.Storage, DailyNeed, 4);
            AssertProductAmountIsEqual(sut.Storage, LuxNeed, 6);
            AssertProductAmountIsEqual(sut.Storage, CurrencyMock1, 8);
            AssertProductAmountIsEqual(sut.Storage, CurrencyMock2, 10);

            // Check it's in For Sale
            AssertProductAmountIsEqual(sut.ForSale, LifeNeed, 2);
            AssertProductAmountIsEqual(sut.ForSale, DailyNeed, 4);
            AssertProductAmountIsEqual(sut.ForSale, LuxNeed, 6);
            AssertProductAmountIsEqual(sut.ForSale, CurrencyMock1, 8);
            AssertProductAmountIsEqual(sut.ForSale, CurrencyMock2, 10);

            // check again, but subtract this time.
            transaction.SetProductAmount(LifeNeed.Object, -2);

            sut.CompleteTransaction(transaction);

            // check it's all there in Storage
            AssertProductAmountIsEqual(sut.Storage, LifeNeed, 0);
            AssertProductAmountIsEqual(sut.Storage, DailyNeed, 6);
            AssertProductAmountIsEqual(sut.Storage, LuxNeed, 9);
            AssertProductAmountIsEqual(sut.Storage, CurrencyMock1, 12);
            AssertProductAmountIsEqual(sut.Storage, CurrencyMock2, 15);

            // Check it's in For Sale
            AssertProductAmountIsEqual(sut.ForSale, DailyNeed, 6);
            AssertProductAmountIsEqual(sut.ForSale, LuxNeed, 9);
            AssertProductAmountIsEqual(sut.ForSale, CurrencyMock1, 12);
            AssertProductAmountIsEqual(sut.ForSale, CurrencyMock2, 15);

            // ensure that Subtracted product is deleted
            Assert.That(sut.ForSale.Contains(LifeNeed.Object), Is.False);
        }

        #endregion CompleteTransaction
    }
}
