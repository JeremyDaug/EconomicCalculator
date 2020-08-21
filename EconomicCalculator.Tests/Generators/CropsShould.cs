using EconomicCalculator.Enums;
using EconomicCalculator.Generators;
using EconomicCalculator.Intermediaries;
using EconomicCalculator.Storage;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace EconomicCalculator.Tests.Generators
{
    [TestFixture]
    public class CropsShould
    {
        private Crops sut;

        private Mock<IProduct> SeedMock;

        private Mock<IProduct> ProductMock1;
        private Mock<IProduct> ProductMock2;

        private string CropName = "TestCrop";
        private int CropCycle = 100;
        private double LaborRequirements = 1;

        [SetUp]
        public void Setup()
        {
            SeedMock = new Mock<IProduct>();

            ProductMock1 = new Mock<IProduct>();
            ProductMock2 = new Mock<IProduct>();

            sut = new Crops
            {
                Name = CropName
            };
        }

        [Test]
        public void WorkProperly()
        {

        }
    }
}
