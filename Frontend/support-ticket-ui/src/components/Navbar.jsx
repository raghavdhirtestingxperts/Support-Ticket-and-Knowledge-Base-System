import React from 'react';
import { Link, useLocation, useNavigate } from 'react-router-dom';
import { IconTicket, IconBookOpen, IconChart } from './Icons';

export default function Navbar() {
  const navigate = useNavigate();
  const location = useLocation();
  const userStr = localStorage.getItem('user');

  if (!userStr) return null;
  const user = JSON.parse(userStr);

  const handleLogout = () => {
    localStorage.removeItem('token');
    localStorage.removeItem('user');
    navigate('/login');
  };

  const initials = user.name
    ? user.name.split(' ').map(n => n[0]).join('').slice(0, 2).toUpperCase()
    : '?';

  const isActive = (path) => location.pathname === path ? 'active' : '';

  return (
    <nav>
      <div className="nav-left">
        <div className="nav-brand">
          <div className="nav-brand-icon"><IconTicket size={14} /></div>
          SupportDesk
        </div>

        <div className="links">
          {user.role === 'Customer' && (
            <>
              <Link to="/customer" className={isActive('/customer')}>
                <IconTicket size={14} /> My Tickets
              </Link>
              <Link to="/kb" className={isActive('/kb')}>
                <IconBookOpen size={14} /> Knowledge Base
              </Link>
            </>
          )}
          {(user.role === 'Agent' || user.role === 'Supervisor') && (
            <>
              <Link to="/board" className={isActive('/board')}>
                <IconTicket size={14} /> Tickets Board
              </Link>
              <Link to="/kb" className={isActive('/kb')}>
                <IconBookOpen size={14} /> Knowledge Base
              </Link>
            </>
          )}
          {user.role === 'Supervisor' && (
            <Link to="/supervisor" className={isActive('/supervisor')}>
              <IconChart size={14} /> Dashboard
            </Link>
          )}
        </div>
      </div>

      <div className="user-info">
        <div className="user-pill">
          <div className="user-avatar">{initials}</div>
          <span>{user.name}</span>
          <span className="user-pill-sep">&middot;</span>
          <span className="user-pill-role">{user.role}</span>
        </div>
        <button className="btn-logout" onClick={handleLogout}>Sign out</button>
      </div>
    </nav>
  );
}
