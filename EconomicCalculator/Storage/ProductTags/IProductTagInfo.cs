﻿using EconomicCalculator.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace EconomicCalculator.Storage.ProductTags
{
    /// <summary>
    /// A tag for a product and it's assocciated information.
    /// </summary>
    public interface IProductTagInfo
    {
        /// <summary>
        /// The Id of the tag.
        /// </summary>
        int Id { get; }

        /// <summary>
        /// The Tag name and text.
        /// </summary>
        string Tag { get; }

        /// <summary>
        /// What params does this tag expect.
        /// </summary>
        List<ParameterType> Params { get; }

        /// <summary>
        /// The number of Params expected
        /// </summary>
        [JsonIgnore]
        int ParamCount { get; }

        /// <summary>
        /// The Regex Pattern of the tag.
        /// </summary>
        [JsonIgnore]
        string RegexPattern { get; }

        /// <summary>
        /// The Description for the tag and how it's parameters are used.
        /// </summary>
        string Description { get; }
    }
}
