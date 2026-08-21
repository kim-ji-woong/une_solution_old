using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DBUtility;
using System.Diagnostics;
using Newtonsoft.Json.Linq;
using SDMSAgent;

namespace SDMSCommander
{
    public partial class MainForm : Form
    {
        private WebDBManager m_dbMgr = null;
         
        private SendCommand sendCmd = null;
        private Timer timer = null;
        // Download 요청 후 로컬PC에 Download 됐는지까지 확인하기 위한 List
        private List<string> m_DownloadList = new List<string>();

        private int m_nSiteID = 1;
        private static MainForm m_instance = null;
        private Network.NetworkManager m_netMgr = null;

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
        private string m_strDownloadTomcatPath
        {
            get { return txt_DownloadTomcatPath.Text; }
        }
        private string m_strLogFilePath
        {
            get { return txt_LogFilePath.Text; }
        }
        private string m_strUploadJspFilePath
        {
            get { return txt_uploadJsp.Text; }
        }

        public MainForm()
        {
            InitializeComponent();
            m_instance = this;

            treeView1.ImageList = imageList1;
            this.Size = new Size(984, 552);

            sendCmd = new SendCommand();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            pnAgentUpdate.Location = pnGetProcList.Location = pnGetFileList.Location = pnUpdate.Location = pnDownload.Location = pnSdmsUpdate.Location = pnFileCopy.Location = new Point(12, 44);
            treeView1.Location = new Point(pnGetFileList.Location.X, pnGetFileList.Location.Y + pnGetFileList.Height + 10);
            btnRefreshDirectory.Location = new Point(treeView1.Location.X, treeView1.Location.Y + treeView1.Height + 10);
            txtDirectoryPath.Location = new Point(btnRefreshDirectory.Location.X + btnRefreshDirectory.Width + 10, btnRefreshDirectory.Location.Y);

            ConnectDB();
            
            m_netMgr = new Network.NetworkManager(m_dbMgr, m_nSiteID);

            SetComboBoxList();

            timer = new Timer();
            timer.Interval = 3000;
            timer.Tick += timer_Tick;
        }

        void timer_Tick(object sender, EventArgs e)
        {
            if (m_DownloadList.Count == 0)
            {
                return;
            }

            ReadCommandResult();
        }

        private void ReadCommandResult()
        {
            string strSQL = "Select ID, TimeStamp, SearchPath from SDMSCommandHistory Where Result=1 And Command = " + (int)SDMSAgent.CommandType.DOWNLOAD;
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null)
                return;
             
            int nResultCount = arrResult.Count; 

            for (int i = 0; i < nResultCount; i += 3)
            { 
                int nID = WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                VariousData<DateTime> dtTimeStamp = WebDBManager.GetDateTimeField(arrResult[i + 1]); 
                string strSearchPath = WebDBManager.GetStringField(arrResult[i + 2].ToString(), "");

                string strParam = dtTimeStamp.Data.ToString("yyyyMMddHHmmss") + "_" + (int)SDMSAgent.CommandType.DOWNLOAD;
                if (m_DownloadList.Contains(strParam))
                {
                    int nIndex = strSearchPath.LastIndexOf('\\');
                    string strFileName = strSearchPath.Substring(nIndex + 1);

                    if (!strFileName.Contains('.') || strFileName.Substring(strFileName.LastIndexOf('.') + 1).Length != 3)
                    {
                        strFileName += ".zip";
                    }

                    string strFilePath = MakePath(m_strDownloadLocalPath, strFileName, false);
                    string strDownPath = MakePath(m_strDownloadTomcatPath, strFileName, true);

                    if (DownloadLocal(strDownPath, strFilePath))
                    {
                        lblDownloadLog.Text = strFilePath + " 다운로드 완료";
                        lblDownloadLog.ForeColor = Color.Green;
                    }
                    else
                    {
                        lblDownloadLog.Text = strFilePath + " 다운로드 실패";
                        lblDownloadLog.ForeColor = Color.Red;
                    }

                    m_DownloadList.Remove(strParam);
                }

                m_dbMgr.GetResultData("UPDATE SDMSCommandHistory SET Result=2 WHERE ID=" + nID, 0);
            }

            if (m_DownloadList.Count == 0)
                timer.Stop();
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
        private void rb_ConnectType_CheckedChanged(object sender, EventArgs e)
        {
            if (rb_ConnectType_SiteID.Checked)
            {
                groupBox_SiteID.Enabled = true;
                groupBox_String.Enabled = false;
            }
            else if (rb_ConnectType_String.Checked)
            {
                groupBox_SiteID.Enabled = false;
                groupBox_String.Enabled = true;
            }
        }

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

        private void chg_Start_SameInfo_CheckedChanged(object sender, EventArgs e)
        {
            //if (chg_Start_SameInfo.Checked)
            //{
            //    txt_Start_FileName.Text = txt_Kill_FileName.Text;
            //}
        }

        private void txt_Kill_FileName_TextChanged(object sender, EventArgs e)
        {
            //if (chg_Start_SameInfo.Checked)
            //    txt_Start_FileName.Text = txt_Kill_FileName.Text;
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
            if (rb_ConnectType_SiteID.Checked)
            {
                int siteID = 0;
                int.TryParse(txt_SiteId.Text, out siteID);
                m_dbMgr = new WebDBManager(siteID);
                m_nSiteID = siteID;
            }
            else if (rb_ConnectType_String.Checked)
            {
                m_dbMgr = new WebDBManager(0);
                m_dbMgr.WebServerURL = txt_WebServerURL.Text;
                m_dbMgr.DatabaseHost = txt_DatabaseHost.Text;
                m_dbMgr.DatabaseName = txt_DatabaseName.Text;
                m_dbMgr.DatabasePort = txt_DatabasePort.Text;
                if (rb_Connect_Mssql.Checked)
                    m_dbMgr.DatabaseType = WebDBManager.DBType.sqlserver;
                else if (rb_Connect_Mysql.Checked)
                    m_dbMgr.DatabaseType = WebDBManager.DBType.mysql;
            }

            bool isSuc = false;
            ArrayList arr = m_dbMgr.GetResultData("SELECT ID FROM Site", 0);
            if (arr == null || arr.Count == 0)
                isSuc = false;            
            else
                isSuc = true;

            if (isSuc)
            {
                label_isConnect.Text = "연결중(" + m_dbMgr.DatabaseName + " " + m_dbMgr.WebServerURL + ")";
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

                SDMSAgent.CommandType commandType = selectedItem.customCommandType;

                if (commandType == SDMSAgent.CommandType.NONE)
                {
                    return;
                }                

                CommandItem cmdItem = new CommandItem();
                cmdItem.TimeStamp = DateTime.Now;
                cmdItem.CmdType = commandType;

                if (commandType == SDMSAgent.CommandType.AGENT_UPDATE)
                {
                    if (!CheckEmpty(txt_AgentPath.Text))
                        throw new ApplicationException("SDMSAgent.exe 파일을 선택하세요");
                    
                    int nIndex = txt_AgentPath.Text.LastIndexOf('\\');
                    string strFileName = txt_AgentPath.Text.Substring(nIndex + 1);
                    
                    FileInfo fi = new FileInfo(txt_AgentPath.Text);
                    byte[] fileContents = File.ReadAllBytes(fi.FullName);
                    UploadMultipart(fileContents, strFileName, "application/octet-stream", m_strUploadJspFilePath);
                }
                else if (commandType == SDMSAgent.CommandType.GET_SERVICE_LIST)
                {
                    
                }
                else if (commandType == SDMSAgent.CommandType.GET_PROC_LIST)
                {
                    if (chk_GetAllProc.Checked)
                    {
                        cmdItem.CmdType = SDMSAgent.CommandType.GET_ALL_PROC_LIST;
                    }
                    else
                    {
                        if (!CheckEmpty(txt_GetProcName.Text))
                            throw new ApplicationException("검색할 프로세스명을 입력하세요.");
                        
                        cmdItem.SearchPath = txt_GetProcName.Text;
                    }
                }
                else if (commandType == SDMSAgent.CommandType.GET_FILE_LIST)
                {
                    if (!CheckEmpty(txt_GetFileListPath.Text))
                        throw new ApplicationException("검색할 폴더 경로를 입력하세요.");
                    
                    cmdItem.SearchPath = txt_GetFileListPath.Text;
                }
                else if (commandType == SDMSAgent.CommandType.UPDATE)
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
                else if (commandType == SDMSAgent.CommandType.DOWNLOAD)
                {
                    if (!CheckEmpty(txt_Down_Path.Text))
                        throw new ApplicationException("Download할 경로 또는 파일경로를 입력하세요.");

                    lblDownloadLog.Text = "";
                    
                    cmdItem.SearchPath = txt_Down_Path.Text;
                }
                else if (commandType == SDMSAgent.CommandType.SDMS_UPDATE)
                {
                    if (!CheckEmpty(txt_SDMSUpdate_LocalPath.Text))
                        throw new ApplicationException("Zip 파일을 선택하세요.");
                    
                }
                else if (commandType == SDMSAgent.CommandType.SOP_SERVER_RESTART)
                { 
                    cmdItem.IsStop = true;
                    cmdItem.IsStopService = true;
                    cmdItem.StopName = "SOPServer";
                    cmdItem.IsStart = true;
                    cmdItem.IsStartService = true;
                    cmdItem.StartName = "SOPServer";                    
                }

                if (cmdItem.CmdType == SDMSAgent.CommandType.UPDATE && cmdItem.IsUpdate)
                {
                    int nIndex = txt_Upload_LocalPath.Text.LastIndexOf('\\');
                    string strFileName = txt_Upload_LocalPath.Text.Substring(nIndex + 1);

                    if (txt_Upload_Path.Text.Substring(txt_Upload_Path.Text.Length - 2) != "\\")
                        cmdItem.UpdateName = txt_Upload_Path.Text + "\\" + strFileName;
                    else
                        cmdItem.UpdateName = txt_Upload_Path.Text + strFileName;

                    FileInfo fi = new FileInfo(txt_Upload_LocalPath.Text);
                    byte[] fileContents = File.ReadAllBytes(fi.FullName);
                    UploadMultipart(fileContents, strFileName, "application/octet-stream", m_strUploadJspFilePath);
                }
                else if (cmdItem.CmdType == SDMSAgent.CommandType.SDMS_UPDATE)
                {
                    int nIndex = txt_SDMSUpdate_LocalPath.Text.LastIndexOf('\\');
                    string strFileName = txt_SDMSUpdate_LocalPath.Text.Substring(nIndex + 1);

                    FileInfo fi = new FileInfo(txt_SDMSUpdate_LocalPath.Text);
                    byte[] fileContents = File.ReadAllBytes(fi.FullName);
                    UploadMultipart(fileContents, strFileName, "application/octet-stream", m_strUploadJspFilePath);

                    cmdItem.UpdateName = strFileName;
                }
                else if (cmdItem.CmdType == SDMSAgent.CommandType.FILE_COPY)
                {
                    if (!CheckEmpty(txtFileCopySourceFileName.Text) || !CheckEmpty(txtFileCopyDestFileName.Text))
                        throw new ApplicationException("출발지/목적지를 모두 입력하세요.");

                    cmdItem.SearchPath = txtFileCopySourceFileName.Text + "@" + txtFileCopyDestFileName.Text; // @로 구분 하자

                    if (chkFileCopyDelete.Checked)
                        cmdItem.IsStop = true;
                }

                if (sendCmd.Execute(m_dbMgr, cmdItem))
                {
                    MessageBox.Show("Command 전송 완료");

                    if (cmdItem.CmdType == SDMSAgent.CommandType.DOWNLOAD)
                        timer.Start();
                }

                if (cmdItem.CmdType == SDMSAgent.CommandType.DOWNLOAD)
                {
                    m_DownloadList.Add(cmdItem.TimeStamp.ToString("yyyyMMddHHmmss") + "_" + (int)cmdItem.CmdType);
                }
            }
            catch (ApplicationException app)
            {
                MessageBox.Show(app.Message);
            }
        } 
        #endregion  

        public async void UploadMultipart(byte[] file, string filename, string contentType, string url)
        {
            HttpClient httpClient = new HttpClient();
            MultipartFormDataContent form = new MultipartFormDataContent();

            form.Add(new StringContent("sa"), "ID");
            form.Add(new StringContent("9449966Ab"), "PASS");

            ByteArrayContent binaryContent = new ByteArrayContent(file, 0, file.Length);
            binaryContent.Headers.Add("Content-Type", contentType);
            form.Add(binaryContent, "file", Path.GetFileName(filename));
            HttpResponseMessage response = await httpClient.PostAsync(url, form);
            
            try
            {
                response.EnsureSuccessStatusCode();     //response false일때 예외 발생. Status code = 200, JSP Custom Result Code 아님. 
            }
            catch (Exception ex)
            {
                //예외일때 처리 (웹서버로 제대로 업로드가 안된 경우---- 500인 경우 서버쪽에서 정상적으로 exception처리가 되지 않음. 400번대인 경우 Tomcat동작 체크할 것.)
                MessageBox.Show(ex.Message);                
                //return false;
            }

            /* 아래의 result에서 Detail한 코드 확인 */

            string result = response.Content.ReadAsStringAsync().Result;

            JObject resultJson = JObject.Parse(result);

            // Web Result [Code : 결과] Json
            //  { code : int, message : "" or Json }	
            //
            // code : 100 인 경우 파일 업로드 성공
            // message : {
            //		filename : "",     // (string)  --- 업로드된 파일 이름.
            //      size : 121231 (int)
            // }
            // 100 이 아닌 경우 메시지는 exception(error) 메시지, 또는 아래의 에러에 대한 메시지. (string)
            // 110 : multipart/form-data 형식이 아닙니다. 폼 데이터 형식으로 전송해야 합니다.
            // 120 : ID 또는 PASS워드가 안 맞음
            // 130 : File이 upload 되지 않음		
            // 140 : File이 클라이언트로부터 전달되지 않음.
            //  -1 : Unknown Error;

            int resultcode = resultJson["code"].ToObject<int>();
            if (resultcode == 100)              //정상
            {
                JObject message = resultJson["message"].ToObject<JObject>();
                string nameOfSavedFile = message["filename"].ToString();
                long sizeOfSavedFile = message["size"].ToObject<long>();        //여기서 업로드한 파일 사이즈와 이름이 같은지 체크해보면 complete.
                MessageBox.Show("업로드 성공: 파일이름 [" + nameOfSavedFile + "]   Size : " + sizeOfSavedFile);
            }
            else
            {
                String message = resultJson["message"].ToString();
                MessageBox.Show("Error Code : [" + resultcode + "]   Message: " + message);
            }

            //if (result.Contains("Upload Finished")) //&& result.IndexOf(filename) > 0)  //업로드가 성공, upload 성공한 파일이름 확인
            //{
            //    Debug.WriteLine("result : " + result);
            //    //업로드 성공시 처리.    
            //    //return true;
            //}
            httpClient.Dispose();

        }  

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

        private void btn_DownloadLogFile_Click(object sender, EventArgs e)
        {
            int nIndex = m_strLogFilePath.LastIndexOf('/');
            string strFileName = m_strLogFilePath.Substring(nIndex + 1); 

            string strFilePath = MakePath(m_strDownloadLocalPath, strFileName, false);

            if (!Directory.Exists(m_strDownloadLocalPath))
                Directory.CreateDirectory(m_strDownloadLocalPath);

            if (DownloadLocal(m_strLogFilePath, strFilePath))
            {
                if (File.Exists(strFilePath))
                {
                    System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
                    startInfo.FileName = strFilePath;
                    System.Diagnostics.Process.Start(startInfo);
                }
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
            string strTag = "/SOP", strXMLFile = "update.xml";
            int nIndex = m_dbMgr.WebServerURL.LastIndexOf(strTag);

            if (nIndex < 0)
            {
                MessageBox.Show(strXMLFile + "이 있는 URL 경로를 확인할 수 없습니다.");
                return;
            }

            string strURL = m_dbMgr.WebServerURL.Substring(0, nIndex);
            strURL += "/update/" + strXMLFile;

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
            m_netMgr.ClientProvider.SendUpdateSystem();
        }
        

        private List<CustomComboBoxItem> cbItems = new List<CustomComboBoxItem>();
        private void SetComboBoxList()
        {
            cbItems.Add(new CustomComboBoxItem("--선택하세요--", SDMSAgent.CommandType.NONE));
            cbItems.Add(new CustomComboBoxItem("Agent Update", SDMSAgent.CommandType.AGENT_UPDATE));
            cbItems.Add(new CustomComboBoxItem("서비스 목록 보기", SDMSAgent.CommandType.GET_SERVICE_LIST));
            cbItems.Add(new CustomComboBoxItem("프로세스 목록 보기", SDMSAgent.CommandType.GET_PROC_LIST));
            cbItems.Add(new CustomComboBoxItem("파일(하위 폴더) 목록 보기", SDMSAgent.CommandType.GET_FILE_LIST));
            cbItems.Add(new CustomComboBoxItem("파일 Upload / 프로세스(서비스) 중지,시작,재시작", SDMSAgent.CommandType.UPDATE));
            cbItems.Add(new CustomComboBoxItem("서버에 있는 파일 Download", SDMSAgent.CommandType.DOWNLOAD));
            cbItems.Add(new CustomComboBoxItem("SDMS update (SDMS 전용 update, 버전.zip만 가능)", SDMSAgent.CommandType.SDMS_UPDATE));
            cbItems.Add(new CustomComboBoxItem("SOPServer Restart", SDMSAgent.CommandType.SOP_SERVER_RESTART));
            cbItems.Add(new CustomComboBoxItem("파일 복사", SDMSAgent.CommandType.FILE_COPY));

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

            SDMSAgent.CommandType selectedCommandType = selectedItem.customCommandType;
            if (selectedCommandType == SDMSAgent.CommandType.AGENT_UPDATE)
                pnAgentUpdate.Visible = true;
            else if (selectedCommandType == SDMSAgent.CommandType.GET_PROC_LIST)
            {
                pnGetProcList.Visible = true;
                chk_GetAllProc_CheckedChanged(chk_GetAllProc, e);
            }
            else if (selectedCommandType == SDMSAgent.CommandType.GET_FILE_LIST)
            {
                pnGetFileList.Visible = true;
                btnRefreshDirectory.Visible = true;
                treeView1.Visible = true;
                txtDirectoryPath.Visible = true;
            }
            else if (selectedCommandType == SDMSAgent.CommandType.UPDATE)
                pnUpdate.Visible = true;
            else if (selectedCommandType == SDMSAgent.CommandType.DOWNLOAD)
                pnDownload.Visible = true;
            else if (selectedCommandType == SDMSAgent.CommandType.SDMS_UPDATE)
                pnSdmsUpdate.Visible = true;
            else if (selectedCommandType == SDMSAgent.CommandType.FILE_COPY)
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

                string strTag = "/SOP", strTXTFile = "SDMSAgent_Drive.txt";
                int nIndex = m_dbMgr.WebServerURL.LastIndexOf(strTag);

                if (nIndex < 0)
                {
                    MessageBox.Show(strTXTFile + "이 있는 URL 경로를 확인할 수 없습니다.");
                    return;
                }

                string strURL = m_dbMgr.WebServerURL.Substring(0, nIndex);
                strURL += "/SOP/" + strTXTFile;

                string strLocalFile = m_strDownloadLocalPath + "\\" + strTXTFile;

                List<string> aaa = new List<string>();

                if (DownloadLocal(strURL, strLocalFile))
                {
                    if (File.Exists(strLocalFile))
                    {
                        using (StreamReader reader = new StreamReader(strLocalFile))
                        {
                            string line;
                            while ((line = reader.ReadLine()) != null)
                            {
                                aaa.Add(line);
                            }
                        }
                    }
                    else
                        throw new ApplicationException(strURL + " download에 실패하였습니다.");
                }
                else
                    throw new ApplicationException(strURL + " download에 실패하였습니다.");

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
    }

    public class CustomComboBoxItem
    {
        public string strDisplayName { get; set; }
        
        public SDMSAgent.CommandType customCommandType { get; set; }

        public CustomComboBoxItem(string displayName, SDMSAgent.CommandType cmdType)
        {
            this.strDisplayName = displayName;
            this.customCommandType = cmdType;
        }
    }
}
