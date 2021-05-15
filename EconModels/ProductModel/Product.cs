using EconModels.Enums;
using EconModels.JobModels;
using EconModels.SkillsModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EconModels.ProductModel
{
    public class Product
    {
        public Product()
        {
            FailsInto = new List<FailsIntoPair>();
            MadeFromFailure = new List<FailsIntoPair>();
            MaintainedBy = new List<MaintenancePair>();
            Maintains = new List<MaintenancePair>();
            Jobs = new List<Job>();
            WantTags = new List<ProductWantTag>();
        }

        public int Id { get; set; }

        /// <summary>
        /// The Name of the Product
        /// </summary>
        [Required, StringLength(30, MinimumLength = 3)]
        public string Name { get; set; }

        /// <summary>
        /// The Variant Name of the Product
        /// </summary>
        [DisplayName("Variant Name")]
        [StringLength(30)]
        public string VariantName { get; set; }

        /// <summary>
        /// The Name of the Unit it is measured in.
        /// </summary>
        [DisplayName("Unit Name")]
        [Required, StringLength(15)]
        public string UnitName { get; set; }

        /// <summary>
        /// The quality of the product.
        /// </summary>
        [Required, Range(0, int.MaxValue)]
        public int Quality { get; set; }

        /// <summary>
        /// The Default starting price, if no other price is
        /// given.
        /// </summary>
        [Required]
        [DisplayName("Default Price")]
        public decimal DefaultPrice { get; set; }

        /// <summary>
        /// The mass of the Product.
        /// </summary>
        [Required, Range(0, double.MaxValue)]
        public double Mass { get; set; }

        /// <summary>
        /// The volume of the Product.
        /// </summary>
        [Required, Range(0, double.MaxValue)]
        public double Bulk { get; set; }

        /// <summary>
        /// The type of product it is.
        /// </summary>
        [Required]
        [DisplayName("Product Type")]
        public ProductTypes ProductType { get; set; }

        /// <summary>
        /// Whether the product can be maintained.
        /// </summary>
        [Required]
        public bool Maintainable { get; set; }

        /// <summary>
        /// Whether the product can be divided further than it's units.
        /// </summary>
        [Required]
        public bool Fractional { get; set; }

        /// <summary>
        /// Whether the Product, when dumped into the environment, flows outwards or
        /// stays in place.
        /// </summary>
        [Required, DefaultValue(false)]
        [DisplayName("Fluid")]
        public bool Fluid { get; set; }

        /// <summary>
        /// The average time it takes for the product to fail.
        /// </summary>
        [Required]
        [DisplayName("Mean Time To Failure")]
        public int MeanTimeToFailure { get; set; }

        // Navigation Properties
        /// <summary>
        /// Products it fails into.
        /// </summary>
        public virtual ICollection<FailsIntoPair> FailsInto { get; set; }

        /// <summary>
        /// WHat products fail into it.
        /// </summary>
        public virtual ICollection<FailsIntoPair> MadeFromFailure { get; set; }

        /// <summary>
        /// What the product is maintained by.
        /// </summary>
        public virtual ICollection<MaintenancePair> MaintainedBy { get; set; }

        /// <summary>
        /// What this product maintains.
        /// </summary>
        public virtual ICollection<MaintenancePair> Maintains { get; set; }

        /// <summary>
        /// What wants the product can satisfy when owned or consumed.
        /// </summary>
        [DisplayName("Want Tags")]
        public virtual ICollection<ProductWantTag> WantTags { get; set; }

        // For jobs, because one way connections are just not in the cards.
        // Job.Labor
        /// <summary>
        /// The Jobs related to this product.
        /// </summary>
        public virtual ICollection<Job> Jobs { get; set; }

        // To Skill.ValidLabors
        /// <summary>
        /// The Skills related to this Product.
        /// </summary>
        public virtual ICollection<Skill> Skills { get; set; }

        /// <summary>
        /// Adds want tags to the product.
        /// </summary>
        /// <param name="tag">The tag to Add.</param>
        /// <returns>The created want tag.</returns>
        public ProductWantTag AddWantTag(string tag)
        {
            var want = new ProductWantTag
            {
                Product = this,
                ProductId = Id,
                Tag = tag
            };
            WantTags.Add(want);
            return want;
        }
    }
}
