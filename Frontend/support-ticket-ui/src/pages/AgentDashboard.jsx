import React, { useState, useEffect } from 'react';
import { Link } from 'react-router-dom';
import { API_BASE_URL } from '../config';
import { IconZap, IconCheck, IconSearch, IconX } from '../components/Icons';

function fmt(dt) {
  return new Date(dt).toLocaleString(undefined, {
    month: 'short', day: 'numeric', hour: '2-digit', minute: '2-digit',
  });
}

export default function AgentDashboard() {
  const [tickets, setTickets]               = useState([]);
  const [error, setError]                   = useState('');
  const [loading, setLoading]               = useState(true);
  const [statusFilter, setStatusFilter]     = useState('');
  const [priorityFilter, setPriorityFilter] = useState('');
  const [assigneeFilter, setAssigneeFilter] = useState('');
  const [agents, setAgents]                 = useState([]);

  const token = localStorage.getItem('token');

  const fetchTickets = async () => {
    setError(''); setLoading(true);
    try {
      let url = `${API_BASE_URL}/api/tickets?`;
      if (statusFilter)   url += `status=${encodeURIComponent(statusFilter)}&`;
      if (priorityFilter) url += `priority=${encodeURIComponent(priorityFilter)}&`;
      if (assigneeFilter) url += `assignedToUserId=${encodeURIComponent(assigneeFilter)}&`;
      const res = await fetch(url, { headers: { Authorization: `Bearer ${token}` } });
      if (!res.ok) throw new Error('Failed to fetch tickets');
      setTickets(await res.json());
    } catch (err) {
      setError(err.message);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => { fetchTickets(); }, [statusFilter, priorityFilter, assigneeFilter]);

  useEffect(() => {
    const fetchAgents = async () => {
      try {
        const res = await fetch(`${API_BASE_URL}/api/users/agents`, {
          headers: { Authorization: `Bearer ${token}` },
        });
        if (res.ok) setAgents(await res.json());
      } catch (err) {
        console.error('Failed to load agents:', err);
      }
    };
    fetchAgents();
  }, []);

  const clearFilters = () => { setStatusFilter(''); setPriorityFilter(''); setAssigneeFilter(''); };
  const hasFilters   = statusFilter || priorityFilter || assigneeFilter;

  return (
    <div className="page-wrapper">
      <div className="section-header">
        <h2>Tickets Board</h2>
        <span className="text-muted text-sm">
          {loading ? '…' : `${tickets.length} tickets`}
        </span>
      </div>

      {error && <p className="error-msg">{error}</p>}

      {/* filters */}
      <div className="filters-bar">
        <div className="filter-group">
          <label htmlFor="filter-status">Status</label>
          <select id="filter-status" value={statusFilter} onChange={e => setStatusFilter(e.target.value)}>
            <option value="">All Statuses</option>
            <option value="Open">Open</option>
            <option value="InProgress">In Progress</option>
            <option value="PendingCustomer">Pending Customer</option>
            <option value="Resolved">Resolved</option>
            <option value="Closed">Closed</option>
          </select>
        </div>
        <div className="filter-group">
          <label htmlFor="filter-priority">Priority</label>
          <select id="filter-priority" value={priorityFilter} onChange={e => setPriorityFilter(e.target.value)}>
            <option value="">All Priorities</option>
            <option value="Low">Low</option>
            <option value="Medium">Medium</option>
            <option value="High">High</option>
            <option value="Critical">Critical</option>
          </select>
        </div>
        <div className="filter-group">
          <label htmlFor="filter-assignee">Assignee</label>
          <select id="filter-assignee" value={assigneeFilter} onChange={e => setAssigneeFilter(e.target.value)}>
            <option value="">All Agents</option>
            {agents.map(a => (
              <option key={a.id} value={a.id}>
                {a.name} ({a.email})
              </option>
            ))}
          </select>
        </div>
        {hasFilters && (
          <button className="btn-ghost btn-sm filter-clear-btn" onClick={clearFilters}>
            <IconX size={12} /> Clear
          </button>
        )}
      </div>

      {loading ? (
        <div className="loading"><div className="spinner" /> Loading tickets…</div>
      ) : tickets.length === 0 ? (
        <div className="card">
          <div className="empty-state">
            <div className="empty-state-icon"><IconSearch size={36} /></div>
            <p>No tickets match the current filters.</p>
          </div>
        </div>
      ) : (
        /* tickets table */
        <div className="table-wrapper">
          <table>
            <thead>
              <tr>
                <th>ID</th>
                <th>Title</th>
                <th>Priority</th>
                <th>Status</th>
                <th>Created By</th>
                <th>Assigned To</th>
                <th>SLA Deadline</th>
                <th>Actions</th>
              </tr>
            </thead>
            <tbody>
              {tickets.map(t => (
                <tr key={t.id}>
                  <td className="table-id">#{t.id}</td>
                  <td className="table-title">
                    <Link to={`/ticket/${t.id}`} className="table-link">
                      {t.title}
                    </Link>
                  </td>
                  <td>
                    <span className={`badge badge-${t.priority.toLowerCase()}`}>{t.priority}</span>
                  </td>
                  <td>
                    <span className={`badge badge-${t.status.replace(/\s+/g, '').toLowerCase()}`}>{t.status}</span>
                  </td>
                  <td>{t.createdByName}</td>
                  <td>
                    {t.assignedToName ? (
                      <strong>{t.assignedToName}</strong>
                    ) : (
                      <em className="text-muted">Unassigned</em>
                    )}
                  </td>
                  <td>
                    <span className="sla-cell">
                      {fmt(t.slaDeadline)}
                      <span className={t.isSlaBreached ? 'badge badge-breached badge-xs' : 'badge badge-onsla badge-xs'}>
                        {t.isSlaBreached ? <><IconZap size={9} /> Breached</> : <><IconCheck size={9} /></>}
                      </span>
                    </span>
                  </td>
                  <td className="table-action">
                    <Link to={`/ticket/${t.id}`}>
                      <button className="btn-sm btn-ghost">Manage</button>
                    </Link>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}
    </div>
  );
}
