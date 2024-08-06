using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using PasswordManager.AI;
using PasswordManager.Common.Api;
using PasswordManager.Common.Api.Docs;
using PasswordManager.Passwords.Services;
using System.Text.Json.Nodes;

namespace PasswordManager.Passwords
{
    public class GeneratePassword : IEndpoint
    {
        public static void Map(IEndpointRouteBuilder app) => app
           .MapPost("/", Handle)
           .WithSummary("Generates a password")
           .WithOpenApi(operation => operation.ConfigureOpenApiOperation());

        public record PasswordGenerationRequest(
            int Length = 15,
            bool RequireSymbols = false,
            bool RequireNumbers = true,
            bool? FullWords = false,
            PasswordType Type = 0
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
