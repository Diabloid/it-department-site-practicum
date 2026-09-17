using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace it_department_site.Migrations
{
    /// <inheritdoc />
    public partial class AddTitleToCategory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "title",
                table: "categories",
                type: "varchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_categories_title",
                table: "categories",
                column: "title",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_categories_title",
                table: "categories");

            migrationBuilder.DropColumn(
                name: "title",
                table: "categories");
        }
    }
}
