using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SistemaGestionPeliculas_Data;
using SistemaGestionPeliculas.Server.Data;
using System.Text;

// 👇 agrega estos usings para las opciones JSON
using System.Text.Json;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// ===== JWT =====
var jwtKey = builder.Configuration["Jwt:Key"];
var jwtIssuer = builder.Configuration["Jwt:Issuer"];
var jwtAudience = builder.Configuration["Jwt:Audience"];
if (string.IsNullOrEmpty(jwtKey) || string.IsNullOrEmpty(jwtIssuer) || string.IsNullOrEmpty(jwtAudience))
    throw new InvalidOperationException("Faltan variables de entorno para JWT");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(o =>
    {
        o.TokenValidationParameters = new TokenValidationParameters
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

// ===== DB =====
builder.Services.AddDbContext<PeliculasContext>(opt =>
    opt.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// ===== Controllers + JSON (evita ciclos y usa camelCase) =====
builder.Services.AddControllers()
    .AddJsonOptions(o =>
    {
        o.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles; // evita 500 por ciclos
        o.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase; // Id -> id
    });

// ===== CORS (prod) =====
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy
            .WithOrigins(
                "https://sistema-gestion-peliculas.vercel.app",
                "http://localhost:5173"
            )
            // habilita cualquier subdominio *.vercel.app (previews)
            .SetIsOriginAllowed(origin =>
            {
                try { return new Uri(origin).Host.EndsWith("vercel.app"); }
                catch { return false; }
            })
            .AllowAnyHeader()
            .AllowAnyMethod();
        // sin AllowCredentials porque usás Bearer (no cookies)
    });
});

var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
builder.WebHost.UseUrls($"http://0.0.0.0:{port}");

var app = builder.Build();

// ===== Seed opcional =====
using (var scope = app.Services.CreateScope())
{
    try
    {
        var context = scope.ServiceProvider.GetRequiredService<PeliculasContext>();
        // Si querés asegurarte schema OK:
        // context.Database.Migrate();

        DbSeeder.SeedPeliculas(context);
        DbSeeder.SetUsuarioAdmin(context, "herber7748@hotmail.com");
    }
    catch (Exception ex)
    {
        Console.WriteLine("=== ERROR EN EL SEEDING ===");
        Console.WriteLine(ex);
    }
}

// ===== ORDEN DE MIDDLEWARES (clave) =====
app.UseHttpsRedirection();

app.UseRouting();                // 👈 agregá esto

app.UseCors("AllowFrontend");    // 👈 CORS antes de Auth y de MapControllers

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapGet("/", () => "API funcionando 🚀");

app.Run();
