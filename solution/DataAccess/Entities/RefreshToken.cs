using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Entities
{
    public partial class RefreshToken
    {
        public int RefreshTokenId {get; set;}

        public string RefreshTokenHash { get; set; } = null!;

        public DateTime ExpiresAt {get; set;}

        public DateTime? RevokedAt {get; set;}

        public int UserId {get; set;}

        public virtual User User { get; set; } = null!;
    }
}
