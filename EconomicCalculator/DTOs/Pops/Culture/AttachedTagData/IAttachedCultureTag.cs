using EconomicCalculator.Enums;
using EconomicCalculator.Objects.Pops.Culture;
using System.Collections.Generic;

namespace EconomicCalculator.DTOs.Pops.Culture.AttachedTagData
{
    public interface IAttachedCultureTag
    {

        CultureTag Tag { get; }

        IList<ParameterType> TagParameterTypes { get; }

        object this[int index] { get; }

        string ToString();
    }
}