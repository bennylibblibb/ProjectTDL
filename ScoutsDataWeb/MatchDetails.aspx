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
		 
		</script>
	</HEAD>
	<body >
		<FORM id="Form1" method="post" runat="server">
			<TABLE align="center">
				 <TR>
					<TD align="left" style="height:30px"> </TD>
				</TR>
				<TR align="center">
					<TD align="left">
                       <anthem:Label ID="lbAction" runat="server"  Width="500px"  >  </anthem:Label> 
                       <anthem:DataGrid id="eventDetails" runat="server" Width="800px"   AllowPaging="false" AutoGenerateColumns="false">
                            <PagerStyle Mode="NumericPages"></PagerStyle>
                           <HeaderStyle Font-Bold="True"    HorizontalAlign ="Center"  Wrap="false" Height="30px" CssClass="grid-header" ></HeaderStyle>
                           <ItemStyle Height="34px"  Wrap="false"  CssClass="grid-item"></ItemStyle>
                          <Columns>   
                              <asp:TemplateColumn HeaderText="Host" Visible ="true" >  <HeaderStyle  CssClass="grid-header" />
                                     <ItemTemplate >
                                     <%# DataBinder.Eval(Container, "DataItem.HKJCDAYCODE")==DBNull.Value?DataBinder.Eval(Container, "DataItem.HNAME"):DataBinder.Eval(Container, "DataItem.HKJCHOSTNAME")   %>
			   <BR/>	 <strong>   <%#DataBinder.Eval(Container, "DataItem.HmNAMECN") %> </strong>   
                                     </ItemTemplate> </asp:TemplateColumn>
                                                    <asp:TemplateColumn HeaderText="Guest"  >  <HeaderStyle  CssClass="grid-header" />
                           <ItemTemplate>
				                            <%# DataBinder.Eval(Container, "DataItem.HKJCDAYCODE")==DBNull.Value?DataBinder.Eval(Container, "DataItem.GNAME"):DataBinder.Eval(Container, "DataItem.HKJCGUESTNAME")   %>
			   <BR/>	 <strong>    <%#DataBinder.Eval(Container, "DataItem.GmNAMECN") %></strong>   
                                                          </ItemTemplate> </asp:TemplateColumn>
                                                                                                   <asp:TemplateColumn HeaderText="START DATE">  <HeaderStyle  CssClass="grid-header" />
                           <ItemTemplate>
				<%#DataBinder.Eval(Container, "DataItem.FHSD_19")==DBNull.Value?"": Convert.ToDateTime (DataBinder.Eval(Container, "DataItem.FHSD_19"))==DateTime.MinValue?"":DataBinder.Eval(Container, "DataItem.FHSD_19") %>
				  <BR/> <%#DataBinder.Eval(Container, "DataItem.STATUS_NAME") %>  
                           </ItemTemplate> </asp:TemplateColumn>
                                                    <asp:TemplateColumn HeaderText="RESULT">  <HeaderStyle  CssClass="grid-header" />
                           <ItemTemplate>
				<%# DataBinder.Eval(Container, "DataItem.RESULT") %>
				</ItemTemplate> </asp:TemplateColumn>
                                                    <asp:TemplateColumn HeaderText="DAYCODE" >  <HeaderStyle  CssClass="grid-header" />
                           <ItemTemplate>
				<%# DataBinder.Eval(Container, "DataItem.HKJCDAYCODE") %>
				</ItemTemplate> </asp:TemplateColumn>
                                                    <asp:TemplateColumn HeaderText="MATCHNO">  <HeaderStyle  CssClass="grid-header" />
                           <ItemTemplate>
				<%# DataBinder.Eval(Container, "DataItem.HKJCMATCHNO") %>
				</ItemTemplate>  </asp:TemplateColumn> 
		</Columns>	</anthem:DataGrid>
                         <anthem:DataGrid id="totalDetails" runat="server" Width="300px"   AllowPaging="false" AutoGenerateColumns="FALSE">
                            <PagerStyle Mode="NumericPages"></PagerStyle>
                           <HeaderStyle Font-Bold="True"    HorizontalAlign ="Center" Wrap="false" Height="34px" CssClass="grid-header" ></HeaderStyle>
                           <ItemStyle Height="30px"    Wrap="false" CssClass="grid-item"></ItemStyle>
                          <Columns>   
                              <asp:TemplateColumn  Visible ="true" >  <HeaderStyle  Width="160px" CssClass="grid-header" />
                                     <ItemTemplate  >
                                       <%#DataBinder.Eval(Container, "DataItem.Type") %> 
                                     </ItemTemplate> </asp:TemplateColumn>
                              <asp:TemplateColumn   Visible ="true" >  <HeaderStyle Width="70px" CssClass="grid-header" />
                                     <ItemTemplate >
                                       <%#DataBinder.Eval(Container, "DataItem.H") %> 
                                     </ItemTemplate> </asp:TemplateColumn>
                              <asp:TemplateColumn   Visible ="true" >  <HeaderStyle Width="70px"  CssClass="grid-header" />
                                     <ItemTemplate >
                                       <%#DataBinder.Eval(Container, "DataItem.G") %> 
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
				</TR>
                <tr style="height:10px"><td >  </td></tr>
                  <tr><td> <hr/></td></tr>
                   <tr style="height:05px"><td >  </td></tr>
				 <TR>
					<TD >
   <anthem:DataGrid id="dgGoalInfo" runat="server" Width="800px"   AllowPaging="false" AutoGenerateColumns="false">  
                           <HeaderStyle Font-Bold="True"    HorizontalAlign ="Center" Wrap="false" Height="30px" CssClass="grid-header" ></HeaderStyle>
                           <ItemStyle Height="34px"    Wrap="false" CssClass="grid-item"></ItemStyle>
                          <Columns>   	
                                <asp:TemplateColumn HeaderText="H/G" Visible ="true" >  <HeaderStyle  CssClass="grid-header" />
                                     <ItemTemplate>
				<%# DataBinder.Eval(Container, "DataItem.hg") %>
				</ItemTemplate> </asp:TemplateColumn>
                                                    <asp:TemplateColumn Visible ="false"   HeaderText="ID"  >
                           <ItemTemplate>
                             <asp:label id ="lbEventid" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.EMATCHID") %>'> </asp:label>
			 				</ItemTemplate>
                                                    </asp:TemplateColumn>
                               <asp:TemplateColumn  Visible ="false"  HeaderText="Team"  >
                           <ItemTemplate>
                             <asp:label id ="lbTeamid" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.team_id") %>'> </asp:label>
			 				</ItemTemplate>
                                                    </asp:TemplateColumn>
                                                    <asp:TemplateColumn HeaderText="Type">
                                                         <HeaderStyle  CssClass="grid-header" />
                           <ItemTemplate>
				<%# DataBinder.Eval(Container, "DataItem.CTYPE") %>
				</ItemTemplate> </asp:TemplateColumn>
                                                    <asp:TemplateColumn HeaderText="Player ID">  <HeaderStyle  CssClass="grid-header" />
                           <ItemTemplate>
                                  <asp:label id ="lbPlayerid" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.PARTICIPANTID") %>'> </asp:label>
			 				 </ItemTemplate> </asp:TemplateColumn>
                                                    <asp:TemplateColumn HeaderText="Player Name">  <HeaderStyle  CssClass="grid-header" />
                           <ItemTemplate>
 <asp:label id ="lbPlayerName" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.player") %>'> </asp:label> 
				</ItemTemplate> </asp:TemplateColumn>
                                                    <asp:TemplateColumn HeaderText="CN Name" >  <HeaderStyle  CssClass="grid-header" />
                         <ItemStyle CssClass="grid-item" Width="180px"></ItemStyle>  <ItemTemplate>
                             <asp:TextBox ID="txtCNName" MaxLength="100"  Enabled="false" runat="server" Width="180px" Text='<%# DataBinder.Eval(Container, "DataItem.PLAYERCHI") %>' > </asp:TextBox>
				         <asp:label id ="lbCNName" Visible ="false"  runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.PLAYERCHI") %>'> </asp:label>
                             </ItemTemplate> </asp:TemplateColumn>
                                                    <asp:TemplateColumn HeaderText="Elapsed">  <HeaderStyle  CssClass="grid-header" />
                           <ItemTemplate> 
				<%# DataBinder.Eval(Container, "DataItem.ELAPSED") %>
				</ItemTemplate>  </asp:TemplateColumn> 
                               <asp:TemplateColumn HeaderText="TIMESTAMP">  <HeaderStyle  CssClass="grid-header" />
                           <ItemTemplate> 
				<%# DataBinder.Eval(Container, "DataItem.LASTTIME") %>
				</ItemTemplate>  </asp:TemplateColumn> 
                          </Columns>	</anthem:DataGrid>
					</TD>
				</TR>
                <tr style="height:20px" align="right"><td>   <strong>  <anthem:Label ID="lbMsg" runat="server" ForeColor="Red" Width="500px"  >  </anthem:Label>  </strong></td></tr>
				 <tr><td align="right"> <anthem:Button  ID="btnSave" runat="server" Text="Edit" />  
				     </td></tr>
			</TABLE>
		</FORM>
	</body>
</HTML>
