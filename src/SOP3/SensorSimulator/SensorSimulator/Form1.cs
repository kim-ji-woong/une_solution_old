using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Collections;
using DBUtility;

namespace SensorSimulator
{
    public partial class Form1 : Form
    {
        static public Form1 Instance;
        private WebDBManager m_dbMgr = null;

        IOManager m_ioMgr = null;
        NetworkManager m_netMgr = null;

        public Form1()
        {
            InitializeComponent();

            Instance = this;
            m_dbMgr = new WebDBManager();
            m_ioMgr = new IOManager();
            m_netMgr = new NetworkManager(m_dbMgr);

            Init();
        }

        public WebDBManager DbMgr
        {
            get { return m_dbMgr; }
            set { m_dbMgr = value; }
        }

        private void Init()
        {
            btnSubmit.Text = "센서 데이터 전송";

            //BuildingGroup에서 key값을 찾아옴
            foreach (KeyValuePair<BuildingGroup, ArrayList> pair in m_ioMgr.D_BuildingGroups)
            {
                cboBuildingGroup.Items.Add(pair.Key);
            }
            if (cboBuildingGroup.Items.Count > 0)
                cboBuildingGroup.SelectedIndex = 0;
        }

        private void cboBuildingGroup_SelectedIndexChanged(object sender, EventArgs e)
        {
            int nSelectedIndex = cboBuildingGroup.SelectedIndex;
            if (nSelectedIndex < 0)
                return;

            cboBuilding.Items.Clear();

            BuildingGroup buildingGroup = (BuildingGroup)cboBuildingGroup.Items[nSelectedIndex];

            if (buildingGroup.ID > 0)
            {
                //if (d_BuildingGroups.ContainsKey(buildingGroup))
                //    return;
                ArrayList arrBuildings = m_ioMgr.D_BuildingGroups[buildingGroup];

                foreach (Building building in arrBuildings)
                {
                    ArrayList arrZones = GetBuildingZones(building.ID);

                    if (arrZones != null)
                    {
                        //Zone이 하나도 없는 빌딩은 콤보박스에 보여주지 않는다.
                        cboBuilding.Items.Add(building);
                    }
                }
            }
            if (cboBuilding.Items.Count > 0)
                cboBuilding.SelectedIndex = 0;
        }

        private void cboBuilding_SelectedIndexChanged(object sender, EventArgs e)
        {
            int nSelectedIndex = cboBuilding.SelectedIndex;
            if (nSelectedIndex < 0)
                return;

            cboFloor.Items.Clear();

            Object obj = cboBuilding.Items[nSelectedIndex];
            Type type = obj.GetType();

            if (type == typeof(Building))
            {
                Building building = (Building)obj;

                AddBuildingZone(m_ioMgr.D_Zones, building);
            }
            else
            {
                cboFloor.Items.Add("-");
            }

            if (cboFloor.Items.Count > 0)
                cboFloor.SelectedIndex = 0;
        }

        private void cboFloor_SelectedIndexChanged(object sender, EventArgs e)
        {
            int nSelectedIndex = cboFloor.SelectedIndex;
            if (nSelectedIndex < 0)
                return;

            cboEquipZone.Items.Clear();

            Object obj = cboFloor.Items[nSelectedIndex];
            Type type = obj.GetType();

            Zone zone = null;

            if (type == typeof(Floor))
            {
                Building building = (Building)cboBuilding.Items[cboBuilding.SelectedIndex];
                Floor floor = (Floor)obj;
                zone = FindZone(building, floor.FloorIndex);
            }

            if (zone == null || zone.ID <= 0)
                return;

            ArrayList arrEquipZones = m_ioMgr.GetEquipmentZoneList(zone);
			if (arrEquipZones == null)
				return;

            foreach (EquipmentZone equipZone in arrEquipZones)
            {
                cboEquipZone.Items.Add(equipZone);
            }

            if (cboEquipZone.Items.Count > 0)
                cboEquipZone.SelectedIndex = 0;
        }

        private bool ContainsSensorZoneType(ComboBox.ObjectCollection collection, SensorZone sensor)
        {
            foreach (SensorZone sensorZone in collection)
            {
                if (sensorZone.Type == sensor.Type)
                    return true;
            }

            return false;
        }

        private void cboEquipZone_SelectedIndexChanged(object sender, EventArgs e)
        {
            cboSensorZone.Items.Clear();

            EquipmentZone equipZone = (EquipmentZone)cboEquipZone.Items[cboEquipZone.SelectedIndex];

            if (!m_ioMgr.D_EquipZoneSensor.ContainsKey(equipZone))
                return;

            ArrayList arrSensorZones = m_ioMgr.D_EquipZoneSensor[equipZone];

            foreach (SensorZone sensor in arrSensorZones)
            {
                if (!ContainsSensorZoneType(cboSensorZone.Items, sensor))
                    cboSensorZone.Items.Add(sensor);
            }

            if (cboSensorZone.Items.Count > 0)
                cboSensorZone.SelectedIndex = 0;

            /*cboSensorZone.Items.Clear();

            if (cboSensorZone.Items.Count > 0)
                cboSensorZone.SelectedIndex = 0;

            int nSelectedIndex = cboBuilding.SelectedIndex;
            if (nSelectedIndex < 0)
                return;

            Building building = (Building)cboBuilding.Items[nSelectedIndex];

            nSelectedIndex = cboFloor.SelectedIndex;
            if (nSelectedIndex < 0)
                return;

            //builing과 층을 가지고 어떤zone인지 찾는다.
            Floor floor = (Floor)cboFloor.Items[nSelectedIndex];
            Zone zone = FindZone(building, floor.FloorIndex);

            if (zone.ID > 0)
            {
				if (!m_ioMgr.D_ZoneSensor.ContainsKey(zone))
                    return;

                //찾아온 Zone을 ZoneSensor와 비교. 맞는값을 배열에 넣음
                ArrayList arrZones = m_ioMgr.D_ZoneSensor[zone];

                foreach (SensorZone sensorZone in arrZones)
                {
                    cboSensorZone.Items.Add(sensorZone);
                }
            }

            if (cboSensorZone.Items.Count > 0)
                cboSensorZone.SelectedIndex = 0;*/
        }
            
        private int nSensor_type = 1;
        private void cboSensorZone_SelectedIndexChanged(object sender, EventArgs e)
        {
            cboisSenser.Items.Clear();

            if (cboSensorZone.SelectedItem.ToString() == "화재 탐지")
            {
                cboisSenser.Items.Add("이상 없음");
                cboisSenser.Items.Add("화재 발생");
                cboisSenser.Items.Add("통신 끊김");
				cboisSenser.Items.Add("통신 연결");
                nSensor_type = 1;
            }
            else if (cboSensorZone.SelectedItem.ToString() == "소화 센서")
            {
                cboisSenser.Items.Add("이상 없음");
                cboisSenser.Items.Add("소화 중");
                cboisSenser.Items.Add("통신 끊김");
				cboisSenser.Items.Add("통신 연결");
                nSensor_type = 2;
            }
            else if (cboSensorZone.SelectedItem.ToString() == "압력 센서")
            {
                cboisSenser.Items.Add("이상 없음");
                cboisSenser.Items.Add("압력 이상");
                cboisSenser.Items.Add("통신 끊김");
				cboisSenser.Items.Add("통신 연결");
                nSensor_type = 3;
            }

            if (cboisSenser.Items.Count > 0)
                cboisSenser.SelectedIndex = 0;
        }
        
        private void button1_Click(object sender, EventArgs e)
        {
            //int SensorID = 0;

            /*int nSelectedIndex = cboBuilding.SelectedIndex;
            if (nSelectedIndex < 0)
                return;

            Building building = (Building)cboBuilding.Items[nSelectedIndex];

            nSelectedIndex = cboFloor.SelectedIndex;
            if (nSelectedIndex < 0)
                return;

            //builing과 층을 가지고 어떤zone인지 찾는다.
            Floor floor = (Floor)cboFloor.Items[nSelectedIndex];
            Zone zone = FindZone(building, floor.FloorIndex);*/

            if (cboEquipZone.SelectedIndex < 0)
                return;

            EquipmentZone equipZone = (EquipmentZone)cboEquipZone.Items[cboEquipZone.SelectedIndex];

            if (equipZone.ID > 0)
                m_netMgr.SendSensorData(equipZone, nSensor_type, (byte)cboisSenser.SelectedIndex);
            
            /*if (zone.ID > 0)
            {                
                //sensorzone = d_SensorZone[zone.ID];
                ArrayList arrSensorZone = m_ioMgr.D_ZoneSensor[zone];

                //선택된 센서아이디를 구함
                foreach (SensorZone sensorZone in arrSensorZone)
                {
                    if(sensorZone.Type == nSensor_type)
                        SensorID = sensorZone.ID;
                }
            }

            //m_dbMgr = new WebDBManager();

            int connected = 0;
            int data = 0;

            if (cboisSenser.SelectedIndex == 0)
            {
                connected = 1;
                data = 0;
            }
            else if (cboisSenser.SelectedIndex == 1)
            {
                connected = 1;
                data = 1;
            }
            else if (cboisSenser.SelectedIndex == 2)
            {
                connected = 0;
                data = 0;
            }

            //SensorZone
            string strUpdate = "Update SensorZone Set Connected ='" + connected + "', Data = '" + data + "' Where Type ='" + nSensor_type + "' and ZoneID = '" + zone.ID + "'";

            m_dbMgr.GetResultData(strUpdate, 0);

            //최대ID값 찾기
            string sqlID = "select max(id) as id from SensorZoneHistory";

            ArrayList arrResult = m_dbMgr.GetResultData(sqlID, 0);
            int nResultCount = arrResult.Count;

            int Max_ID = 0;
            for (int i = 0; i < nResultCount; i += 1)
            {
                //Data가 아예 안들어가 있을경우 0부터 시작
                int Find_Maxid = WebDBManager.GetIntField(arrResult[i].ToString(), 0);
                Max_ID = Find_Maxid;
            }
            Max_ID++;

            DateTime dtNow = DateTime.Now;
            string strDateTimeField = string.Format("{0} {1}:{2}:{3}", dtNow.ToShortDateString(), dtNow.Hour, dtNow.Minute, dtNow.Second);

            //History
            string sqlInsert = "insert into SensorZoneHistory(ID, SensorID,Connected,Data,Time) Values('" 
                + Max_ID + "','" + SensorID + "','" + connected + "','" + data + "','" + strDateTimeField + "')";

            //가장 최근에 올라온 데이터와 같을 경우 비교
            CompareData(Max_ID, SensorID, connected, data);

            if (CompareResult == true)
                m_dbMgr.GetResultData(sqlInsert, 0);*/
        }

        private bool CompareResult = false;
        private void CompareData(int max_ID, int sensorID, int connected, int data)
        {
            Console.WriteLine("hello");

            max_ID--;

            //m_dbMgr = new WebDBManager();

            //가장 최근데이터
            string strSQL = "select * from SensorZoneHistory where id = '" + max_ID + "'";
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null)
            {
                CompareResult = true;
                return;
            }

            int nResultCount = arrResult.Count;
            
            //Data가 비어있다면
            if (nResultCount < 1)
                CompareResult = true;

            for (int i = 0; i < nResultCount - 3; i += 4)
            {
                int comp_SensorID = WebDBManager.GetIntField(arrResult[i + 1].ToString(), -1);
                int comp_Connected = WebDBManager.GetIntField(arrResult[i + 2].ToString(), -1);
                int comp_Data = WebDBManager.GetIntField(arrResult[i + 3].ToString(), -1);

                //최근값과 똑같은값이 들어올 때 false
                if (comp_SensorID == sensorID && comp_Connected == connected && comp_Data == data)
                    CompareResult = false;
                else
                    CompareResult = true;
            }
        }


        private void AddBuildingZone(Dictionary<int, Zone> dicZones, Building building)
        {
            ArrayList arrFloor = new ArrayList();

            foreach (KeyValuePair<int, Zone> pair in dicZones)
            {
                if (pair.Value.Building == building)
                    arrFloor.Add(new Floor(pair.Value.FloorIndex + pair.Value.AddFloor));
            }

            arrFloor.Sort();

            foreach (Floor floor in arrFloor)
            {
                cboFloor.Items.Add(floor);
            }
        }

        Zone FindZone(Building building, float fFloorIndex)
        {
            int nFloorIndex = fFloorIndex > 0.0f ? (int)(fFloorIndex + 0.01f) : (int)(fFloorIndex - 0.01f);
            string strAddFloor = string.Format("{0:f1}", fFloorIndex - nFloorIndex);

            foreach (KeyValuePair<int, Zone> pair in m_ioMgr.D_Zones)
            {
                Zone zone = pair.Value;

                if (zone.Building == building && zone.FloorIndex == nFloorIndex)
                {
                    if (strAddFloor == string.Format("{0:f1}", zone.AddFloor))
                        return zone;
                }
            }
            return null;
        }

        public ArrayList GetBuildingZones(int nBuildingID)
        {
            if (m_ioMgr.D_BuildingZones.ContainsKey(nBuildingID))
                return m_ioMgr.D_BuildingZones[nBuildingID];

            return null;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            m_netMgr.ReleaseThread();
        }

        //public int GetSensorZoneID(int nZoneID)
        //{
        //    if (d_SensorZone.ContainsKey(nZoneID))
        //        //return d_SensorZone[nZoneID];

        //    return -1;
        //}
    }
}
