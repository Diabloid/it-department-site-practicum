using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace it_department_site.Migrations
{
    /// <inheritdoc />
    public partial class RefactorPosts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropColumn(
            //    name: "page_slug",
            //    table: "menu_items");

            migrationBuilder.AlterColumn<string>(
                name: "content",
                table: "posts",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            //migrationBuilder.AddColumn<string>(
            //    name: "status",
            //    table: "posts",
            //    type: "ENUM('Draft', 'Published')",
            //    nullable: false,
            //    defaultValue: "Draft")
            //    .Annotation("MySql:CharSet", "utf8mb4");

            //migrationBuilder.CreateIndex(
            //    name: "IX_posts_slug",
            //    table: "posts",
            //    column: "slug",
            //    unique: true);

            //migrationBuilder.AddForeignKey(
            //    name: "FK_posts_users_author_id",
            //    table: "posts",
            //    column: "author_id",
            //    principalTable: "users",
            //    principalColumn: "id",
            //    onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_posts_users_author_id",
                table: "posts");

            migrationBuilder.DropIndex(
                name: "IX_posts_slug",
                table: "posts");

            migrationBuilder.DropColumn(
                name: "status",
                table: "posts");

            migrationBuilder.AlterColumn<string>(
                name: "content",
                table: "posts",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "page_slug",
                table: "menu_items",
                type: "varchar(255)",
                maxLength: 255,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }
    }
}
