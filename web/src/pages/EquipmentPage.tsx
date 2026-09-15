import React, { useState } from 'react';
import { useGetEquipmentQuery } from '../store/apiSlice';
import { useTranslation } from '../context/I18nContext';
import { Wrench, CheckCircle2, XCircle, Search, Lock } from 'lucide-react';

export const EquipmentPage: React.FC = () => {
  const { t } = useTranslation();
  const [searchTerm, setSearchTerm] = useState('');
  const { data: assets = [], isLoading } = useGetEquipmentQuery();

  const filteredAssets = assets.filter(
    (a) =>
      a.name.toLowerCase().includes(searchTerm.toLowerCase()) ||
      a.assetTag.toLowerCase().includes(searchTerm.toLowerCase()) ||
      a.category.toLowerCase().includes(searchTerm.toLowerCase())
  );

  return (
    <div className="space-y-6 max-w-6xl mx-auto">
      {/* Header */}
      <div>
        <h1 className="text-2xl font-black text-slate-900 dark:text-slate-100 tracking-tight flex items-center gap-2">
          <Wrench className="w-6 h-6 text-amber-500 dark:text-amber-400" />
          <span>{t('equipment_title')}</span>
        </h1>
        <p className="text-xs text-slate-500 dark:text-slate-400 mt-1">
          {t('equipment_subtitle')}
        </p>
      </div>

      {/* Lockout / Tagout Notice */}
      <div className="bg-white dark:bg-slate-900 border border-slate-200 dark:border-slate-800 rounded-2xl p-5 shadow-sm">
        <div className="flex items-center gap-2 text-amber-600 dark:text-amber-400 font-bold text-sm mb-1">
          <Lock className="w-4 h-4" />
          <span>{t('equipment_loto_title')}</span>
        </div>
        <p className="text-xs text-slate-600 dark:text-slate-300 mb-3">
          {t('equipment_loto_desc')}
        </p>
        <div className="grid grid-cols-1 sm:grid-cols-2 gap-3 text-xs">
          <div className="bg-slate-50 dark:bg-slate-950 p-3.5 rounded-xl border border-slate-200 dark:border-slate-800 flex items-center justify-between">
            <div>
              <div className="font-mono text-amber-600 dark:text-amber-400 font-bold">ISO-B3-VALVE-01</div>
              <div className="text-slate-500 dark:text-slate-400 text-[11px] mt-0.5">Solvent supply isolation manifold</div>
            </div>
            <span className="bg-emerald-100 dark:bg-emerald-950 text-emerald-700 dark:text-emerald-400 border border-emerald-300 dark:border-emerald-800 px-2 py-0.5 rounded font-mono text-[10px] font-bold">
              LOCKED
            </span>
          </div>
          <div className="bg-slate-50 dark:bg-slate-950 p-3.5 rounded-xl border border-slate-200 dark:border-slate-800 flex items-center justify-between">
            <div>
              <div className="font-mono text-amber-600 dark:text-amber-400 font-bold">ISO-B3-ELEC-04</div>
              <div className="text-slate-500 dark:text-slate-400 text-[11px] mt-0.5">415V Main busbar isolator switch</div>
            </div>
            <span className="bg-emerald-100 dark:bg-emerald-950 text-emerald-700 dark:text-emerald-400 border border-emerald-300 dark:border-emerald-800 px-2 py-0.5 rounded font-mono text-[10px] font-bold">
              TAGGED
            </span>
          </div>
        </div>
      </div>

      {/* Search Bar */}
      <div className="bg-white dark:bg-slate-900 p-4 rounded-2xl border border-slate-200 dark:border-slate-800 flex items-center justify-between shadow-sm">
        <div className="w-full max-w-md relative">
          <Search className="w-4 h-4 text-slate-400 absolute left-3 top-1/2 -translate-y-1/2" />
          <input
            type="text"
            placeholder={t('equipment_search_placeholder')}
            value={searchTerm}
            onChange={(e) => setSearchTerm(e.target.value)}
            className="w-full pl-9 pr-4 py-2.5 bg-slate-50 dark:bg-slate-950 border border-slate-300 dark:border-slate-700 rounded-xl text-xs text-slate-900 dark:text-white placeholder-slate-400 dark:placeholder-slate-500 focus:outline-none focus:ring-1 focus:ring-amber-500 transition-colors"
          />
        </div>
      </div>

      {/* Equipment Table */}
      <div className="bg-white dark:bg-slate-900 rounded-2xl border border-slate-200 dark:border-slate-800 overflow-hidden shadow-sm">
        {isLoading ? (
          <div className="p-8 text-center text-slate-500 dark:text-slate-400 text-xs">Loading equipment inventory...</div>
        ) : (
          <div className="overflow-x-auto">
            <table className="w-full text-left text-xs border-collapse">
              <thead>
                <tr className="bg-slate-50 dark:bg-slate-950/80 border-b border-slate-200 dark:border-slate-800 text-slate-500 dark:text-slate-400 uppercase text-[11px] font-semibold">
                  <th className="py-3 px-4">{t('equipment_th_tag')}</th>
                  <th className="py-3 px-4">{t('equipment_th_name')}</th>
                  <th className="py-3 px-4">{t('equipment_th_category')}</th>
                  <th className="py-3 px-4">{t('equipment_th_inspection')}</th>
                  <th className="py-3 px-4">{t('equipment_th_calibration')}</th>
                  <th className="py-3 px-4">{t('equipment_th_readiness')}</th>
                </tr>
              </thead>
              <tbody className="divide-y divide-slate-200 dark:divide-slate-800/60">
                {filteredAssets.map((asset) => {
                  const isReady = asset.isInspectionValid && asset.isCalibrationValid;

                  return (
                    <tr key={asset.id} className="hover:bg-slate-50 dark:hover:bg-slate-800/50 transition-colors">
                      <td className="py-3 px-4 font-mono font-bold text-amber-600 dark:text-amber-400">{asset.assetTag}</td>
                      <td className="py-3 px-4 font-semibold text-slate-800 dark:text-slate-200">{asset.name}</td>
                      <td className="py-3 px-4 text-slate-500 dark:text-slate-400">{asset.category}</td>
                      <td className="py-3 px-4">
                        {asset.isInspectionValid ? (
                          <span className="text-emerald-600 dark:text-emerald-400 flex items-center gap-1 font-mono font-semibold">
                            <CheckCircle2 className="w-3.5 h-3.5" /> {t('equipment_status_indate')}
                          </span>
                        ) : (
                          <span className="text-rose-600 dark:text-rose-400 flex items-center gap-1 font-mono font-bold">
                            <XCircle className="w-3.5 h-3.5" /> {t('equipment_status_overdue')}
                          </span>
                        )}
                      </td>
                      <td className="py-3 px-4">
                        {asset.isCalibrationValid ? (
                          <span className="text-emerald-600 dark:text-emerald-400 flex items-center gap-1 font-mono font-semibold">
                            <CheckCircle2 className="w-3.5 h-3.5" /> {t('equipment_status_certified')}
                          </span>
                        ) : (
                          <span className="text-slate-400 dark:text-slate-500 font-mono">N/A</span>
                        )}
                      </td>
                      <td className="py-3 px-4">
                        <span
                          className={`px-2.5 py-0.5 rounded text-[10px] uppercase font-bold border ${
                            isReady
                              ? 'bg-emerald-100 dark:bg-emerald-950 text-emerald-700 dark:text-emerald-400 border-emerald-300 dark:border-emerald-800'
                              : 'bg-rose-100 dark:bg-rose-950 text-rose-700 dark:text-rose-400 border-rose-300 dark:border-rose-800'
                          }`}
                        >
                          {isReady ? t('equipment_status_cleared') : t('equipment_status_restricted')}
                        </span>
                      </td>
                    </tr>
                  );
                })}
              </tbody>
            </table>
          </div>
        )}
      </div>
    </div>
  );
};
