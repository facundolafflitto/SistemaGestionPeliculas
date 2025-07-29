using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaGestionPeliculas_Data;
using SistemaGestionPeliculas.TransferObject.Models;
using Newtonsoft.Json.Linq;
using System.Net.Http;

namespace SistemaGestionPeliculas.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PeliculasController : ControllerBase
    {
        private readonly PeliculasContext _context;
        private readonly IConfiguration _config;

        public PeliculasController(PeliculasContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var peliculas = await _context.Peliculas.ToListAsync();
            return Ok(peliculas);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Pelicula nueva)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            _context.Peliculas.Add(nueva);
            await _context.SaveChangesAsync();
            return Ok(nueva);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] Pelicula actualizada)
        {
            var existente = await _context.Peliculas.FindAsync(id);
            if (existente == null) return NotFound();

            existente.Titulo = actualizada.Titulo;
            existente.Genero = actualizada.Genero;
            existente.Año = actualizada.Año;
            existente.ImagenUrl = actualizada.ImagenUrl;

            await _context.SaveChangesAsync();
            return Ok(existente);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var pelicula = await _context.Peliculas.FindAsync(id);
            if (pelicula == null) return NotFound();

            _context.Peliculas.Remove(pelicula);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // Endpoint para buscar en OMDb
        [HttpGet("buscar-omdb")]
        public async Task<IActionResult> BuscarOMDb([FromQuery] string titulo)
        {
            string apiKey = _config["OMDb:ApiKey"] ?? "";
            string url = $"http://www.omdbapi.com/?t={Uri.EscapeDataString(titulo)}&apikey={apiKey}";

            using var httpClient = new HttpClient();
            var response = await httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
                return BadRequest("No se pudo obtener información de OMDb.");

            var jsonString = await response.Content.ReadAsStringAsync();
            var data = JObject.Parse(jsonString);

            if (data["Response"]?.ToString() != "True")
                return NotFound("Película no encontrada en OMDb.");

            int anio = 0;
            int.TryParse(data["Year"]?.ToString(), out anio);

            var pelicula = new Pelicula
            {
                Titulo = data["Title"]?.ToString() ?? "",
                Genero = data["Genre"]?.ToString() ?? "",
                Año = anio,
                ImagenUrl = data["Poster"]?.ToString() ?? ""
            };

            return Ok(pelicula);
        }

        // Nuevo endpoint para tráiler y sinopsis desde TMDb
        [HttpGet("trailer-tmdb")]
        public async Task<IActionResult> GetTrailerTMDb([FromQuery] string titulo)
        {
            string apiKey = _config["TMDb:ApiKey"] ?? "";
            using var httpClient = new HttpClient();

            var searchUrlES = $"https://api.themoviedb.org/3/search/movie?api_key={apiKey}&language=es-ES&query={Uri.EscapeDataString(titulo)}";
            var searchResp = await httpClient.GetAsync(searchUrlES);
            var searchJson = await searchResp.Content.ReadAsStringAsync();
            var searchData = JObject.Parse(searchJson);

            var firstMovie = searchData["results"]?.FirstOrDefault();
            if (firstMovie == null)
            {
                var searchUrlEN = $"https://api.themoviedb.org/3/search/movie?api_key={apiKey}&language=en-US&query={Uri.EscapeDataString(titulo)}";
                var searchRespEN = await httpClient.GetAsync(searchUrlEN);
                var searchJsonEN = await searchRespEN.Content.ReadAsStringAsync();
                var searchDataEN = JObject.Parse(searchJsonEN);

                firstMovie = searchDataEN["results"]?.FirstOrDefault();
                if (firstMovie == null)
                    return NotFound("Película no encontrada en TMDb.");
            }

            var movieId = firstMovie["id"].ToString();

            var sinopsis = firstMovie["overview"]?.ToString() ?? "";
            if (string.IsNullOrWhiteSpace(sinopsis))
            {
                var movieInfoUrlEN = $"https://api.themoviedb.org/3/movie/{movieId}?api_key={apiKey}&language=en-US";
                var movieInfoRespEN = await httpClient.GetAsync(movieInfoUrlEN);
                var movieInfoJsonEN = await movieInfoRespEN.Content.ReadAsStringAsync();
                var movieDataEN = JObject.Parse(movieInfoJsonEN);
                sinopsis = movieDataEN["overview"]?.ToString() ?? "Sin sinopsis.";
            }

            var tmdbRating = firstMovie["vote_average"]?.ToString() ?? "";
            var tmdbVotes = firstMovie["vote_count"]?.ToString() ?? "";

            var videosUrl = $"https://api.themoviedb.org/3/movie/{movieId}/videos?api_key={apiKey}";
            var videosResp = await httpClient.GetAsync(videosUrl);
            var videosJson = await videosResp.Content.ReadAsStringAsync();
            var videosData = JObject.Parse(videosJson);

            var videos = videosData["results"];
            var trailer = videos?.FirstOrDefault(v =>
                v["site"]?.ToString() == "YouTube" &&
                (
                    v["type"]?.ToString().ToLower().Contains("trailer") == true ||
                    v["type"]?.ToString().ToLower().Contains("teaser") == true ||
                    v["type"]?.ToString().ToLower().Contains("clip") == true
                )
            );
            if (trailer == null)
                trailer = videos?.FirstOrDefault(v => v["site"]?.ToString() == "YouTube");

            string youtubeUrl = null;
            if (trailer != null)
            {
                var youtubeKey = trailer["key"]?.ToString();
                youtubeUrl = $"https://www.youtube.com/watch?v={youtubeKey}";
            }

            return Ok(new { youtubeUrl, sinopsis, tmdbRating, tmdbVotes });
        }
    }
}
