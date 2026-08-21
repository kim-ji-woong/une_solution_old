using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SVMSEventReciver
{
    public partial class FormMain : Form
    {
#if !SERVICE
        private SVMSEventReciver eventReciver = null;

        private static FormMain _instance = null;
        public static FormMain Instance
        {
            get
            {
                return _instance;
            }
        }

        public FormMain()
        {
            _instance = this;
            InitializeComponent();
        }

        private int m_nSiteID = 100;
        private void ReadSiteID()
        {
            DBUtility.Utility util = new DBUtility.Utility();
            string szSiteID = util.getinivalue("Server Connection Info", "siteid");
            if (szSiteID == null || szSiteID == "")
            {                
                return;
            }

            int nSiteId = 1;
            if (int.TryParse(szSiteID, out nSiteId))
            {
                m_nSiteID  = nSiteId;
            }
            else
            {                
                return;
            }
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            ReadSiteID();
            eventReciver = new SVMSEventReciver(m_nSiteID);

            lbSVMS.Text = "접속안됨";
            lbSVMS.ForeColor = Color.Red;

            lbSOPServer.Text = "접속안됨";
            lbSOPServer.ForeColor = Color.Red;

            eventReciver.ConnectServer();
            eventReciver.RequestCameraList();

            timer1.Enabled = true;
            timer1.Start();
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {
        }

        private void button1_Click(object sender, EventArgs e)
        {
           
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if(eventReciver!= null)
            {
                bool bConnect = eventReciver.IsConnect;
                if( bConnect == true)
                {
                    lbSVMS.Text = "접속중";
                    lbSVMS.ForeColor = Color.Green;
                }
                else
                {
                    lbSVMS.Text = "접속안됨";
                    lbSVMS.ForeColor = Color.Red;
                }
                
                if(eventReciver.Client == null)
                {
                    lbSOPServer.Text = "접속안됨";
                    lbSOPServer.ForeColor = Color.Red;
                }
                else
                {
                    if(eventReciver.Client.ClientProvider.IsConnected)
                    {                  
                        lbSOPServer.Text = "접속중";
                        lbSOPServer.ForeColor = Color.Green;
                    }
                    else
                    {
                        lbSOPServer.Text = "접속안됨";
                        lbSOPServer.ForeColor = Color.Red;
                    }
                }                
            }            
        }

        //int RTSPVoDIndex = -1;
        //public void SetRecordURL(DateTime time, string url, short port, string userName, string pass)
        //{
        //    this.Invoke((MethodInvoker)delegate
        //    {
        //        //if (RTSPVoDIndex != -1)
        //        //    axRTSPLiveScreen1.CloseRTSPLiveScreen(RTSPVoDIndex);

        //        DateTime _queryDateTime = time;
        //        TimeZone oTimeZone = TimeZone.CurrentTimeZone;
        //        TimeSpan oSpan = oTimeZone.GetUtcOffset(new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day));
        //        _queryDateTime = _queryDateTime.Subtract(oSpan);

        //        Console.WriteLine("After queryDateTime: " + _queryDateTime);
             
        //        this.VoDPlay(url, port, "", "", (ulong)_queryDateTime.Ticks, true, false);
        //    });    
        //}
        
        //private void ui_btnDeviceVoD_Click(object sender, EventArgs e)
        //{
        //    //lock (_lockObjectForVideoPlay)
        //    //{
               
        //   // }
        //}

        //private void VoDPlay(string serverURL, short portNumber, string userID, string userPassword, ulong startTick, bool isEncryption, bool isVoDPause)
        //{
        //    //lock (_lockObjectForVideoPlay)
        //    {
        //       // RTSPVoDIndex = axRTSPLiveScreen1.OpenRTSPVoDScreen(serverURL, portNumber, "", "", startTick, 1, 0);
        //    }

        //    string playInfo = "[VoD] → " + serverURL + " (port:" + portNumber.ToString() + ")";
        //}

        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            timer1.Stop();
            timer1.Enabled = false;           
            eventReciver.Dispose();
        }

        private int GetSensorTagInfoID(int nSensorZoneID)
        {
            string strSQL = "Select min(ID) from SensorTagInfo where SensorZoneID = " + nSensorZoneID.ToString();
            System.Collections.ArrayList arrResult = eventReciver.DBManager.GetResultData(strSQL, 0);

            if (arrResult == null || arrResult.Count == 0)
                return -1;

            DBUtility.VariousData<int> id = DBUtility.WebDBManager.GetIntField(arrResult[0].ToString());

            if (id == null)
                return -1;

            return id.Data;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if(eventReciver != null)
            {
                int nSensorZoneID = -1;
                string szSzID = textBox1.Text;
                if(int.TryParse(szSzID, out nSensorZoneID))
                {
                    SensorZone zone = eventReciver.IOManager.GetSensorZone(nSensorZoneID);
                    if(zone != null)
                    {
                        int nSensorType = (int)zone.Type;
                        int nData = 1;
                        int nSensorTagInfoID = GetSensorTagInfoID(nSensorZoneID);
                        eventReciver.Client.SendSensorData(nSensorZoneID, nSensorTagInfoID, nSensorType, nData, "", "1", -1);
                    }                    
                }                
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (eventReciver != null)
            {
                int nSensorZoneID = -1;
                string szSzID = textBox1.Text;
                if (int.TryParse(szSzID, out nSensorZoneID))
                {
                    SensorZone zone = eventReciver.IOManager.GetSensorZone(nSensorZoneID);
                    if( zone != null)
                    {
                        int nSensorType = (int)zone.Type;
                        int nData = 0;
                        int nSensorTagInfoID = GetSensorTagInfoID(nSensorZoneID);
                        eventReciver.Client.SendSensorData(nSensorZoneID, nSensorTagInfoID, nSensorType, nData, "", "1", -1);
                    }                    
                }
            }
        }


        private bool m_bFire = true;
        private bool m_bFence = true;

        private void ckbFire_CheckedChanged(object sender, EventArgs e)
        {
            if( ckbFire.Checked == true)
            {
                m_bFire = true;
                if (eventReciver != null)
                    eventReciver.ReciveFireSignal = m_bFire;
            }
            else
            {
                m_bFire = false;
                if (eventReciver != null)
                    eventReciver.ReciveFireSignal = m_bFire;
            }
        }

        private void ckbIntr_CheckedChanged(object sender, EventArgs e)
        {
            if (ckbIntr.Checked == true)
            {
                m_bFence = true;
                if (eventReciver != null)
                    eventReciver.ReciveFenceSignal = m_bFence;
            }
            else
            {
                m_bFence = false;
                if (eventReciver != null)
                    eventReciver.ReciveFenceSignal = m_bFence;
            }
        }        
#endif
    }

}
