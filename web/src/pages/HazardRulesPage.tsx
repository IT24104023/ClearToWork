import React from 'react';
import { useGetZonesQuery } from '../store/apiSlice';
import { AlertTriangle, Wind, ShieldAlert, Layers } from 'lucide-react';

export const HazardRulesPage: React.FC = () => {
  const { data: zones = [], isLoading } = useGetZonesQuery();

  return (
    <div className="space-y-6 max-w-6xl mx-auto">
      {/* Header */}
      <div>
        <h1 className="text-2xl font-black text-slate-100 tracking-tight flex items-center gap-2">
          <AlertTriangle className="w-6 h-6 text-amber-400" />
          <span>Hazard Rulebook & SIMOPS Incompatibility Matrix</span>
        </h1>
        <p className="text-xs text-slate-400 mt-1">
          Student 4 Component · Atmospheric collision rules, simultaneous operations (SIMOPS), and weather envelope constraints.
        </p>
      </div>

      {/* Rule HR-07 Incompatibility Card */}
      <div className="bg-slate-900 border border-slate-800 rounded-xl p-6 shadow-sm space-y-4">
        <div className="flex items-center gap-2 text-rose-400 font-bold text-base pb-3 border-b border-slate-800">
          <ShieldAlert className="w-5 h-5" />
          <span>SIMOPS Incompatibility Rule HR-07 (High Explosive Risk)</span>
        </div>

        <div className="bg-rose-950/20 border border-rose-800/60 p-4 rounded-lg text-xs leading-relaxed text-rose-200">
          <strong>Mandatory Clearance Constraint:</strong> Hot work (welding, grinding, open sparks) is strictly forbidden in any zone immediately adjacent to volatile solvent spray operations or solvent storage.
        </div>

        <div className="grid grid-cols-1 sm:grid-cols-2 gap-4 text-xs font-mono">
          <div className="bg-slate-950 p-3 rounded-lg border border-slate-800">
            <span className="text-slate-500 uppercase text-[10px] block font-sans">Primary Hazard</span>
            <span className="text-amber-400 font-bold text-sm">HOT_WORK (Welding & Cutting)</span>
            <div className="text-slate-400 text-[11px] font-sans mt-1">Sparks, slag, ignition source</div>
          </div>

          <div className="bg-slate-950 p-3 rounded-lg border border-slate-800">
            <span className="text-slate-500 uppercase text-[10px] block font-sans">Conflicting Hazard</span>
            <span className="text-rose-400 font-bold text-sm">SOLVENT_PAINTING (Volatile Vapour)</span>
            <div className="text-slate-400 text-[11px] font-sans mt-1">Lower Explosive Limit (LEL) atmospheric risk</div>
          </div>
        </div>
      </div>

      {/* Weather Safety Envelopes Card */}
      <div className="bg-slate-900 border border-slate-800 rounded-xl p-6 shadow-sm space-y-4">
        <div className="flex items-center gap-2 text-amber-400 font-bold text-base pb-3 border-b border-slate-800">
          <Wind className="w-5 h-5" />
          <span>Environmental Operational Limits (Open-Meteo Integration)</span>
        </div>

        <div className="grid grid-cols-1 sm:grid-cols-2 gap-4 text-xs">
          <div className="bg-slate-950 p-4 rounded-lg border border-slate-800">
            <div className="flex items-center justify-between font-bold text-slate-200 text-sm mb-1">
              <span>Hot Work (Elevated Platform)</span>
              <span className="font-mono text-amber-400">35.0 km/h</span>
            </div>
            <p className="text-slate-400 text-[11px] leading-relaxed">
              Maximum permissible wind gust speed. Gusts exceeding 35 km/h disperse shielding gas and carry hot slag into adjacent areas. Prohibited during active rain.
            </p>
          </div>

          <div className="bg-slate-950 p-4 rounded-lg border border-slate-800">
            <div className="flex items-center justify-between font-bold text-slate-200 text-sm mb-1">
              <span>Working at Height & Crane Lifting</span>
              <span className="font-mono text-amber-400">30.0 km/h</span>
            </div>
            <p className="text-slate-400 text-[11px] leading-relaxed">
              Strict suspension of elevated work when wind speeds exceed 30 km/h or lightning forecast within 15 km.
            </p>
          </div>
        </div>
      </div>

      {/* Plant Spatial Zones & Adjacency */}
      <div className="bg-slate-900 border border-slate-800 rounded-xl p-6 shadow-sm space-y-4">
        <div className="flex items-center gap-2 text-slate-200 font-bold text-base pb-3 border-b border-slate-800">
          <Layers className="w-5 h-5 text-amber-400" />
          <span>Industrial Site Zones & Spatial Adjacency Graph</span>
        </div>

        {isLoading ? (
          <div className="text-xs text-slate-400 py-4 text-center">Loading site zones...</div>
        ) : (
          <div className="grid grid-cols-1 md:grid-cols-2 gap-4 text-xs">
            {zones.map((zone) => (
              <div key={zone.id} className="bg-slate-950 p-4 rounded-lg border border-slate-800 space-y-2">
                <div className="flex items-center justify-between">
                  <span className="font-bold text-slate-100 text-sm">{zone.name}</span>
                  <span className="font-mono font-bold text-amber-400 bg-slate-900 px-2 py-0.5 rounded border border-slate-800">
                    {zone.code}
                  </span>
                </div>
                <div className="text-slate-400 text-[11px]">
                  GPS: {zone.latitude.toFixed(4)}, {zone.longitude.toFixed(4)} · Radius: {zone.radiusMeters}m
                </div>
                <div className="text-[11px] text-slate-400 font-mono">
                  QR Tag: <span className="text-slate-300">{zone.qrCodePayload}</span>
                </div>
                <div className="pt-2 border-t border-slate-800/80">
                  <span className="text-[10px] uppercase font-bold text-slate-500 block mb-1">
                    Adjacent Risk Transfer Zones:
                  </span>
                  <div className="flex flex-wrap gap-1.5">
                    {zone.adjacentZoneCodes.length === 0 ? (
                      <span className="text-slate-500 italic">None</span>
                    ) : (
                      zone.adjacentZoneCodes.map((adj, aIdx) => (
                        <span
                          key={aIdx}
                          className="bg-slate-900 text-rose-300 border border-rose-900/40 px-2 py-0.5 rounded font-mono text-[10px] font-bold"
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
