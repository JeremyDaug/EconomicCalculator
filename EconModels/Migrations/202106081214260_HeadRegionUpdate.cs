namespace EconModels.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class HeadRegionUpdate : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Planets", "HeadRegionId", "dbo.Regions");
            DropIndex("dbo.Planets", new[] { "HeadRegionId" });
            RenameColumn(table: "dbo.Planets", name: "HeadRegionId", newName: "HeadRegion_Id");
            AlterColumn("dbo.Planets", "HeadRegion_Id", c => c.Int());
            CreateIndex("dbo.Planets", "HeadRegion_Id");
            AddForeignKey("dbo.Planets", "HeadRegion_Id", "dbo.Regions", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Planets", "HeadRegion_Id", "dbo.Regions");
            DropIndex("dbo.Planets", new[] { "HeadRegion_Id" });
            AlterColumn("dbo.Planets", "HeadRegion_Id", c => c.Int(nullable: false));
            RenameColumn(table: "dbo.Planets", name: "HeadRegion_Id", newName: "HeadRegionId");
            CreateIndex("dbo.Planets", "HeadRegionId");
            AddForeignKey("dbo.Planets", "HeadRegionId", "dbo.Regions", "Id", cascadeDelete: true);
        }
    }
}
