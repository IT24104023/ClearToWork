import React, { useState, useEffect } from 'react';
import { useTranslation } from '../context/I18nContext';
import {
  Database,
  RefreshCw,
  Trash2,
  CheckCircle2,
  AlertTriangle,
  HardDrive,
  Table,
  Sparkles,
  Server
} from 'lucide-react';

interface DbSummary {
  provider: string;
  databaseFile: string;
  sizeBytes: number;
  sizeFormatted: string;
  status: string;
  tableCounts: Record<string, number>;
}

export const DatabaseAdminPage: React.FC = () => {
  const { t } = useTranslation();

  const [summary, setSummary] = useState<DbSummary | null>(null);
  const [loading, setLoading] = useState(false);
  const [actionMessage, setActionMessage] = useState<string | null>(null);
  const [errorMessage, setErrorMessage] = useState<string | null>(null);
  const [showConfirmReset, setShowConfirmReset] = useState(false);

  const fetchSummary = async () => {
    setLoading(true);
    try {
      const resp = await fetch('/api/Admin/database/summary', {
        headers: {
          Authorization: `Bearer ${localStorage.getItem('ctw_token') || ''}`
        }
      });
      if (!resp.ok) throw new Error('Failed to fetch summary');
      const data = await resp.json();
      setSummary(data);
    } catch (err: any) {
      setErrorMessage(err.message || 'Error connecting to database');
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchSummary();
  }, []);

  const handleSeed = async () => {
    setLoading(true);
    setActionMessage(null);
    setErrorMessage(null);
    try {
      const resp = await fetch('/api/Admin/database/seed', {
        method: 'POST',
        headers: {
          Authorization: `Bearer ${localStorage.getItem('ctw_token') || ''}`
        }
      });
      if (!resp.ok) throw new Error('Failed to seed database');
      const data = await resp.json();
      setActionMessage(data.message);
      await fetchSummary();
    } catch (err: any) {
      setErrorMessage(err.message || 'Seeding failed');
    } finally {
      setLoading(false);
    }
  };

  const handleReset = async () => {
    setLoading(true);
    setActionMessage(null);
    setErrorMessage(null);
    setShowConfirmReset(false);
    try {
      const resp = await fetch('/api/Admin/database/reset', {
        method: 'POST',
        headers: {
          Authorization: `Bearer ${localStorage.getItem('ctw_token') || ''}`
        }
      });
      if (!resp.ok) throw new Error('Failed to reset database');
      const data = await resp.json();
      setActionMessage(data.message);
      await fetchSummary();
    } catch (err: any) {
      setErrorMessage(err.message || 'Reset failed');
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="p-6 space-y-6 max-w-7xl mx-auto">
      {/* Header */}
      <div className="flex flex-col md:flex-row md:items-center md:justify-between gap-4 border-b border-slate-800 pb-4">
        <div className="flex items-center space-x-3">
          <div className="bg-amber-500/20 text-amber-400 p-2.5 rounded-xl border border-amber-500/30">
            <Database className="w-6 h-6" />
          </div>
          <div>
            <h1 className="text-2xl font-bold text-white tracking-tight">{t('db_admin_title')}</h1>
            <p className="text-xs text-slate-400 mt-0.5">{t('db_admin_subtitle')}</p>
          </div>
        </div>

        <div className="flex items-center space-x-3">
          <button
            onClick={fetchSummary}
            disabled={loading}
            className="flex items-center space-x-1.5 bg-slate-800 hover:bg-slate-700 text-slate-200 px-3 py-2 rounded-xl text-xs font-semibold border border-slate-700 transition-colors"
          >
            <RefreshCw className={`w-3.5 h-3.5 ${loading ? 'animate-spin' : ''}`} />
            <span>Refresh</span>
          </button>

          <button
            onClick={handleSeed}
            disabled={loading}
            className="flex items-center space-x-1.5 bg-amber-500 hover:bg-amber-400 text-slate-950 px-3.5 py-2 rounded-xl text-xs font-bold transition-colors shadow-sm"
          >
            <Sparkles className="w-3.5 h-3.5" />
            <span>{t('db_action_seed')}</span>
          </button>

          <button
            onClick={() => setShowConfirmReset(true)}
            disabled={loading}
            className="flex items-center space-x-1.5 bg-rose-950/40 hover:bg-rose-900/60 text-rose-300 border border-rose-800/60 px-3.5 py-2 rounded-xl text-xs font-bold transition-colors"
          >
            <Trash2 className="w-3.5 h-3.5 text-rose-400" />
            <span>{t('db_action_reset')}</span>
          </button>
        </div>
      </div>

      {/* Notifications */}
      {actionMessage && (
        <div className="p-3.5 bg-emerald-950/40 border border-emerald-800/60 rounded-xl text-emerald-300 text-xs flex items-center gap-2">
          <CheckCircle2 className="w-4 h-4 text-emerald-400 shrink-0" />
          <span>{actionMessage}</span>
        </div>
      )}

      {errorMessage && (
        <div className="p-3.5 bg-rose-950/40 border border-rose-800/60 rounded-xl text-rose-300 text-xs flex items-center gap-2">
          <AlertTriangle className="w-4 h-4 text-rose-400 shrink-0" />
          <span>{errorMessage}</span>
        </div>
      )}

      {/* Database Overview Cards */}
      {summary && (
        <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
          <div className="bg-slate-900 dark:bg-slate-900/80 p-4 rounded-xl border border-slate-800">
            <div className="flex items-center justify-between text-slate-400 text-xs mb-1">
              <span>Database Provider</span>
              <Server className="w-4 h-4 text-sky-400" />
            </div>
            <div className="text-lg font-bold text-white">{summary.provider}</div>
            <div className="text-[11px] text-slate-500 mt-1">ORM: Entity Framework Core 8</div>
          </div>

          <div className="bg-slate-900 dark:bg-slate-900/80 p-4 rounded-xl border border-slate-800">
            <div className="flex items-center justify-between text-slate-400 text-xs mb-1">
              <span>Database Size</span>
              <HardDrive className="w-4 h-4 text-amber-400" />
            </div>
            <div className="text-lg font-bold text-amber-400 font-mono">{summary.sizeFormatted}</div>
            <div className="text-[11px] text-slate-500 mt-1">File: {summary.databaseFile}</div>
          </div>

          <div className="bg-slate-900 dark:bg-slate-900/80 p-4 rounded-xl border border-slate-800">
            <div className="flex items-center justify-between text-slate-400 text-xs mb-1">
              <span>Health Status</span>
              <CheckCircle2 className="w-4 h-4 text-emerald-400" />
            </div>
            <div className="text-lg font-bold text-emerald-400">{summary.status}</div>
            <div className="text-[11px] text-slate-500 mt-1">WAL Journaling Active</div>
          </div>
        </div>
      )}

      {/* Table Records Grid */}
      {summary && (
        <div className="bg-slate-900 dark:bg-slate-900/80 rounded-2xl border border-slate-800 overflow-hidden shadow-md">
          <div className="px-5 py-4 bg-slate-950/60 border-b border-slate-800 flex items-center justify-between">
            <div className="flex items-center space-x-2">
              <Table className="w-4 h-4 text-amber-400" />
              <h2 className="font-bold text-white text-sm">Entity Relational Record Counts</h2>
            </div>
            <span className="text-xs font-mono text-slate-500">{Object.keys(summary.tableCounts).length} Relational Entities</span>
          </div>

          <div className="p-5 grid grid-cols-2 sm:grid-cols-3 lg:grid-cols-5 gap-3">
            {Object.entries(summary.tableCounts).map(([table, count]) => (
              <div key={table} className="p-3 bg-slate-950/70 border border-slate-800/80 rounded-xl">
                <div className="text-[11px] text-slate-400 truncate mb-1" title={table}>{table}</div>
                <div className="text-xl font-bold font-mono text-white">{count}</div>
                <div className="text-[10px] text-emerald-400/80 mt-0.5">Rows Verified</div>
              </div>
            ))}
          </div>
        </div>
      )}

      {/* Confirmation Modal */}
      {showConfirmReset && (
        <div className="fixed inset-0 bg-slate-950/80 backdrop-blur-sm z-50 flex items-center justify-center p-4">
          <div className="bg-slate-900 border border-slate-700 rounded-2xl p-6 max-w-md w-full space-y-4 shadow-2xl">
            <div className="flex items-center gap-3 text-rose-400">
              <AlertTriangle className="w-6 h-6" />
              <h3 className="font-bold text-white text-lg">Confirm Database Recreate</h3>
            </div>
            <p className="text-xs text-slate-300 leading-relaxed">
              {t('db_confirm_reset')}
            </p>
            <div className="flex justify-end gap-3 pt-2">
              <button
                onClick={() => setShowConfirmReset(false)}
                className="px-4 py-2 rounded-xl text-xs font-semibold bg-slate-800 hover:bg-slate-700 text-slate-300"
              >
                Cancel
              </button>
              <button
                onClick={handleReset}
                className="px-4 py-2 rounded-xl text-xs font-bold bg-rose-600 hover:bg-rose-500 text-white"
              >
                Yes, Recreate Database
              </button>
            </div>
          </div>
        </div>
      )}
    </div>
  );
};
