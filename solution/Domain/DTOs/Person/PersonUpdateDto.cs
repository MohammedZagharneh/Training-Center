using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.DTOs.Person
{
    public class PersonUpdateDto
    {
        //uniqueness of email and phone number are handled in business layer

        [Required]
        [StringLength(50, MinimumLength = 1)]
        [RegularExpression(@"^[\p{L}\p{M}]+(?:[ '\-][\p{L}\p{M}]+)*$")]
        public string FirstName { get; set; } = null!;

        [Required]
        [StringLength(50, MinimumLength = 1)]
        [RegularExpression(@"^[\p{L}\p{M}]+(?:[ '\-][\p{L}\p{M}]+)*$")]
        public string LastName { get; set; } = null!;

        [EmailAddress]
        [StringLength(100, MinimumLength = 10)]
        public string? Email { get; set; }

        public DateOnly? BirthDate { get; set; }

        [StringLength(10, MinimumLength = 10)]
        [RegularExpression(@"^[0-9]{10}$")]
        public string? PhoneNumber { get; set; }
    }
}
