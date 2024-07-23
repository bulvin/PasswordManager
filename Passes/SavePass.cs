using BCrypt.Net;
using Microsoft.AspNetCore.Http.HttpResults;
using PasswordManager.Common.Api;
using PasswordManager.Data;
using PasswordManager.Security.Encryption;

namespace PasswordManager.Passwords
{
    public class SavePass : IEndpoint
    {
        public static void Map(IEndpointRouteBuilder app) => app
            .MapPost("/", Handle)
            .WithSummary("Save a new pass");

        public record Request(string WebsiteUrl, string Username, string Passwd);
        public record Response(int Id);


        private static async Task<Results<Created<Response>, BadRequest>> Handle(Request request, AppDbContext database, Crypto crypto, CancellationToken cancellationToken)
        {
            bool created = Uri.TryCreate(request.WebsiteUrl, UriKind.Relative, out var nameUrl);
            if (!created) {
                return TypedResults.BadRequest();
            }

            var encryptPassword = crypto.EncryptString(request.Passwd);
           
            var pass = new Pass
            {
                WebsiteUrl = nameUrl!,
                Username = request.Username,
                Password = encryptPassword,
            };

            await database.Passes.AddAsync(pass, cancellationToken);
            await database.SaveChangesAsync(cancellationToken);

            return TypedResults.Created($"/api/passes/{pass.Id}", new Response(pass.Id));

        }


        
    }
}
