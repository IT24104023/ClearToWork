import React, { useState, useRef, useEffect } from 'react';
import { Link, useLocation } from 'react-router-dom';
import { useTranslation } from '../../context/I18nContext';
import type { Language } from '../../i18n/translations';
import { useTheme } from '../../context/ThemeContext';
import {
  ShieldCheck,
  ChevronDown,
  Globe,
  Sun,
  Moon,
  Menu,
  X,
  ArrowRight,
  LogIn,
  UserPlus,
} from 'lucide-react';

export interface CardNavLink {
  label: string;
  ariaLabel?: string;
  href?: string;
  badge?: string;
  description?: string;
}

export interface CardNavItem {
  label: string;
  bgColor?: string;
  textColor?: string;
  links: CardNavLink[];
}

export interface CardNavProps {
  logo?: React.ReactNode;
  logoAlt?: string;
  items?: CardNavItem[];
  baseColor?: string;
  menuColor?: string;
  buttonBgColor?: string;
  buttonTextColor?: string;
  theme?: 'light' | 'dark';
  className?: string;
}

export const CardNav: React.FC<CardNavProps> = ({
  logo,
  items = [],
  className = '',
}) => {
  const [activeItem, setActiveItem] = useState<number | null>(null);
  const [isMobileOpen, setIsMobileOpen] = useState(false);
  const [isLangOpen, setIsLangOpen] = useState(false);
  const navRef = useRef<HTMLDivElement>(null);
  const location = useLocation();
  const { language, setLanguage, t } = useTranslation();
  const { theme, toggleTheme } = useTheme();

  // Close dropdown on outside click
  useEffect(() => {
    const handleClickOutside = (event: MouseEvent) => {
      if (navRef.current && !navRef.current.contains(event.target as Node)) {
        setActiveItem(null);
        setIsLangOpen(false);
      }
    };
    document.addEventListener('mousedown', handleClickOutside);
    return () => document.removeEventListener('mousedown', handleClickOutside);
  }, []);

  // Close mobile menu on route change
  useEffect(() => {
    setIsMobileOpen(false);
    setActiveItem(null);
  }, [location.pathname]);

  const defaultItems: CardNavItem[] = [
    {
      label: t('nav_platform') || 'Platform',
      bgColor: '#0f172a',
      textColor: '#f8fafc',
      links: [
        { label: 'Multi-Agent Pipeline', href: '/#agents', badge: '5 Agents', description: 'LangGraph deterministic consensus & safety envelopes' },
        { label: 'SIMOPS Conflict Matrix', href: '/#simops', badge: 'Spatial', description: 'Geospatial proximity and incompatible hot work detection' },
        { label: 'Meteorological Guard', href: '/#weather', badge: 'Open-Meteo', description: '35km/h wind gusts & rain precipitation checks' },
      ],
    },
    {
      label: t('nav_about') || 'About Us',
      bgColor: '#1e1b4b',
      textColor: '#f8fafc',
      links: [
        { label: 'System Overview & Mission', href: '/about', description: 'Petrochemical plant risk prevention and safety compliance' },
        { label: 'Engineering Team & Ownership', href: '/about#team', description: 'Student component architecture & fail-safe engine' },
        { label: 'Architecture & Specifications', href: '/docs', description: 'Interactive ADRs and EF Core relational designs' },
      ],
    },
    {
      label: t('nav_contact') || 'Contact & Plant Ops',
      bgColor: '#1e293b',
      textColor: '#f8fafc',
      links: [
        { label: '24/7 Plant Emergency Hotline', href: '/contact#emergency', badge: 'Emergency', description: 'Priority dispatch and safety officer channels' },
        { label: 'Request Industrial Demo', href: '/contact', description: 'Custom refinery deployment & contractor onboarding' },
      ],
    },
  ];


  const navItems = items.length > 0 ? items : defaultItems;

  return (
    <header className={`sticky top-0 z-50 w-full backdrop-blur-xl bg-slate-950/80 border-b border-slate-800/80 transition-colors ${className}`}>
      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <div className="flex items-center justify-between h-20" ref={navRef}>
          {/* Logo */}
          <Link to="/" className="flex items-center space-x-3 group">
            {logo ? (
              logo
            ) : (
              <div className="relative">
                <div className="w-11 h-11 rounded-xl bg-gradient-to-tr from-amber-500 via-amber-400 to-orange-500 flex items-center justify-center shadow-lg shadow-amber-500/20 group-hover:scale-105 transition-all">
                  <ShieldCheck className="w-6 h-6 text-slate-950 stroke-[2.5]" />
                </div>
                <div className="absolute -inset-0.5 bg-amber-400/30 rounded-xl blur-sm -z-10 group-hover:blur-md transition-all" />
              </div>
            )}
            <div className="flex flex-col">
              <span className="text-xl font-bold tracking-tight bg-gradient-to-r from-slate-100 via-slate-200 to-amber-400 bg-clip-text text-transparent">
                ClearToWork <span className="text-amber-400">AI</span>
              </span>
              <span className="text-[10px] tracking-wider uppercase text-slate-400 font-semibold">
                Industrial Safety Clearance
              </span>
            </div>
          </Link>

          {/* Desktop Interactive Card Nav */}
          <nav className="hidden lg:flex items-center space-x-1">
            {navItems.map((item, index) => {
              const isOpen = activeItem === index;
              return (
                <div key={item.label} className="relative">
                  <button
                    onClick={() => setActiveItem(isOpen ? null : index)}
                    onMouseEnter={() => setActiveItem(index)}
                    className={`px-4 py-2 rounded-xl text-sm font-medium transition-all duration-300 flex items-center space-x-1.5 ${
                      isOpen
                        ? 'bg-slate-800/90 text-amber-400 shadow-inner'
                        : 'text-slate-300 hover:text-white hover:bg-slate-900/60'
                    }`}
                  >
                    <span>{item.label}</span>
                    <ChevronDown
                      className={`w-4 h-4 transition-transform duration-300 ${
                        isOpen ? 'rotate-180 text-amber-400' : 'text-slate-500'
                      }`}
                    />
                  </button>

                  {/* Dropdown Card */}
                  {isOpen && (
                    <div
                      className="absolute top-full left-1/2 -translate-x-1/2 pt-3 w-84 sm:w-96 z-50 animate-in fade-in slide-in-from-top-2 duration-200"
                      onMouseLeave={() => setActiveItem(null)}
                    >
                      <div
                        className="rounded-2xl p-4 shadow-2xl border border-slate-700/80 backdrop-blur-2xl transition-all"
                        style={{
                          backgroundColor: item.bgColor || '#0f172a',
                          color: item.textColor || '#ffffff',
                        }}
                      >
                        <div className="text-xs uppercase tracking-wider font-bold text-amber-400/90 mb-3 px-2 flex items-center justify-between">
                          <span>{item.label}</span>
                          <span className="w-1.5 h-1.5 rounded-full bg-amber-400 animate-pulse" />
                        </div>
                        <div className="space-y-1">
                          {item.links.map((link) => (
                            <Link
                              key={link.label}
                              to={link.href || '#'}
                              onClick={() => setActiveItem(null)}
                              className="group/link flex items-start p-2.5 rounded-xl hover:bg-white/10 transition-colors"
                            >
                              <div className="flex-1">
                                <div className="flex items-center space-x-2">
                                  <span className="text-sm font-semibold text-slate-100 group-hover/link:text-amber-300 transition-colors">
                                    {link.label}
                                  </span>
                                  {link.badge && (
                                    <span className="text-[10px] px-2 py-0.5 rounded-full bg-amber-500/20 text-amber-300 border border-amber-500/30 font-medium">
                                      {link.badge}
                                    </span>
                                  )}
                                </div>
                                {link.description && (
                                  <p className="text-xs text-slate-400 mt-0.5 line-clamp-1">
                                    {link.description}
                                  </p>
                                )}
                              </div>
                              <ArrowRight className="w-4 h-4 text-slate-500 opacity-0 -translate-x-1 group-hover/link:opacity-100 group-hover/link:translate-x-0 transition-all text-amber-400 mt-1" />
                            </Link>
                          ))}
                        </div>
                      </div>
                    </div>
                  )}
                </div>
              );
            })}
          </nav>

          {/* Right Action Bar (i18n, Theme, Login, Register) */}
          <div className="hidden sm:flex items-center space-x-3">
            {/* Language Selector */}
            <div className="relative">
              <button
                onClick={() => setIsLangOpen(!isLangOpen)}
                className="p-2.5 rounded-xl text-slate-300 hover:text-white hover:bg-slate-900 border border-slate-800 transition-all flex items-center space-x-1.5"
                title="Switch Language"
              >
                <Globe className="w-4 h-4 text-amber-400" />
                <span className="text-xs uppercase font-bold tracking-wider">{language}</span>
              </button>

              {isLangOpen && (
                <div className="absolute right-0 top-full mt-2 w-36 rounded-xl bg-slate-900 border border-slate-800 shadow-xl p-1 z-50 animate-in fade-in">
                  {(['en', 'si', 'ta'] as Language[]).map((lang) => (
                    <button
                      key={lang}
                      onClick={() => {
                        setLanguage(lang);
                        setIsLangOpen(false);
                      }}
                      className={`w-full text-left px-3 py-2 rounded-lg text-xs font-semibold flex items-center justify-between ${
                        language === lang
                          ? 'bg-amber-500 text-slate-950 font-bold'
                          : 'text-slate-300 hover:bg-slate-800'
                      }`}
                    >
                      <span>
                        {lang === 'en' ? 'English' : lang === 'si' ? 'සිංහල' : 'தமிழ்'}
                      </span>
                    </button>
                  ))}
                </div>
              )}
            </div>

            {/* Theme Toggle */}
            <button
              onClick={toggleTheme}
              className="p-2.5 rounded-xl text-slate-300 hover:text-white hover:bg-slate-900 border border-slate-800 transition-all"
              title="Toggle Theme"
            >
              {theme === 'dark' ? (
                <Sun className="w-4 h-4 text-amber-400" />
              ) : (
                <Moon className="w-4 h-4 text-slate-300" />
              )}
            </button>

            {/* Register Button */}
            <Link
              to="/register"
              className="px-4 py-2 rounded-xl text-sm font-semibold text-slate-200 hover:text-white hover:bg-slate-800/80 border border-slate-700/80 transition-all flex items-center space-x-1.5"
            >
              <UserPlus className="w-4 h-4 text-slate-400" />
              <span>Register</span>
            </Link>

            {/* Sign In CTA */}
            <Link
              to="/login"
              className="relative group px-5 py-2.5 rounded-xl text-sm font-bold text-slate-950 bg-gradient-to-r from-amber-400 via-amber-300 to-amber-500 hover:from-amber-300 hover:to-amber-400 shadow-lg shadow-amber-500/20 hover:shadow-amber-500/40 hover:scale-[1.02] active:scale-[0.98] transition-all flex items-center space-x-2"
            >
              <LogIn className="w-4 h-4 stroke-[2.5]" />
              <span>Sign In</span>
            </Link>
          </div>

          {/* Mobile Menu Button */}
          <div className="flex lg:hidden items-center space-x-2">
            <Link
              to="/login"
              className="px-3.5 py-1.5 rounded-lg text-xs font-bold text-slate-950 bg-amber-400"
            >
              Sign In
            </Link>
            <button
              onClick={() => setIsMobileOpen(!isMobileOpen)}
              className="p-2.5 rounded-xl bg-slate-900 border border-slate-800 text-slate-300 hover:text-white"
            >
              {isMobileOpen ? <X className="w-6 h-6" /> : <Menu className="w-6 h-6" />}
            </button>
          </div>
        </div>

        {/* Mobile Navigation Drawer */}
        {isMobileOpen && (
          <div className="lg:hidden border-t border-slate-800 py-4 px-2 space-y-3 animate-in slide-in-from-top-4 duration-200">
            {navItems.map((item) => (
              <div key={item.label} className="bg-slate-900/60 rounded-xl p-3 border border-slate-800">
                <div className="text-xs font-bold uppercase tracking-wider text-amber-400 mb-2">
                  {item.label}
                </div>
                <div className="space-y-1.5">
                  {item.links.map((link) => (
                    <Link
                      key={link.label}
                      to={link.href || '#'}
                      onClick={() => setIsMobileOpen(false)}
                      className="block p-2 rounded-lg text-sm font-medium text-slate-200 hover:bg-slate-800"
                    >
                      {link.label}
                    </Link>
                  ))}
                </div>
              </div>
            ))}
            <div className="pt-2 flex items-center justify-between px-2">
              <div className="flex items-center space-x-2">
                {(['en', 'si', 'ta'] as Language[]).map((lang) => (
                  <button
                    key={lang}
                    onClick={() => setLanguage(lang)}
                    className={`px-3 py-1 rounded-lg text-xs font-bold ${
                      language === lang ? 'bg-amber-500 text-slate-950' : 'bg-slate-800 text-slate-300'
                    }`}
                  >
                    {lang.toUpperCase()}
                  </button>
                ))}
              </div>
              <Link
                to="/register"
                onClick={() => setIsMobileOpen(false)}
                className="text-xs text-amber-400 font-bold hover:underline"
              >
                Register New User →
              </Link>
            </div>
          </div>
        )}
      </div>
    </header>
  );
};

export default CardNav;
