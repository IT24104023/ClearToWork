import React from 'react';
import { Link } from 'react-router-dom';

export const LandingHomePage: React.FC = () => {
  return (
    <div className="min-h-screen bg-slate-950 text-slate-100 flex flex-col items-center justify-center p-6 text-center">
      <h1 className="text-4xl sm:text-6xl font-extrabold text-amber-500 mb-4">ClearToWork AI</h1>
      <p className="text-slate-400 max-w-2xl text-lg mb-8">
        Autonomous Permit-to-Work, SIMOPS Collision Avoidance, and Workforce Competency Verification System.
      </p>
      <Link to="/permits" className="bg-amber-500 hover:bg-amber-600 text-slate-950 px-6 py-3 rounded-lg font-bold transition-colors">
        Enter Operations Portal
      </Link>
    </div>
  );
};
