using Microsoft.AspNetCore.Mvc; // Para controlar endpoints en una API
using Microsoft.IdentityModel.Tokens; // Para trabajar con tokens JWT
using System.IdentityModel.Tokens.Jwt; // Para crear y manipular tokens JWT
using System.Security.Claims; // Para manejar claims (información del usuario en el token)
using System.Text; // Para codificar la clave secreta del token
using SistemaGestionPeliculas.TransferObject.Models; // Modelos de transferencia de datos
using SistemaGestionPeliculas_Data; // Contexto de base de datos (Entity Framework)

namespace SistemaGestionPeliculas.Server.Controllers
{
    // Indicamos que esta clase es un controlador de API
    [ApiController]
    // Ruta base para los endpoints de este controlador: api/auth
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _config; // Para acceder a configuración, como la clave JWT
        private readonly PeliculasContext _context; // Contexto de base de datos (Entity Framework)

        // Constructor: inyectamos la configuración y el contexto de base de datos
        public AuthController(IConfiguration config, PeliculasContext context)
        {
            _config = config;
            _context = context;
        }

        // Endpoint: POST /api/auth/login
        // Recibe un objeto LoginRequest con Email y Password
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest login)
        {
            // Buscamos al usuario en la base de datos por email
            var usuarioDb = _context.Usuarios.FirstOrDefault(u => u.Email == login.Email);

            // Si no se encuentra, devolvemos un 401 (No autorizado)
            if (usuarioDb == null)
                return Unauthorized("Usuario no encontrado");

            // Verificamos si la contraseña ingresada coincide con la contraseña hasheada en la base
            if (!BCrypt.Net.BCrypt.Verify(login.Password, usuarioDb.PasswordHash))
                return Unauthorized("Contraseña incorrecta");

            // Si el login es válido, generamos un array de claims (información del usuario que irá en el token)
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, usuarioDb.Email), // Email del usuario
                new Claim("UserId", usuarioDb.Id.ToString()), // ID del usuario
                new Claim(ClaimTypes.Role, usuarioDb.Rol) // Rol (admin, usuario, etc.)
            };

            // Creamos la clave simétrica usando la clave guardada en appsettings.json
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256); // Firma del token con algoritmo HMAC-SHA256

            // Creamos el token con toda la información
            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"], // Quien emite el token
                audience: _config["Jwt:Audience"], // Para quién es el token
                claims: claims, // Los datos del usuario
                expires: DateTime.Now.AddHours(1), // Tiempo de expiración
                signingCredentials: creds // Firma del token
            );

            // Retornamos el token al cliente
            return Ok(new
            {
                token = new JwtSecurityTokenHandler().WriteToken(token)
            });
        }

        // Endpoint: POST /api/auth/register
        // Recibe un objeto RegisterRequest con Email, Password y Rol (opcional)
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest model)
        {
            // Verificamos que el email no esté ya registrado
            if (_context.Usuarios.Any(u => u.Email == model.Email))
                return BadRequest("El email ya está registrado");

            // Creamos un nuevo objeto Usuario con los datos recibidos
            var usuario = new Usuario
            {
                Email = model.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password), // Hasheamos la contraseña
                Rol = model.Rol ?? "usuario" // Si no se indica rol, se asigna "usuario" por defecto
            };

            // Agregamos el nuevo usuario al contexto y guardamos los cambios en la base
            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            // Respondemos con éxito
            return Ok("Usuario registrado correctamente");
        }
    }

    // Modelo para el login: contiene email y contraseña
    public class LoginRequest
    {
        public string Email { get; set; } = "";
        public string Password { get; set; } = "";
    }

    // Modelo para el registro: contiene email, contraseña y rol (opcional)
    public class RegisterRequest
    {
        public string Email { get; set; } = "";
        public string Password { get; set; } = "";
        public string? Rol { get; set; } // Puede ser null
    }
}
