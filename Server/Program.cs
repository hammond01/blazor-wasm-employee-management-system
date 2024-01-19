using Microsoft.EntityFrameworkCore;
using ServerLibrary.Data;
using ServerLibrary.Helper;
using ServerLibrary.Repositories.Contracts;
using ServerLibrary.Repositories.Implementation;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<AppDbContext>(
    options =>
        options.UseSqlServer(
            builder.Configuration.GetConnectionString("SQL")
                ?? throw new InvalidOperationException("Connection string 'SQL' not found.")
        )
);
//config JWT credentials
builder.Services.Configure<JWTSection>(builder.Configuration.GetSection("Appsettings"));
var secretKey = builder.Configuration["Appsettings:SecretKey"];
var secretKeyBytes = Encoding.UTF8.GetBytes(secretKey!);

builder.Services.AddScoped<IUserAccount, UserAccountRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
