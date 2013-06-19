using System;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;

namespace EFConcurrencyCheckSample.Entities
{
    public class ConfContext : DbContext
    {
        public ConfContext()
        {
            Database.Log = (msg) => Trace.TraceInformation(msg);
        }

        public IDbSet<Person> People { get; set; }

        public override int SaveChanges()
        {
            foreach (IEntity entity in ChangeTracker.Entries()
                .Where(e => e.State == EntityState.Added)
                .Select(e => e.Entity as IEntity))
            {
                entity.CreatedOn = DateTimeOffset.Now;

                IUpdatesTrackable updatesTrackableEntity = entity as IUpdatesTrackable;
                if (updatesTrackableEntity != null)
                {
                    updatesTrackableEntity.LastUpdatedOn = DateTimeOffset.Now;
                }
            }

            foreach (IUpdatesTrackable entity in ChangeTracker.Entries()
                .Where(e => e.Entity is IUpdatesTrackable && e.State == EntityState.Modified)
                .Select(e => e.Entity as IUpdatesTrackable))
            {
                entity.LastUpdatedOn = DateTimeOffset.Now;
            }

            return base.SaveChanges();
        }
    }
}