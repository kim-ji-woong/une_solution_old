using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;

namespace CCTVChecker
{
    public partial class FormMain : Form
    {
        private static FormMain m_instance = null;
        public static FormMain Instance
        {
            get { return m_instance; }
        }
        private DBUtility.WebDBManager m_dbMgr = new DBUtility.WebDBManager();
        public DBUtility.WebDBManager DBManager
        {
            get { return m_dbMgr; }
        }

        private FormCCTVGuide m_frmCCTVGuide = null;

        public FormCCTVGuide CCTVGuide
        {
            get { return m_frmCCTVGuide; }
            set { m_frmCCTVGuide = value; }
        }

        private Form4CCTV m_frmMultiCCTV = null;
        public Form4CCTV MultiCCTV
        {
            get { return m_frmMultiCCTV; }
            set { m_frmMultiCCTV = value; }
        }

        FormCCTVList m_frmCCTVList = null;
        public FormCCTVList CCTVList
        {
            get { return m_frmCCTVList; }
            set { m_frmCCTVList = value; }
        }

        FormCCTVInfo m_frmCCTVInfo = null;

        public FormMain()
        {
            m_instance = this;
            InitializeComponent();

            m_frmCCTVInfo = new FormCCTVInfo();
            m_frmCCTVInfo.Show();

            InitCCTV();
            InitDB();
        }

        private void btnCCTV_Click(object sender, EventArgs e)
        {
            m_frmCCTVList = new FormCCTVList();
            m_frmCCTVList.Show(this);
        }

        private void InitCCTV()
        {
            m_frmCCTVGuide = new FormCCTVGuide();
            m_frmCCTVGuide.TopLevel = false;
            panel2.Controls.Add(m_frmCCTVGuide);
            m_frmCCTVGuide.Show();

            m_frmMultiCCTV = new Form4CCTV(this);
            m_frmMultiCCTV.TopLevel = false;
            m_frmMultiCCTV.TopMost = false;
            m_frmMultiCCTV.ShowInTaskbar = false;

            panelCCTV.Controls.Add(m_frmMultiCCTV);
            m_frmMultiCCTV.Dock = DockStyle.Fill;
            m_frmMultiCCTV.Show();
        }

        private void InitDB()
        {
            ZoneManager.Instance.LoadBuildingData();
            ZoneManager.Instance.LoadZones();
            ZoneManager.Instance.LoadEquipmentZone();

            CCTVManager.Instance.LoadCCTV(false);
            CCTVManager.Instance.LoadCCTV(true);

            CCTVManager.Instance.LoadEquipZoneCCTV();
            CCTVManager.Instance.LoadEquipZoneTempCCTV();
        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            EquipmentZone equipZone = (EquipmentZone)cmbEquipZone.Items[cmbEquipZone.SelectedIndex];

            //CCTV[] cctvs = CCTVManager.Instance.GetTempCCTVArray(equipZone);
            CCTV[] cctvs = CCTVManager.Instance.GetMixCCTVArray(equipZone);
            if (cctvs == null)
                return;

            m_frmMultiCCTV.ClearSelection(null);
            m_frmMultiCCTV.LoadCCTV(cctvs);
            Debug.WriteLine(equipZone.ToString());
        }

        private void cmbGroup_SelectedIndexChanged(object sender, EventArgs e)
        {
            int nIdx = cmbGroup.SelectedIndex;
            if (nIdx == -1)
                return;

            cmbBuilding.Items.Clear();

            BuildingGroup group = (BuildingGroup)cmbGroup.SelectedItem;

            if (group == ZoneManager.Instance.OutdoorBuildingGroup)
            {
                cmbBuilding.Enabled = false;
                cmbFloor.Enabled = false;

                cmbFloor.Items.Clear();
                cmbEquipZone.Items.Clear();

                foreach (KeyValuePair<int, Zone> pair in ZoneManager.Instance.DicOutdoorZones)
                {
                    ArrayList arrEquipZones = ZoneManager.Instance.GetEquipmentZoneList(pair.Value);
                    if (arrEquipZones == null)
                        return;

                    foreach (EquipmentZone equipZone in arrEquipZones)
                    {
                        cmbEquipZone.Items.Add(equipZone);
                    }
                }

                if (cmbEquipZone.Items.Count > 0)
                    cmbEquipZone.SelectedIndex = 0;
            }
            else
            {
                cmbBuilding.Enabled = true;
                cmbBuilding.Enabled = true;
                cmbFloor.Enabled = true;

                foreach (Building buidling in group.BuildingList)
                {
                    cmbBuilding.Items.Add(buidling);
                }

                if (cmbBuilding.Items.Count > 0)
                    cmbBuilding.SelectedIndex = 0;
            }
        }

        private void cmbBuilding_SelectedIndexChanged(object sender, EventArgs e)
        {
            int nIdx = cmbBuilding.SelectedIndex;
            if (nIdx == -1)
                return;

            Building building = (Building)cmbBuilding.SelectedItem;
            if (building == null)
                return;

            cmbFloor.Items.Clear();
            ArrayList arrFloor = new ArrayList();

            foreach (KeyValuePair<int, Zone> pair in ZoneManager.Instance.DicZones)
            {
                if (pair.Value.Building == building)
                    arrFloor.Add(new Floor(pair.Value.FloorIndex + pair.Value.AddFloor));
            }

            arrFloor.Sort();

            foreach (Floor floor in arrFloor)
            {
               cmbFloor.Items.Add(floor);
            }


            if (cmbFloor.Items.Count > 0)
                cmbFloor.SelectedIndex = 0;
        }

        Zone FindZone(Building building, float fFloorIndex)
        {
            int nFloorIndex = fFloorIndex > 0.0f ? (int)(fFloorIndex + 0.01f) : (int)(fFloorIndex - 0.01f);
            string strAddFloor = string.Format("{0:f1}", fFloorIndex - nFloorIndex);

            foreach (KeyValuePair<int, Zone> pair in ZoneManager.Instance.DicZones)
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

        private void cmbFloor_SelectedIndexChanged(object sender, EventArgs e)
        {
            int nSelectedIndex = cmbFloor.SelectedIndex;
            if (nSelectedIndex < 0)
                return;

            cmbEquipZone.Items.Clear();

            Object obj = cmbFloor.Items[nSelectedIndex];
            Type type = obj.GetType();

            Zone zone = null;

            if (type == typeof(Floor))
            {
                Building building = (Building)cmbBuilding.Items[cmbBuilding.SelectedIndex];
                Floor floor = (Floor)obj;
                zone = FindZone(building, floor.FloorIndex);
            }

            if (zone == null || zone.ID <= 0)
                return;

            ArrayList arrEquipZones = ZoneManager.Instance.GetEquipmentZoneList(zone);
            if (arrEquipZones == null)
                return;

            foreach (EquipmentZone equipZone in arrEquipZones)
            {
                cmbEquipZone.Items.Add(equipZone);
            }

            if (cmbEquipZone.Items.Count > 0)
                cmbEquipZone.SelectedIndex = 0;
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            foreach (KeyValuePair<int, BuildingGroup> pair in ZoneManager.Instance.DicBuildingGroup)
            {
                cmbGroup.Items.Add(pair.Value);
            }

            if (ZoneManager.Instance.DicOutdoorZones.Count > 0)
                cmbGroup.Items.Add(ZoneManager.Instance.OutdoorBuildingGroup);

            if (cmbGroup.Items.Count > 0)
                cmbGroup.SelectedIndex = 0;

        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            CCTV[] arrCCTVs = m_frmMultiCCTV.GetSelectedCCTVs();

            if (arrCCTVs == null)
                return;

            if (cmbEquipZone.SelectedIndex < 0)
                return;

            EquipmentZone equipZone = (EquipmentZone)cmbEquipZone.Items[cmbEquipZone.SelectedIndex];
            
            if (equipZone == null)
                return;

            CCTVManager.Instance.UpdateDBEquipZoneCCTV(arrCCTVs, equipZone);
        }

        private void btnFill_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("모든 설비영역에 대하여 4개의 CCTV 가운데 NULL인 것은 Temp CCTV List에서 채워 넣으시겠습니까?", "확인", MessageBoxButtons.YesNo)
                == System.Windows.Forms.DialogResult.Yes)
            {
                CCTVManager.Instance.FillCCTVs();
            }
        }

    }
}
