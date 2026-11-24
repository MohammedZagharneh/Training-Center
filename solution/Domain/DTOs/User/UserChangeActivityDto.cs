using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Domain.DTOs.User
{
    public class UserChangeActivityDto
    {
        [Required]
        [Range(1, int.MaxValue)]
        public int UserId { get; set; } 
    }
}
