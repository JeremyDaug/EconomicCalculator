using EconomicCalculator.Storage.Population;
using EconomicCalculator.Storage.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EconomicCalculator.Storage.Organizations
{
    public interface ITerritory : IEquatable<ITerritory>, IEqualityComparer<ITerritory>, ISqlReader
    {
        #region GeneralData

        /// <summary>
        /// The Unique Id of the Territory
        /// </summary>
        Guid Id { get; }

        /// <summary>
        /// The Coordinates of the Territory on the map, currently uses Hex (axial) Coordinates.
        /// May abstract for more flexibility.
        /// </summary>
        HexCoord Coord { get; }

        /// <summary>
        /// The name of the territory, this is optional.
        /// </summary>
        string Name { get; }

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
        /// Mostly meant for Coastal Territory, lakes, or seas.
        /// </summary>
        double WaterLevel { get; }

        /// <summary>
        /// Whether the territory has a river or not.
        /// </summary>
        bool HasRiver { get; }

        /// <summary>
        /// The average humidity/Rainfall of the area.
        /// </summary>
        int Humidity { get; }

        /// <summary>
        /// The Average Tempurature of the territory.
        /// </summary>
        int Tempurature { get; }

        /// <summary>
        /// How difficult the terrain is to cross.
        /// </summary>
        int Roughness { get; }

        /// <summary>
        /// The product form of land in the territory.
        /// </summary>
        IPlot Plot { get; }

        #endregion GeneralData

        #region Organization

        /// <summary>
        /// The level of infrastructure, is build in portions with
        /// each whole number increase being a major step and overcoming
        /// roughness.
        /// This is a place holder and needs to be expanded upon.
        /// </summary>
        double InfrastructureLevel { get; }

        /// <summary>
        /// Links to the neighboring Hexes. Exact organization TBD.
        /// </summary>
        IReadOnlyList<ITerritory> Neighbors { get; }

        /// <summary>
        /// Connection to the territory's Market.
        /// </summary>
        IMarket Market { get; }

        /// <summary>
        /// Who lives here, should be roughly equal to market's population.
        /// </summary>
        IPopulations Residents { get; }

        // planet connection Placeholder

        /// <summary>
        /// The owner of the Territory. 
        /// May be null if unclaimed, or no clear owner.
        /// </summary>
        IGovernor Owner { get; }

        /// <summary>
        /// The amount of land yet owned.
        /// </summary>
        double AvailableLand { get; }

        /// <summary>
        /// Who owns what land and how much.
        /// </summary>
        IDictionary<Guid, double> Ownership { get; }

        /// <summary>
        /// Governors who have claims on the land.
        /// </summary>
        IDictionary<Guid, IGovernor> Claimants { get; }

        #endregion Organization

        #region HelperFuncs

        /// <summary>
        /// The current price of unused land, set by the territory owner.
        /// </summary>
        /// <returns>The current price of the unused land.</returns>
        double UnusedLandPrice();

        /// <summary>
        /// Buy unused/unowned land.
        /// </summary>
        /// <param name="amonut">The amount to buy (</param>
        /// <returns>The land bought.</returns>
        /// <remarks>
        /// Land bought this way is bought from the owning government.
        /// It comes in a few varieties. Purchasing, homesteading, etc.
        /// </remarks>
        /// <exception cref="ArgumentOutOfRangeException">
        /// If the amount sought is not available.
        /// </exception>
        IReadOnlyProductAmountCollection BuyLand(double amonut, IPopulationGroup buyer);

        /// <summary>
        /// Moves land ownership from one person to another.
        /// </summary>
        /// <param name="buyer">The person recieving the land.</param>
        /// <param name="acres">How much land they're buying.</param>
        /// <param name="seller">Who they're buying it from.</param>
        /// <exception cref="ArgumentNullException"><paramref name="buyer"/> is null.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="seller"/> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">If <paramref name="acres"/> is not between 0 and the seller's available acres.</exception>
        /// <exception cref="KeyNotFoundException">If Seller Does not own any land.</exception>
        void UpdateOwnership(IPopulationGroup buyer, double acres, IPopulationGroup seller);

        // housing

        // factories

        // Warehouses/Merchants

        #endregion HelperFuncs
    }
}
