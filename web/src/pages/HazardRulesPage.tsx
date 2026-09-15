import React from 'react';
import { useGetZonesQuery } from '../store/apiSlice';
import { useTranslation } from '../context/I18nContext';
import { AlertTriangle, Wind, ShieldAlert, Layers } from 'lucide-react';

export const HazardRulesPage: React.FC = () => {
  const { t } = useTranslation();
  const { data: zones = [], isLoading } = useGetZonesQuery();

  return (
    <div className="space-y-6 max-w-6xl mx-auto">
      {/* Header */}
      <div>
        <h1 className="text-2xl font-black text-slate-900 dark:text-slate-100 tracking-tight flex items-center gap-2">
          <AlertTriangle className="w-6 h-6 text-amber-500 dark:text-amber-400" />
          <span>{t('hazard_title')}</span>
        </h1>
        <p className="text-xs text-slate-500 dark:text-slate-400 mt-1">
          {t('hazard_subtitle')}
        </p>
      </div>

      {/* Rule HR-07 Incompatibility Card */}
      <div className="bg-white dark:bg-slate-900 border border-slate-200 dark:border-slate-800 rounded-2xl p-6 shadow-sm space-y-4">
        <div className="flex items-center gap-2 text-rose-600 dark:text-rose-400 font-bold text-base pb-3 border-b border-slate-200 dark:border-slate-800">
          <ShieldAlert className="w-5 h-5" />
          <span>{t('hazard_simops_title')}</span>
        </div>

        <div className="bg-rose-50 dark:bg-rose-950/20 border border-rose-200 dark:border-rose-800/60 p-4 rounded-xl text-xs leading-relaxed text-rose-800 dark:text-rose-200">
          <strong>Mandatory Clearance Constraint:</strong> {t('hazard_simops_desc')}
        </div>

        <div className="grid grid-cols-1 sm:grid-cols-2 gap-4 text-xs font-mono">
          <div className="bg-slate-50 dark:bg-slate-950 p-3.5 rounded-xl border border-slate-200 dark:border-slate-800">
            <span className="text-slate-500 dark:text-slate-400 uppercase text-[10px] block font-sans">{t('hazard_primary')}</span>
            <span className="text-amber-600 dark:text-amber-400 font-bold text-sm">HOT_WORK (Welding & Cutting)</span>
            <div className="text-slate-500 dark:text-slate-400 text-[11px] font-sans mt-1">Sparks, slag, ignition source</div>
          </div>

          <div className="bg-slate-50 dark:bg-slate-950 p-3.5 rounded-xl border border-slate-200 dark:border-slate-800">
            <span className="text-slate-500 dark:text-slate-400 uppercase text-[10px] block font-sans">{t('hazard_conflicting')}</span>
            <span className="text-rose-600 dark:text-rose-400 font-bold text-sm">SOLVENT_PAINTING (Volatile Vapour)</span>
            <div className="text-slate-500 dark:text-slate-400 text-[11px] font-sans mt-1">Lower Explosive Limit (LEL) atmospheric risk</div>
          </div>
        </div>
      </div>

      {/* Weather Safety Envelopes Card */}
      <div className="bg-white dark:bg-slate-900 border border-slate-200 dark:border-slate-800 rounded-2xl p-6 shadow-sm space-y-4">
        <div className="flex items-center gap-2 text-amber-600 dark:text-amber-400 font-bold text-base pb-3 border-b border-slate-200 dark:border-slate-800">
          <Wind className="w-5 h-5" />
          <span>{t('hazard_weather_title')}</span>
        </div>

        <div className="grid grid-cols-1 sm:grid-cols-2 gap-4 text-xs">
          <div className="bg-slate-50 dark:bg-slate-950 p-4 rounded-xl border border-slate-200 dark:border-slate-800">
            <div className="flex items-center justify-between font-bold text-slate-800 dark:text-slate-200 text-sm mb-1">
              <span>Hot Work (Elevated Platform)</span>
              <span className="font-mono text-amber-600 dark:text-amber-400 font-bold">35.0 km/h</span>
            </div>
            <p className="text-slate-500 dark:text-slate-400 text-[11px] leading-relaxed">
              Maximum permissible wind gust speed. Gusts exceeding 35 km/h disperse shielding gas and carry hot slag into adjacent areas. Prohibited during active rain.
            </p>
          </div>

          <div className="bg-slate-50 dark:bg-slate-950 p-4 rounded-xl border border-slate-200 dark:border-slate-800">
            <div className="flex items-center justify-between font-bold text-slate-800 dark:text-slate-200 text-sm mb-1">
              <span>Working at Height & Crane Lifting</span>
              <span className="font-mono text-amber-600 dark:text-amber-400 font-bold">30.0 km/h</span>
            </div>
            <p className="text-slate-500 dark:text-slate-400 text-[11px] leading-relaxed">
              Strict suspension of elevated work when wind speeds exceed 30 km/h or lightning forecast within 15 km.
            </p>
          </div>
        </div>
      </div>

      {/* Plant Spatial Zones & Adjacency */}
      <div className="bg-white dark:bg-slate-900 border border-slate-200 dark:border-slate-800 rounded-2xl p-6 shadow-sm space-y-4">
        <div className="flex items-center gap-2 text-slate-900 dark:text-slate-200 font-bold text-base pb-3 border-b border-slate-200 dark:border-slate-800">
          <Layers className="w-5 h-5 text-amber-500 dark:text-amber-400" />
          <span>{t('hazard_zones_title')}</span>
        </div>

        {isLoading ? (
          <div className="text-xs text-slate-500 dark:text-slate-400 py-4 text-center">Loading site zones...</div>
        ) : (
          <div className="grid grid-cols-1 md:grid-cols-2 gap-4 text-xs">
            {zones.map((zone) => (
              <div key={zone.id} className="bg-slate-50 dark:bg-slate-950 p-4 rounded-xl border border-slate-200 dark:border-slate-800 space-y-2">
                <div className="flex items-center justify-between">
                  <span className="font-bold text-slate-900 dark:text-slate-100 text-sm">{zone.name}</span>
                  <span className="font-mono font-bold text-amber-600 dark:text-amber-400 bg-slate-100 dark:bg-slate-900 px-2 py-0.5 rounded border border-slate-200 dark:border-slate-800">
                    {zone.code}
                  </span>
                </div>
                <div className="text-slate-500 dark:text-slate-400 text-[11px]">
                  GPS: {zone.latitude.toFixed(4)}, {zone.longitude.toFixed(4)} · Radius: {zone.radiusMeters}m
                </div>
                <div className="text-[11px] text-slate-500 dark:text-slate-400 font-mono">
                  QR Tag: <span className="text-slate-700 dark:text-slate-300 font-semibold">{zone.qrCodePayload}</span>
                </div>
                <div className="pt-2 border-t border-slate-200 dark:border-slate-800/80">
                  <span className="text-[10px] uppercase font-bold text-slate-500 block mb-1">
                    {t('hazard_adjacent_transfer')}
                  </span>
                  <div className="flex flex-wrap gap-1.5">
                    {zone.adjacentZoneCodes.length === 0 ? (
                      <span className="text-slate-400 dark:text-slate-500 italic">None</span>
                    ) : (
                      zone.adjacentZoneCodes.map((adj, aIdx) => (
                        <span
                          key={aIdx}
                          className="bg-rose-50 dark:bg-slate-900 text-rose-700 dark:text-rose-300 border border-rose-200 dark:border-rose-900/40 px-2 py-0.5 rounded font-mono text-[10px] font-bold"
                        >
                          {adj} (SIMOPS Linked)
                        </span>
                      ))
                    )}
                  </div>
                </div>
              </div>
            ))}
          </div>
        )}
      </div>
    </div>
  );
};
