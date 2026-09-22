import React, { useState } from 'react';
import {
  useUpdatePermitDraftMutation,
  useGetZonesQuery,
  useGetWorkersQuery,
  useGetEquipmentQuery,
} from '../store/apiSlice';
import { X, Pencil, AlertCircle } from 'lucide-react';
import type { PermitDetails, UpdatePermitRequest } from '../types';

interface EditPermitModalProps {
  permit: PermitDetails;
  onClose: () => void;
  onUpdated?: () => void;
}

const PERMIT_TYPES = [
  { code: 'HOT_WORK', name: 'Hot Work Operational Permit' },
  { code: 'SOLVENT_PAINTING', name: 'Solvent Application Permit' },
];

export const EditPermitModal: React.FC<EditPermitModalProps> = ({
  permit,
  onClose,
  onUpdated,
}) => {
  const [updatePermit, { isLoading }] = useUpdatePermitDraftMutation();
  const { data: zones = [] } = useGetZonesQuery();
  const { data: workers = [] } = useGetWorkersQuery();
  const { data: equipment = [] } = useGetEquipmentQuery();

  const [permitTypeCode, setPermitTypeCode] = useState(permit.permitTypeCode || 'HOT_WORK');
  const [zoneId, setZoneId] = useState(
    zones.find((z) => z.name === permit.zoneName || z.code === permit.zoneCode)?.id || ''
  );
  const [objective, setObjective] = useState(permit.objectiveDescription || '');

  const startIso = permit.scheduledStartTime ? new Date(permit.scheduledStartTime) : new Date();
  const endIso = permit.scheduledEndTime ? new Date(permit.scheduledEndTime) : new Date();

  const [startDate, setStartDate] = useState(startIso.toISOString().split('T')[0]);
  const [startTime, setStartTime] = useState(
    startIso.toTimeString().slice(0, 5) || '09:00'
  );
  const [endTime, setEndTime] = useState(
    endIso.toTimeString().slice(0, 5) || '12:00'
  );

  const [selectedWorkerIds, setSelectedWorkerIds] = useState<string[]>(
    permit.assignedWorkers?.map((w) => w.id) || []
  );
  const [selectedAssetIds, setSelectedAssetIds] = useState<string[]>(
    permit.assignedAssets?.map((a) => a.id) || []
  );
  const [error, setError] = useState<string | null>(null);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError(null);

    // Fallback zone id if not explicitly set yet
    const effectiveZoneId = zoneId || zones[0]?.id;

    if (!effectiveZoneId || !objective.trim() || !startDate) {
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
      const typesResp = await fetch('/api/permits/types');
      const types: { id: string; code: string }[] = typesResp.ok ? await typesResp.json() : [];
      const matched = types.find((t) => t.code === permitTypeCode);
      if (!matched) {
        setError('Could not resolve permit type.');
        return;
      }

      const payload: UpdatePermitRequest = {
        permitTypeId: matched.id,
        zoneId: effectiveZoneId,
        objectiveDescription: objective,
        scheduledStartTime: startDt.toISOString(),
        scheduledEndTime: endDt.toISOString(),
        workerIds: selectedWorkerIds,
        assetIds: selectedAssetIds,
      };

      await updatePermit({ id: permit.id, body: payload }).unwrap();
      onUpdated?.();
      onClose();
    } catch (err: any) {
      setError(err?.data?.message || err?.message || 'Failed to update permit draft.');
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
    <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/70 backdrop-blur-sm p-4 text-left">
      <div className="bg-white dark:bg-slate-900 border border-slate-200 dark:border-slate-700 rounded-3xl shadow-2xl w-full max-w-2xl max-h-[90vh] overflow-y-auto">
        <div className="flex items-center justify-between p-5 border-b border-slate-200 dark:border-slate-800">
          <div className="flex items-center gap-2">
            <Pencil className="w-5 h-5 text-amber-500" />
            <div>
              <h2 className="text-base font-black text-slate-900 dark:text-white">
                Edit Permit Draft ({permit.permitNumber})
              </h2>
              <p className="text-xs text-slate-500 dark:text-slate-400 mt-0.5">
                Update work parameters or remediation before resubmitting for AI Review.
              </p>
            </div>
          </div>
          <button onClick={onClose} className="text-slate-400 hover:text-slate-600 dark:hover:text-white">
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

          <div>
            <label className="text-xs font-semibold text-slate-700 dark:text-slate-300 block mb-1.5">
              Work Objective &amp; Hazard Scope <span className="text-rose-500">*</span>
            </label>
            <textarea
              rows={2}
              required
              value={objective}
              onChange={(e) => setObjective(e.target.value)}
              className="w-full bg-slate-50 dark:bg-slate-950 border border-slate-300 dark:border-slate-700 rounded-xl px-3 py-2 text-xs text-slate-900 dark:text-white placeholder-slate-400 dark:placeholder-slate-500 focus:outline-none focus:border-amber-500"
            />
          </div>

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

          <div>
            <label className="text-xs font-semibold text-slate-700 dark:text-slate-300 block mb-1.5">
              Assign Certified Workforce
            </label>
            <div className="bg-slate-50 dark:bg-slate-950 border border-slate-300 dark:border-slate-700 rounded-xl p-2 max-h-36 overflow-y-auto space-y-1">
              {workers.map((w) => (
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
                  <span className="text-xs text-slate-900 dark:text-slate-200 font-semibold">
                    {w.firstName} {w.lastName}
                  </span>
                  <span className="text-[10px] text-slate-500 font-mono">({w.badgeNumber})</span>
                  <span className="text-[10px] text-slate-400 ml-auto">{w.trade}</span>
                </label>
              ))}
            </div>
          </div>

          <div>
            <label className="text-xs font-semibold text-slate-700 dark:text-slate-300 block mb-1.5">
              Assign Safety Equipment
            </label>
            <div className="bg-slate-50 dark:bg-slate-950 border border-slate-300 dark:border-slate-700 rounded-xl p-2 max-h-36 overflow-y-auto space-y-1">
              {equipment.map((a) => (
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
                  <span className="text-xs text-slate-900 dark:text-slate-200 font-mono font-semibold">
                    {a.assetTag}
                  </span>
                  <span className="text-xs text-slate-700 dark:text-slate-300">{a.name}</span>
                  <span className="text-[10px] text-slate-400 ml-auto">{a.category}</span>
                </label>
              ))}
            </div>
          </div>

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
              <Pencil className="w-3.5 h-3.5" />
              {isLoading ? 'Saving...' : 'Update Permit Draft'}
            </button>
          </div>
        </form>
      </div>
    </div>
  );
};
