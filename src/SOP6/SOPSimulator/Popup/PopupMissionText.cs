using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SOPMonitoringSystem
{
    public partial class PopupMissionText : Form
    {
        private bool m_initPosition = false;

        protected static PopupMissionText instance = null;
        public static PopupMissionText Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new PopupMissionText();
                }
                return instance;
            }
        }

        public PopupMissionText()
        {
           // this.Parent = FormMain.Instance;
            InitializeComponent();
        }


        public void SetText(string szText, string szTextTarget, string szTextMedium, string szTextSender, string szTextPerformer, Sections.Section section)
        {
            this.Activate();
            this.TopMost = true;
            this.BringToFront();


            if (!FormSOP.Instance.ShowMissionText)
                return;

            if (Visible == false)
            {
                this.TopMost = true;
                this.Show();
            }

            if (this.WindowState != FormWindowState.Normal)
            {
                this.WindowState = FormWindowState.Normal;
                this.Activate();
            }

            string str = GetSectionText(szText, szTextTarget, szTextMedium, szTextSender, szTextPerformer, section);
            //string str = string.Format("[임무내용]\r\n{0}\r\n\r\n[임무전달 방식]\r\n{1}\r\n\r\n[임무 수신자]\r\n{2}",
            //    szText, szTextMedium, szTextTarget);

            textBox1.Text = str;

            /*textBox1.Text = szText;
			textBox2.Text = szTextTarget;
            textBoxMedium.Text = szTextMedium;*/
        }

        /// <summary>
        /// 컨트롤의 활성화와는 관계없이 무조건 텍스트만 수정하는 기능
        /// </summary>
        public void SetOnlyText(string szText, string szTextTarget, string szTextMedium, string szTextSender, string szTextPerformer, Sections.Section section)
        {
            textBox1.Text = GetSectionText(szText, szTextTarget, szTextMedium, szTextSender, szTextPerformer, section);
        }

        private string GetSectionText(string szText, string szTextTarget, string szTextMedium, string szTextSender, string szTextPerformer, Sections.Section section)
        {
            szText = szText.Replace("\n", "\r\n");

            if (section == null)
                return szText;

            string str = "";

            if (section.GetComponentType() == Sections.Section.ComponentType.PROCESS)
            {
                Sections.SectionProcess sectionProcess = (Sections.SectionProcess)section;

                // changed by mwkim 2015-10-06 현재 전파수단은 항상 빈값으로 입력하고 있으므로 출력항목에서 제외시킴.
                //str = string.Format("[현재단계] {0}\r\n\r\n[실행자] {1}\r\n\r\n[수명자] {2}\r\n\r\n[전파수단] {3}\r\n\r\n[임무내용]\r\n{4}",
                str = string.Format("【현재단계】 {0}\r\n\r\n【발신자】 {1}\r\n\r\n【실행자】 {2}\r\n\r\n【수신자】 {3}\r\n\r\n【임무내용】\r\n{4}",
                    sectionProcess.TextUP,
                    szTextSender,
                    szTextPerformer,
                    szTextTarget,
                    szText);

                this.Size = new Size(this.Size.Width, 280);
            }
            else if (section.GetComponentType() == Sections.Section.ComponentType.INTERNAL)
            {
                Sections.SectionInternal sectionInternal = (Sections.SectionInternal)section;

                if ((sectionInternal.Data as Sections.SectionDataInternal).UseMobileApp)
                {
                    str = String.Format("【현재단계】 {0}\r\n\r\n【수신자】 {1}\r\n\r\n【임무내용】\r\n{2}",
                        section.Title,
                        szTextTarget,
                        szText);

                    this.Size = new Size(this.Size.Width, 200);
                }
                else
                {
                    str = String.Format("【현재단계】 {0}\r\n\r\n【임무내용】\r\n{1}",
                        section.Title,
                        szText);

                    this.Size = new Size(this.Size.Width, 200);
                }
            }
            else
            {
                str = string.Format("【현재단계】 {0}\r\n\r\n【임무내용】\r\n{1}",
                    section.Title,
                    szText);

                this.Size = new Size(this.Size.Width, 166);
            }
            
            return str;
        }

        private void PopupMissionText_FormClosing(object sender, FormClosingEventArgs e)
        {
            //instance = null;
            if (!FormSOP.Instance.CloseThread)
            {
                e.Cancel = true;
                this.Hide();
            }
        }

        private void PopupMissionText_Load(object sender, EventArgs e)
        {
            if (!m_initPosition)
            {
                this.Location = new Point(FormFrame.Instance.Location.X + this.Location.X, FormFrame.Instance.Location.Y + this.Location.Y);
                m_initPosition = true;
            }
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            // ESC 키에 대해서 현재 창을 닫도록 함.
            if (keyData == Keys.Escape)
                this.Close();

            return true;
        }

    }
}
