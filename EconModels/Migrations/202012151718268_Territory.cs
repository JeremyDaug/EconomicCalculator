namespace EconModels.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Territory : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.LandOwners",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Owner_Id = c.Int(nullable: false),
                        Territory_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.PopulationGroups", t => t.Owner_Id, cascadeDelete: true)
                .ForeignKey("dbo.Territories", t => t.Territory_Id, cascadeDelete: true)
                .Index(t => t.Owner_Id)
                .Index(t => t.Territory_Id);
            
            CreateTable(
                "dbo.OwnedProperties",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Owner_Id = c.Int(nullable: false),
                        Product_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.PopulationGroups", t => t.Owner_Id, cascadeDelete: true)
                .ForeignKey("dbo.Products", t => t.Product_Id, cascadeDelete: true)
                .Index(t => t.Owner_Id)
                .Index(t => t.Product_Id);
            
            CreateTable(
                "dbo.Territories",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 40),
                        X = c.Int(),
                        Y = c.Int(),
                        Z = c.Int(),
                        Extent = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Elevation = c.Int(nullable: false),
                        WaterLevel = c.Decimal(nullable: false, precision: 18, scale: 2),
                        HasRiver = c.Boolean(nullable: false),
                        Humidity = c.Int(nullable: false),
                        Tempurature = c.Int(nullable: false),
                        Roughness = c.Int(nullable: false),
                        InfrastructureLevel = c.Int(nullable: false),
                        AvailableLand = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.TerritoryConnections",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        StartId = c.Int(nullable: false),
                        EndId = c.Int(nullable: false),
                        Distance = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Territories", t => t.EndId)
                .ForeignKey("dbo.Territories", t => t.StartId)
                .Index(t => new { t.StartId, t.EndId }, unique: true, name: "UniqueCoupling");
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.LandOwners", "Territory_Id", "dbo.Territories");
            DropForeignKey("dbo.TerritoryConnections", "StartId", "dbo.Territories");
            DropForeignKey("dbo.TerritoryConnections", "EndId", "dbo.Territories");
            DropForeignKey("dbo.LandOwners", "Owner_Id", "dbo.PopulationGroups");
            DropForeignKey("dbo.OwnedProperties", "Product_Id", "dbo.Products");
            DropForeignKey("dbo.OwnedProperties", "Owner_Id", "dbo.PopulationGroups");
            DropIndex("dbo.TerritoryConnections", "UniqueCoupling");
            DropIndex("dbo.OwnedProperties", new[] { "Product_Id" });
            DropIndex("dbo.OwnedProperties", new[] { "Owner_Id" });
            DropIndex("dbo.LandOwners", new[] { "Territory_Id" });
            DropIndex("dbo.LandOwners", new[] { "Owner_Id" });
            DropTable("dbo.TerritoryConnections");
            DropTable("dbo.Territories");
            DropTable("dbo.OwnedProperties");
            DropTable("dbo.LandOwners");
        }
    }
}
