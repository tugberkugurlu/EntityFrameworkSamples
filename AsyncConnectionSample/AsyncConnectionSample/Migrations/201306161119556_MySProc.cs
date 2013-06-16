namespace AsyncConnectionSample.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MySProc : DbMigration
    {
        private const string CreateMyProcString = @"
            CREATE PROCEDURE " + Constants.SProcName_GetPeopleByAge  + @" @Age INT
            AS
            BEGIN
                
                SET NOCOUNT ON;

                SELECT 
                    Id, Name, Age, BirthDate, CreatedOn 
                FROM 
                    [dbo].[People]
                WHERE 
                    Age = @Age;
            END";

        private const string DropMyProcString = 
            "DROP PROCEDURE " + Constants.SProcName_GetPeopleByAge + ";";

        public override void Up()
        {
            Sql(CreateMyProcString);
        }
        
        public override void Down()
        {
            Sql(DropMyProcString);
        }
    }
}