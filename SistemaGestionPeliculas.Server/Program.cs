using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SistemaGestionPeliculas_Data;
using SistemaGestionPeliculas.Server.Data;
using SistemaGestionPeliculas.TransferObject.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// 🔐 JWT Config desde variables de entorno
var jwtKey = builder.Configuration["Jwt:Key"];
var jwtIssuer = builder.Configuration["Jwt:Issuer"];
var jwtAudience = builder.Configuration["Jwt:Audience"];

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey!))
        };
    });

// 💾 Configuración de EF Core con SQLite
builder.Services.AddDbContext<PeliculasContext>(options =>
    options.UseSqlite("Data Source=peliculas.db"));

// 🔧 Servicios y controladores
builder.Services.AddControllers();

// 🌐 CORS: permite origenes específicos (Vercel y localhost para desarrollo)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins(
            "https://sistema-gestion-peliculas.vercel.app",
            "http://localhost:5173"
        )
        .AllowAnyHeader()
        .AllowAnyMethod();
        // Si usás cookies, también agregar: .AllowCredentials();
    });
});

// 👂 Railway espera un puerto dinámico
var port = Environment.GetEnvironmentVariable("PORT") ?? "5000";
builder.WebHost.UseUrls($"http://*:{port}");

var app = builder.Build();

// 🧪 Seeding inicial de la base de datos
using (var scope = app.Services.CreateScope())
{
    try
    {
        var context = scope.ServiceProvider.GetRequiredService<PeliculasContext>();
        DbSeeder.SeedPeliculas(context);
        DbSeeder.SetUsuarioAdmin(context, "facundolafflitto@yahoo.com.ar");
    }
    catch (Exception ex)
    {
        Console.WriteLine("=== ERROR EN EL SEEDING ===");
        Console.WriteLine(ex.ToString());
    }
}


// 🚀 Middleware
app.UseCors("AllowFrontend"); // 👈 se aplica la política CORS por nombre
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapGet("/", () => "API funcionando 🚀");


app.Run();
