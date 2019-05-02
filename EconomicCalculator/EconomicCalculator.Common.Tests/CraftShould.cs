using Moq;
using NUnit.Framework;
using System;

namespace EconomicCalculator.Common.Tests
{
    [TestFixture]
    public class CraftShould
    {
        private Craft sut;
        private Mock<Job> jobMock;

        [SetUp]
        public void Setup()
        {
            sut = new Craft
            {
                Name = "Test",
                Id = Guid.NewGuid(),
                Job = jobMock.Object,
                //Inputs = new ProductAmount
            };
        }

        [Test]
        public void CallProductPriceCorrectly()
        {

        }
    }
}
