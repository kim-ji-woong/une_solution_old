using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DBUtility2;

namespace SOPMonitoringSystem.Popup.Login
{
    public partial class FormLogin : Form, ITextBoxOwner
    {
        private bool m_bLeftMouseDown = false;
        private Point m_ptMove;

        private WebDBManager m_dbMgr = null;
        private NetworkWebManager m_netMgr = null;
        private Point m_ptLogo = new Point();
        private Image m_imgLogo = null;

        private bool m_systemInput = false;
        private string m_strArguments = "";

        private static FormLogin m_instance = null;

        public static FormLogin Instance
        {
            get { return m_instance; }
        }

        public FormLogin(string strArguments)
        {
            m_instance = this;
            InitializeComponent();

            m_strArguments = strArguments;
            SetManager();
            m_imgLogo = global::SOPMonitoringSystem.Properties.Resources.LoginLogo;

            textBoxPW.Owner = textBoxID.Owner = this;
            SetKeepButtonClickedImage();
        }

        private bool SetManager()
        {
            Utility ini = new Utility();
            string strSiteID = ini.getinivalue("Server Connection Info", "siteid");

            int nSiteID = 1;

            if (strSiteID.Length > 0)
            {
                if (int.TryParse(strSiteID, out nSiteID))
                {
                    m_dbMgr = new WebDBManager(null, nSiteID);
                    m_netMgr = new NetworkWebManager(m_dbMgr, SOPWebServer.ClientType.LOGIN_SERVER, SOPWebServer.ClientSubType.INTEGRATED_MANAGER);
                    m_netMgr.ReleaseConnection();
                    return true;
                }
            }

            return false;
        }

        private void FormLogin_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                m_bLeftMouseDown = true;
                m_ptMove = PointToScreen(new Point(e.X, e.Y));
            }
        }

        private void FormLogin_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (m_bLeftMouseDown == true)
                {
                    Point pt = PointToScreen(new Point(e.X, e.Y));
                    int dx = pt.X - m_ptMove.X;
                    int dy = pt.Y - m_ptMove.Y;
                    if (!(dx == 0 && dy == 0))
                    {
                        Point ptCur = this.Location;
                        this.Location = new Point(ptCur.X + dx, ptCur.Y + dy);
                        m_ptMove.X += dx;
                        m_ptMove.Y += dy;
                    }
                }
            }
        }

        private void FormLogin_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                m_bLeftMouseDown = false;
        }

        private void FormLogin_Load(object sender, EventArgs e)
        {
            if (m_netMgr == null)
            {
                MessageBox.Show("시작하기 위한 설정파일을 찾을수 없습니다.");
                this.Close();
            }
            else
            {
                ReadState();

                if (textBoxPW.Text.Length == 0)
                    textBoxPW.Hide();

                if (textBoxID.Text.Length == 0)
                    textBoxID.Hide();
            }
        }

        private void ReadState()
        {
            string strAutoLogin = RegUtil.ReadRegValue("IntegratedManager", "AutoLogin", m_dbMgr.SiteID);
            string strLastID = RegUtil.ReadRegValue("IntegratedManager", "LastUser", m_dbMgr.SiteID);
            string strLastPass = RegUtil.ReadRegValue("IntegratedManager", "LastEncr", m_dbMgr.SiteID);

            if (strLastID != null && strLastID.Length > 0)
                textBoxID.Text = strLastID;

            if (strLastPass != null && strLastPass.Length > 0)
                textBoxPW.Text = Enc(strLastPass, false);

            if (strAutoLogin == "1")
            {
                m_systemInput = true;
                btnKeepLogin.IsChecked = true;
                m_systemInput = false;

                if (strLastID != null && strLastID.Length > 0 && strLastPass != null && strLastPass.Length > 0)
                {
                    btnLogin_Click(null, null);
                }
            }
        }

        private void FormLogin_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (m_netMgr != null)
                m_netMgr.ReleaseThread();
        }

        private string Enc(string str, bool encrypt)
        {
            string key = new string(new char[] { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z', '1', '2', '3', '4', '5', '6' });

            if (encrypt)
                return AES256Cipher.AES_encrypt(str, key);

            return AES256Cipher.AES_decrypt(str, key);
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            string strID = textBoxID.Text.Trim();
            string strPW = textBoxPW.Text.Trim();

            if (strID.Length == 0)
            {
                textBoxID.Focus();
                MessageBox.Show("ID를 입력하세요.");
                return;
            }

            if (strPW.Length == 0)
            {
                textBoxPW.Focus();
                MessageBox.Show("비밀번호를 입력하세요.");
                return;
            }

            btnLogin.Enabled = false;
            string strEnc = Enc(strPW, true);

            if (btnKeepLogin.IsChecked)
            {
                RegUtil.WriteRegValue("IntegratedManager", "LastUser", strID, m_dbMgr.SiteID);
                RegUtil.WriteRegValue("IntegratedManager", "LastEncr", strEnc, m_dbMgr.SiteID);
            }

            m_netMgr.LoginUser(strID, strEnc);
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void FormLogin_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.DrawImage(m_imgLogo, m_ptLogo);
        }

        private void FormLogin_Resize(object sender, EventArgs e)
        {
            int x = (this.Size.Width - m_imgLogo.Size.Width) / 2;
            int y = 15;

            m_ptLogo = new Point(x, y);
        }

        public void ReceiveLoginResult(bool success, string strMessage)
        {
            this.Invoke((MethodInvoker)delegate
            {
                if (success)
                {
                    int nIndex = strMessage.IndexOf('_');

                    if (nIndex >= 0)
                    {
                        string strID = strMessage.Substring(0, nIndex);
                        string strNickName = strMessage.Substring(nIndex + 1);

                        int nID;

                        if (int.TryParse(strID, out nID))
                        {
                            BeginSystem(nID, strNickName);
                        }
                    }

                    this.Close();
                }
                else
                {
                    btnLogin.Enabled = true;
                    MessageBox.Show(strMessage);
                }
            });
        }

        private void BeginSystem(int nUserID, string strUserName)
        {
            System.Diagnostics.Process process = System.Diagnostics.Process.GetCurrentProcess();
            string strProcessName = process.MainModule.FileName;

            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
            startInfo.FileName = strProcessName;
            startInfo.ErrorDialog = true;
            startInfo.Arguments = string.Format("{0} \"{1}\"", nUserID, strUserName);

            if (m_strArguments.Length > 0)
                startInfo.Arguments += " " + m_strArguments;

            try
            {
                System.Diagnostics.Process.Start(startInfo);
            }
            catch (Exception ex)
            {
            }
        }

        public void GetFocus(ImageTextBox textBox)
        {
            if (textBox == textBoxPW)
            {
                if (textBoxID.Text.Length == 0)
                    textBoxID.Hide();
            }
            else if (textBox == textBoxID)
            {
                if (textBoxPW.Text.Length == 0)
                    textBoxPW.Hide();
            }
        }

        private void panelTextBox_MouseDown(object sender, MouseEventArgs e)
        {
            if (sender == panelPW)
            {
                textBoxPW.Show();
                textBoxPW.Focus();
            }
            else
            {
                textBoxID.Show();
                textBoxID.Focus();
            }
        }

        private void btnKeepLogin_Click(object sender, EventArgs e)
        {
            btnKeepLogin.IsChecked = !btnKeepLogin.IsChecked;

            if (btnKeepLogin.IsChecked == true)
            {
                RegUtil.WriteRegValue("IntegratedManager", "AutoLogin", "1", m_dbMgr.SiteID);
            }
            else
            {
                RegUtil.WriteRegValue("IntegratedManager", "AutoLogin", "0", m_dbMgr.SiteID);
            }

            SetKeepButtonClickedImage();
            btnKeepLogin.Refresh();
        }

        private void SetKeepButtonClickedImage()
        {
            if (btnKeepLogin.IsChecked)
                btnKeepLogin.ClickedImage = btnKeepLogin.CheckedImage;
            else
                btnKeepLogin.ClickedImage = btnKeepLogin.NormalImage;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void textBoxID_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Tab)
                System.Diagnostics.Trace.WriteLine("Tab Clicked");
        }

        private void textBox_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            TextBox textBox = sender == textBoxID ? textBoxPW : textBoxID;

            if (e.KeyCode == Keys.Tab)
            {
                if (textBox.Visible == false)
                    textBox.Show();
            }
            else if (e.KeyCode == Keys.Enter)
                btnLogin_Click(null, null);
        }

        private void panel_Click(object sender, EventArgs e)
        {
            TextBox textBox = sender == panelID ? textBoxID : textBoxPW;

            textBox.Show();
            textBox.Focus();
        }
    }
}
