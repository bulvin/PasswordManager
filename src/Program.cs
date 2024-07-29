using Microsoft.EntityFrameworkCore;
using PasswordManager;
using PasswordManager.Data;
using PasswordManager.Passwords.Services;
using PasswordManager.Security.Encryption;
using Microsoft.IdentityModel.Tokens;
using PasswordManager.Security.Services;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

var initialConfig = builder.Configuration;
var key = initialConfig["Encryption:Key"];

if (string.IsNullOrEmpty(key))
{
   
    AesKeyGenerator.GenerateKeyAndStoreInAppSettings();

    var newConfiguration = new ConfigurationBuilder()
         .AddJsonFile("appsettings.json", optional: true, reloadOnChange: false)
         .AddJsonFile("appsettings.development.json", optional: true, reloadOnChange: true)
         .AddEnvironmentVariables()
         .Build();
}

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.CustomSchemaIds(type => type.FullName?.Replace('+', '.'));
    options.InferSecuritySchemes();
});

builder.Services.Configure<CryptOptions>(builder.Configuration.GetSection("Encryption"));

builder.Services.AddTransient<Crypto>();
builder.Services.AddTransient<IGenerate, RandomPassword>();

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

builder.Services.AddAuthentication();
builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("Jwt"));
builder.Services.AddTransient<Jwt>();
builder.Services.AddTransient<BcryptPasswordHasher>();


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapEndpoints();

app.Run();
