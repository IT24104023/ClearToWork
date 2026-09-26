import React from 'react';

export const EquipmentCalibrationCertificateModal: React.FC<{ certId: string }> = ({ certId }) => (
  <div className="modal-content">
    <h4>Calibration Certificate #{certId}</h4>
    <p>Calibrated against NIST traceable standard gas.</p>
  </div>
);
