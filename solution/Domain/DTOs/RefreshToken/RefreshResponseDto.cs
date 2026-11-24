using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.DTOs.RefreshToken
{
    public class RefreshResponseDto
    {
        public string NewJwtToken { get; set; } = null!;

        public string RefreshToken { get; set; } = null!;
    }
}
