using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using PasswordManager.Common.Api;
using PasswordManager.Data;
using PasswordManager.Security.Services;

namespace PasswordManager.Security.Endpoints
{
    public class Signup : IEndpoint
    {
        public static void Map(IEndpointRouteBuilder app) => app
            .MapPost("/signup", Handle)
            .WithSummary("Creates a new user account");

        public record Request(
            string Username,
            string Password
        );
        public record Response(string Token);

        private static async Task<Results<Ok<Response>, BadRequest>> Handle(
            Request request,
            AppDbContext database,
            Jwt jwt,
            CancellationToken cancellationToken
        )
        {
            var exists = await database.Users
                .AnyAsync(u => u.Name == request.Username, cancellationToken);
          
            if (exists) 
            {
                return TypedResults.BadRequest();
            }

            var user = new User
            {
                Name = request.Username,
                Password = request.Password,
            };

            await database.Users.AddAsync(user, cancellationToken);
            await database.SaveChangesAsync(cancellationToken);

            var token = jwt.GenerateToken(user);
            var response = new Response(token);

            return TypedResults.Ok(response);

        }
       
    }
}
