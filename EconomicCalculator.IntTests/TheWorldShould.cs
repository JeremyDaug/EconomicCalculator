using EconomicCalculator.Intermediaries;
using EconomicCalculator.Runner;
using EconomicCalculator.Storage;
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

            //TestMarket1 = new Market()

            //sut.AddMarket(TestMarket1);
        }

        [Test]
        public void Test1()
        {
            Assert.Pass();
        }
    }
}