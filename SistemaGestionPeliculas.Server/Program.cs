using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SistemaGestionPeliculas_Data;
using SistemaGestionPeliculas.Server.Data;
using SistemaGestionPeliculas.TransferObject.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// 🔐 Configuración desde variables de entorno
var jwtKey = builder.Configuration["Jwt:Key"];
var jwtIssuer = builder.Configuration["Jwt:Issuer"];
var jwtAudience = builder.Configuration["Jwt:Audience"];

if (string.IsNullOrEmpty(jwtKey) || string.IsNullOrEmpty(jwtIssuer) || string.IsNullOrEmpty(jwtAudience))
{
    Console.WriteLine("=== Faltan variables de entorno para JWT ===");
    throw new InvalidOperationException("Faltan variables de entorno para JWT");
}

// Autenticación JWT
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

// Configuración de EF Core con SQL Server
builder.Services.AddDbContext<PeliculasContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Servicios y controladores
builder.Services.AddControllers();

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins(
            "https://sistema-gestion-peliculas.vercel.app",
            "https://sistema-gestion-peliculas-tm0oydq53-facundos-projects-26cddd25.vercel.app",
            "http://localhost:5173"
        )
        .AllowAnyHeader()
        .AllowAnyMethod();
        // Si usás cookies o tokens en cookie: .AllowCredentials();
    });
});

// Railway espera un puerto dinámico
var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
Console.WriteLine($"Puerto asignado: {port}");
builder.WebHost.UseUrls($"http://0.0.0.0:{port}");

var app = builder.Build();

// Seeding inicial de la base de datos
using (var scope = app.Services.CreateScope())
{
    try
    {
        var context = scope.ServiceProvider.GetRequiredService<PeliculasContext>();
        DbSeeder.SeedPeliculas(context);
        DbSeeder.SetUsuarioAdmin(context, "herber7748@hotmail.com");
    }
    catch (Exception ex)
    {
        Console.WriteLine("=== ERROR EN EL SEEDING ===");
        Console.WriteLine(ex.ToString());
    }
}

// Middleware
app.UseCors("AllowFrontend");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapGet("/", () => "API funcionando 🚀");

app.Run();
