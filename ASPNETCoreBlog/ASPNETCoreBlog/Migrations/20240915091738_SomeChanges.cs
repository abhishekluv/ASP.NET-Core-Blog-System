using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ASPNETCoreBlog.Migrations
{
    /// <inheritdoc />
    public partial class SomeChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BlogAuthorSummary",
                table: "SiteSettings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CustomJavaScript",
                table: "SiteSettings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DNSPreconnect",
                table: "SiteSettings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DNSPrefetch",
                table: "SiteSettings",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BlogAuthorSummary",
                table: "SiteSettings");

            migrationBuilder.DropColumn(
                name: "CustomJavaScript",
                table: "SiteSettings");

            migrationBuilder.DropColumn(
                name: "DNSPreconnect",
                table: "SiteSettings");

            migrationBuilder.DropColumn(
                name: "DNSPrefetch",
                table: "SiteSettings");
        }
    }
}
