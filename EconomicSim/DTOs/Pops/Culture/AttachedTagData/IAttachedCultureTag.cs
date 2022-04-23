using System.Collections.Generic;
using EconomicSim.Enums;
using EconomicSim.Objects.Pops.Culture;

namespace EconomicSim.DTOs.Pops.Culture.AttachedTagData
{
    public interface IAttachedCultureTag
    {

        CultureTag Tag { get; }

        IList<ParameterType> TagParameterTypes { get; }

        object this[int index] { get; }

        string ToString();
    }
}