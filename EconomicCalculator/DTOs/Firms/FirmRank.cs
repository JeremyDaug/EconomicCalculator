﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EconomicCalculator.DTOs.Firms
{
    /// <summary>
    /// What rank and by extension rough size a firm is.
    /// </summary>
    public enum FirmRank
    {
        /// <summary>
        /// Lowest rank of firm. Can have one job, and exist in one location.
        /// </summary>
        Firm,
        /// <summary>
        /// Second rank of Firm. Can have multiple jobs and exist in multiple locations.
        /// </summary>
        Company,
        /// <summary>
        /// Third rank of firm. Can own companies.
        /// </summary>
        Corporation,
        /// <summary>
        /// Highest rank of Firm, can own Corporations.
        /// </summary>
        Megacorp
    }
}
