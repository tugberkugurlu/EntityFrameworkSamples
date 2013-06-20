using System;

namespace TestWithTransactionScope.Data.Entities
{
    public interface IUpdatesTrackable
    {
        DateTimeOffset LastUpdatedOn { get; set; }
    }
}
