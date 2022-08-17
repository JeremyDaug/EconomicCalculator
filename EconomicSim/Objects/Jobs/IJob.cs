using System.Text.Json.Serialization;
using EconomicSim.Objects.Processes;
using EconomicSim.Objects.Products;
using EconomicSim.Objects.Skills;
using EconomicSim.Objects.Wants;

namespace EconomicSim.Objects.Jobs
{
    /// <summary>
    /// Read Only Job Interface.
    /// </summary>
    [JsonConverter(typeof(JobJsonConverter))]
    public interface IJob
    {
        /// <summary>
        /// Job Id.
        /// </summary>
        int Id { get; }

        /// <summary>
        /// Name of the Job.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Variant name for the job, must be unique when combined
        /// with primary name.
        /// </summary>
        string VariantName { get; }

        /// <summary>
        /// The primary labor the job uses.
        /// </summary>
        IProduct Labor { get; }

        /// <summary>
        /// The Skill the job primarily uses.
        /// </summary>
        ISkill Skill { get; }

        /// <summary>
        /// The Processes that are part of this job.
        /// </summary>
        IReadOnlyList<IProcess> Processes { get; }

        IReadOnlyDictionary<Product, decimal> InputProducts { get; }
        IReadOnlyDictionary<Product, decimal> OptionalInputProducts { get; }
        IReadOnlyDictionary<Product, decimal> CapitalProducts { get; }
        IReadOnlyDictionary<Product, decimal> OptionalCapitalProducts { get; }
        IReadOnlyDictionary<Product, decimal> OutputProducts { get; }
        
        IReadOnlyDictionary<Want, decimal> InputWants { get; }
        IReadOnlyDictionary<Want, decimal> OptionalInputWants { get; }
        IReadOnlyDictionary<Want, decimal> CapitalWants { get; }
        IReadOnlyDictionary<Want, decimal> OptionalCapitalWants { get; }
        IReadOnlyDictionary<Want, decimal> OutputWants { get; }

        void Refresh();

        string GetName();
    }
}
