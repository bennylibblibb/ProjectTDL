<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ENTTeamMapping.aspx.cs" Inherits="WebTeamMapping.ENTTeamMapping" %>
<%@ Register Assembly="AspNetPager" Namespace="Wuqi.Webdiyer" TagPrefix="webdiyer" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
    <script language="javascript">
        function showAddPage() {
            window.open("ENetTeamMapDetail.aspx", "_blank", "height=200, width=300, toolbar =no, menubar=no, scrollbars=no, resizable=no, location=no, status=no");
            
        }
    </script>
    <style type="text/css">
        .addteammsg {
            text-align:center;
            color:red;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    
    </div>
         <table width="90%">

            <tr>
                <td style="border:1px solid #808080;">
                     <table width="100%">
                         <tr>
                             <th colspan="8" style="background-color:antiquewhite;border-bottom:1px solid #808080;">Search:</th>
                         </tr>
                         <tr>
                                <td width="130px"> Search Field:</td>
                                <td width="130px"> 
                                <asp:DropDownList AutoPostBack="false" ID="dpSearchType" runat="server" Width="150px">
                                 <asp:ListItem Value=""></asp:ListItem>
                                 <asp:ListItem Value="HKJC EngName">HKJC EngName</asp:ListItem>
                                 <asp:ListItem Value="HKJC Short EN">HKJC Short Name</asp:ListItem>
                                 <asp:ListItem Value="ENetTeamName">ENet Team Name</asp:ListItem>
                                </asp:DropDownList>
                                </td>
                                 <td width="130px">
                                     Key Word:
                                 </td>

                                <td width="130px">
                                    <asp:TextBox ID="txtKeyWord" runat="server"></asp:TextBox>                    
                                </td>
                                <td width="100px"><asp:Button ID="btnSearch" Width="100" runat="server" Text="Search" OnClick="btnSearch_Click" /></td>
                                <td> <%--<input type="button" value="Add" id="btnAdd" style="width:75px;" onclick="showAddPage();" />--%></td>
                                <td > </td>
                             <td></td>
                        </tr>
                     </table>
                </td>
              
            </tr>
              
            <tr>
                
               <td style="border:1px solid #808080;">
                   <table width="100%">
                       <tr>
                           <th colspan="8" style="background-color:antiquewhite;border-bottom:1px solid #808080;">Add New Team Mapping:</th>
                       </tr>
                       <tr>
                            <td width="130px">HKJC EngName:</td>
                            <td width="130px"><asp:TextBox ID="txtAddHJEngTeamName" runat="server"></asp:TextBox></td>
                            <td width="130px">ENet TeamName:</td>
                            <td width="130px"><asp:TextBox ID="txtAddENetTeamName" runat="server"></asp:TextBox></td>
                                <td width="100px">ENet ID:</td>
                            <td width="130px"><asp:TextBox ID="txtAddEnetTeamID" runat="server"></asp:TextBox></td>
                            <td> 
                                <asp:Button ID="btnAddNew" Width="100" runat="server" Text="Add" OnClick="btnAddNew_Click" />
                            </td>
                           <td>&nbsp;</td>
                       </tr>
                   </table>
               </td>
               
            </tr>
             <tr>
                 <td ><asp:Label ID="lbAddMsg" runat="server" Width="100%" CssClass="addteammsg" Text=""></asp:Label></td>
             </tr>
        </table>
     
       
        <br/>

        <asp:GridView ID="GVTeam" RowStyle-Height="25px" Width="90%" AutoGenerateColumns="false" runat="server" OnRowCancelingEdit="GVTeam_RowCancelingEdit" OnRowEditing="GVTeam_RowEditing" OnRowUpdating="GVTeam_RowUpdating" OnRowDeleting="GVTeam_RowDeleting">
            <Columns>
                <asp:TemplateField Visible="False" HeaderStyle-BackColor="antiquewhite"  HeaderText="HKJC_ITeam_Code">
                    <ItemTemplate><asp:Label ID="lbTeamCode" runat="server" Text='<%# Bind("ITEAM_CODE") %>'></asp:Label></ItemTemplate>
                    <HeaderStyle BackColor="AntiqueWhite"></HeaderStyle>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="HKJC ENG NAME" HeaderStyle-BackColor="antiquewhite" >
                    <ItemTemplate>
                        <asp:Label ID="lbHKJCEngName" runat="server" Text='<%# Bind("CENG_NAME") %>'></asp:Label>
                    </ItemTemplate>
                    <EditItemTemplate>
                        <asp:TextBox ID="txtHKJCEngName" runat="server" Text='<%#Bind("CENG_NAME")%>'></asp:TextBox>
                    </EditItemTemplate>

<HeaderStyle BackColor="AntiqueWhite"></HeaderStyle>
                </asp:TemplateField>
                <%--<asp:BoundField  HeaderText="HKJC Short EN" HeaderStyle-BackColor="antiquewhite" DataField="CSHT_ENG_NAME" />--%>

                 <asp:TemplateField HeaderText="HKJC Short Name" HeaderStyle-BackColor="antiquewhite" >
                    <ItemTemplate>
                        <asp:Label ID="lbCSHT_ENG_NAME" runat="server" Text='<%# Bind("CSHT_ENG_NAME") %>'></asp:Label>
                    </ItemTemplate>
                   <%-- <EditItemTemplate>
                        <asp:TextBox ID="txtCSHT_ENG_NAME" runat="server" Text='<%#Bind("CSHT_ENG_NAME")%>'></asp:TextBox>
                    </EditItemTemplate>--%>

<HeaderStyle BackColor="AntiqueWhite"></HeaderStyle>
                </asp:TemplateField>

                 <%--<asp:BoundField HeaderText="hkjc Ch Name" HeaderStyle-BackColor="antiquewhite" DataField="CCHI_NAME"/>--%>

                <asp:TemplateField HeaderText="hkjc Ch Name" HeaderStyle-BackColor="antiquewhite" >
                    <ItemTemplate>
                        <asp:Label ID="lbCCHI_NAME" runat="server" Text='<%# Bind("CCHI_NAME") %>'></asp:Label>
                    </ItemTemplate>
                   <%-- <EditItemTemplate>
                        <asp:TextBox ID="txtCCHI_NAME" runat="server" Text='<%#Bind("CCHI_NAME")%>'></asp:TextBox>
                    </EditItemTemplate>--%>

<HeaderStyle BackColor="AntiqueWhite"></HeaderStyle>
                </asp:TemplateField>

                 <asp:TemplateField HeaderText="ENET_TEAM_ID" HeaderStyle-BackColor="antiquewhite" >
                    <ItemTemplate>
                        <asp:Label ID="lbEnetTeamID" runat="server" Text='<%# Bind("ENET_TEAM_ID") %>'></asp:Label>
                    </ItemTemplate>
                      <EditItemTemplate>
                        <asp:TextBox ID="txtEnetTeamID" runat="server" Text='<%#Bind("ENET_TEAM_ID")%>'></asp:TextBox>
                    </EditItemTemplate>

<HeaderStyle BackColor="AntiqueWhite"></HeaderStyle>
                </asp:TemplateField>

                 <asp:TemplateField HeaderText="ENET_TEAM_NAME" HeaderStyle-BackColor="antiquewhite" >
                    <ItemTemplate>
                        <asp:Label ID="lbEnetTeamName" runat="server" Text='<%# Bind("ENET_TEAM_NAME") %>'></asp:Label>
                    </ItemTemplate>
                      <EditItemTemplate>
                        <asp:TextBox ID="txtEnetTeamName" runat="server" Text='<%#Bind("ENET_TEAM_NAME")%>'></asp:TextBox>
                    </EditItemTemplate>

<HeaderStyle BackColor="AntiqueWhite"></HeaderStyle>
                </asp:TemplateField>

                <asp:TemplateField HeaderText="" ShowHeader="False">
                <EditItemTemplate>
                 <%--   <asp:Button runat="server" CommandName="Update"  Text="更新" />--%>
             <asp:LinkButton ID="lkbtnUpdate" runat="server" CausesValidation="False" 
                CommandName="Update" Text="Update"></asp:LinkButton>
                <asp:LinkButton ID="lkbtnCancel" runat="server" CausesValidation="False" 
                CommandName="Cancel" Text="Cancel"></asp:LinkButton>
                </EditItemTemplate>
                <ItemTemplate>
                <asp:LinkButton ID="lkbtnEdit" runat="server" CausesValidation="False" 
                CommandName="Edit" Text="Edit"></asp:LinkButton>
                </ItemTemplate>
                </asp:TemplateField>

                 <asp:TemplateField HeaderText="">
                    <ItemTemplate>
                        <asp:LinkButton runat="server" Text="Delete" CommandName="Delete" OnClientClick="return confirm('sure to delete?')"></asp:LinkButton>
                    </ItemTemplate>
                   
                </asp:TemplateField>
             <%--    <asp:CommandField  HeaderText="删除" ShowDeleteButton="True" />--%>

                 
            </Columns>
            <EditRowStyle Font-Names="Arial Narrow" Font-Size="Medium" />
        </asp:GridView>
        <br />
           <webdiyer:AspNetPager ID="pager" runat="server" PageSize="23"
              CssClass="page-ctrl" CustomInfoHTML=" %CurrentPageIndex% / %PageCount%" 
              CustomInfoSectionWidth="10%" FirstPageText="首页" LastPageText="末页" 
              NextPageText="下一页" NumericButtonCount="5" onpagechanged="pager_PageChanged" 
              PrevPageText="上一页" ShowCustomInfoSection="Left" ShowNavigationToolTip="True">
       </webdiyer:AspNetPager>  


        <br />


    </form>
</body>
</html>
