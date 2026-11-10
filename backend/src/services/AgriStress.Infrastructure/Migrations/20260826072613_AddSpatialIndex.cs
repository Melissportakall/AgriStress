using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AgriStress.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSpatialIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_SoilZones_Boundary",
                table: "SoilZones",
                column: "Boundary")
                .Annotation("Npgsql:IndexMethod", "GIST");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_SoilZones_Boundary",
                table: "SoilZones");
        }
    }
}
