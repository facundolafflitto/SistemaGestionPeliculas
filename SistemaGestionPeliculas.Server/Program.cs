using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using SistemaGestionPeliculas_Data;
using SistemaGestionPeliculas.Server.Data;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// JWT
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

// DB
builder.Services.AddDbContext<PeliculasContext>(opt =>
    opt.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddControllers();

// CORS (permití prod + localhost + cualquier preview de vercel.app)
builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
    {
        policy
            // Permite tu prod exacto
            .WithOrigins("https://sistema-gestion-peliculas.vercel.app",
                         "http://localhost:5173")
            // Además, habilita **cualquier** subdominio *.vercel.app (para previews)
            .SetIsOriginAllowed(origin =>
            {
                if (string.IsNullOrEmpty(origin)) return false;
                try { return new Uri(origin).Host.EndsWith("vercel.app"); }
                catch { return false; }
            })
            .WithMethods("GET","POST","PUT","DELETE","OPTIONS")
            .WithHeaders(HeaderNames.Authorization, HeaderNames.ContentType, "X-Requested-With");
            // .AllowCredentials(); // solo si usás cookies. Para Bearer NO hace falta.
    });
});

var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
builder.WebHost.UseUrls($"http://0.0.0.0:{port}");

var app = builder.Build();

// (Opcional pero recomendado)
app.UseHttpsRedirection();

// 👇 CORS SIEMPRE ANTES de Auth/Endpoints
app.UseCors("Frontend");

app.UseAuthentication();
app.UseAuthorization();

// (Opcional) responder preflight si algo raro lo corta
app.MapMethods("/api/{**any}", new[] { "OPTIONS" }, () => Results.NoContent());

app.MapControllers();
app.MapGet("/", () => "API funcionando 🚀");

// Seed
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

app.Run();
