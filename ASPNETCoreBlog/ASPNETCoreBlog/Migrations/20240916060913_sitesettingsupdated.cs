using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ASPNETCoreBlog.Migrations
{
    /// <inheritdoc />
    public partial class sitesettingsupdated : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FooterCallToAction",
                table: "SiteSettings");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FooterCallToAction",
                table: "SiteSettings",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
