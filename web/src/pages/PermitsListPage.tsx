import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { useSelector } from 'react-redux';
import type { RootState } from '../store';
import {
  useGetPermitsQuery,
  useSubmitPermitForAiReviewMutation,
  useCreatePermitDraftMutation,
  useDeletePermitDraftMutation,
  useGetZonesQuery,
  useGetWorkersQuery,
  useGetEquipmentQuery,
} from '../store/apiSlice';
import { StatusBadge } from '../components/StatusBadge';
import { EditPermitModal } from '../components/EditPermitModal';
import { useTranslation } from '../context/I18nContext';
import {
  Search, Filter, Cpu, ArrowUpRight, AlertCircle, RefreshCw,
  FileText, Plus, X, Pencil, Trash2,
} from 'lucide-react';
import type { PermitStatus, PermitDetails } from '../types';


// ─── New Permit Modal ────────────────────────────────────────────────────────

interface NewPermitModalProps {
  onClose: () => void;
  onCreated: () => void;
}

const PERMIT_TYPES = [
  { id: 'hot_work', code: 'HOT_WORK', name: 'Hot Work Operational Permit' },
  { id: 'solvent', code: 'SOLVENT_PAINTING', name: 'Solvent Application Permit' },
];

const NewPermitModal: React.FC<NewPermitModalProps> = ({ onClose, onCreated }) => {
  const [createPermit, { isLoading }] = useCreatePermitDraftMutation();
  const { data: zones = [] } = useGetZonesQuery();
  const { data: workers = [] } = useGetWorkersQuery();
  const { data: equipment = [] } = useGetEquipmentQuery();

  const [permitTypeCode, setPermitTypeCode] = useState('HOT_WORK');
  const [zoneId, setZoneId] = useState('');
  const [objective, setObjective] = useState('');
  const [startDate, setStartDate] = useState('');
  const [startTime, setStartTime] = useState('09:00');
  const [endTime, setEndTime] = useState('12:00');
  const [selectedWorkerIds, setSelectedWorkerIds] = useState<string[]>([]);
  const [selectedAssetIds, setSelectedAssetIds] = useState<string[]>([]);
  const [error, setError] = useState<string | null>(null);

  // derive permitTypeId from zones lookup on backend — pass code in objective tag
  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError(null);

    if (!zoneId || !objective.trim() || !startDate) {
      setError('Please fill in all required fields.');
      return;
    }

    const startDt = new Date(`${startDate}T${startTime}:00`);
    const endDt = new Date(`${startDate}T${endTime}:00`);
    if (endDt <= startDt) {
      setError('End time must be after start time.');
      return;
    }

    try {
      // The backend CreatePermit endpoint takes PermitTypeId (Guid).
      // We don't have the Guid here — pass a temporary workaround using the
      // permitType code embedded in objective and the first matching type from zones.
      // For the full demo flow, we'd call GET /api/permits/types first.
      // Instead, use the Swagger endpoint to get permit type IDs then create.
      const typesResp = await fetch('/api/permits/types');
      const types: { id: string; code: string }[] = typesResp.ok
        ? await typesResp.json()
        : [];
      const matched = types.find((t) => t.code === permitTypeCode);
      if (!matched) {
        setError('Could not resolve permit type. Ensure the backend is running.');
        return;
      }

      await createPermit({
        permitTypeId: matched.id,
        zoneId,
        objectiveDescription: objective,
        scheduledStartTime: startDt.toISOString(),
        scheduledEndTime: endDt.toISOString(),
        workerIds: selectedWorkerIds,
        assetIds: selectedAssetIds,
        photoUrls: [],
      }).unwrap();

      onCreated();
      onClose();
    } catch (err: any) {
      setError(err?.data?.message || err?.message || 'Failed to create permit draft.');
    }
  };

  const toggleWorker = (id: string) =>
    setSelectedWorkerIds((prev) =>
      prev.includes(id) ? prev.filter((x) => x !== id) : [...prev, id]
    );

  const toggleAsset = (id: string) =>
    setSelectedAssetIds((prev) =>
      prev.includes(id) ? prev.filter((x) => x !== id) : [...prev, id]
    );

  return (
    <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/70 backdrop-blur-sm p-4">
      <div className="bg-white dark:bg-slate-900 border border-slate-200 dark:border-slate-700 rounded-3xl shadow-2xl w-full max-w-2xl max-h-[90vh] overflow-y-auto">
        {/* Modal Header */}
        <div className="flex items-center justify-between p-5 border-b border-slate-200 dark:border-slate-800">
          <div>
            <h2 className="text-base font-black text-slate-900 dark:text-white">New Permit-to-Work Draft</h2>
            <p className="text-xs text-slate-500 dark:text-slate-400 mt-0.5">
              Draft will be saved for AI Clearance review before HSE sign-off.
            </p>
          </div>
          <button onClick={onClose} className="text-slate-400 hover:text-slate-600 dark:hover:text-white transition">
            <X className="w-5 h-5" />
          </button>
        </div>

        <form onSubmit={handleSubmit} className="p-5 space-y-5">
          {error && (
            <div className="p-3 bg-rose-50 dark:bg-rose-950/50 border border-rose-300 dark:border-rose-700 rounded-xl text-rose-700 dark:text-rose-300 text-xs flex items-center gap-2">
              <AlertCircle className="w-4 h-4 shrink-0" />
              <span>{error}</span>
            </div>
          )}

          {/* Permit Type */}
          <div className="grid grid-cols-1 sm:grid-cols-2 gap-4">
            <div>
              <label className="text-xs font-semibold text-slate-700 dark:text-slate-300 block mb-1.5">
                Permit Type <span className="text-rose-500">*</span>
              </label>
              <select
                value={permitTypeCode}
                onChange={(e) => setPermitTypeCode(e.target.value)}
                className="w-full bg-slate-50 dark:bg-slate-950 border border-slate-300 dark:border-slate-700 rounded-xl px-3 py-2 text-xs text-slate-900 dark:text-white focus:outline-none focus:border-amber-500"
              >
                {PERMIT_TYPES.map((pt) => (
                  <option key={pt.code} value={pt.code}>
                    {pt.name}
                  </option>
                ))}
              </select>
            </div>

            <div>
              <label className="text-xs font-semibold text-slate-700 dark:text-slate-300 block mb-1.5">
                Plant Zone Location <span className="text-rose-500">*</span>
              </label>
              <select
                value={zoneId}
                onChange={(e) => setZoneId(e.target.value)}
                required
                className="w-full bg-slate-50 dark:bg-slate-950 border border-slate-300 dark:border-slate-700 rounded-xl px-3 py-2 text-xs text-slate-900 dark:text-white focus:outline-none focus:border-amber-500"
              >
                <option value="">Select a zone...</option>
                {zones.map((z) => (
                  <option key={z.id} value={z.id}>
                    {z.name} ({z.code})
                  </option>
                ))}
              </select>
            </div>
          </div>

          {/* Objective Description */}
          <div>
            <label className="text-xs font-semibold text-slate-700 dark:text-slate-300 block mb-1.5">
              Work Objective &amp; Hazard Scope <span className="text-rose-500">*</span>
            </label>
            <textarea
              rows={2}
              required
              value={objective}
              onChange={(e) => setObjective(e.target.value)}
              placeholder="e.g. Hot work cutting steel beam next to solvent line..."
              className="w-full bg-slate-50 dark:bg-slate-950 border border-slate-300 dark:border-slate-700 rounded-xl px-3 py-2 text-xs text-slate-900 dark:text-white placeholder-slate-400 dark:placeholder-slate-500 focus:outline-none focus:border-amber-500"
            />
          </div>

          {/* Schedule */}
          <div className="grid grid-cols-1 sm:grid-cols-3 gap-3">
            <div>
              <label className="text-xs font-semibold text-slate-700 dark:text-slate-300 block mb-1.5">
                Date <span className="text-rose-500">*</span>
              </label>
              <input
                type="date"
                required
                value={startDate}
                onChange={(e) => setStartDate(e.target.value)}
                className="w-full bg-slate-50 dark:bg-slate-950 border border-slate-300 dark:border-slate-700 rounded-xl px-3 py-2 text-xs text-slate-900 dark:text-white focus:outline-none focus:border-amber-500"
              />
            </div>
            <div>
              <label className="text-xs font-semibold text-slate-700 dark:text-slate-300 block mb-1.5">
                Start Time
              </label>
              <input
                type="time"
                value={startTime}
                onChange={(e) => setStartTime(e.target.value)}
                className="w-full bg-slate-50 dark:bg-slate-950 border border-slate-300 dark:border-slate-700 rounded-xl px-3 py-2 text-xs text-slate-900 dark:text-white focus:outline-none focus:border-amber-500"
              />
            </div>
            <div>
              <label className="text-xs font-semibold text-slate-700 dark:text-slate-300 block mb-1.5">
                End Time
              </label>
              <input
                type="time"
                value={endTime}
                onChange={(e) => setEndTime(e.target.value)}
                className="w-full bg-slate-50 dark:bg-slate-950 border border-slate-300 dark:border-slate-700 rounded-xl px-3 py-2 text-xs text-slate-900 dark:text-white focus:outline-none focus:border-amber-500"
              />
            </div>
          </div>

          {/* Workers */}
          <div>
            <label className="text-xs font-semibold text-slate-700 dark:text-slate-300 block mb-1.5">
              Assign Certified Workforce
            </label>
            <div className="bg-slate-50 dark:bg-slate-950 border border-slate-300 dark:border-slate-700 rounded-xl p-2 max-h-36 overflow-y-auto space-y-1">
              {workers.length === 0 ? (
                <div className="text-xs text-slate-400 dark:text-slate-500 text-center py-2">No workers loaded</div>
              ) : (
                workers.map((w) => (
                  <label
                    key={w.id}
                    className="flex items-center gap-2.5 px-2 py-1.5 hover:bg-slate-100 dark:hover:bg-slate-800 rounded-lg cursor-pointer"
                  >
                    <input
                      type="checkbox"
                      checked={selectedWorkerIds.includes(w.id)}
                      onChange={() => toggleWorker(w.id)}
                      className="accent-amber-500"
                    />
                    <span className="text-xs text-slate-900 dark:text-slate-200 font-semibold">{w.firstName} {w.lastName}</span>
                    <span className="text-[10px] text-slate-500 font-mono">({w.badgeNumber})</span>
                    <span className="text-[10px] text-slate-400 ml-auto">{w.trade}</span>
                  </label>
                ))
              )}
            </div>
          </div>

          {/* Equipment */}
          <div>
            <label className="text-xs font-semibold text-slate-700 dark:text-slate-300 block mb-1.5">
              Assign Safety Equipment
            </label>
            <div className="bg-slate-50 dark:bg-slate-950 border border-slate-300 dark:border-slate-700 rounded-xl p-2 max-h-36 overflow-y-auto space-y-1">
              {equipment.length === 0 ? (
                <div className="text-xs text-slate-400 dark:text-slate-500 text-center py-2">No equipment loaded</div>
              ) : (
                equipment.map((a) => (
                  <label
                    key={a.id}
                    className="flex items-center gap-2.5 px-2 py-1.5 hover:bg-slate-100 dark:hover:bg-slate-800 rounded-lg cursor-pointer"
                  >
                    <input
                      type="checkbox"
                      checked={selectedAssetIds.includes(a.id)}
                      onChange={() => toggleAsset(a.id)}
                      className="accent-amber-500"
                    />
                    <span className="text-xs text-slate-900 dark:text-slate-200 font-mono font-semibold">{a.assetTag}</span>
                    <span className="text-xs text-slate-700 dark:text-slate-300">{a.name}</span>
                    <span className="text-[10px] text-slate-400 ml-auto">{a.category}</span>
                  </label>
                ))
              )}
            </div>
          </div>

          {/* Actions */}
          <div className="flex justify-end gap-3 pt-2 border-t border-slate-200 dark:border-slate-800">
            <button
              type="button"
              onClick={onClose}
              className="px-4 py-2 bg-slate-100 dark:bg-slate-800 hover:bg-slate-200 dark:hover:bg-slate-700 text-slate-700 dark:text-slate-300 rounded-xl text-xs font-semibold transition"
            >
              Cancel
            </button>
            <button
              type="submit"
              disabled={isLoading}
              className="flex items-center gap-1.5 px-4 py-2 bg-amber-500 hover:bg-amber-400 disabled:opacity-50 text-slate-950 rounded-xl text-xs font-bold transition shadow-sm"
            >
              <Plus className="w-3.5 h-3.5" />
              {isLoading ? 'Creating…' : 'Create Draft Permit'}
            </button>
          </div>
        </form>
      </div>
    </div>
  );
};

// ─── Main Page ────────────────────────────────────────────────────────────────

export const PermitsListPage: React.FC = () => {
  const { t } = useTranslation();
  const [searchTerm, setSearchTerm] = useState('');
  const [statusFilter, setStatusFilter] = useState<string>('ALL');
  const [showNewPermit, setShowNewPermit] = useState(false);
  const [editingPermit, setEditingPermit] = useState<PermitDetails | null>(null);
  const navigate = useNavigate();

  const user = useSelector((state: RootState) => state.auth.user);
  // Per RBAC: Admins, AreaSupervisors, and ContractorSupervisors can create/edit permits
  // SafetyOfficers cannot create/edit permits
  const canCreatePermit = user?.role !== 'SafetyOfficer';

  const { data: permits = [], isLoading, error, refetch } = useGetPermitsQuery();
  const [submitForAiReview, { isLoading: isSubmitting }] = useSubmitPermitForAiReviewMutation();
  const [deletePermit] = useDeletePermitDraftMutation();

  const handleTriggerAiReview = async (e: React.MouseEvent, permitId: string) => {
    e.stopPropagation();
    try {
      await submitForAiReview(permitId).unwrap();
      refetch();
    } catch (err: any) {
      alert(`AI Review failed: ${err.data?.message || 'Server error'}`);
    }
  };

  const handleDeleteDraft = async (e: React.MouseEvent, permit: PermitDetails) => {
    e.stopPropagation();
    if (window.confirm(`Are you sure you want to delete permit draft ${permit.permitNumber}?`)) {
      try {
        await deletePermit(permit.id).unwrap();
        refetch();
      } catch (err: any) {
        alert(err?.data?.message || 'Failed to delete permit draft.');
      }
    }
  };

  const handleEditDraft = (e: React.MouseEvent, permit: PermitDetails) => {
    e.stopPropagation();
    setEditingPermit(permit);
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
      {showNewPermit && (
        <NewPermitModal
          onClose={() => setShowNewPermit(false)}
          onCreated={() => refetch()}
        />
      )}

      {editingPermit && (
        <EditPermitModal
          permit={editingPermit}
          onClose={() => setEditingPermit(null)}
          onUpdated={() => refetch()}
        />
      )}

      {/* Header */}
      <div className="flex flex-wrap items-center justify-between gap-4">
        <div>
          <h1 className="text-2xl font-black text-slate-900 dark:text-slate-100 tracking-tight">
            {t('permit_list_title')}
          </h1>
          <p className="text-xs text-slate-500 dark:text-slate-400 mt-1">
            {t('permit_list_subtitle')}
          </p>
        </div>

        <div className="flex items-center gap-3">
          <button
            onClick={() => refetch()}
            className="flex items-center gap-1.5 px-3.5 py-2 bg-white dark:bg-slate-900 hover:bg-slate-50 dark:hover:bg-slate-800 text-slate-700 dark:text-slate-300 rounded-xl text-xs border border-slate-200 dark:border-slate-800 transition shadow-sm"
          >
            <RefreshCw className="w-3.5 h-3.5" />
            <span>Refresh</span>
          </button>

          {canCreatePermit && (
            <button
              onClick={() => setShowNewPermit(true)}
              className="flex items-center gap-1.5 px-4 py-2 bg-amber-500 hover:bg-amber-400 text-slate-950 rounded-xl text-xs font-bold transition shadow-sm"
            >
              <Plus className="w-3.5 h-3.5" />
              <span>{t('permit_new_btn')}</span>
            </button>
          )}
        </div>
      </div>

      {/* Filter and Search Bar */}
      <div className="bg-white dark:bg-slate-900 p-4 rounded-2xl border border-slate-200 dark:border-slate-800 flex flex-wrap gap-4 items-center justify-between shadow-sm">
        <div className="flex-1 min-w-[280px] relative">
          <Search className="w-4 h-4 text-slate-400 dark:text-slate-500 absolute left-3 top-1/2 -translate-y-1/2" />
          <input
            type="text"
            placeholder="Search by Permit #, Zone, Objective, or Supervisor..."
            value={searchTerm}
            onChange={(e) => setSearchTerm(e.target.value)}
            className="w-full pl-9 pr-4 py-2.5 bg-slate-50 dark:bg-slate-950 border border-slate-300 dark:border-slate-700 rounded-xl text-xs text-slate-900 dark:text-white placeholder-slate-400 dark:placeholder-slate-500 focus:outline-none focus:ring-1 focus:ring-amber-500"
          />
        </div>

        <div className="flex items-center gap-2 overflow-x-auto pb-1 sm:pb-0">
          <Filter className="w-4 h-4 text-slate-400 dark:text-slate-500 shrink-0 mr-1" />
          {[
            { id: 'ALL', label: t('permit_filter_all') },
            { id: 'Draft', label: t('status_draft') },
            { id: 'PENDING', label: t('permit_filter_pending') },
            { id: 'Approved', label: t('permit_filter_approved') },
            { id: 'Active', label: t('status_active') },
            { id: 'Refused', label: t('permit_filter_refused') },
          ].map((tab) => (
            <button
              key={tab.id}
              onClick={() => setStatusFilter(tab.id)}
              className={`px-3 py-1.5 rounded-xl text-xs font-semibold whitespace-nowrap transition ${
                statusFilter === tab.id
                  ? 'bg-amber-500 text-slate-950 font-bold shadow-sm'
                  : 'bg-slate-100 dark:bg-slate-800 text-slate-600 dark:text-slate-400 hover:text-slate-900 dark:hover:text-slate-200'
              }`}
            >
              {tab.label}
            </button>
          ))}
        </div>
      </div>

      {/* Permits Table */}
      <div className="bg-white dark:bg-slate-900 rounded-2xl border border-slate-200 dark:border-slate-800 overflow-hidden shadow-sm">
        {isLoading ? (
          <div className="p-12 text-center text-slate-400 flex flex-col items-center gap-3">
            <RefreshCw className="w-6 h-6 animate-spin text-amber-500" />
            <span className="text-sm">Loading permit register...</span>
          </div>
        ) : error ? (
          <div className="p-8 text-center text-rose-500 flex flex-col items-center gap-2">
            <AlertCircle className="w-6 h-6" />
            <span className="text-sm font-semibold">Failed to connect to ClearToWork Backend API.</span>
            <span className="text-xs text-slate-400">Ensure the ASP.NET Core service is active on port 5000.</span>
          </div>
        ) : filteredPermits.length === 0 ? (
          <div className="p-12 text-center text-slate-500 dark:text-slate-400 flex flex-col items-center gap-2">
            <FileText className="w-8 h-8 text-slate-400 dark:text-slate-600 mb-1" />
            <span className="text-sm font-semibold text-slate-700 dark:text-slate-300">No permits found matching current filters.</span>
            <span className="text-xs text-slate-400 dark:text-slate-500">Adjust your search keyword or selected status tab.</span>
            {canCreatePermit && (
              <button
                onClick={() => setShowNewPermit(true)}
                className="mt-3 flex items-center gap-1.5 px-4 py-2 bg-amber-500 hover:bg-amber-400 text-slate-950 rounded-xl text-xs font-bold transition shadow-sm"
              >
                <Plus className="w-3.5 h-3.5" />
                Create First Permit
              </button>
            )}
          </div>
        ) : (
          <div className="overflow-x-auto">
            <table className="w-full text-left text-xs border-collapse">
              <thead>
                <tr className="bg-slate-50 dark:bg-slate-950/80 border-b border-slate-200 dark:border-slate-800 text-slate-500 dark:text-slate-400 uppercase tracking-wider text-[11px] font-semibold">
                  <th className="py-3.5 px-4">Permit #</th>
                  <th className="py-3.5 px-4">Hazard / Type</th>
                  <th className="py-3.5 px-4">Location Zone</th>
                  <th className="py-3.5 px-4">Scheduled Window</th>
                  <th className="py-3.5 px-4">Supervisor</th>
                  <th className="py-3.5 px-4">Clearance Status</th>
                  <th className="py-3.5 px-4 text-right">Actions</th>
                </tr>
              </thead>
              <tbody className="divide-y divide-slate-200 dark:divide-slate-800/60">
                {filteredPermits.map((permit) => (
                  <tr
                    key={permit.id}
                    onClick={() => navigate(`/permits/${permit.id}`)}
                    className="hover:bg-slate-50 dark:hover:bg-slate-800/50 cursor-pointer transition-colors"
                  >
                    <td className="py-3.5 px-4 font-mono font-bold text-amber-600 dark:text-amber-400">
                      {permit.permitNumber}
                    </td>
                    <td className="py-3.5 px-4">
                      <div className="font-semibold text-slate-800 dark:text-slate-200">{permit.permitTypeName}</div>
                      <div className="text-[11px] text-slate-500 dark:text-slate-400 truncate max-w-xs">{permit.objectiveDescription}</div>
                    </td>
                    <td className="py-3.5 px-4">
                      <span className="font-mono bg-slate-100 dark:bg-slate-800 px-2 py-0.5 rounded border border-slate-200 dark:border-slate-700 text-slate-700 dark:text-slate-300">
                        {permit.zoneCode}
                      </span>
                      <div className="text-[11px] text-slate-500 dark:text-slate-400 mt-0.5">{permit.zoneName}</div>
                    </td>
                    <td className="py-3.5 px-4 font-mono text-slate-700 dark:text-slate-300">
                      <div>{new Date(permit.scheduledStartTime).toLocaleDateString()}</div>
                      <div className="text-[11px] text-slate-500 dark:text-slate-400">
                        {new Date(permit.scheduledStartTime).toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' })}
                        {' – '}
                        {new Date(permit.scheduledEndTime).toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' })}
                      </div>
                    </td>
                    <td className="py-3.5 px-4 text-slate-700 dark:text-slate-300">
                      {permit.supervisorName}
                    </td>
                    <td className="py-3.5 px-4">
                      <StatusBadge status={permit.status as PermitStatus} />
                    </td>
                    <td className="py-3.5 px-4 text-right space-x-1.5 whitespace-nowrap">
                      {canCreatePermit && (permit.status === 'Draft' || permit.status === 'Refused') && (
                        <>
                          <button
                            onClick={(e) => handleEditDraft(e, permit)}
                            title="Edit Permit Draft"
                            className="inline-flex items-center p-1.5 text-slate-400 hover:text-amber-500 hover:bg-slate-100 dark:hover:bg-slate-800 rounded-lg transition"
                          >
                            <Pencil className="w-3.5 h-3.5" />
                          </button>
                          <button
                            onClick={(e) => handleDeleteDraft(e, permit)}
                            title="Delete Permit Draft"
                            className="inline-flex items-center p-1.5 text-slate-400 hover:text-rose-500 hover:bg-slate-100 dark:hover:bg-slate-800 rounded-lg transition"
                          >
                            <Trash2 className="w-3.5 h-3.5" />
                          </button>
                        </>
                      )}

                      {permit.status === 'Draft' && (
                        <button
                          onClick={(e) => handleTriggerAiReview(e, permit.id)}
                          disabled={isSubmitting}
                          className="inline-flex items-center gap-1 px-2.5 py-1 bg-purple-100 dark:bg-purple-900/60 hover:bg-purple-200 dark:hover:bg-purple-800 text-purple-800 dark:text-purple-300 border border-purple-300 dark:border-purple-700 rounded-lg text-[11px] font-semibold transition"
                        >
                          <Cpu className="w-3 h-3" />
                          <span>AI Review</span>
                        </button>
                      )}
                      <span className="inline-flex items-center gap-1 text-slate-400 dark:text-slate-400 hover:text-amber-600 dark:hover:text-amber-400 font-semibold text-xs ml-1">
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