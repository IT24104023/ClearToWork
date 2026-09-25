import React from 'react';

export const WorkerMedicalClearanceModal: React.FC<{ isOpen: boolean; onClose: () => void }> = ({ isOpen, onClose }) => {
  if (!isOpen) return null;
  return (
    <div className="modal-backdrop">
      <div className="modal-card">
        <h3>Offshore Medical Clearance Verification</h3>
        <p>Status: <strong>FIT FOR DUTY</strong> (Expires: 2027-09-26)</p>
        <button onClick={onClose} className="btn-primary">Close</button>
      </div>
    </div>
  );
};
