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
    public class SeriesController : ControllerBase
    {
        private readonly PeliculasContext _context;

        public SeriesController(PeliculasContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var series = await _context.Series.ToListAsync();
            return Ok(series);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Serie nueva)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            _context.Series.Add(nueva);
            await _context.SaveChangesAsync();
            return Ok(nueva);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] Serie actualizada)
        {
            var existente = await _context.Series.FindAsync(id);
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
            var serie = await _context.Series.FindAsync(id);
            if (serie == null) return NotFound();

            _context.Series.Remove(serie);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // Endpoint para info desde TMDb
        [HttpGet("trailer-tmdb")]
        public async Task<IActionResult> GetTrailerTMDb([FromQuery] string titulo)
        {
            string apiKey = "63b5cc9831401c5eb7ed475b2eefda79"; // <-- Poné tu key real acá
            using var httpClient = new HttpClient();

            // Buscar serie por título en español, después en inglés
            var searchUrlES = $"https://api.themoviedb.org/3/search/tv?api_key={apiKey}&language=es-ES&query={Uri.EscapeDataString(titulo)}";
            var searchResp = await httpClient.GetAsync(searchUrlES);
            var searchJson = await searchResp.Content.ReadAsStringAsync();
            var searchData = JObject.Parse(searchJson);

            var firstSerie = searchData["results"]?.FirstOrDefault();
            if (firstSerie == null)
            {
                var searchUrlEN = $"https://api.themoviedb.org/3/search/tv?api_key={apiKey}&language=en-US&query={Uri.EscapeDataString(titulo)}";
                var searchRespEN = await httpClient.GetAsync(searchUrlEN);
                var searchJsonEN = await searchRespEN.Content.ReadAsStringAsync();
                var searchDataEN = JObject.Parse(searchJsonEN);

                firstSerie = searchDataEN["results"]?.FirstOrDefault();
                if (firstSerie == null)
                    return NotFound("Serie no encontrada en TMDb.");
            }

            var serieId = firstSerie["id"].ToString();
            var sinopsis = firstSerie["overview"]?.ToString() ?? "Sin sinopsis.";
            var tmdbRating = firstSerie["vote_average"]?.ToString() ?? "";
            var tmdbVotes = firstSerie["vote_count"]?.ToString() ?? "";

            // Buscar videos (tráiler)
            var videosUrl = $"https://api.themoviedb.org/3/tv/{serieId}/videos?api_key={apiKey}";
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

[HttpGet("tmdb-detalle")]
public async Task<IActionResult> ObtenerDetalleSerie([FromQuery] string titulo)
{
    string apiKey = "63b5cc9831401c5eb7ed475b2eefda79";
    using var httpClient = new HttpClient();
    // Buscar el ID de la serie
    var searchUrl = $"https://api.themoviedb.org/3/search/tv?api_key={apiKey}&language=es-ES&query={Uri.EscapeDataString(titulo)}";
    var searchResp = await httpClient.GetAsync(searchUrl);
    var searchJson = await searchResp.Content.ReadAsStringAsync();
    var searchData = JObject.Parse(searchJson);

    var firstSerie = searchData["results"]?.FirstOrDefault();
    if (firstSerie == null)
        return NotFound("Serie no encontrada en TMDb");

    var seriesId = firstSerie["id"].ToString();

    // Traer detalle de la serie con temporadas
    var detalleUrl = $"https://api.themoviedb.org/3/tv/{seriesId}?api_key={apiKey}&language=es-ES";
    var detalleResp = await httpClient.GetAsync(detalleUrl);
    var detalleJson = await detalleResp.Content.ReadAsStringAsync();
    var detalleData = JObject.Parse(detalleJson);

    // Traer info de cada temporada y episodio
    var temporadas = new List<object>();
    foreach (var temporada in detalleData["seasons"])
    {
        int seasonNumber = temporada["season_number"].Value<int>();
        var temporadaUrl = $"https://api.themoviedb.org/3/tv/{seriesId}/season/{seasonNumber}?api_key={apiKey}&language=es-ES";
        var tempResp = await httpClient.GetAsync(temporadaUrl);
        var tempJson = await tempResp.Content.ReadAsStringAsync();
        var tempData = JObject.Parse(tempJson);

        temporadas.Add(new {
            numero = tempData["season_number"]?.ToString() ?? "",
            nombre = tempData["name"]?.ToString() ?? "",
            overview = tempData["overview"]?.ToString() ?? "",
            poster = tempData["poster_path"] != null ? "https://image.tmdb.org/t/p/w300" + tempData["poster_path"] : null,
            episodios = tempData["episodes"].Select(ep => new {
                numero = ep["episode_number"]?.ToString() ?? "",
                nombre = ep["name"]?.ToString() ?? "",
                overview = ep["overview"]?.ToString() ?? "",
                imagen = ep["still_path"] != null ? "https://image.tmdb.org/t/p/w300" + ep["still_path"] : null,
                fecha = ep["air_date"]?.ToString() ?? ""
            }).ToList()
        });
    }

    return Ok(new {
        titulo = detalleData["name"]?.ToString() ?? "",
        overview = detalleData["overview"]?.ToString() ?? "",
        poster = detalleData["poster_path"] != null ? "https://image.tmdb.org/t/p/w500" + detalleData["poster_path"] : null,
        temporadas
    });
}


    }
}
