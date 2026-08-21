using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BuildingSMS
{
    public partial class FormMain : Form
    {
        private ZoneManager m_zoneManager = new ZoneManager();
        private SendManager m_sendManager = null;

        public FormMain()
        {
            InitializeComponent();
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            int nBuildingCount = m_zoneManager.GetBuildingCount();

            for (int i=0;i<nBuildingCount;i++)
            {
                Building building = m_zoneManager.GetBuilding(i);
                cboBuildings.Items.Add(building);
            }

            if (cboBuildings.Items.Count > 0)
                cboBuildings.SelectedIndex = 0;
        }

        private void cboBuildings_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboBuildings.SelectedIndex < 0)
                return;

            Building building = (Building)cboBuildings.Items[cboBuildings.SelectedIndex];
            cboFloors.Items.Clear();

            for (int i=building.MinFloor;i<=building.MaxFloor;i++)
            {
                string str = i < 0 ? string.Format("지하 {0}층", -i) : string.Format("{0}층", i + 1);
                cboFloors.Items.Add(str);
            }

            if (cboFloors.Items.Count > 0)
                cboFloors.SelectedIndex = 0;
        }

        private void btnFire_Click(object sender, EventArgs e)
        {
            Building building = (Building)cboBuildings.Items[cboBuildings.SelectedIndex];

            btnSendSMS.Enabled = true;
            textBoxOutput.Text = "";

            string strFloor = cboFloors.Items[cboFloors.SelectedIndex].ToString();
            int nFloorIndex = GetFloorIndex(strFloor);

            if (building.BuildingName == "Hotel")
                m_sendManager = new HotelManager(building, nFloorIndex);
            else if (building.BuildingName == "Retail")
                m_sendManager = new RetailManager(building, nFloorIndex);
            else if (building.BuildingName == "OfficeA")
                m_sendManager = new Tower01Manager(building, nFloorIndex);
            else if (building.BuildingName == "OfficeB")
                m_sendManager = new Tower02Manager(building, nFloorIndex);

            List<int> floors = new List<int>();
            bool isLast;

            if (m_sendManager.GetNext(floors, 1, out isLast) == false)
            {
                MessageBox.Show("계산할 수 없습니다.");
            }
            else
            {
                SetResult(floors, isLast);
            }
        }

        private int GetFloorIndex(string strFloor)
        {
            bool underground = false;

            if (strFloor.StartsWith("지하"))
            {
                underground = true;
                strFloor = strFloor.Substring(2).Trim();
            }

            strFloor = strFloor.Replace("층", "").Trim();

            int nFloorIndex;

            if (int.TryParse(strFloor, out nFloorIndex))
            {
                if (underground)
                    return -nFloorIndex;
                else
                    return nFloorIndex - 1;
            }

            return 0;
        }

        private void SetResult(List<int> floors, bool isLast)
        {
            string strLog = "";

            foreach (int nFloorIndex in floors)
            {
                string str = nFloorIndex < 0 ? string.Format("지하 {0}층", -nFloorIndex) : string.Format("{0}층", nFloorIndex + 1);

                if (strLog.Length == 0)
                    strLog = str;
                else
                    strLog += ", " + str;
            }

            if (textBoxOutput.Text.Length == 0)
                textBoxOutput.Text = "수신자 : " + strLog;
            else
                textBoxOutput.Text += "\r\n수신자 : " + strLog;

            if (isLast)
                btnSendSMS.Enabled = false;
        }

        private void btnSendSMS_Click(object sender, EventArgs e)
        {
            if (m_sendManager == null)
                return;

            List<int> floors = new List<int>();
            bool isLast;

            if (m_sendManager.GetNext(floors, 1, out isLast) == false)
            {
                MessageBox.Show("계산할 수 없습니다.");
            }
            else
            {
                SetResult(floors, isLast);
            }
        }

        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
        }
    }
}
