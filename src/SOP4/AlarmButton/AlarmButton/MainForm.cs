using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using Utilities;
using DBUtility;

namespace AlarmButton 
{
	public partial class MainForm : Form 
    {
		private GlobalKeyboardHook gkh = new GlobalKeyboardHook();

        private const string ALARM_SIMULATION_DB = "AlarmSimulation";
        private DBUtility.WebDBManager m_dbMgr;
        private int m_nSiteID = 100;

        private const Keys FIRE_ON = Keys.F9;
        private const Keys FIRE_OFF = Keys.F10;
        private const Keys POLLUTION_ON = Keys.F11;
        private const Keys POLLUTION_OFF = Keys.F12;

        private const Keys FIRE_ON2 = Keys.VolumeUp;
        private const Keys FIRE_OFF2 = Keys.VolumeDown;
        private const Keys POLLUTION_ON2 = Keys.D3;
        private const Keys POLLUTION_OFF2 = Keys.D4;

        private int ReadSiteID()
        {
            DBUtility.Utility ini = new DBUtility.Utility();
            string strSiteID = ini.getinivalue("Server Connection Info", "siteid");

            int nSiteID = 1;

            if (strSiteID.Length > 0)
            {
                int.TryParse(strSiteID, out nSiteID);
            }

            return nSiteID;
        }

       
		public MainForm() 
        {
			InitializeComponent();

            m_nSiteID = ReadSiteID();
            m_dbMgr = new DBUtility.WebDBManager(ALARM_SIMULATION_DB, m_nSiteID);
		}

		private void Form1_Load(object sender, EventArgs e) 
        {
            gkh.HookedKeys.Add(FIRE_ON);
			gkh.HookedKeys.Add(FIRE_OFF);

            gkh.HookedKeys.Add(POLLUTION_ON);
            gkh.HookedKeys.Add(POLLUTION_OFF);


            gkh.HookedKeys.Add(FIRE_ON2);
            gkh.HookedKeys.Add(FIRE_OFF2);

            gkh.HookedKeys.Add(POLLUTION_ON2);
            gkh.HookedKeys.Add(POLLUTION_OFF2);
            //gkh.HookedKeys.Add(Keys.Enter);


			gkh.KeyDown += new KeyEventHandler(gkh_KeyDown);
			gkh.KeyUp += new KeyEventHandler(gkh_KeyUp);
		}

		void gkh_KeyUp(object sender, KeyEventArgs e) 
        {

			e.Handled = true;
		}

		void gkh_KeyDown(object sender, KeyEventArgs e) 
        {

            if (e.KeyCode == FIRE_ON || e.KeyCode == FIRE_ON2)
            {
                SetNewAlarms("화재", "1");
            }
            else if (e.KeyCode == FIRE_OFF || e.KeyCode == FIRE_OFF2)
            {
                int nTagID = GetTagID("화재");
                SetNewAlarms("화재", "0\t" + nTagID);
            }
            else if (e.KeyCode == POLLUTION_ON || e.KeyCode == POLLUTION_ON2)
            {
                SetNewAlarms("오염", "1\t암모니아" );
            }
            else if (e.KeyCode == POLLUTION_OFF || e.KeyCode == POLLUTION_OFF2)
            {
                int nTagID = GetTagID("오염");
                SetNewAlarms("오염", "0\t" + nTagID);
            }
			e.Handled = true;
		}

        private int GetTagID(string szType)
        {            
            string strSQL = "Select SensorTagInfoID from AlarmBoard where SiteID = " + m_nSiteID.ToString() + " AND AlarmName like '%"+szType+"%'";          
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL, 0, ALARM_SIMULATION_DB);
          
            if (arrResult == null || arrResult.Count == 0)
                return -1;

            int nSensorTagInfoID;
            foreach (object obj in arrResult)
            {
                if (int.TryParse(obj.ToString().Trim(), out nSensorTagInfoID))
                    return nSensorTagInfoID;
            }
            return -1;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            SetNewAlarms("지진", "진도\t3");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SetNewAlarms("화재", "1");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            SetNewAlarms("오염", "1\t암모니아");
        }

        private void button4_Click(object sender, EventArgs e)
        {
            SetNewAlarms("화재", "0");
        }

        private void button5_Click(object sender, EventArgs e)
        {
            int nTagID = GetTagID("오염");
            SetNewAlarms("오염", "0\t" + nTagID.ToString());
        }

        private int GetMaxData(string strTableName, string strDBName)
        {
            string strSQL = "Select max(ID) from " + strTableName;
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL, 0, strDBName);
            if (arrResult == null || arrResult.Count == 0)
                return 0;

            VariousData<int> id = WebDBManager.GetIntField(arrResult[0].ToString());
            return id == null ? 0 : id.Data;
        }

        private void SetNewAlarms(string szType, string onOff)
        {
            int nMaxID = GetMaxData("RequestAlarm", ALARM_SIMULATION_DB);
            string strSQL = string.Format("INSERT INTO RequestAlarm (ID, AlarmCategory, AlarmParameter, SiteID ) VALUES ( {0},'{1}','{2}',{3})", (nMaxID + 1), szType, onOff, m_nSiteID);
            m_dbMgr.GetResultData(strSQL, 0, ALARM_SIMULATION_DB);
        }
	}
}