import React from 'react';
import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom';
import { Provider } from 'react-redux';
import { store } from './store';
import { Navbar } from './components/Navbar';
import { Sidebar } from './components/Sidebar';
import { ProtectedRoute } from './components/ProtectedRoute';
import { LoginPage } from './pages/LoginPage';
import { PermitsListPage } from './pages/PermitsListPage';
import { PermitDetailPage } from './pages/PermitDetailPage';
import { AnalyticsPage } from './pages/AnalyticsPage';
import { WorkforcePage } from './pages/WorkforcePage';
import { EquipmentPage } from './pages/EquipmentPage';
import { HazardRulesPage } from './pages/HazardRulesPage';

const AppLayout: React.FC = () => {
  return (
    <div className="min-h-screen bg-slate-950 text-slate-100 flex flex-col font-sans selection:bg-amber-500 selection:text-slate-950">
      <Navbar />
      <div className="flex-1 flex overflow-hidden">
        <Sidebar />
        <main className="flex-1 p-6 md:p-8 overflow-y-auto max-h-[calc(100vh-4rem)]">
          <Routes>
            <Route path="/" element={<PermitsListPage />} />
            <Route path="/permits/:id" element={<PermitDetailPage />} />
            <Route
              path="/analytics"
              element={<AnalyticsPage />}
            />
            <Route path="/workforce" element={<WorkforcePage />} />
            <Route path="/equipment" element={<EquipmentPage />} />
            <Route path="/hazard-rules" element={<HazardRulesPage />} />
            <Route path="*" element={<Navigate to="/" replace />} />
          </Routes>
        </main>
      </div>
    </div>
  );
};

export const App: React.FC = () => {
  return (
    <Provider store={store}>
      <BrowserRouter>
        <Routes>
          <Route path="/login" element={<LoginPage />} />
          <Route element={<ProtectedRoute />}>
            <Route path="/*" element={<AppLayout />} />
          </Route>
        </Routes>
      </BrowserRouter>
    </Provider>
  );
};

export default App;
