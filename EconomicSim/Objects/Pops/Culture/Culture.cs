using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using EconomicSim.Helpers;
using EconomicSim.Objects.Products;
using EconomicSim.Objects.Wants;

namespace EconomicSim.Objects.Pops.Culture
{
    /// <summary>
    /// Culture Data Class
    /// </summary>
    [JsonConverter(typeof(CultureJsonConverter))]
    internal class Culture : ICulture
    {
        /// <summary>
        /// The Culture's Id.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The Name of the Culture.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The Variant Name of the Culture, if it's based on another.
        /// </summary>
        public string VariantName { get; set; }

        /// <summary>
        /// The growth bonus or penalty
        /// </summary>
        public decimal GrowthModifier { get; set; }

        /// <summary>
        /// The death bonus or penalty.
        /// </summary>
        public decimal DeathModifier { get; set; }

        /// <summary>
        /// The products desired by members of the culture.
        /// </summary>
        public List<NeedDesire> Needs { get; set; }
        IReadOnlyList<INeedDesire> ICulture.Needs => Needs;

        /// <summary>
        /// Wants desired by members of the Culture.
        /// </summary>
        public List<WantDesire> Wants { get; set; }
        IReadOnlyList<IWantDesire> ICulture.Wants => Wants;

        /// <summary>
        /// The Culture's Tags.
        /// </summary>
        public List<TagData<CultureTag>> Tags { get; set; }
        IReadOnlyList<ITagData<CultureTag>> ICulture.Tags => Tags;

        public string GetName()
        {
            if (!string.IsNullOrWhiteSpace(VariantName))
                return $"{Name}({VariantName})";
            return Name;
        }
        
        public override string ToString()
        {
            if (string.IsNullOrEmpty(VariantName))
            {
                return Name;
            }
            return string.Format("{0}({1})", Name, VariantName);
        }
    }
}
