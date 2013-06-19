using System;
using System.ComponentModel.DataAnnotations;

namespace EFConcurrencyCheckSample.Models
{
    public class PersonRequestModel
    {
        [Required]
        [StringLength(25)]
        public string Name { get; set; }

        [Required]
        public DateTime? BirthDate { get; set; }
    }
}