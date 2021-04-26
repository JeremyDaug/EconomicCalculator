﻿using EconModels.JobModels;
using EconModels.TerritoryModel;
using System;
using System.Collections.Generic;
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
            SpeciesBreakdown = new List<SpeciesBreakdown>();        }

        public int Id { get; set; }

        // May remove this
        [StringLength(30)]
        public string Name { get; set; }

        // Todo, Make Required
        public int TerritoryId { get; set; }

        // Todo, Make Required
        [ForeignKey("TerritoryId")]
        public virtual Territory Territory { get; set; }

        // The Total Population Count, should be equal to the sum of the culture breakdown.
        [Required, Range(0, double.MaxValue)]
        public decimal Infants { get; set; }

        [Required, Range(0, double.MaxValue)]
        public decimal Children { get; set; }

        [Required, Range(0, double.MaxValue)]
        public decimal Adults { get; set; }

        [Required, Range(0, double.MaxValue)]
        public decimal Seniors { get; set; }

        // Growth Rate depends on species and culture
        // Wants/needs depend on Species and culture.

        // Skill level applies to the skill of the culture's primary job.

        [Required, Range(0, int.MaxValue)]
        public int SkillLevel { get; set; }

        [Required]
        public int PrimaryJobId { get; set; }

        [Required, ForeignKey("PrimaryJobId")]
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

        public void ShiftPartyPercent(PoliticalGroup pol, double percent)
        {
            if (PoliticalBreakdown.Any(x => x.PoliticalGroup == pol))
            {
                PoliticalBreakdown.Add(new PoliticalBreakdown
                {
                    ParentId = Id,
                    Parent = this,
                    Percent = 0,
                    PoliticalGroup = pol,
                    PoliticalGroupId = pol.Id
                });
            }

            PoliticalBreakdown.Single(x => x.PoliticalGroup == pol)
                .Percent += percent;

            var newSum = PolPercent();

            foreach (var group in PoliticalBreakdown)
            {
                group.Percent = group.Percent / newSum;
            }
        }
    }
}
