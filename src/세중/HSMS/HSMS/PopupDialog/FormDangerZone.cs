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

namespace HSMS
{
    public partial class FormDangerZone : Form
    {
        private DataZone m_beforeZone = null;
        private DBConn conn = null;

        private Dictionary<DataZone, ZoneGroup> m_dicChangedZone = new Dictionary<DataZone, ZoneGroup>();

        public FormDangerZone()
        {
            InitializeComponent();

            m_beforeZone = new DataZone();
            conn = new DBConn("HSMS");
        }

        private void InitComboBox()
        {
            int nZoneGroupCount = FormMain.Instance.DataMgr.GetZoneGroupCount();

            for (int i = 0; i < nZoneGroupCount; i++)
            {
                ZoneGroup group = FormMain.Instance.DataMgr.GetZoneGroup(i);
                int nZoneCount = group.GetZoneCount();

                for (int j = 0; j < nZoneCount;j++ )
                //foreach (DataZone zone in FormMain.Instance.DataMgr.DataZones)
                {
                    DataZone zone = group.GetZone(j);

                    if (zone.ZoneName == "PLAN")
                        continue;

                    cmbZoneList.Items.Add(zone.Clone());
                }

                cmbGroupList.Items.Add(group);
            }

            if (cmbZoneList != null && cmbZoneList.Items.Count > 0)
                cmbZoneList.SelectedIndex = 0;
        }

        private void ResetCheckBox()
        {
            //m_bCheckedAll = true;

            if (chkLevelAll.Checked == true)
                chkLevelAll.Checked = false;
            else
            {
                m_bCheckedAll = true;
                chkLevel1.Checked = false;
                chkLevel2.Checked = false;
                chkLevel3.Checked = false;
                chkLevel4.Checked = false;
                chkLevel5.Checked = false;
                m_bCheckedAll = false;
            }
        }

        private void SetCheckBox()
        {
            DataZone zone = (DataZone)cmbZoneList.SelectedItem;
            if (zone == null)
                return;

            int nCount = zone.GetPermitLevelCount();

            if (nCount == 5)
            {
                m_bCheckedAll = true;
                chkLevelAll.Checked = true;
                m_bCheckedAll = false;
            }
            else
            {
                for (int i = 0; i < nCount; i++)
                {
                    int nPermitLevel = zone.GetPermitLevel(i);

                    if (nPermitLevel == 1)
                    {
                        m_bCheckedOne = true;
                        chkLevel1.Checked = true;
                        m_bCheckedOne = false;
                    }
                    if (nPermitLevel == 2)
                    {
                        m_bCheckedOne = true;
                        chkLevel2.Checked = true;
                        m_bCheckedOne = false;
                    }
                    if (nPermitLevel == 3)
                    {
                        m_bCheckedOne = true;
                        chkLevel3.Checked = true;
                        m_bCheckedOne = false;
                    }
                    if (nPermitLevel == 4)
                    {
                        m_bCheckedOne = true;
                        chkLevel4.Checked = true;
                        m_bCheckedOne = false;
                    }
                    if (nPermitLevel == 5)
                    {
                        m_bCheckedOne = true;
                        chkLevel5.Checked = true;
                        m_bCheckedOne = false;
                    }
                }
            }
        }

        private int m_nPreSelectIndex = -1;
        private void cmbZoneList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (m_nPreSelectIndex != -1)
            {
                DataZone zone = (DataZone)cmbZoneList.Items[m_nPreSelectIndex];
                zone.RemoveAllPermitLevels();

                if (chkLevelAll.Checked)
                {
                    zone.AddPermitLevel(1);
                    zone.AddPermitLevel(2);
                    zone.AddPermitLevel(3);
                    zone.AddPermitLevel(4);
                    zone.AddPermitLevel(5);
                }
                else
                {
                    if (chkLevel1.Checked)
                        zone.AddPermitLevel(1);
                    if (chkLevel2.Checked)
                        zone.AddPermitLevel(2);
                    if (chkLevel3.Checked)
                        zone.AddPermitLevel(3);
                    if (chkLevel4.Checked)
                        zone.AddPermitLevel(4);
                    if (chkLevel5.Checked)
                        zone.AddPermitLevel(5);
                }

                cmbGroupList.SelectedItem = zone.ZoneGroup;
            }
           
            m_nPreSelectIndex = cmbZoneList.SelectedIndex;

            if (cmbZoneList.SelectedIndex >= 0)
            {
                DataZone zone = (DataZone)cmbZoneList.Items[m_nPreSelectIndex];
                cmbGroupList.SelectedItem = zone.ZoneGroup;
            }
            else
                cmbGroupList.SelectedIndex = -1;

            ResetCheckBox();
            SetCheckBox();
        }

        private void cmbGroupList_SelectedIndexChanged(object sender, EventArgs e)
        {
            DataZone zone = (DataZone)cmbZoneList.SelectedItem;

            if (zone == null)
                return;

            ZoneGroup group = (ZoneGroup)cmbGroupList.SelectedItem;

            if (zone.ZoneGroup == group)
            {
                if (m_dicChangedZone.ContainsKey(zone))
                    m_dicChangedZone.Remove(zone);
            }
            else
                m_dicChangedZone[zone] = group;
        }

        private string GetPremitLevelString(DataZone zone)
        {
            int nCount = zone.GetPermitLevelCount();
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < nCount; i++)
            {
                if (i != 0)
                {
                    sb.Append(",");
                }
                sb.Append(zone.GetPermitLevel(i));

            }
            return sb.ToString();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (m_nPreSelectIndex != -1)
            {
                DataZone zone = (DataZone)cmbZoneList.Items[m_nPreSelectIndex];
                zone.RemoveAllPermitLevels();

                if (chkLevelAll.Checked)
                {
                    zone.AddPermitLevel(1);
                    zone.AddPermitLevel(2);
                    zone.AddPermitLevel(3);
                    zone.AddPermitLevel(4);
                    zone.AddPermitLevel(5);
                }
                else
                {
                    if (chkLevel1.Checked)
                        zone.AddPermitLevel(1);
                    if (chkLevel2.Checked)
                        zone.AddPermitLevel(2);
                    if (chkLevel3.Checked)
                        zone.AddPermitLevel(3);
                    if (chkLevel4.Checked)
                        zone.AddPermitLevel(4);
                    if (chkLevel5.Checked)
                        zone.AddPermitLevel(5);
                }
            }

            foreach (DataZone zone in cmbZoneList.Items)
            {
                DataZone originZone = FormMain.Instance.DataMgr.FindZone(zone.ZoneName);

                if (originZone != null)
                //foreach (DataZone originZone in FormMain.Instance.DataMgr.DataZones)
                {                    
                    //if (originZone.ZoneName == zone.ZoneName)
                    {
                        bool bdifferent = false;
                        if (originZone.GetPermitLevelCount() == zone.GetPermitLevelCount())
                        {
                            for (int i = 0; i < originZone.GetPermitLevelCount(); i++)
                            {
                                if (originZone.GetPermitLevel(i) != zone.GetPermitLevel(i))
                                {
                                    bdifferent = true;
                                    break;
                                }
                            }
                        }
                        else
                        {
                            bdifferent = true;
                        }

                        if (bdifferent == true)
                        {
                            EditPermitLevel editPermitLevel = new EditPermitLevel();

                            editPermitLevel.SQLType = ChangedData.UPDATE;
                            editPermitLevel.Zone = zone;
                            editPermitLevel.PermitLevel = GetPremitLevelString(zone);

                            editPermitLevel.Update(conn);
                        }
                    }
                }
            }

            SendChangedZoneGroup();

            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }

        private void SendChangedZoneGroup()
        {
            if (m_dicChangedZone.Count == 0)
                return;

            ArrayList arrDatas = new ArrayList();

            arrDatas.Add((int)ChangeDataType.CHANGE_ZONE_GROUP);
            arrDatas.Add(ChangedData.UPDATE);

            foreach (KeyValuePair<DataZone, ZoneGroup> pair in m_dicChangedZone)
            {
                arrDatas.Add(pair.Key.ID);
                arrDatas.Add(pair.Value.ToString());
            }

            byte[] bytes = ClientProvider.MakeBytes(TCP_ID.CHANGE_DB_DATA, arrDatas);
            FormMain.Instance.NetMgr.ClientProvider.Send(bytes, 0, bytes.Length);
        }

        private void chkLevel_CheckedChanged(object sender, EventArgs e)
        {
            chkLevelAll.Checked = false;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
        }

        private void CheckChange(object sender, EventArgs e)
        {
            if (m_bCheckedAll == true)
                return;
            CheckBox chkBox = (CheckBox)sender;
            if (chkBox.Checked == false)
            {
                m_bCheckedOne = true;
                chkLevelAll.Checked = false;
                m_bCheckedOne = false;
            }
        }
        private void chkLevel1_CheckedChanged(object sender, EventArgs e)
        {
            CheckChange(sender, e);
        }

        private void chkLevel2_CheckedChanged(object sender, EventArgs e)
        {
            CheckChange(sender, e);
        }

        private void chkLevel3_CheckedChanged(object sender, EventArgs e)
        {
            CheckChange(sender, e);
        }

        private void chkLevel4_CheckedChanged(object sender, EventArgs e)
        {
            CheckChange(sender, e);
        }

        private void chkLevel5_CheckedChanged(object sender, EventArgs e)
        {
            CheckChange(sender, e);
        }

        private bool m_bCheckedOne = false;
        private bool m_bCheckedAll = false;
        private void chkLevelAll_CheckedChanged_1(object sender, EventArgs e)
        {
            if (m_bCheckedOne == true)
                return;

            CheckBox chkBox = (CheckBox)sender;
            if (chkBox.Checked == true)
            {
                m_bCheckedAll = true;
                chkLevel1.Checked = true;
                chkLevel2.Checked = true;
                chkLevel3.Checked = true;
                chkLevel4.Checked = true;
                chkLevel5.Checked = true;
                m_bCheckedAll = false;
            }
            else
            {
                m_bCheckedAll = true;
                chkLevel1.Checked = false;
                chkLevel2.Checked = false;
                chkLevel3.Checked = false;
                chkLevel4.Checked = false;
                chkLevel5.Checked = false;
                m_bCheckedAll = false;
            }
        }

        private void FormDangerZone_Load(object sender, EventArgs e)
        {
            InitComboBox();
        }

        private void btnAddGroup_Click(object sender, EventArgs e)
        {
            DataZone zone = (DataZone)cmbZoneList.SelectedItem;

            if (zone == null)
                return;

            FormAddGroup frm = new FormAddGroup();

            frm.SetGridHeader("영역그룹 이름");
            frm.SetTitle("영역그룹 추가");
            frm.DefGroupName = ZoneGroup.DefaultZoneGroup.GroupName;
            frm.DefGroupNickName = ZoneGroup.DefaultZoneGroup.ToString();

            foreach (ZoneGroup group in cmbGroupList.Items)
            {
                frm.AddGroupName(group.ToString());
            }

            if (frm.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                ZoneGroup group = new ZoneGroup(frm.NewGroupName);
                cmbGroupList.Items.Add(group);
                cmbGroupList.SelectedItem = group;

                m_dicChangedZone[zone] = group;
            }
        }
    }
}
