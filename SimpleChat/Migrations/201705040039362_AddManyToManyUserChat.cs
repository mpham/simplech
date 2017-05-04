namespace SimpleChat.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddManyToManyUserChat : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Users", "Chat_ChatId", "dbo.Chats");
            DropIndex("dbo.Users", new[] { "Chat_ChatId" });
            CreateTable(
                "dbo.UserChats",
                c => new
                    {
                        UserChatId = c.Int(nullable: false, identity: true),
                        UserId = c.Int(nullable: false),
                        ChatId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.UserChatId)
                .ForeignKey("dbo.Chats", t => t.ChatId, cascadeDelete: true)
                .ForeignKey("dbo.Users", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.ChatId);
            
            DropColumn("dbo.Users", "Chat_ChatId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Users", "Chat_ChatId", c => c.Int());
            DropForeignKey("dbo.UserChats", "UserId", "dbo.Users");
            DropForeignKey("dbo.UserChats", "ChatId", "dbo.Chats");
            DropIndex("dbo.UserChats", new[] { "ChatId" });
            DropIndex("dbo.UserChats", new[] { "UserId" });
            DropTable("dbo.UserChats");
            CreateIndex("dbo.Users", "Chat_ChatId");
            AddForeignKey("dbo.Users", "Chat_ChatId", "dbo.Chats", "ChatId");
        }
    }
}
