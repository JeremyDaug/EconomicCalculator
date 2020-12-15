using EconModels.PopulationModel;
using System.ComponentModel.DataAnnotations;

namespace EconModels.TerritoryModel
{
    public class LandOwner
    {
        public int Id { get; set; }

        // Where the land is
        [Required]
        public Territory Territory { get; set; }

        // The Pop which owns the land
        [Required]
        public PopulationGroup Owner { get; set; }

        // how much they own.
        [Required]
        public decimal Amount { get; set; }
    }
}