using System;
using System.Collections.Generic;
using System.Text;

namespace Identity.Application.Features.Login.ViewModels
{
    public record LoginResponseVM(
        string AccessToken,
        string RefreshToken,
        int ExpiresIn,
        string Role,
        string? DriverStatus);
    
}
