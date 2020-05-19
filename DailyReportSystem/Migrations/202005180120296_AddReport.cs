namespace DailyReportSystem.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddReport : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Reports", "ApprovalStatus", c => c.Int());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Reports", "ApprovalStatus", c => c.String());
        }
    }
}
