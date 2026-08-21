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

namespace ConfigSetter4Service
{
    public partial class FormMain : Form
    {
        private int m_nSiteID = -1;

        public FormMain()
        {
            InitializeComponent();
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            if (ReadSiteID() == false)
            {
                labelState.ForeColor = Color.Red;
                labelState.Text = "config.ini를 찾을수 없습니다.";
                return;
            }

            WebDBManager dbMgr = new WebDBManager(m_nSiteID);

            if (SaveConfigFile(dbMgr))
                labelState.Text = "config.ini 확인이 끝났습니다.";
            else
            {
                labelState.ForeColor = Color.Red;
                labelState.Text = "config.ini 정리 작업이 실패하였습니다.";
            }
        }

        private bool SaveConfigFile(WebDBManager dbMgr)
        {
            DBUtility.Utility ini = new DBUtility.Utility();

            if (!SaveConfigFile("Server Connection Info", "webserver_url", dbMgr.WebServerURL, ini))
                return false;

            if (!SaveConfigFile("Server Connection Info", "server_ip", dbMgr.DatabaseHost, ini))
                return false;

            string strPort = "";

            if (dbMgr.DatabaseType == WebDBManager.DBType.mysql)
                strPort = "3306";
            else if (dbMgr.DatabaseType == WebDBManager.DBType.sqlserver)
                strPort = "1433";
            else
                return false;

            if (!SaveConfigFile("Server Connection Info", "server_port", strPort, ini))
                return false;

            if (!SaveConfigFile("Server Connection Info", "server_db", dbMgr.DatabaseName, ini))
                return false;

            return true;
        }

        private bool SaveConfigFile(string strSection, string strKey, string strTarget, Utility ini)
        {
            string strValue = ini.getinivalue(strSection, strKey);

            if (strTarget != strValue)
            {
                ini.setinivalue(strSection, strKey, strTarget);

                if (ini.getinivalue(strSection, strKey) != strTarget)
                    return false;
            }

            return true;
        }

        private bool ReadSiteID()
        {
            DBUtility.Utility ini = new DBUtility.Utility();
            string strSiteID = ini.getinivalue("Server Connection Info", "siteid");

            if (strSiteID.Length > 0)
            {
                if (int.TryParse(strSiteID, out m_nSiteID))
                    return true;
            }

            return false;
        }
    }
}
