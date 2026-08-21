using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using UnE.GUI;

namespace FireManagement.Docking
{
    public partial class FormFileLoad : Form
    {
        private FormBuildingGroupList m_FormBuildingList = null;

        private IOManager m_ioMgr;
        private FMFHeader m_header = null;

        public FormFileLoad(IOManager ioMgr)
        {
            InitializeComponent();

            InitPanel();
            //DocumentVersion();
            m_ioMgr = ioMgr;
            this.Focus();
            this.Select();

        }

        private void InitPanel()
        {
            //FormMain2 frmMain = FormMain2.Instance;

            //m_FormBuildingList.Location = PointToClient(new Point(frmMain.PanelLeft.Right, frmMain.PanelTop.Location.Y + frmMain.PanelTop.Height));
            //m_FormBuildingList.Size = new Size(frmMain.PanelMain.Width, frmMain.PanelMain.Height);
        }

        private void DocumentVersion()
        {
            int nGridViewHeight = dataGridVersion.Height / 4;

            dataGridVersion.ColumnCount = 2;

            string[] strItem = new string[] { "버전", "파일 생성일", "작성자", "설명" };
            string[] strValue = null;

            if (m_header != null)
            {
                strValue = new string[] { m_header.Version, m_header.Time.ToString(), m_header.Writer, m_header.Description };
            }

            for (int i = 0; i < 4; i++)
            {
                DataGridViewRow gridRow = new DataGridViewRow();
                gridRow.Height = nGridViewHeight;

                DataGridViewCell cell = null;

                cell = new DataGridViewTextBoxCell();
                cell.Value = strItem[i];
                gridRow.Cells.Add(cell);

                cell = new DataGridViewTextBoxCell();
                cell.Value = strValue == null ? "" : strValue[i];
                gridRow.Cells.Add(cell);

                dataGridVersion.Rows.Add(gridRow);
            }

            DataGridViewCellStyle cs = dataGridVersion.DefaultCellStyle.Clone();
            cs.BackColor = Color.Gray;
            cs.SelectionBackColor = Color.Gray;
            cs.Font = new Font("맑은 고딕", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            cs.ForeColor = System.Drawing.Color.FromArgb(1,1,1);
            dataGridVersion.Rows[0].Cells[0].Style = cs;

            dataGridVersion.Font = new Font("맑은 고딕", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            dataGridVersion.ForeColor = System.Drawing.Color.FromArgb(1, 1, 1);

            //dataGridVersion.Rows[1].Cells[1].Value = "";// DateTime.Today.ToString();

        }


        private void FormFileLoad_Load(object sender, EventArgs e)
        {
            foreach (KeyValuePair<BuildingGroup, ArrayList> groups in m_ioMgr.AllBuildingGroups)
            {
                SetBuildingGroup(groups.Key);
                break;
            }
            DocumentVersion();
        }

        private void btnBuildingGroup_Click(object sender, EventArgs e)
        {
            FormMain2 frmMain = FormMain2.Instance;

            RibbonButton rbBtn = (RibbonButton)sender;
            TextPictureBox pb = (TextPictureBox)rbBtn.Tag;
            BuildingGroup group = (BuildingGroup)pb.Tag;

            if (m_FormBuildingList == null || m_FormBuildingList.IsDisposed)
            {
                m_FormBuildingList = new FormBuildingGroupList(group, 1);

                m_FormBuildingList.StartPosition = FormStartPosition.Manual;

                Point pt = PointToScreen(new Point(frmMain.PanelMain.Location.X - frmMain.PanelLeft.Width, frmMain.PanelMain.Location.Y - frmMain.PanelTop.Height));
                Size sz = new Size(frmMain.PanelMain.Size.Width - frmMain.PanelLeft.Width, frmMain.PanelMain.Size.Height - frmMain.PanelBottom.Height);

                m_FormBuildingList.Location = new Point(pt.X, pt.Y);
                m_FormBuildingList.Size = new Size(sz.Width, sz.Height);
                m_FormBuildingList.Dock = DockStyle.Fill;
            }

            m_FormBuildingList.SelectGroup(group);
            m_FormBuildingList.ShowDialog();
        }

        private void btnBuilding_Click(object sender, EventArgs e)
        {
            FormMain2 frmMain = FormMain2.Instance;
            
            RibbonButton rbBtn = (RibbonButton)sender;
            TextPictureBox pbBuilding = (TextPictureBox)rbBtn.Tag;

            if (pbBuilding == null)
                return;

            Building building = (Building)pbBuilding.Tag;

            TextPictureBox pbGroup = (TextPictureBox)pictureBoxGroup;
            BuildingGroup group = (BuildingGroup)pbGroup.Tag;

            if (m_FormBuildingList == null || m_FormBuildingList.IsDisposed)
            {
                m_FormBuildingList = new FormBuildingGroupList(group, building, 2);

                m_FormBuildingList.StartPosition = FormStartPosition.Manual;

                Point pt = PointToScreen(new Point(frmMain.PanelMain.Location.X - frmMain.PanelLeft.Width, frmMain.PanelMain.Location.Y - frmMain.PanelTop.Height));
                Size sz = new Size(frmMain.PanelMain.Size.Width - frmMain.PanelLeft.Width, frmMain.PanelMain.Size.Height - frmMain.PanelBottom.Height);

                m_FormBuildingList.Location = new Point(pt.X, pt.Y);
                m_FormBuildingList.Size = new Size(sz.Width, sz.Height);
                m_FormBuildingList.Dock = DockStyle.Fill;
            }

            m_FormBuildingList.SelectBuilding(building);
            m_FormBuildingList.ShowDialog();
        }

        private void btnFloor_Click(object sender, EventArgs e)
        {
            FormMain2 frmMain = FormMain2.Instance;

            RibbonButton rbBtn = (RibbonButton)sender;
            TextPictureBox pbZone = (TextPictureBox)rbBtn.Tag;
            Floor floor = (Floor)pbZone.Tag;

            TextPictureBox pbBuilding = (TextPictureBox)pictureBoxBuilding;
            Building building = (Building)pbBuilding.Tag;

            if (m_FormBuildingList == null || m_FormBuildingList.IsDisposed)
            {
                m_FormBuildingList = new FormBuildingGroupList(building, floor, 3);

                m_FormBuildingList.StartPosition = FormStartPosition.Manual;

                Point pt = PointToScreen(new Point(frmMain.PanelMain.Location.X - frmMain.PanelLeft.Width, frmMain.PanelMain.Location.Y - frmMain.PanelTop.Height));
                Size sz = new Size(frmMain.PanelMain.Size.Width - frmMain.PanelLeft.Width, frmMain.PanelMain.Size.Height - frmMain.PanelBottom.Height);

                m_FormBuildingList.Location = new Point(pt.X, pt.Y);
                m_FormBuildingList.Size = new Size(sz.Width, sz.Height);
                m_FormBuildingList.Dock = DockStyle.Fill;
            }

            m_FormBuildingList.SelectFloor(floor);
            m_FormBuildingList.ShowDialog();
        }

        public BuildingGroup GetCurrentBuildingGroup()
        {
            TextPictureBox pbGroup = (TextPictureBox)pictureBoxGroup;
            return (BuildingGroup)pbGroup.Tag;
        }

        public void SetBuildingGroup(BuildingGroup group)
        {
            btnBuildingGroup.Tag = pictureBoxGroup;
            pictureBoxGroup.Tag = group;
            pictureBoxGroup.Text = group.BuildingGroupName;

            SetChageBuildingGroup(group);
        }

        public void SetBuilding(Building building)
        {
            btnBuilding.Tag = pictureBoxBuilding;
            pictureBoxBuilding.Tag = building;
            pictureBoxBuilding.Text = building.BuildingName;
        }

        public void SetFloor(Floor floor)
        {
            btnFloor.Tag = pictureBoxFloor;
            pictureBoxFloor.Tag = floor;
            pictureBoxFloor.Text = floor.ToString();
        }


        public void SetChageBuildingGroup(BuildingGroup group)
        {
            if (m_ioMgr.AllBuildingGroups.ContainsKey(group))
            {
                ArrayList arrBuilding = m_ioMgr.AllBuildingGroups[group];
                if (arrBuilding.Count == 0)
                {
                    pictureBoxBuilding.Text = "";
                    pictureBoxFloor.Text = "";
                    pictureBoxBuilding.Tag = null;
                    pictureBoxFloor.Tag = null;
                    return;
                }
                Building building = (Building)arrBuilding[0];

                SetBuilding(building);
                SetChangeBuilding(building);
            }
        }

        public void SetChangeBuilding(Building building)
        {
            if (m_ioMgr.BuildingZones.ContainsKey(building.ID))
            {
                ArrayList arrZone = m_ioMgr.BuildingZones[building.ID];
                Zone zone = (Zone)arrZone[0];

                Floor floor = new Floor(zone.FloorIndex + zone.AddFloor);

                SetBuilding(building);
                SetFloor(floor);
            }
        }


        private void initPictureBox()
        {

        }

        private void btnLoadFile_Click(object sender, EventArgs e)
        {
            FormMain2 frmMain = FormMain2.Instance;

            TextPictureBox pbBuilding = (TextPictureBox)pictureBoxBuilding;
            Building building = (Building)pbBuilding.Tag;

            if (building == null)
                return;

            TextPictureBox pbFloor = (TextPictureBox)pictureBoxFloor;
            Floor floor = (Floor)pbFloor.Tag;



            if (floor == null)
                return;

            Zone prevZone = frmMain.CurrentZone;
            Zone zone = frmMain.IOManager.FindZone(building, floor.FloorIndex);

            LoadZone(zone, prevZone);
            
        }

        public bool LoadZone(Zone zone, Zone prevZone)
        {
            FormMain2 frmMain = FormMain2.Instance;

            Building building = zone.Building;
            Floor floor = new Floor(zone.FloorIndex + zone.AddFloor);

            if (zone == null)
            {
                string strMsg = string.Format("{0} {1}에 해당하는 Zone 정보가 DB에 존재하지 않습니다.", building.BuildingName, floor.ToString());
                MessageBox.Show(strMsg);
                return false;
            }
            else
            {
                if (frmMain.CurrentZone == zone)
                {
                    frmMain.SelectFireManagerTab(1);
                    return false;
                }
                frmMain.CurrentZone = zone;
            }

            if (zone.DXFFilePath == "")
            {
                string strMsg = string.Format("{0} {1}에 해당하는 도면 파일이 존재하지 않습니다.", building.BuildingName, floor.ToString());
                MessageBox.Show(strMsg);
                return false;
            }
            else
            {
                if (prevZone != null)
                    frmMain.IOManager.CompareZoneEquipmentsToDB(prevZone);

                string szFileName = GetIndoorFilePath(zone.DXFFilePath); 
                if (!LoadDXF(szFileName, zone))
                {
                    string strMsg = string.Format("{0} 파일을 여는데 실패하였습니다.", szFileName);
                    MessageBox.Show(strMsg);
                    return false;
                }

                EventManager.Instance.ProcessEvent(Event.NEW_DXF_OPENED);
                FormMain2.Instance.TypePictureBoxTab = 1;
            }

            return true;
        }

        private string GetIndoorFilePath(string strOriginFilePath)
        {
            int nIndex = strOriginFilePath.LastIndexOf('.');

            if (nIndex < 0)
            {
                return strOriginFilePath + "." + FormMain2.Instance.IndoorFileType;
            }

            string strFilePath = strOriginFilePath.Substring(0, nIndex + 1) + FormMain2.Instance.IndoorFileType;
            return strFilePath;
        }

        private bool LoadDXF(string strPath, Zone zone)
        {
            int nFECount, nHDCount, nFACount, nFRCount;

            if (FormMain2.Instance.DXFManager.LoadEquipment(strPath, zone, out nFECount, out nHDCount, out nFACount, out nFRCount))
            {
                //dataGridEquipment.Rows[0].Cells[2].Value = nFECount.ToString();
                //dataGridEquipment.Rows[1].Cells[2].Value = nHDCount.ToString();
                //dataGridEquipment.Rows[2].Cells[2].Value = nFACount.ToString();
                //dataGridEquipment.Columns[2].Visible = true;

                //EventManager.Instance.ProcessEvent(Event.NEW_DXF_OPENED);
                // 적용 버튼을 누르기 전에는 안보이도록 한다.
                //FormMain.Instance.DXFControl.Visible = false;

                return true;
            }

            return false;
        }

        public void ReloadData()
        {

        }

        public void SetDocumentInfo(FMFHeader header)
        {
            m_header = header;
        }
    }
}
