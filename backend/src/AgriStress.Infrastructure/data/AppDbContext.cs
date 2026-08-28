using Microsoft.EntityFrameworkCore;
using AgriStress.Domain.Entities;

namespace AgriStress.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Field> Fields => Set<Field>();
    public DbSet<SoilZone> SoilZones => Set<SoilZone>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // PostGIS uzantısını EF Core'a tanıtıyoruz
        modelBuilder.HasPostgresExtension("postgis");

        // EPSG:4326 (WGS84 Standart GPS Koordinatları)
        modelBuilder.Entity<Field>()
            .Property(f => f.Location)
            .HasColumnType("geometry(Point, 4326)");

        modelBuilder.Entity<Field>()
            .Property(f => f.Boundary)
            .HasColumnType("geometry(Polygon, 4326)");

        modelBuilder.Entity<SoilZone>()
            .Property(s => s.Boundary)
            .HasColumnType("geometry(MultiPolygon, 4326)");

        //bu fonksiyon r-tree hiyerarsisini olusturur.
        //tüm tarlaları tek tek okuyup her birinin bounding boxını olusturur.
        //bu kareler db de tutulmaz harddiskte indeks dosyası olarak tutulur.
        modelBuilder.Entity<SoilZone>()
            .HasIndex(s => s.Boundary)
            .HasMethod("GIST"); // Spatial index için GIST kullanımı
    }
}