namespace YouConf.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddEmailToUserProfile : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.UserProfile", "Email", c => c.String(maxLength: 250));
        }
        
        public override void Down()
        {
            DropColumn("dbo.UserProfile", "Email");
        }
    }
}
