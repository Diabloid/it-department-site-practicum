using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace it_department_site.Migrations
{
    /// <inheritdoc />
    public partial class IT168_RemoveUnusedSiteSettingsFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "custom_css",
                table: "site_settings");

            migrationBuilder.DropColumn(
                name: "footer_css",
                table: "site_settings");

            migrationBuilder.DropColumn(
                name: "footer_html",
                table: "site_settings");

            migrationBuilder.DropColumn(
                name: "header_css",
                table: "site_settings");

            migrationBuilder.DropColumn(
                name: "header_html",
                table: "site_settings");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "custom_css",
                table: "site_settings",
                type: "text",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "footer_css",
                table: "site_settings",
                type: "text",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "footer_html",
                table: "site_settings",
                type: "text",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "header_css",
                table: "site_settings",
                type: "text",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "header_html",
                table: "site_settings",
                type: "text",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }
    }
}
