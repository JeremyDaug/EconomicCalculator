namespace EconModels.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Jobs : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Jobs",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 30),
                        JobType = c.Int(nullable: false),
                        JobCategory = c.Int(nullable: false),
                        SkillName = c.String(nullable: false, maxLength: 30),
                        SkillLevel = c.Int(nullable: false),
                        LaborRequirements = c.Double(nullable: false),
                        Process_Id = c.Int(nullable: false),
                        Job_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Processes", t => t.Process_Id, cascadeDelete: true)
                .ForeignKey("dbo.Jobs", t => t.Job_Id)
                .Index(t => t.Process_Id)
                .Index(t => t.Job_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Jobs", "Job_Id", "dbo.Jobs");
            DropForeignKey("dbo.Jobs", "Process_Id", "dbo.Processes");
            DropIndex("dbo.Jobs", new[] { "Job_Id" });
            DropIndex("dbo.Jobs", new[] { "Process_Id" });
            DropTable("dbo.Jobs");
        }
    }
}
