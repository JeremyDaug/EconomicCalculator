using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EconomicCalculator.Storage.Population;
using EconomicCalculator.Storage.Products;

namespace EconomicCalculator.Storage.Organizations
{
    public class Territory : ITerritory
    {
        private int _elevation;
        private double _waterCoverage;
        private int _roughness;
        private int _humidity;
        private double _infrastructureLevel;
        private List<ITerritory> _neighbors;

        public Territory()
        {
            Id = Guid.NewGuid();
            _neighbors = new List<ITerritory>();
            Ownership = new Dictionary<Guid, double>();
            Claimants = new Dictionary<Guid, IGovernor>();
        }

        #region GeneralData

        /// <summary>
        /// The Unique Id of the Territory
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// The Coordinates of the Territory on the map, currently uses Hex (axial) Coordinates.
        /// May abstract for more flexibility.
        /// </summary>
        public HexCoord Coord { get; set; }

        /// <summary>
        /// The name of the territory, this is optional.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The size of the territory in Acres.
        /// </summary>
        public double Extent { get; set; }

        /// <summary>
        /// The height of the territory (Unit TBD).
        /// 0 is not necissarily sea level.
        /// </summary>
        public int Elevation
        {
            get { return _elevation; }
            set { _elevation = value; }
        }

        /// <summary>
        /// The percentage of water that covers the land.
        /// Mostly meant for Coastal Territory, lakes, or seas.
        /// </summary>
        public double WaterCoverage
        {
            get => _waterCoverage;
            set
            {
                if (value < 0) // Water Level between 0 and 1 creates coast, 1 and above adds depth to a sea tile.
                    throw new ArgumentOutOfRangeException("Water Level must be greater than 1.");
                _waterCoverage = value;
            }
        }

        /// <summary>
        /// Placeholder for the amount of water in the region (probably measured in cubic KM).
        /// </summary>
        public double WaterQuantity { get; set; }

        /// <summary>
        /// Whether the territory has a river or not.
        /// </summary>
        public bool HasRiver { get; set; }

        /// <summary>
        /// The average humidity/Rainfall of the area.
        /// </summary>
        public int Humidity
        {
            get => _humidity;
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException("Humidity must be non-negative.");

                _humidity = value;
            }
        }

        /// <summary>
        /// The Average Tempurature of the territory.
        /// </summary>
        public int Tempurature { get; set; }

        /// <summary>
        /// How difficult the terrain is to cross.
        /// </summary>
        public int Roughness
        {
            get => _roughness;
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException("Roughness must greater than or equal to 0.");

                _roughness = value;
            }
        }

        /// <summary>
        /// The product form of land in the territory.
        /// </summary>
        public IPlot Plot { get; set; }

        #endregion GeneralData

        #region Organization

        /// <summary>
        /// The level of infrastructure, is build in portions with
        /// each whole number increase being a major step and overcoming
        /// roughness.
        /// TODO, update and replace with Public Goods as infrastructure.
        /// </summary>
        public double InfrastructureLevel
        {
            get => _infrastructureLevel;
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException("Infrastructure level must be non-negative.");

                _infrastructureLevel = value;
            }
        }

        // TODO Local Resources Will Go Here

        /// <summary>
        /// The depth of exploitation reached, and how much more it can be explored
        /// for material resources.
        /// </summary>
        public int ExploitationLevel { get; set; }

        /// <summary>
        /// Links to the neighboring Hexes. Exact organization TBD.
        /// </summary>
        public IReadOnlyList<ITerritory> Neighbors => _neighbors;

        /// <summary>
        /// Sets this territory's neighboring tiles, removing previous neighbors. 
        /// TODO make better method of adding neighbors, could be used with teleportation to make abnormal connections.
        /// </summary>
        /// <param name="neighbors">The list of Adjacent Territories.</param>
        public void SetNeighbors(IReadOnlyList<ITerritory> neighbors)
        {
            _neighbors = neighbors.ToList();
        }

        /// <summary>
        /// Connection to the territory's Market.
        /// </summary>
        public IMarket Market { get; set; }

        /// <summary>
        /// Who lives here, should be equal (at least in part) to the population of the market.
        /// </summary>
        public IPopulations Residents { get; set; }

        // public IPlanet Planet { get; set; } TODO

        // planet connection Placeholder

        /// <summary>
        /// The owner of the Territory. 
        /// May be null if unclaimed, or no clear owner.
        /// </summary>
        public IGovernor Owner { get; set; }

        /// <summary>
        /// The amount of land yet to be bought.
        /// TODO, update this to include and update upon purchase ownership and
        /// changes in water coverage.
        /// </summary>
        public double AvailableLand { get; set; }

        /// <summary>
        /// Who owns land and how much.
        /// </summary>
        public IDictionary<Guid, double> Ownership { get; }

        /// <summary>
        /// Governors who have claims on the land.
        /// </summary>
        public IDictionary<Guid, IGovernor> Claimants { get; }

        #endregion Organization

        #region HelperFuncs

        /// <summary>
        /// The current price of unused land in Abs Value, set by the territory owner.
        /// </summary>
        /// <returns>The current unit (1 acre) price of the unused land.</returns>
        public double UnusedLandPrice()
        {
            // TODO Update this to call the Governor.
            // Currently returns 0, allowing people to gain unused land by simply claiming it.
            return 0;
        }

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
        public IReadOnlyProductAmountCollection BuyLand(double amount, IPopulationGroup buyer)
        {
            // TODO, update when UnusedLandPrice is updated also, as that will require checking to see
            // if paying the governor is required, and that whole song and dance.

            if (buyer is null)
                throw new ArgumentNullException();

            // round to the smallest unit (0.1 acres).
            amount = Math.Round(amount, 1);

            if (amount <= 0)
                throw new ArgumentOutOfRangeException("Amount must be greater than 0.");

            var result = new ProductAmountCollection();

            // if no land is available, just return.
            if (AvailableLand == 0)
            {
                return result;
            }
            else if (amount < AvailableLand) // if more available land than is sought, then add.
            {
                result.AddProducts(Plot, amount);
            }
            else
            {// if less, then take what you can get.
                result.AddProducts(Plot, AvailableLand);
            }

            // Mark the owner in the territory.
            if (Ownership.ContainsKey(buyer.Id))
                Ownership[buyer.Id] += result.GetProductValue(Plot);
            else
                Ownership[buyer.Id] = result.GetProductValue(Plot);

            // return result
            return result;
        }

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
        public void UpdateOwnership(IPopulationGroup buyer, double acres, IPopulationGroup seller)
        {
            // null checks
            if (buyer is null)
                throw new ArgumentNullException(nameof(buyer));
            if (seller is null)
                throw new ArgumentNullException(nameof(seller));

            // round our acres
            acres = Math.Round(acres, 1);

            // Ensure it's positive.
            if (acres <= 0)
                throw new ArgumentOutOfRangeException("Acres for transfer must be greater than 0.");

            // Get how much the seller has.
            var sellLands = Ownership[seller.Id]; // throws in seller has no land.

            // ensure the land sought is not greater than the land available.
            if (acres > sellLands)
                throw new ArgumentOutOfRangeException("Acres being bought is more than the seller has available.");

            // since acres is useful, we can commense the transaction
            Ownership[seller.Id] -= acres;
            if (Ownership.ContainsKey(buyer.Id))
                Ownership[buyer.Id] += acres;
            else
                Ownership[buyer.Id] = acres;

            // transaction complete, GTFO
            return;
        }

        #endregion HelperFuncs

        public bool Equals(ITerritory other)
        {
            return this.Id == other.Id;
        }

        public bool Equals(ITerritory x, ITerritory y)
        {
            return x.Id == y.Id;
        }

        public int GetHashCode(ITerritory obj)
        {
            return obj.Id.GetHashCode();
        }

        public void LoadFromSql(SqlConnection connection)
        {
            throw new NotImplementedException();
        }
    }
}
