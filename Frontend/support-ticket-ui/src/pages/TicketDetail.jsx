import React, { useState, useEffect } from 'react';
import { useParams, Link } from 'react-router-dom';
import { API_BASE_URL } from '../config';
import {
  IconArrowLeft, IconClipboard, IconClock, IconMessageSquare,
  IconSettings, IconUser, IconZap, IconCheck
} from '../components/Icons';

function fmt(dt) {
  return new Date(dt).toLocaleString(undefined, {
    month: 'short', day: 'numeric', year: 'numeric', hour: '2-digit', minute: '2-digit',
  });
}

const ALLOWED_TRANSITIONS = {
  'Open': ['InProgress'],
  'InProgress': ['PendingCustomer', 'Resolved'],
  'PendingCustomer': ['InProgress'],
  'Resolved': ['Closed'],
  'Closed': [],
};

const STATUS_LABELS = {
  InProgress: 'In Progress',
  PendingCustomer: 'Pending Customer',
};

export default function TicketDetail() {
  const { id } = useParams();
  const [ticket, setTicket]               = useState(null);
  const [error, setError]                 = useState('');
  const [success, setSuccess]             = useState('');
  const [agentIdInput, setAgentIdInput]   = useState('');
  const [statusNote, setStatusNote]       = useState('');
  const [commentContent, setCommentContent] = useState('');
  const [actionLoading, setActionLoading] = useState(false);
  const [agents, setAgents]               = useState([]);

  const token = localStorage.getItem('token');
  const user  = JSON.parse(localStorage.getItem('user'));
  const isStaff = user && (user.role === 'Agent' || user.role === 'Supervisor');

  const fetchTicketDetails = async () => {
    setError('');
    try {
      const res = await fetch(`${API_BASE_URL}/api/tickets/${id}`, {
        headers: { Authorization: `Bearer ${token}` },
      });
      if (!res.ok) throw new Error('Ticket not found or access denied');
      setTicket(await res.json());
    } catch (err) { setError(err.message); }
  };

  const fetchAgents = async () => {
    try {
      const res = await fetch(`${API_BASE_URL}/api/users/agents`, {
        headers: { Authorization: `Bearer ${token}` },
      });
      if (res.ok) setAgents(await res.json());
    } catch (err) { console.error('Failed to load agents:', err); }
  };

  useEffect(() => {
    fetchTicketDetails();
    if (user && (user.role === 'Agent' || user.role === 'Supervisor')) {
      fetchAgents();
    }
  }, [id]);

  const withAction = async (fn) => {
    setError(''); setSuccess(''); setActionLoading(true);
    try { await fn(); } catch (err) { setError(err.message); }
    finally { setActionLoading(false); }
  };

  const handleAssignAgent = (e) => {
    e.preventDefault();
    withAction(async () => {
      const res = await fetch(`${API_BASE_URL}/api/tickets/${id}/assign`, {
        method: 'PUT',
        headers: { 'Content-Type': 'application/json', Authorization: `Bearer ${token}` },
        body: JSON.stringify({ agentUserId: parseInt(agentIdInput, 10) }),
      });
      if (!res.ok) { const d = await res.json(); throw new Error(d.message || 'Failed to assign'); }
      setSuccess('Ticket assigned!'); setAgentIdInput(''); fetchTicketDetails();
    });
  };

  const handleStatusUpdate = (newStatus) => {
    withAction(async () => {
      const res = await fetch(`${API_BASE_URL}/api/tickets/${id}/status`, {
        method: 'PUT',
        headers: { 'Content-Type': 'application/json', Authorization: `Bearer ${token}` },
        body: JSON.stringify({ newStatus, note: statusNote }),
      });
      if (!res.ok) { const d = await res.json(); throw new Error(d.message || 'Failed to update'); }
      setSuccess(`Status moved to ${STATUS_LABELS[newStatus] || newStatus}`);
      setStatusNote(''); fetchTicketDetails();
    });
  };

  const handleAddComment = (e) => {
    e.preventDefault();
    withAction(async () => {
      const res = await fetch(`${API_BASE_URL}/api/tickets/${id}/comments`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json', Authorization: `Bearer ${token}` },
        body: JSON.stringify({ content: commentContent }),
      });
      if (!res.ok) { const d = await res.json(); throw new Error(d.message || 'Failed to post'); }
      setSuccess('Comment posted!'); setCommentContent(''); fetchTicketDetails();
    });
  };

  if (error && !ticket) return (
    <div className="page-wrapper">
      <p className="error-msg">{error}</p>
      <Link to="/"><button className="btn-ghost"><IconArrowLeft size={14} /> Back to Dashboard</button></Link>
    </div>
  );

  if (!ticket) return <div className="loading loading-top"><div className="spinner" /> Loading ticket…</div>;

  const nextStatuses = ALLOWED_TRANSITIONS[ticket.status] || [];

  // shared detail & timeline blocks
  const renderDetails = () => (
    <div className="card">
      <div className="card-header">
        <div className="card-title"><IconClipboard size={16} /> Details</div>
      </div>
      <p className="detail-description">{ticket.description}</p>
      <div className="detail-grid">
        {[
          { label: 'Created By',    value: ticket.createdByName },
          { label: 'Assigned To',   value: ticket.assignedToName || '—' },
          { label: 'Created At',    value: fmt(ticket.createdAt) },
          { label: 'SLA Deadline',  value: fmt(ticket.slaDeadline) },
        ].map(row => (
          <div key={row.label} className="detail-cell">
            <div className="detail-cell-label">{row.label}</div>
            <div className="detail-cell-value">{row.value}</div>
          </div>
        ))}
      </div>
    </div>
  );

  const renderTimeline = () => (
    <div className="card">
      <div className="card-header">
        <div className="card-title"><IconClock size={16} /> Timeline</div>
        <span className="text-muted text-xs">
          {ticket.history?.length || 0} event{ticket.history?.length !== 1 ? 's' : ''}
        </span>
      </div>
      {!ticket.history || ticket.history.length === 0 ? (
        <p className="text-muted text-sm">No status changes yet.</p>
      ) : (
        <ul className="timeline">
          {ticket.history.map(h => (
            <li key={h.id} className="timeline-item">
              <div className="timeline-content">
                <div className="timeline-transition">
                  <span className="text-secondary">{h.oldStatus || 'Open'}</span>
                  <span className="timeline-arrow">→</span>
                  <strong>{STATUS_LABELS[h.newStatus] || h.newStatus}</strong>
                </div>
                {h.note && <div className="timeline-note">"{h.note}"</div>}
                <div className="timeline-time">by {h.changedByName} · {fmt(h.changedAt)}</div>
              </div>
            </li>
          ))}
        </ul>
      )}
    </div>
  );

  const renderComments = () => (
    <div className="card">
      <div className="card-header">
        <div className="card-title">
          <IconMessageSquare size={16} /> Comments
          {ticket.comments?.length > 0 && (
            <span className="comment-count">{ticket.comments.length}</span>
          )}
        </div>
      </div>

      <form onSubmit={handleAddComment} className="comment-form">
        <div className="form-group form-group-sm">
          <textarea
            placeholder="Write a comment…"
            value={commentContent}
            onChange={e => setCommentContent(e.target.value)}
            required
            maxLength={2000}
            className="comment-textarea"
          />
        </div>
        <button type="submit" className="btn-sm" disabled={actionLoading}>
          {actionLoading ? 'Posting…' : 'Post Comment'}
        </button>
      </form>

      <div className="comments-list">
        {!ticket.comments || ticket.comments.length === 0 ? (
          <p className="text-muted text-sm">No comments yet.</p>
        ) : (
          ticket.comments.map(c => (
            <div key={c.id} className="comment-card">
              <div className="comment-header">
                <span className="comment-author">{c.userName}</span>
                <span className="comment-time">{fmt(c.createdAt)}</span>
              </div>
              <div className="comment-body">{c.content}</div>
            </div>
          ))
        )}
      </div>
    </div>
  );

  return (
    <div className="page-wrapper">
      {/* back link */}
      <Link to="/" className="back-nav">
        <IconArrowLeft size={14} /> Back to Dashboard
      </Link>

      {/* heading */}
      <div className="ticket-heading">
        <div className="ticket-heading-badges">
          <span className="ticket-heading-id">TICKET #{ticket.id}</span>
          <span className={`badge badge-${ticket.priority.toLowerCase()}`}>{ticket.priority}</span>
          <span className={`badge badge-${ticket.status.replace(/\s+/g,'').toLowerCase()}`}>{ticket.status}</span>
          <span className={ticket.isSlaBreached ? 'badge badge-breached' : 'badge badge-onsla'}>
            {ticket.isSlaBreached ? <><IconZap size={11} /> SLA Breached</> : <><IconCheck size={11} /> On SLA</>}
          </span>
        </div>
        <h2 className="ticket-heading-title">{ticket.title}</h2>
      </div>

      {error   && <p className="error-msg">{error}</p>}
      {success && <p className="success-msg">{success}</p>}

      {isStaff ? (
        <div className="dashboard-grid-two-col">
          {/* left col */}
          <div>
            {renderDetails()}
            {renderTimeline()}
            {renderComments()}
          </div>

          {/* sidebar */}
          <div className="ticket-sidebar">
            {/* assign agent form */}
            {user.role === 'Agent' && ticket.status !== 'Closed' && (
              <div className="card card-no-mb">
                <div className="card-header">
                  <div className="card-title"><IconUser size={16} /> Assign Agent</div>
                </div>
                <form onSubmit={handleAssignAgent}>
                  <div className="form-group">
                    <label htmlFor="assign-agent">Select Agent</label>
                    <select
                      id="assign-agent"
                      value={agentIdInput}
                      onChange={e => setAgentIdInput(e.target.value)}
                      required
                    >
                      <option value="">Choose Agent...</option>
                      {agents.map(a => (
                        <option key={a.id} value={a.id}>
                          {a.name} ({a.email})
                        </option>
                      ))}
                    </select>
                  </div>
                  <button type="submit" disabled={actionLoading} className="btn-full">
                    {actionLoading ? 'Assigning…' : 'Assign'}
                  </button>
                </form>
              </div>
            )}

            {/* status form */}
            {(user.role === 'Agent' || user.role === 'Supervisor') && ticket.status !== 'Closed' && (
              <div className="card card-no-mb">
                <div className="card-header">
                  <div className="card-title"><IconSettings size={16} /> Manage Status</div>
                </div>

                <div className="status-current">
                  <div className="status-current-label">Current</div>
                  <span className={`badge badge-${ticket.status.replace(/\s+/g,'').toLowerCase()} badge-md`}>
                    {STATUS_LABELS[ticket.status] || ticket.status}
                  </span>
                </div>

                <div className="form-group">
                  <label htmlFor="status-note">Transition Note</label>
                  <input id="status-note" type="text" placeholder="Optional note…" value={statusNote}
                    onChange={e => setStatusNote(e.target.value)} maxLength={500} />
                </div>

                <div className="status-buttons">
                  {nextStatuses.length === 0 ? (
                    <p className="text-muted text-sm">No transitions available.</p>
                  ) : (
                    nextStatuses.map(s => (
                      <button key={s} className="btn-status" onClick={() => handleStatusUpdate(s)} disabled={actionLoading}>
                        Move to {STATUS_LABELS[s] || s}
                      </button>
                    ))
                  )}
                </div>
              </div>
            )}
          </div>
        </div>
      ) : (
        <div className="content-narrow">
          {/* info message */}
          <div className="card card-info">
            <div className="card-info-text">
              <strong>Need an update?</strong><br />
              Use the comments section to add more details or ask the support team.
            </div>
          </div>

          {renderDetails()}
          {renderTimeline()}
          {renderComments()}
        </div>
      )}
    </div>
  );
}
