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
        static public FormMain Instance;
        private WebDBManager m_dbMgr = null;

        private IOManager m_ioMgr = null;
		public IOManager DataManager
		{
			get { return m_ioMgr; }
		}

        NetworkManager m_netMgr = null;

        public FormMain()
        {
            InitializeComponent();

            Instance = this;
            m_dbMgr = new WebDBManager();
            m_ioMgr = new IOManager();
            m_netMgr = new NetworkManager(m_dbMgr);

            init();
        }

        public WebDBManager DbMgr
        {
            get { return m_dbMgr; }
            set { m_dbMgr = value; }
        }

		private  CheckBox [] checks = new CheckBox [25];
		public void OnConnectReciver(int nID)
		{
			FormMain.Instance.Invoke((MethodInvoker)delegate
			{
				checks[nID - 1].Checked = true;
			});
			
		}
		public void OnDisconnectReciver(int nID)
		{
			FormMain.Instance.Invoke((MethodInvoker)delegate
			{
				checks[nID - 1].Checked = false;
			});
		}


        private void init()
        {
            ArrayList arReciverList = FormMain.Instance.DataManager.GetReciverList();

            foreach (Reciver reciver in arReciverList)
            {
                cmbReciver.Items.Add(reciver);
            }

			CheckBox [] checks2 = {
				chkBox1, chkBox2, chkBox3, chkBox4, chkBox5, 
				chkBox6, chkBox7, chkBox8, chkBox9, chkBox10, 
				chkBox11, chkBox12, chkBox13, chkBox14, chkBox15, 
			 	chkBox16, chkBox17, chkBox18, chkBox19, chkBox20, 
				chkBox21, chkBox22, chkBox23, chkBox24, chkBox25, 
			};
			checks = checks2;

			int i = 0;
			foreach (Reciver reciver in arReciverList)
			{
				checks2[reciver.ID - 1].Text = reciver.Place;
			}           
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
        
        private bool CompareResult = false;
        private void CompareData(int max_ID, int sensorID, int connected, int data)
        {
            Console.WriteLine("hello");

            max_ID--;

            //가장 최근데이터
            string strSQL = "select * from SensorZoneHistory where id = '" + max_ID + "'";
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null)
            {
                CompareResult = true;
                return;
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

            //cmbData.SelectedIndex = 0;
                
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

            int nEquipzoneID = curcuit.TargetZoneID;
            
            int nTagNum = curcuit.TagNum;
            int nData = m_nSelectData;
            int nSensorType = curcuit.SensorType;
			
            m_netMgr.SendSensorData(nEquipzoneID, nSensorType, nData, "", nTagNum.ToString());

            Debug.WriteLine("[SOP서버로 회로 이름 " + curcuit.Name + " 에 대해 " + nData.ToString() + " 값 전송, Equipzone : " + nEquipzoneID + "]");
                       
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
                m_netMgr.ShutdownSensorThread = true;
                Refresh();
            }
        }

        public void AddLog(object strMsg)
        {
            //FormMain.Instance.Invoke((MethodInvoker)delegate
            //{
            //    textBox1.Text = textBox1.Text + strMsg.ToString() ;
            //});
          
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
				
				int nEquipzoneID = c.TargetZoneID;

				int nTagNum = c.TagNum;
				int nData = 0;
				int nSensorType = c.SensorType;

				m_netMgr.SendSensorData(nEquipzoneID, nSensorType, nData, "", nTagNum.ToString());
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
    }
}
