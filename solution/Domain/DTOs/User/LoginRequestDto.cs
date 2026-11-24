using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Domain.DTOs.User
{
    public class LoginRequestDto
    {
        [Required]
        [StringLength(100, MinimumLength = 5)]
        [RegularExpression(@"^[a-zA-Z0-9]+$")]
        public string Username { get; set; } = null!;

        [Required]
        [StringLength(100, MinimumLength = 8)]
        [RegularExpression(@"^\S+$")]
        public string Password { get; set; } = null!;
    }
}
