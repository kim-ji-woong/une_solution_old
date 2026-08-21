using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;

namespace FireManagement
{
    public partial class FormBuildingGroupList : Form
    {
        //private System.Windows.Forms.PictureBox pictureBox1;

        private BuildingGroup group = null;
        private Building building = null;
        private Floor floor = null;

        private int btnType = 1;

        public void ResizeControl()
        {
            FormMain2 frmMain = FormMain2.Instance;
            Point pt = PointToScreen(new Point(frmMain.PanelMain.Location.X - frmMain.PanelLeft.Width, frmMain.PanelMain.Location.Y - frmMain.PanelTop.Height));
            Size sz = new Size(frmMain.PanelMain.Size.Width - frmMain.PanelLeft.Width, frmMain.PanelMain.Size.Height - frmMain.PanelBottom.Height);

            btnCancel.Location = new Point(sz.Width / 2 - btnCancel.Width - 30, frmMain.PanelMain.Height- 100);
            btnSelect.Location = new Point(sz.Width / 2, btnCancel.Location.Y);

            gridViewBuildingGroup.Height = btnCancel.Location.Y - 15 - frmMain.PanelTop.Height;

        }


        public FormBuildingGroupList(BuildingGroup agroup, int nBtnType)
        {
            InitComponent(nBtnType);
            group = agroup;

            initPanel();
        }

        public FormBuildingGroupList(BuildingGroup agroup, Building abuilding, int nBtnType)
        {
            InitComponent(nBtnType);
            group = agroup;
            building = abuilding;

            initPanel();
        }

        public FormBuildingGroupList(Building abuilding, Floor afloor, int nBtnType)
        {
            InitComponent(nBtnType);
            building = abuilding;
            floor = afloor;

            initPanel();
        }

        private void InitComponent(int nBtnType)
        {
            InitializeComponent();
            Set_GridView();

            ResizeControl();

            btnType = nBtnType;
        }

        private void initPanel()
        {
            IOManager mgr = FormMain2.Instance.IOManager;

            if (btnType == 1)
            {
                foreach (KeyValuePair<BuildingGroup, ArrayList> pair in mgr.AllBuildingGroups)
                {
                    BuildingGroup buildingGroup = pair.Key;
                    gridViewBuildingGroup.Rows.Add(buildingGroup);

                    lblSelectName.Text = "건물그룹 선택";
                }
            }
            else if (btnType == 2)
            {
                if (group == null)
                    return;

                ArrayList arrBuilding = mgr.AllBuildingGroups[group];
                foreach (Building b in arrBuilding)
                {
                    gridViewBuildingGroup.Rows.Add(b);

                    lblSelectName.Text = "건물 선택";
                }
            }
            else if (btnType == 3)
            {
                if (building == null)
                    return;

                ArrayList arrZone = mgr.BuildingZones[building.ID];
                foreach (Zone z in arrZone)
                {
                    Floor floor = new Floor(z.AddFloor + z.FloorIndex);
                    gridViewBuildingGroup.Rows.Add(floor);

                    lblSelectName.Text = "건물 층 선택";
                }
            }
        }

        private void Set_GridView()
        {
            gridViewBuildingGroup.ColumnCount = 1;
            gridViewBuildingGroup.RowTemplate.Height = 98;

            gridViewBuildingGroup.Font = new Font("맑은 고딕", 28);
            gridViewBuildingGroup.ForeColor = System.Drawing.Color.FromArgb(6,6,6);
        }

        private void FormBuildingGroupList_Load(object sender, EventArgs e)
        {
        }

        public void SelectGroup(BuildingGroup group)
        {
            foreach (DataGridViewRow row in gridViewBuildingGroup.Rows)
            {
                if (group == (BuildingGroup)row.Cells[0].Value)
                {
                    row.Cells[0].Selected = true;
                    return;
                }
            }
        }

        public void SelectBuilding(Building building)
        {
            foreach (DataGridViewRow row in gridViewBuildingGroup.Rows)
            {
                if (building == (Building)row.Cells[0].Value)
                {
                    row.Cells[0].Selected = true;
                    return;
                }
            }
        }

        public void SelectFloor(Floor floor)
        {
            string strTarget = floor.ToString();

            foreach (DataGridViewRow row in gridViewBuildingGroup.Rows)
            {
                string strSrc = ((Floor)row.Cells[0].Value).ToString();

                if (strTarget == strSrc)
                {
                    row.Cells[0].Selected = true;
                    return;
                }
            }
        }

        private void btnSelect_Click(object sender, EventArgs e)
        {
            if (btnType == 1)
            {
                if (gridViewBuildingGroup.RowCount == 0)
                    return;

                int nRowIndex = gridViewBuildingGroup.CurrentCell.RowIndex;

                BuildingGroup group = (BuildingGroup)gridViewBuildingGroup.Rows[nRowIndex].Cells[0].Value;

                FormMain2.Instance.FormFileLoad.SetBuildingGroup(group);

                //FormMain2.Instance.FormFileLoad.SetChageBuildingGroup(group);
            }
            else if (btnType == 2)
            {
                if (gridViewBuildingGroup.RowCount == 0)
                    return;

                int nRowIndex = gridViewBuildingGroup.CurrentCell.RowIndex;

                Building building = (Building)gridViewBuildingGroup.Rows[nRowIndex].Cells[0].Value;

                FormMain2.Instance.FormFileLoad.SetChangeBuilding(building);
            }
            else if (btnType == 3)
            {
                if (gridViewBuildingGroup.RowCount == 0)
                    return;

                int nRowIndex = gridViewBuildingGroup.CurrentCell.RowIndex;
  
                Floor floor = (Floor)gridViewBuildingGroup.Rows[nRowIndex].Cells[0].Value;

                FormMain2.Instance.FormFileLoad.SetFloor(floor);
            }

            FormMain2.Instance.Refresh();

            this.Dispose();
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Dispose();
            this.Close();
        }

        private void gridViewBuildingGroup_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (btnType == 1)
            {
                int nRowIndex = gridViewBuildingGroup.CurrentCell.RowIndex;
                BuildingGroup group = (BuildingGroup)gridViewBuildingGroup.Rows[nRowIndex].Cells[0].Value;

                FormMain2.Instance.FormFileLoad.SetBuildingGroup(group);

                //FormMain2.Instance.FormFileLoad.SetChageBuildingGroup(group);
            }
            else if (btnType == 2)
            {
                int nRowIndex = gridViewBuildingGroup.CurrentCell.RowIndex;
                Building building = (Building)gridViewBuildingGroup.Rows[nRowIndex].Cells[0].Value;

                FormMain2.Instance.FormFileLoad.SetChangeBuilding(building);
            }
            else if (btnType == 3)
            {
                int nRowIndex = gridViewBuildingGroup.CurrentCell.RowIndex;
                Floor floor = (Floor)gridViewBuildingGroup.Rows[nRowIndex].Cells[0].Value;

                FormMain2.Instance.FormFileLoad.SetFloor(floor);
            }
            FormMain2.Instance.Refresh();

            this.Dispose();
            this.Close();
        }
    }
}
