using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Windows.Forms;
using DBUtility2;
using SDMSAgent;
using System.Xml;

namespace SDMSCommander
{
    public partial class MainForm : Form
    {
        private WebDBManager m_dbMgr = null;
         
        private SendCommand sendCmd = null;

        private int m_nSiteID = 1;
        private static MainForm m_instance = null;
        private Network.NetworkWebManager m_netMgr = null;

        public static MainForm Instance
        {
            get { return m_instance; }
        }

        public int SiteID
        {
            get { return m_nSiteID; }
        }

        private string m_strDownloadLocalPath
        {
            get { return txt_DownloadLocalPath.Text; }
        }

        private string m_strServerLogFilePath
        {
            get { return txt_LogFilePath.Text; }
        }

        public MainForm()
        {
            InitializeComponent();
            m_instance = this;

            treeView1.ImageList = imageList1;
            this.Size = new Size(845, 480);

            sendCmd = new SendCommand();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            pnAgentUpdate.Location = pnGetProcList.Location = pnGetFileList.Location = pnUpdate.Location = pnDownload.Location = pnSdmsUpdate.Location = pnFileCopy.Location = new Point(12, 44);
            treeView1.Location = new Point(pnGetFileList.Location.X, pnGetFileList.Location.Y + pnGetFileList.Height + 10);
            btnRefreshDirectory.Location = new Point(treeView1.Location.X, treeView1.Location.Y + treeView1.Height + 10);
            txtDirectoryPath.Location = new Point(btnRefreshDirectory.Location.X + btnRefreshDirectory.Width + 10, btnRefreshDirectory.Location.Y);

            ConnectDB();
            
            m_netMgr = new Network.NetworkWebManager(m_dbMgr, m_nSiteID);

            SetComboBoxList();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="strDownloadPath">다운받는 경로</param>
        /// <param name="strLocalPath">다운받을 경로</param>
        private bool DownloadLocal(string strDownloadPath, string strLocalPath)
        {
            if (strDownloadPath.Length == 0 || strLocalPath.Length == 0)
                return false;

            string strExtension = Path.GetExtension(strDownloadPath);
             
            try
            {
                int nSlash = strDownloadPath.LastIndexOf('/');
                string strTemp = "";

                for (int i = nSlash + 1; i < strDownloadPath.Length; i++)
                {
                    char ch = strDownloadPath.ElementAt(i);

                    if (ch > 256)
                        strTemp += '_';
                    else
                        strTemp += ch;
                }

                strDownloadPath = strDownloadPath.Substring(0, nSlash + 1) + strTemp;

                int nIndex = strDownloadPath.LastIndexOf('\\');
                string strFileName = strDownloadPath.Substring(nIndex + 1);

                //int nIndex = strDownloadPath.LastIndexOf('\\');
                //string strLast = strDownloadPath.Substring(nIndex + 1);   
                 
                System.Net.WebClient web = new System.Net.WebClient();

                Uri uri = new Uri(strDownloadPath);

                CredentialCache credentials = new CredentialCache();
                NetworkCredential netCredential = new NetworkCredential("sop", "sop");
                credentials.Add(uri, "Basic", netCredential);
                web.Credentials = new NetworkCredential("sop", "sop");

                if (System.IO.File.Exists(strLocalPath))
                {
                    System.IO.FileInfo file = new FileInfo(strLocalPath);
                    file.IsReadOnly = false;
                    System.IO.File.Delete(strLocalPath);
                }

                web.DownloadFile(strDownloadPath, strLocalPath);                

                return true;
            } 
            catch (Exception e)
            {
                System.Diagnostics.Trace.WriteLine(e.Message + " / 다운받는 경로 : " + strDownloadPath + " / 다운받을 경로 : " + strLocalPath);
                return false;
            }
        }
         
        #region CheckedChanged 이벤트        
        private void chk_CheckedChanged(object sender, EventArgs e)
        {
            if (chk_Kill.Checked)
                groupBox_Kill.Enabled = true;
            else
                groupBox_Kill.Enabled = false;

            if (chk_Upload.Checked)
                groupBox_Down.Enabled = true;
            else
                groupBox_Down.Enabled = false;

            if (chk_Start.Checked)
                groupBox_Start.Enabled = true;
            else
                groupBox_Start.Enabled = false;
        }

        private void rb_Kill_CheckedChanged(object sender, EventArgs e)
        { 
            rb_Start_Proc.Checked = rb_Kill_Proc.Checked;
            rb_Start_Service.Checked = rb_Kill_Service.Checked; 
        }

        private void chk_GetAllProc_CheckedChanged(object sender, EventArgs e)
        {
            if (chk_GetAllProc.Checked)
                txt_GetProcName.Enabled = false;
            else
                txt_GetProcName.Enabled = true;
        }
        #endregion

        #region 함수
        private bool CheckEmpty(string txt)
        {
            if (txt.Trim().Length == 0)
                return false;
            return true;
        } 
        #endregion

        #region 버튼 이벤트
        private void ConnectDB()
        {
            this.Cursor = Cursors.WaitCursor;
            
            int siteID = 0;
            int.TryParse(txt_SiteId.Text, out siteID);
            m_dbMgr = new WebDBManager(siteID);
            m_nSiteID = siteID;
            
            bool isSuc = false;
            ArrayList arr = m_dbMgr.GetResultData("SELECT ID FROM Site", 0);
            if (arr == null || arr.Count == 0)
                isSuc = false;            
            else
                isSuc = true;

            if (isSuc)
            {
                label_isConnect.Text = "연결중(" + m_dbMgr.WebServerURL + ")";
                label_isConnect.ForeColor = Color.Green;
            }
            else
            {
                label_isConnect.Text = "연결안됨";
                label_isConnect.ForeColor = Color.Red;
            }
            this.Cursor = Cursors.Default;
        }
        private void btn_Connect_Click(object sender, EventArgs e)
        {
            ConnectDB();
        }
        
        private void btn_SendCommand_Click(object sender, EventArgs e)
        {
            try
            {
                if (m_dbMgr == null)
                    throw new ApplicationException("DB 연결 안됨");

                CustomComboBoxItem selectedItem = cbCommand.SelectedItem as CustomComboBoxItem;                
                if (selectedItem == null)
                    throw new ApplicationException("전송할 Command를 선택하세요.");

                CommandType commandType = selectedItem.customCommandType;

                if (commandType == CommandType.NONE)
                {
                    return;
                }                

                CommandItem cmdItem = new CommandItem();
                cmdItem.TimeStamp = DateTime.Now;
                cmdItem.CmdType = commandType;

                if (commandType == CommandType.AGENT_UPDATE)
                {
                    int nIndex = txt_AgentPath.Text.LastIndexOf('\\');
                    string strFileName = txt_AgentPath.Text.Substring(nIndex + 1);

                    if (!CheckEmpty(txt_AgentPath.Text) || strFileName != "SDMSAgent.exe")
                        throw new ApplicationException("SDMSAgent.exe 파일을 선택하세요");

                    string strError = "";
                    DBUtility2.UpDownManager.UploadFile(txt_AgentPath.Text, m_dbMgr.WebServerURL, out strError);

                    if (strError.Length > 0)
                        throw new ApplicationException(strError);                    
                }
                else if (commandType == CommandType.GET_PROC_LIST)
                {
                    if (chk_GetAllProc.Checked)
                    {
                        cmdItem.CmdType = CommandType.GET_ALL_PROC_LIST;
                    }
                    else
                    {
                        if (!CheckEmpty(txt_GetProcName.Text))
                            throw new ApplicationException("검색할 프로세스명을 입력하세요.");
                        
                        cmdItem.SearchPath = txt_GetProcName.Text;
                    }
                }
                else if (commandType == CommandType.GET_FILE_LIST)
                {
                    if (!CheckEmpty(txt_GetFileListPath.Text))
                        throw new ApplicationException("검색할 폴더 경로를 입력하세요.");
                    
                    cmdItem.SearchPath = txt_GetFileListPath.Text;
                }
                else if (commandType == CommandType.UPDATE)
                {
                    if (!chk_Kill.Checked && !chk_Upload.Checked && !chk_Start.Checked)
                        throw new ApplicationException("한 가지 이상의 작업을 선택하세요.");

                    if (chk_Kill.Checked)
                    {
                        if (!CheckEmpty(txt_Kill_FileName.Text))
                        {
                            if (rb_Kill_Proc.Checked)
                                throw new ApplicationException("Kill Process명을 입력하세요.");
                            else if (rb_Kill_Service.Checked)
                                throw new ApplicationException("Kill Service명을 입력하세요.");
                        }

                        cmdItem.IsStop = true;
                        if (rb_Kill_Proc.Checked)
                            cmdItem.IsStopService = false;
                        else if (rb_Kill_Service.Checked)
                            cmdItem.IsStopService = true;
                        cmdItem.StopName = txt_Kill_FileName.Text;
                    }

                    if (chk_Upload.Checked)
                    {
                        if (!CheckEmpty(txt_Upload_Path.Text))
                            throw new ApplicationException("Upload할 경로를 입력하세요.");
                        if (!CheckEmpty(txt_Upload_LocalPath.Text))
                            throw new ApplicationException("Upload할 파일를 선택하세요.");

                        cmdItem.IsUpdate = true;
                        
                        int nIndex = txt_Upload_LocalPath.Text.LastIndexOf('\\');
                        string strFileName = txt_Upload_LocalPath.Text.Substring(nIndex + 1);

                        //if (txt_Upload_Path.Text.Substring(txt_Upload_Path.Text.Length - 2) != "\\")
                        //    cmdItem.UpdateName = txt_Upload_Path.Text + "\\";
                        //else
                        cmdItem.UpdateName = txt_Upload_Path.Text;
                        cmdItem.SearchPath = MakePath(txt_UploadServerPath.Text, strFileName, false);

                        string strError;
                        FileAttributes attr = File.GetAttributes(txt_Upload_LocalPath.Text);
                        if ((attr & FileAttributes.Directory) == FileAttributes.Directory)
                            DBUtility2.UpDownManager.UploadFolder(txt_Upload_LocalPath.Text, m_dbMgr.WebServerURL, out strError, "");
                        else
                            DBUtility2.UpDownManager.UploadFile(txt_Upload_LocalPath.Text, m_dbMgr.WebServerURL, out strError, "");

                        if (strError.Length > 0)
                            throw new ApplicationException(strError);
                    }

                    if (chk_Start.Checked)
                    {
                        if (!CheckEmpty(txt_Start_FileName.Text))
                        {
                            if (rb_Start_Proc.Checked)
                                throw new ApplicationException("Start Process명을 입력하세요.");
                            else if (rb_Start_Service.Checked)
                                throw new ApplicationException("Start Service명을 입력하세요.");
                        }

                        if (rb_Start_Proc.Checked && !txt_Start_FileName.Text.Contains(@"\"))
                            throw new ApplicationException(@"Full path 입력하세요. ex)C:\TTSServer\TTSServer.exe");

                        cmdItem.IsStart = true;
                        if (rb_Start_Proc.Checked)
                            cmdItem.IsStartService = false;
                        else if (rb_Start_Service.Checked)
                            cmdItem.IsStartService = true;
                        cmdItem.StartName = txt_Start_FileName.Text;
                    }
                }
                else if (commandType == CommandType.DOWNLOAD)
                {
                    if (!CheckEmpty(txt_Down_Path.Text))
                        throw new ApplicationException("Download할 폴더(파일)경로를 입력하세요.");

                    lblDownloadLog.Text = "";
                    
                    cmdItem.SearchPath = txt_Down_Path.Text;

                    int nIndex = txt_Down_Path.Text.LastIndexOf('\\');
                    string strFileName = txt_Down_Path.Text.Substring(nIndex + 1);

                    if (!strFileName.Contains('.') || strFileName.Substring(strFileName.LastIndexOf('.') + 1).Length != 3)
                    {
                        strFileName += ".zip";
                    }
                    
                    string strFilePath = MakePath(m_strDownloadLocalPath, strFileName, false); 

                    string strError = "";
                    if (DBUtility2.UpDownManager.DownloadFile(txt_Down_Path.Text, m_strDownloadLocalPath + "\\" + strFileName, m_dbMgr.WebServerURL, out strError))
                    {                        
                        lblDownloadLog.Text = strFilePath + " 다운로드 완료";
                        lblDownloadLog.ForeColor = Color.Green;
                    }
                    else
                    {
                        if (strError.Length > 0)
                            throw new ApplicationException(strError);

                        lblDownloadLog.Text = strFilePath + " 다운로드 실패";
                        lblDownloadLog.ForeColor = Color.Red;
                    }
                }
                else if (commandType == CommandType.SDMS_UPDATE)
                {
                    if (!CheckEmpty(txt_SDMSUpdate_LocalPath.Text))
                        throw new ApplicationException("Zip 파일을 선택하세요.");

                    int nIndex = txt_SDMSUpdate_LocalPath.Text.LastIndexOf('\\');
                    string strFileName = txt_SDMSUpdate_LocalPath.Text.Substring(nIndex + 1);
                    
                    string strError = "";
                    DBUtility2.UpDownManager.UploadFile(txt_SDMSUpdate_LocalPath.Text, m_dbMgr.WebServerURL, out strError, "");
                    if (strError.Length > 0)
                        throw new ApplicationException(strError);

                    cmdItem.UpdateName = strFileName;
                }
                else if (commandType == CommandType.SOP_SERVER_RESTART)
                { 
                    cmdItem.IsStop = true;
                    cmdItem.IsStopService = true;
                    cmdItem.StopName = "SOPServer";
                    cmdItem.IsStart = true;
                    cmdItem.IsStartService = true;
                    cmdItem.StartName = "SOPServer";                    
                }
                else if (cmdItem.CmdType == CommandType.FILE_COPY)
                {
                    if (!CheckEmpty(txtFileCopySourceFileName.Text) || !CheckEmpty(txtFileCopyDestFileName.Text))
                        throw new ApplicationException("출발지/목적지를 모두 입력하세요.");

                    cmdItem.SearchPath = txtFileCopySourceFileName.Text + "@" + txtFileCopyDestFileName.Text; // @로 구분 하자

                    if (chkFileCopyDelete.Checked)
                        cmdItem.IsStop = true;
                }

                if (commandType != CommandType.DOWNLOAD)
                {
                    if (sendCmd.Execute(m_dbMgr, cmdItem))
                    {
                        MessageBox.Show("Command 전송 완료");
                    } 
                }
            }
            catch (ApplicationException app)
            {
                MessageBox.Show(app.Message);
            }
        }
        #endregion
        
        private void btn_OpenFile_Click(object sender, EventArgs e)
        {             
            OpenFileDialog dialog = new OpenFileDialog();
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                Button btn = sender as Button;
                if (btn == btn_AgentOpenFile)
                    txt_AgentPath.Text = dialog.FileName;
                else if (btn == btn_UploadOpenFile)
                    txt_Upload_LocalPath.Text = dialog.FileName;
            }
        }

        private void btn_UploadOpenFolder_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                Button btn = sender as Button;
                txt_Upload_LocalPath.Text = dialog.SelectedPath;
            }
        }

        private void btn_DownloadLogFile_Click(object sender, EventArgs e)
        {
            try
            {
                string strTXTFile = "SDMSAgent.log";
                string strServerLogFilePath = MakePath(m_strServerLogFilePath, strTXTFile, false);

                string strLocalLogFilePath = MakePath(m_strDownloadLocalPath, strTXTFile, false);

                List<string> aaa = new List<string>();

                string strError = "";
                if (DBUtility2.UpDownManager.DownloadFile(strServerLogFilePath, strLocalLogFilePath, m_dbMgr.WebServerURL, out strError))
                {
                    if (strError.Length > 0)
                        throw new ApplicationException(strError);

                    if (File.Exists(strLocalLogFilePath))
                    {
                        System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
                        startInfo.FileName = strLocalLogFilePath;
                        System.Diagnostics.Process.Start(startInfo);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        } 

        private string MakePath(string strPath1, string strPath2, bool isUrl)
        {
            if (isUrl) // http://127.0.0.1:8080/SOP/Download/
            {
                if (strPath1.Substring(strPath1.Length - 1) == "/")
                    return strPath1 + strPath2;
                else
                    return strPath1 + "/" + strPath2;
            }
            else // C:\DownloadTemp\
            {
                if (strPath1.Substring(strPath1.Length - 2) == "\\")
                    return strPath1 + strPath2;
                else
                    return strPath1 + "\\" + strPath2;
            } 
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (m_netMgr != null)
                m_netMgr.ReleaseThread();
        }

        private void btn_SDMSUpdateOpenFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                txt_SDMSUpdate_LocalPath.Text = dialog.FileName;
            }
        }

        private void ShowUpdateXML()
        {
            /*string strTag = "/SOP", strXMLFile = "update.xml";
            int nIndex = m_dbMgr.WebServerURL.LastIndexOf(strTag);

            if (nIndex < 0)
            {
                MessageBox.Show(strXMLFile + "이 있는 URL 경로를 확인할 수 없습니다.");
                return;
            }*/

            string strXMLFile = "update.xml";
            string strURL = m_dbMgr.WebServerURL.EndsWith("/") ? m_dbMgr.WebServerURL + "Update/" : m_dbMgr.WebServerURL + "/Update/";
            strURL += strXMLFile;

            string strLocalFile = m_strDownloadLocalPath + "\\" + strXMLFile;

            if (DownloadLocal(strURL, strLocalFile))
            {
                if (File.Exists(strLocalFile))
                {
                    System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
                    startInfo.FileName = "notepad.exe";
                    startInfo.Arguments = strLocalFile;
                    System.Diagnostics.Process.Start(startInfo);
                }
                else
                    MessageBox.Show(strURL + " download에 실패하였습니다.");
            }
            else
                MessageBox.Show(strURL + " download에 실패하였습니다.");
        }

        private void UpdateSDMS()
        {
            if (m_netMgr == null || m_netMgr.IsConnected == false)
            {
                MessageBox.Show("서버와 연결할 수 없습니다.");
                return;
            }

            m_netMgr.SendUpdateSystem();
        }
        

        private List<CustomComboBoxItem> cbItems = new List<CustomComboBoxItem>();
        private void SetComboBoxList()
        {
            cbItems.Add(new CustomComboBoxItem("--선택하세요--", CommandType.NONE));
            cbItems.Add(new CustomComboBoxItem("Agent Update", CommandType.AGENT_UPDATE));
            cbItems.Add(new CustomComboBoxItem("서비스 목록 보기", CommandType.GET_SERVICE_LIST));
            cbItems.Add(new CustomComboBoxItem("프로세스 목록 보기", CommandType.GET_PROC_LIST));
            cbItems.Add(new CustomComboBoxItem("파일(하위 폴더) 목록 보기", CommandType.GET_FILE_LIST));
            cbItems.Add(new CustomComboBoxItem("파일 Upload / 프로세스(서비스) 중지,시작,재시작", CommandType.UPDATE));
            cbItems.Add(new CustomComboBoxItem("서버에 있는 파일 Download", CommandType.DOWNLOAD));
            cbItems.Add(new CustomComboBoxItem("SDMS update (SDMS 전용 update, 버전.zip만 가능)", CommandType.SDMS_UPDATE));
            cbItems.Add(new CustomComboBoxItem("SOPServer Restart", CommandType.SOP_SERVER_RESTART));
            cbItems.Add(new CustomComboBoxItem("파일 복사", CommandType.FILE_COPY));

            foreach (CustomComboBoxItem item in cbItems)
            {
                cbCommand.Items.Add(item);
            }

            cbCommand.SelectedIndex = 0;
            cbCommand.DisplayMember = "strDisplayName";
            cbCommand.ValueMember = "customCommandType";
            cbCommand.DropDownStyle = ComboBoxStyle.DropDownList;
        }

        private void cbCommand_SelectedIndexChanged(object sender, EventArgs e)
        {
            pnAgentUpdate.Visible = pnGetProcList.Visible = pnGetFileList.Visible = pnUpdate.Visible = pnDownload.Visible 
                = pnSdmsUpdate.Visible = pnFileCopy.Visible = btnRefreshDirectory.Visible = treeView1.Visible = txtDirectoryPath.Visible = false;

            CustomComboBoxItem selectedItem = cbCommand.SelectedItem as CustomComboBoxItem;
            if (selectedItem == null)
                return;

            CommandType selectedCommandType = selectedItem.customCommandType;
            if (selectedCommandType == CommandType.AGENT_UPDATE)
                pnAgentUpdate.Visible = true;
            else if (selectedCommandType == CommandType.GET_PROC_LIST)
            {
                pnGetProcList.Visible = true;
                chk_GetAllProc_CheckedChanged(chk_GetAllProc, e);
            }
            else if (selectedCommandType == CommandType.GET_FILE_LIST)
            {
                pnGetFileList.Visible = true;
                btnRefreshDirectory.Visible = true;
                treeView1.Visible = true;
                txtDirectoryPath.Visible = true;
            }
            else if (selectedCommandType == CommandType.UPDATE)
                pnUpdate.Visible = true;
            else if (selectedCommandType == CommandType.DOWNLOAD)
                pnDownload.Visible = true;
            else if (selectedCommandType == CommandType.SDMS_UPDATE)
                pnSdmsUpdate.Visible = true;
            else if (selectedCommandType == CommandType.FILE_COPY)
                pnFileCopy.Visible = true;

            if (selectedItem == null)
                throw new ApplicationException("전송할 Command를 선택하세요.");
        }

        private void btnSDMSUpdateShowXML_Click(object sender, EventArgs e)
        {
            ShowUpdateXML();
        }

        private void btnSDMSUpdateNow_Click(object sender, EventArgs e)
        {
            UpdateSDMS();
        }

        private void RefreshDirecotry()
        {
            try
            {
                treeView1.Nodes.Clear();
                                
                string strTXTFile = "SDMSAgent_Drive.log";                
                string strServerLogFilePath = MakePath(m_strServerLogFilePath, strTXTFile, false);

                string strLocalLogFilePath = MakePath(m_strDownloadLocalPath, strTXTFile, false);
                
                List<string> aaa = new List<string>();

                string strError = "";
                if (DBUtility2.UpDownManager.DownloadFile(strServerLogFilePath, strLocalLogFilePath, m_dbMgr.WebServerURL, out strError))
                {
                    if (File.Exists(strLocalLogFilePath))
                    {
                        using (StreamReader reader = new StreamReader(strLocalLogFilePath))
                        {
                            string line;
                            while ((line = reader.ReadLine()) != null)
                            {
                                aaa.Add(line);
                            }
                        }
                    }
                    else
                        throw new ApplicationException("서버 경로 : " + strServerLogFilePath + " 로컬 경로 : " + strLocalLogFilePath + " download에 실패하였습니다.");
                }
                else
                    throw new ApplicationException("서버 경로 : " + strServerLogFilePath + " 로컬 경로 : " + strLocalLogFilePath + " download에 실패하였습니다.");

                //treeView1.Nodes.Add("Root", "Root");
                
                foreach (string aa in aaa)
                {
                    string a = aa.Remove(0, 3);

                    int nAIndex = a.LastIndexOf("\\");

                    if (nAIndex < 0)
                        return;

                    string strLastFolderName = a.Substring(nAIndex + 1);
                    string strFolderPath = a.Substring(0, nAIndex);

                    TreeNode node = FindNode(strFolderPath, strLastFolderName, treeView1.Nodes);

                    TreeNode newNode = new TreeNode();
                    newNode.Name = strFolderPath + "\\" + strLastFolderName;
                    newNode.Text = strLastFolderName;

                    if (aa.Substring(0, 3) == "[D]") //Directory
                    {
                        newNode.ImageIndex = 0;

                        if (node == null)
                            treeView1.Nodes.Add(newNode);
                        else
                        {
                            node.Nodes.Add(newNode);
                        }                           
                    }
                    else if (aa.Substring(0, 3) == "[F]") //File
                    {
                        newNode.ImageIndex = 1;
                        if (node == null)
                            continue;
                        else
                            node.Nodes.Add(newNode);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private TreeNode FindNode(string key, string text, TreeNodeCollection nodes = null)
        {
            if (nodes == null)
            {
                //if (treeView1.Nodes == null)
                //    return null;

                //nodes = treeView1.Nodes;

                return null;
            }

            int nAIndex = key.LastIndexOf("\\");
            if (nAIndex < 0)
                return null;

            string strLastName = key.Substring(nAIndex + 1);
            string strFolderPath = key.Substring(0, nAIndex);

            foreach (TreeNode node in nodes)
            {
                if (node.Name == key)
                    return node;

                TreeNode findNode = FindNode(key, text, node.Nodes);
                if (findNode != null)
                    return findNode;
            }

            return null;
        }

        private void btnRefreshDirectory_Click(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;
            RefreshDirecotry();
            this.Cursor = Cursors.Default;
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node == null)
            {
                txtDirectoryPath.Text = "";
                return;
            }

            txtDirectoryPath.Text = e.Node.Name;
        }

        private void btnNewUpdateXML_Click(object sender, EventArgs e)
        {
            try
            {
                XmlTextWriter writer = new XmlTextWriter("update.xml", System.Text.Encoding.UTF8);

                writer.Formatting = Formatting.Indented;
                writer.WriteStartDocument();

                WriteUpdateFile(writer);

                writer.WriteEndDocument();
                writer.Close();

                System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo("notepad.exe");
                startInfo.Arguments = "update.xml";
                System.Diagnostics.Process.Start(startInfo);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine("btnNewUpdateXML_Click Error : " + ex.Message);
            }
        }

        private void WriteUpdateFile(XmlTextWriter writer)
        {
            try
            {
                writer.WriteStartElement("update");

                writer.WriteStartElement("type");
                writer.WriteString("AutoUpdater");
                writer.WriteFullEndElement();

                writer.WriteStartElement("versions");

                writer.WriteStartElement("lastVersion");
                writer.WriteString("1.000");
                writer.WriteFullEndElement();

                WriteVersion(writer, "1.000");

                // versions
                writer.WriteFullEndElement();

                // update
                writer.WriteFullEndElement();
            }
            catch (Exception e)
            {
                System.Diagnostics.Trace.WriteLine("WriteUpdateFile Error : " + e.Message);
            }
        }

        private void WriteVersion(XmlTextWriter writer, string strVersion)
        {
            try
            {
                writer.WriteStartElement("version");

                writer.WriteStartAttribute("id");
                writer.WriteString(strVersion);
                writer.WriteEndAttribute();

                writer.WriteStartElement("name");
                writer.WriteString(strVersion);
                writer.WriteFullEndElement();

                DateTime dtNow = DateTime.Now;
                string strDate = string.Format("{0}-{1:00}-{2:00}", dtNow.Year, dtNow.Month, dtNow.Day);

                writer.WriteStartElement("date");
                writer.WriteString(strDate);
                writer.WriteFullEndElement();

                writer.WriteStartElement("forceUpdate");
                writer.WriteString("false");
                writer.WriteFullEndElement();

                writer.WriteStartElement("location");
                writer.WriteString(strVersion);
                writer.WriteFullEndElement();

                writer.WriteStartElement("target");

                writer.WriteStartAttribute("file");
                writer.WriteString("filelist.lst");
                writer.WriteEndAttribute();

                writer.WriteFullEndElement();

                writer.WriteStartElement("revision");
                writer.WriteString(strVersion);
                writer.WriteFullEndElement();

                // version
                writer.WriteFullEndElement();
            }
            catch (Exception e)
            {
                System.Diagnostics.Trace.WriteLine("WriteVersion Error : " + e.Message);
            }
        }
    }

    public class CustomComboBoxItem
    {
        public string strDisplayName { get; set; }
        
        public CommandType customCommandType { get; set; }

        public CustomComboBoxItem(string displayName, CommandType cmdType)
        {
            this.strDisplayName = displayName;
            this.customCommandType = cmdType;
        }
    }
}
