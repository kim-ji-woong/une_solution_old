using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace HelpViewerFriend
{
    public partial class Form1 : Form
    {
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

        public Form1()
        {
            InitializeComponent();
        }

        private void btnReceiveList_Click(object sender, EventArgs e)
        {
            SendCommand("GetAllPageList");
        }

        private void SendCommand(string strCommand)
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

        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case WM_COPYDATA:
                    COPYDATASTRUCT cds = (COPYDATASTRUCT)m.GetLParam(typeof(COPYDATASTRUCT));
                    OnReceive(cds);
                    return;
            }

            base.WndProc(ref m);
        }

        private void OnReceive(COPYDATASTRUCT cds)
        {
            string[] tokens = cds.lpData.Split('\t');
            int nTokenCount = tokens.Count();

            if (nTokenCount == 0)
                return;

            string strCommand = tokens[0].Trim();

            if (strCommand == "AllPageList")
                SetTreeItems(tokens);
        }

        private void SetTreeItems(string[] tokens)
        {
            int nTokenCount = tokens.Count();
            treeView1.Nodes.Clear();

            string strItem = null;
            TreeNode currentNode = null;

            for (int i=1;i<nTokenCount;i++)
            {
                int dir = GetTreeItemText(tokens[i].Trim(), ref strItem);

                if (strItem == null)
                    return;

                if (dir == 0)
                {
                    if (currentNode == null || currentNode.Parent == null)
                        currentNode = treeView1.Nodes.Add(strItem);
                    else
                        currentNode = currentNode.Parent.Nodes.Add(strItem);
                }
                else if (dir > 0)
                {
                    currentNode = currentNode.Nodes.Add(strItem);
                }
                else if (dir < 0)
                {
                    for (int j=0;j>dir;j--)
                    {
                        if (currentNode == null)
                            return;

                        currentNode = currentNode.Parent;
                    }

                    if (currentNode == null || currentNode.Parent == null)
                        currentNode = treeView1.Nodes.Add(strItem);
                    else
                        currentNode = currentNode.Parent.Nodes.Add(strItem);
                }
            }

            treeView1.ExpandAll();
        }

        private int GetTreeItemText(string str, ref string strItem)
        {
            int nIndex1 = str.IndexOf('(');
            int nIndex2 = str.IndexOf(')');

            if (nIndex1 < 0 || nIndex2 <= nIndex1)
            {
                strItem = null;
                return 0;
            }

            string strNo = str.Substring(nIndex1 + 1, nIndex2 - nIndex1 - 1);
            int no;

            if (int.TryParse(strNo, out no) == false)
            {
                strItem = null;
                return 0;
            }

            strItem = str.Substring(nIndex2 + 1);
            return no;
        }

        private void btnOpenPage_Click(object sender, EventArgs e)
        {
            if (treeView1.SelectedNode == null)
                return;

            SendCommand("SelectNode\t" + treeView1.SelectedNode.FullPath);
        }

        private void btnOpenPageFromID_Click(object sender, EventArgs e)
        {
            string strID = textBoxID.Text.Trim();

            if (strID.Length == 0)
            {
                textBoxID.Focus();
                MessageBox.Show("ID를 입력하세요.");
                return;
            }

            SendCommand("SelectID\t" + strID);
        }
    }
}
