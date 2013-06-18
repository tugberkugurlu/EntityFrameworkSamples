using System;
using System.ComponentModel.DataAnnotations;

namespace EFConcurrencyCheckSample.Entities
{
    public class Person : IEntity<int>, IUpdatesTrackable
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime BirthDate { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
        public DateTimeOffset LastUpdatedOn { get; set; }

        [Timestamp]
        public byte[] Timestamp { get; set; }
    }
}