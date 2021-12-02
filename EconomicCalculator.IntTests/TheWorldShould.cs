using EconomicCalculator.Refactor.Storage;
using EconomicCalculator.Runner;
using EconomicCalculator.DTOs;
using NUnit.Framework;

namespace Tests
{
    public class TheWorldShould
    {
        private World sut;

        private IMarket TestMarket1;

        [SetUp]
        public void Setup()
        {
            sut = new World();

            //sut.AddMarket(TestMarket1);
        }

        [Test]
        public void Test1()
        {
            Assert.Pass();
        }
    }
}