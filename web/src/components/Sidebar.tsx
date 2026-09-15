import React from 'react';
import { NavLink } from 'react-router-dom';
import { useSelector } from 'react-redux';
import type { RootState } from '../store';
import { useTranslation } from '../context/I18nContext';
import {
  FileCheck2,
  Users2,
  Wrench,
  AlertTriangle,
  BarChart3,
  Bot,
  Database,
  UserCheck,
  BookOpen,
  Sliders
} from 'lucide-react';

export const Sidebar: React.FC = () => {
  const user = useSelector((state: RootState) => state.auth.user);
  const { t } = useTranslation();

  const isSafetyOfficer = user?.role === 'SafetyOfficer';
  const isAdmin = user?.role === 'Administrator';
  const isAreaSupervisor = user?.role === 'AreaSupervisor';

  const operationsItems = [
    {
      to: '/permits',
      label: t('nav_permits'),
      icon: FileCheck2,
      desc: 'Reviews, draft & submissions',
    },
    {
      to: '/analytics',
      label: t('nav_analytics'),
      icon: BarChart3,
      desc: 'Safe failures & causes',
      restricted: !isSafetyOfficer && !isAdmin && !isAreaSupervisor,
    },
    {
      to: '/workforce',
      label: t('nav_workforce'),
      icon: Users2,
      desc: 'Competencies & certs',
    },
    {
      to: '/equipment',
      label: t('nav_equipment'),
      icon: Wrench,
      desc: 'Readiness & isolations',
    },
    {
      to: '/hazard-rules',
      label: t('nav_hazard_rules'),
      icon: AlertTriangle,
      desc: 'SIMOPS & weather limits',
    },
  ];

  const adminItems = [
    {
      to: '/admin/agents',
      label: t('nav_agent_command'),
      icon: Bot,
      desc: '5-Agent monitor & QChat',
      restricted: !isAdmin && !isSafetyOfficer,
    },
    {
      to: '/admin/database',
      label: t('nav_database_admin'),
      icon: Database,
      desc: 'Tables, seed & reset',
      restricted: !isAdmin,
    },
    {
      to: '/profile',
      label: t('nav_profile'),
      icon: UserCheck,
      desc: 'Avatar & RBAC claims',
    },
    {
      to: '/docs',
      label: t('nav_documentation'),
      icon: BookOpen,
      desc: 'Architecture & ADR specs',
    },
  ];

  return (
    <aside className="w-64 bg-white dark:bg-slate-950 border-r border-slate-200 dark:border-slate-800/80 flex flex-col shrink-0 min-h-[calc(100vh-4rem)] transition-colors">
      <div className="p-4 space-y-4">
        {/* Section 1: Operations */}
        <div>
          <div className="px-3 py-1.5 text-[11px] font-bold text-slate-500 dark:text-slate-500 uppercase tracking-wider">
            Safety Operations
          </div>
          <div className="space-y-1 mt-1">
            {operationsItems
              .filter((item) => !item.restricted)
              .map((item) => {
                const Icon = item.icon;
                return (
                  <NavLink
                    key={item.to}
                    to={item.to}
                    end={item.to === '/'}
                    className={({ isActive }) =>
                      `flex items-start space-x-3 px-3 py-2.5 rounded-xl text-sm transition-all ${
                        isActive
                          ? 'bg-amber-500/15 text-amber-600 dark:text-amber-400 border border-amber-500/30 font-semibold shadow-sm'
                          : 'text-slate-700 dark:text-slate-400 hover:text-slate-950 dark:hover:text-white hover:bg-slate-100 dark:hover:bg-slate-900/60'
                      }`
                    }
                  >
                    <Icon className="w-4 h-4 shrink-0 mt-0.5 text-amber-500 dark:text-amber-400/90" />
                    <div className="truncate">
                      <div className="leading-tight">{item.label}</div>
                      <div className="text-[11px] text-slate-500 dark:text-slate-500 font-normal leading-tight mt-0.5">
                        {item.desc}
                      </div>
                    </div>
                  </NavLink>
                );
              })}
          </div>
        </div>

        {/* Section 2: Administration & System */}
        <div>
          <div className="px-3 py-1.5 text-[11px] font-bold text-slate-500 dark:text-slate-500 uppercase tracking-wider">
            System & Command
          </div>
          <div className="space-y-1 mt-1">
            {adminItems
              .filter((item) => !item.restricted)
              .map((item) => {
                const Icon = item.icon;
                return (
                  <NavLink
                    key={item.to}
                    to={item.to}
                    className={({ isActive }) =>
                      `flex items-start space-x-3 px-3 py-2.5 rounded-xl text-sm transition-all ${
                        isActive
                          ? 'bg-sky-500/15 text-sky-600 dark:text-sky-400 border border-sky-500/30 font-semibold shadow-sm'
                          : 'text-slate-700 dark:text-slate-400 hover:text-slate-950 dark:hover:text-white hover:bg-slate-100 dark:hover:bg-slate-900/60'
                      }`
                    }
                  >
                    <Icon className="w-4 h-4 shrink-0 mt-0.5 text-sky-500 dark:text-sky-400/90" />
                    <div className="truncate">
                      <div className="leading-tight">{item.label}</div>
                      <div className="text-[11px] text-slate-500 dark:text-slate-500 font-normal leading-tight mt-0.5">
                        {item.desc}
                      </div>
                    </div>
                  </NavLink>
                );
              })}
          </div>
        </div>
      </div>

      {/* Footer Info */}
      <div className="mt-auto p-4 border-t border-slate-200 dark:border-slate-800/80">
        <div className="bg-slate-50 dark:bg-slate-900/70 p-3 rounded-xl border border-slate-200 dark:border-slate-800 text-xs">
          <div className="flex items-center gap-1.5 text-amber-600 dark:text-amber-400 font-semibold mb-1">
            <Sliders className="w-3.5 h-3.5" />
            <span>Multi-Agent Engine</span>
          </div>
          <p className="text-slate-600 dark:text-slate-400 text-[11px] leading-relaxed">
            LangGraph StateGraph active. Strict fail-closed clearance enforced.
          </p>
        </div>
      </div>
    </aside>
  );
};

export default Sidebar;
