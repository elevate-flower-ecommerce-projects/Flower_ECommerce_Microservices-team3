using System;
using System.Collections.Generic;
using System.Text;

namespace Identity.Application.Features.ChangePassword.DTOs
{
    public record ChangePasswordRequestDto(
    string CurrentPassword,
    string NewPassword,
    string ConfirmNewPassword
);
}
