namespace EconModels.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PopGroupUpdate : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PopulationGroups", "Infants", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.PopulationGroups", "Children", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.PopulationGroups", "Adults", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.PopulationGroups", "Seniors", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            DropColumn("dbo.PopulationGroups", "Count");
        }
        
        public override void Down()
        {
            AddColumn("dbo.PopulationGroups", "Count", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            DropColumn("dbo.PopulationGroups", "Seniors");
            DropColumn("dbo.PopulationGroups", "Adults");
            DropColumn("dbo.PopulationGroups", "Children");
            DropColumn("dbo.PopulationGroups", "Infants");
        }
    }
}
