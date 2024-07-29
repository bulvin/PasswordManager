using PasswordManager.Common.Api;
using PasswordManager.Passes.Endpoints;
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
            endpoints.MapPasswordEndpoints();
        }

        private static void MapPassEndpoints(this IEndpointRouteBuilder app)
        {
        
            var passesGroup = app.MapGroup("/passes")
                .WithTags("Passes");

            passesGroup
                .MapEndpoint<SavePass>()
                .MapEndpoint<GetPassById>()
                .MapEndpoint<UpdatePass>()
                .MapEndpoint<DeletePass>()
                .MapEndpoint<GetPasses>();

        }
        private static void MapPasswordEndpoints(this IEndpointRouteBuilder app)
        {
            var passwordGroup = app.MapGroup("/password-generator")
                .WithTags("Passwords");

            passwordGroup.MapEndpoint<GenerateRandomPassword>();
        }

        private static IEndpointRouteBuilder MapEndpoint<TEndpoint>(this IEndpointRouteBuilder app) where TEndpoint : IEndpoint
        {
            TEndpoint.Map(app);
            return app;
        }
    }
}
