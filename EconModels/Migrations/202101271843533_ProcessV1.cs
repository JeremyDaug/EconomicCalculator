namespace EconModels.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ProcessV1 : DbMigration
    {
        public override void Up()
        { // Updates Processes and adds new products and their failures.
            RenameColumn(table: "dbo.ProcessCapitals", name: "ParentId", newName: "ProcessId");
            RenameColumn(table: "dbo.ProcessInputs", name: "ParentId", newName: "ProcessId");
            RenameColumn(table: "dbo.ProcessOutputs", name: "ParentId", newName: "ProcessId");
            RenameColumn(table: "dbo.ProcessCapitals", name: "ProductId", newName: "CapitalId");
            RenameColumn(table: "dbo.ProcessInputs", name: "ProductId", newName: "InputId");
            RenameColumn(table: "dbo.ProcessOutputs", name: "ProductId", newName: "OutputId");
            DropPrimaryKey("dbo.ProcessCapitals");
            DropPrimaryKey("dbo.ProcessInputs");
            DropPrimaryKey("dbo.ProcessOutputs");
            AddPrimaryKey("dbo.ProcessCapitals", new[] { "ProcessId", "CapitalId" });
            AddPrimaryKey("dbo.ProcessInputs", new[] { "ProcessId", "InputId" });
            AddPrimaryKey("dbo.ProcessOutputs", new[] { "ProcessId", "OutputId" });
            DropColumn("dbo.ProcessCapitals", "Id");
            DropColumn("dbo.ProcessInputs", "Id");
            DropColumn("dbo.ProcessOutputs", "Id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.ProcessOutputs", "Id", c => c.Int(nullable: false, identity: true));
            AddColumn("dbo.ProcessInputs", "Id", c => c.Int(nullable: false, identity: true));
            AddColumn("dbo.ProcessCapitals", "Id", c => c.Int(nullable: false, identity: true));
            DropPrimaryKey("dbo.ProcessOutputs");
            DropPrimaryKey("dbo.ProcessInputs");
            DropPrimaryKey("dbo.ProcessCapitals");
            AddPrimaryKey("dbo.ProcessOutputs", "Id");
            AddPrimaryKey("dbo.ProcessInputs", "Id");
            AddPrimaryKey("dbo.ProcessCapitals", "Id");
            RenameColumn(table: "dbo.ProcessOutputs", name: "OutputId", newName: "ProductId");
            RenameColumn(table: "dbo.ProcessInputs", name: "InputId", newName: "ProductId");
            RenameColumn(table: "dbo.ProcessCapitals", name: "CapitalId", newName: "ProductId");
            RenameColumn(table: "dbo.ProcessOutputs", name: "ProcessId", newName: "ParentId");
            RenameColumn(table: "dbo.ProcessInputs", name: "ProcessId", newName: "ParentId");
            RenameColumn(table: "dbo.ProcessCapitals", name: "ProcessId", newName: "ParentId");
        }
    }
}
