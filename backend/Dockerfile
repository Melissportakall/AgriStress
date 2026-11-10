# 1. Derleme (Build) Aşaması
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Proje dosyalarını kopyala ve restore et (Önbellek optimizasyonu için)
COPY src/services/AgriStress.Domain/AgriStress.Domain.csproj src/services/AgriStress.Domain/
COPY src/services/AgriStress.Application/AgriStress.Application.csproj src/services/AgriStress.Application/
COPY src/services/AgriStress.Infrastructure/AgriStress.Infrastructure.csproj src/services/AgriStress.Infrastructure/
COPY src/services/AgriStress.API/AgriStress.Api.csproj src/services/AgriStress.API/

RUN dotnet restore src/services/AgriStress.API/AgriStress.Api.csproj

# Tüm kaynak kodları kopyala ve yayınla
COPY src/ src/
RUN dotnet publish src/services/AgriStress.API/AgriStress.Api.csproj -c Release -o /app/publish /p:UseAppHost=false

# 2. Çalışma (Runtime) Aşaması
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

# Derlenen API çıktısını kopyala
COPY --from=build /app/publish .

# 17.5 MB'lık Pilot Toprak GeoJSON veri setini kopyala
COPY data/ ./data/

# API Portu
ENV ASPNETCORE_URLS=http://+:5114
EXPOSE 5114

ENTRYPOINT ["dotnet", "AgriStress.Api.dll"]
