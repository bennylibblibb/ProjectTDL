namespace JC_SoccerWeb.BLL
{
    using JC_SoccerWeb.Common;
    using JC_SoccerWeb.DAL;
    using System;
    using System.Data;
    using FirebirdSql.Data.FirebirdClient;
    using System.Security.Cryptography;
    using System.Threading;

    public class Users : IDisposable
    {
        public const string CREATE_DATE_PARM = "@CREATE_DATE";
        private FbCommand deleteCommand;
        public const string DEPARTMENT_PARM = "@DEPARTMENT";
        private FbDataAdapter dsCommand = new FbDataAdapter();
        private FbCommand insertCommand;
        private FbCommand loadCommand;
        private const int saltLength = 4;
        public const string SHAREDGP_RIGHT_PARM = "@SHAREDGP_RIGHT";
        private FbCommand updateCommand;
        public const string USER_EMAIL_PARM = "@USER_EMAIL";
        public const string USER_ID_PARM = "@USER_ID";
        public const string USER_NAME_PARM = "@USER_NAME";
        public const string USER_Pwd_PARM = "@USER_Pwd";
        public const string USER_RIGHT_PARM = "@USER_RIGHT";

        public Users()
        {
            this.dsCommand.SelectCommand = new FbCommand();
            this.dsCommand.SelectCommand.Connection = new FbConnection(AppFlag.HkjcDBConn);
            this.dsCommand.TableMappings.Add("Table", "USER_PROFILE");
        }

        private bool CompareByteArray(byte[] array1, byte[] array2)
        {
            if (array1.Length != array2.Length)
            {
                return false;
            }
            for (int i = 0; i < array1.Length; i++)
            {
                if (array1[i] != array2[i])
                {
                    return false;
                }
            }
            return true;
        }

        private bool ComparePasswords(byte[] storedPassword, byte[] hashedPassword)
        {
            if (((storedPassword == null) || (hashedPassword == null)) || (hashedPassword.Length != (storedPassword.Length - 4)))
            {
                return false;
            }
            byte[] saltValue = new byte[4];
            int num = storedPassword.Length - 4;
            for (int i = 0; i < 4; i++)
            {
                saltValue[i] = storedPassword[num + i];
            }
            byte[] buffer2 = this.CreateSaltedPassword(saltValue, hashedPassword);
            return this.CompareByteArray(storedPassword, buffer2);
        }

        private byte[] CreateSaltedPassword(byte[] saltValue, byte[] unsaltedPassword)
        {
            byte[] array = new byte[unsaltedPassword.Length + saltValue.Length];
            unsaltedPassword.CopyTo(array, 0);
            saltValue.CopyTo(array, unsaltedPassword.Length);
            byte[] buffer2 = SHA1.Create().ComputeHash(array);
            byte[] buffer3 = new byte[buffer2.Length + saltValue.Length];
            buffer2.CopyTo(buffer3, 0);
            saltValue.CopyTo(buffer3, buffer2.Length);
            return buffer3;
        }

        public bool DeleteUser(UserData userdata)
        {
            try
            {
                if (this.dsCommand == null)
                {
                    throw new ObjectDisposedException(base.GetType().FullName);
                }
                this.dsCommand.DeleteCommand = this.GetDeleteCommand();
                this.dsCommand.Update(userdata, "USER_PROFILE");
            }
            catch (Exception exception)
            {
                string str = exception.ToString();
                Files.CicsWriteError(DateTime.Now.ToString("HH:mm:ss") + "   UserData  " + exception.ToString());
            }
            if (userdata.HasErrors)
            {
                userdata.Tables["USER_PROFILE"].GetErrors()[0].ClearErrors();
                return false;
            }
            userdata.AcceptChanges();
            return true;
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing && (this.dsCommand != null))
            {
                if (this.dsCommand.SelectCommand != null)
                {
                    if (this.dsCommand.SelectCommand.Connection != null)
                    {
                        this.dsCommand.SelectCommand.Connection.Dispose();
                    }
                    this.dsCommand.SelectCommand.Dispose();
                }
                this.dsCommand.Dispose();
                this.dsCommand = null;
            }
        }

        private FbCommand GetDeleteCommand()
        {
            if (this.deleteCommand == null)
            {
                string cmdText = "DELETE FROM  USER_PROFILE   where USER_ID=? ";
                this.deleteCommand = new FbCommand(cmdText, new FbConnection(AppFlag.HkjcDBConn));
                this.deleteCommand.CommandType = CommandType.Text;
                FbParameterCollection parameters = this.deleteCommand.Parameters;
                parameters.Add(new FbParameter("@USER_ID", FbDbType.VarChar, 11));
                parameters["@USER_ID"].SourceColumn = "USER_ID";
            }
            return this.deleteCommand;
        }

        private FbCommand GetInsertCommand()
        {
            if (this.insertCommand == null)
            {
                string cmdText = "INSERT INTO  USER_PROFILE  ( USER_ID ,  USER_NAME ,  USER_PWD , USER_RIGHT, SHAREDGP_RIGHT ,  USER_EMAIL ,  DEPARTMENT ,  CREATE_DATE ) VALUES (?,?,?,?,?,?,?,?)";
                this.insertCommand = new FbCommand(cmdText, new FbConnection(AppFlag.HkjcDBConn));
                this.insertCommand.CommandType = CommandType.Text;
                FbParameterCollection parameters = this.insertCommand.Parameters;
                parameters.Add(new FbParameter("@USER_ID", FbDbType.VarChar, 11));
                parameters.Add(new FbParameter("@USER_NAME", FbDbType.VarChar, 0x15));
                parameters.Add(new FbParameter("@USER_Pwd", FbDbType.VarChar, 11));
                parameters.Add(new FbParameter("@USER_RIGHT", FbDbType.VarChar, 9));
                parameters.Add(new FbParameter("@SHAREDGP_RIGHT", FbDbType.VarChar, 1));
                parameters.Add(new FbParameter("@USER_EMAIL", FbDbType.VarChar, 0x33));
                parameters.Add(new FbParameter("@DEPARTMENT", FbDbType.VarChar, 5));
                parameters.Add(new FbParameter("@CREATE_DATE", FbDbType.Date));
                parameters["@USER_ID"].SourceColumn = "USER_ID";
                parameters["@USER_NAME"].SourceColumn = "USER_NAME";
                parameters["@USER_Pwd"].SourceColumn = "USER_PWD";
                parameters["@USER_RIGHT"].SourceColumn = "USER_RIGHT";
                parameters["@SHAREDGP_RIGHT"].SourceColumn = "SHAREDGP_RIGHT";
                parameters["@USER_EMAIL"].SourceColumn = "USER_EMAIL";
                parameters["@DEPARTMENT"].SourceColumn = "DEPARTMENT";
                parameters["@CREATE_DATE"].SourceColumn = "CREATE_DATE";
            }
            return this.insertCommand;
        }

        private FbCommand GetLoadCommand()
        {
            if (this.loadCommand == null)
            {
                this.loadCommand = new FbCommand();
                this.loadCommand.Connection = new FbConnection(AppFlag.HkjcDBConn);
                this.loadCommand.CommandType = CommandType.Text;
            }
            return this.loadCommand;
        }

        private FbCommand GetUpdateCommand()
        {
            if (this.updateCommand == null)
            {
                string cmdText = "UPDATE   USER_PROFILE SET   USER_NAME=? ,  USER_PWD=? ,  USER_RIGHT=? , SHAREDGP_RIGHT=? ,  USER_EMAIL=? ,  DEPARTMENT=? ,  CREATE_DATE=? where USER_ID=?";
                this.updateCommand = new FbCommand(cmdText, new FbConnection(AppFlag.HkjcDBConn));
                this.updateCommand.CommandType = CommandType.Text;
                FbParameterCollection parameters = this.updateCommand.Parameters;
                parameters.Add(new FbParameter("@USER_NAME", FbDbType.VarChar, 0x15));
                parameters.Add(new FbParameter("@USER_Pwd", FbDbType.VarChar, 11));
                parameters.Add(new FbParameter("@USER_RIGHT", FbDbType.VarChar, 9));
                parameters.Add(new FbParameter("@SHAREDGP_RIGHT", FbDbType.VarChar, 1));
                parameters.Add(new FbParameter("@USER_EMAIL", FbDbType.VarChar, 0x33));
                parameters.Add(new FbParameter("@DEPARTMENT", FbDbType.VarChar, 5));
                parameters.Add(new FbParameter("@CREATE_DATE", FbDbType.Date));
                parameters.Add(new FbParameter("@USER_ID", FbDbType.VarChar, 11));
                parameters["@USER_NAME"].SourceColumn = "USER_NAME";
                parameters["@USER_Pwd"].SourceColumn = "USER_PWD";
                parameters["@USER_RIGHT"].SourceColumn = "USER_RIGHT";
                parameters["@SHAREDGP_RIGHT"].SourceColumn = "SHAREDGP_RIGHT";
                parameters["@USER_EMAIL"].SourceColumn = "USER_EMAIL";
                parameters["@DEPARTMENT"].SourceColumn = "DEPARTMENT";
                parameters["@CREATE_DATE"].SourceColumn = "CREATE_DATE";
                parameters["@USER_ID"].SourceColumn = "USER_ID";
                parameters["@USER_ID"].SourceVersion = DataRowVersion.Original;
            }
            return this.updateCommand;
        }

        public UserData GetUserByUSER_ID(string user_id, string password)
        {
            UserData data;
            using (Users users = new Users())
            { 
            	data = users.LoadUserByUSER_ID(user_id);
            } 
            DataRowCollection rows = data.Tables["USER_PROFILE"].Rows;
            if (rows.Count == 1)
            {
                if (password == rows[0]["USER_PWD"].ToString())
                {
                    return data;
                 } 
                return null;
            } 
            return null;
        }

        public UserData GetUserDataBy(string strCondition)
        {
            if (this.dsCommand == null)
            {
                throw new ObjectDisposedException(base.GetType().FullName);
            }
            UserData dataSet = new UserData();
            this.dsCommand.SelectCommand = this.GetLoadCommand();
            this.dsCommand.SelectCommand.CommandText = "SELECT * from USER_PROFILE   " + strCondition + "  ";
            this.dsCommand.Fill(dataSet);
            return dataSet;
        }

        public bool InsertUser(UserData userdata)
        {
            UserData data;
            using (Users users = new Users())
            {
                data = users.LoadUserByUSER_ID(userdata.Tables[0].Rows[0][0].ToString());
            }
            if (data.Tables["USER_PROFILE"].Rows.Count == 1)
            {
                return false;
            }
            try
            {
                if (this.dsCommand == null)
                {
                    throw new ObjectDisposedException(base.GetType().FullName);
                }
                this.dsCommand.InsertCommand = this.GetInsertCommand();
                this.dsCommand.Update(userdata, "USER_PROFILE");
            }
            catch (Exception exception)
            {
                string str = exception.ToString();
                Files.CicsWriteError(DateTime.Now.ToString("HH:mm:ss") + "   UserData  " + exception.ToString());
            }
            if (userdata.HasErrors)
            {
                userdata.Tables["USER_PROFILE"].GetErrors()[0].ClearErrors();
                return false;
            }
            userdata.AcceptChanges();
            return true;
        }

        public UserData LoadUserByUSER_ID(string user_id)
        {
            UserData dataSet = new UserData();
            try
            {
                if (this.dsCommand == null)
                {
                    throw new ObjectDisposedException(base.GetType().FullName);
                }
                this.dsCommand.SelectCommand = this.GetLoadCommand();
                this.dsCommand.SelectCommand.CommandText = "SELECT * from USER_PROFILE    WHERE USER_ID= '" + user_id + "'  ";
                this.dsCommand.Fill(dataSet);
            }
            catch (Exception exception)
            {
                Files.CicsWriteError(DateTime.Now.ToString("HH:mm:ss") + "   UserData LoadUserByUSER_ID  " + user_id + " " + exception.ToString());
                //Thread.Sleep(0xea60);
            }
            finally
            {
                if (this.dsCommand.SelectCommand.Connection.State == ConnectionState.Open)
                {
                    this.dsCommand.SelectCommand.Connection.Close();
                }
            }
            return dataSet;
        }

        public UserData LoadUserByUSER_IDtest(string user_id)
        {
            UserData dataSet = new UserData();
            this.loadCommand = new FbCommand("Select   *   from   USER_PROFILE   where   USER_ID=   ?", new FbConnection(AppFlag.HkjcDBConn));
            FbParameter parameter = new FbParameter("@SystemID", FbDbType.VarChar, 11);
            parameter.Value = user_id;
            this.loadCommand.Parameters.Add(parameter);
            this.dsCommand.SelectCommand = this.loadCommand;
            this.dsCommand.Fill(dataSet, "USER_PROFILE");
            return dataSet;
        }

        public bool UpdateUser(UserData userdata)
        {
            try
            {
                if (this.dsCommand == null)
                {
                    throw new ObjectDisposedException(base.GetType().FullName);
                }
                this.dsCommand.UpdateCommand = this.GetUpdateCommand();
                this.dsCommand.Update(userdata, "USER_PROFILE");
            }
            catch (Exception exception)
            {
                string str = exception.ToString();
                Files.CicsWriteError(DateTime.Now.ToString("HH:mm:ss") + "   UserData  " + exception.ToString());
            }
            if (userdata.HasErrors)
            {
                userdata.Tables["USER_PROFILE"].GetErrors()[0].ClearErrors();
                return false;
            }
            userdata.AcceptChanges();
            return true;
        }

        public static bool UserCheck(string username)
        {
            bool flag = false;
            UserData data = new Users().LoadUserByUSER_ID(username);
            if ((data.Tables[0].Rows.Count > 0) && (data.Tables[0].Rows[0]["USER_RIGHT"].ToString() == AppFlag.CentaSmsAdmin))
            {
                flag = true;
            }
            return flag;
        }

        public static bool UserExtraGroup(string username)
        {
            bool flag = false;
            UserData data = new Users().LoadUserByUSER_ID(username);
            if ((data.Tables[0].Rows.Count > 0) && (data.Tables[0].Rows[0]["USER_RIGHT"].ToString() == AppFlag.CentaSmsExtra))
            {
                flag = true;
            }
            return flag;
        }

        public static bool UserShareGroup(string username)
        {
            bool flag = false;
            UserData data = new Users().LoadUserByUSER_ID(username);
            if ((data.Tables[0].Rows.Count > 0) && (data.Tables[0].Rows[0]["SHAREDGP_RIGHT"].ToString() == "Y"))
            {
                flag = true;
            }
            return flag;
        }
    }
}

