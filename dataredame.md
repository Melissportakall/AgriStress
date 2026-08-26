Markdown
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
│   ├── AgriStress.Application/     # İş Mantığı, DTO'lar, Kural Motorları, Stres Değerlendirme Servisleri
│   ├── AgriStress.Infrastructure/  # PostgreSQL + PostGIS (EF Core + NetTopologySuite), GeoJSON Import Altyapısı
│   └── AgriStress.Api/             # ASP.NET Core Web API Controller'ları, Swagger/OpenAPI Dokümantasyonu
├── data/
│   └── pilot_soil_data.geojson     # Pilot bölgeye ait detaylı toprak kimyası ve uygunluk veri seti
└── docs/                           # CBS katmanları, QGIS şablonları ve mimari şemalar
📊 Pilot Bölge Toprak Veri Seti Öznitelik Haritası (SoilZone)
PostGIS üzerinde tutulan referans toprak poligonları (SoilZones) aşağıdaki kritik tarımsal metrikleri içerir:

1. Makro Besin Elementleri & Kimyasal Denge
Toplam Azot (topn / topn_yet): Topraktaki azot rezervi (g/kg veya %) ve bitki büyümesi için yeterlilik durumu (Düşük, Orta, Yeterli).

Fosfor (P 
2
​
 O 
5
​
  / p205_yet): Kök gelişimi, çiçeklenme ve tohum tutumu için fosfor konsantrasyonu ve yeterlilik sınıfı.

Potasyum (K 
2
​
 O / k20_yet): Kuraklık direnci, su dengesi ve hücresel dayanıklılık göstergesi.

Organik Madde (org_mad_y): Toprak verimliliği, mikroorganizma faaliyeti ve su tutma kapasitesi oranı (%).

2. Mikro Besin Elementleri (Ağır Mineral & İz Element Analizi)
Demir (fe_ppm / fe_yet), Çinko (zn_ppm / zn_yet), Mangan (mn_ppm), Bakır (cu_ppm), Bor (b_ppm): Kloroz ve mikro besin stresini saptayan ppm düzeyinde laboratuvar hassasiyetli mineral değerleri.

3. Fiziksel Doku & Arazi Jeomorfolojisi
Toprak Bünyesi / Tekstür (teks_ust, teks_alt): Üst ve alt kök katmanı dokusu (L: Tın, CL: Killi Tın, SL: Kumlu Tın).

Eğim & Derinlik (egim_sin, der_sin): Arazi eğimi (%6-12 Orta Eğimli) ve kök salma derinliği (Orta Derin, Çok Sığ).

Arazi Kullanım Kabiliyeti (akk_sin_t): Tarımsal üretime uygunluk sınıflandırması (Örn: III. Sınıf - Tarımsal üretime orta uygun).

4. Tarımsal Ürün Uygunluk ve Reçete Grubu
Bahçe & Meyve/Sebze Uygunluğu (smeyseb_aciklama): Arazide yüksek verimle yetiştirilebilecek ürünler (Örn: Şeftali, İncir, Ceviz, Zeytin, Domates, Biber, Enginar).

Tarla Bitkileri Uygunluğu (starla_aciklama, ktarla_aciklama): Sulu ve kuru tarla bitkileri önerileri (Örn: Şeker Pancarı, Fasulye, Yem Bitkileri, Nohut, Lavanta).

🛠️ Teknoloji Yığını
Backend Framework: .NET 8 / C# (ASP.NET Core Web API)

Veritabanı & Mekânsal Motor: PostgreSQL 16 + PostGIS Eklentisi

Mekânsal Kütüphane: NetTopologySuite & NetTopologySuite.IO.GeoJSON

Koordinat Referans Sistemleri (CRS):

Veri Seti Ham Koordinatları: EPSG:32635 (UTM Zone 35N - Metrik)

API & Harita Projeksiyonu: EPSG:4326 (WGS84 - Standart Enlem/Boylam)

CBS Doğrulama: QGIS (PostGIS doğrudan bağlantısı)

🔄 Mekânsal Çalışma Mantığı (Spatial Point-in-Polygon)
İşaretleme: Çiftçi web/mobil arayüzde tarlasına tıklar; koordinat (Latitude, Longitude) API'ye iletilir.

PostGIS Mekânsal Kesişim: Gelen Point(Lon, Lat, 4326) noktası, PostGIS veritabanındaki SoilZones poligon katmanıyla ST_Contains veya ST_Intersects üzerinden 1 milisaniyenin altında eşleştirilir.

Kural & Stres Motoru: Poligondan okunan Azot, Fosfor, Potasyum, Organik Madde ve Eğim metrikleri değerlendirilerek tarlanın Stres Seviyesi, Gübreleme İhtiyacı ve Önerilen Ürünler Listesi tek seferde üretilir.

🚀 Sonraki Adımlarımız
SoilZone Entity & PostGIS Tablosunun Oluşturulması:

GeoJSON özniteliklerini (Azot, Fosfor, Potasyum, Mineraller, Tekstür ve Ürün Açıklamaları) barındıran EF Core modelinin ve tablosunun tanımlanması.

GeoJSON Pilot Veri Aktarım (Seeder / Import) Servisi:

pilot_soil_data.geojson dosyasını okuyan, UTM koordinatlarını EPSG:4326'ya dönüştüren ve PostGIS tablosuna yazan veri aktarım komutunun yazılması.

Mekânsal Kesişim & Toprak Teşhis Servisi (SpatialSoilService):

Yeni bir tarla (Field) eklendiğinde koordinatın hangi SoilZone içine düştüğünü ST_Contains ile bulan ve tarlanın toprak karnesini otomatik dolduran servisin entegrasyonu.

Toprak Stres ve Ürün Reçetesi Kural Motoru:

Çiftçiye anlaşılır bir özet rapor (Düşük Azot Riski, Mükemmel Organik Madde, Önerilen Ürünler: Biber, Domates, Zeytin) sunan analiz uç noktasının yazılması.


<FollowUp label="İlk sonraki adım olan SoilZone entity'sini ve PostGIS tablosunu oluşturmaya geçelim mi?" query="GeoJSON özniteliklerine uygun SoilZone entity sınıfını ve EF Core AppDbContext yapılandırmasını nasıl yazarız?"/>