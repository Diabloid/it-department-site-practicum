using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace it_department_site.Migrations
{
    /// <inheritdoc />
    public partial class ChangeGraduateTableName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Graduates",
                table: "Graduates");

            migrationBuilder.RenameTable(
                name: "Graduates",
                newName: "graduates");

            migrationBuilder.AddPrimaryKey(
                name: "PK_graduates",
                table: "graduates",
                column: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_graduates",
                table: "graduates");

            migrationBuilder.RenameTable(
                name: "graduates",
                newName: "Graduates");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Graduates",
                table: "Graduates",
                column: "id");
        }
    }
}
