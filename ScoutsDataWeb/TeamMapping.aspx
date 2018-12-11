<%@ Page Language="C#" AutoEventWireup="false" CodeBehind="TeamMapping.aspx.cs" Inherits="JC_SoccerWeb.TeamMapping" %>
<%@ Register TagPrefix="anthem" Namespace="Anthem" Assembly="Anthem" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Team Mapping</title>
      <meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR">
    <meta content="C#" name="CODE_LANGUAGE">
    <meta content="JavaScript" name="vs_defaultClientScript">
    <meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
    <LINK href="CentaSmsStyle.css" type="text/css" rel="stylesheet">
</head>
<body>
    <form id="form1" runat="server">
        <TABLE align="center">
				 <TR valign="bottom">
					<TD align="left" valign="bottom" style="height:30px"> <asp:Label ID="lbEvent"  Font-Bold="true"  runat="server" > </asp:Label> </TD>
				</TR>  
            <tr valign="top"><td valign="top"> <hr/></td></tr>
           <tr style="height:5px"><td >  </td></tr> 
				<TR align="center">
					<TD align="center">
                         <anthem:DataGrid id="dgTeams" runat="server" Width="550px"   AllowPaging="false" AutoGenerateColumns="FALSE">
                            <PagerStyle Mode="NumericPages"></PagerStyle>
                           <HeaderStyle Font-Bold="True"    HorizontalAlign ="Center"  Wrap="false" Height="34px" CssClass="grid-header" ></HeaderStyle>
                           <ItemStyle Height="34px"     CssClass="grid-item"></ItemStyle><Columns>
                 <asp:TemplateColumn HeaderText="ID"   >
                                                        <HeaderStyle HorizontalAlign="Left"  Wrap="true" Width="50px" CssClass="grid-header" VerticalAlign="Middle"></HeaderStyle><ItemStyle CssClass="grid-item"  ></ItemStyle><ItemTemplate>
                                                            <anthem:Label id="lbID" runat="server" Width="50px"  Text='<%# DataBinder.Eval(Container, "DataItem.ID") %>'></anthem:Label>
                                                        </ItemTemplate>
                                                    </asp:TemplateColumn> 
                 <asp:TemplateColumn HeaderText="NAME"   >
                                                        <HeaderStyle HorizontalAlign="Left"  Wrap="true" Width="150px" CssClass="grid-header" VerticalAlign="Middle"></HeaderStyle>
                                                <ItemStyle  Wrap="true" CssClass="grid-item" Width="150px"></ItemStyle><ItemTemplate>
                                                       <%# DataBinder.Eval(Container, "DataItem.TNAME") %> 
                                                        </ItemTemplate>
                                                    </asp:TemplateColumn>
                                <asp:TemplateColumn HeaderText="Short Name"   >
                                                        <HeaderStyle HorizontalAlign="Left"  Wrap="true" Width="120px" CssClass="grid-header" VerticalAlign="Middle"></HeaderStyle>
                                    <ItemStyle CssClass="grid-item"  ></ItemStyle><ItemTemplate>
                                                            <anthem:Label id="lbShort_Name" runat="server" Width="120px"  Text='<%# DataBinder.Eval(Container, "DataItem.SHORT_NAME") %>'></anthem:Label>
                                                        </ItemTemplate>
                                                    </asp:TemplateColumn>
                                <asp:TemplateColumn HeaderText="HKJC_NAME"   >
                                                        <HeaderStyle HorizontalAlign="Left"  Wrap="true" Width="80px" CssClass="grid-header" VerticalAlign="Middle"></HeaderStyle><ItemStyle CssClass="grid-item" ></ItemStyle><ItemTemplate>
                                                            <anthem:Label id="lbHKJC_NAME" runat="server" Width="80px"  Text='<%# DataBinder.Eval(Container, "DataItem.HKJC_NAME") %>'></anthem:Label>
                                                        </ItemTemplate>
                                                    </asp:TemplateColumn>
                                <asp:TemplateColumn HeaderText="HKJC_NAME_CN"   >
                                                        <HeaderStyle HorizontalAlign="Left"  Wrap="true" Width="50px" CssClass="grid-header" VerticalAlign="Middle"></HeaderStyle><ItemStyle CssClass="grid-item" ></ItemStyle><ItemTemplate>
                                                            <anthem:Label id="lbHKJC_NAME_CN" runat="server" Width="50px"  Text='<%# DataBinder.Eval(Container, "DataItem.HKJC_NAME_CN") %>'></anthem:Label>
                                                        </ItemTemplate>
                                                    </asp:TemplateColumn>
                                <asp:TemplateColumn HeaderText="HKJC_ID"   >
                                                        <HeaderStyle HorizontalAlign="Left"  Wrap="true" Width="50px" CssClass="grid-header" VerticalAlign="Middle"></HeaderStyle><ItemStyle CssClass="grid-item"  ></ItemStyle><ItemTemplate>
                                                            <anthem:Label id="lbIDHKJC_ID" runat="server" Width="50px"  Text='<%# DataBinder.Eval(Container, "DataItem.HKJC_ID") %>'></anthem:Label>
                                                        </ItemTemplate>
                                                    </asp:TemplateColumn>
                         </Columns> </anthem:DataGrid>   
					</TD>
				</TR>
                <tr style="height:10px"><td >  </td></tr>
                  <tr><td> <hr/></td></tr> 
                <tr style="height:20px" align="right"><td>   <strong>  <anthem:Label ID="lbMsg" runat="server" ForeColor="Red" Width="300px"    >  </anthem:Label>  </strong></td></tr>
				 <tr><td align="right"> <anthem:Button  ID="btnSave" runat="server" Text="Save"  />  
				     </td></tr>
			</TABLE> 
    </form>
</body>
</html>
