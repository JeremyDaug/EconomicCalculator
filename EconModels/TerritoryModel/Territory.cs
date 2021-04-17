using EconModels.PopulationModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EconModels.TerritoryModel
{
    public class Territory
    {
        public Territory()
        {
            OutgoingConnections = new List<TerritoryConnection>();
            IncomingConnections = new List<TerritoryConnection>();
            Pops = new List<PopulationGroup>();
            LandOwners = new List<LandOwner>();
        }

        public int Id { get; set; }

        [Required, StringLength(40)]
        public string Name { get; set; }

        // Coordinates for within an organized group
        public int? X { get; set; }
        public int? Y { get; set; }
        public int? Z { get; set; }

        [Required]
        public decimal Extent { get; set; }

        [Required]
        public int Elevation { get; set; }

        [Required, Range(0, int.MaxValue)]
        public decimal WaterLevel { get; set; }

        [Required]
        public bool HasRiver { get; set; }

        [Required]
        public int Humidity { get; set; }

        [Required]
        public int Tempurature { get; set; }

        [Required]
        public int Roughness { get; set; }

        [Required] // TODO expand this for multiple types and kinds of Infrastructure.
        public int InfrastructureLevel { get; set; }

        // Adjacent Territories
        // These are special connections that are not generated
        public virtual ICollection<TerritoryConnection> OutgoingConnections { get; set; }

        // These are the incoming special connections.
        public virtual ICollection<TerritoryConnection> IncomingConnections { get; set; }

        // Market Connection Placeholder.

        public virtual ICollection<PopulationGroup> Pops { get; set; }

        // Governor Placeholder

        // Land unclaimed by owners.
        [Required, Range(0, int.MaxValue)]
        public decimal AvailableLand { get; set; }

        public virtual ICollection<LandOwner> LandOwners { get; set; }

        // Claimants handled by governor not territory.
    }
}
