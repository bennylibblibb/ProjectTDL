using System.Windows.Controls;
using System.Collections;

namespace GOSTS.Behavior
{
    /// <summary>
    /// A wrapper to keep Tab.SelectedItem and PersistTabBehavior.SelectedItemProperty in sync
    /// </summary>
    public class PersistTabSelectedItemHandler
    {
        public TabControl Tab { get; private set; }

        public PersistTabSelectedItemHandler(TabControl tab)
        {
            Tab = tab;
            Tab.SelectionChanged += ChangeSelectionFromUi;
        }

        public void Dispose()
        {
            Tab.SelectionChanged -= ChangeSelectionFromUi;
            Tab = null;
        }

public void ChangeSelectionFromProperty()
{
    var selectedObject = Tab.GetValue(PersistTabBehavior.SelectedItemProperty);

    if (selectedObject == null)
    {
        Tab.SelectedItem = null;
        return;
    }

    foreach (TabItem tabItem in Tab.Items)
    {
        if (tabItem.DataContext == selectedObject)
        {
            if (!tabItem.IsSelected)
                tabItem.IsSelected = true;

            break;
        }
    }
}

private void ChangeSelectionFromUi(object sender, SelectionChangedEventArgs e)
{
    if (e.AddedItems.Count >= 1)
    {
        var selectedObject = e.AddedItems[0];

        var selectedItem = selectedObject as TabItem;

        if (selectedItem != null)
            SelectedItemProperty(selectedItem);
    }
}

private void SelectedItemProperty(TabItem selectedTabItem)
{
    var tabObjects = Tab.GetValue(PersistTabBehavior.ItemsSourceProperty) as IEnumerable;

    if (tabObjects == null)
        return;

    foreach (var tabObject in tabObjects)
    {
        if (tabObject == selectedTabItem.DataContext)
        {
            PersistTabBehavior.SetSelectedItem(Tab, tabObject);
            return;
        }
    }
}
    }
}
