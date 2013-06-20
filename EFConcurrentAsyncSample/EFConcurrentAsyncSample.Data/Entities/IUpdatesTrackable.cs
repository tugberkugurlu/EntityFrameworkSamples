using System;

namespace EFConcurrentAsyncSample.Data.Entities
{
    public interface IUpdatesTrackable
    {
        DateTimeOffset LastUpdatedOn { get; set; }
    }
}
