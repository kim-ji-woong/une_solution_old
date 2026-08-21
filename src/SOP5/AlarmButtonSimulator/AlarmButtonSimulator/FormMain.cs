using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DBUtility;
using UnE.Sensor;
using UnE.Spatial;
using System.IO.Ports;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Collections;

namespace AlarmButtonSimulator
{
    public partial class FormMain : Form
    {
        private WebDBManager m_dbMgr = new WebDBManager(2);
        private DataManager m_dataMgr = null;
        private System.IO.Ports.SerialPort SP;
        private string m_strSensorTagNoHeader = "";
        private string m_strSensorTypeHeader = "";
        private string m_strSensorTagNameHeader = "";

        private FacilityManagerGroup m_facilityManagerGroup = null;
        private Circuit m_selectedSensor = null;
        private string m_strSMSCaller = "";
        private bool m_isSimulationMode = false;

        private bool m_changedMessage = false;
        private bool m_systemInput = false;

        // 각 센서(태그)별로 가장 마지막에 발생한 알람시간
        private Dictionary<Circuit, DateTime> m_dicSensorTagTime = new Dictionary<Circuit, DateTime>();

        private static FormMain m_instance = null;

        private event SerialDataReceivedEventHandler handler;

        NetworkManager networkManager = null;

        public static FormMain Instance
        {
            get { return m_instance; }
        }

        public WebDBManager DBManager
        {
            get { return m_dbMgr; }
        }

        public int SiteID
        {
            get { return 2; }
        }

        public FormMain()
        {
            CheckForIllegalCrossThreadCalls = false;
            m_instance = this;
            InitializeComponent();
            m_dataMgr = new DataManager(m_dbMgr, 2);
            networkManager = new NetworkManager();
            initAlarmComPort();
        }
        
        private void FormMain_Load(object sender, EventArgs e)
        {
            m_strSensorTagNoHeader = labelSensorTagID.Text;
            m_strSensorTypeHeader = labelSensorTagType.Text;
            m_strSensorTagNameHeader = labelSensorName.Text;

            InitSensorTagTree("");
            InitDatas();
            
        }
        private void initAlarmComPort()
        {
            cmbPort.BeginUpdate();
            foreach (string comport in SerialPort.GetPortNames())
            {
                cmbPort.Items.Add(comport);
            }
            cmbPort.EndUpdate();

            this.SP = new System.IO.Ports.SerialPort("COM1");
            SP.PortName = "COM1";
            SP.BaudRate = (int)9600;
            SP.DataBits = (int)8;
            SP.Parity = Parity.None;
            SP.StopBits = StopBits.One;
            SP.Handshake = Handshake.None;
            SP.RtsEnable = true;
        }
        private void InitDatas()
        {
            m_systemInput = true;
            XMLManager mgr = new XMLManager();

            if (mgr.Read() == false)
                return;

            if (mgr.SelectedSensor != null)
            {
                Circuit circuit = m_dataMgr.GetSensorTag(mgr.SelectedSensor.ID);

                if (circuit != null)
                {
                    labelSensorTagID.Text = m_strSensorTagNoHeader + string.Format("{0:00}-{1:00}-{2}-{3:000}",
                        circuit.ReciverID,
                        (circuit.TagNum / 10000) % 100,
                        (circuit.TagNum % 10000) / 1000,
                        circuit.TagNum % 1000);

                    labelSensorTagType.Text = m_strSensorTypeHeader + IFacility.GetFacilityTypeString(circuit.SensorType);
                    labelSensorName.Text = m_strSensorTagNameHeader + circuit.Name;
                }

                m_selectedSensor = circuit;
            }

            for (int i=0;i<3;i++)
            {
                ButtonOption option = mgr.GetButtonOption(i);

                if (option == null)
                    continue;

                if (i == 0)
                {
                    checkBoxSMS1.Checked = option.UseSMS;
                    checkBoxBroadcast1.Checked = option.UseBroadcast;
                }
                else if (i == 1)
                {
                    checkBoxSMS2.Checked = option.UseSMS;
                    checkBoxBroadcast2.Checked = option.UseBroadcast;
                }
                else if (i == 2)
                {
                    checkBoxSMS3.Checked = option.UseSMS;
                    checkBoxBroadcast3.Checked = option.UseBroadcast;
                }
            }

            checkBoxBroadcastSiren.Checked = mgr.BroadcastSiren;

            textBoxSMSMessage.Text = mgr.SMSMessage;
            textBoxBroadcastMessage.Text = mgr.BroadcastMessage;

            if (mgr.FacilityManagerGroup != null)
            {
                SetFacilityManagerGroup(mgr.FacilityManagerGroup);
            }

            m_strSMSCaller = mgr.SMSCaller;
            m_isSimulationMode = mgr.IsSimulationMode;

            m_systemInput = false;
        }

        private void InitSensorTagTree(string strSearchWord)
        {
            treeSensorTag.Nodes.Clear();

            treeSensorTag.Visible = false;
            m_dataMgr.MakeSensorTagTree(treeSensorTag, strSearchWord);
            treeSensorTag.Visible = true;

            if (treeSensorTag.Nodes.Count > 0)
                treeSensorTag.SelectedNode = treeSensorTag.Nodes[0];
        }

        private void treeSensorTag_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node != null && e.Node.Tag != null && e.Node.Tag is Circuit)
            {
                Circuit tag = (Circuit)e.Node.Tag;

                labelSensorTagID.Text = m_strSensorTagNoHeader + string.Format("{0:00}-{1:00}-{2}-{3:000}",
                    tag.ReciverID,
                    (tag.TagNum / 10000) % 100,
                    (tag.TagNum % 10000) / 1000,
                    tag.TagNum % 1000);

                labelSensorTagType.Text = m_strSensorTypeHeader + IFacility.GetFacilityTypeString(tag.SensorType);
                labelSensorName.Text = m_strSensorTagNameHeader + tag.Name;

                m_selectedSensor = tag;
                SaveConfig();
            }
            /*else
            {
                labelSensorTagID.Text = m_strSensorTagNoHeader;
                labelSensorTagType.Text = m_strSensorTypeHeader;
                labelSensorName.Text = m_strSensorTagNameHeader;
            }*/
        }

        private void txtSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                btnSearch.PerformClick();
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            RecreateSensorTree();
        }

        private void RecreateSensorTree()
        {
            string strSearchText = this.txtSearch.Text;

            if (treeSensorTag != null)
                treeSensorTag.Visible = false;

            InitSensorTagTree(strSearchText.Trim());

            if (treeSensorTag != null)
                treeSensorTag.Visible = true;
        }

        private void btnSMSReceivers_Click(object sender, EventArgs e)
        {
            FormEditManager mgr = new FormEditManager(IFacility.FacilityType.FIRE_SENSOR, m_facilityManagerGroup);
            
            if (mgr.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
            {
                FacilityManagerGroup group = mgr.GetFacilityManagerGroup();
                SetFacilityManagerGroup(group);

                SaveConfig();
            }
        }

        private void SetFacilityManagerGroup(FacilityManagerGroup group)
        {
            m_facilityManagerGroup = group;

            if (m_facilityManagerGroup == null)
                textBoxSMSReceivers.Text = "";
            else
            {
                string strManager = "";
                string strPhoneNumber = "";

                foreach (FacilityManager mgr in m_facilityManagerGroup.CompanyMembers)
                {
                    DataCompanyMember member = DataManager.Instance.GetCompanyMember(mgr.MemberID);

                    if (member == null)
                        continue;

                    if (strManager.Length == 0)
                        strManager = member.MemberName;
                    else
                        strManager += ";" + member.MemberName;
                }

                foreach (FacilityManager mgr in m_facilityManagerGroup.ExternalCompanyMembers)
                {
                    DataExternalMember member = DataManager.Instance.GetExternalMember(mgr.MemberID);

                    if (member == null)
                        continue;

                    if (strManager.Length == 0)
                        strManager = member.Name;
                    else
                        strManager += ";" + member.Name;
                }

                foreach (FacilityManager mgr in m_facilityManagerGroup.RegularTeams)
                {
                    DataTeam team = DataManager.Instance.GetRegularTeam(mgr.MemberID);

                    if (team == null)
                        continue;

                    if (strManager.Length == 0)
                        strManager = team.TeamName;
                    else
                        strManager += ";" + team.TeamName;
                }

                foreach (FacilityManager mgr in m_facilityManagerGroup.ExternalTeams)
                {
                    DataTeam team = DataManager.Instance.GetExternalTeam(mgr.MemberID);

                    if (team == null)
                        continue;

                    if (strManager.Length == 0)
                        strManager = team.TeamName;
                    else
                        strManager += ";" + team.TeamName;
                }

                foreach (FacilityManager mgr in m_facilityManagerGroup.ControlRoomMembers)
                {
                    DataTeamControlRoom team = (DataTeamControlRoom)mgr.Tag;

                    if (team == null)
                        continue;

                    if (strManager.Length == 0)
                        strManager = team.TeamName;
                    else
                        strManager += ";" + team.TeamName;
                }

                textBoxSMSReceivers.Text = strManager;
            }
        }

        private void SaveConfig()
        {
            XMLManager mgr = new XMLManager();
            mgr.Write(m_facilityManagerGroup, m_selectedSensor, checkBoxSMS1.Checked, checkBoxBroadcast1.Checked, checkBoxSMS2.Checked, checkBoxBroadcast2.Checked, checkBoxSMS3.Checked, checkBoxBroadcast3.Checked, textBoxSMSMessage.Text.Trim(), textBoxBroadcastMessage.Text.Trim(), checkBoxBroadcastSiren.Checked);
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            m_selectedSensor = null;

            labelSensorTagID.Text = m_strSensorTagNoHeader;
            labelSensorTagType.Text = m_strSensorTypeHeader;
            labelSensorName.Text = m_strSensorTagNameHeader;

            SaveConfig();
        }

        private void checkBox_CheckedChanged(object sender, EventArgs e)
        {
            if (m_systemInput)
                return;

            SaveConfig();
        }

        private void textBoxMessage_TextChanged(object sender, EventArgs e)
        {
            if (m_systemInput)
                return;

            m_changedMessage = true;
        }

        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (m_changedMessage)
                SaveConfig();
        }

        // Return 값 : 수신 전화번호 리스트
        public List<string> GetMessageInfo(int nIndex, out bool useSMS, out bool useBroadcast, out bool useBroadcastSiren, out string strSMSMessage, out string strBroadcastMessage, out string strSMSCaller)
        {
            List<string> phoneNumbers = null;

            useSMS = useBroadcast = false;
            strSMSMessage = strBroadcastMessage = strSMSCaller = "";
            useBroadcastSiren = checkBoxBroadcastSiren.Checked;

            if (m_isSimulationMode)
            {
                if (m_selectedSensor == null)
                    return phoneNumbers;
            }
            else
            {
                DateTime timeStamp = new DateTime();
                Circuit sensor = ReadCurrentAlarm(ref timeStamp);

                if (sensor == null)
                    return phoneNumbers;

                DateTime dtPrev;

                if (m_dicSensorTagTime.TryGetValue(sensor, out dtPrev))
                {
                    // 가장 마지막에 읽었던 알람신호보다 더 이전 알람이면 무시한다.
                    if (dtPrev >= timeStamp)
                        return phoneNumbers;
                }

                m_dicSensorTagTime[sensor] = timeStamp;
                m_selectedSensor = sensor;
            }

            FireSensor sensorZone = DataManager.Instance.GetSensorZone(m_selectedSensor.SensorZoneID);

            if (sensorZone == null)
                return phoneNumbers;

            EquipmentZone equipZone = DataManager.Instance.GetEquipZone(sensorZone.EquipZoneID);

            if (equipZone == null)
                return phoneNumbers;

            if (nIndex == 1)
            {
                useSMS = checkBoxSMS1.Checked;
                useBroadcast = checkBoxBroadcast1.Checked;
            }
            else if (nIndex == 2)
            {
                useSMS = checkBoxSMS2.Checked;
                useBroadcast = checkBoxBroadcast2.Checked;
            }
            else if (nIndex == 3)
            {
                useSMS = checkBoxSMS3.Checked;
                useBroadcast = checkBoxBroadcast3.Checked;
            }

            string strLocationTag = "{location}";

            if (useSMS)
            {
                strSMSCaller = m_strSMSCaller;
                strSMSMessage = GetMessage(textBoxSMSMessage.Text.Trim(), equipZone, strLocationTag);

                if (m_facilityManagerGroup != null)
                {
                    Dictionary<string, string> dicPhoneNumbers = new Dictionary<string, string>();
                    m_dataMgr.AddPhoneNumberFromGroup(dicPhoneNumbers, m_facilityManagerGroup);
                    phoneNumbers = dicPhoneNumbers.Values.ToList();
                }
            }

            if (useBroadcast)
            {
                strBroadcastMessage = GetMessage(textBoxBroadcastMessage.Text.Trim(), equipZone, strLocationTag);
            }

            return phoneNumbers;
        }

        private Circuit ReadCurrentAlarm(ref DateTime timeStamp)
        {
            string szText = "SELECT srh.id, srh.SensorHistoryID, srh.ReactionType, srh.Time, srh.Message, srh.Param1, srh.Param2, srh.Param3, srh.Param4, srh.Param5, szh.SensorID FROM SensorReactionHistory as srh ";
            szText += "INNER JOIN  SensorZoneHistory as szh on srh.SensorHistoryID = szh.ID ";
            szText += "WHERE SensorHistoryID in (  SELECT srh2.SensorHistoryID FROM SensorReactionHistory as srh2 WHERE srh2.ReactionType in ( 0, 60, 62, 898, 899, 921, 961 ) ) ";
            szText += " AND SensorHistoryID not in (  SELECT srh3.SensorHistoryID FROM SensorReactionHistory as srh3 WHERE srh3.ReactionType in (21, 23, 33, 50, 70, 919, 920,939,940 )) ";
            szText += " AND szh.SiteID = " + SiteID.ToString();
            szText += " ORDER BY srh.Time, szh.SensorID";

            string strSQL = string.Format(szText, SiteID);

            ArrayList arrResult = m_dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null)
                return null;

            int nResultCount = arrResult.Count;

            DateTime dtDefault = new DateTime();
            DateTime dtNow = DateTime.Now;
            DateTime dt24 = dtNow.AddHours(-24.0);

            SortedList<int, int> keyExistList = new SortedList<int, int>();
            int nSensorZoneID = -1;
            bool isSuccess;
            Dictionary<int, DateTime> dicSensorZoneIDs = new Dictionary<int, DateTime>();

            for (int i = 0; i < nResultCount - 10; i += 11)
            {
                int nID = DBUtility.WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                int nHistoryID = DBUtility.WebDBManager.GetIntField(arrResult[i + 1].ToString(), -1);
                int nReactionType = DBUtility.WebDBManager.GetIntField(arrResult[i + 2].ToString(), -1);
                DateTime time = DBUtility.WebDBManager.GetDateTimeField(arrResult[i + 3], dtDefault);
                string strMessage = DBUtility.WebDBManager.GetStringField(arrResult[i + 4], "");
                string strParam1 = DBUtility.WebDBManager.GetStringField(arrResult[i + 5], "");
                string strParam2 = DBUtility.WebDBManager.GetStringField(arrResult[i + 6], "");
                string strParam3 = DBUtility.WebDBManager.GetStringField(arrResult[i + 7], "");
                string strParam4 = DBUtility.WebDBManager.GetStringField(arrResult[i + 8], "");
                string strParam5 = DBUtility.WebDBManager.GetStringField(arrResult[i + 9], "");

                if (time < dt24)
                {
                    continue;
                }

                nSensorZoneID = DBUtility.WebDBManager.GetIntField(arrResult[i + 10].ToString(), -1);

                if (nReactionType == (int)libSensorProcess.ReactionType.BEGIN_PSM_STATUS || nReactionType == (int)libSensorProcess.ReactionType.CHANGE_PSM_ALARM_DEPTH)
                {
                    nSensorZoneID = DBUtility.WebDBManager.GetIntField(arrResult[i + 6].ToString(), -1);
                }

                if (nID < 0 || nHistoryID < 0)
                    continue;

                string szHashKey = nHistoryID.ToString() + "_-_" + nReactionType.ToString() + "_-_" + strMessage;
                int nHash = szHashKey.GetHashCode();
                if (keyExistList.ContainsKey(nHash))
                    continue;

                keyExistList.Add(nHash, nHash);

                libSensorProcess.ReactionType type = ToReactionType(nReactionType, out isSuccess);

                if (type == libSensorProcess.ReactionType.SEND_SMS || type == libSensorProcess.ReactionType.RUN_BROADCAST)
                    continue;

                if (!isSuccess)
                    continue;

                // 화학물질 센서는 통합처리되므로 data가 같은 SensorZone이므로 각기 SensorZone의 Data를 확인하도록 한다.
                // skkim 2016-02-26 
                string szText2 = "SELECT Data FROM SensorZone WHERE ID = {0}";
                string szSQL2 = string.Format(szText2, nSensorZoneID);
                ArrayList arrResult2 = m_dbMgr.GetResultData(szSQL2, 0);
                if (arrResult2 == null || arrResult2.Count == 0)
                    continue;

                int nSensorData = DBUtility.WebDBManager.GetIntField(arrResult2[0].ToString(), -1);
                if (nSensorData == 1 || nSensorData == 21 || nSensorData == 22 || nSensorData == 23)
                {
                    dicSensorZoneIDs[nSensorZoneID] = time;
                }
            }

            if (dicSensorZoneIDs.Count == 0)
                return null;

            DBUtility.VariousData<DateTime> dtLast = null;

            // 가장 마지막에 발생한 알람만 읽어온다.
            foreach (KeyValuePair<int, DateTime> pair in dicSensorZoneIDs)
            {
                if (dtLast == null)
                {
                    nSensorZoneID = pair.Key;
                    dtLast = new VariousData<DateTime>(pair.Value);
                }
                else if (dtLast.Data < pair.Value)
                {
                    nSensorZoneID = pair.Key;
                    dtLast.Data = pair.Value;
                }
            }

            timeStamp = dtLast.Data;
            return DataManager.Instance.GetSensorTagBySensorZoneID(nSensorZoneID);
        }

        private static Dictionary<int, libSensorProcess.ReactionType> m_dicReactionType = null;
        private static libSensorProcess.ReactionType ToReactionType(int nType, out bool isSuccess)
        {
            isSuccess = true;

            if (m_dicReactionType == null)
            {
                m_dicReactionType = new Dictionary<int, libSensorProcess.ReactionType>();

                foreach (libSensorProcess.ReactionType type in Enum.GetValues(typeof(libSensorProcess.ReactionType)))
                {
                    m_dicReactionType[(int)type] = type;
                }
            }

            libSensorProcess.ReactionType fType;

            if (m_dicReactionType.TryGetValue(nType, out fType))
                return fType;

            isSuccess = false;
            return libSensorProcess.ReactionType.ETC;
        }

        private string GetMessage(string strOrigin, EquipmentZone equipZone, string strLocationTag)
        {
            string strLower = strOrigin.ToLower();

            int nTagIndex = -1;

            do
            {
                nTagIndex = strLower.IndexOf(strLocationTag);

                if (nTagIndex >= 0)
                {
                    strLower = strLower.Substring(0, nTagIndex) + equipZone.ZoneName + strLower.Substring(nTagIndex + strLocationTag.Length);
                    strOrigin = strOrigin.Substring(0, nTagIndex) + equipZone.ZoneName + strOrigin.Substring(nTagIndex + strLocationTag.Length);
                }
            }
            while (nTagIndex >= 0);

            return strOrigin;
        }
        private void SP_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                SerialPort sp = (SerialPort)sender;
                lock (sp)
                {
                    if (sp.IsOpen && sp.BytesToRead > 0)
                    {
                        //usb converter로 연결해서인지 데이터 버퍼 수신이 이상함.
                        // string data = SP.ReadExisting();
                        int readable = sp.BytesToRead;
                        Stopwatch sw = new Stopwatch();
                        sw.Start();
                        while (readable <= 19)
                        {
                            readable = sp.BytesToRead;
                            if (sw.ElapsedMilliseconds == 2000)
                            {
                                break;
                            }
                        }
                        Debug.WriteLine("Buffer Read Size = " + readable);
                        //if (readable < 19) throw new IOException("Data 수신 이상! - " + readable);

                        byte[] buffer = new byte[readable];
                        //There is no accurate method for checking how many bytes are read 
                        //unless you check the return from the Read method 

                        int bytesRead = sp.Read(buffer, 0, buffer.Length);


                        if (buffer[0] == 0x02)          //stx
                        {
                            ////For the example assume the data we are received is ASCII data. 
                            string keycode = Encoding.ASCII.GetString(buffer, 1, 2);

                            if (keycode.Equals("14"))        //1개의 벨 스위치일때. 현재 비상벨 스위치 타입
                            {
                                string displayID = Encoding.ASCII.GetString(buffer, 11, 3).Trim();
                                int displayNum = int.Parse(displayID);

                                String bellChipID = Encoding.ASCII.GetString(buffer, 4, 6);
                                Debug.WriteLine(bellChipID);
                                
                                //1번 비상벨 "5270C6"
                                //2번 비상벨,316BFD
                                //3번 비상벨, C76576

                                networkManager.OnReceive(displayNum);                               
                            }
                        }
                    }
                    else
                    {
                        throw new IOException("Data 수신 이상! Not Open");
                    }
                }

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                Thread.Sleep(1000);
            }
            finally
            {

                if (handler != null)
                    this.SP.DataReceived -= handler;
                SP.Close();
                Thread.Sleep(500);
                ConnectComPort();
            }

        }
        private void ConnectComPort()
        {
            handler = new System.IO.Ports.SerialDataReceivedEventHandler(this.SP_DataReceived);
            this.SP.DataReceived += handler;
            SP.Open();
            if (SP.IsOpen)
            {
                rbText.Text = string.Format("{0}{1}", rbText.Text, "\r\n[Success] Port Open!!");
                rbText.Text = "[" + SP.PortName.ToString() + "] Port Open Connect!!";
                lbStatus.Text = "Connect!!";
                btnOpen.Visible = false;
                btnPortClose.Visible = true;
            }
            else
            {
                rbText.Text = string.Format("{0}{1}", rbText.Text, "\r\n[Fail] Port Open!!");
                rbText.Text = "[" + SP.PortName.ToString() + "] Port Open Failed!";
                lbStatus.Text = "[Fail] Port Open!";
                lbStatus.ForeColor = Color.Red;
            }
        }

        private void btnOpen_Click(object sender, EventArgs e)
        {
            ConnectComPort();
        }

        private void cmbDataBits_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (cmbDataBits.SelectedIndex)
            {
                case 0:
                    SP.DataBits = 8;
                    break;
                case 1:
                    SP.DataBits = 7;
                    break;
                default:
                    SP.DataBits = 8;
                    break;
            }
        }

        private void cmbPort_SelectedIndexChanged(object sender, EventArgs e)
        {
            SP.PortName = cmbPort.SelectedItem.ToString();
        }

        private void cmbBRate_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (cmbBRate.SelectedIndex)
            {
                case 0:
                    SP.BaudRate = (int)9600;
                    break;
                case 1:
                    SP.BaudRate = (int)14400;
                    break;
                case 2:
                    SP.BaudRate = (int)19200;
                    break;
                case 3:
                    SP.BaudRate = (int)38400;
                    break;
                case 4:
                    SP.BaudRate = (int)57600;
                    break;
                case 5:
                    SP.BaudRate = (int)115200;
                    break;
                default:
                    SP.BaudRate = (int)19200;
                    break;
            }
        }

        private void cmbParity_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (cmbParity.SelectedIndex)
            {
                case 0:
                    SP.Parity = Parity.Even;
                    break;
                case 1:
                    SP.Parity = Parity.Mark;
                    break;
                case 2:
                    SP.Parity = Parity.None;
                    break;
                case 3:
                    SP.Parity = Parity.Odd;
                    break;
                case 4:
                    SP.Parity = Parity.Space;
                    break;
                default:
                    SP.Parity = Parity.None;
                    break;
            }
        }

        private void cmbStopBits_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (cmbStopBits.SelectedIndex)
            {
                case 0:
                    //SP.StopBits = StopBits.None;
                    MessageBox.Show("이 값은 지원되지 않습니다");
                    break;
                case 1:
                    SP.StopBits = StopBits.One;
                    break;
                case 2:
                    SP.StopBits = StopBits.OnePointFive;
                    break;
                case 3:
                    SP.StopBits = StopBits.Two;
                    break;
                default:
                    SP.StopBits = StopBits.One;
                    break;
            }
        }

     

        private void FormMain_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (SP.IsOpen)
            {
                SP.Close();
            }
        }

        private void btnPortClose_Click(object sender, EventArgs e)
        {
            SP.Close();
            rbText.Text += "\r\n" + "[" + SP.PortName.ToString() + "] Port Close!!";
            lbStatus.Text = "Not Connect!!";
            btnOpen.Visible = true;
            btnPortClose.Visible = false;
        }
    }
}
