namespace MasterDetailSample
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Windows.Forms;

    public class detailControl : TabControl
    {
        internal DataSet _cDataset;
        internal List<DataGridView> childGrid = new List<DataGridView>();

        public void Add(string tableName, string pageCaption)
        {
            TabPage page = new TabPage
            {
                Text = pageCaption
            };
            base.TabPages.Add(page);
            DataGridView view = new DataGridView
            {
                Dock = DockStyle.Fill,
                DataSource = new DataView(this._cDataset.Tables[tableName])
            };
            page.Controls.Add(view);
            cModule.applyGridTheme(ref view);
            cModule.setGridRowHeader(ref view, false);
            view.RowPostPaint += new DataGridViewRowPostPaintEventHandler(cModule.rowPostPaint_HeaderCount);
            this.childGrid.Add(view);
        }
        public void AddData(DataTable table, string pageCaption)
        {
            TabPage page = new TabPage
            {
                Text = pageCaption,
                Name = "tpNewTeam"
            };
            base.TabPages.Add(page);
            DataGridView view = new DataGridView
            {
                Name = "dgvNewTeams",
                Dock = DockStyle.Fill,
                DataSource = new DataView(table)
            };
            page.Controls.Add(view);
            cModule.applyGridTheme(ref view);
            cModule.setGridRowHeader(ref view, false);
            view.RowPostPaint += new DataGridViewRowPostPaintEventHandler(cModule.rowPostPaint_HeaderCount);
            this.childGrid.Add(view);
        }

        public void BindData(DataTable table, string pageCaption)
        {
            List<DataGridView> dgvs = this.childGrid;
            if (pageCaption == "Teams")
            {
                // List<DataGridView> dgvs = this.childGrid;
                DataGridView dgv = dgvs[0];
                dgv.DataSource = table;
            }
            //else if (pageCaption == "Players")
            //{
            //    //List<DataGridView> dgvs = this.childGrid[0];
            //    DataGridView dgv = dgvs[1];
            //    dgv.DataSource = table;
            //}
            else if (pageCaption == "HPlayers")
            {
                //List<DataGridView> dgvs = this.childGrid[0];
                DataGridView dgv = dgvs[1];
                dgv.DataSource = table;
            }
            else if (pageCaption == "GPlayers")
            {
                //List<DataGridView> dgvs = this.childGrid[0];
                DataGridView dgv = dgvs[2];
                dgv.DataSource = table;
            }
            else if (pageCaption == "Details")
            {
                //List<DataGridView> dgvs = this.childGrid[0];
                DataGridView dgv = dgvs[3];
                dgv.DataSource = table;
            }
            else if (pageCaption == "Results")
            {
                //List<DataGridView> dgvs = this.childGrid[0];
                DataGridView dgv = dgvs[4];
                dgv.DataSource = table;
            }
            else if (pageCaption == "Stats")
            {
                //List<DataGridView> dgvs = this.childGrid[0];
                DataGridView dgv = dgvs[5];
                dgv.DataSource = table;
            }
            else if (pageCaption == "Incidents")
            {
                //List<DataGridView> dgvs = this.childGrid[0];
                DataGridView dgv = dgvs[6];
                dgv.DataSource = table;
            }
            else if (pageCaption == "GoalInfo")
            {
                //List<DataGridView> dgvs = this.childGrid[0];
                DataGridView dgv = dgvs[7];
                dgv.DataSource = table;
            }
            else if (pageCaption == "MatchDetails")
            {
                //List<DataGridView> dgvs = this.childGrid[0];
                DataGridView dgv = dgvs[8];
                dgv.DataSource = table;
            }
            //cModule.applyGridTheme(ref dgv);
            //cModule.setGridRowHeader(ref dgv, false);
            //dgv.RowPostPaint += new DataGridViewRowPostPaintEventHandler(cModule.rowPostPaint_HeaderCount);

            //   TabPage page = new TabPage
            //{
            //    Text = pageCaption,
            //    Name = "tpNewTeam"
            //};
            //base.TabPages.Add(page);
            //DataGridView view = new DataGridView
            //{
            //    Name = "dgvNewTeams",
            //    Dock = DockStyle.Fill,
            //    DataSource = new DataView(ds.Tables[0])
            //};
            //page.Controls.Add(view);
            //cModule.applyGridTheme(ref view);
            //cModule.setGridRowHeader(ref view, false);
            //view.RowPostPaint += new DataGridViewRowPostPaintEventHandler(cModule.rowPostPaint_HeaderCount);
            //this.childGrid.Add(view);
        }
    }
}