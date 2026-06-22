import React, { useState, useEffect } from 'react';
import { Link } from 'react-router-dom';
import { API_BASE_URL } from '../config';
import { IconRefresh, IconUsers, IconZap, IconCheck, IconUser } from '../components/Icons';

function fmt(dt) {
  return new Date(dt).toLocaleString(undefined, {
    month: 'short', day: 'numeric', hour: '2-digit', minute: '2-digit',
  });
}

export default function SupervisorDashboard() {
  const [workloads, setWorkloads]             = useState([]);
  const [breachedTickets, setBreachedTickets] = useState([]);
  const [workloadError, setWorkloadError]     = useState('');
  const [breachedError, setBreachedError]     = useState('');
  const [loading, setLoading]                 = useState(true);

  const fetchData = async () => {
    const token = localStorage.getItem('token');
    setLoading(true);
    try {
      const res = await fetch(`${API_BASE_URL}/api/dashboard/agent-workload`, {
        headers: { Authorization: `Bearer ${token}` },
      });
      if (!res.ok) throw new Error('Failed to load agent workloads');
      setWorkloads(await res.json());
    } catch (err) { setWorkloadError(err.message); }

    try {
      const res = await fetch(`${API_BASE_URL}/api/tickets/breached`, {
        headers: { Authorization: `Bearer ${token}` },
      });
      if (!res.ok) throw new Error('Failed to load breached tickets');
      setBreachedTickets(await res.json());
    } catch (err) { setBreachedError(err.message); }
    setLoading(false);
  };

  useEffect(() => { fetchData(); }, []);

  const totalOpen     = workloads.reduce((s, w) => s + w.openTicketCount, 0);
  const totalResolved = workloads.reduce((s, w) => s + w.resolvedTicketCount, 0);
  const totalBreached = breachedTickets.length;

  return (
    <div className="page-wrapper">
      <div className="section-header section-header-mb">
        <h2>Supervisor Dashboard</h2>
        <button onClick={fetchData} className="btn-ghost btn-sm">
          <IconRefresh size={14} /> Refresh
        </button>
      </div>

      {/* kpi stat cards */}
      <div className="stat-grid">
        <div className="stat-card">
          <div className="stat-label">Active Agents</div>
          <div className="stat-value is-info">{workloads.length}</div>
        </div>
        <div className="stat-card">
          <div className="stat-label">Open Tickets</div>
          <div className={`stat-value ${totalOpen > 0 ? 'is-warning' : ''}`}>{totalOpen}</div>
        </div>
        <div className="stat-card">
          <div className="stat-label">Resolved</div>
          <div className="stat-value is-success">{totalResolved}</div>
        </div>
        <div className="stat-card">
          <div className="stat-label">SLA Breached</div>
          <div className={`stat-value ${totalBreached > 0 ? 'is-danger' : 'is-success'}`}>{totalBreached}</div>
        </div>
      </div>

      {loading ? (
        <div className="loading"><div className="spinner" /> Loading data…</div>
      ) : (
        <div className="dashboard-grid cols-eq" style={{ alignItems: 'start' }}>

          {/* agent workloads */}
          <div>
            <div className="section-header section-header-sm">
              <h3>Agent Workloads</h3>
            </div>
            {workloadError && <p className="error-msg">{workloadError}</p>}
            {workloads.length === 0 ? (
              <div className="card">
                <div className="empty-state">
                  <div className="empty-state-icon"><IconUsers size={36} /></div>
                  <p>No agent data available.</p>
                </div>
              </div>
            ) : (
              <div className="card-stack">
                {workloads.map(w => (
                  <div key={w.agentUserId} className="card card-compact">
                    {/* agent identity */}
                    <div className="agent-identity">
                      <div className="agent-avatar">
                        {w.agentName.charAt(0).toUpperCase()}
                      </div>
                      <div>
                        <div className="agent-name">{w.agentName}</div>
                        <div className="agent-meta">Agent · ID {w.agentUserId}</div>
                      </div>
                      {w.breachedTicketCount > 0 && (
                        <span className="badge badge-breached agent-breach-badge">
                          <IconZap size={10} /> {w.breachedTicketCount} breached
                        </span>
                      )}
                    </div>

                    {/* stats */}
                    <div className="agent-stats">
                      {[
                        { label: 'Open',     value: w.openTicketCount,     color: w.openTicketCount > 5 ? 'var(--amber)' : 'var(--text-primary)' },
                        { label: 'Resolved', value: w.resolvedTicketCount, color: 'var(--green)' },
                        { label: 'Breached', value: w.breachedTicketCount, color: w.breachedTicketCount > 0 ? 'var(--red)' : 'var(--text-muted)' },
                      ].map(s => (
                        <div key={s.label} className="agent-stat-cell">
                          <div className="agent-stat-value" style={{ color: s.color }}>{s.value}</div>
                          <div className="agent-stat-label">{s.label}</div>
                        </div>
                      ))}
                    </div>
                  </div>
                ))}
              </div>
            )}
          </div>

          {/* breached tickets */}
          <div>
            <div className="section-header section-header-sm">
              <h3 className={totalBreached > 0 ? 'text-danger' : ''}>
                {totalBreached > 0 ? <><IconZap size={16} /> SLA Breached ({totalBreached})</> : <><IconCheck size={16} /> SLA Status</>}
              </h3>
            </div>
            {breachedError && <p className="error-msg">{breachedError}</p>}
            {breachedTickets.length === 0 ? (
              <div className="card">
                <div className="empty-state">
                  <div className="empty-state-icon"><IconCheck size={36} /></div>
                  <p>All tickets are within SLA — great work!</p>
                </div>
              </div>
            ) : (
              <div className="card-stack">
                {breachedTickets.map(t => (
                  <div key={t.id} className="card card-compact card-breached">
                    <div className="breached-header">
                      <div>
                        <div className="breached-id">#{t.id}</div>
                        <div className="breached-title">{t.title}</div>
                      </div>
                      <span className="badge badge-breached">+{t.hoursOverdue}h overdue</span>
                    </div>

                    <div className="badge-row">
                      <span className={`badge badge-${t.priority.toLowerCase()}`}>{t.priority}</span>
                      <span className={`badge badge-${t.status.replace(/\s+/g,'').toLowerCase()}`}>{t.status}</span>
                      {t.assignedToName
                        ? <span className="text-secondary text-sm"><IconUser size={12} /> {t.assignedToName}</span>
                        : <span className="text-muted text-sm text-italic">Unassigned</span>}
                    </div>

                    <Link to={`/ticket/${t.id}`}>
                      <button className="btn-ghost btn-sm btn-full">View &amp; Reassign</button>
                    </Link>
                  </div>
                ))}
              </div>
            )}
          </div>

        </div>
      )}
    </div>
  );
}
