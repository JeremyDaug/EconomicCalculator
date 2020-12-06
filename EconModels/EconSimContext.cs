using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EconModels.JobModels;
using EconModels.ProcessModel;
using EconModels.ProductModel;

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

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            // Base Call, no big deal, just required.
            base.OnModelCreating(modelBuilder);

            // Failure Source
            modelBuilder.Entity<FailsIntoPair>()
                .HasRequired(x => x.Source)
                .WithMany(x => x.FailsInto)
                .HasForeignKey(x => x.SourceId)
                .WillCascadeOnDelete(false);

            // Failure Result
            modelBuilder.Entity<FailsIntoPair>()
                .HasRequired(x => x.Result)
                .WithMany(x => x.MadeFromFailure)
                .HasForeignKey(x => x.ResultId)
                .WillCascadeOnDelete(false);

            // Maintenance Source
            modelBuilder.Entity<MaintenancePair>()
                .HasRequired(x => x.Source)
                .WithMany(x => x.Maintenance)
                .HasForeignKey(x => x.SourceId)
                .WillCascadeOnDelete(false);

            // maintenance Result
            modelBuilder.Entity<MaintenancePair>()
                .HasRequired(x => x.Result)
                .WithMany(x => x.Maintains)
                .HasForeignKey(x => x.ResultId)
                .WillCascadeOnDelete(false);
        }
    }
}
