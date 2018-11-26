<%@ Register TagPrefix="anthem" Namespace="Anthem" Assembly="Anthem" %>
<%@ Import namespace="JC_SoccerWeb.UserControl"%>
<%@ Import namespace="JC_SoccerWeb"%>
<%@ Control Language="c#" AutoEventWireup="false" Codebehind="UserTabs.ascx.cs" Inherits="JC_SoccerWeb.UserControl.UserTabs" TargetSchema="http://schemas.microsoft.com/intellisense/ie5"%>
<table cellSpacing="0" cellPadding="0" width="100%" border="0">
	<tr>
		<td vAlign="bottom"><anthem:DataList id="tabs" SelectedItemStyle-CssClass="admin-tab-active" ItemStyle-CssClass="admin-tab-inactive"
				runat="server" EnableViewState="False" RepeatDirection="Horizontal">
				<SelectedItemStyle CssClass="admin-tab-active"></SelectedItemStyle>
				<SelectedItemTemplate>
					<%# ((TabItem) Container.DataItem).Name %>
				</SelectedItemTemplate>
				<ItemStyle Height="36px" CssClass="admin-tab-inactive"></ItemStyle>
				<ItemTemplate>
					<a href='<%= Global.GetApplicationPath(Request) %>/<%# ((TabItem) Container.DataItem).Path %>'>
						<%# ((TabItem) Container.DataItem).Name %>
					</a>
				</ItemTemplate>
			</anthem:DataList></td>
	</tr>
</table>
