namespace EconModels.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ProcessBase : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ProcessCapitals",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ParentId = c.Int(nullable: false),
                        ProductId = c.Int(nullable: false),
                        Amount = c.Double(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Processes", t => t.ParentId, cascadeDelete: true)
                .ForeignKey("dbo.Products", t => t.ProductId, cascadeDelete: true)
                .Index(t => new { t.ParentId, t.ProductId }, unique: true, name: "UniqueCoupling");
            
            CreateTable(
                "dbo.Processes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 30),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ProcessInputs",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ParentId = c.Int(nullable: false),
                        ProductId = c.Int(nullable: false),
                        Amount = c.Double(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Processes", t => t.ParentId, cascadeDelete: true)
                .ForeignKey("dbo.Products", t => t.ProductId, cascadeDelete: true)
                .Index(t => new { t.ParentId, t.ProductId }, unique: true, name: "UniqueCoupling");
            
            CreateTable(
                "dbo.ProcessOutputs",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ParentId = c.Int(nullable: false),
                        ProductId = c.Int(nullable: false),
                        Amount = c.Double(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Processes", t => t.ParentId, cascadeDelete: true)
                .ForeignKey("dbo.Products", t => t.ProductId, cascadeDelete: true)
                .Index(t => new { t.ParentId, t.ProductId }, unique: true, name: "UniqueCoupling");
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ProcessCapitals", "ProductId", "dbo.Products");
            DropForeignKey("dbo.ProcessOutputs", "ProductId", "dbo.Products");
            DropForeignKey("dbo.ProcessOutputs", "ParentId", "dbo.Processes");
            DropForeignKey("dbo.ProcessInputs", "ProductId", "dbo.Products");
            DropForeignKey("dbo.ProcessInputs", "ParentId", "dbo.Processes");
            DropForeignKey("dbo.ProcessCapitals", "ParentId", "dbo.Processes");
            DropIndex("dbo.ProcessOutputs", "UniqueCoupling");
            DropIndex("dbo.ProcessInputs", "UniqueCoupling");
            DropIndex("dbo.ProcessCapitals", "UniqueCoupling");
            DropTable("dbo.ProcessOutputs");
            DropTable("dbo.ProcessInputs");
            DropTable("dbo.Processes");
            DropTable("dbo.ProcessCapitals");
        }
    }
}
