using System;
using System.Collections.Generic;
using System.Text;

namespace Blocks.Contracts.Payment
{
    public sealed record CardSessionResultDto(
        Guid OrderId,
        string SessionId,
        string SessionUrl,
        string? SuccessUrl,
        string? CancelUrl,
        DateTime? ExpiresAt,
        decimal Amount,
        string Currency,
        DateTime EstimatedDeliveryAt
    );
}
