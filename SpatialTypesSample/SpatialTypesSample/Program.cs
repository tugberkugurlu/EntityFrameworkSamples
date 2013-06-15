using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Data.Entity.Spatial;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpatialTypesSample {

    class Program {

        static void Main(string[] args) {

            // Get the DefaultCoordinateSystemId:
            // System.Data.Entity.Spatial.DbGeography.DefaultCoordinateSystemId

            // InsertItemWithDbGeographyValue();
            // InsertItemWithDbGeographyValueAdoNet();

            DbGeography targetLocation = DbGeography.FromText("POINT(28.231981 36.800385)");
            // GetNearestAccommodationProperties(targetLocation, 10);
            // GetNearestAccommodationPropertiesWithSpecificDistance(targetLocation, 5f);

            Console.ReadKey();
        }

        private static void InsertItemWithDbGeographyValue() {

            using (ReservationContext ctx = new ReservationContext()) {

                AccommodationProperty accommProperty = new AccommodationProperty {

                    Name = "Club & Hotel Letoonia",
                    // In order: POINT(Long Lat)
                    Location = DbGeography.FromText("POINT(29.097218832015983 36.63622153677499)")
                };

                ctx.AccommodationProperties.Add(accommProperty);
                ctx.SaveChanges();
            }
        }

        private static void GetNearestAccommodationProperties(DbGeography targetLocation, int takeTop) {

            // ref: http://www.west-wind.com/weblog/posts/2012/Jun/21/Basic-Spatial-Data-with-SQL-Server-and-Entity-Framework-50
            // quote: The STDistance function returns the straight line distance between the passed 
            //        in point and the point in the database field. The result for 
            //        SRID 4326 is always in meters.

            using (ReservationContext ctx = new ReservationContext()) {

                // Order by distance (nearest one is at the top)
                var accommProperties = (from accommProperty in ctx.AccommodationProperties
                                        let distance = accommProperty.Location.Distance(targetLocation)
                                        orderby distance
                                        select new { 

                                            Name = accommProperty.Name, 
                                            Distance = distance 

                                        }).Take(takeTop).ToArray();

                foreach (var accommProperty in accommProperties) {

                    Console.WriteLine("Name: {0}, Distance: {1} meters", accommProperty.Name, accommProperty.Distance);
                }
            }
        }

        private static void GetNearestAccommodationPropertiesWithSpecificDistance(DbGeography targetLocation, float distanceInKm) {

            using (ReservationContext ctx = new ReservationContext()) {

                float distanceInMeters = (distanceInKm * 1000);
                var accommProperties = (from accommProperty in ctx.AccommodationProperties
                                        let distance = accommProperty.Location.Distance(targetLocation)
                                        where distance <= distanceInMeters
                                        orderby distance
                                        select new {

                                            Name = accommProperty.Name,
                                            Distance = distance

                                        }).ToArray();

                foreach (var accommProperty in accommProperties) {

                    Console.WriteLine("Name: {0}, Distance: {1} meters", accommProperty.Name, accommProperty.Distance);
                }
            }
        }

        public void InsertItemWithDbGeographyValueAdoNet() {

            // ref: 
            // Geometry column: STGeomFromText and SRID (what is an SRID?)
            // http://stackoverflow.com/questions/373589/geometry-column-stgeomfromtext-and-srid-what-is-an-srid
            // Spatial Reference Identifiers (SRIDs): the DefaultCoordinateSystemId is 4326
            // http://msdn.microsoft.com/en-us/library/bb964707(v=sql.100).aspx
            // STGeomFromText (geometry Data Type)
            // http://msdn.microsoft.com/en-us/library/bb933823.aspx
            // Well-known text format (WKT)
            // http://en.wikipedia.org/wiki/Well-known_text
            // Create, Construct, and Query geography Instances
            // http://msdn.microsoft.com/en-us/library/bb895266.aspx

            /*
                DECLARE @0 AS NVARCHAR = 'Ece Saray Marina & Resort Hotel';
                DECLARE @1 AS geography = geography::STGeomFromText('POINT(29.100557230783124 36.622470691046324)', 4326);

                insert [dbo].[AccommodationProperties]([Name], [Location])
                values (@0, @1)
                select [Id]
                from [dbo].[AccommodationProperties]
                where @@ROWCOUNT > 0 and [Id] = scope_identity()
             */
        }
    }

    public class AccommodationProperty {

        public int Id { get; set; }
        public string Name { get; set; }
        public DbGeography Location { get; set; }
    }

    public class ReservationContext : DbContext {

        public ReservationContext() {

            this.Database.Log = Console.WriteLine;
        }

        public IDbSet<AccommodationProperty> AccommodationProperties { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder) {

            //// Optional: If you would like your database table names to be singular
            // modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }
    }
}