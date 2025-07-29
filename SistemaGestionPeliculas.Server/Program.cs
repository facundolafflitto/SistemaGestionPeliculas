using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SistemaGestionPeliculas_Data;
using SistemaGestionPeliculas.Server.Data;
using SistemaGestionPeliculas.TransferObject.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// 🔐 Configuración JWT desde variables de entorno (Railway las provee)
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

// 💾 DB Context con SQLite
builder.Services.AddDbContext<PeliculasContext>(options =>
    options.UseSqlite("Data Source=peliculas.db"));

// 🔧 Servicios adicionales
builder.Services.AddControllers();
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("http://localhost:5173") // Podés cambiarlo a tu frontend en producción
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

// 🧪 Inicialización de datos
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<PeliculasContext>();
    DbSeeder.SeedPeliculas(context);

    // Asignar admin a un usuario específico
    DbSeeder.SetUsuarioAdmin(context, "facundolafflitto@yahoo.com.ar");
}

// 🚀 Middleware
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// 🛠️ Railway espera que escuchemos en el puerto definido por $PORT
var port = Environment.GetEnvironmentVariable("PORT") ?? "5000";
app.Urls.Add($"http://*:{port}");

app.Run();
