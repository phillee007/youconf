namespace YouConf.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddConferenceAdministrators : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ConferenceUserProfiles",
                c => new
                    {
                        Conference_Id = c.Int(nullable: false),
                        UserProfile_UserId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Conference_Id, t.UserProfile_UserId })
                .ForeignKey("dbo.Conferences", t => t.Conference_Id, cascadeDelete: true)
                .ForeignKey("dbo.UserProfile", t => t.UserProfile_UserId, cascadeDelete: false)
                .Index(t => t.Conference_Id)
                .Index(t => t.UserProfile_UserId);
            
        }
        
        public override void Down()
        {
            DropIndex("dbo.ConferenceUserProfiles", new[] { "UserProfile_UserId" });
            DropIndex("dbo.ConferenceUserProfiles", new[] { "Conference_Id" });
            DropForeignKey("dbo.ConferenceUserProfiles", "UserProfile_UserId", "dbo.UserProfile");
            DropForeignKey("dbo.ConferenceUserProfiles", "Conference_Id", "dbo.Conferences");
            DropTable("dbo.ConferenceUserProfiles");
        }
    }
}
