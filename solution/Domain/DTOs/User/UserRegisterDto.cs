using Domain.DTOs.Person;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.DTOs.User
{
    public class UserRegisterDto
    {
        [Required]
        [StringLength(100, MinimumLength = 5)]
        [RegularExpression(@"^[a-zA-Z0-9]+$")]
        public string Username { get; set; } = null!;

        [Required]
        [StringLength(100, MinimumLength = 8)]
        [RegularExpression(@"^\S+$")]
        public string Password { get; set; } = null!;

        [Required]
        public PersonCreateDto Person { get; set; } = null!;
    }
}
