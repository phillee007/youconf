namespace YouConf.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddPresentationsToSpeaker : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Speakers", "Presentation_Id", "dbo.Presentations");
            DropIndex("dbo.Speakers", new[] { "Presentation_Id" });
            CreateTable(
                "dbo.SpeakerPresentations",
                c => new
                    {
                        Speaker_Id = c.Int(nullable: false),
                        Presentation_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Speaker_Id, t.Presentation_Id })
                .ForeignKey("dbo.Speakers", t => t.Speaker_Id, cascadeDelete: true)
                .ForeignKey("dbo.Presentations", t => t.Presentation_Id, cascadeDelete: false)
                .Index(t => t.Speaker_Id)
                .Index(t => t.Presentation_Id);
            
            DropColumn("dbo.Speakers", "Presentation_Id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Speakers", "Presentation_Id", c => c.Int());
            DropIndex("dbo.SpeakerPresentations", new[] { "Presentation_Id" });
            DropIndex("dbo.SpeakerPresentations", new[] { "Speaker_Id" });
            DropForeignKey("dbo.SpeakerPresentations", "Presentation_Id", "dbo.Presentations");
            DropForeignKey("dbo.SpeakerPresentations", "Speaker_Id", "dbo.Speakers");
            DropTable("dbo.SpeakerPresentations");
            CreateIndex("dbo.Speakers", "Presentation_Id");
            AddForeignKey("dbo.Speakers", "Presentation_Id", "dbo.Presentations", "Id");
        }
    }
}
