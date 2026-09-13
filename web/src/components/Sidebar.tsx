import React from 'react';
import { NavLink } from 'react-router-dom';
import { useSelector } from 'react-redux';
import type { RootState } from '../store';
import {
  FileCheck2,
  Users2,
  Wrench,
  AlertTriangle,
  BarChart3,
  Sliders,
} from 'lucide-react';

export const Sidebar: React.FC = () => {
  const user = useSelector((state: RootState) => state.auth.user);
  const isSafetyOfficer = user?.role === 'SafetyOfficer';
  const isAdmin = user?.role === 'Administrator';

  const navItems = [
    {
      to: '/',
      label: 'Permits & Clearance',
      icon: FileCheck2,
      desc: 'Active reviews & submissions',
    },
    {
      to: '/analytics',
      label: 'Safety Analytics',
      icon: BarChart3,
      desc: 'Refusal causes & metrics',
      restricted: !isSafetyOfficer && !isAdmin,
    },
    {
      to: '/workforce',
      label: 'Workforce Competency',
      icon: Users2,
      desc: 'Certifications & 30-day expiry',
    },
    {
      to: '/equipment',
      label: 'Equipment Readiness',
      icon: Wrench,
      desc: 'Inspections & gas calibration',
    },
    {
      to: '/hazard-rules',
      label: 'Hazard Rulebook',
      icon: AlertTriangle,
      desc: 'SIMOPS matrix & weather limits',
    },
  ];

  return (
    <aside className="w-64 bg-slate-900 border-r border-slate-800 flex flex-col shrink-0 min-h-[calc(100vh-4rem)]">
      <div className="p-4 space-y-1">
        <div className="px-3 py-2 text-xs font-bold text-slate-500 uppercase tracking-wider">
          Safety Operations
        </div>
        {navItems
          .filter((item) => !item.restricted)
          .map((item) => {
            const Icon = item.icon;
            return (
              <NavLink
                key={item.to}
                to={item.to}
                end={item.to === '/'}
                className={({ isActive }) =>
                  `flex items-start space-x-3 px-3 py-2.5 rounded-lg text-sm transition-colors ${
                    isActive
                      ? 'bg-amber-500/15 text-amber-400 border border-amber-500/30 font-semibold'
                      : 'text-slate-400 hover:text-slate-200 hover:bg-slate-800'
                  }`
                }
              >
                <Icon className="w-5 h-5 shrink-0 mt-0.5" />
                <div className="truncate">
                  <div className="leading-tight">{item.label}</div>
                  <div className="text-[11px] text-slate-500 font-normal leading-tight mt-0.5">
                    {item.desc}
                  </div>
                </div>
              </NavLink>
            );
          })}
      </div>

      <div className="mt-auto p-4 border-t border-slate-800/80">
        <div className="bg-slate-950 p-3 rounded-lg border border-slate-800 text-xs">
          <div className="flex items-center gap-1.5 text-amber-400 font-semibold mb-1">
            <Sliders className="w-3.5 h-3.5" />
            <span>Agentic Clearance Engine</span>
          </div>
          <p className="text-slate-400 text-[11px] leading-relaxed">
            Multi-Agent StateGraph active. Deterministic safety verification enforced prior to human HSE sign-off.
          </p>
        </div>
      </div>
    </aside>
  );
};
