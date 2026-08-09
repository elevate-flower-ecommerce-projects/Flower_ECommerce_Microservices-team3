using System;
using System.Collections.Generic;
using System.Text;

namespace Blocks.Contracts.Events
{
    public record DriverApplicationApprovedEvent(Guid UserId, string Email);
    public record DriverApplicationRejectedEvent(string Email, string Reason);
}
