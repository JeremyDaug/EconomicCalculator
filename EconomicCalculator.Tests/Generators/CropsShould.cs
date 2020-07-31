using EconomicCalculator.Generators;
using EconomicCalculator.Intermediaries;
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

        private Mock<IProduct> ProductMock1;
        private Mock<IProduct> ProductMock2;

        [SetUp]
        public void Setup()
        {
            sut = new Crops();
        }
    }
}
