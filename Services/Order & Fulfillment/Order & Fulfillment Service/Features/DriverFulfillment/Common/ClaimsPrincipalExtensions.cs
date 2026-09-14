using System.Security.Claims;
using Blocks.Contracts.Security;

namespace Order___Fulfillment_Service.Features.DriverFulfillment.Common;

public static class ClaimsPrincipalExtensions
{
    public static Guid? GetDriverId(this ClaimsPrincipal user)
    {
        var rawId = user.FindFirstValue(ClaimTypes.NameIdentifier)
                    ?? user.FindFirstValue("sub")
                    ?? user.FindFirstValue(FlowerClaimTypes.CustomerId);

        return Guid.TryParse(rawId, out var id) ? id : null;
    }
}
