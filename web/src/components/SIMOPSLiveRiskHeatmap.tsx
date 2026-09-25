import React from 'react';

export const SIMOPSLiveRiskHeatmap: React.FC = () => {
  return (
    <div className="risk-heatmap-card">
      <h4>SIMOPS Live Spatial Heatmap</h4>
      <div className="zone-grid">
        <div className="zone-cell zone-red">Zone A1: Hot Work + Pressure Test (CRITICAL)</div>
        <div className="zone-cell zone-green">Zone B2: Cold Mechanical (SAFE)</div>
      </div>
    </div>
  );
};
