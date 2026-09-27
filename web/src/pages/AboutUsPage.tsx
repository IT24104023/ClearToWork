import React from 'react';
import { Link } from 'react-router-dom';
import { CardNav } from '../components/animations/CardNav';
import { TiltCard } from '../components/animations/TiltCard';
import { useTranslation } from '../context/I18nContext';
import {
  ArrowRight,
  Sparkles,
} from 'lucide-react';

export const AboutUsPage: React.FC = () => {
  const { t } = useTranslation();

  return (
    <div className="min-h-screen bg-slate-50 dark:bg-slate-950 text-slate-900 dark:text-slate-100 selection:bg-amber-500 selection:text-slate-950 flex flex-col font-sans transition-colors">
      <CardNav />

      {/* Hero Section */}
      <section className="relative pt-12 pb-16 px-4 sm:px-6 lg:px-8 max-w-7xl mx-auto w-full">
        <div className="text-center space-y-4 max-w-3xl mx-auto">
          <div className="inline-flex items-center space-x-2 px-3.5 py-1.5 rounded-full bg-amber-500/10 border border-amber-500/30 text-amber-600 dark:text-amber-400 text-xs font-bold uppercase tracking-wider">
            <Sparkles className="w-3.5 h-3.5" />
            <span>{t('about_badge')}</span>
          </div>
          <h1 className="text-4xl sm:text-6xl font-extrabold tracking-tight text-slate-900 dark:text-slate-100">
            {t('about_title')}
          </h1>
          <p className="text-base sm:text-lg text-slate-600 dark:text-slate-300 leading-relaxed">
            {t('about_subtitle')}
          </p>
        </div>

        {/* Team Photo Container */}
        <div className="mt-12 max-w-4xl mx-auto">
          <div className="relative rounded-3xl overflow-hidden border border-slate-200 dark:border-slate-700/80 bg-white dark:bg-slate-900 shadow-2xl group">
            {/* Team Image */}
            <div className="relative aspect-[4/3] sm:aspect-[16/10] w-full overflow-hidden">
              <img
                src="/team-photo.jpg"
                alt="ClearToWork AI Development Team"
                className="w-full h-full object-cover object-center group-hover:scale-[1.02] transition-transform duration-700 ease-out"
              />
              {/* Subtle Gradient Vignette */}
              <div className="absolute inset-0 bg-gradient-to-t from-slate-950/90 via-slate-950/30 to-transparent" />

              {/* Overlay Badge & Caption */}
              <div className="absolute bottom-6 left-6 right-6 flex flex-col sm:flex-row items-start sm:items-end justify-between gap-4">
                <div className="space-y-1">
                  <div className="inline-flex items-center space-x-2 px-3 py-1 rounded-full bg-amber-500/90 text-slate-950 text-xs font-black uppercase tracking-wider shadow-lg">
                    <Sparkles className="w-3.5 h-3.5" />
                    <span>{t('about_team_badge')}</span>
                  </div>
                  <h3 className="text-xl sm:text-2xl font-black text-white drop-shadow-md">
                    {t('about_team_heading')}
                  </h3>
                  <p className="text-xs sm:text-sm text-slate-200 drop-shadow max-w-xl">
                    {t('about_team_desc')}
                  </p>
                </div>

                <div className="shrink-0 flex items-center space-x-2 bg-slate-900/80 backdrop-blur-md px-3.5 py-2 rounded-xl border border-slate-700/80 text-xs font-mono text-amber-400">
                  <span className="w-2 h-2 rounded-full bg-emerald-400 animate-pulse" />
                  <span>{t('about_team_stat')}</span>
                </div>
              </div>
            </div>

            {/* Ambient Lighting */}
            <div className="absolute -inset-1 bg-gradient-to-r from-amber-500/20 via-orange-500/10 to-sky-500/20 blur-xl opacity-60 -z-10" />
          </div>
        </div>
      </section>

      {/* Mission & Problem Statement */}
      <section className="py-16 px-4 sm:px-6 lg:px-8 max-w-7xl mx-auto w-full border-t border-slate-200 dark:border-slate-800">
        <div className="grid grid-cols-1 md:grid-cols-2 gap-8 items-center">
          <div className="space-y-4">
            <span className="text-xs uppercase tracking-widest text-amber-600 dark:text-amber-400 font-bold">
              {t('about_problem_badge')}
            </span>
            <h2 className="text-3xl font-extrabold text-slate-900 dark:text-slate-100">
              {t('about_problem_heading')}
            </h2>
            <p className="text-slate-600 dark:text-slate-300 text-sm leading-relaxed">
              {t('about_problem_desc')}
            </p>
            <ul className="space-y-2.5 text-sm text-slate-700 dark:text-slate-400">
              <li className="flex items-start space-x-2">
                <span className="text-red-500 font-bold mt-0.5">✕</span>
                <span><strong>{t('about_problem_1_title')}</strong> {t('about_problem_1_desc')}</span>
              </li>
              <li className="flex items-start space-x-2">
                <span className="text-red-500 font-bold mt-0.5">✕</span>
                <span><strong>{t('about_problem_2_title')}</strong> {t('about_problem_2_desc')}</span>
              </li>
              <li className="flex items-start space-x-2">
                <span className="text-red-500 font-bold mt-0.5">✕</span>
                <span><strong>{t('about_problem_3_title')}</strong> {t('about_problem_3_desc')}</span>
              </li>
            </ul>
          </div>

          <div className="p-8 rounded-3xl bg-white dark:bg-slate-900/90 border border-slate-200 dark:border-slate-800 shadow-xl space-y-4">
            <span className="text-xs uppercase tracking-widest text-amber-600 dark:text-amber-400 font-bold">
              {t('about_solution_badge')}
            </span>
            <h2 className="text-3xl font-extrabold text-slate-900 dark:text-slate-100">
              {t('about_solution_heading')}
            </h2>
            <p className="text-slate-600 dark:text-slate-300 text-sm leading-relaxed">
              {t('about_solution_desc')}
            </p>
            <div className="grid grid-cols-2 gap-3 pt-2">
              <div className="p-3 rounded-xl bg-slate-100 dark:bg-slate-950 border border-slate-200 dark:border-slate-800">
                <div className="text-lg font-bold text-amber-600 dark:text-amber-400">&lt; 80ms</div>
                <div className="text-xs text-slate-500 dark:text-slate-400">{t('home_stat_latency')}</div>
              </div>
              <div className="p-3 rounded-xl bg-slate-100 dark:bg-slate-950 border border-slate-200 dark:border-slate-800">
                <div className="text-lg font-bold text-emerald-600 dark:text-emerald-400">100%</div>
                <div className="text-xs text-slate-500 dark:text-slate-400">{t('home_stat_fail_closed')}</div>
              </div>
            </div>
          </div>
        </div>
      </section>

      {/* Component & Engineering Ownership Breakdown */}
      <section id="team" className="py-16 px-4 sm:px-6 lg:px-8 max-w-7xl mx-auto w-full border-t border-slate-200 dark:border-slate-800">
        <div className="text-center space-y-3 max-w-2xl mx-auto mb-12">
          <span className="text-xs uppercase tracking-widest text-amber-600 dark:text-amber-400 font-bold">
            {t('about_ownership_badge')}
          </span>
          <h2 className="text-3xl font-extrabold text-slate-900 dark:text-slate-100">
            {t('about_ownership_heading')}
          </h2>
          <p className="text-slate-600 dark:text-slate-400 text-sm">
            {t('about_ownership_desc')}
          </p>
        </div>

        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6">
          {/* Student 1 */}
          <TiltCard className="h-full">
            <div className="p-6 rounded-2xl bg-white dark:bg-slate-900 border border-slate-200 dark:border-slate-800 shadow-md space-y-4 h-full flex flex-col justify-between">
              <div className="space-y-3">
                <div className="w-10 h-10 rounded-xl bg-sky-500/10 border border-sky-500/30 flex items-center justify-center text-sky-500 dark:text-sky-400 font-bold">
                  S1
                </div>
                <h3 className="text-lg font-bold text-slate-900 dark:text-slate-100">{t('about_s1_title')}</h3>
                <p className="text-xs text-slate-600 dark:text-slate-400 leading-relaxed">
                  {t('about_s1_desc')}
                </p>
              </div>
              <div className="text-xs text-sky-600 dark:text-sky-400 font-mono font-semibold">
                {t('home_agent_2_title')}
              </div>
            </div>
          </TiltCard>

          {/* Student 2 */}
          <TiltCard className="h-full">
            <div className="p-6 rounded-2xl bg-white dark:bg-slate-900 border border-slate-200 dark:border-slate-800 shadow-md space-y-4 h-full flex flex-col justify-between">
              <div className="space-y-3">
                <div className="w-10 h-10 rounded-xl bg-orange-500/10 border border-orange-500/30 flex items-center justify-center text-orange-500 dark:text-orange-400 font-bold">
                  S2
                </div>
                <h3 className="text-lg font-bold text-slate-900 dark:text-slate-100">{t('about_s2_title')}</h3>
                <p className="text-xs text-slate-600 dark:text-slate-400 leading-relaxed">
                  {t('about_s2_desc')}
                </p>
              </div>
              <div className="text-xs text-orange-600 dark:text-orange-400 font-mono font-semibold">
                {t('home_agent_3_title')}
              </div>
            </div>
          </TiltCard>

          {/* Student 3 */}
          <TiltCard className="h-full">
            <div className="p-6 rounded-2xl bg-white dark:bg-slate-900 border border-slate-200 dark:border-slate-800 shadow-md space-y-4 h-full flex flex-col justify-between">
              <div className="space-y-3">
                <div className="w-10 h-10 rounded-xl bg-amber-500/10 border border-amber-500/30 flex items-center justify-center text-amber-600 dark:text-amber-400 font-bold">
                  S3
                </div>
                <h3 className="text-lg font-bold text-slate-900 dark:text-slate-100">{t('about_s3_title')}</h3>
                <p className="text-xs text-slate-600 dark:text-slate-400 leading-relaxed">
                  {t('about_s3_desc')}
                </p>
              </div>
              <div className="text-xs text-amber-600 dark:text-amber-400 font-mono font-semibold">
                {t('home_agent_1_title')}
              </div>
            </div>
          </TiltCard>

          {/* Student 4 */}
          <TiltCard className="h-full">
            <div className="p-6 rounded-2xl bg-white dark:bg-slate-900 border border-slate-200 dark:border-slate-800 shadow-md space-y-4 h-full flex flex-col justify-between">
              <div className="space-y-3">
                <div className="w-10 h-10 rounded-xl bg-emerald-500/10 border border-emerald-500/30 flex items-center justify-center text-emerald-600 dark:text-emerald-400 font-bold">
                  S4
                </div>
                <h3 className="text-lg font-bold text-slate-900 dark:text-slate-100">{t('about_s4_title')}</h3>
                <p className="text-xs text-slate-600 dark:text-slate-400 leading-relaxed">
                  {t('about_s4_desc')}
                </p>
              </div>
              <div className="text-xs text-emerald-600 dark:text-emerald-400 font-mono font-semibold">
                {t('home_agent_4_title')}
              </div>
            </div>
          </TiltCard>
        </div>
      </section>

      {/* CTA Section */}
      <section className="py-12 px-4 sm:px-6 lg:px-8 max-w-7xl mx-auto w-full text-center">
        <Link
          to="/login"
          className="inline-flex items-center space-x-2 px-8 py-3.5 rounded-xl font-bold text-slate-950 bg-amber-400 hover:bg-amber-300 transition-all shadow-lg shadow-amber-500/25"
        >
          <span>{t('home_cta_signin')}</span>
          <ArrowRight className="w-4 h-4" />
        </Link>
      </section>

      {/* Footer */}
      <footer className="border-t border-slate-200 dark:border-slate-800 bg-white dark:bg-slate-950 py-8 px-4 text-center text-xs text-slate-500">
        {t('home_footer_copy')}
      </footer>
    </div>
  );
};

export default AboutUsPage;
