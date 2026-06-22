import React, { useState, useEffect } from 'react';
import { useNavigate, Link } from 'react-router-dom';
import { API_BASE_URL } from '../config';
import { IconUser, IconWrench, IconChart, IconChevronRight, IconEye, IconEyeOff } from '../components/Icons';

const ROLE_CONFIG = {
  Customer: {
    icon: <IconUser size={18} />,
    label: 'Customer Portal',
    desc: 'Submit & track your support tickets',
    colorClass: 'portal-btn-customer',
    iconBg: 'rgba(79,142,247,0.15)',
  },
  Agent: {
    icon: <IconWrench size={18} />,
    label: 'Agent Portal',
    desc: 'Manage and resolve support tickets',
    colorClass: 'portal-btn-agent',
    iconBg: 'rgba(63,185,80,0.15)',
  },
  Supervisor: {
    icon: <IconChart size={18} />,
    label: 'Supervisor Portal',
    desc: 'Monitor team performance & SLAs',
    colorClass: 'portal-btn-supervisor',
    iconBg: 'rgba(168,85,247,0.15)',
  },
};

export default function Login() {
  const navigate = useNavigate();
  const [selectedRole, setSelectedRole] = useState(null);
  const [loginEmail, setLoginEmail] = useState('');
  const [loginPassword, setLoginPassword] = useState('');
  const [loginError, setLoginError] = useState('');
  const [loading, setLoading] = useState(false);
  const [showPassword, setShowPassword] = useState(false);

  const resetFormStates = () => {
    setLoginEmail('');
    setLoginPassword('');
    setLoginError('');
    setShowPassword(false);
  };

  useEffect(() => { resetFormStates(); }, []);

  const handleLogin = async (e) => {
    e.preventDefault();
    setLoginError('');
    setLoading(true);

    try {
      const res = await fetch(`${API_BASE_URL}/api/auth/login`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ email: loginEmail, password: loginPassword }),
      });

      if (!res.ok) throw new Error('Invalid email or password');

      const data = await res.json();

      if (data.role !== selectedRole)
        throw new Error(`This account does not have ${selectedRole} access.`);

      localStorage.setItem('token', data.token);
      localStorage.setItem('user', JSON.stringify({
        id: data.userId, name: data.name, email: data.email,
        role: data.role, tenantId: data.tenantId,
      }));

      navigate('/');
      window.location.reload();
    } catch (err) {
      setLoginError(err.message);
    } finally {
      setLoading(false);
    }
  };

  // role selection
  if (selectedRole === null) {
    return (
      <div className="auth-page">
        <div className="auth-card">
          <div className="auth-logo">
            <svg width="22" height="22" viewBox="0 0 24 24" fill="none" stroke="#fff" strokeWidth="2.2" strokeLinecap="round">
              <path d="M2 9a3 3 0 0 1 0 6v2a2 2 0 0 0 2 2h16a2 2 0 0 0 2-2v-2a3 3 0 0 1 0-6V7a2 2 0 0 0-2-2H4a2 2 0 0 0-2 2Z" />
              <path d="M13 5v2" /><path d="M13 17v2" /><path d="M13 11v2" />
            </svg>
          </div>
          <h1 className="auth-title">SupportDesk</h1>
          <p className="auth-subtitle">Choose your portal to continue</p>

          <div className="portal-buttons">
            {Object.entries(ROLE_CONFIG).map(([role, cfg]) => (
              <button
                key={role}
                className={`portal-btn ${cfg.colorClass}`}
                onClick={() => { resetFormStates(); setSelectedRole(role); }}
              >
                <div className="portal-btn-content">
                  <div className="portal-icon" style={{ background: cfg.iconBg }}>
                    {cfg.icon}
                  </div>
                  <div className="portal-btn-text">
                    <div className="portal-btn-label">{cfg.label}</div>
                    <div className="portal-btn-desc">{cfg.desc}</div>
                  </div>
                </div>
                <span className="portal-arrow"><IconChevronRight size={16} /></span>
              </button>
            ))}
          </div>
        </div>
      </div>
    );
  }

  const cfg = ROLE_CONFIG[selectedRole];

  // login form
  return (
    <div className="auth-page">
      <div className="auth-card">
        <div className="auth-logo" style={{ background: cfg.iconBg, boxShadow: 'none', border: '1px solid var(--border)' }}>
          {cfg.icon}
        </div>

        <h1 className="auth-title">Sign In</h1>
        <p className="auth-subtitle">
          <span className="role-pill">{cfg.icon} {selectedRole}</span>
        </p>

        {loginError && (
          <p className="error-msg">{loginError}</p>
        )}

        <form onSubmit={handleLogin}>
          <div className="form-group">
            <label htmlFor="login-email">Email Address</label>
            <input
              id="login-email"
              type="email"
              value={loginEmail}
              onChange={(e) => setLoginEmail(e.target.value)}
              placeholder="you@example.com"
              required
              maxLength={256}
              autoComplete="username"
            />
          </div>
          <div className="form-group">
            <label htmlFor="login-password">Password</label>
            <div className="password-input-wrapper">
              <input
                id="login-password"
                type={showPassword ? 'text' : 'password'}
                value={loginPassword}
                onChange={(e) => setLoginPassword(e.target.value)}
                placeholder="••••••••"
                required
                maxLength={128}
                autoComplete="current-password"
              />
              <button
                type="button"
                className="password-toggle-btn"
                onClick={() => setShowPassword(!showPassword)}
                aria-label={showPassword ? 'Hide password' : 'Show password'}
              >
                {showPassword ? <IconEyeOff size={18} /> : <IconEye size={18} />}
              </button>
            </div>
          </div>
          <button type="submit" disabled={loading} className="btn-full">
            {loading ? <><span className="spinner spinner-sm" /> Signing in…</> : 'Sign In'}
          </button>
        </form>

        <div className="auth-footer">
          <Link to="/register" className="register-link">
            Don't have an account? Register here
          </Link>
          <span
            className="back-link"
            onClick={() => { resetFormStates(); setSelectedRole(null); }}
          >
            ← Back to portal selection
          </span>
        </div>
      </div>
    </div>
  );
}
