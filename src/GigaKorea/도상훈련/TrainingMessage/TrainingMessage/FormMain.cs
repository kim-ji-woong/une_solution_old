using DBUtility2;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TrainingMessage.Data;
using TrainingMessage.Popup_Dialog;

namespace TrainingMessage
{
    public partial class FormMain : Form
    {
        //private int m_nSiteID = 1;
        private string m_strNickName = "";
        private string m_strReceiver = "";
        private string m_strMessage = "";

        private WebDBManager m_dbMainMgr = null;
        private DataManager m_dataMgr = null;
        public DataManager DataManager
        {
            get { return m_dataMgr; }
        }

        private static FormMain m_instance = null;
        public static FormMain Instance
        {
            get { return m_instance; }
        }

        public string Receiver { get; set; }

        public FormMain(string[] args)
        {
            InitializeComponent();
            m_instance = this;

            //m_nSiteID = ReadSiteID();
            int nArgs = 0;
            int nMainSiteID;

            nArgs = args.Count();

            // 인수 체크
            if (nArgs == 1)
            {
                m_strMessage = args[0];
            }
            else if (nArgs == 2)
            {
                m_strMessage = args[0];
                m_strReceiver = args[1];

            }

            // main site id 읽기
            if (ReadConfig("MainSiteID", out nMainSiteID) == false)
                nMainSiteID = 300;

            m_dbMainMgr = new WebDBManager(nMainSiteID);
            m_dataMgr = new DataManager(m_dbMainMgr);
        }

        private bool ReadConfig(string strName, out int value)
        {
            string strValue = System.Configuration.ConfigurationManager.AppSettings[strName].ToString().Trim();
            return int.TryParse(strValue, out value);
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            // 발신자 표시
            m_strNickName = ReadNickName();
            if (m_strNickName == null || m_strNickName == "")
            {
                MessageBox.Show("유저 닉네임이 없습니다. 통합관리자로 다시 로그인 해주세요.", "경고");
                this.Close();
                return;
            }
            else
            {
                txtSender.Text = m_strNickName;
            }

            // 메시지 표시
            if (m_strMessage != null || m_strMessage != "")
            {
                txtMessage.Text = m_strMessage;
            }

            // 수신자
            if (m_strReceiver != null || m_strReceiver != "")
            {
                txtReceiver.Text = m_strReceiver;
            }
        }

        #region 폼 이동
        private bool m_bLeftMouseDown = false;
        private Point m_ptMove = new Point();
        private bool m_isClicked = false;
        private Point m_ptOrigin = new Point();

        private void Form_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                m_bLeftMouseDown = true;
                m_ptMove = Control.MousePosition;
                m_ptOrigin = this.Location;
            }

            m_isClicked = true;
        }

        private void Form_MouseMove(object sender, MouseEventArgs e)
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

        private void Form_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
                m_bLeftMouseDown = false;

            m_isClicked = false;
        }
        #endregion

        private int ReadSiteID()
        {
            int nSiteId = -1;

            Utility util = new Utility();
            string szSiteID = util.getinivalue("Server Connection Info", "siteid");
            if (szSiteID == null || szSiteID == "")
                return nSiteId;
            else
                int.TryParse(szSiteID, out nSiteId);

            return nSiteId;
        }

        private string ReadNickName()
        {
            string strNickName = "";

            Utility util = new Utility();
            strNickName = util.getinivalue("Server Connection Info", "userNickName");

            if (strNickName == null || strNickName == "")
                return null;

            return strNickName;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            FormSelectMember form = new FormSelectMember(m_strReceiver);
            form.StartPosition = FormStartPosition.CenterParent;

            if (form.ShowDialog() == DialogResult.Yes)
            {
                txtReceiver.Text = Receiver;
                m_strReceiver = Receiver;
            }
        }

        private void btnSpread_Click(object sender, EventArgs e)
        {
            string[] arrReceiver = m_strReceiver.Split(',');
            int nCount = arrReceiver.Length;
            m_strMessage = txtMessage.Text;

            for (int i = 0; i < nCount; i++)
            {
                string strNickName = arrReceiver[i].Trim();

                // 전송 insert 쿼리
                m_dataMgr.InsertLinkMessage(m_strNickName, strNickName, m_strMessage);
            }

            MessageBox.Show("메시지를 성공적으로 보냈습니다.", "성공", MessageBoxButtons.OK);
            this.Close();
        }
    }
}
