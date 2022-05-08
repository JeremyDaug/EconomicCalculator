using EconomicSim.Enums;
using EconomicSim.Objects.Pops.Species;

namespace EconomicSim.DTOs.Pops.Species.AttachedTagData
{
    public interface IAttachedSpeciesTag
    {
        SpeciesTag Tag { get; }

        IList<ParameterType> TagParameterTypes { get; }

        object this[int index] { get; }

        string ToString();
    }
}
