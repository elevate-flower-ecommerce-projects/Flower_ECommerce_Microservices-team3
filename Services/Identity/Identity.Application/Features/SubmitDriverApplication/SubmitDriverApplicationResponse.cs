using System;
using System.Collections.Generic;
using System.Text;
using Identity.Domain.Enums;

namespace Identity.Application.Features.Drivers.Commands.SubmitDriverApplication
{
    public record SubmitDriverApplicationResponse(
        Guid ApplicationId,
        DriverApplicationStatus Status
    );
}
