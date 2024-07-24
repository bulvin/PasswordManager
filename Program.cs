using Microsoft.EntityFrameworkCore;
using PasswordManager;
using PasswordManager.Data;
using PasswordManager.Security.Encryption;
using System.Buffers.Text;
using System.Security.Cryptography;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.CustomSchemaIds(type => type.FullName?.Replace('+', '.'));
    options.InferSecuritySchemes();
});

builder.Services.Configure<CryptOptions>(builder.Configuration.GetSection("Encryption"));
builder.Services.AddTransient<Crypto>();

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
