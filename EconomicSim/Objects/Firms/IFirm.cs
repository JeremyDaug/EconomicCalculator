using System.Text.Json.Serialization;
using EconomicSim.Objects.Market;
using EconomicSim.Objects.Products;
using EconomicSim.Objects.Technology;

namespace EconomicSim.Objects.Firms
{
    /// <summary>
    /// Read Only Firm Interface
    /// </summary>
    [JsonConverter(typeof(FirmJsonConverter))]
    public interface IFirm : ICanSell
    {
        /// <summary>
        /// Id of the firm
        /// </summary>
        int Id { get; }

        /// <summary>
        /// Name of the firm.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// The rank of the firm.
        /// </summary>
        FirmRank FirmRank { get; }

        /// <summary>
        /// How ownership of the firm is structured.
        /// </summary>
        OwnershipStructure OwnershipStructure { get; }

        /// <summary>
        /// How Profits from the firm are distributed.
        /// </summary>
        ProfitStructure ProfitStructure { get; }

        /// <summary>
        /// The firms this firm owns.
        /// </summary>
        IReadOnlyList<IFirm> Children { get; }

        /// <summary>
        /// The Firm that owns this firm.
        /// </summary>
        IFirm? Parent { get; }

        /// <summary>
        /// The Jobs the Firm oversees, how it pays them, 
        /// and at what rate it pays.
        /// </summary>
        IReadOnlyList<IFirmJob> Jobs { get; }
        
        /// <summary>
        /// The products that the firm produces with intent to sell.
        /// </summary>
        IReadOnlyList<IProduct> FirmOutputs { get; }

        /// <summary>
        /// The products that this firm tries to sell.
        /// </summary>
        IReadOnlyDictionary<IProduct, decimal> Products { get; }

        /// <summary>
        /// What resources the Firm owns. Bought goods go here,
        /// made goods go here and are sold from here.
        /// </summary>
        IReadOnlyDictionary<IProduct, decimal> Resources { get; }

        /// <summary>
        /// The market which the Firm is centered out of.
        /// </summary>
        IMarket HeadQuarters { get; }

        /// <summary>
        /// The regions where the company operates, buying, selling,
        /// and working. Must have at least one piece of property in
        /// this market to do so.
        /// </summary>
        IReadOnlyList<IMarket> Regions { get; }

        /// <summary>
        /// The techs available to the Firm.
        /// </summary>
        IReadOnlyList<(ITechnology tech, int research)> Techs { get; }

        // Research stuff here.
    }
}
