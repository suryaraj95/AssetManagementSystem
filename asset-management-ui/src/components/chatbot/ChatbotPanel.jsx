import React, { useState, useRef, useEffect } from 'react';
import useAuthStore from '../../store/authStore';

const API_BASE = import.meta.env.VITE_API_URL || 'http://localhost:5001';

// Renders basic markdown: **bold**, bullet lists, code blocks
function MessageBubble({ role, content }) {
  const isUser = role === 'user';

  // Simple markdown-like renderer
  const renderContent = (text) => {
    const lines = text.split('\n');
    const elements = [];
    let i = 0;
    while (i < lines.length) {
      const line = lines[i];

      // --- 1. Code Block ---
      if (line.trim().startsWith('```')) {
        let code = [];
        i++;
        while (i < lines.length && !lines[i].trim().startsWith('```')) {
          code.push(lines[i]);
          i++;
        }
        elements.push(
          <pre key={i} className="chatbot-code-block">
            <code>{code.join('\n')}</code>
          </pre>
        );
      }
      // --- 2. Table ---
      else if (line.trim().startsWith('|')) {
        const tableRows = [];
        while (i < lines.length && lines[i].trim().startsWith('|')) {
          const row = lines[i].trim();
          // Skip separator lines like |---|---|
          if (!row.match(/^\|[\s\-\|]+$/)) {
            const cells = row.split('|').filter((_, idx, arr) => idx > 0 && idx < arr.length - 1);
            tableRows.push(cells.map(c => c.trim()));
          }
          i++;
        }
        if (tableRows.length > 0) {
          elements.push(
            <div key={i} style={{ overflowX: 'auto', margin: '8px 0' }}>
              <table className="chatbot-table">
                <thead>
                  <tr>
                    {tableRows[0].map((cell, idx) => (
                      <th key={idx}>{formatInline(cell)}</th>
                    ))}
                  </tr>
                </thead>
                <tbody>
                  {tableRows.slice(1).map((row, rIdx) => (
                    <tr key={rIdx}>
                      {row.map((cell, cIdx) => (
                        <td key={cIdx}>{formatInline(cell)}</td>
                      ))}
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
          );
          i--; // adjust because the outer while also increments
        }
      }
      // --- 3. Bullet List ---
      else if (line.trim().startsWith('- ') || line.trim().startsWith('* ')) {
        elements.push(
          <li key={i} style={{ marginLeft: 14, listStyleType: 'disc', fontSize: 13, marginBottom: 2 }}>
            {formatInline(line.trim().slice(2))}
          </li>
        );
      }
      // --- 4. Numbered List ---
      else if (line.trim().match(/^\d+\. /)) {
        elements.push(
          <li key={i} style={{ marginLeft: 14, listStyleType: 'decimal', fontSize: 13, marginBottom: 2 }}>
            {formatInline(line.trim().replace(/^\d+\. /, ''))}
          </li>
        );
      }
      // --- 5. Headers ---
      else if (line.startsWith('### ')) {
        elements.push(<p key={i} style={{ fontWeight: 700, fontSize: 13, margin: '6px 0 2px' }}>{line.slice(4)}</p>);
      } else if (line.startsWith('## ')) {
        elements.push(<p key={i} style={{ fontWeight: 700, fontSize: 14, margin: '8px 0 4px', borderBottom: '1px solid #e2e8f0', paddingBottom: 2 }}>{line.slice(3)}</p>);
      }
      // --- 6. Empty Line ---
      else if (line.trim() === '') {
        elements.push(<div key={i} style={{ height: 8 }} />);
      }
      // --- 7. Regular Text ---
      else {
        elements.push(<p key={i} style={{ margin: '2px 0', fontSize: 13, lineHeight: '1.5' }}>{formatInline(line)}</p>);
      }
      i++;
    }
    return elements;
  };

  const formatInline = (text) => {
    if (!text) return '';
    // **bold**
    const parts = text.split(/(\*\*[^*]+\*\*)/g);
    return parts.map((p, idx) =>
      p.startsWith('**') && p.endsWith('**')
        ? <strong key={idx}>{p.slice(2, -2)}</strong>
        : p
    );
  };

  return (
    <div style={{
      display: 'flex',
      justifyContent: isUser ? 'flex-end' : 'flex-start',
      marginBottom: 10,
    }}>
      {!isUser && (
        <div style={{
          width: 28, height: 28, borderRadius: '50%', flexShrink: 0,
          background: 'linear-gradient(135deg, #6366f1, #8b5cf6)',
          display: 'flex', alignItems: 'center', justifyContent: 'center',
          fontSize: 13, color: '#fff', marginRight: 8, marginTop: 2
        }}>🤖</div>
      )}
      <div style={{
        maxWidth: '82%',
        background: isUser
          ? 'linear-gradient(135deg, #6366f1 0%, #8b5cf6 100%)'
          : '#f1f5f9',
        color: isUser ? '#fff' : '#1e293b',
        borderRadius: isUser ? '16px 4px 16px 16px' : '4px 16px 16px 16px',
        padding: '10px 14px',
        boxShadow: '0 1px 3px rgba(0,0,0,0.08)',
      }}>
        {renderContent(content)}
      </div>
      {isUser && (
        <div style={{
          width: 28, height: 28, borderRadius: '50%', flexShrink: 0,
          background: '#e2e8f0',
          display: 'flex', alignItems: 'center', justifyContent: 'center',
          fontSize: 13, marginLeft: 8, marginTop: 2
        }}>👤</div>
      )}
    </div>
  );
}

function TypingIndicator() {
  return (
    <div style={{ display: 'flex', alignItems: 'center', marginBottom: 10 }}>
      <div style={{
        width: 28, height: 28, borderRadius: '50%',
        background: 'linear-gradient(135deg, #6366f1, #8b5cf6)',
        display: 'flex', alignItems: 'center', justifyContent: 'center',
        fontSize: 13, color: '#fff', marginRight: 8
      }}>🤖</div>
      <div style={{
        background: '#f1f5f9', borderRadius: '4px 16px 16px 16px',
        padding: '10px 16px', display: 'flex', gap: 4, alignItems: 'center'
      }}>
        {[0, 1, 2].map(i => (
          <span key={i} style={{
            width: 7, height: 7, borderRadius: '50%', background: '#94a3b8',
            animation: 'bounce 1.2s infinite',
            animationDelay: `${i * 0.2}s`,
            display: 'inline-block'
          }} />
        ))}
      </div>
    </div>
  );
}

const SUGGESTED_PROMPTS = [
  'How many assets are available?',
  'List employees without any assigned assets',
  'Show assets expiring warranty soon',
  'Any assets in poor or broken condition?',
];

export default function ChatbotPanel() {
  const { token } = useAuthStore();
  const [open, setOpen] = useState(false);
  const [input, setInput] = useState('');
  const [messages, setMessages] = useState([
    {
      role: 'assistant',
      content: "Hi! I'm your **Asset Management AI Assistant**. 👋\n\nAsk me anything about your asset data — inventory counts, employee assignments, warranty status, pending requests, and more.\n\nWhat would you like to know?"
    }
  ]);
  const [loading, setLoading] = useState(false);
  const [cooldown, setCooldown] = useState(0); // seconds remaining before next send
  const bottomRef = useRef(null);
  const inputRef = useRef(null);
  const cooldownRef = useRef(null);

  useEffect(() => {
    if (bottomRef.current) bottomRef.current.scrollIntoView({ behavior: 'smooth' });
  }, [messages, loading, open]);

  useEffect(() => {
    if (open && inputRef.current) inputRef.current.focus();
  }, [open]);

  const sendMessage = async (text) => {
    const userMsg = text || input.trim();
    if (!userMsg || loading) return;
    setInput('');

    const history = messages.map(m => ({ role: m.role, content: m.content }));
    const newMessages = [...messages, { role: 'user', content: userMsg }];
    setMessages(newMessages);
    setLoading(true);

    try {
      const res = await fetch(`${API_BASE}/api/chatbot/chat`, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
          Authorization: `Bearer ${token}`,
        },
        body: JSON.stringify({ message: userMsg, history }),
      });

      if (!res.ok) {
        throw new Error(`Server error: ${res.status}`);
      }

      const data = await res.json();
      setMessages(prev => [...prev, { role: 'assistant', content: data.reply }]);
    } catch (err) {
      setMessages(prev => [...prev, {
        role: 'assistant',
        content: `❌ **Error:** ${err.message || 'Something went wrong. Please try again.'}`
      }]);
    } finally {
      setLoading(false);
      // Start 8s cooldown to protect free-tier rate limits
      setCooldown(8);
      clearInterval(cooldownRef.current);
      cooldownRef.current = setInterval(() => {
        setCooldown(prev => {
          if (prev <= 1) { clearInterval(cooldownRef.current); return 0; }
          return prev - 1;
        });
      }, 1000);
    }
  };

  const handleKeyDown = (e) => {
    if (e.key === 'Enter' && !e.shiftKey) {
      e.preventDefault();
      if (!loading && !cooldown) sendMessage();
    }
  };

  const clearChat = () => {
    setMessages([{
      role: 'assistant',
      content: "Chat cleared. How can I help you with your asset data?"
    }]);
  };

  return (
    <>
      {/* Bounce animation keyframes */}
      <style>{`
        @keyframes bounce {
          0%, 60%, 100% { transform: translateY(0); opacity: 0.5; }
          30% { transform: translateY(-5px); opacity: 1; }
        }
        @keyframes slideUp {
          from { opacity: 0; transform: translateY(20px) scale(0.97); }
          to   { opacity: 1; transform: translateY(0) scale(1); }
        }
        @keyframes pulse-ring {
          0% { box-shadow: 0 0 0 0 rgba(99, 102, 241, 0.4); }
          70% { box-shadow: 0 0 0 10px rgba(99, 102, 241, 0); }
          100% { box-shadow: 0 0 0 0 rgba(99, 102, 241, 0); }
        }
        .chatbot-table {
          width: 100%;
          border-collapse: collapse;
          font-size: 12px;
          margin: 4px 0;
          background: #fff;
          border-radius: 8px;
          overflow: hidden;
          box-shadow: 0 1px 2px rgba(0,0,0,0.05);
        }
        .chatbot-table th {
          background: #f8fafc;
          padding: 8px;
          text-align: left;
          font-weight: 600;
          border-bottom: 2px solid #e2e8f0;
          color: #475569;
        }
        .chatbot-table td {
          padding: 8px;
          border-bottom: 1px solid #f1f5f9;
          color: #334155;
        }
        .chatbot-table tr:last-child td {
          border-bottom: none;
        }
        .chatbot-table tr:nth-child(even) {
          background: #fcfdfe;
        }
        .chatbot-code-block {
          background: rgba(0,0,0,0.08);
          border-radius: 6px;
          padding: 8px 10px;
          font-size: 12px;
          overflow-x: auto;
          white-space: pre-wrap;
          margin: 4px 0;
          border: 1px solid rgba(0,0,0,0.05);
        }
      `}</style>

      {/* Floating bubble button */}
      <button
        onClick={() => setOpen(o => !o)}
        title="Asset AI Assistant"
        style={{
          position: 'fixed', bottom: 28, right: 28, zIndex: 1000,
          width: 56, height: 56, borderRadius: '50%', border: 'none',
          background: open
            ? 'linear-gradient(135deg, #4f46e5, #7c3aed)'
            : 'linear-gradient(135deg, #6366f1, #8b5cf6)',
          cursor: 'pointer',
          display: 'flex', alignItems: 'center', justifyContent: 'center',
          fontSize: 22,
          boxShadow: '0 4px 20px rgba(99,102,241,0.45)',
          transition: 'all 0.2s ease',
          animation: !open ? 'pulse-ring 2s infinite' : 'none',
        }}
      >
        {open ? '✕' : '💬'}
      </button>

      {/* Chat panel */}
      {open && (
        <div style={{
          position: 'fixed', bottom: 96, right: 28, zIndex: 999,
          width: 380, height: 560,
          background: '#ffffff',
          borderRadius: 18,
          boxShadow: '0 20px 60px rgba(0,0,0,0.18), 0 0 0 1px rgba(99,102,241,0.1)',
          display: 'flex', flexDirection: 'column',
          overflow: 'hidden',
          animation: 'slideUp 0.25s ease',
        }}>
          {/* Header */}
          <div style={{
            background: 'linear-gradient(135deg, #6366f1 0%, #8b5cf6 100%)',
            padding: '14px 18px',
            display: 'flex', alignItems: 'center', justifyContent: 'space-between',
            flexShrink: 0,
          }}>
            <div style={{ display: 'flex', alignItems: 'center', gap: 10 }}>
              <div style={{
                width: 36, height: 36, borderRadius: '50%',
                background: 'rgba(255,255,255,0.2)',
                display: 'flex', alignItems: 'center', justifyContent: 'center', fontSize: 18
              }}>🤖</div>
              <div>
                <div style={{ color: '#fff', fontWeight: 700, fontSize: 14 }}>Asset AI Assistant</div>
                <div style={{ color: 'rgba(255,255,255,0.75)', fontSize: 11 }}>
                  <span style={{
                    display: 'inline-block', width: 7, height: 7, borderRadius: '50%',
                    background: '#4ade80', marginRight: 4, verticalAlign: 'middle'
                  }} />
                  Powered by Groq
                </div>
              </div>
            </div>
            <button
              onClick={clearChat}
              title="Clear chat"
              style={{
                background: 'rgba(255,255,255,0.15)', border: 'none', borderRadius: 8,
                color: '#fff', padding: '4px 10px', fontSize: 11, cursor: 'pointer',
              }}
            >
              Clear
            </button>
          </div>

          {/* Messages */}
          <div style={{
            flex: 1, overflowY: 'auto', padding: '14px 14px 4px',
            scrollbarWidth: 'thin', scrollbarColor: '#e2e8f0 transparent',
          }}>
            {messages.map((msg, idx) => (
              <MessageBubble key={idx} role={msg.role} content={msg.content} />
            ))}
            {loading && <TypingIndicator />}
            <div ref={bottomRef} />
          </div>

          {/* Suggested prompts — only show when only the welcome message is present */}
          {messages.length === 1 && !loading && (
            <div style={{ padding: '0 12px 8px', display: 'flex', flexWrap: 'wrap', gap: 6 }}>
              {SUGGESTED_PROMPTS.map((prompt, i) => (
                <button
                  key={i}
                  onClick={() => sendMessage(prompt)}
                  style={{
                    background: '#f1f5f9', border: '1px solid #e2e8f0',
                    borderRadius: 20, padding: '5px 11px', fontSize: 11,
                    color: '#475569', cursor: 'pointer', whiteSpace: 'nowrap',
                    transition: 'all 0.15s',
                  }}
                  onMouseEnter={e => { e.target.style.background = '#e0e7ff'; e.target.style.borderColor = '#818cf8'; }}
                  onMouseLeave={e => { e.target.style.background = '#f1f5f9'; e.target.style.borderColor = '#e2e8f0'; }}
                >
                  {prompt}
                </button>
              ))}
            </div>
          )}

          {/* Input area */}
          <div style={{
            borderTop: '1px solid #f1f5f9', padding: '10px 12px',
            display: 'flex', gap: 8, alignItems: 'flex-end', flexShrink: 0,
          }}>
            <textarea
              ref={inputRef}
              value={input}
              onChange={e => setInput(e.target.value)}
              onKeyDown={handleKeyDown}
              placeholder="Ask about your assets..."
              rows={1}
              disabled={loading}
              style={{
                flex: 1, resize: 'none', border: '1.5px solid #e2e8f0',
                borderRadius: 12, padding: '9px 12px', fontSize: 13,
                outline: 'none', fontFamily: 'inherit', lineHeight: '1.4',
                maxHeight: 100, overflowY: 'auto', color: '#1e293b',
                transition: 'border-color 0.15s',
                background: loading ? '#f8fafc' : '#fff',
              }}
              onFocus={e => { e.target.style.borderColor = '#6366f1'; }}
              onBlur={e => { e.target.style.borderColor = '#e2e8f0'; }}
            />
            <button
              onClick={() => sendMessage()}
              disabled={loading || cooldown > 0 || !input.trim()}
              style={{
                width: 38, height: 38, borderRadius: 10, border: 'none',
                background: loading || cooldown > 0 || !input.trim()
                  ? '#e2e8f0'
                  : 'linear-gradient(135deg, #6366f1, #8b5cf6)',
                cursor: loading || cooldown > 0 || !input.trim() ? 'default' : 'pointer',
                display: 'flex', alignItems: 'center', justifyContent: 'center',
                fontSize: cooldown > 0 ? 11 : 16, flexShrink: 0, transition: 'all 0.15s',
                color: loading || cooldown > 0 || !input.trim() ? '#94a3b8' : '#fff',
              }}
            >
              {loading ? '⏳' : cooldown > 0 ? `${cooldown}s` : '➤'}
            </button>
          </div>
        </div>
      )}
    </>
  );
}
