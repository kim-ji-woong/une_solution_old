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
using DBUtility;
using System.Diagnostics;

namespace SensorMonitor
{

    public partial class FormMain : Form
    {
#if !SERVICE

        static public FormMain Instance;
        private WebDBManager m_dbMgr = null;

        private IOManager m_ioMgr = null;
		public IOManager DataManager
		{
			get { return m_ioMgr; }
		}


        private void ReadSiteID()
        {
            DBUtility.Utility util = new DBUtility.Utility();
            string szSiteID = util.getinivalue("Server Connection Info", "siteid");
            if (szSiteID == null || szSiteID == "")
            {
                MessageBox.Show("Site ID가 지정되지 않았습니다. ini파일을 확인하세요", "실행 오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
                return;
            }

            int nSiteId = 1;
            if (int.TryParse(szSiteID, out nSiteId))
            {
                m_nSiteID = nSiteId;
            }
            else
            {
                MessageBox.Show("잘못된 Site ID입니다.. ini파일을 확인하세요", "실행 오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
                return;
            }
        }
        private UnE.Log.LogFileCleanupTask m_CleanUpTask = null;
        NetworkManager m_netMgr = null;
        private int m_nSiteID = 1;
        public FormMain()
        {
            InitializeComponent();

            Instance = this;
            
            ReadSiteID();

            SOPMonitor monitor = new SOPMonitor();
                       
            m_CleanUpTask = new UnE.Log.LogFileCleanupTask();
            m_CleanUpTask.CleanUp();
            m_CleanUpTask.BeginDailyTask(m_CleanUpTask.CleanUp);

            m_dbMgr = new WebDBManager(m_nSiteID);
            monitor.DbMgr = m_dbMgr;

            m_ioMgr = new IOManager(m_nSiteID);
            monitor.IoMgr = m_ioMgr;

            m_netMgr = new NetworkManager(m_dbMgr, "127.0.0.1", m_nSiteID);
            monitor.Network = m_netMgr;
            
            init();
        }

        public WebDBManager DbMgr
        {
            get { return m_dbMgr; }
            set { m_dbMgr = value; }
        }

		//private  CheckBox [] checks = new CheckBox [25];
        // Key : Receiver ID
        private Dictionary<int, CheckBox> m_dicReceiverCheckBox = new Dictionary<int, CheckBox>();
		public void OnConnectReciver(int nID)
		{
			FormMain.Instance.Invoke((MethodInvoker)delegate
			{
                int nIdx = nID - 1;
                if (nID == 40)
                    nIdx = 25;
                //checks[nIdx].Checked = true;

                CheckBox chk = null;

                if (m_dicReceiverCheckBox.TryGetValue(nIdx, out chk))
                {
                    chk.Checked = true;
                }
			});
			
		}
		public void OnDisconnectReciver(int nID)
		{
			FormMain.Instance.Invoke((MethodInvoker)delegate
			{
                int nIdx = nID - 1;
                if (nID == 40)
                    nIdx = 25;
                //checks[nIdx].Checked = false;

                CheckBox chk = null;

                if (m_dicReceiverCheckBox.TryGetValue(nIdx, out chk))
                {
                    chk.Checked = false;
                }
			});
		}


        private void init()
        {
            ArrayList arReciverList = (ArrayList)FormMain.Instance.DataManager.GetReciverList().Clone();

            foreach (Reciver reciver in arReciverList)
            {
                cmbReciver.Items.Add(reciver);
            }

			/*CheckBox [] checks2 = {
				chkBox1, chkBox2, chkBox3, chkBox4, chkBox5, 
				chkBox6, chkBox7, chkBox8, chkBox9, chkBox10, 
				chkBox11, chkBox12, chkBox13, chkBox14, chkBox15, 
			 	chkBox16, chkBox17, chkBox18, chkBox19, chkBox20, 
				chkBox21, chkBox22, chkBox23, chkBox24, chkBox25, 
                chkBox26, chkBox27,
			};
			checks = checks2;*/

            List<CheckBox> checkBoxes = new List<CheckBox>();

            checkBoxes.Add(chkBox1);
            checkBoxes.Add(chkBox6);
            checkBoxes.Add(chkBox11);
            checkBoxes.Add(chkBox16);
            checkBoxes.Add(chkBox21);
            checkBoxes.Add(chkBox2);
            checkBoxes.Add(chkBox7);
            checkBoxes.Add(chkBox12);
            checkBoxes.Add(chkBox17);
            checkBoxes.Add(chkBox22);
            checkBoxes.Add(chkBox3);
            checkBoxes.Add(chkBox8);
            checkBoxes.Add(chkBox13);
            checkBoxes.Add(chkBox18);
            checkBoxes.Add(chkBox23);
            checkBoxes.Add(chkBox4);
            checkBoxes.Add(chkBox9);
            checkBoxes.Add(chkBox14);
            checkBoxes.Add(chkBox19);
            checkBoxes.Add(chkBox24);
            checkBoxes.Add(chkBox5);
            checkBoxes.Add(chkBox10);
            checkBoxes.Add(chkBox15);
            checkBoxes.Add(chkBox20);
            checkBoxes.Add(chkBox25);
            checkBoxes.Add(chkBox26);
            checkBoxes.Add(chkBox27);

            if (arReciverList.Count > 27)
            {
                for (int i=27;i<arReciverList.Count;i++)
                {
                    int x = i % 5;
                    int y = i / 5;
                    int yPos = checkBoxes[(y - 1) * 5 + x].Location.Y + checkBoxes[(y - 1) * 5 + x].Location.Y - checkBoxes[(y - 2) * 5 + x].Location.Y;

                    CheckBox chk = new CheckBox();

                    chk.AutoSize = true;
                    chk.Location = new System.Drawing.Point(checkBoxes[x].Location.X,  yPos);
                    chk.Size = checkBoxes[0].Size;
                    chk.Text = "1";
                    chk.UseVisualStyleBackColor = true;

                    checkBoxes[0].Parent.Controls.Add(chk);
                    checkBoxes.Add(chk);
                }
            }

            for (int i=0;i<arReciverList.Count;i++)
            {
                Reciver reciver = (Reciver)arReciverList[i];

                int nIdx = reciver.ID - 1;
                if (reciver.ID == 40)
                    nIdx = 25;

                m_dicReceiverCheckBox[nIdx] = checkBoxes[i];
                checkBoxes[i].Text = reciver.Place;
            }

            /*foreach (Reciver reciver in arReciverList)
			{
                int nIdx = reciver.ID - 1;
                if (reciver.ID == 40)
                    nIdx = 25;
                checks2[nIdx].Text = reciver.Place;
			}*/
        }
    
        private bool ContainsSensorZoneType(ComboBox.ObjectCollection collection, SensorZone sensor)
        {
            foreach (SensorZone sensorZone in collection)
            {
                if (sensorZone.Type == sensor.Type)
                    return true;
            }
            return false;
        }
        
        private bool CompareData(int max_ID, int sensorID, int connected, int data)
        {
            bool CompareResult = false;
            Console.WriteLine("hello");

            max_ID--;

            //가장 최근데이터
            string strSQL = "select * from SensorZoneHistory where id = '" + max_ID + "'";
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null)
            {
                return true;
            }

            int nResultCount = arrResult.Count;
            
            //Data가 비어있다면
            if (nResultCount < 1)
                CompareResult = true;

            for (int i = 0; i < nResultCount - 3; i += 4)
            {
                int comp_SensorID = WebDBManager.GetIntField(arrResult[i + 1].ToString(), -1);
                int comp_Connected = WebDBManager.GetIntField(arrResult[i + 2].ToString(), -1);
                int comp_Data = WebDBManager.GetIntField(arrResult[i + 3].ToString(), -1);

                //최근값과 똑같은값이 들어올 때 false
                if (comp_SensorID == sensorID && comp_Connected == connected && comp_Data == data)
                    CompareResult = false;
                else
                    CompareResult = true;
            }
            return CompareResult;
        }
      

        Zone FindZone(Building building, float fFloorIndex)
        {
            int nFloorIndex = fFloorIndex > 0.0f ? (int)(fFloorIndex + 0.01f) : (int)(fFloorIndex - 0.01f);
            string strAddFloor = string.Format("{0:f1}", fFloorIndex - nFloorIndex);

            foreach (KeyValuePair<int, Zone> pair in m_ioMgr.D_Zones)
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

        public ArrayList GetBuildingZones(int nBuildingID)
        {
            if (m_ioMgr.D_BuildingZones.ContainsKey(nBuildingID))
                return m_ioMgr.D_BuildingZones[nBuildingID];

            return null;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            m_netMgr.ReleaseThread();
        }

        private void cmbReciver_SelectionChangeCommitted(object sender, EventArgs e)
        {
            int nIdx = cmbReciver.SelectedIndex;
            if (nIdx < 0)
                return;

            Reciver reciver = (Reciver)cmbReciver.SelectedItem;
            
            cmbCircuit.Items.Clear();

            foreach (KeyValuePair<string, Curcuit> pair in reciver.Curcuits)
            {
                cmbCircuit.Items.Add(pair.Value);
            }
        }

      
        private void cmbCircuit_SelectionChangeCommitted(object sender, EventArgs e)
        {
            int nIdx = cmbCircuit.SelectedIndex;
            if (nIdx < 0)
                return;    
        }

        private int m_nSelectData = -1;
        private void cmbData_SelectedIndexChanged(object sender, EventArgs e)
        {
            m_nSelectData = cmbData.SelectedIndex;
        }

        private void button1_Click(object sender, EventArgs e)
        {
			if (m_nSelectData == -1)
				return;

            Curcuit curcuit = (Curcuit)cmbCircuit.SelectedItem;
            if (curcuit == null)
                return;

            int nSensorZoneID = curcuit.TargetZoneID;

            int nTagNum = curcuit.TagNum;
            int nData = m_nSelectData;
            int nSensorType = curcuit.SensorType;
			
            m_netMgr.SendSensorData(nSensorZoneID, curcuit.ID, nSensorType, nData, "", nTagNum.ToString());

            Debug.WriteLine("[SOP서버로 회로 이름 " + curcuit.Name + " 에 대해 " + nData.ToString() + " 값 전송, Equipzone : " + nSensorZoneID + "]");
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            bool bCheck = checkBox1.Checked;
            if (bCheck == true)
            {
                checkBox1.Text = "수신기 연결 끊기";
                label2.Text = "연결됨";
                m_netMgr.CreateReciverProvider();
                Refresh();
            }
            else
            {
                checkBox1.Text = "수신기 통신 연결";
                label2.Text = "연결안됨";
                m_netMgr.ReleaseThread();
                Refresh();
            }
        }

        public void AddLog(object strMsg)
        {
            Debug.WriteLine(strMsg.ToString());
        }

		private void button2_Click(object sender, EventArgs e)
		{
			Reciver reciver = (Reciver)cmbReciver.SelectedItem;
			if (reciver == null)
				return;

			foreach (KeyValuePair<string, Curcuit> curcuit in reciver.Curcuits)
			{
				Curcuit c = curcuit.Value; 
				
				int nSensorZoneID = c.TargetZoneID;

				int nTagNum = c.TagNum;
				int nData = 0;
				int nSensorType = c.SensorType;

				m_netMgr.SendSensorData(nSensorZoneID, c.ID, nSensorType, nData, "", nTagNum.ToString());
			}
		}

		private void FormMain_Load(object sender, EventArgs e)
		{
		}

		private void checkBox14_CheckedChanged(object sender, EventArgs e)
		{
		}

		private void checkBox13_CheckedChanged(object sender, EventArgs e)
		{
		}

		private void checkBox12_CheckedChanged(object sender, EventArgs e)
		{
		}

		private void checkBox15_CheckedChanged(object sender, EventArgs e)
		{
		}

		private void checkBox16_CheckedChanged(object sender, EventArgs e)
		{
		}

#endif
    }
}
