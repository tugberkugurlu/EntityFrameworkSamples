using System;

namespace TestWithTransactionScope.Data.Entities
{
    public class Session : IEntity<int>, IUpdatesTrackable
    {
        public int Id { get; set; }
        public int PersonId { get; set; }
        public string Name { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
        public DateTimeOffset LastUpdatedOn { get; set; }

        public Person Person { get; set; }
    }
}
