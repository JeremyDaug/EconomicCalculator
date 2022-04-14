using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EconomicCalculator.Objects.Wants
{
    /// <summary>
    /// A generic want, that can be satisfied by various things.
    /// </summary>
    internal class Want : IWant
    {
        /// <summary>
        /// THe Id of the want.
        /// </summary>
        public int Id { get; set; }
        int IWant.Id => Id;

        /// <summary>
        /// Want's name.
        /// </summary>
        public string Name { get; set; }
        string IWant.Name => Name;

        /// <summary>
        /// A short explanation of what the want is for.
        /// </summary>
        public string Description { get; set; }
        string IWant.Description => Description;

        public override string ToString()
        {
            return Name;
        }
    }
}
