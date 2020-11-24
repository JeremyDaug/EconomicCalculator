using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EconModels.ProductModel;

namespace EconModels
{
    public class EconSimContext : DbContext
    {
        // Default Constructor, Connects to EconModelDB
        public EconSimContext() : base("EconModelConnectionString")
        {

        }

        public EconSimContext(string connectionString) : base(connectionString)
        {
            Database.SetInitializer<EconSimContext>(null);
        }

        // Products
        public DbSet<Product> Products { get; set; }
        public DbSet<FailsIntoPair> FailurePairs { get; set; }
        public DbSet<MaintenancePair> MaintenancePairs { get; set; }
    }
}
