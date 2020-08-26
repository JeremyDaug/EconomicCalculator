using EconomicCalculator.Storage;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace EconomicCalculator.Tests.Storage
{
    [TestFixture]
    class PopulationGroupShould
    {
        private PopulationGroup sut;

        [SetUp]
        public void Setup()
        {
            sut = new PopulationGroup();
        }
    }
}
