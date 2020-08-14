using EconomicCalculator.Generators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EconomicCalculator.Intermediaries
{
    /// <summary>
    /// The Market Interface.
    /// </summary>
    public interface IMarket
    {
        /// <summary>
        /// The name of the market.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// The name of a variant Market.
        /// </summary> TODO implement Later
        // string VariantName { get; }

        /// <summary>
        /// The Total Population in the market.
        /// Any population not in a job is considered unemployed. 
        /// Unemployment currently not implemented.
        /// </summary>
        double TotalPopulation { get; }

        // Populations in the market
        /// <summary>
        /// The Population who live within the market's zone.
        /// </summary>
        IList<IPopulation> Pops { get; }

        /// <summary>
        /// Available Crops in the market (Farmers can pick any they want every year)
        /// </summary>
        IList<ICrops> AvailableCrops { get; }

        /// <summary>                       
        /// What mines are located trivially close to the market.
        /// </summary>
        IList<IMine> AvailableMines { get; }

        /// <summary>
        /// What Currencies are available and accepted in the market.
        /// </summary>
        IList<ICurrency> AvailableCurrencies { get; }

        /// <summary>
        /// The available processes in the market that people can work.
        /// </summary>
        IList<IProcess> AvailableProcesses { get; }

        /// <summary>
        /// The Production Cycle
        /// </summary>
        void ProductionCycle();

        #region Goods

        /// <summary>
        /// The goods that the market produces.
        /// </summary>
        IList<IProduct> AvailableGoods { get; }

        /// <summary>
        /// The number of available goods on the market.
        /// </summary>
        IDictionary<string, double> AvailableGoodAmounts { get; }

        #endregion Goods

        #region PrintFunctions

        /// <summary>
        /// Prints the mines in the market.
        /// </summary>
        string PrintMines();

        /// <summary>
        /// Prints the crops
        /// </summary>
        string PrintCrops();

        /// <summary>
        /// Prints the Processes
        /// </summary>
        string PrintProcesses();

        /// <summary>
        /// Prints the Currencies
        /// </summary>
        string PrintCurrencies();

        /// <summary>
        /// Prints the Populations
        /// </summary>
        string PrintPops();

        /// <summary>
        /// Prints the Products
        /// </summary>
        string PrintProducts();

        #endregion PrintFunctions
    }
}
