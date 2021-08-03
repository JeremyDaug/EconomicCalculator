using EconomicCalculator.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EconomicCalculator.Storage.Technology
{
    public interface ITechnology
    {
        /// <summary>
        /// The unique Id of the Technology.
        /// </summary>
        int Id { get; }

        /// <summary>
        /// The name of the technology.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// The Category of the Tech.
        /// </summary>
        TechCategory Category { get; }
    }
}
