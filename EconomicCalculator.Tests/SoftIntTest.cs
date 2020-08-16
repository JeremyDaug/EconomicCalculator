using EconomicCalculator.Runner;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace EconomicCalculator.Tests
{
    /// <summary>
    /// A test class which tests the classes together.
    /// Not a true integration test as it does not connect to
    /// a DB, but a sanity check for initial construction and planing.
    /// </summary>
    [TestFixture]
    public class SoftIntTest
    {
        private World sutWorld;

        [SetUp]
        public void Setup()
        {
            sutWorld = new World();
        }
    }
}
