using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.DTOs.RefreshToken
{
    public class RefreshTokenCreateDto
    {
        public string RefreshTokenHash { get; set; } = null!;
        public DateTime ExpiresAt { get; set; }

        public int UserId { get; set; }

    }
}
