using EconomicCalculator.Enums;
using EconomicCalculator.Objects.Pops.Species;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EconomicCalculator.DTOs.Pops.Species.AttachedTagData
{
    public interface IAttachedSpeciesTag
    {
        SpeciesTag Tag { get; }

        IList<ParameterType> TagParameterTypes { get; }

        object this[int index] { get; }

        string ToString();
    }
}
