using System.Text.Json.Serialization;
using EconomicSim.Objects.Processes;
using EconomicSim.Objects.Products;

namespace EconomicSim.Objects.Wants
{
    /// <summary>
    /// Want Read Only Interface
    /// </summary>
    public interface IWant
    {
        /// <summary>
        /// The ID of the want.
        /// </summary>
        [JsonIgnore] 
        int Id { get; }

        /// <summary>
        /// The Name of the Want
        /// </summary>
        string Name { get; }

        /// <summary>
        /// A short explanation of the want and how it should be used.
        /// </summary>
        string Description { get; }
        
        /// <summary>
        /// Products which this want can be gotten from if used.
        /// </summary>
        [JsonIgnore]
        ISet<IProduct> UseSources { get; }
        
        /// <summary>
        /// Products which this want can be gotten from if consumed.
        /// </summary>
        [JsonIgnore]
        ISet<IProduct> ConsumptionSources { get; }
        
        /// <summary>
        /// Products which this want can be gotten from if Owned.
        /// </summary>
        [JsonIgnore]
        ISet<IProduct> OwnershipSources { get; }
        
        /// <summary>
        /// The processes which produce this Wont (includes Use and Consumption Processes).
        /// </summary>
        [JsonIgnore]
        ISet<IProcess> ProcessSources { get; }
    }
}
