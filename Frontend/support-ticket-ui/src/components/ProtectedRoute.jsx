import React from 'react';
import { Navigate } from 'react-router-dom';
import { IconShield } from './Icons';

export default function ProtectedRoute({ children, allowedRoles }) {
  const token = localStorage.getItem('token');
  const userStr = localStorage.getItem('user');

  if (!token || !userStr) {
    return <Navigate to="/login" replace />;
  }

  const user = JSON.parse(userStr);

  if (allowedRoles && !allowedRoles.includes(user.role)) {
    return (
      <div className="access-denied-page">
        <div className="access-denied-card">
          <div className="access-denied-icon">
            <IconShield size={40} />
          </div>
          <h2 className="access-denied-title">Access Denied</h2>
          <p className="access-denied-text">
            You do not have permission to view this page.
          </p>
          <button onClick={() => window.location.href = '/'}>
            Go to Dashboard
          </button>
        </div>
      </div>
    );
  }

  return children;
}
