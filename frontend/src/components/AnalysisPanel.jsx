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

          {report.deficiencies && report.deficiencies.length > 0 && (
            <div className="deficiencies">
              <h4>⚠️ Tespit Edilen Eksiklikler</h4>
              <ul>
                {report.deficiencies.map((def, idx) => (
                  <li key={idx}>{def}</li>
                ))}
              </ul>
            </div>
          )}

          {report.recommendedFruitsVegetables && (
            <div className="recommendations">
              <h4>🍎 Uzman Önerisi</h4>
              <p>{report.recommendedFruitsVegetables}</p>
            </div>
          )}
        </div>
      )}
    </div>
  );
};

export default AnalysisPanel;
