using System.Text.Json.Serialization;

namespace EconomicSim.Objects.Wants
{
    /// <summary>
    /// A generic want, that can be satisfied by various things.
    /// </summary>
    public class Want : IWant
    {
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

        public override string ToString()
        {
            return Name;
        }
    }
}
