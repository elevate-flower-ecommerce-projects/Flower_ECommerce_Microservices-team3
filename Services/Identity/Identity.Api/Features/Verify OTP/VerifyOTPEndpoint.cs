using System.Reflection.Metadata.Ecma335;
using Identity.Application.Features.Verify_OTP;
using MediatR;

namespace Identity.Api.Features.Verify_OTP
{
    public static class VerifyOTPEndpoint
    {
        public static IEndpointRouteBuilder MapVerifyOTPEndpoint(this IEndpointRouteBuilder app)
        {
            app.MapPost("/auth/verify-otp",
    async (
        VerifyOtpCommand command,
        ISender sender,
        CancellationToken cancellationToken) =>
    {
        var result = await sender.Send(command,
                                       cancellationToken);

        return result;
    })
    .WithName("VerifyOtp")
    .WithTags("Authentication");
            return app;
        }

    }
}
