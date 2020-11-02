using EconomicCalculator.Storage.Jobs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EconomicCalculator.Storage.Processes
{
    /// <summary>
    /// A recipe or process to turn some set of goods into another good or set of goods.
    /// </summary>
    public interface IProcess : IEquatable<IProcess>, IEqualityComparer<IProcess>
    {
        #region Data

        /// <summary>
        /// The Unique Id of the process
        /// </summary>
        Guid Id { get; }

        /// <summary>
        /// The name of the process.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// The goods or items which need to be consumed to produce the good.
        /// This includes raw materials as well as labor costs.
        /// </summary>
        IReadOnlyProductAmountCollection Inputs { get; }

        /// <summary>
        /// Goods which are needed to produce the good, but are not consumed.
        /// This includes both capital goods, land, and buildings.
        /// </summary>
        IReadOnlyProductAmountCollection Capital { get; }

        /// <summary>
        /// The products produced by the process.
        /// </summary>
        IReadOnlyProductAmountCollection Outputs { get; }

        #endregion Data

        #region HelperFunctions

        /// <summary>
        /// Whether a this process Produces an output of the <paramref name="other"/> process.
        /// </summary>
        /// <param name="other">The process we want to check we have an input to.</param>
        /// <returns>True if this process produces an input to the <paramref name="other"/> process.</returns>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="other"/> is null.
        /// </exception>
        bool CreatesInputFor(IProcess other);

        /// <summary>
        /// Whether this process takes an output product as an input.
        /// </summary>
        /// <param name="other">The process we want to check as taking output from.</param>
        /// <returns>True if this process takes an output product from <paramref name="other"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="other"/> is null.
        /// </exception>
        bool TakesOutputFrom(IProcess other);

        /// <summary>
        /// Whether a this process Produces an output of the <paramref name="other"/> process.
        /// </summary>
        /// <param name="other">The Job we want to check we have an input to.</param>
        /// <returns>True if this process produces an input to the <paramref name="other"/> process.</returns>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="other"/> is null.
        /// </exception>
        bool CreatesInputFor(IJob other);

        /// <summary>
        /// Whether this process takes an output product as an input.
        /// </summary>
        /// <param name="other">The Job we want to check as taking output from.</param>
        /// <returns>True if this process takes an output product from <paramref name="other"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="other"/> is null.
        /// </exception>
        bool TakesOutputFrom(IJob other);

        /// <summary>
        /// The Average daily requirement of capital goods, taking both
        /// good failure and good maintenance into account.
        /// </summary>
        /// <returns>The expected daily requirements of goods and maintenance.</returns>
        IReadOnlyProductAmountCollection AverageCapitalRequirements();

        #endregion HelperFunctions
    }
}
