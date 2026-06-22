import React from 'react';

export default function Footer() {
  return (
    <footer className="app-footer">
      <div className="footer-inner">
        <span className="footer-brand">SupportDesk</span>
        <span className="footer-sep">·</span>
        <span className="footer-copy">&copy; {new Date().getFullYear()} All rights reserved</span>
      </div>
    </footer>
  );
}
