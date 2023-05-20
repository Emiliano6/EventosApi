using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EventosApi.Migrations
{
    /// <inheritdoc />
    public partial class agregar : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Organizadores_Eventos_EventoId",
                table: "Organizadores");

            migrationBuilder.AlterColumn<int>(
                name: "EventoId",
                table: "Organizadores",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Organizadores_Eventos_EventoId",
                table: "Organizadores",
                column: "EventoId",
                principalTable: "Eventos",
                principalColumn: "EventoId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Organizadores_Eventos_EventoId",
                table: "Organizadores");

            migrationBuilder.AlterColumn<int>(
                name: "EventoId",
                table: "Organizadores",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Organizadores_Eventos_EventoId",
                table: "Organizadores",
                column: "EventoId",
                principalTable: "Eventos",
                principalColumn: "EventoId");
        }
    }
}
