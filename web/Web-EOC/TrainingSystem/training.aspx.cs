using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MySql.Data.MySqlClient;
using System.Diagnostics;
using MySql.Data.MySqlClient;

namespace TrainingSystem
{
    public partial class training : System.Web.UI.Page
    {
        public string SelectedBuildingName = "";
        private static SensorTester.SensorSignal sensorSignal = new SensorTester.SensorSignal(3, "192.168.0.182");
            
        protected void Page_Load(object sender, EventArgs e)
        {
            sensorSignal.ConnectServer();
        }

        public string UpdateBuildingDropDownServerSide()
        {

            string sql = @"Select bd.ID, BuildingID,BuildingName,MaxFloor,MinFloor,BroadCastingText, bd.DisplayText 
from  sop_3.Building as bd
INNER JOIN  sop_3.BuildingGroup as bdg on bd.BuildingGroupID = bdg.ID
where bdg.SiteID = 3
limit 20";

            MySqlConnection dbConnection;
            MySqlDataReader dr;
            runDatabase(sql, out dbConnection, out dr);

            string message = "";

            while (dr.Read())
            {
                string buildingName = (string)dr["BuildingName"];
                buildingName = buildingName.Replace("\"", "");
                buildingName = buildingName.Replace("'", "");
                buildingName = buildingName.Replace(",", "_");
                message += buildingName + "," + (int)dr["ID"] + "," + (int)dr["MinFloor"] + "," + (int)dr["MaxFloor"] + "@@";
            }

            // + (int)dr["MaxFloor"] + "," + (int)dr["MinFloor"] + "@@"
        
            dr.Close();
            dbConnection.Close();
            DataBind();

            //return  "'" + message + "'";

            hdf_Test.Value = message;

            return message;
        }



        public string GetMinMaxFloor()
        {
            string buildingName = Request.Form["accomodationAnswer"];

            if (null == buildingName || buildingName.Length == 0)
                return "";



            string sql = @"Select BuildingID,BuildingName,MaxFloor,MinFloor,BroadCastingText, bd.DisplayText 
from building as bd
INNER JOIN BuildingGroup as bdg on bd.BuildingGroupID = bdg.ID
where bdg.SiteID = 3 and BuildingName='" + buildingName + "'";

            MySqlConnection dbConnection;
            MySqlDataReader dr;
            runDatabase(sql, out dbConnection, out dr);

            string result = "";

            if(dr.Read())
            {
                int minFloor = (int)dr["MinFloor"];
                int maxFloor = (int)dr["MaxFloor"];

                result = minFloor.ToString() + "," + maxFloor.ToString();
            }

            dr.Close();
            dbConnection.Close();
            DataBind();

            return result;
        }

        [System.Web.Services.WebMethod]
        public static string CheckDisaster()
        {
            return status.CheckDisaster();
        }

        [System.Web.Services.WebMethod]
        public static string SendSms(string text,string receiver)
        {
            //ProcessStartInfo startinfo = new ProcessStartInfo();
            //startinfo.Arguments = string.Format("{0} {1} {2} \"{3}\"","10.131.5.6","027144133",receiver, text);
            //startinfo.FileName = @"C:\Users\user\Documents\Visual Studio 2013\Projects\Trainingsystem\AspNetCaller\bin\Debug\AspNetCaller.exe";
            //startinfo.WorkingDirectory = System.IO.Path.GetDirectoryName(startinfo.FileName);
            //Process myProcess = Process.Start(startinfo);

            _sendSms(text, 3);

            return "";
        }


        [System.Web.Services.WebMethod]
        public static string InsertBroadcastMessage(string text,string useSiren,string repeatCount,string siteId)
        {
            string result = "OK";

            string sql = @"INSERT INTO  sop_3.Broadcast ";

            string dateNow = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            sql = sql + string.Format("VALUES('{0}',{1},{2},{3},'{4}',{5})", text, useSiren, 1, repeatCount, dateNow, siteId); 


            MySqlConnection dbConnection;
            MySqlDataReader dr;
            runDatabase(sql, out dbConnection, out dr);

            dr.Close();
            dbConnection.Close();

            InsertBroadcastHistory(text, useSiren, repeatCount, siteId);

            return result;
        }

        private static void _sendSms(string message,int siteId)
        {
            libSMS.IMessageClient client = libSMS.MessageClientFactory.CreateMessageClient(siteId, "127.0.0.1");

            client.SendSMS("02714133", "01045414731",message);
            client.SendSMS("02714133", "01025893257", message);
            client.SendSMS("02714133", "01073036070", message);
        }

        private static string InsertBroadcastHistory(string text,string useSiren,string repeatCount,string siteId)
        {
            string result = "OK";

            string sql = @"INSERT INTO  sop_3.broadcasthistory ";

            string dateNow = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            sql = sql + string.Format("VALUES(NULL,'{0}',{1},{2},{3},'{4}','{5}',{6})", text, useSiren, 1, repeatCount,"" ,dateNow, siteId);


            MySqlConnection dbConnection;
            MySqlDataReader dr;
            runDatabase(sql, out dbConnection, out dr);

            dr.Close();
            dbConnection.Close();

            return result;
        }

        private static void sendSensorSignal(string sensorId,string sensorTypeId)
        {
            if (sensorTypeId == "101") //화재
                sensorSignal.SendSensorActivate(int.Parse(sensorId), 1,false);            
            else if (sensorTypeId == "11") //누출
                sensorSignal.SendSensorActivate(int.Parse(sensorId), 1, true);            
        }

        [System.Web.Services.WebMethod]
        public static string InsertTrainningSensorActivation(string equipZoneId,string sensorTypeId)
        {
            string sensorList = GetSensorZoneList(equipZoneId);

            string [] seperator = {"###"};

            string[] sensorElements = sensorList.Split(seperator,StringSplitOptions.RemoveEmptyEntries);

            if(sensorElements.Length > 0)
            {
                //_insertTrainningSensorActivation(sensorElements[0]); //대표 센서 하나만 신호를 발송한다.
                sendSensorSignal(sensorElements[0],sensorTypeId);
            }
                
            

            //foreach(string sensorId in sensorElements)
            //{
            //    _insertTrainningSensorActivation(sensorId);
            //}

            string result = "OK";

            return result;
        }

        private static string _insertTrainningSensorActivation(string sensorZoneId)
        {
            string sql = @"INSERT INTO  sop_3.TrainningSensorActivation ";

            string dateNow = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            sql = sql + string.Format("VALUES('{0}','{1}','{2}')", dateNow, sensorZoneId, "TRUE");

            MySqlConnection dbConnection;
            MySqlDataReader dr;
            runDatabase(sql, out dbConnection, out dr);

            dr.Close();
            dbConnection.Close();

            string result = "";

            return result;
        }
        
        [System.Web.Services.WebMethod]
        public static string GetBuildingList(string buildingGroupId)
        {
            string result = "";

            string sql = @"SELECT ID
      ,BuildingID
      ,BuildingCode
      ,BuildingName
      ,BuildingGroupID
      ,MaxFloor
      ,MinFloor
      ,BroadCastingText
      ,DisplayText
  FROM  sop_3.Building
  WHERE BuildingGroupID = " + buildingGroupId;

            MySqlConnection dbConnection;
            MySqlDataReader dr;
            runDatabase(sql, out dbConnection, out dr);

            if (dr.Read())
            {
                result += (int)dr["ID"] + "###" + (string)dr["DisplayText"];
            }

            while (dr.Read())
            {
                result += "###" + (int)dr["ID"] + "###" + (string)dr["DisplayText"];
            }

            dr.Close();
            dbConnection.Close();

            return result;
        }

        [System.Web.Services.WebMethod]
        public static string GetBuildingGroupList(string siteId)
        {
            string result = "";

            string sql = @"SELECT ID
      ,GroupName
      ,SiteID
      ,TextCenter
      ,DisplayText
  FROM  sop_3.BuildingGroup
  WHERE SiteID = " + siteId;

            MySqlConnection dbConnection;
            MySqlDataReader dr;
            runDatabase(sql, out dbConnection, out dr);


            if(dr.Read())
            {
                result += (int)dr["ID"] + "###" + (string)dr["DisplayText"];
            }

            while(dr.Read())
            {
                result += "###" + (int)dr["ID"] + "###" + (string)dr["DisplayText"];
            }

            dr.Close();
            dbConnection.Close();

            return result;
        }
        [System.Web.Services.WebMethod]
        public static string GetEquipZoneList(string buildingId,string sensorType)
        {
            string sql = @"SELECT ez.ID as EquipZoneId
, ez.DisplayText
,FloorIndex
,zn.BuildingID
  FROM  sop_3.Zone as zn  
  INNER JOIN  sop_3.SensorZone as sz on sz.Zone = zn.ID
  INNER JOIN  sop_3.EquipmentZone as ez on ez.ID = sz.EquipZoneID
  INNER JOIN  sop_3.Building as bd on bd.ID = zn.BuildingID
WHERE bd.ID = " + buildingId + " AND sz.Type =" + sensorType + @" GROUP BY ez.ID,ez.DisplayText,FloorIndex,zn.BuildingID";

            string result = "";

            MySqlConnection dbConnection;
            MySqlDataReader dr;
            runDatabase(sql, out dbConnection, out dr);

            if (dr.Read())
            {
                result += (int)dr["EquipZoneId"] + "###" + (string)dr["DisplayText"];
            }

            while(dr.Read())
            {
                result += "###" + (int)dr["EquipZoneId"] + "###" + (string)dr["DisplayText"];
            }

            dr.Close();
            dbConnection.Close();

            return result;
        }


        [System.Web.Services.WebMethod]
        public static string GetZoneImagePath(string equipZoneId)
        {
            string result = "";

            string sql= @"SELECT ez.ID
	  ,zn.DXFFileName
  FROM  sop_3.EquipmentZone as ez
  INNER JOIN SensorZone as sz on sz.EquipZoneID = ez.ID
  INNER JOIN Zone as zn on zn.ID = sz.Zone
  WHERE ez.ID = " + equipZoneId;

            MySqlConnection dbConnection;
            MySqlDataReader dr;
            runDatabase(sql, out dbConnection, out dr);

            if(dr.Read())
            {
                //if (System.DBNull != dr["DXFFileName"])

                if (Convert.IsDBNull(dr["DXFFileName"]))
                    result = "no image file path.";
                else
                    result = (string)dr["DXFFileName"];
            }

            dr.Close();
            dbConnection.Close();

            return result;
        }
        [System.Web.Services.WebMethod]
        public static string GetSensorZoneList(string equipZoneId)
        {

            string sql = @"SELECT ID
      ,Type
      ,Connected
      ,EquipZoneID
      ,Description
      ,Zone
  FROM  sop_3.SensorZone
  WHERE EquipZoneID = " + equipZoneId;

            MySqlConnection dbConnection;
            MySqlDataReader dr;
            runDatabase(sql, out dbConnection, out dr);

            string result = "";

            if(dr.Read())
            {
                result += dr["ID"] + "###";
            }

            while(dr.Read())
            {
                result += "###" + dr["ID"];
            }

            dr.Close();
            dbConnection.Close();

            return result;
        }


        [System.Web.Services.WebMethod]
        public static string GetSensorZoneList(string buildingId,string floorIndex,string sensorType)
        {
            string sql = @"SELECT zn.ID as ZoneID
	  ,sz.ID as SensorZoneID
      ,ZoneName
      ,zn.SiteID
      ,BuildingID
      ,FloorIndex
      ,DXFFileName
      ,zn.DisplayText
	  ,Description
	  ,sz.Type
  FROM  sop_3.Zone as zn
  INNER JOIN  sop_3.SensorZone as sz on sz.Zone = zn.ID
  where zn.BuildingID = " + buildingId + " and zn.FloorIndex = " + floorIndex + " and sz.[Type] = " + sensorType;

            MySqlConnection dbConnection;
            MySqlDataReader dr;
            runDatabase(sql, out dbConnection, out dr);

            string result = "";            

            while (dr.Read())
            {
                string description = (string)dr["Description"];
                description = description.Replace("'", "");
                description = description.Replace("\"", "");
                description = description.Replace(",", "_");
                result += (int)dr["SensorZoneID"] + "###" + description + "###" +(string)dr["DXFFileName"]+ "@@";
            }

            dr.Close();
            dbConnection.Close();


            //DataBind();

            return result;
        }

        [System.Web.Services.WebMethod]
        public static string GetEquipZoneBroadcastName(string equipmentZoneId)
        {
            string sql = @"SELECT ID
      ,ZoneName
      ,BroadcastName
      ,SiteID
  FROM  sop_3.EquipmentZone
  WHERE ID = " + equipmentZoneId;
            
            string result = "";

            MySqlConnection dbConnection;
            MySqlDataReader dr;
            runDatabase(sql, out dbConnection, out dr);

            if (dr.Read())
            {
                result = (string)dr["BroadcastName"];
            }

            dr.Close();
            dbConnection.Close();

            return result;

        }

        [System.Web.Services.WebMethod]
        public static string GetEquipZoneMaterialName(string equipmentZoneId)
        {
            string sql = @"SELECT psn.ID
      ,SensorName      
      ,MaterialType   
      ,SensorTypeName
	  ,EquipZoneID
	  ,pm.MaterialName as PsmMaterialName
  FROM  sop_3.PSMSensor as psn
 INNER JOIN  sop_3.EquipmentZone as ez on ez.ID = psn.EquipZoneID
 INNER JOIN  sop_3.PSMMaterial as pm on pm.ID = psn.MaterialType
 WHERE ez.ID =" + equipmentZoneId;

            string result = "";

            MySqlConnection dbConnection;
            MySqlDataReader dr;
            runDatabase(sql, out dbConnection, out dr);

            if (dr.Read())
            {
                result = (string)dr["PsmMaterialName"];
            }

            dr.Close();
            dbConnection.Close();

            return result;
        }

        public static void runDatabase(string sql, out MySqlConnection dbConnection, out MySqlDataReader dr)
        {

            dbConnection = new MySqlConnection("server=192.168.0.182;port=3306;uid=sa;pwd=9449966Ab;database=SOP_3;");
            dbConnection.Open();
            MySqlCommand cmd;

            cmd = new MySqlCommand(sql, dbConnection);


            dr = cmd.ExecuteReader();
        }

        [System.Web.Services.WebMethod]
        public static string GetSensorDescriprionByEquipmentZoneId(string equipmentZoneId)
        {
            //text, ntext는 정렬에 사용불가. 형변환이 필요함.
            string sql = @"SELECT sz.Type as SensorType
  ,CAST(ft.Description AS char(255)) as SensorDescription
  FROM  sop_3.EquipmentZone as ez
  INNER JOIN SensorZone as sz on sz.EquipZoneID = ez.ID
  INNER JOIN FacilityType as ft on ft.ID = sz.Type
  WHERE ez.ID = " + equipmentZoneId + @" GROUP BY sz.Type , CAST(ft.Description AS char(255)) ";

            string result = "";

            MySqlConnection dbConnection;
            MySqlDataReader dr;
            runDatabase(sql, out dbConnection, out dr);

            if(dr.Read())
            {
                result = (string)dr["SensorDescription"];
            }

            dr.Close();
            dbConnection.Close();

            return result;
        }

        //public static string InsertSensorHistory(string equipZoneId)
        //{
        //    string result = "";

        //    string sqlSensorZoneList = @"";

        //    MySqlConnection dbConnection;
        //    MySqlDataReader dr;
        //    runDatabase(sqlSensorZoneList, out dbConnection, out dr);

        //    while(dr.Read())
        //    {

        //    }

        //    return result;
        //}

        [System.Web.Services.WebMethod]
        public static string GetSiteNameByEquipZoneId(string equipZoneId)
        {
            string sql = @"SELECT st.ID
      ,SiteName
      ,TeamID
  FROM  sop_3.Site as st
  INNER JOIN EquipmentZone as ez on ez.SiteID = st.ID
  WHERE ez.ID =" + equipZoneId + @" limit 1000";

            string result = "";

            MySqlConnection dbConnection;
            MySqlDataReader dr;
            runDatabase(sql, out dbConnection, out dr);

            if(dr.Read())
            {
                result = (string)dr["SiteName"];

                result = result.Trim();
            }

            dr.Close();
            dbConnection.Close();

            return result;
        }

        [System.Web.Services.WebMethod]
        public static string GetSmsMessage(string siteId, string disasterCategory)
        {
            string result = "";

            result = main.GetSmsTemplate(siteId, disasterCategory);

            return result;
        }

        [System.Web.Services.WebMethod]
        public static string GetBroadcastMessage(string siteId, string disasterType)
        {
            string result = "";

            result = main.GetBroadcastTemplate(siteId, disasterType);

            result += "###" + disasterType;

            return result;
        }

        [System.Web.Services.WebMethod]
        public static string GetTotalSensorZoneList(string siteId, string sensorType)
        {
            string result = "";
            string sql = @"SELECT zn.ID as ZoneID
	  ,sz.ID as SensorZoneID
      ,ZoneName
      ,zn.SiteID
      ,BuildingID
      ,FloorIndex
      ,DXFFileName
      ,zn.DisplayText
	  ,Description
	  ,sz.Type
  FROM  sop_3.Zone as zn
  INNER JOIN  sop_3.SensorZone as sz on sz.Zone = zn.ID
  ORDER BY ZoneID";

            MySqlConnection dbConnection;
            MySqlDataReader dr;
            runDatabase(sql, out dbConnection, out dr);

            int preZoneId = int.MinValue;

            while (dr.Read())
            {
                int zoneId = (int)dr["ZoneID"];
                string zoneName = (string)dr["ZoneName"];

                zoneName = zoneName.Replace(",", "_");

                if(preZoneId != zoneId)
                {
                    result += "@@@" + zoneId + ",," + zoneName; ;
                    preZoneId = zoneId;
                }

                string sensorZoneName = (string)dr["Description"];

                sensorZoneName = sensorZoneName.Replace(",","_");

                int sensorZoneId = (int)dr["SensorZoneID"];

                result += "###" + sensorZoneId +",," +  sensorZoneName;
            }

            dr.Close();
            dbConnection.Close();

            return result;
        }
    }
}