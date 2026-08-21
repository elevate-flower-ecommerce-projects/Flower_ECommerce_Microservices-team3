using Identity.Application.Features.ForgotPassword;
using MediatR;

namespace Identity.Api.Features.Forgot_Password
{
    public static class ForgotPasswordEndpoint
    {
        public static IEndpointRouteBuilder MapForgotPasswordEndpoint(this IEndpointRouteBuilder app)
        {

            app.MapPost("/auth/forgot-password",
            async (ForgotPasswordCommand command,
                ISender sender,
                CancellationToken cancellationToken) =>
            {
                var result = await sender.Send(command, cancellationToken);

                return result;
            }).WithName("ForgotPassword")
              .WithTags("Authentication");

            return app;
        }
    }
}
