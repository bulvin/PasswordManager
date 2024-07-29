using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using PasswordManager.Common.Api;
using PasswordManager.Data;
using PasswordManager.Security.Encryption;

namespace PasswordManager.Passes.Endpoints
{
    public class GetPassById : IEndpoint
    {
        public static void Map(IEndpointRouteBuilder app) => app
            .MapGet("/{Id:guid}", Handle)
            .WithSummary("Gets a post by id");

        public record Request(Guid Id);
        public record Response(
            Guid Id,
            string Url,
            string Username,
            string Password
        );

        private static async Task<Results<Ok<Response>, NotFound>> Handle(
            [AsParameters] Request request,
            AppDbContext database,
            Crypto crypto,
            CancellationToken cancellationToken)

        {

            var pass = await database.Passes
               .Where(p => p.Id == request.Id)
               .Select(p => new Response
               (
                   p.Id,
                   p.WebsiteUrl.ToString(),
                   p.Username,
                   crypto.DecryptString(p.Password, p.IV)

               ))
               .SingleOrDefaultAsync(cancellationToken);

            return pass is null
                ? TypedResults.NotFound()
                : TypedResults.Ok(pass);

        }

    }
}
