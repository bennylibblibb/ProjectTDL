<%@ Page Language="C#" AutoEventWireup="false" CodeBehind="Matches.aspx.cs" Inherits="JC_SoccerWeb.Matches" %>
<%@ Register TagPrefix="anthem" Namespace="Anthem" Assembly="Anthem" %>
<%--<%@ Register TagPrefix="uc1" TagName="MenuTabs" Src="UserControl/MenuTabs.ascx" %>--%> 
<%--<%@ Register TagPrefix="uc1" TagName="SendTabs" Src="UserControl/SendTabs.ascx" %>--%>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
<HEAD>
    <title>HKJC</title>
    <meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR">
    <meta content="C#" name="CODE_LANGUAGE">
    <meta content="JavaScript" name="vs_defaultClientScript">
    <meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
    <LINK href="CentaSmsStyle.css" type="text/css" rel="stylesheet">
     <script language="JavaScript" src="JavaScript.js" type ="text/javascript"></script>
<script type="text/javascript"> 
    
    function ClickLinkBtn(obj) {   
        obj.onclick();  
    }  

</script> 
        </HEAD>
        <body>
        <form id="fm1" method="post" runat="server">
    <table cellSpacing="0" cellPadding="0" width="100%" border="0">
        <tr>
            <td class="top_bar01_bg">
           <!--     <img src="resource/mango_logo_title.gif" width="359" height="49"> -->
            </td>
        </tr>
        <tr style="display:none" >
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
                                 <%--<uc1:menutabs id="MenuTabs1" runat="server"></uc1:menutabs>--%> <asp:PlaceHolder ID="PlaceHolder1" runat="server"></asp:PlaceHolder>
                        </td>
                    </tr>
                    <TR vAlign="top">
                        <TD vAlign="top" class="left_bar_bg" style="width:2px;height:100%">
                            <%--<uc1:sendtabs id="SendTabs1" runat="server"></uc1:sendtabs>--%> 
                        </TD>
                        <TD vAlign="top" class="tan-border02" height="1">
                            <Anthem:panel id="plRankDetails" runat="server">
                                <TABLE style="BORDER-COLLAPSE: collapse;  WIDTH:1380px; HEIGHT:600px"
                                       cellSpacing="10" cellPadding="0" rules="none" align="left" border="0">
                                    <TR style="display:none">
                                        <TD vAlign="top" align="left" colspan="5" height="5"></TD>
                                    </TR> 



                                    <TR  style="height:30px" valign="middle"  >
                                        <TD     align="center"     style="width:250px"   >
                                        <span > <strong>START DATE From : </strong></span> 
                                      <anthem:TextBox ID="txtFrom" runat="server"   onclick="SelectDate(this)"   Width="100px">        </anthem:TextBox>
                                       </TD>  
                                        <TD   align="left"  style="width:150px"    > <span > <strong> To : </strong></span>
                                             <anthem:TextBox ID="txtTo" runat="server"    onclick="SelectDate(this)"  Width="100px">   </anthem:TextBox>
                                        </TD>
                                        <TD   align="center"   style="width:150px"    > 
                                            <anthem:DropDownList ID="dplLeague" runat="server" AutoCallBack="True" Width="100px" >
                                            <asp:ListItem Selected="True" Value="-1">All</asp:ListItem>
                                                <asp:ListItem Value="Not started">Not started</asp:ListItem>  
                                                <asp:ListItem Value="inprogress">inprogress</asp:ListItem>  
                                                <asp:ListItem Value="Finished">Finished</asp:ListItem>   
                                                <asp:ListItem Value="Interrupted">Interrupted</asp:ListItem> 
                                                  <asp:ListItem Value="Abandoned">Abandoned</asp:ListItem> 
                                                <asp:ListItem Value="Cancelled">Cancelled</asp:ListItem> 
                                                <asp:ListItem Value="deleted">deleted</asp:ListItem>  
                                                <asp:ListItem Value="unknown">unknown</asp:ListItem>  
                                            </anthem:DropDownList>      
                                        </TD>
                                        <TD   align="left"   style="width:150px"    >
                                            <anthem:Button ID="btnEdit" runat="server" Text="Get"     Width="60px" />  
                                         <strong>  <anthem:Label ID="lbMsg" runat="server" ForeColor="Red" Width="120px" Visible="false">  </anthem:Label>  </strong> </TD>
                                        <TD  align="left"     style="width:600px" >  
                                                                <anthem:RadioButtonList ID="cbDay" runat="server" RepeatDirection="Horizontal"  Width="200px"  AutoCallBack="true"  >
                                                                <asp:ListItem Value ="-1" Selected="True">All</asp:ListItem>
                                                                <asp:ListItem>MON</asp:ListItem> 
                                                                <asp:ListItem>TUE</asp:ListItem>
                                                                <asp:ListItem>WED</asp:ListItem>
                                                                <asp:ListItem>THU</asp:ListItem>
                                                                <asp:ListItem>FRI</asp:ListItem> 
                                                                <asp:ListItem>SAT</asp:ListItem>
                                                                <asp:ListItem>SUN</asp:ListItem>  
                                             </anthem:RadioButtonList> 
                                        </TD>
                                        </TR> 




                                   <TR valign="top" >
                                        <TD    valign="top" height="1" colspan="5" >   <hr/>    </TD> 
                                  </TR> 
                                    <TR  valign="top">
                                        <TD vAlign="top" align="left" colSpan="5"  ><%--OnSelectedIndexChanged="dgRankDetails_SelectedIndexChanged"--%>
                                            <anthem:DataGrid id="dgRankDetails" runat="server"   Width="100%"  AlternatingItemStyle-BackColor="#EFEFEF"   AllowPaging="True" AutoGenerateColumns="False"  >
                                                <PagerStyle Mode="NumericPages"></PagerStyle><HeaderStyle Font-Bold="True"></HeaderStyle><EditItemStyle ></EditItemStyle><ItemStyle Height="34px" CssClass="grid-item"></ItemStyle>
                                                 <SelectedItemStyle   BackColor="#CECB7B"></SelectedItemStyle> 
                                                <Columns>
                                                    <asp:TemplateColumn HeaderText=""   >
                                                        <HeaderStyle HorizontalAlign="Left" Width="20px" CssClass="grid-header" VerticalAlign="Middle"></HeaderStyle>
                                                        <ItemStyle Width="30px" CssClass="grid-item"></ItemStyle>
                                                        <ItemTemplate> 
  <asp:LinkButton id="btnSelect"  runat="server"  CommandName="Select" Text=' <%#this.dgRankDetails.CurrentPageIndex * this.dgRankDetails.PageSize + Container.ItemIndex + 1%>'> </asp:LinkButton>
                                                          </ItemTemplate>
                                                    </asp:TemplateColumn>
                                                    <asp:TemplateColumn HeaderText="ID"   >
                                                        <HeaderStyle HorizontalAlign="Left"  Wrap="true" Width="50px" CssClass="grid-header" VerticalAlign="Middle"></HeaderStyle><ItemStyle CssClass="grid-item" Width="80px"></ItemStyle><ItemTemplate>
                                                            <anthem:Label id="lbID" runat="server" Width="50px"    Text='<%# DataBinder.Eval(Container, "DataItem.ID") %>'></anthem:Label>
                                                           <%--<asp:LinkButton ID="btnSelect" Visible="false" runat="server" Width="50px" CommandName="Select" Text='<%# DataBinder.Eval(Container, "DataItem.ID") %>'> </asp:LinkButton>--%>
                                                          
                                                       </ItemTemplate>
                                                    </asp:TemplateColumn>
                                                        <asp:TemplateColumn HeaderText="LEAGUE">
                                                        <HeaderStyle Wrap="true" HorizontalAlign="Left" Width="60px" CssClass="grid-header" VerticalAlign="Middle"></HeaderStyle><ItemStyle CssClass="grid-item"></ItemStyle><ItemTemplate>
                                                            <anthem:Label id="lbCLEAGUE_OUTPUT_NAME" runat="server" Width="60px" Text='<%# DataBinder.Eval(Container, "DataItem.CLEAGUE_HKJC_NAME") %>'></anthem:Label>
                                                        </ItemTemplate>
                                                    </asp:TemplateColumn>
                                                    <asp:TemplateColumn HeaderText="NAME" Visible ="true" >
                                                        <HeaderStyle HorizontalAlign="Left" Width="120px" CssClass="grid-header" VerticalAlign="Middle"></HeaderStyle><ItemStyle CssClass="grid-item" Width="80px"></ItemStyle><ItemTemplate>
                                                            <anthem:Label id="lbNAME" BackColor='<%#DataBinder.Eval(Container, "DataItem.MAPPINGSTATUS") is DBNull?System.Drawing.Color.Empty:  Convert.ToBoolean(DataBinder.Eval(Container, "DataItem.MAPPINGSTATUS")) == true ? System.Drawing.Color.Red:System.Drawing.Color.Empty %>'    runat="server" Width="120px" Text='<%# DataBinder.Eval(Container, "DataItem.NAME") %>'></anthem:Label>
                                                     </ItemTemplate>
                                                    </asp:TemplateColumn>
                                                    <asp:TemplateColumn HeaderText="START DATE"   Visible ="true" >
                                                        <HeaderStyle HorizontalAlign="Left"  Wrap="true" Width="80px" CssClass="grid-header" VerticalAlign="Middle"></HeaderStyle><ItemStyle CssClass="grid-item" Width="80px"></ItemStyle><ItemTemplate>
                                                            <anthem:Label id="lbSTART_DATE" runat="server" Width="80px" Text='<%# DataBinder.Eval(Container, "DataItem.START_DATE") %>'></anthem:Label>
                                                        </ItemTemplate>
                                                    </asp:TemplateColumn>
                                                    <asp:TemplateColumn HeaderText="H GOAL" Visible ="true" >
                                                        <HeaderStyle HorizontalAlign="Left" Wrap="true" Width="20px" CssClass="grid-header" VerticalAlign="Middle"></HeaderStyle><ItemStyle CssClass="grid-item" Width="80px"></ItemStyle><ItemTemplate>
                                                            <anthem:Label id="lbH_GOAL" runat="server" Width="20px" Text='<%# DataBinder.Eval(Container, "DataItem.H_GOAL") %>'></anthem:Label>
                                                        </ItemTemplate>
                                                    </asp:TemplateColumn>
                                                    <asp:TemplateColumn HeaderText="G GOAL" Visible ="true" >
                                                        <HeaderStyle HorizontalAlign="Left" Wrap="true" Width="20px" CssClass="grid-header" VerticalAlign="Middle"></HeaderStyle><ItemStyle CssClass="grid-item" Width="80px"></ItemStyle><ItemTemplate>
                                                            <anthem:Label id="lbG_GOAL" runat="server" Width="20px" Text='<%# DataBinder.Eval(Container, "DataItem.G_GOAL") %>'></anthem:Label>
                                                        </ItemTemplate>
                                                    </asp:TemplateColumn> 
                                                    <asp:TemplateColumn HeaderText="H YELLOW">
                                                        <HeaderStyle Wrap="true" HorizontalAlign="Left" Width="20px" CssClass="grid-header" VerticalAlign="Middle"></HeaderStyle><ItemStyle CssClass="grid-item"></ItemStyle><ItemTemplate>
                                                            <anthem:Label id="lbH_YELLOW" runat="server" Width="20px" Text='<%# DataBinder.Eval(Container, "DataItem.H_YELLOW") %>'></anthem:Label>
                                                        </ItemTemplate>
                                                    </asp:TemplateColumn><asp:TemplateColumn HeaderText="G YELLOW">
                                                        <HeaderStyle Wrap="true" HorizontalAlign="Left" Width="20px" CssClass="grid-header" VerticalAlign="Middle"></HeaderStyle><ItemStyle CssClass="grid-item"></ItemStyle><ItemTemplate>
                                                            <anthem:Label id="lbG_YELLOW" runat="server" Width="20px" Text='<%# DataBinder.Eval(Container, "DataItem.G_YELLOW") %>'></anthem:Label>
                                                        </ItemTemplate>
                                                    </asp:TemplateColumn>
                                                    <%--<asp:TemplateColumn HeaderText="排名">
                                                        <HeaderStyle Wrap="false" HorizontalAlign="Left" Width="120px" CssClass="grid-header" VerticalAlign="Middle"></HeaderStyle><ItemStyle Width="100px" CssClass="grid-item"></ItemStyle><ItemTemplate>
                                                            <anthem:Label id="lbIRANK" runat="server" Width="120px" Text='<%# DataBinder.Eval(Container, "DataItem.IRANK") %>'></anthem:Label>
                                                            <anthem:TextBox id="txtlbIRANK2" runat="server" Width="120px" Visible ="false" Text='<%# DataBinder.Eval(Container, "DataItem.IRANK") %>'></anthem:TextBox>
                                                             <Anthem:RangeValidator ID="RangeValidator1" runat="server" ControlToValidate="txtlbIRANK2" ErrorMessage="RangeValidator" Type="Integer" Visible ="false" ></Anthem:RangeValidator> 
                                                        </ItemTemplate><EditItemTemplate>
                                                             <anthem:TextBox id="txtlbIRANK" Visible ="false"  runat="server" Width="120px" Text='<%# DataBinder.Eval(Container, "DataItem.IRANK") %>'>
                                                            </anthem:TextBox>
                                                            <anthem:Label id="lbIRANK2" runat="server" Width="120px" Text='<%# DataBinder.Eval(Container, "DataItem.IRANK") %>'></anthem:Label>
                                                        </EditItemTemplate> 
                                                          <!--R.ID ,R.NAME  ,R.START_DATE,G.H_GOAL,G.G_GOAL,G.H_YELLOW,G.G_YELLOW,G.H_RED,G.G_RED,E.HKJCHOSTNAME,E.HKJCGUESTNAME,E.HKJCDAYCODE,E.HKJCMATCHNO -->
                                                  
                                                    </asp:TemplateColumn><asp:TemplateColumn HeaderText="H_RED">
                                                        <HeaderStyle Wrap="false" HorizontalAlign="Left" Width="100px" CssClass="grid-header" VerticalAlign="Middle"></HeaderStyle><ItemStyle Width="100px" CssClass="grid-item"></ItemStyle><ItemTemplate>
                                                            <anthem:Label id="lbIHOSTORYRANK" runat="server" Width="100px" Text='<%# DataBinder.Eval(Container, "DataItem.H_RED") %>'></anthem:Label>
                                                            <anthem:TextBox id="txtIHOSTORYRANK2" runat="server" Width="100px"  Visible ="false" Text='<%# DataBinder.Eval(Container, "DataItem.IHOSTORYRANK") %>'></anthem:TextBox>                                                             
                                                              </ItemTemplate><EditItemTemplate>
                                                            <anthem:TextBox id="txtIHOSTORYRANK" runat="server" Width="100px" Text='<%# DataBinder.Eval(Container, "DataItem.IHOSTORYRANK") %>'>
                                                            </anthem:TextBox>
                                                            <anthem:Label id="lbIHOSTORYRANK3" Visible ="false"  runat="server" Width="100px" Text='<%# DataBinder.Eval(Container, "DataItem.IHOSTORYRANK") %>'></anthem:Label>
                                                        </EditItemTemplate>
                                                    </asp:TemplateColumn>--%>

                                                      <asp:TemplateColumn HeaderText="H RED" Visible ="true" >
                                                        <HeaderStyle HorizontalAlign="Left" Wrap="true" Width="20px" CssClass="grid-header" VerticalAlign="Middle"></HeaderStyle><ItemStyle CssClass="grid-item" Width="80px"></ItemStyle><ItemTemplate>
                                                            <anthem:Label id="lbH_RED" runat="server" Width="20px" Text='<%# DataBinder.Eval(Container, "DataItem.H_RED") %>'></anthem:Label>
                                                        </ItemTemplate>
                                                    </asp:TemplateColumn>
                                                    <asp:TemplateColumn HeaderText="G RED" Visible ="true" >
                                                        <HeaderStyle HorizontalAlign="Left" Wrap="true" Width="20px" CssClass="grid-header" VerticalAlign="Middle"></HeaderStyle><ItemStyle CssClass="grid-item" Width="80px"></ItemStyle><ItemTemplate>
                                                            <anthem:Label id="lbG_RED" runat="server" Width="20px" Text='<%# DataBinder.Eval(Container, "DataItem.G_RED") %>'></anthem:Label>
                                                        </ItemTemplate>
                                                    </asp:TemplateColumn>
                                                    <asp:TemplateColumn HeaderText="JC HOST" Visible ="true" >
                                                        <HeaderStyle HorizontalAlign="Left" Wrap="true" Width="100px" CssClass="grid-header" VerticalAlign="Middle"></HeaderStyle><ItemStyle CssClass="grid-item" ></ItemStyle><ItemTemplate>
                                                            <anthem:Label id="lbHKJCHOSTNAME" runat="server" Width="100px" Text='<%# DataBinder.Eval(Container, "DataItem.HKJCHOSTNAME") %>'></anthem:Label> 
                                                            <anthem:Label id="lbHKJCHOSTNAMEcn" runat="server" Width="100px" Text='<%# DataBinder.Eval(Container, "DataItem.HKJCHOSTNAME_CN") %>'></anthem:Label>
                                                             </ItemTemplate>
                                                    </asp:TemplateColumn> 
                                                    <asp:TemplateColumn HeaderText="JC GUEST">
                                                       <HeaderStyle Wrap="true" HorizontalAlign="Left"  Width="100px" CssClass="grid-header" VerticalAlign="Middle"></HeaderStyle><ItemStyle CssClass="grid-item"></ItemStyle><ItemTemplate>
                                                            <anthem:Label id="lbHKJCGUESTNAME" runat="server" Width="100px" Text='<%# DataBinder.Eval(Container, "DataItem.HKJCGUESTNAME") %>'></anthem:Label>
                                                       <anthem:Label id="lbHKJCGUESTNAMEcn" runat="server" Width="100px" Text='<%# DataBinder.Eval(Container, "DataItem.HKJCGUESTNAME_CN") %>'></anthem:Label>
                                                            </ItemTemplate>
                                                    </asp:TemplateColumn><asp:TemplateColumn HeaderText="DAY CODE">
                                                        <HeaderStyle Wrap="true" HorizontalAlign="Left" Width="50px" CssClass="grid-header" VerticalAlign="Middle"></HeaderStyle><ItemStyle CssClass="grid-item"></ItemStyle><ItemTemplate>
                                                            <anthem:Label id="lbHKJCDAYCODE" runat="server" Width="50px" Text='<%# DataBinder.Eval(Container, "DataItem.HKJCDAYCODE") %>'></anthem:Label>
                                                        </ItemTemplate>
                                                        <EditItemTemplate>  
                                                            <anthem:DropDownList ID="dplDayCode"  Width="50px" runat="server"> 
                                                                <asp:ListItem>MON</asp:ListItem> 
                                                                <asp:ListItem>TUE</asp:ListItem>
                                                                <asp:ListItem>WED</asp:ListItem>
                                                                <asp:ListItem>THU</asp:ListItem>
                                                                <asp:ListItem>FRI</asp:ListItem> 
                                                                <asp:ListItem>SAT</asp:ListItem>
                                                                <asp:ListItem>SUN</asp:ListItem> 
                                                                </anthem:DropDownList> 
                                                            <anthem:Label id="lbDAYCODE" runat="server" Width="50px" Text='<%# DataBinder.Eval(Container, "DataItem.HKJCDAYCODE") %>'></anthem:Label>
                                                        </EditItemTemplate> 
                                                    </asp:TemplateColumn>
                                                    <asp:TemplateColumn HeaderText="MATCH NO">
                                                        <HeaderStyle Wrap="true" HorizontalAlign="Left" Width="50px" CssClass="grid-header" VerticalAlign="Middle"></HeaderStyle><ItemStyle CssClass="grid-item"></ItemStyle><ItemTemplate>
                                                            <anthem:Label id="lbHKJCMATCHNO" runat="server" Width="50px" Text='<%# DataBinder.Eval(Container, "DataItem.HKJCMATCHNO") %>'></anthem:Label>
                                                        </ItemTemplate>
                                                        <EditItemTemplate>
                                                             <anthem:TextBox id="txtMATCHNO"    runat="server" Width="50px" Text='<%# DataBinder.Eval(Container, "DataItem.HKJCMATCHNO") %>'>
                                                            </anthem:TextBox>
                                                            <anthem:Label id="lbMATCHNO" runat="server" Width="50px" Text='<%# DataBinder.Eval(Container, "DataItem.HKJCMATCHNO") %>'></anthem:Label>
                                                        </EditItemTemplate>  
                                                    </asp:TemplateColumn>

                                                    <asp:TemplateColumn HeaderText="STATUS">
                                                        <HeaderStyle Wrap="false" HorizontalAlign="Left" Width="70px" CssClass="grid-header" VerticalAlign="Middle"></HeaderStyle><ItemStyle CssClass="grid-item"></ItemStyle><ItemTemplate>
                                                            <anthem:Label id="lbHKJCISTATUS" runat="server" Width="70px" Text='<%# DataBinder.Eval(Container, "DataItem.STATUS") %>'></anthem:Label>
                                                        </ItemTemplate>
                                                         </asp:TemplateColumn>

                                                    <asp:TemplateColumn HeaderText="MATCH DATETIME">
                                                        <HeaderStyle HorizontalAlign="Left" Wrap="true" Width="80px" CssClass="grid-header" VerticalAlign="Middle"></HeaderStyle><ItemStyle CssClass="grid-item" Width="120px"></ItemStyle><ItemTemplate>
                                                            <anthem:Label id="lbCTIMESTAMP" runat="server" Width="80px" Text='<%# DataBinder.Eval(Container, "DataItem.CMATCHDATETIME ") %>'></anthem:Label>
                                                        </ItemTemplate>
                                                    </asp:TemplateColumn>
   <asp:TemplateColumn HeaderText="Booked"  Visible="FALSE" >
                                                        <HeaderStyle Wrap="false" HorizontalAlign="Left" Width="40px" CssClass="grid-header" VerticalAlign="Middle"></HeaderStyle><ItemStyle CssClass="grid-item"></ItemStyle><ItemTemplate>
                                                            <anthem:Label id="lbBooked" runat="server" Width="40px" Text='<%# DataBinder.Eval(Container, "DataItem.MAPPINGSTATUS") %>'></anthem:Label>
                                                        </ItemTemplate>
                                                         </asp:TemplateColumn>
                                                    
   <asp:TemplateColumn HeaderText="TimeStamp">
                                                        <HeaderStyle Wrap="false" HorizontalAlign="Left" Width="80px" CssClass="grid-header" VerticalAlign="Middle"></HeaderStyle><ItemStyle CssClass="grid-item"></ItemStyle><ItemTemplate>
                                                            <anthem:Label id="lbTimestamp" runat="server" Width="80px" Text='<%# DataBinder.Eval(Container, "DataItem.cTimeStamp") %>'></anthem:Label>
                                                        </ItemTemplate>
                                                         </asp:TemplateColumn>

                                                    <asp:TemplateColumn HeaderText="Goals" Visible="false">
                                                        <HeaderStyle Wrap="false" HorizontalAlign="Left" Width="40px" CssClass="grid-header" VerticalAlign="Middle"></HeaderStyle><ItemStyle CssClass="grid-item"></ItemStyle><ItemTemplate>
                                                         <a href=# onclick="window.open('MatchDetails.aspx?Type=HKJC&ID=<%# DataBinder.Eval(Container, "DataItem.ID") %>','','scrollbars=yes, resizable=yes, Width=900,height=400, top=200,left=200')" >																	
																		<b>Goals</b>	</a>   </ItemTemplate>
                                                         </asp:TemplateColumn>
                                                    <asp:TemplateColumn HeaderText="Team Mapping">
                                                        <HeaderStyle Wrap="true" HorizontalAlign="Left" Width="40px" CssClass="grid-header" VerticalAlign="Middle"></HeaderStyle><ItemStyle CssClass="grid-item"></ItemStyle><ItemTemplate>
                                                          <a href=# onclick="window.open('TeamMapping.aspx?Type=HKJC&id=<%# DataBinder.Eval(Container, "DataItem.ID") %>','','scrollbars=yes, resizable=yes, Width=650,height=280, top=200,left=200')" >																	
																		<b>    <%# DataBinder.Eval(Container, "DataItem.MAPPING_STATUS")==DBNull.Value? "Mapping" :Convert.ToBoolean(DataBinder.Eval(Container, "DataItem.MAPPING_STATUS"))? "Mapped":"Mapping"  %>  </b>	</a></ItemTemplate>
                                                         </asp:TemplateColumn>
<ASP:EDITCOMMANDCOLUMN Visible="FALSE" EditText="Edit" CancelText="Cancel" UpdateText="Update" ItemStyle-Font-Bold="True" ButtonType="LinkButton">
                                                        <HEADERSTYLE Width="150px"  Wrap="false" VerticalAlign="Middle" CssClass="grid-header" HorizontalAlign="Left"></HEADERSTYLE><ITEMSTYLE Width="120px" CssClass="grid-item"></ITEMSTYLE>
                                                    </ASP:EDITCOMMANDCOLUMN><asp:ButtonColumn  Visible="FALSE" Text="刪除" CommandName="Delete" ItemStyle-Font-Bold="True">
                                                        <HeaderStyle HorizontalAlign="Left" Wrap="false" Width="150px" CssClass="grid-header" VerticalAlign="Middle"></HeaderStyle><ItemStyle Width="120px" CssClass="grid-item"></ItemStyle>
                                                    </asp:ButtonColumn>
                                                </Columns>
                                            </anthem:DataGrid>
                                        </TD>
                                    </TR>
                                     <TR style="height:10%;display:none">
                                        <TD vAlign="top" align="left" colspan="5" height="5"></TD>
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
