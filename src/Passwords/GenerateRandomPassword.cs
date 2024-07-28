using Microsoft.AspNetCore.Http.HttpResults;
using PasswordManager.Common.Api;
using PasswordManager.Passwords.Services;

namespace PasswordManager.Passwords
{
    public class GenerateRandomPassword : IEndpoint
    {
        public static void Map(IEndpointRouteBuilder app) => app
           .MapPost("/", Handle)
           .WithSummary("Generates a password");

        public record Request(
           int Length,
           bool RequireSymbols,
           bool RequireNumbers
        );

        public record Response(string Password);

       

        private static async Task<Ok<Response>> Handle(
            [AsParameters] Request request,
            RandomPassword randomPassword)
        {
            var password = randomPassword.Generate(request.Length, request.RequireNumbers, request.RequireSymbols);

            var response = new Response(password.ToString());

            await Task.CompletedTask;
            return TypedResults.Ok(response);
        }
    }
}
