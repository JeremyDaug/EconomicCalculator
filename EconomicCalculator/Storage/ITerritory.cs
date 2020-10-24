using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EconomicCalculator.Storage
{
    public interface ITerritory : IEquatable<ITerritory>, IEqualityComparer<ITerritory>, ISqlReader
    {
        /// <summary>
        /// The name of the territory
        /// </summary>
        string Name { get; }

        /// <summary>
        /// The Unique Id of the Territory
        /// </summary>
        Guid Id { get; }

        // planet connection

        // Owner Connection

        /// <summary>
        /// Connection to the territory's Market.
        /// </summary>
        IMarket Market { get; }

        /// <summary>
        /// The smallest unit of land that can be bought (1/10 of an acre).
        /// </summary>
        string SmallestUnit { get; }

        /// <summary>
        /// The size of the territory in Acres.
        /// </summary>
        double Extent { get; }

        /// <summary>
        /// The height of the territory (Unit TBD).
        /// 0 is not necissarily sea level.
        /// </summary>
        int Elevation { get; }

        /// <summary>
        /// The percentage of water that covers the land.
        /// Mostly meant for Coastal Territory, or very large lakes.
        /// </summary>
        double WaterLevel { get; }

        /// <summary>
        /// Whether the territory has a river or not.
        /// </summary>
        bool HasRiver { get; }

        /// <summary>
        /// Who owns what land and how much.
        /// </summary>
        IDictionary<Guid, double> Ownership { get; }

        /// <summary>
        /// Governors who have claims on the land.
        /// </summary>
        IDictionary<Guid, IGovernor> Claimants { get; }

        /// <summary>
        /// The Amount of unbought land.
        /// </summary>
        /// <returns>The amount of unbought land.</returns>
        double AvailableLand();

        /// <summary>
        /// The current price of unused land, set by the territory owner.
        /// </summary>
        /// <returns>The current price of the unused land.</returns>
        double UnusedLandPrice();

        /// <summary>
        /// Buy unused land.
        /// </summary>
        /// <param name="amonut">The amount to buy (</param>
        /// <returns>The land bought.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// If the amount sought is not available.
        /// </exception>
        IProduct BuyLand(double amonut);

        // housing

        // factories

        // Warehouses/Merchants
    }
}
