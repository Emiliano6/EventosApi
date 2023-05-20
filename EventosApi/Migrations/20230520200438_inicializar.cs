using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EventosApi.Migrations
{
    /// <inheritdoc />
    public partial class inicializar : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Organizadores_Eventos_EventoId",
                table: "Organizadores");

            migrationBuilder.DropIndex(
                name: "IX_Organizadores_EventoId",
                table: "Organizadores");

            migrationBuilder.AlterColumn<string>(
                name: "Ubicacion",
                table: "Eventos",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "EventoOrganizador",
                columns: table => new
                {
                    EventosEventoId = table.Column<int>(type: "int", nullable: false),
                    OrganizadoresOrganizadorId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventoOrganizador", x => new { x.EventosEventoId, x.OrganizadoresOrganizadorId });
                    table.ForeignKey(
                        name: "FK_EventoOrganizador_Eventos_EventosEventoId",
                        column: x => x.EventosEventoId,
                        principalTable: "Eventos",
                        principalColumn: "EventoId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EventoOrganizador_Organizadores_OrganizadoresOrganizadorId",
                        column: x => x.OrganizadoresOrganizadorId,
                        principalTable: "Organizadores",
                        principalColumn: "OrganizadorId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EventoOrganizador_OrganizadoresOrganizadorId",
                table: "EventoOrganizador",
                column: "OrganizadoresOrganizadorId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EventoOrganizador");

            migrationBuilder.AlterColumn<string>(
                name: "Ubicacion",
                table: "Eventos",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_Organizadores_EventoId",
                table: "Organizadores",
                column: "EventoId");

            migrationBuilder.AddForeignKey(
                name: "FK_Organizadores_Eventos_EventoId",
                table: "Organizadores",
                column: "EventoId",
                principalTable: "Eventos",
                principalColumn: "EventoId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
