namespace EconModels.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RegionGridStoring : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.RegionHexCoords",
                c => new
                    {
                        RegionId = c.Int(nullable: false),
                        X = c.Int(nullable: false),
                        Y = c.Int(nullable: false),
                        Z = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.RegionId, t.X, t.Y, t.Z })
                .ForeignKey("dbo.Regions", t => t.RegionId, cascadeDelete: true)
                .Index(t => t.RegionId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.RegionHexCoords", "RegionId", "dbo.Regions");
            DropIndex("dbo.RegionHexCoords", new[] { "RegionId" });
            DropTable("dbo.RegionHexCoords");
        }
    }
}
