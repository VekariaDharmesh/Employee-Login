<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Register.aspx.cs" Inherits="EmployeeApp.Register" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <title>Employee Registration</title>
    <link rel="stylesheet" href="Styles/Site.css" />
</head>
<body>
    <form id="form1" runat="server">
        <div class="card">
            <h1>Employee Registration</h1>
            <p class="subtitle">Fill in the form below to create your employee profile</p>

            <asp:Label ID="lblMessage" runat="server" CssClass="message" Text="" />

            <div class="form-group">
                <label for="txtFullName">Full Name</label>
                <asp:TextBox ID="txtFullName" runat="server" placeholder="Enter full name" />
            </div>

            <div class="form-group">
                <label for="txtEmployeeCode">Employee Code</label>
                <asp:TextBox ID="txtEmployeeCode" runat="server" placeholder="Enter employee code" autocomplete="off" />
            </div>

            <div class="form-group">
                <label for="txtEmail">Email Address</label>
                <asp:TextBox ID="txtEmail" runat="server" TextMode="Email" placeholder="Enter email address" />
            </div>

            <div class="form-group">
                <label for="txtPassword">Password</label>
                <asp:TextBox ID="txtPassword" runat="server" TextMode="Password" placeholder="Create a password" />
            </div>

            <asp:Button ID="btnRegister" runat="server" Text="Create Account" CssClass="btn-primary" OnClick="btnRegister_Click" />

            <div class="link-row">
                Already have an account? <a href="Login.aspx">Sign in</a>
            </div>
        </div>
    </form>
</body>
</html>
