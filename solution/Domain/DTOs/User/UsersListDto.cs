using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.DTOs.User
{
    public class UsersListDto
    {
        public int UserId { get; set; }
        public string Username { get; set; } = null!;
        public string Role { get; set; } = null!;
        public bool IsActive { get; set; } 
        public int PersonId {get; set; }

        public string FirstName { get; set; } = null!;

        public string LastName { get; set; } = null!;

        public string NationalNumber { get; set; } = null!;

        public string? Email { get; set; }

        public string? PhoneNumber { get; set; }
    }
}
