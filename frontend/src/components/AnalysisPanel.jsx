import React from 'react';

const AnalysisPanel = ({ report, loading, isOpen, onClose }) => {
  return (
    <div className={`analysis-panel glass ${isOpen ? 'open' : 'closed'}`}>
      <button className="close-btn" onClick={onClose}>✕</button>
      
      {loading ? (
        <div className="panel-inner">
          <div className="loading-spinner"></div>
          <p style={{textAlign: 'center', color: '#4b5563', fontWeight: '500'}}>Toprak verileri analiz ediliyor...</p>
        </div>
      ) : !report ? (
        <div className="panel-inner">
          <p style={{textAlign: 'center', color: '#4b5563'}}>Lütfen haritadan bir konum seçin.</p>
        </div>
      ) : (
        <div className="panel-inner">
          <h3>📊 Toprak Sağlık Raporu</h3>
          
          <div className={`stress-badge ${report.generalStressLevel.includes('Yüksek') ? 'danger' : report.generalStressLevel.includes('Orta') ? 'warning' : 'success'}`}>
            Stres Seviyesi: {report.generalStressLevel}
          </div>

          <div className="metrics-grid">
            <div className="metric">
              <span>Azot (N)</span>
              <strong>{report.nitrogenValue ?? '-'}</strong>
              <small>{report.nitrogenStatus}</small>
            </div>
            <div className="metric">
              <span>Fosfor (P)</span>
              <strong>{report.phosphorusValue ?? '-'}</strong>
              <small>{report.phosphorusStatus}</small>
            </div>
            <div className="metric">
              <span>Organik Madde</span>
              <strong>{report.organicMatterPercentage ? `%${report.organicMatterPercentage}` : '-'}</strong>
            </div>
          </div>

          {report.weather && (
            <div className="weather-info glass" style={{ marginTop: '1rem', padding: '1rem', borderRadius: '8px', background: 'rgba(255, 255, 255, 0.5)' }}>
              <h4 style={{ margin: '0 0 0.5rem 0', color: '#1f2937' }}>🌤️ Anlık Hava Durumu</h4>
              <div style={{ display: 'flex', gap: '1rem' }}>
                <div>
                  <span style={{ fontSize: '0.8rem', color: '#4b5563', display: 'block' }}>Sıcaklık</span>
                  <strong style={{ fontSize: '1.2rem', color: '#10b981' }}>{report.weather.temperature}°C</strong>
                </div>
                <div>
                  <span style={{ fontSize: '0.8rem', color: '#4b5563', display: 'block' }}>Rüzgar</span>
                  <strong style={{ fontSize: '1.2rem', color: '#3b82f6' }}>{report.weather.windSpeed} km/s</strong>
                </div>
              </div>
            </div>
          )}

          {report.deficiencies && report.deficiencies.length > 0 && (
            <div className="deficiencies" style={{ marginTop: '1.5rem' }}>
              <h4>⚠️ Tespit Edilen Eksiklikler</h4>
              <ul>
                {report.deficiencies.map((def, idx) => (
                  <li key={idx}>{def}</li>
                ))}
              </ul>
            </div>
          )}

          {report.recommendedFruitsVegetables && (
            <div className="recommendations" style={{ marginTop: '1.5rem' }}>
              <h4>🍎 Uzman Önerisi</h4>
              <p>{report.recommendedFruitsVegetables}</p>
            </div>
          )}

          {report.improvementRecipe && (
            <div className="improvement-recipe glass" style={{ marginTop: '1.5rem', padding: '1rem', borderRadius: '8px', background: 'rgba(16, 185, 129, 0.15)', borderLeft: '4px solid #10b981' }}>
              <h4 style={{ margin: '0 0 0.5rem 0', color: '#047857', display: 'flex', alignItems: 'center', gap: '0.5rem' }}>
                <span style={{ fontSize: '1.2rem' }}>👨‍🌾</span> Akıllı Reçete
              </h4>
              <p style={{ margin: 0, color: '#4b5563', fontSize: '0.9rem', lineHeight: '1.4', whiteSpace: 'pre-line' }}>
                {report.improvementRecipe}
              </p>
            </div>
          )}
        </div>
      )}
    </div>
  );
};

export default AnalysisPanel;
