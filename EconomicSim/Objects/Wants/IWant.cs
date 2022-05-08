using System.Text.Json.Serialization;

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
    }
}
