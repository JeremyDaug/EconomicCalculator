namespace EconModels.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PopulationGroupUpdate : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.PopulationCultureBreakdowns", name: "Parent_Id", newName: "ParentId");
            RenameColumn(table: "dbo.OwnedProperties", name: "Owner_Id", newName: "OwnerId");
            RenameColumn(table: "dbo.PopulationCultureBreakdowns", name: "Culture_Id", newName: "CultureId");
            RenameColumn(table: "dbo.OwnedProperties", name: "Product_Id", newName: "ProductId");
            RenameIndex(table: "dbo.PopulationCultureBreakdowns", name: "IX_Parent_Id", newName: "IX_ParentId");
            RenameIndex(table: "dbo.PopulationCultureBreakdowns", name: "IX_Culture_Id", newName: "IX_CultureId");
            RenameIndex(table: "dbo.OwnedProperties", name: "IX_Owner_Id", newName: "IX_OwnerId");
            RenameIndex(table: "dbo.OwnedProperties", name: "IX_Product_Id", newName: "IX_ProductId");
            DropPrimaryKey("dbo.PopulationCultureBreakdowns");
            DropPrimaryKey("dbo.OwnedProperties");
            CreateTable(
                "dbo.CultureWants",
                c => new
                    {
                        CultureId = c.Int(nullable: false),
                        Want = c.String(nullable: false, maxLength: 20),
                        NeedType = c.Int(nullable: false),
                        Amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => new { t.CultureId, t.Want, t.NeedType })
                .ForeignKey("dbo.Cultures", t => t.CultureId, cascadeDelete: true)
                .Index(t => t.CultureId);
            
            CreateTable(
                "dbo.CultureTags",
                c => new
                    {
                        CultureId = c.Int(nullable: false),
                        Tag = c.String(nullable: false, maxLength: 30),
                    })
                .PrimaryKey(t => new { t.CultureId, t.Tag })
                .ForeignKey("dbo.Cultures", t => t.CultureId, cascadeDelete: true)
                .Index(t => t.CultureId);
            
            CreateTable(
                "dbo.PoliticalBreakdowns",
                c => new
                    {
                        ParentId = c.Int(nullable: false),
                        PoliticalGroupId = c.Int(nullable: false),
                        Amount = c.Double(nullable: false),
                    })
                .PrimaryKey(t => new { t.ParentId, t.PoliticalGroupId })
                .ForeignKey("dbo.PopulationGroups", t => t.ParentId, cascadeDelete: true)
                .ForeignKey("dbo.PoliticalGroups", t => t.PoliticalGroupId, cascadeDelete: true)
                .Index(t => t.ParentId)
                .Index(t => t.PoliticalGroupId);
            
            CreateTable(
                "dbo.PoliticalGroups",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 30),
                        VariantName = c.String(nullable: false, maxLength: 30),
                        Radicalism = c.Double(nullable: false),
                        Nationalism = c.Double(nullable: false),
                        Centralization = c.Double(nullable: false),
                        Authority = c.Double(nullable: false),
                        Planning = c.Double(nullable: false),
                        Militarism = c.Double(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => new { t.Name, t.VariantName }, unique: true);
            
            CreateTable(
                "dbo.PoliticalTags",
                c => new
                    {
                        GroupId = c.Int(nullable: false),
                        Tag = c.String(nullable: false, maxLength: 30),
                    })
                .PrimaryKey(t => new { t.GroupId, t.Tag })
                .ForeignKey("dbo.PoliticalGroups", t => t.GroupId, cascadeDelete: true)
                .Index(t => t.GroupId);
            
            CreateTable(
                "dbo.SpeciesBreakdowns",
                c => new
                    {
                        ParentId = c.Int(nullable: false),
                        SpeciesId = c.Int(nullable: false),
                        Percent = c.Double(nullable: false),
                    })
                .PrimaryKey(t => new { t.ParentId, t.SpeciesId })
                .ForeignKey("dbo.PopulationGroups", t => t.ParentId, cascadeDelete: true)
                .ForeignKey("dbo.Species", t => t.SpeciesId, cascadeDelete: true)
                .Index(t => t.ParentId)
                .Index(t => t.SpeciesId);
            
            CreateTable(
                "dbo.Species",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 30),
                        VariantName = c.String(nullable: false, maxLength: 30),
                        SpeciesGrowthRate = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TempuraturePreference = c.Double(nullable: false),
                        GravityPreference = c.Double(nullable: false),
                        InfantPhaseLength = c.Double(nullable: false),
                        ChildPhaseLength = c.Double(nullable: false),
                        AdultPhaseLength = c.Double(nullable: false),
                        AverageLifeSpan = c.Double(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => new { t.Name, t.VariantName }, unique: true);
            
            CreateTable(
                "dbo.SpeciesAnathemas",
                c => new
                    {
                        SpeciesId = c.Int(nullable: false),
                        AnathemaId = c.Int(nullable: false),
                        Amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Tag = c.String(nullable: false, maxLength: 30),
                    })
                .PrimaryKey(t => new { t.SpeciesId, t.AnathemaId })
                .ForeignKey("dbo.Products", t => t.AnathemaId, cascadeDelete: true)
                .ForeignKey("dbo.Species", t => t.SpeciesId, cascadeDelete: true)
                .Index(t => t.SpeciesId)
                .Index(t => t.AnathemaId);
            
            CreateTable(
                "dbo.SpeciesAversions",
                c => new
                    {
                        SpeciesId = c.Int(nullable: false),
                        Aversion = c.String(nullable: false, maxLength: 20),
                        Amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Tag = c.String(nullable: false, maxLength: 30),
                    })
                .PrimaryKey(t => new { t.SpeciesId, t.Aversion })
                .ForeignKey("dbo.Species", t => t.SpeciesId, cascadeDelete: true)
                .Index(t => t.SpeciesId);
            
            CreateTable(
                "dbo.SpeciesNeeds",
                c => new
                    {
                        SpeciesId = c.Int(nullable: false),
                        NeedId = c.Int(nullable: false),
                        Amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Tag = c.String(nullable: false, maxLength: 30),
                    })
                .PrimaryKey(t => new { t.SpeciesId, t.NeedId })
                .ForeignKey("dbo.Products", t => t.NeedId, cascadeDelete: true)
                .ForeignKey("dbo.Species", t => t.SpeciesId, cascadeDelete: true)
                .Index(t => t.SpeciesId)
                .Index(t => t.NeedId);
            
            CreateTable(
                "dbo.SpeciesWants",
                c => new
                    {
                        SpeciesId = c.Int(nullable: false),
                        Want = c.String(nullable: false, maxLength: 20),
                        Amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Tag = c.String(nullable: false, maxLength: 30),
                    })
                .PrimaryKey(t => new { t.SpeciesId, t.Want })
                .ForeignKey("dbo.Species", t => t.SpeciesId, cascadeDelete: true)
                .Index(t => t.SpeciesId);
            
            CreateTable(
                "dbo.SpeciesTags",
                c => new
                    {
                        SpeciesId = c.Int(nullable: false),
                        Tag = c.String(nullable: false, maxLength: 30),
                    })
                .PrimaryKey(t => new { t.SpeciesId, t.Tag })
                .ForeignKey("dbo.Species", t => t.SpeciesId, cascadeDelete: true)
                .Index(t => t.SpeciesId);
            
            CreateTable(
                "dbo.Religions",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 30),
                        Sect = c.String(nullable: false, maxLength: 30),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.RelatedCultures",
                c => new
                    {
                        ParentId = c.Int(nullable: false),
                        ChildId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.ParentId, t.ChildId })
                .ForeignKey("dbo.Cultures", t => t.ParentId)
                .ForeignKey("dbo.Cultures", t => t.ChildId)
                .Index(t => t.ParentId)
                .Index(t => t.ChildId);
            
            CreateTable(
                "dbo.PoliticalAllies",
                c => new
                    {
                        AllyId = c.Int(nullable: false),
                        RevId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.AllyId, t.RevId })
                .ForeignKey("dbo.PoliticalGroups", t => t.AllyId)
                .ForeignKey("dbo.PoliticalGroups", t => t.RevId)
                .Index(t => t.AllyId)
                .Index(t => t.RevId);
            
            CreateTable(
                "dbo.PoliticalEnemies",
                c => new
                    {
                        EnemyId = c.Int(nullable: false),
                        RevId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.EnemyId, t.RevId })
                .ForeignKey("dbo.PoliticalGroups", t => t.EnemyId)
                .ForeignKey("dbo.PoliticalGroups", t => t.RevId)
                .Index(t => t.EnemyId)
                .Index(t => t.RevId);
            
            AddPrimaryKey("dbo.PopulationCultureBreakdowns", new[] { "ParentId", "CultureId" });
            AddPrimaryKey("dbo.OwnedProperties", new[] { "OwnerId", "ProductId" });
            CreateIndex("dbo.Cultures", new[] { "Name", "VariantName" }, unique: true);
            DropColumn("dbo.PopulationGroups", "SkillName");
            DropColumn("dbo.PopulationCultureBreakdowns", "Id");
            DropColumn("dbo.OwnedProperties", "Id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.OwnedProperties", "Id", c => c.Int(nullable: false, identity: true));
            AddColumn("dbo.PopulationCultureBreakdowns", "Id", c => c.Int(nullable: false, identity: true));
            AddColumn("dbo.PopulationGroups", "SkillName", c => c.String(nullable: false, maxLength: 30));
            DropForeignKey("dbo.SpeciesBreakdowns", "SpeciesId", "dbo.Species");
            DropForeignKey("dbo.SpeciesTags", "SpeciesId", "dbo.Species");
            DropForeignKey("dbo.SpeciesWants", "SpeciesId", "dbo.Species");
            DropForeignKey("dbo.SpeciesNeeds", "SpeciesId", "dbo.Species");
            DropForeignKey("dbo.SpeciesNeeds", "NeedId", "dbo.Products");
            DropForeignKey("dbo.SpeciesAversions", "SpeciesId", "dbo.Species");
            DropForeignKey("dbo.SpeciesAnathemas", "SpeciesId", "dbo.Species");
            DropForeignKey("dbo.SpeciesAnathemas", "AnathemaId", "dbo.Products");
            DropForeignKey("dbo.SpeciesBreakdowns", "ParentId", "dbo.PopulationGroups");
            DropForeignKey("dbo.PoliticalBreakdowns", "PoliticalGroupId", "dbo.PoliticalGroups");
            DropForeignKey("dbo.PoliticalTags", "GroupId", "dbo.PoliticalGroups");
            DropForeignKey("dbo.PoliticalEnemies", "RevId", "dbo.PoliticalGroups");
            DropForeignKey("dbo.PoliticalEnemies", "EnemyId", "dbo.PoliticalGroups");
            DropForeignKey("dbo.PoliticalAllies", "RevId", "dbo.PoliticalGroups");
            DropForeignKey("dbo.PoliticalAllies", "AllyId", "dbo.PoliticalGroups");
            DropForeignKey("dbo.PoliticalBreakdowns", "ParentId", "dbo.PopulationGroups");
            DropForeignKey("dbo.CultureTags", "CultureId", "dbo.Cultures");
            DropForeignKey("dbo.RelatedCultures", "ChildId", "dbo.Cultures");
            DropForeignKey("dbo.RelatedCultures", "ParentId", "dbo.Cultures");
            DropForeignKey("dbo.CultureWants", "CultureId", "dbo.Cultures");
            DropIndex("dbo.PoliticalEnemies", new[] { "RevId" });
            DropIndex("dbo.PoliticalEnemies", new[] { "EnemyId" });
            DropIndex("dbo.PoliticalAllies", new[] { "RevId" });
            DropIndex("dbo.PoliticalAllies", new[] { "AllyId" });
            DropIndex("dbo.RelatedCultures", new[] { "ChildId" });
            DropIndex("dbo.RelatedCultures", new[] { "ParentId" });
            DropIndex("dbo.SpeciesTags", new[] { "SpeciesId" });
            DropIndex("dbo.SpeciesWants", new[] { "SpeciesId" });
            DropIndex("dbo.SpeciesNeeds", new[] { "NeedId" });
            DropIndex("dbo.SpeciesNeeds", new[] { "SpeciesId" });
            DropIndex("dbo.SpeciesAversions", new[] { "SpeciesId" });
            DropIndex("dbo.SpeciesAnathemas", new[] { "AnathemaId" });
            DropIndex("dbo.SpeciesAnathemas", new[] { "SpeciesId" });
            DropIndex("dbo.Species", new[] { "Name", "VariantName" });
            DropIndex("dbo.SpeciesBreakdowns", new[] { "SpeciesId" });
            DropIndex("dbo.SpeciesBreakdowns", new[] { "ParentId" });
            DropIndex("dbo.PoliticalTags", new[] { "GroupId" });
            DropIndex("dbo.PoliticalGroups", new[] { "Name", "VariantName" });
            DropIndex("dbo.PoliticalBreakdowns", new[] { "PoliticalGroupId" });
            DropIndex("dbo.PoliticalBreakdowns", new[] { "ParentId" });
            DropIndex("dbo.CultureTags", new[] { "CultureId" });
            DropIndex("dbo.CultureWants", new[] { "CultureId" });
            DropIndex("dbo.Cultures", new[] { "Name", "VariantName" });
            DropPrimaryKey("dbo.OwnedProperties");
            DropPrimaryKey("dbo.PopulationCultureBreakdowns");
            DropTable("dbo.PoliticalEnemies");
            DropTable("dbo.PoliticalAllies");
            DropTable("dbo.RelatedCultures");
            DropTable("dbo.Religions");
            DropTable("dbo.SpeciesTags");
            DropTable("dbo.SpeciesWants");
            DropTable("dbo.SpeciesNeeds");
            DropTable("dbo.SpeciesAversions");
            DropTable("dbo.SpeciesAnathemas");
            DropTable("dbo.Species");
            DropTable("dbo.SpeciesBreakdowns");
            DropTable("dbo.PoliticalTags");
            DropTable("dbo.PoliticalGroups");
            DropTable("dbo.PoliticalBreakdowns");
            DropTable("dbo.CultureTags");
            DropTable("dbo.CultureWants");
            AddPrimaryKey("dbo.OwnedProperties", "Id");
            AddPrimaryKey("dbo.PopulationCultureBreakdowns", "Id");
            RenameIndex(table: "dbo.OwnedProperties", name: "IX_ProductId", newName: "IX_Product_Id");
            RenameIndex(table: "dbo.OwnedProperties", name: "IX_OwnerId", newName: "IX_Owner_Id");
            RenameIndex(table: "dbo.PopulationCultureBreakdowns", name: "IX_CultureId", newName: "IX_Culture_Id");
            RenameIndex(table: "dbo.PopulationCultureBreakdowns", name: "IX_ParentId", newName: "IX_Parent_Id");
            RenameColumn(table: "dbo.OwnedProperties", name: "ProductId", newName: "Product_Id");
            RenameColumn(table: "dbo.PopulationCultureBreakdowns", name: "CultureId", newName: "Culture_Id");
            RenameColumn(table: "dbo.OwnedProperties", name: "OwnerId", newName: "Owner_Id");
            RenameColumn(table: "dbo.PopulationCultureBreakdowns", name: "ParentId", newName: "Parent_Id");
        }
    }
}
