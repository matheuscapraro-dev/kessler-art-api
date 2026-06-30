using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kessler.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddWorkItemsKanban : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "customer_phone",
                table: "commission_requests",
                type: "character varying(40)",
                maxLength: 40,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(40)",
                oldMaxLength: 40);

            migrationBuilder.AlterColumn<string>(
                name: "customer_name",
                table: "commission_requests",
                type: "character varying(160)",
                maxLength: 160,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(160)",
                oldMaxLength: 160);

            migrationBuilder.AlterColumn<string>(
                name: "customer_email",
                table: "commission_requests",
                type: "character varying(256)",
                maxLength: 256,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(256)",
                oldMaxLength: 256);

            migrationBuilder.AddColumn<double>(
                name: "Position",
                table: "commission_requests",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            // Default válido para as encomendas já existentes (enum salvo como string).
            migrationBuilder.AddColumn<string>(
                name: "Priority",
                table: "commission_requests",
                type: "character varying(16)",
                maxLength: 16,
                nullable: false,
                defaultValue: "Normal");

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "commission_requests",
                type: "character varying(160)",
                maxLength: 160,
                nullable: true);

            // Encomendas existentes são, por definição, pedidos de cliente.
            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "commission_requests",
                type: "character varying(24)",
                maxLength: 24,
                nullable: false,
                defaultValue: "Encomenda");

            migrationBuilder.CreateTable(
                name: "commission_tasks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CommissionRequestId = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    IsDone = table.Column<bool>(type: "boolean", nullable: false),
                    Position = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_commission_tasks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_commission_tasks_commission_requests_CommissionRequestId",
                        column: x => x.CommissionRequestId,
                        principalTable: "commission_requests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_commission_tasks_CommissionRequestId",
                table: "commission_tasks",
                column: "CommissionRequestId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "commission_tasks");

            migrationBuilder.DropColumn(
                name: "Position",
                table: "commission_requests");

            migrationBuilder.DropColumn(
                name: "Priority",
                table: "commission_requests");

            migrationBuilder.DropColumn(
                name: "Title",
                table: "commission_requests");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "commission_requests");

            migrationBuilder.AlterColumn<string>(
                name: "customer_phone",
                table: "commission_requests",
                type: "character varying(40)",
                maxLength: 40,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(40)",
                oldMaxLength: 40,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "customer_name",
                table: "commission_requests",
                type: "character varying(160)",
                maxLength: 160,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(160)",
                oldMaxLength: 160,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "customer_email",
                table: "commission_requests",
                type: "character varying(256)",
                maxLength: 256,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(256)",
                oldMaxLength: 256,
                oldNullable: true);
        }
    }
}
