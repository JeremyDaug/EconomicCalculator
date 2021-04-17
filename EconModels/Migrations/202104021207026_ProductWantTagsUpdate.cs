namespace EconModels.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ProductWantTagsUpdate : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.CultureNeeds", name: "Culture_Id", newName: "CultureId");
            RenameColumn(table: "dbo.CultureNeeds", name: "Need_Id", newName: "NeedId");
            RenameIndex(table: "dbo.CultureNeeds", name: "IX_Culture_Id", newName: "IX_CultureId");
            RenameIndex(table: "dbo.CultureNeeds", name: "IX_Need_Id", newName: "IX_NeedId");
            DropPrimaryKey("dbo.CultureNeeds");
            CreateTable(
                "dbo.ProductWantTags",
                c => new
                    {
                        ProductId = c.Int(nullable: false),
                        Tag = c.String(nullable: false, maxLength: 20),
                    })
                .PrimaryKey(t => new { t.ProductId, t.Tag })
                .ForeignKey("dbo.Products", t => t.ProductId, cascadeDelete: true)
                .Index(t => new { t.ProductId, t.Tag }, unique: true, name: "UniqueCoupling");
            
            AddColumn("dbo.Cultures", "VariantName", c => c.String(maxLength: 30));
            AlterColumn("dbo.Cultures", "Name", c => c.String(nullable: false, maxLength: 30));
            AddPrimaryKey("dbo.CultureNeeds", new[] { "CultureId", "NeedId", "NeedType" });
            DropColumn("dbo.CultureNeeds", "Id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.CultureNeeds", "Id", c => c.Int(nullable: false, identity: true));
            DropForeignKey("dbo.ProductWantTags", "ProductId", "dbo.Products");
            DropIndex("dbo.ProductWantTags", "UniqueCoupling");
            DropPrimaryKey("dbo.CultureNeeds");
            AlterColumn("dbo.Cultures", "Name", c => c.String());
            DropColumn("dbo.Cultures", "VariantName");
            DropTable("dbo.ProductWantTags");
            AddPrimaryKey("dbo.CultureNeeds", "Id");
            RenameIndex(table: "dbo.CultureNeeds", name: "IX_NeedId", newName: "IX_Need_Id");
            RenameIndex(table: "dbo.CultureNeeds", name: "IX_CultureId", newName: "IX_Culture_Id");
            RenameColumn(table: "dbo.CultureNeeds", name: "NeedId", newName: "Need_Id");
            RenameColumn(table: "dbo.CultureNeeds", name: "CultureId", newName: "Culture_Id");
        }
    }
}
