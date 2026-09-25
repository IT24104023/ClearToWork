import React from 'react';

export const PermitRevalidationModal: React.FC<{ permitNo: string }> = ({ permitNo }) => (
  <div className="modal-content">
    <h4>Revalidate Permit {permitNo}</h4>
    <p>Perform gas re-check and joint site inspection before signing 12h extension.</p>
  </div>
);
