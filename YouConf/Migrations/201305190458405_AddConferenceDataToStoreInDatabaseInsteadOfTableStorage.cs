namespace YouConf.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddConferenceDataToStoreInDatabaseInsteadOfTableStorage : DbMigration
    {
        public override void Up()
        {
            //CreateTable(
            //    "dbo.UserProfile",
            //    c => new
            //        {
            //            UserId = c.Int(nullable: false, identity: true),
            //            UserName = c.String(),
            //        })
            //    .PrimaryKey(t => t.UserId);
            
            CreateTable(
                "dbo.Conferences",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        HashTag = c.String(nullable: false, maxLength: 50),
                        Name = c.String(nullable: false, maxLength: 250),
                        Description = c.String(),
                        Abstract = c.String(nullable: false),
                        StartDate = c.DateTime(nullable: false),
                        EndDate = c.DateTime(nullable: false),
                        TimeZoneId = c.String(nullable: false),
                        HangoutId = c.String(maxLength: 50),
                        TwitterWidgetId = c.Long(),
                        AvailableToPublic = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Presentations",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 500),
                        Abstract = c.String(nullable: false),
                        StartTime = c.DateTime(nullable: false),
                        Duration = c.Int(nullable: false),
                        YouTubeVideoId = c.String(maxLength: 250),
                        ConferenceId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Conferences", t => t.ConferenceId, cascadeDelete: true)
                .Index(t => t.ConferenceId);
            
            CreateTable(
                "dbo.Speakers",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 200),
                        Bio = c.String(nullable: false),
                        Url = c.String(maxLength: 250),
                        Email = c.String(maxLength: 150),
                        AvatarUrl = c.String(maxLength: 250),
                        ConferenceId = c.Int(nullable: false),
                        Presentation_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Conferences", t => t.ConferenceId, cascadeDelete: true)
                .ForeignKey("dbo.Presentations", t => t.Presentation_Id)
                .Index(t => t.ConferenceId)
                .Index(t => t.Presentation_Id);
            
        }
        
        public override void Down()
        {
            DropIndex("dbo.Speakers", new[] { "Presentation_Id" });
            DropIndex("dbo.Speakers", new[] { "ConferenceId" });
            DropIndex("dbo.Presentations", new[] { "ConferenceId" });
            DropForeignKey("dbo.Speakers", "Presentation_Id", "dbo.Presentations");
            DropForeignKey("dbo.Speakers", "ConferenceId", "dbo.Conferences");
            DropForeignKey("dbo.Presentations", "ConferenceId", "dbo.Conferences");
            DropTable("dbo.Speakers");
            DropTable("dbo.Presentations");
            DropTable("dbo.Conferences");
            //DropTable("dbo.UserProfile");
        }
    }
}
