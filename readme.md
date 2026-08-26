# 🌾 AgriStress - Tarımsal Arazi ve Toprak Stres Analiz Platformu (Pilot Bölge Sürümü)

AgriStress; çiftçilerin ve ziraat uzmanlarının arazilerini coğrafi bilgi sistemleri (CBS / GIS) üzerinde işaretleyerek **toprak kimyası, makro/mikro besin rezervleri, arazi fizyografisi ve tarımsal ürün uygunluğunu** milisaniyeler içinde mekânsal analizlerle sorgulamasını sağlayan modern bir Hassas Tarım (AgriTech) karar destek platformudur.

Sistem, pilot bölge olarak seçilen tarım havzasındaki resmî ve detaylı büyük toprak haritalama vektör veri setini PostGIS üzerinde mekânsal indekslerle işleyerek dış API bağımlılığı olmadan yerel, deterministik ve yüksek doğruluklu bir toprak teşhis motoru sunar.

---

## 🏗️ Proje Mimarisi (Clean Architecture)

Proje kurumsal .NET ve Clean Architecture prensiplerine göre katmanlandırılmıştır:

```text
AgriStress/
├── src/
│   ├── AgriStress.Domain/          # Varlıklar (Entities: Field, SoilZone), Değer Nesneleri, Repository/Service Arayüzleri
│   ├── AgriStress.Application/     # İş Mantığı, DTO'lar, Kural Motorları, Stres Değerlendirme Servisleri (Planlanıyor)
│   ├── AgriStress.Infrastructure/  # PostgreSQL + PostGIS (EF Core + NetTopologySuite), GeoJSON Import Altyapısı
│   └── AgriStress.Api/             # ASP.NET Core Web API Controller'ları, Swagger/OpenAPI Dokümantasyonu
├── data/
│   └── pilot_soil_data.geojson     # Pilot bölgeye ait detaylı toprak kimyası ve uygunluk veri seti
└── docs/                           # CBS katmanları, QGIS şablonları ve mimari şemalar
```

## 📊 Pilot Bölge Toprak Veri Seti Öznitelik Haritası (SoilZone)
PostGIS üzerinde tutulan referans toprak poligonları (SoilZones) aşağıdaki kritik tarımsal metrikleri içerir:

1. **Makro Besin Elementleri & Kimyasal Denge**
   - **Toplam Azot (topn / topn_yet):** Topraktaki azot rezervi ve yeterlilik durumu.
   - **Fosfor (P2O5 / p205_yet):** Kök gelişimi için fosfor konsantrasyonu ve sınıfı.
   - **Potasyum (K2O / k20_yet):** Kuraklık direnci göstergesi.
   - **Organik Madde (org_mad_y):** Toprak verimliliği ve su tutma kapasitesi oranı.

2. **Mikro Besin Elementleri (Ağır Mineral & İz Element Analizi)**
   - **Demir, Çinko, Mangan, Bakır, Bor:** Kloroz ve mikro besin stresini saptayan ppm düzeyinde değerler.

3. **Fiziksel Doku & Arazi Jeomorfolojisi**
   - **Toprak Bünyesi / Tekstür (teks_ust, teks_alt):** Üst ve alt kök katmanı dokusu.
   - **Eğim & Derinlik (egim_sin, der_sin):** Arazi eğimi ve kök salma derinliği.
   - **Arazi Kullanım Kabiliyeti (akk_sin_t):** Tarımsal üretime uygunluk sınıflandırması.

4. **Tarımsal Ürün Uygunluk ve Reçete Grubu**
   - **Bahçe & Meyve/Sebze Uygunluğu (smeyseb_aciklama):** Örn: Şeftali, İncir, Ceviz, Zeytin, Domates, Biber, Enginar vb.
   - **Tarla Bitkileri Uygunluğu (starla_aciklama, ktarla_aciklama):** Sulu ve kuru tarla bitkileri önerileri.

## 🛠️ Teknoloji Yığını
- **Backend Framework:** .NET 10 / C# (ASP.NET Core Web API)
- **Frontend Framework:** React.js (Vite)
- **Harita ve CBS:** Leaflet (React-Leaflet)
- **Veritabanı & Mekânsal Motor:** PostgreSQL 16 + PostGIS Eklentisi
- **Mekânsal Kütüphane:** NetTopologySuite & NetTopologySuite.IO.GeoJSON
- **Koordinat Referans Sistemleri (CRS):**
  - Veri Seti Ham Koordinatları: EPSG:32635 (UTM Zone 35N - Metrik)
  - API & Harita Projeksiyonu: EPSG:4326 (WGS84 - Standart Enlem/Boylam)

## 🔄 Mekânsal Çalışma Mantığı (Spatial Point-in-Polygon)
1. **İşaretleme:** Çiftçi arayüzde tarlasına tıklar; koordinat (Latitude, Longitude) API'ye iletilir.
2. **PostGIS Mekânsal Kesişim:** Gelen Point(Lon, Lat, 4326) noktası, PostGIS veritabanındaki `SoilZones` poligon katmanıyla `ST_Contains` veya `ST_Intersects` üzerinden eşleştirilir.
3. **Kural & Stres Motoru:** Poligondan okunan metrikler değerlendirilerek tarlanın Stres Seviyesi ve Önerilen Ürünler Listesi üretilir.

## 📐 Geliştirme Standartları ve Tasarım Kararları
Projede kod kalitesini (Clean Code) ve kurumsal standartları korumak adına aşağıdaki kurallar uygulanmıştır:
- **PascalCase C# İsimlendirmeleri ve Entity Mapping:** Veritabanındaki tablo sütun isimleri genelde alt tireli (`org_mad_y`, `teks_ust` vb.) olsa da, C# tarafında Domain entity'lerimiz tamamen PascalCase standardında (örn: `OrganikMaddeYuzdesi`) yazılmış ve EF Core'un `[Column("...")]` attribute'ü kullanılarak tablo sütunlarına haritalanmıştır (Mapping).
- **MultiPolygon Tercihi:** Toprak özellikleri gibi coğrafi (GIS) veri setlerinde, aynı özelliğe sahip alanlar bölünmüş olarak (ortasından yol geçen tarla gibi) gelebileceğinden, SoilZone geometri tipi olarak tekil `Polygon` yerine endüstri standardı olan ve parçalı yapıları destekleyen `MultiPolygon` tercih edilmiştir.
- **Neden PostGIS ve Spatial Index (GIST) Kullanıldı?:** 17MB veya daha büyük coğrafi veri setleri içinde "Verilen enlem/boylam hangi tarlaya (çokgene) düşüyor?" sorusuna yanıt aramak klasik veritabanı taramalarıyla (Full Table Scan) oldukça yavaştır. Redis gibi bellek-içi (In-Memory) araçlar da matematiksel geometri kesişimlerinde yetersiz kalır. Bu nedenle endüstri standardı olan **PostGIS** kullanılmış ve `AppDbContext` üzerinden `Boundary` (Sınır) kolonuna **GIST (Spatial Index)** eklenmiştir. Bu sayede harita üzerinde tıklanan bir noktanın tespiti, veriler taranmadan milisaniyeler içinde doğrudan bulunur.
- **EF Core CLI:** Veritabanı tablolarının oluşturulması ve yapısal güncellemeleri manuel SQL yazmak yerine Code-First yaklaşımıyla `dotnet ef migrations` (EF Core CLI) kullanılarak yönetilmektedir.

---

## 🚀 Çalıştırma (Geliştirme Ortamı)

1. Bağımlılıkları geri yükleyin ve derleyin:
```bash
dotnet restore
dotnet build
```

2. PostgreSQL + PostGIS çalıştırmak için Docker örneği:
```bash
docker run --name agristress-db -e POSTGRES_PASSWORD=postgres -e POSTGRES_USER=postgres -e POSTGRES_DB=agristress_db -p 5434:5432 -d postgis/postgis:15-3.4
```

3. Migration'ları uygulayın:
```bash
dotnet tool install --global dotnet-ef
dotnet ef database update --project src/AgriStress.Infrastructure --startup-project src/AgriStress.Api
```

4. API'yi çalıştırın:
```bash
dotnet run --project src/AgriStress.Api/AgriStress.Api.csproj
```

Swagger UI geliştirme ortamında tipik olarak uygulamanın dinlediği porta göre (örn. `http://localhost:5000/swagger`) erişilebilir.