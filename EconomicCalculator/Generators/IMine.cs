using EconomicCalculator.Enums;
using EconomicCalculator.Intermediaries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EconomicCalculator.Generators
{
    public interface IMine : IJob
    {
        /// <summary>
        /// The name of the mine.
        /// </summary>
        new string Name { get; }

        /// <summary>
        /// The variant name for unique instances of mines.
        /// </summary> 
        // string VariantName { get; }

        /// <summary>
        /// The type of mine it is.
        /// </summary>
        MineType MineType { get; }

        /// <summary>
        /// The type of rock being mined.
        /// </summary>
        RockType RockType { get; }

        // Labor Costs
        /// <summary>
        /// How much labor of various skills is required in a day.
        /// </summary>
        new double LaborRequirements { get; }

        #region Capital

        // Setup Costs / Maintenance Todo Implement Later
        /// <summary>
        /// The capital needed to run the mine at all.
        /// </summary>
        // IList<IProduct> Capital { get; }

        /// <summary>
        /// How much of each capital product is needed.
        /// </summary>
        // IDictionary<string, double> CapitalAmounts { get; }

        #endregion Capital

        #region Requirements
        
        // This is not implemented, though it does exist.
        /// <summary>
        /// The requirements to keep the mine running each day.
        /// </summary>
        IList<IProduct> Requirements { get; }

        /// <summary>
        /// How much of each requirement is needed per day.
        /// </summary>
        IDictionary<string, double> RequirementAmounts { get; }

        #endregion Requirements

        #region Products

        // Production results
        /// <summary>
        /// The products produced from the mine per day.
        /// </summary>
        IList<IProduct> Products { get; }

        /// <summary>
        /// How much of each product is produced per day.
        /// </summary>
        IDictionary<string, double> ProductAmounts { get; }

        #endregion Products
    }
}
