using Microsoft.EntityFrameworkCore.Migrations;

namespace DesafioAPI.Migrations
{
    public partial class PopulandoBanco : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Foto",
                table: "Starters",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.InsertData(
                table: "Categorias",
                columns: new[] { "Id", "Nome", "Tecnologia" },
                values: new object[,]
                {
                    { 1, "Turma-1", "C#" },
                    { 2, "Turma-2", "Java" },
                    { 3, "Turma-3", "PHP" }
                });

            migrationBuilder.InsertData(
                table: "Starters",
                columns: new[] { "Id", "CategoriaId", "Cpf", "Email", "Foto", "Letra", "Nome" },
                values: new object[,]
                {
                    { 1, 1, "11111111111", "allison@starter.com", "padra.png", "ALOR", "Allison" },
                    { 2, 1, "22222222222", "clecio@starter.com", "padra.png", "CLCI", "Clécio" },
                    { 3, 2, "33333333333", "ubiratan@starter.com", "padra.png", "UBRT", "Ubiratan" },
                    { 4, 2, "44444444444", "joao@starter.com", "padra.png", "JOAO", "João" },
                    { 5, 3, "55555555555", "Antonio@starter.com", "padra.png", "ANTN", "Caio" }
                });

            migrationBuilder.InsertData(
                table: "Usuarios",
                columns: new[] { "Id", "Email", "Role", "Senha", "User" },
                values: new object[,]
                {
                    { 1, "admin@admin", "Admin", "Microsoft.IdentityModel.Tokens.SymmetricSecurityKey, KeyId: '', InternalId: '7oX8tkrORTyp1NW0V3PE_HycjI3JEyLTYoldK1y0Caw'.", "Admin" },
                    { 2, "usuario@usuario", "Usuario", "Microsoft.IdentityModel.Tokens.SymmetricSecurityKey, KeyId: '', InternalId: '7oX8tkrORTyp1NW0V3PE_HycjI3JEyLTYoldK1y0Caw'.", "Usuario" }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Categorias",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Categorias",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Categorias",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Starters",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Starters",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Starters",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Starters",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Starters",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DropColumn(
                name: "Foto",
                table: "Starters");
        }
    }
}
