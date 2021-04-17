using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EconModels.PopulationModel
{
    /// <summary>
    /// Placeholder class, just holds existant religions
    /// doesn't do much compared to other things directly.
    /// </summary>
    public class Religion
    {
        public int Id { get; set; }

        [Required, StringLength(30, MinimumLength = 3)]
        public string Name { get; set; }

        [Required, StringLength(30)]
        public string Sect { get; set; }

        // holy city / holy site placeholder
    }
}
