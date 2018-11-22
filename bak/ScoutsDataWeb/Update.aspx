<%@ Register TagPrefix="anthem" Namespace="Anthem" Assembly="Anthem" %>
<%@ Register TagPrefix="uc1" TagName="MenuTabs" Src="UserControl/MenuTabs.ascx" %>
<%@ Page language="c#" Codebehind="Update.aspx.cs" AutoEventWireup="false" Inherits="JC_SoccerWeb.Update" ValidateRequest="false" %>
<%@ Register TagPrefix="uc1" TagName="SendTabs" Src="UserControl/SendTabs.ascx" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
<HEAD>
    <title>Telecom Digital MMS Services</title>
    <meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR">
    <meta content="C#" name="CODE_LANGUAGE">
    <meta content="JavaScript" name="vs_defaultClientScript">
    <meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
    <LINK href="CentaSmsStyle.css" type="text/css" rel="stylesheet">
<script type="text/javascript">
    function CheckNum(obj) {
        var strvalue = obj.value;
        if (isNaN(strvalue)) {
            alert("Number");
        } else { 
        } 
    }
</script>
    <style type="text/css">
    .auto-style1 { font-size: medium; }
    .auto-style2 {
        width: 4%;
    }
    .auto-style3 {
        width: 934px;
         height: 30px;
    }
    </style>
        </HEAD>
        <body>
        <form id="fm1" method="post" runat="server">
    <table cellSpacing="0" cellPadding="0" width="100%" border="0">
        <tr>
            <td class="top_bar01_bg">
           <!--     <img src="resource/mango_logo_title.gif" width="359" height="49"> -->
            </td>
        </tr>
        <tr>
            <td class="top_bar02_bg" vAlign="top" height="15">
                <P align="left">
                    <FONT color="#ffffff">
                        <IMG height="15" src="resource/spacer.gif" width="15">
                    </FONT>
                    用戶:&nbsp;<asp:label id="lbUser" runat="server"></asp:label>
                </P>
            </td>
        </tr>
    </table>
    <table cellSpacing="0" cellPadding="0" width="100%" border="0">
        <tr >
            <td vAlign="top">
                <table class="admin-tan-border" id="table1" cellSpacing="0" cellPadding="0"
                       width="100%" border="0">
                    <tr>
                        <td class="admin-table" style="HEIGHT: 10px" vAlign="top" height="2">
                            <IMG height="15" src="resource/spacer.gif" width="15">
                        </td>
                        <td   vAlign="top"> 
                                <uc1:menutabs id="MenuTabs1" runat="server"></uc1:menutabs>
                        </td>
                    </tr>
                    <TR vAlign="top">
                        <TD vAlign="top" class="left_bar_bg" style="width:2px;height:100%">
                            <uc1:sendtabs id="SendTabs1" runat="server"></uc1:sendtabs> 
                        </TD>
                        <TD vAlign="top" class="tan-border02" height="1">
                            <Anthem:panel id="plRankDetails" runat="server">
                                <TABLE style="BORDER-COLLAPSE: collapse;  WIDTH:1200px; HEIGHT:600px"
                                       cellSpacing="10" cellPadding="0" rules="none" align="left" border="0">
                                    <TR>
                                        <TD vAlign="top" align="left" colspan="5" height="10"></TD>
                                    </TR> 
                                    <TR  >
                                        <TD  valign="top" align="center"  height="1"  style="height: 27px" >
                                        <span > <strong> Histroy Rank:</strong></span> 
                                        <anthem:DropDownList ID="dplLeague" runat="server" AutoCallBack="True" Width="150px">
                                            <asp:ListItem Selected="True" Value="All">All</asp:ListItem>
                                        </anthem:DropDownList>  
                                          </TD>  
                                        <TD valign="top" align="left" height="1"   >
                                            <anthem:Button ID="btnEdit" runat="server" Text="Edit"     Width="60px" /> 
                                        </TD>
                                        <TD valign="top" align="center" height="1"> <anthem:CheckBoxList ID="cblIP"  Enabled="false"  runat="server"  RepeatDirection="Horizontal"  Width="300px"  ></anthem:CheckBoxList>
                                        </TD>
                                        <TD   align="left" height="1">
                                          <strong>  <anthem:Label ID="lbMsg" runat="server" ForeColor="Red" Width="300px">
                                            </anthem:Label> </strong>
                                        </TD>
                                        <TD   height="1" style="width:200px" > </TD>
                                       <TR>
                                        <TD  align="left" height="1" colspan="5" >   <hr/>    </TD> 
                               </TR>
                                    <TR>
                                        <TD vAlign="top" align="left" colSpan="5" height="256">
                                            <anthem:DataGrid id="dgRankDetails" runat="server" Width="100%"   AllowPaging="True" AutoGenerateColumns="False"  >
                                                <PagerStyle Mode="NumericPages"></PagerStyle><HeaderStyle Font-Bold="True"></HeaderStyle><EditItemStyle ></EditItemStyle><ItemStyle Height="34px" CssClass="grid-item"></ItemStyle><Columns>
                                                    <asp:TemplateColumn HeaderText="ID" Visible ="false" >
                                                        <HeaderStyle HorizontalAlign="Left" Width="30px" CssClass="grid-header" VerticalAlign="Middle"></HeaderStyle><ItemStyle Width="30px" CssClass="grid-item"></ItemStyle>
                                                        <ItemTemplate>
                                                            <%#this.dgRankDetails.CurrentPageIndex * this.dgRankDetails.PageSize + Container.ItemIndex + 1%>
                                                        </ItemTemplate>
                                                    </asp:TemplateColumn><asp:TemplateColumn HeaderText="IHEADER_ID" Visible ="false" >
                                                        <HeaderStyle HorizontalAlign="Left" Width="80px" CssClass="grid-header" VerticalAlign="Middle"></HeaderStyle><ItemStyle CssClass="grid-item" Width="80px"></ItemStyle><ItemTemplate>
                                                            <anthem:Label id="lbIHEADER_ID" runat="server" Width="80px" Text='<%# DataBinder.Eval(Container, "DataItem.IHEADER_ID") %>'></anthem:Label>
                                                        </ItemTemplate>
                                                    </asp:TemplateColumn><asp:TemplateColumn HeaderText="聯賽">
                                                        <HeaderStyle Wrap="false" HorizontalAlign="Left" Width="120px" CssClass="grid-header" VerticalAlign="Middle"></HeaderStyle><ItemStyle CssClass="grid-item"></ItemStyle><ItemTemplate>
                                                            <anthem:Label id="lbCLEAGUEALIAS" runat="server" Width="120px" Text='<%# DataBinder.Eval(Container, "DataItem.CLEAGUEALIAS") %>'></anthem:Label>
                                                        </ItemTemplate>
                                                    </asp:TemplateColumn><asp:TemplateColumn HeaderText="队伍">
                                                        <HeaderStyle Wrap="false" HorizontalAlign="Left" Width="120px" CssClass="grid-header" VerticalAlign="Middle"></HeaderStyle><ItemStyle CssClass="grid-item"></ItemStyle><ItemTemplate>
                                                            <anthem:Label id="lbCTEAM" runat="server" Width="120px" Text='<%# DataBinder.Eval(Container, "DataItem.CTEAM") %>'></anthem:Label>
                                                        </ItemTemplate>
                                                    </asp:TemplateColumn><asp:TemplateColumn HeaderText="排名">
                                                        <HeaderStyle Wrap="false" HorizontalAlign="Left" Width="120px" CssClass="grid-header" VerticalAlign="Middle"></HeaderStyle><ItemStyle Width="100px" CssClass="grid-item"></ItemStyle><ItemTemplate>
                                                            <anthem:Label id="lbIRANK" runat="server" Width="120px" Text='<%# DataBinder.Eval(Container, "DataItem.IRANK") %>'></anthem:Label>
                                                            <anthem:TextBox id="txtlbIRANK2" runat="server" Width="120px" Visible ="false" Text='<%# DataBinder.Eval(Container, "DataItem.IRANK") %>'></anthem:TextBox>
                                                            <%--<Anthem:RangeValidator ID="RangeValidator1" runat="server" ControlToValidate="txtlbIRANK2" ErrorMessage="RangeValidator" Type="Integer" Visible ="false" ></Anthem:RangeValidator>--%>
                                                        </ItemTemplate><EditItemTemplate>
                                                             <anthem:TextBox id="txtlbIRANK" Visible ="false"  runat="server" Width="120px" Text='<%# DataBinder.Eval(Container, "DataItem.IRANK") %>'>
                                                            </anthem:TextBox>
                                                            <anthem:Label id="lbIRANK2" runat="server" Width="120px" Text='<%# DataBinder.Eval(Container, "DataItem.IRANK") %>'></anthem:Label>
                                                        </EditItemTemplate>
                                                    </asp:TemplateColumn><asp:TemplateColumn HeaderText="曆史排名">
                                                        <HeaderStyle Wrap="false" HorizontalAlign="Left" Width="100px" CssClass="grid-header" VerticalAlign="Middle"></HeaderStyle><ItemStyle Width="100px" CssClass="grid-item"></ItemStyle><ItemTemplate>
                                                            <anthem:Label id="lbIHOSTORYRANK" runat="server" Width="100px" Text='<%# DataBinder.Eval(Container, "DataItem.IHOSTORYRANK") %>'></anthem:Label>
                                                            <anthem:TextBox id="txtIHOSTORYRANK2" runat="server" Width="100px"  Visible ="false" Text='<%# DataBinder.Eval(Container, "DataItem.IHOSTORYRANK") %>'></anthem:TextBox>                                                             
                                                              </ItemTemplate><EditItemTemplate>
                                                            <anthem:TextBox id="txtIHOSTORYRANK" runat="server" Width="100px" Text='<%# DataBinder.Eval(Container, "DataItem.IHOSTORYRANK") %>'>
                                                            </anthem:TextBox>
                                                            <anthem:Label id="lbIHOSTORYRANK3" Visible ="false"  runat="server" Width="100px" Text='<%# DataBinder.Eval(Container, "DataItem.IHOSTORYRANK") %>'></anthem:Label>
                                                        </EditItemTemplate>
                                                    </asp:TemplateColumn>

                                                    <asp:TemplateColumn HeaderText="曆史積分">
                                                        <HeaderStyle Wrap="false" HorizontalAlign="Left" Width="100px" CssClass="grid-header" VerticalAlign="Middle"></HeaderStyle><ItemStyle Width="100px" CssClass="grid-item"></ItemStyle><ItemTemplate>
                                                            <anthem:Label id="lbISCORE" runat="server" Width="100px" Text='<%# DataBinder.Eval(Container, "DataItem.IHOSTORYSCORE") %>'></anthem:Label>
                                                            <anthem:TextBox id="txtISCORE2" runat="server" Width="100px"  Visible ="false" Text='<%# DataBinder.Eval(Container, "DataItem.IHOSTORYSCORE") %>'></anthem:TextBox>                                                             
                                                        </ItemTemplate><EditItemTemplate>
                                                            <anthem:TextBox id="txtISCORE" runat="server" Width="100px" Text='<%# DataBinder.Eval(Container, "DataItem.IHOSTORYSCORE") %>'>
                                                            </anthem:TextBox>
                                                            <anthem:Label id="lbISCORE3" Visible ="false"  runat="server" Width="100px" Text='<%# DataBinder.Eval(Container, "DataItem.IHOSTORYSCORE") %>'></anthem:Label>
                                                        </EditItemTemplate>
                                                    </asp:TemplateColumn>
                                                    
                                                    <asp:TemplateColumn HeaderText="更新日期">
                                                        <HeaderStyle HorizontalAlign="Left" Width="200px" CssClass="grid-header" VerticalAlign="Middle"></HeaderStyle><ItemStyle CssClass="grid-item" Width="120px"></ItemStyle><ItemTemplate>
                                                            <anthem:Label id="lbCUPDATEDATE" runat="server" Width="200px" Text='<%# DataBinder.Eval(Container, "DataItem.CTIMESTAMP ") %>'></anthem:Label>
                                                        </ItemTemplate>
                                                    </asp:TemplateColumn><ASP:EDITCOMMANDCOLUMN EditText="編輯" CancelText="取消" UpdateText="更新" ItemStyle-Font-Bold="True" ButtonType="LinkButton">
                                                        <HEADERSTYLE Width="120px" VerticalAlign="Middle" CssClass="grid-header" HorizontalAlign="Left"></HEADERSTYLE><ITEMSTYLE Width="120px" CssClass="grid-item"></ITEMSTYLE>
                                                    </ASP:EDITCOMMANDCOLUMN><asp:ButtonColumn Text="刪除" CommandName="Delete" ItemStyle-Font-Bold="True">
                                                        <HeaderStyle HorizontalAlign="Left" Width="120px" CssClass="grid-header" VerticalAlign="Middle"></HeaderStyle><ItemStyle Width="120px" CssClass="grid-item"></ItemStyle>
                                                    </asp:ButtonColumn>
                                                </Columns>
                                            </anthem:DataGrid>
                                        </TD>
                                    </TR><TR>
                                        <TD colSpan="5"></TD>
                                    </TR>
                                </TABLE>
                            </Anthem:panel>

                        </TD>
                    </TR>
                </table>
            </td>
        </tr>
    </table>
</form>
</body>
</HTML>