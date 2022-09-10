using System.Text.Json.Serialization;
using EconomicSim.Objects.Processes;
using EconomicSim.Objects.Products;

namespace EconomicSim.Objects.Wants
{
    /// <summary>
    /// A generic want, that can be satisfied by various things.
    /// </summary>
    public class Want : IWant
    {
        public Want()
        {
            Name = "";
            Description = "";
            UseSources = new List<IProduct>();
            ConsumptionSources = new List<IProduct>();
            OwnershipSources = new List<IProduct>();
            ProcessSources = new List<IProcess>();
        }

        public Want(IWant copy)
        {
            Name = copy.Name;
            Description = copy.Description;
            UseSources = new List<IProduct>();
            ConsumptionSources = new List<IProduct>();
            OwnershipSources = new List<IProduct>();
            ProcessSources = new List<IProcess>();
        }
        
        /// <summary>
        /// The Id of the want.
        /// </summary>
        [JsonIgnore]
        public int Id { get; set; }
        int IWant.Id => Id;

        /// <summary>
        /// Want's name.
        /// </summary>
        public string Name { get; set; } = "NULL";
        string IWant.Name => Name;

        /// <summary>
        /// A short explanation of what the want is for.
        /// </summary>
        public string Description { get; set; } = "NULL";
        string IWant.Description => Description;
        
        /// <summary>
        /// Products which this want can be gotten from if used.
        /// </summary>
        [JsonIgnore]
        public IList<IProduct> UseSources { get; }
        
        /// <summary>
        /// Products which this want can be gotten from if consumed.
        /// </summary>
        [JsonIgnore]
        public IList<IProduct> ConsumptionSources { get; }
        
        /// <summary>
        /// Products which this want can be gotten from if Owned.
        /// </summary>
        [JsonIgnore]
        public IList<IProduct> OwnershipSources { get; }
        
        /// <summary>
        /// The processes which produce this Wont (includes Use and Consumption Processes).
        /// </summary>
        [JsonIgnore]
        public IList<IProcess> ProcessSources { get; }

        public override string ToString()
        {
            return Name;
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as Product);
        }

        public bool Equals(Product? obj)
        {
            if (obj == null) return false;
            return string.Equals(Name, obj.Name);
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }
    }
}
