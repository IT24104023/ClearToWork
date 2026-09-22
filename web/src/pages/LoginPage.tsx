import React, { useState } from 'react';
import { useDispatch } from 'react-redux';
import { useNavigate } from 'react-router-dom';
import { useLoginMutation } from '../store/apiSlice';
import { setCredentials } from '../store/authSlice';
import { useTranslation } from '../context/I18nContext';
import { ShieldAlert, LogIn, HardHat, ShieldCheck, Settings } from 'lucide-react';
import type { UserRole } from '../types';

export const LoginPage: React.FC = () => {
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [errorMsg, setErrorMsg] = useState('');
  const [login, { isLoading }] = useLoginMutation();
  const dispatch = useDispatch();
  const navigate = useNavigate();
  const { t } = useTranslation();

  const handleLoginSubmit = async (e?: React.FormEvent, customEmail?: string, customPassword?: string) => {
    if (e) e.preventDefault();
    setErrorMsg('');
    const emailToUse = customEmail || email;
    const passwordToUse = customPassword || password;

    try {
      const res = await login({ email: emailToUse, password: passwordToUse }).unwrap();
      dispatch(
        setCredentials({
          token: res.token,
          user: {
            id: res.userId || res.id || '',
            fullName: res.fullName,
            email: res.email,
            role: res.role as UserRole,
            contractorId: res.contractorId,
            token: res.token,
          },
        })
      );
      navigate('/permits');
    } catch (err: any) {
      setErrorMsg(err.data?.message || err?.message || 'Authentication failed. Please check credentials.');
    }
  };

  const setDemoUser = (demoEmail: string) => {
    setEmail(demoEmail);
    setPassword('Password123!');
    handleLoginSubmit(undefined, demoEmail, 'Password123!');
  };

  return (
    <div className="min-h-screen bg-slate-50 dark:bg-slate-950 flex flex-col justify-center py-12 sm:px-6 lg:px-8 text-slate-900 dark:text-slate-100 transition-colors">
      <div className="sm:mx-auto sm:w-full sm:max-w-md text-center">
        <div className="mb-4">
          <button
            onClick={() => navigate('/')}
            className="inline-flex items-center space-x-1.5 text-xs text-amber-600 dark:text-amber-400 hover:underline font-semibold transition"
          >
            <span>← {t('nav_home')}</span>
          </button>
        </div>
        <div className="inline-flex items-center justify-center bg-amber-500 text-slate-950 p-3 rounded-2xl shadow-xl shadow-amber-500/10 mb-4">
          <ShieldAlert className="w-10 h-10 text-slate-950" />
        </div>
        <h2 className="text-3xl font-extrabold tracking-tight text-slate-900 dark:text-white">
          ClearToWork <span className="text-amber-500 dark:text-amber-400">AI</span>
        </h2>
        <p className="mt-2 text-sm text-slate-600 dark:text-slate-400">
          {t('header_subtitle')}
        </p>
      </div>

      <div className="mt-8 sm:mx-auto sm:w-full sm:max-w-md">
        <div className="bg-white dark:bg-slate-900 py-8 px-6 shadow-2xl rounded-3xl border border-slate-200 dark:border-slate-800 sm:px-10">
          {/* Quick Demo Role Selector */}
          <div className="mb-6">
            <label className="block text-xs font-bold text-slate-500 dark:text-slate-400 uppercase tracking-wider mb-2">
              Select Demo Role (Viva / Evaluation)
            </label>
            <div className="grid grid-cols-1 gap-2">
              <button
                type="button"
                onClick={() => setDemoUser('safety@cleartowork.com')}
                className="flex items-center justify-between px-3 py-2 bg-slate-50 dark:bg-slate-800 hover:bg-slate-100 dark:hover:bg-slate-700 rounded-xl text-left text-xs border border-slate-200 dark:border-slate-700 transition"
              >
                <div className="flex items-center gap-2">
                  <ShieldCheck className="w-4 h-4 text-emerald-500 dark:text-emerald-400" />
                  <div>
                    <div className="font-semibold text-slate-800 dark:text-slate-200">Elena Rostova ({t('role_safety_officer')})</div>
                    <div className="text-slate-500 dark:text-slate-400 text-[10px]">Authoritative Sign-Off & Rulebook Owner</div>
                  </div>
                </div>
                <span className="text-[10px] bg-emerald-100 dark:bg-emerald-950 text-emerald-700 dark:text-emerald-400 px-2 py-0.5 rounded font-mono">HSE</span>
              </button>

              <button
                type="button"
                onClick={() => setDemoUser('supervisor@contractor.com')}
                className="flex items-center justify-between px-3 py-2 bg-slate-50 dark:bg-slate-800 hover:bg-slate-100 dark:hover:bg-slate-700 rounded-xl text-left text-xs border border-slate-200 dark:border-slate-700 transition"
              >
                <div className="flex items-center gap-2">
                  <HardHat className="w-4 h-4 text-amber-500 dark:text-amber-400" />
                  <div>
                    <div className="font-semibold text-slate-800 dark:text-slate-200">David Miller ({t('role_supervisor')})</div>
                    <div className="text-slate-500 dark:text-slate-400 text-[10px]">Permit Submissions & Activation</div>
                  </div>
                </div>
                <span className="text-[10px] bg-amber-100 dark:bg-amber-950 text-amber-700 dark:text-amber-400 px-2 py-0.5 rounded font-mono">Contractor</span>
              </button>

              <button
                type="button"
                onClick={() => setDemoUser('areasup@cleartowork.com')}
                className="flex items-center justify-between px-3 py-2 bg-slate-50 dark:bg-slate-800 hover:bg-slate-100 dark:hover:bg-slate-700 rounded-xl text-left text-xs border border-slate-200 dark:border-slate-700 transition"
              >
                <div className="flex items-center gap-2">
                  <ShieldCheck className="w-4 h-4 text-sky-500 dark:text-sky-400" />
                  <div>
                    <div className="font-semibold text-slate-800 dark:text-slate-200">James Whitfield ({t('role_area_supervisor')})</div>
                    <div className="text-slate-500 dark:text-slate-400 text-[10px]">Zone Spatial Control & SIMOPS Monitoring</div>
                  </div>
                </div>
                <span className="text-[10px] bg-sky-100 dark:bg-sky-950 text-sky-700 dark:text-sky-400 px-2 py-0.5 rounded font-mono">Area</span>
              </button>

              <button
                type="button"
                onClick={() => setDemoUser('admin@cleartowork.com')}
                className="flex items-center justify-between px-3 py-2 bg-slate-50 dark:bg-slate-800 hover:bg-slate-100 dark:hover:bg-slate-700 rounded-xl text-left text-xs border border-slate-200 dark:border-slate-700 transition"
              >
                <div className="flex items-center gap-2">
                  <Settings className="w-4 h-4 text-blue-500 dark:text-blue-400" />
                  <div>
                    <div className="font-semibold text-slate-800 dark:text-slate-200">{t('role_admin')}</div>
                    <div className="text-slate-500 dark:text-slate-400 text-[10px]">Sites, Zones & User Roles</div>
                  </div>
                </div>
                <span className="text-[10px] bg-blue-100 dark:bg-blue-950 text-blue-700 dark:text-blue-400 px-2 py-0.5 rounded font-mono">Admin</span>
              </button>
            </div>
          </div>

          <div className="relative my-4">
            <div className="absolute inset-0 flex items-center">
              <div className="w-full border-t border-slate-200 dark:border-slate-800"></div>
            </div>
            <div className="relative flex justify-center text-xs uppercase">
              <span className="bg-white dark:bg-slate-900 px-2 text-slate-400 font-medium">Or Credentials</span>
            </div>
          </div>

          <form className="space-y-4" onSubmit={handleLoginSubmit}>
            {errorMsg && (
              <div className="bg-rose-50 dark:bg-rose-950/50 border border-rose-200 dark:border-rose-800 text-rose-700 dark:text-rose-300 px-3 py-2.5 rounded-xl text-xs">
                {errorMsg}
              </div>
            )}

            <div>
              <label className="block text-xs font-semibold text-slate-700 dark:text-slate-300">{t('reg_email')}</label>
              <input
                type="email"
                required
                value={email}
                onChange={(e) => setEmail(e.target.value)}
                className="mt-1 block w-full px-3.5 py-2.5 bg-slate-50 dark:bg-slate-950 border border-slate-300 dark:border-slate-700 rounded-xl text-sm text-slate-900 dark:text-white placeholder-slate-400 dark:placeholder-slate-500 focus:outline-none focus:ring-2 focus:ring-amber-500 focus:border-transparent transition-colors"
                placeholder="name@company.com"
              />
            </div>

            <div>
              <label className="block text-xs font-semibold text-slate-700 dark:text-slate-300">{t('reg_password')}</label>
              <input
                type="password"
                required
                value={password}
                onChange={(e) => setPassword(e.target.value)}
                className="mt-1 block w-full px-3.5 py-2.5 bg-slate-50 dark:bg-slate-950 border border-slate-300 dark:border-slate-700 rounded-xl text-sm text-slate-900 dark:text-white placeholder-slate-400 dark:placeholder-slate-500 focus:outline-none focus:ring-2 focus:ring-amber-500 focus:border-transparent transition-colors"
              />
            </div>

            <button
              type="submit"
              disabled={isLoading}
              className="w-full mt-2 flex justify-center items-center gap-2 py-3 px-4 border border-transparent rounded-xl shadow-lg shadow-amber-500/20 text-sm font-bold text-slate-950 bg-gradient-to-r from-amber-400 via-amber-300 to-amber-500 hover:from-amber-300 hover:to-amber-400 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-amber-500 disabled:opacity-50 transition-all"
            >
              {isLoading ? (
                <span>Signing in...</span>
              ) : (
                <>
                  <LogIn className="w-4 h-4" />
                  <span>{t('nav_login')}</span>
                </>
              )}
            </button>
          </form>

          <div className="mt-4 pt-4 border-t border-slate-200 dark:border-slate-800 text-center text-xs text-slate-500 dark:text-slate-400">
            {t('reg_have_account') === 'Already registered?' ? 'Need a contractor account?' : 'ගිණුමක් අවශ්‍යද?'}{' '}
            <button
              onClick={() => navigate('/register')}
              className="text-amber-600 dark:text-amber-400 font-bold hover:underline"
            >
              {t('nav_register')} →
            </button>
          </div>
        </div>
      </div>
    </div>
  );
};

export default LoginPage;
