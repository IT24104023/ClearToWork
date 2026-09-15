import React, { useState, useEffect } from 'react';
import { useTranslation } from '../context/I18nContext';
import {
  UserCheck,
  Save,
  CheckCircle2,
  AlertCircle,
  KeyRound,
  Camera
} from 'lucide-react';

const PRESET_AVATARS = [
  'https://images.unsplash.com/photo-1507003211169-0a1dd7228f2d?w=150&auto=format&fit=crop&q=80',
  'https://images.unsplash.com/photo-1573496359142-b8d87734a5a2?w=150&auto=format&fit=crop&q=80',
  'https://images.unsplash.com/photo-1534528741775-53994a69daeb?w=150&auto=format&fit=crop&q=80',
  'https://images.unsplash.com/photo-1500648767791-00dcc994a43e?w=150&auto=format&fit=crop&q=80',
  'https://images.unsplash.com/photo-1580489944761-15a19d654956?w=150&auto=format&fit=crop&q=80',
  'https://images.unsplash.com/photo-1472099645785-5658abf4ff4e?w=150&auto=format&fit=crop&q=80'
];

export const ProfilePage: React.FC = () => {
  const { t } = useTranslation();

  const [fullName, setFullName] = useState('');
  const [email, setEmail] = useState('');
  const [role, setRole] = useState('');
  const [phoneNumber, setPhoneNumber] = useState('');
  const [department, setDepartment] = useState('');
  const [bio, setBio] = useState('');
  const [avatarUrl, setAvatarUrl] = useState('');
  const [permissions, setPermissions] = useState<string[]>([]);

  const [loading, setLoading] = useState(false);
  const [successMessage, setSuccessMessage] = useState<string | null>(null);
  const [errorMessage, setErrorMessage] = useState<string | null>(null);

  const fetchProfile = async () => {
    try {
      const resp = await fetch('/api/Users/profile', {
        headers: {
          Authorization: `Bearer ${localStorage.getItem('ctw_token') || ''}`
        }
      });
      if (!resp.ok) throw new Error('Failed to load profile');
      const data = await resp.json();
      setFullName(data.fullName || '');
      setEmail(data.email || '');
      setRole(data.role || '');
      setPhoneNumber(data.phoneNumber || '');
      setDepartment(data.department || '');
      setBio(data.bio || '');
      setAvatarUrl(data.avatarUrl || PRESET_AVATARS[0]);
      setPermissions(data.permissions || []);
    } catch (err: any) {
      setErrorMessage(err.message || 'Error fetching user profile');
    }
  };

  useEffect(() => {
    fetchProfile();
  }, []);

  const handleSave = async (e: React.FormEvent) => {
    e.preventDefault();
    setLoading(true);
    setSuccessMessage(null);
    setErrorMessage(null);

    try {
      const resp = await fetch('/api/Users/profile', {
        method: 'PUT',
        headers: {
          'Content-Type': 'application/json',
          Authorization: `Bearer ${localStorage.getItem('ctw_token') || ''}`
        },
        body: JSON.stringify({
          fullName,
          avatarUrl,
          phoneNumber,
          department,
          bio
        })
      });

      if (!resp.ok) throw new Error('Failed to update profile');
      await resp.json();
      setSuccessMessage('Safety profile and credentials successfully updated.');
    } catch (err: any) {
      setErrorMessage(err.message || 'Profile update failed');
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="p-6 space-y-6 max-w-5xl mx-auto">
      {/* Header */}
      <div className="flex items-center space-x-3 border-b border-slate-200 dark:border-slate-800 pb-4">
        <div className="bg-amber-500/20 text-amber-600 dark:text-amber-400 p-2.5 rounded-xl border border-amber-500/30">
          <UserCheck className="w-6 h-6" />
        </div>
        <div>
          <h1 className="text-2xl font-bold text-slate-900 dark:text-white tracking-tight">{t('profile_title')}</h1>
          <p className="text-xs text-slate-500 dark:text-slate-400 mt-0.5">{t('profile_subtitle')}</p>
        </div>
      </div>

      {/* Notifications */}
      {successMessage && (
        <div className="p-3.5 bg-emerald-50 dark:bg-emerald-950/40 border border-emerald-300 dark:border-emerald-800/60 rounded-xl text-emerald-800 dark:text-emerald-300 text-xs flex items-center gap-2 animate-in fade-in">
          <CheckCircle2 className="w-4 h-4 text-emerald-600 dark:text-emerald-400 shrink-0" />
          <span>{successMessage}</span>
        </div>
      )}

      {errorMessage && (
        <div className="p-3.5 bg-rose-50 dark:bg-rose-950/40 border border-rose-300 dark:border-rose-800/60 rounded-xl text-rose-800 dark:text-rose-300 text-xs flex items-center gap-2 animate-in fade-in">
          <AlertCircle className="w-4 h-4 text-rose-600 dark:text-rose-400 shrink-0" />
          <span>{errorMessage}</span>
        </div>
      )}

      <div className="grid grid-cols-1 md:grid-cols-12 gap-6">
        {/* Left: Avatar & Identity Card (4 cols) */}
        <div className="md:col-span-4 space-y-4">
          <div className="bg-white dark:bg-slate-900 p-5 rounded-2xl border border-slate-200 dark:border-slate-800 text-center shadow-sm">
            <div className="relative inline-block mb-3">
              <img
                src={avatarUrl || PRESET_AVATARS[0]}
                alt={fullName}
                className="w-24 h-24 rounded-full object-cover border-2 border-amber-400 shadow-md mx-auto"
              />
              <div className="absolute bottom-0 right-0 bg-slate-900 dark:bg-slate-950 text-amber-400 p-1.5 rounded-full border border-amber-400/60 shadow">
                <Camera className="w-3.5 h-3.5" />
              </div>
            </div>

            <h2 className="text-base font-bold text-slate-900 dark:text-white leading-tight">{fullName || 'User'}</h2>
            <div className="text-xs font-mono text-amber-600 dark:text-amber-400 mt-1 uppercase tracking-wider font-semibold">{role}</div>
            <div className="text-xs text-slate-500 dark:text-slate-400 mt-1">{email}</div>

            {/* Preset Avatars Picker */}
            <div className="mt-4 pt-4 border-t border-slate-200 dark:border-slate-800">
              <div className="text-[11px] font-semibold text-slate-500 dark:text-slate-400 mb-2">{t('profile_choose_avatar')}</div>
              <div className="flex justify-center gap-2">
                {PRESET_AVATARS.map((url, i) => (
                  <img
                    key={i}
                    src={url}
                    alt={`Preset ${i}`}
                    onClick={() => setAvatarUrl(url)}
                    className={`w-7 h-7 rounded-full object-cover cursor-pointer border transition-transform hover:scale-110 ${
                      avatarUrl === url ? 'border-amber-500 ring-2 ring-amber-500/40' : 'border-slate-300 dark:border-slate-700 opacity-60'
                    }`}
                  />
                ))}
              </div>
            </div>
          </div>
        </div>

        {/* Right: Editable Form & Permissions (8 cols) */}
        <div className="md:col-span-8 space-y-6">
          <form onSubmit={handleSave} className="bg-white dark:bg-slate-900 p-6 rounded-2xl border border-slate-200 dark:border-slate-800 shadow-sm space-y-4">
            <div className="grid grid-cols-1 sm:grid-cols-2 gap-4">
              <div>
                <label className="text-xs font-semibold text-slate-700 dark:text-slate-300 block mb-1">
                  {t('profile_full_name')}
                </label>
                <input
                  type="text"
                  value={fullName}
                  onChange={e => setFullName(e.target.value)}
                  className="w-full bg-slate-50 dark:bg-slate-950 border border-slate-300 dark:border-slate-700 rounded-xl px-3 py-2 text-xs text-slate-900 dark:text-white focus:outline-none focus:border-amber-400"
                  required
                />
              </div>

              <div>
                <label className="text-xs font-semibold text-slate-700 dark:text-slate-300 block mb-1">
                  {t('profile_email')}
                </label>
                <input
                  type="email"
                  value={email}
                  disabled
                  className="w-full bg-slate-100 dark:bg-slate-950/60 border border-slate-200 dark:border-slate-800 rounded-xl px-3 py-2 text-xs text-slate-400 dark:text-slate-500 cursor-not-allowed"
                />
              </div>

              <div>
                <label className="text-xs font-semibold text-slate-700 dark:text-slate-300 block mb-1">
                  {t('profile_phone')}
                </label>
                <input
                  type="text"
                  value={phoneNumber}
                  onChange={e => setPhoneNumber(e.target.value)}
                  placeholder="+94 77 123 4567"
                  className="w-full bg-slate-50 dark:bg-slate-950 border border-slate-300 dark:border-slate-700 rounded-xl px-3 py-2 text-xs text-slate-900 dark:text-white focus:outline-none focus:border-amber-400"
                />
              </div>

              <div>
                <label className="text-xs font-semibold text-slate-700 dark:text-slate-300 block mb-1">
                  {t('profile_department')}
                </label>
                <input
                  type="text"
                  value={department}
                  onChange={e => setDepartment(e.target.value)}
                  placeholder="e.g. Operations & Welding"
                  className="w-full bg-slate-50 dark:bg-slate-950 border border-slate-300 dark:border-slate-700 rounded-xl px-3 py-2 text-xs text-slate-900 dark:text-white focus:outline-none focus:border-amber-400"
                />
              </div>
            </div>

            <div>
              <label className="text-xs font-semibold text-slate-700 dark:text-slate-300 block mb-1">
                {t('profile_avatar_url')}
              </label>
              <input
                type="url"
                value={avatarUrl}
                onChange={e => setAvatarUrl(e.target.value)}
                placeholder="https://..."
                className="w-full bg-slate-50 dark:bg-slate-950 border border-slate-300 dark:border-slate-700 rounded-xl px-3 py-2 text-xs text-slate-900 dark:text-white focus:outline-none focus:border-amber-400"
              />
            </div>

            <div>
              <label className="text-xs font-semibold text-slate-700 dark:text-slate-300 block mb-1">
                {t('profile_bio')}
              </label>
              <textarea
                value={bio}
                onChange={e => setBio(e.target.value)}
                rows={3}
                placeholder="Describe your qualifications and plant clearance responsibilities..."
                className="w-full bg-slate-50 dark:bg-slate-950 border border-slate-300 dark:border-slate-700 rounded-xl px-3 py-2 text-xs text-slate-900 dark:text-white focus:outline-none focus:border-amber-400 leading-relaxed"
              />
            </div>

            <div className="flex justify-end pt-2">
              <button
                type="submit"
                disabled={loading}
                className="flex items-center space-x-1.5 bg-amber-500 hover:bg-amber-400 disabled:opacity-50 text-slate-950 font-bold px-4 py-2 rounded-xl text-xs transition-colors shadow-sm"
              >
                <Save className="w-3.5 h-3.5" />
                <span>{t('profile_save_changes')}</span>
              </button>
            </div>
          </form>

          {/* Permissions Matrix */}
          <div className="bg-white dark:bg-slate-900 p-6 rounded-2xl border border-slate-200 dark:border-slate-800 shadow-sm">
            <div className="flex items-center space-x-2 mb-2">
              <KeyRound className="w-4 h-4 text-amber-500 dark:text-amber-400" />
              <h3 className="text-sm font-bold text-slate-900 dark:text-white">{t('profile_permissions_title')}</h3>
            </div>
            <p className="text-[11px] text-slate-500 dark:text-slate-400 mb-4">{t('profile_permissions_desc')}</p>

            <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-2">
              {permissions.map((perm) => (
                <div
                  key={perm}
                  className="p-2.5 bg-slate-50 dark:bg-slate-950 border border-slate-200 dark:border-slate-800 rounded-xl flex items-center justify-between"
                >
                  <span className="font-mono text-xs text-emerald-600 dark:text-emerald-400 font-semibold">{perm}</span>
                  <span className="text-[10px] bg-emerald-100 dark:bg-emerald-950 text-emerald-700 dark:text-emerald-300 px-2 py-0.5 rounded font-bold">
                    Granted
                  </span>
                </div>
              ))}
            </div>
          </div>
        </div>
      </div>
    </div>
  );
};
