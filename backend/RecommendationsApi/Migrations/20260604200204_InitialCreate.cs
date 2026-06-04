using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RecommendationsApi.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SelectionHistories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    ShootingType = table.Column<string>(type: "text", nullable: false),
                    SelectedLensId = table.Column<int>(type: "integer", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SelectionHistories", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SelectionHistories_UserId",
                table: "SelectionHistories",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_SelectionHistories_UserId_ShootingType_CreatedAt",
                table: "SelectionHistories",
                columns: new[] { "UserId", "ShootingType", "CreatedAt" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SelectionHistories");
        }
    }
}
