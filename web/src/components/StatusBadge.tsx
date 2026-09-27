import React from 'react';
import type { PermitStatus } from '../types';

interface StatusBadgeProps {
  status: PermitStatus | string;
}

export const StatusBadge: React.FC<StatusBadgeProps> = ({ status }) => {
  const getBadgeStyle = () => {
    switch (status) {
      case 'Draft':
        return 'bg-slate-800 text-slate-300 border-slate-600';
      case 'Submitted':
        return 'bg-sky-950 text-sky-300 border-sky-700';
      case 'AiReview':
        return 'bg-purple-950 text-purple-300 border-purple-700 animate-pulse';
      case 'PendingApproval':
        return 'bg-amber-950 text-amber-300 border-amber-700';
      case 'Approved':
        return 'bg-emerald-950 text-emerald-300 border-emerald-700';
      case 'Active':
        return 'bg-blue-950 text-blue-300 border-blue-700';
      case 'Closed':
        return 'bg-slate-900 text-slate-500 border-slate-700';
      case 'Refused':
        return 'bg-rose-950 text-rose-300 border-rose-700';
      case 'Expired':
        return 'bg-orange-950 text-orange-300 border-orange-700';
      default:
        return 'bg-slate-800 text-slate-400 border-slate-600';
    }
  };

  const getLabel = () => {
    switch (status) {
      case 'Draft':
        return 'Draft';
      case 'Submitted':
        return 'Submitted';
      case 'AiReview':
        return 'AI Agents Reviewing';
      case 'PendingApproval':
        return 'Pending HSE Sign-Off';
      case 'Approved':
        return 'Approved';
      case 'Active':
        return 'Active on Site';
      case 'Closed':
        return 'Closed';
      case 'Refused':
        return 'Refused (Safe Failure)';
      case 'Expired':
        return 'Expired';
      default:
        return status;
    }
  };

  return (
    <span
      className={`inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-semibold border ${getBadgeStyle()}`}
    >
      <span className="w-1.5 h-1.5 rounded-full bg-current mr-1.5"></span>
      {getLabel()}
    </span>
  );
};
