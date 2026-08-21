using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace TrainingSystem
{
    public partial class status : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        [System.Web.Services.WebMethod]
        public static string GetScheduleList()
        {
            string result = "";

            string sql = @"SELECT st.ID
      ,SiteName
      ,TeamID
	  ,ts.Schedule
	  ,ts.TimeStamp
  FROM  sop_3.Site as st
  INNER JOIN  sop_3.publicschedule as ts on ts.SiteID = st.ID
  ORDER BY ts.TimeStamp DESC
 limit 5";


            MySqlConnection dbConnection;
            MySqlDataReader dr;
            training.runDatabase(sql, out dbConnection, out dr);

            while (dr.Read())
            {
                result += "<li><a href=''>[" + ((DateTime)dr["TimeStamp"]).ToShortDateString() + "] " + dr["SiteName"].ToString() + "<span class='small'>- " + dr["Schedule"].ToString() + "</span><span class='file'><img src='images/icon_file.png'></span></a></li>";
            }

            dr.Close();
            dbConnection.Close();

            return result;
        }

        [System.Web.Services.WebMethod]
        public static string UpdateActionHistoryConfirm(string actionHistoryId)
        {
            string dateNow = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            string sql = @"UPDATE ActionStepHistory
SET ConfirmAlert = 1 WHERE ID=" + actionHistoryId;

            MySqlConnection dbConnection;
            MySqlDataReader dr;
            training.runDatabase(sql, out dbConnection, out dr);

            dr.Close();
            dbConnection.Close();

            return "";
        }

        [System.Web.Services.WebMethod]
        public static string CheckDisaster()
        {
            string sql = @"Select ash.ID, ash.ActionStepID, 
ash.RealMode, 
ash.BeginTime, 
st.SiteName,
dis.DisasterName
from ActionStepHistory as ash
INNER JOIN ActionStep as step on step.ID = ash.ActionStepID and ash.EndTime is null and CancelTime is null and ConfirmAlert = 0
INNER JOIN Disaster as dis on step.DisasterID = dis.ID
INNER JOIN SubDisasterCategory as sdc on dis.SubDisasterID = sdc.ID
INNER JOIN DisasterCategory as dc on dc.ID = sdc.DisasterID
INNER JOIN Site as st on st.ID = dc.SiteID
ORDER BY ash.ID DESC";

            MySqlConnection dbConnection;
            MySqlDataReader dr;
            training.runDatabase(sql, out dbConnection, out dr);

            string result = "";

            if(dr.Read())
            {
                result = ((DateTime)dr["BeginTime"]).ToString() + "###" + (string)dr["DisasterName"] + "###" + (string)dr["SiteName"] + "###" + (int)dr["ID"];
            }

            dr.Close();
            dbConnection.Close();

            return result;
        }
    }
}