using DBUtility2;
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
using UnE.Sensor;
using UnE.Spatial;

namespace SDMS_Building.PopupDialog.Config
{
    public partial class FormManager : Form
    {
        private UEWpfControl.WpfComboBox m_cbType = null;
        private UEWpfControl.WpfComboBox m_cbBuilding = null;
        private UEWpfControl.WpfComboBox m_cbFloor = null;

        private int FIRE_SENSOR = -1;
        private int PSM_SENSOR = -1;
        private int STRONG_WIND = -1;
        private int EARTHQUAKE_SENSOR = -1;
        private int FIREWALL_SENSOR = -1;
        private int DOOR_SENSOR = -1;
        private int BLACKOUT_SENSOR = -1;

        public FormManager()
        {
            InitializeComponent();
            this.DoubleBuffered = true;

            m_cbType = new UEWpfControl.WpfComboBox();
            eleType.Child = m_cbType;
            m_cbType.customComboBox.SelectionChanged += EleTypeComboBox_SelectionChanged;
            m_cbType.SetSize(eleType.Width, eleType.Height);

            m_cbBuilding = new UEWpfControl.WpfComboBox();
            eleBuilding.Child = m_cbBuilding;
            m_cbBuilding.customComboBox.SelectionChanged += EleBuildingComboBox_SelectionChanged;
            m_cbBuilding.SetSize(eleBuilding.Width, eleBuilding.Height);

            m_cbFloor = new UEWpfControl.WpfComboBox();
            eleFloor.Child = m_cbFloor;
            m_cbFloor.customComboBox.SelectionChanged += EleFloorComboBox_SelectionChanged;
            m_cbFloor.SetSize(eleFloor.Width, eleFloor.Height);
        }

        

        private IFacility.FacilityType mCurrentType = IFacility.FacilityType.NONE;
        private void EleTypeComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            mCurrentType = IFacility.FacilityType.NONE;

            if (m_cbType.customComboBox.SelectedIndex == FIRE_SENSOR)
            {
                mCurrentType = IFacility.FacilityType.FIRE_SENSOR;
            }
            else if (m_cbType.customComboBox.SelectedIndex == PSM_SENSOR)
            {
                mCurrentType = IFacility.FacilityType.PSM_SENSOR;
            }
            else if (m_cbType.customComboBox.SelectedIndex == STRONG_WIND)
            {
                mCurrentType = IFacility.FacilityType.STRONG_WIND;
            }
            else if (m_cbType.customComboBox.SelectedIndex == EARTHQUAKE_SENSOR)
            {
                mCurrentType = IFacility.FacilityType.Earthquake;
            }
            else if (m_cbType.customComboBox.SelectedIndex == FIREWALL_SENSOR)
            {
                mCurrentType = IFacility.FacilityType.FIREWALL;
            }
            else if (m_cbType.customComboBox.SelectedIndex == DOOR_SENSOR)
            {
                mCurrentType = IFacility.FacilityType.DOOR;
            }
            else if (m_cbType.customComboBox.SelectedIndex == BLACKOUT_SENSOR)
            {
                mCurrentType = IFacility.FacilityType.BLACKOUT;
            }

            // 지진은 건물 정보가 필요없음
            if (mCurrentType == IFacility.FacilityType.Earthquake)
            {
                eleBuilding.Visible = false;
                eleFloor.Visible = false;
                label3.Visible = false;
            }
            else
            {
                eleBuilding.Visible = true;
                eleFloor.Visible = true;
                label3.Visible = true;
            }

            if (m_cbFloor.customComboBox.SelectedItem != null && m_cbFloor.customComboBox.SelectedIndex != 0)
                LoadFloorManager();
            else if (m_cbBuilding.customComboBox.SelectedItem != null && m_cbBuilding.customComboBox.SelectedIndex != 0)
                LoadBuildingManager();
            else
                LoadEntireManager();
        }

        private void LoadEntireManager()
        {
            dataGridView1.Rows.Clear();

            FacilityManagerGroup group = FormMain.Instance.DataManager.GetEntireFacilityManagerGroup(mCurrentType, true);
            AddGridData(group);
        }

        private void EleBuildingComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            m_cbFloor.customComboBox.Items.Clear();
            m_cbFloor.customComboBox.Items.Add("모두");
            m_cbFloor.customComboBox.SelectedIndex = 0;

            if (m_cbBuilding.customComboBox.SelectedItem == null || m_cbBuilding.customComboBox.SelectedIndex == 0)
            {
                LoadEntireManager();
                return;
            }

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

            LoadBuildingManager();
        }

        private void LoadBuildingManager()
        {
            dataGridView1.Rows.Clear();

            object obj = m_cbBuilding.customComboBox.Items[m_cbBuilding.customComboBox.SelectedIndex];
            Type type = obj.GetType();

            // 건물 관리자 검색
            if (type == typeof(Building))
            {
                Building building = (Building)obj;
                FacilityManagerGroup group = FormMain.Instance.DataManager.GetBuildingFacilityManagerGroup(mCurrentType, building, true);

                AddGridData(group);
            }
        }

        private void EleFloorComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (m_cbFloor.customComboBox.SelectedItem == null || m_cbFloor.customComboBox.SelectedIndex == 0)
            {
                LoadBuildingManager();
                return;
            }
                
            LoadFloorManager();
        }

        private void LoadFloorManager()
        {
            dataGridView1.Rows.Clear();
            List<int> listEquipZoneID = new List<int>();
            //ArrayList arrEquipZoneID = null;

            // 층 정보를 받아
            string floor = (string)m_cbFloor.customComboBox.SelectedItem.ToString().Trim();

            // 해당 층 이킵존을 조회
            listEquipZoneID = GetEquipZoneList(floor);
            
            if (listEquipZoneID == null)
                return;

            // 이킵존 리스트를 이용하여 담당자 싹다 조회
            foreach (int nEquipZoneID in listEquipZoneID)
            {
                //int nEquipZoneID = Convert.ToInt32(EquipZoneID.ToString());

                EquipmentZone zone = ZoneManager.Instance.GetEquipZone(nEquipZoneID);
                FacilityManagerGroup group = FormMain.Instance.DataManager.GetEquipZoneFacilityManagerGroup(mCurrentType, zone, true);

                if (group != null)
                    AddGridData(group);
            }
        }

        private void FormManager_Load(object sender, EventArgs e)
        {
            InitGridView();

            InitTypeComboBox();
            InitBuildingComboBox();
        }

        private void InitGridView()
        {
            for (int i = 0; i < dataGridView1.Columns.Count; i++)
            {
                dataGridView1.Columns[i].SortMode = DataGridViewColumnSortMode.NotSortable;
            }
        }

        private void InitTypeComboBox()
        {
            int nItemIndex = 0;
            m_cbType.customComboBox.Items.Add(Data.CommonString.POI_Fire_Kor);
            FIRE_SENSOR = nItemIndex++;

            if (UnE.SOP.ProxySOP.Instance.UsePSM == true)
            {
                m_cbType.customComboBox.Items.Add(Data.CommonString.POI_Gas_Kor);
                PSM_SENSOR = nItemIndex++;
            }
            
            if (UnE.SOP.ProxySOP.Instance.UseEarthquake == true)
            {
                m_cbType.customComboBox.Items.Add(Data.CommonString.POI_Earthquake_Kor);
                EARTHQUAKE_SENSOR = nItemIndex++;
            }

            if (UnE.SOP.ProxySOP.Instance.UseStrongWind == true)
            {
                m_cbType.customComboBox.Items.Add(Data.CommonString.POI_StrongWind_Kor);
                STRONG_WIND = nItemIndex++;
            }

            if (UnE.SOP.ProxySOP.Instance.UseBlackout == true)
            {
                m_cbType.customComboBox.Items.Add(Data.CommonString.POI_Blackout_Kor);
                BLACKOUT_SENSOR = nItemIndex++;
            }
            
            m_cbType.customComboBox.SelectedIndex = 0;
        }

        private void InitBuildingComboBox()
        {
            //m_cbBuilding.customComboBox.DisplayMemberPath = "BuildingName";
            m_cbBuilding.customComboBox.Items.Clear();
            m_cbBuilding.customComboBox.Items.Add("모두");

            foreach (KeyValuePair<int, Building> item in UnE.Spatial.ZoneManager.Instance.DicBuildings)
            {
                m_cbBuilding.customComboBox.Items.Add(item.Value);
            }

            if (m_cbBuilding.customComboBox.Items.Count > 0)
                m_cbBuilding.customComboBox.SelectedIndex = 0;
        }

        private string GetLimitText(int nUpperLimit)
        {
            if (nUpperLimit > 0)
                return "급 및 그 상위 직급";
            else if (nUpperLimit < 0)
                return "급 및 그 하위 직급";

            return "급";
        }

        private void AddGridData(FacilityManagerGroup group)
        {
            if (group == null)
                return;

            int nIndex = 1;
            string strDescription = "";

            foreach (FacilityManager mgr in group.RegularTeams)
            {
                if (mgr.LevelLimit == 0)
                    continue;

                DataTeam team = (DataTeam)mgr.Tag;
                if (team == null)
                    continue;

                //string szSub = mgr.UpperLimit == true ? "급 및 그 상위 직급" : "급 및 그 하위 직급";
                string szSub = GetLimitText(mgr.UpperLimit);

                // 당직자
                if (mgr.MemberType == 6)
                    strDescription = "";
                else
                {
                    strDescription = mgr.LevelLimit < 0 ? "팀원 모두" : (mgr.LevelLimit.ToString() + szSub);
                }

                AddGridRowData(ref nIndex, team.TeamName, strDescription, mgr);
            }

            foreach (FacilityManager mgr in group.CompanyMembers)
            {
                DataCompanyMember member = (DataCompanyMember)mgr.Tag;

                if (member == null)
                    continue;

                DataTeam team = member.GetFirstTeam();

                if (team == null)
                    continue;

                AddGridRowData(ref nIndex, member.MemberName, team.TeamName, mgr);
            }

            foreach (FacilityManager mgr in group.ExternalTeams)
            {
                DataTeam team = (DataTeam)mgr.Tag;

                if (team == null)
                    continue;

                AddGridRowData(ref nIndex, team.TeamName, team.CompanyName, mgr);
            }

            foreach (FacilityManager mgr in group.ExternalCompanyMembers)
            {
                DataExternalMember member = (DataExternalMember)mgr.Tag;

                if (member == null)
                    continue;

                //DataTeam team = member.GetFirstTeam();
                DataTeam team = member.Team;

                if (team == null)
                    continue;

                if (team.CompanyName != team.TeamName)
                {
                    if (team.CompanyName.Length == 0)
                        AddGridRowData(ref nIndex, member.Name, team.TeamName, mgr);
                    else if (team.TeamName.Length == 0)
                        AddGridRowData(ref nIndex, member.Name, team.CompanyName, mgr);
                    else
                        AddGridRowData(ref nIndex, member.Name, team.CompanyName + " " + team.TeamName, mgr);
                }
                else
                    AddGridRowData(ref nIndex, member.Name, team.CompanyName, mgr);
            }

            foreach (FacilityManager mgr in group.ControlRoomMembers)
            {
                DataTeam team = (DataTeam)mgr.Tag;

                if (team == null)
                    continue;

                string strParentTeamName = "";

                if (team.ParentTeam != null)
                    strParentTeamName = team.ParentTeam.TeamName;

                AddGridRowData(ref nIndex, team.TeamName, strParentTeamName, mgr);
            }
        }

        private void AddGridRowData(ref int nIndex, string strName, string strDescription, FacilityManager mgr)
        {
            if (FindGridRow(mgr.MemberID, mgr.MemberType) != null)
                return;

            int rowIndex = dataGridView1.Rows.Add();
            dataGridView1.Rows[rowIndex].Cells[colNo.Index].Value = rowIndex + 1;
            dataGridView1.Rows[rowIndex].Cells[colNo.Index].Tag = mgr;
            dataGridView1.Rows[rowIndex].Cells[colName.Index].Value = strName;
            dataGridView1.Rows[rowIndex].Cells[colETC.Index].Value = strDescription;

            nIndex++;
        }

        private DataGridViewRow FindGridRow(int nMemberID, int nMemberType)
        {
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (row.Cells[0].Tag == null)
                    continue;

                FacilityManager mgrSrc = (FacilityManager)row.Cells[0].Tag;

                if (mgrSrc.MemberID == nMemberID &&
                    mgrSrc.MemberType == nMemberType)
                    return row;
            }

            return null;
        }


        private List<int> GetEquipZoneList(string floor)
        {
            WebDBManager dbMgr = FormMain.Instance.DBManager;
            int nSiteID = UnE.SOP.ProxySOP.Instance.SiteID;

            List<int> listEquipZoneID = new List<int>();

            string szText = "select EquipmentZone.ID from Zone inner join EquipmentZone on Zone.ID = EquipmentZone.LinkedZoneIDList where Zone.ZoneName = '{0}' AND EquipmentZone.SiteID = {1}";
            string strSQL = string.Format(szText, floor, nSiteID);

            ArrayList arrResult = dbMgr.GetResultData(strSQL);

            if (arrResult == null || arrResult.Count == 0)
                return null;
            
            for (int i = 0; arrResult.Count > i; i++)
            {
                int nEquipZoneID = WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                listEquipZoneID.Add(nEquipZoneID);
            }

            return listEquipZoneID;
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (((System.Windows.Forms.MouseEventArgs)(e)) != null && ((System.Windows.Forms.MouseEventArgs)(e)).Button != System.Windows.Forms.MouseButtons.Left)
                return;

            IFacility.FacilityType type = mCurrentType;
            Building buildingCurrent = null;
            Floor floorCurrent = null;

            if (m_cbBuilding.customComboBox.SelectedItem != null && m_cbBuilding.customComboBox.SelectedIndex != 0)
                buildingCurrent = (Building)m_cbBuilding.customComboBox.SelectedItem;
            if (m_cbFloor.customComboBox.SelectedItem != null && m_cbFloor.customComboBox.SelectedIndex != 0)
                floorCurrent = (Floor)m_cbFloor.customComboBox.SelectedItem;

            Point ptLocation = this.PointToScreen(this.Location);

            FormManagerEdit frm = new FormManagerEdit(type, buildingCurrent, floorCurrent);
            frm.StartPosition = FormStartPosition.Manual;
            frm.Location = new Point(ptLocation.X + this.Size.Width + 40, ptLocation.Y - 70);
            DialogResult result = frm.ShowDialog();

            

            if (result == System.Windows.Forms.DialogResult.OK)
            {
                if (m_cbFloor.customComboBox.SelectedItem != null && m_cbFloor.customComboBox.SelectedIndex != 0)
                    LoadFloorManager();
                else if (m_cbBuilding.customComboBox.SelectedItem != null && m_cbBuilding.customComboBox.SelectedIndex != 0)
                    LoadBuildingManager();
                else
                    LoadEntireManager();
            }
        }

        public void Save()
        {
            FormMain.Instance.DataManager.ReUpdateFacilityManager();
        }
    }
}
