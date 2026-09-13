import type { PermitStatus } from '../types';

interface StatusBadgeProps {
  status: PermitStatus | string;
}

export const StatusBadge: React.FC<StatusBadgeProps> = ({ status }) => {
  const getBadgeStyle = () => {
    switch (status) {
      case 'Approved':
        return 'bg-emerald-100 text-emerald-800 border-emerald-300';
      case 'PendingApproval':
        return 'bg-amber-100 text-amber-800 border-amber-300';
      case 'AiReview':
        return 'bg-purple-100 text-purple-800 border-purple-300 animate-pulse';
      case 'Refused':
        return 'bg-rose-100 text-rose-800 border-rose-300';
      case 'Active':
        return 'bg-blue-100 text-blue-800 border-blue-300';
      case 'Closed':
        return 'bg-slate-100 text-slate-700 border-slate-300';
      default:
        return 'bg-gray-100 text-gray-700 border-gray-300';
    }
  };

  const getLabel = () => {
    switch (status) {
      case 'PendingApproval':
        return 'Pending HSE Sign-Off';
      case 'AiReview':
        return 'AI Agents Reviewing';
      case 'Refused':
        return 'Refused (Safe Failure)';
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
