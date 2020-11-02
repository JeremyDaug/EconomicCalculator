using EconomicCalculator.Storage.Jobs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EconomicCalculator.Storage.Processes
{
    internal class Process : IProcess
    {
        public IProductAmountCollection _inputs;
        public IProductAmountCollection _capital;
        public IProductAmountCollection _outputs;

        public Process()
        {
            Id = Guid.NewGuid();
            _inputs = new ProductAmountCollection();
            _outputs = new ProductAmountCollection();
            _capital = new ProductAmountCollection();
        }

        public Guid Id { get; set; }

        public string Name { get; set; }

        public IReadOnlyProductAmountCollection Inputs { get => _inputs; }

        public IReadOnlyProductAmountCollection Capital { get => _capital; }

        public IReadOnlyProductAmountCollection Outputs { get => _outputs; }

        public IReadOnlyProductAmountCollection AverageCapitalRequirements()
        {
            // prepare our result;
            var result = new ProductAmountCollection();

            // for each product in capital requirements.
            foreach (var pair in Capital)
            {
                // get the product and the amount needed.
                var product = pair.Item1;
                var amount = pair.Item2;

                // Add the product to our result, multiplying the amount needed by the daily failure chance
                // that is how many we expect each day on average (assuming maintenance is met).
                result.AddProducts(product, amount * product.DailyFailureChance);
            }

            // return our result.
            return result;
        }

        public bool CreatesInputFor(IProcess other)
        {
            if (other is null)
                throw new ArgumentNullException(nameof(other));

            foreach (var pair in Outputs)
            {
                // get product
                var product = pair.Item1;

                // if product is in other's inputs, return true.
                if (other.Inputs.Contains(product))
                    return true;
                // else go to the next product.
            }

            // if nothing matches, then returrn false.
            return false;
        }

        public bool TakesOutputFrom(IProcess other)
        {
            if (other is null)
                throw new ArgumentNullException(nameof(other));

            foreach (var pair in Inputs)
            {
                // get product
                var product = pair.Item1;

                // if product is in other's outputs, return true.
                if (other.Outputs.Contains(product))
                    return true;
                // else go to the next product.
            }

            // if nothing matches, then returrn false.
            return false;
        }

        public bool CreatesInputFor(IJob other)
        {
            if (other is null)
                throw new ArgumentNullException(nameof(other));

            foreach (var pair in Outputs)
            {
                // get product
                var product = pair.Item1;

                // if product is in other's inputs, return true.
                if (other.Inputs.Contains(product))
                    return true;
                // else go to the next product.
            }

            // if nothing matches, then returrn false.
            return false;
        }

        public bool TakesOutputFrom(IJob other)
        {
            if (other is null)
                throw new ArgumentNullException(nameof(other));

            foreach (var pair in Inputs)
            {
                // get product
                var product = pair.Item1;

                // if product is in other's outputs, return true.
                if (other.Outputs.Contains(product))
                    return true;
                // else go to the next product.
            }

            // if nothing matches, then returrn false.
            return false;
        }

        public bool Equals(IProcess other)
        {
            return this.Id == other.Id;
        }

        public bool Equals(IProcess x, IProcess y)
        {
            return x.Id == y.Id;
        }

        public int GetHashCode(IProcess obj)
        {
            return obj.Id.GetHashCode();
        }
    }
}
