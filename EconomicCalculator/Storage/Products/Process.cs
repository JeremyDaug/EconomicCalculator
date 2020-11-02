using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EconomicCalculator.Storage.Products
{
    internal class Process : IProcess
    {
        public Guid Id => throw new NotImplementedException();

        public string Name => throw new NotImplementedException();

        public IProductAmountCollection Inputs => throw new NotImplementedException();

        public IProductAmountCollection Capital => throw new NotImplementedException();

        public IProductAmountCollection Outputs => throw new NotImplementedException();

        public IProductAmountCollection AverageCapitalRequirements()
        {
            throw new NotImplementedException();
        }

        public bool CreatesInputFor(IProcess other)
        {
            throw new NotImplementedException();
        }

        public bool TakesOutputFrom(IProcess other)
        {
            throw new NotImplementedException();
        }

        public bool Equals(IProcess other)
        {
            throw new NotImplementedException();
        }

        public bool Equals(IProcess x, IProcess y)
        {
            throw new NotImplementedException();
        }

        public int GetHashCode(IProcess obj)
        {
            throw new NotImplementedException();
        }
    }
}
