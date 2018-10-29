namespace MasterDetailSample
{
    using Microsoft.VisualBasic.CompilerServices;
    using System;
    using System.Collections;
    using System.Drawing;
    using System.Runtime.InteropServices;
    using System.Windows.Forms;

    [StandardModule]
    internal sealed class cModule
    {
        private static DataGridViewCellStyle amountCellStyle;
        private static DataGridViewCellStyle dateCellStyle;
        private static DataGridViewCellStyle gridCellStyle;
        private static DataGridViewCellStyle gridCellStyle2;
        private static DataGridViewCellStyle gridCellStyle3;
        public static int  pageIndex;

        static cModule()
        {
            DataGridViewCellStyle style1 = new DataGridViewCellStyle {
                Alignment = DataGridViewContentAlignment.MiddleRight
            };
            dateCellStyle = style1;
            DataGridViewCellStyle style2 = new DataGridViewCellStyle {
                Alignment = DataGridViewContentAlignment.MiddleRight,
                Format = "N2"
            };
            amountCellStyle = style2;
            DataGridViewCellStyle style3 = new DataGridViewCellStyle {
                Alignment = DataGridViewContentAlignment.MiddleLeft,
                BackColor = Color.FromArgb(100, 80, 60),
                Font = new Font("Segoe UI", 10f, FontStyle.Regular, GraphicsUnit.Point, 0),
                ForeColor = SystemColors.ControlLightLight,
                SelectionBackColor = SystemColors.Highlight,
                SelectionForeColor = SystemColors.HighlightText,
                WrapMode = DataGridViewTriState.True
            };
            gridCellStyle = style3;
            DataGridViewCellStyle style4 = new DataGridViewCellStyle {
                Alignment = DataGridViewContentAlignment.MiddleLeft,
                BackColor = SystemColors.ControlLightLight,
                Font = new Font("Segoe UI", 10f, FontStyle.Regular, GraphicsUnit.Point, 0),
                ForeColor = SystemColors.ControlText,
                SelectionBackColor = Color.FromArgb(200, 200, 125),// Color.FromArgb(0x9b, 0xbb, 0x59),
                SelectionForeColor = SystemColors.HighlightText,
                WrapMode = DataGridViewTriState.False
            };
            gridCellStyle2 = style4;
            DataGridViewCellStyle style5 = new DataGridViewCellStyle {
                Alignment = DataGridViewContentAlignment.MiddleLeft,
                BackColor = Color.FromArgb(240, 240, 220),//                Color.Lavender,
                Font = new Font("Segoe UI", 10f, FontStyle.Regular, GraphicsUnit.Point, 0),
                ForeColor = SystemColors.WindowText,
                SelectionBackColor = Color.FromArgb(200, 200, 125),
                SelectionForeColor = SystemColors.HighlightText,
                WrapMode = DataGridViewTriState.True
            };
            gridCellStyle3 = style5;
        }

        public static void applyGridTheme(ref DataGridView grid)
        {
            grid.AllowUserToAddRows = false;
            grid.AllowUserToDeleteRows = false;
            grid.BackgroundColor = SystemColors.Window;
            grid.BorderStyle = BorderStyle.None;
            grid.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
            grid.ColumnHeadersDefaultCellStyle = gridCellStyle;
            grid.ColumnHeadersHeight = 0x20;
            grid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            grid.DefaultCellStyle = gridCellStyle2;
            grid.EnableHeadersVisualStyles = false;
            grid.GridColor = SystemColors.GradientInactiveCaption;
            grid.ReadOnly = true;
            grid.RowHeadersVisible = true;
            grid.RowHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
            grid.RowHeadersDefaultCellStyle = gridCellStyle3;
            grid.Font = gridCellStyle.Font;
        }

        public static void rowPostPaint_HeaderCount(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            DataGridView view = (DataGridView)sender;
            string s = ((e.RowIndex + 1) + 30 * (pageIndex-1)).ToString();
            StringFormat format = new StringFormat
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            };
            object[] arguments = new object[] { e.RowIndex };
            Rectangle layoutRectangle = new Rectangle(e.RowBounds.Left, e.RowBounds.Top, view.RowHeadersWidth, Conversions.ToInteger(Operators.SubtractObject(e.RowBounds.Height, NewLateBinding.LateGet(NewLateBinding.LateGet(sender, null, "rows", arguments, null, null, null), null, "DividerHeight", new object[0], null, null, null))));
            e.Graphics.DrawString(s, view.Font, SystemBrushes.ControlText, layoutRectangle, format);
        }

        public static void setGridRowHeader(ref DataGridView dgv, bool hSize = false)
        {
            IEnumerator enumerator;
            dgv.TopLeftHeaderCell.Value = "NO ";
            dgv.TopLeftHeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgv.AutoResizeRowHeadersWidth(DataGridViewRowHeadersWidthSizeMode.AutoSizeToDisplayedHeaders);
            try
            {
                enumerator = dgv.Columns.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    DataGridViewColumn current = (DataGridViewColumn) enumerator.Current;
                    if (current.ValueType.ToString() == typeof(DateTime).ToString())
                    {
                        current.DefaultCellStyle = dateCellStyle;
                    }
                    else if ((current.ValueType.ToString() == typeof(decimal).ToString()) | (current.ValueType.ToString() == typeof(double).ToString()))
                    {
                        current.DefaultCellStyle = amountCellStyle;
                    }
                }
            }
            finally
            {
             
            }
            if (hSize)
            {
                dgv.RowHeadersWidth += 0x10;
            }
            dgv.AutoResizeColumns();
        }
    }
}

