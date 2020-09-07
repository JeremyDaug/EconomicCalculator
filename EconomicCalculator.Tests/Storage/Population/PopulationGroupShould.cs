﻿using EconomicCalculator.Storage;
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
            DailyNeed = new Mock<IProduct>();
            DailyNeed.Setup(x => x.Id).Returns(DailyNeedId);
            LuxNeed = new Mock<IProduct>();
            LuxNeed.Setup(x => x.Id).Returns(LuxNeedId);

            LifeNeedsMock = new ProductAmountCollection();
            LifeNeedsMock.AddProducts(LifeNeed.Object, 1);
            DailyNeedsMock = new ProductAmountCollection();
            DailyNeedsMock.AddProducts(DailyNeed.Object, 1);
            LuxuryNeedsMock = new ProductAmountCollection();
            LuxuryNeedsMock.AddProducts(LuxNeed.Object, 1);

            #endregion Needs

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

            LaborMock = new Mock<IProduct>();
            LaborMock.Setup(x => x.Id).Returns(LaborId);

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
                SecondaryJobs = new List<IJob>(),
                SkillLevel = SkillLevel,
                SkillName = SkillName,
            };

            sut.InitializeStorage();

        }

        // A helper function for avsserting products in the collection are correct.
        private void AssertProductAmountIsEqual(IProductAmountCollection collection, Mock<IProduct> product, double value)
        {
            Assert.That(collection.GetProductAmount(product.Object), Is.EqualTo(value));
        }

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
    }
}
