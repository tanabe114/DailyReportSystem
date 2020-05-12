namespace DailyReportSystem.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddReports1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Reports", "AttendanceTime", c => c.DateTime());
            AddColumn("dbo.Reports", "Leavingime", c => c.DateTime());
            AlterColumn("dbo.Reports", "NegotiationStatus", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Reports", "NegotiationStatus", c => c.String(nullable: false));
            DropColumn("dbo.Reports", "Leavingime");
            DropColumn("dbo.Reports", "AttendanceTime");
        }
    }
}
