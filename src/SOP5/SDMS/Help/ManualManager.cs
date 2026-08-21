using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using UnE.GUI;

namespace SDMS.Help
{
    public class ManualManager
    {
        private IntPtr m_Handle = new IntPtr();
        public IntPtr Handle
        {
            get { return m_Handle; }
            set { m_Handle = value; }
        }
        
        public bool IsHelpMode
        {
            get { return FormMain.Instance.IsHelpMode; }
            set { FormMain.Instance.IsHelpMode = value; }
        }

        private Dictionary<Control, string> m_dicControlIDs = new Dictionary<Control, string>();
        public Dictionary<Control, string> DicControlIDs
        {
            get { return m_dicControlIDs; }
        }

        private Form Frm = null;
        public ManualManager(Form frm)
        {
            this.Frm = frm;
        }

        public string GetID(Control ctrl)
        {
            if (!m_dicControlIDs.ContainsKey(ctrl))
                return "";

            return m_dicControlIDs[ctrl];
        }

        public void SetID(Control ctrl, string name)
        {
            m_dicControlIDs[ctrl] = name;
        }

        public void Clear()
        {
            m_dicControlIDs.Clear();
        }

        public void ProcessEvent()
        {
            foreach (KeyValuePair<Control, string> item in m_dicControlIDs)
            {
                item.Key.MouseDown += Key_MouseDown;

                item.Key.MouseEnter += Key_MouseEnter;
                item.Key.MouseLeave += Key_MouseLeave;
            }
        }

        void Key_MouseDown(object sender, MouseEventArgs e)
        {
            if (!IsHelpMode)
                return;

            if (IsHelpMode && e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                FormMain.Instance.btnTargetHelp_Click(null, null);
                return;
            }

            if (IsHelpMode)
            {
                Control ctrl = sender as Control;
                if (ctrl == null)
                    return;

                string strID = GetID(ctrl);
                if (strID.Length == 0)
                    return;

                RunViewer();
                SendViewerCommand("SelectID\t" + strID);
            }
            
            return;
        }
         
        private Pen mPen = new Pen(Color.Red);
        private SolidBrush opaqueBrush = new SolidBrush(Color.FromArgb(128, 0x28, 0x28, 0x28));
        void Key_MouseEnter(object sender, EventArgs e)
        {
            if (!IsHelpMode)
                return;
            
            Control ctrl = sender as Control;
            if (ctrl == null)
                return;

            Frm.Cursor = Cursors.Help;

            Graphics g = ctrl.CreateGraphics();

            Rectangle r = new Rectangle(1, 1, ctrl.Width - 2, ctrl.Height - 2);
            g.FillRectangle(opaqueBrush, r);
        }

        void Key_MouseLeave(object sender, EventArgs e)
        {
            Control ctrl = sender as Control;
            if (ctrl == null)
                return;

            ctrl.Refresh();
            Frm.Cursor = Cursors.Default;
        }

        const int WM_COPYDATA = 0x4A;

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, uint wParam, ref COPYDATASTRUCT lParam);

        public struct COPYDATASTRUCT
        {
            public IntPtr dwData;
            public int cbData;
            [MarshalAs(UnmanagedType.LPStr)]
            public string lpData;
        }

        private void RunViewer()
        {
            Process[] process = Process.GetProcessesByName("HelpViewer");

            if (process.Length == 0)
            {
                string strFilePath = GetHelpViewerPath();

                if (strFilePath == null)
                    return;

                System.Diagnostics.ProcessStartInfo info = new System.Diagnostics.ProcessStartInfo();

                info.CreateNoWindow = true;
                info.FileName = strFilePath;

                System.Diagnostics.Process process2 = new System.Diagnostics.Process();
                process2.StartInfo = info;

                process2.Start();

                int nCount = 0;
                while (true)
                {                    
                    if (nCount == 2)
                        break;
                    System.Threading.Thread.Sleep(1000);
                    nCount++;
                }
            }
        }

        public static string GetHelpViewerPath()
        {
            string strDir = System.IO.Path.GetDirectoryName(Application.ExecutablePath);
            string strFilePath = strDir + "\\HelpViewer.exe";

            if (System.IO.File.Exists(strFilePath))
                return strFilePath;

            strFilePath = strDir + "\\..\\SOP\\HelpViewer.exe";

            if (System.IO.File.Exists(strFilePath))
                return strFilePath;

            return null;
        }

        private void SendViewerCommand(string strCommand)
        {
            Process[] process = Process.GetProcessesByName("HelpViewer");

            if (process.Length > 0)
            {
                byte[] buff = System.Text.Encoding.Default.GetBytes(strCommand);

                COPYDATASTRUCT cds = new COPYDATASTRUCT();
                cds.dwData = this.Handle;
                cds.cbData = buff.Length + 1;
                cds.lpData = strCommand;

                SendMessage(process[0].MainWindowHandle, WM_COPYDATA, 0, ref cds);
            }
        }
    }
}
