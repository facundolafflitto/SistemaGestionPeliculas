using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace SistemaGestionPeliculas_Data
{
    public class PeliculasContextFactory : IDesignTimeDbContextFactory<PeliculasContext>
    {
        public PeliculasContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<PeliculasContext>();
            optionsBuilder.UseSqlServer("Server=localhost\\SQLEXPRESS02;Database=GestionSistemaPeliculas;Trusted_Connection=True;TrustServerCertificate=True;Encrypt=True");
            return new PeliculasContext(optionsBuilder.Options);
        }
    }
}
