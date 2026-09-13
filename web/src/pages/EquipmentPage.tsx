import React, { useState } from 'react';
import { useGetEquipmentQuery } from '../store/apiSlice';
import { Wrench, CheckCircle2, XCircle, Search, Lock } from 'lucide-react';

export const EquipmentPage: React.FC = () => {
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
        <h1 className="text-2xl font-black text-slate-100 tracking-tight flex items-center gap-2">
          <Wrench className="w-6 h-6 text-amber-400" />
          <span>Equipment, Isolation & Asset Readiness Register</span>
        </h1>
        <p className="text-xs text-slate-400 mt-1">
          Student 2 Component · Gas monitor calibration, fire extinguisher inspection records, and zone isolation points.
        </p>
      </div>

      {/* Lockout / Tagout Notice */}
      <div className="bg-slate-900 border border-slate-800 rounded-xl p-5 shadow-sm">
        <div className="flex items-center gap-2 text-amber-400 font-bold text-sm mb-1">
          <Lock className="w-4 h-4" />
          <span>Zone B3 Mezzanine — Active Lock-Out / Tag-Out (LOTO) Points</span>
        </div>
        <p className="text-xs text-slate-300 mb-3">
          Prior to permit clearance, physical isolation manifolds are verified in safe state.
        </p>
        <div className="grid grid-cols-1 sm:grid-cols-2 gap-3 text-xs">
          <div className="bg-slate-950 p-3 rounded-lg border border-slate-800 flex items-center justify-between">
            <div>
              <div className="font-mono text-amber-400 font-bold">ISO-B3-VALVE-01</div>
              <div className="text-slate-400 text-[11px] mt-0.5">Solvent supply isolation manifold</div>
            </div>
            <span className="bg-emerald-950 text-emerald-400 border border-emerald-800 px-2 py-0.5 rounded font-mono text-[10px] font-bold">
              LOCKED
            </span>
          </div>
          <div className="bg-slate-950 p-3 rounded-lg border border-slate-800 flex items-center justify-between">
            <div>
              <div className="font-mono text-amber-400 font-bold">ISO-B3-ELEC-04</div>
              <div className="text-slate-400 text-[11px] mt-0.5">415V Main busbar isolator switch</div>
            </div>
            <span className="bg-emerald-950 text-emerald-400 border border-emerald-800 px-2 py-0.5 rounded font-mono text-[10px] font-bold">
              TAGGED
            </span>
          </div>
        </div>
      </div>

      {/* Search Bar */}
      <div className="bg-slate-900 p-4 rounded-xl border border-slate-800 flex items-center justify-between">
        <div className="w-full max-w-md relative">
          <Search className="w-4 h-4 text-slate-500 absolute left-3 top-1/2 -translate-y-1/2" />
          <input
            type="text"
            placeholder="Search equipment by tag, name, or category..."
            value={searchTerm}
            onChange={(e) => setSearchTerm(e.target.value)}
            className="w-full pl-9 pr-4 py-2 bg-slate-950 border border-slate-700 rounded-lg text-xs text-white placeholder-slate-500 focus:outline-none focus:ring-1 focus:ring-amber-500"
          />
        </div>
      </div>

      {/* Equipment Table */}
      <div className="bg-slate-900 rounded-xl border border-slate-800 overflow-hidden shadow-sm">
        {isLoading ? (
          <div className="p-8 text-center text-slate-400 text-xs">Loading equipment inventory...</div>
        ) : (
          <table className="w-full text-left text-xs border-collapse">
            <thead>
              <tr className="bg-slate-950/80 border-b border-slate-800 text-slate-400 uppercase text-[11px] font-semibold">
                <th className="py-3 px-4">Asset Tag</th>
                <th className="py-3 px-4">Equipment Name</th>
                <th className="py-3 px-4">Category</th>
                <th className="py-3 px-4">Inspection Status</th>
                <th className="py-3 px-4">Gas Calibration</th>
                <th className="py-3 px-4">Readiness Status</th>
              </tr>
            </thead>
            <tbody className="divide-y divide-slate-800/60">
              {filteredAssets.map((asset) => {
                const isReady = asset.isInspectionValid && asset.isCalibrationValid;

                return (
                  <tr key={asset.id} className="hover:bg-slate-800/50 transition-colors">
                    <td className="py-3 px-4 font-mono font-bold text-amber-400">{asset.assetTag}</td>
                    <td className="py-3 px-4 font-semibold text-slate-200">{asset.name}</td>
                    <td className="py-3 px-4 text-slate-400">{asset.category}</td>
                    <td className="py-3 px-4">
                      {asset.isInspectionValid ? (
                        <span className="text-emerald-400 flex items-center gap-1 font-mono font-semibold">
                          <CheckCircle2 className="w-3.5 h-3.5" /> In-Date
                        </span>
                      ) : (
                        <span className="text-rose-400 flex items-center gap-1 font-mono font-bold">
                          <XCircle className="w-3.5 h-3.5" /> OVERDUE
                        </span>
                      )}
                    </td>
                    <td className="py-3 px-4">
                      {asset.isCalibrationValid ? (
                        <span className="text-emerald-400 flex items-center gap-1 font-mono font-semibold">
                          <CheckCircle2 className="w-3.5 h-3.5" /> Certified
                        </span>
                      ) : (
                        <span className="text-slate-500 font-mono">N/A</span>
                      )}
                    </td>
                    <td className="py-3 px-4">
                      <span
                        className={`px-2.5 py-0.5 rounded text-[10px] uppercase font-bold border ${
                          isReady
                            ? 'bg-emerald-950 text-emerald-400 border-emerald-800'
                            : 'bg-rose-950 text-rose-400 border-rose-800'
                        }`}
                      >
                        {isReady ? 'CLEARED' : 'RESTRICTED'}
                      </span>
                    </td>
                  </tr>
                );
              })}
            </tbody>
          </table>
        )}
      </div>
    </div>
  );
};
