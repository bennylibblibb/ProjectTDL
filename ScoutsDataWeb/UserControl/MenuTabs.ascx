<%@ Register TagPrefix="anthem" Namespace="Anthem" Assembly="Anthem" %>
<%@ Control Language="c#" AutoEventWireup="false" Codebehind="MenuTabs.ascx.cs" Inherits="JC_SoccerWeb.UserControl.MenuTabs" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
<%@ Import namespace="JC_SoccerWeb.UserControl"%>
<%@ Import namespace="JC_SoccerWeb"%>
  <script type="text/javascript">
      function CheckUrl(obj) { 
        //   return true;
        if (obj == "HKJC") {
            var deoks = document.getElementById("deok");
         //   alert(deoks.value);
            if (deoks.value == 'a') {
                deoks.value = 'b';
             //    alert(deoks.value);
                return true;
            }
            else {
                return false;
            }
        }
    }
</script>

<table  cellSpacing="0" cellPadding="0"   border="0">
	<tr>
		<td  ><anthem:datalist id="tabs" SelectedItemStyle-CssClass="admin-tab-active"   ItemStyle-CssClass="admin-tab-inactive"
				runat="server" EnableViewState="False"  RepeatDirection="Horizontal">
				<SelectedItemStyle CssClass="admin-tab-active"></SelectedItemStyle>
				<SelectedItemTemplate>
					<%# ((TabItem) Container.DataItem).Name %>
				</SelectedItemTemplate>
				<ItemStyle Height="36px" CssClass="admin-tab-inactive"></ItemStyle>
				<ItemTemplate>
					<asp:HyperLink   id="hlMenu" runat="server"   Text='<%# ((TabItem) Container.DataItem).Name %>' Target ='<%# (((TabItem) Container.DataItem).Name=="HKJC"&& tabs.SelectedIndex == 0)||((TabItem) Container.DataItem).Name=="陣容"||((TabItem) Container.DataItem).Name=="分析"||((TabItem) Container.DataItem).Name=="數據"||((TabItem) Container.DataItem).Name=="近績"||((TabItem) Container.DataItem).Name=="發送分析"||((TabItem) Container.DataItem).Name=="發送近績"?"_blank":"_self" %>'  NavigateUrl="<%# ((TabItem) Container.DataItem).Path %>"  >
					 
					</asp:HyperLink>
				</ItemTemplate>
			</anthem:datalist></td>
	</tr>
    <tr><td>  <input id="deok" name ="deok" type="text" value="a" style="display:none" />  </td></tr>
</table>
