<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Welcome.aspx.cs" Inherits="EmployeeApp.Welcome" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <title>Employee Dashboard</title>
    <link rel="stylesheet" href="Styles/Site.css" />
</head>
<body>
    <form id="form1" runat="server">
        <div class="card welcome-card">
            <div class="card-header-icon">👋</div>
            <span class="welcome-badge">● Signed In</span>

            <h1>Welcome, <asp:Literal ID="litFullName" runat="server" /></h1>
            <p class="subtitle">Employee portal session is active</p>

            <div class="welcome-info-box">
                <div class="welcome-info-row">
                    <span class="welcome-info-label">Employee Code</span>
                    <span class="welcome-info-value"><asp:Literal ID="litEmployeeCode" runat="server" /></span>
                </div>
                <div class="welcome-info-row">
                    <span class="welcome-info-label">Account Status</span>
                    <span class="welcome-info-value" style="color: #15803d;">Verified</span>
                </div>
            </div>

            <asp:Button ID="btnLogout" runat="server" Text="Sign Out" CssClass="btn-secondary" OnClick="btnLogout_Click" />
        </div>
    </form>
</body>
</html>
