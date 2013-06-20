using System.Collections.Generic;

namespace EFConcurrentAsyncSample.Api.Models
{
    public class ConfDto
    {
        public IEnumerable<PersonDto> People { get; set; }
        public IEnumerable<RoomDto> Rooms { get; set; }
        public IEnumerable<SessionDto> Sessions { get; set; }
    }
}