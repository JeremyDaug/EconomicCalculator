using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EconModels.TerritoryModel
{
    public class TerritoryConnection
    {
        public int Id { get; set; }

        [Required, Index("UniqueCoupling", 1, IsUnique = true)]
        public int StartId { get; set; }

        public virtual Territory Start { get; set; }

        [Required, Index("UniqueCoupling", 2, IsUnique = true)]
        public int EndId { get; set; }

        public virtual Territory End { get; set; }

        // The Any special distance between the two.
        public int? Distance { get; set; }
    }
}
