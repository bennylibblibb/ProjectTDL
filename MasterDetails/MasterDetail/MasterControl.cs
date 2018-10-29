namespace MasterDetailSample
{
    using Microsoft.VisualBasic.CompilerServices;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Data;
    using System.Diagnostics;
    using System.Drawing;
    using System.Runtime.CompilerServices;
    using System.Windows.Forms;

    public class MasterControl : DataGridView
    {
        private DataSet _cDataset;
        private string _filterFormat;
        private string _foreignKey;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never), AccessedThroughProperty("RowHeaderIconList")]
        private ImageList _RowHeaderIconList;
        public detailControl childView;
        // internal bool collapseRow;
        public bool collapseRow;
        private IContainer components;
        //   internal List<int> rowCurrent;
        public List<int> rowCurrent;
        //internal object rowDefaultDivider;
        public object rowDefaultDivider;
        // internal object rowDefaultHeight;
        public object rowDefaultHeight;
        internal object rowDividerMargin;
        // internal object rowExpandedDivider;
        public object rowExpandedDivider;
        //internal object rowExpandedHeight;
        public object rowExpandedHeight;
        public int currentPage=0;
        public MasterControl()
        {

        }

        public  void MasterControls(ref DataSet cDataset)
        {
           // base.RowHeaderMouseClick += new DataGridViewCellMouseEventHandler(this.MasterControl_RowHeaderMouseClick);
            base.RowPostPaint += new DataGridViewRowPostPaintEventHandler(this.MasterControl_RowPostPaint);
            base.Scroll += new ScrollEventHandler(this.MasterControl_Scroll);
            base.SelectionChanged += new EventHandler(this.MasterControl_SelectionChanged);
            this.rowCurrent = new List<int>();
            this.rowDefaultHeight = 22;
            this.rowExpandedHeight =  300-120;
            this.rowDefaultDivider = 0;
            this.rowExpandedDivider = 300-22;
            this.rowDividerMargin = 5;
            detailControl control = new detailControl {
                Height = Conversions.ToInteger(Microsoft.VisualBasic.CompilerServices.Operators.SubtractObject(this.rowExpandedDivider, Microsoft.VisualBasic.CompilerServices.Operators.MultiplyObject(this.rowDividerMargin, 2))),
                Visible = false
            };
            this.childView = control;
            base.Controls.Add(this.childView);
            this.InitializeComponent();
            this._cDataset = cDataset;
            this.childView._cDataset = cDataset;
            DataGridView grid = this;
            cModule.applyGridTheme(ref grid);
                 this.Dock = DockStyle.Fill;
            //this.AllowUserToOrderColumns = true;
            //this.Anchor = (AnchorStyles)((((AnchorStyles.Top | AnchorStyles.Left))));
            //| AnchorStyles.Bottom)| AnchorStyles.Right));
          //  this.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
           // this.Height = 200;
           // this.Location = new Point(3, 3);
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            ComponentResourceManager manager = new ComponentResourceManager(typeof(MasterControl));
            this.RowHeaderIconList = new ImageList(this.components);
            ((ISupportInitialize) this).BeginInit();
            base.SuspendLayout();
            this.RowHeaderIconList.ImageStream = (ImageListStreamer) manager.GetObject("RowHeaderIconList.ImageStream");
            this.RowHeaderIconList.TransparentColor = Color.Transparent;
            this.RowHeaderIconList.Images.SetKeyName(0, "expand.png");
            this.RowHeaderIconList.Images.SetKeyName(1, "collapse.png");
            ((ISupportInitialize) this).EndInit();
            base.ResumeLayout(false);
        }

        private void MasterControl_RowHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            Rectangle rectangle = new Rectangle(Conversions.ToInteger(Microsoft.VisualBasic.CompilerServices.Operators.DivideObject(Microsoft.VisualBasic.CompilerServices.Operators.SubtractObject(this.rowDefaultHeight, 0x10), 2)), Conversions.ToInteger(Microsoft.VisualBasic.CompilerServices.Operators.DivideObject(Microsoft.VisualBasic.CompilerServices.Operators.SubtractObject(this.rowDefaultHeight, 0x10), 2)), 0x10, 0x10);
            if (rectangle.Contains(e.Location))
            {
                if (this.rowCurrent.Contains(e.RowIndex))
                {
                    this.rowCurrent.Clear();
                    base.Rows[e.RowIndex].Height = Conversions.ToInteger(this.rowDefaultHeight);
                    base.Rows[e.RowIndex].DividerHeight = Conversions.ToInteger(this.rowDefaultDivider);
                }
                else
                {
                    if (this.rowCurrent.Count > 0)
                    {
                        int num = this.rowCurrent[0];
                        this.rowCurrent.Clear();
                        base.Rows[num].Height = Conversions.ToInteger(this.rowDefaultHeight);
                        base.Rows[num].DividerHeight = Conversions.ToInteger(this.rowDefaultDivider);
                        base.ClearSelection();
                        this.collapseRow = true;
                        base.Rows[num].Selected = true;
                    }
                    this.rowCurrent.Add(e.RowIndex);
                    base.Rows[e.RowIndex].Height = Conversions.ToInteger(this.rowExpandedHeight);
                    base.Rows[e.RowIndex].DividerHeight = Conversions.ToInteger(this.rowExpandedDivider);
                }
                base.ClearSelection();
                this.collapseRow = true;
                base.Rows[e.RowIndex].Selected = true;
            }
            else
            {
                this.collapseRow = false;
            }
        }

        private void MasterControl_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            Rectangle rect = new Rectangle(Conversions.ToInteger(Microsoft.VisualBasic.CompilerServices.Operators.AddObject(e.RowBounds.X, Microsoft.VisualBasic.CompilerServices.Operators.DivideObject(Microsoft.VisualBasic.CompilerServices.Operators.SubtractObject(this.rowDefaultHeight, 0x10), 2))), Conversions.ToInteger(Microsoft.VisualBasic.CompilerServices.Operators.AddObject(e.RowBounds.Y, Microsoft.VisualBasic.CompilerServices.Operators.DivideObject(Microsoft.VisualBasic.CompilerServices.Operators.SubtractObject(this.rowDefaultHeight, 0x10), 2))), 0x10, 0x10);
            if (this.collapseRow)
            {
                if (this.rowCurrent.Contains(e.RowIndex))
                {
                    object[] arguments = new object[] { e.RowIndex };
                    object[] objArray2 = new object[1];
                    object[] objArray3 = new object[] { e.RowIndex };
                    objArray2[0] = Microsoft.VisualBasic.CompilerServices.Operators.SubtractObject(NewLateBinding.LateGet(NewLateBinding.LateGet(sender, null, "Rows", objArray3, null, null, null), null, "height", new object[0], null, null, null), this.rowDefaultHeight);
                    NewLateBinding.LateSetComplex(NewLateBinding.LateGet(sender, null, "Rows", arguments, null, null, null), null, "DividerHeight", objArray2, null, null, false, true);
                    e.Graphics.DrawImage(this.RowHeaderIconList.Images[1], rect);
                    this.childView.Location = new Point(Conversions.ToInteger(Microsoft.VisualBasic.CompilerServices.Operators.AddObject(e.RowBounds.Left, NewLateBinding.LateGet(sender, null, "RowHeadersWidth", new object[0], null, null, null))), Conversions.ToInteger(Microsoft.VisualBasic.CompilerServices.Operators.AddObject(Microsoft.VisualBasic.CompilerServices.Operators.AddObject(e.RowBounds.Top, this.rowDefaultHeight), 5)));
                    this.childView.Width = Conversions.ToInteger(Microsoft.VisualBasic.CompilerServices.Operators.SubtractObject(e.RowBounds.Right, NewLateBinding.LateGet(sender, null, "rowheaderswidth", new object[0], null, null, null)));
                    object[] objArray4 = new object[] { e.RowIndex };
                    this.childView.Height = Conversions.ToInteger(Microsoft.VisualBasic.CompilerServices.Operators.SubtractObject(NewLateBinding.LateGet(NewLateBinding.LateGet(sender, null, "Rows", objArray4, null, null, null), null, "DividerHeight", new object[0], null, null, null), 10));
                    this.childView.Visible = true;
                }
                else
                {
                    this.childView.Visible = false;
                    e.Graphics.DrawImage(this.RowHeaderIconList.Images[0], rect);
                }
                this.collapseRow = false;
            }
            else if (this.rowCurrent.Contains(e.RowIndex))
            {
                object[] objArray5 = new object[] { e.RowIndex };
                object[] objArray6 = new object[1];
                object[] objArray7 = new object[] { e.RowIndex };
                objArray6[0] = Microsoft.VisualBasic.CompilerServices.Operators.SubtractObject(NewLateBinding.LateGet(NewLateBinding.LateGet(sender, null, "Rows", objArray7, null, null, null), null, "height", new object[0], null, null, null), this.rowDefaultHeight);
                NewLateBinding.LateSetComplex(NewLateBinding.LateGet(sender, null, "Rows", objArray5, null, null, null), null, "DividerHeight", objArray6, null, null, false, true);
                e.Graphics.DrawImage(this.RowHeaderIconList.Images[1], rect);
                this.childView.Location = new Point(Conversions.ToInteger(Microsoft.VisualBasic.CompilerServices.Operators.AddObject(e.RowBounds.Left, NewLateBinding.LateGet(sender, null, "RowHeadersWidth", new object[0], null, null, null))), Conversions.ToInteger(Microsoft.VisualBasic.CompilerServices.Operators.AddObject(Microsoft.VisualBasic.CompilerServices.Operators.AddObject(e.RowBounds.Top, this.rowDefaultHeight), 5)));
                this.childView.Width = Conversions.ToInteger(Microsoft.VisualBasic.CompilerServices.Operators.SubtractObject(e.RowBounds.Right, NewLateBinding.LateGet(sender, null, "rowheaderswidth", new object[0], null, null, null)));
                object[] objArray8 = new object[] { e.RowIndex };
                this.childView.Height = Conversions.ToInteger(Microsoft.VisualBasic.CompilerServices.Operators.SubtractObject(NewLateBinding.LateGet(NewLateBinding.LateGet(sender, null, "Rows", objArray8, null, null, null), null, "DividerHeight", new object[0], null, null, null), 10));
                this.childView.Visible = true;
            }
            else
            {
                e.Graphics.DrawImage(this.RowHeaderIconList.Images[0], rect);
            }
            cModule.pageIndex = currentPage;
            cModule.rowPostPaint_HeaderCount(RuntimeHelpers.GetObjectValue(sender), e );
        }

        private void MasterControl_Scroll(object sender, ScrollEventArgs e)
        {
            if (this.rowCurrent.Count > 0)
            {
                this.collapseRow = true;
                base.ClearSelection();
                base.Rows[this.rowCurrent[0]].Selected = true;
            }
        }

        private void MasterControl_SelectionChanged(object sender, EventArgs e)
        {
            if ((base.RowCount > 0) && this.rowCurrent.Contains(base.CurrentRow.Index))
            {
                foreach (DataGridView view in this.childView.childGrid)
                {
                   // ((DataView) view.DataSource).RowFilter = string.Format(this._filterFormat, RuntimeHelpers.GetObjectValue(base[this._foreignKey, base.CurrentRow.Index].Value));
                }
            }
        }

        public void setParentSource(string tableName, string foreignKey)
        {
            base.DataSource = new DataView(this._cDataset.Tables[tableName]);
            DataGridView dgv = this;
            cModule.setGridRowHeader(ref dgv, false);
            this._foreignKey = foreignKey;
            if (((this._cDataset.Tables[tableName].Columns[foreignKey].GetType().ToString() == typeof(int).ToString()) | (this._cDataset.Tables[tableName].Columns[foreignKey].GetType().ToString() == typeof(double).ToString())) | (this._cDataset.Tables[tableName].Columns[foreignKey].GetType().ToString() == typeof(decimal).ToString()))
            {
                this._filterFormat = foreignKey + "={0}";
            }
            else
            {
                this._filterFormat = foreignKey + "='{0}'";
            }
        }

        internal virtual ImageList RowHeaderIconList
        {
            [CompilerGenerated]
            get => 
                this._RowHeaderIconList;
            [MethodImpl(MethodImplOptions.Synchronized), CompilerGenerated]
            set
            {
                this._RowHeaderIconList = value;
            }
        }

        public enum rowHeaderIcons
        {
            expand,
            collapse
        }
    }
}

