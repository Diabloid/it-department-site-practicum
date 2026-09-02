using System;
using Microsoft.EntityFrameworkCore.Migrations;
#nullable disable
namespace it_department_site.Migrations
{
    /// <inheritdoc />
    public partial class AddRolesToUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "roles",
                table: "users",
                type: "longtext",
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");
        }
        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "roles",
                table: "users");
        }
    }
}