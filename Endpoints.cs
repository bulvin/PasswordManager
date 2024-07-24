using PasswordManager.Common.Api;
using PasswordManager.Passwords;
using System.Net.NetworkInformation;

namespace PasswordManager
{
    public static class Endpoints
    {
        public static void MapEndpoints(this WebApplication app)
        {
            var endpoints = app.MapGroup("")
                .WithOpenApi(); 

      
            endpoints.MapPassEndpoints();
        }

        private static void MapPassEndpoints(this IEndpointRouteBuilder app)
        {
        
            var passesGroup = app.MapGroup("/passes")
                .WithTags("Passes");

            passesGroup.MapEndpoint<SavePass>();

        }

        private static IEndpointRouteBuilder MapEndpoint<TEndpoint>(this IEndpointRouteBuilder app) where TEndpoint : IEndpoint
        {
            TEndpoint.Map(app);
            return app;
        }
    }
}
