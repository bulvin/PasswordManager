using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using PasswordManager.Common.Api;
using PasswordManager.Data;
using PasswordManager.Security.Encryption;

namespace PasswordManager.Passes
{
    public class UpdatePass : IEndpoint
    {
        public static void Map(IEndpointRouteBuilder app) => app
            .MapPut("/", Handle)
            .WithSummary("Updates a pass");

        public record Request(int Id, string Url, string Username, string Password);

        private static async Task<Ok> Handle(
            Request request,
            AppDbContext database,
            Crypto crypto,
            CancellationToken cancellationToken)
        {

            var pass = await database.Passes.SingleOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
            pass.WebsiteUrl = Uri.TryCreate(request.Url, UriKind.RelativeOrAbsolute, out var uri) ? uri : pass.WebsiteUrl;
            
            pass.Username = request.Username;
            
            var (updatedPassword, IV) = crypto.EncryptString(request.Password);
            pass.Password = updatedPassword;
            pass.IV = IV;

            await database.SaveChangesAsync(cancellationToken);
       
            return TypedResults.Ok();

        }
        
    }
}
