namespace JC_SoccerWeb.BLL
{
    using JC_SoccerWeb.Common;
    using JC_SoccerWeb.DAL;
    using System;
    using System.Data;
    using FirebirdSql.Data.FirebirdClient;

    public class SmsActions : IDisposable
    {
        public const string ACTION_ENDTIME_PARM = "@ACTION_ENDTIME";
        public const string ACTION_REMARKS_PARM = "@ACTION_REMARKS";
        public const string ACTION_STARTTIME_PARM = "@ACTION_STARTTIME";
        public const string CACTION_HANDLER_PARM = "@CACTION_HANDLER";
        public const string CSMS_ACTION_CONTENT_PARM = "@CSMS_ACTION_CONTENT";
        public const string CSMS_ACTION_STATUS_PARM = "@CSMS_ACTION_STATUS";
        public const string CSMS_ACTION_TYPE_PARM = "@CSMS_ACTION_TYPE";
        private FbCommand deleteCommand;
        private FbDataAdapter dsCommand = new FbDataAdapter();
        private FbCommand insertCommand;
        public const string ISMS_IREC_NO_PARM = "@ISMS_IREC_NO";
        private FbCommand loadCommand;
        private FbCommand updateCommand;

        public SmsActions()
        {
            this.dsCommand.SelectCommand = new FbCommand();
            this.dsCommand.SelectCommand.Connection = new FbConnection(AppFlag.CENTASMSINTEConn);
            this.dsCommand.TableMappings.Add("Table", "SMS_ACTIONS");
        }

        public bool DeleteGroup(SmsActionData data)
        {
            try
            {
                if (this.dsCommand == null)
                {
                    throw new ObjectDisposedException(base.GetType().FullName);
                }
                this.dsCommand.DeleteCommand = this.GetDeleteCommand();
                this.dsCommand.Update(data, "SMS_ACTIONS");
            }
            catch (Exception exception)
            {
                string str = exception.ToString();
                Files.CicsWriteError(DateTime.Now.ToString("HH:mm:ss") + "   SmsActionData  " + exception.ToString());
            }
            if (data.HasErrors)
            {
                data.Tables["SMS_ACTIONS"].GetErrors()[0].ClearErrors();
                return false;
            }
            data.AcceptChanges();
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
                string cmdText = "DELETE FROM  SMS_ACTIONS   where ISMS_IREC_NO=? ";
                this.deleteCommand = new FbCommand(cmdText, new FbConnection(AppFlag.CENTASMSINTEConn));
                this.deleteCommand.CommandType = CommandType.Text;
                FbParameterCollection parameters = this.deleteCommand.Parameters;
                parameters.Add(new FbParameter("@ISMS_IREC_NO", FbDbType.Integer));
                parameters["@ISMS_IREC_NO"].SourceColumn = "ISMS_IREC_NO";
                parameters["@ISMS_IREC_NO"].SourceVersion = DataRowVersion.Original;
            }
            return this.deleteCommand;
        }

        private FbCommand GetInsertCommand()
        {
            if (this.insertCommand == null)
            {
                string cmdText = "INSERT INTO  SMS_ACTIONS  (CSMS_ACTION_CONTENT,CSMS_ACTION_TYPE,CSMS_ACTION_STATUS,CACTION_HANDLER,ACTION_STARTTIME,ACTION_ENDTIME,ACTION_REMARKS ) VALUES (?,?,?,?,?,?,?)";
                this.insertCommand = new FbCommand(cmdText, new FbConnection(AppFlag.CENTASMSINTEConn));
                this.insertCommand.CommandType = CommandType.Text;
                FbParameterCollection parameters = this.insertCommand.Parameters;
                parameters.Add(new FbParameter("@CSMS_ACTION_CONTENT", FbDbType.VarChar, 50));
                parameters.Add(new FbParameter("@CSMS_ACTION_TYPE", FbDbType.VarChar, 10));
                parameters.Add(new FbParameter("@CSMS_ACTION_STATUS", FbDbType.VarChar, 10));
                parameters.Add(new FbParameter("@CACTION_HANDLER", FbDbType.VarChar, 11));
                parameters.Add(new FbParameter("@ACTION_STARTTIME", FbDbType.VarChar, 0x18));
                parameters.Add(new FbParameter("@ACTION_ENDTIME", FbDbType.VarChar, 0x18));
                parameters.Add(new FbParameter("@ACTION_REMARKS", FbDbType.VarChar, 0xbb8));
                parameters["@CSMS_ACTION_CONTENT"].SourceColumn = "CSMS_ACTION_CONTENT";
                parameters["@CSMS_ACTION_TYPE"].SourceColumn = "CSMS_ACTION_TYPE";
                parameters["@CSMS_ACTION_STATUS"].SourceColumn = "CSMS_ACTION_STATUS";
                parameters["@CACTION_HANDLER"].SourceColumn = "CACTION_HANDLER";
                parameters["@ACTION_STARTTIME"].SourceColumn = "ACTION_STARTTIME";
                parameters["@ACTION_ENDTIME"].SourceColumn = "ACTION_ENDTIME";
                parameters["@ACTION_REMARKS"].SourceColumn = "ACTION_REMARKS";
            }
            return this.insertCommand;
        }

        private FbCommand GetLoadCommand()
        {
            if (this.loadCommand == null)
            {
                this.loadCommand = new FbCommand();
                this.loadCommand.Connection = new FbConnection(AppFlag.CENTASMSINTEConn);
                this.loadCommand.CommandType = CommandType.Text;
            }
            return this.loadCommand;
        }

        private FbCommand GetUpdateCommand()
        {
            if (this.updateCommand == null)
            {
                string cmdText = "UPDATE   SMS_ACTIONS SET   CSMS_ACTION_STATUS=?  , ACTION_ENDTIME=? , ACTION_REMARKS=? where ACTION_STARTTIME=? and ACTION_ENDTIME=?";
                this.updateCommand = new FbCommand(cmdText, new FbConnection(AppFlag.CENTASMSINTEConn));
                this.updateCommand.CommandType = CommandType.Text;
                FbParameterCollection parameters = this.updateCommand.Parameters;
                parameters.Add(new FbParameter("@CSMS_ACTION_STATUS", FbDbType.VarChar, 10));
                parameters.Add(new FbParameter("@ACTION_ENDTIME", FbDbType.VarChar, 0x18));
                parameters.Add(new FbParameter("@ACTION_REMARKS", FbDbType.VarChar, 0xbb8));
                parameters.Add(new FbParameter("@ACTION_STARTTIME", FbDbType.VarChar, 0x18));
                parameters.Add(new FbParameter("oldACTION_ENDTIME_PARM", FbDbType.VarChar, 0x18));
                parameters["@CSMS_ACTION_STATUS"].SourceColumn = "CSMS_ACTION_STATUS";
                parameters["@ACTION_ENDTIME"].SourceColumn = "ACTION_ENDTIME";
                parameters["@ACTION_REMARKS"].SourceColumn = "ACTION_REMARKS";
                parameters["@ACTION_STARTTIME"].SourceColumn = "ACTION_STARTTIME";
                parameters["oldACTION_ENDTIME_PARM"].SourceColumn = "ACTION_ENDTIME";
                parameters["oldACTION_ENDTIME_PARM"].SourceVersion = DataRowVersion.Original;
            }
            return this.updateCommand;
        }

        public bool InsertAction(SmsActionData data)
        {
            try
            {
                if (this.dsCommand == null)
                {
                    throw new ObjectDisposedException(base.GetType().FullName);
                }
                this.dsCommand.InsertCommand = this.GetInsertCommand();
                this.dsCommand.Update(data, "SMS_ACTIONS");
            }
            catch (Exception exception)
            {
                string str = exception.ToString();
                Files.CicsWriteError(DateTime.Now.ToString("HH:mm:ss") + "   SmsActionData  " + exception.ToString());
            }
            if (data.HasErrors)
            {
                data.Tables["SMS_ACTIONS"].GetErrors()[0].ClearErrors();
                return false;
            }
            data.AcceptChanges();
            return true;
        }

        public SmsActionData LoadActionByCondition(string starttime)
        {
            SmsActionData dataSet = new SmsActionData();
            try
            {
                if (this.dsCommand == null)
                {
                    throw new ObjectDisposedException(base.GetType().FullName);
                }
                this.dsCommand.SelectCommand = this.GetLoadCommand();
                this.dsCommand.SelectCommand.CommandText = "SELECT * from SMS_ACTIONS    WHERE ACTION_STARTTIME= '" + starttime + "'";
                this.dsCommand.Fill(dataSet);
            }
            catch (Exception exception)
            {
                string str = exception.ToString();
                Files.CicsWriteError(DateTime.Now.ToString("HH:mm:ss") + "   SmsActionData.LoadActionByCondition  " + exception.ToString());
                return dataSet;
            }
            return dataSet;
        }

        public SmsActionData LoadActionByConditionIrecno(int irecno)
        {
            SmsActionData dataSet = new SmsActionData();
            try
            {
                if (this.dsCommand == null)
                {
                    throw new ObjectDisposedException(base.GetType().FullName);
                }
                this.dsCommand.SelectCommand = this.GetLoadCommand();
                this.dsCommand.SelectCommand.CommandText = "SELECT * from SMS_ACTIONS    WHERE ISMS_IREC_NO= " + irecno.ToString();
                this.dsCommand.Fill(dataSet);
            }
            catch (Exception exception)
            {
                string str = exception.ToString();
                Files.CicsWriteError(DateTime.Now.ToString("HH:mm:ss") + "   SmsActionData.LoadActionByConditionIrecno  " + exception.ToString());
                return dataSet;
            }
            return dataSet;
        }

        public SmsActionData LoadActionByID()
        {
            SmsActionData dataSet = new SmsActionData();
            try
            {
                if (this.dsCommand == null)
                {
                    throw new ObjectDisposedException(base.GetType().FullName);
                }
                this.dsCommand.SelectCommand = this.GetLoadCommand();
                this.dsCommand.SelectCommand.CommandText = "select * from SMS_ACTIONS where ISMS_IREC_NO=( SELECT max(ISMS_IREC_NO) from SMS_ACTIONS) ";
                this.dsCommand.Fill(dataSet);
            }
            catch (Exception exception)
            {
                string str = exception.ToString();
                Files.CicsWriteError(DateTime.Now.ToString("HH:mm:ss") + "   SmsActionData.LoadActionByCondition  " + exception.ToString());
                return dataSet;
            }
            return dataSet;
        }

        public SmsActionData LoadAllAction()
        {
            if (this.dsCommand == null)
            {
                throw new ObjectDisposedException(base.GetType().FullName);
            }
            SmsActionData dataSet = new SmsActionData();
            try
            {
                this.dsCommand.SelectCommand = this.GetLoadCommand();
                this.dsCommand.SelectCommand.CommandText = "SELECT * from SMS_ACTIONS order by ISMS_IREC_NO desc ";
                this.dsCommand.Fill(dataSet);
            }
            catch (Exception exception)
            {
                string str = exception.ToString();
                Files.CicsWriteError(DateTime.Now.ToString("HH:mm:ss") + "   SmsActionData.LoadActionByCondition  " + exception.ToString());
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

        public bool UpdateAction(SmsActionData data)
        {
            try
            {
                if (this.dsCommand == null)
                {
                    throw new ObjectDisposedException(base.GetType().FullName);
                }
                this.dsCommand.UpdateCommand = this.GetUpdateCommand();
                this.dsCommand.Update(data, "SMS_ACTIONS");
            }
            catch (Exception exception)
            {
                string str = exception.ToString();
                Files.CicsWriteError(DateTime.Now.ToString("HH:mm:ss") + "   SmsActionData  " + exception.ToString());
            }
            if (data.HasErrors)
            {
                data.Tables["SMS_ACTIONS"].GetErrors()[0].ClearErrors();
                return false;
            }
            data.AcceptChanges();
            return true;
        }
    }
}

