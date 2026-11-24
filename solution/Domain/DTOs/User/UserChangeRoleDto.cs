using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Domain.Enums;

namespace Domain.DTOs.User
{
    public class UserChangeRoleDto
    {
        [Required]
        [Range(1, int.MaxValue)]
        public int UserId { get; set; }

        [Required]
        [EnumDataType(typeof(RoleStatus))]
        public RoleStatus Role { get; set; }
    }
}
