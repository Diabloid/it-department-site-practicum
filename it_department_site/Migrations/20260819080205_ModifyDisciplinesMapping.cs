using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace it_department_site.Migrations
{
    /// <inheritdoc />
    public partial class ModifyDisciplinesMapping : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_employee_disciplines_disciplines_DisciplinesId",
                table: "employee_disciplines");

            migrationBuilder.DropForeignKey(
                name: "FK_employee_disciplines_employees_EmployeesId",
                table: "employee_disciplines");

            migrationBuilder.RenameColumn(
                name: "EmployeesId",
                table: "employee_disciplines",
                newName: "employee_id");

            migrationBuilder.RenameColumn(
                name: "DisciplinesId",
                table: "employee_disciplines",
                newName: "discipline_id");

            migrationBuilder.RenameIndex(
                name: "IX_employee_disciplines_EmployeesId",
                table: "employee_disciplines",
                newName: "IX_employee_disciplines_employee_id");

            migrationBuilder.AddForeignKey(
                name: "FK_employee_disciplines_disciplines_discipline_id",
                table: "employee_disciplines",
                column: "discipline_id",
                principalTable: "disciplines",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_employee_disciplines_employees_employee_id",
                table: "employee_disciplines",
                column: "employee_id",
                principalTable: "employees",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_employee_disciplines_disciplines_discipline_id",
                table: "employee_disciplines");

            migrationBuilder.DropForeignKey(
                name: "FK_employee_disciplines_employees_employee_id",
                table: "employee_disciplines");

            migrationBuilder.RenameColumn(
                name: "employee_id",
                table: "employee_disciplines",
                newName: "EmployeesId");

            migrationBuilder.RenameColumn(
                name: "discipline_id",
                table: "employee_disciplines",
                newName: "DisciplinesId");

            migrationBuilder.RenameIndex(
                name: "IX_employee_disciplines_employee_id",
                table: "employee_disciplines",
                newName: "IX_employee_disciplines_EmployeesId");

            migrationBuilder.AddForeignKey(
                name: "FK_employee_disciplines_disciplines_DisciplinesId",
                table: "employee_disciplines",
                column: "DisciplinesId",
                principalTable: "disciplines",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_employee_disciplines_employees_EmployeesId",
                table: "employee_disciplines",
                column: "EmployeesId",
                principalTable: "employees",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
