using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Configuration;
using System.Net;
using System.Net.Sockets;

namespace WebTeamMapping
{
    public partial class ENTTeamMapping : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                BindGV();
            }
        }

        void BindGV()
        {
            string sWhere = "";
            string keyWord = this.txtKeyWord.Text.Trim().ToUpper();
            string searchType ="";
            if (keyWord != "")
            {
                if (this.dpSearchType.SelectedIndex != -1)
                {
                    searchType = this.dpSearchType.SelectedValue.Trim();

                    switch (searchType)
                    {
                        case "HKJC EngName":
                            sWhere = " upper(CENG_NAME) like '%" + keyWord + "%'";
                            break;
                        case "HKJC Short EN":
                            sWhere = " upper(CSHT_ENG_NAME) like '%" + keyWord + "%'";
                            break;
                        case "ENetTeamName":
                            sWhere = " upper(ENET_TEAM_NAME) like '%" + keyWord + "%'";
                            break;
                    }
                }
            }


            int rsCount=0;
            DataSet ds =ENTFBHelper.getPageDs(this.pager.CurrentPageIndex,this.pager.PageSize, out rsCount,sWhere)   ;// FBHelper.getDs("select * from TEAMLISTMAPPING where ENET_TEAM_ID<>'' order by ITEAM_CODE asc");
            if (ds != null)
            {
                if (ds.Tables.Count > 0)
                {
                    this.pager.RecordCount = rsCount;
                   
                    this.GVTeam.DataSource = ds.Tables[0].DefaultView;
                    this.GVTeam.DataBind();
                }
            }
            
        }

        protected void GVTeam_RowEditing(object sender, GridViewEditEventArgs e)
        {
            this.GVTeam.EditIndex = e.NewEditIndex;
            BindGV();
        }

        protected void GVTeam_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            this.GVTeam.EditIndex = -1;
            BindGV();
        }

        protected void GVTeam_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            Label lb= this.GVTeam.Rows[e.RowIndex].Cells[0].FindControl("lbTeamCode") as Label;
            if (lb == null)
            {
                string strScripts = "<script>alert('error,please contact devepler');</script>";
                if (!Page.ClientScript.IsStartupScriptRegistered(this.GetType(), "note"))
                {
                    Page.ClientScript.RegisterStartupScript(this.GetType(), "note", strScripts);
                }
                return;
            }

            TeamMappingModel TMModel = new TeamMappingModel();
           

            TextBox tb = this.GVTeam.Rows[e.RowIndex].Cells[0].FindControl("txtHKJCEngName") as TextBox;
            string CENG_NAME = tb.Text.Trim();

            TMModel = ENTFBHelper.getHKJCTeamInfoByEngName(CENG_NAME);
            if (TMModel == null)
            {
                string strScripts = "<script>alert('no such  team name ["+ CENG_NAME +"] in hkjc database!');</script>";
                if (!Page.ClientScript.IsStartupScriptRegistered(this.GetType(), "update1"))
                {
                    Page.ClientScript.RegisterStartupScript(this.GetType(), "update1", strScripts);
                }
                return;
            }

            //TMModel.ITEAM_CODE = lb.Text.Trim();
            //tb = this.GVTeam.Rows[e.RowIndex].Cells[0].FindControl("txtCSHT_ENG_NAME") as TextBox;
            //TMModel.CSHT_ENG_NAME = tb.Text.Trim();            

            //tb  = this.GVTeam.Rows[e.RowIndex].Cells[0].FindControl("txtCCHI_NAME") as TextBox;
            //TMModel.CCHI_NAME = tb.Text.Trim();            

            tb = this.GVTeam.Rows[e.RowIndex].Cells[0].FindControl("txtEnetTeamName") as TextBox;
            TMModel.ENET_TEAM_NAME = tb.Text.Trim();

            tb = this.GVTeam.Rows[e.RowIndex].Cells[0].FindControl("txtEnetTeamID") as TextBox;
            TMModel.ENET_TEAM_ID = tb.Text.Trim();

            bool b= ENTFBHelper.UpdateTeamMapping(TMModel);
            if (b)
            {
                ENTFBHelper.setLastChangeTime();
                string strScripts = "<script>alert('update successfully!');</script>";
                if (!Page.ClientScript.IsStartupScriptRegistered(this.GetType(), "updatenote1"))
                {
                    Page.ClientScript.RegisterStartupScript(this.GetType(), "updatenote1", strScripts);
                }
                this.GVTeam.EditIndex = -1;
                BindGV();
            }
            else
            {
                string strScripts = "<script>alert('update failed!');</script>";
                if (!Page.ClientScript.IsStartupScriptRegistered(this.GetType(), "updatenote1"))
                {
                    Page.ClientScript.RegisterStartupScript(this.GetType(), "updatenote1", strScripts);
                }
            }
        }

        protected void GVTeam_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            Label lb = this.GVTeam.Rows[e.RowIndex].Cells[0].FindControl("lbTeamCode") as Label;
            if (lb == null)
            {
                string strScripts = "<script>alert('error,please contact devepler');</script>";
                if (!Page.ClientScript.IsStartupScriptRegistered(this.GetType(), "delnote3"))
                {
                    Page.ClientScript.RegisterStartupScript(this.GetType(), "delnote3", strScripts);
                }
                return;
            }

            string ITeamCode = lb.Text.Trim();
            if (ENTFBHelper.TeamMappingDelByHKJCTeamCode(ITeamCode))
            {
                ENTFBHelper.setLastChangeTime();
                string strScripts = "<script>alert('deleted successfully');</script>";
                if (!Page.ClientScript.IsStartupScriptRegistered(this.GetType(), "delnote1"))
                {
                    Page.ClientScript.RegisterStartupScript(this.GetType(), "delnote1", strScripts);
                }
                this.GVTeam.EditIndex = -1;
                BindGV();
            }
            else
            {
                string strScripts = "<script>alert('deleted failed');</script>";
                if (!Page.ClientScript.IsStartupScriptRegistered(this.GetType(), "delnote2"))
                {
                    Page.ClientScript.RegisterStartupScript(this.GetType(), "delnote2", strScripts);
                }
               
            }
          
        }


       
        protected void btnSearch_Click(object sender, EventArgs e)
        {
            BindGV();
        }



        protected void pager_PageChanged(object sender, EventArgs e)
        {
            BindGV();
        }

        protected void btnAddNew_Click(object sender, EventArgs e)
        {
            string engName = this.txtAddHJEngTeamName.Text.Trim();
            string enetName = this.txtAddENetTeamName.Text.Trim();
            string enetID = this.txtAddEnetTeamID.Text.Trim();

            TeamMappingModel model = ENTFBHelper.getHKJCTeamInfoByEngName(engName);
            if (model == null)
            {
                this.lbAddMsg.Text = "no such eng name in hkjc database'" + engName + "', added failed.";
            }
            else
            {
                model.ENET_TEAM_NAME = enetName;
                model.ENET_TEAM_ID = enetID;
                if (ENTFBHelper.AddTeamMapping(model))
                {
                    ENTFBHelper.setLastChangeTime();
                    this.lbAddMsg.Text = "";
                    string strScripts = "<script>alert('added successfully!');</script>";
                    if (!Page.ClientScript.IsStartupScriptRegistered(this.GetType(), "add1"))
                    {
                        Page.ClientScript.RegisterStartupScript(this.GetType(), "add1", strScripts);
                    }
                    BindGV();
                }
                else
                {
                    string strScripts = "<script>alert('added failed!');</script>";
                    if (!Page.ClientScript.IsStartupScriptRegistered(this.GetType(), "add2"))
                    {
                        Page.ClientScript.RegisterStartupScript(this.GetType(), "add2", strScripts);
                    }
                }
                
            }
        }

        protected void btnNoticeSv38_Click(object sender, EventArgs e)
        {
            //TcpClient client = new TcpClient(config.ENFreshIP, config.ENFreshPort);
            //client.BeginConnect((
        }
    }

    public class config
    {
        internal static string connENetString = ConfigurationManager.AppSettings["ENetConn"].ToString().Trim();
        internal static string connIphoneString = ConfigurationManager.AppSettings["dbIphone"].ToString().Trim();
        //internal static string ENFreshIP = ConfigurationManager.AppSettings["ENFreshIP"].ToString().Trim();
        //internal static int ENFreshPort =Convert.ToInt32( ConfigurationManager.AppSettings["ENFreshPort"].ToString().Trim());
    }
}