import React from 'react';
import { Link } from 'react-router-dom';
import { useTranslation } from '../context/I18nContext';
import { CardNav } from '../components/animations/CardNav';
import { Hero3DCanvas } from '../components/animations/Hero3DCanvas';
import { PixelSwap } from '../components/animations/PixelSwap';
import { TiltCard } from '../components/animations/TiltCard';
import {
  ShieldCheck,
  ShieldAlert,
  Wind,
  Users,
  Wrench,
  Compass,
  ArrowRight,
  AlertTriangle,
  Sparkles,
} from 'lucide-react';

export const LandingHomePage: React.FC = () => {
  const { t } = useTranslation();

  return (
    <div className="min-h-screen bg-slate-50 dark:bg-slate-950 text-slate-900 dark:text-slate-100 selection:bg-amber-500 selection:text-slate-950 flex flex-col font-sans overflow-x-hidden transition-colors duration-300">
      {/* 3D Interactive Animated Navigation */}
      <CardNav />

      {/* Hero Section with 3D Canvas Background */}
      <section className="relative min-h-[92vh] flex items-center justify-center pt-8 pb-20 px-4 sm:px-6 lg:px-8 overflow-hidden">
        {/* 3D Particle & Lattice Interactive Background */}
        <Hero3DCanvas />

        {/* Ambient Radial Lighting */}
        <div className="absolute top-1/4 left-1/2 -translate-x-1/2 -translate-y-1/2 w-[600px] sm:w-[900px] h-[400px] bg-gradient-to-b from-amber-500/15 via-orange-500/5 to-transparent rounded-full blur-3xl pointer-events-none -z-10" />

        <div className="relative max-w-7xl mx-auto w-full z-10">
          <div className="text-center space-y-6 max-w-4xl mx-auto">
            {/* Live Status Pill */}
            <div className="inline-flex items-center space-x-2.5 px-4 py-2 rounded-full bg-white/90 dark:bg-slate-900/90 border border-amber-500/40 text-amber-600 dark:text-amber-400 text-xs font-bold uppercase tracking-wider backdrop-blur-md shadow-lg shadow-amber-500/10">
              <span className="w-2 h-2 rounded-full bg-amber-500 animate-ping" />
              <span>{t('home_badge')}</span>
            </div>

            {/* Main Headline */}
            <h1 className="text-4xl sm:text-6xl lg:text-7xl font-extrabold tracking-tight text-slate-900 dark:text-slate-100 leading-[1.1]">
              {t('home_hero_title_1')} <br className="hidden sm:inline" />
              <span className="bg-gradient-to-r from-amber-500 via-amber-400 to-orange-500 bg-clip-text text-transparent">
                {t('home_hero_title_2')}
              </span>
            </h1>

            {/* Subtitle */}
            <p className="text-base sm:text-lg lg:text-xl text-slate-600 dark:text-slate-300 max-w-2xl mx-auto leading-relaxed font-normal">
              {t('home_hero_subtitle')}
            </p>

            {/* CTA Action Buttons */}
            <div className="pt-4 flex flex-col sm:flex-row items-center justify-center gap-4">
              <Link
                to="/login"
                className="w-full sm:w-auto px-8 py-4 rounded-2xl text-base font-bold text-slate-950 bg-gradient-to-r from-amber-400 via-amber-300 to-amber-500 hover:from-amber-300 hover:to-amber-400 shadow-xl shadow-amber-500/25 hover:shadow-amber-500/40 hover:scale-[1.02] active:scale-[0.98] transition-all flex items-center justify-center space-x-3"
              >
                <span>{t('home_btn_launch')}</span>
                <ArrowRight className="w-5 h-5 stroke-[2.5]" />
              </Link>
              <Link
                to="/register"
                className="w-full sm:w-auto px-7 py-4 rounded-2xl text-base font-semibold text-slate-700 dark:text-slate-200 hover:text-slate-950 dark:hover:text-white bg-white/90 dark:bg-slate-900/90 hover:bg-slate-100 dark:hover:bg-slate-800 border border-slate-300 dark:border-slate-700/80 shadow-lg transition-all flex items-center justify-center space-x-2"
              >
                <Users className="w-5 h-5 text-amber-500" />
                <span>{t('home_btn_register')}</span>
              </Link>
            </div>
          </div>

          {/* Interactive PixelSwap Showcase Section */}
          <div className="mt-16 sm:mt-24 max-w-4xl mx-auto">
            <div className="text-center mb-4">
              <span className="text-xs uppercase tracking-widest text-slate-500 dark:text-slate-400 font-bold">
                {t('home_pixel_compare')}
              </span>
            </div>

            <PixelSwap
              className="border border-slate-300 dark:border-slate-800 shadow-2xl shadow-black/20 dark:shadow-black/80"
              pixelSize={56}
              gap={2}
              pixelRadius={6}
              pixelScale={0.35}
              duration={1300}
              pixelDuration={450}
              pattern="random"
              fade={true}
              trigger="hover"
              firstContent={
                <div className="bg-gradient-to-br from-white via-slate-50 to-red-50 dark:from-slate-900 dark:via-slate-900/95 dark:to-red-950/40 p-6 sm:p-10 rounded-2xl border border-red-500/30 flex flex-col md:flex-row items-center justify-between gap-6 shadow-md">
                  <div className="space-y-3 max-w-lg text-left">
                    <div className="inline-flex items-center space-x-2 px-3 py-1 rounded-md bg-red-500/15 text-red-600 dark:text-red-400 text-xs font-bold uppercase tracking-wider border border-red-500/30">
                      <AlertTriangle className="w-3.5 h-3.5" />
                      <span>{t('home_legacy_title')}</span>
                    </div>
                    <h3 className="text-2xl font-bold text-slate-900 dark:text-slate-100">
                      {t('home_legacy_heading')}
                    </h3>
                    <p className="text-sm text-slate-600 dark:text-slate-400 leading-relaxed">
                      {t('home_legacy_desc')}
                    </p>
                    <div className="flex flex-wrap gap-2 pt-2">
                      <span className="text-xs px-2.5 py-1 rounded-md bg-red-100 dark:bg-red-950/60 text-red-700 dark:text-red-300 border border-red-300 dark:border-red-900/60 font-mono">
                        {t('home_legacy_tag1')}
                      </span>
                      <span className="text-xs px-2.5 py-1 rounded-md bg-red-100 dark:bg-red-950/60 text-red-700 dark:text-red-300 border border-red-300 dark:border-red-900/60 font-mono">
                        {t('home_legacy_tag2')}
                      </span>
                    </div>
                  </div>
                  <div className="w-24 h-24 rounded-2xl bg-red-500/10 border border-red-500/30 flex items-center justify-center text-red-500 dark:text-red-400 shrink-0">
                    <ShieldAlert className="w-12 h-12 stroke-[1.5]" />
                  </div>
                </div>
              }
              secondContent={
                <div className="bg-gradient-to-br from-white via-slate-50 to-amber-50 dark:from-slate-900 dark:via-slate-900/95 dark:to-amber-950/40 p-6 sm:p-10 rounded-2xl border border-amber-500/40 flex flex-col md:flex-row items-center justify-between gap-6 shadow-md">
                  <div className="space-y-3 max-w-lg text-left">
                    <div className="inline-flex items-center space-x-2 px-3 py-1 rounded-md bg-amber-500/20 text-amber-700 dark:text-amber-400 text-xs font-bold uppercase tracking-wider border border-amber-500/30">
                      <Sparkles className="w-3.5 h-3.5" />
                      <span>{t('home_ai_title')}</span>
                    </div>
                    <h3 className="text-2xl font-bold text-slate-900 dark:text-slate-100">
                      {t('home_ai_heading')}
                    </h3>
                    <p className="text-sm text-slate-600 dark:text-slate-300 leading-relaxed">
                      {t('home_ai_desc')}
                    </p>
                    <div className="flex flex-wrap gap-2 pt-2">
                      <span className="text-xs px-2.5 py-1 rounded-md bg-emerald-100 dark:bg-emerald-950/60 text-emerald-700 dark:text-emerald-300 border border-emerald-300 dark:border-emerald-900/60 font-mono">
                        {t('home_ai_tag1')}
                      </span>
                      <span className="text-xs px-2.5 py-1 rounded-md bg-amber-100 dark:bg-amber-950/60 text-amber-700 dark:text-amber-300 border border-amber-300 dark:border-amber-900/60 font-mono">
                        {t('home_ai_tag2')}
                      </span>
                    </div>
                  </div>
                  <div className="w-24 h-24 rounded-2xl bg-amber-500/10 border border-amber-500/40 flex items-center justify-center text-amber-600 dark:text-amber-400 shrink-0 shadow-lg shadow-amber-500/20">
                    <ShieldCheck className="w-12 h-12 stroke-[2]" />
                  </div>
                </div>
              }
            />
          </div>
        </div>
      </section>

      {/* Live System Metrics Bar */}
      <section className="border-y border-slate-200 dark:border-slate-800/80 bg-white/60 dark:bg-slate-900/50 backdrop-blur-md py-8 transition-colors">
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
          <div className="grid grid-cols-2 md:grid-cols-4 gap-6 text-center">
            <div className="p-4 rounded-xl bg-slate-100/80 dark:bg-slate-900/60 border border-slate-200 dark:border-slate-800">
              <div className="text-3xl sm:text-4xl font-extrabold text-amber-500 dark:text-amber-400 font-mono">
                100%
              </div>
              <div className="text-xs font-semibold text-slate-500 dark:text-slate-400 uppercase tracking-wider mt-1">
                {t('home_stat_fail_closed')}
              </div>
            </div>
            <div className="p-4 rounded-xl bg-slate-100/80 dark:bg-slate-900/60 border border-slate-200 dark:border-slate-800">
              <div className="text-3xl sm:text-4xl font-extrabold text-amber-500 dark:text-amber-400 font-mono">
                5
              </div>
              <div className="text-xs font-semibold text-slate-500 dark:text-slate-400 uppercase tracking-wider mt-1">
                {t('home_stat_agents')}
              </div>
            </div>
            <div className="p-4 rounded-xl bg-slate-100/80 dark:bg-slate-900/60 border border-slate-200 dark:border-slate-800">
              <div className="text-3xl sm:text-4xl font-extrabold text-amber-500 dark:text-amber-400 font-mono">
                &lt; 80ms
              </div>
              <div className="text-xs font-semibold text-slate-500 dark:text-slate-400 uppercase tracking-wider mt-1">
                {t('home_stat_latency')}
              </div>
            </div>
            <div className="p-4 rounded-xl bg-slate-100/80 dark:bg-slate-900/60 border border-slate-200 dark:border-slate-800">
              <div className="text-3xl sm:text-4xl font-extrabold text-amber-500 dark:text-amber-400 font-mono">
                3
              </div>
              <div className="text-xs font-semibold text-slate-500 dark:text-slate-400 uppercase tracking-wider mt-1">
                {t('home_stat_languages')}
              </div>
            </div>
          </div>
        </div>
      </section>

      {/* 5-Agent Architecture Showcase */}
      <section id="agents" className="py-24 px-4 sm:px-6 lg:px-8 max-w-7xl mx-auto w-full">
        <div className="text-center space-y-4 max-w-3xl mx-auto mb-16">
          <span className="text-xs uppercase tracking-widest text-amber-600 dark:text-amber-400 font-bold">
            {t('home_agents_title')}
          </span>
          <h2 className="text-3xl sm:text-5xl font-extrabold text-slate-900 dark:text-slate-100">
            {t('home_agents_heading')}
          </h2>
          <p className="text-slate-600 dark:text-slate-400 text-base sm:text-lg">
            {t('home_agents_desc')}
          </p>
        </div>

        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
          {/* Agent 1 */}
          <TiltCard className="h-full">
            <div className="h-full p-6 rounded-2xl bg-white dark:bg-slate-900/80 border border-slate-200 dark:border-slate-800 hover:border-amber-500/50 shadow-md transition-all flex flex-col justify-between">
              <div className="space-y-3">
                <div className="w-12 h-12 rounded-xl bg-amber-500/10 border border-amber-500/30 flex items-center justify-center text-amber-500 dark:text-amber-400">
                  <Compass className="w-6 h-6 stroke-[2]" />
                </div>
                <div className="text-xs font-bold uppercase tracking-wider text-amber-600 dark:text-amber-400 font-mono">
                  Node 1 · Student 3
                </div>
                <h3 className="text-xl font-bold text-slate-900 dark:text-slate-100">{t('home_agent_1_title')}</h3>
                <p className="text-sm text-slate-600 dark:text-slate-400 leading-relaxed">
                  {t('home_agent_1_desc')}
                </p>
              </div>
              <div className="pt-4 border-t border-slate-100 dark:border-slate-800 text-xs text-slate-400 font-mono">
                Tool: <code>get_permit_type_template</code>
              </div>
            </div>
          </TiltCard>

          {/* Agent 2 */}
          <TiltCard className="h-full">
            <div className="h-full p-6 rounded-2xl bg-white dark:bg-slate-900/80 border border-slate-200 dark:border-slate-800 hover:border-amber-500/50 shadow-md transition-all flex flex-col justify-between">
              <div className="space-y-3">
                <div className="w-12 h-12 rounded-xl bg-sky-500/10 border border-sky-500/30 flex items-center justify-center text-sky-500 dark:text-sky-400">
                  <Users className="w-6 h-6 stroke-[2]" />
                </div>
                <div className="text-xs font-bold uppercase tracking-wider text-sky-600 dark:text-sky-400 font-mono">
                  Node 2 · Student 1
                </div>
                <h3 className="text-xl font-bold text-slate-900 dark:text-slate-100">{t('home_agent_2_title')}</h3>
                <p className="text-sm text-slate-600 dark:text-slate-400 leading-relaxed">
                  {t('home_agent_2_desc')}
                </p>
              </div>
              <div className="pt-4 border-t border-slate-100 dark:border-slate-800 text-xs text-slate-400 font-mono">
                Tools: <code>get_worker_certificates</code>, <code>find_eligible_workers</code>
              </div>
            </div>
          </TiltCard>

          {/* Agent 3 */}
          <TiltCard className="h-full">
            <div className="h-full p-6 rounded-2xl bg-white dark:bg-slate-900/80 border border-slate-200 dark:border-slate-800 hover:border-amber-500/50 shadow-md transition-all flex flex-col justify-between">
              <div className="space-y-3">
                <div className="w-12 h-12 rounded-xl bg-orange-500/10 border border-orange-500/30 flex items-center justify-center text-orange-500 dark:text-orange-400">
                  <Wrench className="w-6 h-6 stroke-[2]" />
                </div>
                <div className="text-xs font-bold uppercase tracking-wider text-orange-600 dark:text-orange-400 font-mono">
                  Node 3 · Student 2
                </div>
                <h3 className="text-xl font-bold text-slate-900 dark:text-slate-100">{t('home_agent_3_title')}</h3>
                <p className="text-sm text-slate-600 dark:text-slate-400 leading-relaxed">
                  {t('home_agent_3_desc')}
                </p>
              </div>
              <div className="pt-4 border-t border-slate-100 dark:border-slate-800 text-xs text-slate-400 font-mono">
                Tools: <code>check_equipment_readiness</code>, <code>get_isolation_points</code>
              </div>
            </div>
          </TiltCard>

          {/* Agent 4 */}
          <TiltCard className="h-full">
            <div className="h-full p-6 rounded-2xl bg-white dark:bg-slate-900/80 border border-slate-200 dark:border-slate-800 hover:border-amber-500/50 shadow-md transition-all flex flex-col justify-between">
              <div className="space-y-3">
                <div className="w-12 h-12 rounded-xl bg-emerald-500/10 border border-emerald-500/30 flex items-center justify-center text-emerald-500 dark:text-emerald-400">
                  <Wind className="w-6 h-6 stroke-[2]" />
                </div>
                <div className="text-xs font-bold uppercase tracking-wider text-emerald-600 dark:text-emerald-400 font-mono">
                  Node 4 · Student 4
                </div>
                <h3 className="text-xl font-bold text-slate-900 dark:text-slate-100">{t('home_agent_4_title')}</h3>
                <p className="text-sm text-slate-600 dark:text-slate-400 leading-relaxed">
                  {t('home_agent_4_desc')}
                </p>
              </div>
              <div className="pt-4 border-t border-slate-100 dark:border-slate-800 text-xs text-slate-400 font-mono">
                Tools: <code>get_zone_conflicts</code>, <code>get_weather_forecast</code>
              </div>
            </div>
          </TiltCard>

          {/* Agent 5 */}
          <TiltCard className="h-full md:col-span-2 lg:col-span-2">
            <div className="h-full p-6 rounded-2xl bg-gradient-to-br from-white via-slate-50 to-amber-50 dark:from-slate-900/90 dark:via-slate-900/80 dark:to-amber-950/20 border border-amber-500/40 shadow-md transition-all flex flex-col justify-between">
              <div className="space-y-3">
                <div className="w-12 h-12 rounded-xl bg-amber-500/20 border border-amber-400/40 flex items-center justify-center text-amber-600 dark:text-amber-300">
                  <ShieldCheck className="w-6 h-6 stroke-[2.5]" />
                </div>
                <div className="text-xs font-bold uppercase tracking-wider text-amber-600 dark:text-amber-400 font-mono">
                  Node 5 · Shared Guardrail Engine
                </div>
                <h3 className="text-xl font-bold text-slate-900 dark:text-slate-100">{t('home_agent_5_title')}</h3>
                <p className="text-sm text-slate-600 dark:text-slate-300 leading-relaxed">
                  {t('home_agent_5_desc')}
                </p>
              </div>
              <div className="pt-4 border-t border-slate-200 dark:border-slate-800 text-xs text-amber-600 dark:text-amber-400/80 font-mono">
                Tool: <code>run_permit_validator</code>
              </div>
            </div>
          </TiltCard>
        </div>
      </section>

      {/* CTA Bottom Banner */}
      <section className="py-16 px-4 sm:px-6 lg:px-8 max-w-7xl mx-auto w-full">
        <div className="rounded-3xl p-8 sm:p-14 bg-gradient-to-r from-amber-500/20 via-orange-500/10 to-slate-200 dark:to-slate-900 border border-amber-500/40 text-center space-y-6 relative overflow-hidden shadow-2xl">
          <div className="relative z-10 max-w-2xl mx-auto space-y-4">
            <h2 className="text-3xl sm:text-4xl font-extrabold text-slate-900 dark:text-slate-100">
              {t('home_cta_heading')}
            </h2>
            <p className="text-slate-600 dark:text-slate-300 text-sm sm:text-base">
              {t('home_cta_desc')}
            </p>
            <div className="pt-4 flex flex-col sm:flex-row items-center justify-center gap-4">
              <Link
                to="/login"
                className="px-8 py-3.5 rounded-xl font-bold text-slate-950 bg-amber-400 hover:bg-amber-300 transition-all shadow-lg shadow-amber-500/30"
              >
                {t('home_cta_signin')}
              </Link>
              <Link
                to="/about"
                className="px-6 py-3.5 rounded-xl font-semibold text-slate-700 dark:text-slate-200 hover:text-slate-950 dark:hover:text-white bg-white/90 dark:bg-slate-900 border border-slate-300 dark:border-slate-700 hover:border-slate-500 transition-all shadow"
              >
                {t('home_cta_about')}
              </Link>
            </div>
          </div>
        </div>
      </section>

      {/* Public Footer */}
      <footer className="border-t border-slate-200 dark:border-slate-900 bg-white/90 dark:bg-slate-950/90 py-12 px-4 sm:px-6 lg:px-8 text-slate-500 dark:text-slate-400 text-xs transition-colors">
        <div className="max-w-7xl mx-auto flex flex-col md:flex-row items-center justify-between gap-6">
          <div className="flex items-center space-x-3">
            <div className="w-8 h-8 rounded-lg bg-amber-500 flex items-center justify-center text-slate-950 font-bold">
              <ShieldCheck className="w-5 h-5" />
            </div>
            <div>
              <span className="font-bold text-slate-900 dark:text-slate-200 text-sm">ClearToWork AI</span>
              <p className="text-[11px] text-slate-500">{t('home_footer_desc')}</p>
            </div>
          </div>
          <div className="flex flex-wrap items-center justify-center gap-6 font-medium">
            <Link to="/about" className="hover:text-amber-500 transition-colors">{t('nav_about')}</Link>
            <Link to="/contact" className="hover:text-amber-500 transition-colors">{t('nav_contact')}</Link>
            <Link to="/docs" className="hover:text-amber-500 transition-colors">{t('nav_documentation')}</Link>
            <Link to="/login" className="hover:text-amber-500 transition-colors">{t('nav_login')}</Link>
            <Link to="/register" className="hover:text-amber-500 transition-colors">{t('nav_register')}</Link>
          </div>
          <div className="text-slate-500">
            {t('home_footer_copy')}
          </div>
        </div>
      </footer>
    </div>
  );
};

export default LandingHomePage;
