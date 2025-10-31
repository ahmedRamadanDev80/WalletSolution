using Microsoft.EntityFrameworkCore;
using Wallet.Application.Interfaces;
using Wallet.Application.Services;
using Wallet.Infrastructure;
using Wallet.Infrastructure.Dapper;
using Wallet.Infrastructure.Data;
using Wallet.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers(); // Use controllers (non-minimal endpoints)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure DbContext - replace connection string in appsettings.Development.json later
builder.Services.AddDbContext<WalletDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection") ?? "Server=.;Database=WalletDemoDb;Trusted_Connection=True;"));


// Dapper options (from appsettings)
builder.Services.Configure<DapperOptions>(builder.Configuration.GetSection("DapperOptions"));

// Register repositories / unit of work / services
builder.Services.AddScoped<IWalletRepository, WalletRepository>();
builder.Services.AddScoped<IWalletTransactionRepository, WalletTransactionRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<WalletService>();
builder.Services.AddSingleton<IDbConnectionFactory, SqlConnectionFactory>();
builder.Services.AddScoped<IDapperWalletRepository, DapperWalletRepository>();

builder.Services.AddCors(opt => opt.AddPolicy("dev", p =>
  p.WithOrigins("http://localhost:5173").AllowAnyHeader().AllowAnyMethod()
));

var app = builder.Build();

// Configure middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Wallet API v1");
        options.RoutePrefix = string.Empty; // swagger at root: http://localhost:5120/
    });
}

app.UseCors("dev");

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers(); // Map controller routes

app.Run();
