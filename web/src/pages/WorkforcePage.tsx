import React, { useState } from 'react';
import { useSelector } from 'react-redux';
import type { RootState } from '../store';
import {
  useGetWorkersQuery,
  useGetExpiryForecastQuery,
  useGetContractorsQuery,
  useGetCertificateTypesQuery,
  useCreateWorkerMutation,
  useUpdateWorkerMutation,
  useDeleteWorkerMutation,
  useAddWorkerCertificateMutation,
  useDeleteWorkerCertificateMutation,
} from '../store/apiSlice';
import { useTranslation } from '../context/I18nContext';
import {
  Users2,
  CheckCircle2,
  XCircle,
  Search,
  CalendarClock,
  Plus,
  Pencil,
  Trash2,
  Award,
  X,
  AlertCircle,
  UserCheck,
  UserX,
} from 'lucide-react';
import type { Worker, CreateWorkerRequest, UpdateWorkerRequest, CreateCertificateRequest } from '../types';

// ─── Add Worker Modal ────────────────────────────────────────────────────────
interface AddWorkerModalProps {
  onClose: () => void;
}

const AddWorkerModal: React.FC<AddWorkerModalProps> = ({ onClose }) => {
  const [createWorker, { isLoading }] = useCreateWorkerMutation();
  const { data: contractors = [] } = useGetContractorsQuery();

  const [firstName, setFirstName] = useState('');
  const [lastName, setLastName] = useState('');
  const [badgeNumber, setBadgeNumber] = useState('');
  const [trade, setTrade] = useState('Welder');
  const [contractorId, setContractorId] = useState('');
  const [error, setError] = useState<string | null>(null);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError(null);

    if (!firstName.trim() || !lastName.trim() || !badgeNumber.trim() || !trade.trim() || !contractorId) {
      setError('Please fill in all required fields.');
      return;
    }

    try {
      const payload: CreateWorkerRequest = {
        firstName: firstName.trim(),
        lastName: lastName.trim(),
        badgeNumber: badgeNumber.trim().toUpperCase(),
        trade: trade.trim(),
        contractorId,
      };
      await createWorker(payload).unwrap();
      onClose();
    } catch (err: any) {
      setError(err?.data?.message || err?.message || 'Failed to create worker.');
    }
  };

  return (
    <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/70 backdrop-blur-sm p-4">
      <div className="bg-white dark:bg-slate-900 border border-slate-200 dark:border-slate-700 rounded-3xl shadow-2xl w-full max-w-lg overflow-hidden">
        <div className="flex items-center justify-between p-5 border-b border-slate-200 dark:border-slate-800">
          <div className="flex items-center gap-2">
            <Users2 className="w-5 h-5 text-amber-500" />
            <h2 className="text-base font-black text-slate-900 dark:text-white">Add New Worker</h2>
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
                First Name <span className="text-rose-500">*</span>
              </label>
              <input
                type="text"
                required
                value={firstName}
                onChange={(e) => setFirstName(e.target.value)}
                placeholder="e.g. Ruwan"
                className="w-full bg-slate-50 dark:bg-slate-950 border border-slate-300 dark:border-slate-700 rounded-xl px-3 py-2 text-xs text-slate-900 dark:text-white focus:outline-none focus:border-amber-500"
              />
            </div>
            <div>
              <label className="text-xs font-semibold text-slate-700 dark:text-slate-300 block mb-1">
                Last Name <span className="text-rose-500">*</span>
              </label>
              <input
                type="text"
                required
                value={lastName}
                onChange={(e) => setLastName(e.target.value)}
                placeholder="e.g. Fernando"
                className="w-full bg-slate-50 dark:bg-slate-950 border border-slate-300 dark:border-slate-700 rounded-xl px-3 py-2 text-xs text-slate-900 dark:text-white focus:outline-none focus:border-amber-500"
              />
            </div>
          </div>

          <div className="grid grid-cols-2 gap-3">
            <div>
              <label className="text-xs font-semibold text-slate-700 dark:text-slate-300 block mb-1">
                Badge # <span className="text-rose-500">*</span>
              </label>
              <input
                type="text"
                required
                value={badgeNumber}
                onChange={(e) => setBadgeNumber(e.target.value)}
                placeholder="e.g. W-1008"
                className="w-full font-mono bg-slate-50 dark:bg-slate-950 border border-slate-300 dark:border-slate-700 rounded-xl px-3 py-2 text-xs text-slate-900 dark:text-white focus:outline-none focus:border-amber-500"
              />
            </div>
            <div>
              <label className="text-xs font-semibold text-slate-700 dark:text-slate-300 block mb-1">
                Trade / Specialty <span className="text-rose-500">*</span>
              </label>
              <input
                type="text"
                required
                value={trade}
                onChange={(e) => setTrade(e.target.value)}
                placeholder="e.g. Welder, Electrician, Painter"
                className="w-full bg-slate-50 dark:bg-slate-950 border border-slate-300 dark:border-slate-700 rounded-xl px-3 py-2 text-xs text-slate-900 dark:text-white focus:outline-none focus:border-amber-500"
              />
            </div>
          </div>

          <div>
            <label className="text-xs font-semibold text-slate-700 dark:text-slate-300 block mb-1">
              Contractor Firm <span className="text-rose-500">*</span>
            </label>
            <select
              required
              value={contractorId}
              onChange={(e) => setContractorId(e.target.value)}
              className="w-full bg-slate-50 dark:bg-slate-950 border border-slate-300 dark:border-slate-700 rounded-xl px-3 py-2 text-xs text-slate-900 dark:text-white focus:outline-none focus:border-amber-500"
            >
              <option value="">Select a contractor...</option>
              {contractors.map((c) => (
                <option key={c.id} value={c.id}>
                  {c.name} ({c.code})
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
              {isLoading ? 'Saving...' : 'Register Worker'}
            </button>
          </div>
        </form>
      </div>
    </div>
  );
};

// ─── Edit Worker Modal ───────────────────────────────────────────────────────
interface EditWorkerModalProps {
  worker: Worker;
  onClose: () => void;
}

const EditWorkerModal: React.FC<EditWorkerModalProps> = ({ worker, onClose }) => {
  const [updateWorker, { isLoading }] = useUpdateWorkerMutation();
  const [firstName, setFirstName] = useState(worker.firstName);
  const [lastName, setLastName] = useState(worker.lastName);
  const [trade, setTrade] = useState(worker.trade);
  const [isActive, setIsActive] = useState(worker.isActive);
  const [error, setError] = useState<string | null>(null);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError(null);

    if (!firstName.trim() || !lastName.trim() || !trade.trim()) {
      setError('Please fill in all required fields.');
      return;
    }

    try {
      const payload: UpdateWorkerRequest = {
        firstName: firstName.trim(),
        lastName: lastName.trim(),
        trade: trade.trim(),
        isActive,
      };
      await updateWorker({ id: worker.id, body: payload }).unwrap();
      onClose();
    } catch (err: any) {
      setError(err?.data?.message || err?.message || 'Failed to update worker.');
    }
  };

  return (
    <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/70 backdrop-blur-sm p-4">
      <div className="bg-white dark:bg-slate-900 border border-slate-200 dark:border-slate-700 rounded-3xl shadow-2xl w-full max-w-lg overflow-hidden">
        <div className="flex items-center justify-between p-5 border-b border-slate-200 dark:border-slate-800">
          <div className="flex items-center gap-2">
            <Pencil className="w-4 h-4 text-amber-500" />
            <h2 className="text-base font-black text-slate-900 dark:text-white">
              Edit Worker ({worker.badgeNumber})
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

          <div className="grid grid-cols-2 gap-3">
            <div>
              <label className="text-xs font-semibold text-slate-700 dark:text-slate-300 block mb-1">
                First Name <span className="text-rose-500">*</span>
              </label>
              <input
                type="text"
                required
                value={firstName}
                onChange={(e) => setFirstName(e.target.value)}
                className="w-full bg-slate-50 dark:bg-slate-950 border border-slate-300 dark:border-slate-700 rounded-xl px-3 py-2 text-xs text-slate-900 dark:text-white focus:outline-none focus:border-amber-500"
              />
            </div>
            <div>
              <label className="text-xs font-semibold text-slate-700 dark:text-slate-300 block mb-1">
                Last Name <span className="text-rose-500">*</span>
              </label>
              <input
                type="text"
                required
                value={lastName}
                onChange={(e) => setLastName(e.target.value)}
                className="w-full bg-slate-50 dark:bg-slate-950 border border-slate-300 dark:border-slate-700 rounded-xl px-3 py-2 text-xs text-slate-900 dark:text-white focus:outline-none focus:border-amber-500"
              />
            </div>
          </div>

          <div>
            <label className="text-xs font-semibold text-slate-700 dark:text-slate-300 block mb-1">
              Trade / Specialty <span className="text-rose-500">*</span>
            </label>
            <input
              type="text"
              required
              value={trade}
              onChange={(e) => setTrade(e.target.value)}
              className="w-full bg-slate-50 dark:bg-slate-950 border border-slate-300 dark:border-slate-700 rounded-xl px-3 py-2 text-xs text-slate-900 dark:text-white focus:outline-none focus:border-amber-500"
            />
          </div>

          <div className="flex items-center gap-2 pt-2">
            <input
              type="checkbox"
              id="isActive"
              checked={isActive}
              onChange={(e) => setIsActive(e.target.checked)}
              className="accent-amber-500 w-4 h-4 rounded cursor-pointer"
            />
            <label htmlFor="isActive" className="text-xs font-semibold text-slate-700 dark:text-slate-300 cursor-pointer">
              Active Status (Permitted for assignment)
            </label>
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
              {isLoading ? 'Updating...' : 'Save Changes'}
            </button>
          </div>
        </form>
      </div>
    </div>
  );
};

// ─── Add Certificate Modal ───────────────────────────────────────────────────
interface AddCertificateModalProps {
  worker: Worker;
  onClose: () => void;
}

const AddCertificateModal: React.FC<AddCertificateModalProps> = ({ worker, onClose }) => {
  const [addWorkerCertificate, { isLoading }] = useAddWorkerCertificateMutation();
  const { data: certTypes = [] } = useGetCertificateTypesQuery();

  const [certificateTypeId, setCertificateTypeId] = useState('');
  const [certificateNumber, setCertificateNumber] = useState('');
  const [issuingBody, setIssuingBody] = useState('National Safety Council');
  const [issueDate, setIssueDate] = useState(new Date().toISOString().split('T')[0]);
  const [expiryDate, setExpiryDate] = useState(() => {
    const d = new Date();
    d.setFullYear(d.getFullYear() + 1);
    return d.toISOString().split('T')[0];
  });
  const [error, setError] = useState<string | null>(null);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError(null);

    if (!certificateTypeId || !certificateNumber.trim() || !issuingBody.trim() || !issueDate || !expiryDate) {
      setError('Please fill in all required fields.');
      return;
    }

    try {
      const payload: CreateCertificateRequest = {
        certificateTypeId,
        certificateNumber: certificateNumber.trim().toUpperCase(),
        issuingBody: issuingBody.trim(),
        issueDate: new Date(issueDate).toISOString(),
        expiryDate: new Date(expiryDate).toISOString(),
      };
      await addWorkerCertificate({ workerId: worker.id, body: payload }).unwrap();
      onClose();
    } catch (err: any) {
      setError(err?.data?.message || err?.message || 'Failed to add certificate.');
    }
  };

  return (
    <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/70 backdrop-blur-sm p-4">
      <div className="bg-white dark:bg-slate-900 border border-slate-200 dark:border-slate-700 rounded-3xl shadow-2xl w-full max-w-lg overflow-hidden">
        <div className="flex items-center justify-between p-5 border-b border-slate-200 dark:border-slate-800">
          <div className="flex items-center gap-2">
            <Award className="w-5 h-5 text-amber-500" />
            <div>
              <h2 className="text-base font-black text-slate-900 dark:text-white">Add Accreditation</h2>
              <p className="text-[11px] text-slate-500 dark:text-slate-400">
                For {worker.firstName} {worker.lastName} ({worker.badgeNumber})
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
              Certificate Type <span className="text-rose-500">*</span>
            </label>
            <select
              required
              value={certificateTypeId}
              onChange={(e) => setCertificateTypeId(e.target.value)}
              className="w-full bg-slate-50 dark:bg-slate-950 border border-slate-300 dark:border-slate-700 rounded-xl px-3 py-2 text-xs text-slate-900 dark:text-white focus:outline-none focus:border-amber-500"
            >
              <option value="">Select Certificate Type...</option>
              {certTypes.map((ct) => (
                <option key={ct.id} value={ct.id}>
                  {ct.name} ({ct.code})
                </option>
              ))}
            </select>
          </div>

          <div className="grid grid-cols-2 gap-3">
            <div>
              <label className="text-xs font-semibold text-slate-700 dark:text-slate-300 block mb-1">
                Certificate # <span className="text-rose-500">*</span>
              </label>
              <input
                type="text"
                required
                value={certificateNumber}
                onChange={(e) => setCertificateNumber(e.target.value)}
                placeholder="e.g. HW-2024-998"
                className="w-full font-mono bg-slate-50 dark:bg-slate-950 border border-slate-300 dark:border-slate-700 rounded-xl px-3 py-2 text-xs text-slate-900 dark:text-white focus:outline-none focus:border-amber-500"
              />
            </div>
            <div>
              <label className="text-xs font-semibold text-slate-700 dark:text-slate-300 block mb-1">
                Issuing Authority <span className="text-rose-500">*</span>
              </label>
              <input
                type="text"
                required
                value={issuingBody}
                onChange={(e) => setIssuingBody(e.target.value)}
                placeholder="e.g. National Safety Authority"
                className="w-full bg-slate-50 dark:bg-slate-950 border border-slate-300 dark:border-slate-700 rounded-xl px-3 py-2 text-xs text-slate-900 dark:text-white focus:outline-none focus:border-amber-500"
              />
            </div>
          </div>

          <div className="grid grid-cols-2 gap-3">
            <div>
              <label className="text-xs font-semibold text-slate-700 dark:text-slate-300 block mb-1">
                Issue Date <span className="text-rose-500">*</span>
              </label>
              <input
                type="date"
                required
                value={issueDate}
                onChange={(e) => setIssueDate(e.target.value)}
                className="w-full bg-slate-50 dark:bg-slate-950 border border-slate-300 dark:border-slate-700 rounded-xl px-3 py-2 text-xs text-slate-900 dark:text-white focus:outline-none focus:border-amber-500"
              />
            </div>
            <div>
              <label className="text-xs font-semibold text-slate-700 dark:text-slate-300 block mb-1">
                Expiry Date <span className="text-rose-500">*</span>
              </label>
              <input
                type="date"
                required
                value={expiryDate}
                onChange={(e) => setExpiryDate(e.target.value)}
                className="w-full bg-slate-50 dark:bg-slate-950 border border-slate-300 dark:border-slate-700 rounded-xl px-3 py-2 text-xs text-slate-900 dark:text-white focus:outline-none focus:border-amber-500"
              />
            </div>
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
              <Award className="w-3.5 h-3.5" />
              {isLoading ? 'Saving...' : 'Add Certificate'}
            </button>
          </div>
        </form>
      </div>
    </div>
  );
};

// ─── Main Workforce Page ─────────────────────────────────────────────────────
export const WorkforcePage: React.FC = () => {
  const { t } = useTranslation();
  const currentUser = useSelector((state: RootState) => state.auth.user);
  const [searchTerm, setSearchTerm] = useState('');

  // Modals state
  const [showAddWorker, setShowAddWorker] = useState(false);
  const [editingWorker, setEditingWorker] = useState<Worker | null>(null);
  const [certTargetWorker, setCertTargetWorker] = useState<Worker | null>(null);

  // Queries & Mutations
  const { data: workers = [] } = useGetWorkersQuery();
  const { data: forecast = [], isLoading: isLoadingForecast } = useGetExpiryForecastQuery();
  const [deleteWorker] = useDeleteWorkerMutation();
  const [deleteCertificate] = useDeleteWorkerCertificateMutation();

  // Role Permissions:
  const canManageWorkers =
    currentUser?.role === 'Administrator' ||
    currentUser?.role === 'AreaSupervisor' ||
    currentUser?.role === 'ContractorSupervisor';
  const canDeleteWorkers =
    currentUser?.role === 'Administrator' || currentUser?.role === 'AreaSupervisor';
  const canManageCerts =
    currentUser?.role === 'Administrator' ||
    currentUser?.role === 'AreaSupervisor' ||
    currentUser?.role === 'SafetyOfficer' ||
    currentUser?.role === 'ContractorSupervisor';
  const canDeleteCerts =
    currentUser?.role === 'Administrator' ||
    currentUser?.role === 'AreaSupervisor' ||
    currentUser?.role === 'SafetyOfficer';

  const handleDeleteWorker = async (w: Worker) => {
    if (window.confirm(`Are you sure you want to delete worker ${w.firstName} ${w.lastName} (${w.badgeNumber})?`)) {
      try {
        await deleteWorker(w.id).unwrap();
      } catch (err: any) {
        alert(err?.data?.message || 'Failed to delete worker.');
      }
    }
  };

  const handleDeleteCertificate = async (certId: string, certName: string) => {
    if (window.confirm(`Remove certificate "${certName}" from worker record?`)) {
      try {
        await deleteCertificate(certId).unwrap();
      } catch (err: any) {
        alert(err?.data?.message || 'Failed to delete certificate.');
      }
    }
  };

  const filteredWorkers = workers.filter(
    (w) =>
      `${w.firstName} ${w.lastName}`.toLowerCase().includes(searchTerm.toLowerCase()) ||
      w.badgeNumber.toLowerCase().includes(searchTerm.toLowerCase()) ||
      w.trade.toLowerCase().includes(searchTerm.toLowerCase()) ||
      w.contractorName.toLowerCase().includes(searchTerm.toLowerCase())
  );

  return (
    <div className="space-y-6 max-w-6xl mx-auto">
      {/* Modals */}
      {showAddWorker && <AddWorkerModal onClose={() => setShowAddWorker(false)} />}
      {editingWorker && (
        <EditWorkerModal worker={editingWorker} onClose={() => setEditingWorker(null)} />
      )}
      {certTargetWorker && (
        <AddCertificateModal
          worker={certTargetWorker}
          onClose={() => setCertTargetWorker(null)}
        />
      )}

      {/* Header */}
      <div className="flex flex-wrap items-center justify-between gap-4">
        <div>
          <h1 className="text-2xl font-black text-slate-900 dark:text-slate-100 tracking-tight flex items-center gap-2">
            <Users2 className="w-6 h-6 text-amber-500 dark:text-amber-400" />
            <span>{t('workforce_title')}</span>
          </h1>
          <p className="text-xs text-slate-500 dark:text-slate-400 mt-1">
            {t('workforce_subtitle')}
          </p>
        </div>

        {canManageWorkers && (
          <button
            onClick={() => setShowAddWorker(true)}
            className="flex items-center gap-1.5 px-4 py-2 bg-amber-500 hover:bg-amber-400 text-slate-950 rounded-xl text-xs font-bold transition shadow-sm"
          >
            <Plus className="w-3.5 h-3.5" />
            <span>Add Worker</span>
          </button>
        )}
      </div>

      {/* 30-Day Proactive Expiry Forecast Alert Card */}
      <div className="bg-amber-50 dark:bg-amber-950/20 border border-amber-300 dark:border-amber-500/40 rounded-2xl p-5 shadow-sm">
        <div className="flex items-center gap-2 text-amber-700 dark:text-amber-400 font-bold text-sm mb-2">
          <CalendarClock className="w-5 h-5 shrink-0" />
          <span>{t('workforce_forecast_title')}</span>
        </div>
        <p className="text-xs text-slate-600 dark:text-slate-300 mb-4">
          {t('workforce_forecast_desc')}
        </p>

        {isLoadingForecast ? (
          <div className="text-xs text-slate-500 dark:text-slate-400 py-4 text-center">
            Checking certification expiries...
          </div>
        ) : forecast.length === 0 ? (
          <div className="text-xs text-emerald-600 dark:text-emerald-400 py-2 font-medium">
            {t('workforce_forecast_valid_all')}
          </div>
        ) : (
          <div className="overflow-x-auto bg-white dark:bg-slate-950/60 rounded-xl border border-slate-200 dark:border-slate-800">
            <table className="w-full text-left text-xs">
              <thead>
                <tr className="border-b border-slate-200 dark:border-slate-800 text-slate-500 dark:text-slate-400 uppercase text-[10px] font-semibold bg-slate-50 dark:bg-slate-900/50">
                  <th className="py-2.5 px-3">{t('workforce_th_name')}</th>
                  <th className="py-2.5 px-3">{t('workforce_th_badge')}</th>
                  <th className="py-2.5 px-3">{t('workforce_th_cert')}</th>
                  <th className="py-2.5 px-3">{t('workforce_th_expiry')}</th>
                  <th className="py-2.5 px-3">{t('workforce_th_remaining')}</th>
                  <th className="py-2.5 px-3">{t('workforce_th_status')}</th>
                </tr>
              </thead>
              <tbody className="divide-y divide-slate-200 dark:divide-slate-800/60 font-mono">
                {forecast.map((item, idx) => (
                  <tr key={idx} className="hover:bg-slate-50 dark:hover:bg-slate-900/40 transition-colors">
                    <td className="py-2.5 px-3 font-sans font-semibold text-slate-900 dark:text-slate-200">
                      {item.workerName}
                    </td>
                    <td className="py-2.5 px-3 text-amber-600 dark:text-amber-400 font-bold">
                      {item.badgeNumber}
                    </td>
                    <td className="py-2.5 px-3 text-slate-700 dark:text-slate-300 font-sans">
                      {item.certificateName}
                    </td>
                    <td className="py-2.5 px-3 text-slate-500 dark:text-slate-400">
                      {new Date(item.expiryDate).toLocaleDateString()}
                    </td>
                    <td className="py-2.5 px-3 font-bold">
                      <span
                        className={
                          item.daysRemaining < 0
                            ? 'text-rose-600 dark:text-rose-400'
                            : 'text-amber-600 dark:text-amber-400'
                        }
                      >
                        {item.daysRemaining < 0
                          ? `${-item.daysRemaining} days overdue`
                          : `${item.daysRemaining} days`}
                      </span>
                    </td>
                    <td className="py-2.5 px-3">
                      <span
                        className={`px-2 py-0.5 rounded text-[10px] uppercase font-bold border ${
                          item.daysRemaining < 0
                            ? 'bg-rose-100 dark:bg-rose-950 text-rose-700 dark:text-rose-400 border-rose-300 dark:border-rose-800'
                            : 'bg-amber-100 dark:bg-amber-950 text-amber-700 dark:text-amber-400 border-amber-300 dark:border-amber-800'
                        }`}
                      >
                        {item.daysRemaining < 0
                          ? t('workforce_status_expired')
                          : t('workforce_status_expiring')}
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
      <div className="bg-white dark:bg-slate-900 p-4 rounded-2xl border border-slate-200 dark:border-slate-800 flex items-center justify-between shadow-sm">
        <div className="w-full max-w-md relative">
          <Search className="w-4 h-4 text-slate-400 absolute left-3 top-1/2 -translate-y-1/2" />
          <input
            type="text"
            placeholder={t('workforce_search_placeholder')}
            value={searchTerm}
            onChange={(e) => setSearchTerm(e.target.value)}
            className="w-full pl-9 pr-4 py-2.5 bg-slate-50 dark:bg-slate-950 border border-slate-300 dark:border-slate-700 rounded-xl text-xs text-slate-900 dark:text-white placeholder-slate-400 dark:placeholder-slate-500 focus:outline-none focus:ring-1 focus:ring-amber-500 transition-colors"
          />
        </div>
      </div>

      {/* Workers Grid */}
      <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
        {filteredWorkers.map((w) => (
          <div
            key={w.id}
            className="bg-white dark:bg-slate-900 border border-slate-200 dark:border-slate-800 rounded-2xl p-5 shadow-sm space-y-3 flex flex-col justify-between"
          >
            <div>
              <div className="flex items-start justify-between gap-2">
                <div>
                  <div className="flex items-center gap-2">
                    <h3 className="font-bold text-slate-900 dark:text-slate-100 text-sm">
                      {w.firstName} {w.lastName}
                    </h3>
                    {w.isActive ? (
                      <span className="inline-flex items-center gap-0.5 text-[10px] text-emerald-600 dark:text-emerald-400 font-semibold">
                        <UserCheck className="w-3 h-3" /> Active
                      </span>
                    ) : (
                      <span className="inline-flex items-center gap-0.5 text-[10px] text-rose-500 font-semibold">
                        <UserX className="w-3 h-3" /> Inactive
                      </span>
                    )}
                  </div>
                  <p className="text-xs text-slate-500 dark:text-slate-400 mt-0.5">
                    {w.trade} · {w.contractorName}
                  </p>
                </div>

                <div className="flex items-center gap-1.5 shrink-0">
                  <span className="font-mono text-xs bg-slate-100 dark:bg-slate-950 text-amber-600 dark:text-amber-400 px-2.5 py-1 rounded-lg border border-slate-200 dark:border-slate-800 font-bold">
                    {w.badgeNumber}
                  </span>

                  {canManageWorkers && (
                    <button
                      onClick={() => setEditingWorker(w)}
                      title="Edit Worker"
                      className="p-1.5 text-slate-400 hover:text-amber-500 hover:bg-slate-100 dark:hover:bg-slate-800 rounded-lg transition"
                    >
                      <Pencil className="w-3.5 h-3.5" />
                    </button>
                  )}

                  {canDeleteWorkers && (
                    <button
                      onClick={() => handleDeleteWorker(w)}
                      title="Delete Worker"
                      className="p-1.5 text-slate-400 hover:text-rose-500 hover:bg-slate-100 dark:hover:bg-slate-800 rounded-lg transition"
                    >
                      <Trash2 className="w-3.5 h-3.5" />
                    </button>
                  )}
                </div>
              </div>

              {/* Certificates Section */}
              <div className="space-y-1.5 pt-3 mt-3 border-t border-slate-200 dark:border-slate-800/80">
                <div className="flex items-center justify-between text-[11px] font-semibold text-slate-500 dark:text-slate-400 uppercase tracking-wider">
                  <span>{t('workforce_accredited_certs')}</span>
                  {canManageCerts && (
                    <button
                      onClick={() => setCertTargetWorker(w)}
                      className="inline-flex items-center gap-1 text-[10px] text-amber-600 dark:text-amber-400 hover:underline capitalize font-bold"
                    >
                      <Plus className="w-3 h-3" /> Add Cert
                    </button>
                  )}
                </div>

                {w.certificates.length === 0 ? (
                  <div className="text-xs text-slate-400 dark:text-slate-500 italic py-1">
                    {t('workforce_no_certs')}
                  </div>
                ) : (
                  w.certificates.map((cert) => (
                    <div
                      key={cert.id}
                      className="flex items-center justify-between text-xs bg-slate-50 dark:bg-slate-950 p-2.5 rounded-xl border border-slate-200 dark:border-slate-800/80 group"
                    >
                      <div className="truncate pr-2">
                        <div className="font-medium text-slate-800 dark:text-slate-300 truncate">
                          {cert.certificateName}
                        </div>
                        <div className="text-[10px] text-slate-500 dark:text-slate-500 font-mono">
                          {cert.certificateNumber} · Exp: {new Date(cert.expiryDate).toLocaleDateString()}
                        </div>
                      </div>
                      <div className="shrink-0 flex items-center gap-2">
                        {cert.status === 'Valid' && cert.daysUntilExpiry > 0 ? (
                          <span className="flex items-center gap-1 text-emerald-600 dark:text-emerald-400 text-[11px] font-mono font-bold">
                            <CheckCircle2 className="w-3.5 h-3.5" /> {t('workforce_status_valid')}
                          </span>
                        ) : (
                          <span className="flex items-center gap-1 text-rose-600 dark:text-rose-400 text-[11px] font-mono font-bold">
                            <XCircle className="w-3.5 h-3.5" /> {t('workforce_status_expired')}
                          </span>
                        )}

                        {canDeleteCerts && (
                          <button
                            onClick={() => handleDeleteCertificate(cert.id, cert.certificateName)}
                            title="Remove Certificate"
                            className="opacity-0 group-hover:opacity-100 p-1 text-slate-400 hover:text-rose-500 transition"
                          >
                            <Trash2 className="w-3 h-3" />
                          </button>
                        )}
                      </div>
                    </div>
                  ))
                )}
              </div>
            </div>
          </div>
        ))}
      </div>
    </div>
  );
};