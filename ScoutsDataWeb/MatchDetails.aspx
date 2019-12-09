<%@ Register TagPrefix="anthem" Namespace="Anthem" Assembly="Anthem" %>
<%@ Page language="c#" Codebehind="MatchDetails.aspx.cs" AutoEventWireup="false" Inherits="JC_SoccerWeb.MatchDetails" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD  runat="server">
		<TITLE  >Details</TITLE>
		<meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR">
		<meta content="C#" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
		 <LINK href="CentaSmsStyle.css" type="text/css" rel="stylesheet">
		<script type="text/javascript">   
		 
    function ClickLinkBtn(obj) {  
        return confirm('Are your sure dffd？');
    }  
		</script>
	</HEAD>
	<body >
		<FORM id="Form1" method="post" runat="server">
			<TABLE align="center">
				 <TR  >
					<TD align="left" colspan="1" style="height:10px"> </TD>
				</TR>
				<TR align="center"   >
					<TD align="left"    >
                         <anthem:Label ID="lbAction" runat="server"  Width="500px"  >  </anthem:Label> 
                         <anthem:Label ID="lbEventid" runat="server"  Visible ="false" >  </anthem:Label> 
                         <anthem:Label ID="lbHomeid" runat="server"  Visible ="false"  >  </anthem:Label> 
                         <anthem:Label ID="lbGuestid" runat="server"    Visible ="false" >  </anthem:Label> 
                          <%-- <hr/> --%>
                       <anthem:DataGrid id="eventDetails" runat="server"     AllowPaging="false" AutoGenerateColumns="false">
                            <PagerStyle Mode="NumericPages"></PagerStyle>
                           <HeaderStyle Font-Bold="True"    HorizontalAlign ="Center"  Wrap="false" Height="30px" CssClass="grid-header" ></HeaderStyle>
                           <ItemStyle Height="34px"  Wrap="false"    CssClass="grid-item"></ItemStyle>
                          <Columns>   
                              <asp:TemplateColumn HeaderText="Host" Visible ="true" >  <HeaderStyle  CssClass="grid-header" Width="120px" />                                     <ItemTemplate >  
                                  <%# DataBinder.Eval(Container, "DataItem.Name")==DBNull.Value? DataBinder.Eval(Container, "DataItem.HKJCHOSTNAME"):DataBinder.Eval(Container, "DataItem.Name").ToString().Substring(0,DataBinder.Eval(Container, "DataItem.Name").ToString().IndexOf(" - ")) %>
			   <BR/>	 <strong>  <%# DataBinder.Eval(Container, "DataItem.Name")==DBNull.Value?DataBinder.Eval(Container, "DataItem.HKJCHOSTNAME_CN"): DataBinder.Eval(Container, "DataItem.HTName") %>   <%-- <%# DataBinder.Eval(Container, "DataItem.HTName")==DataBinder.Eval(Container, "DataItem.HKJCHOSTNAME_CN")?DataBinder.Eval(Container, "DataItem.HTName"):( DataBinder.Eval(Container, "DataItem.HTName")==DataBinder.Eval(Container, "DataItem.HKJCGUESTNAME_CN"))?DataBinder.Eval(Container, "DataItem.HKJCHOSTNAME") :""  %>  --%> </strong>   
                                     </ItemTemplate> </asp:TemplateColumn>
                              <asp:TemplateColumn HeaderText="Guest"  >  <HeaderStyle  CssClass="grid-header" Width="120px" />
                           <ItemTemplate><%# DataBinder.Eval(Container, "DataItem.Name")==DBNull.Value?DataBinder.Eval(Container, "DataItem.HKJCGUESTNAME"):DataBinder.Eval(Container, "DataItem.Name").ToString().Substring(DataBinder.Eval(Container, "DataItem.Name").ToString().IndexOf(" - ")+3,DataBinder.Eval(Container, "DataItem.Name").ToString().Length-DataBinder.Eval(Container, "DataItem.Name").ToString().IndexOf(" - ")-3) %> <BR/>
                               <strong><%# DataBinder.Eval(Container, "DataItem.Name")==DBNull.Value?DataBinder.Eval(Container, "DataItem.HKJCGUESTNAME_CN"): DataBinder.Eval(Container, "DataItem.GTName") %> </strong>   
                                                          </ItemTemplate> </asp:TemplateColumn>
                              <asp:TemplateColumn HeaderText="StartDate">  <HeaderStyle  CssClass="grid-header"  Width="150px"  />  <ItemTemplate>
				      <%#DataBinder.Eval(Container, "DataItem.FHSD_19")==DBNull.Value?Convert.ToDateTime (DataBinder.Eval(Container, "DataItem.CMATCHDATETIME")).ToString("yyyy/MM/dd HH:mm:ss"): Convert.ToDateTime (DataBinder.Eval(Container, "DataItem.FHSD_19"))==DateTime.MinValue? Convert.ToDateTime (DataBinder.Eval(Container, "DataItem.start_date")).ToString("yyyy/MM/dd HH:mm:ss"):Convert.ToDateTime (DataBinder.Eval(Container, "DataItem.FHSD_19")).ToString("yyyy/MM/dd HH:mm:ss") %>
				        </ItemTemplate> </asp:TemplateColumn> 
                              <asp:TemplateColumn HeaderText="Result">  <HeaderStyle  CssClass="grid-header" Width="120px"/>  <ItemStyle  HorizontalAlign="Center"  CssClass="grid-item"></ItemStyle> <ItemTemplate>
                       <asp:TextBox ID="txtResult1"   runat="server" Width="30px" text='<%# DataBinder.Eval(Container, "DataItem.RESULT")==DBNull.Value?"":DataBinder.Eval(Container, "DataItem.RESULT").ToString().Substring(0,DataBinder.Eval(Container, "DataItem.RESULT").ToString().IndexOf(":")) %>'></asp:TextBox>  
				       :  <asp:TextBox ID="txtResult2"   runat="server" Width="30px" text='<%# DataBinder.Eval(Container, "DataItem.RESULT")==DBNull.Value?"":DataBinder.Eval(Container, "DataItem.RESULT").ToString().Substring(DataBinder.Eval(Container, "DataItem.RESULT").ToString().IndexOf(":")+1,DataBinder.Eval(Container, "DataItem.RESULT").ToString().Length-DataBinder.Eval(Container, "DataItem.RESULT").ToString().IndexOf(":")-1) %>'></asp:TextBox>  
				      </ItemTemplate> </asp:TemplateColumn>
                              <asp:TemplateColumn HeaderText="TimeOfGame">  <HeaderStyle  CssClass="grid-header" Width="100px" /> <ItemStyle  HorizontalAlign="Center"  CssClass="grid-item"/>  <ItemTemplate>
				      <asp:TextBox ID="txtElapsed"   runat="server" Width="60px"  text='<%#(DataBinder.Eval(Container, "DataItem.ELAPSED")).ToString()=="-1"?"":DataBinder.Eval(Container, "DataItem.ELAPSED") %>'></asp:TextBox>  
				   				    </ItemTemplate> </asp:TemplateColumn>
                              <asp:TemplateColumn HeaderText="Status">  <HeaderStyle  CssClass="grid-header" Width="100px"  />  <ItemTemplate>    
                             <asp:Label ID="lbGoalInfoStatus"   runat="server" Width="150px" text=' <%#DataBinder.Eval(Container, "DataItem.STATUS_NAME") %>'></asp:Label>  
                             <BR/> <asp:Label ID="lbGoalInfoStatus2"  Visible="false"  runat="server"  text='<%#DataBinder.Eval(Container, "DataItem.GAMESTATUS") %>'></asp:Label>  
                                   <asp:DropDownList ID="dplLeagues"   runat="server" AutoCallBack="True" Width="120px" >
                                                     <asp:ListItem Selected="True" Value="All">-Select-</asp:ListItem>  
                                                                <asp:ListItem Value="0">未</asp:ListItem> 
                                                                <asp:ListItem Value="1">上</asp:ListItem>
                                                                <asp:ListItem Value="2">休</asp:ListItem>
                                                                <asp:ListItem Value="3">下</asp:ListItem>
                                                                <asp:ListItem Value="4">完</asp:ListItem> 
                                                                <asp:ListItem Value="5">加</asp:ListItem>
                       <asp:ListItem Value="6">消</asp:ListItem>
                       <asp:ListItem Value="7">斬</asp:ListItem>
                       <asp:ListItem Value="8">改</asp:ListItem>
                       <asp:ListItem Value="9">延</asp:ListItem>
                       <asp:ListItem Value="10">待</asp:ListItem>
                       <asp:ListItem Value="11">没</asp:ListItem>
                               </asp:DropDownList> 
                           </ItemTemplate> </asp:TemplateColumn> 
                                <asp:TemplateColumn  HeaderText="Comments" Visible ="true" >  <HeaderStyle  Width="150px" CssClass="grid-header" /> <ItemStyle   VerticalAlign="Bottom"  CssClass="grid-item"/> 
                                     <ItemTemplate  > 
                                           <asp:TextBox ID="txtComments"   Visible="true" runat="server" Width="150px" text='<%#DataBinder.Eval(Container, "DataItem.CCOMMENTS") %>'></asp:TextBox> 
                                      </ItemTemplate> </asp:TemplateColumn>
                              <asp:TemplateColumn HeaderText="DayCode" >  <HeaderStyle  CssClass="grid-header" Width="80px" /><ItemStyle  HorizontalAlign="Center"  CssClass="grid-item"/>
                           <ItemTemplate>
				<%# DataBinder.Eval(Container, "DataItem.HKJCDAYCODE") %>
				</ItemTemplate> </asp:TemplateColumn>
                              <asp:TemplateColumn HeaderText="MatchNo">  <HeaderStyle  CssClass="grid-header" Width="80px"/><ItemStyle  HorizontalAlign="Center"  CssClass="grid-item"/>
                           <ItemTemplate>
				<%# DataBinder.Eval(Container, "DataItem.HKJCMATCHNO") %>
				</ItemTemplate>  </asp:TemplateColumn> 
		                 </Columns>	
                       </anthem:DataGrid> 
                        <p></p>
                           <div align="right"> 
                           <anthem:CheckBox id="chkToLive"   Font-Bold="true"    AutoPostBack ="true" runat="server" Text="To Live" Width="100px"  Enabled="true" Visible="true"> </anthem:CheckBox>&nbsp;&nbsp;
                           <asp:CheckBox id="chkAlert" runat="server" Text="Alert"  Width="100px"  Enabled="true" Visible="true"> </asp:CheckBox>&nbsp;&nbsp;
                               <strong>  <anthem:Label ID="lbMsgResult" runat="server" ForeColor="Red" Width="80px"  >  </anthem:Label>  </strong>&nbsp;&nbsp;
                           <anthem:Button  ID="btnSavetoLive" runat="server" Text="Send to LiveGoal" Visible="true" /> 
                         </div> 
                        <hr />
                       <anthem:DataGrid id="totalDetails" runat="server" Width="300px"   AllowPaging="false" AutoGenerateColumns="FALSE">
                            <PagerStyle Mode="NumericPages"></PagerStyle>
                           <HeaderStyle Font-Bold="True"    HorizontalAlign ="Center" Wrap="false" Height="34px" CssClass="grid-header" ></HeaderStyle>
                           <ItemStyle Height="30px"    Wrap="false" CssClass="grid-item"></ItemStyle>
                          <Columns>   
                              <asp:TemplateColumn  Visible ="true" >  <HeaderStyle  Width="160px" CssClass="grid-header" />
                                     <ItemTemplate  > 
                                           <asp:Label ID="lbType"   Visible="true" runat="server" Width="120px" text='<%#DataBinder.Eval(Container, "DataItem.Type") %>'></asp:Label> 
                                      </ItemTemplate> </asp:TemplateColumn>
                              <asp:TemplateColumn   Visible ="true" >  <HeaderStyle Width="70px" CssClass="grid-header" />
                                     <ItemTemplate >
                                             <asp:TextBox ID="txtHValue"  Enabled="true" runat="server" Width="80px" text='<%#DataBinder.Eval(Container, "DataItem.H") %>'></asp:TextBox>
                                               <asp:Label ID="lbHValue"   Visible="false" runat="server" Width="80px" text='<%#DataBinder.Eval(Container, "DataItem.H") %>'></asp:Label>     
                                                                       </ItemTemplate> </asp:TemplateColumn>
                              <asp:TemplateColumn   Visible ="true" >  <HeaderStyle Width="70px"  CssClass="grid-header" />
                                     <ItemTemplate >
                                         <asp:TextBox ID="txtGValue"  Enabled="true" runat="server" Width="80px" text='<%#DataBinder.Eval(Container, "DataItem.G") %>'></asp:TextBox>
                                          <asp:Label ID="lbGValue"   Visible="false" runat="server" Width="80px" text='<%#DataBinder.Eval(Container, "DataItem.G") %>'></asp:Label>     
                                         </ItemTemplate> </asp:TemplateColumn>

                         <%--  <asp:TemplateColumn HeaderText="Shots"  >  <HeaderStyle  CssClass="grid-header" />
                           <ItemTemplate>    <%#DataBinder.Eval(Container, "DataItem.CSHOTS") %> 
                                                          </ItemTemplate> </asp:TemplateColumn>
                            <asp:TemplateColumn HeaderText="Fouls">  <HeaderStyle  CssClass="grid-header" />
                           <ItemTemplate>  <%#DataBinder.Eval(Container, "DataItem.CFOULS") %>  
                           </ItemTemplate> </asp:TemplateColumn>
                            <asp:TemplateColumn HeaderText="Corner">  <HeaderStyle  CssClass="grid-header" />
                           <ItemTemplate>
				<%# DataBinder.Eval(Container, "DataItem.CCORNER_KICKS") %>
				</ItemTemplate> </asp:TemplateColumn>
                 <asp:TemplateColumn HeaderText="Offside" >  <HeaderStyle  CssClass="grid-header" />
                 <ItemTemplate>
				<%# DataBinder.Eval(Container, "DataItem.COFFSIDES") %>
				</ItemTemplate> 
                 </asp:TemplateColumn>  
                                <asp:TemplateColumn HeaderText="Yellow Card" >  <HeaderStyle  CssClass="grid-header" />
                 <ItemTemplate>
				<%# DataBinder.Eval(Container, "DataItem.CYELLOW_CARDS") %>
				</ItemTemplate> 
                 </asp:TemplateColumn> 
                                <asp:TemplateColumn HeaderText="Red Card" >  <HeaderStyle  CssClass="grid-header" />
                 <ItemTemplate>
				<%# DataBinder.Eval(Container, "DataItem.CRED_CARDS") %>
				</ItemTemplate> 
                 </asp:TemplateColumn> 
                                <asp:TemplateColumn HeaderText="Attacks" >  <HeaderStyle  CssClass="grid-header" />
                 <ItemTemplate>
				<%# DataBinder.Eval(Container, "DataItem.CATTACKS") %>
				</ItemTemplate> 
                 </asp:TemplateColumn> 
                                <asp:TemplateColumn HeaderText="Substitution" >  <HeaderStyle  CssClass="grid-header" />
                 <ItemTemplate>
				<%# DataBinder.Eval(Container, "DataItem.CSUBSTITUTIONS") %>
				</ItemTemplate> 
                 </asp:TemplateColumn> 
                                <asp:TemplateColumn HeaderText="Throw-ins" >  <HeaderStyle  CssClass="grid-header" />
                 <ItemTemplate>
				<%# DataBinder.Eval(Container, "DataItem.CTHROWINS") %>
				</ItemTemplate> 
                 </asp:TemplateColumn> 
                                <asp:TemplateColumn HeaderText="Goal Kicks" >  <HeaderStyle  CssClass="grid-header" />
                 <ItemTemplate>
				<%# DataBinder.Eval(Container, "DataItem.CGOALKICKS") %>
				</ItemTemplate> 
                 </asp:TemplateColumn> 
                 <asp:TemplateColumn HeaderText="Possession" >  <HeaderStyle  CssClass="grid-header" />
                 <ItemTemplate>
				<%# DataBinder.Eval(Container, "DataItem.CPOSSESSION") %>
				</ItemTemplate> 
                 </asp:TemplateColumn> --%>
		</Columns> 
                         </anthem:DataGrid>
					</TD>
                    <%--<td align="left" valign="bottom">  </td>--%>
				</TR> 
                <tr style="height:10px"> <td  colspan="1" >  </td></tr>
				 <TR >
					<TD colspan="1"  >
                     <anthem:DataGrid id="dgGoalInfo" runat="server" Width="100%"    AllowPaging="false" AutoGenerateColumns="false">  
                           <HeaderStyle Font-Bold="True"    HorizontalAlign ="Center" Wrap="false" Height="30px" CssClass="grid-header" ></HeaderStyle>
                           <ItemStyle Height="34px"    Wrap="false" CssClass="grid-item"></ItemStyle>
                          <Columns>   
                              <asp:TemplateColumn HeaderText="H/G" HeaderStyle-Width="80px"   >  <HeaderStyle  CssClass="grid-header" />
                                     <ItemTemplate>                             
                              <asp:DropDownList ID="dplHG"  Enabled="true"    runat="server" Width="60px" > 
                             
                              <asp:ListItem Value="H">H</asp:ListItem>
                              <asp:ListItem Value="G">G</asp:ListItem>
                              </asp:DropDownList> 
                             <asp:label id ="lbHG" runat="server" Visible ="false"   Text='<%# DataBinder.Eval(Container, "DataItem.hg") %>'> </asp:label> 
			                 </ItemTemplate> </asp:TemplateColumn>
                               <asp:TemplateColumn Visible ="false"   HeaderText="ID"  >
                           <ItemTemplate>
                             <asp:label id ="lbEventid" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.EMATCHID") %>'> </asp:label>
                               <asp:label id ="lbIncid" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.INCIDENTS_ID") %>'> </asp:label>
			 				</ItemTemplate>
                                                    </asp:TemplateColumn>
                               <asp:TemplateColumn  Visible ="false"  HeaderText="Team"  >
                           <ItemTemplate>
                             <asp:label id ="lbTeamid" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.team_id") %>'> </asp:label>
			 				</ItemTemplate>
                                                    </asp:TemplateColumn>
                               <asp:TemplateColumn HeaderText="Type" HeaderStyle-Width="60px" >
                                <HeaderStyle  CssClass="grid-header" /> <ItemTemplate>
                                     <asp:label id ="lbType" runat="server" Visible="false" Text=' <%# DataBinder.Eval(Container, "DataItem.CTYPE") %>'> </asp:label> 
                                    <asp:DropDownList ID="dplType"   runat="server"  Width="80px" >
                                   
                              <asp:ListItem >goal</asp:ListItem>
                              <asp:ListItem >rcard</asp:ListItem>
                              <asp:ListItem >ycard</asp:ListItem>
                              </asp:DropDownList>  	 
				                </ItemTemplate> </asp:TemplateColumn>
                               <asp:TemplateColumn  Visible ="false" HeaderText="Player ID">  <HeaderStyle  CssClass="grid-header" />
                           <ItemTemplate>
                                  <asp:label id ="lbPlayerid" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.PARTICIPANTID") %>'> </asp:label>
			 				 </ItemTemplate> </asp:TemplateColumn>
                     <asp:TemplateColumn HeaderStyle-Width="80px"  HeaderText="NO">  <HeaderStyle  CssClass="grid-header" /> <ItemTemplate>  
                     <asp:label id ="lbScoreNo" runat="server" Visible="false" Text='<%# DataBinder.Eval(Container, "DataItem.IPLAYER_NO") %>'> </asp:label> 
                     <asp:TextBox id ="txtScoreNo" Width="60px" runat="server" Enabled="true" Text='<%# DataBinder.Eval(Container, "DataItem.IPLAYER_NO") %>'> </asp:TextBox> 
				    </ItemTemplate>  </asp:TemplateColumn> 
                               <asp:TemplateColumn HeaderStyle-Width="100px"  HeaderText="Player Name">  <HeaderStyle  CssClass="grid-header" /> <ItemStyle CssClass="grid-item" Width="180px"></ItemStyle> 
                           <ItemTemplate>
                           <asp:label id ="lbPlayerName" Width="150px" runat="server" Visible="false" Text='<%# DataBinder.Eval(Container, "DataItem.player") %>'> </asp:label> 
			               <asp:TextBox ID="txtPlayerName" MaxLength="100"   runat="server" Width="180px" Text='<%# DataBinder.Eval(Container, "DataItem.player") %>' > </asp:TextBox>
                               </ItemTemplate> </asp:TemplateColumn>
                               <asp:TemplateColumn HeaderStyle-Width="100px"   HeaderText="CN Name" >  <HeaderStyle  CssClass="grid-header" /> <ItemStyle CssClass="grid-item" Width="180px"></ItemStyle>  <ItemTemplate>
                             <asp:TextBox ID="txtCNName" MaxLength="100"  Enabled="true" runat="server" Width="180px" Text='<%# DataBinder.Eval(Container, "DataItem.PLAYERCHI") %>' > </asp:TextBox>
				         <asp:label id ="lbCNName" Visible ="false"  runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.PLAYERCHI") %>'> </asp:label>
                             </ItemTemplate> </asp:TemplateColumn> 
                             <asp:TemplateColumn  HeaderStyle-Width="100px"   HeaderText="Result">  <HeaderStyle  CssClass="grid-header" Width="120px"/>  <ItemStyle  HorizontalAlign="Center"  CssClass="grid-item"></ItemStyle> <ItemTemplate>
                             <asp:TextBox ID="txtResult1"  Enabled="true" runat="server" Width="30px" text='<%# DataBinder.Eval(Container, "DataItem.resultH") %>'></asp:TextBox>  : 
                             <asp:TextBox ID="txtResult2"  Enabled="true" runat="server" Width="30px" text='<%# DataBinder.Eval(Container, "DataItem.resultG") %>'></asp:TextBox>  
				            </ItemTemplate> </asp:TemplateColumn>
                              <asp:TemplateColumn   HeaderStyle-Width="100px" HeaderText="At" >  <HeaderStyle  CssClass="grid-header" /> <ItemTemplate>
                              <asp:DropDownList ID="dplAt" Enabled="true" runat="server"  Width="80px" >
                               
                              <asp:ListItem Value="00">1st half</asp:ListItem>
                              <asp:ListItem Value="01">2nd half</asp:ListItem>
                              </asp:DropDownList>  	   
                                  <asp:label id ="lbMatchStatus" runat="server"  Visible="false"  Text='<%# DataBinder.Eval(Container, "DataItem.MatchStatus") %>'> </asp:label> 
			                  </ItemTemplate> </asp:TemplateColumn>
                          <asp:TemplateColumn  HeaderStyle-Width="80px" HeaderText="Elapsed">  <HeaderStyle  CssClass="grid-header" /> <ItemTemplate> 
		                  <asp:label id ="lbELAPSED" runat="server"  Visible="false"  Text='<%# DataBinder.Eval(Container, "DataItem.ELAPSED") %>'> </asp:label> 
                          <asp:TextBox id ="txtElapsed" runat="server" Width="60px" Enabled="true" Text='<%# DataBinder.Eval(Container, "DataItem.ELAPSED") %>'> </asp:TextBox> 
				          </ItemTemplate>  </asp:TemplateColumn> 
                           <asp:TemplateColumn HeaderText="Own Goal">  <HeaderStyle  CssClass="grid-header" /> <ItemTemplate> 
			               	 <asp:CheckBox id="chkOwnGoal" runat="server"  Enabled="true"></asp:CheckBox>
			               </ItemTemplate>  </asp:TemplateColumn> 
                          <asp:TemplateColumn  Visible ="false" HeaderText="TIMESTAMP">  <HeaderStyle  CssClass="grid-header" /> <ItemTemplate> 
			               	<%# DataBinder.Eval(Container, "DataItem.LASTTIME") %>
			               </ItemTemplate>  </asp:TemplateColumn> 
                          </Columns>	</anthem:DataGrid>
					</TD>
				</TR>
                <tr style="height:40px" align="right" valign="bottom" >
                    <td>   <strong>  <anthem:Label ID="lbMsg" runat="server" ForeColor="Red" Width="500px"  >  </anthem:Label>  </strong>
                    <anthem:Button  ID="btnAddRow" runat="server" Text="添加臨時行" Width="80px"/>  &nbsp;&nbsp;
                    <anthem:Button  ID="btnSave" runat="server" Text="發送" Width="80px"/>  &nbsp;&nbsp;
			     	<anthem:Button  ID="btnLiveEdit" runat="server" Text="Save" Width="80px"/>&nbsp;&nbsp;
                    <anthem:Button  ID="btnCancel" runat="server" Text="清除發送" Width="80px"/> 
                    </td>
                    <%--<td>   </td>--%> 
                </tr>
				 <tr> <td align="right" colspan="1" ><hr /> </td></tr>
                 <tr style="height:10px" ><td colspan="1" > </td></tr>
			</TABLE>
		</FORM>
	</body>
</HTML>
