using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace it_department_site.Migrations
{
    /// <inheritdoc />
    public partial class RefactorMenuItems : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                CREATE TABLE IF NOT EXISTS `menu_items` (
                    `id` int NOT NULL AUTO_INCREMENT,
                    `name` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
                    `parent_id` int NULL,
                    `page_id` int NULL,
                    `page_slug` varchar(255) NULL, 
                    `sort_order` int NOT NULL DEFAULT 0,
                    PRIMARY KEY (`id`)
                ) CHARACTER SET=utf8mb4;
            ");

            migrationBuilder.AddColumn<string>(
                name: "custom_url",
                table: "menu_items",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "custom_url",
                table: "menu_items");
        }
    }
}
