using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.DTOs.RefreshToken
{
    public class LogoutRequestDto
    {
        [Required]
        [RegularExpression(@"^\S+$")]
        public string RefreshToken { get; set; } = null!;
    }
}
