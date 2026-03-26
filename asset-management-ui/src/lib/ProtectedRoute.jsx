import React from 'react';
import useAuthStore from '../store/authStore';
import { Navigate, Outlet } from 'react-router-dom';

export const ProtectedRoute = ({ allowedRoles }) => {
  const { user, token } = useAuthStore();

  if (!token || !user) {
    return <Navigate to="/login" replace />;
  }

  if (allowedRoles && !allowedRoles.includes(user.role)) {
    return <Navigate to="/" replace />;
  }

  return <Outlet />;
};
