namespace EconModels.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Products",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        VariantName = c.String(),
                        UnitName = c.String(),
                        Quality = c.Int(nullable: false),
                        DefaultPrice = c.Double(nullable: false),
                        Bulk = c.Double(nullable: false),
                        ProductTypes = c.Int(nullable: false),
                        Maintainable = c.Boolean(nullable: false),
                        Fractional = c.Boolean(nullable: false),
                        MeanTimeToFailure = c.Int(nullable: false),
                        Product_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Products", t => t.Product_Id)
                .Index(t => t.Product_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Products", "Product_Id", "dbo.Products");
            DropIndex("dbo.Products", new[] { "Product_Id" });
            DropTable("dbo.Products");
        }
    }
}
