using Microsoft.EntityFrameworkCore.Migrations;
using NetTopologySuite.Geometries;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace AgriStress.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSoilZone : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SoilZones",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Boundary = table.Column<MultiPolygon>(type: "geometry(MultiPolygon, 4326)", nullable: true),
                    topn = table.Column<double>(type: "double precision", nullable: true),
                    topn_yet = table.Column<string>(type: "text", nullable: true),
                    p205 = table.Column<double>(type: "double precision", nullable: true),
                    p205_yet = table.Column<string>(type: "text", nullable: true),
                    k2o = table.Column<double>(type: "double precision", nullable: true),
                    k2o_yet = table.Column<string>(type: "text", nullable: true),
                    org_mad_y = table.Column<double>(type: "double precision", nullable: true),
                    fe_ppm = table.Column<double>(type: "double precision", nullable: true),
                    fe_yet = table.Column<string>(type: "text", nullable: true),
                    zn_ppm = table.Column<double>(type: "double precision", nullable: true),
                    zn_yet = table.Column<string>(type: "text", nullable: true),
                    mn_ppm = table.Column<double>(type: "double precision", nullable: true),
                    cu_ppm = table.Column<double>(type: "double precision", nullable: true),
                    b_ppm = table.Column<double>(type: "double precision", nullable: true),
                    teks_ust = table.Column<string>(type: "text", nullable: true),
                    teks_alt = table.Column<string>(type: "text", nullable: true),
                    egim_sin = table.Column<string>(type: "text", nullable: true),
                    der_sin = table.Column<string>(type: "text", nullable: true),
                    akk_sin_t = table.Column<string>(type: "text", nullable: true),
                    smeyseb_aciklama = table.Column<string>(type: "text", nullable: true),
                    starla_aciklama = table.Column<string>(type: "text", nullable: true),
                    ktarla_aciklama = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SoilZones", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SoilZones");
        }
    }
}
