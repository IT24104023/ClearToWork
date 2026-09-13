import React from 'react';
import { useDispatch, useSelector } from 'react-redux';
import { useNavigate } from 'react-router-dom';
import type { RootState } from '../store';
import { logout } from '../store/authSlice';
import { ShieldAlert, LogOut, UserCheck } from 'lucide-react';

export const Navbar: React.FC = () => {
  const dispatch = useDispatch();
  const navigate = useNavigate();
  const user = useSelector((state: RootState) => state.auth.user);

  const handleLogout = () => {
    dispatch(logout());
    navigate('/login');
  };

  return (
    <header className="bg-slate-900 text-white border-b border-slate-800 sticky top-0 z-50">
      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 h-16 flex items-center justify-between">
        <div className="flex items-center space-x-3 cursor-pointer" onClick={() => navigate('/')}>
          <div className="bg-amber-500 text-slate-950 p-2 rounded-lg font-black tracking-wider flex items-center gap-1.5 shadow-md">
            <ShieldAlert className="w-6 h-6 text-slate-950" />
            <span className="text-lg font-extrabold tracking-tight">ClearToWork</span>
            <span className="text-xs bg-slate-950 text-amber-400 px-1.5 py-0.5 rounded font-mono ml-1">AI</span>
          </div>
          <div className="hidden md:block pl-2 border-l border-slate-700">
            <span className="text-xs text-slate-400 font-medium tracking-wide uppercase">
              Petrochemical Plant #01 · High-Risk Safety Clearance Engine
            </span>
          </div>
        </div>

        {user && (
          <div className="flex items-center space-x-4">
            <div className="flex items-center space-x-2 text-sm bg-slate-800 px-3 py-1.5 rounded-lg border border-slate-700">
              <UserCheck className="w-4 h-4 text-amber-400" />
              <div>
                <span className="font-semibold text-slate-200">{user.fullName}</span>
                <span className="mx-2 text-slate-500">|</span>
                <span className="text-xs font-mono text-amber-400 uppercase tracking-wider">{user.role}</span>
              </div>
            </div>

            <button
              onClick={handleLogout}
              className="flex items-center space-x-1.5 text-xs text-slate-400 hover:text-rose-400 hover:bg-slate-800 px-3 py-2 rounded-lg transition-colors border border-transparent hover:border-slate-700"
              title="Sign Out"
            >
              <LogOut className="w-4 h-4" />
              <span className="hidden sm:inline">Logout</span>
            </button>
          </div>
        )}
      </div>
    </header>
  );
};
