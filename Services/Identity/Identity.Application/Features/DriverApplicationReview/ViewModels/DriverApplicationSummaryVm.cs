using System;
using System.Collections.Generic;
using System.Text;

namespace Identity.Application.Features.DriverApplicationReview.ViewModels
{
    public record DriverApplicationSummaryVm(
    Guid Id,
    Guid UserId,
    string FullName,
    string Status,
    DateTime SubmittedAt
    );
}
