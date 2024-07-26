using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using PasswordManager.Common.Api;
using PasswordManager.Data;

namespace PasswordManager.Passes
{
    public class DeletePass : IEndpoint
    {
        public static void Map(IEndpointRouteBuilder app) => app
            .MapDelete("/{id}", Handle)
            .WithSummary("Deletes a pass");

        public record Request(int Id);

        private static async Task<Results<Ok, NotFound>> Handle(
           [AsParameters] Request request,
            AppDbContext database,
            CancellationToken cancellationToken
            )
        {
            var rowsDeleted = await database.Passes
                .Where(p => p.Id == request.Id)
                .ExecuteDeleteAsync(cancellationToken);

            return rowsDeleted == 1
                ? TypedResults.Ok()
                : TypedResults.NotFound();
        }

        
    }
}
