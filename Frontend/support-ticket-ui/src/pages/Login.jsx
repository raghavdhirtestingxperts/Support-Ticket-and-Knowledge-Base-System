import React, { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { API_BASE_URL } from '../config';

export default function Login() {
  const navigate = useNavigate();

  const [selectedRole, setSelectedRole] = useState(null);

  const [loginEmail, setLoginEmail] = useState('');
  const [loginPassword, setLoginPassword] = useState('');
  const [loginError, setLoginError] = useState('');

  const resetFormStates = () => {
    setLoginEmail('');
    setLoginPassword('');
    setLoginError('');
  };

  useEffect(() => {
    resetFormStates();
  }, []);

  // submit login request
  const handleLogin = async (e) => {
    e.preventDefault();
    setLoginError('');

    try {
      const res = await fetch(`${API_BASE_URL}/api/auth/login`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ email: loginEmail, password: loginPassword })
      });

      if (!res.ok) {
        throw new Error('Invalid email or password');
      }

      const data = await res.json();

      if (data.role !== selectedRole) {
        throw new Error(`This account does not have permission to access the ${selectedRole} portal.`);
      }

      localStorage.setItem('token', data.token);
      localStorage.setItem('user', JSON.stringify({
        id: data.userId,
        name: data.name,
        email: data.email,
        role: data.role,
        tenantId: data.tenantId
      }));

      navigate('/');
      window.location.reload(); // refresh to load proper navbar
    } catch (err) {
      setLoginError(err.message);
    }
  };

  if (selectedRole === null) {
    return (
      <div className="auth-page">
        <div className="auth-card" style={{ textAlign: 'center' }}>
          <h2 style={{ fontSize: '1.4em', marginBottom: '8px', color: '#0066cc' }}>Welcome to Ticket Support System</h2>
          <h3 style={{ fontSize: '1.1em', fontWeight: 'normal', color: '#666666', marginBottom: '25px' }}>
            Please select the portal role you would like to log in to:
          </h3>
          <div style={{ display: 'flex', flexDirection: 'column', gap: '15px' }}>
            <button
              onClick={() => { resetFormStates(); setSelectedRole('Customer'); }}
              style={{ padding: '14px', fontSize: '15px' }}
            >
              Customer Portal
            </button>
            <button
              onClick={() => { resetFormStates(); setSelectedRole('Agent'); }}
              style={{ padding: '14px', fontSize: '15px', backgroundColor: '#008000', borderColor: '#006600' }}
            >
              Agent Portal
            </button>
            <button
              onClick={() => { resetFormStates(); setSelectedRole('Supervisor'); }}
              style={{ padding: '14px', fontSize: '15px', backgroundColor: '#475569', borderColor: '#334155' }}
            >
              Supervisor Portal
            </button>
          </div>


        </div>
      </div>
    );
  }

  return (
    <div className="auth-page">
      <div className="auth-card">
        <div style={{ marginBottom: '15px' }}>
          <a onClick={() => { resetFormStates(); setSelectedRole(null); }}>
            ← Back to Portal Selection
          </a>
        </div>
        <h2 style={{ fontSize: '1.3em', marginBottom: '4px', color: '#0066cc', textAlign: 'center' }}>
          Welcome to Ticket Support System
        </h2>
        <h3 style={{ fontSize: '1.1em', textAlign: 'center', marginBottom: '10px', fontWeight: '500' }}>
          Login ({selectedRole})
        </h3>
        


        {loginError && <p className="error-msg">{loginError}</p>}
        <form onSubmit={handleLogin} style={{ border: 'none', padding: 0, boxShadow: 'none', margin: 0 }}>
          <div className="form-group">
            <label>Email Address</label>
            <input
              type="email"
              value={loginEmail}
              onChange={(e) => setLoginEmail(e.target.value)}
              required
              autoComplete="username"
            />
          </div>
          <div className="form-group">
            <label>Password</label>
            <input
              type="password"
              value={loginPassword}
              onChange={(e) => setLoginPassword(e.target.value)}
              required
              autoComplete="current-password"
            />
          </div>
          <button
            type="submit"
            style={{
              width: '100%',
              ...(selectedRole === 'Agent'
                ? { backgroundColor: '#008000', borderColor: '#006600' }
                : selectedRole === 'Supervisor'
                ? { backgroundColor: '#475569', borderColor: '#334155' }
                : {})
            }}
          >
            Sign In
          </button>
        </form>
      </div>
    </div>
  );
}
