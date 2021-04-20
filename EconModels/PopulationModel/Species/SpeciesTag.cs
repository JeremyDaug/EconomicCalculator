﻿using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EconModels.PopulationModel
{
    public class SpeciesTag
    {
        /// <summary>
        /// The Species this tag is attached to.
        /// </summary>
        [Required]
        [DisplayName("SpeciesId")]
        public int SpeciesId { get; set; }

        /// <summary>
        /// The Species Attached to.
        /// </summary>
        [ForeignKey("SpeciesId")]
        [DisplayName("Species")]
        public virtual Species Species { get; set; }

        /// <summary>
        /// The tag of the species with data inside.
        /// </summary>
        [Required, StringLength(30)]
        [DisplayName("Tag")]
        public string Tag { get; set; }
    }
}