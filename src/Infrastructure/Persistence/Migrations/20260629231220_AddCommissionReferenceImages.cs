using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kessler.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddCommissionReferenceImages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "commission_reference_images",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CommissionRequestId = table.Column<Guid>(type: "uuid", nullable: false),
                    StorageKey = table.Column<string>(type: "character varying(400)", maxLength: 400, nullable: false),
                    Url = table.Column<string>(type: "character varying(600)", maxLength: 600, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_commission_reference_images", x => x.Id);
                    table.ForeignKey(
                        name: "FK_commission_reference_images_commission_requests_CommissionR~",
                        column: x => x.CommissionRequestId,
                        principalTable: "commission_requests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_commission_reference_images_CommissionRequestId",
                table: "commission_reference_images",
                column: "CommissionRequestId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "commission_reference_images");
        }
    }
}
