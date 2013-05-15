using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManyToManyEFSample {

    class Program {

        static void Main(string[] args) {

            //InsertWithFeatures();
            InvokeInsertWithExistingFeatures();
            //RemoveByCheckingItFirst();
            //GetAHotelWithFeatures();

            Console.ReadLine();
        }

        // Helper for Error Messages

        static void GenerateErrorOutput(DataException dataException) {

            if (dataException == null) {

                throw new ArgumentNullException("dataException");
            }

            int innerLevel = 0;
            Exception currentException = dataException;
            while (currentException != null) {

                WriteIndents(innerLevel);
                Console.WriteLine("Type: {0} | Message: {1}", currentException.GetType(), currentException.Message);
                SqlException sqlException = currentException as SqlException;
                if (sqlException != null) {

                    WriteIndents(innerLevel);
                    GenerateSqlExceptionOutput(sqlException, innerLevel);
                }

                currentException = currentException.InnerException;
                innerLevel++;
                if (currentException != null) {

                    WriteIndents(innerLevel);
                    Console.WriteLine("Inner Exception:");
                }
            }
        }

        static void GenerateSqlExceptionOutput(SqlException sqlException, int innerLevel) {
            
            // Full SqlException.Number list: http://forums.asp.net/t/1575883.aspx/1
            // The following query will get the error message number and message:
            // SELECT * FROM master.dbo.sysmessages WHERE msglangid = 1033;

            Console.WriteLine("SqlException:");

            StringBuilder errorMessages = new StringBuilder();
            for (int i = 0; i < sqlException.Errors.Count; i++) {

                errorMessages.Append("Index #" + i + "\n" +
                    "Message: " + sqlException.Errors[i].Message + "\n" +
                    "Error Number: " + sqlException.Errors[i].Number + "\n" +
                    "LineNumber: " + sqlException.Errors[i].LineNumber + "\n" +
                    "Source: " + sqlException.Errors[i].Source + "\n" +
                    "Procedure: " + sqlException.Errors[i].Procedure + "\n");
            }

            WriteIndents(innerLevel);
            Console.WriteLine(errorMessages.ToString());
        }

        static void WriteIndents(int level) {

            for (int i = 0; i < level; i++) {

                Console.Write("\t");
            }
        }

        // Invoke Methods

        static void InvokeInsertWithExistingFeatures() {

            try {

                InsertWithExistingFeatures();
            }
            catch (DbUpdateException dbUpdateException) {

                GenerateErrorOutput(dbUpdateException);
            }
        }

        // Actual methods

        static void InsertWithFeatures() {

            AccommodationProperty accProp = new AccommodationProperty {
                Name = "Aqua Hotel",
                AccommodationPropertyFeatures = new List<AccommodationPropertyFeature> { 
                    new AccommodationPropertyFeature { Name = "Open Pool" },
                    new AccommodationPropertyFeature { Name = "Closed Pool" }
                }
            };

            using (var ctx = new ReservationSystemContext()) {

                ctx.AccommodationProperties.Add(accProp);
                ctx.SaveChanges();
            }
        }

        static void InsertWithExistingFeatures() {

            // Existing feature Ids: 1, 2
            var feature1 = new AccommodationPropertyFeature { Id = 5 };
            var feature2 = new AccommodationPropertyFeature { Id = 6 };

            using (var ctx = new ReservationSystemContext()) {

                var dbSet = ctx.Set<AccommodationPropertyFeature>();
                dbSet.Attach(feature1);
                dbSet.Attach(feature2);

                AccommodationProperty accProp = new AccommodationProperty {
                    Name = "Bar Hotel",
                    AccommodationPropertyFeatures = new List<AccommodationPropertyFeature> { 
                        feature1, feature2
                    }
                };

                ctx.AccommodationProperties.Add(accProp);
                ctx.SaveChanges();
            }

            using (var ctx = new ReservationSystemContext()) {

                var dbSet = ctx.Set<AccommodationPropertyFeature>();
                dbSet.Attach(feature1);
                dbSet.Attach(feature2);

                AccommodationProperty accProp = new AccommodationProperty {
                    Name = "Foo Hotel",
                    AccommodationPropertyFeatures = new List<AccommodationPropertyFeature> { 
                        feature1, feature2
                    }
                };

                ctx.AccommodationProperties.Add(accProp);
                ctx.SaveChanges();
            }
        }

        static void RemoveByCheckingItFirst() { 

            // Remove feature 1 from hotel 1
            using (var ctx = new ReservationSystemContext()) {

                // AccommodationProperty accommProperty = ctx.AccommodationProperties.FirstOrDefault(accProp => accProp.Id == 1);
                var accommProperty = ctx
                    .AccommodationProperties
                    .Include(accProp => accProp.AccommodationPropertyFeatures)
                    .Where(accProp => accProp.Id == 1)
                    .Select(accProp => new { AccProp = accProp, Feature = accProp.AccommodationPropertyFeatures.FirstOrDefault(accFeature => accFeature.Id == 1) })
                    .FirstOrDefault();

                if (accommProperty != null && accommProperty.AccProp != null && accommProperty.Feature != null) {

                    // If we do this, we will first get all the AccommodationPropertyFeatures 
                    // for the related hotel which is not I want. That's why we are doing the above
                    // weird query which is most optimized way for this operation.
                    // AccommodationPropertyFeature feature1 = 
                    //    accommProperty.AccommodationPropertyFeatures.FirstOrDefault(accFeature => accFeature.Id == 1);

                    accommProperty.AccProp.AccommodationPropertyFeatures.Remove(accommProperty.Feature);
                    ctx.SaveChanges();
                }
            }
        }

        static void GetAHotelWithFeatures() {

            using (var ctx = new ReservationSystemContext()) {
            }
        }
    }

    public static class DataExceptionExtensions {

        public static SqlException ExtractSqlException(this DataException dataException) {

            Exception currentException = dataException;
            while (currentException != null) {

                SqlException sqlException = currentException as SqlException;
                if (sqlException != null) {

                    return sqlException;
                }

                currentException = currentException.InnerException;
            }

            return null;
        }
    }

    public class ReservationSystemContext : DbContext {

        public IDbSet<AccommodationProperty> AccommodationProperties { get; set; }
        public IDbSet<AccommodationPropertyFeature> AccommodationPropertyFeatures { get; set; }
    }

    public class AccommodationProperty {

        public int Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<AccommodationPropertyFeature> AccommodationPropertyFeatures { get; set; }
    }

    public class AccommodationPropertyFeature {

        public int Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<AccommodationProperty> AccommodationProperties { get; set; }
    }
}