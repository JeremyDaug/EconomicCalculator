using EconModels.JobModels;
using EconModels.TerritoryModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EconModels.PopulationModel
{
    public class PopulationGroup
    {
        public PopulationGroup()
        {
            CultureBreakdown = new List<CultureBreakdown>();
            PoliticalBreakdown = new List<PoliticalBreakdown>();
            SpeciesBreakdown = new List<SpeciesBreakdown>();
            OwnedProperties = new List<OwnedProperty>();
        }

        public int Id { get; set; }

        // May remove this
        [StringLength(30)]
        [DisplayName("Name")]
        public string Name { get; set; }

        [Required]
        public int TerritoryId { get; set; }

        [ForeignKey("TerritoryId")]
        [DisplayName("Territory")]
        public virtual Territory Territory { get; set; }

        // The Total Population Count, should be equal to the sum of the culture breakdown.
        [Required, Range(0, double.MaxValue)]
        [DisplayFormat(DataFormatString = "{0:0}")]
        public decimal Infants { get; set; }

        [Required, Range(0, double.MaxValue)]
        [DisplayFormat(DataFormatString = "{0:0}")]
        public decimal Children { get; set; }

        [Required, Range(0, double.MaxValue)]
        [DisplayFormat(DataFormatString = "{0:0}")]
        public decimal Adults { get; set; }

        [Required, Range(0, double.MaxValue)]
        [DisplayFormat(DataFormatString = "{0:0}")]
        public decimal Seniors { get; set; }

        // Growth Rate depends on species and culture
        // Wants/needs depend on Species and culture.

        // Skill level applies to the skill of the culture's primary job.

        [Required, Range(0, int.MaxValue)]
        public int SkillLevel { get; set; }

        [Required]
        public int PrimaryJobId { get; set; }

        [ForeignKey("PrimaryJobId")]
        [DisplayName("Primary Job")]
        public virtual Job PrimaryJob { get; set; }

        [Required, Range(0, int.MaxValue)]
        public int Priority { get; set; }

        // Culture Breakdown Table
        public virtual ICollection<CultureBreakdown> CultureBreakdown { get; set; }

        // Species Breakdown Table
        public virtual ICollection<SpeciesBreakdown> SpeciesBreakdown { get; set; }

        // Political Breakdown Table
        public virtual ICollection<PoliticalBreakdown> PoliticalBreakdown { get; set; }

        // Population Group Property, may go over storage limits later,
        public virtual ICollection<OwnedProperty>  OwnedProperties { get; set; }

        /// <summary>
        /// Sets population with these percents.
        /// </summary>
        /// <param name="count">The total Population.</param>
        /// <param name="infant">Percent that are infants.</param>
        /// <param name="children">Percent that are Children.</param>
        /// <param name="seniors">Percent that are Seniors.</param>
        public void SetPopulation(int count, decimal infant, decimal children, decimal seniors)
        {
            Infants = count * infant;
            Children = count * children;
            Seniors = count * seniors;
            Adults = count - Infants - Children - Seniors;
        }

        private double PolPercent()
        {
            return PoliticalBreakdown.Sum(x => x.Percent);
        }

        private double CulPercent()
        {
            return CultureBreakdown.Sum(x => x.Percent);
        }

        private double SpePercent()
        {
            return SpeciesBreakdown.Sum(x => x.Percent);
        }

        /// <summary>
        /// Sets the percent of a party in the population group equal to the desired value.
        /// It will take shares from all others proportionally.
        /// </summary>
        /// <param name="pol">The political group we are setting.</param>
        /// <param name="percent">The percent we are setting it to.</param>
        public void SetPartyPercent(PoliticalGroup pol, double percent)
        {
            if (percent <= 0 || percent > 1)
                throw new ArgumentOutOfRangeException("Percent must be greater than 0 or less than or equal to 1.");

            if (!PoliticalBreakdown.Any(x => x.PoliticalGroup.Id == pol.Id))
            {
                PoliticalBreakdown.Add(new PoliticalBreakdown
                {
                    ParentId = Id,
                    Percent = 0,
                    PoliticalGroupId = pol.Id
                });
            }

            double oldPercent = PoliticalBreakdown
                .Single(x => x.PoliticalGroupId == pol.Id)
                .Percent;

            double add = (oldPercent - percent) / (percent - 1);

             PoliticalBreakdown
                .Single(x => x.PoliticalGroupId == pol.Id)
                .Percent += add;

            var newSum = PolPercent();

            foreach (var group in PoliticalBreakdown)
            {
                group.Percent = group.Percent / newSum;
            }
        }

        public void ShiftPartyPercent(PoliticalGroup pol, double percent)
        {
            if (!PoliticalBreakdown.Any(x => x.PoliticalGroup == pol))
            {
                PoliticalBreakdown.Add(new PoliticalBreakdown
                {
                    ParentId = Id,
                    Percent = 0,
                    PoliticalGroupId = pol.Id
                });
            }

            PoliticalBreakdown.Single(x => x.PoliticalGroupId == pol.Id)
                .Percent += percent;

            var newSum = PolPercent();

            foreach (var group in PoliticalBreakdown)
            {
                group.Percent = group.Percent / newSum;
            }
        }

        public void NormalizeCultures()
        {
            var newSum = CulPercent();

            foreach (var part in CultureBreakdown)
            {
                part.Percent = part.Percent / newSum;
            }
        }

        public void NormalizePolitics()
        {
            var newSum = PolPercent();

            foreach (var part in PoliticalBreakdown)
            {
                part.Percent = part.Percent / newSum;
            }
        }

        public void NormalizeSpecies()
        {
            var newSum = SpePercent();

            foreach (var part in SpeciesBreakdown)
            {
                part.Percent = part.Percent / newSum;
            }
        }

        /// <summary>
        /// Sets the percent of a species in the population group equal to the desired value.
        /// It will take shares from all others proportionally.
        /// </summary>
        /// <param name="spe">The political group we are setting.</param>
        /// <param name="percent">The percent we are setting it to.</param>
        public void SetSpeciesPercent(Species spe, double percent)
        {
            if (percent <= 0 || percent > 1)
                throw new ArgumentOutOfRangeException("Percent must be greater than 0 or less than or equal to 1.");

            if (!SpeciesBreakdown.Any(x => x.SpeciesId == spe.Id))
            {
                SpeciesBreakdown.Add(new SpeciesBreakdown
                {
                    ParentId = Id,
                    Percent = 0,
                    SpeciesId = spe.Id
                });
            }

            double oldPercent = SpeciesBreakdown
                .Single(x => x.SpeciesId == spe.Id)
                .Percent;

            double add = (oldPercent - percent) / (percent - 1);

            SpeciesBreakdown
               .Single(x => x.SpeciesId == spe.Id)
               .Percent += add;

            var newSum = SpePercent();

            foreach (var group in SpeciesBreakdown)
            {
                group.Percent = group.Percent / newSum;
            }
        }

        public void ShiftSpeciesPercent(Species spe, double percent)
        {
            if (!SpeciesBreakdown.Any(x => x.SpeciesId == spe.Id))
            {
                SpeciesBreakdown.Add(new SpeciesBreakdown
                {
                    ParentId = Id,
                    Percent = 0,
                    SpeciesId = spe.Id
                });
            }

            SpeciesBreakdown.Single(x => x.SpeciesId == spe.Id)
                .Percent += percent;

            var newSum = SpePercent();

            foreach (var part in SpeciesBreakdown)
            {
                part.Percent = part.Percent / newSum;
            }
        }

        /// <summary>
        /// Sets the percent of a culture in the population group equal to the desired value.
        /// It will take shares from all others proportionally.
        /// </summary>
        /// <param name="cult">The political group we are setting.</param>
        /// <param name="percent">The percent we are setting it to.</param>
        public void SetCulturePercent(Culture cult, double percent)
        {
            if (percent <= 0 || percent > 1)
                throw new ArgumentOutOfRangeException("Percent must be greater than 0 or less than or equal to 1.");

            if (!CultureBreakdown.Any(x => x.CultureId == cult.Id))
            {
                CultureBreakdown.Add(new CultureBreakdown
                {
                    ParentId = Id,
                    Percent = 0,
                    CultureId = cult.Id
                });
            }

            double oldPercent = CultureBreakdown
                .Single(x => x.CultureId == cult.Id)
                .Percent;

            double add = (oldPercent - percent) / (percent - 1);

            CultureBreakdown
               .Single(x => x.CultureId == cult.Id)
               .Percent += add;

            var newSum = CulPercent();

            foreach (var group in CultureBreakdown)
            {
                group.Percent = group.Percent / newSum;
            }
        }

        public void ShiftCulturePercent(Culture cult, double percent)
        {
            if (!CultureBreakdown.Any(x => x.CultureId == cult.Id))
            {
                CultureBreakdown.Add(new CultureBreakdown
                {
                    ParentId = Id,
                    Percent = 0,
                    CultureId = cult.Id
                });
            }

            CultureBreakdown.Single(x => x.CultureId == cult.Id)
                .Percent += percent;

            var newSum = CulPercent();

            foreach (var cul in CultureBreakdown)
            {
                cul.Percent = cul.Percent / newSum;
            }
        }
    }
}
