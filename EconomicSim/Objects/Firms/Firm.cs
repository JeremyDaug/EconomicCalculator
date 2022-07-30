using System.Text.Json.Serialization;
using EconomicSim.Objects.Market;
using EconomicSim.Objects.Products;
using EconomicSim.Objects.Technology;

namespace EconomicSim.Objects.Firms
{
    /// <summary>
    /// Firm Data Class
    /// </summary>
    [JsonConverter(typeof(FirmJsonConverter))]
    public class Firm : IFirm
    {
        public Firm()
        {
            Regions = new List<Market.Market>();
            Resources = new Dictionary<Product, decimal>();
            Products = new Dictionary<Product, decimal>();
            Jobs = new List<FirmJob>();
            Children = new List<Firm>();
            Techs = new List<(Technology.Technology tech, int research)>();
        }

        /// <summary>
        /// Id of the firm
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Name of the firm.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The rank of the firm.
        /// </summary>
        public FirmRank FirmRank { get; set; }

        /// <summary>
        /// How ownership of the firm is structured.
        /// </summary>
        public OwnershipStructure OwnershipStructure { get; set; }

        /// <summary>
        /// How Profits from the firm are distributed.
        /// </summary>
        public ProfitStructure ProfitStructure { get; set; }

        /// <summary>
        /// The firms this firm owns.
        /// </summary>
        public List<Firm> Children { get; set; }
        IReadOnlyList<IFirm> IFirm.Children => Children;

        /// <summary>
        /// The Firm that owns this firm.
        /// </summary>
        public Firm? Parent { get; set; }
        IFirm? IFirm.Parent => Parent;

        /// <summary>
        /// The Jobs the Firm oversees, how it pays them, 
        /// and at what rate it pays.
        /// </summary>
        public List<FirmJob> Jobs { get; set; }
        IReadOnlyList<IFirmJob> IFirm.Jobs => Jobs;

        /// <summary>
        /// The products that this firm tries to sell.
        /// </summary>
        public Dictionary<Product, decimal> Products { get; set; }
        IReadOnlyDictionary<IProduct, decimal> IFirm.Products 
            => Products.ToDictionary(x => (IProduct)x.Key,
                x => x.Value);

        /// <summary>
        /// What resources the Firm owns. Bought goods go here,
        /// made goods go here and are sold from here.
        /// </summary>
        public Dictionary<Product, decimal> Resources { get; set; }
        IReadOnlyDictionary<IProduct, decimal> IFirm.Resources 
            => Resources.ToDictionary(x => (IProduct)x.Key,
                x => x.Value); 

        /// <summary>
        /// The market which the Firm is centered out of.
        /// </summary>
        public Market.Market HeadQuarters { get; set; }
        IMarket IFirm.HeadQuarters => HeadQuarters;

        /// <summary>
        /// The regions where the company operates, buying, selling,
        /// and working. Must have at least one piece of property in
        /// this market to do so.
        /// </summary>
        public List<Market.Market> Regions { get; set; }
        IReadOnlyList<IMarket> IFirm.Regions => Regions;

        /// <summary>
        /// The techs available to the Firm.
        /// </summary>
        public List<(Technology.Technology tech, int research)> Techs { get; set; }
        IReadOnlyList<(ITechnology tech, int research)> IFirm.Techs => Techs
            .Select(x => ((ITechnology)x.tech, x.research)).ToList();

        #region AssistantData

        

        #endregion
    }
}
