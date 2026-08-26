using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace it_department_site.Migrations
{
    /// <inheritdoc />
    public partial class AddEmployeeToUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "employee_id",
                table: "users",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_users_employee_id",
                table: "users",
                column: "employee_id",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_users_employees_employee_id",
                table: "users",
                column: "employee_id",
                principalTable: "employees",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_users_employees_employee_id",
                table: "users");

            migrationBuilder.DropIndex(
                name: "IX_users_employee_id",
                table: "users");

            migrationBuilder.DropColumn(
                name: "employee_id",
                table: "users");
        }
    }
}
