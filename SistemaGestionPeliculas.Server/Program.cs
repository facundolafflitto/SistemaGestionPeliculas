using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SistemaGestionPeliculas_Data;
using SistemaGestionPeliculas.Server.Data;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// 🔐 JWT desde variables de entorno
var jwtKey = builder.Configuration["Jwt:Key"];
var jwtIssuer = builder.Configuration["Jwt:Issuer"];
var jwtAudience = builder.Configuration["Jwt:Audience"];
if (string.IsNullOrEmpty(jwtKey) || string.IsNullOrEmpty(jwtIssuer) || string.IsNullOrEmpty(jwtAudience))
{
    Console.WriteLine("=== Faltan variables de entorno para JWT ===");
    throw new InvalidOperationException("Faltan variables de entorno para JWT");
}

// 🔐 Autenticación JWT
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

// 🗄️ EF Core PostgreSQL
builder.Services.AddDbContext<PeliculasContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// API
builder.Services.AddControllers();

// 🌐 CORS (política declarativa; la dejamos por si luego volvemos a modo “limpio”)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy
            // Para pruebas podrías dejarlo abierto. Como igualmente vamos a forzar CORS con middleware debajo,
            // esta política es secundaria por ahora.
            .AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

// Puerto Railway
var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
Console.WriteLine($"Puerto asignado: {port}");
builder.WebHost.UseUrls($"http://0.0.0.0:{port}");

var app = builder.Build();

// 🔧 Seeding inicial
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

// ====== ORDEN CORRECTO DE MIDDLEWARES ======

// 1) CORS declarativo (no hace daño tenerlo)
app.UseCors("AllowFrontend");

// 2) 🔥 CORS FORZADO (DEBUG) — asegura que SIEMPRE salgan los headers CORS y responda preflight
app.Use(async (ctx, next) =>
{
    var origin = ctx.Request.Headers["Origin"].ToString();
    if (!string.IsNullOrEmpty(origin))
    {
        // Si no usás cookies, podés devolver "*" acá. Con Bearer no necesitas credentials.
        ctx.Response.Headers["Access-Control-Allow-Origin"] = origin;
        ctx.Response.Headers["Vary"] = "Origin";
        ctx.Response.Headers["Access-Control-Allow-Headers"] = "authorization,content-type,x-requested-with";
        ctx.Response.Headers["Access-Control-Allow-Methods"] = "GET,POST,PUT,DELETE,OPTIONS";
        // ctx.Response.Headers["Access-Control-Allow-Credentials"] = "true"; // solo si usás cookies
    }

    if (string.Equals(ctx.Request.Method, "OPTIONS", StringComparison.OrdinalIgnoreCase))
    {
        ctx.Response.StatusCode = 204;
        await ctx.Response.CompleteAsync();
        return;
    }

    await next();
});

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

// 3) (extra) Catch-all OPTIONS por si algo se escapa
app.MapMethods("/api/{**any}", new[] { "OPTIONS" }, () => Results.NoContent());

// Endpoints
app.MapControllers();
app.MapGet("/", () => "API funcionando 🚀");

app.Run();
