namespace JC_SoccerWeb.DAL
{
    using System;
    using System.Data;
    using System.Runtime.Serialization;

    public class SmsActionData : DataSet
    {
        public const string ACTION_ENDTIME = "ACTION_ENDTIME";
        public const string ACTION_REMARKS = "ACTION_REMARKS";
        public const string ACTION_STARTTIME = "ACTION_STARTTIME";
        public const string CACTION_HANDLER = "CACTION_HANDLER";
        public const string CSMS_ACTION_CONTENT = "CSMS_ACTION_CONTENT";
        public const string CSMS_ACTION_STATUS = "CSMS_ACTION_STATUS";
        public const string CSMS_ACTION_TYPE = "CSMS_ACTION_TYPE";
        public const string ISMS_IREC_NO = "ISMS_IREC_NO";
        public const string SMS_ACTIONS = "SMS_ACTIONS";

        public SmsActionData()
        {
            this.BuildDataTables();
        }

        private SmsActionData(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        private void BuildDataTables()
        {
            DataTable table = new DataTable("SMS_ACTIONS");
            DataColumnCollection columns = table.Columns;
            columns.Add("ISMS_IREC_NO", typeof(int));
            columns.Add("CSMS_ACTION_CONTENT", typeof(string));
            columns.Add("CSMS_ACTION_TYPE", typeof(string));
            columns.Add("CSMS_ACTION_STATUS", typeof(string));
            columns.Add("CACTION_HANDLER", typeof(string));
            columns.Add("ACTION_STARTTIME", typeof(string));
            columns.Add("ACTION_ENDTIME", typeof(string));
            columns.Add("ACTION_REMARKS", typeof(string));
            base.Tables.Add(table);
        }
    }
}

