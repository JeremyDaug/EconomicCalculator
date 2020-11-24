using System;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebInterface.Models
{
    public class Market
    {
        public int ID { get; set; }

        [StringLength(60, MinimumLength = 3)]
        public string Title { get; set; }

        [Display(Name = "Release Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime ReleaseDate { get; set; }

        [RegularExpression(@"^[A-Z]+[a-zA-Z'\s]*$")]
        [Required]
        [StringLength(30)]
        public string Genre { get; set; }

        [Range(1,100)]
        [DataType(DataType.Currency)]
        public decimal Price { get; set; }

        [Required, StringLength(5)]
        public string Rating { get; set; }
    }

    public class MarketDBContext : DbContext
    {
        public DbSet<Market> Markets { get; set; }
    }
}