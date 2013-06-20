using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TestWithTransactionScope.Data.Entities
{
    public class Person : IEntity<int>, IUpdatesTrackable
    {
        public int Id { get; set; }

        [Required]
        [StringLength(25)]
        public string Name { get; set; }

        [DataType(DataType.Date)]
        public DateTime BirthDate { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
        public DateTimeOffset LastUpdatedOn { get; set; }

        public ICollection<Session> Sessions { get; set; }
    }
}