using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SpeechLib;

namespace SOPManager
{
    enum PlayMode { PLAY, PAUSE, RESUME, STOP }

    public partial class PopupPreListenMessage : Form
    {
        private PlayMode m_playMode = PlayMode.STOP;

        private bool m_bLeftMouseDown = false;
        private Point m_ptMove = new Point();

        private string m_strTextOrigin = null;
        private DateTime m_dtTime = new DateTime();
        private string m_strLocation = null;

        public PopupPreListenMessage(string strText, DateTime dtTime, string strLocation)
        {
            InitializeComponent();

            m_strTextOrigin = strText;
            m_dtTime = dtTime;
            m_strLocation = strLocation;

            ParseText(); 
               
            UpdateControlSize();
        }

        public void UpdateControlSize()
        {
            Double[] dWindowRate = FormMain.Instance.GetCurWindowRate();
            double WindowRateWidth = dWindowRate[0];
            double WindowRateHeight = dWindowRate[1];

            this.Size = new System.Drawing.Size((int)(this.Size.Width * WindowRateWidth), (int)(this.Size.Height * WindowRateHeight));

            foreach (Control ctl in this.Controls)
            {
                HaveControl(ctl, WindowRateWidth, WindowRateHeight);
            }
        }

        private void HaveControl(Control pctl, double WindowRateWidth, double WindowRateHeight)
        {
            foreach (Control ctl in pctl.Controls)
            {
                if (ctl.Controls.Count > 0)
                    HaveControl(ctl, WindowRateWidth, WindowRateHeight);

                FormMain.Instance.UpdateWindowRate(ctl, WindowRateWidth, WindowRateHeight);
            }
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Escape)
            {
                this.Close();
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        #region 폼 이동
        private void TitleBar_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                m_bLeftMouseDown = true;
                m_ptMove = this.PointToScreen(new Point(e.X, e.Y));
            }
        }

        private void TitleBar_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (m_bLeftMouseDown == true)
                {
                    Point pt = this.PointToScreen(new Point(e.X, e.Y));
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

        private void TitleBar_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
                m_bLeftMouseDown = false;
        } 
        #endregion

        private void btnOK_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void radioMode_CheckedChanged(object sender, EventArgs e)
        {
            ParseText();
        }

        private void ParseText()
        { 
            string strResult = UnE.Utility.SOPSimulatorScript.Parse(m_strTextOrigin, m_dtTime, m_strLocation);
            textBoxPreview.Text = strResult;
        }

        private SpVoice voice = null;
        private bool m_bPlay = false;

        private void ChgPlayMode(PlayMode mode)
        {
            if (mode == PlayMode.PLAY)
            {
                voice = new SpVoice();
                voice.Speak(textBoxPreview.Text, SpeechVoiceSpeakFlags.SVSFlagsAsync);
                voice.EndStream += voice_EndStream;

                m_bPlay = true;
            }
            else if (mode == PlayMode.RESUME)
            {
                voice.Resume();
                m_bPlay = true;
            }
            else if (mode == PlayMode.STOP)
            {
                if (voice != null)
                {
                    voice.Pause();
                    voice = null;
                }

                m_bPlay = false;

                btnPreListen.ImageNormal = global::SOPManager.Properties.Resources.PreListen_Play;
                btnPreListen.ImageMouseOver = global::SOPManager.Properties.Resources.PreListen_Play_Click;
                btnPreListen.ImageClicked = global::SOPManager.Properties.Resources.PreListen_Play_Click;

                btnPreListen.Refresh();
            }
            else if (mode == PlayMode.PAUSE)
            {
                if (voice != null)
                    voice.Pause();

                m_bPlay = false;
            }

            if (m_bPlay)
            {
                btnPreListen.ImageNormal = global::SOPManager.Properties.Resources.PreListen_Pause;
                btnPreListen.ImageMouseOver = global::SOPManager.Properties.Resources.PreListen_Pause_Click;
                btnPreListen.ImageClicked = global::SOPManager.Properties.Resources.PreListen_Pause_Click;
            }
            else
            {
                btnPreListen.ImageNormal = global::SOPManager.Properties.Resources.PreListen_Play;
                btnPreListen.ImageMouseOver = global::SOPManager.Properties.Resources.PreListen_Play_Click;
                btnPreListen.ImageClicked = global::SOPManager.Properties.Resources.PreListen_Play_Click;
            }
            btnPreListen.Refresh();
        }

        private void btnPreListen_Click(object sender, EventArgs e)
        {
            if (textBoxPreview.Text.Length == 0)
                return;

            if (m_bPlay && voice != null)
            {
                ChgPlayMode(PlayMode.PAUSE);
            }
            else if (!m_bPlay && voice != null)
            {
                ChgPlayMode(PlayMode.RESUME);
            }
            else
            {
                ChgPlayMode(PlayMode.PLAY);
            } 
        }

        void voice_EndStream(int StreamNumber, object StreamPosition)
        {
            ChgPlayMode(PlayMode.STOP);
        }

        public delegate void changeMsg(string msg);
        public event changeMsg ChangeMsg;

        private void btnApply_Click(object sender, EventArgs e)
        {
            if (ChangeMsg != null)
                ChangeMsg(textBoxPreview.Text);
        } 
        private void btnStop_Click(object sender, EventArgs e)
        {
            ChgPlayMode(PlayMode.STOP);
        }

        private void PopupPreListenMessage_FormClosing(object sender, FormClosingEventArgs e)
        {
            ChgPlayMode(PlayMode.STOP);
        }        
    }
}
