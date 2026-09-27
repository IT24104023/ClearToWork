import React from 'react';
import { NavLink } from 'react-router-dom';
import { FileText, Users, ShieldAlert, Cpu, Layers, Grid, BarChart3, Wrench } from 'lucide-react';

interface SidebarProps {
  isOpen?: boolean;
  onClose?: () => void;
}

export const Sidebar: React.FC<SidebarProps> = ({ isOpen, onClose }) => {
  const links = [
    { to: '/permits', label: 'Permits Lifecycle', icon: FileText },
    { to: '/workforce', label: 'Workforce & Badges', icon: Users },
    { to: '/equipment', label: 'Equipment & Calibration', icon: Wrench },
    { to: '/hazard-rules', label: 'Hazard & SIMOPS Rules', icon: ShieldAlert },
    { to: '/hazard-rules/rulebook', label: 'Spatial Rulebook', icon: Layers },
    { to: '/hazard-rules/matrix', label: 'Conflict Matrix', icon: Grid },
    { to: '/admin/agents', label: 'AI Agent Command Center', icon: Cpu },
  ];

  return (
    <aside className={`w-64 bg-slate-900 border-r border-slate-800 p-4 text-slate-300 flex flex-col gap-1.5 ${isOpen ? 'block' : 'hidden md:block'}`}>
      <div className="px-3 py-2 text-xs font-semibold text-slate-500 uppercase tracking-wider">
        Operations Navigation
      </div>
      {links.map((link) => {
        const Icon = link.icon;
        return (
          <NavLink
            key={link.to}
            to={link.to}
            onClick={onClose}
            end={link.to === '/permits' || link.to === '/'}
            className={({ isActive }) =>
              `flex items-center gap-3 px-3 py-2.5 rounded-lg text-sm font-medium transition-all ${
                isActive
                  ? 'bg-amber-500/15 text-amber-400 border border-amber-500/30 shadow-sm'
                  : 'hover:bg-slate-800/80 text-slate-400 hover:text-slate-200'
              }`
            }
          >
            <Icon className="w-4 h-4 text-amber-500/90" />
            <span>{link.label}</span>
          </NavLink>
        );
      })}
    </aside>
  );
};
