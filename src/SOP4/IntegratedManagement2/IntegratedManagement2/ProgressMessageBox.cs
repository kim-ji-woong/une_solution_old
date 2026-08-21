using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IntegratedManagement2
{
    public partial class ProgressMessageBox : Form
    {
        // 연습모드 준비 완료후 Delay Count
        private int m_nDelayCount = 0;
        private static Color m_FrameColor = Color.FromArgb(60, 56, 71);

        public static void Show()
        {
            if (FormMain.Instance.SimulationDBManager.PrepareStatus != SimulationDBManager.LocalDBMode.PREPARED)
            {
                ProgressMessageBox box = new ProgressMessageBox();
                box.Text = "알림";

                MessageBoxFrame frame = new MessageBoxFrame(box);
                frame.Size = new Size(box.Size.Width + 10, box.Size.Height + 35);
                frame.SetFrameColor(m_FrameColor);
                frame.ShowInTaskbar = false;
                frame.CloseButtonImage = global::IntegratedManagement2.Properties.Resources.CloseWindow_Normal;
                frame.StartPosition = FormStartPosition.CenterParent;
                frame.Sizable = false;

                frame.ShowDialog();
            }
        }

        protected ProgressMessageBox()
        {
            InitializeComponent();

            m_nDelayCount = 0;
            timer1.Start();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (FormMain.Instance.SimulationDBManager.PrepareStatus == SimulationDBManager.LocalDBMode.CANNOT_USE)
            {
                labelMessage.Text = "연습모드를 사용할 수 없습니다.";
                btnClose.Enabled = true;

                if (++m_nDelayCount >= 10)
                {
                    timer1.Stop();
                    this.Close();
                }
            }
            else if (FormMain.Instance.SimulationDBManager.PrepareStatus == SimulationDBManager.LocalDBMode.PROCESSING)
            {
                labelMessage.Text = "데이터 준비중입니다.";
                int nQueryCount = FormMain.Instance.SimulationDBManager.TotalQueryCount;
                int nProcessed = FormMain.Instance.SimulationDBManager.ProcessedQueryCount;

                if (nQueryCount > 0)
                {
                    progressBar1.Value = nProcessed * (progressBar1.Maximum - progressBar1.Minimum) / nQueryCount;
                }
            }
            else if (FormMain.Instance.SimulationDBManager.PrepareStatus == SimulationDBManager.LocalDBMode.PREPARED)
            {
                labelMessage.Text = "연습모드를 시작합니다.";
                btnClose.Enabled = true;

                if (++m_nDelayCount >= 10)
                {
                    timer1.Stop();
                    this.Close();
                }
            }
        }

        private void ProgressMessageBox_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (FormMain.Instance.SimulationDBManager.PrepareStatus == SimulationDBManager.LocalDBMode.PROCESSING)
                e.Cancel = true;
            else
                timer1.Stop();
        }
    }

    class MessageBoxFrame : UnE.Utility.UMessageBoxFrame
    {
        public MessageBoxFrame(Form frm)
            : base(frm)
        {
        }

        protected override void CloseButtonClicked()
        {
            if (FormMain.Instance.SimulationDBManager.PrepareStatus == SimulationDBManager.LocalDBMode.PROCESSING)
                return;
            else
                base.CloseButtonClicked();
        }
    }
}
