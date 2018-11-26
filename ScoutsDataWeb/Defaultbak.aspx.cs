using System;
using System.Collections.Generic;
using System.Data; 
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using FirebirdSql.Data.FirebirdClient;

namespace JC_SoccerWeb
{
    public partial class Defaults : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                string connectionString = GetConnectionString();

                using (FbConnection connection =
                    new FbConnection(connectionString))
                {
                    //Create a SqlDataAdapter for the Suppliers table.
                    FbDataAdapter adapter = new FbDataAdapter();

                    // A table mapping names the DataTable.
                    adapter.TableMappings.Add("Table", "Suppliers");

                    // Open the connection.
                    connection.Open();
                    Console.WriteLine("The Connection is open.");

                    // Create a SqlCommand to retrieve Suppliers data.
                    FbCommand command = new FbCommand(
                        "SELECT * FROM USER_PROFILE;",
                        connection);
                    command.CommandType = CommandType.Text;

                    // Set the SqlDataAdapter's SelectCommand.
                    adapter.SelectCommand = command;

                    // Fill the DataSet.
                    DataSet dataSet = new DataSet("Suppliers");
                    adapter.Fill(dataSet);
                    GridView1.DataSource = dataSet.Tables[0].DefaultView;
                    GridView1.DataBind();
                    // Close the connection.
                    connection.Close();
                    Console.WriteLine("The SqlConnection is closed.");

                }
            }
            catch ( Exception exp)
            {
                string exps = exp.ToString();
            }
        }

          string GetConnectionString()
        {
            // To avoid storing the connection string in your code, 
            // you can retrieve it from a configuration file.
            return "User=SYSDBA;Password=masterkey;Database=E:\\Project\\Database\\JC_Soccer_ID_iPhone\\DB\\HKJCIPHONE.fdb;DataSource=127.0.0.1;Port=30500;Dialect=3;Charset=NONE;Role=;Connection lifetime=15;Pooling=true;MinPoolSize=0;MaxPoolSize=50;Packet Size=8192;ServerType=0";
        }
    }
}