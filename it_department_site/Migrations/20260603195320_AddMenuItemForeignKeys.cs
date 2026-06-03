using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace it_department_site.Migrations
{
    /// <inheritdoc />
    public partial class AddMenuItemForeignKeys : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropIndex(
            //    name: "IX_menu_items_MenuItemId",
            //    table: "menu_items");

            //migrationBuilder.DropColumn(
            //    name: "MenuItemId",
            //    table: "menu_items");

            //migrationBuilder.CreateTable(
            //    name: "pages",
            //    columns: table => new
            //    {
            //        Id = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
            //        Title = table.Column<string>(type: "longtext", nullable: false)
            //            .Annotation("MySql:CharSet", "utf8mb4"),
            //        Slug = table.Column<string>(type: "longtext", nullable: false)
            //            .Annotation("MySql:CharSet", "utf8mb4"),
            //        Status = table.Column<int>(type: "int", nullable: false),
            //        Content_main = table.Column<string>(type: "longtext", nullable: false)
            //            .Annotation("MySql:CharSet", "utf8mb4"),
            //        Content_left = table.Column<string>(type: "longtext", nullable: false)
            //            .Annotation("MySql:CharSet", "utf8mb4"),
            //        Content_right = table.Column<string>(type: "longtext", nullable: false)
            //            .Annotation("MySql:CharSet", "utf8mb4"),
            //        ContentJson = table.Column<string>(type: "longtext", nullable: false)
            //            .Annotation("MySql:CharSet", "utf8mb4"),
            //        LayoutType = table.Column<string>(type: "longtext", nullable: false)
            //            .Annotation("MySql:CharSet", "utf8mb4"),
            //        CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
            //        UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
            //        AllowComments = table.Column<bool>(type: "tinyint(1)", nullable: false),
            //        Views = table.Column<int>(type: "int", nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_pages", x => x.Id);
            //    })
            //    .Annotation("MySql:CharSet", "utf8mb4");

            //migrationBuilder.CreateIndex(
            //    name: "IX_menu_items_page_id",
            //    table: "menu_items",
            //    column: "page_id");

            //migrationBuilder.CreateIndex(
            //    name: "IX_menu_items_parent_id",
            //    table: "menu_items",
            //    column: "parent_id");

            //migrationBuilder.AddForeignKey(
            //    name: "FK_menu_items_menu_items_parent_id",
            //    table: "menu_items",
            //    column: "parent_id",
            //    principalTable: "menu_items",
            //    principalColumn: "id");

            //migrationBuilder.AddForeignKey(
            //    name: "FK_menu_items_pages_page_id",
            //    table: "menu_items",
            //    column: "page_id",
            //    principalTable: "pages",
            //    principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_menu_items_menu_items_parent_id",
                table: "menu_items");

            migrationBuilder.DropForeignKey(
                name: "FK_menu_items_pages_page_id",
                table: "menu_items");

            migrationBuilder.DropTable(
                name: "pages");

            migrationBuilder.DropIndex(
                name: "IX_menu_items_page_id",
                table: "menu_items");

            migrationBuilder.DropIndex(
                name: "IX_menu_items_parent_id",
                table: "menu_items");

            migrationBuilder.AddColumn<int>(
                name: "MenuItemId",
                table: "menu_items",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_menu_items_MenuItemId",
                table: "menu_items",
                column: "MenuItemId");
        }
    }
}
