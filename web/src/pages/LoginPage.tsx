import React, { useState } from 'react';
import { useDispatch } from 'react-redux';
import { useNavigate } from 'react-router-dom';
import { useLoginMutation } from '../store/apiSlice';
import { setCredentials } from '../store/authSlice';
import { ShieldAlert, LogIn, HardHat, ShieldCheck, Settings } from 'lucide-react';
import type { UserRole } from '../types';

export const LoginPage: React.FC = () => {
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [errorMsg, setErrorMsg] = useState('');
  const [login, { isLoading }] = useLoginMutation();
  const dispatch = useDispatch();
  const navigate = useNavigate();

  const handleLoginSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setErrorMsg('');
    try {
      const res = await login({ email, password }).unwrap();
      dispatch(
        setCredentials({
          token: res.token,
          user: {
            id: res.id,
            fullName: res.fullName,
            email: res.email,
            role: res.role as UserRole,
            contractorId: res.contractorId,
            token: res.token,
          },
        })
      );
      navigate('/');
    } catch (err: any) {
      setErrorMsg(err.data?.message || 'Authentication failed. Please check credentials.');
    }
  };

  const setDemoUser = (demoEmail: string) => {
    setEmail(demoEmail);
    setPassword('Password123!');
  };

  return (
    <div className="min-h-screen bg-slate-950 flex flex-col justify-center py-12 sm:px-6 lg:px-8 text-slate-100">
      <div className="sm:mx-auto sm:w-full sm:max-w-md text-center">
        <div className="inline-flex items-center justify-center bg-amber-500 text-slate-950 p-3 rounded-2xl shadow-xl shadow-amber-500/10 mb-4">
          <ShieldAlert className="w-10 h-10 text-slate-950" />
        </div>
        <h2 className="text-3xl font-extrabold tracking-tight text-white">
          ClearToWork <span className="text-amber-400">AI</span>
        </h2>
        <p className="mt-2 text-sm text-slate-400">
          Agentic Permit-to-Work & Industrial Safety Clearance Platform
        </p>
      </div>

      <div className="mt-8 sm:mx-auto sm:w-full sm:max-w-md">
        <div className="bg-slate-900 py-8 px-6 shadow-2xl rounded-2xl border border-slate-800 sm:px-10">
          {/* Quick Demo Role Selector */}
          <div className="mb-6">
            <label className="block text-xs font-bold text-slate-400 uppercase tracking-wider mb-2">
              Select Demo Role (Viva / Evaluation)
            </label>
            <div className="grid grid-cols-1 gap-2">
              <button
                type="button"
                onClick={() => setDemoUser('safety@cleartowork.com')}
                className="flex items-center justify-between px-3 py-2 bg-slate-800 hover:bg-slate-700 rounded-lg text-left text-xs border border-slate-700 transition"
              >
                <div className="flex items-center gap-2">
                  <ShieldCheck className="w-4 h-4 text-emerald-400" />
                  <div>
                    <div className="font-semibold text-slate-200">Elena Rostova (HSE Safety Officer)</div>
                    <div className="text-slate-400 text-[10px]">Authoritative Sign-Off & Rulebook Owner</div>
                  </div>
                </div>
                <span className="text-[10px] bg-emerald-950 text-emerald-400 px-2 py-0.5 rounded font-mono">React</span>
              </button>

              <button
                type="button"
                onClick={() => setDemoUser('supervisor@contractor.com')}
                className="flex items-center justify-between px-3 py-2 bg-slate-800 hover:bg-slate-700 rounded-lg text-left text-xs border border-slate-700 transition"
              >
                <div className="flex items-center gap-2">
                  <HardHat className="w-4 h-4 text-amber-400" />
                  <div>
                    <div className="font-semibold text-slate-200">David Miller (Contractor Supervisor)</div>
                    <div className="text-slate-400 text-[10px]">Permit Submissions & Activation</div>
                  </div>
                </div>
                <span className="text-[10px] bg-amber-950 text-amber-400 px-2 py-0.5 rounded font-mono">Mobile/Web</span>
              </button>

              <button
                type="button"
                onClick={() => setDemoUser('areasup@cleartowork.com')}
                className="flex items-center justify-between px-3 py-2 bg-slate-800 hover:bg-slate-700 rounded-lg text-left text-xs border border-slate-700 transition"
              >
                <div className="flex items-center gap-2">
                  <ShieldCheck className="w-4 h-4 text-sky-400" />
                  <div>
                    <div className="font-semibold text-slate-200">James Whitfield (Area Supervisor)</div>
                    <div className="text-slate-400 text-[10px]">Zone Spatial Control & SIMOPS Monitoring</div>
                  </div>
                </div>
                <span className="text-[10px] bg-sky-950 text-sky-400 px-2 py-0.5 rounded font-mono">Area</span>
              </button>

              <button
                type="button"
                onClick={() => setDemoUser('admin@cleartowork.com')}
                className="flex items-center justify-between px-3 py-2 bg-slate-800 hover:bg-slate-700 rounded-lg text-left text-xs border border-slate-700 transition"
              >
                <div className="flex items-center gap-2">
                  <Settings className="w-4 h-4 text-blue-400" />
                  <div>
                    <div className="font-semibold text-slate-200">System Administrator</div>
                    <div className="text-slate-400 text-[10px]">Sites, Zones & User Roles</div>
                  </div>
                </div>
                <span className="text-[10px] bg-blue-950 text-blue-400 px-2 py-0.5 rounded font-mono">React</span>
              </button>
            </div>
          </div>

          <div className="relative my-4">
            <div className="absolute inset-0 flex items-center">
              <div className="w-full border-t border-slate-800"></div>
            </div>
            <div className="relative flex justify-center text-xs uppercase">
              <span className="bg-slate-900 px-2 text-slate-500 font-medium">Or Credentials</span>
            </div>
          </div>

          <form className="space-y-4" onSubmit={handleLoginSubmit}>
            {errorMsg && (
              <div className="bg-rose-950/50 border border-rose-800 text-rose-300 px-3 py-2.5 rounded-lg text-xs">
                {errorMsg}
              </div>
            )}

            <div>
              <label className="block text-xs font-medium text-slate-300">Email Address</label>
              <input
                type="email"
                required
                value={email}
                onChange={(e) => setEmail(e.target.value)}
                className="mt-1 block w-full px-3 py-2 bg-slate-950 border border-slate-700 rounded-lg text-sm text-white placeholder-slate-500 focus:outline-none focus:ring-2 focus:ring-amber-500 focus:border-transparent"
                placeholder="name@company.com"
              />
            </div>

            <div>
              <label className="block text-xs font-medium text-slate-300">Password</label>
              <input
                type="password"
                required
                value={password}
                onChange={(e) => setPassword(e.target.value)}
                className="mt-1 block w-full px-3 py-2 bg-slate-950 border border-slate-700 rounded-lg text-sm text-white placeholder-slate-500 focus:outline-none focus:ring-2 focus:ring-amber-500 focus:border-transparent"
              />
            </div>

            <button
              type="submit"
              disabled={isLoading}
              className="w-full mt-2 flex justify-center items-center gap-2 py-2.5 px-4 border border-transparent rounded-lg shadow-sm text-sm font-bold text-slate-950 bg-amber-500 hover:bg-amber-400 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-amber-500 disabled:opacity-50 transition"
            >
              {isLoading ? (
                <span>Signing in...</span>
              ) : (
                <>
                  <LogIn className="w-4 h-4" />
                  <span>Sign In</span>
                </>
              )}
            </button>
          </form>
        </div>
      </div>
    </div>
  );
};
