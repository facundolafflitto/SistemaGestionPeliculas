namespace SistemaGestionPeliculas.TransferObject.Models
{
    public class Dashboard
    {
        public int UserId { get; set; }
        public string Email { get; set; } = string.Empty;

        public int TotalPeliculas { get; set; }
        public int TotalSeries { get; set; }

        public List<GenreCount> TopGenerosPeliculas { get; set; } = new();
        public List<GenreCount> TopGenerosSeries   { get; set; } = new();

        public List<MediaItem> PeliculasFavoritas { get; set; } = new();
        public List<MediaItem> SeriesFavoritas    { get; set; } = new();
    }

    public class GenreCount
    {
        public string Genero { get; set; } = string.Empty;
        public int Count { get; set; }
    }

    public class MediaItem
    {
        public int Id { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public string? Genero { get; set; }
        public int Año { get; set; }
        public string? ImagenUrl { get; set; }
    }
}
