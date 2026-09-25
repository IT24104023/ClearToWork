import React, { useState } from 'react';
import { useSelector } from 'react-redux';
import type { RootState } from '../store';
import {
  useGetEquipmentQuery,
  useGetZonesQuery,
  useCreateEquipmentMutation,
  useUpdateEquipmentMutation,
  useDeleteEquipmentMutation,
  useAddInspectionRecordMutation,
  useAddCalibrationRecordMutation,
  useGetIsolationPointsQuery,
  useCreateIsolationPointMutation,
  useUpdateIsolationStateMutation,
} from '../store/apiSlice';
import { useTranslation } from '../context/I18nContext';
import {
  Wrench,
  CheckCircle2,
  XCircle,
  Search,
  Lock,
  Plus,
  Pencil,
  Trash2,
  ClipboardCheck,
  Gauge,
  X,
  AlertCircle,
  Tag,
} from 'lucide-react';
import type {
  Asset,
  CreateAssetRequest,
  UpdateAssetRequest,
  CreateInspectionRequest,
  CreateCalibrationRequest,
  CreateIsolationPointRequest,
} from '../types';

// ─── Add Equipment Modal ─────────────────────────────────────────────────────
interface AddEquipmentModalProps {
  onClose: () => void;
}

const AddEquipmentModal: React.FC<AddEquipmentModalProps> = ({ onClose }) => {
  const [createEquipment, { isLoading }] = useCreateEquipmentMutation();
  const { data: zones = [] } = useGetZonesQuery();

  const [assetTag, setAssetTag] = useState('');
  const [name, setName] = useState('');
  const [category, setCategory] = useState('Ventilation');
  const [currentZoneId, setCurrentZoneId] = useState('');
  const [error, setError] = useState<string | null>(null);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError(null);

    if (!assetTag.trim() || !name.trim() || !category.trim()) {
      setError('Please fill in all required fields.');
      return;
    }

    try {
      const payload: CreateAssetRequest = {
        assetTag: assetTag.trim().toUpperCase(),
        name: name.trim(),
        category: category.trim(),
        currentZoneId: currentZoneId || undefined,
      };
      await createEquipment(payload).unwrap();
      onClose();
    } catch (err: any) {
      setError(err?.data?.message || err?.message || 'Failed to register equipment.');
    }
  };

  return (
    <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/70 backdrop-blur-sm p-4">
      <div className="bg-white dark:bg-slate-900 border border-slate-200 dark:border-slate-700 rounded-3xl shadow-2xl w-full max-w-lg overflow-hidden">
        <div className="flex items-center justify-between p-5 border-b border-slate-200 dark:border-slate-800">
          <div className="flex items-center gap-2">
            <Wrench className="w-5 h-5 text-amber-500" />
            <h2 className="text-base font-black text-slate-900 dark:text-white">Register Safety Equipment</h2>
          </div>
          <button onClick={onClose} className="text-slate-400 hover:text-slate-600 dark:hover:text-white">
            <X className="w-5 h-5" />
          </button>
        </div>

        <form onSubmit={handleSubmit} className="p-5 space-y-4">
          {error && (
            <div className="p-3 bg-rose-50 dark:bg-rose-950/50 border border-rose-300 dark:border-rose-700 rounded-xl text-rose-700 dark:text-rose-300 text-xs flex items-center gap-2">
              <AlertCircle className="w-4 h-4 shrink-0" />
              <span>{error}</span>
            </div>
          )}

          <div className="grid grid-cols-2 gap-3">
            <div>
              <label className="text-xs font-semibold text-slate-700 dark:text-slate-300 block mb-1">
                Asset Tag <span className="text-rose-500">*</span>
              </label>
              <input
                type="text"
                required
                value={assetTag}
                onChange={(e) => setAssetTag(e.target.value)}
                placeholder="e.g. EX-BLOWER-04"
                className="w-full font-mono bg-slate-50 dark:bg-slate-950 border border-slate-300 dark:border-slate-700 rounded-xl px-3 py-2 text-xs text-slate-900 dark:text-white focus:outline-none focus:border-amber-500"
              />
            </div>
            <div>
              <label className="text-xs font-semibold text-slate-700 dark:text-slate-300 block mb-1">
                Category <span className="text-rose-500">*</span>
              </label>
              <select
                required
                value={category}
                onChange={(e) => setCategory(e.target.value)}
                className="w-full bg-slate-50 dark:bg-slate-950 border border-slate-300 dark:border-slate-700 rounded-xl px-3 py-2 text-xs text-slate-900 dark:text-white focus:outline-none focus:border-amber-500"
              >
                <option value="Ventilation">Ventilation</option>
                <option value="Gas Detection">Gas Detection</option>
                <option value="Electrical">Electrical</option>
                <option value="Fire Protection">Fire Protection</option>
                <option value="Lifting">Lifting</option>
                <option value="Access / Scaffolding">Access / Scaffolding</option>
              </select>
            </div>
          </div>

          <div>
            <label className="text-xs font-semibold text-slate-700 dark:text-slate-300 block mb-1">
              Equipment / Asset Name <span className="text-rose-500">*</span>
            </label>
            <input
              type="text"
              required
              value={name}
              onChange={(e) => setName(e.target.value)}
              placeholder="e.g. Explosion-Proof Portable Extraction Fan"
              className="w-full bg-slate-50 dark:bg-slate-950 border border-slate-300 dark:border-slate-700 rounded-xl px-3 py-2 text-xs text-slate-900 dark:text-white focus:outline-none focus:border-amber-500"
            />
          </div>

          <div>
            <label className="text-xs font-semibold text-slate-700 dark:text-slate-300 block mb-1">
              Assigned Plant Zone Location
            </label>
            <select
              value={currentZoneId}
              onChange={(e) => setCurrentZoneId(e.target.value)}
              className="w-full bg-slate-50 dark:bg-slate-950 border border-slate-300 dark:border-slate-700 rounded-xl px-3 py-2 text-xs text-slate-900 dark:text-white focus:outline-none focus:border-amber-500"
            >
              <option value="">Unassigned (Central Tool Depot)</option>
              {zones.map((z) => (
                <option key={z.id} value={z.id}>
                  {z.name} ({z.code})
                </option>
              ))}
            </select>
          </div>

          <div className="flex justify-end gap-3 pt-3 border-t border-slate-200 dark:border-slate-800">
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
              {isLoading ? 'Saving...' : 'Register Asset'}
            </button>
          </div>
        </form>
      </div>
    </div>
  );
};

// ─── Edit Equipment Modal ────────────────────────────────────────────────────
interface EditEquipmentModalProps {
  asset: Asset;
  onClose: () => void;
}

const EditEquipmentModal: React.FC<EditEquipmentModalProps> = ({ asset, onClose }) => {
  const [updateEquipment, { isLoading }] = useUpdateEquipmentMutation();
  const { data: zones = [] } = useGetZonesQuery();

  const [name, setName] = useState(asset.name);
  const [category, setCategory] = useState(asset.category);
  const [status, setStatus] = useState(asset.status || 'Available');
  const [currentZoneId, setCurrentZoneId] = useState(asset.currentZoneId || '');
  const [error, setError] = useState<string | null>(null);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError(null);

    if (!name.trim() || !category.trim()) {
      setError('Please fill in all required fields.');
      return;
    }

    try {
      const payload: UpdateAssetRequest = {
        name: name.trim(),
        category: category.trim(),
        status,
        currentZoneId: currentZoneId || undefined,
      };
      await updateEquipment({ id: asset.id, body: payload }).unwrap();
      onClose();
    } catch (err: any) {
      setError(err?.data?.message || err?.message || 'Failed to update equipment.');
    }
  };

  return (
    <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/70 backdrop-blur-sm p-4">
      <div className="bg-white dark:bg-slate-900 border border-slate-200 dark:border-slate-700 rounded-3xl shadow-2xl w-full max-w-lg overflow-hidden">
        <div className="flex items-center justify-between p-5 border-b border-slate-200 dark:border-slate-800">
          <div className="flex items-center gap-2">
            <Pencil className="w-4 h-4 text-amber-500" />
            <h2 className="text-base font-black text-slate-900 dark:text-white">
              Edit Equipment ({asset.assetTag})
            </h2>
          </div>
          <button onClick={onClose} className="text-slate-400 hover:text-slate-600 dark:hover:text-white">
            <X className="w-5 h-5" />
          </button>
        </div>

        <form onSubmit={handleSubmit} className="p-5 space-y-4">
          {error && (
            <div className="p-3 bg-rose-50 dark:bg-rose-950/50 border border-rose-300 dark:border-rose-700 rounded-xl text-rose-700 dark:text-rose-300 text-xs flex items-center gap-2">
              <AlertCircle className="w-4 h-4 shrink-0" />
              <span>{error}</span>
            </div>
          )}

          <div>
            <label className="text-xs font-semibold text-slate-700 dark:text-slate-300 block mb-1">
              Equipment Name <span className="text-rose-500">*</span>
            </label>
            <input
              type="text"
              required
              value={name}
              onChange={(e) => setName(e.target.value)}
              className="w-full bg-slate-50 dark:bg-slate-950 border border-slate-300 dark:border-slate-700 rounded-xl px-3 py-2 text-xs text-slate-900 dark:text-white focus:outline-none focus:border-amber-500"
            />
          </div>

          <div className="grid grid-cols-2 gap-3">
            <div>
              <label className="text-xs font-semibold text-slate-700 dark:text-slate-300 block mb-1">
                Category <span className="text-rose-500">*</span>
              </label>
              <input
                type="text"
                required
                value={category}
                onChange={(e) => setCategory(e.target.value)}
                className="w-full bg-slate-50 dark:bg-slate-950 border border-slate-300 dark:border-slate-700 rounded-xl px-3 py-2 text-xs text-slate-900 dark:text-white focus:outline-none focus:border-amber-500"
              />
            </div>
            <div>
              <label className="text-xs font-semibold text-slate-700 dark:text-slate-300 block mb-1">
                Status <span className="text-rose-500">*</span>
              </label>
              <select
                value={status}
                onChange={(e) => setStatus(e.target.value)}
                className="w-full bg-slate-50 dark:bg-slate-950 border border-slate-300 dark:border-slate-700 rounded-xl px-3 py-2 text-xs text-slate-900 dark:text-white focus:outline-none focus:border-amber-500"
              >
                <option value="Available">Available</option>
                <option value="Reserved">Reserved</option>
                <option value="InUse">InUse</option>
                <option value="OutOfService">OutOfService</option>
              </select>
            </div>
          </div>

          <div>
            <label className="text-xs font-semibold text-slate-700 dark:text-slate-300 block mb-1">
              Plant Zone Location
            </label>
            <select
              value={currentZoneId}
              onChange={(e) => setCurrentZoneId(e.target.value)}
              className="w-full bg-slate-50 dark:bg-slate-950 border border-slate-300 dark:border-slate-700 rounded-xl px-3 py-2 text-xs text-slate-900 dark:text-white focus:outline-none focus:border-amber-500"
            >
              <option value="">Unassigned (Central Tool Depot)</option>
              {zones.map((z) => (
                <option key={z.id} value={z.id}>
                  {z.name} ({z.code})
                </option>
              ))}
            </select>
          </div>

          <div className="flex justify-end gap-3 pt-3 border-t border-slate-200 dark:border-slate-800">
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
              {isLoading ? 'Saving...' : 'Save Changes'}
            </button>
          </div>
        </form>
      </div>
    </div>
  );
};

// ─── Log Inspection Modal ────────────────────────────────────────────────────
interface LogInspectionModalProps {
  asset: Asset;
  onClose: () => void;
}

const LogInspectionModal: React.FC<LogInspectionModalProps> = ({ asset, onClose }) => {
  const [addInspection, { isLoading }] = useAddInspectionRecordMutation();
  const [inspectorName, setInspectorName] = useState('Senior HSE Inspector');
  const [isPassed, setIsPassed] = useState(true);
  const [notes, setNotes] = useState('Visual check complete; seals and spark guards intact.');
  const [nextInspectionDate, setNextInspectionDate] = useState(() => {
    const d = new Date();
    d.setMonth(d.getMonth() + 6);
    return d.toISOString().split('T')[0];
  });
  const [error, setError] = useState<string | null>(null);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError(null);

    if (!inspectorName.trim()) {
      setError('Please provide inspector name.');
      return;
    }

    try {
      const payload: CreateInspectionRequest = {
        inspectorName: inspectorName.trim(),
        isPassed,
        notes: notes.trim() || undefined,
        nextInspectionDate: nextInspectionDate ? new Date(nextInspectionDate).toISOString() : undefined,
      };
      await addInspection({ id: asset.id, body: payload }).unwrap();
      onClose();
    } catch (err: any) {
      setError(err?.data?.message || err?.message || 'Failed to log inspection record.');
    }
  };

  return (
    <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/70 backdrop-blur-sm p-4">
      <div className="bg-white dark:bg-slate-900 border border-slate-200 dark:border-slate-700 rounded-3xl shadow-2xl w-full max-w-lg overflow-hidden">
        <div className="flex items-center justify-between p-5 border-b border-slate-200 dark:border-slate-800">
          <div className="flex items-center gap-2">
            <ClipboardCheck className="w-5 h-5 text-emerald-500" />
            <div>
              <h2 className="text-base font-black text-slate-900 dark:text-white">Log Asset Inspection</h2>
              <p className="text-[11px] text-slate-500 dark:text-slate-400">
                {asset.name} ({asset.assetTag})
              </p>
            </div>
          </div>
          <button onClick={onClose} className="text-slate-400 hover:text-slate-600 dark:hover:text-white">
            <X className="w-5 h-5" />
          </button>
        </div>

        <form onSubmit={handleSubmit} className="p-5 space-y-4">
          {error && (
            <div className="p-3 bg-rose-50 dark:bg-rose-950/50 border border-rose-300 dark:border-rose-700 rounded-xl text-rose-700 dark:text-rose-300 text-xs flex items-center gap-2">
              <AlertCircle className="w-4 h-4 shrink-0" />
              <span>{error}</span>
            </div>
          )}

          <div>
            <label className="text-xs font-semibold text-slate-700 dark:text-slate-300 block mb-1">
              Inspector Name <span className="text-rose-500">*</span>
            </label>
            <input
              type="text"
              required
              value={inspectorName}
              onChange={(e) => setInspectorName(e.target.value)}
              className="w-full bg-slate-50 dark:bg-slate-950 border border-slate-300 dark:border-slate-700 rounded-xl px-3 py-2 text-xs text-slate-900 dark:text-white focus:outline-none focus:border-amber-500"
            />
          </div>

          <div className="grid grid-cols-2 gap-3">
            <div>
              <label className="text-xs font-semibold text-slate-700 dark:text-slate-300 block mb-1">
                Inspection Verdict <span className="text-rose-500">*</span>
              </label>
              <select
                value={isPassed ? 'true' : 'false'}
                onChange={(e) => setIsPassed(e.target.value === 'true')}
                className="w-full bg-slate-50 dark:bg-slate-950 border border-slate-300 dark:border-slate-700 rounded-xl px-3 py-2 text-xs text-slate-900 dark:text-white focus:outline-none focus:border-amber-500"
              >
                <option value="true">PASSED - In-Service</option>
                <option value="false">FAILED - Restrict Out-of-Service</option>
              </select>
            </div>
            <div>
              <label className="text-xs font-semibold text-slate-700 dark:text-slate-300 block mb-1">
                Next Inspection Due
              </label>
              <input
                type="date"
                value={nextInspectionDate}
                onChange={(e) => setNextInspectionDate(e.target.value)}
                className="w-full bg-slate-50 dark:bg-slate-950 border border-slate-300 dark:border-slate-700 rounded-xl px-3 py-2 text-xs text-slate-900 dark:text-white focus:outline-none focus:border-amber-500"
              />
            </div>
          </div>

          <div>
            <label className="text-xs font-semibold text-slate-700 dark:text-slate-300 block mb-1">
              Inspection Notes / Verification Evidence
            </label>
            <textarea
              rows={2}
              value={notes}
              onChange={(e) => setNotes(e.target.value)}
              className="w-full bg-slate-50 dark:bg-slate-950 border border-slate-300 dark:border-slate-700 rounded-xl px-3 py-2 text-xs text-slate-900 dark:text-white placeholder-slate-400 focus:outline-none focus:border-amber-500"
            />
          </div>

          <div className="flex justify-end gap-3 pt-3 border-t border-slate-200 dark:border-slate-800">
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
              className="flex items-center gap-1.5 px-4 py-2 bg-emerald-600 hover:bg-emerald-500 disabled:opacity-50 text-white rounded-xl text-xs font-bold transition shadow-sm"
            >
              <ClipboardCheck className="w-3.5 h-3.5" />
              {isLoading ? 'Recording...' : 'Submit Inspection'}
            </button>
          </div>
        </form>
      </div>
    </div>
  );
};

// ─── Log Calibration Modal ───────────────────────────────────────────────────
interface LogCalibrationModalProps {
  asset: Asset;
  onClose: () => void;
}

const LogCalibrationModal: React.FC<LogCalibrationModalProps> = ({ asset, onClose }) => {
  const [addCalibration, { isLoading }] = useAddCalibrationRecordMutation();
  const [calibratedBy, setCalibratedBy] = useState('Certified Calibration Lab Ltd');
  const [certificateNumber, setCertificateNumber] = useState(`CAL-${asset.assetTag}-2024`);
  const [isPassed, setIsPassed] = useState(true);
  const [nextCalibrationDate, setNextCalibrationDate] = useState(() => {
    const d = new Date();
    d.setMonth(d.getMonth() + 12);
    return d.toISOString().split('T')[0];
  });
  const [error, setError] = useState<string | null>(null);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError(null);

    if (!calibratedBy.trim() || !certificateNumber.trim()) {
      setError('Please fill in all required fields.');
      return;
    }

    try {
      const payload: CreateCalibrationRequest = {
        calibratedBy: calibratedBy.trim(),
        certificateNumber: certificateNumber.trim().toUpperCase(),
        isPassed,
        nextCalibrationDate: nextCalibrationDate ? new Date(nextCalibrationDate).toISOString() : undefined,
      };
      await addCalibration({ id: asset.id, body: payload }).unwrap();
      onClose();
    } catch (err: any) {
      setError(err?.data?.message || err?.message || 'Failed to log calibration record.');
    }
  };

  return (
    <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/70 backdrop-blur-sm p-4">
      <div className="bg-white dark:bg-slate-900 border border-slate-200 dark:border-slate-700 rounded-3xl shadow-2xl w-full max-w-lg overflow-hidden">
        <div className="flex items-center justify-between p-5 border-b border-slate-200 dark:border-slate-800">
          <div className="flex items-center gap-2">
            <Gauge className="w-5 h-5 text-amber-500" />
            <div>
              <h2 className="text-base font-black text-slate-900 dark:text-white">Log Calibration Certification</h2>
              <p className="text-[11px] text-slate-500 dark:text-slate-400">
                {asset.name} ({asset.assetTag})
              </p>
            </div>
          </div>
          <button onClick={onClose} className="text-slate-400 hover:text-slate-600 dark:hover:text-white">
            <X className="w-5 h-5" />
          </button>
        </div>

        <form onSubmit={handleSubmit} className="p-5 space-y-4">
          {error && (
            <div className="p-3 bg-rose-50 dark:bg-rose-950/50 border border-rose-300 dark:border-rose-700 rounded-xl text-rose-700 dark:text-rose-300 text-xs flex items-center gap-2">
              <AlertCircle className="w-4 h-4 shrink-0" />
              <span>{error}</span>
            </div>
          )}

          <div>
            <label className="text-xs font-semibold text-slate-700 dark:text-slate-300 block mb-1">
              Calibrated By (Organization / Lab) <span className="text-rose-500">*</span>
            </label>
            <input
              type="text"
              required
              value={calibratedBy}
              onChange={(e) => setCalibratedBy(e.target.value)}
              className="w-full bg-slate-50 dark:bg-slate-950 border border-slate-300 dark:border-slate-700 rounded-xl px-3 py-2 text-xs text-slate-900 dark:text-white focus:outline-none focus:border-amber-500"
            />
          </div>

          <div className="grid grid-cols-2 gap-3">
            <div>
              <label className="text-xs font-semibold text-slate-700 dark:text-slate-300 block mb-1">
                Calibration Cert # <span className="text-rose-500">*</span>
              </label>
              <input
                type="text"
                required
                value={certificateNumber}
                onChange={(e) => setCertificateNumber(e.target.value)}
                className="w-full font-mono bg-slate-50 dark:bg-slate-950 border border-slate-300 dark:border-slate-700 rounded-xl px-3 py-2 text-xs text-slate-900 dark:text-white focus:outline-none focus:border-amber-500"
              />
            </div>
            <div>
              <label className="text-xs font-semibold text-slate-700 dark:text-slate-300 block mb-1">
                Calibration Outcome <span className="text-rose-500">*</span>
              </label>
              <select
                value={isPassed ? 'true' : 'false'}
                onChange={(e) => setIsPassed(e.target.value === 'true')}
                className="w-full bg-slate-50 dark:bg-slate-950 border border-slate-300 dark:border-slate-700 rounded-xl px-3 py-2 text-xs text-slate-900 dark:text-white focus:outline-none focus:border-amber-500"
              >
                <option value="true">PASSED - Certified</option>
                <option value="false">FAILED - Out of Tolerance</option>
              </select>
            </div>
          </div>

          <div>
            <label className="text-xs font-semibold text-slate-700 dark:text-slate-300 block mb-1">
              Next Calibration Due
            </label>
            <input
              type="date"
              value={nextCalibrationDate}
              onChange={(e) => setNextCalibrationDate(e.target.value)}
              className="w-full bg-slate-50 dark:bg-slate-950 border border-slate-300 dark:border-slate-700 rounded-xl px-3 py-2 text-xs text-slate-900 dark:text-white focus:outline-none focus:border-amber-500"
            />
          </div>

          <div className="flex justify-end gap-3 pt-3 border-t border-slate-200 dark:border-slate-800">
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
              <Gauge className="w-3.5 h-3.5" />
              {isLoading ? 'Saving...' : 'Record Calibration'}
            </button>
          </div>
        </form>
      </div>
    </div>
  );
};

// ─── Add Isolation Point Modal ───────────────────────────────────────────────
interface AddIsolationPointModalProps {
  defaultZoneId: string;
  onClose: () => void;
}

const AddIsolationPointModal: React.FC<AddIsolationPointModalProps> = ({ defaultZoneId, onClose }) => {
  const [createPoint, { isLoading }] = useCreateIsolationPointMutation();
  const { data: zones = [] } = useGetZonesQuery();

  const [code, setCode] = useState('');
  const [name, setName] = useState('');
  const [zoneId, setZoneId] = useState(defaultZoneId || (zones[0]?.id || ''));
  const [notes, setNotes] = useState('');
  const [error, setError] = useState<string | null>(null);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError(null);

    if (!code.trim() || !name.trim() || !zoneId) {
      setError('Please fill in all required fields.');
      return;
    }

    try {
      const payload: CreateIsolationPointRequest = {
        code: code.trim().toUpperCase(),
        name: name.trim(),
        zoneId,
        notes: notes.trim() || undefined,
      };
      await createPoint(payload).unwrap();
      onClose();
    } catch (err: any) {
      setError(err?.data?.message || err?.message || 'Failed to create isolation point.');
    }
  };

  return (
    <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/70 backdrop-blur-sm p-4">
      <div className="bg-white dark:bg-slate-900 border border-slate-200 dark:border-slate-700 rounded-3xl shadow-2xl w-full max-w-lg overflow-hidden">
        <div className="flex items-center justify-between p-5 border-b border-slate-200 dark:border-slate-800">
          <div className="flex items-center gap-2">
            <Lock className="w-5 h-5 text-amber-500" />
            <h2 className="text-base font-black text-slate-900 dark:text-white">Register LOTO Isolation Point</h2>
          </div>
          <button onClick={onClose} className="text-slate-400 hover:text-slate-600 dark:hover:text-white">
            <X className="w-5 h-5" />
          </button>
        </div>

        <form onSubmit={handleSubmit} className="p-5 space-y-4">
          {error && (
            <div className="p-3 bg-rose-50 dark:bg-rose-950/50 border border-rose-300 dark:border-rose-700 rounded-xl text-rose-700 dark:text-rose-300 text-xs flex items-center gap-2">
              <AlertCircle className="w-4 h-4 shrink-0" />
              <span>{error}</span>
            </div>
          )}

          <div className="grid grid-cols-2 gap-3">
            <div>
              <label className="text-xs font-semibold text-slate-700 dark:text-slate-300 block mb-1">
                Isolation Code <span className="text-rose-500">*</span>
              </label>
              <input
                type="text"
                required
                value={code}
                onChange={(e) => setCode(e.target.value)}
                placeholder="e.g. ISO-Z1-VALVE-05"
                className="w-full font-mono bg-slate-50 dark:bg-slate-950 border border-slate-300 dark:border-slate-700 rounded-xl px-3 py-2 text-xs text-slate-900 dark:text-white focus:outline-none focus:border-amber-500"
              />
            </div>
            <div>
              <label className="text-xs font-semibold text-slate-700 dark:text-slate-300 block mb-1">
                Plant Zone Location <span className="text-rose-500">*</span>
              </label>
              <select
                required
                value={zoneId}
                onChange={(e) => setZoneId(e.target.value)}
                className="w-full bg-slate-50 dark:bg-slate-950 border border-slate-300 dark:border-slate-700 rounded-xl px-3 py-2 text-xs text-slate-900 dark:text-white focus:outline-none focus:border-amber-500"
              >
                {zones.map((z) => (
                  <option key={z.id} value={z.id}>
                    {z.name} ({z.code})
                  </option>
                ))}
              </select>
            </div>
          </div>

          <div>
            <label className="text-xs font-semibold text-slate-700 dark:text-slate-300 block mb-1">
              Isolation Point Description <span className="text-rose-500">*</span>
            </label>
            <input
              type="text"
              required
              value={name}
              onChange={(e) => setName(e.target.value)}
              placeholder="e.g. Solvent manifold supply valve #3"
              className="w-full bg-slate-50 dark:bg-slate-950 border border-slate-300 dark:border-slate-700 rounded-xl px-3 py-2 text-xs text-slate-900 dark:text-white focus:outline-none focus:border-amber-500"
            />
          </div>

          <div>
            <label className="text-xs font-semibold text-slate-700 dark:text-slate-300 block mb-1">
              Safety / Procedure Notes
            </label>
            <textarea
              rows={2}
              value={notes}
              onChange={(e) => setNotes(e.target.value)}
              placeholder="e.g. Lockout with Master Lock #410 and bleed residual line pressure..."
              className="w-full bg-slate-50 dark:bg-slate-950 border border-slate-300 dark:border-slate-700 rounded-xl px-3 py-2 text-xs text-slate-900 dark:text-white placeholder-slate-400 focus:outline-none focus:border-amber-500"
            />
          </div>

          <div className="flex justify-end gap-3 pt-3 border-t border-slate-200 dark:border-slate-800">
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
              {isLoading ? 'Saving...' : 'Add Isolation Point'}
            </button>
          </div>
        </form>
      </div>
    </div>
  );
};

// ─── Main Equipment Page ─────────────────────────────────────────────────────
export const EquipmentPage: React.FC = () => {
  const { t } = useTranslation();
  const currentUser = useSelector((state: RootState) => state.auth.user);
  const [searchTerm, setSearchTerm] = useState('');

  // Modals state
  const [showAddEquipment, setShowAddEquipment] = useState(false);
  const [editingAsset, setEditingAsset] = useState<Asset | null>(null);
  const [inspectTargetAsset, setInspectTargetAsset] = useState<Asset | null>(null);
  const [calibrateTargetAsset, setCalibrateTargetAsset] = useState<Asset | null>(null);
  const [showAddIsolation, setShowAddIsolation] = useState(false);

  // Queries
  const { data: assets = [], isLoading } = useGetEquipmentQuery();
  const { data: zones = [] } = useGetZonesQuery();
  const [selectedZoneId, setSelectedZoneId] = useState<string>('');

  // Fallback selected zone ID
  const effectiveZoneId = selectedZoneId || (zones[0]?.id ?? '');
  const { data: isolationPoints = [] } = useGetIsolationPointsQuery(effectiveZoneId, {
    skip: !effectiveZoneId,
  });

  // Mutations
  const [deleteEquipment] = useDeleteEquipmentMutation();
  const [updateIsolationState] = useUpdateIsolationStateMutation();

  // Role permissions:
  const canManageEquipment =
    currentUser?.role === 'Administrator' || currentUser?.role === 'AreaSupervisor';
  const canLogHse =
    currentUser?.role === 'Administrator' ||
    currentUser?.role === 'AreaSupervisor' ||
    currentUser?.role === 'SafetyOfficer';

  const handleDeleteEquipment = async (asset: Asset) => {
    if (window.confirm(`Are you sure you want to delete asset ${asset.name} (${asset.assetTag})?`)) {
      try {
        await deleteEquipment(asset.id).unwrap();
      } catch (err: any) {
        alert(err?.data?.message || 'Failed to delete equipment.');
      }
    }
  };

  const handleToggleState = async (pointId: string, newState: 'Open' | 'LockedOut' | 'TaggedOut') => {
    try {
      await updateIsolationState({
        id: pointId,
        body: { state: newState, notes: `State toggled to ${newState} by ${currentUser?.fullName || 'User'}` },
      }).unwrap();
    } catch (err: any) {
      alert(err?.data?.message || 'Failed to update isolation state.');
    }
  };

  const filteredAssets = assets.filter(
    (a) =>
      a.name.toLowerCase().includes(searchTerm.toLowerCase()) ||
      a.assetTag.toLowerCase().includes(searchTerm.toLowerCase()) ||
      a.category.toLowerCase().includes(searchTerm.toLowerCase())
  );

  return (
    <div className="space-y-6 max-w-6xl mx-auto">
      {/* Modals */}
      {showAddEquipment && <AddEquipmentModal onClose={() => setShowAddEquipment(false)} />}
      {editingAsset && (
        <EditEquipmentModal asset={editingAsset} onClose={() => setEditingAsset(null)} />
      )}
      {inspectTargetAsset && (
        <LogInspectionModal
          asset={inspectTargetAsset}
          onClose={() => setInspectTargetAsset(null)}
        />
      )}
      {calibrateTargetAsset && (
        <LogCalibrationModal
          asset={calibrateTargetAsset}
          onClose={() => setCalibrateTargetAsset(null)}
        />
      )}
      {showAddIsolation && (
        <AddIsolationPointModal
          defaultZoneId={effectiveZoneId}
          onClose={() => setShowAddIsolation(false)}
        />
      )}

      {/* Header */}
      <div className="flex flex-wrap items-center justify-between gap-4">
        <div>
          <h1 className="text-2xl font-black text-slate-900 dark:text-slate-100 tracking-tight flex items-center gap-2">
            <Wrench className="w-6 h-6 text-amber-500 dark:text-amber-400" />
            <span>{t('equipment_title')}</span>
          </h1>
          <p className="text-xs text-slate-500 dark:text-slate-400 mt-1">
            {t('equipment_subtitle')}
          </p>
        </div>

        {canManageEquipment && (
          <button
            onClick={() => setShowAddEquipment(true)}
            className="flex items-center gap-1.5 px-4 py-2 bg-amber-500 hover:bg-amber-400 text-slate-950 rounded-xl text-xs font-bold transition shadow-sm"
          >
            <Plus className="w-3.5 h-3.5" />
            <span>Register Equipment</span>
          </button>
        )}
      </div>

      {/* Interactive Lockout / Tagout (LOTO) Section */}
      <div className="bg-white dark:bg-slate-900 border border-slate-200 dark:border-slate-800 rounded-2xl p-5 shadow-sm space-y-4">
        <div className="flex flex-wrap items-center justify-between gap-3 pb-3 border-b border-slate-200 dark:border-slate-800">
          <div className="flex items-center gap-2 text-amber-600 dark:text-amber-400 font-bold text-sm">
            <Lock className="w-4 h-4" />
            <span>{t('equipment_loto_title')}</span>
          </div>

          <div className="flex items-center gap-2">
            <select
              value={effectiveZoneId}
              onChange={(e) => setSelectedZoneId(e.target.value)}
              className="bg-slate-50 dark:bg-slate-950 border border-slate-300 dark:border-slate-700 rounded-xl px-3 py-1.5 text-xs text-slate-900 dark:text-white font-medium"
            >
              {zones.map((z) => (
                <option key={z.id} value={z.id}>
                  Zone: {z.name} ({z.code})
                </option>
              ))}
            </select>

            {canManageEquipment && (
              <button
                onClick={() => setShowAddIsolation(true)}
                className="flex items-center gap-1 px-3 py-1.5 bg-slate-100 dark:bg-slate-800 hover:bg-slate-200 dark:hover:bg-slate-700 text-slate-800 dark:text-slate-200 text-xs font-semibold rounded-xl transition"
              >
                <Plus className="w-3 h-3 text-amber-500" />
                <span>Add Point</span>
              </button>
            )}
          </div>
        </div>

        <p className="text-xs text-slate-600 dark:text-slate-300">
          {t('equipment_loto_desc')}
        </p>

        {isolationPoints.length === 0 ? (
          <div className="text-xs text-slate-400 dark:text-slate-500 italic py-2 text-center">
            No isolation points registered for this zone.
          </div>
        ) : (
          <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-3 text-xs">
            {isolationPoints.map((point) => (
              <div
                key={point.id}
                className="bg-slate-50 dark:bg-slate-950 p-4 rounded-xl border border-slate-200 dark:border-slate-800 flex flex-col justify-between gap-3 shadow-sm"
              >
                <div>
                  <div className="flex items-center justify-between">
                    <span className="font-mono text-amber-600 dark:text-amber-400 font-bold">
                      {point.code}
                    </span>
                    <span
                      className={`px-2 py-0.5 rounded font-mono text-[10px] font-bold border ${
                        point.currentState === 'LockedOut'
                          ? 'bg-amber-100 dark:bg-amber-950 text-amber-700 dark:text-amber-400 border-amber-300 dark:border-amber-800'
                          : point.currentState === 'TaggedOut'
                          ? 'bg-blue-100 dark:bg-blue-950 text-blue-700 dark:text-blue-400 border-blue-300 dark:border-blue-800'
                          : 'bg-emerald-100 dark:bg-emerald-950 text-emerald-700 dark:text-emerald-400 border-emerald-300 dark:border-emerald-800'
                      }`}
                    >
                      {point.currentState === 'LockedOut'
                        ? 'LOCKED'
                        : point.currentState === 'TaggedOut'
                        ? 'TAGGED'
                        : 'OPEN / CLEAR'}
                    </span>
                  </div>
                  <div className="text-slate-700 dark:text-slate-300 font-medium text-xs mt-1">
                    {point.name}
                  </div>
                  {point.notes && (
                    <div className="text-slate-400 text-[11px] mt-0.5 italic">{point.notes}</div>
                  )}
                </div>

                <div className="flex items-center gap-1.5 pt-2 border-t border-slate-200 dark:border-slate-800">
                  <span className="text-[10px] text-slate-500 font-semibold uppercase mr-1">Set State:</span>
                  <button
                    onClick={() => handleToggleState(point.id, 'Open')}
                    className={`px-2 py-1 rounded text-[10px] font-bold border transition ${
                      point.currentState === 'Open'
                        ? 'bg-emerald-600 text-white border-emerald-600'
                        : 'bg-white dark:bg-slate-900 text-slate-600 dark:text-slate-400 border-slate-200 dark:border-slate-800 hover:border-emerald-500'
                    }`}
                  >
                    Open
                  </button>
                  <button
                    onClick={() => handleToggleState(point.id, 'LockedOut')}
                    className={`px-2 py-1 rounded text-[10px] font-bold border transition ${
                      point.currentState === 'LockedOut'
                        ? 'bg-amber-500 text-slate-950 border-amber-500'
                        : 'bg-white dark:bg-slate-900 text-slate-600 dark:text-slate-400 border-slate-200 dark:border-slate-800 hover:border-amber-500'
                    }`}
                  >
                    <Lock className="w-2.5 h-2.5 inline mr-0.5" /> Lock
                  </button>
                  <button
                    onClick={() => handleToggleState(point.id, 'TaggedOut')}
                    className={`px-2 py-1 rounded text-[10px] font-bold border transition ${
                      point.currentState === 'TaggedOut'
                        ? 'bg-blue-600 text-white border-blue-600'
                        : 'bg-white dark:bg-slate-900 text-slate-600 dark:text-slate-400 border-slate-200 dark:border-slate-800 hover:border-blue-500'
                    }`}
                  >
                    <Tag className="w-2.5 h-2.5 inline mr-0.5" /> Tag
                  </button>
                </div>
              </div>
            ))}
          </div>
        )}
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
          <div className="p-8 text-center text-slate-500 dark:text-slate-400 text-xs">
            Loading equipment inventory...
          </div>
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
                  <th className="py-3 px-4 text-right">Actions</th>
                </tr>
              </thead>
              <tbody className="divide-y divide-slate-200 dark:divide-slate-800/60">
                {filteredAssets.map((asset) => {
                  const isReady = asset.isInspectionValid && asset.isCalibrationValid;

                  return (
                    <tr
                      key={asset.id}
                      className="hover:bg-slate-50 dark:hover:bg-slate-800/50 transition-colors"
                    >
                      <td className="py-3 px-4 font-mono font-bold text-amber-600 dark:text-amber-400">
                        {asset.assetTag}
                      </td>
                      <td className="py-3 px-4">
                        <div className="font-semibold text-slate-800 dark:text-slate-200">
                          {asset.name}
                        </div>
                        <div className="text-[10px] text-slate-400 capitalize">Status: {asset.status}</div>
                      </td>
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
                        {asset.nextInspectionDate && (
                          <div className="text-[10px] text-slate-400 font-mono">
                            Due: {new Date(asset.nextInspectionDate).toLocaleDateString()}
                          </div>
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
                        {asset.nextCalibrationDate && (
                          <div className="text-[10px] text-slate-400 font-mono">
                            Due: {new Date(asset.nextCalibrationDate).toLocaleDateString()}
                          </div>
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
                      <td className="py-3 px-4 text-right">
                        <div className="inline-flex items-center gap-1">
                          {canLogHse && (
                            <>
                              <button
                                onClick={() => setInspectTargetAsset(asset)}
                                title="Log Inspection"
                                className="p-1.5 text-slate-400 hover:text-emerald-500 hover:bg-slate-100 dark:hover:bg-slate-800 rounded-lg transition"
                              >
                                <ClipboardCheck className="w-3.5 h-3.5" />
                              </button>
                              <button
                                onClick={() => setCalibrateTargetAsset(asset)}
                                title="Log Calibration"
                                className="p-1.5 text-slate-400 hover:text-amber-500 hover:bg-slate-100 dark:hover:bg-slate-800 rounded-lg transition"
                              >
                                <Gauge className="w-3.5 h-3.5" />
                              </button>
                            </>
                          )}

                          {canManageEquipment && (
                            <>
                              <button
                                onClick={() => setEditingAsset(asset)}
                                title="Edit Asset"
                                className="p-1.5 text-slate-400 hover:text-amber-500 hover:bg-slate-100 dark:hover:bg-slate-800 rounded-lg transition"
                              >
                                <Pencil className="w-3.5 h-3.5" />
                              </button>
                              <button
                                onClick={() => handleDeleteEquipment(asset)}
                                title="Delete Asset"
                                className="p-1.5 text-slate-400 hover:text-rose-500 hover:bg-slate-100 dark:hover:bg-slate-800 rounded-lg transition"
                              >
                                <Trash2 className="w-3.5 h-3.5" />
                              </button>
                            </>
                          )}
                        </div>
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