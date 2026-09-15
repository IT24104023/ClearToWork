import React, { useState } from 'react';
import { useTranslation } from '../context/I18nContext';
import {
  BookOpen,
  Layers,
  Bot,
  Shield,
  Database
} from 'lucide-react';

export const DocsViewerPage: React.FC = () => {
  const { t } = useTranslation();
  const [activeTab, setActiveTab] = useState<'arch' | 'agents' | 'security' | 'db'>('arch');

  return (
    <div className="p-6 space-y-6 max-w-6xl mx-auto">
      {/* Header */}
      <div className="flex items-center space-x-3 border-b border-slate-200 dark:border-slate-800 pb-4">
        <div className="bg-sky-500/20 text-sky-600 dark:text-sky-400 p-2.5 rounded-xl border border-sky-500/30">
          <BookOpen className="w-6 h-6" />
        </div>
        <div>
          <h1 className="text-2xl font-bold text-slate-900 dark:text-white tracking-tight">{t('docs_title')}</h1>
          <p className="text-xs text-slate-500 dark:text-slate-400 mt-0.5">{t('docs_subtitle')}</p>
        </div>
      </div>

      {/* Navigation Tabs */}
      <div className="flex flex-wrap space-x-2 border-b border-slate-200 dark:border-slate-800">
        <button
          onClick={() => setActiveTab('arch')}
          className={`px-4 py-2.5 text-xs font-bold rounded-t-xl border-b-2 transition-all flex items-center gap-2 ${
            activeTab === 'arch'
              ? 'border-amber-500 text-amber-600 dark:text-amber-400 bg-amber-500/10 dark:bg-slate-900/60'
              : 'border-transparent text-slate-500 hover:text-slate-900 dark:text-slate-400 dark:hover:text-white'
          }`}
        >
          <Layers className="w-4 h-4" />
          <span>{t('docs_tab_architecture')}</span>
        </button>

        <button
          onClick={() => setActiveTab('agents')}
          className={`px-4 py-2.5 text-xs font-bold rounded-t-xl border-b-2 transition-all flex items-center gap-2 ${
            activeTab === 'agents'
              ? 'border-amber-500 text-amber-600 dark:text-amber-400 bg-amber-500/10 dark:bg-slate-900/60'
              : 'border-transparent text-slate-500 hover:text-slate-900 dark:text-slate-400 dark:hover:text-white'
          }`}
        >
          <Bot className="w-4 h-4" />
          <span>{t('docs_tab_agents')}</span>
        </button>

        <button
          onClick={() => setActiveTab('security')}
          className={`px-4 py-2.5 text-xs font-bold rounded-t-xl border-b-2 transition-all flex items-center gap-2 ${
            activeTab === 'security'
              ? 'border-amber-500 text-amber-600 dark:text-amber-400 bg-amber-500/10 dark:bg-slate-900/60'
              : 'border-transparent text-slate-500 hover:text-slate-900 dark:text-slate-400 dark:hover:text-white'
          }`}
        >
          <Shield className="w-4 h-4" />
          <span>{t('docs_tab_security')}</span>
        </button>

        <button
          onClick={() => setActiveTab('db')}
          className={`px-4 py-2.5 text-xs font-bold rounded-t-xl border-b-2 transition-all flex items-center gap-2 ${
            activeTab === 'db'
              ? 'border-amber-500 text-amber-600 dark:text-amber-400 bg-amber-500/10 dark:bg-slate-900/60'
              : 'border-transparent text-slate-500 hover:text-slate-900 dark:text-slate-400 dark:hover:text-white'
          }`}
        >
          <Database className="w-4 h-4" />
          <span>{t('docs_tab_database')}</span>
        </button>
      </div>

      {/* Tab 1: System Architecture */}
      {activeTab === 'arch' && (
        <div className="bg-white dark:bg-slate-900 p-6 rounded-2xl border border-slate-200 dark:border-slate-800 space-y-4 text-xs text-slate-700 dark:text-slate-300 leading-relaxed shadow-sm">
          <h2 className="text-base font-bold text-slate-900 dark:text-white flex items-center gap-2">
            <Layers className="w-4 h-4 text-amber-500 dark:text-amber-400" />
            <span>4-Tier Integrated Architecture</span>
          </h2>
          <p>
            ClearToWork AI strictly enforces clean separation of concerns across presentation, application gateway, relational storage, and an autonomous multi-agent evaluation subsystem.
          </p>

          <div className="grid grid-cols-1 md:grid-cols-4 gap-3 my-4">
            <div className="p-3.5 bg-slate-50 dark:bg-slate-950 border border-slate-200 dark:border-slate-800 rounded-xl">
              <div className="text-[10px] font-mono text-amber-600 dark:text-amber-400 font-bold uppercase mb-1">Tier 1: Client</div>
              <div className="font-bold text-slate-900 dark:text-white mb-1">React + Vite</div>
              <p className="text-[11px] text-slate-600 dark:text-slate-400">
                Redux Toolkit, trilingual i18n (EN/SI/TA), dark/light theming, role-guarded routes.
              </p>
            </div>

            <div className="p-3.5 bg-slate-50 dark:bg-slate-950 border border-slate-200 dark:border-slate-800 rounded-xl">
              <div className="text-[10px] font-mono text-sky-600 dark:text-sky-400 font-bold uppercase mb-1">Tier 2: Backend</div>
              <div className="font-bold text-slate-900 dark:text-white mb-1">ASP.NET Core 8 API</div>
              <p className="text-[11px] text-slate-600 dark:text-slate-400">
                JWT bearer auth, deterministic rule engine, Open-Meteo cache & retry handler.
              </p>
            </div>

            <div className="p-3.5 bg-slate-50 dark:bg-slate-950 border border-slate-200 dark:border-slate-800 rounded-xl">
              <div className="text-[10px] font-mono text-emerald-600 dark:text-emerald-400 font-bold uppercase mb-1">Tier 3: Database</div>
              <div className="font-bold text-slate-900 dark:text-white mb-1">EF Core + Relational</div>
              <p className="text-[11px] text-slate-600 dark:text-slate-400">
                Normalized entities, foreign key constraints, transactional asset reservations.
              </p>
            </div>

            <div className="p-3.5 bg-slate-50 dark:bg-slate-950 border border-slate-200 dark:border-slate-800 rounded-xl">
              <div className="text-[10px] font-mono text-purple-600 dark:text-purple-400 font-bold uppercase mb-1">Tier 4: Agentic AI</div>
              <div className="font-bold text-slate-900 dark:text-white mb-1">LangGraph DAG</div>
              <p className="text-[11px] text-slate-600 dark:text-slate-400">
                5 distinct safety agents, shared state graph, X-Agent-Secret isolation.
              </p>
            </div>
          </div>

          <div className="p-4 bg-slate-50 dark:bg-slate-950/60 border border-slate-200 dark:border-slate-800 rounded-xl">
            <div className="font-bold text-slate-900 dark:text-white mb-1">Third-Party Integration: Open-Meteo API</div>
            <p className="text-[11px] text-slate-600 dark:text-slate-400">
              Queries real-time meteorological conditions (wind speed, wind gusts, rain probability) without paid API keys. Enforces strict safety rules: elevated hot work and crane lifts are refused when gusts exceed 35.0 km/h.
            </p>
          </div>
        </div>
      )}

      {/* Tab 2: Agentic Workflow */}
      {activeTab === 'agents' && (
        <div className="bg-white dark:bg-slate-900 p-6 rounded-2xl border border-slate-200 dark:border-slate-800 space-y-4 text-xs text-slate-700 dark:text-slate-300 leading-relaxed shadow-sm">
          <h2 className="text-base font-bold text-slate-900 dark:text-white flex items-center gap-2">
            <Bot className="w-4 h-4 text-amber-500 dark:text-amber-400" />
            <span>5-Agent LangGraph Pipeline Specification</span>
          </h2>
          <p>
            To prevent generative hallucinations and satisfy the SE3090 marking rubric, each agent has a unique role, owner, and tool allow-list:
          </p>

          <div className="space-y-3 mt-3">
            <div className="p-3.5 bg-slate-50 dark:bg-slate-950 border border-slate-200 dark:border-slate-800 rounded-xl flex items-start justify-between">
              <div>
                <div className="font-bold text-slate-900 dark:text-white text-xs">1. Planning & Coordination Agent (Student 3)</div>
                <div className="text-[11px] text-slate-600 dark:text-slate-400 mt-0.5">
                  Decomposes permit objectives, checks permit type limits, and mandates fire watch.
                </div>
                <div className="text-[10px] font-mono text-sky-600 dark:text-sky-400 mt-1">Tools: get_permit_type_template()</div>
              </div>
              <span className="text-[10px] bg-sky-100 dark:bg-sky-950 text-sky-700 dark:text-sky-300 px-2 py-0.5 rounded font-mono">Node 1</span>
            </div>

            <div className="p-3.5 bg-slate-50 dark:bg-slate-950 border border-slate-200 dark:border-slate-800 rounded-xl flex items-start justify-between">
              <div>
                <div className="font-bold text-slate-900 dark:text-white text-xs">2. Personnel & Competency Agent (Student 1)</div>
                <div className="text-[11px] text-slate-600 dark:text-slate-400 mt-0.5">
                  Audits worker badges against national trade certifications. Flags expiries and suggests qualified replacement workers.
                </div>
                <div className="text-[10px] font-mono text-sky-600 dark:text-sky-400 mt-1">Tools: get_worker_certificates(), find_eligible_workers()</div>
              </div>
              <span className="text-[10px] bg-sky-100 dark:bg-sky-950 text-sky-700 dark:text-sky-300 px-2 py-0.5 rounded font-mono">Node 2</span>
            </div>

            <div className="p-3.5 bg-slate-50 dark:bg-slate-950 border border-slate-200 dark:border-slate-800 rounded-xl flex items-start justify-between">
              <div>
                <div className="font-bold text-slate-900 dark:text-white text-xs">3. Resource & Isolation Agent (Student 2)</div>
                <div className="text-[11px] text-slate-600 dark:text-slate-400 mt-0.5">
                  Audits equipment calibration and monthly inspections. Queries Lock-Out / Tag-Out (LOTO) isolation points.
                </div>
                <div className="text-[10px] font-mono text-sky-600 dark:text-sky-400 mt-1">Tools: check_equipment_readiness(), get_isolation_points()</div>
              </div>
              <span className="text-[10px] bg-sky-100 dark:bg-sky-950 text-sky-700 dark:text-sky-300 px-2 py-0.5 rounded font-mono">Node 3</span>
            </div>

            <div className="p-3.5 bg-slate-50 dark:bg-slate-950 border border-slate-200 dark:border-slate-800 rounded-xl flex items-start justify-between">
              <div>
                <div className="font-bold text-slate-900 dark:text-white text-xs">4. Site Conditions & Hazard Control Agent (Student 4)</div>
                <div className="text-[11px] text-slate-600 dark:text-slate-400 mt-0.5">
                  Evaluates SIMOPS clashes across adjacent zones (e.g. Hot Work vs Solvent Painting). Evaluates live Open-Meteo wind gusts.
                </div>
                <div className="text-[10px] font-mono text-sky-600 dark:text-sky-400 mt-1">Tools: get_zone_conflicts(), get_weather_forecast()</div>
              </div>
              <span className="text-[10px] bg-sky-100 dark:bg-sky-950 text-sky-700 dark:text-sky-300 px-2 py-0.5 rounded font-mono">Node 4</span>
            </div>

            <div className="p-3.5 bg-slate-50 dark:bg-slate-950 border border-slate-200 dark:border-slate-800 rounded-xl flex items-start justify-between">
              <div>
                <div className="font-bold text-slate-900 dark:text-white text-xs">5. Validation & Safety Agent (Shared Engine)</div>
                <div className="text-[11px] text-slate-600 dark:text-slate-400 mt-0.5">
                  Enforces fail-safe clearance. If any violation is found, returns REFUSED_SAFE_FAILURE with actionable remediation.
                </div>
                <div className="text-[10px] font-mono text-sky-600 dark:text-sky-400 mt-1">Tools: run_permit_validator()</div>
              </div>
              <span className="text-[10px] bg-emerald-100 dark:bg-emerald-950 text-emerald-700 dark:text-emerald-300 px-2 py-0.5 rounded font-mono">Node 5 (Guardrail)</span>
            </div>
          </div>
        </div>
      )}

      {/* Tab 3: Security & Auth */}
      {activeTab === 'security' && (
        <div className="bg-white dark:bg-slate-900 p-6 rounded-2xl border border-slate-200 dark:border-slate-800 space-y-4 text-xs text-slate-700 dark:text-slate-300 leading-relaxed shadow-sm">
          <h2 className="text-base font-bold text-slate-900 dark:text-white flex items-center gap-2">
            <Shield className="w-4 h-4 text-amber-500 dark:text-amber-400" />
            <span>Security Architecture & Cryptographic RBAC</span>
          </h2>
          <p>
            ClearToWork AI applies defense-in-depth principles across authentication, authorization, and network service isolation.
          </p>

          <div className="space-y-3">
            <div className="p-4 bg-slate-50 dark:bg-slate-950 border border-slate-200 dark:border-slate-800 rounded-xl">
              <div className="font-bold text-slate-900 dark:text-white mb-1">1. JWT Bearer Tokens with HMAC-SHA256</div>
              <p className="text-[11px] text-slate-600 dark:text-slate-400">
                User claims include Subject ID, Full Name, Email, Role, and Contractor ID. Tokens expire in 7 days with 5-minute clock-skew validation.
              </p>
            </div>

            <div className="p-4 bg-slate-50 dark:bg-slate-950 border border-slate-200 dark:border-slate-800 rounded-xl">
              <div className="font-bold text-slate-900 dark:text-white mb-1">2. Internal Agent Subsystem Isolation</div>
              <p className="text-[11px] text-slate-600 dark:text-slate-400">
                The Python LangGraph microservice is bound strictly to internal addresses and verifies an internal cryptographic header (<code>X-Agent-Secret</code>). Direct client access from React or Flutter is prohibited.
              </p>
            </div>

            <div className="p-4 bg-slate-50 dark:bg-slate-950 border border-slate-200 dark:border-slate-800 rounded-xl">
              <div className="font-bold text-slate-900 dark:text-white mb-1">3. Immutable Safety Audit Log</div>
              <p className="text-[11px] text-slate-600 dark:text-slate-400">
                Every agent evaluation run, tool call latency, input parameter, and human safety officer sign-off is permanently persisted in the relational database with UTC timestamps.
              </p>
            </div>
          </div>
        </div>
      )}

      {/* Tab 4: Database ERD */}
      {activeTab === 'db' && (
        <div className="bg-white dark:bg-slate-900 p-6 rounded-2xl border border-slate-200 dark:border-slate-800 space-y-4 text-xs text-slate-700 dark:text-slate-300 leading-relaxed shadow-sm">
          <h2 className="text-base font-bold text-slate-900 dark:text-white flex items-center gap-2">
            <Database className="w-4 h-4 text-amber-500 dark:text-amber-400" />
            <span>Relational Schema & Entity Relationships</span>
          </h2>
          <p>
            The normalized schema features strict foreign keys, transactional equipment reservations, and comprehensive auditing:
          </p>

          <div className="grid grid-cols-1 sm:grid-cols-2 gap-3 my-3">
            <div className="p-3 bg-slate-50 dark:bg-slate-950 border border-slate-200 dark:border-slate-800 rounded-xl">
              <div className="font-bold text-slate-900 dark:text-white text-xs">Workforce Domain (Student 1)</div>
              <div className="text-[11px] text-slate-600 dark:text-slate-400 mt-1 font-mono">
                Contractors ➔ Workers ➔ WorkerCertificates ➔ CertificateTypes
              </div>
            </div>

            <div className="p-3 bg-slate-50 dark:bg-slate-950 border border-slate-200 dark:border-slate-800 rounded-xl">
              <div className="font-bold text-slate-900 dark:text-white text-xs">Equipment Domain (Student 2)</div>
              <div className="text-[11px] text-slate-600 dark:text-slate-400 mt-1 font-mono">
                Assets ➔ InspectionRecords, CalibrationRecords, IsolationPoints
              </div>
            </div>

            <div className="p-3 bg-slate-50 dark:bg-slate-950 border border-slate-200 dark:border-slate-800 rounded-xl">
              <div className="font-bold text-slate-900 dark:text-white text-xs">Permits Domain (Student 3)</div>
              <div className="text-[11px] text-slate-600 dark:text-slate-400 mt-1 font-mono">
                PermitRequests ➔ PermitWorkers, PermitAssets, Approvals, EvidencePhotos
              </div>
            </div>

            <div className="p-3 bg-slate-50 dark:bg-slate-950 border border-slate-200 dark:border-slate-800 rounded-xl">
              <div className="font-bold text-slate-900 dark:text-white text-xs">Hazards & SIMOPS (Student 4)</div>
              <div className="text-[11px] text-slate-600 dark:text-slate-400 mt-1 font-mono">
                Sites ➔ Zones ➔ ZoneAdjacencies, HazardTypes ➔ HazardRules
              </div>
            </div>
          </div>
        </div>
      )}
    </div>
  );
};
