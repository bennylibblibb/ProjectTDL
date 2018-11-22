<%@ Register TagPrefix="anthem" Namespace="Anthem" Assembly="Anthem" %>
<%@ Control Language="c#" AutoEventWireup="false" Codebehind="MenuTabs.ascx.cs" Inherits="JC_SoccerWeb.UserControl.MenuTabs" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
<%@ Import namespace="JC_SoccerWeb.UserControl"%>
<%@ Import namespace="JC_SoccerWeb"%>
<table  cellSpacing="0" cellPadding="0"   border="0">
	<tr>
		<td  ><anthem:datalist id="tabs" SelectedItemStyle-CssClass="admin-tab-active" ItemStyle-CssClass="admin-tab-inactive"
				runat="server" EnableViewState="False"  RepeatDirection="Horizontal">
				<SelectedItemStyle CssClass="admin-tab-active"></SelectedItemStyle>
				<SelectedItemTemplate>
					<%# ((TabItem) Container.DataItem).Name %>
				</SelectedItemTemplate>
				<ItemStyle Height="36px" CssClass="admin-tab-inactive"></ItemStyle>
				<ItemTemplate>
					<asp:HyperLink id="hlMenu"   NavigateUrl="<%# ((TabItem) Container.DataItem).Path %>" runat="server" >
						<%# ((TabItem) Container.DataItem).Name %>
					</asp:HyperLink>
				</ItemTemplate>
			</anthem:datalist></td>
	</tr>
</table>
