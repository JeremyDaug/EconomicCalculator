using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EconomicSim.Objects.Processes.ProductionTags
{
    /// <summary>
    /// Tags for Process inputs, outputs, and capital.
    /// </summary>
    public enum ProductionTag
    {
        /// <summary>
        /// Optional<#>
        /// For Inputs or capital only. This is optional, and it's inclusion
        /// increases throughput for the process. # is the amount it increases
        /// per unit of input. The amount requested is the maximum it can accept
        /// at normal throughput.
        /// The amount requested does scale with other throughput increases.
        /// Optional goods use their own consumption process and do not need
        /// to be taken into account. <seealso cref="Consumed"/>
        /// </summary>
        Optional,
        /// <summary>
        /// An input that is consumed. It's consumption process should be
        /// combine with this one implicitly, rather than be destroyed and 
        /// the process take care of it's output.
        /// </summary>
        Consumed,
        /// <summary>
        /// A fixed want or product. No matter how much a process increases,
        /// only this amount will be requested for a 
        /// </summary>
        Fixed,
        /// <summary>
        /// Investment<days>. An input or capital which is claimed for the duration
        /// of a long process that requires multiple days of the process.
        /// TOOD, come back to this later.
        /// </summary>
        Investment,
        /// <summary>
        /// For output products only. This product is not put into storage, but rather
        /// automatically released into the environment.
        /// </summary>
        Pollutant,
        /// <summary>
        /// Chance<Group;Weight>, Outputs only. This product is not guaranteed, 
        /// but instead has a chance of being output by the process.
        /// Group is what output group it is part of. Weight is how likely it's group is
        /// to be output. Only one group can be output for any specific process.
        /// If outputs share groups, they come out together.
        /// </summary>
        Chance,
        /// <summary>
        /// For Output only. This product is has a negative amount and if added to another
        /// process it removes this product from the outputs.
        /// </summary>
        Offset,
        /// <summary>
        /// DivisionCapital<Level>, this is capital required by the process when division of labor
        /// is being applied. Level is the reduction level at which the capital is required.
        /// Every level beyond the required reduction multiplies the requirement by the
        /// process's division multiplier.
        /// </summary>
        DivisionCapital,
        /// <summary>
        /// DivisionLabor<Level>, this is any additional input required by division of
        /// labor activities. Level is the reduction level at which this input becomes required.
        /// Each level beyond the required reduction multiplies the requirement by the process's
        /// division multiplier.
        /// </summary>
        DivisionInput,
        AutomationCapital,
        AutomationInput
    }
}
