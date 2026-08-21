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

namespace AgentCommander
{
    public partial class FormMain : Form
    {
        private WebDBManager m_dbMgr = new WebDBManager(500);

        public FormMain()
        {
            InitializeComponent();

            // DB Insert 서버
            m_dbMgr.WebServerURL = "http://183.104.147.144:18080/SOP";
            //m_dbMgr.WebServerURL = "http://127.0.0.1:8080/SOP"; //test할때
            m_dbMgr.DatabaseHost = "127.0.0.1";
            m_dbMgr.DatabaseName = "KPX";
            m_dbMgr.DatabaseType = WebDBManager.DBType.mysql;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (radioUpdate.Checked) // Agent Update
                Command.ProcessUpdate(m_dbMgr, Command.AGENT_UPDATE, true);
            else if (radioScreenCapture.Checked) // Screen Capture
                Command.ProcessScreenCapture(m_dbMgr);  
            else if (radioClientUpdate.Checked)
                Command.ProcessUpdate(m_dbMgr, Command.CLIENT_UPDATE, true);
            else if (radioServerUpdate.Checked)
                Command.ProcessUpdate(m_dbMgr, Command.SERVER_UPDATE);
            else if (radioTankServerUpdate.Checked)
                Command.ProcessUpdate(m_dbMgr, Command.TANK_SERVER_UPDATE);
            else if (radioPushServerUpdate.Checked)
                Command.ProcessUpdate(m_dbMgr, Command.PUSH_SERVER_UPDATE);
            else if (radioUserAcceptanceUpdate.Checked)
                Command.ProcessUpdate(m_dbMgr, Command.USER_ACCEPTANCE_UPDATE);
            else if (radioJspFileUpdate.Checked)
            {
                if (textBox_jspFileName.Text.Length == 0)
                {
                    MessageBox.Show("UPDATE할 파일명을 입력하세요");
                    return;
                }
                Command.FileUpdate(m_dbMgr, Command.JSP_UPDATE, textBox_jspFileName.Text);
            }
            else if (radioChkStatus.Checked)
                Command.ProcessUpdate(m_dbMgr, Command.CHECK_STATUS);
            else if (radioServerDllUpdate.Checked)
            {
                if (textBox_serverDll.Text.Length == 0)
                {
                    MessageBox.Show("UPDATE할 파일명을 입력하세요");
                    return;
                }
                Command.FileUpdate(m_dbMgr, Command.SERVER_DLL_UPDATE, textBox_serverDll.Text);
            }
            else if (radioDownloadZipFile.Checked)
            {
                if (DownloadFile(textBoxZipTargetFolderPath, textBoxZipFileName, Command.ZIP_FILE_UPDATE) == false)
                    return;
            }
            else if (radioDownloadNormalFile.Checked)
            {
                if (DownloadFile(textBoxNormalTargetFolderPath, textBoxNormalFileName, Command.NORMAL_FILE_UPDATE) == false)
                    return;
            }
            else if (radioSearchFolder.Checked)
            {
                if (textBoxSearchFolderPath.Text.Trim().Length == 0)
                {
                    textBoxSearchFolderPath.Focus();
                    MessageBox.Show("탐색할 폴더 경로를 입력하세요");
                    return;
                }

                Command.FileUpdate(m_dbMgr, Command.SEARCH_FOLDER, textBoxSearchFolderPath.Text.Trim(), true);
            }
            else
            {
                if (textBox_path.Text.Trim().Length == 0)
                {
                    textBox_path.Focus();
                    MessageBox.Show("경로 또는 파일명을 입력하세요");
                    return;
                }

                if (radioButton_procKill.Checked) // Process Kill
                    Command.FileUpdate(m_dbMgr, Command.PROCESS_KILL, textBox_path.Text, checkBox_areaType.Checked);
                else if (radioButton_procStart.Checked) // Process Start
                    Command.FileUpdate(m_dbMgr, Command.PROCESS_START, textBox_path.Text, checkBox_areaType.Checked);
                else if (radioButton_file.Checked) // File Update
                    Command.FileUpdate(m_dbMgr, Command.FILE_UPDATE, textBox_path.Text, checkBox_areaType.Checked); 
                else if (radioButton_serviceStop.Checked) // Service Stop
                    Command.FileUpdate(m_dbMgr, Command.SERVICE_STOP, textBox_path.Text, checkBox_areaType.Checked);
                else if (radioButton_serviceStart.Checked) // Service Start
                    Command.FileUpdate(m_dbMgr, Command.SERVICE_START, textBox_path.Text, checkBox_areaType.Checked);
            }
            MessageBox.Show("Command 입력 완료");
        }

        private bool DownloadFile(TextBox texboxFolderPath, TextBox textBoxFileName, int nCommandID)
        {
            if (textBoxFileName.Text.Trim().Length == 0)
            {
                textBoxFileName.Focus();
                MessageBox.Show("UPDATE할 파일명을 입력하세요");
                return false;
            }

            string strParameter = texboxFolderPath.Text.Trim() + "?" + textBoxFileName.Text.Trim();
            Command.FileUpdate(m_dbMgr, nCommandID, strParameter, true);
            return true;
        }
    }
}
