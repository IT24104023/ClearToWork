import React, { useState } from 'react';
import { useGetWorkersQuery, useGetExpiryForecastQuery } from '../store/apiSlice';
import { Users2, CheckCircle2, XCircle, Search, CalendarClock } from 'lucide-react';

export const WorkforcePage: React.FC = () => {
  const [searchTerm, setSearchTerm] = useState('');
  const { data: workers = [] } = useGetWorkersQuery();
  const { data: forecast = [], isLoading: isLoadingForecast } = useGetExpiryForecastQuery();

  const filteredWorkers = workers.filter(
    (w) =>
      `${w.firstName} ${w.lastName}`.toLowerCase().includes(searchTerm.toLowerCase()) ||
      w.badgeNumber.toLowerCase().includes(searchTerm.toLowerCase()) ||
      w.trade.toLowerCase().includes(searchTerm.toLowerCase()) ||
      w.contractorName.toLowerCase().includes(searchTerm.toLowerCase())
  );

  return (
    <div className="space-y-6 max-w-6xl mx-auto">
      {/* Header */}
      <div>
        <h1 className="text-2xl font-black text-slate-100 tracking-tight flex items-center gap-2">
          <Users2 className="w-6 h-6 text-amber-400" />
          <span>Workforce Competency & Certification Register</span>
        </h1>
        <p className="text-xs text-slate-400 mt-1">
          Student 1 Component · Active trade qualifications, compliance tracking, and 30-day proactive expiry forecasting.
        </p>
      </div>

      {/* 30-Day Proactive Expiry Forecast Alert Card */}
      <div className="bg-amber-950/20 border border-amber-500/40 rounded-xl p-5 shadow-sm">
        <div className="flex items-center gap-2 text-amber-400 font-bold text-sm mb-2">
          <CalendarClock className="w-5 h-5" />
          <span>30-Day Proactive Certification Expiry Forecast (§5 Non-CRUD Operation)</span>
        </div>
        <p className="text-xs text-slate-300 mb-4">
          Automated foresight identifies certificates expiring or lapsed within 30 days to prevent job site clearance stoppages.
        </p>

        {isLoadingForecast ? (
          <div className="text-xs text-slate-400 py-4 text-center">Checking certification expiries...</div>
        ) : forecast.length === 0 ? (
          <div className="text-xs text-emerald-400 py-2 font-medium">All active personnel certificates are valid beyond 30 days.</div>
        ) : (
          <div className="overflow-x-auto bg-slate-950/60 rounded-lg border border-slate-800">
            <table className="w-full text-left text-xs">
              <thead>
                <tr className="border-b border-slate-800 text-slate-400 uppercase text-[10px] font-semibold">
                  <th className="py-2.5 px-3">Worker Name</th>
                  <th className="py-2.5 px-3">Badge #</th>
                  <th className="py-2.5 px-3">Certificate</th>
                  <th className="py-2.5 px-3">Expiry Date</th>
                  <th className="py-2.5 px-3">Days Remaining</th>
                  <th className="py-2.5 px-3">Status</th>
                </tr>
              </thead>
              <tbody className="divide-y divide-slate-800/60 font-mono">
                {forecast.map((item, idx) => (
                  <tr key={idx} className="hover:bg-slate-900/40">
                    <td className="py-2.5 px-3 font-sans font-semibold text-slate-200">{item.workerName}</td>
                    <td className="py-2.5 px-3 text-amber-400">{item.badgeNumber}</td>
                    <td className="py-2.5 px-3 text-slate-300 font-sans">{item.certificateName}</td>
                    <td className="py-2.5 px-3 text-slate-400">{new Date(item.expiryDate).toLocaleDateString()}</td>
                    <td className="py-2.5 px-3 font-bold">
                      <span className={item.daysRemaining < 0 ? 'text-rose-400' : 'text-amber-400'}>
                        {item.daysRemaining < 0 ? `${-item.daysRemaining} days overdue` : `${item.daysRemaining} days`}
                      </span>
                    </td>
                    <td className="py-2.5 px-3">
                      <span className={`px-2 py-0.5 rounded text-[10px] uppercase font-bold ${
                        item.daysRemaining < 0 ? 'bg-rose-950 text-rose-400 border border-rose-800' : 'bg-amber-950 text-amber-400 border border-amber-800'
                      }`}>
                        {item.daysRemaining < 0 ? 'EXPIRED' : 'EXPIRING'}
                      </span>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        )}
      </div>

      {/* Search Bar */}
      <div className="bg-slate-900 p-4 rounded-xl border border-slate-800 flex items-center justify-between">
        <div className="w-full max-w-md relative">
          <Search className="w-4 h-4 text-slate-500 absolute left-3 top-1/2 -translate-y-1/2" />
          <input
            type="text"
            placeholder="Search workers by name, badge, trade, or contractor..."
            value={searchTerm}
            onChange={(e) => setSearchTerm(e.target.value)}
            className="w-full pl-9 pr-4 py-2 bg-slate-950 border border-slate-700 rounded-lg text-xs text-white placeholder-slate-500 focus:outline-none focus:ring-1 focus:ring-amber-500"
          />
        </div>
      </div>

      {/* Workers Grid */}
      <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
        {filteredWorkers.map((w) => (
            <div key={w.id} className="bg-slate-900 border border-slate-800 rounded-xl p-5 shadow-sm space-y-3">
              <div className="flex items-start justify-between">
                <div>
                  <h3 className="font-bold text-slate-100 text-sm">
                    {w.firstName} {w.lastName}
                  </h3>
                  <p className="text-xs text-slate-400 mt-0.5">
                    {w.trade} · {w.contractorName}
                  </p>
                </div>
                <span className="font-mono text-xs bg-slate-950 text-amber-400 px-2 py-1 rounded border border-slate-800 font-bold">
                  {w.badgeNumber}
                </span>
              </div>

              <div className="space-y-1.5 pt-2 border-t border-slate-800/80">
                <div className="text-[11px] font-semibold text-slate-400 uppercase tracking-wider">
                  Accredited Certifications
                </div>
                {w.certificates.length === 0 ? (
                  <div className="text-xs text-slate-500 italic">No certifications registered.</div>
                ) : (
                  w.certificates.map((cert) => (
                    <div
                      key={cert.id}
                      className="flex items-center justify-between text-xs bg-slate-950 p-2 rounded border border-slate-800/80"
                    >
                      <div className="truncate pr-2">
                        <div className="font-medium text-slate-300 truncate">{cert.certificateName}</div>
                        <div className="text-[10px] text-slate-500 font-mono">
                          {cert.certificateNumber} · Exp: {new Date(cert.expiryDate).toLocaleDateString()}
                        </div>
                      </div>
                      <div className="shrink-0">
                        {cert.status === 'Valid' && cert.daysUntilExpiry > 0 ? (
                          <span className="flex items-center gap-1 text-emerald-400 text-[11px] font-mono font-bold">
                            <CheckCircle2 className="w-3.5 h-3.5" /> Valid
                          </span>
                        ) : (
                          <span className="flex items-center gap-1 text-rose-400 text-[11px] font-mono font-bold">
                            <XCircle className="w-3.5 h-3.5" /> Expired
                          </span>
                        )}
                      </div>
                    </div>
                  ))
                )}
              </div>
            </div>
        ))}
      </div>
    </div>
  );
};
