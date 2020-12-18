namespace EconModels.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Market1 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ProductPrices",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        MarketId = c.Int(nullable: false),
                        ProductId = c.Int(nullable: false),
                        MarketPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Markets", t => t.MarketId, cascadeDelete: true)
                .ForeignKey("dbo.Products", t => t.ProductId, cascadeDelete: true)
                .Index(t => new { t.MarketId, t.ProductId }, unique: true, name: "UniqueCoupling");
            
            CreateTable(
                "dbo.Markets",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 40),
                        Territory_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Territories", t => t.Territory_Id)
                .Index(t => t.Territory_Id);
            
            AddColumn("dbo.Products", "Market_Id", c => c.Int());
            AddColumn("dbo.PopulationGroups", "Market_Id", c => c.Int());
            CreateIndex("dbo.Products", "Market_Id");
            CreateIndex("dbo.PopulationGroups", "Market_Id");
            AddForeignKey("dbo.PopulationGroups", "Market_Id", "dbo.Markets", "Id");
            AddForeignKey("dbo.Products", "Market_Id", "dbo.Markets", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ProductPrices", "ProductId", "dbo.Products");
            DropForeignKey("dbo.Products", "Market_Id", "dbo.Markets");
            DropForeignKey("dbo.Markets", "Territory_Id", "dbo.Territories");
            DropForeignKey("dbo.ProductPrices", "MarketId", "dbo.Markets");
            DropForeignKey("dbo.PopulationGroups", "Market_Id", "dbo.Markets");
            DropIndex("dbo.Markets", new[] { "Territory_Id" });
            DropIndex("dbo.ProductPrices", "UniqueCoupling");
            DropIndex("dbo.PopulationGroups", new[] { "Market_Id" });
            DropIndex("dbo.Products", new[] { "Market_Id" });
            DropColumn("dbo.PopulationGroups", "Market_Id");
            DropColumn("dbo.Products", "Market_Id");
            DropTable("dbo.Markets");
            DropTable("dbo.ProductPrices");
        }
    }
}
