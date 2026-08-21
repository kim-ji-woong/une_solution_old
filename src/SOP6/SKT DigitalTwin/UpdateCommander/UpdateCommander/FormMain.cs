using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using DBUtility2;
using System.Configuration;
using System.Collections;

namespace UpdateCommander
{
    public partial class FormMain : Form
    {
        public enum CommandType { Update = 0, Stop, Start };
        public enum CommandResultType { UnknownCommand = -1, UpdateSeccess = 0, UpdateFail, StopSuccess, StopFail, StartSuccess, StartFail };

        private const string ServerVersionFile = "server.ver";
        private const string ClientVersionFile = "client.ver";
        private const string InitVersion = "V1.000";

        private WebDBManager m_dbMgr = null;
        private int m_nSelectedSiteID = 0;

        public FormMain()
        {
            InitializeComponent();
            cboSite.SelectedIndex = 0;

            SetDBManager();
        }

        private bool SetDBManager()
        {
            string strSiteID = ConfigurationManager.AppSettings.Get("siteid");
            
            if (strSiteID == null || strSiteID.Length == 0)
                return false;

            int nSiteID;

            if (int.TryParse(strSiteID, out nSiteID) == false)
                return false;

            m_dbMgr = new WebDBManager(nSiteID);
            return true;
        }

        private void btnServerUpdate_Click(object sender, EventArgs e)
        {
            OpenUpdateZipFile(textBoxServerUpdate);
        }

        private void btnClientUpdate_Click(object sender, EventArgs e)
        {
            OpenUpdateZipFile(textBoxClientUpdate);
        }

        private void OpenUpdateZipFile(TextBox textBox)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "zip files (*.zip)|*.zip";
                openFileDialog.FilterIndex = 2;
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    textBox.Text = openFileDialog.FileName;
                }
            }
        }

        private void radioServer_CheckedChanged(object sender, EventArgs e)
        {
            if (radioUpdateServer.Checked)
            {
                textBoxServerUpdate.Enabled = btnServerUpdate.Enabled = true;
            }
            else
            {
                textBoxServerUpdate.Enabled = btnServerUpdate.Enabled = false;
            }
        }

        private void radioClient_CheckedChanged(object sender, EventArgs e)
        {
            if (radioUpdateClient.Checked)
            {
                textBoxClientUpdate.Enabled = btnClientUpdate.Enabled = true;
            }
            else
            {
                textBoxClientUpdate.Enabled = btnClientUpdate.Enabled = false;
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            int nSelectedSite = cboSite.SelectedIndex;

            if (nSelectedSite == 0)
            {
                MessageBox.Show("업데이트할 사이트를 먼저 선택하세요.");
                return;
            }

            m_nSelectedSiteID = nSelectedSite;

            if (checkBoxServer.Checked == false && checkBoxClient.Checked == false)
            {
                MessageBox.Show("업데이트할 서버 또는 클라이언트를 CheckBox에서 선택하세요.");
                return;
            }

            string strServerCommand, strClientCommand;
            string strServerParameter, strClientParameter;

            if (CheckServer(out strServerCommand, out strServerParameter) == false)
                return;
            if (CheckClient(out strClientCommand, out strClientParameter) == false)
                return;

            DateTime dtNow = DateTime.Now;

            if (SendUpdateQuery(strServerCommand, strServerParameter, strClientCommand, strClientParameter, dtNow) == false)
            {
                MessageBox.Show(m_dbMgr.LastErrorMessage);
                return;
            }

            btnUpdate.Enabled = false;
            
            // 비동기 처리
            CheckUpdateResult(nSelectedSite, cboSite.Text, dtNow);
        }

        private async Task CheckUpdateResult(int nSiteID, string strSiteName, DateTime time)
        {
            string strResult = "";

            // 타임아웃 : 5분
            double timeoutSeconds = 300;
            DateTime dtPrev = DateTime.Now;

            while (true)
            {
                strResult = await GetUpdateResult(nSiteID, time);

                // 에러발생
                if (strResult == null)
                {
                    btnUpdate.Enabled = true;
                    return;
                }
                else if (strResult.Length > 0)
                    break;

                TimeSpan span = DateTime.Now - dtPrev;

                if (span.TotalSeconds >= timeoutSeconds)
                    break;

                await Task.Delay(1000);
            }

            btnUpdate.Enabled = true;

            if (strResult.Length > 0)
                MessageBox.Show("[" + strSiteName + "] : " + strResult);
        }

        private async Task<string> GetUpdateResult(int nSiteID, DateTime time)
        {
            string strTime = string.Format("{0}-{1:00}-{2:00} {3:00}:{4:00}:{5:00}", time.Year, time.Month, time.Day, time.Hour, time.Minute, time.Second);

            string strSQL = string.Format("Select ServerResult, ClientResult, ServerMessage, ClientMessage from AutoUpdateHistory where CommandID = {0}{1:000} and CommandTime = '{2}'",
                m_dbMgr.SiteID, nSiteID, strTime);

            ArrayList arrResult = m_dbMgr.GetResultData(strSQL);

            if (arrResult != null && arrResult.Count >= 4)
            {
                VariousData<int> serverResult = WebDBManager.GetIntField(arrResult[0].ToString());
                VariousData<int> clientResult = WebDBManager.GetIntField(arrResult[1].ToString());
                string strServerResultMessage = WebDBManager.GetStringField(arrResult[2]);
                string strClientResultMessage = WebDBManager.GetStringField(arrResult[3]);

                if (serverResult == null && clientResult == null)
                    return null;
                else if (serverResult == null)
                {
                    return GetResultString(clientResult.Data);
                }
                else if (clientResult == null)
                {
                    return GetResultString(serverResult.Data);
                }
                else
                {
                    string strServerResult = "[서버]" + GetResultString(serverResult.Data);
                    string strClientResult = "[클라이언트]" + GetResultString(clientResult.Data);
                    return strServerResult + "\r\n" + strClientResult;
                }
            }

            return "";
        }

        private string GetResultString(int nResult)
        {
            if (nResult == (int)CommandResultType.StartFail)
                return "[시작] 실패";
            else if (nResult == (int)CommandResultType.StartSuccess)
                return "[시작] 성공";
            else if (nResult == (int)CommandResultType.StopFail)
                return "[종료] 실패";
            else if (nResult == (int)CommandResultType.StopSuccess)
                return "[종료] 성공";
            else if (nResult == (int)CommandResultType.UpdateFail)
                return "[업데이트] 실패";
            else if (nResult == (int)CommandResultType.UpdateSeccess)
                return "[업데이트] 성공";

            return "이거는 성공도 실패도 아닌데...";
        }

        private bool SendUpdateQuery(string strServerCommand, string strServerParameter, string strClientCommand, string strClientParameter, DateTime time)
        {
            string strTime = string.Format("{0}-{1:00}-{2:00} {3:00}:{4:00}:{5:00}", time.Year, time.Month, time.Day, time.Hour, time.Minute, time.Second);

            string strSQL = string.Format("Update AutoUpdate set TimeStamp = '{0}', ServerCommand = {1}, ClientCommand = {2}, ServerParameter = {3}, ClientParameter = {4} where ID = {5}{6:000}",
                strTime, strServerCommand, strClientCommand, strServerParameter, strClientParameter, m_dbMgr.SiteID, m_nSelectedSiteID);

            return m_dbMgr.GetResultData(strSQL) != null;
        }

        private bool CheckServer(out string strCommand, out string strParameter)
        {
            return CheckArrange(out strCommand, out strParameter, checkBoxServer, radioStartServer, radioStopServer, radioUpdateServer, textBoxServerName, textBoxServerUpdate);
        }

        private bool CheckClient(out string strCommand, out string strParameter)
        {
            return CheckArrange(out strCommand, out strParameter, checkBoxClient, radioStartClient, radioStopClient, radioUpdateClient, textBoxClientName, textBoxClientUpdate);
        }

        private string GetVersionFile(bool isServer)
        {
            string strSite = string.Format("_{0}{1:000}", m_dbMgr.SiteID, m_nSelectedSiteID);
            string strVersionFile = isServer ? ServerVersionFile : ClientVersionFile;

            int nIndex = strVersionFile.LastIndexOf('.');

            string strFileName = strVersionFile.Substring(0, nIndex) + strSite;
            return strFileName + strVersionFile.Substring(nIndex);
        }

        private bool CheckArrange(out string strCommand, out string strParameter, CheckBox checkItem, RadioButton radioStart, RadioButton radioStop, RadioButton radioUpdate, TextBox textBoxName, TextBox textBoxUpdate)
        {
            strCommand = strParameter = "NULL";

            if (checkItem.Checked == false)
                return true;

            string strName = textBoxName.Text.Trim();
            string strLabelName = textBoxName == textBoxServerName ? labelServerName.Text.Replace(":", "").Trim() : labelClientName.Text.Replace(":", "").Trim();
            string strVersionFile = GetVersionFile(textBoxName == textBoxServerName);
            //string strVersionFile = textBoxName == textBoxServerName ? ServerVersionFile : ClientVersionFile;

            if (radioStart.Checked)
            {
                if (strName.Length == 0)
                {
                    textBoxName.Focus();
                    MessageBox.Show(strLabelName + "을 먼저 입력하세요.");
                    return false;
                }
                else
                {
                    strCommand = ((int)CommandType.Start).ToString();
                    strParameter = "'" + strName + "'";
                    return true;
                }
            }
            else if (radioStop.Checked)
            {
                if (strName.Length == 0)
                {
                    textBoxName.Focus();
                    MessageBox.Show(strLabelName + "을 먼저 입력하세요.");
                    return false;
                }
                else
                {
                    strCommand = ((int)CommandType.Stop).ToString();
                    strParameter = "'" + strName + "'";
                    return true;
                }
            }
            else if (radioUpdate.Checked)
            {
                string strUpdateFile = textBoxUpdate.Text.Trim();

                if (strUpdateFile.Length == 0)
                {
                    textBoxUpdate.Focus();
                    MessageBox.Show("업데이트할 파일을 먼저 입력하세요.");
                    return false;
                }

                if (File.Exists(strUpdateFile) == false)
                {
                    textBoxUpdate.Focus();
                    MessageBox.Show("존재하지 않는 파일입니다.\r\n" + strUpdateFile);
                    return false;
                }

                if (strUpdateFile.ToLower().EndsWith(".zip") == false)
                {
                    textBoxUpdate.Focus();
                    MessageBox.Show("업데이트할 파일은 zip 파일만 가능합니다.");
                    return false;
                }

                string strErrorMessage;
                if (UploadFile(strUpdateFile, out strErrorMessage) == false)
                {
                    MessageBox.Show(strErrorMessage);
                    return false;
                }

                string strVersion = ReadVersion(strVersionFile);
                strVersion = UpdateVersion(strVersion, strVersionFile);

                if (strVersion == null)
                    return false;

                if (strName.Length == 0)
                {
                    textBoxName.Focus();
                    MessageBox.Show(strLabelName + "을 먼저 입력하세요.");
                    return false;
                }

                int nIndex = strUpdateFile.LastIndexOf('\\');
                string strUpdateFileName = nIndex < 0 ? strUpdateFile.Trim() : strUpdateFile.Substring(nIndex + 1).Trim();

                strCommand = ((int)CommandType.Update).ToString();
                strParameter = string.Format("'{0} {1} {2}'", strVersion, strName, strUpdateFileName);
                return true;
            }

            MessageBox.Show("라디오박스를 통하여 처리할 옵션을 먼저 선택하세요.");
            return false;
        }

        private bool UploadFile(string strFile, out string strErrorMessage)
        {
            string strUploadBase = ConfigurationManager.AppSettings.Get("uploadBase");

            if (strUploadBase == null || strUploadBase.Length == 0)
            {
                strErrorMessage = "Upload 경로를 확인할 수 없습니다.";
                return false;
            }

            if (strUploadBase.EndsWith("\\") == false)
                strUploadBase += "\\";

            return UpDownManager.UploadFile(strFile, m_dbMgr.WebServerURL, out strErrorMessage, string.Format("{0}{1}{2:000}", strUploadBase, m_dbMgr.SiteID, m_nSelectedSiteID));
        }

        private string UpdateVersion(string strVersion, string strVersionFile)
        {
            string strVersionNumber = strVersion.Substring(1).Trim();

            double dVersion;

            if (double.TryParse(strVersionNumber, out dVersion) == false)
            {
                MessageBox.Show("버전을 확인할 수 없습니다.");
                return null;
            }

            dVersion += 0.001;
            string strNewVersion = string.Format("V{0:F3}", dVersion);

            StreamWriter writer = new StreamWriter(strVersionFile, false, System.Text.Encoding.UTF8);
            writer.Write(strNewVersion);
            writer.Close();

            return strNewVersion;
        }

        private string ReadVersion(string strVersionFile)
        {
            if (File.Exists(strVersionFile))
            {
                StreamReader reader = new StreamReader(strVersionFile, System.Text.Encoding.UTF8);
                string strVersion = reader.ReadLine().Trim();
                reader.Close();

                return strVersion;
            }

            StreamWriter writer = new StreamWriter(strVersionFile, false, System.Text.Encoding.UTF8);
            writer.Write(InitVersion);
            writer.Close();

            return InitVersion;
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            if (m_dbMgr == null)
            {
                MessageBox.Show("UpdateCommander.exe.config 파일을 찾을수 없습니다.\r\n프로그램이 종료됩니다.");
                this.Close();
            }
        }
    }
}
