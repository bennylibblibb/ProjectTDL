namespace JC_SoccerWeb.DAL
{
    using System;
    using System.Data;
    using System.Runtime.Serialization;

    public class UserData : DataSet
    {
        public const string CREATE_DATE = "CREATE_DATE";
        public const string DEPARTMENT = "DEPARTMENT";
        public const string SHAREDGP_RIGHT = "SHAREDGP_RIGHT";
        public const string USER_EMAIL = "USER_EMAIL";
        public const string USER_ID = "USER_ID";
        public const string USER_NAME = "USER_NAME";
        public const string USER_PROFILE = "USER_PROFILE";
        public const string USER_Pwd = "USER_PWD";
        public const string USER_RIGHT = "USER_RIGHT";

        public UserData()
        {
            this.BuildDataTables();
        }

        private UserData(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        private void BuildDataTables()
        {
            DataTable table = new DataTable("USER_PROFILE");
            DataColumnCollection columns = table.Columns;
            columns.Add("USER_ID", typeof(string));
            columns.Add("USER_NAME", typeof(string));
            columns.Add("USER_PWD", typeof(string));
            columns.Add("USER_RIGHT", typeof(string));
            columns.Add("SHAREDGP_RIGHT", typeof(string));
            columns.Add("USER_EMAIL", typeof(string));
            columns.Add("DEPARTMENT", typeof(string));
            columns.Add("CREATE_DATE", typeof(DateTime));
            base.Tables.Add(table);
        }
    }
}

