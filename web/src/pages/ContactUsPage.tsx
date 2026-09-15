import React, { useState } from 'react';
import { Link } from 'react-router-dom';
import { CardNav } from '../components/animations/CardNav';
import { TiltCard } from '../components/animations/TiltCard';
import { useTranslation } from '../context/I18nContext';
import {
  Mail,
  Phone,
  MapPin,
  Send,
  ShieldAlert,
  Clock,
  CheckCircle2,
  Building2,
  LifeBuoy,
} from 'lucide-react';

export const ContactUsPage: React.FC = () => {
  const { t } = useTranslation();

  const [formData, setFormData] = useState({
    name: '',
    email: '',
    facility: 'Refinery Alpha (Main Plant)',
    subject: '',
    message: '',
  });
  const [isSubmitted, setIsSubmitted] = useState(false);
  const [loading, setLoading] = useState(false);

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    setLoading(true);
    setTimeout(() => {
      setLoading(false);
      setIsSubmitted(true);
    }, 800);
  };

  return (
    <div className="min-h-screen bg-slate-50 dark:bg-slate-950 text-slate-900 dark:text-slate-100 selection:bg-amber-500 selection:text-slate-950 flex flex-col font-sans transition-colors">
      <CardNav />

      <main className="flex-1 max-w-7xl mx-auto w-full px-4 sm:px-6 lg:px-8 py-12">
        <div className="text-center space-y-3 max-w-2xl mx-auto mb-12">
          <div className="inline-flex items-center space-x-2 px-3.5 py-1.5 rounded-full bg-amber-500/10 border border-amber-500/30 text-amber-600 dark:text-amber-400 text-xs font-bold uppercase tracking-wider">
            <LifeBuoy className="w-3.5 h-3.5" />
            <span>{t('contact_badge')}</span>
          </div>
          <h1 className="text-4xl sm:text-5xl font-extrabold tracking-tight text-slate-900 dark:text-slate-100">
            {t('contact_title')}
          </h1>
          <p className="text-slate-600 dark:text-slate-400 text-sm sm:text-base">
            {t('contact_subtitle')}
          </p>
        </div>

        {/* 24/7 Plant Emergency Hotline Banner */}
        <div id="emergency" className="mb-12 p-6 rounded-2xl bg-gradient-to-r from-red-500/10 via-red-500/5 to-amber-500/5 dark:from-red-950/40 dark:via-red-900/20 dark:to-slate-900 border border-red-500/30 flex flex-col sm:flex-row items-center justify-between gap-4 shadow-xl">
          <div className="flex items-center space-x-4">
            <div className="w-12 h-12 rounded-xl bg-red-500/20 border border-red-500/40 flex items-center justify-center text-red-500 dark:text-red-400 shrink-0">
              <ShieldAlert className="w-7 h-7 stroke-[2]" />
            </div>
            <div>
              <div className="text-xs uppercase tracking-wider font-bold text-red-600 dark:text-red-400">
                {t('contact_emergency_title')}
              </div>
              <div className="text-xl sm:text-2xl font-black text-slate-900 dark:text-slate-100 font-mono mt-0.5">
                +1 (800) 555-SAFE / Ext. 911
              </div>
            </div>
          </div>
          <div className="text-xs text-red-700 dark:text-red-300/80 max-w-xs text-center sm:text-right font-medium">
            {t('contact_emergency_desc')}
          </div>
        </div>

        <div className="grid grid-cols-1 lg:grid-cols-12 gap-8">
          {/* Contact Form */}
          <div className="lg:col-span-7">
            <TiltCard>
              <div className="p-8 rounded-3xl bg-white dark:bg-slate-900/90 border border-slate-200 dark:border-slate-800 shadow-2xl space-y-6">
                <div className="space-y-1">
                  <h2 className="text-2xl font-bold text-slate-900 dark:text-slate-100">{t('contact_form_title')}</h2>
                  <p className="text-xs text-slate-500 dark:text-slate-400">
                    {t('contact_form_desc')}
                  </p>
                </div>

                {isSubmitted ? (
                  <div className="p-8 rounded-2xl bg-emerald-50 dark:bg-emerald-950/40 border border-emerald-500/30 text-center space-y-3 animate-in fade-in">
                    <div className="w-12 h-12 mx-auto rounded-full bg-emerald-500/20 flex items-center justify-center text-emerald-600 dark:text-emerald-400">
                      <CheckCircle2 className="w-6 h-6" />
                    </div>
                    <h3 className="text-lg font-bold text-emerald-700 dark:text-emerald-300">{t('contact_success_title')}</h3>
                    <p className="text-xs text-slate-600 dark:text-slate-300 max-w-sm mx-auto">
                      {t('contact_success_desc')}
                    </p>
                    <button
                      onClick={() => {
                        setIsSubmitted(false);
                        setFormData({ name: '', email: '', facility: 'Refinery Alpha (Main Plant)', subject: '', message: '' });
                      }}
                      className="mt-2 text-xs text-amber-600 dark:text-amber-400 hover:underline font-semibold"
                    >
                      ← Send another message
                    </button>
                  </div>
                ) : (
                  <form onSubmit={handleSubmit} className="space-y-4">
                    <div className="grid grid-cols-1 sm:grid-cols-2 gap-4">
                      <div className="space-y-1.5">
                        <label className="text-xs font-semibold text-slate-700 dark:text-slate-300">{t('contact_field_name')}</label>
                        <input
                          type="text"
                          required
                          value={formData.name}
                          onChange={(e) => setFormData({ ...formData, name: e.target.value })}
                          placeholder="David Miller"
                          className="w-full px-4 py-2.5 rounded-xl bg-slate-50 dark:bg-slate-950 border border-slate-300 dark:border-slate-800 focus:border-amber-500 text-sm text-slate-900 dark:text-slate-100 focus:outline-none transition-colors"
                        />
                      </div>
                      <div className="space-y-1.5">
                        <label className="text-xs font-semibold text-slate-700 dark:text-slate-300">{t('contact_field_email')}</label>
                        <input
                          type="email"
                          required
                          value={formData.email}
                          onChange={(e) => setFormData({ ...formData, email: e.target.value })}
                          placeholder="supervisor@contractor.com"
                          className="w-full px-4 py-2.5 rounded-xl bg-slate-50 dark:bg-slate-950 border border-slate-300 dark:border-slate-800 focus:border-amber-500 text-sm text-slate-900 dark:text-slate-100 focus:outline-none transition-colors"
                        />
                      </div>
                    </div>

                    <div className="grid grid-cols-1 sm:grid-cols-2 gap-4">
                      <div className="space-y-1.5">
                        <label className="text-xs font-semibold text-slate-700 dark:text-slate-300">{t('contact_field_facility')}</label>
                        <select
                          value={formData.facility}
                          onChange={(e) => setFormData({ ...formData, facility: e.target.value })}
                          className="w-full px-4 py-2.5 rounded-xl bg-slate-50 dark:bg-slate-950 border border-slate-300 dark:border-slate-800 focus:border-amber-500 text-sm text-slate-900 dark:text-slate-100 focus:outline-none transition-colors"
                        >
                          <option>Refinery Alpha (Cracking Unit)</option>
                          <option>Offshore Fabrication Yard B</option>
                          <option>Petrochemical Terminal 04</option>
                          <option>General Support / Enterprise Inquiry</option>
                        </select>
                      </div>
                      <div className="space-y-1.5">
                        <label className="text-xs font-semibold text-slate-700 dark:text-slate-300">{t('contact_field_subject')}</label>
                        <input
                          type="text"
                          required
                          value={formData.subject}
                          onChange={(e) => setFormData({ ...formData, subject: e.target.value })}
                          placeholder="SIMOPS Rule Integration"
                          className="w-full px-4 py-2.5 rounded-xl bg-slate-50 dark:bg-slate-950 border border-slate-300 dark:border-slate-800 focus:border-amber-500 text-sm text-slate-900 dark:text-slate-100 focus:outline-none transition-colors"
                        />
                      </div>
                    </div>

                    <div className="space-y-1.5">
                      <label className="text-xs font-semibold text-slate-700 dark:text-slate-300">{t('contact_field_message')}</label>
                      <textarea
                        rows={4}
                        required
                        value={formData.message}
                        onChange={(e) => setFormData({ ...formData, message: e.target.value })}
                        placeholder="Please detail your inquiry or refinery safety clearance requirement..."
                        className="w-full px-4 py-2.5 rounded-xl bg-slate-50 dark:bg-slate-950 border border-slate-300 dark:border-slate-800 focus:border-amber-500 text-sm text-slate-900 dark:text-slate-100 focus:outline-none transition-colors"
                      />
                    </div>

                    <button
                      type="submit"
                      disabled={loading}
                      className="w-full py-3.5 rounded-xl font-bold text-slate-950 bg-gradient-to-r from-amber-400 via-amber-300 to-amber-500 hover:from-amber-300 hover:to-amber-400 transition-all shadow-lg shadow-amber-500/20 flex items-center justify-center space-x-2"
                    >
                      {loading ? (
                        <div className="w-5 h-5 border-2 border-slate-950 border-t-transparent rounded-full animate-spin" />
                      ) : (
                        <>
                          <span>{t('contact_btn_submit')}</span>
                          <Send className="w-4 h-4" />
                        </>
                      )}
                    </button>
                  </form>
                )}
              </div>
            </TiltCard>
          </div>

          {/* Plant Locations & Info Cards */}
          <div className="lg:col-span-5 space-y-6">
            <div className="p-6 rounded-3xl bg-white dark:bg-slate-900 border border-slate-200 dark:border-slate-800 shadow-lg space-y-4">
              <h3 className="text-lg font-bold text-slate-900 dark:text-slate-100 flex items-center space-x-2">
                <Building2 className="w-5 h-5 text-amber-500 dark:text-amber-400" />
                <span>{t('contact_hq_title')}</span>
              </h3>
              <div className="space-y-3 text-sm text-slate-600 dark:text-slate-300">
                <div className="flex items-start space-x-3">
                  <MapPin className="w-4 h-4 text-amber-500 dark:text-amber-400 shrink-0 mt-1" />
                  <span>{t('contact_hq_address')}</span>
                </div>
                <div className="flex items-center space-x-3">
                  <Mail className="w-4 h-4 text-amber-500 dark:text-amber-400 shrink-0" />
                  <span>support@cleartowork.ai</span>
                </div>
                <div className="flex items-center space-x-3">
                  <Phone className="w-4 h-4 text-amber-500 dark:text-amber-400 shrink-0" />
                  <span>+1 (800) 555-WORK</span>
                </div>
              </div>
            </div>

            <div className="p-6 rounded-3xl bg-white dark:bg-slate-900 border border-slate-200 dark:border-slate-800 shadow-lg space-y-3">
              <h3 className="text-base font-bold text-slate-900 dark:text-slate-100 flex items-center space-x-2">
                <Clock className="w-4 h-4 text-amber-500 dark:text-amber-400" />
                <span>{t('contact_response_title')}</span>
              </h3>
              <p className="text-xs text-slate-600 dark:text-slate-400 leading-relaxed">
                {t('contact_response_desc')}
              </p>
              <div className="pt-2">
                <Link
                  to="/login"
                  className="text-xs text-amber-600 dark:text-amber-400 font-bold hover:underline flex items-center space-x-1"
                >
                  <span>{t('home_cta_signin')} →</span>
                </Link>
              </div>
            </div>
          </div>
        </div>
      </main>

      <footer className="border-t border-slate-200 dark:border-slate-800 bg-white dark:bg-slate-950 py-8 px-4 text-center text-xs text-slate-500">
        {t('home_footer_copy')}
      </footer>
    </div>
  );
};

export default ContactUsPage;
