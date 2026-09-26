import React from 'react';

export const PermitShiftHandoverModal: React.FC<{ isOpen: boolean; onClose: () => void }> = ({ isOpen, onClose }) => {
  if (!isOpen) return null;
  return (
    <div className="modal-card">
      <h3>12-Hour Shift Handover Confirmation</h3>
      <p>Outgoing Authority: M. Zakee | Incoming Authority: Night Duty PA</p>
      <button onClick={onClose} className="btn-primary">Sign & Handover</button>
    </div>
  );
};
