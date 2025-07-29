namespace SistemaGestionPeliculas.TransferObject.Models
{
    public class Pelicula
    {
        public int Id { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public string Genero { get; set; } = string.Empty;
        public int Año { get; set; }
        public string ImagenUrl { get; set; } = string.Empty;
    }
}
