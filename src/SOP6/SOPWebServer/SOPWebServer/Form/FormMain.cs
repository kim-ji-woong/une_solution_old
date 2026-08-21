using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.ServiceModel;
using DBUtility2;
using System.Collections;

namespace SOPWebServer
{
    public partial class FormMain : Form, IMainWindow
    {
        private ServiceHost m_serviceHost = null;
        private static FormMain m_instance = null;
        
        public FormMain()
        {
            InitializeComponent();
            m_instance = this;
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            InitServiceHost(ref m_serviceHost, this);
        }

        public static void InitServiceHost(ref ServiceHost serviceHost, IMainWindow mainWindow)
        {
            string strWebServerURL = "", strDBName = "", strID = "", strPW = "";
            int nDBType = 0;
            int nSiteID = SOPWebService.ReadSiteID(ref strWebServerURL, ref strDBName, ref nDBType, ref strID, ref strPW);
            
            PostOffice.Instance.MainWindow = mainWindow;
            PostOffice.Instance.Start(nSiteID, strWebServerURL, strDBName, nDBType, strID, strPW);

            serviceHost = new ServiceHost(typeof(PostBoxService));
            serviceHost.Open();

            SaveServicePort(serviceHost, nSiteID, strWebServerURL, strDBName, nDBType, strID, strPW);

            FormMain.m_instance.Text = "SOPWebServer SiteID : " + nSiteID + ", WebServerURL : " + strWebServerURL + ", DBName : " + strDBName;
        }

        private static int GetPort(string strURL)
        {
            int nIndex = strURL.LastIndexOf(':');

            if (nIndex < 0)
                return 0;

            int nIndex2 = strURL.IndexOf('/', nIndex + 1);

            string strPort = "";

            if (nIndex2 < 0)
                strPort = strURL.Substring(nIndex + 1).Trim();
            else
                strPort = strURL.Substring(nIndex + 1, nIndex2 - nIndex - 1);

            int nPort = 0;

            if (int.TryParse(strPort, out nPort) == false)
                nPort = 80;

            return nPort;
        }

        private static void SaveServicePort(ServiceHost host, int nSiteID, string strWebServerURL, string strDBName, int nDBType, string strID, string strPW)
        {
            DirectDBManager dbMgr = DirectDBManager.MakeInstance((DirectDBManager.DBType)nDBType, strWebServerURL, strID, strPW, strDBName);
            dbMgr.SiteID = nSiteID;
            //WebDBManager dbMgr = new WebDBManager(nSiteID);
            
            if (dbMgr.Connect() == false)
                return;

            foreach (System.ServiceModel.Description.ServiceEndpoint ep in host.Description.Endpoints)
            {
                string strURL = ep.Address.ToString().ToLower();
                int nPort = GetPort(strURL);

                if (strURL.EndsWith("mex"))
                    SaveServerPort(dbMgr, nPort, ServerPort.SOP_WEB_SERVER_MEX);
                else
                    SaveServerPort(dbMgr, nPort, ServerPort.SOP_WEB_SERVER);
            }

            dbMgr.Close();
        }

        private static void SaveServerPort(DirectDBManager dbMgr/*WebDBManager dbMgr*/, int nPort, string strPortName)
        {
            string strSQL = "Select Port from SensorServerPort where Name = '" + strPortName + "' and SiteID = " + dbMgr.SiteID.ToString();
            ArrayList arrResult = dbMgr.GetResultData(strSQL);

            if (arrResult == null || arrResult.Count == 0)
            {
                strSQL = string.Format("Insert into SensorServerPort (Port, SiteID, Name) values ({0}, {1}, '{2}')",
                    nPort, dbMgr.SiteID, strPortName);
                dbMgr.GetResultData(strSQL);
            }
            else
            {
                strSQL = string.Format("Update SensorServerPort set Port = {0} where Name = '{1}' and SiteID = {2}",
                    nPort, strPortName, dbMgr.SiteID);
                dbMgr.GetResultData(strSQL);
            }
        }

        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            CloseService(m_serviceHost);
        }

        public static void CloseService(ServiceHost host)
        {
            if (host != null)
            {
                PostOffice.Instance.Stop();
                host.Close();
            }
        }

        /*private int ReadSiteID(ref string strWebServerURL, ref string strDBName, ref int nDBType, ref string strID, ref string strPW)
        {
            DBUtility2.Utility util = new DBUtility2.Utility();
            string strValue = util.getinivalue("Server Connection Info", "siteid");

            int nSiteID = 1;

            if (strValue == null || strValue.Length == 0)
                return nSiteID;

            int.TryParse(strValue, out nSiteID);
            return nSiteID;
        }*/

        private void btnShowAlarms_Click(object sender, EventArgs e)
        {
            List<AgentFactory.AlarmData> alarms = ServerProcess.Data.AlarmManager.Instance.CurrentAlarms;
            int nAlarmIndex = 0;

            string str = "";

            foreach (AgentFactory.AlarmData alarm in alarms)
            {
                string strAlarm = string.Format("[Alarm_{0}]\r\n", ++nAlarmIndex);
                strAlarm += string.Format("SensorZoneHistoryID : {0}\r\n", alarm.SensorZoneHistoryID);
                strAlarm += string.Format("SensorReactionHistoryID : {0}\r\n", alarm.SensorReactionHistoryID);
                strAlarm += string.Format("SensorZoneID : {0}\r\n", alarm.SensorZoneID);
                strAlarm += string.Format("Alarm Depth : {0}\r\n", alarm.AlarmDepth);
                strAlarm += string.Format("Status : {0}\r\n", alarm.Status.ToString());
                strAlarm += string.Format("Message : {0}\r\n", alarm.Message);

                str += strAlarm;
            }

            textBoxResult.Text = str;
        }

        public void AddClient(int nClientType, int nClientSubType, string strIP, int nPort)
        {
            try
            {
                int nRowIndex = gridClients.Rows.Add();

                if (nRowIndex < 0)
                    return;

                DataGridViewRow row = gridClients.Rows[nRowIndex];

                row.Cells[0].Value = nRowIndex + 1;
                row.Cells[0].Tag = nPort;
                row.Cells[1].Value = SOPWebServer.ClientType.ToString(nClientType);
                row.Cells[1].Tag = nClientType;
                row.Cells[2].Value = SOPWebServer.ClientSubType.ToString(nClientSubType);
                row.Cells[2].Tag = nClientSubType;
                row.Cells[3].Value = string.Format("{0}:{1}", strIP, nPort);
                row.Cells[3].Tag = strIP;
            }
            catch (Exception e)
            {
                System.Diagnostics.Trace.WriteLine("FormMain.AddClient Error : " + e.Message);
            }
        }

        public void RemoveClient(string strIP, int nPort)
        {
            string strConnection = string.Format("{0}:{1}", strIP, nPort);

            this.Invoke((MethodInvoker)delegate
            {
                int nRowCount = gridClients.Rows.Count;
                int nIndex = -1;

                try
                {
                    for (int i = 0; i < nRowCount; i++)
                    {
                        DataGridViewRow row = gridClients.Rows[i];

                        if (row.Cells[3].Value.ToString() == strConnection)
                        {
                            gridClients.Rows.RemoveAt(i);
                            nIndex = i;
                            nRowCount--;
                        }
                    }

                    for (int i = nIndex; i < nRowCount; i++)
                    {
                        DataGridViewRow row = gridClients.Rows[i];
                        row.Cells[0].Value = i + 1;
                    }
                }
                catch (Exception e)
                {
                    System.Diagnostics.Trace.WriteLine("FormMain.RemoveClient Error : " + e.Message);
                }
            });
        }

        private void gridClients_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                DataGridView.HitTestInfo hitInfo = gridClients.HitTest(e.X, e.Y);

                if (hitInfo.RowIndex >= 0 && hitInfo.ColumnIndex >= 0)
                {
                    DataGridViewRow row = gridClients.Rows[hitInfo.RowIndex];
                    popupMenuClients.Tag = row;
                    popupMenuClients.Show(gridClients, e.Location);
                }
            }
        }

        private void tsMenuCloseClient_Click(object sender, EventArgs e)
        {
            DataGridViewRow row = (DataGridViewRow)popupMenuClients.Tag;

            if (row == null)
                return;

            int nPort = (int)row.Cells[0].Tag;
            int nClientType = (int)row.Cells[1].Tag;
            int nClientSubType = (int)row.Cells[2].Tag;
            string strIP = (string)row.Cells[3].Tag;

            PostOffice.Instance.RemoveClient(nClientType, nClientSubType, strIP, nPort);
        }

        private void btnShowSOPControl_Click(object sender, EventArgs e)
        {
            /*ServerProcess.Client.SOPSimulatorServer.ClientData data = ServerProcess.Client.SOPSimulatorServer.Instance.ControlClient;

            if (data == null)
            {
                textBoxResult.Text = "제어권 가진 Client 없음";
            }
            else
            {
                string str = string.Format("제어권 가진 Client : {0}:{1}", data.IP, data.Port);
                textBoxResult.Text = str;
            }*/
        }
    }
}
