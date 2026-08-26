import React, { useState } from 'react';
import MapComponent from './components/MapComponent';
import AnalysisPanel from './components/AnalysisPanel';
import './App.css';

function App() {
  const [report, setReport] = useState(null);
  const [loading, setLoading] = useState(false);
  const [location, setLocation] = useState(null);
  const [panelOpen, setPanelOpen] = useState(false);

  const handleLocationSelect = async (lat, lon) => {
    setLocation({ lat, lon });
    setLoading(true);
    setPanelOpen(true);
    setReport(null);

    try {
      const response = await fetch(`http://localhost:5114/api/SoilAnalysis/analyze?lat=${lat}&lon=${lon}`);
      
      if (!response.ok) {
        if (response.status === 404) {
          alert('Bu bölge için toprak analizi bulunamadı. Lütfen Bursa / Nilüfer pilot bölgesinden bir yer seçin.');
        } else {
          alert('Analiz sırasında bir hata oluştu.');
        }
        setLoading(false);
        return;
      }
      
      const data = await response.json();
      setReport(data);
    } catch (error) {
      console.error("Error fetching analysis:", error);
      alert('Sunucuya bağlanılamadı. Backend API çalışıyor mu kontrol edin.');
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="app-container">
      <div className="floating-header glass">
        <h2>🌱 AgriStress Analiz</h2>
        <p>Bursa / Nilüfer</p>
      </div>
      <MapComponent onLocationSelect={handleLocationSelect} selectedLocation={location} />
      <AnalysisPanel report={report} loading={loading} isOpen={panelOpen} onClose={() => setPanelOpen(false)} />
    </div>
  );
}

export default App;
