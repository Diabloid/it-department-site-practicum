using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace it_department_site.Migrations
{
    /// <inheritdoc />
    public partial class UpdetPostsAdeTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "allow_comments",
                table: "pages");

            migrationBuilder.RenameColumn(
                name: "views",
                table: "pages",
                newName: "ViewsCount");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ViewsCount",
                table: "pages",
                newName: "views");

            migrationBuilder.AddColumn<sbyte>(
                name: "allow_comments",
                table: "pages",
                type: "tinyint(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: (sbyte)0);
        }
    }
}
