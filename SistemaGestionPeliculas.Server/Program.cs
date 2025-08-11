using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SistemaGestionPeliculas_Data;
using SistemaGestionPeliculas.Server.Data;
using System.Text;

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

builder.Services.AddControllers();

// ===== CORS (prod) =====
// Permite tu dominio de Vercel y localhost. Si usás previews, habilitá el SetIsOriginAllowed de abajo.
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

// CORS SIEMPRE antes de Auth y antes de MapControllers
app.UseCors("AllowFrontend");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapGet("/", () => "API funcionando 🚀");

app.Run();
