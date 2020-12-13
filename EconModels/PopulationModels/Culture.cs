﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EconModels.PopulationModels
{
    public class Culture
    {
        public Culture()
        {
            CultureNeeds = new List<CultureNeeds>();
        }

        public int Id { get; set; }

        public string Name { get; set; }

        [Required, Range(-1, 1)]
        public double CultureGrowthRate { get; set; }

        public virtual ICollection<CultureNeeds> CultureNeeds { get; set; }
    }
}
