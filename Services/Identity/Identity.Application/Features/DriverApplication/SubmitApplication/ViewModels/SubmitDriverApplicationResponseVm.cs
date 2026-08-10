using Identity.Domain.Enums;

namespace Identity.Application.Features.DriverApplication.SubmitApplication.ViewModels;

public record SubmitDriverApplicationResponseVm(
    Guid ApplicationId,
    Guid UserId,
    DriverApplicationStatus Status,
    string Message
);
