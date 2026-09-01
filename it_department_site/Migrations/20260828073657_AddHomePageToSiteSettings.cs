using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace it_department_site.Migrations
{
    /// <inheritdoc />
    public partial class AddHomePageToSiteSettings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "home_page_id",
                table: "site_settings",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_site_settings_home_page_id",
                table: "site_settings",
                column: "home_page_id");

            migrationBuilder.AddForeignKey(
                name: "FK_site_settings_pages_home_page_id",
                table: "site_settings",
                column: "home_page_id",
                principalTable: "pages",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_site_settings_pages_home_page_id",
                table: "site_settings");

            migrationBuilder.DropIndex(
                name: "IX_site_settings_home_page_id",
                table: "site_settings");

            migrationBuilder.DropColumn(
                name: "home_page_id",
                table: "site_settings");
        }
    }
}
