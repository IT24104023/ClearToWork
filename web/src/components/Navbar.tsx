import React from 'react';
import { Link } from 'react-router-dom';

interface NavbarProps {
  onToggleMobileSidebar?: () => void;
  isMobileSidebarOpen?: boolean;
}

export const Navbar: React.FC<NavbarProps> = ({ onToggleMobileSidebar }) => {
  return (
    <header className="h-16 bg-slate-900 border-b border-slate-800 px-4 flex items-center justify-between text-white">
      <div className="flex items-center gap-3">
        <button onClick={onToggleMobileSidebar} className="md:hidden p-2 text-slate-400 hover:text-white">
          ☰
        </button>
        <Link to="/" className="text-xl font-bold text-amber-500 tracking-wider">
          ClearToWork AI
        </Link>
      </div>
      <div className="flex items-center gap-4">
        <span className="text-xs bg-emerald-900/60 text-emerald-400 px-2.5 py-1 rounded-full border border-emerald-700/50">
          ● System Operational
        </span>
      </div>
    </header>
  );
};
