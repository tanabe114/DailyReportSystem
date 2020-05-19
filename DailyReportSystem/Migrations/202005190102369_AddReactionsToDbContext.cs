namespace DailyReportSystem.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddReactionsToDbContext : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Reactions",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        EmployeeId = c.String(),
                        ReportId = c.String(),
                        Category = c.Int(nullable: false),
                        CreatedAt = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Reactions");
        }
    }
}
