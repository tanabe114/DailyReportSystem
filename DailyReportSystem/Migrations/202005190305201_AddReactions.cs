namespace DailyReportSystem.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddReactions : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Reactions", "ReportId", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Reactions", "ReportId", c => c.String());
        }
    }
}
