using System;
using System.Collections.Generic;
using System.Text;

namespace Identity.Application.Features.Register.CustomerRegisteration.ViewModels
{
    public record RegisterResponseVm(
        Guid Id,
        bool IsSuccess
    );
}
