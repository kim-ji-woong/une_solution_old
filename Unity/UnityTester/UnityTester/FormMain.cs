using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DBUtility2;
using System.Collections;
using System.IO;

namespace UnityTester
{
    public partial class FormMain : Form
    {
        private FormUnity m_frm = null;
        private Dictionary<int, List<AlarmZone>> m_dicFloorAlarmZones = new Dictionary<int, List<AlarmZone>>();
        // Key : Zone ID
        // Value : Floor Index
        private Dictionary<int, int> m_dicZoneFloorIDs = null;
        // Key : EquipmentZone
        private Dictionary<int, AlarmZone> m_dicAlarmZones = new Dictionary<int, AlarmZone>();
        // Key : EquipZone ID
        // Value : Zone ID
        private Dictionary<int, int> m_dicEquipZoneZone = new Dictionary<int, int>();
        private WebDBManager m_dbMgr = null;
        private string m_strOutdoorModelName = "";
        // Key : Zone ID
        private Dictionary<int, List<SensorTag>> m_dicZoneSensors = new Dictionary<int, List<SensorTag>>();
        private FormSensorList m_frmSensorList = null;

        public FormMain()
        {
            InitializeComponent();
            labelStatus.Text = "";

            string strSiteID = System.Configuration.ConfigurationManager.AppSettings.Get("siteid");

            int nSiteID;

            if (int.TryParse(strSiteID, out nSiteID))
                m_dbMgr = new WebDBManager(nSiteID);
            else
                m_dbMgr = new WebDBManager(200);

            m_strOutdoorModelName = System.Configuration.ConfigurationManager.AppSettings.Get("outdoor");
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            labelEquipZoneID.Visible = false;
            InitAlarmZones();
            ReadSensors();
        }

        private void btnRunUnity_Click(object sender, EventArgs e)
        {
            m_frm = new FormUnity(m_dbMgr);
            m_frm.Show();
        }

        /*private string GetSceneName(int nIndex)
        {
            switch (nIndex)
            {
                case 0:
                    return "out_door_191112_final";

                case 1:
                    return "b05f";

                case 2:
                    return "b04f";

                case 3:
                    return "b03f";

                case 4:
                    return "b02f";

                case 5:
                    return "b01f";

                case 6:
                    return "01f";

                case 7:
                    return "02f";

                case 8:
                    return "03f";

                case 9:
                    return "04f";

                case 10:
                    return "05f";

                case 11:
                    return "06f";

                case 12:
                    return "07F";
            }

            return "";
        }*/

        private void btnChangeScene_Click(object sender, EventArgs e)
        {
            if (cboScenes.SelectedIndex < 0 || m_frm == null)
                return;

            Zone zone = (Zone)cboScenes.Items[cboScenes.SelectedIndex];
            m_frm.SelectScene(zone.SceneName);

            if (cboScenes.SelectedIndex > 0)
                m_frm.SetSceneTitle(zone.ZoneName);
            else
                m_frm.SetSceneTitle("");

            if (cboScenes.SelectedIndex == 0)
            {
                btnShowSensorList.Enabled = false;

                if (m_frmSensorList != null && m_frmSensorList.IsDisposed == false)
                {
                    m_frmSensorList.Close();
                    m_frmSensorList = null;
                }
            }
            else
            {
                btnShowSensorList.Enabled = true;

                if (m_frmSensorList != null && m_frmSensorList.IsDisposed == false)
                {
                    List<SensorTag> sensors;

                    if (m_dicZoneSensors.TryGetValue(zone.ID, out sensors))
                        m_frmSensorList.SetSensors(zone, sensors);
                }
            }
        }

        // Key : ZoneID
        // Value : Scene Index
        private Dictionary<int, int> AddScenes()
        {
            string strSQL = "Select ZoneID, ZoneName, FloorIndex, AddFloor, SceneName from Zone, ZoneScene where Zone.ID = ZoneScene.ZoneID and SiteID = " + m_dbMgr.SiteID.ToString();
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL);

            if (arrResult == null)
                return null;

            int nResultCount = arrResult.Count;
            Dictionary<int, int> dicZoneIDs = new Dictionary<int, int>();
            //Dictionary<float, float> dicFloors = new Dictionary<float, float>();

            for (int i = 0; i < nResultCount - 4; i += 5)
            {
                VariousData<int> id = WebDBManager.GetIntField(arrResult[i].ToString());
                string strZoneName = WebDBManager.GetStringField(arrResult[i + 1]);
                VariousData<int> floorIndex = WebDBManager.GetIntField(arrResult[i + 2].ToString());
                VariousData<float> addFloor = WebDBManager.GetFloatField(arrResult[i + 3].ToString());
                string strSceneName = WebDBManager.GetStringField(arrResult[i + 4]);

                if (id == null || strZoneName == null || floorIndex == null || strSceneName == null)
                    continue;

                float fFloorIndex = floorIndex.Data;
                if (addFloor != null)
                    fFloorIndex += addFloor.Data;

                /*if (dicFloors.ContainsKey(fFloorIndex))
                    break;
                else
                    dicFloors[fFloorIndex] = fFloorIndex;*/

                Zone zone = new Zone();

                zone.ID = id.Data;
                zone.ZoneName = strZoneName;
                zone.FloorIndex = floorIndex.Data;
                zone.SceneName = strSceneName;

                int nIndex = cboScenes.Items.Add(zone);
                //int nIndex = cboScenes.Items.Add(strZoneName);
                dicZoneIDs[id.Data] = nIndex;
            }

            return dicZoneIDs;
        }

        // Key : ZoneID
        // Value : Scene Index
        private void AddAlarmZones(Dictionary<int, int> dicZoneIDs)
        {
            string strSQL = "Select ez.ID, ez.ZoneName, ez.LinkedZoneIDList, ezv.VolumeName from EquipmentZone as ez, EquipZoneVolume as ezv where ez.ID = ezv.EquipZoneID";
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL);

            if (arrResult == null)
                return;

            m_dicAlarmZones.Clear();
            int nResultCount = arrResult.Count;

            for (int i=0;i<nResultCount-3;i+=4)
            {
                VariousData<int> equipZoneID = WebDBManager.GetIntField(arrResult[i].ToString());
                string strEquipZoneName = WebDBManager.GetStringField(arrResult[i + 1]);
                string strZoneIDs = WebDBManager.GetStringField(arrResult[i + 2]);
                string strVolumeName = WebDBManager.GetStringField(arrResult[i + 3]);

                if (equipZoneID == null || strEquipZoneName == null || strZoneIDs == null || strVolumeName == null)
                    continue;

                string[] tokens = strZoneIDs.Split(',');

                int nZoneID;

                if (int.TryParse(tokens[0].Trim(), out nZoneID) == false)
                    continue;

                int nSceneIndex;

                if (dicZoneIDs.TryGetValue(nZoneID, out nSceneIndex) == false)
                    continue;

                AlarmZone equipZone = new AlarmZone();

                equipZone.EquipZoneID = equipZoneID.Data;
                equipZone.EquipZoneName = strEquipZoneName;
                equipZone.Volume = strVolumeName;

                List<AlarmZone> equipZones;

                if (m_dicFloorAlarmZones.TryGetValue(nSceneIndex, out equipZones) == false)
                {
                    equipZones = new List<AlarmZone>();
                    m_dicFloorAlarmZones[nSceneIndex] = equipZones;
                }

                equipZones.Add(equipZone);
                m_dicAlarmZones[equipZone.EquipZoneID] = equipZone;
                m_dicEquipZoneZone[equipZone.EquipZoneID] = nZoneID;
            }
        }

        private void AddOutdoorZone()
        {
            if (cboScenes.Items.Count == 0)
                return;

            if (cboScenes.Items[0] is string)
            {
                string strSceneName = (string)cboScenes.Items[0];

                Zone zone = new Zone();
                zone.ZoneName = strSceneName;
                zone.SceneName = m_strOutdoorModelName;
                //zone.SceneName = "out_door_191112_final";

                cboScenes.Items.RemoveAt(0);
                cboScenes.Items.Insert(0, zone);
            }
        }

        private void InitAlarmZones()
        {
            m_dicZoneFloorIDs = null;
            Dictionary<int, int> dicZoneIDs = AddScenes();

            if (dicZoneIDs == null)
                return;

            AddOutdoorZone();
            AddAlarmZones(dicZoneIDs);
            m_dicZoneFloorIDs = dicZoneIDs;
        }

        private void cboScenes_SelectedIndexChanged(object sender, EventArgs e)
        {
            cboAlarmZones.Items.Clear();

            if (cboScenes.SelectedIndex < 0)
                return;

            List<AlarmZone> alarmZones;

            if (m_dicFloorAlarmZones.TryGetValue(cboScenes.SelectedIndex, out alarmZones))
            {
                foreach (AlarmZone zone in alarmZones)
                {
                    cboAlarmZones.Items.Add(zone);
                }

                if (cboAlarmZones.Items.Count > 0)
                    cboAlarmZones.SelectedIndex = 0;
            }
        }

        private void btnShowAlarmZone_Click(object sender, EventArgs e)
        {
            if (cboAlarmZones.SelectedIndex < 0 || m_frm == null)
                return;

            AlarmZone alarmZone = (AlarmZone)cboAlarmZones.Items[cboAlarmZones.SelectedIndex];

            if (alarmZone == null)
                return;

            m_frm.ShowAlarmZone(alarmZone.Volume);
        }

        public void ShowAlarmZone(string strVolumeName)
        {
            m_frm.ShowAlarmZone(strVolumeName);
        }

        private void btnCaptureAlarmZone_Click(object sender, EventArgs e)
        {
            labelStatus.Text = "";
            labelStatus.Visible = true;

            string strFolder = textBoxImagePath.Text.Trim();

            if (strFolder.Length == 0)
            {
                textBoxImagePath.Focus();
                MessageBox.Show("이미지 저장 폴더를 입력하세요.");
                return;
            }

            if (Directory.Exists(strFolder) == false)
            {
                textBoxImagePath.Focus();
                MessageBox.Show("이미지 저장 폴더가 유효하지 않은 경로입니다.");
                return;
            }

            string strSQL = "Select ID, LinkedZoneIDList from EquipmentZone where SiteID = " + m_dbMgr.SiteID.ToString();
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL);

            if (arrResult == null)
                return;

            int nZoneID, nPrevZoneID = -1, nFloorIndex, nPrevAlarmZoneIndex = -1;
            AlarmZone zone;
            int nResultCount = arrResult.Count;

            for (int i=0;i<nResultCount-1;i+=2)
            {
                VariousData<int> id = WebDBManager.GetIntField(arrResult[i].ToString());
                string strZoneIDList = WebDBManager.GetStringField(arrResult[i + 1]);

                if (id == null || strZoneIDList == null || strZoneIDList.Length == 0)
                    continue;

                string[] tokens = strZoneIDList.Split(',');

                if (int.TryParse(tokens[0].Trim(), out nZoneID) == false)
                    continue;

                if (m_dicAlarmZones.TryGetValue(id.Data, out zone) == false)
                    continue;

                if (m_dicZoneFloorIDs.TryGetValue(nZoneID, out nFloorIndex) == false)
                    continue;

                if (cboScenes.SelectedIndex != nFloorIndex)
                {
                    cboScenes.SelectedIndex = nFloorIndex;
                    btnChangeScene_Click(null, null);
                }

                int nIndex = GetAlarmZoneIndex(zone.EquipZoneID);
                //int nIndex = GetAlarmZoneIndex(zone.EquipZoneName);
                int nCurrentAlarmZoneIndex = cboScenes.SelectedIndex * 1000 + nIndex;

                if (nIndex >= 0)
                {
                    if (nCurrentAlarmZoneIndex != nPrevAlarmZoneIndex)
                    {
                        cboAlarmZones.SelectedIndex = nIndex;
                        btnShowAlarmZone_Click(null, null);
                    }
                }

                System.Threading.Thread.Sleep(3000);

                string strImagePath = strFolder + "\\" + id.Data.ToString() + ".png";
                m_frm.SaveImage(strImagePath);
                
                nPrevAlarmZoneIndex = nCurrentAlarmZoneIndex;
                labelStatus.Text = string.Format("{0} / {1}", i / 2 + 1, nResultCount / 2);
                labelStatus.Refresh();
            }
        }

        private int GetAlarmZoneIndex(int nEquipZoneID)
        {
            for (int i = 0; i < cboAlarmZones.Items.Count; i++)
            {
                AlarmZone zone = (AlarmZone)cboAlarmZones.Items[i];

                if (zone.EquipZoneID == nEquipZoneID)
                    return i;
            }

            return -1;
        }
        /*private int GetAlarmZoneIndex(string strZoneName)
        {
            for (int i = 0; i< cboAlarmZones.Items.Count;i++)
            {
                AlarmZone zone = (AlarmZone)cboAlarmZones.Items[i];

                if (zone.EquipZoneName == strZoneName)
                    return i;
            }

            return -1;
        }*/

        private void checkBoxEditMode_CheckedChanged(object sender, EventArgs e)
        {
            m_frm.SetEditMode(checkBoxEditMode.Checked);
        }

        private void ReadSensors()
        {
            string strSQL = "select ID, TagID, SensorName, EquipZoneID from SensorTagInfo where TagID is not NULL and SensorZoneID > 0 and SensorType = " + ((int)UnE.Sensor.IFacility.FacilityType.FIRE_SENSOR).ToString() + " order by TagID";
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL);

            if (arrResult == null)
                return;

            AlarmZone alarmZone;
            List<SensorTag> sensors;
            int nZoneID;

            int nResultCount = arrResult.Count;

            for (int i=0;i<nResultCount-3;i+=4)
            {
                VariousData<int> id = WebDBManager.GetIntField(arrResult[i].ToString());
                VariousData<int> tagID = WebDBManager.GetIntField(arrResult[i + 1].ToString());
                string strSensorName = WebDBManager.GetStringField(arrResult[i + 2]);
                VariousData<int> equipZoneID = WebDBManager.GetIntField(arrResult[i + 3].ToString());

                if (id == null || tagID == null || strSensorName == null || equipZoneID == null)
                    continue;

                if (m_dicAlarmZones.TryGetValue(equipZoneID.Data, out alarmZone) == false)
                    continue;

                if (m_dicEquipZoneZone.TryGetValue(equipZoneID.Data, out nZoneID) == false)
                    continue;

                if (m_dicZoneSensors.TryGetValue(nZoneID, out sensors) == false)
                {
                    sensors = new List<SensorTag>();
                    m_dicZoneSensors[nZoneID] = sensors;
                }

                SensorTag sensor = new SensorTag();

                sensor.ID = id.Data;
                sensor.TagID = tagID.Data;
                sensor.SensorName = strSensorName;
                sensor.EquipZoneID = equipZoneID.Data;
                sensor.AlarmZone = alarmZone;

                sensors.Add(sensor);
            }
        }

        private void btnShowSensorList_Click(object sender, EventArgs e)
        {
            if (m_frmSensorList == null || m_frmSensorList.IsDisposed)
            {
                m_frmSensorList = new FormSensorList(this, m_dbMgr);

                Zone zone = (Zone)cboScenes.Items[cboScenes.SelectedIndex];

                List<SensorTag> sensors;

                if (m_dicZoneSensors.TryGetValue(zone.ID, out sensors))
                    m_frmSensorList.SetSensors(zone, sensors);
            }

            btnShowSensorList.Enabled = false;
            m_frmSensorList.Show();
        }

        public void OnCloseSensorList()
        {
            if (cboScenes.SelectedIndex > 0)
                btnShowSensorList.Enabled = true;

            m_frmSensorList = null;
        }

        private void cboAlarmZones_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboAlarmZones.SelectedIndex < 0)
            {
                labelEquipZoneID.Visible = false;
            }
            else
            {
                AlarmZone zone = (AlarmZone)cboAlarmZones.Items[cboAlarmZones.SelectedIndex];
                labelEquipZoneID.Text = zone.EquipZoneID.ToString();
                labelEquipZoneID.Visible = true;
            }
        }
    }

    public class AlarmZone
    {
        private int m_nEqupZoneID = -1;
        private string m_strName = "";
        private string m_strVolumeName = "";
        
        public int EquipZoneID
        {
            get { return m_nEqupZoneID; }
            set { m_nEqupZoneID = value; }
        }

        public string EquipZoneName
        {
            get { return m_strName; }
            set { m_strName = value; }
        }

        public string Volume
        {
            get { return m_strVolumeName; }
            set { m_strVolumeName = value; }
        }

        public override string ToString()
        {
            return m_strName;
        }
    }

    public class Zone
    {
        private int m_nID = -1;
        private float m_fFloorIndex = -1;
        private string m_strZoneName = "";
        private string m_strSceneName = "";

        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }

        public float FloorIndex
        {
            get { return m_fFloorIndex; }
            set { m_fFloorIndex = value; }
        }

        public string ZoneName
        {
            get { return m_strZoneName; }
            set { m_strZoneName = value; }
        }

        public string SceneName
        {
            get { return m_strSceneName; }
            set { m_strSceneName = value; }
        }

        public override string ToString()
        {
            return m_strZoneName;
        }
    }

    public class SensorTag
    {
        private int m_nID = -1;
        private int m_nTagID = -1;
        private string m_strSensorName = "";
        private int m_nEquipZoneID = -1;
        private int m_nTabHighIndex = -1;
        private int m_nTabLowIndex = -1;
        private int m_nRelayFirstIndex = -1;
        private int m_nRelaySecondIndex = -1;
        private AlarmZone m_alarmZone = null;
        private string m_strVolumeName = "";

        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }

        public int TagID
        {
            get { return m_nTagID; }
            set { SetTagID(value); }
        }

        public string SensorName
        {
            get { return m_strSensorName; }
            set { m_strSensorName = value; }
        }

        public int EquipZoneID
        {
            get { return m_nEquipZoneID; }
            set { m_nEquipZoneID = value; }
        }

        public int TabHighIndex
        {
            get { return m_nTabHighIndex; }
        }

        public int TabLowIndex
        {
            get { return m_nTabLowIndex; }
        }

        public int RelayFirstIndex
        {
            get { return m_nRelayFirstIndex; }
        }

        public int RelaySecondIndex
        {
            get { return m_nRelaySecondIndex; }
        }

        public AlarmZone AlarmZone
        {
            get { return m_alarmZone; }
            set
            {
                m_alarmZone = value;

                if (m_alarmZone != null)
                    SetVolume(m_alarmZone.Volume);
                else
                    SetVolume("");
            }
        }

        public string VolumeName
        {
            get { return m_strVolumeName; }
            set
            {
                m_alarmZone.Volume = value;
                SetVolume(value);
            }
        }

        private void SetVolume(string strVolumeName)
        {
            string[] tokens = strVolumeName.Split('\t');

            m_strVolumeName = "";

            foreach (string strVolume in tokens)
            {
                if (m_strVolumeName.Length == 0)
                    m_strVolumeName = strVolume.Trim();
                else
                    m_strVolumeName += ", " + strVolume.Trim();
            }
        }

        private void SetTagID(int nTagID)
        {
            m_nTagID = nTagID;

            m_nTabHighIndex = m_nTagID / 10000000;
            m_nTabLowIndex = (m_nTagID % 10000000) / 100000;
            m_nRelayFirstIndex = (m_nTagID % 100000) / 100;
            m_nRelaySecondIndex = m_nTagID % 100;
        }
    }
}
