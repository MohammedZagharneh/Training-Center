using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Domain.DTOs.User
{
    public class ChangePasswordRequestDto
    {

        [Required]
        [StringLength(100, MinimumLength = 8)]
        [RegularExpression(@"^\S+$")]
        public string OldPassword { get; set; } = null!;

        [Required]
        [StringLength(100, MinimumLength = 8)]
        [RegularExpression(@"^\S+$")]
        public string NewPassword { get; set; } = null!;
    }

}
