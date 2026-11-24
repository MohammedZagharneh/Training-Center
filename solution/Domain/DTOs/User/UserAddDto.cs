using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.DTOs.User
{
    public class UserAddDto
    {
        public string Username { get; set; } = null!;

        public string PasswordHash { get; set; } = null!;

        public RoleStatus Role {  get; set; }

        public bool IsActive { get; set; }

        public int PersonId { get; set; }
    }
}
