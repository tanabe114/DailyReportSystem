namespace DailyReportSystem.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddReports2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Reports", "LeavingTime", c => c.DateTime());
            DropColumn("dbo.Reports", "Leavingime");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Reports", "Leavingime", c => c.DateTime());
            DropColumn("dbo.Reports", "LeavingTime");
        }
    }
}
