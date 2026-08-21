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

            m_netMgr = new NetworkManager(m_dbMgr, null, m_nSiteID);
            monitor.Network = m_netMgr;
            
            init();
        }

        public WebDBManager DbMgr
        {
            get { return m_dbMgr; }
            set { m_dbMgr = value; }
        }

		private  CheckBox [] checks = new CheckBox [25];
        // Key : Receiver ID
        // Value : CheckBox Control
        private Dictionary<int, CheckBox> m_dicReceiverCheckBox = new Dictionary<int, CheckBox>();
		public void OnConnectReciver(int nID)
		{
			FormMain.Instance.Invoke((MethodInvoker)delegate
			{
                CheckBox checkBox = null;

                if (m_dicReceiverCheckBox.TryGetValue(nID, out checkBox))
                {
                    checkBox.Checked = true;
                }
                /*int nIdx = nID - 1;
                if (nID == 40)
                    nIdx = 25;
                checks[nIdx].Checked = true;*/
			});
			
		}
		public void OnDisconnectReciver(int nID)
		{
			FormMain.Instance.Invoke((MethodInvoker)delegate
			{
                CheckBox checkBox = null;

                if (m_dicReceiverCheckBox.TryGetValue(nID, out checkBox))
                {
                    checkBox.Checked = false;
                }
                /*int nIdx = nID - 1;
                if (nID == 40)
                    nIdx = 25;
                checks[nIdx].Checked = false;*/
			});
		}


        private void init()
        {
            ArrayList arReciverList = (ArrayList)FormMain.Instance.DataManager.GetReciverList().Clone();

            foreach (Reciver reciver in arReciverList)
            {
                cmbReciver.Items.Add(reciver);
            }

            CheckBox[] checks2 = {
				chkBox1, chkBox6, chkBox11, chkBox16, chkBox21, 
				chkBox2, chkBox7, chkBox12, chkBox17, chkBox22, 
				chkBox3, chkBox8, chkBox13, chkBox18, chkBox23, 
			 	chkBox4, chkBox9, chkBox14, chkBox19, chkBox24, 
				chkBox5, chkBox10, chkBox15, chkBox20, chkBox25, 
                chkBox26, chkBox27,
			};
			/*CheckBox [] checks2 = {
				chkBox1, chkBox2, chkBox3, chkBox4, chkBox5, 
				chkBox6, chkBox7, chkBox8, chkBox9, chkBox10, 
				chkBox11, chkBox12, chkBox13, chkBox14, chkBox15, 
			 	chkBox16, chkBox17, chkBox18, chkBox19, chkBox20, 
				chkBox21, chkBox22, chkBox23, chkBox24, chkBox25, 
                chkBox26, chkBox27,
			};*/

            // 추가된 수신반 리스트를 동적으로 추가하기
            #region AddDynamicSensorServer
            //checks = checks2;
            List<CheckBox> addedCheckBox = new List<CheckBox>();
            int width = chkBox6.Location.X - chkBox1.Location.X;
            int height = chkBox2.Location.Y - chkBox1.Location.Y;
            
            for (int i = checks2.Count(); i < arReciverList.Count;i++ )
            {
                int x = chkBox1.Location.X + (i % 5) * width;
                int y = chkBox1.Location.Y + (i / 5) * height;

                CheckBox checkBoxReceiver = new CheckBox();

                checkBoxReceiver.AutoSize = chkBox1.AutoSize;
                checkBoxReceiver.Location = new System.Drawing.Point(x, y);
                checkBoxReceiver.Size = chkBox1.Size;
                checkBoxReceiver.Text = chkBox1.Text;
                checkBoxReceiver.UseVisualStyleBackColor = chkBox1.UseVisualStyleBackColor;

                chkBox1.Parent.Controls.Add(checkBoxReceiver);
                addedCheckBox.Add(checkBoxReceiver);
            }

            checks = new CheckBox[checks2.Count() + addedCheckBox.Count];

            for (int i=0;i<checks2.Count();i++)
            {
                checks[i] = checks2[i];
            }

            for (int i = 0; i < addedCheckBox.Count;i++ )
            {
                checks[i + checks2.Count()] = addedCheckBox[i];
            }
            #endregion

            for (int i=0;i<arReciverList.Count;i++)
            {
                Reciver reciver = (Reciver)arReciverList[i];
                checks[i].Text = reciver.Place;
                m_dicReceiverCheckBox[reciver.ID] = checks[i];
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

            foreach (KeyValuePair<int, Curcuit> pair in reciver.Curcuits)
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
                labelConnectionState.Text = "연결됨";
                m_netMgr.CreateReciverProvider();
                Refresh();
            }
            else
            {
                checkBox1.Text = "수신기 통신 연결";
                labelConnectionState.Text = "연결안됨";
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

			foreach (KeyValuePair<int, Curcuit> curcuit in reciver.Curcuits)
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
