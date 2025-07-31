using Microsoft.EntityFrameworkCore;
using SistemaGestionPeliculas.TransferObject.Models;

namespace SistemaGestionPeliculas_Data
{
    public class PeliculasContext : DbContext
    {
        public PeliculasContext(DbContextOptions<PeliculasContext> options)
            : base(options) { }

        public DbSet<Pelicula> Peliculas { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Serie> Series { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Relación muchos a muchos: Usuario <-> Pelicula (favoritas)
            modelBuilder.Entity<Usuario>()
                .HasMany(u => u.Favoritas)
                .WithMany(p => p.UsuariosFavoritos);

            // Relación muchos a muchos: Usuario <-> Serie (series favoritas)
            modelBuilder.Entity<Usuario>()
                .HasMany(u => u.SeriesFavoritas)
                .WithMany(s => s.UsuariosFavoritos);
        }
    }
}
