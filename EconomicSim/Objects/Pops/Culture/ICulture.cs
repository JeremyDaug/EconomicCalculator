using System.Text.Json.Serialization;
using EconomicSim.Helpers;

namespace EconomicSim.Objects.Pops.Culture
{
    /// <summary>
    /// Read Only Culture Interface
    /// </summary>
    [JsonConverter(typeof(CultureJsonConverter))]
    public interface ICulture
    {
        /// <summary>
        /// The Id of the Culture
        /// </summary>
        int Id { get; }

        /// <summary>
        /// The name of the Culture
        /// </summary>
        string Name { get; }

        /// <summary>
        /// The variant name for a subculture.
        /// </summary>
        string VariantName { get; }

        /// <summary>
        /// Additional Growth rate from the Culture.
        /// </summary>
        decimal GrowthModifier { get; }

        /// <summary>
        /// Additional Death rate from Culture.
        /// </summary>
        decimal DeathModifier { get; }

        /// <summary>
        /// The products desired by this culture.
        /// </summary>
        IReadOnlyList<INeedDesire> Needs { get; }

        /// <summary>
        /// The wants desired by this culture.
        /// </summary>
        IReadOnlyList<IWantDesire> Wants { get; }

        /// <summary>
        /// The Culture's Tags.
        /// </summary>
        IReadOnlyList<ITagData<CultureTag>> Tags { get; }

        string GetName();
    }
}
