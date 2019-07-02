using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using GOSTS;
using GOSTS.Common;
using System.Data;
using System.Threading;
using WPF.MDI;
using System.ComponentModel;
using System.Collections;
using System.Collections.ObjectModel;

namespace GOSTS
{
    public static class DataRowHelper
    {

        public static string getColValue(this DataRow dr, int ColIndex)
        {
            if (dr.Table.Columns.Count <= ColIndex) return "";
            string str = dr[ColIndex].ToString().Trim();
            return str;
        }

        public static string getColValue(this DataRow dr, string colName)
        {
            colName = colName.Trim();
            if (dr == null) return "";
            if (dr.Table == null) return "";
            if (dr.Table.Columns.Contains(colName) == false) return "";
            string str = dr[colName].ToString().Trim();            
            return str;
        }

        public static int getColIntValue(this DataRow dr, string colName, int defaultValue)
        {
            if (dr.Table.Columns.Contains(colName) == false) return defaultValue;
            string str = dr[colName].ToString();
            str = str.Trim();
            try
            {
                int b = Convert.ToInt32(str);
                return b;
            }
            catch { }
            return defaultValue;
        }

        public static uint getColUIntValue(this DataRow dr, string colName, uint defaultValue)
        {
            if (dr.Table.Columns.Contains(colName) == false) return defaultValue;
            string str = dr[colName].ToString();
            str = str.Trim();
            try
            {
                uint b = Convert.ToUInt32(str);
                return b;
            }
            catch { }
            return defaultValue;
        }

        public static decimal getDecimalValue(this DataRow dr, string colName, decimal defaultValue)
        {
            string str = getColValue(dr, colName);
            try
            {
                decimal b = Convert.ToDecimal(str);
                return b;
            }
            catch { }
            return defaultValue;
        }

        public static DataRowView CopyDrv(this DataRowView drv)
        {
            try
            {
                DataTable dt = drv.Row.Table.Clone();
                DataRow newRow = dt.NewRow();
                newRow.ItemArray = drv.Row.ItemArray;
                dt.Rows.Add(newRow);
                dt.AcceptChanges();
                return dt.DefaultView[0];
            }
            catch (Exception ex)
            {
            }
            return null;
        }
   
    }
}
