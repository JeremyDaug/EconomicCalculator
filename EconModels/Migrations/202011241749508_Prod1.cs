namespace EconModels.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Prod1 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.ProductProducts", "Product_Id", "dbo.Products");
            DropForeignKey("dbo.ProductProducts", "Product_Id1", "dbo.Products");
            DropIndex("dbo.ProductProducts", new[] { "Product_Id" });
            DropIndex("dbo.ProductProducts", new[] { "Product_Id1" });
            AddColumn("dbo.Products", "Mass", c => c.Double(nullable: false));
            AlterColumn("dbo.Products", "Name", c => c.String(nullable: false, maxLength: 30));
            AlterColumn("dbo.Products", "VariantName", c => c.String(maxLength: 30));
            AlterColumn("dbo.Products", "UnitName", c => c.String(nullable: false));
            AlterColumn("dbo.Products", "DefaultPrice", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            DropTable("dbo.ProductProducts");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.ProductProducts",
                c => new
                    {
                        Product_Id = c.Int(nullable: false),
                        Product_Id1 = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Product_Id, t.Product_Id1 });
            
            AlterColumn("dbo.Products", "DefaultPrice", c => c.Double(nullable: false));
            AlterColumn("dbo.Products", "UnitName", c => c.String());
            AlterColumn("dbo.Products", "VariantName", c => c.String());
            AlterColumn("dbo.Products", "Name", c => c.String());
            DropColumn("dbo.Products", "Mass");
            CreateIndex("dbo.ProductProducts", "Product_Id1");
            CreateIndex("dbo.ProductProducts", "Product_Id");
            AddForeignKey("dbo.ProductProducts", "Product_Id1", "dbo.Products", "Id");
            AddForeignKey("dbo.ProductProducts", "Product_Id", "dbo.Products", "Id");
        }
    }
}
