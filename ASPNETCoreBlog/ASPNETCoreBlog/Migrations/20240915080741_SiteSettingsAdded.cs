using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ASPNETCoreBlog.Migrations
{
    /// <inheritdoc />
    public partial class SiteSettingsAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SiteSettings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SiteName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsRegister = table.Column<bool>(type: "bit", nullable: false),
                    LogoURL = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FaviconURL = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SiteAuthor = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SiteFooter = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FooterCallToAction = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GoogleSiteVerification = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GoogleAnalytics = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsCustomCSSOn = table.Column<bool>(type: "bit", nullable: false),
                    CustomCSS = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsCustomJSOn = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SiteSettings", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SiteSettings");
        }
    }
}
