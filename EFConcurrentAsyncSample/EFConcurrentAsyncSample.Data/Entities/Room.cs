using System;

namespace EFConcurrentAsyncSample.Data.Entities
{
    public class Room : IEntity<int>, IUpdatesTrackable
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
        public DateTimeOffset LastUpdatedOn { get; set; }
    }
}
