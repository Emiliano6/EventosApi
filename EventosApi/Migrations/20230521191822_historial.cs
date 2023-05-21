using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EventosApi.Migrations
{
    /// <inheritdoc />
    public partial class historial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EventoUsuario_Eventos_FavoritosEventoId",
                table: "EventoUsuario");

            migrationBuilder.DropForeignKey(
                name: "FK_EventoUsuario_Usuarios_UsuarioFavoritosUsuarioId",
                table: "EventoUsuario");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EventoUsuario",
                table: "EventoUsuario");

            migrationBuilder.RenameTable(
                name: "EventoUsuario",
                newName: "EventoUsuarioRegistrado");

            migrationBuilder.RenameColumn(
                name: "UsuarioFavoritosUsuarioId",
                table: "EventoUsuarioRegistrado",
                newName: "RegistradosUsuarioId");

            migrationBuilder.RenameColumn(
                name: "FavoritosEventoId",
                table: "EventoUsuarioRegistrado",
                newName: "HistorialEventoId");

            migrationBuilder.RenameIndex(
                name: "IX_EventoUsuario_UsuarioFavoritosUsuarioId",
                table: "EventoUsuarioRegistrado",
                newName: "IX_EventoUsuarioRegistrado_RegistradosUsuarioId");

            migrationBuilder.AddColumn<int>(
                name: "EventoId",
                table: "Promociones",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "EspaciosDisponibles",
                table: "Eventos",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_EventoUsuarioRegistrado",
                table: "EventoUsuarioRegistrado",
                columns: new[] { "HistorialEventoId", "RegistradosUsuarioId" });

            migrationBuilder.CreateTable(
                name: "UsuarioEventoFavorito",
                columns: table => new
                {
                    FavoritosEventoId = table.Column<int>(type: "int", nullable: false),
                    UsuarioFavoritosUsuarioId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsuarioEventoFavorito", x => new { x.FavoritosEventoId, x.UsuarioFavoritosUsuarioId });
                    table.ForeignKey(
                        name: "FK_UsuarioEventoFavorito_Eventos_FavoritosEventoId",
                        column: x => x.FavoritosEventoId,
                        principalTable: "Eventos",
                        principalColumn: "EventoId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UsuarioEventoFavorito_Usuarios_UsuarioFavoritosUsuarioId",
                        column: x => x.UsuarioFavoritosUsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "UsuarioId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UsuarioEventoFavorito_UsuarioFavoritosUsuarioId",
                table: "UsuarioEventoFavorito",
                column: "UsuarioFavoritosUsuarioId");

            migrationBuilder.AddForeignKey(
                name: "FK_EventoUsuarioRegistrado_Eventos_HistorialEventoId",
                table: "EventoUsuarioRegistrado",
                column: "HistorialEventoId",
                principalTable: "Eventos",
                principalColumn: "EventoId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_EventoUsuarioRegistrado_Usuarios_RegistradosUsuarioId",
                table: "EventoUsuarioRegistrado",
                column: "RegistradosUsuarioId",
                principalTable: "Usuarios",
                principalColumn: "UsuarioId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EventoUsuarioRegistrado_Eventos_HistorialEventoId",
                table: "EventoUsuarioRegistrado");

            migrationBuilder.DropForeignKey(
                name: "FK_EventoUsuarioRegistrado_Usuarios_RegistradosUsuarioId",
                table: "EventoUsuarioRegistrado");

            migrationBuilder.DropTable(
                name: "UsuarioEventoFavorito");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EventoUsuarioRegistrado",
                table: "EventoUsuarioRegistrado");

            migrationBuilder.DropColumn(
                name: "EventoId",
                table: "Promociones");

            migrationBuilder.DropColumn(
                name: "EspaciosDisponibles",
                table: "Eventos");

            migrationBuilder.RenameTable(
                name: "EventoUsuarioRegistrado",
                newName: "EventoUsuario");

            migrationBuilder.RenameColumn(
                name: "RegistradosUsuarioId",
                table: "EventoUsuario",
                newName: "UsuarioFavoritosUsuarioId");

            migrationBuilder.RenameColumn(
                name: "HistorialEventoId",
                table: "EventoUsuario",
                newName: "FavoritosEventoId");

            migrationBuilder.RenameIndex(
                name: "IX_EventoUsuarioRegistrado_RegistradosUsuarioId",
                table: "EventoUsuario",
                newName: "IX_EventoUsuario_UsuarioFavoritosUsuarioId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_EventoUsuario",
                table: "EventoUsuario",
                columns: new[] { "FavoritosEventoId", "UsuarioFavoritosUsuarioId" });

            migrationBuilder.AddForeignKey(
                name: "FK_EventoUsuario_Eventos_FavoritosEventoId",
                table: "EventoUsuario",
                column: "FavoritosEventoId",
                principalTable: "Eventos",
                principalColumn: "EventoId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_EventoUsuario_Usuarios_UsuarioFavoritosUsuarioId",
                table: "EventoUsuario",
                column: "UsuarioFavoritosUsuarioId",
                principalTable: "Usuarios",
                principalColumn: "UsuarioId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
