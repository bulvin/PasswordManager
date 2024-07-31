using Microsoft.AspNetCore.Http.HttpResults;
using PasswordManager.AI;
using PasswordManager.Common.Api;
using PasswordManager.Passwords.Services;
using System.Text.Json.Nodes;

namespace PasswordManager.Passwords
{
    public class GenerateRandomPassword : IEndpoint
    {
        public static void Map(IEndpointRouteBuilder app) => app
           .MapPost("/", Handle)
           .WithSummary("Generates a password");

        public record PasswordGenerationRequest(
            PasswordType Type, 
            int Length, 
            bool RequireNumbers,
            bool RequireSymbols, 
            bool FullWords
        );

        public record Response(string Password);

        private static async Task<Ok<Response>> Handle(
           [AsParameters] PasswordGenerationRequest request,
           PasswordGeneratorFactory passwordGeneratorFactory
        )
        {
            var strategy = passwordGeneratorFactory.GetStrategy(request.Type);
            var generatedPassword = await strategy.GeneratePasswordAsync(request);
            var response = new Response(generatedPassword);

            return TypedResults.Ok(response);
        }
    }
}
