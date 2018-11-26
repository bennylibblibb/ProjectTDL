<%@ Import namespace="JC_SoccerWeb.BLL"%>
<%@ Import namespace="JC_SoccerWeb"%>
<%@ Import namespace="JC_SoccerWeb.UserControl"%>
<%@ Control Language="c#" AutoEventWireup="false" Codebehind="GroupTabs.ascx.cs" Inherits="JC_SoccerWeb.UserControl.GroupTabs" TargetSchema="http://schemas.microsoft.com/intellisense/ie5"%>
<%@ Register TagPrefix="anthem" Namespace="Anthem" Assembly="Anthem" %>
<table cellSpacing="0" cellPadding="0" width="100%" border="0">
	<tr>
		<td vAlign="bottom"><anthem:datalist id="tabs" RepeatDirection="Horizontal" EnableViewState="False" runat="server" ItemStyle-CssClass="admin-tab-inactive"
				SelectedItemStyle-CssClass="admin-tab-active">
				<ItemTemplate>
					<asp:HyperLink id="hlGroup"   NavigateUrl="<%# ((TabItem) Container.DataItem).Path %>" runat="server" >
						<%# ((TabItem) Container.DataItem).Name %>
					</asp:HyperLink>
				</ItemTemplate>
				<SelectedItemStyle CssClass="admin-tab-active"></SelectedItemStyle>
				<ItemStyle Height="36px" CssClass="admin-tab-inactive"></ItemStyle>
				<SelectedItemTemplate>
					<%# ((TabItem) Container.DataItem).Name %>
				</SelectedItemTemplate>
			</anthem:datalist></td>
	</tr>
</table>
