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
        public DbSet<ProductWantTag> ProductWantTags { get; set; }
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
        public DbSet<CultureNeed> CultureNeeds { get; set; }
        public DbSet<CultureWant> CultureWants { get; set; }
        public DbSet<CultureTag> CultureTags { get; set; }
        public DbSet<Culture> Cultures { get; set; }

        // Species
        public DbSet<SpeciesAnathema> SpeciesAnathemas { get; set; }
        public DbSet<SpeciesAversion> SpeciesAversions { get; set; }
        public DbSet<SpeciesWant> SpeciesWants { get; set; }
        public DbSet<SpeciesNeed> SpeciesNeeds { get; set; }
        public DbSet<Species> Species { get; set; }

        // Political Groups
        public DbSet<PoliticalTag> PoliticalTags { get; set; }
        public DbSet<PoliticalGroup> PoliticalGroups { get; set; }

        public DbSet<Religion> Religion { get; set; }

        // Population Groups
        public DbSet<CultureBreakdown> PopCultureBreakdowns { get; set; }
        public DbSet<SpeciesBreakdown> PopSpeciesBreakdowns { get; set; }
        public DbSet<PoliticalBreakdown> PopPoliticalBreakdowns { get; set; }
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

            // Want Tags
            modelBuilder.Entity<ProductWantTag>()
                .HasKey(x => new { x.ProductId, x.Tag });

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

            // Culture
            // Shouldn't need to explain many to one connections from culture out.
            modelBuilder.Entity<CultureNeed>()
                .HasKey(p => new { p.CultureId, p.NeedId, p.NeedType });
            modelBuilder.Entity<CultureWant>()
                .HasKey(p => new { p.CultureId, p.Want, p.NeedType });
            modelBuilder.Entity<CultureTag>()
                .HasKey(p => new { p.CultureId, p.Tag });
            modelBuilder.Entity<Culture>()
                .HasIndex(p => new { p.Name, p.VariantName })
                .IsUnique();

            // Culture <-> Culture
            modelBuilder.Entity<Culture>()
                .HasMany(x => x.RelationChild)
                .WithMany(x => x.RelationParent)
                .Map(x =>
                {
                    x.MapLeftKey("ParentId");
                    x.MapRightKey("ChildId");
                    x.ToTable("RelatedCultures");
                });

            // Political Tag
            modelBuilder.Entity<PoliticalTag>()
                .HasKey(x => new { x.GroupId, x.Tag });
            // Political Groups
            modelBuilder.Entity<PoliticalGroup>()
                .HasIndex(x => new { x.Name, x.VariantName })
                .IsUnique();
            // Political Allies
            modelBuilder.Entity<PoliticalGroup>()
                .HasMany(x => x.Allies)
                .WithMany(x => x.AlliesRev)
                .Map(x =>
                {
                    x.MapLeftKey("AllyId");
                    x.MapRightKey("RevId");
                    x.ToTable("PoliticalAllies");
                });
            // Political Entities
            modelBuilder.Entity<PoliticalGroup>()
                .HasMany(x => x.Enemies)
                .WithMany(x => x.EnemiesRev)
                .Map(x =>
                {
                    x.MapLeftKey("EnemyId");
                    x.MapRightKey("RevId");
                    x.ToTable("PoliticalEnemies");
                });

            // Species
            modelBuilder.Entity<SpeciesWant>()
                .HasKey(x => new { x.SpeciesId, x.Want });
            modelBuilder.Entity<SpeciesNeed>()
                .HasKey(x => new { x.SpeciesId, x.NeedId });
            modelBuilder.Entity<SpeciesTag>()
                .HasKey(x => new { x.SpeciesId, x.Tag });
            modelBuilder.Entity<SpeciesAversion>()
                .HasKey(x => new { x.SpeciesId, x.Aversion });
            modelBuilder.Entity<SpeciesAnathema>()
                .HasKey(x => new { x.SpeciesId, x.AnathemaId });
            modelBuilder.Entity<Species>()
                .HasIndex(x => new { x.Name, x.VariantName })
                .IsUnique();

            // Species to species not included as name matching
            // is all that is needed and must remain the main
            // method.

            // property 
            modelBuilder.Entity<OwnedProperty>()
                .HasKey(x => new { x.OwnerId, x.ProductId });

            // Breakdowns
            modelBuilder.Entity<SpeciesBreakdown>()
                .HasKey(x => new { x.ParentId, x.SpeciesId });
            modelBuilder.Entity<CultureBreakdown>()
                .HasKey(x => new { x.ParentId, x.CultureId });
            modelBuilder.Entity<PoliticalBreakdown>()
                .HasKey(x => new { x.ParentId, x.PoliticalGroupId });

            // Pop Group Indeces
            // There should only be one pop group per job in each territory.
            modelBuilder.Entity<PopulationGroup>()
                .HasIndex(x => new { x.TerritoryId, x.PrimaryJobId })
                .IsUnique();

            // Territory
            // Territory index
            modelBuilder.Entity<Territory>()
                .HasIndex(x => x.Name)
                .IsUnique();

            // Public Goods
            modelBuilder.Entity<PublicGood>()
                .HasKey(x => new { x.TerritoryId, x.GoodId });

            modelBuilder.Entity<Territory>()
                .HasMany(x => x.PublicGoods)
                .WithRequired(x => x.Territory)
                .WillCascadeOnDelete(true);

            // Local Resources
            modelBuilder.Entity<LocalResource>()
                .HasKey(x => new { x.ResourceId, x.TerritoryId });

            modelBuilder.Entity<Territory>()
                .HasMany(x => x.LocalResources)
                .WithRequired(x => x.Territory)
                .WillCascadeOnDelete(true);

            // Outgoing Connections
            modelBuilder.Entity<TerritoryConnection>()
                .HasKey(x => new { x.StartId, x.EndId });
            modelBuilder.Entity<Territory>()
                .HasMany(x => x.OutgoingConnections)
                .WithRequired(x => x.Start)
                .HasForeignKey(x => x.StartId)
                .WillCascadeOnDelete(true);

            // Incoming Connections
            modelBuilder.Entity<Territory>()
                .HasMany(x => x.IncomingConnections)
                .WithRequired(x => x.End)
                .HasForeignKey(x => x.EndId)
                .WillCascadeOnDelete(false);

            // Pop Contained Connection
            modelBuilder.Entity<Territory>()
                .HasMany(x => x.Pops)
                .WithRequired(x => x.Territory) // make required later.
                .WillCascadeOnDelete(false);

            // Land Owners
            modelBuilder.Entity<LandOwner>()
                .HasKey(x => new { x.TerritoryId, x.OwnerId });
            modelBuilder.Entity<Territory>()
                .HasMany(x => x.LandOwners)
                .WithRequired(x => x.Territory)
                .WillCascadeOnDelete(true);
        }

        public System.Data.Entity.DbSet<EconModels.PopulationModel.SpeciesTag> SpeciesTags { get; set; }
    }
}
