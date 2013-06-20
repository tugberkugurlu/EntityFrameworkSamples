namespace EFConcurrentAsyncSample.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Room : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Rooms",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        CreatedOn = c.DateTimeOffset(nullable: false),
                        LastUpdatedOn = c.DateTimeOffset(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Rooms");
        }
    }
}
