namespace EconModels.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PopGroups : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CultureNeeds",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        NeedType = c.Int(nullable: false),
                        Culture_Id = c.Int(nullable: false),
                        Need_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Cultures", t => t.Culture_Id, cascadeDelete: true)
                .ForeignKey("dbo.Products", t => t.Need_Id, cascadeDelete: true)
                .Index(t => t.Culture_Id)
                .Index(t => t.Need_Id);
            
            CreateTable(
                "dbo.Cultures",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        CultureGrowthRate = c.Double(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.PopulationCultureBreakdowns",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Amount = c.Double(nullable: false),
                        Culture_Id = c.Int(nullable: false),
                        Parent_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Cultures", t => t.Culture_Id, cascadeDelete: true)
                .ForeignKey("dbo.PopulationGroups", t => t.Parent_Id, cascadeDelete: true)
                .Index(t => t.Culture_Id)
                .Index(t => t.Parent_Id);
            
            CreateTable(
                "dbo.PopulationGroups",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 30),
                        Count = c.Decimal(nullable: false, precision: 18, scale: 2),
                        SkillName = c.String(nullable: false, maxLength: 30),
                        SkillLevel = c.Int(nullable: false),
                        PrimaryJobId = c.Int(nullable: false),
                        Priority = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Jobs", t => t.PrimaryJobId, cascadeDelete: true)
                .Index(t => t.PrimaryJobId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.PopulationCultureBreakdowns", "Parent_Id", "dbo.PopulationGroups");
            DropForeignKey("dbo.PopulationGroups", "PrimaryJobId", "dbo.Jobs");
            DropForeignKey("dbo.PopulationCultureBreakdowns", "Culture_Id", "dbo.Cultures");
            DropForeignKey("dbo.CultureNeeds", "Need_Id", "dbo.Products");
            DropForeignKey("dbo.CultureNeeds", "Culture_Id", "dbo.Cultures");
            DropIndex("dbo.PopulationGroups", new[] { "PrimaryJobId" });
            DropIndex("dbo.PopulationCultureBreakdowns", new[] { "Parent_Id" });
            DropIndex("dbo.PopulationCultureBreakdowns", new[] { "Culture_Id" });
            DropIndex("dbo.CultureNeeds", new[] { "Need_Id" });
            DropIndex("dbo.CultureNeeds", new[] { "Culture_Id" });
            DropTable("dbo.PopulationGroups");
            DropTable("dbo.PopulationCultureBreakdowns");
            DropTable("dbo.Cultures");
            DropTable("dbo.CultureNeeds");
        }
    }
}
