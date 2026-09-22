<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="EmployeeApp.Login" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <title>Employee Sign In</title>
    <link rel="stylesheet" href="Styles/Site.css" />
</head>
<body>
    <form id="form1" runat="server">
        <div class="card">
            <h1>Employee Sign In</h1>
            <p class="subtitle">Enter your employee code and password to access your account</p>

            <asp:Label ID="lblMessage" runat="server" CssClass="message" Text="" />

            <div class="form-group">
                <label for="txtEmployeeCode">Employee Code</label>
                <asp:TextBox ID="txtEmployeeCode" runat="server" placeholder="Enter employee code" autocomplete="off" />
            </div>

            <div class="form-group">
                <label for="txtPassword">Password</label>
                <asp:TextBox ID="txtPassword" runat="server" TextMode="Password" placeholder="Enter your password" />
            </div>

            <asp:Button ID="btnLogin" runat="server" Text="Sign In" CssClass="btn-primary" OnClick="btnLogin_Click" />

            <div class="link-row">
                Don't have an account? <a href="Register.aspx">Register here</a>
            </div>
        </div>
    </form>
</body>
</html>
