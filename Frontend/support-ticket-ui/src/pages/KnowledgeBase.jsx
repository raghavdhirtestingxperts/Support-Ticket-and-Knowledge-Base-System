import React, { useState, useEffect } from 'react';
import { API_BASE_URL } from '../config';
import { IconSearch, IconFileText, IconEdit, IconTrash, IconSave, IconSend } from '../components/Icons';

export default function KnowledgeBase() {
  const [articles, setArticles]       = useState([]);
  const [searchQuery, setSearchQuery] = useState('');
  const [error, setError]             = useState('');
  const [success, setSuccess]         = useState('');
  const [loading, setLoading]         = useState(true);
  const [submitting, setSubmitting]   = useState(false);
  const [editingArticleId, setEditingArticleId] = useState(null);

  const [title, setTitle] = useState('');
  const [body, setBody]   = useState('');
  const [tags, setTags]   = useState('');

  const token = localStorage.getItem('token');
  const user  = JSON.parse(localStorage.getItem('user'));
  const isStaff = user && (user.role === 'Agent' || user.role === 'Supervisor');

  const fetchArticles = async () => {
    setError(''); setLoading(true);
    try {
      const url = searchQuery
        ? `${API_BASE_URL}/api/kb/articles?search=${encodeURIComponent(searchQuery)}`
        : `${API_BASE_URL}/api/kb/articles`;
      const res = await fetch(url, { headers: { Authorization: `Bearer ${token}` } });
      if (!res.ok) throw new Error('Failed to fetch articles');
      setArticles(await res.json());
    } catch (err) {
      setError(err.message);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => { fetchArticles(); }, [searchQuery]);

  const handleStartEdit = (article) => {
    setError(''); setSuccess('');
    setEditingArticleId(article.id);
    setTitle(article.title);
    setBody(article.body);
    setTags(article.tags || '');
    window.scrollTo({ top: 0, behavior: 'smooth' });
  };

  const handleCancelEdit = () => {
    setEditingArticleId(null);
    setTitle(''); setBody(''); setTags('');
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    setError(''); setSuccess(''); setSubmitting(true);
    try {
      const method = editingArticleId ? 'PUT' : 'POST';
      const url = editingArticleId 
        ? `${API_BASE_URL}/api/kb/articles/${editingArticleId}`
        : `${API_BASE_URL}/api/kb/articles`;
      
      const res = await fetch(url, {
        method,
        headers: { 'Content-Type': 'application/json', Authorization: `Bearer ${token}` },
        body: JSON.stringify({ title, body, tags }),
      });
      if (!res.ok) {
        const errText = await res.text();
        throw new Error(errText || `Failed to ${editingArticleId ? 'update' : 'create'} article`);
      }
      setSuccess(`Article ${editingArticleId ? 'updated' : 'published'}!`);
      setEditingArticleId(null);
      setTitle(''); setBody(''); setTags('');
      fetchArticles();
    } catch (err) {
      setError(err.message);
    } finally {
      setSubmitting(false);
    }
  };

  const handleDeleteArticle = async (articleId) => {
    if (!window.confirm('Are you sure you want to delete this article?')) return;
    setError(''); setSuccess('');
    try {
      const res = await fetch(`${API_BASE_URL}/api/kb/articles/${articleId}`, {
        method: 'DELETE',
        headers: { Authorization: `Bearer ${token}` },
      });
      if (!res.ok) {
        const errText = await res.text();
        throw new Error(errText || 'Failed to delete article');
      }
      setSuccess('Article deleted.');
      if (editingArticleId === articleId) handleCancelEdit();
      fetchArticles();
    } catch (err) {
      setError(err.message);
    }
  };

  const renderArticleCard = (a, i) => (
    <div key={a.id} className="article-card" style={{ animationDelay: `${i * 0.05}s` }}>
      <div className="article-title">{a.title}</div>
      <div className="article-meta">
        <span><IconEdit size={12} /> {a.createdByName}</span>
        <span>&middot;</span>
        <span>{new Date(a.createdAt).toLocaleDateString(undefined, { year: 'numeric', month: 'short', day: 'numeric' })}</span>
        {a.tags && a.tags.split(',').map(tag => (
          <span key={tag.trim()} className="article-tag">#{tag.trim()}</span>
        ))}
      </div>
      <div className="article-body">{a.body}</div>
      {isStaff && (user.role === 'Supervisor' || a.createdByUserId === user.id) && (
        <div className="article-actions">
          <button className="btn-ghost btn-sm" onClick={() => handleStartEdit(a)}>
            <IconEdit size={13} /> Edit
          </button>
          <button className="btn-ghost btn-sm btn-danger" onClick={() => handleDeleteArticle(a.id)}>
            <IconTrash size={13} /> Delete
          </button>
        </div>
      )}
    </div>
  );

  return (
    <div className="page-wrapper">
      <div className="section-header">
        <h2>Knowledge Base</h2>
        <span className="text-muted text-sm">
          {articles.length} article{articles.length !== 1 ? 's' : ''}
        </span>
      </div>

      {error   && <p className="error-msg">{error}</p>}
      {success && <p className="success-msg">{success}</p>}

      {/* search bar */}
      <div className="search-bar">
        <span className="search-icon"><IconSearch size={16} /></span>
        <input
          type="text"
          placeholder="Search articles by title, content, or tags…"
          value={searchQuery}
          onChange={(e) => setSearchQuery(e.target.value)}
        />
      </div>

      {isStaff ? (
        <div className="dashboard-grid cols-2" style={{ alignItems: 'start' }}>
          {/* article list */}
          <div>
            {loading ? (
              <div className="loading"><div className="spinner" /> Loading articles…</div>
            ) : articles.length === 0 ? (
              <div className="card">
                <div className="empty-state">
                  <div className="empty-state-icon"><IconFileText size={36} /></div>
                  <p>{searchQuery ? `No articles match "${searchQuery}"` : 'No articles yet.'}</p>
                </div>
              </div>
            ) : (
              articles.map((a, i) => renderArticleCard(a, i))
            )}
          </div>

          {/* publish/edit form */}
          <div className="card">
            <div className="card-header">
              <div className="card-title">
                <div className="card-icon card-icon-green"><IconEdit size={14} /></div>
                {editingArticleId ? `Edit Article #${editingArticleId}` : 'Publish Article'}
              </div>
            </div>

            <form onSubmit={handleSubmit}>
              <div className="form-group">
                <label htmlFor="kb-title">Title</label>
                <input
                  id="kb-title"
                  type="text"
                  value={title}
                  onChange={(e) => setTitle(e.target.value)}
                  placeholder="Article title"
                  required
                  maxLength={200}
                />
              </div>
              <div className="form-group">
                <label htmlFor="kb-tags">Tags</label>
                <input
                  id="kb-tags"
                  type="text"
                  placeholder="e.g. login, error, windows"
                  value={tags}
                  onChange={(e) => setTags(e.target.value)}
                  maxLength={500}
                />
              </div>
              <div className="form-group">
                <label htmlFor="kb-body">Content</label>
                <textarea
                  id="kb-body"
                  value={body}
                  onChange={(e) => setBody(e.target.value)}
                  placeholder="Write the article content…"
                  required
                  maxLength={10000}
                  className="textarea-tall"
                />
              </div>
              
              <div className="form-actions">
                <button type="submit" className="btn-success" disabled={submitting} style={{ flex: 1 }}>
                  {submitting ? (
                    <><span className="spinner spinner-sm" /> Saving...</>
                  ) : editingArticleId ? (
                    <><IconSave size={14} /> Save Changes</>
                  ) : (
                    <><IconSend size={14} /> Publish Article</>
                  )}
                </button>
                {editingArticleId && (
                  <button type="button" className="btn-ghost" onClick={handleCancelEdit}>
                    Cancel
                  </button>
                )}
              </div>
            </form>
          </div>
        </div>
      ) : (
        <div className="content-narrow">
          {loading ? (
            <div className="loading"><div className="spinner" /> Loading articles…</div>
          ) : articles.length === 0 ? (
            <div className="card">
              <div className="empty-state">
                <div className="empty-state-icon"><IconFileText size={36} /></div>
                <p>{searchQuery ? `No articles match "${searchQuery}"` : 'No articles yet.'}</p>
              </div>
            </div>
          ) : (
            articles.map((a, i) => renderArticleCard(a, i))
          )}
        </div>
      )}
    </div>
  );
}
