using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using SistemaGestionPeliculas_Data;

namespace SistemaGestionPeliculas_Data
{
    public class PeliculasContextFactory : IDesignTimeDbContextFactory<PeliculasContext>
    {
        public PeliculasContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<PeliculasContext>();
            optionsBuilder.UseSqlite("Data Source=peliculas.db");

            return new PeliculasContext(optionsBuilder.Options);
        }
    }
}
