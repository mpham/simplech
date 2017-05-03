namespace SimpleChat.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RenameUserId : DbMigration
    {
        public override void Up()
        {
            DropPrimaryKey("dbo.Users");
            DropColumn("dbo.Users", "Id");
            AddColumn("dbo.Users", "UserId", c => c.Int(nullable: false, identity: true));
            AddPrimaryKey("dbo.Users", "UserId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Users", "Id", c => c.Int(nullable: false, identity: true));
            DropPrimaryKey("dbo.Users");
            DropColumn("dbo.Users", "UserId");
            AddPrimaryKey("dbo.Users", "Id");
        }
    }
}
