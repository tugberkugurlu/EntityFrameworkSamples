using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsyncConnectionSample {

    class Program {

        static void Main(string[] args) {

            // ref:
            // ADO.NET Asynchronous Programming
            // http://msdn.microsoft.com/en-us/library/hh211418.aspx
            // SqlConnectionStringBuilder.AsynchronousProcessing Property
            // http://msdn.microsoft.com/en-us/library/system.data.sqlclient.sqlconnectionstringbuilder.asynchronousprocessing.aspx
            // Async processing flag required to use async?
            // http://forums.asp.net/t/1851612.aspx/1

            // InsertAsync();
            // GetAsync();
            // GetWithSqlQueryAsync();
            // GetThrougSProcWithSqlQueryAsync();

            Console.ReadLine();
        }

        public async static Task InsertAsync() {

            using (PeopleContext ctx = new PeopleContext()) {

                var person = new Person {
                    Name = "Tugberk",
                    Age = 26,
                    BirthDate = new DateTime(1987, 9, 19),
                    CreatedOn = DateTimeOffset.Now
                };

                ctx.People.Add(person);
                await ctx.SaveChangesAsync();
            }
        }

        public async static Task GetAsync() {

            using (PeopleContext ctx = new PeopleContext()) {

                var people = await ctx.People.ToArrayAsync();
                foreach (var person in people) {

                    Console.WriteLine(person.Name);
                }
            }
        }

        public async static Task GetWithSqlQueryAsync() {

            using (PeopleContext ctx = new PeopleContext()) {

                DbRawSqlQuery<Person> query = ctx.Database.SqlQuery<Person>("SELECT Id, Name, Age, BirthDate, CreatedOn FROM People");
                Person[] people = await query.ToArrayAsync();

                foreach (Person person in people) {

                    Console.WriteLine(person.Name);
                }
            }
        }

        public async static Task GetThrougSProcWithSqlQueryAsync() {

            // ref: http://stackoverflow.com/a/4874600/463785

            using (PeopleContext ctx = new PeopleContext()) {

                DbRawSqlQuery<Person> query = ctx.Database.SqlQuery<Person>(
                    string.Concat(Constants.SProcName_GetPeopleByAge, " @Age"), 
                    new SqlParameter("Age", 26));

                Person[] people = await query.ToArrayAsync();
                foreach (Person person in people) {

                    Console.WriteLine(person.Name);
                }
            }
        }
    }

    public class PeopleContext : DbContext {

        public PeopleContext() {

            Database.Log = Console.WriteLine;
        }

        public IDbSet<Person> People { get; set; }
    }

    public class Person {

        public int Id { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
        public DateTime BirthDate { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
    }
}