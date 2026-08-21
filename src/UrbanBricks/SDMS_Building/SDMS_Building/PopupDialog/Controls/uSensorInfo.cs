using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Collections;
using UnE.GUI;
using SDMS_Building.Properties;
using static UnE.Sensor.IFacility;
using UnE.Sensor;
using SDMS;
using SDMS_Building.Data;
using UnE.PSM;
using UnE.Spatial;
using UnE.Util.Unity;

namespace SDMS_Building.PopupDialog.Controls
{
    public partial class uSensorInfo : UserControl
    {
        private List<RibbonButton> m_arrSensorInfos = null;
        public List<RibbonButton> ArrSensorInfos
        {
            get { return m_arrSensorInfos; }
            set { m_arrSensorInfos = value; }
        }
        
        private Timer m_timermove = null;
        public Timer TimerMove
        {
            get { return m_timermove; }
            set { m_timermove = value; }
        }

        private FacilityType m_selectedType = FacilityType.FIRE_SENSOR;
        public FacilityType SelectedType
        {
            get { return m_selectedType; }
            set
            {
                m_selectedType = value;
                SetLayerState();
            }
        }

        private Dictionary<FacilityType, string> m_dicFacilityTypeName = new Dictionary<FacilityType, string>();

        public uSensorInfo()
        {
            InitializeComponent();
            
            m_timermove = new Timer();
            m_timermove.Interval = 100;
            m_timermove.Tick += MoveTimer_Tick;            
            m_timermove.Enabled = true;
        }

        private void MoveTimer_Tick(object sender, EventArgs e)
        {
            if (m_arrSensorInfos == null)
                return;

            Point beginLoc = m_arrSensorInfos[m_nShowIndex].Location; // beginLoc가 pnMain.Location 되어야함
            Point curLoc = pnMain.Location;

            if (beginLoc.X != curLoc.X)
            {
                int gap = 15;
                int x = 0;
                if (beginLoc.X * -1 > curLoc.X)
                    x = pnMain.Location.X + gap;
                else
                    x = pnMain.Location.X - gap;

                int targetX = beginLoc.X * -1;

                pnMain.Location = new Point(x, curLoc.Y);

                if (pnMain.Location.X == targetX)
                    m_timermove.Enabled = false;                
            }
            else
                m_timermove.Enabled = false;
        }

        public RibbonButton BtnFire = null;
        public RibbonButton BtnCCTV = null;
        public RibbonButton BtnDoor = null;
        public void InitSensorInfo()
        {            
            Color unCheckedForeColor = Color.FromArgb(0x84, 0x91, 0xd6);
            if (m_arrSensorInfos == null)
            {
                m_arrSensorInfos = new List<RibbonButton>();

                // 화재
                RibbonButton rbtn = CreateButton(Resources.poi_fire_normal, Resources.poi_fire_off, Color.FromArgb(0xff, 0x4b, 0x6a), unCheckedForeColor, Data.CommonString.POI_Fire_Kor);
                rbtn.Tag = FacilityType.FIRE_SENSOR;
                m_dicFacilityTypeName[FacilityType.FIRE_SENSOR] = Data.CommonString.POI_Fire;
                m_arrSensorInfos.Add(rbtn);
                BtnFire = rbtn; // System Load 후 처음 조회할 항목

                // CCTV
                rbtn = CreateButton(Resources.poi_cctv_normal, Resources.poi_cctv_off, Color.FromArgb(0x12, 0x94, 0xff), unCheckedForeColor, Data.CommonString.POI_CCTV_Kor);
                rbtn.Tag = FacilityType.CCTV;
                m_dicFacilityTypeName[FacilityType.CCTV] = Data.CommonString.POI_CCTV;
                m_arrSensorInfos.Add(rbtn);
                BtnCCTV = rbtn;

                //if (UnE.SOP.ProxySOP.Instance.UsePSM)
                //{
                //    string txt = "누출";
                //    if (UnE.SOP.ProxySOP.Instance.SiteID == 201)
                //        txt = Data.CommonString.POI_Gas_Kor;
                    
                //    rbtn = CreateButton(Resources.poi_psm_normal, Resources.poi_psm_off, Color.FromArgb(0x9b, 0x77, 0xa4), unCheckedForeColor, txt);
                //    rbtn.Tag = FacilityType.PSM_SENSOR;
                //    m_dicFacilityTypeName[FacilityType.PSM_SENSOR] = Data.CommonString.POI_Gas;
                //    m_arrSensorInfos.Add(rbtn);
                //}
                if (UnE.SOP.ProxySOP.Instance.UseDoor)
                {
                    rbtn = CreateButton(Resources.poi_door_normal, Resources.poi_door_off, Color.FromArgb(0xef, 0xab, 0x00), unCheckedForeColor, Data.CommonString.POI_Door_Kor);
                    rbtn.Tag = FacilityType.DOOR;
                    m_dicFacilityTypeName[FacilityType.DOOR] = Data.CommonString.POI_Door;
                    m_arrSensorInfos.Add(rbtn);
                    BtnDoor = rbtn;
                }
                if (UnE.SOP.ProxySOP.Instance.UseFirewall)
                {
                    rbtn = CreateButton(Resources.poi_firewall_normal, Resources.poi_firewall_off, Color.FromArgb(0xb6, 0x7d, 0x5e), unCheckedForeColor, Data.CommonString.POI_FireWall_Kor);
                    rbtn.Tag = FacilityType.FIREWALL;
                    m_dicFacilityTypeName[FacilityType.FIREWALL] = Data.CommonString.POI_FireWall;
                    m_arrSensorInfos.Add(rbtn);
                }

                int nEmpty = 35;
                int x = 0;

                for (int i = 0; i < m_arrSensorInfos.Count; i++)
                {
                    RibbonButton rbtn2 = m_arrSensorInfos[i];
                    rbtn2.Location = new Point(x, 0);
                    x += rbtn2.Width + nEmpty;
                }

                pnMain.Size = new Size(m_arrSensorInfos.Count * m_btnSize.Width + ((m_arrSensorInfos.Count - 1) * nEmpty), m_btnSize.Height);
                pnMain.Location = new Point(0, 0);
                pnMain.Parent = this;
            }
        }

        private int m_nShowIndex = 0;
        public int ShowIndex
        {
            get { return m_nShowIndex; }
            set { m_nShowIndex = value; }
        }

        private Size m_btnSize = new Size(55, 97);
        private RibbonButton CreateButton(Image checkedImg, Image unCheckedImg, Color foreColorChecked, Color foreColorUnchecked, string txt)
        {
            RibbonButton rbtn = new RibbonButton();
            rbtn.CheckedImage = checkedImg;
            rbtn.CheckedMouseOver = checkedImg;
            rbtn.MouseOverImage = unCheckedImg;
            rbtn.NormalImage = unCheckedImg;
            rbtn.CustomImageRect = new System.Drawing.Rectangle(0, 0, 55, 55);
            rbtn.ForeColor = foreColorUnchecked;
            rbtn.ForeColorChecked = foreColorChecked;
            rbtn.ForeColorCheckedMouseOver = foreColorChecked;
            rbtn.ForeColorDisabled = foreColorUnchecked;
            rbtn.ForeColorMouseOver = foreColorUnchecked;
            rbtn.ForeColorsByTypeUse = true;
            rbtn.InitButtonWidth = 55;
            rbtn.Size = m_btnSize;
            rbtn.Text = txt;
            rbtn.TextLocation = new System.Drawing.Point(0, 60);
            rbtn.TextPos = UnE.GUI.RibbonButton.TextPosition.BOTTOM;
            rbtn.ToolTipText = txt;
            rbtn.UseCustomImageRect = true;
            rbtn.UseTextLocation = false;
            rbtn.UseVisualStyleBackColor = true;
            rbtn.Parent = pnMain;
            rbtn.Click += Rbtn_Click;

            return rbtn;
        }

        public void Rbtn_Click(object sender, EventArgs e)
        {
            RibbonButton rbtn = sender as RibbonButton;

            for (int i = 0; i < m_arrSensorInfos.Count; i++)
            {
                RibbonButton rbtn2 = m_arrSensorInfos[i] as RibbonButton;
                if (rbtn == rbtn2)
                    rbtn2.IsChecked = true;
                else
                    rbtn2.IsChecked = false;

                rbtn2.Refresh();
            }
            
            FacilityType type = (FacilityType)rbtn.Tag;
            SelectedType = type;

            Zone zone = FormMain.Instance.GetZone();
            //Display(zone);
        }

        public void SelectType(IFacility.FacilityType type)
        {
            for (int i = 0; i < m_arrSensorInfos.Count; i++)
            {
                RibbonButton rbtn = m_arrSensorInfos[i] as RibbonButton;
                if ((IFacility.FacilityType)rbtn.Tag == type)
                    rbtn.IsChecked = true;
                else
                    rbtn.IsChecked = false;

                rbtn.Refresh();
            }

            SelectedType = type;

            //Zone zone = FormMain.Instance.GetZone();
            //Display(zone);
        }

        //public void Display(Zone zone)
        //{
        //    if (zone == null)
        //    {
        //        FormMain.Instance.SetSensorInfo(null, SelectedType);
        //    }
        //    else
        //    {
        //        List<EquipmentZone> equipZoneList = ZoneManager.Instance.GetEquipmentZoneList(zone);
                
        //        bool isCCTV = false;

        //        List<SensorInfo> results = new List<SensorInfo>();
        //        if (SelectedType == FacilityType.FIRE_SENSOR)
        //        {
        //            if (equipZoneList != null && equipZoneList.Count > 0)
        //            {
        //                foreach (KeyValuePair<int, List<ISensor>> sensors2 in SensorManager.Instance.DicFireSensor)
        //                {
        //                    foreach (ISensor sensor in sensors2.Value)
        //                    {
        //                        foreach (EquipmentZone equipZone in equipZoneList)
        //                        {
        //                            if (equipZone.ID == sensor.EquipZoneID)
        //                            {
        //                                SensorInfo result = new SensorInfo();
        //                                result.SensorID = sensor.OrgSensorID;
        //                                result.SensorName = sensor.SensorName;
        //                                result.DeActivate = sensor.DeActivate;
        //                                result.ISConnected = sensor.Connected;
        //                                result.Sensor = sensor;
        //                                results.Add(result);                                        
        //                                break;
        //                            }
        //                        }
        //                    }
        //                } 
        //            }
        //        }
        //        else if (SelectedType == FacilityType.CCTV)
        //        {
        //            isCCTV = true;
        //            foreach (CCTV cctv in CCTVManager.Instance.OutdoorCCTVList)
        //            {
        //                SensorInfo result = new SensorInfo();
        //                result.SensorID = cctv.ID;
        //                result.SensorName = cctv.AccessKey;
        //                result.DeActivate = true;
        //                result.ISConnected = false;
        //                result.Sensor = cctv;
        //                results.Add(result);
        //            }

        //            foreach (CCTV cctv in CCTVManager.Instance.IndoorCCTVList)
        //            {                        
        //                SensorInfo result = new SensorInfo();
        //                result.SensorID = cctv.ID;
        //                result.SensorName = cctv.AccessKey;
        //                result.DeActivate = true;
        //                result.ISConnected = false;
        //                result.Sensor = cctv;
        //                results.Add(result);
        //            }
        //        }
        //        else if (SelectedType == FacilityType.PSM_SENSOR)
        //        {
        //            if (equipZoneList != null && equipZoneList.Count > 0)
        //            {
        //                List<PSMSensor> sensors = PSMManager.Instance.GetSensors();
        //                foreach (PSMSensor psm in sensors)
        //                {
        //                    foreach (EquipmentZone equipZone in equipZoneList)
        //                    {
        //                        if (equipZone.ID == psm.EquipZoneID)
        //                        {
        //                            bool conntected = false;
        //                            bool deActivate = false;
        //                            PSMManager.Instance.GetSensorDeActivate(psm.ID, ref conntected, ref deActivate);

        //                            SensorInfo result = new SensorInfo();
        //                            result.SensorID = psm.ID;
        //                            result.SensorName = psm.Name;
        //                            result.DeActivate = deActivate;
        //                            result.ISConnected = conntected;
        //                            result.Sensor = psm;
        //                            results.Add(result);
        //                            break;
        //                        }
        //                    }
        //                } 
        //            }
        //        }
        //        else if (SelectedType == FacilityType.DOOR)
        //        {
        //            if (equipZoneList != null && equipZoneList.Count > 0 && FormMain.Instance.DataManager.DicEtcSensor.ContainsKey(FacilityType.DOOR))
        //            {
        //                List<EtcSensor> doorSensors = FormMain.Instance.DataManager.DicEtcSensor[FacilityType.DOOR];
        //                if (doorSensors != null && doorSensors.Count > 0)
        //                {
        //                    foreach (EtcSensor sensor in doorSensors)
        //                    {                                
        //                        foreach (EquipmentZone equipZone in equipZoneList)
        //                        {
        //                            if (equipZone.LinkedZone == sensor.POI.Zone)
        //                            {
        //                                SensorInfo result = new SensorInfo();
        //                                result.SensorID = sensor.ID;
        //                                result.SensorName = sensor.SensorName;
        //                                result.DeActivate = sensor.DeActivate;
        //                                result.ISConnected = sensor.Connected;
        //                                result.Sensor = sensor;
        //                                results.Add(result);
        //                                break;
        //                            }
        //                        } 
        //                    }
        //                } 
        //            }
        //        }

        //        FormMain.Instance.SetSensorInfo(results, SelectedType);
        //    }
        //}

        public void SetLayerState()
        {
            if (FormMain.Instance.CurrentTab == UnE.View.Content.ContentOwnerTab.ADMIN_TAB)
            {
                Panel4Unity panel = (Panel4Unity)FormMain.Instance.ContentManager.ContentForm.OutdoorView;

                string strType;

                if (m_dicFacilityTypeName.TryGetValue(SelectedType, out strType))
                {
                    List<string> itemTypeNames = new List<string>();
                    itemTypeNames.Add(strType);
                    panel.ShowIconLayers(itemTypeNames, true);
                }
            }
        }

        public void ShowAllLayer()
        {
            Panel4Unity panel = (Panel4Unity)FormMain.Instance.ContentManager.ContentForm.OutdoorView;
            panel.ShowAllIconLayers();
        }

        public List<IFacility.FacilityType> GetEnableList()
        {
            List<IFacility.FacilityType> enableList = new List<IFacility.FacilityType>();

            foreach (Control ctrl in pnMain.Controls)
            {
                if (ctrl is RibbonButton)
                {
                    RibbonButton btn = (RibbonButton)ctrl;

                    if (btn.Enabled)
                        enableList.Add((IFacility.FacilityType)btn.Tag);
                }
            }

            return enableList;
        }

        public IFacility.FacilityType SetEnableList(List<IFacility.FacilityType> enableList, IFacility.FacilityType selectedType)
        {
            RibbonButton selectedButton = null;

            foreach (Control ctrl in pnMain.Controls)
            {
                if (ctrl is RibbonButton)
                {
                    RibbonButton btn = (RibbonButton)ctrl;

                    if ((IFacility.FacilityType)btn.Tag == selectedType)
                        selectedButton = btn;

                    IFacility.FacilityType type = (IFacility.FacilityType)btn.Tag;
                    btn.Enabled = enableList.Contains(type);
                }
            }

            // 비활성화 상태인 버튼이 선택되어 있으면 선택을 해제하고 활성화된 버튼을 선택시킨다.
            if (selectedButton != null && selectedButton.Enabled == false)
            {
                selectedType = IFacility.FacilityType.NONE;

                foreach (Control ctrl in pnMain.Controls)
                {
                    if (ctrl is RibbonButton)
                    {
                        RibbonButton btn = (RibbonButton)ctrl;

                        if (btn.Enabled)
                        {
                            selectedType = (IFacility.FacilityType)btn.Tag;
                            break;
                        }
                    }
                }
            }

            return selectedType;
        }
    }

    public class SensorInfo
    {
        public int SensorID { get; set; }
        public string SensorName { get; set; }
        public bool ISConnected { get; set; }
        public bool DeActivate { get;set; }
        public object Sensor { get; set; }
    }
}
