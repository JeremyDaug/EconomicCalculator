using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EconomicSim.Objects.Processes;
using EconomicSim.Objects.Products;
using EconomicSim.Objects.Skills;

namespace EconomicSim.Objects.Jobs
{
    /// <summary>
    /// Job Data Class
    /// </summary>
    internal class Job : IJob
    {
        public List<IProcess> _processes;

        public Job()
        {
            _processes = new List<IProcess>();
        }

        /// <summary>
        /// Job Id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Name of the job.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The Variant Name of a job, should be unique with Primary name.
        /// </summary>
        public string VariantName { get; set; }

        /// <summary>
        /// The primary Labor Product of the job.
        /// </summary>
        public IProduct Labor { get; set; }

        /// <summary>
        /// The skill the job uses.
        /// </summary>
        public ISkill Skill { get; set; }

        /// <summary>
        /// The PRocesses that are done by the job.
        /// </summary>
        public IReadOnlyList<IProcess> Processes { get => _processes; }
    }
}
