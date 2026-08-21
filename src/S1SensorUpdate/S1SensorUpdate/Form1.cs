using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Collections;
using DBUtility;
using System.Data.SqlClient;

namespace S1SensorUpdate
{
    public partial class Form1 : Form
    {
        public static DBUtility.WebDBManager s1DbMgr;
        public static DBUtility.WebDBManager uneDbMgr;


        public Form1()
        {
            InitializeComponent();

            textBox_s1Ip.Text = "";
            textBox_s1Name.Text = "";
            textBox_uneIp.Text = "";
            textBox_uneName.Text = "";
            textBox_unePw.Text = "";
            textBox_uneUid.Text = "";
        }
        private void button_run_Click(object sender, EventArgs e)
        {
            try
            {
                int nSiteID = ReadSiteID();

                if (nSiteID > 0)
                {
                    Dictionary<int, EquipmentZone> dicLocationEquipZone = ReadAccessLinkLocation(s1DbMgr);

                    if (dicLocationEquipZone == null)
                        return;

                    string strDBConnection = ReadAccessDBConnectionInfo(nSiteID);

                    if (strDBConnection == null)
                        return;

                    Dictionary<int, AccessDevice> dicDevice = ReadAccessLinkDevice(nSiteID);
                    ReadAccessDeviceList(strDBConnection, dicLocationEquipZone, dicDevice, nSiteID);
                }
                /*s1DbMgr = new DBUtility.WebDBManager(100);
                s1DbMgr.DatabaseName = textBox_s1Name.Text;
                s1DbMgr.WebServerURL = "http://" + textBox_s1Ip.Text + ":8080/SOP";
                s1DbMgr.DatabaseType = DBUtility.WebDBManager.DBType.sqlserver;
                //TODO: 연결 성공여부 체크
                  
                System.Collections.ArrayList externalDeviceArr =
                    s1DbMgr.GetResultData("select DeviceID, DeviceName, LocationID, LocationName, EqTypeID, EqTypeName from View_External_Device", 0);
                if (externalDeviceArr == null || externalDeviceArr.Count == 0) throw new ApplicationException("S1 View_External_Device 정보가 없습니다.");

                string strConn = string.Format("Server={0};Database={1};Uid={2};Pwd={3};", textBox_uneIp.Text, textBox_uneName.Text, textBox_uneUid.Text, textBox_unePw.Text);
                using (MySql.Data.MySqlClient.MySqlConnection conn = new MySql.Data.MySqlClient.MySqlConnection(strConn))
                {
                    conn.Open();
                    if (conn.State != ConnectionState.Open) throw new ApplicationException("MySql 연결 실패");
                    MySql.Data.MySqlClient.MySqlCommand cmd;
                    MySql.Data.MySqlClient.MySqlDataReader rdr;
                    DataTable table; 

                    //EquipZoneId 구하기
                    StringBuilder sb = new StringBuilder();
                    sb.Append("SELECT LocationID, EquipZoneID, LinkedZoneIdList ");
                    sb.Append("  FROM AccessLink_View_External_Location as loc ");
                    sb.Append(" INNER JOIN EquipmentZone as ez ON loc.EquipZoneID = ez.ID ");
                    sb.Append(" WHERE loc.siteID=100 ");

                    cmd = new MySql.Data.MySqlClient.MySqlCommand(sb.ToString(), conn);
                    rdr = cmd.ExecuteReader();
                    table = new DataTable();
                    table.Load(rdr);

                    if (table == null || table.Rows.Count == 0) throw new ApplicationException("UNE AccessLink_View_External_Location 정보가 없습니다."); 

                    Dictionary<int, AccessLinkLocation> dicLocationLink = new Dictionary<int, AccessLinkLocation>();
                    foreach (DataRow row in table.Rows)
                    {
                        if (!dicLocationLink.ContainsKey(Convert.ToInt32(row["LocationID"])))
                            dicLocationLink.Add(Convert.ToInt32(row["LocationID"]), new AccessLinkLocation(Convert.ToInt32(row["EquipZoneID"]), Convert.ToInt32(row["LinkedZoneIdList"])));    
                    } 

                    //FacilityType 갯수 체크
                    cmd = new MySql.Data.MySqlClient.MySqlCommand("SELECT ID FROM FacilityType WHERE LinkedTableName = 'S1Access'", conn);
                    rdr = cmd.ExecuteReader();
                    table = new DataTable();
                    table.Load(rdr);

                    List<int> facilityTypeList = new List<int>();
                    if (table != null || table.Rows.Count > 0)
                    {                       
                        foreach (DataRow row in table.Rows)
                        {
                            facilityTypeList.Add(Convert.ToInt32(row["ID"]));
                        }
                    }  

                    //Device와 S1Access Link
                    Dictionary<int, int> dicAccessLinkDevice = new Dictionary<int, int>();
                    cmd = new MySql.Data.MySqlClient.MySqlCommand("SELECT DeviceID, S1AccessID FROM AccessLink_View_External_Device;", conn);
                    rdr = cmd.ExecuteReader();
                    table = new DataTable();
                    table.Load(rdr);

                    if (table != null || table.Rows.Count > 0)
                    {
                        foreach (DataRow row in table.Rows)
                        {
                            if (!dicAccessLinkDevice.ContainsKey(Convert.ToInt32(row["DeviceID"])))
                                dicAccessLinkDevice.Add(Convert.ToInt32(row["DeviceID"]), Convert.ToInt32(row["S1AccessID"]));
                        } 
                    }
                    int s1AccessId = 0;
                    int sensorServerId = 0;
                    int sensorZoneId = 0;
                    int sensorTagInfoId = 0;
                    int tagNo = 0;
                    cmd = new MySql.Data.MySqlClient.MySqlCommand("SELECT IFNULL((SELECT MAX(IFNULL(ID, 0))+1 FROM S1Access),1);", conn);
                    using (rdr = cmd.ExecuteReader())
                    {
                        rdr.Read();
                        s1AccessId = Convert.ToInt32(rdr[0]);
                    } 

                    cmd = new MySql.Data.MySqlClient.MySqlCommand("SELECT IFNULL((SELECT MAX(IFNULL(ID, 0))+1 FROM SensorServerInfo),1);", conn);
                    using (rdr = cmd.ExecuteReader())
                    {
                        rdr.Read();
                        sensorServerId = Convert.ToInt32(rdr[0]);
                    }

                    cmd = new MySql.Data.MySqlClient.MySqlCommand("SELECT IFNULL((SELECT MAX(IFNULL(ID, 0))+1 FROM SensorZone),1);", conn);
                    using (rdr = cmd.ExecuteReader())
                    {
                        rdr.Read();
                        sensorZoneId = Convert.ToInt32(rdr[0]);
                    }
                     
                    cmd = new MySql.Data.MySqlClient.MySqlCommand("SELECT IFNULL((SELECT MAX(IFNULL(ID, 0))+1 FROM SensorTagInfo),1);", conn);
                    using (rdr = cmd.ExecuteReader())
                    {
                        rdr.Read();
                        sensorTagInfoId = Convert.ToInt32(rdr[0]);
                    }
                     
                    cmd = new MySql.Data.MySqlClient.MySqlCommand("SELECT IFNULL((SELECT MAX(IFNULL(TagNo, 0))+1 FROM SensorTagInfo),1);", conn);
                    using (rdr = cmd.ExecuteReader())
                    {
                        rdr.Read();
                        tagNo = Convert.ToInt32(rdr[0]);
                    }
                     
                    sb = new StringBuilder();
                    for (int i = 0; i < externalDeviceArr.Count; i += 6)
                    {
                        int deviceId = Convert.ToInt32(externalDeviceArr[i]);
                        string name = (externalDeviceArr[i + 1].ToString() == "null") ? string.Empty : externalDeviceArr[i + 1].ToString();
                        int locationId = Convert.ToInt32(externalDeviceArr[i + 2]);
                        string positionName = (externalDeviceArr[i + 3].ToString() == "null") ? string.Empty : externalDeviceArr[i + 3].ToString();
                        int zoneId = 0;
                        int equipZoneId = 0;
                        if (dicLocationLink.ContainsKey(locationId)) zoneId = dicLocationLink[locationId].ZoneId;
                        if (dicLocationLink.ContainsKey(locationId)) equipZoneId = dicLocationLink[locationId].EquipZoneId;
                        int isIndoor = 1;
                        string description = (externalDeviceArr[i + 5].ToString() == "null") ? string.Empty : externalDeviceArr[i + 5].ToString();

                        bool isChg = false; //변경 여부
                        //UNE 데이터에 있을때 = 변경사항 CHECK 하고 UPDATE
                        if (dicAccessLinkDevice.ContainsKey(deviceId))
                        {
                            isChg = false;

                            StringBuilder sb2 = new StringBuilder();
                            sb2.Append("SELECT ID, Name, PositionName, ZoneID, Description ");
                            sb2.Append("  FROM S1Access ac ");
                            sb2.Append(" INNER JOIN AccessLink_View_External_Device dev ");
                            sb2.Append("    ON ac.ID=dev.s1accessid ");
                            sb2.Append(" WHERE dev.DeviceID=" + deviceId);

                            cmd = new MySql.Data.MySqlClient.MySqlCommand(sb2.ToString(), conn);
                            using (rdr = cmd.ExecuteReader())
                            {
                                rdr.Read();
                                if (rdr == null) continue;

                                //데이터가 변경되었다면 관련 데이터 삭제하고 다시 Insert
                                if (rdr["Name"].ToString() != name || rdr["PositionName"].ToString() != positionName || Convert.ToInt32(rdr["ZoneID"]) != zoneId || rdr["Description"].ToString()!= description)
                                {
                                    isChg = true;
                                    sb.Append("DELETE FROM SensorTagInfo WHERE SensorServerID=" + dicAccessLinkDevice[deviceId] + ";");
                                    sb.Append("DELETE FROM SensorZone WHERE OrgSensorID=" + dicAccessLinkDevice[deviceId] + ";");
                                    sb.Append("DELETE FROM SensorServerInfo WHERE ID=" + dicAccessLinkDevice[deviceId] + ";");
                                    sb.Append("DELETE FROM S1Access WHERE ID=" + dicAccessLinkDevice[deviceId] + ";");
                                    sb.Append("DELETE FROM AccessLink_View_External_Device WHERE DeviceID=" + deviceId + ";");
                                }
                            }
                        }

                        if (!dicAccessLinkDevice.ContainsKey(deviceId) || isChg) //UNE 데이터에 없을때 = INSERT, UNE에 데이터가 있지만 변경되었을때
                        { 
                            sb.Append("INSERT INTO S1Access (ID, Name, PositionName, X, Y, Z, ZoneID, IsIndoor, Description) ");
                            sb.AppendFormat("VALUES ({0}, '{1}', '{2}', 0, 0, 0, {3}, {4}, '{5}'); ", s1AccessId, name, positionName, zoneId, isIndoor, description);
                             
                            sb.Append("INSERT INTO AccessLink_View_External_Device (DeviceID, S1AccessID, SiteID) ");
                            sb.AppendFormat("VALUES ({0}, {1}, 100); ", deviceId, s1AccessId);
                             
                            sb.Append("INSERT INTO SensorServerInfo (ID, Place, IP, MacAddr, Baudrate, Mode, FlowCtrl, Multiport, Timeout, ReciverID, SiteID) ");
                            sb.AppendFormat("VALUES ({0}, '{1}', '192.168.0.210', '00:90:E8:3B:21:C2', '9600',3, 0, 2, 3000, 1, 100); ", sensorServerId, positionName);
                            
                            for (int f = 0; f < facilityTypeList.Count; f++)
                            {
                                int type = Convert.ToInt32(facilityTypeList[f]);
                                 
                                sb.Append("INSERT INTO SensorZone (ID, Type, Connected, EquipZoneId, Data, Description, OrgSensorId, Zone) ");
                                sb.AppendFormat("VALUES ({0}, {1}, {2}, {3}, {4}, '{5}', {6}, {7}); ", sensorZoneId, type, 0, equipZoneId, 0, name, s1AccessId, zoneId);

                                sb.Append("INSERT INTO SensorTagInfo (ID, SensorServerID, TagNo, SensorName, SensorType, EquipZoneID, SensorZoneID, Description) ");
                                sb.AppendFormat("VALUES ({0}, {1}, {2}, '{3}', {4}, {5}, {6}, '{7}'); ", sensorTagInfoId, sensorServerId, tagNo, name, type, equipZoneId, sensorZoneId, name); 
                                sensorZoneId++;
                                sensorTagInfoId++;
                                tagNo++;
                            }

                            s1AccessId++;
                            sensorServerId++;
                        }
                    }
                    if (sb.Length != 0)
                    {
                        cmd.CommandText = sb.ToString();
                        if (cmd.ExecuteNonQuery() > 0) MessageBox.Show("update 완료");
                    }
                    conn.Close();
                }*/
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private int ReadSiteID()
        {
            DBUtility.Utility util = new DBUtility.Utility();
            string szSiteID = util.getinivalue("Server Connection Info", "siteid");
            if (szSiteID == null || szSiteID == "")
            {
                System.Diagnostics.Trace.WriteLine("Site ID가 지정되지 않았습니다. ini파일을 확인하세요");
                return -1;
            }

            int nSiteId = 1;

            if (int.TryParse(szSiteID, out nSiteId) == false)
            {
                System.Diagnostics.Trace.WriteLine("잘못된 Site ID입니다. ini파일을 확인하세요");
                return -1;
            }

            s1DbMgr = new DBUtility.WebDBManager(nSiteId);
            return nSiteId;
        }

        // Return 값 : Key(Location ID)
        private Dictionary<int, EquipmentZone> ReadAccessLinkLocation(WebDBManager dbMgr)
        {
            string strSQL = "select LocationID, EquipZoneID, ez.ZoneName, ez.LinkedZoneIDList ";
            strSQL += "from accesslink_view_external_location as location, EquipmentZone as ez ";
            strSQL += "where location.EquipZoneID = ez.ID";

            // Key : LocationID
            Dictionary<int, EquipmentZone> dicLocationInfo = new Dictionary<int, EquipmentZone>();
            // Key : EquipZoneID
            Dictionary<int, EquipmentZone> dicEquipZone = new Dictionary<int, EquipmentZone>();
            // Key : ZoneID
            Dictionary<int, List<EquipmentZone>> dicZoneIDs = new Dictionary<int, List<EquipmentZone>>();

            ArrayList arrResult = dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null)
                return null;

            string strZoneIDs = "";

            int nResultCount = arrResult.Count;

            for (int i=0;i<nResultCount-3;i+=4)
            {
                VariousData<int> locationID = WebDBManager.GetIntField(arrResult[i].ToString());
                VariousData<int> equipZoneID = WebDBManager.GetIntField(arrResult[i + 1].ToString());
                string strZoneName = WebDBManager.GetStringField(arrResult[i + 2]);
                string strLinkedZoneIDList = WebDBManager.GetStringField(arrResult[i + 3]);

                if (locationID == null || equipZoneID == null || strZoneName == null || strLinkedZoneIDList == null)
                    continue;

                string[] tokens = strLinkedZoneIDList.Split(',');

                if (tokens.Count() == 0)
                    continue;

                string strFirstZoneID = tokens[0].Trim();
                int nZoneID;

                if (int.TryParse(strFirstZoneID, out nZoneID) == false)
                    continue;

                List<EquipmentZone> equipZones = null;

                if (dicZoneIDs.TryGetValue(nZoneID, out equipZones) == false)
                {
                    equipZones = new List<EquipmentZone>();
                    dicZoneIDs[nZoneID] = equipZones;

                    if (strZoneIDs.Length == 0)
                        strZoneIDs = nZoneID.ToString();
                    else
                        strZoneIDs += ", " + nZoneID.ToString();
                }

                EquipmentZone equipZone = new EquipmentZone();
                equipZone.ID = equipZoneID.Data;
                equipZone.Name = strZoneName;

                equipZones.Add(equipZone);

                dicLocationInfo[locationID.Data] = equipZone;
                dicEquipZone[equipZone.ID] = equipZone;
            }

            if (strZoneIDs.Length > 0)
            {
                strSQL = "Select ID, ZoneName from Zone where ID in (" + strZoneIDs + ")";
                arrResult = dbMgr.GetResultData(strSQL, 0);

                if (arrResult == null)
                    return null;

                nResultCount = arrResult.Count;

                for (int i=0;i<nResultCount-1;i+=2)
                {
                    VariousData<int> id = WebDBManager.GetIntField(arrResult[i].ToString());
                    string strZoneName = WebDBManager.GetStringField(arrResult[i + 1]);

                    if (id == null || strZoneName == null)
                        continue;

                    Zone zone = new Zone();
                    zone.ID = id.Data;
                    zone.Name = strZoneName;

                    List<EquipmentZone> equipZones = null;

                    if (dicZoneIDs.TryGetValue(zone.ID, out equipZones))
                    {
                        foreach (EquipmentZone equipZone in equipZones)
                        {
                            equipZone.LinkedZone = zone;
                        }
                    }
                }
            }

            return dicLocationInfo;
        }

        private string ReadAccessDBConnectionInfo(int nSiteID)
        {
            string strSQL = "Select PropertyValue from OptionSDMS where PropertyName = 'AccessSecurityDBConnection' and SiteID = " + nSiteID.ToString();
            ArrayList arrResult = s1DbMgr.GetResultData(strSQL, 0);

            if (arrResult == null || arrResult.Count == 0)
                return null;

            string strConnectionInfo = WebDBManager.GetStringField(arrResult[0]);

            string[] tokens = strConnectionInfo.Split(';');

            if (tokens.Count() >= 5)
            {
                string strDBType = tokens[0].Trim();
                string strServerURL = tokens[1].Trim();
                string strDatabaseName = tokens[2].Trim();
                string strUserName = tokens[3].Trim();
                string strPassword = tokens[4].Trim();

                if (strDBType.ToUpper() == "SQLSERVER")
                {
                    string strConnection = string.Format("Data Source={0};Initial Catalog={1};User id={2};Password={3};", strServerURL, strDatabaseName, strUserName, strPassword);
                    return strConnection;
                }
            }

            return null;
        }

        // 기존에 등록된 Device 정보를 얻어온다.
        // Key : Device ID
        private Dictionary<int, AccessDevice> ReadAccessLinkDevice(int nSiteID)
        {
            string strSQL = "select DeviceID, S1AccessID, s1.Name ";
            strSQL += "from accesslink_view_external_device as device, S1Access as s1 where device.S1AccessID = s1.ID and SiteID = " + nSiteID.ToString();
            
            // Key : DeviceID
            Dictionary<int, AccessDevice> dicDevice = new Dictionary<int, AccessDevice>();

            ArrayList arrResult = s1DbMgr.GetResultData(strSQL, 0);

            if (arrResult == null)
                return null;

            string strZoneIDs = "";

            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - 2; i += 3)
            {
                VariousData<int> deviceID = WebDBManager.GetIntField(arrResult[i].ToString());
                VariousData<int> accessID = WebDBManager.GetIntField(arrResult[i + 1].ToString());
                string strDeviceName = WebDBManager.GetStringField(arrResult[i + 2]);

                if (deviceID == null || accessID == null)
                    continue;

                if (strDeviceName == null)
                    strDeviceName = "";

                AccessDevice device = new AccessDevice();
                device.ID = deviceID.Data;
                device.S1AccessID = accessID.Data;
                device.Name = strDeviceName;

                dicDevice[deviceID.Data] = device;
            }

            return dicDevice;
        }

        // dicDevice : DeviceID
        private void ReadAccessDeviceList(string strConnection, Dictionary<int, EquipmentZone> dicLocationEquipZone, Dictionary<int, AccessDevice> dicDevice, int nSiteID)
        {
            if (dicDevice == null)
                return;

            List<int> accessTypeList = ReadAccessFacilityTypes();

            if (accessTypeList == null || accessTypeList.Count == 0)
                return;

            SqlConnection accessDBConnection = new SqlConnection();
            accessDBConnection.ConnectionString = strConnection;
            accessDBConnection.Open();

            if (accessDBConnection.State != System.Data.ConnectionState.Open)
                return;

            string strSQL = "select device.DeviceID, DeviceName, EqTypeName, device.LocationID, location.LocationName ";
            strSQL += "from View_External_Device as device, View_External_Location as location ";
            strSQL += "where device.LocationID = location.LocationID";

            SqlCommand cmd = new SqlCommand(strSQL, accessDBConnection);
            SqlDataReader reader = cmd.ExecuteReader();

            if (reader == null)
            {
                accessDBConnection.Close();
                return;
            }

            AccessDevice device = null;

            while (reader.Read())
            {
                int nDeviceID = (int)reader[0];
                string strDeviceName = reader.IsDBNull(1) ? "" : reader[1].ToString();
                string strEqTypeName = reader.IsDBNull(2) ? "" : reader[2].ToString();
                int nLocationID = (int)reader[3];
                string strLocationName = reader.IsDBNull(4) ? "" : reader[4].ToString();

                if (strEqTypeName.Length > 0)
                {
                    if (strDeviceName.Length > 0)
                        strDeviceName += "(" + strEqTypeName + ")";
                    else
                        strDeviceName = strEqTypeName;
                }

                if (dicDevice.TryGetValue(nDeviceID, out device))
                {
                    // 이미 등록된 데이터이다.
                    if (device.Name != strDeviceName)
                        UpdateS1AccessDeviceName(device.S1AccessID, strDeviceName);

                    continue;
                }

                EquipmentZone equipZone = null;

                if (dicLocationEquipZone.TryGetValue(nLocationID, out equipZone))
                {
                    int nS1AccessID = AddS1Access(strDeviceName, equipZone);

                    if (nS1AccessID < 0)
                    {
                        reader.Close();
                        accessDBConnection.Close();
                        return;
                    }

                    if (AddSensorZoneNSensorTagInfos(nS1AccessID, equipZone, accessTypeList) == false)
                    {
                        reader.Close();
                        accessDBConnection.Close();
                        return;
                    }

                    if (AddAccessLinkDevice(nDeviceID, nS1AccessID, nSiteID) == false)
                    {
                        reader.Close();
                        accessDBConnection.Close();
                        return;
                    }
                }
            }

            reader.Close();
            accessDBConnection.Close();
        }

        private void UpdateS1AccessDeviceName(int nS1AccessID, string strDeviceName)
        {
            string strSQL = string.Format("Update S1Access set Name = '{0}' where ID = {1}", strDeviceName, nS1AccessID);
            s1DbMgr.GetResultData(strSQL, 0);
        }

        private bool AddAccessLinkDevice(int nDeviceID, int nS1AccessID, int nSiteID)
        {
            string strSQL = string.Format("Insert into accesslink_view_external_device (DeviceID, S1AccessID, SiteID) values ({0}, {1}, {2})",
                nDeviceID, nS1AccessID, nSiteID);
            return s1DbMgr.GetResultData(strSQL, 0) != null;
        }

        private int GetS1AccessSensorServerInfoID()
        {
            string strSQL = "SELECT ID FROM sensorserverinfo where Place = 'S1AccessServer'";
            ArrayList arrResult = s1DbMgr.GetResultData(strSQL, 0);

            if (arrResult == null || arrResult.Count == 0)
                return -1;

            return WebDBManager.GetIntField(arrResult[0].ToString(), -1);
        }

        private bool AddSensorZoneNSensorTagInfos(int nS1AccessID, EquipmentZone equipZone, List<int> typeList)
        {
            int nSensorZoneMaxID = GetLastID("SensorZone", "Type in (1001, 1002, 1003, 1004, 2000, 2100, 2110, 2200, 2300, 3000, 3004, 3008)");
            int nSensorZoneID = nSensorZoneMaxID < 6000 ? 6000 : nSensorZoneMaxID + 1;

            int nSensorTagInfoMaxID = GetLastID("SensorTagInfo", "SensorType in (1001, 1002, 1003, 1004, 2000, 2100, 2110, 2200, 2300, 3000, 3004, 3008)");
            int nSensorTagInfoID = nSensorTagInfoMaxID < 6000 ? 6000 : nSensorTagInfoMaxID + 1;

            string strFormatSensorZone = "Insert into SensorZone (ID, Type, Connected, EquipZoneID, Data, Description, OrgSensorID, Zone) values ({0}, {1}, null, {2}, 0, '{3}', {4}, {5})";
            string strFormatSensorTagInfo = "Insert into SensorTagInfo (ID, SensorServerID, TagNo, SensorName, SensorType, EquipZoneID, SensorZoneID, Description) values ({0}, {1}, {2}, '{3}', {4}, {5}, {6}, NULL)";

            int nSensorServerInfoID = GetS1AccessSensorServerInfoID();

            foreach (int nFacilityTypeID in typeList)
            {
                string strSQL = string.Format(strFormatSensorZone, nSensorZoneID++, nFacilityTypeID, equipZone.ID, equipZone.Name, nS1AccessID, equipZone.LinkedZone.ID);

                if (s1DbMgr.GetResultData(strSQL, 0) == null)
                    return false;

                if (nSensorServerInfoID > 0)
                {
                    strSQL = string.Format(strFormatSensorTagInfo, nSensorTagInfoID++, nSensorServerInfoID, nSensorZoneID - 1, equipZone.Name, nFacilityTypeID,
                        equipZone.ID, nSensorZoneID - 1);

                    if (s1DbMgr.GetResultData(strSQL, 0) == null)
                        return false;
                }
            }

            return true;
        }

        private int GetLastID(string strTableName, string strCondition = null)
        {
            string strSQL = "Select max(ID) from " + strTableName;

            if (strCondition != null)
                strSQL += " where " + strCondition;

            ArrayList arrResult = s1DbMgr.GetResultData(strSQL, 0);

            if (arrResult == null || arrResult.Count == 0)
                return 0;

            VariousData<int> id = WebDBManager.GetIntField(arrResult[0].ToString());
            return id == null ? 0 : id.Data;
        }

        private int AddS1Access(string strDeviceName, EquipmentZone equipZone)
        {
            int nID = GetLastID("S1Access") + 1;

            string strFormat = "Insert into S1Access (ID, Name, PositionName, X, Y, Z, ZoneID, IsIndoor, Description) values ({0}, '{1}', '{2}', 0.0, 0.0, 0.0, {3}, 1, NULL)";
            string strSQL = string.Format(strFormat, nID, strDeviceName, equipZone.Name, equipZone.LinkedZone.ID);
            return s1DbMgr.GetResultData(strSQL, 0) == null ? -1 : nID;
        }

        private List<int> ReadAccessFacilityTypes()
        {
            string strSQL = "Select ID from FacilityType where LinkedTableName = 'S1Access'";
            ArrayList arrResult = s1DbMgr.GetResultData(strSQL, 0);

            if (arrResult == null)
                return null;

            List<int> typeList = new List<int>();
            int nResultCount = arrResult.Count;

            for (int i=0;i<nResultCount;i++)
            {
                VariousData<int> id = WebDBManager.GetIntField(arrResult[i].ToString());

                if (id == null)
                    continue;

                typeList.Add(id.Data);
            }

            return typeList;
        }

        //private void button_run_Click(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        s1DbMgr = new DBUtility.WebDBManager(100);
        //        s1DbMgr.DatabaseName = textBox_s1Name.Text;
        //        s1DbMgr.WebServerURL = "http://" + textBox_s1Ip.Text + ":8080/SOP";
        //        s1DbMgr.DatabaseType = DBUtility.WebDBManager.DBType.sqlserver;
        //        //TODO: 연결 성공여부 체크

        //        uneDbMgr = new DBUtility.WebDBManager(100);
        //        uneDbMgr.DatabaseName = textBox_uneName.Text;
        //        uneDbMgr.WebServerURL = "http://" + textBox_uneIp.Text + ":8080/SOP";
        //        uneDbMgr.DatabaseType = DBUtility.WebDBManager.DBType.mysql;
        //        //TODO: 연결 성공여부 체크

        //        System.Collections.ArrayList externalDeviceArr =
        //            s1DbMgr.GetResultData("select DeviceID, DeviceName, LocationID, LocationName, EqTypeID, EqTypeName from View_External_Device", 0);
        //        if (externalDeviceArr == null || externalDeviceArr.Count == 0) throw new ApplicationException("S1 View_External_Device 정보가 없습니다.");
                
        //        string strConn = string.Format("Server={0};Database={1};Uid=root;Pwd=9966;", textBox_uneIp.Text, textBox_uneName.Text);
        //        //MySql.Data.MySqlClient.MySqlConnection conn = new MySql.Data.MySqlClient.MySqlConnection(strConn);
        //        using (MySql.Data.MySqlClient.MySqlConnection conn = new MySql.Data.MySqlClient.MySqlConnection(strConn))
        //        {
        //            conn.Open();
        //            if (conn.State != ConnectionState.Open) throw new ApplicationException("MySql 연결 실패");

        //            MySql.Data.MySqlClient.MySqlCommand cmd = new MySql.Data.MySqlClient.MySqlCommand("SELECT ID, Name FROM s1access", conn); 
        //            MySql.Data.MySqlClient.MySqlDataReader rdr = cmd.ExecuteReader();
        //            DataTable table = new DataTable();
        //            table.Load(rdr);
                    
        //            conn.Close();
        //        }  
        //        //EquipZoneId 구하기
        //        StringBuilder sb = new StringBuilder();
        //        sb.Append("SELECT LocationID, EquipZoneID, LinkedZoneIdList ");
        //        sb.Append("  FROM AccessLink_View_External_Location as loc ");
        //        sb.Append(" INNER JOIN EquipmentZone as ez ON loc.EquipZoneID = ez.ID ");
        //        sb.Append(" WHERE loc.siteID=100 ");
        //        System.Collections.ArrayList locationLinkArr = uneDbMgr.GetResultData(sb.ToString(), 0);
        //        if (locationLinkArr == null || locationLinkArr.Count == 0) throw new ApplicationException("UNE AccessLink_View_External_Location 정보가 없습니다.");

        //        Dictionary<int, AccessLinkLocation> dicLocationLink = new Dictionary<int, AccessLinkLocation>();
        //        for (int i = 0; i < locationLinkArr.Count; i += 3)
        //        {
        //            if (!dicLocationLink.ContainsKey(Convert.ToInt32(locationLinkArr[i])))
        //                dicLocationLink.Add(Convert.ToInt32(locationLinkArr[i]), new AccessLinkLocation(Convert.ToInt32(locationLinkArr[i + 1]), Convert.ToInt32(locationLinkArr[i + 2])));
        //        }

        //        //FacilityType 갯수 체크
        //        List<int> facilityTypeList = new List<int>();
        //        System.Collections.ArrayList facilityTypeArr = new System.Collections.ArrayList();
        //        facilityTypeArr = uneDbMgr.GetResultData("SELECT ID FROM FacilityType WHERE LinkedTableName = 'S1Access'", 0);
        //        if (facilityTypeArr != null || facilityTypeArr.Count > 0)
        //        {
        //            for (int i = 0; i < facilityTypeArr.Count; i++)
        //            {
        //                facilityTypeList.Add(Convert.ToInt32(facilityTypeArr[i]));
        //            }
        //        }

        //        //Device와 S1Access Link
        //        Dictionary<int, int> dicAccessLinkDevice = new Dictionary<int, int>();
        //        System.Collections.ArrayList AccessLinkDeviceArr = new System.Collections.ArrayList();
        //        AccessLinkDeviceArr = uneDbMgr.GetResultData("SELECT DeviceID, S1AccessID FROM AccessLink_View_External_Device;", 0);
        //        if (AccessLinkDeviceArr != null || AccessLinkDeviceArr.Count > 0)
        //        {
        //            for (int i = 0; i < AccessLinkDeviceArr.Count; i += 2)
        //            {
        //                if (!dicAccessLinkDevice.ContainsKey(Convert.ToInt32(AccessLinkDeviceArr[i])))
        //                    dicAccessLinkDevice.Add(Convert.ToInt32(AccessLinkDeviceArr[i]), Convert.ToInt32(AccessLinkDeviceArr[i + 1]));
        //            }
        //        }

        //        int s1AccessId = Convert.ToInt32(uneDbMgr.GetResultData("SELECT ISNULL((SELECT MAX(ISNULL(ID, 0))+1 FROM S1Access),1);", 0)[0]);
        //        int sensorServerId = Convert.ToInt32(uneDbMgr.GetResultData("SELECT ISNULL((SELECT MAX(ISNULL(ID, 0))+1 FROM SensorServerInfo),1);", 0)[0]);
        //        int sensorZoneId = Convert.ToInt32(uneDbMgr.GetResultData("SELECT ISNULL((SELECT MAX(ISNULL(ID, 0))+1 FROM SensorZone),1);", 0)[0]);
        //        int sensorTagInfoId = Convert.ToInt32(uneDbMgr.GetResultData("SELECT ISNULL((SELECT MAX(ISNULL(ID, 0))+1 FROM SensorTagInfo),1);", 0)[0]);
        //        int tagNo = Convert.ToInt32(uneDbMgr.GetResultData("SELECT ISNULL((SELECT MAX(ISNULL(TagNo, 0))+1 FROM SensorTagInfo),1);", 0)[0]);
        //        sb = new StringBuilder();
        //        for (int i = 0; i < externalDeviceArr.Count; i += 6)
        //        {
        //            int deviceId = Convert.ToInt32(externalDeviceArr[i]);
        //            string name = (externalDeviceArr[i + 1].ToString() == "null") ? string.Empty : externalDeviceArr[i + 1].ToString();
        //            int locationId = Convert.ToInt32(externalDeviceArr[i + 2]);
        //            string positionName = (externalDeviceArr[i + 3].ToString() == "null") ? string.Empty : externalDeviceArr[i + 3].ToString();
        //            int zoneId = 0;
        //            int equipZoneId = 0;
        //            if (dicLocationLink.ContainsKey(locationId)) zoneId = dicLocationLink[locationId].ZoneId;
        //            if (dicLocationLink.ContainsKey(locationId)) equipZoneId = dicLocationLink[locationId].EquipZoneId;
        //            int isIndoor = 1;
        //            string description = (externalDeviceArr[i + 5].ToString() == "null") ? string.Empty : externalDeviceArr[i + 5].ToString();

        //            bool isChg = false; //변경 여부
        //            //UNE 데이터에 있을때 = 변경사항 CHECK 하고 UPDATE
        //            if (dicAccessLinkDevice.ContainsKey(deviceId))
        //            {
        //                isChg = false;

        //                StringBuilder sb2 = new StringBuilder();
        //                sb2.Append("SELECT ID, Name, PositionName, ZoneID, Description ");
        //                sb2.Append("  FROM S1Access ac ");
        //                sb2.Append(" INNER JOIN AccessLink_View_External_Device dev ");
        //                sb2.Append("    ON ac.ID=dev.s1accessid ");
        //                sb2.Append(" WHERE dev.DeviceID=" + deviceId);
        //                System.Collections.ArrayList arr = uneDbMgr.GetResultData(sb2.ToString(), 0);
        //                if (arr == null || arr.Count == 0) continue;

        //                int oS1AccessId = Convert.ToInt32(arr[0]);

        //                for (int j = 0; j < arr.Count; j += 5)
        //                {
        //                    //데이터가 변경되었다면 관련 데이터 삭제하고 다시 Insert
        //                    if (arr[j + 1].ToString() != name || arr[j + 2].ToString() != positionName || Convert.ToInt32(arr[j + 3]) != zoneId || arr[j + 4].ToString() != description)
        //                    {
        //                        isChg = true;
        //                        sb.Append("DELETE FROM SensorTagInfo WHERE SensorServerID=" + dicAccessLinkDevice[deviceId]);
        //                        sb.Append("DELETE FROM SensorZone WHERE OrgSensorID=" + dicAccessLinkDevice[deviceId]);
        //                        sb.Append("DELETE FROM SensorServerInfo WHERE ID=" + dicAccessLinkDevice[deviceId]);
        //                        sb.Append("DELETE FROM S1Access WHERE ID=" + dicAccessLinkDevice[deviceId]);
        //                        sb.Append("DELETE FROM AccessLink_View_External_Device WHERE DeviceID=" + deviceId);
        //                    }
        //                }
        //            }

        //            if (!dicAccessLinkDevice.ContainsKey(deviceId) || isChg) //UNE 데이터에 없을때 = INSERT, UNE에 데이터가 있지만 변경되었을때
        //            {
        //                //sb = new StringBuilder();
        //                sb.Append("INSERT INTO S1Access (ID, Name, PositionName, X, Y, Z, ZoneID, IsIndoor, Description) ");
        //                sb.AppendFormat("VALUES ({0}, '{1}', '{2}', 0, 0, 0, {3}, {4}, '{5}'); ", s1AccessId, name, positionName, zoneId, isIndoor, description);
        //                //uneDbMgr.GetResultData(sb.ToString(), 0);

        //                //sb = new StringBuilder();
        //                sb.Append("INSERT INTO AccessLink_View_External_Device (DeviceID, S1AccessID, SiteID) ");
        //                sb.AppendFormat("VALUES ({0}, {1}, 100); ", deviceId, s1AccessId);
        //                //uneDbMgr.GetResultData(sb.ToString(), 0);

        //                //sb = new StringBuilder();
        //                sb.Append("INSERT INTO SensorServerInfo (ID, Place, IP, MacAddr, Baudrate, Mode, FlowCtrl, Multiport, Timeout, ReciverID, SiteID) ");
        //                sb.AppendFormat("VALUES ({0}, '{1}', '192.168.0.210', '00:90:E8:3B:21:C2', '9600',3, 0, 2, 3000, 1, 100); ", sensorServerId, positionName);
        //                //uneDbMgr.GetResultData(sb.ToString(), 0);

        //                for (int f = 0; f < facilityTypeArr.Count; f++)
        //                {
        //                    int type = Convert.ToInt32(facilityTypeArr[f]);

        //                    //sb = new StringBuilder();
        //                    sb.Append("INSERT INTO SensorZone (ID, Type, Connected, EquipZoneId, Data, Description, OrgSensorId, Zone) ");
        //                    sb.AppendFormat("VALUES ({0}, {1}, {2}, {3}, {4}, '{5}', {6}, {7}); ", sensorZoneId, type, 0, equipZoneId, 0, name, s1AccessId, zoneId);
        //                    //uneDbMgr.GetResultData(sb.ToString(), 0);

        //                    //sb = new StringBuilder();
        //                    sb.Append("INSERT INTO SensorTagInfo (ID, SensorServerID, TagNo, SensorName, SensorType, EquipZoneID, SensorZoneID, Description) ");
        //                    sb.AppendFormat("VALUES ({0}, {1}, {2}, '{3}', {4}, {5}, {6}, '{7}'); ", sensorTagInfoId, sensorServerId, tagNo, name, type, equipZoneId, sensorZoneId, name);
        //                    //uneDbMgr.GetResultData(sb.ToString(), 0);
        //                    sensorZoneId++;
        //                    sensorTagInfoId++;
        //                    tagNo++;
        //                }

        //                s1AccessId++;
        //                sensorServerId++;
        //            }
        //        }
        //        if (uneDbMgr.GetResultData(sb.ToString(), 0) != null)
        //            MessageBox.Show("성공");
        //        else
        //            MessageBox.Show("실패");
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex.Message);
        //    }
        //} 
    }
    public class S1AccessLinkDevice
    {
        public int DeviceId { get; set; } 
        public int S1AccessId { get; set; } 

        public S1AccessLinkDevice(int deviceId, int s1AccessId)
        {
            this.DeviceId = deviceId;
            this.S1AccessId = s1AccessId;
        }
    }

    public class AccessLinkLocation
    {
        public int EquipZoneId { get; set; }
        public int ZoneId { get; set; }

        public AccessLinkLocation(int equipZoneId, int zoneId)
        {
            this.EquipZoneId = equipZoneId;
            this.ZoneId = zoneId;
        }
    }

    public class ViewExternalDevice
    {
        public int DeviceId { get; set; }
        public string DeviceName { get; set; }
        public int LocationId { get; set; }
        public string LocationName { get; set; }
        public int EqTypeId { get; set; }
        public string EqTypeName { get; set; }

        public ViewExternalDevice(int deviceId, string deviceName, int locationId, string locationName, int eqTypeId, string eqTypeName)
        {
            this.DeviceId = deviceId;
            this.DeviceName = deviceName;
            this.LocationId = locationId;
            this.LocationName = locationName;
            this.EqTypeId = eqTypeId;
            this.EqTypeName = eqTypeName;
        }
    }

    public class S1Access
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string PositionName { get; set; }
        public int ZoneId { get; set; }
        public int IsIndoor { get; set; }
        public string Description { get; set; }

        public S1Access(int id, string name, string positionName, int zoneId, int isIndoor, string description)
        {
            this.Id = id;
            this.Name = name;
            this.PositionName = positionName;
            this.ZoneId = zoneId;
            this.IsIndoor = isIndoor;
            this.Description = description;
        }
    }
}
