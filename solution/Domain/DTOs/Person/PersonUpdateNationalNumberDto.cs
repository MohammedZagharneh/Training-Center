using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.DTOs.Person
{
    public class PersonUpdateNationalNumberDto
    {
        [Required]
        [StringLength(50, MinimumLength = 2)]
        [RegularExpression(@"^[a-zA-Z0-9]+$")]
        public string NationalNumber { get; set; } = null!;
    }
}
