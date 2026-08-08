using System;
using System.Collections.Generic;
using System.Text;

namespace Identity.Application.Features.Register.CustomerRegisteration.DTOs
{
    public record RegisterRequestDto(
        string FirstName,
        string LastName,
        string Email,
        string PhoneNumber,
        string Gender,
        string Password,
        string ConfirmPassword
    );
}
