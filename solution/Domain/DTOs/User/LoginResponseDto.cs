using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.DTOs.User
{
    public class LoginResponseDto
    {
        public string JwtToken { get; set; } = null!;

        public string RefreshToken { get; set; } = null!;
    }
}
