namespace EconModels.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveRequiredVariants : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.PoliticalGroups", new[] { "Name", "VariantName" });
            DropIndex("dbo.Species", new[] { "Name", "VariantName" });
            AlterColumn("dbo.PoliticalGroups", "VariantName", c => c.String(maxLength: 30));
            AlterColumn("dbo.Species", "VariantName", c => c.String(maxLength: 30));
            CreateIndex("dbo.PoliticalGroups", new[] { "Name", "VariantName" }, unique: true);
            CreateIndex("dbo.Species", new[] { "Name", "VariantName" }, unique: true);
        }
        
        public override void Down()
        {
            DropIndex("dbo.Species", new[] { "Name", "VariantName" });
            DropIndex("dbo.PoliticalGroups", new[] { "Name", "VariantName" });
            AlterColumn("dbo.Species", "VariantName", c => c.String(nullable: false, maxLength: 30));
            AlterColumn("dbo.PoliticalGroups", "VariantName", c => c.String(nullable: false, maxLength: 30));
            CreateIndex("dbo.Species", new[] { "Name", "VariantName" }, unique: true);
            CreateIndex("dbo.PoliticalGroups", new[] { "Name", "VariantName" }, unique: true);
        }
    }
}
