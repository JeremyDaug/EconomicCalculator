namespace EconModels.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ProcessVariantName : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Processes", "VariantName", c => c.String(maxLength: 20));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Processes", "VariantName");
        }
    }
}
