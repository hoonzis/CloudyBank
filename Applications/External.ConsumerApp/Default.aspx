<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="True"
    CodeBehind="Default.aspx.cs" Inherits="ConsumerApp._Default" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="Server">
    <h1>Testing OAuth Consumer</h1>
	<fieldset title="Authorization">
    <table>
        <tr>
            <td>Url of the OAuth handshake enpoint:</td>
            <td><asp:TextBox ID="authHandleTextBox" runat="server" Text="http://octoinnovation.cloudapp.net/OAuth.ashx" Width="500"/></td>
        </tr>
        <tr>
            <td>Url of the service to access:</td>
            <td><asp:TextBox ID="serviceTextBox" runat="server" Text="http://octoinnovation.cloudapp.net/OpenServices/DataService.svc/GetAccounts" Width="500"/></td>
        </tr>
        <tr>
            <td>Consumer application key:</td>
            <td><asp:TextBox ID="consumerKeyTextBox" runat="server" Text="key1" /></td>
        </tr>
        <tr>
            <td>Consumer secret password:</td>
            <td><asp:TextBox ID="consumerSecretTextBox" runat="server" Text="secret1" /></td>
        </tr>
        <tr>
            <td></td>
            <td><asp:Button ID="getAuthorizationButton" runat="server" Text="Get Authorization" OnClick="getAuthorizationButton_Click" /></td>
        </tr>
        
        <tr>
            <td></td>
            <td><asp:Label ID="authorizationLabel" runat="server" /></td>
        </tr>
        <tr>
            <td>
            </td>
            <td><asp:Button ID="Button1" runat="server" Text="Get Data" OnClick="getData_Click" /></td>
        </tr>
        <tr>
            <td>Recieved raw data in JSON:</td>
            <td><asp:Label ID="dataLabel" runat="server" /></td>
        </tr>
        <tr>
            <td>Parsed data: </td>
            <td><asp:ListBox ID="accountListBox" runat="server" Width="300" /></td>
        </tr>
        <tr>
            <td colspan="2"><asp:Label ID="OutputLabel" runat="server"/></td>
        </tr>
        


    </table>
    	
	</fieldset>
</asp:Content>
