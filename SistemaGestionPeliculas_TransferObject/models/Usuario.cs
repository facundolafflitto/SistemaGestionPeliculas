namespace SistemaGestionPeliculas.TransferObject.Models
{
public class Usuario
{
    public int Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string Rol { get; set; } = "usuario"; // "admin" o "usuario"
}
}