using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EconModels.JobModels;
using EconModels.MarketModel;
using EconModels.PopulationModel;
using EconModels.ProcessModel;
using EconModels.ProductModel;
using EconModels.TerritoryModel;

namespace EconModels
{
    public class EconSimContext : DbContext
    {
        // Default Constructor, Connects to EconModelDB
        public EconSimContext() : base("EconModelDB") // TODO figure out how to attach to existing DB.
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

        // Processes
        public DbSet<Process> Processes { get; set; }
        public DbSet<ProcessInput> ProcessInputs { get; set; }
        public DbSet<ProcessOutput> ProcessOutputs { get; set; }
        public DbSet<ProcessCapital> ProcessCapitals { get; set; }

        // Jobs
        public DbSet<Job> Jobs { get; set; }

        // Cultures
        public DbSet<CultureNeeds> CultureNeeds { get; set; }
        public DbSet<Culture> Cultures { get; set; }

        // Population Groups
        public DbSet<PopulationCultureBreakdown> PopCultureBreakdowns { get; set; }
        public DbSet<PopulationGroup> PopulationGroups { get; set; }
        public DbSet<OwnedProperty> OwnedProperties { get; set; }

        // Territory
        public DbSet<Territory> Territories { get; set; }
        public DbSet<TerritoryConnection> TerritoryConnections { get; set; }
        public DbSet<LandOwner> LandOwners { get; set; }

        // Market
        public DbSet<Market> Markets { get; set; }
        public DbSet<ProductPrices> MarketPrices { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {

            Database.SetInitializer<EconSimContext>(null);
            // Base Call, no big deal, just required.
            base.OnModelCreating(modelBuilder);

            // Failure
            modelBuilder.Entity<FailsIntoPair>()
                .HasKey(x => new { x.SourceId, x.ResultId });

            modelBuilder.Entity<Product>()
                .HasMany(x => x.FailsInto)
                .WithRequired(x => x.Source)
                .HasForeignKey(x => x.SourceId)
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<Product>()
                .HasMany(x => x.MadeFromFailure)
                .WithRequired(x => x.Result)
                .HasForeignKey(x => x.ResultId)
                .WillCascadeOnDelete(false);

            // Maintenance
            modelBuilder.Entity<MaintenancePair>()
                .HasKey(x => new { x.SourceId, x.ResultId });

            modelBuilder.Entity<Product>()
                .HasMany(x => x.Maintains)
                .WithRequired(x => x.Source)
                .HasForeignKey(x => x.SourceId)
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<Product>()
                .HasMany(x => x.MaintainedBy)
                .WithRequired(x => x.Result)
                .HasForeignKey(x => x.ResultId)
                .WillCascadeOnDelete(false);

            // Process Connections
            modelBuilder.Entity<ProcessInput>()
                .HasKey(x => new { x.ProcessId, x.InputId });
            modelBuilder.Entity<ProcessOutput>()
                .HasKey(x => new { x.ProcessId, x.OutputId });
            modelBuilder.Entity<ProcessCapital>()
                .HasKey(x => new { x.ProcessId, x.CapitalId });
            // May not need, TODO test whether it's needed or not.

            // Territory Connections
            modelBuilder.Entity<Territory>()
                .HasMany(x => x.OutgoingConnections)
                .WithRequired(x => x.Start)
                .HasForeignKey(x => x.StartId)
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<Territory>()
                .HasMany(x => x.IncomingConnections)
                .WithRequired(x => x.End)
                .HasForeignKey(x => x.EndId)
                .WillCascadeOnDelete(false);
        }
    }
}
