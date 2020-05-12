namespace DailyReportSystem.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddReports : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Reports", "NegotiationStatus", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Reports", "NegotiationStatus");
        }
    }
}
