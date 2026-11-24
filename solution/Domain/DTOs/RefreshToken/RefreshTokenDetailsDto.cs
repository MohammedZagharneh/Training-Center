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
        public DateTime ExpiresAt { get; set; }
        public DateTime? RevokedAt { get; set; }
        
        public bool IsActive { get; set; }

        public string Role { get; set; } = null!;
    }
}
