namespace JC_SoccerWeb
{
    using System;
    using System.Collections;
    using System.Web.UI;

    public class CommonPage : Page
    {
        protected override void OnInit(EventArgs e)
        {
            //if (this.Session[this.Session.SessionID] == null)
            //{
            //    this.Session[this.Session.SessionID] = "s";
            //}
            Hashtable hashtable = (Hashtable) base.Application["Online"];
            if (hashtable != null)
            {
                IDictionaryEnumerator enumerator = hashtable.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    if ((enumerator.Key != null) && enumerator.Key.ToString().Equals(this.Session.SessionID))
                    {
                        if ((enumerator.Value == null) || !"XXXXXX".Equals(enumerator.Value.ToString()))
                        {
                            break;
                        }
                        hashtable.Remove(this.Session.SessionID);
                        base.Application.Lock();
                        base.Application["Online"] = hashtable;
                        base.Application.UnLock();
                        string format = "<script language=javascript>window.location.replace('{0}')</script>";
                        base.Response.Write(string.Format(format, "Default.aspx?redtype=unonly"));
                        return;
                    }
                }
            }
            RegisterStartupScript("key", " <script language='javascript'>setTimeout(\"window.location.replace('Default.aspx?redtype=again')\"," + (Session.Timeout * 60 * 0x3e8).ToString() + "); </script> ");
        }

        private void Page_Load(object sender, EventArgs e)
        {
        }
    }
}

