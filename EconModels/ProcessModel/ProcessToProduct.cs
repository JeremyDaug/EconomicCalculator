using EconModels.ProductModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EconModels.ProcessModel
{
    public class ProcessToProduct
    {
        public int Id { get; set; }

        [Required, Index("UniqueCoupling", 1, IsUnique = true)]
        public int ParentId { get; set; }

        public virtual Process Parent { get; set; }

        [Required, Index("UniqueCoupling", 2, IsUnique = true)]
        public int ProductId { get; set; }

        public virtual Product Product { get; set; }

        [Required]
        public double Amount { get; set; }
    }

    public class ProcessInput : ProcessToProduct { }

    public class ProcessOutput : ProcessToProduct { }

    public class ProcessCapital : ProcessToProduct { }
}
