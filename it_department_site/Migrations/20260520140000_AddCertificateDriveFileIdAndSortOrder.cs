using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace it_department_site.Migrations
{
    public partial class AddCertificateDriveFileIdAndSortOrder : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "drive_file_id",
                table: "employee_certificates",
                type: "varchar(128)",
                maxLength: 128,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<int>(
                name: "sort_order",
                table: "employee_certificates",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "drive_file_id",
                table: "employee_certificates");

            migrationBuilder.DropColumn(
                name: "sort_order",
                table: "employee_certificates");
        }
    }
}
