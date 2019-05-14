using NUnit.Framework;
using StorageManager.ProcessManagers;
using EconomicCalculator.Common.Processes;
using System.IO;
using System;
using System.Collections.Generic;

namespace StorageManager.Tests
{
    [TestFixture]
    public class ButcherManagerShould
    {
        private const string fullTestPath = "butchers.xml";
        private ButcherManager sut;
        private string testPath;

        [SetUp]
        public void Setup()
        {
            sut = new ButcherManager();
            testPath = Path.GetFullPath("D:\\Projects\\EconomicCalculator\\EconomicCalculator\\DataStorage\\"+fullTestPath);
            Console.WriteLine(testPath);
        }

        [Test]
        public void LoadButchersFromFlie()
        {
            var result = sut.LoadButchers(testPath);

            Assert.That(result, Is.TypeOf<List<Butcher>>());
        }
    }
}
