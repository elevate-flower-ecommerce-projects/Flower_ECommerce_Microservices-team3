using Blocks.Contracts.Common;
using Identity.Application.Features.DriverApplicationReview.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Identity.Application.Features.DriverApplicationReview.Commands
{
    public record CreateDriverCommand(CreateDriverDto Data) : IRequest<Result<bool>>;
}
