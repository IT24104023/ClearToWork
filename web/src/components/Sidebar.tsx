import React from 'react';
import { NavLink } from 'react-router-dom';

interface SidebarProps {
  isOpen?: boolean;
  onClose?: () => void;
}

export const Sidebar: React.FC<SidebarProps> = ({ isOpen, onClose }) => {
  const links = [
    { to: '/permits', label: 'Permits Lifecycle' },
    { to: '/workforce', label: 'Workforce & Badges' },
    { to: '/equipment', label: 'Equipment & Calibration' },
    { to: '/hazard-rules', label: 'Hazard & SIMOPS Rules' },
    { to: '/admin/agents', label: 'Agent Command Center' },
  ];

  return (
    <aside className={`w-64 bg-slate-900 border-r border-slate-800 p-4 text-slate-300 flex flex-col gap-2 ${isOpen ? 'block' : 'hidden md:block'}`}>
      {links.map((link) => (
        <NavLink
          key={link.to}
          to={link.to}
          onClick={onClose}
          className={({ isActive }) =>
            `px-3 py-2 rounded-md text-sm font-medium transition-colors ${
              isActive ? 'bg-amber-500/10 text-amber-400 border border-amber-500/30' : 'hover:bg-slate-800 text-slate-400 hover:text-slate-200'
            }`
          }
        >
          {link.label}
        </NavLink>
      ))}
    </aside>
  );
};
