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

        public record Request(
           int Length,
           bool RequireSymbols,
           bool RequireNumbers
        );

        public record Response(string Password);

        private static async Task<Ok<Response>> Handle(
            [AsParameters] Request request,
            RandomPassword randomPassword,
            IGroqApiClient groqClient)
        {
            // var password = randomPassword.Generate(request.Length, request.RequireNumbers, request.RequireSymbols);

            var groqRequest = new JsonObject
            {
                ["model"] = "llama3-8b-8192",
                ["temperature"] = 1.5,
                ["max_tokens"] = 150,
                ["top_p"] = 0.9,
                ["frequency_penalty"] = 0.8,
                ["presence_penalty"] = 0.6,
                ["stop"] = "\n",
                ["messages"] = new JsonArray
                {
                    new JsonObject
                    {
                        ["role"] = "system",
                        ["content"] = "You are a password generator. Your task is to create unique, memorable, and secure passwords."
                    },
                    new JsonObject
                    {
                        ["role"] = "user",
                        ["content"] = $@"Generate a memorable password with the following criteria:
            - Exactly {request.Length} unique words
            - Unrelated words
            - Replace some characters with fitting symbols (e.g., 'a' with '@', 's' with '$'): {(request.RequireSymbols ? "yes" : "no")}
            - Add numbers to words: {(request.RequireNumbers ? "yes" : "no")}
            - Split words using '-'
            - Mix of common and uncommon words
            - Words from various categories (e.g., animals, objects, actions, adjectives, foods)
            - Ensure no word is repeated
            Respond with only the generated password."
                    }
                }
            };

            var result = await groqClient.CreateChatCompletionAsync(groqRequest);
            var generatedPassword = result?["choices"]?[0]?["message"]?["content"]?.ToString().TrimEnd() ?? "No response found";

            var response = new Response(generatedPassword);

            await Task.CompletedTask;
            return TypedResults.Ok(response);
        }
    }
}
