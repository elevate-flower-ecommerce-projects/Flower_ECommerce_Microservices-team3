using System;
using System.Collections.Generic;
using System.Text;

namespace Identity.Application.Features.Login.DTOs
{
    public record LoginRequestDto(
        string Email,
        string Password);
    
}
