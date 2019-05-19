using NUnit.Framework;
using StorageManager.ProcessManagers;
using EconomicCalculator.Common.Processes;
using System.IO;
using System;
using System.Collections.Generic;
using EconomicCalculator.Common.Resource;

namespace StorageManager.Tests
{
    [TestFixture]
    public class ButcherManagerShould
    {
        private const string testLoadPath = "butchers.xml";
        private const string testSavePath = "butcherTestSave.xml";
        private ButcherManager sut;
        private Butcher testButcher;
        private string fullTestLoadPath;
        private string fullTestSavePath;

        [SetUp]
        public void Setup()
        {
            testButcher = new Butcher
            {
                Name = "TestName",
                Variant = "TestVariant",
                Animal = "TestCow",
                Products = new List<InputOutputs>
                {
                    new InputOutputs
                    {
                        Name = "TestBeef",
                        Amount = 1234,
                        PricePerUnit = 10
                    }
                },
                PriceMultiplier = 1
            };
            sut = new ButcherManager();

            sut.Butchers.Add(testButcher);
            fullTestLoadPath =
                Path.GetFullPath("D:\\Projects\\EconomicCalculator\\EconomicCalculator\\TestDataStorage\\"+testLoadPath);
            fullTestSavePath =
                Path.GetFullPath("D:\\Projects\\EconomicCalculator\\EconomicCalculator\\TestDataStorage\\" + testSavePath);
            Console.WriteLine(testLoadPath);
        }

        [Test]
        public void LoadButchersFromFlie()
        {
            var result = sut.LoadButchers(fullTestLoadPath);
        }

        [Test]
        public void SaveButchersToTargetFile()
        {
            var result = sut.SaveButchers(fullTestLoadPath);

            sut.SaveButchers(fullTestSavePath, result);
        }
    }
}
