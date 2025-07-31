namespace SistemaGestionPeliculas.TransferObject.Models
{
    public class Usuario
    {
        public int Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string Rol { get; set; } = "usuario";

        public List<Pelicula> Favoritas { get; set; } = new List<Pelicula>();
        public List<Serie> SeriesFavoritas { get; set; } = new List<Serie>();
    }
}
