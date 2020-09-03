using EconomicCalculator.Storage;
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
        private const double PopCount = 1000;
        private Mock<IProduct> LaborMock;
        private const int Priority1 = 1;
        private const int Priority2 = 2;
        private const string SkillName = "TestName";
        private const int SkillLevel = 0;
        private IProductAmountCollection LifeNeedsMock;
        private IProductAmountCollection DailyNeedsMock;
        private IProductAmountCollection LuxuryNeedsMock;
        private IProductAmountCollection StorageMock;

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

            LaborMock = new Mock<IProduct>();     

            StorageMock = new ProductAmountCollection();

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
                Storage = StorageMock
            };
        }

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

        #endregion ProductionPhase

        #region BuyPhase
        #endregion BuyPhase

        #region Consume
        #endregion Consume

        #region BuyGoods
        #endregion BuyGoods
    }
}
