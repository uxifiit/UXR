namespace UXR.Models.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FixRelationsAndReplaceSessionNameInNodeStatusUpdateWithValue : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.NodeStatus", "SessionId", "dbo.Sessions");
            DropForeignKey("dbo.Nodes", "GroupId", "dbo.Groups");
            DropIndex("dbo.Nodes", new[] { "GroupId" });
            DropIndex("dbo.NodeStatus", new[] { "SessionId" });
            AddColumn("dbo.NodeStatus", "CurrentSession", c => c.String());
            AlterColumn("dbo.Nodes", "GroupId", c => c.Int(nullable: false));
            CreateIndex("dbo.Nodes", "GroupId");
            AddForeignKey("dbo.Nodes", "GroupId", "dbo.Groups", "Id", cascadeDelete: true);
            DropColumn("dbo.NodeStatus", "SessionId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.NodeStatus", "SessionId", c => c.Int());
            DropForeignKey("dbo.Nodes", "GroupId", "dbo.Groups");
            DropIndex("dbo.Nodes", new[] { "GroupId" });
            AlterColumn("dbo.Nodes", "GroupId", c => c.Int());
            DropColumn("dbo.NodeStatus", "CurrentSession");
            CreateIndex("dbo.NodeStatus", "SessionId");
            CreateIndex("dbo.Nodes", "GroupId");
            AddForeignKey("dbo.Nodes", "GroupId", "dbo.Groups", "Id");
            AddForeignKey("dbo.NodeStatus", "SessionId", "dbo.Sessions", "Id");
        }
    }
}
