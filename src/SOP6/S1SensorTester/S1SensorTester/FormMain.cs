using DBUtility2;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using UnE.GUI;

namespace SensorTester
{
    public partial class FormMain : Form
    {
        public enum Resolution { NONE = -1, FourK = 0, FullHD = 1, Other = 2 }

        static public FormMain Instance;
        private WebDBManager m_dbMgr = null;


        private NetworkManager m_netMgr = null;
        private DataManager m_dataMgr = null;

        private string m_strSensorTagNoHeader = "";
        private string m_strSensorTypeHeader = "";
        private string m_strSensorTagNameHeader = "";

        private int m_nSiteID = 100;
        private Point m_ptStart = new Point();
        
        // Key : SensorZone ID
        // Value : SensorType
        private Dictionary<int, int> m_dicSensorZoneTypes = new Dictionary<int, int>();

        public FormMain(string strServerAddr, string strTitle, Point ptStart)
        {
            m_nSiteID = LoadSiteID();
            //m_dbMgr = new WebDBManager("EDU_100", m_nSiteID);
            m_dbMgr = new WebDBManager(m_nSiteID);
            
            InitializeComponent();

            this.DoubleBuffered = true;

            Instance = this;

            m_dataMgr = new DataManager(m_dbMgr, m_nSiteID);

            if (strTitle != null)
                Text = strTitle;

            // 서버 IP가 인자로 전달되었으므로 UI에서는 표시하지 않는다.
            if (strServerAddr != null)
            {
                textBox4.Text = strServerAddr;
                textBox4.Visible = false;
                btnConnect.Visible = false;
            }

            m_ptStart = ptStart;

            if (m_nSiteID < 100 || m_nSiteID == 102)
            {
                checkBox1.Visible = checkBox2.Visible = checkBox3.Visible = checkBox4.Visible = false;
            }
            else if (m_nSiteID == 101)
            {
                checkBox2.Text = "S1 Secom";
                checkBox3.Visible = checkBox4.Visible = false;
            }
             
            InitCtrlSize(this);
            labelTagNo.Text = "";
        }

        private int LoadSiteID()
        {
            Utility ini = new Utility();
            string strSiteID = ini.getinivalue("Server Connection Info", "siteid");
            //string strSiteID = m_dbMgr.LoadIni("siteid", "Server Connection Info");

            int nSiteID = 1;

            if (strSiteID.Length > 0)
            {
                int.TryParse(strSiteID, out nSiteID);
            }

            return nSiteID;
        }

        public WebDBManager DbMgr
        {
            get { return m_dbMgr; }
            set { m_dbMgr = value; }
        }

        private CheckBox[] checks = new CheckBox[25];
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


        public void Init()
        {
            this.Invoke((MethodInvoker)delegate
            {
                if (m_netMgr.ClientProvider.IsConnected)
                {
                    btnSend.Enabled = btnRecovery.Enabled = btnRecoverAll.Enabled = true;

                    if (timer1.Tag == null)
                    {
                        timer1.Start();
                        timer1.Tag = true;
                    }
                }
                else
                {
                    btnSend.Enabled = btnRecovery.Enabled = btnRecoverAll.Enabled = false;
                }
            });
        }


        private bool CompareResult = false;
        //private void CompareData(int max_ID, int sensorID, int connected, int data)
        //{
        //    Console.WriteLine("hello");

        //    max_ID--;

        //    //가장 최근데이터
        //    string strSQL = "select * from SensorZoneHistory where id = '" + max_ID + "'";
        //    ArrayList arrResult = m_dbMgr.GetResultData(strSQL);

        //    if (arrResult == null)
        //    {
        //        CompareResult = true;
        //        return;
        //    }

        //    int nResultCount = arrResult.Count;

        //    //Data가 비어있다면
        //    if (nResultCount < 1)
        //        CompareResult = true;

        //    for (int i = 0; i < nResultCount - 3; i += 4)
        //    {
        //        int comp_SensorID = WebDBManager.GetIntField(arrResult[i + 1].ToString(), -1);
        //        int comp_Connected = WebDBManager.GetIntField(arrResult[i + 2].ToString(), -1);
        //        int comp_Data = WebDBManager.GetIntField(arrResult[i + 3].ToString(), -1);

        //        //최근값과 똑같은값이 들어올 때 false
        //        if (comp_SensorID == sensorID && comp_Connected == connected && comp_Data == data)
        //            CompareResult = false;
        //        else
        //            CompareResult = true;
        //    }
        //}



        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            m_netMgr.ReleaseThread();
        }




        public void AddLog(object strMsg)
        {
            //FormMain.Instance.Invoke((MethodInvoker)delegate
            //{
            //    textBox1.Text = textBox1.Text + strMsg.ToString() ;
            //});

            Debug.WriteLine(strMsg.ToString());
        }


        private void FormMain_Load(object sender, EventArgs e)
        {
            this.Location = new Point(this.Location.X + m_ptStart.X, this.Location.Y + m_ptStart.Y);

            m_strSensorTagNoHeader = labelSensorTagID.Text;
            m_strSensorTypeHeader = labelSensorTagType.Text;
            m_strSensorTagNameHeader = labelSensorName.Text;

            initSensorTagTree("", true, true, true, true, true);

            LoadSensorZones();

            if (textBox4.Visible == false)
                btnConnect_Click(null, null);
        }

        private void LoadSensorZones()
        {
            string strSQL = "Select ID, Type from SensorZone";
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL);

            if (arrResult == null)
                return;

            int nResultCount = arrResult.Count;

            for (int i=0;i<nResultCount-1;i+=2)
            {
                VariousData<int> id = WebDBManager.GetIntField(arrResult[i].ToString());
                VariousData<int> type = WebDBManager.GetIntField(arrResult[i + 1].ToString());

                if (id == null || type == null)
                    continue;

                m_dicSensorZoneTypes[id.Data] = type.Data;
            }
        }


        private void initSensorTagTree(string strSearchWord,  bool bFireSensor, bool bAccess , bool bSVMS, bool bEmpoll, bool bSecom )
        {
            fireNodes = null;
            accesNodes = null;
            svmsNodes = null;
            empollNodes = null;
            secomNodes = null;

            treeSensorTag.Nodes.Clear();

            treeSensorTag.Visible = false;
            m_dataMgr.MakeSensorTagTree(treeSensorTag, strSearchWord, bFireSensor, bAccess, bSVMS, bEmpoll, bSecom);
            treeSensorTag.Visible = true;

            foreach (TreeNode node in treeSensorTag.Nodes)
            {
                if ((int)node.Tag == 0)
                {
                    fireNodes = node;
                }
                else if ((int)node.Tag == 4000)
                {
                    empollNodes = node;
                }
                else if ((int)node.Tag == 900)
                {
                    svmsNodes = node;
                }
                else if ((int)node.Tag == 1000)
                {
                    accesNodes = node;
                }
                else if ((int)node.Tag == 5001)
                    secomNodes = node;
            }
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            string strServerAddr = textBox4.Text;
            m_netMgr = new NetworkManager(m_dbMgr, strServerAddr, m_nSiteID);

            strServerAddr = m_dbMgr.WebServerURL.Replace("http://", "");


            int nServerPort = m_netMgr.GetServerPort();
            //m_netMgr.ClientProvider.Connect(strServerAddr, nServerPort);
            // ConnectionThread를 사용하도록 하여 중간에 접속이 끊어져도 자동으로 다시 복구하도록 수정
            m_netMgr.RunConnectionThread(strServerAddr, nServerPort);

            //Init();
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            /*TreeNode node1 = GetNode("화재센서\\백화점\\백화점 1층\\RE-TPS1-1F FRONT RM RCN1-EV-01");
            TreeNode node2 = GetNode("화재센서\\백화점\\백화점 2층\\RE-TPS1-2F FRONT RM RCN1-EV-01");

            if (node1 != null && node2 != null)
            {
                treeSensorTag.SelectedNode = node1;
                SendData(0x92);
                treeSensorTag.SelectedNode = node2;
                SendData(0x92);
            }*/

            if (button1.Enabled && button2.Enabled)
            {
                SendData(0x87);
            }
            else
                SendData(0x92);
        }

        private TreeNode GetNode(string strFullPath)
        {
            TreeNodeCollection nodes = treeSensorTag.Nodes;
            string[] tokens = strFullPath.Split('\\');
            TreeNode node = null;

            foreach (string strNodeName in tokens)
            {
                node = GetNode(strNodeName, nodes);

                if (node == null)
                    return null;

                nodes = node.Nodes;
            }

            return node;
        }

        private TreeNode GetNode(string strNodeName, TreeNodeCollection nodes)
        {
            foreach (TreeNode node in nodes)
            {
                if (node.Text == strNodeName)
                    return node;
            }

            return null;
        }

        private void btnOff_Click(object sender, EventArgs e)
        {
            SendData(0x93);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SendData(0x88);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            SendData(0x89);
        }

        
        private void btnRecovery_Click(object sender, EventArgs e)
        {
            SendData(0x93);

            if (gridCurrent.Rows.Count == 0)
            {
                labelSensorTagID.Text = m_strSensorTagNoHeader;
                return;
            }

            int nIdx = gridCurrent.Rows.Count - 1;
            gridCurrent.Rows[nIdx].Selected = true;
        }

        private void btnRecoverAll_Click(object sender, EventArgs e)
        {
            // 현재 들어온 신호를 하나씩 돌면서 처리,,
            for (int nIndex = gridCurrent.Rows.Count - 1; nIndex >= 0; nIndex--)
            {
                gridCurrent.Rows[nIndex].Selected = true;
                SendData(0x93);
            }

            //SendData(0x91);
        }

        private bool SendData(byte msgType)
        {
            // 복구 신호
            if (msgType == 0x93)
            {
                // 복구 신호일 경우 Grid의 선택된 값을 사용하여 Tree의 선택노드를 재설정한다.
                if (gridCurrent.SelectedCells.Count == 0)
                    return false;

                if (gridCurrent.SelectedCells[0].Tag == null || (gridCurrent.SelectedCells[0].Tag is SensorTag) == false)
                    return false;


                int nIdx = gridCurrent.SelectedCells[0].RowIndex;
                gridCurrent.Rows[nIdx].Selected = true;

                //TreeNode node = (TreeNode)gridCurrent.SelectedCells[0].Tag;
                //treeSensorTag.SelectedNode = node;
            }

            //if (treeSensorTag.SelectedNode == null || labelSensorTagID.Text == m_strSensorTagNoHeader)
            //    return false;

            if (labelSensorTagID.Text == m_strSensorTagNoHeader)
                return false;

            string szDate = DateTime.Now.ToString();
            string strSensorTagNo = labelSensorTagID.Text.Remove(0, m_strSensorTagNoHeader.Length);
            string strSensorTagName = labelSensorName.Text.Remove(0, m_strSensorTagNameHeader.Length);
            string strSensorTagType = labelSensorTagType.Text.Remove(0, m_strSensorTypeHeader.Length);

            string szData = szDate + "," + strSensorTagNo + "," + strSensorTagName + "," + strSensorTagType;
            byte[] byte2 = ClientProvider.MakeBytes(szData);

            int nLength = byte2.Length + 8;
            byte[] bytes = new byte[nLength];



            bytes[0] = 0x02;
            bytes[1] = (byte)((byte)((nLength-2) / 128) + 0x80);
            bytes[2] = (byte)((byte)((nLength-2) % 128) + 0x80);

            bytes[3] = 0x80;
            bytes[4] = 0x80;
            bytes[5] = msgType;

            bytes[6] = 0x80;

            bytes[nLength - 1] = 0x03;


            System.Buffer.BlockCopy(byte2, 0, bytes, 7, byte2.Length);

            //if( m_nSiteID == 2)
                m_netMgr.Send_NoLengthByte(bytes, m_netMgr.ClientProvider);
            
            //else
            //    m_netMgr.Send(bytes, m_netMgr.ClientProvider);
            return true;
        }  

        private Image imgSendDefault = global::SensorTester.Properties.Resources.BtnSend_Default;
        private Image imgSendClick = global::SensorTester.Properties.Resources.BtnSend_Click;
        private Image imgSendDisable = global::SensorTester.Properties.Resources.BtnSend_Disable;

        private Image imgStep1Default = global::SensorTester.Properties.Resources.BtnStep1_Default;
        private Image imgStep1Click = global::SensorTester.Properties.Resources.BtnStep1_Click;
        private Image imgStep1Disable = global::SensorTester.Properties.Resources.BtnStep1_Disabled;

        private void treeSensorTag_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node != null && e.Node.Tag != null && e.Node.Tag is SensorTag)
            {
                SensorTag tag = (SensorTag)e.Node.Tag;

                if (m_nSiteID == 201)
                {
                    labelSensorTagID.Text = m_strSensorTagNoHeader + string.Format("{0:00}-{1:00}-{2}-{3:000}",
                                tag.ReceiverID,
                                (tag.SensorTagNo / 10000000) % 100000,
                                (tag.SensorTagNo % 10000000) / 1000000,
                                tag.SensorTagNo % 1000000);
                }
                else if (m_nSiteID == 205)
                {
                    labelSensorTagID.Text = m_strSensorTagNoHeader + string.Format("{0:00}-{1:00}-{2}-{3:000}",
                                tag.ReceiverID, 0, 0, tag.SensorTagNo);
                }
                else
                {
                    labelSensorTagID.Text = m_strSensorTagNoHeader + string.Format("{0:00}-{1:00}-{2}-{3:000}",
                                tag.ReceiverID,
                                (tag.SensorTagNo / 10000) % 100,
                                (tag.SensorTagNo % 10000) / 1000,
                                tag.SensorTagNo % 1000);
                }

                labelSensorTagType.Text = m_strSensorTypeHeader + tag.TagType.ToString();
                labelSensorName.Text = m_strSensorTagNameHeader + tag.SensorName;

                if (tag.TagType == SensorTag.SensorType.PSM센서)
                {
                    button1.Enabled = true;
                    button2.Enabled = true;
                    btnSend.ImageNormal = imgStep1Default;
                    btnSend.ImageClicked = imgStep1Click;
                    btnSend.ImageMouseOver = imgStep1Click;
                    btnSend.ImageDisabled = imgStep1Disable;
                    btnSend.Refresh();
                }
                else
                {
                    button1.Enabled = false;
                    button2.Enabled = false;
                    btnSend.ImageNormal = imgSendDefault;
                    btnSend.ImageClicked = imgSendClick;
                    btnSend.ImageMouseOver = imgSendClick;
                    btnSend.ImageDisabled = imgSendDisable;
                    btnSend.Refresh();
                }

                labelTagNo.Text = tag.SensorTagID.ToString();
            }
            else
            {
                labelSensorTagID.Text = m_strSensorTagNoHeader;
                labelSensorTagType.Text = m_strSensorTypeHeader;
                labelSensorName.Text = m_strSensorTagNameHeader;
                labelTagNo.Text = "";
            }
        }

        private void comboBox1_Leave(object sender, EventArgs e)
        {
            if (comboBox1.Text.Length == 0)
                return;

            if (!comboBox1.Items.Contains(comboBox1.Text))
                comboBox1.Items.Add(comboBox1.Text);
        }

        // 현재 Alarm이 발생중인 SensorReactionLog에 대한 Query 조건문
        private string GetAlarmReactionHistoryQueryString()
        {
            string strCondition = ((int)libSensorProcess.ReactionType.BEGIN_STATUS).ToString();
            strCondition += ", " + ((int)libSensorProcess.ReactionType.NOTIFY_SIGNAL).ToString();

            return "(" + strCondition + ")";
        }

        //현재 Alarm이 꺼진 SensorReactionLog에 대한 Query조건문
        private string GetAlarmOffReactionHistoryQueryString()
        {
            string strCondition = ((int)libSensorProcess.ReactionType.MALFUNCTION).ToString();
            strCondition += ", " + ((int)libSensorProcess.ReactionType.IGNORE_SIGNAL).ToString();
            strCondition += ", " + ((int)libSensorProcess.ReactionType.IGNORE_SOP).ToString();
            strCondition += ", " + ((int)libSensorProcess.ReactionType.END_STATUS).ToString();
            strCondition += ", " + ((int)libSensorProcess.ReactionType.USER_RESET).ToString();
            strCondition += ", " + ((int)libSensorProcess.ReactionType.TIME_OUT).ToString();

            return "(" + strCondition + ")";
        }

        private void timer1_Tick(object sender, EventArgs e)
        {

            // 쿼리 성능 문제로 아래와 같이 수정 (2016-7-21 skkim)
            string szText = "SELECT srh.id, srh.SensorHistoryID, srh.ReactionType, srh.Time, srh.Message, srh.Param1, srh.Param2, srh.Param3, srh.Param4, srh.Param5, szh.SensorID, szh.Param3 FROM SensorReactionHistory as srh ";
            szText += "INNER JOIN  SensorZoneHistory as szh on srh.SensorHistoryID = szh.ID ";
            szText += "WHERE SensorHistoryID in (  SELECT srh2.SensorHistoryID FROM SensorReactionHistory as srh2 WHERE srh2.ReactionType in " + GetAlarmReactionHistoryQueryString() + " ) ";
            szText += " AND SensorHistoryID not in (  SELECT srh3.SensorHistoryID FROM SensorReactionHistory as srh3 WHERE srh3.ReactionType in " + GetAlarmOffReactionHistoryQueryString() + " ) ";
            //szText += "WHERE SensorHistoryID in (  SELECT srh2.SensorHistoryID FROM SensorReactionHistory as srh2 WHERE srh2.ReactionType in ( 0, 60, 62, 898, 899, 921, 961 ) ) ";
            //szText += " AND SensorHistoryID not in (  SELECT srh3.SensorHistoryID FROM SensorReactionHistory as srh3 WHERE srh3.ReactionType in (21, 23, 33, 50, 61, 70, 919, 920,939,940, 969, 970, 1000 )) ";
            szText += " AND szh.SiteID = " + m_nSiteID.ToString();
            szText += " ORDER BY srh.Time, szh.SensorID";

            //string szText = "SELECT srh.id, srh.SensorHistoryID, srh.ReactionType, srh.Time, srh.Message, srh.Param1, srh.Param2, srh.Param3, srh.Param4, srh.Param5, szh.SensorID ";
            //szText += "FROM SensorReactionHistory as srh, SensorZoneHistory as szh ";
            //szText += "WHERE SensorHistoryID in (";
            //szText += "         SELECT srh2.SensorHistoryID ";
            //szText += "         FROM SensorReactionHistory as srh2, SensorZoneHistory as szh2 ";
            //szText += "         WHERE szh2.Id = srh2.SensorHistoryID and srh2.ReactionType in ( 0, 60, 62) ) ";
            //szText += "     AND SensorHistoryID not in (";
            //szText += "         SELECT srh3.SensorHistoryID ";
            //szText += "         FROM SensorReactionHistory as srh3, SensorZoneHistory as szh3 ";
            //szText += "         WHERE szh3.Id = srh3.SensorHistoryID and srh3.ReactionType in (21, 23, 33, 50, 70)) ";
            //szText += "     AND srh.SensorHistoryID = szh.ID ";
            //szText += "     AND szh.SiteID = {0} ";
            ////szText += "     AND sz.Data in (1, 21, 22, 23) ";
            //szText += "     AND ( srh.Time between DATEADD(hour,-24,getdate()) and GETDATE()) ";
            //szText += "     ORDER BY srh.Time, szh.SensorID";

            string strSQL = string.Format(szText, m_nSiteID);

            ArrayList arrResult = m_dbMgr.GetResultData(strSQL);

            if (arrResult == null)
                return;

            int nResultCount = arrResult.Count;

            DateTime dtDefault = new DateTime();

            SensorReactionLog log = new SensorReactionLog();
            bool isSuccess;
            int nMaxID = -1, nPrevSensorID = -1, nSensorID = -1;
            int nPrevCount = 0;

            ArrayList arrTimeHistory = new ArrayList();

            SortedList<int, int> keyExistList = new SortedList<int, int>();

            List<int> sensorZoneIDs = new List<int>();

            DateTime dtNow = DateTime.Now;
            DateTime dt24 = dtNow.AddHours(-24.0);

            for (int i = 0; i < nResultCount - 11; i += 12)
            {
                int nID = WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                int nHistoryID = WebDBManager.GetIntField(arrResult[i + 1].ToString(), -1);
                int nReactionType = WebDBManager.GetIntField(arrResult[i + 2].ToString(), -1);
                DateTime time = WebDBManager.GetDateTimeField(arrResult[i + 3], dtDefault);
                string strMessage = WebDBManager.GetStringField(arrResult[i + 4], "");
                string strParam1 = WebDBManager.GetStringField(arrResult[i + 5], "");
                string strParam2 = WebDBManager.GetStringField(arrResult[i + 6], "");
                string strParam3 = WebDBManager.GetStringField(arrResult[i + 7], "");
                string strParam4 = WebDBManager.GetStringField(arrResult[i + 8], "");
                string strParam5 = WebDBManager.GetStringField(arrResult[i + 9], "");

                //if( time < dt24 )
                //{
                //    continue;
                //}

                if (nID < 0 || nHistoryID < 0)
                    continue;

                nSensorID = WebDBManager.GetIntField(arrResult[i + 10].ToString(), -1);

                int nSensorType = -1;
                m_dicSensorZoneTypes.TryGetValue(nSensorID, out nSensorType);

                if ((nReactionType == (int)libSensorProcess.ReactionType.BEGIN_STATUS && nSensorType == (int)UnE.Sensor.IFacility.FacilityType.PSM_SENSOR) || nReactionType == (int)libSensorProcess.ReactionType.CHANGE_ALARM_DEPTH)
                //if (nReactionType == (int)libSensorProcess.ReactionType.BEGIN_PSM_STATUS || nReactionType == (int)libSensorProcess.ReactionType.CHANGE_PSM_ALARM_DEPTH)
                {
                    nSensorID = WebDBManager.GetIntField(arrResult[i + 6].ToString(), -1);
                }

                string strSensorZoneIDs = WebDBManager.GetStringField(arrResult[i + 11]);

                if (strSensorZoneIDs == null || strSensorZoneIDs.Length == 0)
                    CheckAlarmSensorZone(nSensorID, nHistoryID, nReactionType, strMessage, keyExistList, sensorZoneIDs);
                else
                {
                    // SensorZoneHistory의 Param3에는 현재 발생한 알람과 연관된 센서중 작동한 SensorZone ID들이 담겨있다.
                    // [2019/10/31] 김지웅
                    string[] ids = strSensorZoneIDs.Split(',');
                    int id;

                    foreach (string strID in ids)
                    {
                        if (int.TryParse(strID.Trim(), out id))
                        {
                            CheckAlarmSensorZone(id, nHistoryID, nReactionType, strMessage, keyExistList, sensorZoneIDs);
                        }
                    }
                }

                /*string szHashKey = nHistoryID.ToString() + "_-_" + nReactionType.ToString() + "_-_" + strMessage;
                int nHash = szHashKey.GetHashCode();
                if (keyExistList.ContainsKey(nHash))
                    continue;

                keyExistList.Add(nHash, nHash);


                libSensorProcess.ReactionType type = SensorReactionLog.ToReactionType(nReactionType, out isSuccess);

                if (type == libSensorProcess.ReactionType.SEND_SMS || type == libSensorProcess.ReactionType.RUN_BROADCAST)
                    continue;

                if (!isSuccess)
                    continue;

                // 화학물질 센서는 통합처리되므로 data가 같은 SensorZone이므로 각기 SensorZone의 Data를 확인하도록 한다.
                // skkim 2016-02-26 
                string szText2 = "SELECT Data FROM SensorZone WHERE ID = {0}";
                string szSQL2 = string.Format(szText2, nSensorID);
                ArrayList arrResult2 = m_dbMgr.GetResultData(szSQL2);
                if (arrResult2 == null || arrResult2.Count == 0)
                    continue;

                int nSensorData = WebDBManager.GetIntField(arrResult2[0].ToString(), -1);
                if (nSensorData == 1 || nSensorData == 21 || nSensorData == 22 || nSensorData == 23)
                {
                    if (!sensorZoneIDs.Contains(nSensorID))
                        sensorZoneIDs.Add(nSensorID);
                }*/
            }

            UpdateGrid(sensorZoneIDs);
        }

        private bool CheckAlarmSensorZone(int nSensorZoneID, int nSensorZoneHistoryID, int nReactionType, string strMessage, SortedList<int, int> keyExistList, List<int> sensorZoneIDs)
        {
            string szHashKey = nSensorZoneHistoryID.ToString() + "_-_" + nSensorZoneID + "_-_" + nReactionType.ToString() + "_-_" + strMessage;
            int nHash = szHashKey.GetHashCode();
            if (keyExistList.ContainsKey(nHash))
                return false;

            keyExistList.Add(nHash, nHash);

            bool isSuccess;
            libSensorProcess.ReactionType type = SensorReactionLog.ToReactionType(nReactionType, out isSuccess);

            if (type == libSensorProcess.ReactionType.SEND_SMS || type == libSensorProcess.ReactionType.RUN_BROADCAST)
                return false;

            if (!isSuccess)
                return false;

            // 화학물질 센서는 통합처리되므로 data가 같은 SensorZone이므로 각기 SensorZone의 Data를 확인하도록 한다.
            // skkim 2016-02-26 
            string szText2 = "SELECT Data FROM SensorZone WHERE ID = {0}";
            string szSQL2 = string.Format(szText2, nSensorZoneID);
            ArrayList arrResult2 = m_dbMgr.GetResultData(szSQL2);
            if (arrResult2 == null || arrResult2.Count == 0)
                return false;

            int nSensorData = WebDBManager.GetIntField(arrResult2[0].ToString(), -1);
            if (nSensorData == 1 || nSensorData == 21 || nSensorData == 22 || nSensorData == 23)
            {
                if (!sensorZoneIDs.Contains(nSensorZoneID))
                {
                    sensorZoneIDs.Add(nSensorZoneID);
                    return true;
                }
            }

            return false;
        }

        private void UpdateGrid(List<int> sensorZoneIDs)
        {
            List<DataGridViewRow> removeRows = new List<DataGridViewRow>();

            foreach (DataGridViewRow row in gridCurrent.Rows)
            {
                if (row.IsNewRow || row.Tag == null)
                    continue;

                if (sensorZoneIDs.Contains((int)row.Tag))
                    sensorZoneIDs.Remove((int)row.Tag);
                else
                    removeRows.Add(row);
            }

            foreach (DataGridViewRow row in removeRows)
            {
                gridCurrent.Rows.Remove(row);
            }

            foreach (int nSensorZoneID in sensorZoneIDs)
            {
                SensorTag sensor = m_dataMgr.GetSensorTagBySensorZoneID(nSensorZoneID);

                if (sensor == null)
                    continue;

                DataGridViewRow row = new DataGridViewRow();
                DataGridViewTextBoxCell cell = new DataGridViewTextBoxCell();
                cell.Value = sensor.SensorName;
                cell.Tag = sensor;
                row.Cells.Add(cell);
                row.Tag = nSensorZoneID;

                gridCurrent.Rows.Add(row);

                

                //TreeNode node = FindTreeNode(nSensorZoneID);

                //if (node == null)
                //    continue;

                //DataGridViewRow row = new DataGridViewRow();
                //DataGridViewTextBoxCell cell = new DataGridViewTextBoxCell();
                //cell.Value = node.Text;
                //row.Cells.Add(cell);
                //gridCurrent.Rows.Add(row);

                //cell.Tag = node;
                //row.Tag = nSensorZoneID;
            }
        }

        private TreeNode FindTreeNode(int nSensorZoneID)
        {
            TreeNode node = FindTreeNode(nSensorZoneID, treeSensorTag.Nodes);
            return node;
        }

        private TreeNode FindTreeNode(int nSensorZoneID, TreeNodeCollection nodes)
        {
            foreach (TreeNode node in nodes)
            {
                if (node.Tag == null)
                    continue;

                if (node.Tag is SensorTag)
                {
                    SensorTag tag = (SensorTag)node.Tag;

                    if (tag.SensorZone != null && tag.SensorZone.ID == nSensorZoneID)
                        return node;
                }

                TreeNode child = FindTreeNode(nSensorZoneID, node.Nodes);

                if (child != null)
                    return child;
            }

            return null;
        }

        private void gridCurrent_SelectionChanged(object sender, EventArgs e)
        {
            foreach (DataGridViewCell cell in gridCurrent.SelectedCells)
            {
                DataGridViewRow row = gridCurrent.Rows[cell.RowIndex];

                if (row.IsNewRow)
                    continue;

                if (cell.Tag == null)
                    continue;

                SensorTag tag = (SensorTag)cell.Tag;

                if (m_nSiteID == 201)
                {
                    labelSensorTagID.Text = m_strSensorTagNoHeader + string.Format("{0:00}-{1:00}-{2}-{3:000}",
                                tag.ReceiverID,
                                (tag.SensorTagNo / 10000000) % 100000,
                                (tag.SensorTagNo % 10000000) / 1000000,
                                tag.SensorTagNo % 1000000);
                }
                else if (m_nSiteID == 205)
                {
                    labelSensorTagID.Text = m_strSensorTagNoHeader + string.Format("{0:00}-{1:00}-{2}-{3:000}",
                                tag.ReceiverID, 0, 0, tag.SensorTagNo);
                }
                else
                {
                    labelSensorTagID.Text = m_strSensorTagNoHeader + string.Format("{0:00}-{1:00}-{2}-{3:000}",
                                tag.ReceiverID,
                                (tag.SensorTagNo / 10000) % 100,
                                (tag.SensorTagNo % 10000) / 1000,
                                tag.SensorTagNo % 1000);
                }

                labelSensorTagType.Text = m_strSensorTypeHeader + tag.TagType.ToString();
                labelSensorName.Text = m_strSensorTagNameHeader + tag.SensorName;

                TreeNode node = FindTreeNode(tag.SensorZone.ID);
                if (node != null)
                {
                    treeSensorTag.SelectedNode = node;
                }

                //TreeNode node = (TreeNode)cell.Tag;
                //treeSensorTag.SelectedNode = node;

                break;
            }
        }


        private string m_strZoneSearchText = String.Empty;
        private int m_nSameZoneSearchCnt = 0;
        
        private void btnZoneSearch_Click(object sender, EventArgs e)
        {
            m_strSensorSearchText = string.Empty;
            m_nSameSensorSearchCnt = 0;

            string strZoneSearchText = this.txtZoneSearch.Text;

            if (String.IsNullOrWhiteSpace(strZoneSearchText))
                return;

            if (String.Equals(m_strZoneSearchText, strZoneSearchText) == false)
            {
                m_strZoneSearchText = strZoneSearchText;
                m_nSameZoneSearchCnt = 0;
            }

            m_nSameZoneSearchCnt++;

            Zone zone = m_dataMgr.GetZoneForSearch(m_nSameZoneSearchCnt, m_strZoneSearchText);

            TreeNode itemNode = null;
            foreach (TreeNode node in treeSensorTag.Nodes)
            {
                itemNode = FromID(zone, node);
                if (itemNode != null)
                {
                    break;
                }
            }

            if (itemNode != null)
            {
                treeSensorTag.SelectedNode = itemNode;
            }
            else
            {
                if (m_nSameZoneSearchCnt > 1)
                {
                    MessageBox.Show("더 이상 일치하는 항목이 없습니다.");
                }
                else
                {
                    MessageBox.Show("일치하는 항목이 없습니다.");
                }
                m_nSameZoneSearchCnt = 0;
            }

        }


        private string m_strSensorSearchText = String.Empty;
        private int m_nSameSensorSearchCnt = 0;

        private void btnSensorSearch_Click(object sender, EventArgs e)
        {
            m_strZoneSearchText = string.Empty;
            m_nSameZoneSearchCnt = 0;

            string strSensorSearchText = this.txtSensorSearch.Text;

            if (String.IsNullOrWhiteSpace(strSensorSearchText))
                return;

            if (String.Equals(m_strSensorSearchText, strSensorSearchText) == false)
            {
                m_strSensorSearchText = strSensorSearchText;
                m_nSameSensorSearchCnt = 0;
            }

            m_nSameSensorSearchCnt++;

            SensorTag sensorTag = m_dataMgr.GetSensorTagForSearch(m_nSameSensorSearchCnt, m_strSensorSearchText);

            TreeNode itemNode = null;
            foreach (TreeNode node in treeSensorTag.Nodes)
            {
                itemNode = FromID(sensorTag, node);
                if (itemNode != null)
                {
                    break;
                }
            }

            if (itemNode != null)
            {
                treeSensorTag.SelectedNode = itemNode;
            }
            else
            {
                if (m_nSameSensorSearchCnt > 1)
                {
                    MessageBox.Show("더 이상 일치하는 항목이 없습니다.");
                }
                else
                {
                    MessageBox.Show("일치하는 항목이 없습니다.");
                }
                m_nSameSensorSearchCnt = 0;
            }
        }

        private bool m_bFireSensor = true;
        private bool m_bAccess  = true;
        private bool m_bSVMS = true;
        private bool m_bEmpoll = true;
        private bool m_bSecom = true;

        private TreeNode fireNodes = null;
        private TreeNode accesNodes = null;
        private TreeNode svmsNodes = null;
        private TreeNode empollNodes = null;
        private TreeNode secomNodes = null;

        private void VisibleAccessTag(bool bVisible)
        {
            if (bVisible == false)
            {
                if (accesNodes != null)
                {
                    treeSensorTag.Visible = false;
                    treeSensorTag.Nodes.Remove(accesNodes);
                    treeSensorTag.Visible = true;
                }
            }
            else
            {
                if (accesNodes != null)
                {
                    treeSensorTag.Visible = false;
                    treeSensorTag.Nodes.Insert(0, accesNodes);
                    treeSensorTag.Visible = true;
                }
            }      
        }

        private void VisibleSVMSTag(bool bVisible)
        {
            if (bVisible == false)
            {
                if (svmsNodes != null)
                    treeSensorTag.Nodes.Remove(svmsNodes);
            }
            else
            {
                if (svmsNodes != null)
                    treeSensorTag.Nodes.Insert(1, svmsNodes);
            }
        }

        private void VisibleFireTag(bool bVisible)
        {
            if (bVisible == false)
            {
                if (fireNodes != null)
                    treeSensorTag.Nodes.Remove(fireNodes);
            }
            else
            {
                if (fireNodes != null)
                    treeSensorTag.Nodes.Insert(2, fireNodes);
            }
        }

        private void VisibleEmpollTag(bool bVisible)
        {
            if (bVisible == false)
            {
                if (empollNodes != null)
                    treeSensorTag.Nodes.Remove(empollNodes);
            }
            else
            {
                if (empollNodes != null)
                    treeSensorTag.Nodes.Insert(3, empollNodes);
            }
        }

        private void VisibleSecomTag(bool bVisible)
        {
            if (bVisible == false)
            {
                if (secomNodes != null)
                {
                    treeSensorTag.Visible = false;
                    treeSensorTag.Nodes.Remove(secomNodes);
                    treeSensorTag.Visible = true;
                }
            }
            else
            {
                if (secomNodes != null)
                {
                    treeSensorTag.Visible = false;
                    treeSensorTag.Nodes.Insert(0, secomNodes);
                    treeSensorTag.Visible = true;
                }
            }
        }

        private void RecreateSensorTree()
        {
            fireNodes = null;
            accesNodes = null;
            svmsNodes = null;
            empollNodes = null;

            string strSearchText = this.txtSearch.Text;

            if (treeSensorTag != null)
                treeSensorTag.Visible = false;
            initSensorTagTree(strSearchText.Trim(), m_bFireSensor, m_bAccess, m_bSVMS, m_bEmpoll, true);

            if (treeSensorTag != null)
                treeSensorTag.Visible = true;

            foreach(TreeNode node in treeSensorTag.Nodes)
            {
                if( (int)node.Tag == 0)
                {
                    fireNodes = node;
                }
                else if((int)node.Tag == 4000)
                {
                    empollNodes = node;
                }
                else if((int)node.Tag == 900)
                {
                    svmsNodes = node;
                }
                else if((int)node.Tag == 1000)
                {
                    accesNodes = node;
                }
                else if ((int)node.Tag == 5001)
                {
                    secomNodes = node;
                }
            }
        }
        
        private void btnSearch_Click(object sender, EventArgs e)
        {
            RecreateSensorTree();
        }

        private TreeNode FromID(object obj, TreeNode rootNode)
        {
            // obj 는 Zone 또는 SensorTag

            if (obj == null)
                return null;


            foreach (TreeNode node in rootNode.Nodes)
            {
                if (node.Tag.Equals(obj))
                    return node;

                TreeNode next = FromID(obj, node);

                if (next != null)
                    return next;
            }

            return null;
        }
        

        private void txtSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                btnZoneSearch.PerformClick();
        }

        private void txtSensorSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                btnSensorSearch.PerformClick();
        }

        private void txtSearch_KeyDown_1(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                btnSearch_Click(null, null);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            SendData(0x94); 
        }

        private void button4_Click(object sender, EventArgs e)
        {
            SendData(0x95);
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox ckb = (CheckBox)sender;
            bool bCheck = ckb.Checked;
            if( ckb.Checked == true)
            {
                if( m_bFireSensor != true)
                {
                    m_bFireSensor = true;
                    VisibleFireTag(m_bFireSensor);
                }               
            }
            else
            {
                if( m_bFireSensor == true )
                {
                    m_bFireSensor = false;
                    VisibleFireTag(m_bFireSensor);
                }
            }            
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {

            CheckBox ckb = (CheckBox)sender;
            bool bCheck = ckb.Checked;
            if (ckb.Checked == true)
            {
                if (m_nSiteID == 100)
                {
                    if (m_bAccess != true)
                    {
                        m_bAccess = true;
                        VisibleAccessTag(m_bAccess);
                    }
                }
                else if (m_nSiteID == 101)
                {
                    if (m_bSecom != true)
                    {
                        m_bSecom = true;
                        VisibleSecomTag(m_bSecom);
                    }
                }
            }
            else
            {
                if (m_nSiteID == 100)
                {
                    if (m_bAccess == true)
                    {
                        m_bAccess = false;
                        VisibleAccessTag(m_bAccess);
                    }
                }
                else if (m_nSiteID == 101)
                {
                    if (m_bSecom == true)
                    {
                        m_bSecom = false;
                        VisibleSecomTag(m_bSecom);
                    }
                }
            }   
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox ckb = (CheckBox)sender;
            bool bCheck = ckb.Checked;
            if (ckb.Checked == true)
            {
                if (m_bSVMS != true)
                {
                    m_bSVMS = true;
                    VisibleSVMSTag(m_bSVMS);
                }
            }
            else
            {
                if (m_bSVMS == true)
                {
                    m_bSVMS = false;
                    VisibleSVMSTag(m_bSVMS);
                }
            }  
        }

        private void checkBox4_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox ckb = (CheckBox)sender;
            bool bCheck = ckb.Checked;
            if (ckb.Checked == true)
            {
                if (m_bEmpoll != true)
                {
                    m_bEmpoll = true;
                    VisibleEmpollTag(m_bEmpoll);
                }
            }
            else
            {
                if (m_bEmpoll == true)
                {
                    m_bEmpoll = false;
                    VisibleEmpollTag(m_bEmpoll);
                }
            }  
        }

        private void btnMinimize_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.timer1.Stop();
            this.Close();            
        }

        #region 폼 이동
        private bool m_bLeftMouseDown = false;
        private Point m_ptMove = new Point();
        private bool m_isClicked = false;
        private Point m_ptOrigin = new Point();

        private void FormMain_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                m_bLeftMouseDown = true;
                m_ptMove = Control.MousePosition;
                m_ptOrigin = this.Location;
            }

            m_isClicked = true;
        }

        private void FormMain_MouseMove(object sender, MouseEventArgs e)
        {
            if (!m_isClicked)
                return;

            if (!m_bLeftMouseDown)
                return;

            Point ptScreen = Control.MousePosition;

            int dx = ptScreen.X - m_ptMove.X;
            int dy = ptScreen.Y - m_ptMove.Y;

            if (dx == 0 && dy == 0)
                return;

            Point ptCur = this.Location;
            this.Location = new Point(ptCur.X + dx, ptCur.Y + dy);
            m_ptMove.X += dx;
            m_ptMove.Y += dy;
        }

        private void FormMain_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
                m_bLeftMouseDown = false;

            m_isClicked = false;
        } 
        #endregion

        private Resolution curResolution = Resolution.NONE;
        
        private void FormMain_LocationChanged(object sender, EventArgs e)
        {
            if (GetResolution())
            {
                SetChildCtrlResize(this, 841, 272);

                float sizePer = 1.0f;
                if (curResolution == Resolution.FourK)
                    sizePer = 2.0f;
                if (curResolution == Resolution.Other)
                    sizePer = 1.5f;

                gridCurrent.Font = new System.Drawing.Font("나눔스퀘어", 9 * sizePer, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            }
        }

        private bool GetResolution()
        {
            int curMonitorIndex = -1;
            for (int i = 0; i < Screen.AllScreens.Count(); i++)
            {
                Screen sc2 = Screen.AllScreens[i];

                Size thisSize = this.Size;
                 
                if (this.Location.X >= sc2.Bounds.X && sc2.Bounds.X + sc2.Bounds.Width > this.Location.X &&
                    this.Location.Y >= sc2.Bounds.Y && sc2.Bounds.Y + sc2.Bounds.Height > this.Location.Y)
                { 
                    curMonitorIndex = i;
                    break;
                }
            }

            if (curMonitorIndex < 0)
                return false;

            Screen sc = Screen.AllScreens[curMonitorIndex];
            if (sc.Bounds.Width == 1920 && sc.Bounds.Height == 1080)
            {
                if (curResolution == Resolution.FullHD)
                    return false;

                curResolution = Resolution.FullHD;
            }
            else if (sc.Bounds.Width == 3840 && sc.Bounds.Height == 2160)
            {
                if (curResolution == Resolution.FourK)
                    return false;

                curResolution = Resolution.FourK;
            }
            else
            {
                if (curResolution == Resolution.Other)
                    return false;

                curResolution = Resolution.Other;
            }

            return true;
        }

        private Dictionary<Control, Point> m_CtrlLoction = new Dictionary<Control, Point>();
        private Dictionary<Control, Size> m_CtrlSize = new Dictionary<Control, Size>();
        private Dictionary<Control, Font> m_CtrlFontSize = new Dictionary<Control, Font>();

        protected void InitCtrlSize(Control ctrl)
        {
            foreach (Control childCtrl in ctrl.Controls)
            {
                if (childCtrl is ImageButton)
                {
                    ImageButton imgBtn = childCtrl as ImageButton;
                    if (imgBtn.ImageNormal == null)
                        m_CtrlSize.Add(childCtrl, imgBtn.Size);
                    else
                        m_CtrlSize.Add(childCtrl, new Size(imgBtn.ImageNormal.Size.Width / 2, imgBtn.ImageNormal.Size.Height / 2));
                }
                else if (childCtrl is Panel)
                {
                    if (childCtrl.BackgroundImage == null)
                        m_CtrlSize.Add(childCtrl, childCtrl.Size);
                    else
                        m_CtrlSize.Add(childCtrl, new Size(childCtrl.BackgroundImage.Size.Width / 2, childCtrl.BackgroundImage.Size.Height / 2));
                }
                else if (childCtrl is Label || childCtrl is CheckBox)
                {
                    m_CtrlFontSize.Add(childCtrl, childCtrl.Font);
                }
                else if (childCtrl is PictureBox)
                {
                    PictureBox pic = childCtrl as PictureBox;
                    if (pic.Image == null)
                        m_CtrlSize.Add(childCtrl, pic.Size);
                    else
                        m_CtrlSize.Add(childCtrl, new Size(pic.Image.Size.Width / 2, pic.Image.Size.Height / 2));
                }
                else if (childCtrl is TextBox || childCtrl is RichTextBox || childCtrl is Button || childCtrl is RadioButton || childCtrl is ImageComboBox || childCtrl is TreeView || childCtrl is DateTimePicker)
                {
                    m_CtrlSize.Add(childCtrl, childCtrl.Size);
                    m_CtrlFontSize.Add(childCtrl, childCtrl.Font);
                }
                else if (childCtrl is DataGridView)
                {
                    m_CtrlSize.Add(childCtrl, childCtrl.Size);
                }

                m_CtrlLoction.Add(childCtrl, childCtrl.Location);
                InitCtrlSize(childCtrl);
            }
        }

        public void SetChildCtrlResize(Control ctrl, int width, int height)
        {
            float sizePer = 1f;
            if (curResolution == Resolution.Other)
                sizePer = 1.5f;
            else if (curResolution == Resolution.FourK)
                sizePer = 2.0f;

            ctrl.Size = new Size(Convert.ToInt32(width * sizePer), Convert.ToInt32(height * sizePer));

            if (m_CtrlLoction.ContainsKey(ctrl))
                ctrl.Location = new Point((int)(m_CtrlLoction[ctrl].X * sizePer), (int)(m_CtrlLoction[ctrl].Y * sizePer));

            foreach (Control childCtrl in ctrl.Controls)
            {
                int width2 = -1;
                int height2 = -1;

                if (m_CtrlSize.ContainsKey(childCtrl))
                {
                    width2 = m_CtrlSize[childCtrl].Width;
                    height2 = m_CtrlSize[childCtrl].Height;
                }

                if (m_CtrlFontSize.ContainsKey(childCtrl))
                {
                    Font font = m_CtrlFontSize[childCtrl];
                    FontFamily fontFamily = font.FontFamily;
                     
                    float fontSize = font.Size;
                    FontStyle fontStyle = font.Style; 

                    childCtrl.Font = new Font(fontFamily, fontSize * sizePer, fontStyle, System.Drawing.GraphicsUnit.Point, ((byte)(129)));

                    // 예외 : Label 일 경우 입력 Text마다 크기가 달라진다
                    if (width2 == -1 && height2 == -1)
                    {
                        width2 = childCtrl.Width;
                        height2 = childCtrl.Height;
                    }
                }

                if (width2 < 0 || height2 < 0)
                    continue;

                SetChildCtrlResize(childCtrl, width2, height2);
            }
        } 
    }

    public class SOPMonitor
    {
        private static FormMain m_nstance = null;

        public static FormMain Instance
        {
            get { return FormMain.Instance; }

        }
    }

    public class SensorReactionLog
    {
        /*public enum ReactionType
        {
            BEGIN_STATUS = 0,
            RUN_BROADCAST = 10,
            SEND_SMS = 11,
            MALFUNCTION = 21,
            NOTIFY_FIRE = 22,
            IGNORE_FIRE = 23,
            TRAINNING_FIRE = 24,
            RUN_SOP = 30,
            RUN_N_CANCEL_SOP = 31,
            FINISH_SOP = 32,
            IGNORE_SOP = 33,
            END_STATUS = 50,
            BEGIN_PSM_STATUS = 60,
            IGNORE_PSM_DETECT = 61,
            CHANGE_PSM_ALARM_DEPTH = 62,
            END_PSM_STATUS = 70,
            ETC = 100,
            
            NOTIFY_SECURITY = 898,
            BEGIN_S1SVMS_STATUS = 899,
            IGNORE_S1SVMS_STATUS = 919,
            END_S1SVMS_STATUS = 920,

            BEGIN_S1ACCESS_STATUS = 921,
            IGNORE_S1ACCESS_STATUS = 939,
            END_S1ACCESS_STATUS = 940
        }*/

        private static Dictionary<int, libSensorProcess.ReactionType> m_dicReactionType = null;

        private int m_nID = -1;
        private int m_nSensorHistoryID = -1;
        private libSensorProcess.ReactionType m_type = libSensorProcess.ReactionType.ETC;
        private DateTime m_time = new DateTime();
        private string m_strMessage = "";
        private string m_strParam1 = "";
        private string m_strParam2 = "";
        private string m_strParam3 = "";
        private string m_strParam4 = "";
        private string m_strParam5 = "";

        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }

        public int SensorHistoryID
        {
            get { return m_nSensorHistoryID; }
            set { m_nSensorHistoryID = value; }
        }

        public libSensorProcess.ReactionType Type
        {
            get { return m_type; }
            set { m_type = value; }
        }

        public DateTime LogTime
        {
            get { return m_time; }
            set { m_time = value; }
        }

        public string Message
        {
            get { return m_strMessage; }
            set { m_strMessage = value; }
        }

        public string Param1
        {
            get { return m_strParam1; }
            set { m_strParam1 = value; }
        }

        public string Param2
        {
            get { return m_strParam2; }
            set { m_strParam2 = value; }
        }

        public string Param3
        {
            get { return m_strParam3; }
            set { m_strParam3 = value; }
        }
        public string Param4
        {
            get { return m_strParam4; }
            set { m_strParam4 = value; }
        }

        public string Param5
        {
            get { return m_strParam5; }
            set { m_strParam5 = value; }
        }

        public int GetBytesCount()
        {
            int nBytesCount = sizeof(int) * 3;  // ID, SensorHistoryID, Type
            nBytesCount += sizeof(long);        // LogTime
            // Message, Param1, Param2
            nBytesCount += (m_strMessage.Length + m_strParam1.Length + m_strParam2.Length) * sizeof(char);
            nBytesCount += (m_strParam3.Length + m_strParam4.Length + m_strParam5.Length) * sizeof(char);
            // FieldCount : 7
            return nBytesCount + 5 * 7 + 2;
        }

        public static void CopyBytes(byte[] bytesDest, ref int nDestOffset, byte[] bytesSrc)
        {
            int nLength = bytesSrc.Length;

            //TcpLib2.ConnectionLog.Instance.WriteLine(string.Format("bytesSrc length : {0}, bytesDest length : {1}, nDestOffset : {2}, nLength : {3}",
            //	bytesSrc.Length, bytesDest.Length, nDestOffset, nLength));

            System.Buffer.BlockCopy(bytesSrc, 0, bytesDest, nDestOffset, nLength);
            nDestOffset += nLength;
        }

        public static libSensorProcess.ReactionType ToReactionType(int nType, out bool isSuccess)
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

        public SensorReactionLog Clone()
        {
            SensorReactionLog log = new SensorReactionLog();

            log.m_nID = m_nID;
            log.m_nSensorHistoryID = m_nSensorHistoryID;
            log.m_type = m_type;
            log.m_time = m_time;
            log.m_strMessage = m_strMessage;
            log.m_strParam1 = m_strParam1;
            log.m_strParam2 = m_strParam2;
            log.m_strParam3 = m_strParam3;
            log.m_strParam4 = m_strParam4;
            log.m_strParam5 = m_strParam5;
            return log;
        }
    }

}
