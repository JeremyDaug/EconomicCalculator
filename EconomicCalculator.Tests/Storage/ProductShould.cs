using EconomicCalculator.Enums;
using EconomicCalculator.Randomizer;
using EconomicCalculator.Refactor.Storage;
using EconomicCalculator.Storage;
using EconomicCalculator.Storage.Products;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace EconomicCalculator.Tests.Intermediaries
{
    [TestFixture]
    public class ProductShould
    {
        private Product sut;

        private Guid TestId = Guid.NewGuid();
        private const string TestName = nameof(TestName);
        private const string TestVarName = nameof(TestVarName);
        private const string TestUnit = nameof(TestUnit);
        private const double TestPrice = 5;
        private const int TestQuality = 0;
        private const int TestMTTF = 100;
        private const ProductTypes TestType = ProductTypes.Good;
        private const bool TestFractional = true;

        private Mock<IRandomizer> randMock;

        private IProductAmountCollection FailsInto;
        private Mock<IProduct> FailProductMock1;
        private Mock<IProduct> FailProductMock2;

        private IProductAmountCollection Maintenance;
        private Mock<IProduct> MaintenaceProductMock1;
        private Mock<IProduct> MaintenaceProductMock2;

        private IProductAmountCollection MaintStorage;

        [SetUp]
        public void Setup()
        {
            randMock = new Mock<IRandomizer>();
            randMock.Setup(x => x.NextDouble())
                .Returns(0.5);

            FailProductMock1 = new Mock<IProduct>();
            FailProductMock1.Setup(x => x.Id)
                .Returns(Guid.NewGuid());
            FailProductMock2 = new Mock<IProduct>();
            FailProductMock2.Setup(x => x.Id)
                .Returns(Guid.NewGuid());
            FailsInto = new ProductAmountCollection();
            FailsInto.AddProducts(FailProductMock1.Object, 1);
            FailsInto.AddProducts(FailProductMock2.Object, 1);

            MaintenaceProductMock1 = new Mock<IProduct>();
            MaintenaceProductMock1.Setup(x => x.Id)
                .Returns(Guid.NewGuid());
            MaintenaceProductMock2 = new Mock<IProduct>();
            MaintenaceProductMock2.Setup(x => x.Id)
                .Returns(Guid.NewGuid());
            Maintenance = new ProductAmountCollection();
            Maintenance.AddProducts(MaintenaceProductMock1.Object, 1);
            Maintenance.AddProducts(MaintenaceProductMock2.Object, 1);

            sut = new Product(randMock.Object);
            sut.Id = TestId;
            sut.Name = TestName;
            sut.VariantName = TestVarName;
            sut.UnitName = TestUnit;
            sut.DefaultPrice = TestPrice;
            sut.Quality = TestQuality;
            sut.MTTF = 100;
            sut.ProductType = TestType;
            sut.Fractional = TestFractional;
            sut.FailsInto = FailsInto;
            sut.Maintenance = Maintenance;
            sut.Maintainable = true;
        }

        private void AssertCollectionContains(IReadOnlyProductAmountCollection col, Mock<IProduct> product, double amount)
        {
            Assert.That(col.GetProductValue(product.Object), Is.EqualTo(amount));
        }

        #region Constructor

        [Test]
        public void ThrowArgumentNullFromConstructorWhenRandomizerIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => new Product(null));
        }

        #endregion Constructor

        #region MTTFAndDailyFailureChance

        [Test]
        [TestCase(100)]
        [TestCase(200)]
        public void ReturnInverseOfMTTFForDailyFailureChance(int MTTF)
        {
            sut.MTTF = MTTF;
            double failureChance = 1.0 / MTTF;
            Assert.AreEqual(sut.DailyFailureChance, failureChance);
        }

        #endregion MTTFAndDailyFailureChance

        #region FailureResults

        [Test]
        [TestCase(1)]
        [TestCase(5)]
        public void ReturnMultipliedScrapProducts(double failed)
        {
            // Get Failure Results.
            var result = sut.FailureResults(failed);

            // Ensure that result is correct.
            AssertCollectionContains(result, FailProductMock1, failed);
            AssertCollectionContains(result, FailProductMock2, failed);
        }

        #endregion FailureResults

        #region FailureProbability

        [Test]
        [TestCase(0)]
        [TestCase(-1)]
        public void ThrowArgumentOutOfRangeExceptionFromFailurePrababilityWhenDaysIsLessThanOne(int days)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => sut.FailureProbability(days));
        }

        [Test]
        public void ReturnsZeroWhenMTTFIsLessThanOneFromFailureProbability()
        {
            sut.MTTF = 0;
            Assert.AreEqual(sut.FailureProbability(100), 0);
        }

        [Test]
        [TestCase(0)]
        [TestCase(-1)]
        public void ThrowArgumentOutOfRangeExceptionFromFailurePrababilityWhenDaysIsLessThanOneButMaintenceMet(int days)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => sut.FailureProbability(days, 1));
        }

        [Test]
        [TestCase(-1)]
        [TestCase(2)]
        public void ThrowsArgumentOutOfRangeWhenMaintenanceMetIsNotBetween0and1(double badMaint)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => sut.FailureProbability(1, badMaint));
        }

        [Test]
        public void ReturnsZeroWhenMTTFIsLessThat1FromFailureProbabilityWithMaintenance()
        {
            sut.MTTF = 0;
            Assert.AreEqual(sut.FailureProbability(1, 1), 0);
        }

        [Test]
        [TestCase(5)] // Above 5, rounding errors begin making equality difficult/impossible.
        [TestCase(4)]
        [TestCase(3)]
        [TestCase(2)]
        public void ReturnHigherFailureChanceWhenMaintenanceMetIs0(int mttf)
        {
            // Setup MTTF
            sut.MTTF = mttf;

            // Get the result (should be 1-(1/MTTF)*2)
            var result = sut.FailureProbability(1, 0);

            double chance = 1 / (double)mttf;

            // Check that it's equal.
            Assert.That(result, Is.EqualTo(chance * 2));
        }

        [Test]
        [TestCase(4)]
        [TestCase(2)]
        public void ReturnCorrectFailureChanceWhenMaintenanceMetIs1(int mttf)
        {
            // Setup MTTF
            sut.MTTF = mttf;

            // Get the result (should be 1-(1/MTTF)*2)
            var result = sut.FailureProbability(1, 1);

            double chance = 1 / (double)mttf;

            // Check that it's equal.
            Assert.That(result, Is.EqualTo(chance));
        }

        #endregion FailureProbability

        #region FailedProducts

        [Test]
        [TestCase(-0.5)]
        [TestCase(-1)]
        [TestCase(-4)]
        public void ThrowOutOfRangeExceptionFromNegativeAmountFromOneParamFailedProducts(double badVal)
        {
            Assert.Throws<ArgumentOutOfRangeException>(
                () => sut.FailedProducts(badVal));
        }

        [Test]
        [TestCase(-0.5)]
        [TestCase(-1)]
        [TestCase(-4)]
        public void ThrowOutOfRangeExceptionFromNegativeAmount(double badVal)
        {
            Assert.Throws<ArgumentOutOfRangeException>(
                () => sut.FailedProducts(badVal, 1));
        }

        [Test]
        [TestCase(-1)]
        [TestCase(1.5)]
        public void ThrowOutOfRangeExceptionFromBadMaintenanceMetValues(double badMaintenance)
        {
            Assert.Throws<ArgumentOutOfRangeException>(
                () => sut.FailedProducts(1, badMaintenance));
        }

        [Test]
        public void ReturnFailedProductsSuccessfully()
        {
            // Setup Randomizer
            randMock.Setup(x => x.Normal(1, 1))
                .Returns(1);
            randMock.Setup(x => x.NextDouble())
                .Returns(0.5);

            // Setup MTTF
            sut.MTTF = 2;

            // Get Result
            var result = sut.FailedProducts(100);

            // Assert Results
            Assert.That(result, Is.EqualTo(50));
        }

        [Test]
        public void ReturnExtraFailedProduct()
        {
            // Setup Randomizer
            randMock.Setup(x => x.Normal(1, 1))
                .Returns(1);
            randMock.Setup(x => x.NextDouble())
                .Returns(0.4);

            // Setup MTTF
            sut.MTTF = 2;

            // Get Result
            var result = sut.FailedProducts(100);

            // Assert Results
            Assert.That(result, Is.EqualTo(51));
        }

        [Test]
        public void ReturnFailedProductsSuccessfullyWithMaintenanceMet()
        {
            // Setup Randomizer
            randMock.Setup(x => x.Normal(1, 1))
                .Returns(1);
            randMock.Setup(x => x.NextDouble())
                .Returns(0.5);

            // Setup MTTF
            sut.MTTF = 2;

            // Get Result
            var result = sut.FailedProducts(100, 1);

            // Assert Results
            Assert.That(result, Is.EqualTo(50));
        }

        [Test]
        public void ReturnExtraFailedProductWithMaintenanceMet()
        {
            // Setup Randomizer
            randMock.Setup(x => x.Normal(1, 1))
                .Returns(1);
            randMock.Setup(x => x.NextDouble())
                .Returns(0.4);

            // Setup MTTF
            sut.MTTF = 2;

            // Get Result
            var result = sut.FailedProducts(100, 1);

            // Assert Results
            Assert.That(result, Is.EqualTo(51));
        }

        [Test]
        public void ReturnMoreFailedProductsWhenMaintenanceLessThan1()
        {
            // Setup Randomizer
            randMock.Setup(x => x.Normal(1, 1))
                .Returns(1);
            randMock.Setup(x => x.NextDouble())
                .Returns(0.5);

            // Setup MTTF
            sut.MTTF = 4;

            // Get Result
            var result = sut.FailedProducts(100, 0);

            // Assert Results
            Assert.That(result, Is.EqualTo(50));
        }

        [Test]
        public void CallRandomizerCorrectlyInFailedProducts()
        {
            // Setup Randomizer
            randMock.Setup(x => x.Normal(1, 1))
                .Returns(1);
            randMock.Setup(x => x.NextDouble())
                .Returns(0.5);

            // Setup MTTF
            sut.MTTF = 4;

            // Get Result
            var result = sut.FailedProducts(100, 0);

            // Ensure RandomizerRan.
            randMock.Verify(x => x.Normal(1, 1), Times.Once);
            randMock.Verify(x => x.NextDouble(), Times.Once);
        }

        #endregion FailedProducts

        #region RunMaintenance

        [Test]
        public void ThrowArgumentNullExceptionFromRunMaintenance()
        {
            var sat = 0.0;
            Assert.Throws<ArgumentNullException>(() => sut.RunMaintenance(1, null, out sat));
        }

        [Test]
        public void ReturnEmptyCollectionAndFullSatisfactionWhenMaintenanceIsEmpty()
        {
            // Setup storage
            MaintStorage = new ProductAmountCollection();

            // setup maintenance
            sut.Maintenance = new ProductAmountCollection();

            // satisfaction Satisfaction
            var sat = 0.0;

            // Get Result
            var result = sut.RunMaintenance(1, MaintStorage, out sat);

            // Assert Satisfaction is correct.
            Assert.That(sat, Is.EqualTo(1));

            // Assert that Result is correct.
            Assert.That(result.Count(), Is.EqualTo(0));
        }

        [Test]
        public void ReturnNecissaryMaintenanceCostsAndMaintenanceMetFromRunMaintenanceWhenStorageIsEmpty()
        {
            // Setup storage
            MaintStorage = new ProductAmountCollection();

            // satisfaction Satisfaction
            var sat = 0.0;

            // Get Result
            var result = sut.RunMaintenance(1, MaintStorage, out sat);

            // Assert Satisfaction is correct.
            Assert.That(sat, Is.EqualTo(0));

            // Assert that Result is correct.
            AssertCollectionContains(result, MaintenaceProductMock1, 0);
            AssertCollectionContains(result, MaintenaceProductMock2, 0);
        }

        [Test]
        public void ReturnNecissaryMaintenanceCostsAndMaintenanceMetFromRunMaintenance()
        {
            // Setup storage
            MaintStorage = new ProductAmountCollection();
            MaintStorage.AddProducts(MaintenaceProductMock1.Object, 1);
            MaintStorage.AddProducts(MaintenaceProductMock2.Object, 1);

            // satisfaction Satisfaction
            var sat = 0.0;

            // Get Result
            var result = sut.RunMaintenance(1, MaintStorage, out sat);

            // Assert Satisfaction is correct.
            Assert.That(sat, Is.EqualTo(1));

            // Assert that Result is correct.
            AssertCollectionContains(result, MaintenaceProductMock1, 1);
            AssertCollectionContains(result, MaintenaceProductMock2, 1);
        }

        [Test]
        public void ReturnNecissaryMaintenanceCostsAndMaintenanceMetFromRunMaintenanceWhenOverstocked()
        {
            // Setup storage
            MaintStorage = new ProductAmountCollection();
            MaintStorage.AddProducts(MaintenaceProductMock1.Object, 5);
            MaintStorage.AddProducts(MaintenaceProductMock2.Object, 5);

            // satisfaction Satisfaction
            var sat = 0.0;

            // Get Result
            var result = sut.RunMaintenance(1, MaintStorage, out sat);

            // Assert Satisfaction is correct.
            Assert.That(sat, Is.EqualTo(1));

            // Assert that Result is correct.
            AssertCollectionContains(result, MaintenaceProductMock1, 1);
            AssertCollectionContains(result, MaintenaceProductMock2, 1);
        }

        [Test]
        public void ReturnPartialSuccessWhenStorageIsNotSatisfactory()
        {
            // Setup storage
            MaintStorage = new ProductAmountCollection();
            MaintStorage.AddProducts(MaintenaceProductMock1.Object, 1);
            MaintStorage.AddProducts(MaintenaceProductMock2.Object, 1);

            // satisfaction Satisfaction
            var sat = 0.0;

            // Get Result
            var result = sut.RunMaintenance(2, MaintStorage, out sat);

            // Assert Satisfaction is correct.
            Assert.That(sat, Is.EqualTo(0.5));

            // Assert that Result is correct.
            AssertCollectionContains(result, MaintenaceProductMock1, 1);
            AssertCollectionContains(result, MaintenaceProductMock2, 1);
        }

        #endregion RunMaintenance

        [Test]
        public void FindEqualityOnId()
        {
            var comp = new Product(randMock.Object);
            comp.Id = TestId;

            Assert.That(sut.Equals(comp), Is.True);
        }
    }
}
