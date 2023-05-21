using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EventosApi.Migrations
{
    /// <inheritdoc />
    public partial class nuevass : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Eventos_Usuarios_UsuarioId",
                table: "Eventos");

            migrationBuilder.DropForeignKey(
                name: "FK_Eventos_Usuarios_UsuarioId1",
                table: "Eventos");

            migrationBuilder.DropForeignKey(
                name: "FK_Organizadores_Usuarios_UsuarioId",
                table: "Organizadores");

            migrationBuilder.DropIndex(
                name: "IX_Organizadores_UsuarioId",
                table: "Organizadores");

            migrationBuilder.DropIndex(
                name: "IX_Eventos_UsuarioId",
                table: "Eventos");

            migrationBuilder.DropIndex(
                name: "IX_Eventos_UsuarioId1",
                table: "Eventos");

            migrationBuilder.DropColumn(
                name: "UsuarioId",
                table: "Organizadores");

            migrationBuilder.DropColumn(
                name: "UsuarioId",
                table: "Eventos");

            migrationBuilder.DropColumn(
                name: "UsuarioId1",
                table: "Eventos");

            migrationBuilder.CreateTable(
                name: "EventoUsuario",
                columns: table => new
                {
                    FavoritosEventoId = table.Column<int>(type: "int", nullable: false),
                    UsuarioFavoritosUsuarioId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventoUsuario", x => new { x.FavoritosEventoId, x.UsuarioFavoritosUsuarioId });
                    table.ForeignKey(
                        name: "FK_EventoUsuario_Eventos_FavoritosEventoId",
                        column: x => x.FavoritosEventoId,
                        principalTable: "Eventos",
                        principalColumn: "EventoId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EventoUsuario_Usuarios_UsuarioFavoritosUsuarioId",
                        column: x => x.UsuarioFavoritosUsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "UsuarioId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrganizadorUsuario",
                columns: table => new
                {
                    SeguidoresUsuarioId = table.Column<int>(type: "int", nullable: false),
                    seguidosOrganizadorId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrganizadorUsuario", x => new { x.SeguidoresUsuarioId, x.seguidosOrganizadorId });
                    table.ForeignKey(
                        name: "FK_OrganizadorUsuario_Organizadores_seguidosOrganizadorId",
                        column: x => x.seguidosOrganizadorId,
                        principalTable: "Organizadores",
                        principalColumn: "OrganizadorId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrganizadorUsuario_Usuarios_SeguidoresUsuarioId",
                        column: x => x.SeguidoresUsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "UsuarioId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EventoUsuario_UsuarioFavoritosUsuarioId",
                table: "EventoUsuario",
                column: "UsuarioFavoritosUsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizadorUsuario_seguidosOrganizadorId",
                table: "OrganizadorUsuario",
                column: "seguidosOrganizadorId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EventoUsuario");

            migrationBuilder.DropTable(
                name: "OrganizadorUsuario");

            migrationBuilder.AddColumn<int>(
                name: "UsuarioId",
                table: "Organizadores",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UsuarioId",
                table: "Eventos",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UsuarioId1",
                table: "Eventos",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Organizadores_UsuarioId",
                table: "Organizadores",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_Eventos_UsuarioId",
                table: "Eventos",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_Eventos_UsuarioId1",
                table: "Eventos",
                column: "UsuarioId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Eventos_Usuarios_UsuarioId",
                table: "Eventos",
                column: "UsuarioId",
                principalTable: "Usuarios",
                principalColumn: "UsuarioId");

            migrationBuilder.AddForeignKey(
                name: "FK_Eventos_Usuarios_UsuarioId1",
                table: "Eventos",
                column: "UsuarioId1",
                principalTable: "Usuarios",
                principalColumn: "UsuarioId");

            migrationBuilder.AddForeignKey(
                name: "FK_Organizadores_Usuarios_UsuarioId",
                table: "Organizadores",
                column: "UsuarioId",
                principalTable: "Usuarios",
                principalColumn: "UsuarioId");
        }
    }
}
