namespace EconModels.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SimplePlanetGrid : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Planets", "Rows", c => c.Int(nullable: false));
            AddColumn("dbo.Planets", "Columns", c => c.Int(nullable: false));
            DropColumn("dbo.Planets", "RowMin");
            DropColumn("dbo.Planets", "RowMax");
            DropColumn("dbo.Planets", "ColMin");
            DropColumn("dbo.Planets", "ColMax");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Planets", "ColMax", c => c.Int(nullable: false));
            AddColumn("dbo.Planets", "ColMin", c => c.Int(nullable: false));
            AddColumn("dbo.Planets", "RowMax", c => c.Int(nullable: false));
            AddColumn("dbo.Planets", "RowMin", c => c.Int(nullable: false));
            DropColumn("dbo.Planets", "Columns");
            DropColumn("dbo.Planets", "Rows");
        }
    }
}
