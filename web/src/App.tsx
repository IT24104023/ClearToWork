import React, { useState } from 'react';
import { BrowserRouter, Routes, Route, Navigate, Outlet } from 'react-router-dom';
import { Provider } from 'react-redux';
import { store } from './store';
import { ThemeProvider } from './context/ThemeContext';
import { I18nProvider } from './context/I18nContext';
import { Navbar } from './components/Navbar';
import { Sidebar } from './components/Sidebar';

// Portal Feature Pages
import { PermitsListPage } from './pages/PermitsListPage';
import { PermitDetailPage } from './pages/PermitDetailPage';
import { AnalyticsPage } from './pages/AnalyticsPage';
import { WorkforcePage } from './pages/WorkforcePage';
import { EquipmentPage } from './pages/EquipmentPage';
import { HazardRulesPage } from './pages/HazardRulesPage';
import { RulebookEditorPage } from './pages/RulebookEditorPage';
import { ConflictMatrixEditorPage } from './pages/ConflictMatrixEditorPage';
import { QChatAgentCommandCenter } from './pages/QChatAgentCommandCenter';
import { DatabaseAdminPage } from './pages/DatabaseAdminPage';
import { ProfilePage } from './pages/ProfilePage';
import { DocsViewerPage } from './pages/DocsViewerPage';

const AppLayout: React.FC = () => {
  const [isMobileSidebarOpen, setIsMobileSidebarOpen] = useState(false);

  return (
    <div className="min-h-screen bg-slate-950 text-slate-100 flex flex-col font-sans selection:bg-amber-500 selection:text-slate-950 transition-colors">
      <Navbar
        onToggleMobileSidebar={() => setIsMobileSidebarOpen(!isMobileSidebarOpen)}
        isMobileSidebarOpen={isMobileSidebarOpen}
      />
      <div className="flex-1 flex overflow-hidden relative">
        <Sidebar
          isOpen={isMobileSidebarOpen}
          onClose={() => setIsMobileSidebarOpen(false)}
        />
        <main className="flex-1 p-3 sm:p-4 md:p-6 overflow-y-auto max-h-[calc(100vh-4rem)] w-full bg-slate-900/50">
          <Outlet />
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
              {/* Main Industrial Safety Operations Portal */}
              <Route element={<AppLayout />}>
                <Route path="/" element={<PermitsListPage />} />
                <Route path="/permits" element={<PermitsListPage />} />
                <Route path="/permits/:id" element={<PermitDetailPage />} />
                <Route path="/analytics" element={<AnalyticsPage />} />
                <Route path="/workforce" element={<WorkforcePage />} />
                <Route path="/equipment" element={<EquipmentPage />} />
                <Route path="/hazard-rules" element={<HazardRulesPage />} />
                <Route path="/hazard-rules/rulebook" element={<RulebookEditorPage />} />
                <Route path="/hazard-rules/matrix" element={<ConflictMatrixEditorPage />} />
                <Route path="/admin/agents" element={<QChatAgentCommandCenter />} />
                <Route path="/admin/database" element={<DatabaseAdminPage />} />
                <Route path="/profile" element={<ProfilePage />} />
                <Route path="/docs" element={<DocsViewerPage />} />
              </Route>

              {/* Catch-all redirect to portal main page */}
              <Route path="*" element={<Navigate to="/" replace />} />
            </Routes>
          </BrowserRouter>
        </I18nProvider>
      </ThemeProvider>
    </Provider>
  );
};

export default App;