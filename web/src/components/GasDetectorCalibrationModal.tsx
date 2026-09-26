import React from 'react';

export const GasDetectorCalibrationModal: React.FC<{ isOpen: boolean; onClose: () => void }> = ({ isOpen, onClose }) => {
  if (!isOpen) return null;
  return (
    <div className="modal-card">
      <h3>Dräger X-am 5000 Daily Bump Test</h3>
      <p>H2S: 25 ppm Span Gas OK | LEL: 50% Methane OK</p>
      <button onClick={onClose} className="btn-primary">Pass Calibration</button>
    </div>
  );
};
