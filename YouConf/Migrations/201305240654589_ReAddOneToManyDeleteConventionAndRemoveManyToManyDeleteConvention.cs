namespace YouConf.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ReAddOneToManyDeleteConventionAndRemoveManyToManyDeleteConvention : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Presentations", "ConferenceId", "dbo.Conferences");
            DropForeignKey("dbo.Speakers", "ConferenceId", "dbo.Conferences");
            DropForeignKey("dbo.ConferenceUserProfiles", "Conference_Id", "dbo.Conferences");
            DropForeignKey("dbo.ConferenceUserProfiles", "UserProfile_UserId", "dbo.UserProfile");
            DropForeignKey("dbo.SpeakerPresentations", "Speaker_Id", "dbo.Speakers");
            DropForeignKey("dbo.SpeakerPresentations", "Presentation_Id", "dbo.Presentations");
            DropIndex("dbo.Presentations", new[] { "ConferenceId" });
            DropIndex("dbo.Speakers", new[] { "ConferenceId" });
            DropIndex("dbo.ConferenceUserProfiles", new[] { "Conference_Id" });
            DropIndex("dbo.ConferenceUserProfiles", new[] { "UserProfile_UserId" });
            DropIndex("dbo.SpeakerPresentations", new[] { "Speaker_Id" });
            DropIndex("dbo.SpeakerPresentations", new[] { "Presentation_Id" });
            AddForeignKey("dbo.Presentations", "ConferenceId", "dbo.Conferences", "Id", cascadeDelete: true);
            AddForeignKey("dbo.Speakers", "ConferenceId", "dbo.Conferences", "Id", cascadeDelete: true);
            AddForeignKey("dbo.ConferenceUserProfiles", "Conference_Id", "dbo.Conferences", "Id");
            AddForeignKey("dbo.ConferenceUserProfiles", "UserProfile_UserId", "dbo.UserProfile", "UserId");
            AddForeignKey("dbo.SpeakerPresentations", "Speaker_Id", "dbo.Speakers", "Id");
            AddForeignKey("dbo.SpeakerPresentations", "Presentation_Id", "dbo.Presentations", "Id");
            CreateIndex("dbo.Presentations", "ConferenceId");
            CreateIndex("dbo.Speakers", "ConferenceId");
            CreateIndex("dbo.ConferenceUserProfiles", "Conference_Id");
            CreateIndex("dbo.ConferenceUserProfiles", "UserProfile_UserId");
            CreateIndex("dbo.SpeakerPresentations", "Speaker_Id");
            CreateIndex("dbo.SpeakerPresentations", "Presentation_Id");
        }
        
        public override void Down()
        {
            DropIndex("dbo.SpeakerPresentations", new[] { "Presentation_Id" });
            DropIndex("dbo.SpeakerPresentations", new[] { "Speaker_Id" });
            DropIndex("dbo.ConferenceUserProfiles", new[] { "UserProfile_UserId" });
            DropIndex("dbo.ConferenceUserProfiles", new[] { "Conference_Id" });
            DropIndex("dbo.Speakers", new[] { "ConferenceId" });
            DropIndex("dbo.Presentations", new[] { "ConferenceId" });
            DropForeignKey("dbo.SpeakerPresentations", "Presentation_Id", "dbo.Presentations");
            DropForeignKey("dbo.SpeakerPresentations", "Speaker_Id", "dbo.Speakers");
            DropForeignKey("dbo.ConferenceUserProfiles", "UserProfile_UserId", "dbo.UserProfile");
            DropForeignKey("dbo.ConferenceUserProfiles", "Conference_Id", "dbo.Conferences");
            DropForeignKey("dbo.Speakers", "ConferenceId", "dbo.Conferences");
            DropForeignKey("dbo.Presentations", "ConferenceId", "dbo.Conferences");
            CreateIndex("dbo.SpeakerPresentations", "Presentation_Id");
            CreateIndex("dbo.SpeakerPresentations", "Speaker_Id");
            CreateIndex("dbo.ConferenceUserProfiles", "UserProfile_UserId");
            CreateIndex("dbo.ConferenceUserProfiles", "Conference_Id");
            CreateIndex("dbo.Speakers", "ConferenceId");
            CreateIndex("dbo.Presentations", "ConferenceId");
            AddForeignKey("dbo.SpeakerPresentations", "Presentation_Id", "dbo.Presentations", "Id", cascadeDelete: true);
            AddForeignKey("dbo.SpeakerPresentations", "Speaker_Id", "dbo.Speakers", "Id", cascadeDelete: true);
            AddForeignKey("dbo.ConferenceUserProfiles", "UserProfile_UserId", "dbo.UserProfile", "UserId", cascadeDelete: true);
            AddForeignKey("dbo.ConferenceUserProfiles", "Conference_Id", "dbo.Conferences", "Id", cascadeDelete: true);
            AddForeignKey("dbo.Speakers", "ConferenceId", "dbo.Conferences", "Id");
            AddForeignKey("dbo.Presentations", "ConferenceId", "dbo.Conferences", "Id");
        }
    }
}
