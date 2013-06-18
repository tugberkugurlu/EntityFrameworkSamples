using System;

namespace EFConcurrencyCheckSample.Entities {
    
    public interface IUpdatesTrackable {

        DateTimeOffset LastUpdatedOn { get; set; }
    }
}