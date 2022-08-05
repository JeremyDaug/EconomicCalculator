using System.Text.Json.Serialization;
using EconomicSim.Helpers;

namespace EconomicSim.Objects.Pops.Species
{
    /// <summary>
    /// Read only interface for species.
    /// </summary>
    [JsonConverter(typeof(SpeciesJsonConverter))]
    public interface ISpecies
    {
        /// <summary>
        /// Species Id
        /// </summary>
        int Id { get; }

        /// <summary>
        /// The name of the Species
        /// </summary>
        string Name { get; }

        /// <summary>
        /// The variant name of the species.
        /// </summary>
        string VariantName { get; set; }

        /// <summary>
        /// The natural growth rate of the Species.
        /// </summary>
        decimal BirthRate { get; }

        /// <summary>
        /// The natural rate of death of the species.
        /// </summary>
        decimal DeathRate { get; }

        /// <summary>
        /// The Product desires a species has by default.
        /// </summary>
        IReadOnlyList<INeedDesire> Needs { get; }

        /// <summary>
        /// The Wants desired by the species.
        /// </summary>
        IReadOnlyList<IWantDesire> Wants { get; }

        /// <summary>
        /// The tags which modify how a species acts more broadly.
        /// </summary>
        IReadOnlyList<ITagData<SpeciesTag>> Tags { get; }

        /// <summary>
        /// Related Species
        /// </summary>
        IReadOnlyList<ISpecies> Relations { get; }

        string GetName();
    }
}
