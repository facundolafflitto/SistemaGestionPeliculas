using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

            var pelicula = await _context.Peliculas.FindAsync(peliculaId);

            if (usuario == null || pelicula == null)
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

            var pelicula = await _context.Peliculas.FindAsync(peliculaId);

            if (usuario == null || pelicula == null)
                return NotFound();

            usuario.Favoritas.Remove(pelicula);
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

            var serie = await _context.Series.FindAsync(serieId);

            if (usuario == null || serie == null)
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

            var serie = await _context.Series.FindAsync(serieId);

            if (usuario == null || serie == null)
                return NotFound();

            usuario.SeriesFavoritas.Remove(serie);
            await _context.SaveChangesAsync();

            return Ok(usuario.SeriesFavoritas);
        }
    }
}
