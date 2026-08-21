using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MySql.Data.MySqlClient;


namespace TrainingSystem
{
    public partial class main : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        [System.Web.Services.WebMethod]
        public static string UpdateMessageTemplate(string message,string disasterCategory,string siteId)
        {
            string result = "";



            return result;
        }

        public static string AddMessageHistory(string sendTime, string message, string memberId, string actionStepId, string actionStepHistoryId)
        {
            string result = "";



            return result;
        }

        public static int GetLastActionStepId()
        {
            string sql = @"SELECT ID
FROM  sop_3.ActionStep
ORDER BY ID DESC";

            MySqlConnection dbConnection;
            MySqlDataReader dr;
            runDatabase(sql, out dbConnection, out dr);

            int id = 0;

            if (dr.Read())
            {
                id = (int)dr["ID"];
            }

            dr.Close();
            dbConnection.Close();

            return id;
        }

        public static int GetLastActionStepHistoryId()
        {
            string sql = @"SELECT ID
FROM  sop_3.ActionStepHistory
ORDER BY ID DESC";

            MySqlConnection dbConnection;
            MySqlDataReader dr;
            runDatabase(sql, out dbConnection, out dr);

            int id = 0;

            if (dr.Read())
            {
                id = (int)dr["ID"];
            }

            dr.Close();
            dbConnection.Close();

            return id;
        }

        public static int GetActionStepId(int disasterId)
        {        

            string sql = @"SELECT ID
      ,DisasterID
  FROM  sop_3.ActionStep
  WHERE DisasterID = " + disasterId;

            MySqlConnection dbConnection;
            MySqlDataReader dr;
            runDatabase(sql, out dbConnection, out dr);

            int actionStepId = -1;

            if(dr.Read())
            {
                actionStepId = (int)dr["ID"];
            }

            dr.Close();
            dbConnection.Close();

            return actionStepId; 
        }

        public static string AddActionStepHistory(string actionStepId, string beginTime, string position, string lastAccessUserId)
        {
            string result = "";



            return result;
        }
        [System.Web.Services.WebMethod]
        public static string UpdateSmsTemplate(string message,string disasterType,string siteId)
        {
            string result = "";

            string sql = @"INSERT INTO  sop_3.messagetemplate ";

            sql = sql + string.Format("VALUES(NULL,'{0}','{1}',{2})", message, disasterType, siteId);

            MySqlConnection dbConnection;
            MySqlDataReader dr;
            runDatabase(sql, out dbConnection, out dr);

            dr.Close();
            dbConnection.Close();

            return result;
        }



        [System.Web.Services.WebMethod]
        public static string UpdateBroadcastTemplate(string text, string useSiren,string repeatCount,string disasterType,string siteId)
        {
            string result = "";

            string sql = @"INSERT INTO  sop_3.broadcasttemplate ";

            sql = sql + string.Format("VALUES(NULL,'{0}',{1},{2},{3},{4})", text, useSiren, repeatCount, siteId, disasterType);

            MySqlConnection dbConnection;
            MySqlDataReader dr;
            runDatabase(sql, out dbConnection, out dr);

            dr.Close();
            dbConnection.Close();

            return result;
        }

        private static void runDatabase(string sql, out MySqlConnection dbConnection, out MySqlDataReader dr)
        {

            dbConnection = new MySqlConnection("server=192.168.0.182;port=3306;uid=sa;pwd=9449966Ab;database=SOP_3;");            
            dbConnection.Open();
            MySqlCommand cmd;

            cmd = new MySqlCommand(sql, dbConnection);


            dr = cmd.ExecuteReader();
        }

        [System.Web.Services.WebMethod]
        public static string GetSmsTemplate(string siteId,string disasterCategory)
        {
            string result = "";

            string sql = @"SELECT ID
      ,Message
      ,DisasterCategory
      ,SiteID
  FROM  sop_3.messagetemplate
  WHERE DisasterCategory = " + disasterCategory + " AND SiteID = " + siteId + " ORDER BY ID DESC";

            MySqlConnection dbConnection;
            MySqlDataReader dr;
            runDatabase(sql, out dbConnection, out dr);

            if (dr.Read())
            {
                //첫번째 것이 최신
                result = (string)dr["Message"];                
            }

            dr.Close();
            dbConnection.Close();

            return result;
        }

        [System.Web.Services.WebMethod]
        public static string GetBroadcastTemplate(string siteId,string disasterCategory)
        {
            string result = "";

            string sql = @"SELECT ID
      ,Text
      ,UseSiren
      ,RepeatCount
      ,SiteID
      ,DisasterCategory
  FROM  sop_3.broadcasttemplate
WHERE SiteID=" + siteId + " AND " + "DisasterCategory = " + disasterCategory + " ORDER BY ID DESC";

            MySqlConnection dbConnection;
            MySqlDataReader dr;
            runDatabase(sql, out dbConnection, out dr);


            if (dr.Read())
            {
                //첫번째 것이 최신
                string text = (string)dr["Text"];
                int useSiren = int.Parse(dr["UseSiren"].ToString());
                int repeadCount = (int)dr["RepeatCount"];                

                result = string.Format("{0}###{1}###{2}", text, useSiren, repeadCount);
            }

            dr.Close();
            dbConnection.Close();

            return result;
        }

        [System.Web.Services.WebMethod]
        public static string DeleteTrainningSchedule(string id)
        {
            string sql = @"DELETE FROM  sop_3.publicschedule WHERE ID=" + id;

            MySqlConnection dbConnection;
            MySqlDataReader dr;
            runDatabase(sql, out dbConnection, out dr);

            dr.Close();
            dbConnection.Close();

            return "OK";
        }

        [System.Web.Services.WebMethod]
        public static string SaveNewSchedule(string siteId,string date,string text)
        {
            string sql = @"INSERT INTO  sop_3.publicschedule ";

            sql = sql + string.Format("VALUES(NULL,'{0}','{1}',{2})", date, text, siteId);

            MySqlConnection dbConnection;
            MySqlDataReader dr;
            runDatabase(sql, out dbConnection, out dr);

            dr.Close();
            dbConnection.Close();

            return "";
        }

        [System.Web.Services.WebMethod]
        public static string GetTrainningScheduleList()
        {
            string result = "";
            string sql = @"SELECT st.ID
      ,st.SiteName
      ,TeamID
	  ,ts.Schedule
	  ,ts.TimeStamp
	  ,ts.ID as ScheduleId
  FROM  sop_3.Site as st
  INNER JOIN  sop_3.publicschedule as ts on ts.SiteID = st.ID
  ORDER BY ts.TimeStamp 
limit 5";

            MySqlConnection dbConnection;
            MySqlDataReader dr;
            runDatabase(sql, out dbConnection, out dr);

            while(dr.Read())
            {
                string html = @"<tr>
                                <td >" + ((DateTime)dr["TimeStamp"]).ToShortDateString() + @" </td>
                                <td class='bbs-title' >" +dr["Schedule"].ToString() + @"</td>
                                <td class='btn-delete'><img src='images/icon_delete.png' onclick='deleteSchedule("+ dr["ScheduleId"].ToString()+  @")'/></td>
                            </tr>";



                result += html;
            }

            //마지막에 추가할수 있는 라인 추가

            string addHtml = @"<tr>
                                <td><input type='text' id='datepicker1' onclick='showDatePicker();'></td>
                                <td class='bbs-title'><input type='text' id='newScheduleText' style='border: none; width: 100%; -webkit-box-sizing: border-box; -moz-box-sizing: border-box; box-sizing: border-box;'></td>
                                <td></td>
                            </tr>";

            result += addHtml;

            dr.Close();
            dbConnection.Close();

            return result;
        }
    }
}