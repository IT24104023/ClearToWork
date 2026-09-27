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
  Sliders,
  X
} from 'lucide-react';

interface SidebarProps {
  isOpen?: boolean;
  onClose?: () => void;
}

interface NavItem {
  to: string;
  label: string;
  icon: React.ComponentType<{ className?: string }>;
  desc: string;
  restricted?: boolean;
}

export const Sidebar: React.FC<SidebarProps> = ({
  isOpen = false,
  onClose,
}) => {
  const user = useSelector((state: RootState) => state.auth.user);
  const { t } = useTranslation();

  const isAdmin = user?.role === 'Administrator';

  const operationsItems: NavItem[] = [
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
      desc: 'Zones & observations',
    },
    {
      to: '/hazard-rules/rulebook',
      label: t('nav_rulebook_editor'),
      icon: BookOpen,
      desc: 'Hazards & control measures',
    },
    {
      to: '/hazard-rules/matrix',
      label: t('nav_conflict_matrix'),
      icon: Sliders,
      desc: 'SIMOPS collision matrix',
    },
  ];

  const adminItems: NavItem[] = [
    {
      to: '/admin/agents',
      label: t('nav_agent_command'),
      icon: Bot,
      desc: '5-Agent monitor & QChat',
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

  const sidebarContent = (
    <div className="flex flex-col h-full justify-between">
      <div className="p-4 space-y-4">
        {/* Mobile Header Close Button */}
        <div className="md:hidden flex items-center justify-between pb-2 border-b border-slate-200 dark:border-slate-800">
          <span className="text-xs font-bold uppercase tracking-wider text-amber-600 dark:text-amber-400">
            ClearToWork Portal
          </span>
          {onClose && (
            <button
              onClick={onClose}
              className="p-1.5 rounded-lg text-slate-500 hover:bg-slate-100 dark:hover:bg-slate-800"
            >
              <X className="w-5 h-5" />
            </button>
          )}
        </div>

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
                    onClick={() => {
                      if (onClose) onClose();
                    }}
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
                    onClick={() => {
                      if (onClose) onClose();
                    }}
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
      <div className="p-4 border-t border-slate-200 dark:border-slate-800/80">
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
    </div>
  );

  return (
    <>
      {/* Desktop Sidebar */}
      <aside className="hidden md:flex w-64 bg-white dark:bg-slate-950 border-r border-slate-200 dark:border-slate-800/80 flex-col shrink-0 min-h-[calc(100vh-4rem)] transition-colors">
        {sidebarContent}
      </aside>

      {/* Mobile Drawer Backdrop & Drawer */}
      {isOpen && (
        <div className="md:hidden fixed inset-0 z-50 flex">
          {/* Backdrop */}
          <div
            className="fixed inset-0 bg-slate-950/70 backdrop-blur-sm animate-in fade-in"
            onClick={onClose}
          />
          {/* Drawer Panel */}
          <div className="relative w-72 max-w-[85vw] bg-white dark:bg-slate-950 border-r border-slate-200 dark:border-slate-800 z-10 flex flex-col h-full shadow-2xl animate-in slide-in-from-left duration-200">
            {sidebarContent}
          </div>
        </div>
      )}
    </>
  );
};

export default Sidebar;
