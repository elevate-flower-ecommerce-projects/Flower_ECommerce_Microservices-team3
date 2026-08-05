using System;
using System.Collections.Generic;
using System.Text;

namespace Identity.Application.Features.Register.ViewModels
{
    public record RegisterResponseVm(
        Guid Id,
        bool IsSuccess
    );
}
