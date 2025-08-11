using System.Text.Json.Serialization;

namespace SistemaGestionPeliculas.TransferObject.Models
{
    public class Serie
    {
        public int Id { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public string Genero { get; set; } = string.Empty;
        public int Año { get; set; }
        public string ImagenUrl { get; set; } = string.Empty;

        [JsonIgnore] // evita ciclo Usuario ↔ Serie
        public List<Usuario> UsuariosFavoritos { get; set; } = new List<Usuario>();
    }
}
