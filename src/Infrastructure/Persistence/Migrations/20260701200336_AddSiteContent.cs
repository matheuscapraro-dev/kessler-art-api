using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kessler.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddSiteContent : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "site_content",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    WhatsApp = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    InstagramUrl = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    AboutTitle = table.Column<string>(type: "character varying(160)", maxLength: 160, nullable: true),
                    AboutIntro = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    AboutStoryTitle = table.Column<string>(type: "character varying(160)", maxLength: 160, nullable: true),
                    AboutStory = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_site_content", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "about_photos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SiteContentId = table.Column<Guid>(type: "uuid", nullable: false),
                    StorageKey = table.Column<string>(type: "character varying(400)", maxLength: 400, nullable: false),
                    Url = table.Column<string>(type: "character varying(600)", maxLength: 600, nullable: false),
                    Caption = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    DisplayOrder = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_about_photos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_about_photos_site_content_SiteContentId",
                        column: x => x.SiteContentId,
                        principalTable: "site_content",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_about_photos_SiteContentId",
                table: "about_photos",
                column: "SiteContentId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "about_photos");

            migrationBuilder.DropTable(
                name: "site_content");
        }
    }
}
