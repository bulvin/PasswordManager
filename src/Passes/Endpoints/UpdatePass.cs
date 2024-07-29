using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using PasswordManager.Common.Api;
using PasswordManager.Data;
using PasswordManager.Security.Encryption;

namespace PasswordManager.Passes.Endpoints
{
    public class UpdatePass : IEndpoint
    {
        public static void Map(IEndpointRouteBuilder app) => app
            .MapPut("/", Handle)
            .WithSummary("Updates a pass");

        public record Request(Guid Id, string Url, string Username, string Password);

        private static async Task<Results<Ok, BadRequest>> Handle(
            Request request,
            AppDbContext database,
            Crypto crypto,
            CancellationToken cancellationToken)
        {

            var pass = await database.Passes.SingleOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

            if (pass == null) return TypedResults.BadRequest();

            if (request.Url != null)
            {
                pass.WebsiteUrl = Uri.TryCreate(request.Url, UriKind.RelativeOrAbsolute, out var uri) ? uri : pass.WebsiteUrl;
            }

            if (string.IsNullOrEmpty(request.Password))
            {
                var (updatedPassword, IV) = crypto.EncryptString(request.Password);
                pass.Password = updatedPassword;
                pass.IV = IV;
            }

            if (request.Username != null)
            {
                pass.Username = request.Username;
            }

            await database.SaveChangesAsync(cancellationToken);

            return TypedResults.Ok();

        }

    }
}
