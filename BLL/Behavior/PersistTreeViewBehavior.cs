﻿using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes; 
using System.Collections.Generic;
using System.Text; 
using System.Windows.Data; 
using System.Windows.Media.Imaging;
using System.Windows.Navigation; 
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using GOSTS.Common;
using GOSTS.ViewModel;
using GOSTS.Behavior;


namespace GOSTS.Behavior
{
    //class PersistTreeViewBehavior
    //{
        public class DragDropAdorner : Adorner
        {
            public DragDropAdorner(UIElement parent)
                : base(parent)
            {
                IsHitTestVisible = false; // Seems Adorner is hit test visible?
                mDraggedElement = parent as FrameworkElement;
            }

            protected override void OnRender(DrawingContext drawingContext)
            {
                base.OnRender(drawingContext);

                if (mDraggedElement != null)
                {
                    Win32.POINT screenPos = new Win32.POINT();
                    if (Win32.GetCursorPos(ref screenPos))
                    {
                        Point pos = PointFromScreen(new Point(screenPos.X, screenPos.Y));
                        Rect rect = new Rect(pos.X, pos.Y, mDraggedElement.ActualWidth , mDraggedElement.ActualHeight*1.2);
                        drawingContext.PushOpacity(1.0);
                        Brush highlight = mDraggedElement.TryFindResource(SystemColors.HighlightBrushKey) as Brush;
                        if (highlight != null)
                            drawingContext.DrawRectangle(highlight, new Pen(Brushes.Transparent, 0), rect);
                        drawingContext.DrawRectangle(new VisualBrush(mDraggedElement),
                            new Pen(Brushes.Transparent, 0), rect);
                        drawingContext.Pop();
                    }
                }
            }

            FrameworkElement mDraggedElement = null;
        }

        public static class Win32
        {
            public struct POINT { public Int32 X; public Int32 Y; }

            // During drag-and-drop operations, the position of the mouse cannot be 
            // reliably determined through GetPosition. This is because control of 
            // the mouse (possibly including capture) is held by the originating 
            // element of the drag until the drop is completed, with much of the 
            // behavior controlled by underlying Win32 calls. As a workaround, you 
            // might need to use Win32 externals such as GetCursorPos.
            [DllImport("user32.dll")]
            public static extern bool GetCursorPos(ref POINT point);
        }

        internal static class Utils
        {
            public static T FindVisualParent<T>(DependencyObject obj) where T : class
            {
                while (obj != null)
                {
                    if (obj is T)
                        return obj as T;

                    obj = VisualTreeHelper.GetParent(obj);
                }

                return null;
            }

        }

        class DataItem : ICloneable
        {
            public DataItem(string header)
            {
                mHeader = header;
            }

            public object Clone()
            {
                DataItem dataItem = new DataItem(mHeader);
                foreach (DataItem item in Items)
                    dataItem.Items.Add((DataItem)item.Clone());
                return dataItem;
            }

            public string Header
            {
                get { return mHeader; }
            }

            public ObservableCollection<DataItem> Items
            {
                get
                {
                    if (mItems == null)
                        mItems = new ObservableCollection<DataItem>();
                    return mItems;
                }
            }

            private string mHeader = string.Empty;
            private ObservableCollection<DataItem> mItems = null;
        }
    //}
}
