namespace EFConcurrentAsyncSample.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class init : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.People",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 25),
                        BirthDate = c.DateTime(nullable: false, storeType: "date"),
                        CreatedOn = c.DateTimeOffset(nullable: false),
                        LastUpdatedOn = c.DateTimeOffset(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Sessions",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        PersonId = c.Int(nullable: false),
                        Name = c.String(),
                        CreatedOn = c.DateTimeOffset(nullable: false),
                        LastUpdatedOn = c.DateTimeOffset(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.People", t => t.PersonId, cascadeDelete: true)
                .Index(t => t.PersonId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Sessions", "PersonId", "dbo.People");
            DropIndex("dbo.Sessions", new[] { "PersonId" });
            DropTable("dbo.Sessions");
            DropTable("dbo.People");
        }
    }
}
