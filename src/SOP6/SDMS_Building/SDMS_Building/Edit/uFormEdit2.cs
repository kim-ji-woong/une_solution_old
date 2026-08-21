using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SDMS_Building.Content;
using UnE.View.Content;
using UnE.Util.Unity;
using UnE.Sensor;
using SDMS_Building.PopupDialog.Controls;
using DBUtility2;
using System.Collections;
using UnE.Spatial;
using System.IO;
using UnE.GUI;

namespace SDMS_Building.Edit
{
    public enum EditType { CCTV, Wall }
    public partial class uFormEdit2 : UserControl
    {
        private IFormContent m_frmContent = null;
        private List<IFacility.FacilityType> m_ownEnableList = new List<IFacility.FacilityType>();
        private List<IFacility.FacilityType> m_otherEnableList = null;
        private uSensorInfo m_sensorInfo = null;
        private Dictionary<IFacility.FacilityType, int> m_dicPOIID = new Dictionary<IFacility.FacilityType, int>();

        // 편집모드에서 추가하였으나 아직 DB에 저장하지 않은 POI들
        private Dictionary<IFacility.FacilityType, List<POI>> m_dicAddedPOIs = new Dictionary<IFacility.FacilityType, List<POI>>();
        // 편집모드에서 삭제하였으나 아직 DB에 반영하지 않은 POI들
        private Dictionary<IFacility.FacilityType, List<POI>> m_dicDeletedPOIs = new Dictionary<IFacility.FacilityType, List<POI>>();
        // 편집모드에서 수정되었으나 아직 DB에 반영하지 않은 POI들
        private Dictionary<IFacility.FacilityType, List<POI>> m_dicChangedPOIs = new Dictionary<IFacility.FacilityType, List<POI>>();

        private Panel4Unity m_panel = null;
        private object m_selectedSensor = null;
        private bool m_systemInput = false;

        private EditType m_curEditType = EditType.CCTV;

        public new bool Visible
        {
            get { return base.Visible; }
            set
            {
                base.Visible = value;

                if (value)
                    AddContentForm();
                else
                    RemoveContentForm();
            }
        }

        public uFormEdit2(uSensorInfo sensorInfo)
        {
            InitializeComponent();
            m_sensorInfo = sensorInfo;
            m_ownEnableList.Add(IFacility.FacilityType.CCTV);

            pnEditCCTV.Parent = this;
            pnEditWall.Parent = this;
            pnEditCCTV.Location = pnEditWall.Location = new Point(256, 0);
        }

        private void AddContentForm()
        {
            ReadSensorIDs();

            if (m_otherEnableList == null)
                m_otherEnableList = FormMain.Instance.GetEnableIconList();

            FormMain.Instance.SetEnableIconList(m_ownEnableList);

            Form frm = FormMain.Instance.ContentManager.RemoveContentForm();
            m_frmContent = (IFormContent)frm;
            Panel4Unity panel = (Panel4Unity)m_frmContent.OutdoorView;
            m_panel = panel;

            panel.EditMode = true;
            panel.ShowLayer(SDMS.ID.ID_LAYER_CCTV, true);
            panel.ShowLayer(SDMS.ID.ID_LAYER_FIREEXT, false);

            SelectEditMode(m_panel);

            frm.Parent = panelBody;
            frm.Dock = DockStyle.Fill;
            panelBody.Controls.Add(frm);

            frm.Show();
        }

        private void SelectEditMode(Panel4Unity panel)
        {
            string strParameter = "";
            Panel4Unity.EditModeType type = Panel4Unity.EditModeType.None;

            if (btnAddCctv.IsChecked)
            {
                type = Panel4Unity.EditModeType.AddIcon;

                int nID = 0;
                string strPOIType = GetCurrentIconType(out nID);
                strParameter = strPOIType + "_" + nID.ToString();
            }
            else if (btnMoveCctv.IsChecked)
                type = Panel4Unity.EditModeType.MoveIcon;
            else if (btnDeleteCctv.IsChecked)
                type = Panel4Unity.EditModeType.DeleteIcon;
            else
                type = Panel4Unity.EditModeType.PickIcon;

            panel.SelectEditMode(type, strParameter);
        }

        private void RemoveContentForm()
        {
            if (m_frmContent == null)
                return;

            if (m_otherEnableList != null)
                FormMain.Instance.SetEnableIconList(m_otherEnableList);

            panelBody.Controls.Remove((Form)m_frmContent);
            ((Panel4Unity)m_frmContent.OutdoorView).EditMode = false;
            FormMain.Instance.ContentManager.AddContentForm();
        }

        private string GetCurrentIconType(out int nID)
        {
            nID = 1;

            if (m_sensorInfo == null)
                return "";

            IFacility.FacilityType type = m_sensorInfo.SelectedType;

            if (m_dicPOIID.TryGetValue(type, out nID) == false)
            {
                nID = 1;
                m_dicPOIID[type] = nID;
            }

            if (type == IFacility.FacilityType.CCTV)
                return Data.CommonString.POI_CCTV;
            else if (type == IFacility.FacilityType.FIRE_SENSOR)
                return Data.CommonString.POI_Fire;
            else if (type == IFacility.FacilityType.PSM_SENSOR)
                return Data.CommonString.POI_Gas;
            else if (type == IFacility.FacilityType.DOOR)
                return Data.CommonString.POI_Door;
            else if (type == IFacility.FacilityType.FIREWALL)
                return Data.CommonString.POI_FireWall;

            return "";
        }

        private void ReadSensorIDs()
        {
            WebDBManager dbMgr = FormMain.Instance.DBManager;

            int nID = GetNextID(dbMgr, Data.CommonString.POI_CCTV_Table);

            if (nID < 0)
                return;

            m_dicPOIID[IFacility.FacilityType.CCTV] = nID;

            nID = GetNextID(dbMgr, Data.CommonString.POI_Fire_Table);

            if (nID < 0)
                return;

            m_dicPOIID[IFacility.FacilityType.FIRE_SENSOR] = nID;

            nID = GetNextID(dbMgr, Data.CommonString.POI_Gas_Table);

            if (nID < 0)
                return;

            m_dicPOIID[IFacility.FacilityType.PSM_SENSOR] = nID;

            nID = GetNextID(dbMgr, Data.CommonString.POI_Door_Table);

            if (nID < 0)
                return;

            m_dicPOIID[IFacility.FacilityType.DOOR] = nID;

            nID = GetNextID(dbMgr, Data.CommonString.POI_FireWall_Table);

            if (nID < 0)
                return;

            m_dicPOIID[IFacility.FacilityType.FIREWALL] = nID;
        }

        private int GetNextID(WebDBManager dbMgr, string strTableName)
        {
            string strSQL = "Select max(ID) from " + strTableName;
            ArrayList arrResult = dbMgr.GetResultData(strSQL);

            if (arrResult == null)
                return -1;

            if (arrResult.Count == 0)
                return 1;

            VariousData<int> id = WebDBManager.GetIntField(arrResult[0].ToString());

            if (id == null)
                return -1;

            return id.Data + 1;
        }

        private POI FindAddedPOI(POI poi)
        {
            if (poi.Facility == null)
                return null;

            List<POI> pois;

            if (m_dicAddedPOIs.TryGetValue(poi.Facility.Type, out pois))
            {
                foreach (POI _poi in pois)
                {
                    if (_poi.ID == poi.ID)
                        return _poi;
                }
            }

            return null;
        }

        public List<SensorInfo> GetAddedPOIs(IFacility.FacilityType type)
        {
            List<POI> pois;

            if (m_dicAddedPOIs.TryGetValue(type, out pois))
            {
                return PoisToSensors(pois);
            }

            return null;
        }

        public List<SensorInfo> GetDeletedPOIs(IFacility.FacilityType type)
        {
            List<POI> pois;

            if (m_dicDeletedPOIs.TryGetValue(type, out pois))
            {
                return PoisToSensors(pois);
            }

            return null;
        }

        private List<SensorInfo> PoisToSensors(List<POI> pois)
        {
            List<SensorInfo> sensors = new List<SensorInfo>();

            foreach (POI poi in pois)
            {
                SensorInfo sensor = PoiToSensor(poi);
                sensors.Add(sensor);
            }

            return sensors;
        }

        private SensorInfo PoiToSensor(POI poi)
        {
            SensorInfo sensor = new SensorInfo();
            sensor.DeActivate = false;
            sensor.ISConnected = false;
            sensor.SensorName = GetPOIName(poi);
            sensor.SensorID = poi.ID;
            sensor.Sensor = poi.Facility;
            return sensor;
        }

        private string GetPOIName(POI poi)
        {
            if (poi.Facility == null)
                return "";

            if (poi.Facility.Type == IFacility.FacilityType.CCTV)
            {
                CCTV cctv = (CCTV)poi.Facility;
                return cctv.AccessKey;
            }
            else if (poi.Facility.Type == IFacility.FacilityType.FIRE_SENSOR)
            {
                FireSensor sensor = (FireSensor)poi.Facility;
                return sensor.SensorName;
            }

            return "";
        }

        public SensorInfo OnAddPOI(POI poi)
        {
            if (poi.Facility == null)
                return null;

            poi.Facility.ID = poi.ID;
            m_dicPOIID[poi.Facility.Type] = poi.ID + 1;
            /*int nID;

            if (m_dicPOIID.TryGetValue(poi.Facility.Type, out nID) == false)
                return null;

            m_dicPOIID[poi.Facility.Type] = nID + 1;*/

            bool isOutdoor;
            Floor floor;

            if (FormMain.Instance.GetFloorInfo(out isOutdoor, out floor))
                poi.Zone = floor.Zone;

            System.Diagnostics.Trace.WriteLine("OnAddPOI : " + poi.Facility.Type.ToString() + ", ID : " + poi.ID);

            List<POI> pois;

            if (m_dicAddedPOIs.TryGetValue(poi.Facility.Type, out pois) == false)
            {
                pois = new List<POI>();
                m_dicAddedPOIs[poi.Facility.Type] = pois;
                SomethingChanged();
            }

            m_systemInput = true;

            if (poi.Facility.Type == IFacility.FacilityType.CCTV)
            {
                CCTV cctv = (CCTV)poi.Facility;
                textBoxURL.Text = cctv.URL;

                if (cctv.URL.EndsWith("mp4"))
                    cctv.CCTVType = (int)UnE.Control.CCTVTypes.MediaPlayer;
                else
                    cctv.CCTVType = (int)UnE.Control.CCTVTypes.RTSP;
            }
            else
                textBoxURL.Text = "";

            m_systemInput = false;
            btnCctv_Click(btnMoveCctv, null);

            pois.Add(poi);
            return PoiToSensor(poi);
        }

        public SensorInfo OnMovePOI(POI poi)
        {
            if (poi.Facility == null)
                return null;

            System.Diagnostics.Trace.WriteLine("OnMovePOI : " + poi.Facility.Type.ToString() + ", ID : " + poi.ID + string.Format(", {0}, {1}, {2}", poi.X, poi.Y, poi.Z));
            POI addedPOI = FindAddedPOI(poi);

            if (addedPOI != null)
            {
                addedPOI.X = poi.X;
                addedPOI.Y = poi.Y;
                addedPOI.Z = poi.Z;
                poi = addedPOI;
            }
            else
            {
                List<POI> pois;

                if (m_dicChangedPOIs.TryGetValue(poi.Facility.Type, out pois) == false)
                {
                    pois = new List<POI>();
                    m_dicChangedPOIs[poi.Facility.Type] = pois;
                }

                if (pois.Contains(poi) == false)
                    pois.Add(poi);
            }

            SomethingChanged();
            return PoiToSensor(poi);
        }

        public SensorInfo OnDeletePOI(POI poi)
        {
            if (poi.Facility == null)
                return null;

            System.Diagnostics.Trace.WriteLine("OnDeletePOI : " + poi.Facility.Type.ToString() + ", ID : " + poi.ID);
            POI addedPOI = FindAddedPOI(poi);

            if (addedPOI != null)
            {
                List<POI> pois;

                if (m_dicAddedPOIs.TryGetValue(poi.Facility.Type, out pois))
                {
                    pois.Remove(addedPOI);
                    poi = addedPOI;
                }
            }
            else
            {
                List<POI> pois;

                if (m_dicDeletedPOIs.TryGetValue(poi.Facility.Type, out pois) == false)
                {
                    pois = new List<POI>();
                    m_dicDeletedPOIs[poi.Facility.Type] = pois;
                }

                pois.Add(poi);
            }

            SomethingChanged();
            return PoiToSensor(poi);
        }

        private void radioEditMode_CheckedChanged(object sender, EventArgs e)
        {
            if (m_panel == null)
                return;

            RadioButton radio = (RadioButton)sender;

            if (radio.Checked == false)
                return;

            SelectEditMode(m_panel);
        }

        public void OnSelectSensor(SensorInfo sensor, IFacility.FacilityType type)
        {
            m_systemInput = true;
            textBoxURL.Text = "";

            if (sensor == null)
                m_selectedSensor = null;
            else
                m_selectedSensor = sensor.Sensor;

            if (type == IFacility.FacilityType.CCTV)
            {
                if (sensor.Sensor != null && sensor.Sensor is CCTV)
                    OnSelectCCTV((CCTV)sensor.Sensor);
            }

            m_systemInput = false;
        }

        private void OnSelectCCTV(CCTV cctv)
        {
            textBoxURL.Text = cctv.URL;
        }

        public void OnChangeText(SensorInfo sensor, string strText, IFacility.FacilityType type)
        {
            List<POI> pois;
            POI addedPOI = null;

            if (m_dicAddedPOIs.TryGetValue(type, out pois))
            {
                foreach (POI poi in pois)
                {
                    if (poi.ID == sensor.SensorID)
                    {
                        addedPOI = poi;
                        break;
                    }
                }
            }

            if (type == IFacility.FacilityType.CCTV)
            {
                if (addedPOI != null && addedPOI.Facility != null && addedPOI.Facility is CCTV)
                {
                    CCTV cctv = (CCTV)addedPOI.Facility;
                    cctv.AccessKey = strText;
                }
                else
                {
                    CCTV cctv = SDMS.CCTVManager.Instance.GetCCTV(sensor.SensorID);

                    if (cctv != null && cctv.POI != null)
                    {
                        cctv.AccessKey = strText;
                        OnMovePOI(cctv.POI);
                    }
                }
            }
        }

        private void textBoxURL_TextChanged(object sender, EventArgs e)
        {
            if (m_systemInput)
                return;

            if (m_selectedSensor != null)
            {
                if (m_selectedSensor is CCTV)
                {
                    CCTV cctv = (CCTV)m_selectedSensor;

                    if (cctv != null && cctv.POI != null)
                    {
                        List<POI> pois;

                        if (m_dicChangedPOIs.TryGetValue(IFacility.FacilityType.CCTV, out pois) == false)
                        {
                            pois = new List<POI>();
                            m_dicChangedPOIs[IFacility.FacilityType.CCTV] = pois;
                        }

                        if (pois.Contains(cctv.POI) == false)
                            pois.Add(cctv.POI);
                    }                    

                    OnChangeCCTVURL((CCTV)m_selectedSensor, textBoxURL.Text.Trim());
                }
            }
        }

        private void OnChangeCCTVURL(CCTV cctv, string strURL)
        {
            cctv.URL = strURL;

            if (strURL.EndsWith("mp4"))
                cctv.CCTVType = (int)UnE.Control.CCTVTypes.MediaPlayer;
            else
                cctv.CCTVType = (int)UnE.Control.CCTVTypes.RTSP;

            SomethingChanged();
        }

        private void SomethingChanged()
        {
            if (HasChange())
            {
                if (btnSave.Enabled)
                    return;

                btnSave.Enabled = true;
                btnSave.Refresh();
            }
            else
            {
                if (btnSave.Enabled == false)
                    return;

                btnSave.Enabled = false;
                btnSave.Refresh();
            }
        }

        public bool HasChange()
        {
            return m_dicAddedPOIs.Count > 0 || m_dicDeletedPOIs.Count > 0 || m_dicChangedPOIs.Count > 0 || m_bChangeWall || m_bChangeSpaceText;
        }

        public void ClearChange()
        {
            // 현재 층을 조회해서 변경된 내용을 복구한다
            if (m_bChangeWall)
                LoadWalls();

            if (m_bChangeSpaceText)
                LoadSpaceTexts();

            if (m_dicAddedPOIs.Count > 0 || m_dicDeletedPOIs.Count > 0 || m_dicChangedPOIs.Count > 0)
                FormMain.Instance.DataManager.LoadCCTVPOI(FormMain.Instance.DBManager, true, true);

            m_dicAddedPOIs.Clear();
            m_dicDeletedPOIs.Clear();
            m_dicChangedPOIs.Clear();
            m_bChangeWall = false;
            m_bChangeSpaceText = false;
            btnSave.Enabled = false;
        }

        private bool m_bRefreshCCTV = false;

        public void Save()
        {
            WebDBManager dbMgr = FormMain.Instance.DBManager;

            if (SaveDelete(dbMgr) == false)
                return;
            if (SaveAdd(dbMgr) == false)
                return;
            if (SaveChange(dbMgr) == false)
                return;

            if (m_bRefreshCCTV)
            {
                // CCTV 정보가 변경되면 FormCCTVLink의 TreeView를 다시 만든다.
                if (FormMain.Instance.FrmCCTVLink != null)
                {
                    System.Threading.Thread newFrmThread = new System.Threading.Thread(FormMain.Instance.FrmCCTVLink.RefreshForm);
                    newFrmThread.Start();
                }

                m_bRefreshCCTV = false;
            }

            if (m_bChangeWall)
            {
                GetWalls();
                m_bChangeWall = false;
            }

            if (m_bChangeSpaceText)
            {
                GetSpaceTexts();
                m_bChangeSpaceText = false;
            }

            btnSave.Enabled = false;
            btnSave.Refresh();
        }

        private bool SaveDelete(WebDBManager dbMgr)
        {
            foreach (KeyValuePair<IFacility.FacilityType, List<POI>> pair in m_dicDeletedPOIs)
            {
                string strTableName = GetTableName(pair.Key);

                if (strTableName.Length == 0)
                    continue;

                string strIDs = GetPOIIDs(pair.Value);

                if (strIDs.Length == 0)
                    continue;

                if (pair.Key == IFacility.FacilityType.CCTV)
                {
                    string strSQL2 = string.Format("Delete from EquipZoneCCTV where CCTV1 in ({0}) or CCTV2 in ({0}) or CCTV3 in ({0}) or CCTV4 in ({0}) or CCTV5 in ({0}) or CCTV6 in ({0})", strIDs);
                    if (dbMgr.GetResultData(strSQL2) == null)
                        return false;

                    m_bRefreshCCTV = true;
                }

                string strSQL = string.Format("Delete from {0} where ID in ({1})", strTableName, strIDs);
                if (dbMgr.GetResultData(strSQL) == null)
                    return false;

                foreach (POI poi in pair.Value)
                {
                    if (poi.Facility != null && poi.Facility is CCTV)
                    {
                        SDMS.CCTVManager.Instance.DeleteCCTV((CCTV)poi.Facility);
                    }
                }
            }

            m_dicDeletedPOIs.Clear();
            return true;
        }

        private bool SaveAdd(WebDBManager dbMgr)
        {
            foreach (KeyValuePair<IFacility.FacilityType, List<POI>> pair in m_dicAddedPOIs)
            {
                foreach (POI poi in pair.Value)
                {
                    string strSQL = GetInsertQuery(pair.Key, poi);

                    if (strSQL.Length == 0)
                        return false;

                    if (dbMgr.GetResultData(strSQL) == null)
                        return false;

                    AddToDataManager(poi);
                }
            }

            m_dicAddedPOIs.Clear();
            return true;
        }

        private bool SaveChange(WebDBManager dbMgr)
        {
            foreach (KeyValuePair<IFacility.FacilityType, List<POI>> pair in m_dicChangedPOIs)
            {
                foreach (POI poi in pair.Value)
                {
                    string strSQL = GetUpdateQuery(pair.Key, poi);

                    if (strSQL.Length == 0)
                        return false;

                    if (dbMgr.GetResultData(strSQL) == null)
                        return false;
                }
            }

            m_dicChangedPOIs.Clear();
            return true;
        }

        private void AddToDataManager(POI poi)
        {
            if (poi.Facility == null)
                return;

            Zone zone = poi.Zone;

            if (zone == null)
            {
                bool isOutdoor;
                Floor floor;

                if (FormMain.Instance.GetFloorInfo(out isOutdoor, out floor) == false)
                    return;

                zone = floor.Zone;

                if (zone == null)
                    return;

                poi.Zone = zone;
            }

            if (poi.Facility.Type == IFacility.FacilityType.CCTV)
                SDMS.CCTVManager.Instance.AddCCTV((CCTV)poi.Facility, zone.ID, poi.X, poi.Y, poi.Z, true);
        }

        private string GetPOIIDs(List<POI> pois)
        {
            string strIDs = "";

            foreach (POI poi in pois)
            {
                if (strIDs.Length == 0)
                    strIDs = poi.ID.ToString();
                else
                    strIDs += "," + poi.ID.ToString();
            }

            return strIDs;
        }

        private string GetTableName(IFacility.FacilityType type)
        {
            if (type == IFacility.FacilityType.CCTV)
                return Data.CommonString.POI_CCTV_Table;
            else if (type == IFacility.FacilityType.FIRE_SENSOR)
                return Data.CommonString.POI_Fire_Table;
            else if (type == IFacility.FacilityType.FIREWALL)
                return Data.CommonString.POI_FireWall_Table;
            else if (type == IFacility.FacilityType.DOOR)
                return Data.CommonString.POI_Door_Table;
            else if (type == IFacility.FacilityType.PSM_SENSOR)
                return Data.CommonString.POI_Gas_Table;

            return "";
        }

        private string GetInsertQuery(IFacility.FacilityType type, POI poi)
        {
            if (type == IFacility.FacilityType.CCTV)
                return GetCCTVInsertQuery(poi);
            else if (type == IFacility.FacilityType.FIRE_SENSOR)
                return GetFireInsertQuery(poi);
            else if (type == IFacility.FacilityType.FIREWALL)
                return GetEtcInsertQuery(poi, Data.CommonString.POI_FireWall_Table);
            else if (type == IFacility.FacilityType.DOOR)
                return GetEtcInsertQuery(poi, Data.CommonString.POI_Door_Table);
            else if (type == IFacility.FacilityType.PSM_SENSOR)
                return GetGasInsertQuery(poi);

            return "";
        }

        private string GetUpdateQuery(IFacility.FacilityType type, POI poi)
        {
            if (type == IFacility.FacilityType.CCTV)
                return GetCCTVUpdateQuery(poi);
            else if (type == IFacility.FacilityType.FIRE_SENSOR)
                return GetFireUpdateQuery(poi);
            else if (type == IFacility.FacilityType.FIREWALL)
                return GetEtcUpdateQuery(poi, Data.CommonString.POI_FireWall_Table);
            else if (type == IFacility.FacilityType.DOOR)
                return GetEtcUpdateQuery(poi, Data.CommonString.POI_Door_Table);
            else if (type == IFacility.FacilityType.PSM_SENSOR)
                return GetGasUpdateQuery(poi);

            return "";
        }
        private string GetCCTVUpdateQuery(POI poi)
        {
            if (poi.Facility != null && poi.Facility is CCTV)
            {
                Zone zone = poi.Zone;

                if (zone == null)
                {
                    bool isOutdoor;
                    Floor floor;

                    if (FormMain.Instance.GetFloorInfo(out isOutdoor, out floor) == false)
                        return "";

                    zone = floor.Zone;

                    if (zone == null)
                        return "";

                    poi.Zone = zone;
                }

                CCTV cctv = (CCTV)poi.Facility;

                string strFormat = "Update " + Data.CommonString.POI_CCTV_Table + " Set CameraName = '{0}', PositionName = '{0}', X = {1}, Y = {2}, Z = {3}, URL = '{4}' where ID = {5}";
                string strSQL = string.Format(strFormat, cctv.AccessKey, poi.X, poi.Y, poi.Z, cctv.URL, poi.ID);
                return strSQL;
            }

            return "";
        }

        private string GetFireUpdateQuery(POI poi)
        {
            if (poi.Facility != null && poi.Facility is FireSensor)
            {
                Zone zone = poi.Zone;

                if (zone == null)
                {
                    bool isOutdoor;
                    Floor floor;

                    if (FormMain.Instance.GetFloorInfo(out isOutdoor, out floor) == false)
                        return "";

                    zone = floor.Zone;

                    if (zone == null)
                        return "";

                    poi.Zone = zone;
                }

                FireSensor sensor = (FireSensor)poi.Facility;

                string strFormat = "Update " + Data.CommonString.POI_Fire_Table + " Set Name = '{0}', PositionName = '{1}', X = {2}, Y = {3}, Z = {4} where ID = {5}";
                string strSQL = string.Format(strFormat, sensor.SensorName, sensor.PositionName, poi.X, poi.Y, poi.Z, poi.ID);
                return strSQL;
            }

            return "";
        }

        private string GetEtcUpdateQuery(POI poi, string strTableName)
        {
            if (poi.Facility != null && poi.Facility is EtcSensor)
            {
                Zone zone = poi.Zone;

                if (zone == null)
                {
                    bool isOutdoor;
                    Floor floor;

                    if (FormMain.Instance.GetFloorInfo(out isOutdoor, out floor) == false)
                        return "";

                    zone = floor.Zone;

                    if (zone == null)
                        return "";

                    poi.Zone = zone;
                }

                EtcSensor sensor = (EtcSensor)poi.Facility;

                string strFormat = "Update " + strTableName + " Set Name = '{0}', PositionName = '{1}', X = {2}, Y = {3}, Z = {4} where ID = {5}";
                string strSQL = string.Format(strFormat, sensor.SensorName, sensor.PositionName, poi.X, poi.Y, poi.Z, poi.ID);
                return strSQL;
            }

            return "";
        }

        private string GetGasUpdateQuery(POI poi)
        {
            return "";
        }

        private string GetCCTVInsertQuery(POI poi)
        {
            if (poi.Facility != null && poi.Facility is CCTV)
            {
                m_bRefreshCCTV = true;

                Zone zone = poi.Zone;

                if (zone == null)
                {
                    bool isOutdoor;
                    Floor floor;

                    if (FormMain.Instance.GetFloorInfo(out isOutdoor, out floor) == false)
                        return "";

                    zone = floor.Zone;

                    if (zone == null)
                        return "";

                    poi.Zone = zone;
                }

                CCTV cctv = (CCTV)poi.Facility;

                string strSQL = "Insert into " + Data.CommonString.POI_CCTV_Table + " (ID, CameraName, IPAddr, Port, PositionName, X, Y, Z, ZoneID, IsIndoor, LOD, Description, HTTPPort, Type, Stream, Channel, UserID, Password, URL, ReversePTZ, BigURL, SmallURL) values ";
                strSQL += string.Format("({0}, '{1}', '', 554, '{1}', {2}, {3}, {4}, {5}, 1, 1, NULL, NULL, 'RTSP', NULL, NULL, NULL, NULL, '{6}', NULL, NULL, NULL)", poi.ID, cctv.AccessKey, poi.X, poi.Y, poi.Z, poi.Zone.ID, cctv.URL);
                return strSQL;
            }

            return "";
        }

        private string GetFireInsertQuery(POI poi)
        {
            if (poi.Facility != null && poi.Facility is FireSensor)
            {
                Zone zone = poi.Zone;

                if (zone == null)
                {
                    bool isOutdoor;
                    Floor floor;

                    if (FormMain.Instance.GetFloorInfo(out isOutdoor, out floor) == false)
                        return "";

                    zone = floor.Zone;

                    if (zone == null)
                        return "";

                    poi.Zone = zone;
                }

                FireSensor sensor = (FireSensor)poi.Facility;

                string strSQL = "Insert into " + Data.CommonString.POI_Fire_Table + " (ID, Name, PositionName, X, Y, Z, ZoneID, IsIndoor, Description, Department, DepartmentPhoneNumber) values ";
                strSQL += string.Format("({0}, '{1}', '{2}', {3}, {4}, {5}, {6}, 1, NULL, NULL, NULL)", poi.ID, sensor.SensorName, sensor.PositionName, poi.X, poi.Y, poi.Z, poi.Zone.ID);
                return strSQL;
            }

            return "";
        }

        private string GetEtcInsertQuery(POI poi, string strTableName)
        {
            if (poi.Facility != null && poi.Facility is EtcSensor)
            {
                Zone zone = poi.Zone;

                if (zone == null)
                {
                    bool isOutdoor;
                    Floor floor;

                    if (FormMain.Instance.GetFloorInfo(out isOutdoor, out floor) == false)
                        return "";

                    zone = floor.Zone;

                    if (zone == null)
                        return "";

                    poi.Zone = zone;
                }

                EtcSensor sensor = (EtcSensor)poi.Facility;

                string strSQL = "Insert into " + strTableName + " (ID, Name, PositionName, X, Y, Z, ZoneID, IsIndoor, Description, Department, DepartmentPhoneNumber) values ";
                strSQL += string.Format("({0}, '{1}', '{2}', {3}, {4}, {5}, {6}, 1, NULL, NULL, NULL)", poi.ID, sensor.SensorName, sensor.PositionName, poi.X, poi.Y, poi.Z, poi.Zone.ID);
                return strSQL;
            }

            return "";
        }

        private string GetGasInsertQuery(POI poi)
        {
            return "";
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            Save();
        }

        public void CurrentMode()
        {
            if (m_curEditType == EditType.Wall)
            {
                m_panel.SetWallEditMode(true);
                m_panel.HideIconLayer("CCTV");
                m_panel.VisibleViewButton("imgWallEditMode", true);
            }
            else
            {
                m_panel.ShowIconLayer("CCTV");
                m_panel.VisibleViewButton("imgEditMode", true);
            }
        }

        public void VisibleImageEditMode(bool visible)
        {
            if (m_panel != null)
            {
                m_panel.VisibleViewButton("imgEditMode", visible);
                m_panel.VisibleViewButton("imgWallEditMode", visible); 
            }
        }

        private void btnAddWall_Click(object sender, EventArgs e)
        {
            FormMain.Instance.ContentManager.ContentForm.AddWall();
        }

        private readonly string m_strWallsInfoPath = @"C:\UNE\Unity\Walls\";
        private readonly string m_strSpaceTextsInfoPath = @"C:\UNE\Unity\SpaceText\";
        private void GetWalls()
        {
            this.Cursor = Cursors.WaitCursor;

            Zone zone = FormMain.Instance.GetZone();
            string strSceneName;
            if (FormMain.Instance.OptionMgr.DicZoneScene.TryGetValue(zone.ID, out strSceneName))
            {
                if (!Directory.Exists(m_strWallsInfoPath))
                    Directory.CreateDirectory(m_strWallsInfoPath);
                
                FormMain.Instance.ContentManager.ContentForm.GetWalls(m_strWallsInfoPath);

                int nSleep = 0;
                int nTimeout = 5 * 1000;
                int nSleepTime = 1000;

                bool isExists = false;
                
                while (nSleep < nTimeout)
                {
                    System.Threading.Thread.Sleep(nSleepTime);
                    nSleep += nSleepTime;
                    
                    DirectoryInfo di = new DirectoryInfo(m_strWallsInfoPath);                    
                    if (di.GetFiles().Length > 0)
                    {
                        isExists = true;
                        break;
                    }
                }

                if (isExists)
                {
                    SaveWall();
                }
            }

            this.Cursor = Cursors.Default;
        }

        public void LoadWalls()
        {
            Zone zone = FormMain.Instance.GetZone();
            if (zone == null)
                return;

            string strSceneName;
            if (!FormMain.Instance.OptionMgr.DicZoneScene.TryGetValue(zone.ID, out strSceneName))
                return;

            string strPath = m_strWallsInfoPath + strSceneName + ".txt";

            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT ID, X, Y, Z, Rotate, Scale ");
            sb.Append("  FROM FakeWall ");
            sb.AppendFormat(" WHERE ZoneID = {0} ", zone.ID);
            
            ArrayList arrResult = FormMain.Instance.DBManager.GetResultData(sb.ToString());
            if (arrResult != null || arrResult.Count > 0)
            {
                List<string> strs = new List<string>();

                for (int i = 0; i < arrResult.Count; i += 6)
                {
                    int nID = DBUtility2.WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                    float x = DBUtility2.WebDBManager.GetFloatField(arrResult[i + 1].ToString(), 0.0f);
                    float y = DBUtility2.WebDBManager.GetFloatField(arrResult[i + 2].ToString(), 0.0f);
                    float z = DBUtility2.WebDBManager.GetFloatField(arrResult[i + 3].ToString(), 0.0f);
                    float rotate = DBUtility2.WebDBManager.GetFloatField(arrResult[i + 4].ToString(), 0.0f);
                    float scale = DBUtility2.WebDBManager.GetFloatField(arrResult[i + 5].ToString(), 0.0f);

                    strs.Add(string.Format("{0},{1},{2},{3},{4}", x, y, z, rotate, scale));
                }

                if (!Directory.Exists(m_strWallsInfoPath))
                    Directory.CreateDirectory(m_strWallsInfoPath);
                
                using (StreamWriter sw = new StreamWriter(strPath, false, Encoding.UTF8))
                {
                    foreach (string str in strs)
                    {
                        sw.WriteLine(str);
                    }
                }
            }
            FormMain.Instance.ContentManager.ContentForm.LoadWalls(strPath, strSceneName);
        }

        private void SaveWall()
        {
            DirectoryInfo dirInfo = new DirectoryInfo(m_strWallsInfoPath);
            FileInfo[] files = dirInfo.GetFiles();
            foreach (FileInfo fileInfo in files)
            {
                string strSceneName = fileInfo.Name.Replace(".txt", "");
                string strPath = fileInfo.FullName;
                int zoneID = -1;

                foreach (KeyValuePair<int, string> item in FormMain.Instance.OptionMgr.DicZoneScene)
                {
                    if (item.Value == strSceneName)
                    {
                        zoneID = item.Key;
                        break;
                    }
                }

                if (zoneID < 0)
                    continue;

                FormMain.Instance.DBManager.GetResultData("Delete From FakeWall Where ZoneID =" + zoneID);

                List<string> querys = new List<string>();

                using (StreamReader sr = new StreamReader(strPath))
                {
                    while (sr.EndOfStream == false)
                    {
                        string strLine = sr.ReadLine().Trim();

                        if (strLine.Length == 0)
                            continue;

                        string[] args = strLine.Split(',');
                        if (args.Length != 5)
                            continue;
                        
                        float x;
                        float y;
                        float z;
                        float rotate;
                        float scale;

                        if (!float.TryParse(args[0], out x) || !float.TryParse(args[1], out y) || !float.TryParse(args[2], out z) || !float.TryParse(args[3], out rotate) || !float.TryParse(args[4], out scale))
                            continue;

                        string strQuery = "Insert Into FakeWall (ID, ZoneID, X, Y, Z, Rotate, Scale) Values ((select isnull(max(id)+1, 1) from FakeWall),{0},{1},{2},{3},{4},{5})";
                        strQuery = string.Format(strQuery, zoneID, x, y, z, rotate, scale);

                        FormMain.Instance.DBManager.GetResultData(strQuery);
                    }
                }
            }

            for (int i = files.Length - 1; i >= 0; i--)
            {
                File.Delete(files[i].FullName);
            }
        }
        
        private bool m_bChangeWall = false;
        public void ChangeWall()
        {
            m_bChangeWall = true;
            SomethingChanged();
        }

        private bool m_bChangeSpaceText = false;
        public void ChangeSpaceText()
        {
            m_bChangeSpaceText = true;
            SomethingChanged();
        }

        private bool m_bSnap = true;
        private void btnSnap_Click(object sender, EventArgs e)
        {
            m_bSnap = !m_bSnap;
            if (m_bSnap)
                btnSnap.ImageNormal = global::SDMS_Building.Properties.Resources.check_Checked;
            else
                btnSnap.ImageNormal = global::SDMS_Building.Properties.Resources.check_UnChecked;
            btnSnap.Refresh();

            FormMain.Instance.ContentManager.ContentForm.SetWallSnap(m_bSnap);
        }

        public void GetWallInfo(float x, float y, float scale, float rotate)
        {
            txtX.Text = Math.Round(x, 2).ToString();
            txtY.Text = Math.Round(y, 2).ToString();
            txtScale.Text = Math.Round(scale, 2).ToString();
            txtRotate.Text = Math.Round(rotate, 2).ToString();
        }
        
        private void btnAddText_Click(object sender, EventArgs e)
        {
            if (txtUserText.Text.Length == 0)
                return;

            FormMain.Instance.ContentManager.ContentForm.AddSpaceText(txtUserText.Text);

            txtUserText.Text = "";
        }

        public void LoadSpaceTexts()
        {
            Zone zone = FormMain.Instance.GetZone();
            if (zone == null)
                return;

            string strSceneName;
            if (!FormMain.Instance.OptionMgr.DicZoneScene.TryGetValue(zone.ID, out strSceneName))
                return;

            List<string> strs = new List<string>();

            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT ID, DisplayText, X, Y, Z ");
            sb.Append("  FROM SpaceText ");
            sb.AppendFormat(" WHERE ZoneID = {0} AND SiteID = {1} ", zone.ID, UnE.SOP.ProxySOP.Instance.SiteID);
            
            ArrayList arrResult = FormMain.Instance.DBManager.GetResultData(sb.ToString());
            if (arrResult != null && arrResult.Count > 0)
            {
                for (int i = 0; i < arrResult.Count; i += 5)
                {
                    int nID = DBUtility2.WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                    string strDisplayText = DBUtility2.WebDBManager.GetStringField(arrResult[i + 1].ToString());
                    float x = DBUtility2.WebDBManager.GetFloatField(arrResult[i + 2].ToString(), 0.0f);
                    float y = DBUtility2.WebDBManager.GetFloatField(arrResult[i + 3].ToString(), 0.0f);
                    float z = DBUtility2.WebDBManager.GetFloatField(arrResult[i + 4].ToString(), 0.0f);

                    strs.Add(string.Format("{0},{1},{2},{3}", strDisplayText, x, y, z));
                }
            }

            if (!Directory.Exists(m_strSpaceTextsInfoPath))
                Directory.CreateDirectory(m_strSpaceTextsInfoPath);

            string strPath = m_strSpaceTextsInfoPath + strSceneName + ".txt";

            using (StreamWriter sw = new StreamWriter(strPath, false, Encoding.UTF8))
            {
                foreach (string str in strs)
                {
                    sw.WriteLine(str);
                }
            }

            FormMain.Instance.ContentManager.ContentForm.LoadSpaceTexts(strPath, strSceneName);
        }

        private void GetSpaceTexts()
        {
            this.Cursor = Cursors.WaitCursor;

            Zone zone = FormMain.Instance.GetZone();
            string strSceneName;
            if (FormMain.Instance.OptionMgr.DicZoneScene.TryGetValue(zone.ID, out strSceneName))
            {
                if (!Directory.Exists(m_strSpaceTextsInfoPath))
                    Directory.CreateDirectory(m_strSpaceTextsInfoPath);

                FormMain.Instance.ContentManager.ContentForm.GetSpaceTexts(m_strSpaceTextsInfoPath);

                int nSleep = 0;
                int nTimeout = 5 * 1000;
                int nSleepTime = 1000;

                bool isExists = false;

                while (nSleep < nTimeout)
                {
                    System.Threading.Thread.Sleep(nSleepTime);
                    nSleep += nSleepTime;

                    DirectoryInfo di = new DirectoryInfo(m_strSpaceTextsInfoPath);
                    if (di.GetFiles().Length > 0)
                    {
                        isExists = true;
                        break;
                    }
                }

                if (isExists)
                {
                    SaveSpaceText();
                }
            }

            this.Cursor = Cursors.Default;
        }

        private void SaveSpaceText()
        {
            DirectoryInfo dirInfo = new DirectoryInfo(m_strSpaceTextsInfoPath);
            FileInfo[] files = dirInfo.GetFiles();
            foreach (FileInfo fileInfo in files)
            {
                string strSceneName = fileInfo.Name.Replace(".txt", "");
                string strPath = fileInfo.FullName;
                int zoneID = -1;

                foreach (KeyValuePair<int, string> item in FormMain.Instance.OptionMgr.DicZoneScene)
                {
                    if (item.Value == strSceneName)
                    {
                        zoneID = item.Key;
                        break;
                    }
                }

                if (zoneID < 0)
                    continue;

                FormMain.Instance.DBManager.GetResultData("Delete From SpaceText Where ZoneID =" + zoneID);

                List<string> querys = new List<string>();

                using (StreamReader sr = new StreamReader(strPath))
                {
                    while (sr.EndOfStream == false)
                    {
                        string strLine = sr.ReadLine().Trim();

                        if (strLine.Length == 0)
                            continue;

                        string[] args = strLine.Split(',');
                        if (args.Length != 4)
                            continue;

                        string text;
                        float x;
                        float y;
                        float z;

                        if (!float.TryParse(args[1], out x) || !float.TryParse(args[2], out y) || !float.TryParse(args[3], out z))
                            continue;

                        text = args[0];
                        string strQuery = "Insert Into SpaceText (ID, ZoneID, DisplayText, X, Y, Z, SiteID) Values ((select isnull(max(id)+1, 1) from SpaceText),{0},'{1}',{2},{3},{4},{5})";
                        strQuery = string.Format(strQuery, zoneID, text, x, y, z, UnE.SOP.ProxySOP.Instance.SiteID);

                        FormMain.Instance.DBManager.GetResultData(strQuery);
                    }
                }
            }

            for (int i = files.Length - 1; i >= 0; i--)
            {
                File.Delete(files[i].FullName);
            }
        }

        private void btnCctvLink_Click(object sender, EventArgs e)
        {
            if (FormMain.Instance.FrmCCTVLink == null)
                return;

            //PopupDialog.PopupBackground back = new PopupDialog.PopupBackground();
            //back.StartPosition = FormStartPosition.Manual;
            //back.Size = new Size(1920, 1080);
            //back.Location = FormMain.Instance.Location;
            //back.Show();
            
            FormMain.Instance.FrmCCTVLink.StartPosition = FormStartPosition.CenterParent;
            FormMain.Instance.FrmCCTVLink.ShowDialog();
            //back.Close();
        }
        
        private void btnMode_Click(object sender, EventArgs e)
        {
            RibbonButton btn = sender as RibbonButton;
            if (btn.IsChecked)
                return;

            btn.IsChecked = true;

            if (btn == btnCCTVMode)
            {
                btnWallMode.IsChecked = false;
                
                m_curEditType = EditType.CCTV;
                pnEditCCTV.Visible = true;
                pnEditWall.Visible = false;

                FormMain.Instance.ContentManager.ContentForm.SetWallEditMode(false);
                m_panel.ShowIconLayer("CCTV");

                m_panel.VisibleViewButton("imgEditMode", true);
                m_panel.VisibleViewButton("imgWallEditMode", false);
            }
            else
            {
                btnCCTVMode.IsChecked = false;

                m_curEditType = EditType.Wall;
                pnEditCCTV.Visible = false;
                pnEditWall.Visible = true;

                btnAddCctv.IsChecked = false;
                btnMoveCctv.IsChecked = false;
                btnDeleteCctv.IsChecked = false;

                FormMain.Instance.ContentManager.ContentForm.SetWallEditMode(true);
                m_panel.HideIconLayer("CCTV");

                m_panel.VisibleViewButton("imgEditMode", false);
                m_panel.VisibleViewButton("imgWallEditMode", true);
            }            

            btnCCTVMode.Refresh();
            btnWallMode.Refresh();
        }

        private void btnCctv_Click(object sender, EventArgs e)
        {
            if (m_panel == null)
                return;

            RibbonButton btn = sender as RibbonButton;
            //if (btn.IsChecked)
            //    return;

            if (btn == btnAddCctv)
            {
                btnMoveCctv.IsChecked = false;
                btnDeleteCctv.IsChecked = false;
            }
            else if (btn == btnMoveCctv)
            {
                btnAddCctv.IsChecked = false;
                btnDeleteCctv.IsChecked = false;
            }
            else if (btn == btnDeleteCctv)
            {
                btnAddCctv.IsChecked = false;
                btnMoveCctv.IsChecked = false;
            }

            btn.IsChecked = !btn.IsChecked;

            btnAddCctv.Refresh();
            btnMoveCctv.Refresh();
            btnDeleteCctv.Refresh();


            SelectEditMode(m_panel);
        }
    }
}
