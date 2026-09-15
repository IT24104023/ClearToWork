import React from 'react';
import { Link } from 'react-router-dom';
import { CardNav } from '../components/animations/CardNav';
import { TiltCard } from '../components/animations/TiltCard';
import {
  Image as ImageIcon,
  ArrowRight,
  Sparkles,
} from 'lucide-react';


export const AboutUsPage: React.FC = () => {
  return (
    <div className="min-h-screen bg-slate-950 text-slate-100 selection:bg-amber-500 selection:text-slate-950 flex flex-col font-sans">
      <CardNav />

      {/* Hero Section */}
      <section className="relative pt-12 pb-16 px-4 sm:px-6 lg:px-8 max-w-7xl mx-auto w-full">
        <div className="text-center space-y-4 max-w-3xl mx-auto">
          <div className="inline-flex items-center space-x-2 px-3.5 py-1.5 rounded-full bg-amber-500/10 border border-amber-500/30 text-amber-400 text-xs font-bold uppercase tracking-wider">
            <Sparkles className="w-3.5 h-3.5" />
            <span>Industrial Safety Engineering</span>
          </div>
          <h1 className="text-4xl sm:text-6xl font-extrabold tracking-tight text-slate-100">
            About <span className="text-amber-400">ClearToWork AI</span>
          </h1>
          <p className="text-base sm:text-lg text-slate-300 leading-relaxed">
            Pioneering autonomous, multi-agent Permit-to-Work (PTW) verification and real-time SIMOPS safety clearance for high-hazard industrial environments.
          </p>
        </div>

        {/* Team Photo / Facility Picture Container Slot */}
        <div className="mt-12 max-w-5xl mx-auto">
          <div className="relative rounded-3xl overflow-hidden border-2 border-dashed border-slate-700 hover:border-amber-500/60 bg-gradient-to-br from-slate-900 via-slate-900/90 to-slate-950 transition-all p-8 sm:p-12 text-center group shadow-2xl">
            <div className="max-w-md mx-auto space-y-4">
              <div className="w-20 h-20 mx-auto rounded-2xl bg-amber-500/10 border border-amber-500/30 flex items-center justify-center text-amber-400 group-hover:scale-105 transition-transform shadow-lg shadow-amber-500/10">
                <ImageIcon className="w-10 h-10" />
              </div>
              <h3 className="text-xl font-bold text-slate-100">
                Engineering Team & Operations Facility
              </h3>
              <p className="text-xs sm:text-sm text-slate-400 leading-relaxed">
                ClearToWork AI Project Development Team & Petrochemical Safety Operations Hub. Drop in your team or facility photo here.
              </p>
              <div className="inline-flex items-center space-x-2 text-xs text-amber-400 font-mono bg-amber-500/10 px-3 py-1.5 rounded-lg border border-amber-500/20">
                <span>Asset Ready: Upload image anytime to display here</span>
              </div>
            </div>

            {/* Background Glow */}
            <div className="absolute -inset-1 bg-gradient-to-r from-amber-500/10 to-orange-500/10 blur-xl opacity-50 -z-10" />
          </div>
        </div>
      </section>

      {/* Mission & Problem Statement */}
      <section className="py-16 px-4 sm:px-6 lg:px-8 max-w-7xl mx-auto w-full border-t border-slate-900">
        <div className="grid grid-cols-1 md:grid-cols-2 gap-8 items-center">
          <div className="space-y-4">
            <span className="text-xs uppercase tracking-widest text-amber-400 font-bold">
              The Industrial Problem
            </span>
            <h2 className="text-3xl font-extrabold text-slate-100">
              Why Legacy PTW Systems Fail
            </h2>
            <p className="text-slate-300 text-sm leading-relaxed">
              In high-hazard facilities like refineries and chemical plants, catastrophic incidents routinely occur due to three root causes:
            </p>
            <ul className="space-y-2.5 text-sm text-slate-400">
              <li className="flex items-start space-x-2">
                <span className="text-red-400 font-bold mt-0.5">✕</span>
                <span><strong>SIMOPS Clashes:</strong> Simultaneous incompatible activities (e.g. Hot welding next to volatile solvent lines) going unnoticed in separate paper permits.</span>
              </li>
              <li className="flex items-start space-x-2">
                <span className="text-red-400 font-bold mt-0.5">✕</span>
                <span><strong>Uncalibrated Gear:</strong> Expired 90-day multi-gas monitors and extinguishers passing physical inspections without cryptographic verification.</span>
              </li>
              <li className="flex items-start space-x-2">
                <span className="text-red-400 font-bold mt-0.5">✕</span>
                <span><strong>Unqualified Crews:</strong> Expired trade certifications for high-risk enclosed welding or confined space entries.</span>
              </li>
            </ul>
          </div>

          <div className="p-8 rounded-3xl bg-slate-900/90 border border-slate-800 space-y-4">
            <span className="text-xs uppercase tracking-widest text-amber-400 font-bold">
              The Autonomous Solution
            </span>
            <h2 className="text-3xl font-extrabold text-slate-100">
              Deterministic 5-Agent Pipeline
            </h2>
            <p className="text-slate-300 text-sm leading-relaxed">
              ClearToWork AI executes a Directed Acyclic Graph (DAG) with five autonomous agents in LangGraph that enforce a <strong>Fail-Closed Safety Policy</strong>.
            </p>
            <div className="grid grid-cols-2 gap-3 pt-2">
              <div className="p-3 rounded-xl bg-slate-950 border border-slate-800">
                <div className="text-lg font-bold text-amber-400">&lt; 80ms</div>
                <div className="text-xs text-slate-400">Consensus Latency</div>
              </div>
              <div className="p-3 rounded-xl bg-slate-950 border border-slate-800">
                <div className="text-lg font-bold text-emerald-400">100%</div>
                <div className="text-xs text-slate-400">Deterministic Safety</div>
              </div>
            </div>
          </div>
        </div>
      </section>

      {/* Component & Engineering Ownership Breakdown */}
      <section id="team" className="py-16 px-4 sm:px-6 lg:px-8 max-w-7xl mx-auto w-full border-t border-slate-900">
        <div className="text-center space-y-3 max-w-2xl mx-auto mb-12">
          <span className="text-xs uppercase tracking-widest text-amber-400 font-bold">
            Engineering Team Architecture
          </span>
          <h2 className="text-3xl font-extrabold text-slate-100">
            System Component Ownership
          </h2>
          <p className="text-slate-400 text-sm">
            Aligned with enterprise safety standards and modular microservice division.
          </p>
        </div>

        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6">
          {/* Student 1 */}
          <TiltCard className="h-full">
            <div className="p-6 rounded-2xl bg-slate-900 border border-slate-800 space-y-4 h-full flex flex-col justify-between">
              <div className="space-y-3">
                <div className="w-10 h-10 rounded-xl bg-sky-500/10 border border-sky-500/30 flex items-center justify-center text-sky-400 font-bold">
                  S1
                </div>
                <h3 className="text-lg font-bold text-slate-100">Student 1: Workforce & Competency</h3>
                <p className="text-xs text-slate-400 leading-relaxed">
                  Worker profiles, badge scanning, trade qualification tracking (Welder, Confined Space), and 30-day credential expiry forecasting.
                </p>
              </div>
              <div className="text-xs text-sky-400 font-mono">
                Personnel & Competency Agent
              </div>
            </div>
          </TiltCard>

          {/* Student 2 */}
          <TiltCard className="h-full">
            <div className="p-6 rounded-2xl bg-slate-900 border border-slate-800 space-y-4 h-full flex flex-col justify-between">
              <div className="space-y-3">
                <div className="w-10 h-10 rounded-xl bg-orange-500/10 border border-orange-500/30 flex items-center justify-center text-orange-400 font-bold">
                  S2
                </div>
                <h3 className="text-lg font-bold text-slate-100">Student 2: Equipment & LOTO</h3>
                <p className="text-xs text-slate-400 leading-relaxed">
                  Safety asset registry (extinguishers, gas monitors), 90-day calibration logs, and physical Lock-Out / Tag-Out isolation points.
                </p>
              </div>
              <div className="text-xs text-orange-400 font-mono">
                Resource & Isolation Agent
              </div>
            </div>
          </TiltCard>

          {/* Student 3 */}
          <TiltCard className="h-full">
            <div className="p-6 rounded-2xl bg-slate-900 border border-slate-800 space-y-4 h-full flex flex-col justify-between">
              <div className="space-y-3">
                <div className="w-10 h-10 rounded-xl bg-amber-500/10 border border-amber-500/30 flex items-center justify-center text-amber-400 font-bold">
                  S3
                </div>
                <h3 className="text-lg font-bold text-slate-100">Student 3: Permit Lifecycle</h3>
                <p className="text-xs text-slate-400 leading-relaxed">
                  JWT authentication, permit drafting state machines, digital QR token clearance issuance, and fire watch requirements.
                </p>
              </div>
              <div className="text-xs text-amber-400 font-mono">
                Planning & Coordination Agent
              </div>
            </div>
          </TiltCard>

          {/* Student 4 */}
          <TiltCard className="h-full">
            <div className="p-6 rounded-2xl bg-slate-900 border border-slate-800 space-y-4 h-full flex flex-col justify-between">
              <div className="space-y-3">
                <div className="w-10 h-10 rounded-xl bg-emerald-500/10 border border-emerald-500/30 flex items-center justify-center text-emerald-400 font-bold">
                  S4
                </div>
                <h3 className="text-lg font-bold text-slate-100">Student 4: SIMOPS & Weather</h3>
                <p className="text-xs text-slate-400 leading-relaxed">
                  Geospatial plant zones, spatial-temporal conflict detection matrix, and live Open-Meteo meteorological integration.
                </p>
              </div>
              <div className="text-xs text-emerald-400 font-mono">
                Site Conditions & Hazard Agent
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
          <span>Sign In to Safety Portal</span>
          <ArrowRight className="w-4 h-4" />
        </Link>
      </section>

      {/* Footer */}
      <footer className="border-t border-slate-900 bg-slate-950 py-8 px-4 text-center text-xs text-slate-500">
        ClearToWork AI · Enterprise Petrochemical Safety Clearance Platform
      </footer>
    </div>
  );
};

export default AboutUsPage;
