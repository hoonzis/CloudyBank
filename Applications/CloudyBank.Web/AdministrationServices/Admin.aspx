<%@ Page Language="C#" AutoEventWireup="True" CodeBehind="Admin.aspx.cs" Inherits="CloudyBank.Web.AdministrationServices.Admin" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Interface for administration and testing</title>
</head>
<body>
    <form id="form1" runat="server">
        <div style="border:1px solid">
            <table>
                <tr>
                    <td><b>Insert new payments into system</b></td>
                </tr>
                <tr>
                    <td><asp:Label Text="Account Number: " runat="server"/></td>
                    <td><asp:TextBox ID="AccountNumberTextBox" runat="server"/></td>
                </tr>
                <tr>
                    <td><asp:Label Text="Transaction Data:" runat="server"/></td>
                    <td><asp:TextBox ID="OperationTextBox" Height="100px" Width="415px" Wrap="true" 
                    runat="server" TextMode="MultiLine"/></td>
                </tr>
                <tr>
                    <td></td>
                    <td><asp:Button Text="Insert Payments" ID="InsertPaymentsButton" OnClick="InsertPaymentsButton_Click" runat="server"/></td>
                </tr>
            </table>
        </div>

        <div style="border:1px solid">
            <table>
                <tr>
                    <td><b>Payments categorization</b></td>
                </tr>
                <tr>
                    <td>Customer ID:</td>
                    <td><asp:TextBox ID="CustomerIDTextBox" runat="server"/></td>
                </tr>
                <tr>
                    <td></td>
                    <td><asp:Button ID="CategorizeButton" Text="Categorize" OnClick="Categorize_Click" runat="server"/></td>
                </tr>
            </table>
        </div>

        <div style="border:1px solid">
            <table>
                <tr>
                    <td><b>Account evolution update</b></td>
                </tr>
                <tr>
                    <td></td>
                    <td><asp:Button ID="ComputeBalancePointsButton" runat="server" OnClick="ComputeBalancePoints_Click" Text="All Accounts Evolution update"/></td>
                </tr>
            </table>
        </div>

        <table>
            <tr>
                <td><asp:Label ID="OutputLabel" runat="server"/></td>
                <td></td>
            </tr>
        </table>
    </form>
</body>
</html>
