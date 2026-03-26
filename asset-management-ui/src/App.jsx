import React from 'react';
import { Routes, Route, BrowserRouter, Navigate } from 'react-router-dom';
import { ProtectedRoute } from './lib/ProtectedRoute';
import { AppShell } from './components/layout/AppShell';
import useAuthStore from './store/authStore';

import LoginPage from './pages/auth/LoginPage';
import DashboardPage from './pages/dashboard/DashboardPage';

// Configuration Pages
import ConfigLayout from './pages/config/ConfigLayout';
import CategoriesPage from './pages/config/CategoriesPage';
import AssetTypesPage from './pages/config/AssetTypesPage';
import BranchesPage from './pages/config/BranchesPage';

// Asset Management Pages
import AssetListPage from './pages/assets/AssetListPage';
import AssetDetailPage from './pages/assets/AssetDetailPage';
import MyAssetsPage from './pages/assets/MyAssetsPage';

// Request Management Page
import RequestListPage from './pages/requests/RequestListPage';

// Notifications Page
import NotificationsPage from './pages/dashboard/NotificationsPage';

// Reporting Component
import StockReportPage from './pages/reports/StockReportPage';

function RootRedirect() {
  const { user } = useAuthStore();
  if (user?.role === 'Employee' || user?.role === 'HR') {
    // If you want HR to go to assets or remain on Dashboard
    // The user's exact specification: "Overview page is not applicable for Employee login"
    // implies it's ok for HR and Admin. I'll strictly enforce Employee diversion.
  }
  if (user?.role === 'Employee') {
    return <Navigate to="/requests" replace />;
  }
  return <DashboardPage />;
}

function App() {
  return (
    <BrowserRouter>
      <Routes>
        <Route path="/login" element={<LoginPage />} />
        
        <Route element={<ProtectedRoute allowedRoles={['Admin', 'HR', 'Employee']} />}>
          <Route element={<AppShell />}>
            <Route path="/" element={<RootRedirect />} />
            
            <Route path="/config" element={<ConfigLayout />}>
              <Route path="categories" element={<CategoriesPage />} />
              <Route path="asset-types" element={<AssetTypesPage />} />
              <Route path="branches" element={<BranchesPage />} />
            </Route>

            <Route path="/assets" element={<AssetListPage />} />
            <Route path="/my-assets" element={<MyAssetsPage />} />
            <Route path="/assets/:id" element={<AssetDetailPage />} />
            <Route path="/requests" element={<RequestListPage />} />
            <Route path="/notifications" element={<NotificationsPage />} />
            <Route path="/reports/stock" element={<StockReportPage />} />
          </Route>
        </Route>
      </Routes>
    </BrowserRouter>
  );
}

export default App;
