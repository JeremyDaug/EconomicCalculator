using NUnit.Framework;
using EconomicCalculator.Storage.Processes;
using System;
using System.Collections.Generic;
using System.Text;
using EconomicCalculator.Storage.Jobs;
using Moq;
using EconomicCalculator.Storage.Products;
using EconomicCalculator.Storage;

namespace EconomicCalculator.Tests.Storage.Processes
{
    [TestFixture]
    public class ProcessShould
    {
        private Process sut;

        private Process other;
        private Mock<IJob> jobMock;
        private IProductAmountCollection jobInputs;
        private IProductAmountCollection jobOutputs;

        private Mock<IProduct> ProductMock1;

        [SetUp]
        public void Setup()
        {
            other = new Process();

            jobMock = new Mock<IJob>();

            jobInputs = new ProductAmountCollection();
            jobOutputs = new ProductAmountCollection();
            
            jobMock.Setup(x => x.Inputs)
                .Returns(jobInputs);
            jobMock.Setup(x => x.Outputs)
                .Returns(jobOutputs);

            var id = Guid.NewGuid();
            ProductMock1 = new Mock<IProduct>();
            ProductMock1.Setup(x => x.Id)
                .Returns(id);

            sut = new Process();
        }

        [Test]
        public void ThrowArgumentNullFromCreatesInputsForProcess()
        {
            Assert.Throws<ArgumentNullException>(() => sut.CreatesInputFor((IProcess)null));
        }

        [Test]
        public void ThrowArgumentNullFromCreatesInputsForJob()
        {
            Assert.Throws<ArgumentNullException>(() => sut.CreatesInputFor((IJob)null));
        }

        [Test]
        public void ThrowArgumentNullFromTakesOutputsFromProcess()
        {
            Assert.Throws<ArgumentNullException>(() => sut.CreatesInputFor((IProcess)null));
        }

        [Test]
        public void ThrowArgumentNullFromTokesOutputsFromJob()
        {
            Assert.Throws<ArgumentNullException>(() => sut.CreatesInputFor((IJob)null));
        }

        [Test]
        public void ReturnFalseWhenProcessDoesNotCreateInputsForProcess()
        {
            var result = sut.CreatesInputFor(other);

            Assert.That(result, Is.False);
        }

        [Test]
        public void ReturnTrueWhenProcessContainsInputsForProcess()
        {
            // Add product to both sut and other.
            other._inputs.AddProducts(ProductMock1.Object, 1);
            sut._outputs.AddProducts(ProductMock1.Object, 1);

            // check they overlap.
            var result = sut.CreatesInputFor(other);

            // Assert it's true.
            Assert.That(result, Is.True);
        }

        [Test]
        public void ReturnFalseWhenProcessDoesNotTakeOutputsFromProcess()
        {
            var result = sut.TakesOutputFrom(other);

            Assert.That(result, Is.False);
        }

        [Test]
        public void ReturnTrueWhenProcessTakesOutputFromProcess()
        {
            // Add product to both sut and other.
            other._outputs.AddProducts(ProductMock1.Object, 1);
            sut._inputs.AddProducts(ProductMock1.Object, 1);

            // check they overlap.
            var result = sut.TakesOutputFrom(other);

            // Assert it's true.
            Assert.That(result, Is.True);
        }

        [Test]
        public void ReturnFalseWhenProcessDoesNotCreateInputsForJob()
        {
            var result = sut.CreatesInputFor(jobMock.Object);

            Assert.That(result, Is.False);
        }

        [Test]
        public void ReturnTrueWhenProcessContainsInputsForJob()
        {
            // Add product to both sut and other.
            jobInputs.AddProducts(ProductMock1.Object, 1);
            sut._outputs.AddProducts(ProductMock1.Object, 1);

            // check they overlap.
            var result = sut.CreatesInputFor(jobMock.Object);

            // Assert it's true.
            Assert.That(result, Is.True);
        }

        [Test]
        public void ReturnFalseWhenProcessDoesNotTakeOutputsFromJob()
        {
            var result = sut.TakesOutputFrom(jobMock.Object);

            Assert.That(result, Is.False);
        }

        [Test]
        public void ReturnTrueWhenProcessTakesOutputFromJob()
        {
            // Add product to both sut and other.
            jobOutputs.AddProducts(ProductMock1.Object, 1);
            sut._inputs.AddProducts(ProductMock1.Object, 1);

            // check they overlap.
            var result = sut.TakesOutputFrom(jobMock.Object);

            // Assert it's true.
            Assert.That(result, Is.True);
        }
    }
}
