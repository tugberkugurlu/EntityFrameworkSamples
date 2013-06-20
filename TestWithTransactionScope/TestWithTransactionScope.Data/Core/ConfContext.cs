using System;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Configuration.Properties.Primitive;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Reflection;
using TestWithTransactionScope.Data.Entities;

namespace TestWithTransactionScope.Data.Core
{
    public class ConfContext : DbContext
    {
        public ConfContext()
            : base("ConfContextDb")
        {
            Configuration.ProxyCreationEnabled = false;
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

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Conventions.Add(new DataTypePropertyAttributeConvention());
        }
    }

    public class DataTypePropertyAttributeConvention : AttributeConfigurationConvention<PropertyInfo, PrimitivePropertyConfiguration, DataTypeAttribute>
    {
        public override void Apply(PropertyInfo memberInfo, PrimitivePropertyConfiguration configuration, DataTypeAttribute attribute)
        {
            if (attribute.DataType == DataType.Date)
            {
                configuration.ColumnType = "Date";
            }
        }
    }
}