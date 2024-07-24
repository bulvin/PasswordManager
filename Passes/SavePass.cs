using BCrypt.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using PasswordManager.Common.Api;
using PasswordManager.Data;
using PasswordManager.Security.Encryption;
using static PasswordManager.Passwords.SavePass;

namespace PasswordManager.Passwords
{
    public class SavePass : IEndpoint
    {
        public static void Map(IEndpointRouteBuilder app) => app
            .MapPost("/", Handle)
            .WithSummary("Save a new pass");

        public record Request(string WebsiteUrl, string Username, string Passwd);
        public record Response(int Id);


        private static async Task<Results<Created, BadRequest>> Handle(
            Request request, 
            AppDbContext database, 
            Crypto crypto,
            CancellationToken cancellationToken)
        {
            bool isValidUrl = Uri.TryCreate(request.WebsiteUrl, UriKind.RelativeOrAbsolute, out var nameUrl)
                       && nameUrl.IsWellFormedOriginalString();

            if (!isValidUrl)
            {
                return TypedResults.BadRequest();
            }

            var (encryptPassword, base64IV) = crypto.EncryptString(request.Passwd);

            var pass = new Pass
            {
                WebsiteUrl = nameUrl!,
                Username = request.Username,
                Password = encryptPassword,
                IV = base64IV,
            };

            await database.Passes.AddAsync(pass, cancellationToken);
            await database.SaveChangesAsync(cancellationToken);


            return TypedResults.Created();
                

        }


        
    }
}
