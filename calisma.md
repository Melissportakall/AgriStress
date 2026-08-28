# 🚜 AgriStress - Çalışma Özeti ve Mimari Kararlar (calisma.md)

Bu belge, AgriStress projesinin baştan sona nasıl inşa edildiğini, hangi teknolojilerin nerede ve **neden** kullanıldığını detaylandıran bir mimari karar raporudur.

---

## 1. Veritabanı ve Mekânsal İşleme (Spatial Engine)
**Kullanılan Teknoloji:** PostgreSQL + PostGIS (Eklenti)  
**Nerede Kullanıldı:** Pilot bölge toprak haritalarını (GeoJSON formatındaki büyük çokgen/poligon verilerini) saklamak ve sorgulamak için.

**Neden Kullanıldı?**
- Elimizde 17MB boyutunda, on binlerce köşeden oluşan karmaşık "MultiPolygon" verileri vardı. 
- Bir çiftçi haritadan bir noktaya tıkladığında, arka planda "Bu X, Y koordinatı (Point), elimizdeki binlerce poligonun hangisinin içine düşüyor?" sorusuna (Point-in-Polygon) yanıt bulmamız gerekiyordu.
- Geleneksel veritabanları (MySQL, düz PostgreSQL) veya bellek-içi (In-Memory) çözümler bu sorguyu yapmak için tüm veriyi tek tek taramak (Full Table Scan) zorundadır. Bu da saniyeler, hatta dakikalar sürer.
- Biz **PostGIS** kullanarak `Boundary` (Sınır) kolonuna **GIST (Spatial Index - Mekânsal İndeks)** ekledik. Böylece `ST_Contains` ve `ST_Transform` gibi PostGIS fonksiyonları sayesinde, tıklanan koordinatın hangi tarlaya ait olduğunu **milisaniyeler** içinde bulabildik.

---

## 2. Backend (Sunucu ve İş Mantığı)
**Kullanılan Teknoloji:** .NET 10 (C#) ASP.NET Core Web API & Clean Architecture  
**Nerede Kullanıldı:** Sistemin beyni olarak veritabanı sorgularını yapmak, API sunmak ve kural motorlarını (Rule Engine) çalıştırmak için.

**Neden Kullanıldı?**
- **Clean Architecture (Temel Mimari):** Projeyi Domain, Infrastructure ve Api olarak katmanlara ayırdık. Bu sayede veritabanı işlemleri (EF Core) ile iş mantığı birbirine karışmadı. Proje gelecekte büyümeye ve takım halinde çalışmaya çok uygun hale geldi.
- **Entity Framework Core (EF Core):** Code-First yaklaşımıyla SQL yazmakla uğraşmadan, C# sınıfları (Class) üzerinden veritabanı tablolarını (`SoilZones`) yönettik. NetTopologySuite kütüphanesi sayesinde PostGIS veri tiplerini (Point, Polygon) C# içerisinde doğuştan desteklenen veri tipleriymiş gibi kullanabildik.

---

## 3. Akıllı Karar Destek Sistemi (Kural Motoru)
**Kullanılan Teknoloji:** C# Kural Motoru (`AgronomyRuleEngine`)  
**Nerede Kullanıldı:** Toprak analizi sonrasında çiftçiye "Akıllı Reçete" (ne yapması gerektiği) sunulan kısımda.

**Neden Kullanıldı?**
- Başlangıçta bu kısmı OpenAI (ChatGPT) veya Grok gibi bir yapay zeka servisiyle (LLM) yapmayı planladık.
- Ancak dış API'lere (LLM'lere) her tıklamada istek atmak hem **API Key (ücret) maliyeti** doğuracak hem de cevap süresini 2-3 saniye uzatacaktı.
- Bunun yerine **Pivot** ettik! Ziraat mühendisliği kurallarını if/else mantığıyla C# kodlarına (`AgronomyRuleEngine.cs`) gömdük (Örn: `if (OrganicMatter < 2.0) { "Organik gübre atın..." }`). 
- Sonuç: Tamamen **ücretsiz**, milisaniyeler içinde çalışan ve dışarıya bağımlı olmayan çok hızlı bir uzman sistem elde ettik.

---

## 4. Dış Veri Kaynakları (Hava Durumu ve Yağış)
**Kullanılan Teknolojiler:** Open-Meteo API & RainViewer API  
**Nerede Kullanıldı:** Tıklanan tarlanın anlık sıcaklık/rüzgar durumunu ve harita üzerindeki yağış (bulut) radarını göstermek için.

**Neden Kullanıldı?**
- **Open-Meteo:** Tamamen ücretsiz ve API Key gerektirmeyen (Açık Kaynak) muazzam bir meteoroloji servisi. Sadece koordinat gönderip anında sonuç almak için backend'e `OpenMeteoWeatherService` olarak eklendi.
- **RainViewer:** Harita üzerinde yağışlı alanları ısı haritası (Heatmap) gibi renklendirerek göstermek için kullanıldı. Leaflet (TileLayer) ile çok kolay entegre edilebildiği ve yine API Key istemediği için mükemmel bir interaktif deneyim sundu.

---

## 5. Frontend (Kullanıcı Arayüzü)
**Kullanılan Teknoloji:** React.js (Vite), React-Leaflet, Vanilla CSS  
**Nerede Kullanıldı:** Çiftçinin veya kullanıcının etkileşime girdiği web ekranında.

**Neden Kullanıldı?**
- **React.js:** Bileşen bazlı (Component-based) yapısı sayesinde haritayı (`MapComponent.jsx`) ve analiz panelini (`AnalysisPanel.jsx`) birbirinden bağımsız, temiz ve yönetilebilir kod blokları halinde yazmamızı sağladı.
- **React-Leaflet:** Harita çizimi (OpenStreetMap) için piyasadaki en hafif ve yetenekli kütüphane.
- **Glassmorphism (Cam Efekti) Tasarımı:** Kullanıcıyı veri boğuculuğundan kurtarmak, premium ve modern bir hissiyat (Apple benzeri tasarım) vermek için arka planı bulanık, yarı saydam şık CSS kartları tasarlandı.

---

## 🚀 Özet
AgriStress, büyük veri (Big Data - 17MB GeoJSON) işleme zorluğunu **PostGIS mekânsal indeksleme** ile aşan, dış hava durumu servislerini **maliyetsiz** (API Key'siz) entegre eden ve pahalı yapay zeka modelleri yerine **kendi iç kural motoruyla (Akıllı Reçete)** çiftçiye anında çözüm üreten, tam teşekküllü ve modern mimarili bir AgriTech (Tarım Teknolojisi) projesidir.
