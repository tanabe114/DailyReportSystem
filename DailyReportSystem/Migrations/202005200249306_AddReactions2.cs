namespace DailyReportSystem.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddReactions2 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Reactions", "Category", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Reactions", "Category", c => c.Int(nullable: false));
        }
    }
}
