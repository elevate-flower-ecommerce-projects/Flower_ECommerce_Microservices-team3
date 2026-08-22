using Blocks.Contracts.Common;
using Identity.Application.Features.DriverApplication.SubmitApplication.DTOs;
using Identity.Application.Features.DriverApplication.SubmitApplication.ViewModels;
using MediatR;

namespace Identity.Application.Features.DriverApplication.SubmitApplication.Commands;

public record SubmitDriverApplicationCommand(SubmitDriverApplicationDto Dto)
    : IRequest<Result<SubmitDriverApplicationResponseVm>>;
