using System;

namespace EFConcurrentAsyncSample.Api.Models
{
    public class PersonDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime BirthDate { get; set; }
    }
}