using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace it_department_site.Migrations
{
    /// <inheritdoc />
    public partial class RefactorSiteSettings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SiteTitle",
                table: "site_settings",
                newName: "site_title");

            migrationBuilder.RenameColumn(
                name: "SiteLogoUrl",
                table: "site_settings",
                newName: "logo_url");

            migrationBuilder.RenameColumn(
                name: "HeaderHtml",
                table: "site_settings",
                newName: "header_html");

            migrationBuilder.RenameColumn(
                name: "HeaderCss",
                table: "site_settings",
                newName: "header_css");

            migrationBuilder.RenameColumn(
                name: "FooterHtml",
                table: "site_settings",
                newName: "footer_html");

            migrationBuilder.RenameColumn(
                name: "FooterCss",
                table: "site_settings",
                newName: "footer_css");

            migrationBuilder.RenameColumn(
                name: "CustomCss",
                table: "site_settings",
                newName: "custom_css");

            migrationBuilder.UpdateData(
                table: "site_settings",
                keyColumn: "site_title",
                keyValue: null,
                column: "site_title",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "site_title",
                table: "site_settings",
                type: "varchar(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(255)",
                oldMaxLength: 255,
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "header_html",
                table: "site_settings",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "header_css",
                table: "site_settings",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "footer_html",
                table: "site_settings",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "footer_css",
                table: "site_settings",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "custom_css",
                table: "site_settings",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "status",
                table: "pages",
                type: "ENUM('Draft', 'Published')",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "site_title",
                table: "site_settings",
                newName: "SiteTitle");

            migrationBuilder.RenameColumn(
                name: "logo_url",
                table: "site_settings",
                newName: "SiteLogoUrl");

            migrationBuilder.RenameColumn(
                name: "header_html",
                table: "site_settings",
                newName: "HeaderHtml");

            migrationBuilder.RenameColumn(
                name: "header_css",
                table: "site_settings",
                newName: "HeaderCss");

            migrationBuilder.RenameColumn(
                name: "footer_html",
                table: "site_settings",
                newName: "FooterHtml");

            migrationBuilder.RenameColumn(
                name: "footer_css",
                table: "site_settings",
                newName: "FooterCss");

            migrationBuilder.RenameColumn(
                name: "custom_css",
                table: "site_settings",
                newName: "CustomCss");

            migrationBuilder.AlterColumn<string>(
                name: "SiteTitle",
                table: "site_settings",
                type: "varchar(255)",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(200)",
                oldMaxLength: 200)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "site_settings",
                keyColumn: "HeaderHtml",
                keyValue: null,
                column: "HeaderHtml",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "HeaderHtml",
                table: "site_settings",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "site_settings",
                keyColumn: "HeaderCss",
                keyValue: null,
                column: "HeaderCss",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "HeaderCss",
                table: "site_settings",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "FooterHtml",
                table: "site_settings",
                type: "longtext",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "site_settings",
                keyColumn: "FooterCss",
                keyValue: null,
                column: "FooterCss",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "FooterCss",
                table: "site_settings",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "site_settings",
                keyColumn: "CustomCss",
                keyValue: null,
                column: "CustomCss",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "CustomCss",
                table: "site_settings",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<int>(
                name: "status",
                table: "pages",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "ENUM('Draft', 'Published')")
                .OldAnnotation("MySql:CharSet", "utf8mb4");
        }
    }
}
