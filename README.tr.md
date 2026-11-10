# 🌾 AgriStress - Tarımsal Arazi ve Toprak Stres Analiz Platformu

[![.NET 10](https://img.shields.io/badge/.NET-10.0-purple.svg)](https://dotnet.microsoft.com/)
[![React 19](https://img.shields.io/badge/React-19.0-blue.svg)](https://react.dev/)
[![PostgreSQL](https://img.shields.io/badge/PostgreSQL-16-336791.svg)](https://www.postgresql.org/)
[![PostGIS](https://img.shields.io/badge/PostGIS-Spatial_Engine-green.svg)](https://postgis.net/)
[![Docker](https://img.shields.io/badge/Docker-Compose-2496ED.svg)](https://www.docker.com/)

> 🇬🇧 **[Click here for English Documentation (İngilizce Dokümantasyona Geç)](README.md)**

AgriStress; çiftçilerin ve ziraat uzmanlarının arazilerini etkileşimli bir **Coğrafi Bilgi Sistemleri (CBS / GIS)** haritası üzerinde işaretleyerek **toprak kimyası, makro/mikro besin rezervleri, arazi fizyografisi ve tarımsal ürün uygunluğunu** milisaniyeler içinde mekânsal analizlerle sorgulamasını sağlayan modern bir **Hassas Tarım (AgriTech) Karar Destek Platformudur**.

Sistem, pilot bölge olarak seçilen Bursa / Nilüfer tarım havzasındaki 17.5 MB boyutundaki büyük toprak haritalama vektör veri setini **PostGIS Mekânsal İndeksleri (GIST)** ile yerel olarak işleyerek, dış yapay zeka (LLM) API maliyeti olmadan yüksek hızlı ve deterministik ziraat reçeteleri sunar.

---

## 🎯 Projenin Temel Amacı: CBS'yi (Coğrafi Bilgi Sistemleri) Öğrenmek

Tarımsal faydalarının yanı sıra **bu projenin hayata geçirilmesindeki birincil mühendislik gayesi; modern Coğrafi Bilgi Sistemleri (CBS / GIS) kavramlarını, mekânsal veritabanı motorlarını ve uzamsal algoritmaları kurumsal bir tam-yığın (Full-Stack) yazılım mimarisinde derinlemesine öğrenmektir.**

### Projede CBS / GIS Adına Neler Yapıldı?

| CBS / GIS Kavramı | AgriStress Projesindeki Uygulaması |
| :--- | :--- |
| **Vektör Büyük Veri & MultiPolygon** | On binlerce tepe noktasından (vertex) oluşan **17.5 MB'lık resmî GeoJSON toprak verisi** parse edilmiştir. Gerçek hayatta ortasından yol veya su kanalı geçen bölünmüş tarlaları desteklemek amacıyla tekil `Polygon` yerine CBS standardı olan `MultiPolygon` geometrisi tercih edilmiştir. |
| **Koordinat Referans Sistemleri (CRS) & Projeksiyon Dönüşümü** | Farklı projeksiyonlar arası uyumsuzluk çözülmüştür: Kullanıcı haritası standart GPS koordinatı olan **`EPSG:4326` (WGS84 Enlem/Boylam)** kullanırken, veri setindeki tarlalar yerel metrik sistem olan **`EPSG:5254` (TUREF TM30 / UTM)** koordinatlarına sahiptir. PostGIS üzerinde `ST_Transform` ile anlık dinamik projeksiyon dönüşümü yapılmıştır. |
| **Mekânsal Kesişim Analizi (Point-in-Polygon - PIP)** | Çiftçinin haritada tıkladığı `Point(Lon, Lat)` noktasının binlerce toprak poligonunun hangisinin içerisine düştüğü PostGIS `ST_Contains` fonksiyonu ile tespit edilmiştir. |
| **Mekânsal İndeksleme (GIST & R-Tree İndeksi)** | 17.5 MB'lık poligon kümesini klasik tam tablo taraması (Full Table Scan) ile aramak saniyeler sürecek bir işlemken, veritabanındaki sınır kolonuna **GIST (Spatial R-Tree Index)** eklenmiş ve sorgu süresi **1-2 milisaniyeye** indirilmiştir. |
| **Çok Katmanlı Web CBS (Web GIS & LayersControl)** | Leaflet katman kontrolcüsü entegre edilerek kullanıcının standart OpenStreetMap vektör altlığı, yüksek çözünürlüklü **ESRI Uydu Görüntüsü (World Imagery)** ve **RainViewer Canlı Yağış Radarı (Heatmap)** katmanları arasında geçiş yapabilmesi sağlanmıştır. |
| **Dinamik İlçe Sınırı Vektör Giydirmesi** | OpenStreetMap Nominatim servisinden Bursa / Nilüfer ilçe poligonu dinamik çekilerek harita üzerinde yeşil vektörel sınır çizgisiyle görselleştirilmiştir. |

---

## 🏗️ Mimari Yapı: Enterprise Clean Architecture

Proje, kurumsal .NET dünyasında kabul gören **Clean Architecture** prensiplerine göre `src/services/` ve `src/tests/` ana dallarına ayrılarak modüler şekilde inşa edilmiştir:

```text
AgriStress/
├── backend/
│   ├── src/
│   │   ├── services/
│   │   │   ├── AgriStress.API/             # Sunum: Controllers, Middlewares, Configurations, Program.cs
│   │   │   ├── AgriStress.Application/     # İş Mantığı: DTOs, Exceptions, Interfaces, AgronomyRuleEngine
│   │   │   ├── AgriStress.Domain/          # Çekirdek: Varlıklar (SoilZone, Field), Enums, BaseEntity
│   │   │   └── AgriStress.Infrastructure/  # Altyapı: DataAccess (PostGIS AppDbContext), Migrations, ExternalServices, Seeder
│   │   └── tests/
│   │       └── AgriStress.UnitTests/       # xUnit Birim Testleri (Ziraat Kural Motoru ve Mantıksal Testler)
│   ├── tools/
│   │   └── SoilTester/                     # SoilGrids REST API için bağımsız test CLI aracı
│   └── data/
│       └── geotoprak_sinifi_32768.geojson  # 17.5 MB pilot bölge vektörel toprak verisi
├── frontend/                               # React 19 + Leaflet interaktif CBS harita arayüzü
├── docker-compose.yml                      # Tüm servisleri tek komutta ayağa kaldıran orkestrasyon
└── README.md                               # Ana GitHub dokümantasyonu (İngilizce)
```

### Katman Sorumluluk Tablosu

| Katman | Sorumluluk | İçerdiği Bileşenler |
| :--- | :--- | :--- |
| **API** | HTTP isteklerini karşılama, rota yönlendirme, CORS, hata yakalama middleware'i | [Controllers](backend/src/services/AgriStress.API/Controllers), [Middlewares](backend/src/services/AgriStress.API/Middlewares), [Configurations](backend/src/services/AgriStress.API/Configurations) |
| **Application** | Uygulama iş mantığı, ziraat kural motoru, DTO modelleri, servis arayüzleri | [AgronomyRuleEngine](backend/src/services/AgriStress.Application/Services/AgronomyRuleEngine.cs), [DTOs](backend/src/services/AgriStress.Application/DTOs), [Exceptions](backend/src/services/AgriStress.Application/Exceptions) |
| **Domain** | Saf iş varlıkları (Entity), sabitler (Enum) ve temel sınıflar (Dış bağımlılığı yoktur) | [SoilZone](backend/src/services/AgriStress.Domain/Entities/SoilZone.cs), [Field](backend/src/services/AgriStress.Domain/Entities/Field.cs), [BaseEntity](backend/src/services/AgriStress.Domain/Common/BaseEntity.cs) |
| **Infrastructure** | Veritabanı erişimi (EF Core + PostGIS), harici API entegrasyonları, Seeder | [AppDbContext](backend/src/services/AgriStress.Infrastructure/DataAccess/AppDbContext.cs), [SpatialSoilService](backend/src/services/AgriStress.Infrastructure/Services/SpatialSoilService.cs), [ExternalServices](backend/src/services/AgriStress.Infrastructure/ExternalServices) |
| **UnitTests** | Kural motorunun sınır değer ve doğru reçete çıktılarının otomatik doğrulanması | [AgronomyRuleEngineTests](backend/src/tests/AgriStress.UnitTests/AgronomyRuleEngineTests.cs) (xUnit) |

---

## 🛠️ Teknoloji Yığını

| Bileşen | Kullanılan Teknoloji | Sürüm | Kullanım Amacı |
| :--- | :--- | :--- | :--- |
| **Backend Framework** | .NET (C#) / ASP.NET Core | 10.0 | Yüksek performanslı REST API |
| **Veritabanı & Mekânsal Motor** | PostgreSQL + PostGIS | 16 / 3.4 | Mekânsal poligon saklama ve GIST arama motoru |
| **Mekânsal CBS Kütüphanesi** | NetTopologySuite (NTS) | 2.6.0 | C# içinde `Point`, `MultiPolygon` CBS nesneleri |
| **ORM & Veri Haritalama** | Entity Framework Core | 10.0 | Code-First veritabanı yönetimi ve PostGIS mapping |
| **Direnç (Resilience)** | Polly | 8.1.0 | Dış servisler için Retry ve Circuit Breaker desenleri |
| **Frontend Framework** | React.js (Vite) | 19.2 / 8.2 | Bileşen bazlı modern tek sayfa uygulaması (SPA) |
| **Harita & CBS Katmanı** | Leaflet & React-Leaflet | 1.9.4 / 5.0 | İnteraktif harita, katman geçişleri ve pinleme |
| **Arayüz Tasarımı** | Vanilla CSS (Glassmorphism) | Native | Modern, yarı saydam buzlu cam estetiği |
| **Konteynerizasyon** | Docker & Docker Compose | Güncel | Tüm sistemi tek komutla ayağa kaldırma |
| **Meteoroloji API'leri** | Open-Meteo & RainViewer | Ücretsiz | Canlı sıcaklık/rüzgar ve yağış radarı |

---

## 🌟 Öne Çıkan Fonksiyonel Özellikler

1. **Deterministik Ziraat Kural Motoru (Sıfır Yapay Zeka Maliyeti):**
   - Her tıklamada dış LLM (ChatGPT / OpenAI) API'lerine para ödemek ve saniyelerce beklemek yerine, projenin içerisine gömülü `AgronomyRuleEngine.cs` uzman sistemi çalışır.
   - Azot, fosfor, potasyum ve organik madde eksikliklerini tespit ederek çiftçiye anında uygulanabilir reçete sunar (Örn: *"Kök gelişimi için dekara 15-20 kg DAP gübresi uygulayın"*).
2. **Dirençli Dış Servis Mimarisi (Polly Fallback):**
   - Global toprak verisi için ISRIC SoilGrids REST API kullanılır; servis çökerse devre kesici (Circuit Breaker) devreye girer ve Open-Meteo toprak nemi/sıcaklığı verilerinden kestirim yapan yedek algoritma çalışır.
3. **Canlı Yağış Isı Haritası:**
   - RainViewer radar katmanı sayesinde harita üzerine canlı bulutlanma ve yağış katmanı giydirilebilir.

---

## 🚀 Hızlı Başlangıç (Tek Komutla Çalıştırma)

### Gereksinimler
- Bilgisayarınızda [Docker Desktop](https://www.docker.com/products/docker-desktop/) kurulu ve çalışır durumda olmalıdır.

### 1. Docker Compose ile Tüm Sistemi Başlatın (Tavsiye Edilen)

Projenin kök dizininde şu tek komutu çalıştırmanız yeterlidir:

```bash
docker compose up --build
```

Bu komut:
1. **PostGIS 16** veritabanını `5434` portunda başlatır.
2. **.NET 10 Web API** servisini `5114` portunda derleyip açar (tabloları otomatik oluşturur ve 17.5 MB'lık GeoJSON verisini aktarır).
3. **React Frontend** uygulamasını Nginx üzerinde `5173` portunda sunar.

### 2. Uygulamalara Erişin

| Servis | URL | Açıklama |
| :--- | :--- | :--- |
| **CBS Haritası & Karar Destek Paneli** | [http://localhost:5173](http://localhost:5173) | Çiftçi / Ziraat uzmanı web arayüzü |
| **Backend API (Swagger UI)** | [http://localhost:5114/swagger](http://localhost:5114/swagger) | Etkileşimli API test ve dokümantasyon ekranı |
| **PostgreSQL / PostGIS** | `localhost:5434` | Dış CBS araçları (QGIS / DBeaver) için doğrudan bağlantı |

---

## 💻 Yerel Geliştirme (Lokalde Debug Etmek İsteyenler İçin)

1. **Sadece PostGIS veritabanını Docker ile başlatın:**
   ```bash
   docker run --name agristress-db -e POSTGRES_PASSWORD=postgres -e POSTGRES_USER=postgres -e POSTGRES_DB=agristress_db -p 5434:5432 -d postgis/postgis:15-3.4
   ```

2. **Backend API'yi çalıştırın:**
   ```bash
   dotnet run --project backend/src/services/AgriStress.API/AgriStress.Api.csproj
   ```

3. **Birim Testlerini Koşturun:**
   ```bash
   dotnet test backend/src/tests/AgriStress.UnitTests/AgriStress.UnitTests.csproj
   ```

4. **Frontend'i başlatın:**
   ```bash
   cd frontend
   npm install
   npm run dev
   ```

---

