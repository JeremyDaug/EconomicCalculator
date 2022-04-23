using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using EconomicSim.Objects.Firms;

namespace EconomicSim.DTOs.Firms
{
    /// <summary>
    /// A productive economic organization in our system.
    /// </summary>
    public class FirmDTO : IFirmDTO
    {
        public FirmDTO()
        {
            Children = new List<string>();
            JobData = new List<IJobWageData>();
            Processes = new List<string>();
            Employees = new List<int>();
            Regions = new List<string>();
            RegionIds = new List<int>();
            ProductPrices = new Dictionary<string, decimal>();
            Resources = new Dictionary<string, decimal>();
        }

        #region ID

        /// <summary>
        /// The Unique Id of the Firm
        /// </summary>
        [JsonIgnore]
        public int Id { get; set; }

        /// <summary>
        /// The name of the firm, should be unique.
        /// </summary>
        public string Name { get; set; }

        // variant name?

        #endregion ID

        #region FirmStructure

        /// <summary>
        /// The firm which owns this firm.
        /// </summary>
        public string ParentFirm { get; set; }

        /// <summary>
        /// The Firms which this firm owns.
        /// </summary>
        public IList<string> Children { get; set; }

        [JsonIgnore]
        public string ChildrenString 
        {
            get
            {
                var result = "";
                foreach (var child in Children)
                    result += child + "\n";
                return result;
            }
        }

        /// <summary>
        /// The rank of the firm.
        /// </summary>
        public string FirmRank
        {
            get { return FirmRankEnum.ToString(); }
            set
            {
                FirmRankEnum = (FirmRank)Enum.Parse(typeof(FirmRank), value);
            }
        }
        
        [JsonIgnore] 
        public FirmRank FirmRankEnum { get; set; }

        /// <summary>
        /// How the ownership of the firm is split or structured.
        /// </summary>
        public string OwnershipStructure
        {
            get { return OwnershipStructureEnum.ToString(); }
            set
            {
                OwnershipStructureEnum = (OwnershipStructure)Enum.Parse(typeof(OwnershipStructure), value);
            }
        }

        [JsonIgnore]
        public OwnershipStructure OwnershipStructureEnum { get; set; }

        /// <summary>
        /// How profits from the firm are distributed.
        /// </summary>
        public string ProfitStructure
        {
            get { return ProfitStructureEnum.ToString(); }
            set
            {
                ProfitStructureEnum = (ProfitStructure)Enum.Parse(typeof(ProfitStructure), value);
            }
        }

        [JsonIgnore]
        public ProfitStructure ProfitStructureEnum { get; set; }

        /// <summary>
        /// How the Firm is organized internally.
        /// </summary>
        public string OrganizationalStructure 
        {
            get { return OrganizationalStructureEnum.ToString(); }
            set 
            {
                OrganizationalStructureEnum
                     = (OrganizationalStructure)Enum.Parse(typeof(OrganizationalStructure), value);
            }
        }

        [JsonIgnore]
        public OrganizationalStructure OrganizationalStructureEnum { get; set; }

        // population groups attached.
        // populations connect to firms for sanity reasons 
        // (having to point to pops from here across multiple
        // territories when the pop groups most likely won't 
        // have nice names to point to is nightmarish)

        // board of directors / owner leaders

        #endregion FirmStructure

        #region Production

        /// <summary>
        /// What jobs are part of this firm,
        /// and what they are payed and in what method.
        /// </summary>
        public IList<IJobWageData> JobData { get; set; }

        [JsonIgnore]
        public string JobsString 
        {
            get
            {
                var result = "";
                foreach (var job in JobData)
                    result += job.ToString() + "\n";
                return result;
            }
        }

        /// <summary>
        /// What processes (stand alone or not) the Firm utilizes.
        /// </summary>
        public IList<string> Processes { get; set; }

        [JsonIgnore]
        public string ProcessesString 
        {
            get
            {
                var result = "";
                foreach (var proc in Processes)
                    result += proc + "\n";
                return result;
            }
        }

        [JsonIgnore]
        public IList<int> Employees { get; set; }

        #endregion Production

        #region Market

        /// <summary>
        /// The market the firm is in or headquartered in
        /// Required.
        /// </summary>
        public string Market { get; set; }

        [JsonIgnore]
        public int MarketId { get; set; }

        /// <summary>
        /// Where the firm does business directly.
        /// </summary>
        public IList<string> Regions { get; set; }

        [JsonIgnore]
        public IList<int> RegionIds { get; set; }

        [JsonIgnore]
        public string RegionString 
        {
            get
            {
                var result = "";
                foreach (var region in Regions)
                    result += region + "\n";
                return result;
            }
        }

        /// <summary>
        /// The prices the firm sells it's product at.
        /// </summary>
        public IDictionary<string, decimal> ProductPrices { get; set; }

        [JsonIgnore]
        public string ProductPricesString 
        {
            get
            {
                var result = "";
                foreach (var prodPrice in ProductPrices)
                    result += prodPrice.Key + " : " + prodPrice.Value + "\n";
                return result;
            }
        }

        /// <summary>
        /// What resources the company has at it's disposal.
        /// Key is the product's full (unique) name.
        /// Value is how much it has stored across all of it's properties.
        /// </summary>
        public IDictionary<string, decimal> Resources { get; set; }

        [JsonIgnore]
        public string ResourcesString
        {
            get
            {
                var result = "";
                foreach (var resource in Resources)
                    result += resource.Key + ": " + resource.Value + "\n";
                return result;
            }
        }

        #endregion Market
    }
}
