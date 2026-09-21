import React, { useState } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { useSelector } from 'react-redux';
import type { RootState } from '../store';
import {
  useGetPermitByIdQuery,
  useRecordDecisionMutation,
  useSubmitPermitForAiReviewMutation,
  useDeletePermitDraftMutation,
} from '../store/apiSlice';
import { StatusBadge } from '../components/StatusBadge';
import { EditPermitModal } from '../components/EditPermitModal';
import { AgentExecutionTimeline } from '../components/AgentExecutionTimeline';
import { useTranslation } from '../context/I18nContext';
import {
  ArrowLeft,
  Calendar,
  MapPin,
  User,
  HardHat,
  Wrench,
  ShieldCheck,
  CheckCircle2,
  XCircle,
  RotateCcw,
  Cpu,
  QrCode,
  FileCheck,
  Pencil,
  Trash2,
} from 'lucide-react';

export const PermitDetailPage: React.FC = () => {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const { t } = useTranslation();
  const currentUser = useSelector((state: RootState) => state.auth.user);

  const { data: permit, isLoading, error, refetch } = useGetPermitByIdQuery(id || '');
  const [recordDecision, { isLoading: isDeciding }] = useRecordDecisionMutation();
  const [submitForAiReview, { isLoading: isReviewing }] = useSubmitPermitForAiReviewMutation();
  const [deletePermit, { isLoading: isDeleting }] = useDeletePermitDraftMutation();

  const [decisionNotes, setDecisionNotes] = useState('');
  const [showSignOffModal, setShowSignOffModal] = useState(false);
  const [showEditModal, setShowEditModal] = useState(false);
  const [decisionAction, setDecisionAction] = useState<'Approved' | 'Rejected' | 'RevisionRequested'>('Approved');

  if (isLoading) {
    return (
      <div className="p-12 text-center text-slate-500 dark:text-slate-400">
        <div className="animate-spin w-8 h-8 border-2 border-amber-500 border-t-transparent rounded-full mx-auto mb-3"></div>
        <span>Loading permit dossier...</span>
      </div>
    );
  }

  if (error || !permit) {
    return (
      <div className="p-8 text-center text-rose-600 dark:text-rose-400 space-y-3">
        <div className="text-lg font-bold">Permit Not Found</div>
        <button
          onClick={() => navigate('/permits')}
          className="text-xs text-amber-600 dark:text-amber-400 hover:underline inline-flex items-center gap-1 font-semibold"
        >
          <ArrowLeft className="w-3 h-3" /> {t('permit_detail_back')}
        </button>
      </div>
    );
  }

  const isSafetyOfficer = currentUser?.role === 'SafetyOfficer';
  const isAdmin = currentUser?.role === 'Administrator';
  const canEditOrDelete = !isSafetyOfficer && (permit.status === 'Draft' || permit.status === 'Refused');
  // Only allow human sign-off once AI has cleared the permit to PendingApproval
  const canDecide = (isSafetyOfficer || isAdmin) && permit.status === 'PendingApproval';

  const handleDecisionSubmit = async () => {
    try {
      await recordDecision({
        permitId: permit.id,
        decision: decisionAction,
        decisionNotes: decisionNotes || (decisionAction === 'Approved' ? 'All safety controls verified.' : 'Refusal reason recorded.'),
      }).unwrap();
      setShowSignOffModal(false);
      refetch();
    } catch (err: any) {
      alert(`Decision recording failed: ${err.data?.message || 'Server error'}`);
    }
  };

  const handleRunAi = async () => {
    try {
      await submitForAiReview(permit.id).unwrap();
      refetch();
    } catch (err: any) {
      alert(`AI Review failed: ${err.data?.message || 'Server error'}`);
    }
  };

  const handleDeleteDraft = async () => {
    if (window.confirm(`Are you sure you want to delete permit draft ${permit.permitNumber}?`)) {
      try {
        await deletePermit(permit.id).unwrap();
        navigate('/permits');
      } catch (err: any) {
        alert(err?.data?.message || 'Failed to delete permit draft.');
      }
    }
  };

  return (
    <div className="space-y-6 max-w-6xl mx-auto pb-12">
      {showEditModal && (
        <EditPermitModal
          permit={permit}
          onClose={() => setShowEditModal(false)}
          onUpdated={() => refetch()}
        />
      )}

      {/* Top Bar */}
      <div className="flex flex-wrap items-center justify-between gap-3">
        <button
          onClick={() => navigate('/permits')}
          className="inline-flex items-center gap-1.5 text-xs text-slate-500 dark:text-slate-400 hover:text-slate-900 dark:hover:text-slate-200 transition font-semibold"
        >
          <ArrowLeft className="w-4 h-4" /> {t('permit_detail_back')}
        </button>

        <div className="flex items-center gap-3">
          {canEditOrDelete && (
            <>
              <button
                onClick={() => setShowEditModal(true)}
                className="flex items-center gap-1.5 px-3.5 py-2 bg-slate-100 dark:bg-slate-800 hover:bg-slate-200 dark:hover:bg-slate-700 text-slate-800 dark:text-slate-200 font-bold rounded-xl text-xs transition border border-slate-200 dark:border-slate-700"
              >
                <Pencil className="w-3.5 h-3.5 text-amber-500" />
                <span>Edit Draft</span>
              </button>
              <button
                onClick={handleDeleteDraft}
                disabled={isDeleting}
                className="flex items-center gap-1.5 px-3.5 py-2 bg-rose-50 dark:bg-rose-950/40 hover:bg-rose-100 dark:hover:bg-rose-900/60 text-rose-700 dark:text-rose-300 font-bold rounded-xl text-xs transition border border-rose-300 dark:border-rose-800"
              >
                <Trash2 className="w-3.5 h-3.5" />
                <span>{isDeleting ? 'Deleting...' : 'Delete Draft'}</span>
              </button>
            </>
          )}

          {permit.status === 'Draft' && (
            <button
              onClick={handleRunAi}
              disabled={isReviewing}
              className="flex items-center gap-2 px-4 py-2 bg-purple-600 hover:bg-purple-500 text-white font-bold rounded-xl text-xs shadow-md transition disabled:opacity-50"
            >
              <Cpu className="w-4 h-4" />
              <span>{isReviewing ? 'Running Agents...' : t('permit_detail_run_ai')}</span>
            </button>
          )}

          {canDecide && (
            <button
              onClick={() => {
                setDecisionAction('Approved');
                setShowSignOffModal(true);
              }}
              className="flex items-center gap-2 px-4 py-2 bg-emerald-600 hover:bg-emerald-500 text-white font-bold rounded-xl text-xs shadow-md transition"
            >
              <ShieldCheck className="w-4 h-4" />
              <span>{t('permit_detail_signoff')}</span>
            </button>
          )}
        </div>
      </div>


      {/* Main Header Card */}
      <div className="bg-white dark:bg-slate-900 border border-slate-200 dark:border-slate-800 rounded-2xl p-6 shadow-sm">
        <div className="flex flex-wrap items-start justify-between gap-4">
          <div>
            <div className="flex items-center gap-3">
              <h1 className="text-2xl font-black text-slate-900 dark:text-slate-100 font-mono tracking-tight">
                {permit.permitNumber}
              </h1>
              <StatusBadge status={permit.status} />
            </div>
            <p className="text-base font-semibold text-amber-600 dark:text-amber-400 mt-1">
              {permit.permitTypeName} ({permit.permitTypeCode})
            </p>
          </div>

          {permit.permitQrToken && (
            <div className="bg-slate-50 dark:bg-slate-950 p-3 rounded-xl border border-slate-200 dark:border-slate-800 flex items-center gap-2.5 text-xs">
              <QrCode className="w-5 h-5 text-emerald-600 dark:text-emerald-400 shrink-0" />
              <div>
                <div className="text-[10px] text-slate-500 dark:text-slate-400 font-medium uppercase">Active Permit Token</div>
                <div className="font-mono text-emerald-600 dark:text-emerald-300 font-bold text-[11px]">{permit.permitQrToken}</div>
              </div>
            </div>
          )}
        </div>

        <p className="mt-4 text-sm text-slate-700 dark:text-slate-300 bg-slate-50 dark:bg-slate-950/60 p-3.5 rounded-xl border border-slate-200 dark:border-slate-800 leading-relaxed">
          {permit.objectiveDescription}
        </p>

        {/* Metadata Badges */}
        <div className="mt-4 grid grid-cols-1 sm:grid-cols-3 gap-3 text-xs pt-4 border-t border-slate-200 dark:border-slate-800/80">
          <div className="flex items-center gap-2 text-slate-700 dark:text-slate-300">
            <MapPin className="w-4 h-4 text-amber-500 dark:text-amber-400 shrink-0" />
            <div>
              <span className="text-slate-500 dark:text-slate-400 block text-[10px] uppercase">Zone Location</span>
              <span className="font-bold">{permit.zoneName} ({permit.zoneCode})</span>
            </div>
          </div>

          <div className="flex items-center gap-2 text-slate-700 dark:text-slate-300">
            <Calendar className="w-4 h-4 text-amber-500 dark:text-amber-400 shrink-0" />
            <div>
              <span className="text-slate-500 dark:text-slate-400 block text-[10px] uppercase">Time Window</span>
              <span className="font-mono">
                {new Date(permit.scheduledStartTime).toLocaleString([], { dateStyle: 'short', timeStyle: 'short' })}
                {' – '}
                {new Date(permit.scheduledEndTime).toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' })}
              </span>
            </div>
          </div>

          <div className="flex items-center gap-2 text-slate-700 dark:text-slate-300">
            <User className="w-4 h-4 text-amber-500 dark:text-amber-400 shrink-0" />
            <div>
              <span className="text-slate-500 dark:text-slate-400 block text-[10px] uppercase">Field Supervisor</span>
              <span className="font-semibold">{permit.supervisorName}</span>
            </div>
          </div>
        </div>
      </div>

      {/* Decision Record Banner if already decided */}
      {permit.approval && (
        <div
          className={`p-4 rounded-2xl border flex items-start gap-3 text-xs shadow-sm ${
            permit.approval.decision === 'Approved'
              ? 'bg-emerald-50 dark:bg-emerald-950/30 border-emerald-300 dark:border-emerald-500/40 text-emerald-800 dark:text-emerald-200'
              : 'bg-rose-50 dark:bg-rose-950/30 border-rose-300 dark:border-rose-500/40 text-rose-800 dark:text-rose-200'
          }`}
        >
          <ShieldCheck className="w-5 h-5 shrink-0 mt-0.5 text-current" />
          <div>
            <div className="font-bold text-sm">
              HSE Decision: {permit.approval.decision} by {permit.approval.safetyOfficerName}
            </div>
            <div className="text-[11px] opacity-80 mt-0.5 font-mono">
              Timestamp: {new Date(permit.approval.decisionTimestamp).toLocaleString()}
            </div>
            <div className="mt-2 p-2.5 bg-white/60 dark:bg-slate-950/40 rounded-xl border border-current/20 leading-relaxed font-sans">
              <strong>Official Notes:</strong> {permit.approval.decisionNotes}
            </div>
          </div>
        </div>
      )}

      {/* Refused/Safe Failure Remediation Banner */}
      {permit.status === 'Refused' && permit.workflowRun && (
        <div className="p-4 rounded-2xl border border-amber-300 dark:border-amber-500/40 bg-amber-50 dark:bg-amber-950/20 text-amber-900 dark:text-amber-200 flex items-start gap-3 text-xs shadow-sm">
          <RotateCcw className="w-5 h-5 shrink-0 mt-0.5 text-amber-600 dark:text-amber-400" />
          <div>
            <div className="font-bold text-sm text-amber-800 dark:text-amber-300">
              Permit Refused — AI Safe Failure. Action Required.
            </div>
            <p className="mt-1 opacity-80 leading-relaxed">
              This permit was refused by the multi-agent engine. To proceed, create a new permit draft incorporating
              the recommended remediation fixes shown in the AI Execution Trace below, then re-submit for AI review.
            </p>
          </div>
        </div>
      )}

      {/* Two Column Layout: Resources vs Agent Trace */}
      <div className="grid grid-cols-1 lg:grid-cols-3 gap-6">
        {/* Left Column: Assigned Workers & Equipment */}
        <div className="space-y-6 lg:col-span-1">
          {/* Workforce Card */}
          <div className="bg-white dark:bg-slate-900 border border-slate-200 dark:border-slate-800 rounded-2xl p-4 shadow-sm">
            <div className="flex items-center gap-2 text-sm font-bold text-slate-800 dark:text-slate-200 pb-3 border-b border-slate-200 dark:border-slate-800">
              <HardHat className="w-4 h-4 text-amber-500 dark:text-amber-400" />
              <span>{t('permit_assigned_personnel')} ({permit.assignedWorkers.length})</span>
            </div>

            <div className="mt-3 space-y-2.5">
              {permit.assignedWorkers.length === 0 ? (
                <div className="text-xs text-slate-400 dark:text-slate-500 italic py-2">{t('permit_no_workers')}</div>
              ) : (
                permit.assignedWorkers.map((w) => {
                  const hasExpired = w.certificates.some((c) => c.status === 'Expired' || c.daysUntilExpiry < 0);
                  return (
                    <div
                      key={w.id}
                      className={`p-3 rounded-xl border text-xs ${
                        hasExpired
                          ? 'bg-rose-50 dark:bg-rose-950/20 border-rose-300 dark:border-rose-800/80 text-rose-800 dark:text-rose-200'
                          : 'bg-slate-50 dark:bg-slate-950/60 border-slate-200 dark:border-slate-800 text-slate-800 dark:text-slate-200'
                      }`}
                    >
                      <div className="flex items-center justify-between">
                        <span className="font-bold">{w.firstName} {w.lastName}</span>
                        <span className="font-mono text-[10px] bg-slate-200 dark:bg-slate-800 text-slate-700 dark:text-slate-400 px-1.5 py-0.5 rounded">
                          {w.badgeNumber}
                        </span>
                      </div>
                      <div className="text-[11px] text-slate-500 dark:text-slate-400 mt-0.5">{w.trade} · {w.contractorName}</div>

                      {w.certificates.map((cert) => (
                        <div key={cert.id} className="mt-1.5 text-[10px] font-mono flex items-center justify-between">
                          <span className="truncate pr-2">{cert.certificateName || cert.certificateCode}</span>
                          <span
                            className={`font-bold ${
                              cert.status === 'Expired' || cert.daysUntilExpiry < 0
                                ? 'text-rose-600 dark:text-rose-400'
                                : 'text-emerald-600 dark:text-emerald-400'
                            }`}
                          >
                            {cert.status} ({cert.daysUntilExpiry}d)
                          </span>
                        </div>
                      ))}
                    </div>
                  );
                })
              )}
            </div>
          </div>

          {/* Equipment Card */}
          <div className="bg-white dark:bg-slate-900 border border-slate-200 dark:border-slate-800 rounded-2xl p-4 shadow-sm">
            <div className="flex items-center gap-2 text-sm font-bold text-slate-800 dark:text-slate-200 pb-3 border-b border-slate-200 dark:border-slate-800">
              <Wrench className="w-4 h-4 text-amber-500 dark:text-amber-400" />
              <span>{t('permit_assigned_equipment')} ({permit.assignedAssets.length})</span>
            </div>

            <div className="mt-3 space-y-2.5">
              {permit.assignedAssets.length === 0 ? (
                <div className="text-xs text-slate-400 dark:text-slate-500 italic py-2">{t('permit_no_equipment')}</div>
              ) : (
                permit.assignedAssets.map((asset) => (
                  <div
                    key={asset.id}
                    className={`p-3 rounded-xl border text-xs ${
                      !asset.isInspectionValid || !asset.isCalibrationValid
                        ? 'bg-rose-50 dark:bg-rose-950/20 border-rose-300 dark:border-rose-800/80 text-rose-800 dark:text-rose-200'
                        : 'bg-slate-50 dark:bg-slate-950/60 border-slate-200 dark:border-slate-800 text-slate-800 dark:text-slate-200'
                    }`}
                  >
                    <div className="flex items-center justify-between">
                      <span className="font-bold">{asset.name}</span>
                      <span className="font-mono text-[10px] bg-slate-200 dark:bg-slate-800 text-slate-700 dark:text-slate-400 px-1.5 py-0.5 rounded">
                        {asset.assetTag}
                      </span>
                    </div>
                    <div className="text-[11px] text-slate-500 dark:text-slate-400 mt-0.5">{asset.category}</div>

                    <div className="mt-1.5 grid grid-cols-2 gap-1 text-[10px] font-mono">
                      <div>
                        Insp:{' '}
                        <span className={asset.isInspectionValid ? 'text-emerald-600 dark:text-emerald-400 font-bold' : 'text-rose-600 dark:text-rose-400 font-bold'}>
                          {asset.isInspectionValid ? 'VALID' : 'OVERDUE'}
                        </span>
                      </div>
                      <div>
                        Calib:{' '}
                        <span className={asset.isCalibrationValid ? 'text-emerald-600 dark:text-emerald-400 font-bold' : 'text-rose-600 dark:text-rose-400 font-bold'}>
                          {asset.isCalibrationValid ? 'VALID' : 'EXPIRED'}
                        </span>
                      </div>
                    </div>
                  </div>
                ))
              )}
            </div>
          </div>
        </div>

        {/* Right Column: Live LangGraph Trace */}
        <div className="lg:col-span-2">
          <AgentExecutionTimeline
            traceJson={permit.workflowRun?.executionTraceJson}
            recommendedFixJson={permit.workflowRun?.recommendedFixJson}
          />
        </div>
      </div>

      {/* Safety Officer Sign-Off Modal */}
      {showSignOffModal && (
        <div className="fixed inset-0 z-50 flex items-center justify-center p-4 bg-slate-950/80 backdrop-blur-sm">
          <div className="bg-white dark:bg-slate-900 border border-slate-200 dark:border-slate-800 rounded-3xl max-w-lg w-full p-6 shadow-2xl space-y-4">
            <div className="flex items-center gap-2 text-base font-bold text-slate-900 dark:text-slate-100 pb-3 border-b border-slate-200 dark:border-slate-800">
              <FileCheck className="w-5 h-5 text-amber-500 dark:text-amber-400" />
              <span>Safety Officer Authoritative Sign-Off</span>
            </div>

            <p className="text-xs text-slate-600 dark:text-slate-400">
              You are certifying safety clearance for Permit <strong className="text-slate-900 dark:text-white font-mono">{permit.permitNumber}</strong> on {permit.zoneName}.
            </p>

            <div className="grid grid-cols-3 gap-2 text-xs">
              <button
                type="button"
                onClick={() => setDecisionAction('Approved')}
                className={`py-2 px-3 rounded-xl border font-bold flex items-center justify-center gap-1.5 transition ${
                  decisionAction === 'Approved'
                    ? 'bg-emerald-600 border-emerald-500 text-white shadow'
                    : 'bg-slate-100 dark:bg-slate-800 border-slate-200 dark:border-slate-700 text-slate-600 dark:text-slate-400'
                }`}
              >
                <CheckCircle2 className="w-3.5 h-3.5" /> Approve
              </button>

              <button
                type="button"
                onClick={() => setDecisionAction('Rejected')}
                className={`py-2 px-3 rounded-xl border font-bold flex items-center justify-center gap-1.5 transition ${
                  decisionAction === 'Rejected'
                    ? 'bg-rose-600 border-rose-500 text-white shadow'
                    : 'bg-slate-100 dark:bg-slate-800 border-slate-200 dark:border-slate-700 text-slate-600 dark:text-slate-400'
                }`}
              >
                <XCircle className="w-3.5 h-3.5" /> Reject
              </button>

              <button
                type="button"
                onClick={() => setDecisionAction('RevisionRequested')}
                className={`py-2 px-3 rounded-xl border font-bold flex items-center justify-center gap-1.5 transition ${
                  decisionAction === 'RevisionRequested'
                    ? 'bg-amber-600 border-amber-500 text-white shadow'
                    : 'bg-slate-100 dark:bg-slate-800 border-slate-200 dark:border-slate-700 text-slate-600 dark:text-slate-400'
                }`}
              >
                <RotateCcw className="w-3.5 h-3.5" /> Revise
              </button>
            </div>

            <div>
              <label className="block text-xs font-semibold text-slate-700 dark:text-slate-300 mb-1">
                Auditable Decision Notes & Instructions
              </label>
              <textarea
                rows={3}
                value={decisionNotes}
                onChange={(e) => setDecisionNotes(e.target.value)}
                placeholder="State basis for decision (e.g. Verified replacement welder W-1204 and rescheduled outside adjacent solvent window)..."
                className="w-full p-3 bg-slate-50 dark:bg-slate-950 border border-slate-300 dark:border-slate-700 rounded-xl text-xs text-slate-900 dark:text-white placeholder-slate-400 dark:placeholder-slate-500 focus:outline-none focus:ring-1 focus:ring-amber-500"
              />
            </div>

            <div className="flex items-center justify-end gap-3 pt-3 border-t border-slate-200 dark:border-slate-800">
              <button
                type="button"
                onClick={() => setShowSignOffModal(false)}
                className="px-4 py-2 bg-slate-100 dark:bg-slate-800 hover:bg-slate-200 dark:hover:bg-slate-700 text-slate-700 dark:text-slate-300 text-xs font-semibold rounded-xl transition"
              >
                Cancel
              </button>
              <button
                type="button"
                disabled={isDeciding}
                onClick={handleDecisionSubmit}
                className="px-4 py-2 bg-amber-500 hover:bg-amber-400 text-slate-950 text-xs font-bold rounded-xl transition disabled:opacity-50 shadow-sm"
              >
                {isDeciding ? 'Submitting Signature...' : 'Confirm Decision'}
              </button>
            </div>
          </div>
        </div>
      )}
    </div>
  );
};