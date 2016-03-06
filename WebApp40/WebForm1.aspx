<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="WebForm1.aspx.vb" Inherits="WebApp40.WebForm1" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <meta name="description" content="The description of my page" />
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <asp:Label ID="Label1" runat="server"></asp:Label>
        </div>
        <div>
            <asp:Label ID="lblViewstat" runat="server"></asp:Label><br />
            <asp:Button ID="btn1" runat="server" Text="増加" />
            <asp:Button ID="btn2" runat="server" Text="退避" />
            <asp:Button ID="btn3" runat="server" Text="復元" />
        </div>
    </form>
</body>
</html>
