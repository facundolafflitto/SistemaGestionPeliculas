using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using SistemaGestionPeliculas.TransferObject.Models;
using SistemaGestionPeliculas_Data;
using Microsoft.AspNetCore.Authorization;

namespace SistemaGestionPeliculas.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly PeliculasContext _context;

        public AuthController(IConfiguration config, PeliculasContext context)
        {
            Console.WriteLine("==== AuthController CARGADO ====");
            _config = config;
            _context = context;
        }

        // LOGIN
        [HttpPost("login")]
        [AllowAnonymous]
        public IActionResult Login([FromBody] LoginRequest login)
        {
            var usuarioDb = _context.Usuarios.FirstOrDefault(u => u.Email == login.Email);

            if (usuarioDb == null)
                return Unauthorized("Usuario no encontrado");

            if (!BCrypt.Net.BCrypt.Verify(login.Password, usuarioDb.PasswordHash))
                return Unauthorized("Contraseña incorrecta");

            // Claims
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, usuarioDb.Email),
                new Claim(ClaimTypes.Role, usuarioDb.Rol),
                // Claims estándar para ID
                new Claim(JwtRegisteredClaimNames.Sub, usuarioDb.Id.ToString()),
                new Claim("userId", usuarioDb.Id.ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: creds
            );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            // Devolver token + datos extra
            return Ok(new
            {
                token = tokenString,
                userId = usuarioDb.Id,
                rol = usuarioDb.Rol
            });
        }

        // REGISTRO
        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterRequest model)
        {
            if (_context.Usuarios.Any(u => u.Email == model.Email))
                return BadRequest("El email ya está registrado");

            var usuario = new Usuario
            {
                Email = model.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password),
                Rol = model.Rol ?? "usuario"
            };

            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            return Ok("Usuario registrado correctamente");
        }

        [HttpGet("ping")]
        public IActionResult Ping()
        {
            return Ok("pong");
        }
    }

    // DTOs
    public class LoginRequest
    {
        public string Email { get; set; } = "";
        public string Password { get; set; } = "";
    }

    public class RegisterRequest
    {
        public string Email { get; set; } = "";
        public string Password { get; set; } = "";
        public string? Rol { get; set; }
    }
}
