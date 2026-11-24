using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.DTOs.RefreshToken
{
    public class RefreshTokenDetailsDto
    {
        public int RefreshTokenId { get; set; }
        public int UserId { get; set; }

        public string Username { get; set; } = null!;

        public DateTime ExpiresAt { get; set; }
        public DateTime? RevokedAt { get; set; }
        
        public bool IsActiveUser { get; set; }

        public string UserRole { get; set; } = null!;
    }
}
