using System;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using System.Transactions;
using TestWithTransactionScope.Data.Core;
using TestWithTransactionScope.Data.Entities;
using Xunit;

namespace TestWithTransactionScope.Test
{
    public class ConfContextGeneralTests
    {
        [Fact]
        public void Should_Save_CreatedOn()
        {
            Person person = new Person
            {
                Name = "Tugberk",
                BirthDate = new DateTime(2008, 05, 23)
            };

            using (ConfContext ctx = new ConfContext())
            {
                using (new TransactionScope())
                {
                    ctx.Entry(person).State = EntityState.Added;
                    ctx.SaveChanges();

                    Assert.NotEqual(person.CreatedOn, default(DateTimeOffset));
                }
            }
        }

        [Fact]
        public void Should_Save_LastUpdatedOn_On_First_Save()
        {
            Person person = new Person
            {
                Name = "Tugberk",
                BirthDate = new DateTime(2008, 05, 23)
            };

            using (ConfContext ctx = new ConfContext())
            {
                using (new TransactionScope())
                {
                    ctx.Entry(person).State = EntityState.Added;
                    ctx.SaveChanges();

                    Assert.NotEqual(person.LastUpdatedOn, default(DateTimeOffset));
                }
            }
        }

        [Fact]
        public void Should_Save_LastUpdatedOn_On_Modified_Save()
        {
            Person person = new Person
            {
                Name = "Tugberk",
                BirthDate = new DateTime(2008, 05, 23)
            };

            using (ConfContext ctx = new ConfContext())
            {
                using (new TransactionScope())
                {
                    ctx.Entry(person).State = EntityState.Added;
                    ctx.SaveChanges();

                    DateTimeOffset firstLastModifiedDate = person.LastUpdatedOn;

                    person.Name = "Tugberk2";
                    ctx.Entry(person).State = EntityState.Modified;
                    ctx.SaveChanges();

                    Assert.True(person.LastUpdatedOn > firstLastModifiedDate);
                }
            }
        }

        [Fact]
        public void Should_Get_Related_Sessions()
        {
            Person person = new Person
            {
                Name = "Tugberk",
                BirthDate = new DateTime(2008, 05, 23),
                Sessions = new Collection<Session>()
            };

            using (ConfContext ctx = new ConfContext())
            {
                using (ctx.Database.BeginTransaction())
                {
                    person.Sessions.Add(new Session { Name = "Ses 1" });
                    person.Sessions.Add(new Session { Name = "Ses 2" });
                    person.Sessions.Add(new Session { Name = "Ses 3" });

                    ctx.People.Add(person);
                    ctx.SaveChanges();

                    Person retrievedPerson = ctx.People.Include(personEntity => personEntity.Sessions).FirstOrDefault();

                    Assert.NotNull(retrievedPerson);
                    Assert.Equal(3, retrievedPerson.Sessions.Count());
                }
            }
        }
    }
}