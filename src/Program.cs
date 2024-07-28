using Microsoft.EntityFrameworkCore;
using PasswordManager;
using PasswordManager.Data;
using PasswordManager.Passwords.Services;
using PasswordManager.Security.Encryption;


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

builder.Services.AddSingleton<Crypto>();
builder.Services.AddTransient<IGenerate, RandomPassword>();

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("Default"));
});
        

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapEndpoints();

app.Run();
