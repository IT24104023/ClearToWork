import React from 'react';
import { useDispatch, useSelector } from 'react-redux';
import { useNavigate } from 'react-router-dom';
import type { RootState } from '../store';
import { logout } from '../store/authSlice';
import { useTranslation } from '../context/I18nContext';
import { useTheme } from '../context/ThemeContext';
import { ShieldAlert, LogOut, Sun, Moon, Globe, UserCheck, Menu, X } from 'lucide-react';
import type { Language } from '../i18n/translations';

interface NavbarProps {
  onToggleMobileSidebar?: () => void;
  isMobileSidebarOpen?: boolean;
}

export const Navbar: React.FC<NavbarProps> = ({
  onToggleMobileSidebar,
  isMobileSidebarOpen = false,
}) => {
  const dispatch = useDispatch();
  const navigate = useNavigate();
  const user = useSelector((state: RootState) => state.auth.user);
  const { language, setLanguage, t } = useTranslation();
  const { theme, toggleTheme } = useTheme();

  const handleLogout = () => {
    dispatch(logout());
    navigate('/login');
  };

  const handleLanguageChange = (e: React.ChangeEvent<HTMLSelectElement>) => {
    setLanguage(e.target.value as Language);
  };

  return (
    <header className="bg-white dark:bg-slate-950 text-slate-900 dark:text-white border-b border-slate-200 dark:border-slate-800 sticky top-0 z-40 transition-colors shadow-sm">
      <div className="max-w-7xl mx-auto px-3 sm:px-6 lg:px-8 h-16 flex items-center justify-between">
        {/* Left Side: Mobile Hamburger + Brand */}
        <div className="flex items-center space-x-2 sm:space-x-3">
          {onToggleMobileSidebar && (
            <button
              onClick={onToggleMobileSidebar}
              className="md:hidden p-2 rounded-xl text-slate-600 dark:text-slate-300 hover:bg-slate-100 dark:hover:bg-slate-800 transition"
              aria-label="Toggle Sidebar"
            >
              {isMobileSidebarOpen ? (
                <X className="w-5 h-5 text-amber-500" />
              ) : (
                <Menu className="w-5 h-5" />
              )}
            </button>
          )}

          <div className="flex items-center space-x-2 sm:space-x-3 cursor-pointer" onClick={() => navigate('/')}>
            <div className="bg-amber-500 text-slate-950 p-1.5 sm:p-2 rounded-xl font-black tracking-wider flex items-center gap-1.5 shadow-md hover:bg-amber-400 transition-colors">
              <ShieldAlert className="w-5 h-5 sm:w-6 sm:h-6 text-slate-950" />
              <span className="text-base sm:text-lg font-extrabold tracking-tight">ClearToWork</span>
              <span className="text-[10px] sm:text-xs bg-slate-950 text-amber-400 px-1.5 py-0.5 rounded font-mono ml-0.5">AI</span>
            </div>
            <div className="hidden lg:block pl-2 border-l border-slate-300 dark:border-slate-700">
              <span className="text-xs text-slate-500 dark:text-slate-400 font-medium tracking-wide uppercase">
                Petrochemical Complex · Multi-Agent Safety Clearance System
              </span>
            </div>
          </div>
        </div>

        {/* Controls: Language, Theme, User Chip, Sign Out */}
        <div className="flex items-center space-x-1.5 sm:space-x-3">
          {/* Trilingual Switcher */}
          <div className="flex items-center space-x-1 bg-slate-100 dark:bg-slate-800/80 px-2 py-1.5 rounded-xl border border-slate-200 dark:border-slate-700 text-xs text-slate-700 dark:text-slate-200">
            <Globe className="w-3.5 h-3.5 text-amber-500 dark:text-amber-400 shrink-0" />
            <select
              value={language}
              onChange={handleLanguageChange}
              className="bg-transparent border-none text-xs font-semibold text-slate-700 dark:text-slate-200 focus:outline-none cursor-pointer pr-1 max-w-[70px] sm:max-w-none"
              title="Change Language"
            >
              <option value="en" className="bg-white dark:bg-slate-900 text-slate-900 dark:text-white">EN</option>
              <option value="si" className="bg-white dark:bg-slate-900 text-slate-900 dark:text-white">සිංහල (SI)</option>
              <option value="ta" className="bg-white dark:bg-slate-900 text-slate-900 dark:text-white">தமிழ் (TA)</option>
            </select>
          </div>

          {/* Theme Switcher */}
          <button
            onClick={toggleTheme}
            className="p-2 rounded-xl bg-slate-100 dark:bg-slate-800/80 border border-slate-200 dark:border-slate-700 text-slate-700 dark:text-slate-300 hover:text-slate-950 dark:hover:text-amber-400 hover:bg-slate-200 dark:hover:bg-slate-700/70 transition-all text-xs flex items-center gap-1.5"
            title={theme === 'dark' ? t('theme_light') : t('theme_dark')}
          >
            {theme === 'dark' ? (
              <Sun className="w-4 h-4 text-amber-400" />
            ) : (
              <Moon className="w-4 h-4 text-slate-700" />
            )}
          </button>

          {/* User Profile & Logout */}
          {user ? (
            <div className="flex items-center space-x-1.5 sm:space-x-2.5">
              <button
                onClick={() => navigate('/profile')}
                className="flex items-center space-x-2 text-sm bg-slate-100 dark:bg-slate-800 hover:bg-slate-200 dark:hover:bg-slate-700/80 px-2.5 py-1.5 rounded-xl border border-slate-200 dark:border-slate-700 transition-colors"
                title={t('nav_profile')}
              >
                <div className="w-6 h-6 rounded-full overflow-hidden bg-amber-500/20 border border-amber-500/50 flex items-center justify-center shrink-0">
                  <UserCheck className="w-3.5 h-3.5 text-amber-600 dark:text-amber-400" />
                </div>
                <div className="text-left hidden md:block">
                  <span className="font-semibold text-xs text-slate-800 dark:text-slate-200 block truncate max-w-[120px]">{user.fullName}</span>
                  <span className="text-[10px] font-mono text-amber-600 dark:text-amber-400 uppercase tracking-wider block">{user.role}</span>
                </div>
              </button>

              <button
                onClick={handleLogout}
                className="flex items-center space-x-1 text-xs text-slate-500 hover:text-rose-600 dark:text-slate-400 dark:hover:text-rose-400 hover:bg-slate-100 dark:hover:bg-slate-800 px-2.5 py-2 rounded-xl transition-colors border border-transparent hover:border-slate-200 dark:hover:border-slate-700"
                title={t('nav_logout')}
              >
                <LogOut className="w-4 h-4" />
                <span className="hidden sm:inline">{t('nav_logout')}</span>
              </button>
            </div>
          ) : (
            <button
              onClick={() => navigate('/login')}
              className="bg-amber-500 hover:bg-amber-400 text-slate-950 font-bold px-3 py-1.5 rounded-xl text-xs transition-colors"
            >
              {t('nav_login')}
            </button>
          )}
        </div>
      </div>
    </header>
  );
};

export default Navbar;
