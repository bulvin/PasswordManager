using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using PasswordManager.Passwords.Services;

namespace PasswordManager.Common.Api.Docs
{
    public static class OpenApiExtensions
    {
        public static OpenApiSchema CreateSchema(string type, string description, bool nullable = false)
        {
            return new OpenApiSchema
            {
                Type = type,
                Description = description,
                Nullable = nullable
            };
        }

        public static OpenApiExample CreateExample(int length, bool requireSymbols, bool requireNumbers, bool fullWords, PasswordType type)
        {
            return new OpenApiExample
            {
                Summary = $"Example for {type} password",
                Value = new OpenApiObject
                {
                    ["length"] = new OpenApiInteger(length),
                    ["requireSymbols"] = new OpenApiBoolean(requireSymbols),
                    ["requireNumbers"] = new OpenApiBoolean(requireNumbers),
                    ["fullWords"] = new OpenApiBoolean(fullWords),
                    ["type"] = new OpenApiInteger((int)type)
                }
            };
        }

        public static OpenApiOperation ConfigureOpenApiOperation(this OpenApiOperation operation)
        {
            operation.Description = "Generates a password based on the specified criteria.";

            var properties = new Dictionary<string, OpenApiSchema>
            {
                ["length"] = CreateSchema("integer", "Length of the password"),
                ["requireSymbols"] = CreateSchema("boolean", "Require symbols in the password"),
                ["requireNumbers"] = CreateSchema("boolean", "Require numbers in the password"),
                ["fullWords"] = CreateSchema("boolean", "Use full words (only applicable for Memorable passwords)", true),
                ["type"] = new OpenApiSchema
                {
                    Type = "integer",
                    Enum = Enum.GetValues(typeof(PasswordType))
                               .Cast<int>()
                               .Select(v => new OpenApiInteger(v) as IOpenApiAny)
                               .ToList(),
                    Description = "Type of password to generate (0: Random, 1: Memorable, 2: Pin)"
                }
            };

            operation.RequestBody = new OpenApiRequestBody
            {
                Content =
            {
                ["application/json"] = new OpenApiMediaType
                {
                    Schema = new OpenApiSchema
                    {
                        Type = "object",
                        Properties = properties
                    },
                    Examples = new Dictionary<string, OpenApiExample>
                    {
                        ["Random Password Example"] = CreateExample(15, false, true, false, PasswordType.Random),
                        ["Memorable Password Example"] = CreateExample(5, false, false, true, PasswordType.Memorable)
                    }
                }
            }
            };

            return operation;
        }
    }
}
