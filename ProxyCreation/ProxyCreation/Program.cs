using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProxyCreation {

    class Program {

        static void Main(string[] args) {

            // ref:
            // http://www.asp.net/mvc/tutorials/getting-started-with-ef-using-mvc/advanced-entity-framework-scenarios-for-an-mvc-web-application
            // http://blogs.msdn.com/b/adonet/archive/2011/02/02/using-dbcontext-in-ef-feature-ctp5-part-8-working-with-proxies.aspx
            
            // Init();
            // GetPeopleWithoutSessions();
            // GetSessionsWithoutPerson();
        }

        public static void Init() {

            using (var ctx = new ConfContext()) {

                ctx.People.Add(new Person {
                    Name = "FooBar",
                    Age = 26,
                    Sessions = new List<Session> { 
                        new Session { Name = "Ses1" },
                        new Session { Name = "Ses2" },
                        new Session { Name = "Ses3" },
                        new Session { Name = "Ses4" },
                        new Session { Name = "Ses5" },
                        new Session { Name = "Ses6" }
                    }
                });

                ctx.People.Add(new Person {
                    Name = "Tugberk",
                    Age = 26,
                    Sessions = new List<Session> { 
                        new Session { Name = "Ses1" },
                        new Session { Name = "Ses2" },
                        new Session { Name = "Ses3" },
                        new Session { Name = "Ses4" },
                        new Session { Name = "Ses5" },
                        new Session { Name = "Ses6" }
                    }
                });

                ctx.SaveChanges();
            }
        }

        public static void GetPeopleWithoutSessions() {

            // if Sessions property on Person is not virtual,
            // we will get NullReferenceException here

            // if Sessions property on Person is virtual and 
            // ProxyCreationEnabled == true, we will visit the database for each 
            // person's Sessions property.

            using (var ctx = new ConfContext()) {

                var people = ctx.People.ToArray();
                foreach (var person in people) {

                    Console.WriteLine(person.Name);
                    foreach (var session in person.Sessions) {

                        Console.WriteLine("\t Session: {0}", session.Name);
                    }
                }
            }
        }

        public static void GetSessionsWithoutPerson() {

            // if Person property on Session is not virtual,
            // we will get NullReferenceException here.

            // if Sessions property on Person is virtual and 
            // ProxyCreationEnabled == true, we will visit the database for each 
            // sessions's Person property.

            using (var ctx = new ConfContext()) {

                var sessions = ctx.Sessions.ToArray();
                foreach (var session in sessions) {

                    Console.WriteLine("{0} by {1}", session.Name, session.Person.Name);
                }
            }
        }
    }

    public class ConfContext : DbContext {

        public ConfContext() {

            Configuration.ProxyCreationEnabled = false;
            Database.Log = Console.WriteLine;
        }

        public IDbSet<Person> People { get; set; }
        public IDbSet<Session> Sessions { get; set; }
    }

    public class Person {

        public int Id { get; set; }
        public string Name { get; set; }
        public byte Age { get; set; }

        public virtual ICollection<Session> Sessions { get; set; }
    }

    public class Session {

        public int Id { get; set; }
        public int PersonId { get; set; }
        public string Name { get; set; }

        public virtual Person Person { get; set; }
    }
}