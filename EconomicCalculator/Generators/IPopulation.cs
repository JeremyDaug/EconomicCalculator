using EconomicCalculator.Enums;
using EconomicCalculator.Intermediaries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EconomicCalculator.Generators
{
    /// <summary>
    /// Population Groups, 
    /// </summary>
    public interface IPopulation
    {
        /// <summary>
        /// The name of the population group.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// The Variant Name for the population group.
        /// </summary> TODO Implement later.
        // string VariantName { get; }

        /// <summary>
        /// The job name of the pop
        /// </summary>
        JobType Job { get; }

        /// <summary>
        /// The Skill Level of the Population
        /// </summary>
        // int SkillLevel { get; } TODO implement Later

        // Population Figures
        /// <summary>
        /// How many of the pop there is.
        /// </summary>
        int Count { get; }

        /// <summary>
        /// The Currencies owned by the population (based on market).
        /// </summary>
        IList<ICurrency> Currencies { get; }

        /// <summary>
        /// The amount of each currency available to the pops collectively (lots of averaging here).
        /// </summary>
        IDictionary<string, double> CurrencyAmmounts { get; }

        // Saleable goods produced this cycle.
        /// <summary>
        /// The goods for sale by the population
        /// </summary>
        IList<IProduct> GoodsForSale { get; }

        /// <summary>
        /// The Amount of Goods available for sale by the population.
        /// </summary>
        IDictionary<string, double> GoodAmmounts { get; }

        // Need Baskets
        /// <summary>
        /// The products needed for life.
        /// </summary>
        IList<IProduct> LifeNeeds { get; }

        /// <summary>
        /// The amount of each product needed.
        /// </summary>
        IDictionary<string, double> LifeNeedAmounts { get; }

        // TODO, implement this stuff later. Just focus on daily bread.
        /// <summary>
        /// The Products for daily needs.
        /// </summary>
        // IList<IProduct> DailyNeeds { get; }

        /// <summary>
        /// The amount of each product needed.
        /// </summary>
        // IDictionary<string, double> DailyNeedAmounts { get; }

        /// <summary>
        /// The Luxury Products the population seeks.
        /// </summary>
        // IList<IProduct> Luxuries { get; }

        /// <summary>
        /// The amount of Luxury Products.
        /// </summary> 
        // IDictionary<string, double> LuxuryAmounts { get; }
    }
}
