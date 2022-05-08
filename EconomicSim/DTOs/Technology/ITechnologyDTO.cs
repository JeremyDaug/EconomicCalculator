using System.Text.Json.Serialization;
using EconomicSim.Enums;

namespace EconomicSim.DTOs.Technology
{
    [Obsolete]
    public interface ITechnologyDTO
    {
        /// <summary>
        /// The tech's Id.
        /// </summary>
        [JsonIgnore]
        int Id { get; }

        /// <summary>
        /// The name of the Technology
        /// </summary>
        string Name { get; }

        /// <summary>
        /// The category of the Technology.
        /// </summary>
        [JsonIgnore]
        TechCategory CategoryEnum { get; }

        string Category { get; }

        int TechBaseCost { get; }

        int Tier { get; }

        string Description { get; }

        [JsonIgnore]
        List<int> FamilyIds { get; }

        List<string> Families { get; }

        [JsonIgnore]
        string FamilyString { get; }

        [JsonIgnore]
        List<int> ChildrenIds { get; }

        List<string> Children { get; }

        [JsonIgnore]
        string ChildrenString { get; }

        [JsonIgnore]
        List<int> ParentIds { get; }

        List<string> Parents { get; }

        [JsonIgnore]
        string ParentsString { get; }
    }
}
