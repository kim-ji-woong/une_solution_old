using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Threading;
//using DBUtility;
using System.IO;
using System.Collections;

namespace EarthquakeSensorServer
{
    public partial class FormMain : Form
    {
        private Socket m_udpSocket = null;
        private Thread m_listenThread = null;
        // private int m_nPortNo = 5000;
        private int m_nPortNo = 5000;
        private bool m_closing = false;

        private int m_nSiteID = 1;
        private Network.NetworkManager m_netMgr = null;
        //private WebDBManager m_dbMgr = null;

        //private int m_nLastRowIndex = -1;
        //private bool m_runTimer = false;
        private StreamWriter m_logger = null;

        private const int MAX_ROW_COUNT = 500;

        private EarthquakeData m_maxData = new EarthquakeData();

        private static FormMain m_instance = null;

        public static FormMain Instance
        {
            get { return m_instance; }
        }

        public FormMain()
        {
            InitializeComponent();
            labelErrorMessage.Text = "";
            labelErrorMessage.Visible = false;

            m_instance = this;

            m_logger = new StreamWriter(".\\EarthquakeSensorServer.log", true, Encoding.UTF8);

            ReadPortNo();

            ReadSiteID();
            RunThread();

            //m_dbMgr = new WebDBManager(m_nSiteID);
            m_netMgr = new Network.NetworkManager(m_nSiteID/*, m_dbMgr*/);
        }

        private void ReadPortNo()
        {
            string strFileName = "Port.txt";

            if (File.Exists(strFileName))
            {
                System.IO.StreamReader reader = new StreamReader(strFileName);
                string strLine = reader.ReadLine().Trim();
                reader.Close();

                int nPort = 0;

                if (int.TryParse(strLine, out nPort))
                {
                    m_nPortNo = nPort;
                }
            }
        }

        private void ReadSiteID()
        {
            /*DBUtility.Utility util = new DBUtility.Utility();
            string szSiteID = util.getinivalue("Server Connection Info", "siteid");
            if (szSiteID == null || szSiteID == "")
            {
                MessageBox.Show("Site ID가 지정되지 않았습니다. ini파일을 확인하세요", "실행 오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
                return;
            }

            if (!int.TryParse(szSiteID, out m_nSiteID))
            {
                MessageBox.Show("잘못된 Site ID입니다.. ini파일을 확인하세요", "실행 오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
                return;
            }*/
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            int nPortNo = m_nPortNo;

            if (int.TryParse(textBoxPortNo.Text, out nPortNo))
                m_nPortNo = nPortNo;

            //SetInternalMessagePopupOnOff();
            //SOPSMS.SetVipPhoneNumbers(m_nSiteID, m_dbMgr);

            m_listenThread = new Thread(new ParameterizedThreadStart(Listen));
            m_listenThread.Start(nPortNo);
        }

        private void SetInternalMessagePopupOnOff()
        {
            if (GetOptionSOPSimulatorBoolean("InternalMessagePopupSMSOnOff"))
                checkBoxInternalMessageSMSPopup.Checked = true;

            if (GetOptionSOPSimulatorBoolean("InternalMessagePopupBroadcastOnOff"))
                checkBoxInternalMessageBroadcastPopup.Checked = true;
        }

        public bool GetOptionSOPSimulatorBoolean(string strValueName)
        {
            /*string strSQL = "Select PropertyValue from OptionSOPSimulator where PropertyName = '" + strValueName + "' and SiteID = " + m_nSiteID.ToString();
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null || arrResult.Count == 0)
                return false;

            string strValue = WebDBManager.GetStringField(arrResult[0]);

            if (strValue == null)
                return false;

            if (strValue == "1")
                return true;*/

            return false;
        }

        public void SetSMSTime(DateTime dtSend)
        {
            string strTime = string.Format("{0}-{1:00}-{2:00} {3:00}:{4:00}:{5:00}", dtSend.Year, dtSend.Month, dtSend.Day, dtSend.Hour, dtSend.Minute, dtSend.Second);

            this.Invoke((MethodInvoker)delegate
            {
                labelSMSTime.Text = strTime;
            });
        }

        private void Listen(object arg)
        {
            int nPortNo = m_nPortNo;

            if (arg != null && arg is int)
                nPortNo = (int)arg;

            nPortNo = 9908;

            try
            {
                m_udpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                EndPoint localEP = new IPEndPoint(IPAddress.Any, nPortNo);
                EndPoint remoteEP = new IPEndPoint(IPAddress.None, nPortNo);

                m_udpSocket.Bind(localEP);

                byte[] receiveBuffer = new byte[512];

                try
                {
                    while (true)
                    {
                        // 기다리고 있다가 remoteEP 로부터 데이터를 받는다
                        // receivedSize  : 받은 바이트수
                        // receiveBuffer : 받은 데이터가 들어갈 저장소
                        // remoteEP      : 데이터를 받아올 원격컴퓨터의 IP종단점
                        int receivedSize = m_udpSocket.ReceiveFrom(receiveBuffer, ref remoteEP);
                        AddMessage(receiveBuffer, receivedSize);

                        // 받은 데이터(receiveBuffer)를 remoteEP 로 다시 보낸다
                        //m_udpSocket.SendTo(receiveBuffer, receivedSize, SocketFlags.None, remoteEP);
                    }
                }
                catch (SocketException se)
                {
                    ShowErrorMessage(se.Message);
                }
                finally
                {
                    m_udpSocket.Close();
                }
            }
            catch (SocketException se)
            {
                ShowErrorMessage(se.Message);
            }
        }

        private void WriteBytes(byte[] bytes, int nSize)
        {
            string str = "";

            for (int i=0;i<nSize;i++)
            {
                str += string.Format("{0:X2} ", bytes[i]);
            }

            m_logger.WriteLine(str);
        }

        private void AddMessage(byte[] bytes, int nSize)
        {
            if (m_closing)
                return;

            string strMessage = Encoding.ASCII.GetString(bytes, 0, nSize);
            //string strMessage = Encoding.UTF8.GetString(bytes, 0, nSize);
            /*string[] tokens = strMessage.Split(new char[]{'&'}, StringSplitOptions.RemoveEmptyEntries);

            int nTokenCount = tokens.Count();

            if (nTokenCount != 8)
            {
                System.Diagnostics.Trace.WriteLine("Unknown Message : " + strMessage);
                return;
            }else*/
            {
                System.Diagnostics.Trace.WriteLine("Recived Message : " + strMessage);
            }

            m_logger.WriteLine(strMessage);
            WriteBytes(bytes, nSize);
            m_logger.Flush();


            
            // 00000127Type=SEISMIC&DateTime=20110225081025&Station=192.168.10.101 SS_ES1&Level=1&Source=1&HPGA =0000.000123&TPGA =0000.000248&MMI=01

            try
            {
               // int nLenght = Convert.ToInt32(tokens[0]);
               
            }
            catch(Exception ex)
            { 
            }
            
            /*string szType = tokens[0].Replace("&","");
            string szTime = tokens[1].Replace("&", "").Replace("DateTime=", "");
            string szStation = tokens[2].Replace("&", "").Replace("Station=", "");
            string szLevel = tokens[3].Replace("&", "").Replace("Level=", "");
            string szSource = tokens[4].Replace("&", "").Replace("Source=", "");
            string szHPGA = tokens[5].Replace("&", "").Replace("HPGA=", "");
            string szTPGA = tokens[6].Replace("&", "").Replace("TPGA=", "");
            string szMMI = tokens[7].Replace("&", "").Replace("MMI=", "");
            
            this.Invoke((MethodInvoker)delegate
            {
                AddGrid(szTime, szStation, szLevel, szSource, szHPGA, szTPGA, szMMI);
            });*/
            ////this.Invoke((MethodInvoker)delegate
            //{
            //    AddGrid(tokens[0].Trim(), tokens[1].Trim(), tokens[2].Trim(), tokens[3].Trim(), tokens[4].Trim(), tokens[5].Trim());
            //});
        }

        private void AddGrid(string strTime, string strStation, int nIntensity, float fMagnitude)
        {
            if (nIntensity <= 0 && fMagnitude <= 0.0f)
                return;

            int nNo = dataGridView1.Rows.Count == 0 ? 1 : (int)dataGridView1.Rows[dataGridView1.Rows.Count - 1].Cells[0].Value + 1;//dataGridView1.Rows.Count + 1;

            DataGridViewRow row = new DataGridViewRow();

            DataGridViewTextBoxCell cell = new DataGridViewTextBoxCell();
            cell.Value = nNo;
            row.Cells.Add(cell);

            cell = new DataGridViewTextBoxCell();
            cell.Value = strStation;
            row.Cells.Add(cell);

            DateTime time = ToDateTime(strTime);
            cell = new DataGridViewTextBoxCell();
            cell.Value = time;
            row.Cells.Add(cell);

            cell = new DataGridViewTextBoxCell();
            cell.Value = "-";
            row.Cells.Add(cell);

            string strLevel = "1";
            m_maxData.SetData(nIntensity, fMagnitude, strStation, strLevel, time);

            cell = new DataGridViewTextBoxCell();
            cell.Value = nIntensity;
            row.Cells.Add(cell);

            cell = new DataGridViewTextBoxCell();
            cell.Value = strLevel;
            row.Cells.Add(cell);

            cell = new DataGridViewTextBoxCell();
            cell.Value = "-";
            row.Cells.Add(cell);

            cell = new DataGridViewTextBoxCell();
            cell.Value = "-";
            row.Cells.Add(cell);

            // MAX_ROW_COUNT 이상은 늘어나지 않도록 한다.
            if (dataGridView1.Rows.Count >= MAX_ROW_COUNT)
                dataGridView1.Rows.RemoveAt(0);

            int nRowIndex = dataGridView1.Rows.Add(row);
            dataGridView1.CurrentCell = row.Cells[0];

            row = dataGridView1.Rows[nRowIndex];
        }

        private void AddGrid(string strTime, string strStation, string strLevel, string strPGA, string strHPGA, string strTPGA, string szMMI)
        {
            int nNo = dataGridView1.Rows.Count == 0 ? 1 : (int)dataGridView1.Rows[dataGridView1.Rows.Count - 1].Cells[0].Value + 1;//dataGridView1.Rows.Count + 1;

            DataGridViewRow row = new DataGridViewRow();

            DataGridViewTextBoxCell cell = new DataGridViewTextBoxCell();
            cell.Value = nNo;
            row.Cells.Add(cell);

            cell = new DataGridViewTextBoxCell();
            cell.Value = strStation;
            row.Cells.Add(cell);

            DateTime time = ToDateTime(strTime);
            cell = new DataGridViewTextBoxCell();
            cell.Value = time;
            row.Cells.Add(cell);

            cell = new DataGridViewTextBoxCell();
            cell.Value = strPGA;
            row.Cells.Add(cell);

            string strGal = strPGA == "1" ? strTPGA : strHPGA;
            int nIntensity = GetIntensity(strGal);
            //System.Diagnostics.Trace.WriteLine("Intensity : " + nIntensity);
            try
            {
                string szTemp = szMMI.Replace("MMI=", "");
                nIntensity = Convert.ToInt32(szTemp);
            }
            catch(Exception ex)
            {
            }

            m_maxData.SetData(nIntensity, strStation, strLevel, time);
            
            cell = new DataGridViewTextBoxCell();
            cell.Value = nIntensity;
            row.Cells.Add(cell);

            cell = new DataGridViewTextBoxCell();
            cell.Value = strLevel;
            row.Cells.Add(cell);

            cell = new DataGridViewTextBoxCell();
            cell.Value = strHPGA;
            row.Cells.Add(cell);

            cell = new DataGridViewTextBoxCell();
            cell.Value = strTPGA;
            row.Cells.Add(cell);

            // MAX_ROW_COUNT 이상은 늘어나지 않도록 한다.
            if (dataGridView1.Rows.Count >= MAX_ROW_COUNT)
                dataGridView1.Rows.RemoveAt(0);

            int nRowIndex = dataGridView1.Rows.Add(row);
            dataGridView1.CurrentCell = row.Cells[0];

            row = dataGridView1.Rows[nRowIndex];

            //if (m_runTimer == false)
            {
                //m_nLastRowIndex = nRowIndex;
                //m_runTimer = true;
                //timer1.Start();
            }
            /*int nAlarmLevel;
            float fMagnitude = -1.0f;
            
            if (int.TryParse(strLevel, out nAlarmLevel))
            {
                //m_netMgr.SendEarthquakeSignal(1, fMagnitude, nAlarmLevel);
                m_netMgr.SendEarthquakeSignal(1, fMagnitude, nIntensity, nAlarmLevel, strStation, time);
            }*/
        }

        private void RunThread()
        {
            System.Threading.Thread t = new System.Threading.Thread(new System.Threading.ThreadStart(SendThread));
            t.Start();
        }

        private void SendThread()
        {
            while (m_closing == false)
            {
                if (m_maxData.IsChanged && m_maxData.Intensity > 1)
                {
                    CheckAfterSignal();

                    DateTime time = Convert.ToDateTime(m_maxData.TimeStamp);
                    m_netMgr.SendEarthquakeSignal(1, m_maxData.Magnitude, m_maxData.Intensity, m_maxData.AlarmLevel, m_maxData.Station, time);
                    System.Diagnostics.Trace.WriteLine("SendAlarm : " + m_maxData.Intensity.ToString() + ", " + m_maxData.AlarmLevel.ToString() + ", " + m_maxData.Station + ", " + time.ToString());

                    m_maxData.IsChanged = false;
                }

                System.Threading.Thread.Sleep(3000);
            }
        }

        // 여진인가?
        private void CheckAfterSignal()
        {
            string strAfter = checkBoxAfter.Checked ? "1" : "0";
            SetOptionSOPSimulatorValue("IsAfterQuake", strAfter, "여진인가?");
        }

        public bool IsAfterSignal()
        {
            return checkBoxAfter.Checked;
        }

        private int GetIntensity(string strGal)
        {
            float fGal = 0.0f;

            if (float.TryParse(strGal, out fGal))
            {
                if (fGal < 1.0f)
                    return 1;
                else if (fGal >= 1.0f && fGal < 2.5f)
                    return 2;
                else if (fGal >= 2.5f && fGal < 5.0f)
                    return 3;
                else if (fGal >= 5.0f && fGal < 10.0f)
                    return 4;
                else if (fGal >= 10.0f && fGal < 25.0f)
                    return 5;
                else if (fGal >= 25.0f && fGal < 50.0f)
                    return 6;
                else if (fGal >= 50.0f && fGal < 100.0f)
                    return 7;
                else if (fGal >= 100.0f && fGal < 250.0f)
                    return 8;
                else if (fGal >= 250.0f && fGal < 500.0f)
                    return 9;
                else if (fGal >= 500.0f && fGal < 750.0f)
                    return 10;
                else if (fGal >= 750.0f && fGal < 980.0f)
                    return 11;
                else if (fGal >= 980.0f)
                    return 12;
            }

            return -1;
        }

        private DateTime ToDateTime(string strTime)
        {
            if (strTime.Length == 14)
            {
                int nYear, nMonth, nDay, nHour, nMin, nSec;
                string strYear = strTime.Substring(0, 4);
                string strMonth = strTime.Substring(4, 2);
                string strDay = strTime.Substring(6, 2);
                string strHour = strTime.Substring(8, 2);
                string strMinute = strTime.Substring(10, 2);
                string strSecond = strTime.Substring(12, 2);

                if (int.TryParse(strYear, out nYear) && int.TryParse(strMonth, out nMonth) && int.TryParse(strDay, out nDay) &&
                    int.TryParse(strHour, out nHour) && int.TryParse(strMinute, out nMin) && int.TryParse(strSecond, out nSec))
                {
                    DateTime time = new DateTime(nYear, nMonth, nDay, nHour, nMin, nSec);
                    return time;
                }
            }

            return new DateTime();
        }

        private void ShowErrorMessage(string strMessage)
        {
            if (m_closing)
                return;

            this.Invoke((MethodInvoker)delegate
            {
                labelErrorMessage.Text = strMessage;
                labelErrorMessage.Visible = true;
            });
        }

        private void btnApply_Click(object sender, EventArgs e)
        {
            int nPortNo = m_nPortNo;

            if (int.TryParse(textBoxPortNo.Text, out nPortNo))
            {
                if (m_nPortNo != nPortNo)
                {
                    m_nPortNo = nPortNo;

                    if (m_listenThread != null)
                    {
                        m_udpSocket.Close();
                        m_listenThread.Interrupt();
                    }

                    m_listenThread = new Thread(new ParameterizedThreadStart(Listen));
                    m_listenThread.Start(nPortNo);
                }
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            m_closing = true;

            if (m_listenThread != null)
            {
                m_netMgr.ReleaseThread();
                m_udpSocket.Close();
                m_listenThread.Interrupt();
            }
        }

        private void btnSimpleInput_Click(object sender, EventArgs e)
        {
            FormSimpleData frm = new FormSimpleData();

            if (checkBoxAfter.Checked)
                frm.Magnitude = 6.0f;

            if (frm.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
            {
                DateTime dtNow = DateTime.Now;
                string strTime = string.Format("{0}{1:00}{2:00}{3:00}{4:00}{5:00}", dtNow.Year, dtNow.Month, dtNow.Day, dtNow.Hour, dtNow.Minute, dtNow.Second);
                AddGrid(strTime, frm.Location, frm.Intensity, frm.Magnitude);
                btnSirenOn_Click(null, null);
            }
        }

        public string GetSMSTag()
        {
            return textBoxSMSTag.Text.Trim();
        }

        private void checkBoxInternalMessagePopup_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox checkOption = (CheckBox)sender;
            string strOption = checkOption == checkBoxInternalMessageSMSPopup ? "InternalMessagePopupSMSOnOff" : "InternalMessagePopupBroadcastOnOff";
            string strValue = checkOption.Checked ? "1" : "0";

            if (checkOption == checkBoxInternalMessageBroadcastPopup)
            {
                if (checkOption.Checked)
                {
                    btnSirenOff_Click(null, null);
                    btnRunBroadcast.Enabled = false;
                }
            }

            SetOptionSOPSimulatorValue(strOption, strValue, "내부상황전파 메시지 On/Off");
            /*string strSQL = "Select ID from OptionSOPSimulator where PropertyName = '" + strOption + "' and SiteID = " + m_nSiteID.ToString();
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL, 0);

            int value = checkOption.Checked ? 1 : 0;
            int nID = 0;
            
            if (arrResult == null || arrResult.Count == 0)
            {
                strSQL = "Select max(ID) from OptionSOPSimulator";
                arrResult = m_dbMgr.GetResultData(strSQL, 0);

                if (arrResult == null || arrResult.Count == 0)
                    nID = 1;
                else
                {
                    nID = WebDBManager.GetIntField(arrResult[0].ToString(), -1) + 1;

                    if (nID < 0)
                        nID = 1;
                }

                strSQL = "Insert into OptionSOPSimulator (ID, PropertyName, PropertyValue, Description, SiteID) values (";
                strSQL += string.Format("{0}, '{1}', '{2}', '내부상황전파 메시지 On/Off', {3})", nID, strOption, value, m_nSiteID);
                m_dbMgr.GetResultData(strSQL, 0);
            }
            else
            {
                nID = WebDBManager.GetIntField(arrResult[0].ToString(), -1);

                if (nID > 0)
                {
                    strSQL = string.Format("Update OptionSOPSimulator set PropertyValue = '{0}' where ID = {1}", value, nID);
                    m_dbMgr.GetResultData(strSQL, 0);
                }
            }*/
        }

        private void SetOptionSOPSimulatorValue(string strPropertyName, string strPropertyValue, string strDescription)
        {
            /*string strSQL = "Select ID from OptionSOPSimulator where PropertyName = '" + strPropertyName + "' and SiteID = " + m_nSiteID.ToString();
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL, 0);

            int nID = 0;

            if (arrResult == null || arrResult.Count == 0)
            {
                strSQL = "Select max(ID) from OptionSOPSimulator";
                arrResult = m_dbMgr.GetResultData(strSQL, 0);

                if (arrResult == null || arrResult.Count == 0)
                    nID = 1;
                else
                {
                    nID = WebDBManager.GetIntField(arrResult[0].ToString(), -1) + 1;

                    if (nID < 0)
                        nID = 1;
                }

                strSQL = "Insert into OptionSOPSimulator (ID, PropertyName, PropertyValue, Description, SiteID) values (";
                strSQL += string.Format("{0}, '{1}', '{2}', '{3}', {4})", nID, strPropertyName, strPropertyValue, strDescription, m_nSiteID);
                m_dbMgr.GetResultData(strSQL, 0);
            }
            else
            {
                nID = WebDBManager.GetIntField(arrResult[0].ToString(), -1);

                if (nID > 0)
                {
                    strSQL = string.Format("Update OptionSOPSimulator set PropertyValue = '{0}' where ID = {1}", strPropertyValue, nID);
                    m_dbMgr.GetResultData(strSQL, 0);
                }
            }*/
        }

        private void btnSirenOn_Click(object sender, EventArgs e)
        {
            string szSQL = "UPDATE WarningLight SET CH1 = 1, CH2 = 1 WHERE ID = 609";
            //m_dbMgr.GetResultData(szSQL, 0);   
        }

        private void btnSirenOff_Click(object sender, EventArgs e)
        {
            string szSQL = "UPDATE WarningLight SET CH1 =0, CH2 = 0 WHERE ID = 609";
            //m_dbMgr.GetResultData(szSQL, 0);   
        }

        private void btnResetVIP_Click(object sender, EventArgs e)
        {
            //SOPSMS.SetVipPhoneNumbers(m_nSiteID, m_dbMgr);
        }

        private void btnRunBroadcast_Click(object sender, EventArgs e)
        {
            //SOPSMS.RunBroadcast(m_dbMgr, m_nSiteID);
        }

        public void EnableBroadcast(bool enabled)
        {
            this.Invoke((MethodInvoker)delegate
            {
                if (FormMain.Instance.GetOptionSOPSimulatorBoolean("UseBroadcast"))
                {
                    btnRunBroadcast.Enabled = enabled;
                }
            });
        }

        private void btnHomeView_Click(object sender, EventArgs e)
        {
            SetSDMSView();
        }

        private void SetSDMSView()
        {
            // 14호기 메인
            m_netMgr.SendSDMSView("Main", DateTime.Now);

            // TT-08화재
            // m_netMgr.SendSDMSView("Custom1", DateTime.Now);

            // 암모니아
            // m_netMgr.SendSDMSView("Custom2", DateTime.Now);

        }

        private void btnCollapseBuilding_Click(object sender, EventArgs e)
        {
            string strBuildingName = textBoxBuildingName.Text.Trim();

            if (strBuildingName.Length == 0)
            {
                textBoxBuildingName.Focus();
                MessageBox.Show("붕괴될 건물명을 입력하세요.");
                return;
            }

            bool isReal = false, finishEvent = false;
            ArrayList arrDatas = new ArrayList();

            arrDatas.Add(strBuildingName);
            arrDatas.Add(isReal);
            arrDatas.Add(finishEvent);

            byte[] bytes = TcpLib2.TcpHelper.MakeBytes(SDMS.TCP_ID.COLLAPSE_BUILDING_DETECT, arrDatas);
            m_netMgr.Send(bytes, m_netMgr.ClientProvier);
        }

        private void btnRecoverBuilding_Click(object sender, EventArgs e)
        {
            string strBuildingName = textBoxBuildingName.Text.Trim();

            if (strBuildingName.Length == 0)
            {
                textBoxBuildingName.Focus();
                MessageBox.Show("건물명을 입력하세요.");
                return;
            }

            bool isReal = false, finishEvent = true;
            ArrayList arrDatas = new ArrayList();

            arrDatas.Add(strBuildingName);
            arrDatas.Add(isReal);
            arrDatas.Add(finishEvent);

            byte[] bytes = TcpLib2.TcpHelper.MakeBytes(SDMS.TCP_ID.COLLAPSE_BUILDING_DETECT, arrDatas);
            m_netMgr.Send(bytes, m_netMgr.ClientProvier);
        }

        //private void timer1_Tick(object sender, EventArgs e)
        //{
        //    /*int nRowCount = dataGridView1.Rows.Count;
        //    string strMaxStation = "", strMaxTime = "";
        //    int nMaxAlarm = -1, nMaxIntensity = -1;

        //    for (int i=m_nLastRowIndex;i<nRowCount;i++)
        //    {
        //        DataGridViewRow row = dataGridView1.Rows[i];

        //        int nAlarmLevel;
        //        int nIntensity;

        //        if (int.TryParse(row.Cells[4].Value.ToString(), out nIntensity) && int.TryParse(row.Cells[5].Value.ToString(), out nAlarmLevel))
        //        {
        //            if (nMaxIntensity < nIntensity)
        //            {
        //                strMaxStation = row.Cells[1].Value.ToString();
        //                strMaxTime = row.Cells[2].Value.ToString();
        //                nMaxAlarm = nAlarmLevel;
        //                nMaxIntensity = nIntensity;
        //            }
        //        }
        //    }

        //    if (nMaxIntensity > 1)
        //    {
        //        //DateTime time = ToDateTime(strMaxTime);
        //        DateTime time = Convert.ToDateTime(strMaxTime);
        //        m_netMgr.SendEarthquakeSignal(1, -1.0f, nMaxIntensity, nMaxAlarm, strMaxStation, time);
        //        System.Diagnostics.Trace.WriteLine("SendAlarm : " + nMaxIntensity.ToString() + ", " + nMaxAlarm.ToString() + ", " + strMaxStation + ", " + time.ToString());
        //    }*/

        //    if (m_maxData.Intensity > 1)
        //    {
        //        DateTime time = Convert.ToDateTime(m_maxData.TimeStamp);
        //        m_netMgr.SendEarthquakeSignal(1, -1.0f, m_maxData.Intensity, m_maxData.AlarmLevel, m_maxData.Station, time);
        //        System.Diagnostics.Trace.WriteLine("SendAlarm : " + m_maxData.Intensity.ToString() + ", " + m_maxData.AlarmLevel.ToString() + ", " + m_maxData.Station + ", " + time.ToString());

        //        m_maxData.Intensity = 0;
        //    }

        //    //m_nLastRowIndex = nRowCount;
        //    //timer1.Stop();
        //    //m_runTimer = false;
        //}
    }

    public class EarthquakeData
    {
        private int m_nIntensity = 0;
        private float m_fMagnitude = -1.0f;
        private int m_nAlarmLevel = 0;
        private string m_strStation = "";
        private DateTime m_timeStamp = new DateTime();

        // 한번 지진 신호를 받으면 m_nSignalWaitingSeconds 동안 그보다 더 큰 신호가 오기 전까지
        // 다른 지진 신호들은 무시한다.
        private int m_nSignalWaitingSeconds = 40;
        private bool m_isChanged = false;
        
        public int Intensity
        {
            get { return m_nIntensity; }
            set { m_nIntensity = value; }
        }

        public float Magnitude
        {
            get { return m_fMagnitude; }
            set { m_fMagnitude = value; }
        }
        
        public int AlarmLevel
        {
            get { return m_nAlarmLevel; }
            set { m_nAlarmLevel = value; }
        }

        public string Station
        {
            get { return m_strStation; }
            set { m_strStation = value; }
        }

        public DateTime TimeStamp
        {
            get { return m_timeStamp; }
            set { m_timeStamp = value; }
        }

        public bool IsChanged
        {
            get { return m_isChanged; }
            set { m_isChanged = value; }
        }

        public void SetData(int nIntensity, string strStation, string strLevel, DateTime timeStamp)
        {
            if (m_nIntensity < nIntensity)
            {
                m_fMagnitude = -1.0f;
                ChangeData(nIntensity, strStation, strLevel, timeStamp);
            }
            else
            {
                TimeSpan span = timeStamp - m_timeStamp;

                if (span.TotalSeconds >= m_nSignalWaitingSeconds)
                {
                    m_fMagnitude = -1.0f;
                    ChangeData(nIntensity, strStation, strLevel, timeStamp);
                }
            }
        }

        public void SetData(int nIntensity, float fMagnitude, string strStation, string strLevel, DateTime timeStamp)
        {
            if (m_nIntensity < nIntensity)
            {
                m_fMagnitude = fMagnitude;
                ChangeData(nIntensity, strStation, strLevel, timeStamp);
            }
            else
            {
                TimeSpan span = timeStamp - m_timeStamp;

                if (span.TotalSeconds >= m_nSignalWaitingSeconds)
                {
                    m_fMagnitude = fMagnitude;
                    ChangeData(nIntensity, strStation, strLevel, timeStamp);
                }
            }
        }

        private void ChangeData(int nIntensity, string strStation, string strLevel, DateTime timeStamp)
        {
            m_nIntensity = nIntensity;
            m_strStation = strStation;
            m_timeStamp = timeStamp;

            int nAlarmLevel;

            if (int.TryParse(strLevel, out nAlarmLevel))
                this.AlarmLevel = nAlarmLevel;

            m_isChanged = true;
        }
    }
}
