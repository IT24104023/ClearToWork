import React from 'react';
import { useGetSafetyAnalyticsQuery } from '../store/apiSlice';
import {
  BarChart3,
  TrendingUp,
  AlertOctagon,
  Clock,
  CheckCircle2,
  ShieldAlert,
} from 'lucide-react';

export const AnalyticsPage: React.FC = () => {
  const { data: analytics, isLoading, error } = useGetSafetyAnalyticsQuery();

  if (isLoading) {
    return (
      <div className="p-12 text-center text-slate-400">
        <div className="animate-spin w-8 h-8 border-2 border-amber-400 border-t-transparent rounded-full mx-auto mb-3"></div>
        <span>Aggregating plant safety telemetry...</span>
      </div>
    );
  }

  if (error || !analytics) {
    return (
      <div className="p-8 text-center text-rose-400">
        Failed to load safety analytics. Ensure backend API is active.
      </div>
    );
  }

  const refusalCauses = Object.entries(analytics.refusalCausesRanking || {});
  const maxRefusalCount = Math.max(...refusalCauses.map(([, count]) => count), 1);

  return (
    <div className="space-y-6 max-w-6xl mx-auto">
      {/* Header */}
      <div>
        <h1 className="text-2xl font-black text-slate-100 tracking-tight flex items-center gap-2">
          <BarChart3 className="w-6 h-6 text-amber-400" />
          <span>HSE Plant Safety Analytics & Refusal Audit</span>
        </h1>
        <p className="text-xs text-slate-400 mt-1">
          Historical trends, root-cause refusal rankings, and Mean Time to Approval (MTTA).
        </p>
      </div>

      {/* Top 4 KPI Cards */}
      <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-4">
        <div className="bg-slate-900 border border-slate-800 rounded-xl p-4 shadow-sm">
          <div className="flex items-center justify-between">
            <span className="text-xs text-slate-400 font-semibold uppercase">Permits Approved</span>
            <CheckCircle2 className="w-4 h-4 text-emerald-400" />
          </div>
          <div className="text-2xl font-black text-white mt-2 font-mono">{analytics.totalPermitsIssued}</div>
          <div className="text-[11px] text-emerald-400 flex items-center gap-1 mt-1 font-medium">
            <TrendingUp className="w-3 h-3" />
            <span>Passed deterministic checks</span>
          </div>
        </div>

        <div className="bg-slate-900 border border-slate-800 rounded-xl p-4 shadow-sm">
          <div className="flex items-center justify-between">
            <span className="text-xs text-slate-400 font-semibold uppercase">Safe Failures / Refused</span>
            <AlertOctagon className="w-4 h-4 text-rose-400" />
          </div>
          <div className="text-2xl font-black text-white mt-2 font-mono">{analytics.totalPermitsRefused}</div>
          <div className="text-[11px] text-rose-400 mt-1 font-medium">
            Incidents prevented before site entry
          </div>
        </div>

        <div className="bg-slate-900 border border-slate-800 rounded-xl p-4 shadow-sm">
          <div className="flex items-center justify-between">
            <span className="text-xs text-slate-400 font-semibold uppercase">Active Hot Work</span>
            <ShieldAlert className="w-4 h-4 text-amber-400" />
          </div>
          <div className="text-2xl font-black text-white mt-2 font-mono">{analytics.activePermitsCount}</div>
          <div className="text-[11px] text-amber-400 mt-1 font-medium">
            Currently undergoing execution on-site
          </div>
        </div>

        <div className="bg-slate-900 border border-slate-800 rounded-xl p-4 shadow-sm">
          <div className="flex items-center justify-between">
            <span className="text-xs text-slate-400 font-semibold uppercase">Mean Time to Approval</span>
            <Clock className="w-4 h-4 text-blue-400" />
          </div>
          <div className="text-2xl font-black text-white mt-2 font-mono">
            {analytics.meanTimeToApprovalHours} <span className="text-sm font-normal text-slate-400">hours</span>
          </div>
          <div className="text-[11px] text-blue-400 mt-1 font-medium">
            Sub-2h turnaround with AI assist
          </div>
        </div>
      </div>

      {/* Refusal Cause Ranking */}
      <div className="bg-slate-900 border border-slate-800 rounded-xl p-6 shadow-sm">
        <h3 className="text-sm font-bold text-slate-100 mb-1">
          Root-Cause Safe Failure Ranking (Incident Prevention)
        </h3>
        <p className="text-xs text-slate-400 mb-6">
          Categorization of unsafe conditions caught by LangGraph agents & deterministic rules before permit sign-off.
        </p>

        <div className="space-y-4">
          {refusalCauses.map(([cause, count], idx) => {
            const percentage = Math.round((count / maxRefusalCount) * 100);
            return (
              <div key={idx} className="space-y-1.5">
                <div className="flex items-center justify-between text-xs">
                  <span className="font-semibold text-slate-200">{cause}</span>
                  <span className="font-mono text-slate-400 font-bold">{count} occurrences</span>
                </div>
                <div className="w-full bg-slate-950 h-3 rounded-full overflow-hidden border border-slate-800">
                  <div
                    className="bg-amber-500 h-full rounded-full transition-all duration-500"
                    style={{ width: `${percentage}%` }}
                  ></div>
                </div>
              </div>
            );
          })}
        </div>
      </div>

      {/* Zone Distribution */}
      <div className="bg-slate-900 border border-slate-800 rounded-xl p-6 shadow-sm">
        <h3 className="text-sm font-bold text-slate-100 mb-1">
          Permit Workload Distribution by Plant Zone
        </h3>
        <p className="text-xs text-slate-400 mb-4">
          Active operational intensity across industrial sectors.
        </p>

        <div className="grid grid-cols-2 sm:grid-cols-4 gap-3">
          {Object.entries(analytics.permitsByZoneDistribution || {}).map(([zone, count], idx) => (
            <div key={idx} className="bg-slate-950 p-3 rounded-lg border border-slate-800">
              <span className="text-slate-500 text-[10px] uppercase font-mono block">Zone Ref {zone}</span>
              <span className="text-lg font-black text-amber-400 font-mono mt-1 block">{count} Permits</span>
            </div>
          ))}
        </div>
      </div>
    </div>
  );
};
