import React, { useState } from 'react';
import { useSelector } from 'react-redux';
import type { RootState } from '../store';
import {
  useGetHazardTypesQuery,
  useCreateHazardTypeMutation,
  useUpdateHazardTypeMutation,
  useDeleteHazardTypeMutation,
  useCreateControlMeasureMutation,
  useUpdateControlMeasureMutation,
  useDeleteControlMeasureMutation,
} from '../store/apiSlice';
import {
  BookOpen,
  Plus,
  Pencil,
  Trash2,
  ShieldCheck,
  Wind,
  CloudRain,
  Search,
  X,
  AlertCircle,
  CheckCircle2,
  ChevronDown,
  ChevronUp,
  Sliders,
  Layers,
} from 'lucide-react';
import { Link } from 'react-router-dom';
import type {
  HazardType,
  ControlMeasure,
  CreateHazardTypeRequest,
  UpdateHazardTypeRequest,
  CreateControlMeasureRequest,
  UpdateControlMeasureRequest,
} from '../types';

// ─── Severity Color Helper ───────────────────────────────────────────────────
const getSeverityBadgeClass = (severity: string) => {
  switch (severity?.toLowerCase()) {
    case 'critical':
      return 'bg-rose-500/10 text-rose-500 border-rose-500/30';
    case 'high':
      return 'bg-orange-500/10 text-orange-500 border-orange-500/30';
    case 'medium':
      return 'bg-amber-500/10 text-amber-500 border-amber-500/30';
    case 'low':
    default:
      return 'bg-emerald-500/10 text-emerald-500 border-emerald-500/30';
  }
};

// ─── Modal: Add / Edit Hazard Type ──────────────────────────────────────────
interface HazardTypeModalProps {
  hazardType?: HazardType | null;
  onClose: () => void;
}

const HazardTypeModal: React.FC<HazardTypeModalProps> = ({ hazardType, onClose }) => {
  const [createHazardType, { isLoading: isCreating }] = useCreateHazardTypeMutation();
  const [updateHazardType, { isLoading: isUpdating }] = useUpdateHazardTypeMutation();

  const isEdit = Boolean(hazardType);
  const [code, setCode] = useState(hazardType?.code || '');
  const [name, setName] = useState(hazardType?.name || '');
  const [severityLevel, setSeverityLevel] = useState<string>(hazardType?.severityLevel || 'High');
  const [maxWindSpeed, setMaxWindSpeed] = useState<string>(
    hazardType?.maxWindSpeedKmh != null ? String(hazardType.maxWindSpeedKmh) : ''
  );
  const [prohibitedInRain, setProhibitedInRain] = useState<boolean>(hazardType?.prohibitedInRain ?? true);
  const [error, setError] = useState<string | null>(null);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError(null);

    if (!code.trim() && !isEdit) {
      setError('Hazard code is required.');
      return;
    }
    if (!name.trim()) {
      setError('Hazard name is required.');
      return;
    }

    try {
      const windVal = maxWindSpeed.trim() === '' ? null : parseFloat(maxWindSpeed);
      if (windVal != null && (isNaN(windVal) || windVal < 0)) {
        setError('Max wind speed must be a valid non-negative number.');
        return;
      }

      if (isEdit && hazardType) {
        const payload: UpdateHazardTypeRequest = {
          name: name.trim(),
          severityLevel,
          maxWindSpeedKmh: windVal,
          prohibitedInRain,
        };
        await updateHazardType({ id: hazardType.id, body: payload }).unwrap();
      } else {
        const payload: CreateHazardTypeRequest = {
          code: code.trim().toUpperCase().replace(/\s+/g, '_'),
          name: name.trim(),
          severityLevel,
          maxWindSpeedKmh: windVal,
          prohibitedInRain,
        };
        await createHazardType(payload).unwrap();
      }
      onClose();
    } catch (err: any) {
      setError(err?.data?.message || err?.message || 'Failed to save hazard type.');
    }
  };

  const isLoading = isCreating || isUpdating;

  return (
    <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/70 backdrop-blur-sm p-4 animate-in fade-in duration-150">
      <div className="bg-white dark:bg-slate-900 border border-slate-200 dark:border-slate-800 rounded-2xl p-6 w-full max-w-lg shadow-2xl space-y-5">
        <div className="flex items-center justify-between border-b border-slate-200 dark:border-slate-800 pb-3">
          <div className="flex items-center gap-2 text-slate-900 dark:text-slate-100 font-bold text-lg">
            <BookOpen className="w-5 h-5 text-amber-500" />
            <span>{isEdit ? 'Edit Hazard Type' : 'Create New Hazard Type'}</span>
          </div>
          <button
            onClick={onClose}
            className="p-1 rounded-lg text-slate-400 hover:text-slate-600 dark:hover:text-slate-200 hover:bg-slate-100 dark:hover:bg-slate-800 transition-colors"
          >
            <X className="w-5 h-5" />
          </button>
        </div>

        {error && (
          <div className="p-3 bg-rose-500/10 border border-rose-500/30 rounded-xl text-rose-500 text-xs flex items-center gap-2">
            <AlertCircle className="w-4 h-4 shrink-0" />
            <span>{error}</span>
          </div>
        )}

        <form onSubmit={handleSubmit} className="space-y-4 text-xs">
          <div>
            <label className="block text-slate-600 dark:text-slate-400 font-medium mb-1">
              Hazard Code <span className="text-rose-500">*</span>
            </label>
            <input
              type="text"
              value={code}
              disabled={isEdit}
              onChange={(e) => setCode(e.target.value)}
              placeholder="e.g. HOT_WORK, CRANE_LIFT, CONFINED_SPACE"
              className="w-full bg-slate-50 dark:bg-slate-950 border border-slate-200 dark:border-slate-800 rounded-xl px-3 py-2 text-slate-900 dark:text-slate-100 font-mono focus:outline-none focus:ring-2 focus:ring-amber-500/50 disabled:opacity-50"
            />
            {isEdit && <span className="text-[10px] text-slate-400 mt-1 block">Code cannot be changed once created.</span>}
          </div>

          <div>
            <label className="block text-slate-600 dark:text-slate-400 font-medium mb-1">
              Hazard Name / Description <span className="text-rose-500">*</span>
            </label>
            <input
              type="text"
              value={name}
              onChange={(e) => setName(e.target.value)}
              placeholder="e.g. Hot Work (Welding, Grinding, Open Flame)"
              className="w-full bg-slate-50 dark:bg-slate-950 border border-slate-200 dark:border-slate-800 rounded-xl px-3 py-2 text-slate-900 dark:text-slate-100 focus:outline-none focus:ring-2 focus:ring-amber-500/50"
            />
          </div>

          <div className="grid grid-cols-1 sm:grid-cols-2 gap-4">
            <div>
              <label className="block text-slate-600 dark:text-slate-400 font-medium mb-1">
                Severity Level <span className="text-rose-500">*</span>
              </label>
              <select
                value={severityLevel}
                onChange={(e) => setSeverityLevel(e.target.value)}
                className="w-full bg-slate-50 dark:bg-slate-950 border border-slate-200 dark:border-slate-800 rounded-xl px-3 py-2 text-slate-900 dark:text-slate-100 focus:outline-none focus:ring-2 focus:ring-amber-500/50"
              >
                <option value="Low">Low</option>
                <option value="Medium">Medium</option>
                <option value="High">High</option>
                <option value="Critical">Critical</option>
              </select>
            </div>

            <div>
              <label className="block text-slate-600 dark:text-slate-400 font-medium mb-1">
                Max Wind Speed (km/h)
              </label>
              <input
                type="number"
                step="0.1"
                min="0"
                value={maxWindSpeed}
                onChange={(e) => setMaxWindSpeed(e.target.value)}
                placeholder="Optional (e.g. 35.0)"
                className="w-full bg-slate-50 dark:bg-slate-950 border border-slate-200 dark:border-slate-800 rounded-xl px-3 py-2 text-slate-900 dark:text-slate-100 focus:outline-none focus:ring-2 focus:ring-amber-500/50"
              />
            </div>
          </div>

          <div className="p-3 bg-slate-50 dark:bg-slate-950/60 rounded-xl border border-slate-200 dark:border-slate-800 flex items-center justify-between">
            <div>
              <div className="font-semibold text-slate-800 dark:text-slate-200">Prohibited During Rain</div>
              <div className="text-[11px] text-slate-500 dark:text-slate-400">
                Automatically fail-close AI permits when precipitation is detected
              </div>
            </div>
            <label className="relative inline-flex items-center cursor-pointer">
              <input
                type="checkbox"
                checked={prohibitedInRain}
                onChange={(e) => setProhibitedInRain(e.target.checked)}
                className="sr-only peer"
              />
              <div className="w-9 h-5 bg-slate-300 peer-focus:outline-none rounded-full peer dark:bg-slate-700 peer-checked:after:translate-x-full peer-checked:after:border-white after:content-[''] after:absolute after:top-[2px] after:left-[2px] after:bg-white after:border-slate-300 after:border after:rounded-full after:h-4 after:w-4 after:transition-all peer-checked:bg-amber-500"></div>
            </label>
          </div>

          <div className="flex justify-end gap-3 pt-3 border-t border-slate-200 dark:border-slate-800">
            <button
              type="button"
              onClick={onClose}
              className="px-4 py-2 rounded-xl text-slate-600 dark:text-slate-400 hover:bg-slate-100 dark:hover:bg-slate-800 font-medium transition-colors"
            >
              Cancel
            </button>
            <button
              type="submit"
              disabled={isLoading}
              className="px-5 py-2 rounded-xl bg-amber-500 hover:bg-amber-600 text-slate-950 font-bold flex items-center gap-1.5 shadow-md shadow-amber-500/20 disabled:opacity-50 transition-colors"
            >
              {isLoading ? 'Saving...' : isEdit ? 'Update Hazard' : 'Create Hazard'}
            </button>
          </div>
        </form>
      </div>
    </div>
  );
};

// ─── Modal: Add / Edit Control Measure ──────────────────────────────────────
interface ControlMeasureModalProps {
  hazardType: HazardType;
  controlMeasure?: ControlMeasure | null;
  onClose: () => void;
}

const ControlMeasureModal: React.FC<ControlMeasureModalProps> = ({
  hazardType,
  controlMeasure,
  onClose,
}) => {
  const [createControlMeasure, { isLoading: isCreating }] = useCreateControlMeasureMutation();
  const [updateControlMeasure, { isLoading: isUpdating }] = useUpdateControlMeasureMutation();

  const isEdit = Boolean(controlMeasure);
  const [code, setCode] = useState(controlMeasure?.code || '');
  const [requirementDescription, setRequirementDescription] = useState(
    controlMeasure?.requirementDescription || ''
  );
  const [isMandatory, setIsMandatory] = useState<boolean>(controlMeasure?.isMandatory ?? true);
  const [error, setError] = useState<string | null>(null);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError(null);

    if (!code.trim() && !isEdit) {
      setError('Control measure code is required.');
      return;
    }
    if (!requirementDescription.trim()) {
      setError('Requirement description is required.');
      return;
    }

    try {
      if (isEdit && controlMeasure) {
        const payload: UpdateControlMeasureRequest = {
          requirementDescription: requirementDescription.trim(),
          isMandatory,
        };
        await updateControlMeasure({ id: controlMeasure.id, body: payload }).unwrap();
      } else {
        const payload: CreateControlMeasureRequest = {
          hazardTypeId: hazardType.id,
          code: code.trim().toUpperCase(),
          requirementDescription: requirementDescription.trim(),
          isMandatory,
        };
        await createControlMeasure(payload).unwrap();
      }
      onClose();
    } catch (err: any) {
      setError(err?.data?.message || err?.message || 'Failed to save control measure.');
    }
  };

  const isLoading = isCreating || isUpdating;

  return (
    <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/70 backdrop-blur-sm p-4 animate-in fade-in duration-150">
      <div className="bg-white dark:bg-slate-900 border border-slate-200 dark:border-slate-800 rounded-2xl p-6 w-full max-w-md shadow-2xl space-y-5">
        <div className="flex items-center justify-between border-b border-slate-200 dark:border-slate-800 pb-3">
          <div className="flex items-center gap-2 text-slate-900 dark:text-slate-100 font-bold text-lg">
            <ShieldCheck className="w-5 h-5 text-amber-500" />
            <span>{isEdit ? 'Edit Control Measure' : `Add Control for ${hazardType.code}`}</span>
          </div>
          <button
            onClick={onClose}
            className="p-1 rounded-lg text-slate-400 hover:text-slate-600 dark:hover:text-slate-200 hover:bg-slate-100 dark:hover:bg-slate-800 transition-colors"
          >
            <X className="w-5 h-5" />
          </button>
        </div>

        {error && (
          <div className="p-3 bg-rose-500/10 border border-rose-500/30 rounded-xl text-rose-500 text-xs flex items-center gap-2">
            <AlertCircle className="w-4 h-4 shrink-0" />
            <span>{error}</span>
          </div>
        )}

        <form onSubmit={handleSubmit} className="space-y-4 text-xs">
          <div>
            <label className="block text-slate-600 dark:text-slate-400 font-medium mb-1">
              Measure Code <span className="text-rose-500">*</span>
            </label>
            <input
              type="text"
              value={code}
              disabled={isEdit}
              onChange={(e) => setCode(e.target.value)}
              placeholder="e.g. CM-HW-01, PPE-GAS-01"
              className="w-full bg-slate-50 dark:bg-slate-950 border border-slate-200 dark:border-slate-800 rounded-xl px-3 py-2 text-slate-900 dark:text-slate-100 font-mono focus:outline-none focus:ring-2 focus:ring-amber-500/50 disabled:opacity-50"
            />
          </div>

          <div>
            <label className="block text-slate-600 dark:text-slate-400 font-medium mb-1">
              Requirement Description <span className="text-rose-500">*</span>
            </label>
            <textarea
              rows={3}
              value={requirementDescription}
              onChange={(e) => setRequirementDescription(e.target.value)}
              placeholder="e.g. Calibrated 4-gas atmospheric monitor active during entry."
              className="w-full bg-slate-50 dark:bg-slate-950 border border-slate-200 dark:border-slate-800 rounded-xl px-3 py-2 text-slate-900 dark:text-slate-100 focus:outline-none focus:ring-2 focus:ring-amber-500/50"
            />
          </div>

          <div className="p-3 bg-slate-50 dark:bg-slate-950/60 rounded-xl border border-slate-200 dark:border-slate-800 flex items-center justify-between">
            <div>
              <div className="font-semibold text-slate-800 dark:text-slate-200">Mandatory Control</div>
              <div className="text-[11px] text-slate-500 dark:text-slate-400">
                Permit issuance strictly refuses if not acknowledged
              </div>
            </div>
            <label className="relative inline-flex items-center cursor-pointer">
              <input
                type="checkbox"
                checked={isMandatory}
                onChange={(e) => setIsMandatory(e.target.checked)}
                className="sr-only peer"
              />
              <div className="w-9 h-5 bg-slate-300 peer-focus:outline-none rounded-full peer dark:bg-slate-700 peer-checked:after:translate-x-full peer-checked:after:border-white after:content-[''] after:absolute after:top-[2px] after:left-[2px] after:bg-white after:border-slate-300 after:border after:rounded-full after:h-4 after:w-4 after:transition-all peer-checked:bg-amber-500"></div>
            </label>
          </div>

          <div className="flex justify-end gap-3 pt-3 border-t border-slate-200 dark:border-slate-800">
            <button
              type="button"
              onClick={onClose}
              className="px-4 py-2 rounded-xl text-slate-600 dark:text-slate-400 hover:bg-slate-100 dark:hover:bg-slate-800 font-medium transition-colors"
            >
              Cancel
            </button>
            <button
              type="submit"
              disabled={isLoading}
              className="px-5 py-2 rounded-xl bg-amber-500 hover:bg-amber-600 text-slate-950 font-bold flex items-center gap-1.5 shadow-md shadow-amber-500/20 disabled:opacity-50 transition-colors"
            >
              {isLoading ? 'Saving...' : isEdit ? 'Update Measure' : 'Add Measure'}
            </button>
          </div>
        </form>
      </div>
    </div>
  );
};

// ─── Main Rulebook Editor Page ───────────────────────────────────────────────
export const RulebookEditorPage: React.FC = () => {
  const user = useSelector((state: RootState) => state.auth.user);
  const { data: hazardTypes = [], isLoading, isError } = useGetHazardTypesQuery();
  const [deleteHazardType] = useDeleteHazardTypeMutation();
  const [deleteControlMeasure] = useDeleteControlMeasureMutation();

  const [searchQuery, setSearchQuery] = useState('');
  const [selectedSeverity, setSelectedSeverity] = useState('ALL');
  const [expandedHazardIds, setExpandedHazardIds] = useState<Record<string, boolean>>({});

  // Modals state
  const [hazardModalOpen, setHazardModalOpen] = useState(false);
  const [editingHazard, setEditingHazard] = useState<HazardType | null>(null);

  const [controlModalOpen, setControlModalOpen] = useState(false);
  const [targetHazardForControl, setTargetHazardForControl] = useState<HazardType | null>(null);
  const [editingControl, setEditingControl] = useState<ControlMeasure | null>(null);

  // Notification message
  const [actionMessage, setActionMessage] = useState<{ type: 'success' | 'error'; text: string } | null>(null);

  const canEdit =
    user?.role === 'Administrator' ||
    user?.role === 'SafetyOfficer' ||
    user?.role === 'AreaSupervisor';

  const canDelete = user?.role === 'Administrator' || user?.role === 'SafetyOfficer';

  const toggleExpand = (id: string) => {
    setExpandedHazardIds((prev) => ({
      ...prev,
      [id]: !prev[id],
    }));
  };

  const handleDeleteHazard = async (id: string, name: string) => {
    if (!window.confirm(`Are you sure you want to delete Hazard Type "${name}"? This will remove all associated control measures.`)) {
      return;
    }
    try {
      await deleteHazardType(id).unwrap();
      setActionMessage({ type: 'success', text: `Hazard Type "${name}" deleted successfully.` });
      setTimeout(() => setActionMessage(null), 4000);
    } catch (err: any) {
      setActionMessage({
        type: 'error',
        text: err?.data?.message || err?.message || 'Failed to delete hazard type.',
      });
    }
  };

  const handleDeleteControl = async (id: string, code: string) => {
    if (!window.confirm(`Are you sure you want to delete Control Measure "${code}"?`)) {
      return;
    }
    try {
      await deleteControlMeasure(id).unwrap();
      setActionMessage({ type: 'success', text: `Control Measure "${code}" deleted successfully.` });
      setTimeout(() => setActionMessage(null), 4000);
    } catch (err: any) {
      setActionMessage({
        type: 'error',
        text: err?.data?.message || err?.message || 'Failed to delete control measure.',
      });
    }
  };

  // Filter hazards
  const filteredHazards = hazardTypes.filter((h) => {
    const matchesSearch =
      h.name.toLowerCase().includes(searchQuery.toLowerCase()) ||
      h.code.toLowerCase().includes(searchQuery.toLowerCase()) ||
      h.controlMeasures?.some((cm) =>
        cm.requirementDescription.toLowerCase().includes(searchQuery.toLowerCase()) ||
        cm.code.toLowerCase().includes(searchQuery.toLowerCase())
      );
    const matchesSeverity = selectedSeverity === 'ALL' || h.severityLevel === selectedSeverity;
    return matchesSearch && matchesSeverity;
  });

  const totalControls = hazardTypes.reduce((acc, curr) => acc + (curr.controlMeasures?.length || 0), 0);
  const criticalHazardsCount = hazardTypes.filter((h) => h.severityLevel === 'Critical').length;
  const rainRestrictedCount = hazardTypes.filter((h) => h.prohibitedInRain).length;

  return (
    <div className="space-y-6 max-w-6xl mx-auto pb-12">
      {/* Breadcrumb & Navigation Tabs */}
      <div className="flex flex-wrap items-center justify-between gap-4 pb-2 border-b border-slate-200 dark:border-slate-800">
        <div className="flex items-center gap-2 text-xs font-medium text-slate-500 dark:text-slate-400">
          <Link to="/hazard-rules" className="hover:text-amber-500 transition-colors">
            Hazard Zones & Observations
          </Link>
          <span>/</span>
          <span className="text-slate-900 dark:text-slate-100 font-bold">Rulebook Editor</span>
        </div>

        <div className="flex items-center gap-2">
          <Link
            to="/hazard-rules/matrix"
            className="px-3 py-1.5 bg-slate-100 dark:bg-slate-800 hover:bg-slate-200 dark:hover:bg-slate-700 text-slate-700 dark:text-slate-200 text-xs font-semibold rounded-xl flex items-center gap-1.5 transition-colors"
          >
            <Sliders className="w-3.5 h-3.5 text-amber-500" />
            <span>Open Conflict Matrix</span>
          </Link>
          <Link
            to="/hazard-rules"
            className="px-3 py-1.5 bg-slate-100 dark:bg-slate-800 hover:bg-slate-200 dark:hover:bg-slate-700 text-slate-700 dark:text-slate-200 text-xs font-semibold rounded-xl flex items-center gap-1.5 transition-colors"
          >
            <Layers className="w-3.5 h-3.5 text-sky-500" />
            <span>Zones & Observations</span>
          </Link>
        </div>
      </div>

      {/* Header */}
      <div className="flex flex-col sm:flex-row sm:items-center justify-between gap-4">
        <div>
          <h1 className="text-2xl font-black text-slate-900 dark:text-slate-100 tracking-tight flex items-center gap-2">
            <BookOpen className="w-6 h-6 text-amber-500 dark:text-amber-400" />
            <span>HSE Rulebook & Control Measures Editor</span>
          </h1>
          <p className="text-xs text-slate-500 dark:text-slate-400 mt-1">
            Authoritative registry of industrial hazard classes, atmospheric weather envelope thresholds, and mandatory safety controls.
          </p>
        </div>

        {canEdit && (
          <button
            onClick={() => {
              setEditingHazard(null);
              setHazardModalOpen(true);
            }}
            className="px-4 py-2.5 bg-amber-500 hover:bg-amber-600 text-slate-950 font-bold text-xs rounded-xl flex items-center gap-2 shadow-lg shadow-amber-500/20 transition-all shrink-0"
          >
            <Plus className="w-4 h-4" />
            <span>Add Hazard Type</span>
          </button>
        )}
      </div>

      {/* Action Notification Alert */}
      {actionMessage && (
        <div
          className={`p-3.5 rounded-xl text-xs flex items-center gap-2.5 border ${
            actionMessage.type === 'success'
              ? 'bg-emerald-500/10 border-emerald-500/30 text-emerald-600 dark:text-emerald-400'
              : 'bg-rose-500/10 border-rose-500/30 text-rose-600 dark:text-rose-400'
          }`}
        >
          {actionMessage.type === 'success' ? (
            <CheckCircle2 className="w-4 h-4 shrink-0" />
          ) : (
            <AlertCircle className="w-4 h-4 shrink-0" />
          )}
          <span>{actionMessage.text}</span>
        </div>
      )}

      {/* Stats Cards */}
      <div className="grid grid-cols-2 sm:grid-cols-4 gap-3">
        <div className="bg-white dark:bg-slate-900 border border-slate-200 dark:border-slate-800 p-4 rounded-2xl shadow-sm">
          <div className="text-[11px] font-bold text-slate-500 uppercase tracking-wider">Hazard Types</div>
          <div className="text-2xl font-black text-slate-900 dark:text-slate-100 mt-1">
            {hazardTypes.length}
          </div>
          <div className="text-[10px] text-slate-400 mt-0.5">Defined categories</div>
        </div>

        <div className="bg-white dark:bg-slate-900 border border-slate-200 dark:border-slate-800 p-4 rounded-2xl shadow-sm">
          <div className="text-[11px] font-bold text-slate-500 uppercase tracking-wider">Control Measures</div>
          <div className="text-2xl font-black text-amber-500 mt-1">{totalControls}</div>
          <div className="text-[10px] text-slate-400 mt-0.5">Mandatory & specific</div>
        </div>

        <div className="bg-white dark:bg-slate-900 border border-slate-200 dark:border-slate-800 p-4 rounded-2xl shadow-sm">
          <div className="text-[11px] font-bold text-slate-500 uppercase tracking-wider">Critical Severity</div>
          <div className="text-2xl font-black text-rose-500 mt-1">{criticalHazardsCount}</div>
          <div className="text-[10px] text-slate-400 mt-0.5">Highest consequence</div>
        </div>

        <div className="bg-white dark:bg-slate-900 border border-slate-200 dark:border-slate-800 p-4 rounded-2xl shadow-sm">
          <div className="text-[11px] font-bold text-slate-500 uppercase tracking-wider">Rain Restricted</div>
          <div className="text-2xl font-black text-sky-500 mt-1">{rainRestrictedCount}</div>
          <div className="text-[10px] text-slate-400 mt-0.5">Weather fail-close</div>
        </div>
      </div>

      {/* Search & Filter Bar */}
      <div className="bg-white dark:bg-slate-900 border border-slate-200 dark:border-slate-800 p-4 rounded-2xl shadow-sm flex flex-col sm:flex-row items-center gap-3">
        <div className="relative flex-1 w-full">
          <Search className="w-4 h-4 absolute left-3 top-1/2 -translate-y-1/2 text-slate-400" />
          <input
            type="text"
            value={searchQuery}
            onChange={(e) => setSearchQuery(e.target.value)}
            placeholder="Search hazard types, codes, or control measure descriptions..."
            className="w-full bg-slate-50 dark:bg-slate-950 border border-slate-200 dark:border-slate-800 rounded-xl pl-9 pr-4 py-2 text-xs text-slate-900 dark:text-slate-100 focus:outline-none focus:ring-2 focus:ring-amber-500/50"
          />
        </div>

        <div className="flex items-center gap-2 w-full sm:w-auto">
          <span className="text-xs text-slate-500 font-medium whitespace-nowrap">Severity:</span>
          <select
            value={selectedSeverity}
            onChange={(e) => setSelectedSeverity(e.target.value)}
            className="bg-slate-50 dark:bg-slate-950 border border-slate-200 dark:border-slate-800 rounded-xl px-3 py-2 text-xs text-slate-900 dark:text-slate-100 focus:outline-none focus:ring-2 focus:ring-amber-500/50 w-full sm:w-auto"
          >
            <option value="ALL">All Severities</option>
            <option value="Low">Low</option>
            <option value="Medium">Medium</option>
            <option value="High">High</option>
            <option value="Critical">Critical</option>
          </select>
        </div>
      </div>

      {/* Hazard Types List */}
      {isLoading ? (
        <div className="py-16 text-center space-y-3">
          <div className="w-8 h-8 border-3 border-amber-500 border-t-transparent rounded-full animate-spin mx-auto" />
          <p className="text-xs text-slate-500 dark:text-slate-400 font-medium">Loading hazard rulebook registry...</p>
        </div>
      ) : isError ? (
        <div className="p-6 bg-rose-500/10 border border-rose-500/30 rounded-2xl text-center space-y-2">
          <AlertCircle className="w-8 h-8 text-rose-500 mx-auto" />
          <h3 className="font-bold text-slate-900 dark:text-slate-100 text-sm">Failed to Load Hazard Rulebook</h3>
          <p className="text-xs text-slate-500 dark:text-slate-400">Please check backend connectivity and retry.</p>
        </div>
      ) : filteredHazards.length === 0 ? (
        <div className="bg-white dark:bg-slate-900 border border-slate-200 dark:border-slate-800 p-12 rounded-2xl text-center space-y-3">
          <BookOpen className="w-10 h-10 text-slate-400 mx-auto opacity-60" />
          <h3 className="font-bold text-slate-900 dark:text-slate-100 text-sm">No Hazard Types Found</h3>
          <p className="text-xs text-slate-500 dark:text-slate-400 max-w-sm mx-auto">
            {searchQuery || selectedSeverity !== 'ALL'
              ? 'No hazard types match the specified search query or severity filter.'
              : 'The hazard rulebook is empty. Click "Add Hazard Type" to define the first safety envelope.'}
          </p>
        </div>
      ) : (
        <div className="space-y-4">
          {filteredHazards.map((hazard) => {
            const isExpanded = expandedHazardIds[hazard.id] !== false; // Default expanded
            const controlCount = hazard.controlMeasures?.length || 0;

            return (
              <div
                key={hazard.id}
                className="bg-white dark:bg-slate-900 border border-slate-200 dark:border-slate-800 rounded-2xl overflow-hidden shadow-sm transition-all"
              >
                {/* Hazard Type Card Header */}
                <div className="p-5 flex flex-col md:flex-row md:items-center justify-between gap-4 border-b border-slate-100 dark:border-slate-800/60">
                  <div className="flex items-start gap-3.5">
                    <button
                      onClick={() => toggleExpand(hazard.id)}
                      className="p-1 rounded-lg hover:bg-slate-100 dark:hover:bg-slate-800 text-slate-400 hover:text-slate-600 dark:hover:text-slate-200 transition-colors mt-0.5"
                    >
                      {isExpanded ? <ChevronUp className="w-5 h-5" /> : <ChevronDown className="w-5 h-5" />}
                    </button>

                    <div>
                      <div className="flex flex-wrap items-center gap-2">
                        <span className="font-mono font-bold text-xs bg-slate-100 dark:bg-slate-800 text-slate-900 dark:text-slate-100 px-2.5 py-1 rounded-lg border border-slate-200 dark:border-slate-700">
                          {hazard.code}
                        </span>
                        <h2 className="font-bold text-slate-900 dark:text-slate-100 text-sm">{hazard.name}</h2>
                        <span
                          className={`text-[10px] font-bold uppercase tracking-wider px-2 py-0.5 rounded-md border ${getSeverityBadgeClass(
                            hazard.severityLevel
                          )}`}
                        >
                          {hazard.severityLevel} Severity
                        </span>
                      </div>

                      {/* Weather limits tags */}
                      <div className="flex flex-wrap items-center gap-3 text-xs text-slate-500 dark:text-slate-400 mt-2">
                        <div className="flex items-center gap-1">
                          <Wind className="w-3.5 h-3.5 text-amber-500" />
                          <span>
                            Max Wind:{' '}
                            <strong className="text-slate-700 dark:text-slate-300 font-mono">
                              {hazard.maxWindSpeedKmh != null ? `${hazard.maxWindSpeedKmh.toFixed(1)} km/h` : 'No Limit'}
                            </strong>
                          </span>
                        </div>

                        <div className="flex items-center gap-1">
                          <CloudRain className="w-3.5 h-3.5 text-sky-500" />
                          <span>
                            Rain Restriction:{' '}
                            <strong
                              className={
                                hazard.prohibitedInRain
                                  ? 'text-rose-600 dark:text-rose-400 font-semibold'
                                  : 'text-emerald-600 dark:text-emerald-400 font-semibold'
                              }
                            >
                              {hazard.prohibitedInRain ? 'Strictly Prohibited' : 'Permitted with Precautions'}
                            </strong>
                          </span>
                        </div>

                        <div className="flex items-center gap-1 text-[11px]">
                          <ShieldCheck className="w-3.5 h-3.5 text-amber-500" />
                          <span>{controlCount} Control {controlCount === 1 ? 'Measure' : 'Measures'}</span>
                        </div>
                      </div>
                    </div>
                  </div>

                  {/* Hazard Type Action Buttons */}
                  <div className="flex items-center gap-2 self-end md:self-center shrink-0">
                    {canEdit && (
                      <button
                        onClick={() => {
                          setTargetHazardForControl(hazard);
                          setEditingControl(null);
                          setControlModalOpen(true);
                        }}
                        className="px-3 py-1.5 bg-amber-500/10 hover:bg-amber-500/20 text-amber-600 dark:text-amber-400 border border-amber-500/30 text-xs font-semibold rounded-xl flex items-center gap-1.5 transition-colors"
                      >
                        <Plus className="w-3.5 h-3.5" />
                        <span>Add Control</span>
                      </button>
                    )}

                    {canEdit && (
                      <button
                        onClick={() => {
                          setEditingHazard(hazard);
                          setHazardModalOpen(true);
                        }}
                        title="Edit Hazard Type"
                        className="p-1.5 text-slate-500 hover:text-slate-800 dark:hover:text-slate-200 hover:bg-slate-100 dark:hover:bg-slate-800 rounded-lg transition-colors"
                      >
                        <Pencil className="w-4 h-4" />
                      </button>
                    )}

                    {canDelete && (
                      <button
                        onClick={() => handleDeleteHazard(hazard.id, hazard.name)}
                        title="Delete Hazard Type"
                        className="p-1.5 text-rose-500 hover:text-rose-700 hover:bg-rose-50 dark:hover:bg-rose-950/40 rounded-lg transition-colors"
                      >
                        <Trash2 className="w-4 h-4" />
                      </button>
                    )}
                  </div>
                </div>

                {/* Collapsible Control Measures Section */}
                {isExpanded && (
                  <div className="p-4 bg-slate-50/60 dark:bg-slate-950/40">
                    <div className="text-[11px] font-bold uppercase tracking-wider text-slate-500 dark:text-slate-400 mb-2 flex items-center justify-between">
                      <span>Prescribed Control Measures & Safeguards</span>
                    </div>

                    {!hazard.controlMeasures || hazard.controlMeasures.length === 0 ? (
                      <div className="py-6 text-center text-xs text-slate-400 dark:text-slate-500 bg-white dark:bg-slate-900 border border-dashed border-slate-200 dark:border-slate-800 rounded-xl">
                        No specific control measures registered for this hazard type yet.
                        {canEdit && (
                          <div className="mt-2">
                            <button
                              onClick={() => {
                                setTargetHazardForControl(hazard);
                                setEditingControl(null);
                                setControlModalOpen(true);
                              }}
                              className="text-amber-500 font-semibold hover:underline"
                            >
                              + Add first control measure
                            </button>
                          </div>
                        )}
                      </div>
                    ) : (
                      <div className="space-y-2">
                        {hazard.controlMeasures.map((measure) => (
                          <div
                            key={measure.id}
                            className="bg-white dark:bg-slate-900 border border-slate-200 dark:border-slate-800 rounded-xl p-3 flex items-start justify-between gap-3 shadow-xs"
                          >
                            <div className="flex items-start gap-2.5">
                              <ShieldCheck className="w-4 h-4 text-emerald-500 shrink-0 mt-0.5" />
                              <div>
                                <div className="flex items-center gap-2">
                                  <span className="font-mono text-xs font-bold text-slate-800 dark:text-slate-200">
                                    {measure.code}
                                  </span>
                                  {measure.isMandatory ? (
                                    <span className="bg-rose-500/10 text-rose-500 border border-rose-500/30 text-[9px] font-bold px-1.5 py-0.5 rounded">
                                      MANDATORY
                                    </span>
                                  ) : (
                                    <span className="bg-slate-100 dark:bg-slate-800 text-slate-500 text-[9px] font-semibold px-1.5 py-0.5 rounded">
                                      RECOMMENDED
                                    </span>
                                  )}
                                </div>
                                <p className="text-xs text-slate-600 dark:text-slate-300 mt-1 leading-relaxed">
                                  {measure.requirementDescription}
                                </p>
                              </div>
                            </div>

                            {/* Control Measure Actions */}
                            <div className="flex items-center gap-1 shrink-0">
                              {canEdit && (
                                <button
                                  onClick={() => {
                                    setTargetHazardForControl(hazard);
                                    setEditingControl(measure);
                                    setControlModalOpen(true);
                                  }}
                                  title="Edit Control Measure"
                                  className="p-1 text-slate-400 hover:text-slate-700 dark:hover:text-slate-200 hover:bg-slate-100 dark:hover:bg-slate-800 rounded-lg transition-colors"
                                >
                                  <Pencil className="w-3.5 h-3.5" />
                                </button>
                              )}

                              {canDelete && (
                                <button
                                  onClick={() => handleDeleteControl(measure.id, measure.code)}
                                  title="Delete Control Measure"
                                  className="p-1 text-rose-400 hover:text-rose-600 hover:bg-rose-50 dark:hover:bg-rose-950/40 rounded-lg transition-colors"
                                >
                                  <Trash2 className="w-3.5 h-3.5" />
                                </button>
                              )}
                            </div>
                          </div>
                        ))}
                      </div>
                    )}
                  </div>
                )}
              </div>
            );
          })}
        </div>
      )}

      {/* Hazard Type Modal */}
      {hazardModalOpen && (
        <HazardTypeModal
          hazardType={editingHazard}
          onClose={() => {
            setHazardModalOpen(false);
            setEditingHazard(null);
          }}
        />
      )}

      {/* Control Measure Modal */}
      {controlModalOpen && targetHazardForControl && (
        <ControlMeasureModal
          hazardType={targetHazardForControl}
          controlMeasure={editingControl}
          onClose={() => {
            setControlModalOpen(false);
            setTargetHazardForControl(null);
            setEditingControl(null);
          }}
        />
      )}
    </div>
  );
};

export default RulebookEditorPage;
