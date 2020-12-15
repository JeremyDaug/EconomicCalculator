using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EconModels.MarketModel
{
    public class Market
    {
        public int Id { get; set; }

        [Required, StringLength(40, MinimumLength = 3)]
        public string Name { get; set; }

        // Total Population, 
        // Currently not stored here, instead created from the sum of population Groups.

        // Territory Placeholder

    }
}
