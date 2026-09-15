import React from 'react';
import { Link } from 'react-router-dom';

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


  return (
    <div className="min-h-screen bg-slate-950 text-slate-100 selection:bg-amber-500 selection:text-slate-950 flex flex-col font-sans overflow-x-hidden">
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
            <div className="inline-flex items-center space-x-2.5 px-4 py-2 rounded-full bg-slate-900/90 border border-amber-500/30 text-amber-400 text-xs font-bold uppercase tracking-wider backdrop-blur-md shadow-lg shadow-amber-500/10">
              <span className="w-2 h-2 rounded-full bg-amber-400 animate-ping" />
              <span>Autonomous Industrial Safety Clearance Engine</span>
            </div>

            {/* Main Headline */}
            <h1 className="text-4xl sm:text-6xl lg:text-7xl font-extrabold tracking-tight text-slate-100 leading-[1.1]">
              Zero Incidents. <br className="hidden sm:inline" />
              <span className="bg-gradient-to-r from-amber-400 via-amber-300 to-orange-400 bg-clip-text text-transparent">
                Multi-Agent AI Clearance.
              </span>
            </h1>

            {/* Subtitle */}
            <p className="text-base sm:text-lg lg:text-xl text-slate-300 max-w-2xl mx-auto leading-relaxed font-normal">
              Eliminate simultaneous hazardous clashes (SIMOPS), uncalibrated safety gear, and unqualified crews across petrochemical plants and refineries with deterministic LangGraph multi-agent evaluation.
            </p>

            {/* CTA Action Buttons */}
            <div className="pt-4 flex flex-col sm:flex-row items-center justify-center gap-4">
              <Link
                to="/login"
                className="w-full sm:w-auto px-8 py-4 rounded-2xl text-base font-bold text-slate-950 bg-gradient-to-r from-amber-400 via-amber-300 to-amber-500 hover:from-amber-300 hover:to-amber-400 shadow-xl shadow-amber-500/25 hover:shadow-amber-500/40 hover:scale-[1.02] active:scale-[0.98] transition-all flex items-center justify-center space-x-3"
              >
                <span>Launch Safety Portal</span>
                <ArrowRight className="w-5 h-5 stroke-[2.5]" />
              </Link>
              <Link
                to="/register"
                className="w-full sm:w-auto px-7 py-4 rounded-2xl text-base font-semibold text-slate-200 hover:text-white bg-slate-900/90 hover:bg-slate-800 border border-slate-700/80 shadow-lg hover:border-slate-600 transition-all flex items-center justify-center space-x-2"
              >
                <Users className="w-5 h-5 text-slate-400" />
                <span>Register New Contractor</span>
              </Link>
            </div>
          </div>

          {/* Interactive PixelSwap Showcase Section */}
          <div className="mt-16 sm:mt-24 max-w-4xl mx-auto">
            <div className="text-center mb-4">
              <span className="text-xs uppercase tracking-widest text-slate-400 font-bold">
                Hover or Click to Compare Paradigm Shift
              </span>
            </div>

            <PixelSwap
              className="border border-slate-800 shadow-2xl shadow-black/80"
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
                <div className="bg-gradient-to-br from-slate-900 via-slate-900/95 to-red-950/40 p-6 sm:p-10 rounded-2xl border border-red-500/30 flex flex-col md:flex-row items-center justify-between gap-6">
                  <div className="space-y-3 max-w-lg">
                    <div className="inline-flex items-center space-x-2 px-3 py-1 rounded-md bg-red-500/20 text-red-400 text-xs font-bold uppercase tracking-wider border border-red-500/30">
                      <AlertTriangle className="w-3.5 h-3.5" />
                      <span>Legacy Vulnerabilities (Manual PTW)</span>
                    </div>
                    <h3 className="text-2xl font-bold text-slate-100">
                      Paper Permits & Spreadsheet Blindspots
                    </h3>
                    <p className="text-sm text-slate-400 leading-relaxed">
                      Manual paperwork leads to simultaneous incompatible hot work in adjacent solvent zones, overdue gas monitor calibrations, and expired welder credentials.
                    </p>
                    <div className="flex flex-wrap gap-2 pt-2">
                      <span className="text-xs px-2.5 py-1 rounded-md bg-red-950/60 text-red-300 border border-red-900/60 font-mono">
                        ❌ High SIMOPS Explosion Risk
                      </span>
                      <span className="text-xs px-2.5 py-1 rounded-md bg-red-950/60 text-red-300 border border-red-900/60 font-mono">
                        ❌ 3-4 Hour Manual Review Delay
                      </span>
                    </div>
                  </div>
                  <div className="w-24 h-24 rounded-2xl bg-red-500/10 border border-red-500/30 flex items-center justify-center text-red-400 shrink-0">
                    <ShieldAlert className="w-12 h-12 stroke-[1.5]" />
                  </div>
                </div>
              }
              secondContent={
                <div className="bg-gradient-to-br from-slate-900 via-slate-900/95 to-amber-950/40 p-6 sm:p-10 rounded-2xl border border-amber-500/40 flex flex-col md:flex-row items-center justify-between gap-6">
                  <div className="space-y-3 max-w-lg">
                    <div className="inline-flex items-center space-x-2 px-3 py-1 rounded-md bg-amber-500/20 text-amber-400 text-xs font-bold uppercase tracking-wider border border-amber-500/30">
                      <Sparkles className="w-3.5 h-3.5" />
                      <span>ClearToWork AI Autonomous Clearance</span>
                    </div>
                    <h3 className="text-2xl font-bold text-slate-100">
                      5-Agent Fail-Closed Clearance Engine
                    </h3>
                    <p className="text-sm text-slate-300 leading-relaxed">
                      Instant spatial-temporal conflict checking, 90-day equipment calibration audit, certified worker replacement synthesis, and live 35km/h wind gust guardrails.
                    </p>
                    <div className="flex flex-wrap gap-2 pt-2">
                      <span className="text-xs px-2.5 py-1 rounded-md bg-emerald-950/60 text-emerald-300 border border-emerald-900/60 font-mono">
                        ✓ &lt;80ms LangGraph Consensus
                      </span>
                      <span className="text-xs px-2.5 py-1 rounded-md bg-amber-950/60 text-amber-300 border border-amber-900/60 font-mono">
                        ✓ Fail-Safe Remediation Guidance
                      </span>
                    </div>
                  </div>
                  <div className="w-24 h-24 rounded-2xl bg-amber-500/10 border border-amber-500/40 flex items-center justify-center text-amber-400 shrink-0 shadow-lg shadow-amber-500/20">
                    <ShieldCheck className="w-12 h-12 stroke-[2]" />
                  </div>
                </div>
              }
            />
          </div>
        </div>
      </section>

      {/* Live System Metrics Bar */}
      <section className="border-y border-slate-800/80 bg-slate-900/50 backdrop-blur-md py-8">
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
          <div className="grid grid-cols-2 md:grid-cols-4 gap-6 text-center">
            <div className="p-4 rounded-xl bg-slate-900/60 border border-slate-800">
              <div className="text-3xl sm:text-4xl font-extrabold text-amber-400 font-mono">
                100%
              </div>
              <div className="text-xs font-semibold text-slate-400 uppercase tracking-wider mt-1">
                Fail-Closed Safety
              </div>
            </div>
            <div className="p-4 rounded-xl bg-slate-900/60 border border-slate-800">
              <div className="text-3xl sm:text-4xl font-extrabold text-amber-400 font-mono">
                5
              </div>
              <div className="text-xs font-semibold text-slate-400 uppercase tracking-wider mt-1">
                LangGraph Agents
              </div>
            </div>
            <div className="p-4 rounded-xl bg-slate-900/60 border border-slate-800">
              <div className="text-3xl sm:text-4xl font-extrabold text-amber-400 font-mono">
                &lt; 80ms
              </div>
              <div className="text-xs font-semibold text-slate-400 uppercase tracking-wider mt-1">
                Evaluation Latency
              </div>
            </div>
            <div className="p-4 rounded-xl bg-slate-900/60 border border-slate-800">
              <div className="text-3xl sm:text-4xl font-extrabold text-amber-400 font-mono">
                3
              </div>
              <div className="text-xs font-semibold text-slate-400 uppercase tracking-wider mt-1">
                Native Languages (EN/SI/TA)
              </div>
            </div>
          </div>
        </div>
      </section>

      {/* 5-Agent Architecture Showcase */}
      <section id="agents" className="py-24 px-4 sm:px-6 lg:px-8 max-w-7xl mx-auto w-full">
        <div className="text-center space-y-4 max-w-3xl mx-auto mb-16">
          <span className="text-xs uppercase tracking-widest text-amber-400 font-bold">
            Autonomous Safety Consensus
          </span>
          <h2 className="text-3xl sm:text-5xl font-extrabold text-slate-100">
            5 Specialized Agent Nodes in Directed Graph
          </h2>
          <p className="text-slate-400 text-base sm:text-lg">
            Each agent executes deterministic domain-specific verification rules over a strongly-typed shared state model before human sign-off.
          </p>
        </div>

        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
          {/* Agent 1 */}
          <TiltCard className="h-full">
            <div className="h-full p-6 rounded-2xl bg-slate-900/80 border border-slate-800 hover:border-amber-500/40 transition-all flex flex-col justify-between">
              <div className="space-y-3">
                <div className="w-12 h-12 rounded-xl bg-amber-500/10 border border-amber-500/30 flex items-center justify-center text-amber-400">
                  <Compass className="w-6 h-6 stroke-[2]" />
                </div>
                <div className="text-xs font-bold uppercase tracking-wider text-amber-400 font-mono">
                  Node 1 · Student 3
                </div>
                <h3 className="text-xl font-bold text-slate-100">Planning & Coordination</h3>
                <p className="text-sm text-slate-400 leading-relaxed">
                  Decomposes permit objectives, enforces hazard duration ceilings, and mandates fire watches for hot work operations.
                </p>
              </div>
              <div className="pt-4 border-t border-slate-800 text-xs text-slate-500 font-mono">
                Tool: <code>get_permit_type_template</code>
              </div>
            </div>
          </TiltCard>

          {/* Agent 2 */}
          <TiltCard className="h-full">
            <div className="h-full p-6 rounded-2xl bg-slate-900/80 border border-slate-800 hover:border-amber-500/40 transition-all flex flex-col justify-between">
              <div className="space-y-3">
                <div className="w-12 h-12 rounded-xl bg-sky-500/10 border border-sky-500/30 flex items-center justify-center text-sky-400">
                  <Users className="w-6 h-6 stroke-[2]" />
                </div>
                <div className="text-xs font-bold uppercase tracking-wider text-sky-400 font-mono">
                  Node 2 · Student 1
                </div>
                <h3 className="text-xl font-bold text-slate-100">Personnel & Competency</h3>
                <p className="text-sm text-slate-400 leading-relaxed">
                  Audits worker badges, verifies trade qualifications, forecasts 30-day credential expiries, and auto-recommends certified replacements.
                </p>
              </div>
              <div className="pt-4 border-t border-slate-800 text-xs text-slate-500 font-mono">
                Tools: <code>get_worker_certificates</code>, <code>find_eligible_workers</code>
              </div>
            </div>
          </TiltCard>

          {/* Agent 3 */}
          <TiltCard className="h-full">
            <div className="h-full p-6 rounded-2xl bg-slate-900/80 border border-slate-800 hover:border-amber-500/40 transition-all flex flex-col justify-between">
              <div className="space-y-3">
                <div className="w-12 h-12 rounded-xl bg-orange-500/10 border border-orange-500/30 flex items-center justify-center text-orange-400">
                  <Wrench className="w-6 h-6 stroke-[2]" />
                </div>
                <div className="text-xs font-bold uppercase tracking-wider text-orange-400 font-mono">
                  Node 3 · Student 2
                </div>
                <h3 className="text-xl font-bold text-slate-100">Resource & Isolation</h3>
                <p className="text-sm text-slate-400 leading-relaxed">
                  Verifies equipment safety inspection readiness, flags 90-day gas monitor calibration tags, and validates physical LOTO isolation.
                </p>
              </div>
              <div className="pt-4 border-t border-slate-800 text-xs text-slate-500 font-mono">
                Tools: <code>check_equipment_readiness</code>, <code>get_isolation_points</code>
              </div>
            </div>
          </TiltCard>

          {/* Agent 4 */}
          <TiltCard className="h-full">
            <div className="h-full p-6 rounded-2xl bg-slate-900/80 border border-slate-800 hover:border-amber-500/40 transition-all flex flex-col justify-between">
              <div className="space-y-3">
                <div className="w-12 h-12 rounded-xl bg-emerald-500/10 border border-emerald-500/30 flex items-center justify-center text-emerald-400">
                  <Wind className="w-6 h-6 stroke-[2]" />
                </div>
                <div className="text-xs font-bold uppercase tracking-wider text-emerald-400 font-mono">
                  Node 4 · Student 4
                </div>
                <h3 className="text-xl font-bold text-slate-100">Site Conditions & Hazard</h3>
                <p className="text-sm text-slate-400 leading-relaxed">
                  Queries adjacent zone permits for SIMOPS clashes, and checks live Open-Meteo weather forecasts to enforce a 35 km/h wind gust ceiling.
                </p>
              </div>
              <div className="pt-4 border-t border-slate-800 text-xs text-slate-500 font-mono">
                Tools: <code>get_zone_conflicts</code>, <code>get_weather_forecast</code>
              </div>
            </div>
          </TiltCard>

          {/* Agent 5 */}
          <TiltCard className="h-full md:col-span-2 lg:col-span-2">
            <div className="h-full p-6 rounded-2xl bg-gradient-to-br from-slate-900/90 via-slate-900/80 to-amber-950/20 border border-amber-500/30 hover:border-amber-400/60 transition-all flex flex-col justify-between">
              <div className="space-y-3">
                <div className="w-12 h-12 rounded-xl bg-amber-500/20 border border-amber-400/40 flex items-center justify-center text-amber-300">
                  <ShieldCheck className="w-6 h-6 stroke-[2.5]" />
                </div>
                <div className="text-xs font-bold uppercase tracking-wider text-amber-400 font-mono">
                  Node 5 · Shared Guardrail Engine
                </div>
                <h3 className="text-xl font-bold text-slate-100">Validation & Safety Guardrail</h3>
                <p className="text-sm text-slate-300 leading-relaxed">
                  Enforces authoritative fail-closed consensus. If any safety violation is detected across any preceding node, marks <code className="text-red-400 bg-red-950/50 px-1.5 py-0.5 rounded">REFUSED_SAFE_FAILURE</code> and synthesizes an automated remediation package with replacement workers and shift windows.
                </p>
              </div>
              <div className="pt-4 border-t border-slate-800 text-xs text-amber-400/80 font-mono">
                Tool: <code>run_permit_validator</code>
              </div>
            </div>
          </TiltCard>
        </div>
      </section>

      {/* CTA Bottom Banner */}
      <section className="py-16 px-4 sm:px-6 lg:px-8 max-w-7xl mx-auto w-full">
        <div className="rounded-3xl p-8 sm:p-14 bg-gradient-to-r from-amber-500/20 via-orange-500/10 to-slate-900 border border-amber-500/30 text-center space-y-6 relative overflow-hidden shadow-2xl">
          <div className="relative z-10 max-w-2xl mx-auto space-y-4">
            <h2 className="text-3xl sm:text-4xl font-extrabold text-slate-100">
              Ready to Upgrade Industrial Safety Clearance?
            </h2>
            <p className="text-slate-300 text-sm sm:text-base">
              Sign in to experience the live clearance pipeline or explore our comprehensive system architecture documentation.
            </p>
            <div className="pt-4 flex flex-col sm:flex-row items-center justify-center gap-4">
              <Link
                to="/login"
                className="px-8 py-3.5 rounded-xl font-bold text-slate-950 bg-amber-400 hover:bg-amber-300 transition-all shadow-lg shadow-amber-500/30"
              >
                Sign In to Safety Portal
              </Link>
              <Link
                to="/about"
                className="px-6 py-3.5 rounded-xl font-semibold text-slate-200 hover:text-white bg-slate-900 border border-slate-700 hover:border-slate-500 transition-all"
              >
                About the Platform →
              </Link>
            </div>
          </div>
        </div>
      </section>

      {/* Public Footer */}
      <footer className="border-t border-slate-900 bg-slate-950/90 py-12 px-4 sm:px-6 lg:px-8 text-slate-400 text-xs">
        <div className="max-w-7xl mx-auto flex flex-col md:flex-row items-center justify-between gap-6">
          <div className="flex items-center space-x-3">
            <div className="w-8 h-8 rounded-lg bg-amber-500 flex items-center justify-center text-slate-950 font-bold">
              <ShieldCheck className="w-5 h-5" />
            </div>
            <div>
              <span className="font-bold text-slate-200 text-sm">ClearToWork AI</span>
              <p className="text-[11px] text-slate-500">Autonomous Safety Clearance & SIMOPS Engine</p>
            </div>
          </div>
          <div className="flex flex-wrap items-center justify-center gap-6 font-medium">
            <Link to="/about" className="hover:text-amber-400 transition-colors">About Us</Link>
            <Link to="/contact" className="hover:text-amber-400 transition-colors">Contact Us</Link>
            <Link to="/docs" className="hover:text-amber-400 transition-colors">Architecture Docs</Link>
            <Link to="/login" className="hover:text-amber-400 transition-colors">Sign In</Link>
            <Link to="/register" className="hover:text-amber-400 transition-colors">Register</Link>
          </div>
          <div className="text-slate-500">
            © 2026 ClearToWork AI · Fail-Closed Industrial Safety Standard
          </div>
        </div>
      </footer>
    </div>
  );
};

export default LandingHomePage;
