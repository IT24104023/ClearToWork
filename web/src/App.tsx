import React from 'react';
import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom';
import { Provider } from 'react-redux';
import { store } from './store';
import { ThemeProvider } from './context/ThemeContext';
import { I18nProvider } from './context/I18nContext';
import { Navbar } from './components/Navbar';
import { Sidebar } from './components/Sidebar';
import { ProtectedRoute } from './components/ProtectedRoute';

// Public Pages
import { LandingHomePage } from './pages/LandingHomePage';
import { AboutUsPage } from './pages/AboutUsPage';
import { ContactUsPage } from './pages/ContactUsPage';
import { RegisterPage } from './pages/RegisterPage';
import { LoginPage } from './pages/LoginPage';

// Protected Portal Pages
import { PermitsListPage } from './pages/PermitsListPage';
import { PermitDetailPage } from './pages/PermitDetailPage';
import { AnalyticsPage } from './pages/AnalyticsPage';
import { WorkforcePage } from './pages/WorkforcePage';
import { EquipmentPage } from './pages/EquipmentPage';
import { HazardRulesPage } from './pages/HazardRulesPage';
import { QChatAgentCommandCenter } from './pages/QChatAgentCommandCenter';
import { DatabaseAdminPage } from './pages/DatabaseAdminPage';
import { ProfilePage } from './pages/ProfilePage';
import { DocsViewerPage } from './pages/DocsViewerPage';

const AppLayout: React.FC = () => {
  return (
    <div className="min-h-screen bg-slate-50 dark:bg-slate-950 text-slate-900 dark:text-slate-100 flex flex-col font-sans selection:bg-amber-500 selection:text-slate-950 transition-colors">
      <Navbar />
      <div className="flex-1 flex overflow-hidden">
        <Sidebar />
        <main className="flex-1 p-4 md:p-6 overflow-y-auto max-h-[calc(100vh-4rem)]">
          <Routes>
            <Route path="/" element={<Navigate to="/permits" replace />} />
            <Route path="/permits" element={<PermitsListPage />} />
            <Route path="/permits/:id" element={<PermitDetailPage />} />
            <Route path="/analytics" element={<AnalyticsPage />} />
            <Route path="/workforce" element={<WorkforcePage />} />
            <Route path="/equipment" element={<EquipmentPage />} />
            <Route path="/hazard-rules" element={<HazardRulesPage />} />
            <Route path="/admin/agents" element={<QChatAgentCommandCenter />} />
            <Route path="/admin/database" element={<DatabaseAdminPage />} />
            <Route path="/profile" element={<ProfilePage />} />
            <Route path="/docs" element={<DocsViewerPage />} />
            <Route path="*" element={<Navigate to="/permits" replace />} />
          </Routes>
        </main>
      </div>
    </div>
  );
};

export const App: React.FC = () => {
  return (
    <Provider store={store}>
      <ThemeProvider>
        <I18nProvider>
          <BrowserRouter>
            <Routes>
              {/* Public Pages */}
              <Route path="/" element={<LandingHomePage />} />
              <Route path="/about" element={<AboutUsPage />} />
              <Route path="/contact" element={<ContactUsPage />} />
              <Route path="/register" element={<RegisterPage />} />
              <Route path="/login" element={<LoginPage />} />

              {/* Authenticated Industrial Safety Operations Portal */}
              <Route element={<ProtectedRoute />}>
                <Route path="/permits" element={<AppLayout />} />
                <Route path="/permits/:id" element={<AppLayout />} />
                <Route path="/analytics" element={<AppLayout />} />
                <Route path="/workforce" element={<AppLayout />} />
                <Route path="/equipment" element={<AppLayout />} />
                <Route path="/hazard-rules" element={<AppLayout />} />
                <Route path="/admin/*" element={<AppLayout />} />
                <Route path="/profile" element={<AppLayout />} />
                <Route path="/docs" element={<AppLayout />} />
              </Route>

              {/* Catch-all */}
              <Route path="*" element={<Navigate to="/" replace />} />
            </Routes>
          </BrowserRouter>
        </I18nProvider>
      </ThemeProvider>
    </Provider>
  );
};

export default App;
