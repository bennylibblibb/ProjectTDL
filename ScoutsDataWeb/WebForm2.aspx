<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="WebForm2.aspx.cs" Inherits="JC_SoccerWeb.WebForm2" %>
<%@ Register TagPrefix="anthem" Namespace="Anthem" Assembly="Anthem" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <anthem:DataGrid id="dgTeams" runat="server" Width="550px"   AllowPaging="false" AutoGenerateColumns="true"></anthem:DataGrid>
    
    </form>
</body>
</html>
