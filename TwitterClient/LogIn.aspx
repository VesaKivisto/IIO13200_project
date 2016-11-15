<%@ Page Language="C#" AutoEventWireup="true" CodeFile="LogIn.aspx.cs" Inherits="LogIn" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            Username: <asp:TextBox ID="txtUsername" runat="server" />
            Password: <asp:TextBox ID="txtPassword" runat="server" />
            <asp:Label ID="lblMessages" runat="server" />
        </div>
    </form>
</body>
</html>
