using Microsoft.EntityFrameworkCore.Migrations;

namespace DesafioAPI.Migrations
{
    public partial class AplicandoRelacionamento : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Role",
                table: "Usuarios",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<int>(
                name: "CategoriaId",
                table: "Starters",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Starters_CategoriaId",
                table: "Starters",
                column: "CategoriaId");

            migrationBuilder.AddForeignKey(
                name: "FK_Starters_Categorias_CategoriaId",
                table: "Starters",
                column: "CategoriaId",
                principalTable: "Categorias",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Starters_Categorias_CategoriaId",
                table: "Starters");

            migrationBuilder.DropIndex(
                name: "IX_Starters_CategoriaId",
                table: "Starters");

            migrationBuilder.DropColumn(
                name: "Role",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "CategoriaId",
                table: "Starters");
        }
    }
}
