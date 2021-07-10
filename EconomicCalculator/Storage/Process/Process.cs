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

        public string VariantName { get; set; }

        /// <summary>
        /// The goods or items which need to be consumed to produce the good.
        /// This includes raw materials as well as labor costs.
        /// </summary>
        public IReadOnlyProductAmountCollection Inputs { get => _inputs; }

        /// <summary>
        /// Goods which are needed to produce the good, but are not consumed.
        /// This includes capital goods, land, buildings, etc.
        /// </summary>
        public IReadOnlyProductAmountCollection Capital { get => _capital; }

        /// <summary>
        /// The products produced by the process.
        /// </summary>
        public IReadOnlyProductAmountCollection Outputs { get => _outputs; }

        #region HelperFunctions

        /// <summary>
        /// The Average daily requirement of capital goods, taking both
        /// good failure and good maintenance into account.
        /// </summary>
        /// <returns>The expected daily requirements of goods and maintenance.</returns>
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

        /// <summary>
        /// Whether a this process Produces an output of the <paramref name="other"/> process.
        /// </summary>
        /// <param name="other">The process we want to check we have an input to.</param>
        /// <returns>True if this process produces an input to the <paramref name="other"/> process.</returns>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="other"/> is null.
        /// </exception>
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

        /// <summary>
        /// Whether this process takes an output product as an input.
        /// </summary>
        /// <param name="other">The process we want to check as taking output from.</param>
        /// <returns>True if this process takes an output product from <paramref name="other"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="other"/> is null.
        /// </exception>
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

        /// <summary>
        /// Whether a this process Produces an output of the <paramref name="other"/> process.
        /// </summary>
        /// <param name="other">The Job we want to check we have an input to.</param>
        /// <returns>True if this process produces an input to the <paramref name="other"/> process.</returns>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="other"/> is null.
        /// </exception>
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

        /// <summary>
        /// Whether this process takes an output product as an input.
        /// </summary>
        /// <param name="other">The Job we want to check as taking output from.</param>
        /// <returns>True if this process takes an output product from <paramref name="other"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="other"/> is null.
        /// </exception>
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

        #endregion HelperFunctions
    }
}
