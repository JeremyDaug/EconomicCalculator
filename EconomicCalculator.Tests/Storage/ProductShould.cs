using EconomicCalculator.Enums;
using EconomicCalculator.Intermediaries;
using EconomicCalculator.Randomizer;
using EconomicCalculator.Storage;
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

        [SetUp]
        public void Setup()
        {
            randMock = new Mock<IRandomizer>();
            randMock.Setup(x => x.NextDouble())
                .Returns(0.5);

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
        }

        [Test]
        [TestCase(100)]
        [TestCase(200)]
        public void ReturnInverseOfMTTFForDailyFailureChance(int MTTF)
        {
            sut.MTTF = MTTF;
            double failureChance = 1.0 / MTTF;
            Assert.AreEqual(sut.DailyFailureChance, failureChance);
        }

        [Test]
        public void ThrowArgumentOutOfRangeExceptionFromFailurePrababilityWhenDaysIsLessThanOne()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => sut.FailureProbability(0));
        }

        [Test]
        public void ReturnsZeroWhenMTTFIsLessThanOneFromFailureProbability()
        {
            sut.MTTF = 0;
            Assert.AreEqual(sut.FailureProbability(100), 0);
        }

        [Test]
        public void FindEqualityOnId()
        {
            var comp = new Product(randMock.Object);
            comp.Id = TestId;

            Assert.That(sut.Equals(comp), Is.True);
        }

        [Test]
        public void ReturnFailedProductsSelectedRandomly()
        {
            var result = sut.FailedProducts(100);

            Assert.That(result, Is.EqualTo(1));

            randMock.Verify(x => x.NextDouble(), Times.Once);
        }
    }
}
