using MediatR;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Identity.Api.Features.Forgot_Password
{
    public static class ResetPasswordEndpoint
    {
        public static IEndpointRouteBuilder MapResetPasswordEndpoint(this IEndpointRouteBuilder app)
        {

            app.MapPost("/auth/reset-password",
            async (
                ResetPasswordCommand command,
                ISender sender,
                CancellationToken cancellationToken) =>
            {
                var result = await sender.Send(command, cancellationToken);

                return result;
            }).WithName("ResetPassword")
              .WithTags("Authentication");

            return app;

        }
    }
}
