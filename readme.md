# 🌾 AgriStress - Agricultural Land & Soil Stress Analysis Platform

[![.NET 10](https://img.shields.io/badge/.NET-10.0-purple.svg)](https://dotnet.microsoft.com/)
[![React 19](https://img.shields.io/badge/React-19.0-blue.svg)](https://react.dev/)
[![PostgreSQL](https://img.shields.io/badge/PostgreSQL-16-336791.svg)](https://www.postgresql.org/)
[![PostGIS](https://img.shields.io/badge/PostGIS-Spatial_Engine-green.svg)](https://postgis.net/)
[![Docker](https://img.shields.io/badge/Docker-Compose-2496ED.svg)](https://www.docker.com/)

> 🇹🇷 **[Türkçe Dokümantasyon için tıklayınız (Switch to Turkish)](README.tr.md)**

AgriStress is a modern **Precision Agriculture (AgriTech) Decision Support System** designed to diagnose soil stress, macro/micro nutrient reserves, land physiography, and crop suitability in **milliseconds** by clicking on agricultural parcels on an interactive Geographic Information Systems (GIS) map.

The platform processes an official pilot dataset (Bursa / Nilüfer basin, 17.5 MB GeoJSON) locally using **PostGIS Spatial Indexing (GIST)**, eliminating third-party AI/LLM API costs while delivering high-speed, deterministic agronomic prescriptions.

---

## 🎯 Educational Motivation: Mastering GIS (Geographic Information Systems)

Beyond its agricultural benefits, **the primary engineering goal of this project was to learn and master practical Geographic Information Systems (GIS / CBS)** in a full-stack, enterprise-grade architecture.

### What Was Accomplished in GIS / Spatial Engineering?

| GIS Domain / Concept | Implementation in AgriStress |
| :--- | :--- |
| **Vector Big Data & MultiPolygon Handling** | Parsed and ingested a **17.5 MB official soil vector dataset** containing tens of thousands of vertices. MultiPolygon geometry was chosen over simple Polygon to support parcels bisected by waterways or roads. |
| **Coordinate Reference Systems (CRS) & Projections** | Solved cross-projection alignment: User map coordinates use **`EPSG:4326` (WGS84 Lat/Lon)**, while local Cadastral cadastral soil data is projected in **`EPSG:5254` (TUREF TM30 / UTM metric)**. Applied real-time coordinate reprojection via `ST_Transform`. |
| **Point-in-Polygon (PIP) Spatial Query** | Accurately matched farmer map click coordinates `Point(Lon, Lat)` to the corresponding agricultural polygon using PostGIS `ST_Contains`. |
| **Spatial Indexing with GIST (R-Tree)** | Prevented costly sequential scans (O(N) Full Table Scan). Added **GIST index** to the boundary column, accelerating spatial intersection queries from seconds down to **1–2 milliseconds**. |
| **Web GIS Multi-Layering** | Integrated Leaflet `LayersControl` allowing smooth toggling between standard OpenStreetMap vector tiles, high-resolution **ESRI World Imagery Satellite Tiles**, and live **RainViewer Weather Radar Heatmap** overlays. |
| **Dynamic Administrative Boundary Rendering** | Programmatically fetched and styled the Bursa/Nilüfer municipal boundary polygon using OpenStreetMap Nominatim GeoJSON API. |

---

## 🏗️ Architecture: Enterprise Clean Architecture

The solution adheres strictly to **Clean Architecture** organized with enterprise `src/services/` and `src/tests/` groupings:

```text
AgriStress/
├── backend/
│   ├── src/
│   │   ├── services/
│   │   │   ├── AgriStress.API/             # Presentation: Controllers, Middlewares, Configurations, Program.cs
│   │   │   ├── AgriStress.Application/     # Use Cases: DTOs, Exceptions, Interfaces, AgronomyRuleEngine
│   │   │   ├── AgriStress.Domain/          # Core: Entities (SoilZone, Field), Enums, BaseEntity
│   │   │   └── AgriStress.Infrastructure/  # Adapters: DataAccess (PostGIS AppDbContext), Migrations, ExternalServices, Seeder
│   │   └── tests/
│   │       └── AgriStress.UnitTests/       # Automated xUnit test suite (Agronomy rules & business logic)
│   ├── tools/
│   │   └── SoilTester/                     # Standalone CLI tool for ISRIC SoilGrids REST API validation
│   └── data/
│       └── geotoprak_sinifi_32768.geojson  # 17.5 MB official regional soil dataset
├── frontend/                               # React 19 + Leaflet GIS client application
├── docker-compose.yml                      # Full-stack container orchestration
└── README.md
```

### Layer Responsibilities Matrix

| Layer | Responsibility | Key Components |
| :--- | :--- | :--- |
| **API** | HTTP interface, routing, CORS, exception handling middleware, dependency injection | [Controllers](backend/src/services/AgriStress.API/Controllers), [Middlewares](backend/src/services/AgriStress.API/Middlewares), [Configurations](backend/src/services/AgriStress.API/Configurations) |
| **Application** | Business rules, calculations, data transfer objects, application contracts | [AgronomyRuleEngine](backend/src/services/AgriStress.Application/Services/AgronomyRuleEngine.cs), [DTOs](backend/src/services/AgriStress.Application/DTOs), [Exceptions](backend/src/services/AgriStress.Application/Exceptions) |
| **Domain** | Pure enterprise domain models, constants, and entities (Zero external dependencies) | [SoilZone](backend/src/services/AgriStress.Domain/Entities/SoilZone.cs), [Field](backend/src/services/AgriStress.Domain/Entities/Field.cs), [BaseEntity](backend/src/services/AgriStress.Domain/Common/BaseEntity.cs) |
| **Infrastructure** | Database access, PostGIS spatial queries, external APIs, resilient HTTP clients | [AppDbContext](backend/src/services/AgriStress.Infrastructure/DataAccess/AppDbContext.cs), [SpatialSoilService](backend/src/services/AgriStress.Infrastructure/Services/SpatialSoilService.cs), [ExternalServices](backend/src/services/AgriStress.Infrastructure/ExternalServices) |
| **UnitTests** | Verification of rules, edge cases, and calculation outcomes | [AgronomyRuleEngineTests](backend/src/tests/AgriStress.UnitTests/AgronomyRuleEngineTests.cs) (xUnit) |

---

## 🛠️ Technology Stack

| Component | Technology | Version | Purpose |
| :--- | :--- | :--- | :--- |
| **Backend Framework** | .NET (C#) / ASP.NET Core | 10.0 | High-performance REST API |
| **Database & GIS Engine** | PostgreSQL + PostGIS | 16 / 3.4 | Spatial polygon storage & index engine |
| **Spatial Library** | NetTopologySuite (NTS) | 2.6.0 | Geometries (`Point`, `MultiPolygon`) in C# |
| **Database ORM** | Entity Framework Core | 10.0 | Code-First migrations & PostGIS mapping |
| **Fault Tolerance** | Polly | 8.1.0 | Retry & Circuit Breaker policies |
| **Frontend Framework** | React.js (Vite) | 19.2 / 8.2 | Component-driven Single Page Application |
| **Web Mapping** | Leaflet & React-Leaflet | 1.9.4 / 5.0 | Interactive GIS map canvas & layers |
| **Styling** | Vanilla CSS (Glassmorphism) | Native | Apple-inspired translucent glass UI |
| **Containerization** | Docker & Docker Compose | Latest | One-command orchestration across all services |
| **Meteorology API** | Open-Meteo & RainViewer | Free APIs | Real-time weather & live precipitation radar |

---

## 🌟 Key Functional Features

1. **Deterministic Agronomy Rule Engine (Zero AI Cost):**
   - Instead of sending slow, costly queries to commercial LLMs, the platform leverages `AgronomyRuleEngine.cs` based on established agronomic science.
   - Evaluates Nitrogen, Phosphorus, Potassium, and Organic Matter thresholds to deliver actionable fertilization prescriptions (e.g., *“Apply 15-20 kg/da DAP fertilizer due to phosphorus deficiency”*).
2. **Resilient SoilGrids & Weather Integration:**
   - Incorporates ISRIC SoilGrids v2.0 REST API with circuit breaker fallbacks.
   - Live temperature and wind speed via Open-Meteo API.
3. **Live Precipitation Heatmap:**
   - RainViewer satellite radar overlay toggleable directly on the Leaflet map.

---

## 🚀 Quick Start (One-Command Setup)

### Prerequisites
- [Docker Desktop](https://www.docker.com/products/docker-desktop/) installed and running.

### 1. Launch All Services via Docker Compose (Recommended)

In the root directory of the project, run:

```bash
docker compose up --build
```

This single command:
1. Starts **PostGIS 16** on port `5434`.
2. Builds and starts the **.NET 10 API** on port `5114` (automatically applies migrations and seeds the 17.5 MB GeoJSON dataset).
3. Builds and serves the **React Frontend** via Nginx on port `5173`.

### 2. Access the Applications

| Application | URL | Description |
| :--- | :--- | :--- |
| **GIS Map & Decision Dashboard** | [http://localhost:5173](http://localhost:5173) | Interactive farmer/agronomist map interface |
| **API Swagger UI** | [http://localhost:5114/swagger](http://localhost:5114/swagger) | Interactive API documentation |
| **PostgreSQL / PostGIS** | `localhost:5434` | Direct connection for QGIS / DBeaver |

---

## 💻 Local Development Workflow (Without Full Docker)

If you prefer to debug locally in your IDE:

1. **Start PostGIS only:**
   ```bash
   docker run --name agristress-db -e POSTGRES_PASSWORD=postgres -e POSTGRES_USER=postgres -e POSTGRES_DB=agristress_db -p 5434:5432 -d postgis/postgis:15-3.4
   ```

2. **Run Backend API:**
   ```bash
   dotnet run --project backend/src/services/AgriStress.API/AgriStress.Api.csproj
   ```

3. **Run Unit Tests:**
   ```bash
   dotnet test backend/src/tests/AgriStress.UnitTests/AgriStress.UnitTests.csproj
   ```

4. **Run Frontend (Vite Dev Server):**
   ```bash
   cd frontend
   npm install
   npm run dev
   ```

---


