import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { useGetPermitsQuery, useSubmitPermitForAiReviewMutation } from '../store/apiSlice';
import { StatusBadge } from '../components/StatusBadge';
import { Search, Filter, Cpu, ArrowUpRight, AlertCircle, RefreshCw, FileText } from 'lucide-react';
import type { PermitStatus } from '../types';

export const PermitsListPage: React.FC = () => {
  const [searchTerm, setSearchTerm] = useState('');
  const [statusFilter, setStatusFilter] = useState<string>('ALL');
  const navigate = useNavigate();

  const { data: permits = [], isLoading, error, refetch } = useGetPermitsQuery();
  const [submitForAiReview, { isLoading: isSubmitting }] = useSubmitPermitForAiReviewMutation();

  const handleTriggerAiReview = async (e: React.MouseEvent, permitId: string) => {
    e.stopPropagation();
    try {
      await submitForAiReview(permitId).unwrap();
      refetch();
    } catch (err: any) {
      alert(`AI Review failed: ${err.data?.message || 'Server error'}`);
    }
  };

  const filteredPermits = permits.filter((p) => {
    const matchesSearch =
      p.permitNumber.toLowerCase().includes(searchTerm.toLowerCase()) ||
      p.objectiveDescription.toLowerCase().includes(searchTerm.toLowerCase()) ||
      p.zoneName.toLowerCase().includes(searchTerm.toLowerCase()) ||
      p.supervisorName.toLowerCase().includes(searchTerm.toLowerCase());

    const matchesStatus =
      statusFilter === 'ALL' ||
      (statusFilter === 'PENDING' && (p.status === 'PendingApproval' || p.status === 'AiReview')) ||
      p.status === statusFilter;

    return matchesSearch && matchesStatus;
  });

  return (
    <div className="space-y-6">
      {/* Header */}
      <div className="flex flex-wrap items-center justify-between gap-4">
        <div>
          <h1 className="text-2xl font-black text-slate-100 tracking-tight">
            Permits-to-Work & Clearance Roster
          </h1>
          <p className="text-xs text-slate-400 mt-1">
            Authoritative site register. Multi-agent evaluation and deterministic safety validation.
          </p>
        </div>

        <div className="flex items-center gap-3">
          <button
            onClick={() => refetch()}
            className="flex items-center gap-1.5 px-3 py-2 bg-slate-900 hover:bg-slate-800 text-slate-300 rounded-lg text-xs border border-slate-700 transition"
          >
            <RefreshCw className="w-3.5 h-3.5" />
            <span>Refresh</span>
          </button>
        </div>
      </div>

      {/* Filter and Search Bar */}
      <div className="bg-slate-900 p-4 rounded-xl border border-slate-800 flex flex-wrap gap-4 items-center justify-between">
        <div className="flex-1 min-w-[280px] relative">
          <Search className="w-4 h-4 text-slate-500 absolute left-3 top-1/2 -translate-y-1/2" />
          <input
            type="text"
            placeholder="Search by Permit #, Zone, Objective, or Supervisor..."
            value={searchTerm}
            onChange={(e) => setSearchTerm(e.target.value)}
            className="w-full pl-9 pr-4 py-2 bg-slate-950 border border-slate-700 rounded-lg text-xs text-white placeholder-slate-500 focus:outline-none focus:ring-1 focus:ring-amber-500"
          />
        </div>

        <div className="flex items-center gap-2 overflow-x-auto pb-1 sm:pb-0">
          <Filter className="w-4 h-4 text-slate-500 shrink-0 mr-1" />
          {[
            { id: 'ALL', label: 'All Permits' },
            { id: 'PENDING', label: 'Pending HSE Review' },
            { id: 'Approved', label: 'Approved' },
            { id: 'Active', label: 'Active on Site' },
            { id: 'Refused', label: 'Refused / Safe Failure' },
          ].map((tab) => (
            <button
              key={tab.id}
              onClick={() => setStatusFilter(tab.id)}
              className={`px-3 py-1.5 rounded-lg text-xs font-medium whitespace-nowrap transition ${
                statusFilter === tab.id
                  ? 'bg-amber-500 text-slate-950 font-bold'
                  : 'bg-slate-800 text-slate-400 hover:text-slate-200'
              }`}
            >
              {tab.label}
            </button>
          ))}
        </div>
      </div>

      {/* Permits Table */}
      <div className="bg-slate-900 rounded-xl border border-slate-800 overflow-hidden shadow-sm">
        {isLoading ? (
          <div className="p-12 text-center text-slate-400 flex flex-col items-center gap-3">
            <RefreshCw className="w-6 h-6 animate-spin text-amber-400" />
            <span className="text-sm">Loading permit register...</span>
          </div>
        ) : error ? (
          <div className="p-8 text-center text-rose-400 flex flex-col items-center gap-2">
            <AlertCircle className="w-6 h-6" />
            <span className="text-sm font-semibold">Failed to connect to ClearToWork Backend API.</span>
            <span className="text-xs text-slate-500">Ensure the ASP.NET Core service is active on port 5000.</span>
          </div>
        ) : filteredPermits.length === 0 ? (
          <div className="p-12 text-center text-slate-400 flex flex-col items-center gap-2">
            <FileText className="w-8 h-8 text-slate-600 mb-1" />
            <span className="text-sm font-semibold text-slate-300">No permits found matching current filters.</span>
            <span className="text-xs text-slate-500">Adjust your search keyword or selected status tab.</span>
          </div>
        ) : (
          <div className="overflow-x-auto">
            <table className="w-full text-left text-xs border-collapse">
              <thead>
                <tr className="bg-slate-950/80 border-b border-slate-800 text-slate-400 uppercase tracking-wider text-[11px] font-semibold">
                  <th className="py-3.5 px-4">Permit #</th>
                  <th className="py-3.5 px-4">Hazard / Type</th>
                  <th className="py-3.5 px-4">Location Zone</th>
                  <th className="py-3.5 px-4">Scheduled Window</th>
                  <th className="py-3.5 px-4">Supervisor</th>
                  <th className="py-3.5 px-4">Clearance Status</th>
                  <th className="py-3.5 px-4 text-right">Actions</th>
                </tr>
              </thead>
              <tbody className="divide-y divide-slate-800/60">
                {filteredPermits.map((permit) => (
                  <tr
                    key={permit.id}
                    onClick={() => navigate(`/permits/${permit.id}`)}
                    className="hover:bg-slate-800/50 cursor-pointer transition-colors"
                  >
                    <td className="py-3.5 px-4 font-mono font-bold text-amber-400">
                      {permit.permitNumber}
                    </td>
                    <td className="py-3.5 px-4">
                      <div className="font-semibold text-slate-200">{permit.permitTypeName}</div>
                      <div className="text-[11px] text-slate-400 truncate max-w-xs">{permit.objectiveDescription}</div>
                    </td>
                    <td className="py-3.5 px-4">
                      <span className="font-mono bg-slate-800 px-2 py-0.5 rounded border border-slate-700 text-slate-300">
                        {permit.zoneCode}
                      </span>
                      <div className="text-[11px] text-slate-400 mt-0.5">{permit.zoneName}</div>
                    </td>
                    <td className="py-3.5 px-4 font-mono text-slate-300">
                      <div>{new Date(permit.scheduledStartTime).toLocaleDateString()}</div>
                      <div className="text-[11px] text-slate-400">
                        {new Date(permit.scheduledStartTime).toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' })}
                        {' – '}
                        {new Date(permit.scheduledEndTime).toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' })}
                      </div>
                    </td>
                    <td className="py-3.5 px-4 text-slate-300">
                      {permit.supervisorName}
                    </td>
                    <td className="py-3.5 px-4">
                      <StatusBadge status={permit.status as PermitStatus} />
                    </td>
                    <td className="py-3.5 px-4 text-right space-x-2">
                      {permit.status === 'Draft' && (
                        <button
                          onClick={(e) => handleTriggerAiReview(e, permit.id)}
                          disabled={isSubmitting}
                          className="inline-flex items-center gap-1 px-2.5 py-1 bg-purple-900/60 hover:bg-purple-800 text-purple-300 border border-purple-700 rounded text-[11px] font-semibold transition"
                        >
                          <Cpu className="w-3 h-3" />
                          <span>Trigger AI Review</span>
                        </button>
                      )}
                      <span className="inline-flex items-center gap-1 text-slate-400 hover:text-amber-400 font-semibold text-xs">
                        <span>Details</span>
                        <ArrowUpRight className="w-3.5 h-3.5" />
                      </span>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        )}
      </div>
    </div>
  );
};
