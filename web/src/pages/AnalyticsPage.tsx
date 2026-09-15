import React from 'react';
import { useGetSafetyAnalyticsQuery } from '../store/apiSlice';
import { useTranslation } from '../context/I18nContext';
import {
  BarChart3,
  TrendingUp,
  AlertOctagon,
  Clock,
  CheckCircle2,
  ShieldAlert,
} from 'lucide-react';

export const AnalyticsPage: React.FC = () => {
  const { t } = useTranslation();
  const { data: analytics, isLoading, error } = useGetSafetyAnalyticsQuery();

  if (isLoading) {
    return (
      <div className="p-12 text-center text-slate-500 dark:text-slate-400">
        <div className="animate-spin w-8 h-8 border-2 border-amber-500 border-t-transparent rounded-full mx-auto mb-3"></div>
        <span>Aggregating plant safety telemetry...</span>
      </div>
    );
  }

  if (error || !analytics) {
    return (
      <div className="p-8 text-center text-rose-600 dark:text-rose-400">
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
        <h1 className="text-2xl font-black text-slate-900 dark:text-slate-100 tracking-tight flex items-center gap-2">
          <BarChart3 className="w-6 h-6 text-amber-500 dark:text-amber-400" />
          <span>{t('analytics_title')}</span>
        </h1>
        <p className="text-xs text-slate-500 dark:text-slate-400 mt-1">
          {t('analytics_subtitle')}
        </p>
      </div>

      {/* Top 4 KPI Cards */}
      <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-4">
        <div className="bg-white dark:bg-slate-900 border border-slate-200 dark:border-slate-800 rounded-2xl p-4 shadow-sm">
          <div className="flex items-center justify-between">
            <span className="text-xs text-slate-500 dark:text-slate-400 font-semibold uppercase">{t('analytics_kpi_approved')}</span>
            <CheckCircle2 className="w-4 h-4 text-emerald-500 dark:text-emerald-400" />
          </div>
          <div className="text-2xl font-black text-slate-900 dark:text-white mt-2 font-mono">{analytics.totalPermitsIssued}</div>
          <div className="text-[11px] text-emerald-600 dark:text-emerald-400 flex items-center gap-1 mt-1 font-medium">
            <TrendingUp className="w-3 h-3" />
            <span>{t('analytics_kpi_approved_sub')}</span>
          </div>
        </div>

        <div className="bg-white dark:bg-slate-900 border border-slate-200 dark:border-slate-800 rounded-2xl p-4 shadow-sm">
          <div className="flex items-center justify-between">
            <span className="text-xs text-slate-500 dark:text-slate-400 font-semibold uppercase">{t('analytics_kpi_refused')}</span>
            <AlertOctagon className="w-4 h-4 text-rose-500 dark:text-rose-400" />
          </div>
          <div className="text-2xl font-black text-slate-900 dark:text-white mt-2 font-mono">{analytics.totalPermitsRefused}</div>
          <div className="text-[11px] text-rose-600 dark:text-rose-400 mt-1 font-medium">
            {t('analytics_kpi_refused_sub')}
          </div>
        </div>

        <div className="bg-white dark:bg-slate-900 border border-slate-200 dark:border-slate-800 rounded-2xl p-4 shadow-sm">
          <div className="flex items-center justify-between">
            <span className="text-xs text-slate-500 dark:text-slate-400 font-semibold uppercase">{t('analytics_kpi_active')}</span>
            <ShieldAlert className="w-4 h-4 text-amber-500 dark:text-amber-400" />
          </div>
          <div className="text-2xl font-black text-slate-900 dark:text-white mt-2 font-mono">{analytics.activePermitsCount}</div>
          <div className="text-[11px] text-amber-600 dark:text-amber-400 mt-1 font-medium">
            {t('analytics_kpi_active_sub')}
          </div>
        </div>

        <div className="bg-white dark:bg-slate-900 border border-slate-200 dark:border-slate-800 rounded-2xl p-4 shadow-sm">
          <div className="flex items-center justify-between">
            <span className="text-xs text-slate-500 dark:text-slate-400 font-semibold uppercase">{t('analytics_kpi_mtta')}</span>
            <Clock className="w-4 h-4 text-blue-500 dark:text-blue-400" />
          </div>
          <div className="text-2xl font-black text-slate-900 dark:text-white mt-2 font-mono">
            {analytics.meanTimeToApprovalHours} <span className="text-sm font-normal text-slate-500 dark:text-slate-400">hours</span>
          </div>
          <div className="text-[11px] text-blue-600 dark:text-blue-400 mt-1 font-medium">
            {t('analytics_kpi_mtta_sub')}
          </div>
        </div>
      </div>

      {/* Refusal Cause Ranking */}
      <div className="bg-white dark:bg-slate-900 border border-slate-200 dark:border-slate-800 rounded-2xl p-6 shadow-sm">
        <h3 className="text-sm font-bold text-slate-900 dark:text-slate-100 mb-1">
          {t('analytics_causes_title')}
        </h3>
        <p className="text-xs text-slate-500 dark:text-slate-400 mb-6">
          {t('analytics_causes_subtitle')}
        </p>

        <div className="space-y-4">
          {refusalCauses.map(([cause, count], idx) => {
            const percentage = Math.round((count / maxRefusalCount) * 100);
            return (
              <div key={idx} className="space-y-1.5">
                <div className="flex items-center justify-between text-xs">
                  <span className="font-semibold text-slate-800 dark:text-slate-200">{cause}</span>
                  <span className="font-mono text-slate-500 dark:text-slate-400 font-bold">{count} occurrences</span>
                </div>
                <div className="w-full bg-slate-100 dark:bg-slate-950 h-3 rounded-full overflow-hidden border border-slate-200 dark:border-slate-800">
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
      <div className="bg-white dark:bg-slate-900 border border-slate-200 dark:border-slate-800 rounded-2xl p-6 shadow-sm">
        <h3 className="text-sm font-bold text-slate-900 dark:text-slate-100 mb-1">
          {t('analytics_zone_dist_title')}
        </h3>
        <p className="text-xs text-slate-500 dark:text-slate-400 mb-4">
          {t('analytics_zone_dist_subtitle')}
        </p>

        <div className="grid grid-cols-2 sm:grid-cols-4 gap-3">
          {Object.entries(analytics.permitsByZoneDistribution || {}).map(([zone, count], idx) => (
            <div key={idx} className="bg-slate-50 dark:bg-slate-950 p-3.5 rounded-xl border border-slate-200 dark:border-slate-800">
              <span className="text-slate-500 dark:text-slate-400 text-[10px] uppercase font-mono block">Zone Ref {zone}</span>
              <span className="text-lg font-black text-amber-600 dark:text-amber-400 font-mono mt-1 block">{count} Permits</span>
            </div>
          ))}
        </div>
      </div>
    </div>
  );
};
