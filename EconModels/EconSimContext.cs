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
using EconModels.SkillsModel;
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

        // Skills
        public DbSet<Skill> Skills { get; set; }

        // Jobs
        public DbSet<Job> Jobs { get; set; }

        // skill and job many-many tables hold this incase needed later.

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

            // Product Additions
            modelBuilder.Entity<Product>()
                .HasIndex(x => new { x.Name, x.VariantName })
                .IsUnique();

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

            // Product Navigation Properties
            // Products <-> Skills
            modelBuilder.Entity<Product>()
                .HasMany(prod => prod.Skills)
                .WithMany(skill => skill.ValidLabors)
                .Map(x =>
                {
                    x.MapLeftKey("LaborId");
                    x.MapRightKey("SkillId");
                    x.ToTable("SkillLabors");
                });
            // Products <-> Jobs
            modelBuilder.Entity<Product>()
                .HasMany(prod => prod.Jobs)
                .WithMany(job => job.Labor)
                .Map(x =>
                {
                    x.MapLeftKey("LaborId");
                    x.MapRightKey("JobId");
                    x.ToTable("JobLabors");
                });

            // Proces Index
            modelBuilder.Entity<Process>()
                .HasIndex(x => new { x.Name, x.VariantName })
                .IsUnique();

            // Process Connections
            modelBuilder.Entity<ProcessInput>()
                .HasKey(x => new { x.ProcessId, x.InputId });
            modelBuilder.Entity<ProcessOutput>()
                .HasKey(x => new { x.ProcessId, x.OutputId });
            modelBuilder.Entity<ProcessCapital>()
                .HasKey(x => new { x.ProcessId, x.CapitalId });
            // May not need, TODO test whether it's needed or not.

            // Process Navigation Properties
            // Processes <-> Job
            modelBuilder.Entity<Process>()
                .HasMany(process => process.Jobs)
                .WithMany(job => job.Processes)
                .Map(x =>
                {
                    x.MapLeftKey("ProcessId");
                    x.MapRightKey("JobId");
                    x.ToTable("JobProcesses");
                });

            // Job Connections
            // self connection Job <-> Job
            modelBuilder.Entity<Job>()
                .HasMany(x => x.RelatedChild)
                .WithMany(x => x.RelatedParent)
                .Map(x =>
                {
                    x.MapLeftKey("ParentId");
                    x.MapRightKey("ChildId");
                    x.ToTable("RelatedJobs");
                });

            // To Job <-> Skills
            modelBuilder.Entity<Job>()
                .HasRequired(x => x.Skill)
                .WithMany(x => x.SkillsJobs)
                .HasForeignKey(x => x.SkillId)
                .WillCascadeOnDelete(false);

            // Skill Connections 
            // Skills <-> Skills
            modelBuilder.Entity<Skill>()
                .HasMany(x => x.RelationChild)
                .WithMany(x => x.RelationParent)
                .Map(x =>
                {
                    x.MapLeftKey("ParentId");
                    x.MapRightKey("ChildId");
                    x.ToTable("RelatedSkills");
                });

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
