<%@ Register TagPrefix="anthem" Namespace="Anthem" Assembly="Anthem" %>
<%@ Register TagPrefix="uc1" TagName="MenuTabs" Src="UserControl/MenuTabs.ascx" %>
<%@ Page language="c#" Codebehind="Update.aspx.cs" AutoEventWireup="false" Inherits="JC_SoccerWeb.Update" ValidateRequest="false" %>
<%@ Register TagPrefix="uc1" TagName="SendTabs" Src="UserControl/SendTabs.ascx" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
<HEAD>
    <title>STATSCORE</title>
    <meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR">
    <meta content="C#" name="CODE_LANGUAGE">
    <meta content="JavaScript" name="vs_defaultClientScript">
    <meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
    <LINK href="CentaSmsStyle.css" type="text/css" rel="stylesheet">
    <script language="JavaScript" src="JavaScript.js" type ="text/javascript"></script>
<script type="text/javascript">
    function CheckNum(obj) {
        var strvalue = obj.value;
        if (isNaN(strvalue)) {
            alert("Number");
        } else { 
        } 
    }   

    function ClickLinkBtn(obj) {  
      //  obj.onclick(); 
    }  

    function OpenMapping(obj) {
        return;
        window.open('TeamMapping.aspx?id='+obj, '', 'scrollbars=yes, resizable=yes, Width=600,height=250, top=200,left=200');
         }

    dFeatures = 'dialogHeight: 450px; dialogWidth: 1049px; dialogTop: 646px; dialogLeft: 4px; edge: Raised; center: Yes; help: Yes; resizable: Yes; status: Yes;'; //default features
    modalWin = "";
            function xShowModalDialog(sURL, vArguments, sFeatures) {
                if (sURL == null || sURL == '') {
                    alert("Invalid URL input.");
                    return false;
                }
                if (vArguments == null || vArguments == '') {
                    vArguments = '';
                }
                if (sFeatures == null || sFeatures == '') {
                    sFeatures = dFeatures;
                }
                if (window.navigator.appVersion.indexOf("MSIE") != -1) {
                    window.showModalDialog(sURL, vArguments, sFeatures);
                    return false;
                }
                sFeatures = sFeatures.replace(/ /gi, '');
                aFeatures = sFeatures.split(";");
                sWinFeat = "directories=0,menubar=0,titlebar=0,toolbar=0,";
                for (x in aFeatures) {
                    aTmp = aFeatures[x].split(":");
                    sKey = aTmp[0].toLowerCase();
                    sVal = aTmp[1];
                    switch (sKey) {
                        case "dialogheight":
                            sWinFeat += "height=" + sVal + ",";
                            pHeight = sVal;
                            break;
                        case "dialogwidth":
                            sWinFeat += "width=" + sVal + ",";
                            pWidth = sVal;
                            break;
                        case "dialogtop":
                            sWinFeat += "screenY=" + sVal + ",";
                            break;
                        case "dialogleft":
                            sWinFeat += "screenX=" + sVal + ",";
                            break;
                        case "resizable":
                            sWinFeat += "resizable=" + sVal + ",";
                            break;
                        case "status":
                            sWinFeat += "status=" + sVal + ",";
                            break;
                        case "center":
                            if (sVal.toLowerCase() == "yes") {
                                sWinFeat += "screenY=" + ((screen.availHeight - pHeight) / 2) + ",";
                                sWinFeat += "screenX=" + ((screen.availWidth - pWidth) / 2) + ",";
                            }
                            break;
                    }
                }
                modalWin = window.open(String(sURL), "", sWinFeat);
                if (vArguments != null && vArguments != '') {
                    modalWin.dialogArguments = vArguments;
                }
            }

            function ShowOrderContact(pagePath, winAttrs) {
                xShowModalDialog(pagePath, '', winAttrs);
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
                                <TABLE style="BORDER-COLLAPSE: collapse;  WIDTH:1380px; HEIGHT:600px"
                                       cellSpacing="10" cellPadding="0" rules="none" align="left" border="0">
                                    <TR style="display:none">
                                        <TD vAlign="top" align="left" colspan="5" height="5"></TD>
                                    </TR> 



                                    <TR  valign="bottom" >
                                        <TD    align="center"  height="1"    >
                                        <span > <strong> From</strong></span> 
                                      <anthem:TextBox ID="txtFrom" runat="server" onclick="SelectDate(this)"  Width="100px">        </anthem:TextBox>
                                       </TD>  
                                        <TD   align="left" height="1"   > <span > <strong> To</strong></span>
                                             <anthem:TextBox ID="txtTo" runat="server" onclick="SelectDate(this)"   Width="100px">   </anthem:TextBox>
                                        </TD>
                                        <TD   align="center" height="1"> 
                                            <anthem:DropDownList ID="dplLeague" runat="server" AutoCallBack="True" Width="100px" >
                                            <asp:ListItem Selected="True" Value="All">All</asp:ListItem>    </anthem:DropDownList>      
                                        </TD>
                                        <TD   align="left" height="1">
                                            <anthem:Button ID="btnEdit" runat="server" Text="Get"     Width="60px" />  
                                        </TD>
                                        <TD  align="left"   style="width:600px" > 
                                            <strong>  <anthem:Label ID="lbMsg" runat="server" ForeColor="Red" Width="500px" Visible="false">  </anthem:Label>  
                                          </strong>
                                             <anthem:CheckBoxList ID="cblIP"  Enabled="false"  runat="server"  RepeatDirection="Horizontal"  Width="10px" Visible ="false"  ></anthem:CheckBoxList>  </TD>
                                        </TR> 




                                   <TR valign="top" >
                                        <TD    valign="top" height="1" colspan="5" >   <hr/>    </TD> 
                                  </TR> 
                                    <TR  valign="top">
                                        <TD vAlign="top" align="left" colSpan="5"  >
                                            <anthem:DataGrid id="dgRankDetails" runat="server" OnSelectedIndexChanged="dgRankDetails_SelectedIndexChanged" Width="100%" AlternatingItemStyle-BackColor="#EFEFEF"   AllowPaging="True" AutoGenerateColumns="False"  >
                                                <PagerStyle Mode="NumericPages"></PagerStyle>
                                                <HeaderStyle Font-Bold="True"></HeaderStyle><EditItemStyle ></EditItemStyle>
                                                <ItemStyle Height="34px" CssClass="grid-item"></ItemStyle>
                                                <SelectedItemStyle   BackColor="#CECB7B"></SelectedItemStyle> 
                                                <Columns>
                                                    <asp:TemplateColumn HeaderText="ID"    Visible="false">
                                                        <HeaderStyle HorizontalAlign="Left" Width="20px" CssClass="grid-header" VerticalAlign="Middle"></HeaderStyle><ItemStyle Width="30px" CssClass="grid-item"></ItemStyle>
                                                        <ItemTemplate>
                                                            <%#this.dgRankDetails.CurrentPageIndex * this.dgRankDetails.PageSize + Container.ItemIndex + 1%>
                                                        </ItemTemplate>
                                                    </asp:TemplateColumn>
                                                     <asp:TemplateColumn HeaderText="ID"   >
                                                        <HeaderStyle HorizontalAlign="Left"  Wrap="true" Width="50px" CssClass="grid-header" VerticalAlign="Middle"></HeaderStyle><ItemStyle CssClass="grid-item" Width="80px"></ItemStyle><ItemTemplate>
                                                            <anthem:Label id="lbID"   Visible="false"  runat="server" Width="50px"  Text='<%# DataBinder.Eval(Container, "DataItem.ID") %>'></anthem:Label>
                                                      <asp:LinkButton ID="btnSelect" runat="server"  CommandName="Select" Text='<%# DataBinder.Eval(Container, "DataItem.ID") %>'> </asp:LinkButton>
                                                            </ItemTemplate>
                                                    </asp:TemplateColumn>
                                                    <asp:TemplateColumn HeaderText="NAME" Visible ="true" >
                                                        <HeaderStyle HorizontalAlign="Left" Width="120px" CssClass="grid-header" VerticalAlign="Middle"></HeaderStyle><ItemStyle CssClass="grid-item" Width="80px"></ItemStyle><ItemTemplate>
                                                            <anthem:Label id="lbNAME" runat="server" Width="120px" Text='<%# DataBinder.Eval(Container, "DataItem.NAME") %>'></anthem:Label>
                                                        </ItemTemplate>
                                                    </asp:TemplateColumn>
                                                      <asp:TemplateColumn HeaderText="STATUS" Visible ="true" >
                                                        <HeaderStyle HorizontalAlign="Left" Width="70px" CssClass="grid-header" VerticalAlign="Middle"></HeaderStyle><ItemStyle CssClass="grid-item" Width="80px"></ItemStyle><ItemTemplate>
                                                            <anthem:Label id="lbSTATUS_NAME" runat="server" Width="70px" Text='<%# DataBinder.Eval(Container, "DataItem.STATUS_NAME") %>'></anthem:Label>
                                                        </ItemTemplate>
                                                    </asp:TemplateColumn>
                                                    <asp:TemplateColumn HeaderText="START DATE"   Visible ="true" >
                                                        <HeaderStyle HorizontalAlign="Left"  Wrap="true" Width="80px" CssClass="grid-header" VerticalAlign="Middle"></HeaderStyle><ItemStyle CssClass="grid-item" Width="80px"></ItemStyle><ItemTemplate>
                                                            <anthem:Label id="lbSTART_DATE" runat="server" Width="80px" Text='<%# DataBinder.Eval(Container, "DataItem.START_DATE") %>'></anthem:Label>
                                                        </ItemTemplate>
                                                    </asp:TemplateColumn>
                                                    <asp:TemplateColumn HeaderText="H GOAL" Visible ="true" >
                                                        <HeaderStyle HorizontalAlign="Left" Wrap="true" Width="30px" CssClass="grid-header" VerticalAlign="Middle"></HeaderStyle><ItemStyle CssClass="grid-item" Width="80px"></ItemStyle><ItemTemplate>
                                                            <anthem:Label id="lbH_GOAL" runat="server" Width="30px" Text='<%# DataBinder.Eval(Container, "DataItem.H_GOAL") %>'></anthem:Label>
                                                        </ItemTemplate>
                                                    </asp:TemplateColumn>
                                                    <asp:TemplateColumn HeaderText="G GOAL" Visible ="true" >
                                                        <HeaderStyle HorizontalAlign="Left" Wrap="true" Width="30px" CssClass="grid-header" VerticalAlign="Middle"></HeaderStyle><ItemStyle CssClass="grid-item" Width="80px"></ItemStyle><ItemTemplate>
                                                            <anthem:Label id="lbG_GOAL" runat="server" Width="30px" Text='<%# DataBinder.Eval(Container, "DataItem.G_GOAL") %>'></anthem:Label>
                                                        </ItemTemplate>
                                                    </asp:TemplateColumn> 
                                                    <asp:TemplateColumn HeaderText="H YELLOW">
                                                        <HeaderStyle Wrap="true" HorizontalAlign="Left" Width="30px" CssClass="grid-header" VerticalAlign="Middle"></HeaderStyle><ItemStyle CssClass="grid-item"></ItemStyle><ItemTemplate>
                                                            <anthem:Label id="lbH_YELLOW" runat="server" Width="30px" Text='<%# DataBinder.Eval(Container, "DataItem.H_YELLOW") %>'></anthem:Label>
                                                        </ItemTemplate>
                                                    </asp:TemplateColumn><asp:TemplateColumn HeaderText="G YELLOW">
                                                        <HeaderStyle Wrap="true" HorizontalAlign="Left" Width="30px" CssClass="grid-header" VerticalAlign="Middle"></HeaderStyle><ItemStyle CssClass="grid-item"></ItemStyle><ItemTemplate>
                                                            <anthem:Label id="lbG_YELLOW" runat="server" Width="30px" Text='<%# DataBinder.Eval(Container, "DataItem.G_YELLOW") %>'></anthem:Label>
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
                                                        <HeaderStyle HorizontalAlign="Left" Wrap="true" Width="25px" CssClass="grid-header" VerticalAlign="Middle"></HeaderStyle><ItemStyle CssClass="grid-item" Width="80px"></ItemStyle><ItemTemplate>
                                                            <anthem:Label id="lbH_RED" runat="server" Width="25px" Text='<%# DataBinder.Eval(Container, "DataItem.H_RED") %>'></anthem:Label>
                                                        </ItemTemplate>
                                                    </asp:TemplateColumn>
                                                    <asp:TemplateColumn HeaderText="G RED" Visible ="true" >
                                                        <HeaderStyle HorizontalAlign="Left" Wrap="true" Width="25px" CssClass="grid-header" VerticalAlign="Middle"></HeaderStyle><ItemStyle CssClass="grid-item" Width="80px"></ItemStyle><ItemTemplate>
                                                            <anthem:Label id="lbG_RED" runat="server" Width="25px" Text='<%# DataBinder.Eval(Container, "DataItem.G_RED") %>'></anthem:Label>
                                                        </ItemTemplate>
                                                    </asp:TemplateColumn>
                                                    <asp:TemplateColumn HeaderText="JC HOST" Visible ="true" >
                                                        <HeaderStyle HorizontalAlign="Left" Wrap="true" Width="120px" CssClass="grid-header" VerticalAlign="Middle"></HeaderStyle><ItemStyle CssClass="grid-item" Width="80px"></ItemStyle><ItemTemplate>
                                                            <anthem:Label id="lbHKJCHOSTNAME" runat="server" Width="120px" Text='<%# DataBinder.Eval(Container, "DataItem.HKJCHOSTNAME") %>'></anthem:Label>
                                                        </ItemTemplate>
                                                    </asp:TemplateColumn> 
                                                    <asp:TemplateColumn HeaderText="JC GUEST">
                                                       <HeaderStyle Wrap="true" HorizontalAlign="Left"  Width="120px" CssClass="grid-header" VerticalAlign="Middle"></HeaderStyle><ItemStyle CssClass="grid-item"></ItemStyle><ItemTemplate>
                                                            <anthem:Label id="lbHKJCGUESTNAME" runat="server" Width="120px" Text='<%# DataBinder.Eval(Container, "DataItem.HKJCGUESTNAME") %>'></anthem:Label>
                                                        </ItemTemplate>
                                                    </asp:TemplateColumn><asp:TemplateColumn HeaderText="DAY CODE">
                                                        <HeaderStyle Wrap="true" HorizontalAlign="Left" Width="60px" CssClass="grid-header" VerticalAlign="Middle"></HeaderStyle><ItemStyle CssClass="grid-item"></ItemStyle><ItemTemplate>
                                                            <anthem:Label id="lbHKJCDAYCODE" runat="server" Width="60px" Text='<%# DataBinder.Eval(Container, "DataItem.HKJCDAYCODE") %>'></anthem:Label>
                                                        </ItemTemplate>
                                                        <EditItemTemplate>  
                                                            <anthem:DropDownList ID="dplDayCode"  Width="60px" runat="server"> 
                                                                <asp:ListItem>MON</asp:ListItem> 
                                                                <asp:ListItem>TUE</asp:ListItem>
                                                                <asp:ListItem>WED</asp:ListItem>
                                                                <asp:ListItem>THU</asp:ListItem>
                                                                <asp:ListItem>FRI</asp:ListItem> 
                                                                <asp:ListItem>SAT</asp:ListItem>
                                                                <asp:ListItem>SUN</asp:ListItem> 
                                                                </anthem:DropDownList> 
                                                            <anthem:Label Visible="false" id="lbDAYCODE" runat="server" Width="70px" Text='<%# DataBinder.Eval(Container, "DataItem.HKJCDAYCODE") %>'></anthem:Label>
                                                        </EditItemTemplate> 
                                                    </asp:TemplateColumn>
                                                    <asp:TemplateColumn HeaderText="MATCH NO">
                                                        <HeaderStyle Wrap="true" HorizontalAlign="Left" Width="70px" CssClass="grid-header" VerticalAlign="Middle"></HeaderStyle><ItemStyle CssClass="grid-item"></ItemStyle><ItemTemplate>
                                                            <anthem:Label id="lbHKJCMATCHNO" runat="server" Width="70px" Text='<%# DataBinder.Eval(Container, "DataItem.HKJCMATCHNO") %>'></anthem:Label>
                                                        </ItemTemplate>
                                                        <EditItemTemplate>
                                                             <anthem:TextBox id="txtMATCHNO"    runat="server" Width="70px" Text='<%# DataBinder.Eval(Container, "DataItem.HKJCMATCHNO") %>'>
                                                            </anthem:TextBox>
                                                            <anthem:Label id="lbMATCHNO" Visible="false"  runat="server" Width="70px" Text='<%# DataBinder.Eval(Container, "DataItem.HKJCMATCHNO") %>'></anthem:Label>
                                                        </EditItemTemplate> 

                                                    </asp:TemplateColumn>  
   

                                                 <asp:TemplateColumn HeaderText="DateTime">
                                                        <HeaderStyle HorizontalAlign="Left" Width="80px" CssClass="grid-header" VerticalAlign="Middle"></HeaderStyle><ItemStyle CssClass="grid-item" Width="120px"></ItemStyle><ItemTemplate>
                                                            <anthem:Label id="lbCTIMESTAMP" runat="server" Width="80px" Text='<%# DataBinder.Eval(Container, "DataItem.CMATCHDATETIME ") %>'></anthem:Label>
                                                        </ItemTemplate>
                                                    </asp:TemplateColumn>
                                                    
                                                       <asp:TemplateColumn HeaderText="Booked">
                                                        <HeaderStyle Wrap="false" HorizontalAlign="Left" Width="40px" CssClass="grid-header" VerticalAlign="Middle"></HeaderStyle><ItemStyle CssClass="grid-item"></ItemStyle><ItemTemplate>
                                                            <anthem:Label id="lbBooked" runat="server" Width="40px" Text='<%# DataBinder.Eval(Container, "DataItem.booked") %>'></anthem:Label>
                                                        </ItemTemplate>
                                                         </asp:TemplateColumn>
                                                      <asp:TemplateColumn HeaderText="Detail">
                                                        <HeaderStyle Wrap="false" HorizontalAlign="Left" Width="50px" CssClass="grid-header" VerticalAlign="Middle"></HeaderStyle><ItemStyle CssClass="grid-item"></ItemStyle><ItemTemplate>
                                                          <a href=# onclick="window.open('MatchDetails.aspx?Type=SCOUT&ID=<%# DataBinder.Eval(Container, "DataItem.ID") %>','','scrollbars=yes, resizable=yes, Width=900,height=400, top=200,left=200')" >																	
																		<b>Detail</b>	</a></ItemTemplate>
                                                         </asp:TemplateColumn>
                                                     <asp:TemplateColumn HeaderText="Team Mapping">
                                                        <HeaderStyle Wrap="true" HorizontalAlign="Left" Width="40px" CssClass="grid-header" VerticalAlign="Middle"></HeaderStyle><ItemStyle CssClass="grid-item"></ItemStyle><ItemTemplate>
                                                          <a href=# onclick="window.open('TeamMapping.aspx?id=<%# DataBinder.Eval(Container, "DataItem.ID") %>','','scrollbars=yes, resizable=yes, Width=650,height=280, top=200,left=200')" >																	
																		<b><%# DataBinder.Eval(Container, "DataItem.MAPPING_STATUS")==DBNull.Value? "Mapping" :Convert.ToBoolean(DataBinder.Eval(Container, "DataItem.MAPPING_STATUS"))? "Mapped":"Mapping"  %> </b>	</a></ItemTemplate>
                                                         </asp:TemplateColumn>
<ASP:EDITCOMMANDCOLUMN EditText="Edit" CancelText="Cancel" UpdateText="Update" ItemStyle-Font-Bold="True" ButtonType="LinkButton">
                                                        <HEADERSTYLE Width="150px"  Wrap="false" VerticalAlign="Middle" CssClass="grid-header" HorizontalAlign="Left"></HEADERSTYLE><ITEMSTYLE Width="120px" CssClass="grid-item"></ITEMSTYLE>
                                                    </ASP:EDITCOMMANDCOLUMN><asp:ButtonColumn Text="Cancel"  CommandName="Delete" ItemStyle-Font-Bold="True">
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