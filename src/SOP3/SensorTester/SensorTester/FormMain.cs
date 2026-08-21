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

namespace SensorTester
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

		public void AddLog(object log)
		{
			Invoke((MethodInvoker)delegate
			{
				m_textBox1.AppendText(log.ToString());
			});
		}
		public void AddLogLine(object log)
		{
			Invoke((MethodInvoker)delegate
			{
				m_textBox1.AppendText("\r\n" +log.ToString());
			});
		}
        NetworkManager m_netMgr = null;

        public FormMain()
        {
            InitializeComponent();

            Instance = this;
            m_dbMgr = new WebDBManager();
            m_ioMgr = new IOManager();
            m_netMgr = new NetworkManager(m_dbMgr);

            Init();
        }

        public WebDBManager DbMgr
        {
            get { return m_dbMgr; }
            set { m_dbMgr = value; }
        }

        private void Init()
        {
			ArrayList arReciver = m_ioMgr.GetReciverList();
			m_cmbRecivers.Items.AddRange(arReciver.ToArray());
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
		       
            
        private int nSensor_type = 1;
        

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

		private void button1_Click_1(object sender, EventArgs e)
		{

		}

		private void m_cmbRecivers_SelectedIndexChanged(object sender, EventArgs e)
		{

		}

		private void m_cmbRecivers_SelectionChangeCommitted(object sender, EventArgs e)
		{

		}

		private void FormMain_Load(object sender, EventArgs e)
		{

		}
    }
}
