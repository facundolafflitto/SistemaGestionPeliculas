using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SistemaGestionPeliculas_Data.Migrations
{
    public partial class AddFavoritasSeries : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PeliculaUsuario",
                columns: table => new
                {
                    FavoritasId = table.Column<int>(type: "integer", nullable: false),
                    UsuariosFavoritosId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PeliculaUsuario", x => new { x.FavoritasId, x.UsuariosFavoritosId });
                    table.ForeignKey(
                        name: "FK_PeliculaUsuario_Peliculas_FavoritasId",
                        column: x => x.FavoritasId,
                        principalTable: "Peliculas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PeliculaUsuario_Usuarios_UsuariosFavoritosId",
                        column: x => x.UsuariosFavoritosId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SerieUsuario",
                columns: table => new
                {
                    SeriesFavoritasId = table.Column<int>(type: "integer", nullable: false),
                    UsuariosFavoritosId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SerieUsuario", x => new { x.SeriesFavoritasId, x.UsuariosFavoritosId });
                    table.ForeignKey(
                        name: "FK_SerieUsuario_Series_SeriesFavoritasId",
                        column: x => x.SeriesFavoritasId,
                        principalTable: "Series",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SerieUsuario_Usuarios_UsuariosFavoritosId",
                        column: x => x.UsuariosFavoritosId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PeliculaUsuario_UsuariosFavoritosId",
                table: "PeliculaUsuario",
                column: "UsuariosFavoritosId");

            migrationBuilder.CreateIndex(
                name: "IX_SerieUsuario_UsuariosFavoritosId",
                table: "SerieUsuario",
                column: "UsuariosFavoritosId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PeliculaUsuario");

            migrationBuilder.DropTable(
                name: "SerieUsuario");
        }
    }
}
