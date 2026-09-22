# Employee Management System (ASP.NET Web Forms + SQL Server)

A clean employee authentication and dashboard application featuring **Registration**, **Sign In**, and a protected **Welcome Dashboard** backed by SQL Server. Built with C# (.NET Framework), ADO.NET, HTML, and CSS.

## Project Structure
- `Database.sql` — Database creation script and `Employees` table setup
- `Web.config` — Database connection configuration
- `Login.aspx` / `Login.aspx.cs` — Employee sign-in page with authentication logic
- `Register.aspx` / `Register.aspx.cs` — Registration page for new employee accounts
- `Welcome.aspx` / `Welcome.aspx.cs` — Protected dashboard displaying active session data
- `Styles/Site.css` — Centralized modern stylesheet
- `server.py` — Local preview server for quick testing without Windows IIS

## Setup in Visual Studio (Windows)

1. **Database Setup**
   - Open **SQL Server Management Studio (SSMS)**.
   - Run `Database.sql` to generate the `EmployeeDB` database, the `Employees` table, and default test credentials (`EMP001` / `123456`).

2. **Open / Create the Solution**
   - In Visual Studio, create a new **ASP.NET Web Application (.NET Framework)** using the **Empty** template with **Web Forms** checked.
   - Ensure the project namespace matches `EmployeeApp`.

3. **Include Project Files**
   - Add `Login.aspx`, `Register.aspx`, `Welcome.aspx`, their code-behind files, and the `Styles` directory to the project.
   - Apply the connection string in `Web.config` pointing to your local SQL instance (`.\SQLEXPRESS` or `localhost`).

4. **Run Application**
   - Right-click `Login.aspx` and select **Set As Start Page**.
   - Press **F5** to start the application in IIS Express.

## Testing on Localhost (Cross-Platform)

To preview and test the complete authentication flow on macOS or Linux:

```bash
python3 server.py
```

Open your browser at **`http://localhost:8080`**.

## Key Implementation Highlights
- **Parameterized Queries**: Uses `SqlParameter` objects across all ADO.NET calls to prevent SQL injection vulnerabilities.
- **Session Protection**: Validates active session keys on `Welcome.aspx` and automatically redirects unauthorized visitors to `Login.aspx`.
- **Session Cleanup**: Performs `Session.Clear()` and `Session.Abandon()` upon sign-out.
# Employee-Login
