using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using FirebirdSql.Data;
using FirebirdSql.Data.FirebirdClient;
using System.Text;
using System.IO;

namespace WebTeamMapping
{
    public class ENTFBHelper
    {
        public static string getSqlScale(string sql)
        {
            FirebirdSql.Data.FirebirdClient.FbConnection cn = new FirebirdSql.Data.FirebirdClient.FbConnection();
            try
            {
                cn.ConnectionString = config.connENetString;
                cn.Open();
                string result = "";
                using (FbCommand selectData = cn.CreateCommand())
                {
                    selectData.CommandText = sql;                   
                    object obj = selectData.ExecuteScalar();
                    if (obj != null)
                    {
                        result = obj.ToString().Trim();
                    }
                }
                cn.Close();
                return result;
            }
            catch (Exception ex)
            {
                IOAccess.WriteErrLog("sql," + sql);
                IOAccess.WriteErrLog("getsqlScale err," + ex.Message);
                cn.Close();
            }

            return "";
        }

        public static bool excuteSql(string sql)
        {
            FirebirdSql.Data.FirebirdClient.FbConnection cn = new FirebirdSql.Data.FirebirdClient.FbConnection();
            try
            {
                cn.ConnectionString = config.connENetString;
                cn.Open();
                FbTransaction myTransaction = cn.BeginTransaction();
                FbCommand myCommand = new FbCommand();
                myCommand.CommandText = sql;
                myCommand.Connection = cn;
                myCommand.Transaction = myTransaction;
                myCommand.Parameters.Clear();

                myCommand.ExecuteNonQuery();
                // Commit changes
                myTransaction.Commit();
                // Free command resources in Firebird Server
                myCommand.Dispose();
                cn.Close();
                return true;
            }
            catch (Exception ex)
            {
                IOAccess.WriteErrLog("sql," + sql);
                IOAccess.WriteErrLog("excuteSql," + ex.Message);
                cn.Close();
                return false;
            }

            return false;
        }



        public static DataSet getDs(string sql)
        {
            FirebirdSql.Data.FirebirdClient.FbConnection cn = new FirebirdSql.Data.FirebirdClient.FbConnection();
            try
            {
                FirebirdSql.Data.FirebirdClient.FbConnectionStringBuilder cs = new FirebirdSql.Data.FirebirdClient.FbConnectionStringBuilder();
                cn.ConnectionString = config.connENetString;
                cn.Open();

                DataSet ds = new DataSet();
                using (FbCommand selectData = cn.CreateCommand())
                {
                    selectData.CommandText = sql;// "select ITEAM_CODE,CCHI_NAME,CENG_NAME,ENET_TEAM_NAME from TEAMLISTMAPPING where ENET_TEAM_NAME=@enetHost or ENET_TEAM_NAME=@enetGuest";

                    //selectData.Parameters.Clear();
                    //selectData.Parameters.Add("@enetHost", FbDbType.VarChar, 100).Value = enetHost;
                    //selectData.Parameters.Add("@enetGuest", FbDbType.VarChar, 100).Value = enetGuest;

                    FbDataAdapter adapter = new FbDataAdapter(selectData);
                    adapter.Fill(ds);
                    adapter.Dispose();
                }

                cn.Close();

                return ds;
            }
            catch (Exception ex)
            {
                IOAccess.WriteErrLog("getDs error," + ex.Message);
                cn.Close();
            }

            return null;
        }


        public static DataSet getDs(string sql,string connString)
        {
            FirebirdSql.Data.FirebirdClient.FbConnection cn = new FirebirdSql.Data.FirebirdClient.FbConnection();
            try
            {
                FirebirdSql.Data.FirebirdClient.FbConnectionStringBuilder cs = new FirebirdSql.Data.FirebirdClient.FbConnectionStringBuilder();
                cn.ConnectionString = connString;
                cn.Open();

                DataSet ds = new DataSet();
                using (FbCommand selectData = cn.CreateCommand())
                {
                    selectData.CommandText = sql;// "select ITEAM_CODE,CCHI_NAME,CENG_NAME,ENET_TEAM_NAME from TEAMLISTMAPPING where ENET_TEAM_NAME=@enetHost or ENET_TEAM_NAME=@enetGuest";


                    FbDataAdapter adapter = new FbDataAdapter(selectData);
                    adapter.Fill(ds);
                    adapter.Dispose();
                }

                cn.Close();

                return ds;
            }
            catch (Exception ex)
            {
                IOAccess.WriteErrLog("getDs(string ,string) error," + ex.Message);
                cn.Close();
            }

            return null;
        }

        public static DataSet getPageDs(int pageIndex,int pageSize,out int rsCount,string searchClause)
        {
            rsCount = 0;
            FirebirdSql.Data.FirebirdClient.FbConnection cn = new FirebirdSql.Data.FirebirdClient.FbConnection();
            try
            {
                FirebirdSql.Data.FirebirdClient.FbConnectionStringBuilder cs = new FirebirdSql.Data.FirebirdClient.FbConnectionStringBuilder();
                cn.ConnectionString = config.connENetString;
                cn.Open();

                string sWhere = " where ENET_TEAM_NAME<>'' ";
                if (searchClause.Trim() != "")
                {
                    sWhere += " and " + searchClause;
                }
                using (FbCommand selectData = cn.CreateCommand())
                {
                    selectData.CommandText = "select count(*) from TEAMLISTMAPPING" + sWhere;
                    object obj = selectData.ExecuteScalar();
                    if (obj != null)
                    {
                        int result = 0;
                        try
                        {
                            result = Convert.ToInt32(obj.ToString().Trim());
                            rsCount = result;
                        }
                        catch (Exception ex)
                        {
                            IOAccess.WriteErrLog("getPageDs p1 error," + sWhere+"," + ex.Message);
                            rsCount = 0;
                        }                        
                    }
                }

                DataSet ds = new DataSet();
                using (FbCommand selectData = cn.CreateCommand())
                {
                    StringBuilder builder =new StringBuilder("select first " + pageSize);
                    if(pageIndex>1)
                    {
                        builder.Append(" skip "+ pageSize* (pageIndex-1)+" ");
                    }
                    builder.Append(" a.ITEAM_CODE, a.CCHI_NAME, a.CENG_NAME, a.CSHT_ENG_NAME, a.ENET_TEAM_NAME, a.ENET_TEAM_ID FROM TEAMLISTMAPPING a " + sWhere + "  order by ITEAM_CODE;");

                    selectData.CommandText = builder.ToString();// "select ITEAM_CODE,CCHI_NAME,CENG_NAME,ENET_TEAM_NAME from TEAMLISTMAPPING where ENET_TEAM_NAME=@enetHost or ENET_TEAM_NAME=@enetGuest";                  
                    FbDataAdapter adapter = new FbDataAdapter(selectData);
                    adapter.Fill(ds);
                    adapter.Dispose();
                }
                cn.Close();

                return ds;
            }
            catch (Exception ex)
            {
                IOAccess.WriteErrLog("getPageDs p2 error," + searchClause + "," + ex.Message);
               
                cn.Close();
            }

            return null;
        }

        public static bool UpdateTeamMapping(TeamMappingModel model)
        {
            string sql = "";
            FirebirdSql.Data.FirebirdClient.FbConnection cn = new FirebirdSql.Data.FirebirdClient.FbConnection();
            try
            {
                cn.ConnectionString = config.connENetString;
                cn.Open();
                FbTransaction myTransaction = cn.BeginTransaction();
                FbCommand myCommand = new FbCommand();
                sql = "UPDATE TEAMLISTMAPPING SET CCHI_NAME=@CCHI_NAME,CENG_NAME=@CENG_NAME,CSHT_ENG_NAME=@CSHT_ENG_NAME,ENET_TEAM_NAME=@ENET_TEAM_NAME,ENET_TEAM_ID=@ENET_TEAM_ID where ITEAM_CODE=@ITEAM_CODE";

                myCommand.CommandText = sql;
                myCommand.Parameters.Clear();
                myCommand.Parameters.Add(new FbParameter("@CCHI_NAME", model.CCHI_NAME));
                myCommand.Parameters.Add(new FbParameter("@CENG_NAME", model.CENG_NAME));
                myCommand.Parameters.Add(new FbParameter("@CSHT_ENG_NAME", model.CSHT_ENG_NAME));
                myCommand.Parameters.Add(new FbParameter("@ENET_TEAM_NAME", model.ENET_TEAM_NAME));
                myCommand.Parameters.Add(new FbParameter("@ENET_TEAM_ID", model.ENET_TEAM_ID));
                myCommand.Parameters.Add(new FbParameter("@ITEAM_CODE", model.ITEAM_CODE));

                myCommand.Connection = cn;
                myCommand.Transaction = myTransaction;
              

                myCommand.ExecuteNonQuery();
                // Commit changes
                myTransaction.Commit();
                // Free command resources in Firebird Server
                myCommand.Dispose();
                cn.Close();
                return true;
            }
            catch (Exception ex)
            {
                IOAccess.WriteErrLog("UpdateTeamMapping ," + sql);
                IOAccess.WriteErrLog("UpdateTeamMapping error," + ex.Message);
                cn.Close();
                return false;
            }

            return false;
        }


        public static bool AddTeamMapping(TeamMappingModel model)
        {
            string sql = "";
            FirebirdSql.Data.FirebirdClient.FbConnection cn = new FirebirdSql.Data.FirebirdClient.FbConnection();
            try
            {
                cn.ConnectionString = config.connENetString;
                cn.Open();
                FbTransaction myTransaction = cn.BeginTransaction();
                FbCommand myCommand = new FbCommand();
                sql = "insert into TEAMLISTMAPPING(ITEAM_CODE,CCHI_NAME,CENG_NAME, CSHT_ENG_NAME, ENET_TEAM_NAME,ENET_TEAM_ID ) values(@ITEAM_CODE,@CCHI_NAME,@CENG_NAME, @CSHT_ENG_NAME, @ENET_TEAM_NAME,@ENET_TEAM_ID )";

                myCommand.CommandText = sql;
                myCommand.Parameters.Clear();
                myCommand.Parameters.Add(new FbParameter("@ITEAM_CODE", model.ITEAM_CODE));
                myCommand.Parameters.Add(new FbParameter("@CCHI_NAME", model.CCHI_NAME));
                myCommand.Parameters.Add(new FbParameter("@CENG_NAME", model.CENG_NAME));
                myCommand.Parameters.Add(new FbParameter("@CSHT_ENG_NAME", model.CSHT_ENG_NAME));
                myCommand.Parameters.Add(new FbParameter("@ENET_TEAM_NAME", model.ENET_TEAM_NAME));
                myCommand.Parameters.Add(new FbParameter("@ENET_TEAM_ID", model.ENET_TEAM_ID));               

                myCommand.Connection = cn;
                myCommand.Transaction = myTransaction;
           

                myCommand.ExecuteNonQuery();
                // Commit changes
                myTransaction.Commit();
                // Free command resources in Firebird Server
                myCommand.Dispose();
                cn.Close();
                return true;
            }
            catch (Exception ex)
            {
                IOAccess.WriteErrLog("AddTeamMapping error," + sql);
                IOAccess.WriteErrLog("AddTeamMapping error," + ex.Message);
                cn.Close();
                return false;
            }

            return false;
        }

        public static bool TeamMappingDelByHKJCTeamCode(string ITeamCode)
        {
            ITeamCode = ITeamCode.Replace("'", "");
            ITeamCode = ITeamCode.Replace(" ", "");
            string sql = "delete from TEAMLISTMAPPING where ITEAM_CODE="+ITeamCode;
            return excuteSql(sql);
        }


        public static TeamMappingModel getHKJCTeamInfoByEngName(string EngName)
        {
            string sql = "select * from TEAMLIST where CENG_NAME='" + EngName + "'";
            DataSet ds = getDs(sql,config.connIphoneString);
            if (ds != null)
            {
                if (ds.Tables.Count > 0)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        DataRow dr = ds.Tables[0].Rows[0];
                        TeamMappingModel model = new TeamMappingModel();
                        model.ITEAM_CODE = dr["ITEAM_CODE"].ToString().Trim();
                        model.CENG_NAME = dr["CENG_NAME"].ToString().Trim();
                        model.CSHT_ENG_NAME = dr["CSHT_ENG_NAME"].ToString().Trim();
                        model.CCHI_NAME = dr["CCHI_NAME"].ToString().Trim();
                        return model;
                    }
                }
            }

            return null;
        }


        public static bool setLastChangeTime()
        {
            string sql = "update  LstUpdate set ut=CURRENT_TIMESTAMP where NAME='teammapping';";
            return ENTFBHelper.excuteSql(sql);
        }
















        public static DataSet getHKJCTeamPageDs(int pageIndex, int pageSize, out int rsCount, string searchClause)
        {
            rsCount = 0;
            FirebirdSql.Data.FirebirdClient.FbConnection cn = new FirebirdSql.Data.FirebirdClient.FbConnection();
            try
            {
                FirebirdSql.Data.FirebirdClient.FbConnectionStringBuilder cs = new FirebirdSql.Data.FirebirdClient.FbConnectionStringBuilder();
                cn.ConnectionString = config.connIphoneString;
                cn.Open();

                string sWhere = " where 1=1 ";
                if (searchClause.Trim() != "")
                {
                    sWhere += " and " + searchClause;
                }
                using (FbCommand selectData = cn.CreateCommand())
                {
                    selectData.CommandText = "select count(*) from TEAMLIST" + sWhere;
                    object obj = selectData.ExecuteScalar();
                    if (obj != null)
                    {
                        int result = 0;
                        try
                        {
                            result = Convert.ToInt32(obj.ToString().Trim());
                            rsCount = result;
                        }
                        catch (Exception ex)
                        {
                            IOAccess.WriteErrLog("getHKJCTeamPageDs p1 error," + sWhere + "," + ex.Message);
                            rsCount = 0;
                        }
                    }
                }

                DataSet ds = new DataSet();
                using (FbCommand selectData = cn.CreateCommand())
                {
                    StringBuilder builder = new StringBuilder("select first " + pageSize);
                    if (pageIndex > 1)
                    {
                        builder.Append(" skip " + pageSize * (pageIndex - 1) + " ");
                    }
                    builder.Append(" a.ITEAM_CODE, a.CCHI_NAME, a.CENG_NAME, a.CSHT_ENG_NAME FROM TEAMLIST a " + sWhere + "  order by ITEAM_CODE;");

                    selectData.CommandText = builder.ToString();// "select ITEAM_CODE,CCHI_NAME,CENG_NAME,ENET_TEAM_NAME from TEAMLISTMAPPING where ENET_TEAM_NAME=@enetHost or ENET_TEAM_NAME=@enetGuest";                  
                    FbDataAdapter adapter = new FbDataAdapter(selectData);
                    adapter.Fill(ds);
                    adapter.Dispose();
                }
                cn.Close();

                return ds;
            }
            catch (Exception ex)
            {
                IOAccess.WriteErrLog("getHKJCTeamPageDs p2 error," + searchClause + "," + ex.Message);

                cn.Close();
            }

            return null;
        }
    }


    public class TeamMappingModel
    {
        public string ITEAM_CODE;
        public string CCHI_NAME;
        public string CENG_NAME;
        public string CSHT_ENG_NAME;
        public string ENET_TEAM_NAME;
        public string ENET_TEAM_ID;
    }


    public class IOAccess
    {
        public static void WriteErrLog(string Msg)
        {
            try
            {
                string LogPath = AppDomain.CurrentDomain.BaseDirectory.ToString() + @"\ENETLog\";
                string filename = "Err" + System.DateTime.Now.ToString("yyyyMMdd") + ".txt";
                if (!Directory.Exists(LogPath))
                {
                    Directory.CreateDirectory(LogPath);
                }
                if (!File.Exists(LogPath + filename))
                {
                    FileStream fs = File.Create(LogPath + filename);
                    fs.Close();
                }

                System.Text.Encoding code = System.Text.Encoding.Default;//.GetEncoding("gb2312");
                StreamWriter writer = new StreamWriter(LogPath + filename, true, code);
                writer.Write("\n\r时间:" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ssss") + " 消息:" + Msg + "\n\r");
                writer.Flush();
                writer.Close();
            }
            catch (Exception ex)
            {
                // MessageBox.Show("写日志失败，忽略继续");
                //AddMsgToListBox("写日志失败，忽略继续");
            }
        }
    }
}