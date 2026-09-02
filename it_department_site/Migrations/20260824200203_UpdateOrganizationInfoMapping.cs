using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace it_department_site.Migrations
{
    /// <inheritdoc />
    public partial class UpdateOrganizationInfoMapping : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "social_links_json",
                table: "organization_info",
                newName: "social_links");

            migrationBuilder.RenameColumn(
                name: "phone_numbers_json",
                table: "organization_info",
                newName: "phone_numbers");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "social_links",
                table: "organization_info",
                newName: "social_links_json");

            migrationBuilder.RenameColumn(
                name: "phone_numbers",
                table: "organization_info",
                newName: "phone_numbers_json");
        }
    }
}
