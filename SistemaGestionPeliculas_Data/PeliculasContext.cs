using Microsoft.EntityFrameworkCore;
using SistemaGestionPeliculas.TransferObject.Models;
using SistemaGestionPeliculas_Data;


namespace SistemaGestionPeliculas_Data
{
    public class PeliculasContext : DbContext
    {
        public PeliculasContext(DbContextOptions<PeliculasContext> options)
            : base(options) { }

        public DbSet<Pelicula> Peliculas { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }

        public DbSet<Serie> Series { get; set; }

    }
}
