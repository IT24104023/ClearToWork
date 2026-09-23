import React, { useState } from 'react';
import { useSelector } from 'react-redux';
import type { RootState } from '../store';
import {
  useGetZonesQuery,
  useGetObservationsQuery,
  useCreateObservationMutation,
  useUpdateObservationMutation,
  useDeleteObservationMutation,
} from '../store/apiSlice';
import { useTranslation } from '../context/I18nContext';
import {
  AlertTriangle,
  Wind,
  ShieldAlert,
  Layers,
  BookOpen,
  Sliders,
  Plus,
  Pencil,
  Trash2,
  Search,
  Eye,
  CheckCircle2,
  AlertCircle,
  X,
  ArrowUpRight,
  User,
  Clock,
} from 'lucide-react';
import { Link } from 'react-router-dom';
import type {
  Observation,
  CreateObservationRequest,
  UpdateObservationRequest,
} from '../types';

// ─── Category Badge Helper ───────────────────────────────────────────────────
const getCategoryBadgeClass = (category: string) => {
  switch (category) {
    case 'NearMiss':
      return 'bg-orange-500/10 text-orange-600 dark:text-orange-400 border-orange-500/30';
    case 'Hazard':
      return 'bg-rose-500/10 text-rose-600 dark:text-rose-400 border-rose-500/30';
    case 'UnsafeAct':
      return 'bg-amber-500/10 text-amber-600 dark:text-amber-400 border-amber-500/30';
    case 'UnsafeCondition':
      return 'bg-purple-500/10 text-purple-600 dark:text-purple-400 border-purple-500/30';
    case 'Positive':
      return 'bg-emerald-500/10 text-emerald-600 dark:text-emerald-400 border-emerald-500/30';
    default:
      return 'bg-slate-100 text-slate-700 border-slate-300';
  }
};

// ─── Modal: Log / Edit Safety Observation ────────────────────────────────────
interface ObservationModalProps {
  observation?: Observation | null;
  onClose: () => void;
}

const ObservationModal: React.FC<ObservationModalProps> = ({ observation, onClose }) => {
  const [createObservation, { isLoading: isCreating }] = useCreateObservationMutation();
  const [updateObservation, { isLoading: isUpdating }] = useUpdateObservationMutation();
  const { data: zones = [] } = useGetZonesQuery();

  const isEdit = Boolean(observation);
  const [zoneId, setZoneId] = useState(observation?.zoneId || (zones[0]?.id || ''));
  const [category, setCategory] = useState<string>(observation?.category || 'Hazard');
  const [description, setDescription] = useState(observation?.description || '');
  const [error, setError] = useState<string | null>(null);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError(null);

    if (!zoneId) {
      setError('Please select a plant zone.');
      return;
    }
    if (!description.trim()) {
      setError('Description of the safety observation is required.');
      return;
    }

    try {
      if (isEdit && observation) {
        const payload: UpdateObservationRequest = {
          zoneId,
          category,
          description: description.trim(),
        };
        await updateObservation({ id: observation.id, body: payload }).unwrap();
      } else {
        const payload: CreateObservationRequest = {
          zoneId,
          category,
          description: description.trim(),
        };
        await createObservation(payload).unwrap();
      }
      onClose();
    } catch (err: any) {
      setError(err?.data?.message || err?.message || 'Failed to submit safety observation.');
    }
  };

  const isLoading = isCreating || isUpdating;

  return (
    <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/70 backdrop-blur-sm p-4 animate-in fade-in duration-150">
      <div className="bg-white dark:bg-slate-900 border border-slate-200 dark:border-slate-800 rounded-2xl p-6 w-full max-w-lg shadow-2xl space-y-5">
        <div className="flex items-center justify-between border-b border-slate-200 dark:border-slate-800 pb-3">
          <div className="flex items-center gap-2 text-slate-900 dark:text-slate-100 font-bold text-lg">
            <Eye className="w-5 h-5 text-amber-500" />
            <span>{isEdit ? 'Edit Safety Observation' : 'Log Safety Observation'}</span>
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
              Plant Zone <span className="text-rose-500">*</span>
            </label>
            <select
              value={zoneId}
              onChange={(e) => setZoneId(e.target.value)}
              className="w-full bg-slate-50 dark:bg-slate-950 border border-slate-200 dark:border-slate-800 rounded-xl px-3 py-2 text-slate-900 dark:text-slate-100 focus:outline-none focus:ring-2 focus:ring-amber-500/50"
            >
              <option value="">Select Plant Zone</option>
              {zones.map((z) => (
                <option key={z.id} value={z.id}>
                  {z.code} — {z.name}
                </option>
              ))}
            </select>
          </div>

          <div>
            <label className="block text-slate-600 dark:text-slate-400 font-medium mb-1">
              Observation Category <span className="text-rose-500">*</span>
            </label>
            <select
              value={category}
              onChange={(e) => setCategory(e.target.value)}
              className="w-full bg-slate-50 dark:bg-slate-950 border border-slate-200 dark:border-slate-800 rounded-xl px-3 py-2 text-slate-900 dark:text-slate-100 focus:outline-none focus:ring-2 focus:ring-amber-500/50"
            >
              <option value="Hazard">Hazard (Physical hazard identified)</option>
              <option value="NearMiss">Near Miss (Incident narrowly avoided)</option>
              <option value="UnsafeAct">Unsafe Act (Non-compliant behavior)</option>
              <option value="UnsafeCondition">Unsafe Condition (Defective equipment/environment)</option>
              <option value="Positive">Positive Practice (Exemplary safety compliance)</option>
            </select>
          </div>

          <div>
            <label className="block text-slate-600 dark:text-slate-400 font-medium mb-1">
              Observation Details & Evidence <span className="text-rose-500">*</span>
            </label>
            <textarea
              rows={4}
              value={description}
              onChange={(e) => setDescription(e.target.value)}
              placeholder="Describe the condition, location, personnel involved, and immediate containment measures taken..."
              className="w-full bg-slate-50 dark:bg-slate-950 border border-slate-200 dark:border-slate-800 rounded-xl px-3 py-2 text-slate-900 dark:text-slate-100 focus:outline-none focus:ring-2 focus:ring-amber-500/50"
            />
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
              {isLoading ? 'Saving...' : isEdit ? 'Update Observation' : 'Log Observation'}
            </button>
          </div>
        </form>
      </div>
    </div>
  );
};

// ─── Main HazardRulesPage Component ──────────────────────────────────────────
export const HazardRulesPage: React.FC = () => {
  const { t } = useTranslation();
  const user = useSelector((state: RootState) => state.auth.user);

  const { data: zones = [], isLoading: zonesLoading } = useGetZonesQuery();
  const [selectedCategory, setSelectedCategory] = useState('ALL');
  const [selectedZoneId, setSelectedZoneId] = useState<string>('');
  const [searchQuery, setSearchQuery] = useState('');

  const {
    data: observations = [],
    isLoading: obsLoading,
    isError: obsError,
  } = useGetObservationsQuery({
    zoneId: selectedZoneId || undefined,
    category: selectedCategory !== 'ALL' ? selectedCategory : undefined,
  });

  const [deleteObservation] = useDeleteObservationMutation();

  // Modals & State
  const [observationModalOpen, setObservationModalOpen] = useState(false);
  const [editingObservation, setEditingObservation] = useState<Observation | null>(null);
  const [actionMessage, setActionMessage] = useState<{ type: 'success' | 'error'; text: string } | null>(null);

  const canLogObservation = Boolean(user);
  const canDeleteObservation =
    user?.role === 'Administrator' ||
    user?.role === 'SafetyOfficer' ||
    user?.role === 'AreaSupervisor';

  const handleDeleteObservation = async (id: string) => {
    if (!window.confirm('Are you sure you want to delete this safety observation?')) {
      return;
    }
    try {
      await deleteObservation(id).unwrap();
      setActionMessage({ type: 'success', text: 'Observation deleted successfully.' });
      setTimeout(() => setActionMessage(null), 4000);
    } catch (err: any) {
      setActionMessage({
        type: 'error',
        text: err?.data?.message || err?.message || 'Failed to delete observation.',
      });
    }
  };

  const filteredObservations = observations.filter((o) => {
    const q = searchQuery.toLowerCase();
    return (
      o.description.toLowerCase().includes(q) ||
      o.reporterName.toLowerCase().includes(q) ||
      o.zoneCode.toLowerCase().includes(q) ||
      o.zoneName.toLowerCase().includes(q) ||
      o.category.toLowerCase().includes(q)
    );
  });

  return (
    <div className="space-y-6 max-w-6xl mx-auto pb-12">
      {/* Header */}
      <div className="flex flex-col sm:flex-row sm:items-center justify-between gap-4">
        <div>
          <h1 className="text-2xl font-black text-slate-900 dark:text-slate-100 tracking-tight flex items-center gap-2">
            <AlertTriangle className="w-6 h-6 text-amber-500 dark:text-amber-400" />
            <span>{t('hazard_title')}</span>
          </h1>
          <p className="text-xs text-slate-500 dark:text-slate-400 mt-1">
            {t('hazard_subtitle')}
          </p>
        </div>

        <div className="flex items-center gap-2 shrink-0">
          {canLogObservation && (
            <button
              onClick={() => {
                setEditingObservation(null);
                setObservationModalOpen(true);
              }}
              className="px-4 py-2.5 bg-amber-500 hover:bg-amber-600 text-slate-950 font-bold text-xs rounded-xl flex items-center gap-2 shadow-lg shadow-amber-500/20 transition-all"
            >
              <Plus className="w-4 h-4" />
              <span>Log Observation</span>
            </button>
          )}
        </div>
      </div>

      {/* Quick Launch Management Cards for Student 4 Editors */}
      <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
        <Link
          to="/hazard-rules/rulebook"
          className="group bg-gradient-to-br from-amber-500/10 via-amber-500/5 to-transparent border border-amber-500/20 hover:border-amber-500/40 rounded-2xl p-5 shadow-sm hover:shadow-md transition-all flex items-start justify-between"
        >
          <div className="space-y-2">
            <div className="flex items-center gap-2 text-amber-600 dark:text-amber-400 font-bold text-sm">
              <BookOpen className="w-5 h-5" />
              <span>Rulebook & Control Measures Editor</span>
            </div>
            <p className="text-xs text-slate-600 dark:text-slate-400 leading-relaxed max-w-md">
              Configure hazard classes (Hot Work, Confined Space, Lifting), weather envelope speed caps, and mandatory mitigation safeguards.
            </p>
            <div className="text-[11px] font-bold text-amber-600 dark:text-amber-400 flex items-center gap-1 group-hover:translate-x-1 transition-transform">
              <span>Open Rulebook Manager</span>
              <ArrowUpRight className="w-3.5 h-3.5" />
            </div>
          </div>
        </Link>

        <Link
          to="/hazard-rules/matrix"
          className="group bg-gradient-to-br from-rose-500/10 via-rose-500/5 to-transparent border border-rose-500/20 hover:border-rose-500/40 rounded-2xl p-5 shadow-sm hover:shadow-md transition-all flex items-start justify-between"
        >
          <div className="space-y-2">
            <div className="flex items-center gap-2 text-rose-600 dark:text-rose-400 font-bold text-sm">
              <Sliders className="w-5 h-5" />
              <span>SIMOPS Incompatibility & Matrix Editor</span>
            </div>
            <p className="text-xs text-slate-600 dark:text-slate-400 leading-relaxed max-w-md">
              Maintain the 2D collision matrix between incompatible operations (Welding vs Solvent Painting) and link spatial plant zone boundaries.
            </p>
            <div className="text-[11px] font-bold text-rose-600 dark:text-rose-400 flex items-center gap-1 group-hover:translate-x-1 transition-transform">
              <span>Open SIMOPS Matrix</span>
              <ArrowUpRight className="w-3.5 h-3.5" />
            </div>
          </div>
        </Link>
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

      {/* Section 1: Safety Observations Register */}
      <div className="bg-white dark:bg-slate-900 border border-slate-200 dark:border-slate-800 rounded-2xl p-6 shadow-sm space-y-5">
        <div className="flex flex-col sm:flex-row sm:items-center justify-between gap-4 pb-4 border-b border-slate-100 dark:border-slate-800">
          <div>
            <h2 className="font-bold text-slate-900 dark:text-slate-100 text-base flex items-center gap-2">
              <Eye className="w-5 h-5 text-amber-500" />
              <span>Field Safety Observations & Near-Miss Register</span>
            </h2>
            <p className="text-xs text-slate-500 dark:text-slate-400 mt-0.5">
              Live safety observations reported across industrial zones informing AI clearance risks.
            </p>
          </div>

          {canLogObservation && (
            <button
              onClick={() => {
                setEditingObservation(null);
                setObservationModalOpen(true);
              }}
              className="px-3 py-1.5 bg-amber-500/10 hover:bg-amber-500/20 text-amber-600 dark:text-amber-400 border border-amber-500/30 text-xs font-semibold rounded-xl flex items-center gap-1.5 transition-colors shrink-0"
            >
              <Plus className="w-3.5 h-3.5" />
              <span>New Observation</span>
            </button>
          )}
        </div>

        {/* Filter controls */}
        <div className="flex flex-col sm:flex-row items-center gap-3">
          <div className="relative flex-1 w-full">
            <Search className="w-4 h-4 absolute left-3 top-1/2 -translate-y-1/2 text-slate-400" />
            <input
              type="text"
              value={searchQuery}
              onChange={(e) => setSearchQuery(e.target.value)}
              placeholder="Search observations by reporter, zone, or text..."
              className="w-full bg-slate-50 dark:bg-slate-950 border border-slate-200 dark:border-slate-800 rounded-xl pl-9 pr-4 py-2 text-xs text-slate-900 dark:text-slate-100 focus:outline-none focus:ring-2 focus:ring-amber-500/50"
            />
          </div>

          <div className="flex flex-wrap items-center gap-2 w-full sm:w-auto">
            <select
              value={selectedZoneId}
              onChange={(e) => setSelectedZoneId(e.target.value)}
              className="bg-slate-50 dark:bg-slate-950 border border-slate-200 dark:border-slate-800 rounded-xl px-3 py-2 text-xs text-slate-900 dark:text-slate-100 focus:outline-none focus:ring-2 focus:ring-amber-500/50"
            >
              <option value="">All Zones</option>
              {zones.map((z) => (
                <option key={z.id} value={z.id}>
                  {z.code} ({z.name})
                </option>
              ))}
            </select>

            <select
              value={selectedCategory}
              onChange={(e) => setSelectedCategory(e.target.value)}
              className="bg-slate-50 dark:bg-slate-950 border border-slate-200 dark:border-slate-800 rounded-xl px-3 py-2 text-xs text-slate-900 dark:text-slate-100 focus:outline-none focus:ring-2 focus:ring-amber-500/50"
            >
              <option value="ALL">All Categories</option>
              <option value="Hazard">Hazard</option>
              <option value="NearMiss">Near Miss</option>
              <option value="UnsafeAct">Unsafe Act</option>
              <option value="UnsafeCondition">Unsafe Condition</option>
              <option value="Positive">Positive Practice</option>
            </select>
          </div>
        </div>

        {/* Observations list */}
        {obsLoading ? (
          <div className="py-12 text-center text-xs text-slate-500">Loading safety observations...</div>
        ) : obsError ? (
          <div className="p-4 bg-rose-500/10 border border-rose-500/30 rounded-xl text-xs text-rose-500 text-center">
            Failed to load safety observations.
          </div>
        ) : filteredObservations.length === 0 ? (
          <div className="py-10 text-center text-xs text-slate-400 bg-slate-50 dark:bg-slate-950 rounded-xl border border-dashed border-slate-200 dark:border-slate-800 space-y-1">
            <Eye className="w-8 h-8 text-slate-400 mx-auto opacity-50 mb-1" />
            <div className="font-semibold text-slate-700 dark:text-slate-300">No safety observations logged yet</div>
            <p className="text-[11px] text-slate-500">Observations reported here feed directly into the multi-agent AI safety review.</p>
          </div>
        ) : (
          <div className="space-y-3">
            {filteredObservations.map((obs) => (
              <div
                key={obs.id}
                className="bg-slate-50 dark:bg-slate-950 p-4 rounded-xl border border-slate-200 dark:border-slate-800 space-y-3 shadow-xs hover:border-slate-300 dark:hover:border-slate-700 transition-colors"
              >
                <div className="flex flex-col sm:flex-row sm:items-center justify-between gap-2">
                  <div className="flex flex-wrap items-center gap-2">
                    <span
                      className={`text-[10px] font-bold uppercase tracking-wider px-2 py-0.5 rounded border ${getCategoryBadgeClass(
                        obs.category
                      )}`}
                    >
                      {obs.category}
                    </span>
                    <span className="font-mono text-xs font-bold text-slate-800 dark:text-slate-200 bg-white dark:bg-slate-900 px-2 py-0.5 rounded border border-slate-200 dark:border-slate-800">
                      {obs.zoneCode} — {obs.zoneName}
                    </span>
                  </div>

                  <div className="flex items-center gap-3 text-slate-500 dark:text-slate-400 text-[11px]">
                    <div className="flex items-center gap-1">
                      <User className="w-3.5 h-3.5" />
                      <span>{obs.reporterName || 'Field Reporter'}</span>
                    </div>
                    <div className="flex items-center gap-1">
                      <Clock className="w-3.5 h-3.5" />
                      <span>{new Date(obs.loggedAt || obs.createdAt).toLocaleDateString()}</span>
                    </div>

                    <div className="flex items-center gap-1 ml-2">
                      {canLogObservation && (
                        <button
                          onClick={() => {
                            setEditingObservation(obs);
                            setObservationModalOpen(true);
                          }}
                          title="Edit Observation"
                          className="p-1 text-slate-400 hover:text-slate-700 dark:hover:text-slate-200 hover:bg-slate-200 dark:hover:bg-slate-800 rounded transition-colors"
                        >
                          <Pencil className="w-3.5 h-3.5" />
                        </button>
                      )}

                      {canDeleteObservation && (
                        <button
                          onClick={() => handleDeleteObservation(obs.id)}
                          title="Delete Observation"
                          className="p-1 text-rose-400 hover:text-rose-600 hover:bg-rose-50 dark:hover:bg-rose-950/40 rounded transition-colors"
                        >
                          <Trash2 className="w-3.5 h-3.5" />
                        </button>
                      )}
                    </div>
                  </div>
                </div>

                <p className="text-xs text-slate-700 dark:text-slate-300 leading-relaxed bg-white dark:bg-slate-900 p-3 rounded-lg border border-slate-200 dark:border-slate-800/60">
                  {obs.description}
                </p>
              </div>
            ))}
          </div>
        )}
      </div>

      {/* Rule HR-07 Incompatibility Overview Card */}
      <div className="bg-white dark:bg-slate-900 border border-slate-200 dark:border-slate-800 rounded-2xl p-6 shadow-sm space-y-4">
        <div className="flex items-center justify-between pb-3 border-b border-slate-200 dark:border-slate-800">
          <div className="flex items-center gap-2 text-rose-600 dark:text-rose-400 font-bold text-base">
            <ShieldAlert className="w-5 h-5" />
            <span>{t('hazard_simops_title')}</span>
          </div>

          <Link
            to="/hazard-rules/matrix"
            className="text-xs text-rose-600 dark:text-rose-400 font-bold hover:underline flex items-center gap-1"
          >
            <span>Manage All SIMOPS Rules</span>
            <ArrowUpRight className="w-3.5 h-3.5" />
          </Link>
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
        <div className="flex items-center justify-between pb-3 border-b border-slate-200 dark:border-slate-800">
          <div className="flex items-center gap-2 text-amber-600 dark:text-amber-400 font-bold text-base">
            <Wind className="w-5 h-5" />
            <span>{t('hazard_weather_title')}</span>
          </div>

          <Link
            to="/hazard-rules/rulebook"
            className="text-xs text-amber-600 dark:text-amber-400 font-bold hover:underline flex items-center gap-1"
          >
            <span>Edit Thresholds</span>
            <ArrowUpRight className="w-3.5 h-3.5" />
          </Link>
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
        <div className="flex items-center justify-between pb-3 border-b border-slate-200 dark:border-slate-800">
          <div className="flex items-center gap-2 text-slate-900 dark:text-slate-200 font-bold text-base">
            <Layers className="w-5 h-5 text-amber-500 dark:text-amber-400" />
            <span>{t('hazard_zones_title')}</span>
          </div>

          <Link
            to="/hazard-rules/matrix"
            className="text-xs text-sky-600 dark:text-sky-400 font-bold hover:underline flex items-center gap-1"
          >
            <span>Edit Spatial Graph</span>
            <ArrowUpRight className="w-3.5 h-3.5" />
          </Link>
        </div>

        {zonesLoading ? (
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

      {/* Observation Modal */}
      {observationModalOpen && (
        <ObservationModal
          observation={editingObservation}
          onClose={() => {
            setObservationModalOpen(false);
            setEditingObservation(null);
          }}
        />
      )}
    </div>
  );
};

export default HazardRulesPage;