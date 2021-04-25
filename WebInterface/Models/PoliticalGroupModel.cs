using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebInterface.Models
{
    public class PoliticalGroupModel
    {
        public int Id { get; set; }

        [Required, StringLength(30)]
        public string Name { get; set; }
        [StringLength(30)]
        public string VariantName { get; set; }
        [Required, Range(0,5)]
        public double Radicalism { get; set; }
        [Required, Range(-1, 1)]
        public double Nationalism { get; set; }
        [Required, Range(-1, 1)]
        public double Centralization { get; set; }
        [Required, Range(-1, 1)]
        public double Authority { get; set; }
        [Required, Range(-1, 1)]
        public double Planning { get; set; }
        [Required, Range(-1, 1)]
        public double Militarism { get; set; }

        // allies
        public int[] SelectedAllyIds { get; set; }
        public IEnumerable<SelectListItem> AllyList { get; set; }

        // enemies
        public int[] SelectedEnemyIds { get; set; }
        public IEnumerable<SelectListItem> EnemyList { get; set; }
    }
}