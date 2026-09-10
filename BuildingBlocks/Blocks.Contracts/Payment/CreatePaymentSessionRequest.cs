using System;
using System.Collections.Generic;
using System.Text;

namespace Blocks.Contracts.Payment
{
    public sealed record CreatePaymentSessionRequest(
        Guid OrderId,
        decimal Amount,
        string Currency,
        PaymentProvider Provider,
        DateTime EstimatedDeliveryAt,
        BillingData BillingData
    );
}
