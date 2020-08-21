using EconomicCalculator.Generators;
using EconomicCalculator.Intermediaries;
using System;
using System.Collections.Generic;

namespace EconomicCalculator.Storage

{
    /// <summary>
    /// The Market Interface, stores information about markets.
    /// </summary>
    public interface IMarket : IEquatable<IMarket>
    {
        /// <summary>
        /// The Unique Id of the market.
        /// </summary>
        Guid Id { get; }

        #region GeneralInfo

        /// <summary>
        /// The name of the market.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// The Total Population in the market.
        /// Any population not in a job is considered unemployed.
        /// </summary>
        double TotalPopulation { get; }

        /// <summary>
        /// The Territorial extent of the Market in acres. -1 means infinite.
        /// </summary>
        int Territory { get; }

        #endregion GeneralInfo

        #region InfoDetails

        // Population Breakdown and management.

        // Territory Breakdown and management.

        #endregion InfoDetails

        #region TheMarket

        // Availability
        /// <summary>
        /// What products are available, and how many are available.
        /// Positive values represent available goods and surplus.
        /// Negative Values represent Shortfalls.
        /// </summary>
        IProductAmountCollection ProductAvailability { get; }

        // Production Phase

        // Buying Phase

        #endregion TheMarket

        #region AvailableOptions

        // Crops
        // Mines
        // Processes
        // Accepted Currencies

        #endregion AvailableOptions

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
