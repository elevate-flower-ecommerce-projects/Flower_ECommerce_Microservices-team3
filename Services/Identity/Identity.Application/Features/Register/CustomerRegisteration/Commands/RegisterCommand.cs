using Blocks.Contracts.Common;
using Identity.Application.Features.Register.CustomerRegisteration.ViewModels;
using Identity.Domain.Enums;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Identity.Application.Features.Register.CustomerRegisteration.Commands
{
    public sealed record RegisterCommand(
        string FirstName,
        string LastName,
        string Email,
        string Password,
        string PhoneNumber,
        Gender Gender)
        : IRequest<Result<RegisterResponseVm>>;
}
