using EconModels.ProductModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EconModels.TerritoryModel
{
    public class InfrastructureRequirements
    {
        /// <summary>
        /// The ID of the Row.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The Id of the product for infrastructure.
        /// </summary>
        public int ProductId { get; set; }

        /// <summary>
        /// The product which represents the infrastructure.
        /// </summary>
        public virtual Product Product { get; set; }

        /// <summary>
        /// How many units per km^2 it must be to meet this
        /// condition.
        /// </summary>
        public decimal Density { get; set; }

        /// <summary>
        /// What this density grants. Tag should stack with lower
        /// density tags.
        /// </summary>
        public string Tag { get; set; }

        /// <summary>
        /// Whether the Infrastructure requirement is a discrete or
        /// continuous feature.
        /// Discrete move in discrete steps
        /// Continuous update in fractional steps.
        /// </summary>
        public bool IsDiscrete { get; set; }
    }
}
