<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="CloudyBank.Web.LoginPage" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>CloudyBank Login Page</title>
</head>
<body>
    <h1>OctoBank - Login Page</h1>
    <form id="form1" runat="server">
    <!-- This part can be used to enable OpenID login to OctoBank -->
    <%--<div>
        <asp:Label ID="Label1" runat="server" Text="OpenID Login" />
        <asp:TextBox ID="openIdBox" runat="server" />
        <asp:Button ID="loginButton" runat="server" Text="Login" OnClick="loginButton_Click" />
        <asp:CustomValidator runat="server" ID="openidValidator" ErrorMessage="Invalid OpenID Identifier"
            ControlToValidate="openIdBox" EnableViewState="false" OnServerValidate="openidValidator_ServerValidate" />
        <br />
        <asp:Label ID="loginFailedLabel" runat="server" EnableViewState="False" Text="Login failed" Visible="False" />
        <asp:Label ID="loginCanceledLabel" runat="server" EnableViewState="False" Text="Login canceled" Visible="False" />
    </div>--%>
    <hr />
    <div>
        <table>
            <tr>
                <td>Client number: </td>
                <td><asp:TextBox ID="txbLogin" runat="server" TextMode="Password"/></td>
            </tr>
            <tr>
                <td>Password: </td>
                <td><asp:TextBox ID="txbPass" runat="server" TextMode="Password"/></td>
            </tr>
            <tr>
                <td></td>
                <td><asp:Button ID="btnLogin"  runat="server" OnClick="Logon_Click" Text="Login"/></td>
            </tr>
        </table>
       
        <p>
        <asp:Label ID="Msg" ForeColor="red" runat="server" />
        </p>
    </div>
</form>
</body>
</html>
