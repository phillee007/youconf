namespace YouConf.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class NotSureWhatHasChangedSoIHaveToAddAMigration : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Presentations", "ConferenceId", "dbo.Conferences");
            DropForeignKey("dbo.Speakers", "ConferenceId", "dbo.Conferences");
            DropIndex("dbo.Presentations", new[] { "ConferenceId" });
            DropIndex("dbo.Speakers", new[] { "ConferenceId" });
            AddForeignKey("dbo.Presentations", "ConferenceId", "dbo.Conferences", "Id");
            AddForeignKey("dbo.Speakers", "ConferenceId", "dbo.Conferences", "Id");
            CreateIndex("dbo.Presentations", "ConferenceId");
            CreateIndex("dbo.Speakers", "ConferenceId");
        }
        
        public override void Down()
        {
            DropIndex("dbo.Speakers", new[] { "ConferenceId" });
            DropIndex("dbo.Presentations", new[] { "ConferenceId" });
            DropForeignKey("dbo.Speakers", "ConferenceId", "dbo.Conferences");
            DropForeignKey("dbo.Presentations", "ConferenceId", "dbo.Conferences");
            CreateIndex("dbo.Speakers", "ConferenceId");
            CreateIndex("dbo.Presentations", "ConferenceId");
            AddForeignKey("dbo.Speakers", "ConferenceId", "dbo.Conferences", "Id", cascadeDelete: true);
            AddForeignKey("dbo.Presentations", "ConferenceId", "dbo.Conferences", "Id", cascadeDelete: true);
        }
    }
}
