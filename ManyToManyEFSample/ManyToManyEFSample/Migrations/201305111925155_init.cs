namespace ManyToManyEFSample.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class init : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AccommodationProperties",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.AccommodationPropertyFeatures",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.AccommodationPropertyFeatureAccommodationProperties",
                c => new
                    {
                        AccommodationPropertyFeature_Id = c.Int(nullable: false),
                        AccommodationProperty_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.AccommodationPropertyFeature_Id, t.AccommodationProperty_Id })
                .ForeignKey("dbo.AccommodationPropertyFeatures", t => t.AccommodationPropertyFeature_Id, cascadeDelete: true)
                .ForeignKey("dbo.AccommodationProperties", t => t.AccommodationProperty_Id, cascadeDelete: true)
                .Index(t => t.AccommodationPropertyFeature_Id)
                .Index(t => t.AccommodationProperty_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AccommodationPropertyFeatureAccommodationProperties", "AccommodationProperty_Id", "dbo.AccommodationProperties");
            DropForeignKey("dbo.AccommodationPropertyFeatureAccommodationProperties", "AccommodationPropertyFeature_Id", "dbo.AccommodationPropertyFeatures");
            DropIndex("dbo.AccommodationPropertyFeatureAccommodationProperties", new[] { "AccommodationProperty_Id" });
            DropIndex("dbo.AccommodationPropertyFeatureAccommodationProperties", new[] { "AccommodationPropertyFeature_Id" });
            DropTable("dbo.AccommodationPropertyFeatureAccommodationProperties");
            DropTable("dbo.AccommodationPropertyFeatures");
            DropTable("dbo.AccommodationProperties");
        }
    }
}
