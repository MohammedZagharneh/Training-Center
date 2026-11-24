using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.DTOs.RefreshToken;

namespace Domain.Enums
{
    public enum RefreshTokenStatus
    {
        Success, Failed
    }

    public record RefreshTokenResult(RefreshTokenStatus Status, RefreshResponseDto? Tokens);
        
}
