using SistemaGestionPeliculas.TransferObject.Models;
using SistemaGestionPeliculas_Data;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace SistemaGestionPeliculas.Server.Data
{
    public static class DbSeeder
    {
        public static void SeedPeliculas(PeliculasContext context)
        {
            context.Database.Migrate();

            if (!context.Peliculas.Any())
            {
                context.Peliculas.AddRange(
                    new Pelicula { Titulo = "El Padrino", Genero = "Drama", Año = 1972, ImagenUrl = "https://upload.wikimedia.org/wikipedia/en/1/1c/Godfather_ver1.jpg" },
                    new Pelicula { Titulo = "El Padrino II", Genero = "Drama", Año = 1974, ImagenUrl = "https://upload.wikimedia.org/wikipedia/en/0/03/Godfather_part_ii.jpg" },
                    new Pelicula { Titulo = "El Padrino III", Genero = "Drama", Año = 1990, ImagenUrl = "https://upload.wikimedia.org/wikipedia/en/2/25/Godfather_part_iii.jpg" },
                    new Pelicula { Titulo = "Interestelar", Genero = "Ciencia Ficción", Año = 2014, ImagenUrl = "https://upload.wikimedia.org/wikipedia/en/b/bc/Interstellar_film_poster.jpg" },
                    new Pelicula { Titulo = "Parasite", Genero = "Thriller", Año = 2019, ImagenUrl = "https://image.tmdb.org/t/p/w500/7IiTTgloJzvGI1TAYymCfbfl3vT.jpg" },
                    new Pelicula { Titulo = "El Caballero Oscuro", Genero = "Acción", Año = 2008, ImagenUrl = "https://upload.wikimedia.org/wikipedia/en/8/8a/Dark_Knight.jpg" },
                    new Pelicula { Titulo = "Forrest Gump", Genero = "Drama", Año = 1994, ImagenUrl = "https://upload.wikimedia.org/wikipedia/en/6/67/Forrest_Gump_poster.jpg" },
                    new Pelicula { Titulo = "Pulp Fiction", Genero = "Crimen", Año = 1994, ImagenUrl = "https://upload.wikimedia.org/wikipedia/en/8/82/Pulp_Fiction_cover.jpg" },
                    new Pelicula { Titulo = "La lista de Schindler", Genero = "Drama", Año = 1993, ImagenUrl = "https://upload.wikimedia.org/wikipedia/en/3/38/Schindler%27s_List_movie.jpg" },
                    new Pelicula { Titulo = "Gladiador", Genero = "Acción", Año = 2000, ImagenUrl = "https://upload.wikimedia.org/wikipedia/en/8/8d/Gladiator_ver1.jpg" },
                    new Pelicula { Titulo = "Inception", Genero = "Ciencia Ficción", Año = 2010, ImagenUrl = "https://upload.wikimedia.org/wikipedia/en/7/7f/Inception_ver3.jpg" },
                    new Pelicula { Titulo = "Matrix", Genero = "Ciencia Ficción", Año = 1999, ImagenUrl = "https://upload.wikimedia.org/wikipedia/en/c/c1/The_Matrix_Poster.jpg" },
                    new Pelicula { Titulo = "Avengers: Endgame", Genero = "Acción", Año = 2019, ImagenUrl = "https://upload.wikimedia.org/wikipedia/en/0/0d/Avengers_Endgame_poster.jpg" },
                    new Pelicula { Titulo = "Titanic", Genero = "Romance", Año = 1997, ImagenUrl = "https://upload.wikimedia.org/wikipedia/en/2/2e/Titanic_poster.jpg" },
                    new Pelicula { Titulo = "Jurassic Park", Genero = "Aventura", Año = 1993, ImagenUrl = "https://upload.wikimedia.org/wikipedia/en/e/e7/Jurassic_Park_poster.jpg" },
                    new Pelicula { Titulo = "Star Wars: Una Nueva Esperanza", Genero = "Ciencia Ficción", Año = 1977, ImagenUrl = "https://upload.wikimedia.org/wikipedia/en/8/87/StarWarsMoviePoster1977.jpg" },
                    new Pelicula { Titulo = "Toy Story", Genero = "Animación", Año = 1995, ImagenUrl = "https://upload.wikimedia.org/wikipedia/en/1/13/Toy_Story.jpg" },
                    new Pelicula { Titulo = "Buscando a Nemo", Genero = "Animación", Año = 2003, ImagenUrl = "https://upload.wikimedia.org/wikipedia/en/2/29/Finding_Nemo.jpg" },
                    new Pelicula { Titulo = "El Rey León", Genero = "Animación", Año = 1994, ImagenUrl = "https://upload.wikimedia.org/wikipedia/en/3/3d/The_Lion_King_poster.jpg" },
                    new Pelicula { Titulo = "Up: Una Aventura de Altura", Genero = "Animación", Año = 2009, ImagenUrl = "https://upload.wikimedia.org/wikipedia/en/0/05/Up_%282009_film%29.jpg" },
                    new Pelicula { Titulo = "Coco", Genero = "Animación", Año = 2017, ImagenUrl = "https://upload.wikimedia.org/wikipedia/en/9/98/Coco_%282017_film%29.png" },
                    new Pelicula { Titulo = "Spider-Man: Un nuevo universo", Genero = "Animación", Año = 2018, ImagenUrl = "https://upload.wikimedia.org/wikipedia/en/6/6a/Spider-Man_Into_the_Spider-Verse_poster.png" },
                    new Pelicula { Titulo = "Rápido y Furioso", Genero = "Acción", Año = 2001, ImagenUrl = "https://upload.wikimedia.org/wikipedia/en/8/8f/Fast_and_the_furious_poster.jpg" },
                    new Pelicula { Titulo = "Shrek", Genero = "Animación", Año = 2001, ImagenUrl = "https://upload.wikimedia.org/wikipedia/en/3/39/Shrek.jpg" },
                    new Pelicula { Titulo = "Los Increíbles", Genero = "Animación", Año = 2004, ImagenUrl = "https://upload.wikimedia.org/wikipedia/en/e/e1/The_Incredibles.jpg" },
                    new Pelicula { Titulo = "Avatar", Genero = "Ciencia Ficción", Año = 2009, ImagenUrl = "https://upload.wikimedia.org/wikipedia/en/b/b0/Avatar-Teaser-Poster.jpg" },
                    new Pelicula { Titulo = "Rocky", Genero = "Drama", Año = 1976, ImagenUrl = "https://upload.wikimedia.org/wikipedia/en/1/18/Rocky_poster.jpg" },
                    new Pelicula { Titulo = "La La Land", Genero = "Musical", Año = 2016, ImagenUrl = "https://upload.wikimedia.org/wikipedia/en/a/ab/La_La_Land_%28film%29.png" },
                    new Pelicula { Titulo = "Gladiador", Genero = "Acción", Año = 2000, ImagenUrl = "https://upload.wikimedia.org/wikipedia/en/8/8d/Gladiator_ver1.jpg" },
                    new Pelicula { Titulo = "Joker", Genero = "Drama", Año = 2019, ImagenUrl = "https://upload.wikimedia.org/wikipedia/en/e/e1/Joker_%282019_film%29_poster.jpg" },
                    new Pelicula { Titulo = "Mujer Maravilla", Genero = "Acción", Año = 2017, ImagenUrl = "https://upload.wikimedia.org/wikipedia/en/e/ed/Wonder_Woman_%282017_film%29.jpg" },
                    new Pelicula { Titulo = "Doctor Strange", Genero = "Ciencia Ficción", Año = 2016, ImagenUrl = "https://upload.wikimedia.org/wikipedia/en/c/c7/Doctor_Strange_poster.jpg" },
                    new Pelicula { Titulo = "Frozen", Genero = "Animación", Año = 2013, ImagenUrl = "https://upload.wikimedia.org/wikipedia/en/0/05/Frozen_%282013_film%29_poster.jpg" },
                    new Pelicula { Titulo = "Black Panther", Genero = "Acción", Año = 2018, ImagenUrl = "https://upload.wikimedia.org/wikipedia/en/0/0c/Black_Panther_film_poster.jpg" },
                    new Pelicula { Titulo = "Soul", Genero = "Animación", Año = 2020, ImagenUrl = "https://upload.wikimedia.org/wikipedia/en/3/3a/Soul_%282020_film%29_poster.jpg" },
                    new Pelicula { Titulo = "La Forma del Agua", Genero = "Fantástico", Año = 2017, ImagenUrl = "https://upload.wikimedia.org/wikipedia/en/3/37/The_Shape_of_Water_%28film%29.png" },
                    new Pelicula { Titulo = "Bohemian Rhapsody", Genero = "Biografía", Año = 2018, ImagenUrl = "https://upload.wikimedia.org/wikipedia/en/4/4d/Bohemian_Rhapsody_poster.png" }
                );
            }

            // Agregar admin si no existe
            if (!context.Usuarios.Any(u => u.Rol == "admin"))
            {
                context.Usuarios.Add(new Usuario
                {
                    Email = "admin@example.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("1234"),
                    Rol = "admin"
                });
            }

            // Agregar usuario si no existe
            if (!context.Usuarios.Any())
            {
                context.Usuarios.Add(new Usuario
                {
                    Email = "usuario@example.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("1234"),
                    Rol = "usuario"
                });
            }

            context.SaveChanges();
        }

        // Método para asignar rol admin a un usuario por email
        public static void SetUsuarioAdmin(PeliculasContext context, string email)
        {
            var usuario = context.Usuarios.FirstOrDefault(u => u.Email == email);
            if (usuario != null && usuario.Rol != "admin")
            {
                usuario.Rol = "admin";
                context.SaveChanges();
            }
        }
    }
}
