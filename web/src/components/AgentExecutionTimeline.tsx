import React from 'react';
import { useTranslation } from '../context/I18nContext';
import { CheckCircle2, XCircle, Clock, Cpu, ArrowRight } from 'lucide-react';

interface AgentExecutionTimelineProps {
  traceJson?: string;
  recommendedFixJson?: string;
}

export const AgentExecutionTimeline: React.FC<AgentExecutionTimelineProps> = ({
  traceJson,
  recommendedFixJson,
}) => {
  const { t } = useTranslation();

  if (!traceJson) {
    return (
      <div className="p-6 bg-white dark:bg-slate-900/60 rounded-2xl border border-slate-200 dark:border-slate-800 text-center text-slate-500 dark:text-slate-400 text-sm shadow-sm">
        No agent execution trace recorded for this permit yet. Submit the permit to trigger the LangGraph orchestration pipeline.
      </div>
    );
  }

  let traceData: any = {};
  let proposedFix: any = null;

  try {
    traceData = typeof traceJson === 'string' ? JSON.parse(traceJson) : traceJson;
  } catch {
    traceData = {};
  }

  try {
    if (recommendedFixJson) {
      proposedFix = typeof recommendedFixJson === 'string' ? JSON.parse(recommendedFixJson) : recommendedFixJson;
    }
  } catch {
    proposedFix = null;
  }

  const steps = traceData.agents_executed || traceData.execution_traces || traceData.step_traces || [];

  return (
    <div className="space-y-6">
      <div className="bg-white dark:bg-slate-900 border border-slate-200 dark:border-slate-800 rounded-2xl p-5 shadow-sm">
        <div className="flex flex-wrap items-center justify-between gap-4 pb-4 border-b border-slate-200 dark:border-slate-800">
          <div>
            <div className="flex items-center gap-2">
              <Cpu className="w-5 h-5 text-amber-500 dark:text-amber-400" />
              <h3 className="font-bold text-slate-900 dark:text-slate-100 text-base">{t('permit_timeline_title')}</h3>
            </div>
            <p className="text-xs text-slate-500 dark:text-slate-400 mt-1 font-mono">
              Workflow ID: {traceData.workflow_id || 'LOCAL-EXEC-001'}
            </p>
          </div>
          <div className="flex items-center gap-2 text-xs">
            <span className="text-slate-500 dark:text-slate-400">{t('permit_timeline_latency')}</span>
            <span className="font-mono text-amber-600 dark:text-amber-400 font-bold">
              {steps.reduce((acc: number, curr: any) => acc + (curr.latency_ms || 0), 0)} ms
            </span>
          </div>
        </div>

        {/* Timeline Items */}
        <div className="mt-6 space-y-4 relative before:absolute before:inset-0 before:left-3.5 before:w-0.5 before:bg-slate-200 dark:before:bg-slate-800">
          {steps.map((step: any, idx: number) => {
            const hasFailure = step.findings && step.findings.some((f: string) => f.toLowerCase().includes('expired') || f.toLowerCase().includes('clash') || f.toLowerCase().includes('overdue') || f.toLowerCase().includes('exceed'));
            const isValidator = step.agent?.includes('Validation') || step.agent_name?.includes('Validation');
            const toolName = step.tool || (step.tools_called && step.tools_called[0]?.tool_name);

            return (
              <div key={idx} className="relative flex items-start space-x-4 pl-1">
                <div
                  className={`relative z-10 w-6 h-6 rounded-full flex items-center justify-center shrink-0 border-2 ${
                    hasFailure
                      ? 'bg-rose-100 dark:bg-rose-950 border-rose-500 text-rose-600 dark:text-rose-400'
                      : isValidator
                      ? 'bg-amber-100 dark:bg-amber-950 border-amber-500 text-amber-600 dark:text-amber-400'
                      : 'bg-emerald-100 dark:bg-emerald-950 border-emerald-500 text-emerald-600 dark:text-emerald-400'
                  }`}
                >
                  {hasFailure ? (
                    <XCircle className="w-3.5 h-3.5" />
                  ) : (
                    <CheckCircle2 className="w-3.5 h-3.5" />
                  )}
                </div>

                <div className="flex-1 bg-slate-50 dark:bg-slate-950/70 border border-slate-200 dark:border-slate-800 rounded-xl p-3.5 hover:border-slate-300 dark:hover:border-slate-700 transition-colors shadow-sm">
                  <div className="flex flex-wrap items-center justify-between gap-2">
                    <div className="flex items-center gap-2">
                      <span className="font-semibold text-slate-900 dark:text-slate-200 text-sm">{step.agent || step.agent_name}</span>
                      <span className="text-xs bg-slate-200 dark:bg-slate-800 text-slate-700 dark:text-slate-400 px-2 py-0.5 rounded-md font-mono">
                        {step.owner}
                      </span>
                    </div>
                    <div className="flex items-center gap-3 text-xs text-slate-500 dark:text-slate-400">
                      {toolName && (
                        <span className="font-mono text-amber-700 dark:text-amber-300 bg-amber-100 dark:bg-amber-950/40 border border-amber-300 dark:border-amber-900/50 px-2 py-0.5 rounded-md">
                          tool: {toolName}
                        </span>
                      )}
                      <span className="flex items-center gap-1 font-mono text-slate-500 dark:text-slate-400">
                        <Clock className="w-3 h-3" />
                        {Math.round(step.latency_ms || 0)}ms
                      </span>
                    </div>
                  </div>

                  {step.verdict && (
                    <div className="mt-2 text-xs">
                      <span className="text-slate-500 dark:text-slate-400">Deterministic Rule Engine Verdict: </span>
                      <span className={`font-bold font-mono px-2 py-0.5 rounded ${step.verdict === 'CLEAR' ? 'bg-emerald-100 dark:bg-emerald-950 text-emerald-700 dark:text-emerald-400 border border-emerald-300 dark:border-emerald-800' : 'bg-rose-100 dark:bg-rose-950 text-rose-700 dark:text-rose-400 border border-rose-300 dark:border-rose-800'}`}>
                        {step.verdict}
                      </span>
                    </div>
                  )}

                  {step.findings && step.findings.length > 0 && (
                    <div className="mt-2.5 pt-2.5 border-t border-slate-200 dark:border-slate-800/80 space-y-1">
                      {step.findings.map((finding: string, fIdx: number) => (
                        <div
                          key={fIdx}
                          className={`text-xs flex items-start gap-1.5 ${
                            finding.toLowerCase().includes('expired') || finding.toLowerCase().includes('clash') || finding.toLowerCase().includes('overdue') || finding.toLowerCase().includes('exceed')
                              ? 'text-rose-700 dark:text-rose-300 font-medium'
                              : 'text-slate-700 dark:text-slate-300'
                          }`}
                        >
                          <ArrowRight className="w-3 h-3 mt-0.5 shrink-0 opacity-60" />
                          <span>{finding}</span>
                        </div>
                      ))}
                    </div>
                  )}
                </div>
              </div>
            );
          })}
        </div>
      </div>

      {/* Recommended Fix Box */}
      {proposedFix && (
        <div className="bg-amber-50 dark:bg-amber-950/20 border border-amber-300 dark:border-amber-500/40 rounded-2xl p-5 shadow-sm">
          <div className="flex items-center gap-2 text-amber-700 dark:text-amber-400 font-bold text-sm mb-3">
            <Cpu className="w-4 h-4" />
            <span>{t('permit_remediation_title')}</span>
          </div>

          <div className="grid grid-cols-1 md:grid-cols-3 gap-3 text-xs mb-3">
            {(proposedFix.suggestedWorkerBadge || proposedFix.SuggestedWorkerBadge) && (
              <div className="bg-white dark:bg-slate-900/80 p-3 rounded-xl border border-slate-200 dark:border-slate-800">
                <div className="text-slate-500 dark:text-slate-400 font-semibold mb-0.5">Certified Worker Replacement</div>
                <div className="text-emerald-600 dark:text-emerald-400 font-mono font-bold">
                  {proposedFix.suggestedWorkerBadge || proposedFix.SuggestedWorkerBadge}
                </div>
              </div>
            )}
            {(proposedFix.suggestedAssetTag || proposedFix.SuggestedAssetTag) && (
              <div className="bg-white dark:bg-slate-900/80 p-3 rounded-xl border border-slate-200 dark:border-slate-800">
                <div className="text-slate-500 dark:text-slate-400 font-semibold mb-0.5">In-Date Equipment Substitute</div>
                <div className="text-emerald-600 dark:text-emerald-400 font-mono font-bold">
                  {proposedFix.suggestedAssetTag || proposedFix.SuggestedAssetTag}
                </div>
              </div>
            )}
            {(proposedFix.suggestedTimeWindow || proposedFix.SuggestedTimeWindow) && (
              <div className="bg-white dark:bg-slate-900/80 p-3 rounded-xl border border-slate-200 dark:border-slate-800">
                <div className="text-slate-500 dark:text-slate-400 font-semibold mb-0.5">Cleared SIMOPS Window</div>
                <div className="text-emerald-600 dark:text-emerald-400 font-mono font-bold">
                  {proposedFix.suggestedTimeWindow || proposedFix.SuggestedTimeWindow}
                </div>
              </div>
            )}
          </div>

          {(proposedFix.summaryExplanation || proposedFix.SummaryExplanation) && (
            <p className="text-slate-700 dark:text-slate-300 text-xs leading-relaxed bg-white/60 dark:bg-slate-900/50 p-3 rounded-xl border border-slate-200 dark:border-slate-800">
              {proposedFix.summaryExplanation || proposedFix.SummaryExplanation}
            </p>
          )}
        </div>
      )}
    </div>
  );
};
