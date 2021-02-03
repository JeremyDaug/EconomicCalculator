namespace EconModels.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ProcessGoodTags : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ProcessCapitals", "Tag", c => c.Int(nullable: false));
            AddColumn("dbo.ProcessInputs", "Tag", c => c.Int(nullable: false));
            AddColumn("dbo.ProcessOutputs", "Tag", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ProcessOutputs", "Tag");
            DropColumn("dbo.ProcessInputs", "Tag");
            DropColumn("dbo.ProcessCapitals", "Tag");
        }
    }
}
