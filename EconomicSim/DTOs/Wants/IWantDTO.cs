using System.Text.Json.Serialization;

namespace EconomicSim.DTOs.Wants
{
    /// <summary>
    /// The Wants available to the system.
    /// </summary>
    [Obsolete]
    public interface IWantDTO
    {
        /// <summary>
        /// The Want Id
        /// </summary>
        [JsonIgnore]
        int Id { get; }

        /// <summary>
        /// The Want Name
        /// </summary>
        string Name { get; }

        /// <summary>
        /// A short description of the want and how it is should be
        /// used elsewhere.
        /// </summary>
        string Description { get; }

        /// <summary>
        /// Takes the want and creates a satisfaction string for
        /// it.
        /// </summary>
        /// <param name="d">The value to place in the "< >" </param>
        /// <returns></returns>
        string ToSatisfactionString(decimal d);
    }
}
