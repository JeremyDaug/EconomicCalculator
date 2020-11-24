namespace EconModels.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Update1 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Products", "Product_Id", "dbo.Products");
            DropIndex("dbo.Products", new[] { "Product_Id" });
            CreateTable(
                "dbo.ProductProducts",
                c => new
                    {
                        Product_Id = c.Int(nullable: false),
                        Product_Id1 = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Product_Id, t.Product_Id1 })
                .ForeignKey("dbo.Products", t => t.Product_Id)
                .ForeignKey("dbo.Products", t => t.Product_Id1)
                .Index(t => t.Product_Id)
                .Index(t => t.Product_Id1);
            
            DropColumn("dbo.Products", "Product_Id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Products", "Product_Id", c => c.Int());
            DropForeignKey("dbo.ProductProducts", "Product_Id1", "dbo.Products");
            DropForeignKey("dbo.ProductProducts", "Product_Id", "dbo.Products");
            DropIndex("dbo.ProductProducts", new[] { "Product_Id1" });
            DropIndex("dbo.ProductProducts", new[] { "Product_Id" });
            DropTable("dbo.ProductProducts");
            CreateIndex("dbo.Products", "Product_Id");
            AddForeignKey("dbo.Products", "Product_Id", "dbo.Products", "Id");
        }
    }
}
