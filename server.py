#!/usr/bin/env python3
# local preview server for testing EmployeeApp on localhost without Windows IIS

import http.server
import http.cookies
import socketserver
import urllib.parse
import sqlite3
import os
import uuid

PORT = int(os.environ.get("PORT", 8080))
BASE_DIR = os.path.dirname(os.path.abspath(__file__))
DB_FILE = os.path.join(BASE_DIR, "EmployeeDB.sqlite")

# session storage
SESSIONS = {}

def init_db():
    conn = sqlite3.connect(DB_FILE)
    cur = conn.cursor()
    cur.execute('''
        CREATE TABLE IF NOT EXISTS Employees (
            EmployeeId INTEGER PRIMARY KEY AUTOINCREMENT,
            FullName TEXT NOT NULL,
            EmployeeCode TEXT NOT NULL UNIQUE,
            Email TEXT NOT NULL UNIQUE,
            Password TEXT NOT NULL,
            CreatedDate TIMESTAMP DEFAULT CURRENT_TIMESTAMP
        )
    ''')
    cur.execute("SELECT COUNT(*) FROM Employees")
    if cur.fetchone()[0] == 0:
        cur.execute('''
            INSERT INTO Employees (FullName, EmployeeCode, Email, Password)
            VALUES (?, ?, ?, ?)
        ''', ('Demo Employee', 'EMP001', 'demo@company.com', '123456'))
    conn.commit()
    conn.close()

def get_session(handler):
    cookie_header = handler.headers.get('Cookie')
    if cookie_header:
        cookie = http.cookies.SimpleCookie(cookie_header)
        if 'ASP.NET_SessionId' in cookie:
            sid = cookie['ASP.NET_SessionId'].value
            if sid in SESSIONS:
                return sid, SESSIONS[sid]
    new_sid = str(uuid.uuid4()).replace('-', '')[:24]
    SESSIONS[new_sid] = {}
    return new_sid, SESSIONS[new_sid]

class AppRequestHandler(http.server.BaseHTTPRequestHandler):

    def send_html(self, html, session_id=None, status=200):
        body = html.encode('utf-8')
        self.send_response(status)
        self.send_header('Content-Type', 'text/html; charset=utf-8')
        self.send_header('Content-Length', str(len(body)))
        if session_id:
            self.send_header('Set-Cookie', f'ASP.NET_SessionId={session_id}; Path=/; HttpOnly')
        self.end_headers()
        self.wfile.write(body)

    def redirect(self, location, session_id=None):
        self.send_response(302)
        self.send_header('Location', location)
        if session_id:
            self.send_header('Set-Cookie', f'ASP.NET_SessionId={session_id}; Path=/; HttpOnly')
        self.end_headers()

    def parse_form_data(self):
        content_length = int(self.headers.get('Content-Length', 0))
        post_body = self.rfile.read(content_length).decode('utf-8')
        parsed = urllib.parse.parse_qs(post_body)
        return {k: v[0] for k, v in parsed.items()}

    def do_GET(self):
        path = urllib.parse.urlparse(self.path).path

        if path in ('/', '/Login.aspx', '/login.aspx', '/Login', '/login'):
            self.render_login()
        elif path in ('/Register.aspx', '/register.aspx', '/Register', '/register'):
            self.render_register()
        elif path in ('/Welcome.aspx', '/welcome.aspx', '/Welcome', '/welcome'):
            self.render_welcome()
        elif path in ('/Logout.aspx', '/logout.aspx', '/Logout', '/logout'):
            self.handle_logout()
        elif path.startswith('/Styles/') or path == '/Styles/Site.css':
            self.serve_css()
        else:
            self.send_error(404, "Page Not Found")

    def do_POST(self):
        path = urllib.parse.urlparse(self.path).path

        if path in ('/', '/Login.aspx', '/login.aspx', '/Login', '/login'):
            self.handle_login_post()
        elif path in ('/Register.aspx', '/register.aspx', '/Register', '/register'):
            self.handle_register_post()
        elif path in ('/Welcome.aspx', '/welcome.aspx', '/Welcome', '/welcome'):
            self.handle_welcome_post()
        else:
            self.send_error(404, "Page Not Found")

    def serve_css(self):
        css_path = os.path.join(BASE_DIR, "Styles", "Site.css")
        if os.path.exists(css_path):
            with open(css_path, 'rb') as f:
                content = f.read()
            self.send_response(200)
            self.send_header('Content-Type', 'text/css')
            self.send_header('Content-Length', str(len(content)))
            self.end_headers()
            self.wfile.write(content)
        else:
            self.send_error(404, "CSS File Not Found")

    def render_login(self, message="", employee_code=""):
        sid, _ = get_session(self)
        is_success = "successful" in message.lower()
        msg_style = "color: #15803d; background: #dcfce7; border-color: #bbf7d0;" if is_success else "color: #dc2626; background: #fef2f2; border-color: #fecaca;"
        msg_html = f'<div class="message" style="{msg_style}">{message}</div>' if message else ''

        html = f"""<!DOCTYPE html>
<html>
<head runat="server">
    <title>Employee Sign In</title>
    <link rel="stylesheet" href="/Styles/Site.css" />
</head>
<body>
    <form id="form1" method="post" action="/Login.aspx">
        <div class="card">
            <div class="card-header-icon">🔐</div>
            <h1>Employee Sign In</h1>
            <p class="subtitle">Enter your employee code and password to access your account</p>

            {msg_html}

            <div class="form-group">
                <label for="txtEmployeeCode">Employee Code</label>
                <input type="text" id="txtEmployeeCode" name="txtEmployeeCode" value="{employee_code}" placeholder="e.g. EMP001" autocomplete="off" required />
            </div>

            <div class="form-group">
                <label for="txtPassword">Password</label>
                <input type="password" id="txtPassword" name="txtPassword" placeholder="Enter your password" required />
            </div>

            <button type="submit" id="btnLogin" name="btnLogin" class="btn-primary">Sign In</button>

            <div class="link-row">
                Don't have an account? <a href="/Register.aspx">Register here</a>
            </div>

            <div style="margin-top: 22px; padding-top: 14px; border-top: 1px solid #e2e8f0; font-size: 12.5px; color: #64748b; text-align: center;">
                💡 Demo Account: <strong>EMP001</strong> &nbsp;|&nbsp; Password: <strong>123456</strong>
            </div>
        </div>
    </form>
</body>
</html>"""
        self.send_html(html, session_id=sid)

    def handle_login_post(self):
        sid, session = get_session(self)
        data = self.parse_form_data()
        employee_code = data.get('txtEmployeeCode', '').strip()
        password = data.get('txtPassword', '').strip()

        if not employee_code or not password:
            self.render_login(message="Please enter both employee code and password.", employee_code=employee_code)
            return

        conn = sqlite3.connect(DB_FILE)
        cur = conn.cursor()
        cur.execute('''
            SELECT EmployeeId, FullName, EmployeeCode
            FROM Employees
            WHERE EmployeeCode = ? AND Password = ?
        ''', (employee_code, password))
        row = cur.fetchone()
        conn.close()

        if row:
            session["EmployeeId"] = str(row[0])
            session["FullName"] = str(row[1])
            session["EmployeeCode"] = str(row[2])
            self.redirect("/Welcome.aspx", session_id=sid)
        else:
            self.render_login(message="Invalid employee code or password.", employee_code=employee_code)

    def render_register(self, message="", full_name="", employee_code="", email=""):
        sid, _ = get_session(self)
        msg_html = f'<div class="message">{message}</div>' if message else ''
        html = f"""<!DOCTYPE html>
<html>
<head runat="server">
    <title>Employee Registration</title>
    <link rel="stylesheet" href="/Styles/Site.css" />
</head>
<body>
    <form id="form1" method="post" action="/Register.aspx">
        <div class="card">
            <div class="card-header-icon">📝</div>
            <h1>Employee Registration</h1>
            <p class="subtitle">Fill in the form below to create your employee profile</p>

            {msg_html}

            <div class="form-group">
                <label for="txtFullName">Full Name</label>
                <input type="text" id="txtFullName" name="txtFullName" value="{full_name}" placeholder="e.g. John Doe" required />
            </div>

            <div class="form-group">
                <label for="txtEmployeeCode">Employee Code</label>
                <input type="text" id="txtEmployeeCode" name="txtEmployeeCode" value="{employee_code}" placeholder="e.g. EMP002" autocomplete="off" required />
            </div>

            <div class="form-group">
                <label for="txtEmail">Email Address</label>
                <input type="email" id="txtEmail" name="txtEmail" value="{email}" placeholder="e.g. john@company.com" required />
            </div>

            <div class="form-group">
                <label for="txtPassword">Password</label>
                <input type="password" id="txtPassword" name="txtPassword" placeholder="Create a secure password" required />
            </div>

            <button type="submit" id="btnRegister" name="btnRegister" class="btn-primary">Create Account</button>

            <div class="link-row">
                Already have an account? <a href="/Login.aspx">Sign in</a>
            </div>
        </div>
    </form>
</body>
</html>"""
        self.send_html(html, session_id=sid)

    def handle_register_post(self):
        data = self.parse_form_data()
        full_name = data.get('txtFullName', '').strip()
        employee_code = data.get('txtEmployeeCode', '').strip()
        email = data.get('txtEmail', '').strip()
        password = data.get('txtPassword', '').strip()

        if not full_name or not employee_code or not email or not password:
            self.render_register(message="Please fill in all fields.", full_name=full_name, employee_code=employee_code, email=email)
            return

        conn = sqlite3.connect(DB_FILE)
        cur = conn.cursor()
        cur.execute('''
            SELECT COUNT(*) FROM Employees WHERE EmployeeCode = ? OR Email = ?
        ''', (employee_code, email))
        count = cur.fetchone()[0]

        if count > 0:
            conn.close()
            self.render_register(message="Employee code or email is already registered.", full_name=full_name, employee_code=employee_code, email=email)
            return

        cur.execute('''
            INSERT INTO Employees (FullName, EmployeeCode, Email, Password)
            VALUES (?, ?, ?, ?)
        ''', (full_name, employee_code, email, password))
        conn.commit()
        conn.close()

        self.render_login(message="Registration successful! Please sign in with your credentials.", employee_code=employee_code)

    def render_welcome(self):
        sid, session = get_session(self)
        if "EmployeeId" not in session:
            self.redirect("/Login.aspx")
            return

        full_name = session.get("FullName", "Employee")
        employee_code = session.get("EmployeeCode", "-")

        html = f"""<!DOCTYPE html>
<html>
<head runat="server">
    <title>Employee Dashboard</title>
    <link rel="stylesheet" href="/Styles/Site.css" />
</head>
<body>
    <form id="form1" method="post" action="/Welcome.aspx">
        <div class="card welcome-card">
            <div class="card-header-icon">👋</div>
            <span class="welcome-badge">● Signed In</span>

            <h1>Welcome, {full_name}</h1>
            <p class="subtitle">Employee portal session is active</p>

            <div class="welcome-info-box">
                <div class="welcome-info-row">
                    <span class="welcome-info-label">Employee Code</span>
                    <span class="welcome-info-value">{employee_code}</span>
                </div>
                <div class="welcome-info-row">
                    <span class="welcome-info-label">Account Status</span>
                    <span class="welcome-info-value" style="color: #15803d;">Verified</span>
                </div>
            </div>

            <button type="submit" id="btnLogout" name="btnLogout" class="btn-secondary">Sign Out</button>
        </div>
    </form>
</body>
</html>"""
        self.send_html(html, session_id=sid)

    def handle_welcome_post(self):
        self.handle_logout()

    def handle_logout(self):
        sid, session = get_session(self)
        session.clear()
        self.redirect("/Login.aspx", session_id=sid)

    def log_message(self, format, *args):
        pass

if __name__ == '__main__':
    init_db()
    socketserver.TCPServer.allow_reuse_address = True
    with socketserver.TCPServer(("", PORT), AppRequestHandler) as httpd:
        print(f"EmployeeApp server running at http://0.0.0.0:{PORT}", flush=True)
        try:
            httpd.serve_forever()
        except KeyboardInterrupt:
            pass
