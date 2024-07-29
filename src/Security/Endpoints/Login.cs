using BCrypt.Net;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using PasswordManager.Common.Api;
using PasswordManager.Data;
using PasswordManager.Security.Services;

namespace PasswordManager.Security.Endpoints
{
    public class Login : IEndpoint
    {
        public static void Map(IEndpointRouteBuilder app) => app
            .MapPost("/login", Handle)
            .WithSummary("Logs in a user");

        public record Request(string Username, string Password);
        public record Response(string Token);

        private static async Task<Results<Ok<Response>, UnauthorizedHttpResult>> Handle(
            Request request,
            AppDbContext database,
            Jwt jwt,
            BcryptPasswordHasher passwordHasher,
            CancellationToken cancellationToken
        )
        {
          
            var user = await database.Users
                .SingleOrDefaultAsync(u => u.Name == request.Username, cancellationToken);
            
            if (user is null || !passwordHasher.Verify(request.Password, user.Password))
            {
                return TypedResults.Unauthorized();
            }

            var token = jwt.GenerateToken(user);
            var response = new Response(token);
            return TypedResults.Ok(response);
        }


        
    }
}
