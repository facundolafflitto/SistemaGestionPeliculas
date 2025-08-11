using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using SistemaGestionPeliculas_Data;
using SistemaGestionPeliculas.TransferObject.Models;

namespace SistemaGestionPeliculas.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsuariosController : ControllerBase
    {
        private readonly PeliculasContext _context;
        public UsuariosController(PeliculasContext context)
        {
            _context = context;
        }

        // --------- FAVORITAS DE PELÍCULAS ---------

        // Ver películas favoritas
        [HttpGet("{id}/favoritas")]
        public async Task<IActionResult> GetFavoritas(int id)
        {
            var usuario = await _context.Usuarios
                .Include(u => u.Favoritas)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (usuario == null)
                return NotFound();

            return Ok(usuario.Favoritas);
        }

        // Agregar película a favoritas
        [HttpPost("{id}/favoritas/{peliculaId}")]
        public async Task<IActionResult> AgregarFavorita(int id, int peliculaId)
        {
            var usuario = await _context.Usuarios
                .Include(u => u.Favoritas)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (usuario == null)
                return NotFound();

            var pelicula = await _context.Peliculas
                .FirstOrDefaultAsync(p => p.Id == peliculaId);

            if (pelicula == null)
                return NotFound();

            if (!usuario.Favoritas.Any(p => p.Id == peliculaId))
                usuario.Favoritas.Add(pelicula);

            await _context.SaveChangesAsync();

            return Ok(usuario.Favoritas);
        }

        // Quitar película de favoritas
        [HttpDelete("{id}/favoritas/{peliculaId}")]
        public async Task<IActionResult> EliminarFavorita(int id, int peliculaId)
        {
            var usuario = await _context.Usuarios
                .Include(u => u.Favoritas)
                .FirstOrDefaultAsync(u => u.Id == id);
            if (usuario == null) return NotFound();

            var favorita = usuario.Favoritas.FirstOrDefault(p => p.Id == peliculaId);
            if (favorita == null)
                return NotFound();

            usuario.Favoritas.Remove(favorita);
            await _context.SaveChangesAsync();
            return Ok(usuario.Favoritas);
        }

        // --------- FAVORITAS DE SERIES ---------

        // Ver series favoritas
        [HttpGet("{id}/seriesfavoritas")]
        public async Task<IActionResult> GetSeriesFavoritas(int id)
        {
            var usuario = await _context.Usuarios
                .Include(u => u.SeriesFavoritas)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (usuario == null)
                return NotFound();

            return Ok(usuario.SeriesFavoritas);
        }

        // Agregar serie a favoritas
        [HttpPost("{id}/seriesfavoritas/{serieId}")]
        public async Task<IActionResult> AgregarSerieFavorita(int id, int serieId)
        {
            var usuario = await _context.Usuarios
                .Include(u => u.SeriesFavoritas)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (usuario == null)
                return NotFound();

            var serie = await _context.Series
                .FirstOrDefaultAsync(s => s.Id == serieId);

            if (serie == null)
                return NotFound();

            if (!usuario.SeriesFavoritas.Any(s => s.Id == serieId))
                usuario.SeriesFavoritas.Add(serie);

            await _context.SaveChangesAsync();

            return Ok(usuario.SeriesFavoritas);
        }

        // Quitar serie de favoritas
        [HttpDelete("{id}/seriesfavoritas/{serieId}")]
        public async Task<IActionResult> EliminarSerieFavorita(int id, int serieId)
        {
            var usuario = await _context.Usuarios
                .Include(u => u.SeriesFavoritas)
                .FirstOrDefaultAsync(u => u.Id == id);
            if (usuario == null) return NotFound();

            var favorita = usuario.SeriesFavoritas.FirstOrDefault(s => s.Id == serieId);
            if (favorita == null)
                return NotFound();

            usuario.SeriesFavoritas.Remove(favorita);
            await _context.SaveChangesAsync();
            return Ok(usuario.SeriesFavoritas);
        }

        // --------- DASHBOARD DEL USUARIO ---------

        // Usa el userId del token (recomendado para "Mi Dashboard")
        [HttpGet("~/api/me/dashboard")]
        [Authorize]
        public async Task<IActionResult> GetMyDashboard()
        {
            var idStr = User.FindFirst("userId")?.Value
                     ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!int.TryParse(idStr, out var userId))
                return Unauthorized();

            return await GetDashboard(userId);
        }

        // Dashboard por id explícito (admin o para debug)
        [HttpGet("{id}/dashboard")]
        public async Task<IActionResult> GetDashboard(int id)
        {
            var usuario = await _context.Usuarios
                .Include(u => u.Favoritas)
                .Include(u => u.SeriesFavoritas)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (usuario == null) return NotFound("Usuario no encontrado");

            string[] Split(string? g) => string.IsNullOrWhiteSpace(g)
                ? Array.Empty<string>()
                : g.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

            var pelis = usuario.Favoritas;
            var series = usuario.SeriesFavoritas;

            var topP = pelis.SelectMany(p => Split(p.Genero))
                .GroupBy(x => x)
                .Select(g => new { genero = g.Key, count = g.Count() })
                .OrderByDescending(x => x.count)
                .Take(5);

            var topS = series.SelectMany(s => Split(s.Genero))
                .GroupBy(x => x)
                .Select(g => new { genero = g.Key, count = g.Count() })
                .OrderByDescending(x => x.count)
                .Take(5);

            var dto = new
            {
                userId = usuario.Id,
                email = usuario.Email,
                totalPeliculas = pelis.Count,
                totalSeries = series.Count,
                topGenerosPeliculas = topP,
                topGenerosSeries = topS,
                peliculasFavoritas = pelis.Select(p => new
                {
                    id = p.Id,
                    titulo = p.Titulo,
                    genero = p.Genero,
                    año = p.Año,
                    imagenUrl = p.ImagenUrl
                }),
                seriesFavoritas = series.Select(s => new
                {
                    id = s.Id,
                    titulo = s.Titulo,
                    genero = s.Genero,
                    año = s.Año,
                    imagenUrl = s.ImagenUrl
                })
            };

            return Ok(dto);
        }
    }
}
