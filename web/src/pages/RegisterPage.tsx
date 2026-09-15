import React, { useState } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { useRegisterMutation } from '../store/apiSlice';
import { CardNav } from '../components/animations/CardNav';
import { TiltCard } from '../components/animations/TiltCard';
import {
  UserPlus,
  Mail,
  Lock,
  User,
  Building,
  CheckCircle2,
  AlertCircle,
  ArrowRight,
} from 'lucide-react';


export const RegisterPage: React.FC = () => {
  const navigate = useNavigate();
  const [registerApi, { isLoading }] = useRegisterMutation();

  const [formData, setFormData] = useState({
    fullName: '',
    email: '',
    password: '',
    confirmPassword: '',
    role: 'Supervisor',
  });

  const [errorMsg, setErrorMsg] = useState<string | null>(null);
  const [successMsg, setSuccessMsg] = useState<string | null>(null);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setErrorMsg(null);
    setSuccessMsg(null);

    if (formData.password !== formData.confirmPassword) {
      setErrorMsg('Passwords do not match. Please verify.');
      return;
    }

    if (formData.password.length < 6) {
      setErrorMsg('Password must be at least 6 characters.');
      return;
    }

    try {
      await registerApi({
        fullName: formData.fullName,
        email: formData.email,
        password: formData.password,
        role: formData.role,
        contractorId: null,
      }).unwrap();

      setSuccessMsg('Account registered successfully! Redirecting to login...');
      setTimeout(() => {
        navigate('/login');
      }, 1500);
    } catch (err: any) {
      setErrorMsg(err?.data?.message || 'Registration failed. Please try again or check your email.');
    }
  };

  return (
    <div className="min-h-screen bg-slate-950 text-slate-100 selection:bg-amber-500 selection:text-slate-950 flex flex-col font-sans">
      <CardNav />

      <main className="flex-1 flex items-center justify-center p-4 sm:p-6 lg:p-8">
        <div className="w-full max-w-md">
          <TiltCard>
            <div className="p-8 sm:p-10 rounded-3xl bg-slate-900/90 border border-slate-800 shadow-2xl backdrop-blur-xl space-y-6">
              {/* Header */}
              <div className="text-center space-y-2">
                <div className="w-12 h-12 mx-auto rounded-2xl bg-amber-500/10 border border-amber-500/30 flex items-center justify-center text-amber-400 shadow-lg shadow-amber-500/10">
                  <UserPlus className="w-6 h-6 stroke-[2.5]" />
                </div>
                <h1 className="text-2xl font-bold text-slate-100 tracking-tight">
                  Register User Profile
                </h1>
                <p className="text-xs text-slate-400">
                  Create an authorized account for ClearToWork AI Permit-to-Work Clearance
                </p>
              </div>

              {/* Feedback Alert */}
              {errorMsg && (
                <div className="p-3.5 rounded-xl bg-red-950/60 border border-red-500/40 text-red-300 text-xs flex items-center space-x-2 animate-in fade-in">
                  <AlertCircle className="w-4 h-4 shrink-0 text-red-400" />
                  <span>{errorMsg}</span>
                </div>
              )}

              {successMsg && (
                <div className="p-3.5 rounded-xl bg-emerald-950/60 border border-emerald-500/40 text-emerald-300 text-xs flex items-center space-x-2 animate-in fade-in">
                  <CheckCircle2 className="w-4 h-4 shrink-0 text-emerald-400" />
                  <span>{successMsg}</span>
                </div>
              )}

              <form onSubmit={handleSubmit} className="space-y-4">
                {/* Full Name */}
                <div className="space-y-1.5">
                  <label className="text-xs font-semibold text-slate-300 flex items-center space-x-1.5">
                    <User className="w-3.5 h-3.5 text-amber-400" />
                    <span>Full Name</span>
                  </label>
                  <input
                    type="text"
                    required
                    placeholder="e.g. David Miller"
                    value={formData.fullName}
                    onChange={(e) => setFormData({ ...formData, fullName: e.target.value })}
                    className="w-full px-4 py-2.5 rounded-xl bg-slate-950 border border-slate-800 focus:border-amber-500 text-sm text-slate-100 focus:outline-none transition-colors"
                  />
                </div>

                {/* Email Address */}
                <div className="space-y-1.5">
                  <label className="text-xs font-semibold text-slate-300 flex items-center space-x-1.5">
                    <Mail className="w-3.5 h-3.5 text-amber-400" />
                    <span>Work Email</span>
                  </label>
                  <input
                    type="email"
                    required
                    placeholder="user@plant.com"
                    value={formData.email}
                    onChange={(e) => setFormData({ ...formData, email: e.target.value })}
                    className="w-full px-4 py-2.5 rounded-xl bg-slate-950 border border-slate-800 focus:border-amber-500 text-sm text-slate-100 focus:outline-none transition-colors"
                  />
                </div>

                {/* Role Selector */}
                <div className="space-y-1.5">
                  <label className="text-xs font-semibold text-slate-300 flex items-center space-x-1.5">
                    <Building className="w-3.5 h-3.5 text-amber-400" />
                    <span>Operational Role</span>
                  </label>
                  <select
                    value={formData.role}
                    onChange={(e) => setFormData({ ...formData, role: e.target.value })}
                    className="w-full px-4 py-2.5 rounded-xl bg-slate-950 border border-slate-800 focus:border-amber-500 text-sm text-slate-100 focus:outline-none transition-colors"
                  >
                    <option value="Supervisor">Contractor Supervisor</option>
                    <option value="SafetyOfficer">HSE Safety Officer</option>
                    <option value="AreaSupervisor">Area Supervisor (SIMOPS)</option>
                    <option value="Administrator">System Administrator</option>
                  </select>
                </div>

                {/* Password */}
                <div className="space-y-1.5">
                  <label className="text-xs font-semibold text-slate-300 flex items-center space-x-1.5">
                    <Lock className="w-3.5 h-3.5 text-amber-400" />
                    <span>Password</span>
                  </label>
                  <input
                    type="password"
                    required
                    placeholder="••••••••"
                    value={formData.password}
                    onChange={(e) => setFormData({ ...formData, password: e.target.value })}
                    className="w-full px-4 py-2.5 rounded-xl bg-slate-950 border border-slate-800 focus:border-amber-500 text-sm text-slate-100 focus:outline-none transition-colors"
                  />
                </div>

                {/* Confirm Password */}
                <div className="space-y-1.5">
                  <label className="text-xs font-semibold text-slate-300 flex items-center space-x-1.5">
                    <Lock className="w-3.5 h-3.5 text-amber-400" />
                    <span>Confirm Password</span>
                  </label>
                  <input
                    type="password"
                    required
                    placeholder="••••••••"
                    value={formData.confirmPassword}
                    onChange={(e) => setFormData({ ...formData, confirmPassword: e.target.value })}
                    className="w-full px-4 py-2.5 rounded-xl bg-slate-950 border border-slate-800 focus:border-amber-500 text-sm text-slate-100 focus:outline-none transition-colors"
                  />
                </div>

                {/* Submit Button */}
                <button
                  type="submit"
                  disabled={isLoading}
                  className="w-full py-3.5 rounded-xl font-bold text-slate-950 bg-gradient-to-r from-amber-400 via-amber-300 to-amber-500 hover:from-amber-300 hover:to-amber-400 shadow-lg shadow-amber-500/25 transition-all flex items-center justify-center space-x-2"
                >
                  {isLoading ? (
                    <div className="w-5 h-5 border-2 border-slate-950 border-t-transparent rounded-full animate-spin" />
                  ) : (
                    <>
                      <span>Complete Registration</span>
                      <ArrowRight className="w-4 h-4 stroke-[2.5]" />
                    </>
                  )}
                </button>
              </form>

              {/* Already have an account */}
              <div className="pt-2 text-center text-xs text-slate-400 border-t border-slate-800">
                Already registered?{' '}
                <Link to="/login" className="text-amber-400 font-bold hover:underline">
                  Sign in here →
                </Link>
              </div>
            </div>
          </TiltCard>
        </div>
      </main>

      <footer className="border-t border-slate-900 bg-slate-950 py-6 text-center text-xs text-slate-500">
        ClearToWork AI · Enterprise Petrochemical Safety Clearance Platform
      </footer>
    </div>
  );
};

export default RegisterPage;
