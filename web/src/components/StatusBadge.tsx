import React from 'react';

export const StatusBadge: React.FC<{ status: string }> = ({ status }) => {
  return (
    <span className="px-2.5 py-0.5 rounded-full text-xs font-semibold bg-amber-500/20 text-amber-400 border border-amber-500/30">
      {status}
    </span>
  );
};
