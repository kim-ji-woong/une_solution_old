using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SDMS_Building.Network;
using UnE.GUI;
using UnE.Sensor;
using UnE.Spatial;

namespace SDMS_Building.PopupDialog.Controls
{
    public partial class FormManualReport : Form
    {
        private UEWpfControl.WpfComboBox m_cbBuilding = null;
        private UEWpfControl.WpfComboBox m_cbFloor = null;

        private IFacility.FacilityType m_curFacilityType = IFacility.FacilityType.FIRE_SENSOR;
        private List<RibbonButton> m_btns = new List<RibbonButton>();
        private int m_nAlarmLevel = 2;
        public FormManualReport()
        {
            InitializeComponent();

            this.DoubleBuffered = true;

            //lblLevel1.BackColor = lblLevel2.BackColor = lblLevel3.BackColor = lblLevel4.BackColor = Color.LightGray;

            rbtnFire.Tag = IFacility.FacilityType.FIRE_SENSOR;
            rbtnEarthquake.Tag = IFacility.FacilityType.Earthquake;
            rbtnPSM.Tag = IFacility.FacilityType.PSM_SENSOR;
            rbtnBlackout.Tag = IFacility.FacilityType.BLACKOUT;            
            rbtnStrongwind.Tag = IFacility.FacilityType.STRONG_WIND;
            rbtnTerror.Tag = IFacility.FacilityType.TERROR;
            rbtnSubmergency.Tag = IFacility.FacilityType.SUBMERGENCY;
            rbtnCorona.Tag = IFacility.FacilityType.CORONA;

            m_btns.Add(rbtnFire);
            m_btns.Add(rbtnEarthquake);
            m_btns.Add(rbtnPSM);
            m_btns.Add(rbtnBlackout);
            m_btns.Add(rbtnStrongwind);
            m_btns.Add(rbtnTerror);
            m_btns.Add(rbtnSubmergency);
            m_btns.Add(rbtnCorona);

            m_cbBuilding = new UEWpfControl.WpfComboBox();
            eleBuilding.Child = m_cbBuilding;
            m_cbBuilding.customComboBox.SelectionChanged += EleBuildingComboBox_SelectionChanged;
            m_cbBuilding.SetSize(eleBuilding.Width, eleBuilding.Height);

            m_cbFloor = new UEWpfControl.WpfComboBox();
            eleFloor.Child = m_cbFloor;
            m_cbFloor.SetSize(eleFloor.Width, eleFloor.Height);
        }

        private void FormManualReport_Load(object sender, EventArgs e)
        {
            InitBuildingComboBox();
        }

        private void FormManualReport_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            g.DrawImage(SDMS_Building.Properties.Resources.manualReportDisaster, 20, 100, 960, 100);
            g.DrawImage(SDMS_Building.Properties.Resources.manualReportStatus, 20, 220, 960, 255);
            //g.DrawImage(SDMS_Building.Properties.Resources.manualReportReporter, 20, 495, 960, 185);
        }

        private void InitBuildingComboBox()
        {
            //m_cbBuilding.customComboBox.DisplayMemberPath = "BuildingName";
            m_cbBuilding.customComboBox.Items.Clear();

            foreach (KeyValuePair<int, Building> item in UnE.Spatial.ZoneManager.Instance.DicBuildings)
            {
                m_cbBuilding.customComboBox.Items.Add(item.Value);
            }

            if (m_cbBuilding.customComboBox.Items.Count > 0)
                m_cbBuilding.customComboBox.SelectedIndex = 0;
        }

        private void EleBuildingComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            m_cbFloor.customComboBox.Items.Clear();
            
            if (m_cbBuilding.customComboBox.SelectedItem == null || m_cbBuilding.customComboBox.SelectedIndex < 0)
                return;

            object obj = m_cbBuilding.customComboBox.Items[m_cbBuilding.customComboBox.SelectedIndex];
            Type type = obj.GetType();

            // 층 콤보박스 채우기
            if (type == typeof(Building))
            {
                Building building = (Building)obj;
                ArrayList arrFloor = (ArrayList)building.FloorList.Clone();

                foreach (Zone floor in arrFloor)
                {
                    m_cbFloor.customComboBox.Items.Add(floor.Floor);
                }
            }

            if (m_cbFloor.customComboBox.Items.Count > 0)
                m_cbFloor.customComboBox.SelectedIndex = 0;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.No;
        }

        private void rbtnFire_Click(object sender, EventArgs e)
        {
            rbtnFire.IsChecked = !rbtnFire.IsChecked;
            rbtnFire.Refresh();
        }

        private void rbtn_Click(object sender, EventArgs e)
        {
            RibbonButton rbtn = sender as RibbonButton;
            if (rbtn == null)
                return;

            IFacility.FacilityType beforeType = m_curFacilityType;
            m_curFacilityType = (IFacility.FacilityType)rbtn.Tag;

            foreach (RibbonButton btn in m_btns)
            {
                if ((IFacility.FacilityType)btn.Tag == beforeType)
                {
                    btn.IsChecked = false;
                    btn.Refresh();
                }

                if ((IFacility.FacilityType)btn.Tag == m_curFacilityType)
                {
                    btn.IsChecked = true;
                    btn.Refresh();
                }
            }

            // 단계 선택이 필요한가 ?
            //if (rbtn == rbtnFire)
            //{
            //    // 필요없음
            //    pnLevel.Visible = false;
            //    //pnLevel.Enabled = false;
            //    //pnLevel.BackColor = Color.LightGray;
            //    //lblLevel1.BackColor = lblLevel2.BackColor = lblLevel3.BackColor = lblLevel4.BackColor = Color.LightGray;
            //}
            //else //if (rbtn == rbtnTerror || rbtn == rbtnStrongwind)
            //{
            //    // 필요함
            //    pnLevel.Visible = true;

            //    //pnLevel.Enabled = true;
            //    //pnLevel.BackColor = Color.White;

            //    lblLevel_Click(lblLevel2, null);
            //}
            
            // 건물 선택이 필요한가 ?
            if (rbtn == rbtnStrongwind)
            {
                // 필요없음
                eleBuilding.Visible = false;
                eleFloor.Visible = false;
                lblFloor.Visible = false;
                lblBuilding.Text = "모든 건물";
            }
            else
            {
                // 필요함
                eleBuilding.Visible = true;
                eleFloor.Visible = true;
                lblFloor.Visible = true;
                lblBuilding.Text = "건물";
            }
        }

        private void lblLevel_Click(object sender, EventArgs e)
        {
            Label label = sender as Label;
            if (label == null)
                return;

            m_nAlarmLevel = Convert.ToInt32(label.Tag);

            if (label == lblLevel1)
            {
                lblLevel1.BackColor = Color.FromArgb(0, 140, 255);
                lblLevel1.ForeColor = Color.White;
                lblLevel2.BackColor = lblLevel3.BackColor = lblLevel4.BackColor = Color.White;
                lblLevel2.ForeColor = lblLevel3.ForeColor = lblLevel4.ForeColor = Color.FromArgb(155, 155, 155);
            }
            else if (label == lblLevel2)
            {
                lblLevel2.BackColor = Color.FromArgb(0, 140, 255);
                lblLevel2.ForeColor = Color.White;
                lblLevel1.BackColor = lblLevel3.BackColor = lblLevel4.BackColor = Color.White;
                lblLevel1.ForeColor = lblLevel3.ForeColor = lblLevel4.ForeColor = Color.FromArgb(155, 155, 155);
            }
            else if (label == lblLevel3)
            {
                lblLevel3.BackColor = Color.FromArgb(0, 140, 255);
                lblLevel3.ForeColor = Color.White;
                lblLevel1.BackColor = lblLevel2.BackColor = lblLevel4.BackColor = Color.White;
                lblLevel1.ForeColor = lblLevel2.ForeColor = lblLevel4.ForeColor = Color.FromArgb(155, 155, 155);
            }
            else if (label == lblLevel4)
            {
                lblLevel4.BackColor = Color.FromArgb(0, 140, 255);
                lblLevel4.ForeColor = Color.White;
                lblLevel1.BackColor = lblLevel2.BackColor = lblLevel3.BackColor = Color.White;
                lblLevel1.ForeColor = lblLevel2.ForeColor = lblLevel3.ForeColor = Color.FromArgb(155, 155, 155);
            }
        }

        private void btnConfirm_Click(object sender, EventArgs e)
        {
            int nSensorZoneHistoryID = 0;
            // 수동화재신고일 경우 EquipZone ID가 아니라 Zone ID 이다.
            int nZoneID = 0;
            int nSensorZoneID = SOPWebServer.Header.ManualReportDefaultID + (int)m_curFacilityType;
            int nSOPGenUserID = FormMain.Instance.nSOPGentUserID;

            // 강풍은 Zone을 선택하지 않음
            if (m_curFacilityType == IFacility.FacilityType.STRONG_WIND)
            {
                nZoneID = -1;
            }   
            else
            {
                Floor floor = m_cbFloor.customComboBox.Items[m_cbFloor.customComboBox.SelectedIndex] as Floor;
                if (floor != null && floor.Zone != null)
                    nZoneID = floor.Zone.ID;
            }

            string strMemo = txtMemo.Text;

            ArrayList arrDatas = new ArrayList();
            arrDatas.Add(nSensorZoneHistoryID);
            arrDatas.Add(nZoneID);
            arrDatas.Add(nSensorZoneID);
            arrDatas.Add(nSOPGenUserID);
            arrDatas.Add(strMemo);
            //if (m_curFacilityType != IFacility.FacilityType.FIRE_SENSOR)
                arrDatas.Add(m_nAlarmLevel);

            byte[] bytes = SOPWebServer.BinaryHelper.MakeBytes(arrDatas);

            NetworkWebManager.Instance.SendMessage(SOPWebServer.Header.NOTIFY_DISASTER, bytes);

            this.Close();
        }
    }
}
