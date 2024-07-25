using Microsoft.AspNetCore.Http.HttpResults;
using PasswordManager.Common.Api;
using System.Security.Cryptography;
using System.Text;

namespace PasswordManager.Passes
{
    public class GeneratePassword : IEndpoint
    {
        public static void Map(IEndpointRouteBuilder app) => app
            .MapPost("/password", Handle)
            .WithSummary("Generates a password");

        public record Response(string Password);

        public record Request(int Length);


        private static async Task<Ok<Response>> Handle([AsParameters] Request request)
        {
            var password = GenerateSecPassword(Math.Min(8, request.Length));

            var response = new Response(password.ToString());

            await Task.CompletedTask; 
            return TypedResults.Ok(response);
        }

        private static string GenerateSecPassword(int length)
        {
            const string validChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890!@#$%^&*()_-+=<>?";

            var password = new StringBuilder();

            using (var rng = RandomNumberGenerator.Create())
            {
                byte[] data = new byte[4];
                for (int i = 0; i < length; i++)
                {
                    rng.GetBytes(data);
                    uint randomValue = BitConverter.ToUInt32(data, 0);
                    password.Append(validChars[(int)(randomValue % (uint)validChars.Length)]);

                }
            }
            return password.ToString();
        }



    }
}
