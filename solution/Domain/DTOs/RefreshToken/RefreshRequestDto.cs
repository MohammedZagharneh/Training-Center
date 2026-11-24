using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Domain.DTOs.RefreshToken
{
    public class RefreshRequestDto
    {
        [Required]
        [RegularExpression(@"^\S+$")]
        public string RefreshToken { get; set; } = null!;
    }
}
