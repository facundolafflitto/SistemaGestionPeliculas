using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace SistemaGestionPeliculas_Data
{
    public class PeliculasContextFactory : IDesignTimeDbContextFactory<PeliculasContext>
    {
        public PeliculasContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<PeliculasContext>();
            optionsBuilder.UseNpgsql("Host=shuttle.proxy.rlwy.net;Port=10147;Database=railway;Username=postgres;Password=YulObGxmUbUkkHAlXHsyeUDiBVQxkObV;SSL Mode=Require;Trust Server Certificate=true");
            return new PeliculasContext(optionsBuilder.Options);
        }
    }
}
