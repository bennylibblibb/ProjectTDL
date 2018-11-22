<%@ Control Language="c#" AutoEventWireup="false" Codebehind="SendTabs.ascx.cs" Inherits="JC_SoccerWeb.UserControl.SendTabs" TargetSchema="http://schemas.microsoft.com/intellisense/ie5"%>
<%@ Import namespace="JC_SoccerWeb"%>
<%@ Import namespace="JC_SoccerWeb.UserControl"%>
<%@ Register TagPrefix="anthem" Namespace="Anthem" Assembly="Anthem" %>
<table style="display:none">
	<tr>
		<td ><anthem:DataList id="tabs" SelectedItemStyle-CssClass="admin-tab-active" ItemStyle-CssClass="admin-tab-inactive"
				runat="server" EnableViewState="false" >
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
