import React, { useState } from 'react';
import { useSelector } from 'react-redux';
import type { RootState } from '../store';
import {
  useGetIncompatibilityRulesQuery,
  useCreateIncompatibilityRuleMutation,
  useUpdateIncompatibilityRuleMutation,
  useDeleteIncompatibilityRuleMutation,
  useGetHazardTypesQuery,
  useGetZonesQuery,
  useGetZoneAdjacenciesQuery,
  useAddZoneAdjacencyMutation,
  useRemoveZoneAdjacencyMutation,
} from '../store/apiSlice';
import {
  Sliders,
  Plus,
  Pencil,
  Trash2,
  ShieldAlert,
  Layers,
  Search,
  X,
  AlertCircle,
  CheckCircle2,
  BookOpen,
  Link as LinkIcon,
  Unlink,
  Grid,
  Info,
  ShieldX,
  ArrowRight,
} from 'lucide-react';
import { Link } from 'react-router-dom';
import type {
  IncompatibilityRule,
  CreateIncompatibilityRuleRequest,
  UpdateIncompatibilityRuleRequest,
  AddZoneAdjacencyRequest,
} from '../types';

// ─── Modal: Add / Edit Incompatibility Rule ──────────────────────────────────
interface IncompatibilityRuleModalProps {
  rule?: IncompatibilityRule | null;
  onClose: () => void;
}

const IncompatibilityRuleModal: React.FC<IncompatibilityRuleModalProps> = ({ rule, onClose }) => {
  const [createRule, { isLoading: isCreating }] = useCreateIncompatibilityRuleMutation();
  const [updateRule, { isLoading: isUpdating }] = useUpdateIncompatibilityRuleMutation();
  const { data: hazardTypes = [] } = useGetHazardTypesQuery();

  const isEdit = Boolean(rule);
  const [ruleCode, setRuleCode] = useState(rule?.ruleCode || '');
  const [primaryHazardId, setPrimaryHazardId] = useState(rule?.primaryHazardId || '');
  const [conflictingHazardId, setConflictingHazardId] = useState(rule?.conflictingHazardId || '');
  const [reason, setReason] = useState(rule?.reason || '');
  const [appliesToAdjacentZones, setAppliesToAdjacentZones] = useState<boolean>(
    rule?.appliesToAdjacentZones ?? true
  );
  const [error, setError] = useState<string | null>(null);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError(null);

    if (!ruleCode.trim() && !isEdit) {
      setError('Rule code is required (e.g. HR-07).');
      return;
    }
    if (!isEdit && (!primaryHazardId || !conflictingHazardId)) {
      setError('Please select both primary and conflicting hazard types.');
      return;
    }
    if (!isEdit && primaryHazardId === conflictingHazardId) {
      setError('Primary and conflicting hazard types must be different.');
      return;
    }
    if (!reason.trim()) {
      setError('A technical justification reason is required.');
      return;
    }

    try {
      if (isEdit && rule) {
        const payload: UpdateIncompatibilityRuleRequest = {
          reason: reason.trim(),
          appliesToAdjacentZones,
        };
        await updateRule({ id: rule.id, body: payload }).unwrap();
      } else {
        const payload: CreateIncompatibilityRuleRequest = {
          ruleCode: ruleCode.trim().toUpperCase(),
          primaryHazardId,
          conflictingHazardId,
          reason: reason.trim(),
          appliesToAdjacentZones,
        };
        await createRule(payload).unwrap();
      }
      onClose();
    } catch (err: any) {
      setError(err?.data?.message || err?.message || 'Failed to save incompatibility rule.');
    }
  };

  const isLoading = isCreating || isUpdating;

  return (
    <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/70 backdrop-blur-sm p-4 animate-in fade-in duration-150">
      <div className="bg-white dark:bg-slate-900 border border-slate-200 dark:border-slate-800 rounded-2xl p-6 w-full max-w-lg shadow-2xl space-y-5">
        <div className="flex items-center justify-between border-b border-slate-200 dark:border-slate-800 pb-3">
          <div className="flex items-center gap-2 text-slate-900 dark:text-slate-100 font-bold text-lg">
            <ShieldAlert className="w-5 h-5 text-rose-500" />
            <span>{isEdit ? 'Edit SIMOPS Collision Rule' : 'Create Incompatibility Rule'}</span>
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
              Rule Code <span className="text-rose-500">*</span>
            </label>
            <input
              type="text"
              value={ruleCode}
              disabled={isEdit}
              onChange={(e) => setRuleCode(e.target.value)}
              placeholder="e.g. HR-07, SIMOPS-09"
              className="w-full bg-slate-50 dark:bg-slate-950 border border-slate-200 dark:border-slate-800 rounded-xl px-3 py-2 text-slate-900 dark:text-slate-100 font-mono focus:outline-none focus:ring-2 focus:ring-amber-500/50 disabled:opacity-50"
            />
          </div>

          <div className="grid grid-cols-1 sm:grid-cols-2 gap-4">
            <div>
              <label className="block text-slate-600 dark:text-slate-400 font-medium mb-1">
                Primary Hazard Type <span className="text-rose-500">*</span>
              </label>
              <select
                value={primaryHazardId}
                disabled={isEdit}
                onChange={(e) => setPrimaryHazardId(e.target.value)}
                className="w-full bg-slate-50 dark:bg-slate-950 border border-slate-200 dark:border-slate-800 rounded-xl px-3 py-2 text-slate-900 dark:text-slate-100 focus:outline-none focus:ring-2 focus:ring-amber-500/50 disabled:opacity-50"
              >
                <option value="">Select Primary Hazard</option>
                {hazardTypes.map((ht) => (
                  <option key={ht.id} value={ht.id}>
                    {ht.code} — {ht.name}
                  </option>
                ))}
              </select>
            </div>

            <div>
              <label className="block text-slate-600 dark:text-slate-400 font-medium mb-1">
                Conflicting Hazard Type <span className="text-rose-500">*</span>
              </label>
              <select
                value={conflictingHazardId}
                disabled={isEdit}
                onChange={(e) => setConflictingHazardId(e.target.value)}
                className="w-full bg-slate-50 dark:bg-slate-950 border border-slate-200 dark:border-slate-800 rounded-xl px-3 py-2 text-slate-900 dark:text-slate-100 focus:outline-none focus:ring-2 focus:ring-amber-500/50 disabled:opacity-50"
              >
                <option value="">Select Conflicting Hazard</option>
                {hazardTypes.map((ht) => (
                  <option key={ht.id} value={ht.id}>
                    {ht.code} — {ht.name}
                  </option>
                ))}
              </select>
            </div>
          </div>

          <div>
            <label className="block text-slate-600 dark:text-slate-400 font-medium mb-1">
              Collision Reason & Technical Rationale <span className="text-rose-500">*</span>
            </label>
            <textarea
              rows={3}
              value={reason}
              onChange={(e) => setReason(e.target.value)}
              placeholder="e.g. Open sparks from welding create catastrophic ignition risk with volatile paint solvent vapours."
              className="w-full bg-slate-50 dark:bg-slate-950 border border-slate-200 dark:border-slate-800 rounded-xl px-3 py-2 text-slate-900 dark:text-slate-100 focus:outline-none focus:ring-2 focus:ring-amber-500/50"
            />
          </div>

          <div className="p-3 bg-slate-50 dark:bg-slate-950/60 rounded-xl border border-slate-200 dark:border-slate-800 flex items-center justify-between">
            <div>
              <div className="font-semibold text-slate-800 dark:text-slate-200">Enforce on Adjacent Zones</div>
              <div className="text-[11px] text-slate-500 dark:text-slate-400">
                Spills collision check over to linked spatial neighbors in plant graph
              </div>
            </div>
            <label className="relative inline-flex items-center cursor-pointer">
              <input
                type="checkbox"
                checked={appliesToAdjacentZones}
                onChange={(e) => setAppliesToAdjacentZones(e.target.checked)}
                className="sr-only peer"
              />
              <div className="w-9 h-5 bg-slate-300 peer-focus:outline-none rounded-full peer dark:bg-slate-700 peer-checked:after:translate-x-full peer-checked:after:border-white after:content-[''] after:absolute after:top-[2px] after:left-[2px] after:bg-white after:border-slate-300 after:border after:rounded-full after:h-4 after:w-4 after:transition-all peer-checked:bg-rose-500"></div>
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
              className="px-5 py-2 rounded-xl bg-rose-500 hover:bg-rose-600 text-white font-bold flex items-center gap-1.5 shadow-md shadow-rose-500/20 disabled:opacity-50 transition-colors"
            >
              {isLoading ? 'Saving...' : isEdit ? 'Update Rule' : 'Create Rule'}
            </button>
          </div>
        </form>
      </div>
    </div>
  );
};

// ─── Modal: Link Spatial Zone Adjacency ───────────────────────────────────────
interface LinkZoneModalProps {
  onClose: () => void;
}

const LinkZoneModal: React.FC<LinkZoneModalProps> = ({ onClose }) => {
  const [addZoneAdjacency, { isLoading }] = useAddZoneAdjacencyMutation();
  const { data: zones = [] } = useGetZonesQuery();

  const [zoneId, setZoneId] = useState('');
  const [adjacentZoneId, setAdjacentZoneId] = useState('');
  const [error, setError] = useState<string | null>(null);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError(null);

    if (!zoneId || !adjacentZoneId) {
      setError('Please select both plant zones to link.');
      return;
    }
    if (zoneId === adjacentZoneId) {
      setError('A zone cannot be linked as adjacent to itself.');
      return;
    }

    try {
      const payload: AddZoneAdjacencyRequest = {
        zoneId,
        adjacentZoneId,
      };
      await addZoneAdjacency(payload).unwrap();
      onClose();
    } catch (err: any) {
      setError(err?.data?.message || err?.message || 'Failed to link zones.');
    }
  };

  return (
    <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/70 backdrop-blur-sm p-4 animate-in fade-in duration-150">
      <div className="bg-white dark:bg-slate-900 border border-slate-200 dark:border-slate-800 rounded-2xl p-6 w-full max-w-md shadow-2xl space-y-5">
        <div className="flex items-center justify-between border-b border-slate-200 dark:border-slate-800 pb-3">
          <div className="flex items-center gap-2 text-slate-900 dark:text-slate-100 font-bold text-lg">
            <LinkIcon className="w-5 h-5 text-sky-500" />
            <span>Link Adjacent Plant Zones</span>
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
              Primary Plant Zone <span className="text-rose-500">*</span>
            </label>
            <select
              value={zoneId}
              onChange={(e) => setZoneId(e.target.value)}
              className="w-full bg-slate-50 dark:bg-slate-950 border border-slate-200 dark:border-slate-800 rounded-xl px-3 py-2 text-slate-900 dark:text-slate-100 focus:outline-none focus:ring-2 focus:ring-sky-500/50"
            >
              <option value="">Select Zone A</option>
              {zones.map((z) => (
                <option key={z.id} value={z.id}>
                  {z.code} — {z.name}
                </option>
              ))}
            </select>
          </div>

          <div>
            <label className="block text-slate-600 dark:text-slate-400 font-medium mb-1">
              Neighboring Adjacent Zone <span className="text-rose-500">*</span>
            </label>
            <select
              value={adjacentZoneId}
              onChange={(e) => setAdjacentZoneId(e.target.value)}
              className="w-full bg-slate-50 dark:bg-slate-950 border border-slate-200 dark:border-slate-800 rounded-xl px-3 py-2 text-slate-900 dark:text-slate-100 focus:outline-none focus:ring-2 focus:ring-sky-500/50"
            >
              <option value="">Select Zone B</option>
              {zones
                .filter((z) => z.id !== zoneId)
                .map((z) => (
                  <option key={z.id} value={z.id}>
                    {z.code} — {z.name}
                  </option>
                ))}
            </select>
          </div>

          <div className="p-3 bg-sky-50 dark:bg-sky-950/40 border border-sky-200 dark:border-sky-800/60 rounded-xl text-sky-800 dark:text-sky-300 text-[11px] leading-relaxed">
            <Info className="w-4 h-4 text-sky-500 inline mr-1 -mt-0.5" />
            Adjacency relationships are symmetric. Linking Zone A to Zone B automatically establishes the bidirectional risk transfer link.
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
              className="px-5 py-2 rounded-xl bg-sky-500 hover:bg-sky-600 text-white font-bold flex items-center gap-1.5 shadow-md shadow-sky-500/20 disabled:opacity-50 transition-colors"
            >
              {isLoading ? 'Linking...' : 'Establish Link'}
            </button>
          </div>
        </form>
      </div>
    </div>
  );
};

// ─── Main Conflict Matrix Page ───────────────────────────────────────────────
export const ConflictMatrixEditorPage: React.FC = () => {
  const user = useSelector((state: RootState) => state.auth.user);
  const { data: rules = [], isLoading: rulesLoading } = useGetIncompatibilityRulesQuery();
  const { data: hazardTypes = [] } = useGetHazardTypesQuery();
  const { data: adjacencies = [], isLoading: adjLoading } = useGetZoneAdjacenciesQuery();

  const [deleteRule] = useDeleteIncompatibilityRuleMutation();
  const [removeZoneAdjacency] = useRemoveZoneAdjacencyMutation();

  const [activeTab, setActiveTab] = useState<'rules' | 'matrix' | 'adjacencies'>('rules');
  const [searchQuery, setSearchQuery] = useState('');

  // Modals
  const [ruleModalOpen, setRuleModalOpen] = useState(false);
  const [editingRule, setEditingRule] = useState<IncompatibilityRule | null>(null);
  const [linkZoneModalOpen, setLinkZoneModalOpen] = useState(false);

  // Notification
  const [actionMessage, setActionMessage] = useState<{ type: 'success' | 'error'; text: string } | null>(null);

  const canEdit =
    user?.role === 'Administrator' ||
    user?.role === 'SafetyOfficer' ||
    user?.role === 'AreaSupervisor';

  const canDelete = user?.role === 'Administrator' || user?.role === 'SafetyOfficer';

  const handleDeleteRule = async (id: string, code: string) => {
    if (!window.confirm(`Are you sure you want to delete Incompatibility Rule "${code}"?`)) {
      return;
    }
    try {
      await deleteRule(id).unwrap();
      setActionMessage({ type: 'success', text: `Rule "${code}" deleted successfully.` });
      setTimeout(() => setActionMessage(null), 4000);
    } catch (err: any) {
      setActionMessage({
        type: 'error',
        text: err?.data?.message || err?.message || 'Failed to delete incompatibility rule.',
      });
    }
  };

  const handleRemoveAdjacency = async (zoneId: string, adjacentZoneId: string, nameA: string, nameB: string) => {
    if (!window.confirm(`Remove spatial adjacency link between "${nameA}" and "${nameB}"?`)) {
      return;
    }
    try {
      await removeZoneAdjacency({ zoneId, adjacentZoneId }).unwrap();
      setActionMessage({ type: 'success', text: `Adjacency link between ${nameA} and ${nameB} removed.` });
      setTimeout(() => setActionMessage(null), 4000);
    } catch (err: any) {
      setActionMessage({
        type: 'error',
        text: err?.data?.message || err?.message || 'Failed to remove adjacency link.',
      });
    }
  };

  const filteredRules = rules.filter((r) => {
    const query = searchQuery.toLowerCase();
    return (
      r.ruleCode.toLowerCase().includes(query) ||
      r.primaryHazardCode.toLowerCase().includes(query) ||
      r.primaryHazardName.toLowerCase().includes(query) ||
      r.conflictingHazardCode.toLowerCase().includes(query) ||
      r.conflictingHazardName.toLowerCase().includes(query) ||
      r.reason.toLowerCase().includes(query)
    );
  });

  return (
    <div className="space-y-6 max-w-6xl mx-auto pb-12">
      {/* Breadcrumb & Sub Navigation */}
      <div className="flex flex-wrap items-center justify-between gap-4 pb-2 border-b border-slate-200 dark:border-slate-800">
        <div className="flex items-center gap-2 text-xs font-medium text-slate-500 dark:text-slate-400">
          <Link to="/hazard-rules" className="hover:text-amber-500 transition-colors">
            Hazard Zones & Observations
          </Link>
          <span>/</span>
          <span className="text-slate-900 dark:text-slate-100 font-bold">Conflict & Matrix Editor</span>
        </div>

        <div className="flex items-center gap-2">
          <Link
            to="/hazard-rules/rulebook"
            className="px-3 py-1.5 bg-slate-100 dark:bg-slate-800 hover:bg-slate-200 dark:hover:bg-slate-700 text-slate-700 dark:text-slate-200 text-xs font-semibold rounded-xl flex items-center gap-1.5 transition-colors"
          >
            <BookOpen className="w-3.5 h-3.5 text-amber-500" />
            <span>Open Rulebook Editor</span>
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
            <Sliders className="w-6 h-6 text-rose-500" />
            <span>SIMOPS Conflict & Incompatibility Matrix Editor</span>
          </h1>
          <p className="text-xs text-slate-500 dark:text-slate-400 mt-1">
            Configure simultaneous operations collision rules, atmospheric vapor constraints, and spatial zone graph adjacencies.
          </p>
        </div>

        <div className="flex items-center gap-2 shrink-0">
          {canEdit && (
            <button
              onClick={() => {
                setEditingRule(null);
                setRuleModalOpen(true);
              }}
              className="px-4 py-2.5 bg-rose-500 hover:bg-rose-600 text-white font-bold text-xs rounded-xl flex items-center gap-2 shadow-lg shadow-rose-500/20 transition-all"
            >
              <Plus className="w-4 h-4" />
              <span>Add Collision Rule</span>
            </button>
          )}

          {canEdit && (
            <button
              onClick={() => setLinkZoneModalOpen(true)}
              className="px-3.5 py-2.5 bg-sky-500/10 hover:bg-sky-500/20 text-sky-600 dark:text-sky-400 border border-sky-500/30 font-bold text-xs rounded-xl flex items-center gap-1.5 transition-all"
            >
              <LinkIcon className="w-4 h-4" />
              <span>Link Zones</span>
            </button>
          )}
        </div>
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

      {/* Tabs */}
      <div className="flex items-center gap-2 border-b border-slate-200 dark:border-slate-800">
        <button
          onClick={() => setActiveTab('rules')}
          className={`px-4 py-2.5 text-xs font-bold border-b-2 flex items-center gap-2 transition-all ${
            activeTab === 'rules'
              ? 'border-rose-500 text-rose-600 dark:text-rose-400'
              : 'border-transparent text-slate-500 hover:text-slate-800 dark:hover:text-slate-200'
          }`}
        >
          <ShieldAlert className="w-4 h-4" />
          <span>Incompatibility Rules ({rules.length})</span>
        </button>

        <button
          onClick={() => setActiveTab('matrix')}
          className={`px-4 py-2.5 text-xs font-bold border-b-2 flex items-center gap-2 transition-all ${
            activeTab === 'matrix'
              ? 'border-rose-500 text-rose-600 dark:text-rose-400'
              : 'border-transparent text-slate-500 hover:text-slate-800 dark:hover:text-slate-200'
          }`}
        >
          <Grid className="w-4 h-4" />
          <span>2D Collision Matrix Visualizer</span>
        </button>

        <button
          onClick={() => setActiveTab('adjacencies')}
          className={`px-4 py-2.5 text-xs font-bold border-b-2 flex items-center gap-2 transition-all ${
            activeTab === 'adjacencies'
              ? 'border-rose-500 text-rose-600 dark:text-rose-400'
              : 'border-transparent text-slate-500 hover:text-slate-800 dark:hover:text-slate-200'
          }`}
        >
          <Layers className="w-4 h-4" />
          <span>Spatial Zone Graph ({adjacencies.length / 2} Links)</span>
        </button>
      </div>

      {/* Tab Content 1: Rules List */}
      {activeTab === 'rules' && (
        <div className="space-y-4">
          <div className="bg-white dark:bg-slate-900 border border-slate-200 dark:border-slate-800 p-4 rounded-2xl shadow-sm">
            <div className="relative w-full">
              <Search className="w-4 h-4 absolute left-3 top-1/2 -translate-y-1/2 text-slate-400" />
              <input
                type="text"
                value={searchQuery}
                onChange={(e) => setSearchQuery(e.target.value)}
                placeholder="Search incompatibility rules by code, hazard names, or collision reasons..."
                className="w-full bg-slate-50 dark:bg-slate-950 border border-slate-200 dark:border-slate-800 rounded-xl pl-9 pr-4 py-2 text-xs text-slate-900 dark:text-slate-100 focus:outline-none focus:ring-2 focus:ring-rose-500/50"
              />
            </div>
          </div>

          {rulesLoading ? (
            <div className="py-16 text-center space-y-3">
              <div className="w-8 h-8 border-3 border-rose-500 border-t-transparent rounded-full animate-spin mx-auto" />
              <p className="text-xs text-slate-500 dark:text-slate-400">Loading SIMOPS collision rules...</p>
            </div>
          ) : filteredRules.length === 0 ? (
            <div className="bg-white dark:bg-slate-900 border border-slate-200 dark:border-slate-800 p-12 rounded-2xl text-center space-y-3">
              <ShieldX className="w-10 h-10 text-slate-400 mx-auto opacity-60" />
              <h3 className="font-bold text-slate-900 dark:text-slate-100 text-sm">No Incompatibility Rules Found</h3>
              <p className="text-xs text-slate-500 dark:text-slate-400 max-w-sm mx-auto">
                {searchQuery
                  ? 'No rules match your search query.'
                  : 'No collision rules configured. Click "Add Collision Rule" to prevent unsafe simultaneous operations.'}
              </p>
            </div>
          ) : (
            <div className="grid grid-cols-1 gap-4">
              {filteredRules.map((rule) => (
                <div
                  key={rule.id}
                  className="bg-white dark:bg-slate-900 border border-slate-200 dark:border-slate-800 rounded-2xl p-5 shadow-sm space-y-4 hover:border-slate-300 dark:hover:border-slate-700 transition-all"
                >
                  <div className="flex flex-col sm:flex-row sm:items-center justify-between gap-3 pb-3 border-b border-slate-100 dark:border-slate-800">
                    <div className="flex items-center gap-2.5">
                      <span className="font-mono font-bold text-xs bg-rose-500/10 text-rose-500 border border-rose-500/30 px-2.5 py-1 rounded-lg">
                        {rule.ruleCode}
                      </span>
                      <span className="text-xs font-bold text-slate-900 dark:text-slate-100">
                        SIMOPS Collision Constraint
                      </span>
                      {rule.appliesToAdjacentZones && (
                        <span className="bg-amber-500/10 text-amber-500 border border-amber-500/30 text-[10px] font-bold px-2 py-0.5 rounded">
                          ADJACENT ZONES SENSITIVE
                        </span>
                      )}
                    </div>

                    <div className="flex items-center gap-1.5 self-end sm:self-center">
                      {canEdit && (
                        <button
                          onClick={() => {
                            setEditingRule(rule);
                            setRuleModalOpen(true);
                          }}
                          title="Edit Rule"
                          className="p-1.5 text-slate-400 hover:text-slate-700 dark:hover:text-slate-200 hover:bg-slate-100 dark:hover:bg-slate-800 rounded-lg transition-colors"
                        >
                          <Pencil className="w-4 h-4" />
                        </button>
                      )}

                      {canDelete && (
                        <button
                          onClick={() => handleDeleteRule(rule.id, rule.ruleCode)}
                          title="Delete Rule"
                          className="p-1.5 text-rose-400 hover:text-rose-600 hover:bg-rose-50 dark:hover:bg-rose-950/40 rounded-lg transition-colors"
                        >
                          <Trash2 className="w-4 h-4" />
                        </button>
                      )}
                    </div>
                  </div>

                  {/* Hazard Collision Pair */}
                  <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                    <div className="bg-slate-50 dark:bg-slate-950 p-3.5 rounded-xl border border-slate-200 dark:border-slate-800">
                      <div className="text-[10px] uppercase font-bold text-amber-500 tracking-wider">
                        Primary Hazard
                      </div>
                      <div className="font-bold text-slate-900 dark:text-slate-100 text-sm mt-0.5">
                        {rule.primaryHazardCode}
                      </div>
                      <div className="text-xs text-slate-500 dark:text-slate-400 mt-0.5">
                        {rule.primaryHazardName}
                      </div>
                    </div>

                    <div className="bg-slate-50 dark:bg-slate-950 p-3.5 rounded-xl border border-rose-200 dark:border-rose-900/40">
                      <div className="text-[10px] uppercase font-bold text-rose-500 tracking-wider">
                        Conflicting Hazard (Strictly Incompatible)
                      </div>
                      <div className="font-bold text-slate-900 dark:text-slate-100 text-sm mt-0.5">
                        {rule.conflictingHazardCode}
                      </div>
                      <div className="text-xs text-slate-500 dark:text-slate-400 mt-0.5">
                        {rule.conflictingHazardName}
                      </div>
                    </div>
                  </div>

                  {/* Technical Reason */}
                  <div className="bg-rose-50/50 dark:bg-rose-950/20 border border-rose-100 dark:border-rose-900/30 p-3 rounded-xl text-xs text-rose-900 dark:text-rose-200 leading-relaxed">
                    <strong>Clearance Constraint Rationale:</strong> {rule.reason}
                  </div>
                </div>
              ))}
            </div>
          )}
        </div>
      )}

      {/* Tab Content 2: 2D Collision Matrix Visualizer */}
      {activeTab === 'matrix' && (
        <div className="bg-white dark:bg-slate-900 border border-slate-200 dark:border-slate-800 p-6 rounded-2xl shadow-sm space-y-4">
          <div>
            <h3 className="font-bold text-slate-900 dark:text-slate-100 text-base flex items-center gap-2">
              <Grid className="w-5 h-5 text-rose-500" />
              <span>2D SIMOPS Incompatibility Collision Grid</span>
            </h3>
            <p className="text-xs text-slate-500 dark:text-slate-400 mt-0.5">
              Visual collision detection grid. Red shields denote incompatible simultaneous activity combinations.
            </p>
          </div>

          {hazardTypes.length === 0 ? (
            <div className="py-8 text-center text-xs text-slate-400">
              No hazard types available. Please configure hazard types in Rulebook Editor first.
            </div>
          ) : (
            <div className="overflow-x-auto">
              <table className="w-full border-collapse text-xs">
                <thead>
                  <tr>
                    <th className="p-3 border border-slate-200 dark:border-slate-800 bg-slate-100 dark:bg-slate-950 text-left font-bold text-slate-600 dark:text-slate-300">
                      Hazard Class
                    </th>
                    {hazardTypes.map((ht) => (
                      <th
                        key={ht.id}
                        className="p-3 border border-slate-200 dark:border-slate-800 bg-slate-100 dark:bg-slate-950 text-center font-bold text-slate-700 dark:text-slate-200 min-w-[120px]"
                      >
                        <div className="font-mono text-xs">{ht.code}</div>
                        <div className="text-[10px] font-normal text-slate-500 truncate max-w-[110px] mx-auto">
                          {ht.name}
                        </div>
                      </th>
                    ))}
                  </tr>
                </thead>
                <tbody>
                  {hazardTypes.map((rowHt) => (
                    <tr key={rowHt.id}>
                      <td className="p-3 border border-slate-200 dark:border-slate-800 bg-slate-50 dark:bg-slate-950 font-bold text-slate-800 dark:text-slate-200">
                        <div className="font-mono text-xs">{rowHt.code}</div>
                        <div className="text-[10px] font-normal text-slate-500">{rowHt.name}</div>
                      </td>

                      {hazardTypes.map((colHt) => {
                        if (rowHt.id === colHt.id) {
                          return (
                            <td
                              key={colHt.id}
                              className="p-3 border border-slate-200 dark:border-slate-800 bg-slate-100/50 dark:bg-slate-900/50 text-center text-slate-400 text-[11px]"
                            >
                              —
                            </td>
                          );
                        }

                        // Check if rule exists
                        const directRule = rules.find(
                          (r) =>
                            (r.primaryHazardId === rowHt.id && r.conflictingHazardId === colHt.id) ||
                            (r.primaryHazardId === colHt.id && r.conflictingHazardId === rowHt.id)
                        );

                        if (directRule) {
                          return (
                            <td
                              key={colHt.id}
                              title={`${directRule.ruleCode}: ${directRule.reason}${
                                directRule.appliesToAdjacentZones ? ' (Applies to adjacent zones)' : ''
                              }`}
                              className="p-3 border border-slate-200 dark:border-slate-800 bg-rose-500/10 text-center cursor-help"
                            >
                              <div className="flex flex-col items-center gap-1">
                                <ShieldAlert className="w-4 h-4 text-rose-500" />
                                <span className="font-mono text-[10px] font-bold text-rose-600 dark:text-rose-400">
                                  {directRule.ruleCode}
                                </span>
                              </div>
                            </td>
                          );
                        }

                        return (
                          <td
                            key={colHt.id}
                            className="p-3 border border-slate-200 dark:border-slate-800 bg-emerald-500/5 text-center"
                          >
                            <CheckCircle2 className="w-4 h-4 text-emerald-500/60 mx-auto" />
                          </td>
                        );
                      })}
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
          )}

          <div className="flex flex-wrap items-center gap-6 pt-3 text-xs text-slate-500">
            <div className="flex items-center gap-2">
              <CheckCircle2 className="w-4 h-4 text-emerald-500" />
              <span>Compatible (Permitted in same/adjacent zones)</span>
            </div>
            <div className="flex items-center gap-2">
              <ShieldAlert className="w-4 h-4 text-rose-500" />
              <span>Incompatible SIMOPS Hazard Collision (Strict Refusal)</span>
            </div>
          </div>
        </div>
      )}

      {/* Tab Content 3: Spatial Zone Adjacency Graph */}
      {activeTab === 'adjacencies' && (
        <div className="space-y-4">
          <div className="bg-white dark:bg-slate-900 border border-slate-200 dark:border-slate-800 p-6 rounded-2xl shadow-sm">
            <div className="flex flex-col sm:flex-row sm:items-center justify-between gap-4 pb-4 border-b border-slate-100 dark:border-slate-800">
              <div>
                <h3 className="font-bold text-slate-900 dark:text-slate-100 text-base flex items-center gap-2">
                  <Layers className="w-5 h-5 text-sky-500" />
                  <span>Plant Spatial Zone Adjacency Registry</span>
                </h3>
                <p className="text-xs text-slate-500 dark:text-slate-400 mt-0.5">
                  Topology graph of industrial sectors used by AI agents to calculate adjacent hazard transfer risks.
                </p>
              </div>

              {canEdit && (
                <button
                  onClick={() => setLinkZoneModalOpen(true)}
                  className="px-4 py-2 bg-sky-500 hover:bg-sky-600 text-white font-bold text-xs rounded-xl flex items-center gap-2 shadow-md shadow-sky-500/20 transition-all shrink-0"
                >
                  <Plus className="w-4 h-4" />
                  <span>Link New Pair</span>
                </button>
              )}
            </div>

            {adjLoading ? (
              <div className="py-12 text-center text-xs text-slate-400">Loading plant topology...</div>
            ) : adjacencies.length === 0 ? (
              <div className="py-12 text-center text-xs text-slate-400">
                No spatial zone adjacency links registered.
              </div>
            ) : (
              <div className="grid grid-cols-1 md:grid-cols-2 gap-3 mt-4">
                {/* Filter distinct pairs */}
                {adjacencies
                  .filter((adj, idx, self) => {
                    // Unique link regardless of direction
                    const firstIdx = self.findIndex(
                      (other) =>
                        (other.zoneId === adj.zoneId && other.adjacentZoneId === adj.adjacentZoneId) ||
                        (other.zoneId === adj.adjacentZoneId && other.adjacentZoneId === adj.zoneId)
                    );
                    return firstIdx === idx;
                  })
                  .map((adj, index) => (
                    <div
                      key={index}
                      className="bg-slate-50 dark:bg-slate-950 p-4 rounded-xl border border-slate-200 dark:border-slate-800 flex items-center justify-between gap-3 shadow-xs"
                    >
                      <div className="flex items-center gap-3">
                        <div className="bg-sky-500/10 border border-sky-500/30 p-2 rounded-lg text-sky-500">
                          <LinkIcon className="w-4 h-4" />
                        </div>
                        <div className="flex items-center gap-2 text-xs">
                          <div>
                            <span className="font-mono font-bold text-slate-900 dark:text-slate-100">
                              {adj.zoneCode}
                            </span>
                            <span className="text-[11px] text-slate-500 block">{adj.zoneName}</span>
                          </div>
                          <ArrowRight className="w-3.5 h-3.5 text-slate-400 shrink-0" />
                          <div>
                            <span className="font-mono font-bold text-slate-900 dark:text-slate-100">
                              {adj.adjacentZoneCode}
                            </span>
                            <span className="text-[11px] text-slate-500 block">{adj.adjacentZoneName}</span>
                          </div>
                        </div>
                      </div>

                      {canEdit && (
                        <button
                          onClick={() =>
                            handleRemoveAdjacency(
                              adj.zoneId,
                              adj.adjacentZoneId,
                              adj.zoneCode,
                              adj.adjacentZoneCode
                            )
                          }
                          title="Unlink Zones"
                          className="p-1.5 text-rose-400 hover:text-rose-600 hover:bg-rose-50 dark:hover:bg-rose-950/40 rounded-lg transition-colors"
                        >
                          <Unlink className="w-4 h-4" />
                        </button>
                      )}
                    </div>
                  ))}
              </div>
            )}
          </div>
        </div>
      )}

      {/* Rule Modal */}
      {ruleModalOpen && (
        <IncompatibilityRuleModal
          rule={editingRule}
          onClose={() => {
            setRuleModalOpen(false);
            setEditingRule(null);
          }}
        />
      )}

      {/* Link Zone Modal */}
      {linkZoneModalOpen && <LinkZoneModal onClose={() => setLinkZoneModalOpen(false)} />}
    </div>
  );
};

export default ConflictMatrixEditorPage;