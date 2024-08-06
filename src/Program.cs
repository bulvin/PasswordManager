using Microsoft.EntityFrameworkCore;
using PasswordManager;
using PasswordManager.Data;
using PasswordManager.Passwords.Services;
using PasswordManager.Security.Encryption;
using Microsoft.IdentityModel.Tokens;
using PasswordManager.Security.Services;
using Microsoft.AspNetCore.Identity;
using PasswordManager.AI;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);


var newConfiguration = new ConfigurationBuilder()
         .AddJsonFile("appsettings.json", optional: true, reloadOnChange: false)
         .AddJsonFile("appsettings.development.json", optional: true, reloadOnChange: true)
         .AddEnvironmentVariables()
         .Build();


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.CustomSchemaIds(type => type.FullName?.Replace('+', '.'));
    options.InferSecuritySchemes();
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "Password Manager API", Version = "v1" });
    options.EnableAnnotations();

});

builder.Services.Configure<CryptOptions>(builder.Configuration.GetSection("Encryption"));

builder.Services.AddTransient<Crypto>();
builder.Services.AddSingleton<PasswordGeneratorFactory>();
builder.Services.AddSingleton<RandomPassword>();

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("Default"));
});

builder.Services.AddAuthentication().AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        IssuerSigningKey = Jwt.SecurityKey(builder.Configuration["Jwt:Key"]!),
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("Jwt"));
builder.Services.AddTransient<Jwt>();
builder.Services.AddTransient<BcryptPasswordHasher>();

var groqApiConfig = builder.Configuration.GetSection("GroqApi");
builder.Services.Configure<GroqApiClientOptions>(groqApiConfig);
builder.Services.AddHttpClient<IGroqApiClient, GroqApiClient>();


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapEndpoints();

app.Run();
