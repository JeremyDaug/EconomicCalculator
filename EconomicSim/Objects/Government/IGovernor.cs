using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EconomicSim.Objects.Market;
using EconomicSim.Objects.Products;
using EconomicSim.Objects.Territory;

namespace EconomicSim.Objects.Government
{
    /// <summary>
    /// Readonly interface for governments
    /// </summary>
    public interface IGovernor
    {
        /// <summary>
        /// Id of the Governor
        /// </summary>
        int Id { get; }

        /// <summary>
        /// The name of the governor.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// The markets this governor owns.
        /// </summary>
        IReadOnlyList<IMarket> Markets { get; }

        /// <summary>
        /// What lands this governor owns.
        /// </summary>
        IReadOnlyList<ITerritory> Territories { get; }

        // Claims

        // Government Controls
        // TODO: Add more detail here

        /// <summary>
        /// how aggressive and expansionist the government is. Value between -1 and 1.
        /// </summary>
        decimal Aggression { get; }

        /// <summary>
        /// How the government interacts with it's economy -1 refuses to interfere, to 1 total control.
        /// </summary>
        decimal EconomicPolicy { get; }

        /// <summary>
        /// The currencies considered valid by the government and used to pay taxes.
        /// </summary>
        IReadOnlyList<IProduct> Currencies { get; }

        /// <summary>
        /// The resources the governor has.
        /// </summary>
        IReadOnlyList<(IProduct product, decimal amount)> Resources { get; }

        /// <summary>
        /// Taxes levied on specific products at sale.
        /// Product is what is taxed.
        /// Percent is how much of it's value is taxed per unit.
        /// specific is the specific items required to pay for the tax.
        /// </summary>
        IReadOnlyList<(IProduct product, decimal percent, IReadOnlyList<(IProduct product, decimal amount)> specific)> ConsumptionTaxes { get; }

        /// <summary>
        /// Taxes levied on owning certain products. 
        /// Product, Percent is percent of the item(s) value.
        /// Specific is the specific items requried to pay the tax per unit.
        /// </summary>
        IReadOnlyList<(IProduct product, decimal percent, IReadOnlyList<(IProduct product, decimal amount)> specific)> PropertyTaxes { get; }

        /// <summary>
        /// Taxes applied across an entire section flatly.
        /// </summary>
        IReadOnlyList<(TaxTarget tax, decimal percent)> GeneralTaxes { get; }

        // TODO rework taxes to be more efficient and well organized, a bunch of lists like this
        // is unacceptable honestly.

        // TODO military connections.
    }
}
