using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Identity.Application.Features.Login.Queries
{
    public record CheckRateLimitQuery(
        string Email,
        string IpAddress
        ):IRequest<bool>;
    
}
