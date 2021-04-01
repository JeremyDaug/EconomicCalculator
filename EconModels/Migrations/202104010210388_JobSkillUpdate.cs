namespace EconModels.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class JobSkillUpdate : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Jobs", "SkillLevel", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Jobs", "SkillLevel", c => c.Single(nullable: false));
        }
    }
}
