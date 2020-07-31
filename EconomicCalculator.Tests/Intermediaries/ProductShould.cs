using EconomicCalculator.Intermediaries;
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

        [SetUp]
        public void Setup()
        {
            sut = new Product
            {
                Name = "TestProduct",
                UnitName = "Pound(s)",
                CurrentPrice = 10.00,
                MTTF = 100
            };
        }

        [Test]
        public void ReturnInverseOfMTTFForDailyFailureChance()
        {
            Assert.AreEqual(sut.DailyFailureChance, 1 / sut.MTTF);
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
    }
}
