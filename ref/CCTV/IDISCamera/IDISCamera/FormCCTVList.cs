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
using System.Collections;

namespace IDISCamera
{
    public partial class FormCCTVList : Form
    {
        private WebDBManager m_dbMgr = null;
        private int m_nSiteID = 1;
        private Form1 m_frmMain = null;

        public FormCCTVList(Form1 frmMain)
        {
            InitializeComponent();
            m_frmMain = frmMain;
        }

        private void FormCCTVList_Load(object sender, EventArgs e)
        {
            if (ReadSiteID())
            {
                m_dbMgr = new WebDBManager(m_nSiteID);
                ReadCCTV();
            }
        }

        private void ReadCCTV()
        {
            string strSQL = "Select ID, CameraName, IPAddr, Port, UserID, Password from CCTV";
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null)
                return;

            int nResultCount = arrResult.Count;

            for (int i=0;i<nResultCount-5;i+=6)
            {
                VariousData<int> id = WebDBManager.GetIntField(arrResult[i].ToString());
                string strCameraName = WebDBManager.GetStringField(arrResult[i + 1]);
                string strIP = WebDBManager.GetStringField(arrResult[i + 2]);
                VariousData<int> port = WebDBManager.GetIntField(arrResult[i + 3].ToString());
                string strUserID = WebDBManager.GetStringField(arrResult[i + 4]);
                string strPW = WebDBManager.GetStringField(arrResult[i + 5]);

                if (id == null || strCameraName == null || strIP == null || port == null)
                    continue;

                if (strUserID == null)
                    strUserID = "";

                if (strPW == null)
                    strPW = "";

                int nIndex = dataGridView1.Rows.Add();
                DataGridViewRow row = dataGridView1.Rows[nIndex];

                row.Cells[0].Value = id.Data;
                row.Cells[1].Value = strCameraName;
                row.Cells[2].Value = strIP;

                CCTVInfo info = new CCTVInfo();

                info.ID = id.Data;
                info.CameraName = strCameraName;
                info.IP = strIP;
                info.Port = port.Data;
                info.UserID = strUserID;
                info.PW = strPW;

                row.Tag = info;
            }
        }

        private bool ReadSiteID()
        {
            DBUtility.Utility util = new DBUtility.Utility();
            string szSiteID = util.getinivalue("Server Connection Info", "siteid");
            if (szSiteID == null || szSiteID == "")
            {
                MessageBox.Show("Site ID가 지정되지 않았습니다. ini파일을 확인하세요", "실행 오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            int nSiteId = 1;

            if (int.TryParse(szSiteID, out nSiteId))
            {
                m_nSiteID = nSiteId;
            }
            else
            {
                MessageBox.Show("잘못된 Site ID입니다.. ini파일을 확인하세요", "실행 오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            return true;
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0)
                return;

            DataGridViewRow row = dataGridView1.Rows[e.RowIndex];
            CCTVInfo info = (CCTVInfo)row.Tag;
            m_frmMain.OnSelectCCTV(info);
        }
    }

    public class CCTVInfo
    {
        private int m_nID = -1;
        private string m_strCameraName = "";
        private string m_strIP = "";
        private int m_nPort = -1;
        private string m_strUserID = "";
        private string m_strPW = "";

        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }

        public string CameraName
        {
            get { return m_strCameraName; }
            set { m_strCameraName = value; }
        }

        public string IP
        {
            get { return m_strIP; }
            set { m_strIP = value; }
        }

        public int Port
        {
            get { return m_nPort; }
            set { m_nPort = value; }
        }

        public string UserID
        {
            get { return m_strUserID; }
            set { m_strUserID = value; }
        }

        public string PW
        {
            get { return m_strPW; }
            set { m_strPW = value; }
        }
    }
}
