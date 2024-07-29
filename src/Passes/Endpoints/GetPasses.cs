using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using PasswordManager.Common.Api;
using PasswordManager.Data;

namespace PasswordManager.Passes.Endpoints
{
    public class GetPasses : IEndpoint
    {
        public static void Map(IEndpointRouteBuilder app) => app
            .MapGet("/", Handle)
            .WithSummary("Gets all passes");

        public record Response(
            Guid Id,
            string Url,
            string Username
        );

        private static async Task<List<Response>> Handle(
            AppDbContext database,
            CancellationToken cancellationToken
        )
        {
            return await database.Passes
                .Select(p => new Response
                (
                    p.Id,
                    p.WebsiteUrl.OriginalString,
                    p.Username
                 ))
                .ToListAsync(cancellationToken);

        }

    }
}
