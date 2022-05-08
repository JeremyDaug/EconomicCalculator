using System.Text.Json.Serialization;
using EconomicSim.Objects.Firms;

namespace EconomicSim.DTOs.Firms
{
    /// <summary>
    /// A productive economic organization in our system.
    /// </summary>
    public interface IFirmDTO
    {
        #region ID

        /// <summary>
        /// The Unique Id of the Firm
        /// </summary>
        [JsonIgnore]
        int Id { get; }

        /// <summary>
        /// The name of the firm, should be unique.
        /// </summary>
        string Name { get; }

        // variant name?

        #endregion ID

        #region FirmStructure

        /// <summary>
        /// The firm which owns this firm.
        /// </summary>
        string ParentFirm { get; }

        /// <summary>
        /// The Firms which this firm owns.
        /// </summary>
        IList<string> Children { get; }

        [JsonIgnore]
        string ChildrenString { get; }

        /// <summary>
        /// The rank of the firm.
        /// </summary>
        string FirmRank { get; }

        [JsonIgnore]
        FirmRank FirmRankEnum { get; }

        /// <summary>
        /// How the ownership of the firm is split or structured.
        /// </summary>
        string OwnershipStructure { get; }

        [JsonIgnore]
        OwnershipStructure OwnershipStructureEnum { get; }

        /// <summary>
        /// How profits from the firm are distributed.
        /// </summary>
        string ProfitStructure { get; }
        
        [JsonIgnore] 
        ProfitStructure ProfitStructureEnum { get; }

        /// <summary>
        /// How the Firm is organized internally.
        /// </summary>
        string OrganizationalStructure { get; }

        [JsonIgnore]
        OrganizationalStructure OrganizationalStructureEnum { get; }

        // population groups attached.
        // populations connect to firms for sanity reasons 
        // (having to point to pops from here across multiple
        // territories when the pop groups most likely won't 
        // have nice names to point to is nightmarish)

        // board of directors / owner leaders

        // HQ Territory

        #endregion FirmStructure

        #region Production

        /// <summary>
        /// What jobs are part of this firm,
        /// and what they are payed and in what method.
        /// </summary>
        IList<IJobWageData> JobData { get; }

        [JsonIgnore]
        string JobsString { get; }

        /// <summary>
        /// What processes (stand alone or not) the Firm utilizes.
        /// </summary>
        IList<string> Processes { get; }

        [JsonIgnore]
        string ProcessesString { get; }

        [JsonIgnore]
        IList<int> Employees { get; }

        #endregion Production

        #region Market

        /// <summary>
        /// The market the firm is in or headquartered in
        /// Required.
        /// </summary>
        string Market { get; }

        [JsonIgnore]
        int MarketId { get; }

        /// <summary>
        /// Where the firm does business directly.
        /// </summary>
        IList<string> Regions { get; }

        [JsonIgnore]
        IList<int> RegionIds { get; }

        [JsonIgnore]
        string RegionString { get; }

        /// <summary>
        /// The prices the firm sells it's product at.
        /// </summary>
        IDictionary<string, decimal> ProductPrices { get; }

        [JsonIgnore]
        string ProductPricesString { get; }

        /// <summary>
        /// What resources the company has at it's disposal.
        /// Key is the product's full (unique) name.
        /// Value is how much it has stored across all of it's properties.
        /// </summary>
        IDictionary<string, decimal> Resources { get; }

        [JsonIgnore]
        string ResourcesString { get; }

        #endregion Market
    }
}
