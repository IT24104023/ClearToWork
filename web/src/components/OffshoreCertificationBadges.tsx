import React from 'react';

export const OffshoreCertificationBadges: React.FC<{ certs: string[] }> = ({ certs }) => (
  <div className="badge-row">
    {certs.map(c => <span key={c} className="badge badge-opito">{c}</span>)}
  </div>
);
