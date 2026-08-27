import React, { useState, useEffect } from 'react';
import { MapContainer, TileLayer, Marker, Popup, useMapEvents, GeoJSON, LayersControl } from 'react-leaflet';
import 'leaflet/dist/leaflet.css';
import L from 'leaflet';

// Fix for default Leaflet marker icon in React
import icon from 'leaflet/dist/images/marker-icon.png';
import iconShadow from 'leaflet/dist/images/marker-shadow.png';

let DefaultIcon = L.icon({
    iconUrl: icon,
    shadowUrl: iconShadow,
    iconSize: [25, 41],
    iconAnchor: [12, 41]
});
L.Marker.prototype.options.icon = DefaultIcon;

// Tıklama olaylarını dinleyen yardımcı component
const MapEvents = ({ onLocationSelect }) => {
  useMapEvents({
    click(e) {
      onLocationSelect(e.latlng.lat, e.latlng.lng);
    }
  });
  return null;
};

const MapComponent = ({ onLocationSelect, selectedLocation }) => {
  // Pilot bölge (Veri setinin tam merkezi)
  const position = [40.10, 28.80];
  const [niluferGeoJSON, setNiluferGeoJSON] = useState(null);
  const [radarPath, setRadarPath] = useState(null);

  useEffect(() => {
    // OpenStreetMap Nominatim üzerinden Nilüfer ilçesinin sınırlarını çekiyoruz
    fetch('https://nominatim.openstreetmap.org/search?state=Bursa&county=Nil%C3%BCfer&polygon_geojson=1&format=json')
      .then(res => res.json())
      .then(data => {
        if (data && data.length > 0 && data[0].geojson) {
          setNiluferGeoJSON(data[0].geojson);
        }
      })
      .catch(err => console.error("Nilüfer sınırı yüklenemedi:", err));

    // RainViewer API'den son radar karesini çekiyoruz
    fetch('https://api.rainviewer.com/public/weather-maps.json')
      .then(res => res.json())
      .then(data => {
        if (data && data.radar && data.radar.past && data.radar.past.length > 0) {
          // En son radar karesini al
          const latestFrame = data.radar.past[data.radar.past.length - 1];
          setRadarPath(latestFrame.path);
        }
      })
      .catch(err => console.error("RainViewer verisi çekilemedi:", err));
  }, []);

  return (
    <div className="map-wrapper">
      <MapContainer center={position} zoom={12} style={{ height: '100%', width: '100%' }}>
        <LayersControl position="topright">
          <LayersControl.BaseLayer checked name="Standart Harita">
            <TileLayer
              attribution='&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors'
              url="https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png"
            />
          </LayersControl.BaseLayer>
          
          <LayersControl.BaseLayer name="Uydu Haritası (ESRI)">
            <TileLayer
              attribution='&copy; Esri'
              url="https://server.arcgisonline.com/ArcGIS/rest/services/World_Imagery/MapServer/tile/{z}/{y}/{x}"
            />
          </LayersControl.BaseLayer>

          {radarPath && (
            <LayersControl.Overlay name="🌧️ Yağış Radarı (RainViewer)">
              <TileLayer
                attribution='&copy; <a href="https://www.rainviewer.com/api.html">RainViewer</a>'
                url={`https://tilecache.rainviewer.com${radarPath}/256/{z}/{x}/{y}/2/1_1.png`}
                opacity={0.6}
                zIndex={10}
                maxNativeZoom={10} // RainViewer yüksek zoom seviyelerini desteklemez
              />
            </LayersControl.Overlay>
          )}
        </LayersControl>
        
        {niluferGeoJSON && (
          <GeoJSON 
            data={niluferGeoJSON} 
            style={{
              color: '#16a34a', // Yeşil sınır
              weight: 3,
              opacity: 0.8,
              fillColor: '#dcfce7', // Açık yeşil dolgu
              fillOpacity: 0.1
            }} 
          />
        )}

        <MapEvents onLocationSelect={onLocationSelect} />
        
        {selectedLocation && (
          <Marker position={[selectedLocation.lat, selectedLocation.lon]}>
            <Popup>
              Seçilen Nokta <br />
              Lat: {selectedLocation.lat.toFixed(4)} <br />
              Lon: {selectedLocation.lon.toFixed(4)}
            </Popup>
          </Marker>
        )}
      </MapContainer>
    </div>
  );
};

export default MapComponent;
