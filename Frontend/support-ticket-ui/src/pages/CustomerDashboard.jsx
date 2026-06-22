import React, { useState, useEffect } from 'react';
import { Link } from 'react-router-dom';
import { API_BASE_URL } from '../config';
import { IconClock, IconZap, IconCheck, IconPlus, IconTicket } from '../components/Icons';

function fmt(dt) {
  return new Date(dt).toLocaleString(undefined, {
    month: 'short', day: 'numeric', hour: '2-digit', minute: '2-digit',
  });
}

function TicketCard({ t }) {
  return (
    <div className="ticket-card">
      <div className="ticket-card-header">
        <span className="ticket-card-id">#{t.id}</span>
        <div className="badge-group">
          <span className={`badge badge-${t.priority.toLowerCase()}`}>{t.priority}</span>
          <span className={`badge badge-${t.status.replace(/\s+/g, '').toLowerCase()}`}>{t.status}</span>
        </div>
      </div>
      <div className="ticket-card-title">{t.title}</div>
      <div className="ticket-card-meta">
        <span className="meta-item"><IconClock size={12} /> {fmt(t.slaDeadline)}</span>
        <span className={t.isSlaBreached ? 'badge badge-breached badge-xs' : 'badge badge-onsla badge-xs'}>
          {t.isSlaBreached ? <><IconZap size={10} /> Breached</> : <><IconCheck size={10} /> On SLA</>}
        </span>
      </div>
      <div className="ticket-card-footer">
        <Link to={`/ticket/${t.id}`}>
          <button className="btn-ghost btn-sm btn-full">View Details</button>
        </Link>
      </div>
    </div>
  );
}

export default function CustomerDashboard() {
  const [tickets, setTickets] = useState([]);
  const [error, setError]     = useState('');
  const [success, setSuccess] = useState('');
  const [loading, setLoading] = useState(true);
  const [submitting, setSubmitting] = useState(false);

  const [title, setTitle]             = useState('');
  const [description, setDescription] = useState('');
  const [priority, setPriority]       = useState('Medium');

  const token = localStorage.getItem('token');

  const fetchMyTickets = async () => {
    try {
      const res = await fetch(`${API_BASE_URL}/api/tickets/my`, {
        headers: { Authorization: `Bearer ${token}` },
      });
      if (!res.ok) throw new Error('Failed to fetch tickets');
      setTickets(await res.json());
    } catch (err) {
      setError(err.message);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => { fetchMyTickets(); }, []);

  const handleSubmitTicket = async (e) => {
    e.preventDefault();
    setError(''); setSuccess(''); setSubmitting(true);
    try {
      const res = await fetch(`${API_BASE_URL}/api/tickets`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json', Authorization: `Bearer ${token}` },
        body: JSON.stringify({ title, description, priority }),
      });
      if (!res.ok) {
        const errText = await res.text();
        throw new Error(errText || 'Failed to create ticket');
      }
      setSuccess('Ticket created successfully!');
      setTitle(''); setDescription(''); setPriority('Medium');
      fetchMyTickets();
    } catch (err) {
      setError(err.message);
    } finally {
      setSubmitting(false);
    }
  };

  const open     = tickets.filter(t => t.status === 'Open').length;
  const active   = tickets.filter(t => t.status === 'InProgress' || t.status === 'PendingCustomer').length;
  const resolved = tickets.filter(t => t.status === 'Resolved' || t.status === 'Closed').length;

  return (
    <div className="page-wrapper">
      <div className="section-header">
        <h2>Customer Panel</h2>
      </div>

      {error   && <p className="error-msg">{error}</p>}
      {success && <p className="success-msg">{success}</p>}

      {/* stats */}
      {tickets.length > 0 && (
        <div className="stat-grid stat-grid-3">
          <div className="stat-card">
            <div className="stat-label">Open</div>
            <div className="stat-value">{open}</div>
          </div>
          <div className="stat-card">
            <div className="stat-label">In Progress</div>
            <div className="stat-value is-info">{active}</div>
          </div>
          <div className="stat-card">
            <div className="stat-label">Resolved</div>
            <div className="stat-value is-success">{resolved}</div>
          </div>
        </div>
      )}

      <div className="customer-dashboard-grid">
        {/* tickets list */}
        <div>
          <div className="section-header section-header-sm">
            <h3>My Tickets</h3>
            <span className="text-muted text-sm">{tickets.length} total</span>
          </div>

          {loading ? (
            <div className="loading"><div className="spinner" /> Loading tickets…</div>
          ) : tickets.length === 0 ? (
            <div className="card">
              <div className="empty-state">
                <div className="empty-state-icon"><IconTicket size={36} /></div>
                <p>No tickets yet. Submit your first ticket →</p>
              </div>
            </div>
          ) : (
            <div className="ticket-card-grid">
              {tickets.map(t => <TicketCard key={t.id} t={t} />)}
            </div>
          )}
        </div>

        {/* submit form */}
        <div className="card sticky-sidebar">
          <div className="card-header">
            <div className="card-title">
              <div className="card-icon card-icon-accent"><IconPlus size={16} /></div>
              New Ticket
            </div>
          </div>
          <form onSubmit={handleSubmitTicket}>
            <div className="form-group">
              <label htmlFor="ticket-title">Title</label>
              <input id="ticket-title" type="text" value={title} onChange={e => setTitle(e.target.value)}
                placeholder="Brief summary of the issue" required maxLength={200} />
            </div>
            <div className="form-group">
              <label htmlFor="ticket-desc">Description</label>
              <textarea id="ticket-desc" value={description} onChange={e => setDescription(e.target.value)}
                placeholder="Describe the issue in detail…" required maxLength={5000} />
            </div>
            <div className="form-group">
              <label htmlFor="ticket-priority">Priority</label>
              <select id="ticket-priority" value={priority} onChange={e => setPriority(e.target.value)}>
                <option value="Low">Low</option>
                <option value="Medium">Medium</option>
                <option value="High">High</option>
                <option value="Critical">Critical</option>
              </select>
            </div>
            <button type="submit" disabled={submitting} className="btn-full">
              {submitting
                ? <><span className="spinner spinner-sm" /> Submitting…</>
                : 'Submit Ticket'}
            </button>
          </form>
        </div>
      </div>
    </div>
  );
}
