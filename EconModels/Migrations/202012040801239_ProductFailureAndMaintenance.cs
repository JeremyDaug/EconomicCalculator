namespace EconModels.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ProductFailureAndMaintenance : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.FailsIntoPairs",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        SourceId = c.Int(nullable: false),
                        ResultId = c.Int(nullable: false),
                        Amount = c.Double(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Products", t => t.ResultId)
                .ForeignKey("dbo.Products", t => t.SourceId)
                .Index(t => new { t.SourceId, t.ResultId }, unique: true, name: "UniqueCoupling");
            
            CreateTable(
                "dbo.MaintenancePairs",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        SourceId = c.Int(nullable: false),
                        ResultId = c.Int(nullable: false),
                        Amount = c.Double(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Products", t => t.ResultId)
                .ForeignKey("dbo.Products", t => t.SourceId)
                .Index(t => new { t.SourceId, t.ResultId }, unique: true, name: "UniqueCoupling");
            
            AlterColumn("dbo.Products", "UnitName", c => c.String(nullable: false, maxLength: 15));
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.FailsIntoPairs", "SourceId", "dbo.Products");
            DropForeignKey("dbo.FailsIntoPairs", "ResultId", "dbo.Products");
            DropForeignKey("dbo.MaintenancePairs", "SourceId", "dbo.Products");
            DropForeignKey("dbo.MaintenancePairs", "ResultId", "dbo.Products");
            DropIndex("dbo.MaintenancePairs", "UniqueCoupling");
            DropIndex("dbo.FailsIntoPairs", "UniqueCoupling");
            AlterColumn("dbo.Products", "UnitName", c => c.String(nullable: false));
            DropTable("dbo.MaintenancePairs");
            DropTable("dbo.FailsIntoPairs");
        }
    }
}
