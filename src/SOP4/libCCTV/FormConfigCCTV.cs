using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UnE.CCTV
{
    public partial class FormConfigCCTV : Form
    {
        //private FormCCTVList m_frmCCTVList = null;
        private Point m_ptLabelEquipZoneOrigin = new Point();
        private Point m_ptComboEquipZoneOrigin = new Point();

        public FormConfigCCTV()
        {
            InitializeComponent();

            ComboHelper.InitBuildingGroupComboBox(cmbGroup);
            //m_frmCCTVList = ProxyCCTV.Instance.CCTVList;

            m_ptLabelEquipZoneOrigin = labelEquipZone.Location;
            m_ptComboEquipZoneOrigin = cmbEquipZone.Location;

            if (UnE.SOP.ProxySOP.Instance.UsePSM == false)
            {
                // 화재는 보여도 되지만, radio 버튼이 하나만 있는건 의미가 없음
                radioFire.Visible = radioPSM.Visible = false;
            }
        }

        private void chkConfigRegionCCTV_CheckedChanged(object sender, EventArgs e)
        {
            bool bChecked = chkConfigRegionCCTV.Checked;
            labelBuildingGroup.Visible = labelBuilding.Visible = labelFloor.Visible = labelEquipZone.Visible = bChecked;

            radioFire.Enabled = radioPSM.Enabled = bChecked;
            radioSensor_CheckedChanged(null, null);

            if( bChecked == true)
            {
                if (this.cmbEquipZone.SelectedIndex < 0)
                    return;

                EquipmentZone zone = (EquipmentZone)cmbEquipZone.Items[cmbEquipZone.SelectedIndex];
                FormMain.Instance.ShowEquipZoneCCTVs(zone);

                Form4CCTV form = FormMain.Instance.CCTVForm;
                if (form != null && form.IsDisposed == false)
                {
                    form.Tag = zone;
                }
            }
            else
            {

                SetDefaultCCTV();
            }
        }

        private void rbNormal_CheckedChanged(object sender, EventArgs e)
        {
            if (rbNormal.Checked == true)
            {
                Form4CCTV form = FormMain.Instance.CCTVForm;
                if (form != null && form.IsDisposed == false)
                {
                    form.SetDefaultCCTV();
                }
            }           
        }

        private void rbSituation_CheckedChanged(object sender, EventArgs e)
        {
            if(rbSituation.Checked == true)
            {
                Form4CCTV form = FormMain.Instance.CCTVForm;
                if (form != null && form.IsDisposed == false)
                {
                    if (form.LastCCTVList != null)
                        form.SetCCTV(form.LastCCTVList, form.ZoneTarget);
                    else
                        form.SetDefaultCCTV();
                }
            }            
        }


        //private bool m_bLoadingList = false;
        private void btnShowCCTVList_Click(object sender, EventArgs e)
        {
            CCTVFormFrame.Instance.PopAllCCTV();

            //if (m_frmCCTVList != null && m_frmCCTVList.IsHandleCreated == true && m_frmCCTVList.Visible == true)
            //    return;

            //if (m_bLoadingList == false)
            //{
            //    m_bLoadingList = true;

            //    m_frmCCTVList = new FormCCTVList();
            //    m_frmCCTVList.Text = "CCTV 리스트";
            //    m_frmCCTVList.Show();
            //}

            //m_bLoadingList = false;	
        }

        private void SetDefaultCCTV()
        {
            FormMain.Instance.ShowLastCCTV();
        }
      
        private void FormConfigCCTV_Load(object sender, EventArgs e)
        {
            //CCTV 모드 설정
        }

        private void FormConfigCCTV_FormClosing(object sender, FormClosingEventArgs e)
        {
            if( chkConfigRegionCCTV.Checked == true)
                SetDefaultCCTV();            
        }

        private void cmbEquipZone_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.cmbEquipZone.SelectedIndex < 0)
                return;
            EquipmentZone zone = (EquipmentZone)cmbEquipZone.Items[cmbEquipZone.SelectedIndex];
            
            
            FormMain.Instance.ShowEquipZoneCCTVs(zone);
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
                zone = ZoneManager.Instance.GetZone(building.BuildingID, floor.FloorIndex);
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
            {
                //if (FormMain.Instance.ComboFireDetect.SelectedItem != null)
                //{
                //    FireDetectProcess process = (FireDetectProcess)FormMain.Instance.ComboFireDetect.SelectedItem;

                //    if (cmbEquipZone.Items.Contains(process.TargetZone))
                //    {
                //        cmbEquipZone.SelectedItem = process.TargetZone;
                //    }
                //    else
                //    {
                //        cmbEquipZone.SelectedIndex = 0;
                //    }
                //}
                //else
                {
                    cmbEquipZone.SelectedIndex = 0;
                }
            }
        }

        private void cmbGroup_SelectedIndexChanged(object sender, EventArgs e)
        {
            int nSelectedIndex = cmbGroup.SelectedIndex;
            if (nSelectedIndex < 0)
                return;
            BuildingGroup buildingGroup = (BuildingGroup)cmbGroup.Items[nSelectedIndex];
            ComboHelper.InitBuildingComboBox(cmbBuilding, buildingGroup);

            cmbBuilding.Sorted = true;
            cmbBuilding.Sorted = false;

            if (cmbBuilding.Items.Count > 0)
                cmbBuilding.SelectedIndex = 0;

            if (buildingGroup != ZoneManager.Instance.OutdoorBuildingGroup)
                cmbFloor.Enabled = true;
        }

        private void cmbBuilding_SelectedIndexChanged(object sender, EventArgs e)
        {
            int nSelectedIndex = cmbBuilding.SelectedIndex;
            if (nSelectedIndex < 0)
                return;

            Object obj = cmbBuilding.Items[nSelectedIndex];
            if (obj.GetType() == typeof(Building))
            {
                ComboHelper.InitFloorComboBox(cmbFloor, (Building)obj);
                //btnSelectZone.Enabled = true;
            }
            else
            {
                cmbFloor.Items.Clear();
                //btnSelectZone.Enabled = false;

                if (chkConfigRegionCCTV.Checked)
                {
                    cmbFloor.Enabled = false;

                    cmbEquipZone.Items.Clear();

                    if (obj.GetType() != typeof(Zone))
                        return;

                    Zone zone = (Zone)obj;
                    ArrayList arrEquipZones = ZoneManager.Instance.GetEquipmentZoneList(zone);

                    foreach (EquipmentZone equipZone in arrEquipZones)
                    {
                        cmbEquipZone.Items.Add(equipZone);
                    }

                    if (cmbEquipZone.Items.Count > 0)
                        cmbEquipZone.SelectedIndex = 0;
                }
                else
                {
                    cmbFloor.Enabled = true;
                    cmbFloor.Items.Add("-");
                }
            }

            if (cmbFloor.Items.Count > 0)
                cmbFloor.SelectedIndex = 0;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();

        }

        private void radioSensor_CheckedChanged(object sender, EventArgs e)
        {
            if (chkConfigRegionCCTV.Checked == false)
            {
                labelBuildingGroup.Visible = labelBuilding.Visible = labelFloor.Visible = labelEquipZone.Visible = false;
                cmbGroup.Visible = cmbBuilding.Visible = cmbFloor.Visible = cmbEquipZone.Visible = false;
                return;
            }

            if (radioFire.Checked)
            {
                labelEquipZone.Location = m_ptLabelEquipZoneOrigin;
                cmbEquipZone.Location = m_ptComboEquipZoneOrigin;

                if (cmbFloor.SelectedIndex >= 0)
                    cmbFloor_SelectedIndexChanged(null, null);
                else
                    cmbEquipZone.Items.Clear();

                cmbGroup.Visible = cmbBuilding.Visible = cmbFloor.Visible = cmbEquipZone.Visible = true;
                labelBuildingGroup.Visible = labelBuilding.Visible = labelFloor.Visible = labelEquipZone.Visible = true;
            }
            else
            {
                labelBuildingGroup.Visible = labelBuilding.Visible = labelFloor.Visible = false;

                labelEquipZone.Location = labelBuildingGroup.Location;
                cmbEquipZone.Location = cmbGroup.Location;
                cmbGroup.Visible = cmbBuilding.Visible = cmbFloor.Visible = false;

                labelEquipZone.Visible = cmbEquipZone.Visible = true;
                SetPSMLocations();
            }
        }

        private void SetPSMLocations()
        {
            cmbEquipZone.Items.Clear();

            foreach (KeyValuePair<int, EquipmentZone> pair in ZoneManager.Instance.DicEquipZones)
            {
                if (pair.Value.ZoneType == EquipmentZone.EquipZoneType.PSM_TYPE)
                    cmbEquipZone.Items.Add(pair.Value);
            }
        }
    }
}
