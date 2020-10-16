using EconomicCalculator.Enums;
using EconomicCalculator.Intermediaries;
using EconomicCalculator.Storage;
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

        [SetUp]
        public void Setup()
        {
            sut = new Product
            {
                Id = TestId,
                Name = TestName,
                VariantName = TestVarName,
                UnitName = TestUnit,
                DefaultPrice = TestPrice,
                Quality = TestQuality,
                MTTF = 100,
                ProductType = TestType,
                Fractional = TestFractional
            };
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
            var comp = new Product
            {
                Id = TestId
            };

            Assert.That(sut.Equals(comp), Is.True);
        }
    }
}
