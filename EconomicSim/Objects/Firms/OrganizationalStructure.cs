using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EconomicSim.Objects.Firms
{
    /// <summary>
    /// How the company is organized, and is allowed to function.
    /// </summary>
    public enum OrganizationalStructure
    {
        /// <summary>
        /// The firm is run as a small store. Highly focused, and independent.
        /// Mostly used for for Disorganized or Associated Firms.
        /// Used almost exclusively by Firms and Companies.
        /// </summary>
        MomAndPop,

        /// <summary>
        /// The firm is organized as a Guild. Subdivisions are treated like smaller
        /// firms, and upper management often has little say in the day to day, instead
        /// they set rules and regulations for the guild, in particular prices.
        /// Mostly for Company and Firm level, but can go higher.
        /// </summary>
        Guild,

        /// <summary>
        /// Reduces the height of the organization, middle management levels are
        /// reduced in scale. Upper Management is much closer to the lower and
        /// employees are often given more independence and responsibility.
        /// Used Primarily in Companies and Firms.
        /// </summary>
        Flat,

        /// <summary>
        /// Divides the company into subcomponents focused on
        /// particular jobs or duties, like Marketing, R&D, Sales,
        /// Operations, etc
        /// Authority is centralized in the higher levels.
        /// Tends to be used by Companies and Corporations.
        /// </summary>
        Functional,

        /// <summary>
        /// Organizes itself around products, projects, and subsidiaries
        /// rather than specialized sections. Divisions are given much more
        /// independence than most.
        /// Common among Companies, Corporations, and Megacorporations.
        /// </summary>
        Divisional,

        /// <summary>
        /// The company organizes itself around a franchise, which is usually a
        /// copy-paste firm with high managerial independence, but low product 
        /// independence. 
        /// Common among Companies, Corporations, and Megacorps.
        /// </summary>
        Franchise,
    }
}
