﻿using System;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;
using Anthem;
using FirebirdSql.Data.FirebirdClient;
using JC_SoccerWeb.Common;

namespace JC_SoccerWeb
{
    public partial class TeamMapping : System.Web.UI.Page
    {
        protected Anthem.DataGrid dgTeams;
        protected Anthem.Button btnSave;
               
        private void InitializeComponent()
        {
            this.dgTeams.ItemDataBound += new DataGridItemEventHandler(this.dgTeams_ItemDataBound);
            this.btnSave.Click += new EventHandler(this.btnSave_Click);
            base.Load += new EventHandler(this.Page_Load);
        } 
        private void dgTeams_ItemDataBound(object sender, DataGridItemEventArgs e)
        {
            DataView dv = dgTeams.DataSource as DataView;
            if (e.Item.ItemType != ListItemType.Header && e.Item.ItemType != ListItemType.Footer)
            {
                System.Web.UI.WebControls.DropDownList ddl = (System.Web.UI.WebControls.DropDownList)((DataGridItem)e.Item).FindControl("dplHkjcName");
                ddl.DataSource = dv;
                ddl.DataBind();
                if(e.Item.ItemIndex==1)
                {
                    ddl.Enabled = false;
                    ddl.BackColor = System.Drawing.Color.White;
                }
                System.Web.UI.WebControls.DropDownList ddl2 = (System.Web.UI.WebControls.DropDownList)((DataGridItem)e.Item).FindControl("dplHKJC_NAME_CN");
                ddl2.DataSource = dv;
                ddl2.DataBind();
                System.Web.UI.WebControls.DropDownList ddl3 = (System.Web.UI.WebControls.DropDownList)((DataGridItem)e.Item).FindControl("dplHKJC_ID");
                ddl3.DataSource = dv;
                ddl3.DataBind();

                ddl.SelectedValue = ((DataRowView)e.Item.DataItem)["HKJCNAME"].ToString();
                ddl2.SelectedValue = ((DataRowView)e.Item.DataItem)["HKJCNAMECN"].ToString();
                ddl3.SelectedValue = ((DataRowView)e.Item.DataItem)["HKJCID"].ToString();
            }
        }
        protected void dplHkjcName_SelectedIndexChanged(object sender, EventArgs e)
        {
            DataView dv = dgTeams.DataSource as DataView;
            DataSet ds= dgTeams.DataSource as DataSet;
            System.Web.UI.WebControls.DropDownList list = (System.Web.UI.WebControls.DropDownList)sender;
            TableCell cell = list.Parent as TableCell;
            DataGridItem item = cell.Parent as DataGridItem;
            //TableCell cell2 = item.Parent as  System.Web.UI.WebControls.ChildTable ;
            Anthem.DataGrid dg = item.NamingContainer  as Anthem.DataGrid;
            string[,] strS = new string [2,6];
            for (int i = 0; i < dg.Items.Count; i++)
            {
                for (int j = 0; j < dg.Items[i].Cells.Count; j++)
                {
                    if (i < 3)
                    {
                        strS[i, j] = ((System.Web.UI.WebControls.Label)dg.Items[i].Cells[j].Controls[1]).Text;
                    }
                    else
                    {
                        strS[i, j] = ((System.Web.UI.WebControls.DropDownList)dg.Items[i].Cells[j].Controls[3]).SelectedItem.Text;
                    }
                }
            }
            // dv = dg.DataSource as DataView;
            //System.Web.UI.WebControls.DropDownList ddl2 = (System.Web.UI.WebControls.DropDownList)(item).FindControl("dplHKJC_NAME_CN");
            //System.Web.UI.WebControls.DropDownList ddl3 = (System.Web.UI.WebControls.DropDownList)(item).FindControl("dplHKJC_ID");

            //  dv.RowFilter = "HKJC_NAME='" +  list.SelectedItem.Text + "'";
            if (strS[0, 3] == list.SelectedItem.Text)
            {
                //for (int i = 0; i <2; i++)
                //{

                ((System.Web.UI.WebControls.DropDownList)(dg.Items[0]).FindControl("dplHkjcName")).SelectedValue = strS[0, 3];
                ((System.Web.UI.WebControls.DropDownList)(dg.Items[0]).FindControl("dplHKJC_NAME_CN")).SelectedValue = strS[0, 4];
                ((System.Web.UI.WebControls.DropDownList)(dg.Items[0]).FindControl("dplHKJC_ID")).SelectedValue = strS[0, 5];
                ((System.Web.UI.WebControls.DropDownList)(dg.Items[1]).FindControl("dplHkjcName")).SelectedValue = strS[1, 3];
                ((System.Web.UI.WebControls.DropDownList)(dg.Items[1]).FindControl("dplHKJC_NAME_CN")).SelectedValue = strS[1, 4];
                ((System.Web.UI.WebControls.DropDownList)(dg.Items[1]).FindControl("dplHKJC_ID")).SelectedValue = strS[1, 5];
                //}
            }
            else if (strS[1, 3] == list.SelectedItem.Text)
            {
                ((System.Web.UI.WebControls.DropDownList)(dg.Items[0]).FindControl("dplHkjcName")).SelectedValue = strS[1, 3];
                ((System.Web.UI.WebControls.DropDownList)(dg.Items[0]).FindControl("dplHKJC_NAME_CN")).SelectedValue = strS[1, 4];
                ((System.Web.UI.WebControls.DropDownList)(dg.Items[0]).FindControl("dplHKJC_ID")).SelectedValue = strS[1, 5];
                ((System.Web.UI.WebControls.DropDownList)(dg.Items[1]).FindControl("dplHkjcName")).SelectedValue = strS[0, 3];
                ((System.Web.UI.WebControls.DropDownList)(dg.Items[1]).FindControl("dplHKJC_NAME_CN")).SelectedValue = strS[0, 4];
                ((System.Web.UI.WebControls.DropDownList)(dg.Items[1]).FindControl("dplHKJC_ID")).SelectedValue = strS[0, 5];

            }
             ((System.Web.UI.WebControls.DropDownList)(dg.Items[1]).FindControl("dplHkjcName")).Enabled = false ;
            ((System.Web.UI.WebControls.DropDownList)(dg.Items[1]).FindControl("dplHkjcName")).BackColor = System.Drawing.Color.White;
            //for (int i = 0; i < dg.Items.Count; i++)
            //{
            //    for (int j = 0; j < dg.Items[i].Cells.Count; j++)
            //    {
            //        if (i < 3)
            //        {
            //            strS[i, j] = ((System.Web.UI.WebControls.Label)dg.Items[i].Cells[j].Controls[1]).Text;
            //        }
            //        else
            //        {
            //            strS[i, j] = ((System.Web.UI.WebControls.DropDownList)dg.Items[i].Cells[j].Controls[3]).SelectedItem.Text;
            //        }
            //    }
            //}
            this.dgTeams.UpdateAfterCallBack = true;
        }

        protected override void OnInit(EventArgs e)
        {
            this.InitializeComponent();
            base.OnInit(e);
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.IsAuthenticated)
            {
                if (!Page.IsPostBack)
                {
                    string sType = this.Request.QueryString["Type"].ToString().Trim();
                   
                    if (sType != "HKJC")
                    {
                        btnSave.Visible = false;
                        btnSave.UpdateAfterCallBack = true;
                    }
                    string sID = this.Request.QueryString["ID"].ToString().Trim();
                    // string sType = this.Request.QueryString["Type"].ToString().Trim();
                    BindTeams(sID);
                }
            }
        }

        private void BindTeams(string id)
        {
            if (id == null || id == "")
            {
                btnSave.Visible = false;
                btnSave.UpdateAfterCallBack = true;
            }
            else
            {
                try
                {
                    using (FbConnection connection = new FbConnection(AppFlag.ScoutsDBConn))
                    {
                        //string queryString = "Select  e.id eid ,t.id,e.NAME ,t.id,t.name TNAME,t.SHORT_NAME, t.HKJC_ID , t.HKJC_NAME, t.HKJC_NAME_CN,t.MAPPING_STATUS from teams t  inner  join  events e  on e.HOME_ID=t.ID or e.GUEST_ID=t.id  where e.id ='" + id + "'  order by t.MAPPING_STATUS";
                          string queryString = "select distinct case when  t.id = r.home_Id  then  1  when  t.id =r.guest_Id   then 2  end as irec , r.id eid ,r.name name, t.id ,t.NAME TNAME,t.SHORT_NAME, t.HKJC_ID,t.HKJC_NAME,t.HKJC_NAME_CN,t.MAPPING_STATUS,e.HKJCHOSTID,e.HKJCHOSTNAME,e.HKJCHOSTNAME_CN, e.HKJCGUESTID, e.HKJCGUESTNAME,e.HKJCGUESTNAME_CN from teams t inner join  EVENTS r on  r.HOME_ID=t.id  or r.guest_id=t.id inner join  EMATCHES e on e.EMATCHID =r.id where r.id =" + id + "  order by irec asc ";
                      //  Files.CicsWriteLog(DateTime.Now.ToString("HH:mm:ss") + " Sql2: " + queryString);
                        using (FbCommand cmd = new FbCommand(queryString))
                        {
                            using (FbDataAdapter fda = new FbDataAdapter())
                            {
                                connection.Open();
                                cmd.Connection = connection;
                                fda.SelectCommand = cmd;
                                using (DataSet data = new DataSet())
                                {
                                    data.Tables.Add(new DataTable("Teams"));
                                    fda.Fill(data.Tables["Teams"]);


                                    if (data.Tables["Teams"].Rows.Count > 0)
                                    {
                                        if (data.Tables["Teams"].Rows.Count == 2)
                                        {
                                            data.Tables[0].Columns.Add("HKJCID", typeof(int));
                                            data.Tables[0].Columns.Add("HKJCNAME", typeof(string));
                                            data.Tables[0].Columns.Add("HKJCNAMECN", typeof(string));
                                           if (data.Tables[0].Rows[0]["HKJC_NAME"].ToString() == data.Tables[0].Rows[0]["HKJCGUESTNAME"].ToString()|| data.Tables[0].Rows[1]["HKJC_NAME"].ToString() == data.Tables[0].Rows[0]["HKJCHOSTNAME"].ToString())
                                            {
                                                data.Tables[0].Rows[0]["HKJCID"] = data.Tables[0].Rows[0]["HKJCGUESTID"];
                                                data.Tables[0].Rows[0]["HKJCNAME"] = data.Tables[0].Rows[0]["HKJCGUESTNAME"];
                                                data.Tables[0].Rows[0]["HKJCNAMECN"] = data.Tables[0].Rows[0]["HKJCGUESTNAME_CN"];
                                                data.Tables[0].Rows[1]["HKJCID"] = data.Tables[0].Rows[1]["HKJCHOSTID"];
                                                data.Tables[0].Rows[1]["HKJCNAME"] = data.Tables[0].Rows[1]["HKJCHOSTNAME"];
                                                data.Tables[0].Rows[1]["HKJCNAMECN"] = data.Tables[0].Rows[1]["HKJCHOSTNAME_CN"];
                                            }
                                           else if(data.Tables[0].Rows[0]["HKJC_NAME"].ToString() == data.Tables[0].Rows[0]["HKJCHOSTNAME"].ToString()|| data.Tables[0].Rows[1]["HKJC_NAME"].ToString() == data.Tables[0].Rows[0]["HKJCGUESTNAME"].ToString())
                                            {
                                                data.Tables[0].Rows[0]["HKJCID"] = data.Tables[0].Rows[0]["HKJCHOSTID"];
                                                data.Tables[0].Rows[0]["HKJCNAME"] = data.Tables[0].Rows[0]["HKJCHOSTNAME"];
                                                data.Tables[0].Rows[0]["HKJCNAMECN"] = data.Tables[0].Rows[0]["HKJCHOSTNAME_CN"];
                                                data.Tables[0].Rows[1]["HKJCID"] = data.Tables[0].Rows[1]["HKJCGUESTID"];
                                                data.Tables[0].Rows[1]["HKJCNAME"] = data.Tables[0].Rows[1]["HKJCGUESTNAME"];
                                                data.Tables[0].Rows[1]["HKJCNAMECN"] = data.Tables[0].Rows[1]["HKJCGUESTNAME_CN"];
                                            } 
                                            else
                                            {
                                                data.Tables[0].Rows[0]["HKJCID"] = data.Tables[0].Rows[0]["HKJCHOSTID"];
                                                data.Tables[0].Rows[0]["HKJCNAME"] = data.Tables[0].Rows[0]["HKJCHOSTNAME"];
                                                data.Tables[0].Rows[0]["HKJCNAMECN"] = data.Tables[0].Rows[0]["HKJCHOSTNAME_CN"];
                                                data.Tables[0].Rows[1]["HKJCID"] = data.Tables[0].Rows[1]["HKJCGUESTID"];
                                                data.Tables[0].Rows[1]["HKJCNAME"] = data.Tables[0].Rows[1]["HKJCGUESTNAME"];
                                                data.Tables[0].Rows[1]["HKJCNAMECN"] = data.Tables[0].Rows[1]["HKJCGUESTNAME_CN"];
                                            }
                                             
                                        }

                                        dgTeams.DataSource = data.Tables[0].DefaultView;
                                        dgTeams.DataBind();
                                        dgTeams.UpdateAfterCallBack = true;
                                        lbEvent.Text = data.Tables["Teams"].Rows[0]["eid"].ToString() + " " + data.Tables["Teams"].Rows[0]["name"].ToString();
                                        this.Page.Title = data.Tables["Teams"].Rows[0]["name"].ToString() + " Team Mapping";

                                        if (data.Tables["Teams"].Rows.Count == 2 && ((data.Tables["Teams"].Rows[0]["MAPPING_STATUS"] is DBNull || !(Convert.ToBoolean(data.Tables["Teams"].Rows[0]["MAPPING_STATUS"])))
                                            || (data.Tables["Teams"].Rows[1]["MAPPING_STATUS"] is DBNull || !(Convert.ToBoolean(data.Tables["Teams"].Rows[1]["MAPPING_STATUS"])))))
                                        {
                                            btnSave.Text = "Save";
                                            btnSave.UpdateAfterCallBack = true;
                                        }
                                        else if (data.Tables["Teams"].Rows.Count == 2 && (Convert.ToBoolean(data.Tables["Teams"].Rows[0]["MAPPING_STATUS"]) || Convert.ToBoolean(data.Tables["Teams"].Rows[1]["MAPPING_STATUS"])))
                                        {
                                            btnSave.Text = "Update";
                                            btnSave.UpdateAfterCallBack = true;
                                        }
                                    }
                                    else
                                    {
                                        btnSave.Visible = false;
                                        btnSave.UpdateAfterCallBack = true;
                                    }
                                }
                            }
                        }
                        connection.Close();
                    }

                    //using (FbConnection connection = new FbConnection(AppFlag.ScoutsDBConn))
                    //{
                    //    string queryString = "Select  e.id eid ,t.id,e.NAME ,t.id,t.name TNAME,t.SHORT_NAME, t.HKJC_ID , t.HKJC_NAME, t.HKJC_NAME_CN,t.MAPPING_STATUS from teams t  inner  join  events e  on e.HOME_ID=t.ID or e.GUEST_ID=t.id  where e.id ='" + id + "'  order by t.MAPPING_STATUS";
                    //    using (FbCommand cmd = new FbCommand(queryString))
                    //    {
                    //        using (FbDataAdapter fda = new FbDataAdapter())
                    //        {
                    //            connection.Open();
                    //            cmd.Connection = connection;
                    //            fda.SelectCommand = cmd;
                    //            using (DataSet data = new DataSet())
                    //            {
                    //                data.Tables.Add(new DataTable("Teams"));
                    //                fda.Fill(data.Tables["Teams"]);
                    //                if (data.Tables["Teams"].Rows.Count > 0)
                    //                {
                    //                    dgTeams.DataSource = data.Tables[0].DefaultView;
                    //                    dgTeams.DataBind();
                    //                    dgTeams.UpdateAfterCallBack = true;
                    //                    lbEvent.Text = data.Tables["Teams"].Rows[0]["eid"].ToString() + " " + data.Tables["Teams"].Rows[0]["name"].ToString();
                    //                    this.Page.Title = data.Tables["Teams"].Rows[0]["name"].ToString() + " Team Mapping";

                    //                    //if (!(data.Tables["Teams"].Rows[0]["MAPPING_STATUS"] is DBNull)&& Convert.ToBoolean(data.Tables["Teams"].Rows[0]["MAPPING_STATUS"]) && !(data.Tables["Teams"].Rows[1]["MAPPING_STATUS"] is DBNull)&& Convert.ToBoolean(data.Tables["Teams"].Rows[1]["MAPPING_STATUS"]))
                    //                    //{
                    //                    //    btnSave.Visible = false;
                    //                    //    btnSave.UpdateAfterCallBack = true;
                    //                    //}
                    //                    if (data.Tables["Teams"].Rows.Count == 2 && ((data.Tables["Teams"].Rows[0]["MAPPING_STATUS"] is DBNull || !(Convert.ToBoolean(data.Tables["Teams"].Rows[0]["MAPPING_STATUS"])))
                    //                        || (data.Tables["Teams"].Rows[1]["MAPPING_STATUS"] is DBNull || !(Convert.ToBoolean(data.Tables["Teams"].Rows[1]["MAPPING_STATUS"])))))
                    //                    {
                    //                        btnSave.Text = "Save";
                    //                        btnSave.UpdateAfterCallBack = true;
                    //                    }
                    //                    else if (data.Tables["Teams"].Rows.Count == 2 && (Convert.ToBoolean(data.Tables["Teams"].Rows[0]["MAPPING_STATUS"]) || Convert.ToBoolean(data.Tables["Teams"].Rows[1]["MAPPING_STATUS"])))
                    //                    {
                    //                        btnSave.Text = "Update";
                    //                        btnSave.UpdateAfterCallBack = true;
                    //                    }
                    //                }
                    //                else
                    //                {
                    //                    btnSave.Visible = false;
                    //                    btnSave.UpdateAfterCallBack = true;
                    //                }
                    //            }
                    //        }
                    //    }
                    //    connection.Close();
                    //}
                }
                catch (Exception exp)
                {
                    string exps = exp.ToString();
                    Files.CicsWriteError(DateTime.Now.ToString("HH:mm:ss") + " BindTeams(),error: " + exps);
                }
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                string eventID = this.lbEvent.Text.Substring(0, this.lbEvent.Text.IndexOf(" "));
                string sHome = "";
                string sGuest = "";
                //foreach (DataGridItem dgi in dgTeams.Items)
                for (int j = 0; j < dgTeams.Items.Count; j++)
                {
                    if (j == 0)
                    {
                        sHome = ((Anthem.Label)dgTeams.Items[j].Cells[0].Controls[1]).Text + "/" +
                       //  ((Anthem.Label)dgTeams.Items[j].Cells[1].Controls[1]).Text + "/" +
                      //     ((Anthem.Label)dgTeams.Items[j].Cells[2].Controls[1]).Text + "/" +
                            ((System.Web.UI.WebControls.DropDownList)dgTeams.Items[j].Cells[3].Controls[3]).SelectedValue + "/" +
                            ((System.Web.UI.WebControls.DropDownList)dgTeams.Items[j].Cells[4].Controls[3]).SelectedValue + "/" +
                           ((System.Web.UI.WebControls.DropDownList)dgTeams.Items[j].Cells[5].Controls[3]).SelectedValue;
                    }
                    else if (j == 1)
                    {
                        sGuest = ((Anthem.Label)dgTeams.Items[j].Cells[0].Controls[1]).Text + "/" +
                       //      ((Anthem.Label)dgTeams.Items[j].Cells[1].Controls[1]).Text + "/" +
                       //    ((Anthem.Label)dgTeams.Items[j].Cells[2].Controls[1]).Text + "/" +
                            ((System.Web.UI.WebControls.DropDownList)dgTeams.Items[j].Cells[3].Controls[3]).SelectedValue + "/" +
                            ((System.Web.UI.WebControls.DropDownList)dgTeams.Items[j].Cells[4].Controls[3]).SelectedValue + "/" +
                           ((System.Web.UI.WebControls.DropDownList)dgTeams.Items[j].Cells[5].Controls[3]).SelectedValue;
                    }

                    //for (int i = 0; i < dgTeams.Items[j].Cells.Count; i++)
                    //{
                    //if (j == 0)
                    //{
                    //    sHome += "";
                    //}
                    //else if (j == 1)
                    //{
                    //    sGuest += "";
                    //}
                    //}
                }

                Files.CicsWriteLog(  DateTime.Now.ToString("HH:mm:ss ") + ((Anthem.Label)dgTeams.Items[0].Cells[1].Controls[1]).Text+"/" +sHome +" -- " + ((Anthem.Label)dgTeams.Items[1].Cells[1].Controls[1]).Text +"/"+ sGuest);

                using (FbConnection connection = new FbConnection(AppFlag.ScoutsDBConn))
                {
                    connection.Open();
                    using (FbCommand cmd2 = new FbCommand())
                    {
                        cmd2.CommandText = "WEB_SYNC_MANUAL_ADDHKJCTEAM";
                        cmd2.CommandType = CommandType.StoredProcedure;
                        cmd2.Connection = connection;
                        cmd2.Parameters.Add("@EVENT_ID", eventID);
                        cmd2.Parameters.Add("@HOME", sHome);
                        cmd2.Parameters.Add("@GUEST", sGuest);
                        int id = Convert.ToInt32(cmd2.ExecuteScalar());
                        //  Files.CicsWriteLog((id == 2 ? DateTime.Now.ToString("HH:mm:ss") + " [Success] " : DateTime.Now.ToString("HH:mm:ss") + " [Failure] ") + "Sure [" + lbEvent.Text + "] on Teams");
                        Files.CicsWriteLog((id >0 ? DateTime.Now.ToString("HH:mm:ss") + " [Success] " : DateTime.Now.ToString("HH:mm:ss") + " [Failure] ") + "Sure [" + lbEvent.Text + "] on Teams");

                        if (id>0)
                        {
                            btnSave.Enabled = false;
                            btnSave.UpdateAfterCallBack = true;
                        }
                    }
                    connection.Close();
                }
            }
            catch (Exception exp)
            {
                string exps = exp.ToString();
                Files.CicsWriteError(DateTime.Now.ToString("HH:mm:ss") + "  btnSave_Click()  " + exps);
            }

            //try
            //{
            //    string eventID = this.lbEvent.Text.Substring(0, this.lbEvent.Text.IndexOf(" "));
            //    using (FbConnection connection = new FbConnection(AppFlag.ScoutsDBConn))
            //    {
            //        connection.Open();
            //        using (FbCommand cmd2 = new FbCommand())
            //        {
            //            cmd2.CommandText = "WEB_SYNC_MANUAL_ADDHKJCTEAM";
            //            cmd2.CommandType = CommandType.StoredProcedure;
            //            cmd2.Connection = connection;
            //            cmd2.Parameters.Add("@EVENT_ID", eventID);
            //            int id = Convert.ToInt32(cmd2.ExecuteScalar());
            //            Files.CicsWriteLog((id == 2 ? DateTime.Now.ToString("HH:mm:ss") + " [Success] " : DateTime.Now.ToString("HH:mm:ss") + " [Failure] ") + "Sure [" + lbEvent.Text + "] on Teams");
            //            if(id==2)
            //            {
            //                btnSave.Enabled = false;
            //                btnSave.UpdateAfterCallBack = true;
            //            }
            //        }
            //        connection.Close();
            //    }
            //}
            //catch (Exception exp)
            //{
            //    string exps = exp.ToString();
            //    Files.CicsWriteError(DateTime.Now.ToString("HH:mm:ss") + "  btnSave_Click()  " + exps);
            //}
        }
        
    }
}