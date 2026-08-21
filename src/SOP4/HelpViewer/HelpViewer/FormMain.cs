using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using Aga.Controls.Tree;
using Microsoft.Win32;

namespace HelpViewer
{
    public partial class FormMain : Form
    {
        private class TreeStyle
        {
            // BackColor
            private Color m_clrTreeBackground = Color.FromArgb(44, 47, 54);
            private Color m_clrSelectedItem = Color.FromArgb(209, 218, 228);
            private Color m_clrSelectedBackground = Color.FromArgb(25, 33, 45);
            
            // Text Color
            private Color m_clrNotSelectedText = Color.FromArgb(255, 255, 255);
            private Color m_clrSelectedItemText = Color.FromArgb(25, 33, 45);
            private Color m_clrSelectedIText = Color.FromArgb(255, 255, 255);

            // Font
            private Font m_fontNotSelected = new Font("맑은 고딕", 10.0f, FontStyle.Regular);
            private Font m_fontSelectedItem = new Font("맑은 고딕", 10.0f, FontStyle.Bold);
            private Font m_fontSelected = new Font("맑은 고딕", 10.0f, FontStyle.Regular);

            public Color TreeBackColor
            {
                get { return m_clrTreeBackground; }
                set { m_clrTreeBackground = value; }
            }

            public Color SelectedItemBackColor
            {
                get { return m_clrSelectedItem; }
                set { m_clrSelectedItem = value; }
            }

            public Color SelectedBackColor
            {
                get { return m_clrSelectedBackground; }
                set { m_clrSelectedBackground = value; }
            }

            public Color NotSelectedTextColor
            {
                get { return m_clrNotSelectedText; }
                set { m_clrNotSelectedText = value; }
            }

            public Color SelectedItemTextColor
            {
                get { return m_clrSelectedItemText; }
                set { m_clrSelectedItemText = value; }
            }

            public Color SelectedTextColor
            {
                get { return m_clrSelectedIText; }
                set { m_clrSelectedIText = value; }
            }

            public Font NotSelectedFont
            {
                get { return m_fontNotSelected; }
                set { m_fontNotSelected = value; }
            }

            public Font SelectedItemFont
            {
                get { return m_fontSelectedItem; }
                set { m_fontSelectedItem = value; }
            }

            public Font SelectedFont
            {
                get { return m_fontSelected; }
                set { m_fontSelected = value; }
            }
        }

        private class LinkLabelEx : LinkLabel
        {
            private string m_strSearch = "";

            public string SearchText
            {
                get { return m_strSearch; }
                set { m_strSearch = value; }
            }
        }

        private int NO_CHILD = -1;
        private const int COLLAPSED = 0;
        private const int COLLAPSED_OVER = 1;
        private const int EXPANDED = 2;
        private const int EXPANDED_OVER = 3;

        private int m_nImageSize = 0;
        private int m_nLeftSpace = 3;

        private const string CONFIG_FILE = "config.ini";
        private const string LOCATION_TAG = "Location";
        private const string SIZE_TAG = "Size";
        private const string SPLIT_DISTANCE_TAG = "SplitDistance";
        private const string MAXIMIZE_TAG = "IsMaximize";

        private Point m_ptPrevLocation = new Point();
        private Size m_sizePrev = new Size();
        private int m_nPrevSplitDistance = 270;
        private bool m_isPrevMaximize = false;

        private string m_strRootURL = "";
        private string m_strWebServerURL = "";
        private TreeNode m_currentMouseOverNode = null;

        private bool m_systemCall = false;

        // 찾기결과 Text를 HTML Viewer에서 보여줄때 Text 배경색상
        private string m_strHighlightText = null;
        private Color m_clrHighlightBackground = Color.Yellow;

        private OpenOption m_openOption = null;

        private Color m_clrNoSelectedTab = Color.FromArgb(62, 62, 62);
        private Color m_clrSelectedList = Color.FromArgb(239, 162, 54);
        private Color m_clrSelectedResult = Color.FromArgb(218, 83, 79);

        /*private bool m_closeApplication = false;

        public bool CloseApplication
        {
            get { return m_closeApplication; }
        }*/

        private TreeModel m_model = null;

        private TreeStyle m_treeStyle = new TreeStyle();
        private int m_nInitUndoSpace = 0;

        public int PrevSplitDistance
        {
            get { return m_nPrevSplitDistance; }
            set { m_nPrevSplitDistance = value; }
        }

        public int CurrentSplitDistance
        {
            get { return this.splitContainerBody.SplitterDistance; }
        }

        public FormMain(OpenOption option)
        {
            InitializeComponent();

            this.WindowState = FormWindowState.Maximized;
            m_model = new TreeModel();
            treeViewAdv1.Model = m_model;

            m_openOption = option;
            SetFolderPath();

            ClearSearchResult();
            //rbtnUndo.Size = rbtnRedo.Size = rbtnPrint.Size = new Size(32, 32);

            RelocatePanelTopControl(panelSearchArea);
            RelocatePanelTopControl(rbtnUndo);
            RelocatePanelTopControl(rbtnRedo);
            RelocatePanelTopControl(rbtnPrint);
            RelocatePanelTopControl(labelSystemName);

            SetTreeStyle();
            rbtnUndo.Size = rbtnRedo.Size = rbtnPrint.Size = new Size(rbtnUndo.Size.Height, rbtnUndo.Size.Height);
            m_nInitUndoSpace = rbtnUndo.Location.X - panelRed.Location.X - panelRed.Size.Width;
        }

        private bool ReadColor(string str, ref Color color)
        {
            string[] tokens = str.Split(',');

            if (tokens.Count() != 3)
                return false;

            int r, g, b;

            if (int.TryParse(tokens[0].Trim(), out r) &&
                int.TryParse(tokens[1].Trim(), out g) &&
                int.TryParse(tokens[2].Trim(), out b))
            {
                color = Color.FromArgb(r, g, b);
                return true;
            }

            return false;
        }

        private bool ReadFont(string str, ref Font font)
        {
            string[] tokens = str.Split(',');

            string strFontName = null;
            float fSize = 11.0f;
            FontStyle style = FontStyle.Regular;

            int nTokenCount = tokens.Count();

            if (nTokenCount == 0)
                return false;

            for (int i = 0; i < nTokenCount;i++ )
            {
                if (i == 0)
                {
                    strFontName = tokens[0].Trim();
                }
                else if (i == 1)
                {
                    float.TryParse(tokens[1].Trim(), out fSize);
                }
                else if (i == 2)
                {
                    if (string.Compare(tokens[2].Trim(), "Bold", true) == 0)
                        style = FontStyle.Bold;
                }
            }

            font = new Font(strFontName, fSize, style);
            return true;
        }

        private void ReadConfig()
        {
            Color color = Color.White;
            Font font = new System.Drawing.Font("맑은 고딕", 11.0f);
            System.IO.StreamReader reader = new System.IO.StreamReader("config.ini", Encoding.UTF8);

            while (!reader.EndOfStream)
            {
                string strLine = reader.ReadLine().Trim();

                if (strLine.Length == 0)
                    continue;

                int nIndex = strLine.IndexOf('=');

                if (nIndex < 0)
                    continue;

                string strTag = strLine.Substring(0, nIndex).Trim();
                string strValue = strLine.Substring(nIndex + 1).Trim();

                if (strTag == "TreeBackColor")
                {
                    if (ReadColor(strValue, ref color))
                        m_treeStyle.TreeBackColor = color;
                }
                else if (strTag == "SelectedItemBackColor")
                {
                    if (ReadColor(strValue, ref color))
                        m_treeStyle.SelectedItemBackColor = color;
                }
                else if (strTag == "SelectedChildBackColor")
                {
                    if (ReadColor(strValue, ref color))
                        m_treeStyle.SelectedBackColor = color;
                }
                else if (strTag == "NotSelectedTextColor")
                {
                    if (ReadColor(strValue, ref color))
                        m_treeStyle.NotSelectedTextColor = color;
                }
                else if (strTag == "SelectedItemTextColor")
                {
                    if (ReadColor(strValue, ref color))
                        m_treeStyle.SelectedItemTextColor = color;
                }
                else if (strTag == "SelectedIChildTextColor")
                {
                    if (ReadColor(strValue, ref color))
                        m_treeStyle.SelectedTextColor = color;
                }
                else if (strTag == "NotSelectedFont")
                {
                    if (ReadFont(strValue, ref font))
                        m_treeStyle.NotSelectedFont = font;
                }
                else if (strTag == "SelectedItemFont")
                {
                    if (ReadFont(strValue, ref font))
                        m_treeStyle.SelectedItemFont = font;
                }
                else if (strTag == "SelectedChildFont")
                {
                    if (ReadFont(strValue, ref font))
                        m_treeStyle.SelectedFont = font;
                }
                else if (strTag == "FrameBackColor")
                {
                    if (ReadColor(strValue, ref color))
                        splitContainerBody.BackColor = color;
                }
            }

            reader.Close();
        }

        private void SetTreeStyle()
        {
            //ReadConfig();
            // Style1
            /*m_treeStyle.TreeBackColor = Color.FromArgb(149, 153, 162);
            m_treeStyle.SelectedBackColor = Color.FromArgb(69, 72, 77);
            m_treeStyle.SelectedItemBackColor = Color.FromArgb(90, 94, 103);
            m_treeStyle.NotSelectedTextColor = Color.White;
            m_treeStyle.SelectedTextColor = Color.White;
            m_treeStyle.SelectedItemTextColor = Color.White;

            // Style3
            m_treeStyle.TreeBackColor = Color.White;
            m_treeStyle.SelectedBackColor = Color.FromArgb(68, 71, 76);
            m_treeStyle.SelectedItemBackColor = Color.FromArgb(254, 80, 71);
            m_treeStyle.NotSelectedTextColor = Color.Black;
            m_treeStyle.SelectedTextColor = Color.White;
            m_treeStyle.SelectedItemTextColor = Color.White;*/

            treeViewAdv1.BackColor = m_treeStyle.TreeBackColor;
            treeViewAdv1.SelectedChildColor = m_treeStyle.SelectedBackColor;
            treeViewAdv1.SelectedColor = m_treeStyle.SelectedItemBackColor;

            treeViewAdv1.TextColor = m_treeStyle.NotSelectedTextColor;
            treeViewAdv1.SelectedTextColor = m_treeStyle.SelectedItemTextColor;
            treeViewAdv1.SelectedChildTextColor = m_treeStyle.SelectedTextColor;

            treeViewAdv1.Font = m_treeStyle.NotSelectedFont;
            treeViewAdv1.SelectedFont = m_treeStyle.SelectedItemFont;
            treeViewAdv1.SelectedChildFont = m_treeStyle.SelectedFont;
        }

        private void RelocatePanelTopControl(Control ctrl)
        {
            ctrl.Location = new Point(ctrl.Location.X, (panelTop.Size.Height - ctrl.Size.Height) / 2);
        }

        private void SetFolderPath()
        {
            System.Text.Encoding pageEncoding = null;
            m_strRootURL = URLReader.GetURL(m_openOption, ref m_strWebServerURL, ref pageEncoding);

            if (pageEncoding != null)
                WebSearch.PageEncoding = pageEncoding;

            /*int nIndex = Application.ExecutablePath.LastIndexOf('\\');

            if (nIndex >= 0)
            {
                m_strRootURL = Application.ExecutablePath.Substring(0, nIndex) + "\\HelpHtml";
                //m_strRootURL = "http://127.0.0.1:8080/HelpHTML";
            }*/
        }        

        private void FormMain_Load(object sender, EventArgs e)
        {
            if (ReadPrevLocationNSize(ref m_ptPrevLocation, ref m_sizePrev, ref m_nPrevSplitDistance, ref m_isPrevMaximize))
                SetPrevLocationNSize(this, m_ptPrevLocation, m_isPrevMaximize, m_sizePrev);

            splitContainerBody.SplitterDistance = m_nPrevSplitDistance;

            InitTree();
            //treeIndex.ExpandAll();

            if (m_openOption == null)
            {
                ExpandTree(m_model.Nodes, 4);
                //ExpandTree(treeIndex.Nodes, 3);
            }
            else
            {
                if (m_openOption.BeginSelection == OpenOption.SelectionOption.NONE)
                {
                    ExpandTree(m_model.Nodes, 4);
                    //ExpandTree(treeIndex.Nodes, 3);
                }
                else if (m_openOption.BeginSelection == OpenOption.SelectionOption.NODE)
                {
                    SelectNode(m_openOption.BeginSelectionArgument);
                }
                else if (m_openOption.BeginSelection == OpenOption.SelectionOption.ID)
                {
                    SelectIDNode(m_openOption.BeginSelectionArgument);
                }

                if (m_openOption.ApplicationName != null)
                    this.Text = m_openOption.ApplicationName;
            }
        }

        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            //m_closeApplication = true;

            if (this.WindowState != FormWindowState.Minimized)
                WriteSizeNLocation(this, CurrentSplitDistance);
        }

        public static void WriteSizeNLocation(Form frm, int nSplitterDistance)
        {
            string strFilePath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\SOP\\HelpHtml\\" + CONFIG_FILE;

            StreamWriter writer = new StreamWriter(strFilePath, false, Encoding.UTF8);

            writer.WriteLine("[HelpViewer]");
            writer.WriteLine(LOCATION_TAG + "=" + frm.Location.X.ToString() + "," + frm.Location.Y.ToString());
            writer.WriteLine(SIZE_TAG + "=" + frm.Size.Width.ToString() + "," + frm.Size.Height.ToString());
            writer.WriteLine(SPLIT_DISTANCE_TAG + "=" + nSplitterDistance.ToString());
            writer.WriteLine(MAXIMIZE_TAG + "=" + (frm.WindowState == FormWindowState.Maximized).ToString());

            writer.Close();
        }

        public static void SetPrevLocationNSize(Form frm, Point ptPrevLocation, bool isPrevMaximize, Size sizePrev)
        {
            frm.StartPosition = FormStartPosition.Manual;
            frm.Location = ptPrevLocation;

            if (isPrevMaximize)
                frm.WindowState = FormWindowState.Maximized;
            else
                frm.Size = sizePrev;
        }

        // 이전에 실행했을 당시의 크기와 위치 정보를 얻어온다.
        public static bool ReadPrevLocationNSize(ref Point ptPrevLocation, ref Size sizePrev, ref int nPrevSplitDistance, ref bool isPrevMaximize)
        {
            string strFolderName = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\SOP";

            if (Directory.Exists(strFolderName) == false)
                Directory.CreateDirectory(strFolderName);

            strFolderName = strFolderName + "\\HelpHtml";

            if (Directory.Exists(strFolderName) == false)
                Directory.CreateDirectory(strFolderName);

            string strFilePath = strFolderName + "\\" + CONFIG_FILE;

            if (File.Exists(strFilePath))
            {
                int x = 0, y = 0;
                bool location = false, size = false, distance = false, maximize = false;

                StreamReader reader = new StreamReader(strFilePath);

                while (!reader.EndOfStream)
                {
                    string[] tokens = reader.ReadLine().Split('=');

                    if (tokens.Count() != 2)
                        continue;

                    string strName = tokens[0].Trim();

                    if (string.Compare(strName, LOCATION_TAG, true) == 0)
                    {
                        if (GetXY(tokens[1], ref x, ref y))
                        {
                            ptPrevLocation.X = x;
                            ptPrevLocation.Y = y;
                            location = true;
                        }
                    }
                    else if (string.Compare(strName, SIZE_TAG, true) == 0)
                    {
                        if (GetXY(tokens[1], ref x, ref y))
                        {
                            sizePrev.Width = x;
                            sizePrev.Height = y;
                            size = true;
                        }
                    }
                    else if (string.Compare(strName, SPLIT_DISTANCE_TAG, true) == 0)
                    {
                        if (int.TryParse(tokens[1].Trim(), out x))
                        {
                            nPrevSplitDistance = x;
                            distance = true;
                        }
                    }
                    else if (string.Compare(strName, MAXIMIZE_TAG, true) == 0)
                    {
                        string strValue = tokens[1].Trim();

                        if (string.Compare(strValue, "true", true) == 0)
                        {
                            isPrevMaximize = true;
                            maximize = true;
                        }
                        else if (string.Compare(strValue, "false", true) == 0)
                        {
                            isPrevMaximize = false;
                            maximize = true;
                        }
                        else if (int.TryParse(tokens[1].Trim(), out x))
                        {
                            isPrevMaximize = x != 1;
                            maximize = true;
                        }
                    }
                }

                reader.Close();

                return location && size && distance && maximize;
            }

            return false;
        }

        private static bool GetXY(string str, ref int x, ref int y)
        {
            string[] xy = str.Trim().Split(',');

            if (xy.Count() != 2)
                return false;

            if (int.TryParse(xy[0].Trim(), out x) == false)
                return false;

            if (int.TryParse(xy[1].Trim(), out y) == false)
                return false;

            return true;
        }

        private bool IsExistURL(string strURL, ref bool isLocalPath, ref string strRemoteResult)
        {
            if (strURL.Length < 2)
                return false;

            char first = strURL.ElementAt(0);
            char second = strURL.ElementAt(1);

            // Local 경로인가?
            if (((first >= 'a' && first <= 'z') || (first >= 'A' && first <= 'Z')) && second == ':')
            {
                isLocalPath = true;
                return Directory.Exists(strURL);
            }

            isLocalPath = false;

            strRemoteResult = WebSearch.SearchURL(strURL, m_strWebServerURL);

            if (strRemoteResult.Contains("<InvalidPath/>"))
                return false;

            return true;
        }

        private List<string> GetItems(string strLine, string strItemName)
        {
            string strBeginTag = "<" + strItemName + ">";
            string strEndTag = "</" + strItemName + ">";

            int nIndex1 = strLine.IndexOf(strBeginTag);
            int nIndex2 = strLine.IndexOf(strEndTag);

            List<string> items = new List<string>();

            if (nIndex1 >= 0 && nIndex2 > nIndex1)
            {
                string strItems = strLine.Substring(nIndex1 + strBeginTag.Length, nIndex2 - nIndex1 - strBeginTag.Length);
                string[] tokens = strItems.Split(';');

                foreach (string strToken in tokens)
                {
                    items.Add(strToken.Trim());
                }
            }

            return items;
        }

        private void FormMain_ResizeBegin(object sender, EventArgs e)
        {
            FixSplitDistance();
        }

        private void FormMain_ResizeEnd(object sender, EventArgs e)
        {
            UnFixSplitDistance();
        }

        private const int SC_RESTORE = 0xF120;
        private const int SC_RESTORE2 = 0xF122;
        private const int SC_MAXIMIZE = 0xF030;
        private const int SC_MAXIMIZE2 = 0xF032;
        private const int SC_MINIMIZE = 0xF020;

        private const int WM_COPYDATA = 0x4A;

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, uint wParam, ref COPYDATASTRUCT lParam);

        public struct COPYDATASTRUCT
        {
            public IntPtr dwData;
            public int cbData;
            [MarshalAs(UnmanagedType.LPStr)]
            public string lpData;
        }

        protected override void WndProc(ref Message m)
        {
            // WM_SYSCOMMAND
            if (m.Msg == 0x0112)
            {
                int wParam = (int)m.WParam;

                if (wParam == SC_RESTORE || wParam == SC_RESTORE2 ||
                    wParam == SC_MAXIMIZE || wParam == SC_MINIMIZE ||
                    wParam == SC_MAXIMIZE2)
                {
                    FixSplitDistance();
                }
            }
            else if (m.Msg == WM_COPYDATA)
            {
                COPYDATASTRUCT cds = (COPYDATASTRUCT)m.GetLParam(typeof(COPYDATASTRUCT));
                OnReceive(cds);
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

            if (strCommand == "GetAllPageList")
                SendTreeItems(cds.dwData);
            else if (strCommand == "SelectNode")
            {
                if (nTokenCount > 1)
                    SelectNode(tokens[1].Trim());
            }
            else if (strCommand == "SelectID")
            {
                if (nTokenCount > 1)
                    SelectIDNode(tokens[1].Trim());
            }
        }

        private void SelectIDNode(string strID)
        {
            Node node = SearchIDNode(m_model.Nodes, strID);

            if (node != null)
            {
                TreeNodeAdv selectedNode = treeViewAdv1.FindNode(m_model.GetPath(node));
                treeViewAdv1.SelectedNode = selectedNode;
            }
        }

        private void SelectNode(string strNodeFullPath)
        {
            string[] nodeNames = strNodeFullPath.Split('\\');
            int nNodeCount = nodeNames.Count();

            TreeNodeAdv node = treeViewAdv1.Root;

            for (int i=0;i<nNodeCount;i++)
            {
                node = FindTreeNodeAdv(node, nodeNames[i]);

                if (node == null)
                    return;
            }

            if (node != treeViewAdv1.Root)
                treeViewAdv1.SelectedNode = node;
        }

        private TreeNodeAdv FindTreeNodeAdv(TreeNodeAdv parent, string strNodeName)
        {
            foreach (TreeNodeAdv node in parent.Nodes)
            {
                if (node.ToString() == strNodeName)
                    return node;
            }

            return null;
        }

        private void SendTreeItems(IntPtr handle)
        {
            TreeNodeAdv root = treeViewAdv1.Root;
            //int nDepth = 0, nPrevDepth = 0;
            TreeNodeAdv prevNode = null;

            string strItem = "";
            
            foreach (TreeNodeAdv node in root.Nodes)
            {
                int dir = prevNode == null ? 0 : node.Level - prevNode.Level;

                if (strItem.Length == 0)
                    strItem = "(" + dir.ToString() + ")" + node.ToString();
                else
                    strItem += "\t(" + dir.ToString() + ")" + node.ToString();

                prevNode = node;
                SetTreeItems(node, ref strItem, ref prevNode);
            }

            if (strItem.Length == 0)
                return;

            string strMessage = "AllPageList\t" + strItem;
            byte[] buff = System.Text.Encoding.Default.GetBytes(strMessage);

            COPYDATASTRUCT cds = new COPYDATASTRUCT();
            cds.dwData = this.Handle;
            cds.cbData = buff.Length + 1;
            cds.lpData = strMessage;

            SendMessage(handle, WM_COPYDATA, 0, ref cds);
        }

        private void SetTreeItems(TreeNodeAdv parent, ref string strItem, ref TreeNodeAdv prevNode)
        {
            if (parent.Nodes.Count == 0)
                return;

            foreach (TreeNodeAdv node in parent.Nodes)
            {
                int dir = node.Level - prevNode.Level;

                if (strItem == null)
                    strItem = "(" + dir.ToString() + ")" + node.ToString();
                else
                    strItem += "\t(" + dir.ToString() + ")" + node.ToString();

                prevNode = node;
                SetTreeItems(node, ref strItem, ref prevNode);
            }
        }

        // FormMain의 크기가 변경될때 Split Distance가 바뀌지 않도록 한다.
        private void FixSplitDistance()
        {
            splitContainerBody.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
        }

        private void UnFixSplitDistance()
        {
            splitContainerBody.FixedPanel = System.Windows.Forms.FixedPanel.None;
        }
        
        private static string regSubkey = "Software\\Microsoft\\Internet Explorer\\PageSetup";
        private RegistryKey rk = Registry.CurrentUser.OpenSubKey(regSubkey, true);

        private void webViewer_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            HighlightHtmlText(m_strHighlightText);
            m_strHighlightText = null;

            if (m_nPrintURLIndex >= 0 && m_bPrintAll)
            {
                int curPage = m_nPrintURLIndex;

                Console.WriteLine(webViewer.Url.LocalPath);
                
                rk.SetValue("footer", "&b" + curPage + " - &p&b");
                 
                webViewer.Print();
                if (m_nPrintURLIndex >= webUrls.Count)
                {
                    m_nPrintURLIndex = -1;
                    webUrls.Clear();
                }
                else
                    ShowNextPrintURL();                

                if (m_bPrintAll && m_nPrintURLIndex == -1)
                {
                    m_bPrintAll = false;                    
                    if (m_strPrintAllLastUrl.Length > 0)
                        webViewer.Navigate(m_strPrintAllLastUrl);

                    m_strPrintAllLastUrl = "";
                    rk.SetValue("footer", "&b&p&b");
                }
            }
        }

        private void webViewer_Navigated(object sender, WebBrowserNavigatedEventArgs e)
        {
            rbtnUndo.Enabled = webViewer.CanGoBack;
            rbtnRedo.Enabled = webViewer.CanGoForward;

            string strFileTag = "file:///";
            string strURL = e.Url.ToString();

            if (strURL.StartsWith(strFileTag))
            {
                strURL = strURL.Substring(strFileTag.Length);
                strURL = strURL.Replace('/', '\\');
            }

            bool isTitle = true;
            int nSharpIndex = strURL.LastIndexOf('#');

            if (nSharpIndex >= 0)
            {
                int nDotIndex = strURL.LastIndexOf('.');

                if (nDotIndex < nSharpIndex)
                    isTitle = false;
            }

            Node node = FindTreeNode(strURL, m_model.Nodes, isTitle);
            //TreeNode node = FindTreeNode(strURL, treeIndex.Nodes, isTitle);

            if (node != null)
            {
                m_systemCall = true;
                treeViewAdv1.SelectedNode = treeViewAdv1.FindNode(m_model.GetPath(node));
                //treeIndex.SelectedNode = node;
                m_systemCall = false;
            }
        }

        private void rbtnUndo_Click(object sender, EventArgs e)
        {
            webViewer.GoBack();
        }

        private void rbtnRedo_Click(object sender, EventArgs e)
        {
            webViewer.GoForward();
        }

        private void rbtnPrint_Click(object sender, EventArgs e)
        {
            Point pt = new Point(rbtnPrint.Location.X + rbtnPrint.Size.Width, rbtnPrint.Location.Y + rbtnPrint.Size.Height);
            printMenu.Show(panelTop, pt);
        }

        private void tsMenuPrint_Click(object sender, EventArgs e)
        {
            webViewer.ShowPrintDialog();
        }

        private void tsPrintAll_Click(object sender, EventArgs e)
        {
            Node selectedNode = FindNode(treeViewAdv1.SelectedNode);

            if (selectedNode.Tag != null && selectedNode.Tag is PageData)
            {
                PageData data = (PageData)selectedNode.Tag;

                if (data.IsPageTitle)
                    m_strPrintAllLastUrl = data.PageURL;
                else
                    m_strPrintAllLastUrl = data.PageURL + "#" + data.LinkName;
            }

            webUrls.Clear();
            TreeNodeAdv rootNode = treeViewAdv1.Root.Nodes[0];
            FindUrls(rootNode);

            if (webUrls != null && webUrls.Count > 0)
            {
                System.Threading.Thread t = new System.Threading.Thread(new System.Threading.ThreadStart(PageSetupThread));
                t.Start();

                webViewer.ShowPageSetupDialog();
            }
        }

        [DllImport("user32.dll")]
        static extern IntPtr GetActiveWindow();

        private PageSetupDialogListener m_pageSetupListener = null;

        private void PageSetupThread()
        {
            IntPtr dlgHandle = IntPtr.Zero;

            while (dlgHandle == IntPtr.Zero)
            {
                this.Invoke((MethodInvoker)delegate
                {
                    IntPtr handle = GetActiveWindow();

                    if (handle != IntPtr.Zero && handle != this.Handle)
                        dlgHandle = handle;
                    else
                        System.Threading.Thread.Sleep(100);
                });
            }

            m_pageSetupListener = new PageSetupDialogListener(dlgHandle, this);
        }

        public void OnPrintPageSetupOK()
        {
            m_nPrintURLIndex = 0;
            m_bPrintAll = true;
            ShowNextPrintURL();
        }

        public void OnPrintPageSetupCancel()
        {
            m_pageSetupListener = null;
            m_strPrintAllLastUrl = "";
        }

        private TreeNodeAdv FindUrls(TreeNodeAdv treeNode, System.Collections.ObjectModel.Collection<Node> nodes = null)
        {            
            if (nodes == null)
            {
                nodes = m_model.Root.Nodes;
            }

            foreach (Node node in nodes)
            {
                if (node.Tag != null && node.Tag is PageData)
                {
                    PageData data = node.Tag as HelpViewer.PageData;
                    if (data.PageURL.Length > 0 && !webUrls.Contains(data.PageURL))
                        webUrls.Add(data.PageURL);

                    TreeNodeAdv findNode = FindUrls(treeNode, node.Nodes);
                    if (findNode != null)
                        return findNode;
                }
            }

            return null;
        }

        private List<string> webUrls = new List<string>();
        private int m_nPrintURLIndex = -1;
        private bool m_bPrintAll = false;
        private string m_strPrintAllLastUrl = "";
        private void ShowNextPrintURL()
        {
            if (webUrls == null)
                return;

            if (m_nPrintURLIndex >= webUrls.Count)
            {                
                return;
            }
                        
            webViewer.Navigate(webUrls[m_nPrintURLIndex]);
            Console.WriteLine(webUrls[m_nPrintURLIndex]);
            m_nPrintURLIndex++;
        }

        private void tsMenuPrintPreview_Click(object sender, EventArgs e)
        {
            webViewer.ShowPrintPreviewDialog();
        }

        private void ClearSearchResult()
        {
            panelSearchResult.Controls.Clear();

            panelSearchResult.Controls.Add(pictureBoxSearchResultIcon);
            panelSearchResult.Controls.Add(labelSearchResult);
            panelSearchResult.Controls.Add(pictureBoxSearchResultLine);
        }

        private void rbtnSearch_Click(object sender, EventArgs e)
        {
            ClearSearchResult();
            string strSearch = textBoxSearch.Text.Trim();

            if (strSearch.Length == 0)
            {
                labelSearchLabel.Text = "검색어를 입력하세요";
                labelSearchLabel.Font = new Font(labelSearchLabel.Font, FontStyle.Italic);
            }
            else
            {
                labelSearchLabel.Text = strSearch;

                if (labelSearchLabel.Font.Italic)
                    labelSearchLabel.Font = new Font(labelSearchLabel.Font, FontStyle.Regular);
            }
            
            textBoxSearch.Visible = false;
            panelSearchLabel.Visible = true;

            if (strSearch.Length == 0)
                return;

            SearchNodes(m_model.Nodes, strSearch);
            //SearchNodes(treeIndex.Nodes, strSearch);

            if (panelSearchResult.Controls.Count <= 3)
                panelSearchResult.Controls.Add(labelNoResult);

            tabControlHeader.SelectedTab = tabPageSearchResultHeader;
        }

        private void tabControlHeader_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabControlHeader.SelectedTab == tabPageIndexHeader)
                tabControlBody.SelectedTab = tabPageIndex;
            else
                tabControlBody.SelectedTab = tabPageSearchResult;
        }

        private void textBoxSearch_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                rbtnSearch_Click(null, null);
            else if (e.KeyCode == Keys.Escape)
            {
                textBoxSearch.Text = "";
                textBoxSearch.Visible = false;
                panelSearchLabel.Visible = true;
            }
        }

        private void HighlightHtmlText(string strText)
        {
            if (strText == null)
                return;

            mshtml.IHTMLDocument2 doc = (mshtml.IHTMLDocument2)webViewer.Document.DomDocument;

            if (doc.body == null)
                return;

            // Text만 바꿔준다.
            HighlightText(doc.body, strText);

            // Text 뿐만 아니라 URL이나 Tag 이름까지도 바꿔버림
            /*StringBuilder html = new StringBuilder(doc.body.outerHTML);

            string strRGB = string.Format("rgb({0}, {1}, {2})", (int)m_clrHighlightBackground.R, (int)m_clrHighlightBackground.G, (int)m_clrHighlightBackground.B);
            string substitution = "<span style='background-color: " + strRGB + ";'>" + strText + "</span>";
            html.Replace(strText, substitution);

            doc.body.innerHTML = html.ToString();*/
        }

        // Text만 골라내어 <span...>으로 감싼다.
        private void HighlightText(mshtml.IHTMLElement body, string strText)
        {
            string strHtml = "";
            string strOriginHtml = body.outerHTML;

            string strRGB = string.Format("rgb({0}, {1}, {2})", (int)m_clrHighlightBackground.R, (int)m_clrHighlightBackground.G, (int)m_clrHighlightBackground.B);
            string substitution = "<span style='background-color: " + strRGB + ";'>" + strText + "</span>";

            int nIndex = strOriginHtml.IndexOf('<');

            if (nIndex > 0)
                strHtml = strOriginHtml.Substring(0, nIndex);

            while (nIndex >= 0)
            {
                int nIndex2 = strOriginHtml.IndexOf('>', nIndex + 1);

                if (nIndex2 < 0)
                {
                    strHtml += strOriginHtml.Substring(nIndex);
                    break;
                }

                strHtml += strOriginHtml.Substring(nIndex, nIndex2 - nIndex + 1);

                nIndex = strOriginHtml.IndexOf('<', nIndex2 + 1);

                if (nIndex > nIndex2 + 1)
                    strHtml += strOriginHtml.Substring(nIndex2 + 1, nIndex - nIndex2 - 1).Replace(strText, substitution);
            }

            body.innerHTML = strHtml;
        }

        private void SearchResultLinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            LinkLabelEx label = (LinkLabelEx)sender;
            Node node = (Node)label.Tag;
            //TreeNode node = (TreeNode)label.Tag;

            TreeNodeAdv selectedNode = treeViewAdv1.FindNode(m_model.GetPath(node));

            if (treeViewAdv1.SelectedNode != selectedNode && selectedNode != null)
            {
                m_strHighlightText = label.SearchText;
            }

            treeViewAdv1.SelectedNode = selectedNode;
            //treeIndex.SelectedNode = node;
            tabControlHeader.SelectedTab = tabPageIndexHeader;

            if (m_strHighlightText == null)
                HighlightHtmlText(label.SearchText);
        }

        private void splitContainerBody_SplitterMoved(object sender, SplitterEventArgs e)
        {
            panelRed.Size = new Size(CurrentSplitDistance, panelRed.Size.Height);

            int nPrintSpace = rbtnPrint.Location.X - rbtnUndo.Location.X;
            int nRedoSpace = rbtnRedo.Location.X - rbtnUndo.Location.X;

            rbtnUndo.Location = new Point(panelRed.Location.X + panelRed.Size.Width + m_nInitUndoSpace, rbtnUndo.Location.Y);
            rbtnRedo.Location = new Point(rbtnUndo.Location.X + nRedoSpace, rbtnRedo.Location.Y);
            rbtnPrint.Location = new Point(rbtnUndo.Location.X + nPrintSpace, rbtnPrint.Location.Y);
        }

        private void labelSearchLabel_MouseEnter(object sender, EventArgs e)
        {
            this.Cursor = Cursors.IBeam;
        }

        private void labelSearchLabel_MouseLeave(object sender, EventArgs e)
        {
            this.Cursor = Cursors.Arrow;
        }

        private void labelSearchLabel_Click(object sender, EventArgs e)
        {
            if (labelSearchLabel.Font.Italic == false)
                textBoxSearch.Text = labelSearchLabel.Text;

            panelSearchLabel.Visible = false;
            textBoxSearch.Visible = true;
            textBoxSearch.Focus();
        }

        #region Aga.Controls.Tree.TreeViewAdv
        private int GetNodeLevel(Node node)
        {
            int nDepth = 1;
            Node parent = node.Parent;

            while (parent != null)
            {
                parent = parent.Parent;
                nDepth++;
            }

            return nDepth;
        }

        // nDepth 까지만 확장한다.
        private void ExpandTree(System.Collections.ObjectModel.Collection<Node> nodes, int nDepth)
        {
            foreach (Node node in nodes)
            {
                if (GetNodeLevel(node) <= nDepth && node.Nodes.Count > 0)
                {
                    TreeNodeAdv treeNode = treeViewAdv1.FindNode(m_model.GetPath(node));

                    if (treeNode != null)
                        treeNode.IsExpanded = true;
                    
                    ExpandTree(node.Nodes, nDepth);
                }
            }
        }

        private void InitTree()
        {
            if (m_strRootURL.Length == 0)
                return;

            bool isLocalPath = false;
            string strRemoteResult = "";

            if (IsExistURL(m_strRootURL, ref isLocalPath, ref strRemoteResult) == false)
                return;
            //if (Directory.Exists(m_strRootURL) == false)
            //    return;

            SetTitle(isLocalPath);

            if (isLocalPath)
                SearchFolder(m_strRootURL, m_model.Nodes);
            else
                SearchWebFolder(m_strRootURL, strRemoteResult, m_model.Nodes);

            if (treeViewAdv1.SelectedNode == null && m_model.Nodes.Count > 0)
            {
                treeViewAdv1.SelectedNode = treeViewAdv1.FindNode(m_model.GetPath(m_model.Nodes[0]));
            }
        }

        private void SetTitle(bool isLocalPath)
        {
            string strPath = "";

            if (isLocalPath)
            {
                strPath = m_strRootURL + "\\title.txt";
            }
            else
            {
                string strFolderName = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\SOP\\HelpHtml";

                try
                {
                    strPath = strFolderName + "\\title.txt";

                    WebClient web = new WebClient();
                    web.DownloadFile(m_strRootURL + "/title.txt", strPath);
                }
                catch (Exception e)
                {
                    System.Diagnostics.Trace.WriteLine(e.Message);
                    return;
                }
            }

            if (File.Exists(strPath) == false)
                return;

            StreamReader reader = new StreamReader(strPath, Encoding.UTF8);

            while (reader.EndOfStream == false)
            {
                string strLine = reader.ReadLine();

                if (strLine.Trim().Length == 0)
                    continue;

                labelSystemName.Text = strLine;
                break;
            }

            reader.Close();
        }

        private void SearchWebFolder(string strFolderPath, string strItems, System.Collections.ObjectModel.Collection<Node> nodes)
        {
            List<string> files = GetItems(strItems, "File");

            if (files == null)
                return;

            if (!strFolderPath.EndsWith("/"))
                strFolderPath += "/";

            foreach (string strFile in files)
            {
                if (strFile.EndsWith("html", true, System.Globalization.CultureInfo.CurrentCulture) || strFile.EndsWith("htm", true, System.Globalization.CultureInfo.CurrentCulture))
                {
                    ReadWebFile(strFolderPath + strFile, nodes);
                }
            }

            List<string> folders = GetItems(strItems, "Folder");

            if (folders == null)
                return;

            string strBaseURL = strFolderPath.Substring(0, strFolderPath.Length - 1);

            foreach (string strFolder in folders)
            {
                Node node = FindTreeNode(strFolder, nodes, strBaseURL, '/');

                if (node != null)
                {
                    string strItems2 = WebSearch.SearchURL(strFolderPath + strFolder, m_strWebServerURL);
                    SearchWebFolder(strFolderPath + strFolder, strItems2, node.Nodes);
                }
            }
        }

        private void ReadWebFile(string strURL, System.Collections.ObjectModel.Collection<Node> nodes)
        {
            string strFolderName = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\SOP\\HelpHtml";

            try
            {
                string strLocalFilePath = strFolderName + "\\temp.html";

                WebClient web = new WebClient();
                web.DownloadFile(strURL, strLocalFilePath);

                ReadFile(strLocalFilePath, nodes, strURL);
            }
            catch (Exception)
            {
            }
        }

        private void SearchFolder(string strFolderPath, System.Collections.ObjectModel.Collection<Node> nodes)
        {
            string[] files = Directory.GetFiles(strFolderPath);

            if (files == null)
                return;

            foreach (string strFile in files)
            {
                if (strFile.EndsWith("html", true, System.Globalization.CultureInfo.CurrentCulture) || strFile.EndsWith("htm", true, System.Globalization.CultureInfo.CurrentCulture))
                {
                    ReadFile(strFile, nodes);
                }
            }

            string[] folders = Directory.GetDirectories(strFolderPath);

            if (folders == null)
                return;

            foreach (string strFolder in folders)
            {
                int nIndex = strFolder.LastIndexOf('\\');

                if (nIndex < 0)
                    continue;

                string strFolderName = strFolder.Substring(nIndex + 1);
                Node node = FindTreeNode(strFolderName, nodes, strFolderPath, '\\');

                if (node != null)
                    SearchFolder(strFolder, node.Nodes);
            }
        }

        private Node FindTreeNode(string strFolderName, System.Collections.ObjectModel.Collection<Node> nodes, string strBaseURL, char delimeter)
        {
            foreach (Node node in nodes)
            {
                if (node.Tag != null && node.Tag is PageData)
                {
                    PageData data = (PageData)node.Tag;

                    int nIndex = data.PageURL.LastIndexOf(delimeter);

                    if (nIndex < 0)
                        continue;

                    string strParentURL = data.PageURL.Substring(0, nIndex);

                    if (strParentURL != strBaseURL)
                        continue;

                    if (data.ContainsChildFolder(strFolderName))
                        return node;

                    Node child = FindTreeNode(strFolderName, node.Nodes, strBaseURL, delimeter);

                    if (child != null)
                        return child;
                }
            }

            // 1. nodes의 노드들을 먼저 검사한다.
            /*foreach (Node node in nodes)
            {
                if (node.Tag != null && node.Tag is PageData)
                {
                    PageData data = (PageData)node.Tag;

                    if (data.ContainsChildFolder(strFolderName))
                        return node;
                }
            }

            // 2. nodes의 자식 노드들을 검사한다.
            foreach (Node node in nodes)
            {
                foreach (Node child in node.Nodes)
                {
                    if (child.Tag != null && child.Tag is PageData)
                    {
                        PageData data = (PageData)child.Tag;

                        if (data.ContainsChildFolder(strFolderName))
                            return child;
                    }
                }
            }*/

            return null;
        }

        private Node FindTreeNode(string strURL, System.Collections.ObjectModel.Collection<Node> nodes, bool isTitle)
        {
            foreach (Node node in nodes)
            {
                if (node.Tag != null && node.Tag is PageData)
                {
                    PageData data = (PageData)node.Tag;

                    if (isTitle == data.IsPageTitle)
                    {
                        if (isTitle == false)
                        {
                            if (strURL == data.PageURL + "#" + data.LinkName)
                                return node;
                        }
                        else
                        {
                            if (strURL == data.PageURL)
                                return node;
                        }
                    }
                }

                Node child = FindTreeNode(strURL, node.Nodes, isTitle);

                if (child != null)
                    return child;
            }

            return null;
        }

        private void ReadFile(string strFilePath, System.Collections.ObjectModel.Collection<Node> nodes, string strPageURL = null)
        {
            if (strPageURL == null)
                strPageURL = strFilePath;

            try
            {
                StreamReader reader = new StreamReader(strFilePath, WebSearch.PageEncoding);
                //StreamReader reader = new StreamReader(strFilePath, Encoding.UTF8);
                HtmlParser parser = new HtmlParser();
                int nLineCount = 0;

                while (!reader.EndOfStream)
                {
                    nLineCount++;
                    string strLine = reader.ReadLine().Trim();

                    if (!parser.ReadLine(strLine))
                    {
                        reader.Close();
                        return;
                    }
                }

                reader.Close();

                AddTreeItem(nodes, parser, strPageURL);
            }
            catch (Exception)
            {
            }
        }

        private void AddNode(System.Collections.ObjectModel.Collection<Node> nodes, Node node, PageData data)
        {
            int nNodeCount = nodes.Count;

            for (int i = 0; i < nNodeCount; i++)
            {
                Node _node = nodes[i];
                Node _node2 = i == nNodeCount - 1 ? null : nodes[i + 1];

                if (_node.Tag != null && _node.Tag is PageData)
                {
                    PageData _data = (PageData)_node.Tag;
                    PageData _data2 = _node2 != null && _node2.Tag != null && _node2.Tag is PageData ? (PageData)_node2.Tag : null;

                    if (data.OrderIndex < _data.OrderIndex)
                    {
                        nodes.Insert(i, node);
                        return;
                    }

                    if (_data2 != null)
                    {
                        if (_data.OrderIndex <= data.OrderIndex && data.OrderIndex <= _data2.OrderIndex)
                        {
                            nodes.Insert(i + 1, node);
                            return;
                        }
                    }
                    else
                    {
                        if (_data.OrderIndex <= data.OrderIndex)
                        {
                            nodes.Insert(i + 1, node);
                            return;
                        }
                    }
                }
            }

            nodes.Add(node);
        }

        private void AddTreeItem(System.Collections.ObjectModel.Collection<Node> nodes, HtmlParser parser, string strPageURL)
        {
            PageData data = parser.Calc();

            if (data == null)
                return;

            data.PageURL = strPageURL;

            Node node = new Node(data.TreeItem);
            node.Tag = data;
            //nodes.Add(node);
            AddNode(nodes, node, data);
            
            AddTreeItem(data, strPageURL, node);
        }

        private void AddTreeItem(PageData data, string strPageURL, Node node)
        {
            foreach (PageData child in data.ChildDatas)
            {
                child.PageURL = strPageURL;

                Node childNode = new Node(child.TreeItem);
                childNode.Tag = child;
                AddNode(node.Nodes, childNode, child);
                
                AddTreeItem(child, strPageURL, childNode);
            }
        }

        private void AddSearchResult(Node node, string strTag)
        {
            int nItemCount = panelSearchResult.Controls.Count - 3;

            LinkLabelEx label = new LinkLabelEx();
            panelSearchResult.Controls.Add(label);

            int nInitPos = 45;

            label.Location = new Point(15, nInitPos + 30 * nItemCount);
            label.Text = node.Text;
            label.Tag = node;
            label.Font = labelNoResult.Font;
            label.LinkColor = label.VisitedLinkColor = labelNoResult.ForeColor;
            label.AutoSize = true;
            label.SearchText = strTag;

            ToolTip tooltip = new ToolTip();
            tooltip.SetToolTip(label, GetFullPath(node));

            label.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.SearchResultLinkClicked);
        }

        private string GetFullPath(Node node)
        {
            string strPath = node.Text;

            while (node.Parent != null)
            {
                strPath = node.Parent.Text + "/" + strPath;
                node = node.Parent;
            }

            if (strPath.StartsWith("/"))
                strPath = strPath.Substring(1);

            return strPath;
        }

        private Node SearchIDNode(System.Collections.ObjectModel.Collection<Node> nodes, string strID)
        {
            foreach (Node node in nodes)
            {
                if (node.Tag != null && node.Tag is PageData)
                {
                    PageData data = (PageData)node.Tag;

                    if (string.Compare(data.ID, strID, true) == 0)
                        return node;
                }

                Node findNode = SearchIDNode(node.Nodes, strID);

                if (findNode != null)
                    return findNode;
            }

            return null;
        }

        private void SearchNodes(System.Collections.ObjectModel.Collection<Node> nodes, string strTag)
        {
            foreach (Node node in nodes)
            {
                if (node.Tag != null && node.Tag is PageData)
                {
                    bool added = false;
                    PageData data = (PageData)node.Tag;

                    // Tag 검사
                    foreach (string tag in data.Tags)
                    {
                        if (string.Compare(tag, strTag, true) == 0)
                        {
                            AddSearchResult(node, strTag);
                            added = true;
                            break;
                        }
                    }

                    // Text 검사
                    if (added == false)
                    {
                        if (data.DisplayText.IndexOf(strTag) >= 0)
                            AddSearchResult(node, strTag);
                    }
                }

                SearchNodes(node.Nodes, strTag);
            }
        }
        
        private Node FindNode(TreeNodeAdv treeNode)
        {
            List<TreeNodeAdv> treeNodes = new List<TreeNodeAdv>();
            treeNodes.Add(treeNode);

            TreeNodeAdv parentTreeNode = treeNode.Level > 1 ? treeNode.Parent : null;

            while (parentTreeNode != null)
            {
                treeNodes.Add(parentTreeNode);
                parentTreeNode = parentTreeNode.Level > 1 ? parentTreeNode.Parent : null;
            }

            Node node = null;
            int nNodeCount = treeNodes.Count;

            for (int i=nNodeCount-1;i>=0;i--)
            {
                if (i == nNodeCount - 1)
                    node = FindNode(treeNodes[i], m_model.Nodes);
                else
                    node = FindNode(treeNodes[i], node.Nodes);

                if (node == null)
                    break;
            }

            return node;
        }

        private Node FindNode(TreeNodeAdv treeNode, System.Collections.ObjectModel.Collection<Node> nodes)
        {
            string strTreeNode = treeNode.ToString();
            List<Node> findList = new List<Node>();

            foreach (Node node in nodes)
            {
                if (strTreeNode == node.Text)
                    findList.Add(node);
            }

            if (findList.Count == 0)
                return null;
            else if (findList.Count == 1)
                return findList[0];

            foreach (Node node in findList)
            {
                if (treeViewAdv1.FindNode(m_model.GetPath(node)) == treeNode)
                    return node;
            }

            return null;
        }

        private void treeViewAdv1_SelectionChanged(object sender, EventArgs e)
        {
            if (m_systemCall)
                return;

            if (treeViewAdv1.SelectedNode == null)
                return;

            Node selectedNode = FindNode(treeViewAdv1.SelectedNode);

            if (selectedNode.Tag != null && selectedNode.Tag is PageData)
            {
                PageData data = (PageData)selectedNode.Tag;

                if (data.IsPageTitle)
                    webViewer.Navigate(data.PageURL);
                else
                    webViewer.Navigate(data.PageURL + "#" + data.LinkName);
            }
        }
        #endregion Aga.Controls.Tree.TreeViewAdv

        #region 일반 TreeView
        // nDepth 까지만 확장한다.
        /*private void ExpandTree(TreeNodeCollection nodes, int nDepth)
        {
            foreach (TreeNode node in nodes)
            {
                if (node.Level <= nDepth && node.Nodes.Count > 0)
                {
                    node.Expand();
                    ExpandTree(node.Nodes, nDepth);
                }
            }
        }

        private void InitTree()
        {
            treeIndex.ImageList = new ImageList();
            treeIndex.ImageList.Images.Add(global::HelpViewer.Properties.Resources.plus_normal);
            treeIndex.ImageList.Images.Add(global::HelpViewer.Properties.Resources.plus_over);
            treeIndex.ImageList.Images.Add(global::HelpViewer.Properties.Resources.minus_normal);
            treeIndex.ImageList.Images.Add(global::HelpViewer.Properties.Resources.minus_over);

            NO_CHILD = treeIndex.ImageList.Images.Count;
            m_nImageSize = treeIndex.ImageList.Images[0].Width;

            if (m_strRootURL.Length == 0)
                return;

            bool isLocalPath = false;
            string strRemoteResult = "";

            if (IsExistURL(m_strRootURL, ref isLocalPath, ref strRemoteResult) == false)
                return;
            //if (Directory.Exists(m_strRootURL) == false)
            //    return;

            if (isLocalPath)
                SearchFolder(m_strRootURL, treeIndex.Nodes);
            else
                SearchWebFolder(m_strRootURL, strRemoteResult, treeIndex.Nodes);

            if (treeIndex.SelectedNode == null && treeIndex.Nodes.Count > 0)
            {
                treeIndex.SelectedNode = treeIndex.Nodes[0];
            }
        }

        private void SearchWebFolder(string strFolderPath, string strItems, TreeNodeCollection nodes)
        {
            List<string> files = GetItems(strItems, "File");

            if (files == null)
                return;

            if (!strFolderPath.EndsWith("/"))
                strFolderPath += "/";

            foreach (string strFile in files)
            {
                if (strFile.EndsWith("html", true, System.Globalization.CultureInfo.CurrentCulture) || strFile.EndsWith("htm", true, System.Globalization.CultureInfo.CurrentCulture))
                {
                    ReadWebFile(strFolderPath + strFile, nodes);
                }
            }

            List<string> folders = GetItems(strItems, "Folder");

            if (folders == null)
                return;

            foreach (string strFolder in folders)
            {
                TreeNode node = FindTreeNode(strFolder, nodes);

                if (node != null)
                {
                    string strItems2 = WebSearch.SearchURL(strFolderPath + strFolder, m_strWebServerURL);
                    SearchWebFolder(strFolderPath + strFolder, strItems2, node.Nodes);
                }
            }
        }

        private void ReadWebFile(string strURL, TreeNodeCollection nodes)
        {
            string strFolderName = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\SOP\\HelpHtml";

            try
            {
                string strLocalFilePath = strFolderName + "\\temp.html";

                WebClient web = new WebClient();
                web.DownloadFile(strURL, strLocalFilePath);

                ReadFile(strLocalFilePath, nodes, strURL);
            }
            catch (Exception)
            {
            }
        }

        private void SearchFolder(string strFolderPath, TreeNodeCollection nodes)
        {
            string[] files = Directory.GetFiles(strFolderPath);

            if (files == null)
                return;

            foreach (string strFile in files)
            {
                if (strFile.EndsWith("html", true, System.Globalization.CultureInfo.CurrentCulture) || strFile.EndsWith("htm", true, System.Globalization.CultureInfo.CurrentCulture))
                {
                    ReadFile(strFile, nodes);
                }
            }

            string[] folders = Directory.GetDirectories(strFolderPath);

            if (folders == null)
                return;

            foreach (string strFolder in folders)
            {
                int nIndex = strFolder.LastIndexOf('\\');

                if (nIndex < 0)
                    continue;

                string strFolderName = strFolder.Substring(nIndex + 1);
                TreeNode node = FindTreeNode(strFolderName, nodes);

                if (node != null)
                    SearchFolder(strFolder, node.Nodes);
            }
        }

        private TreeNode FindTreeNode(string strFolderName, TreeNodeCollection nodes)
        {
            foreach (TreeNode node in nodes)
            {
                if (node.Tag != null && node.Tag is PageData)
                {
                    PageData data = (PageData)node.Tag;

                    if (data.ContainsChildFolder(strFolderName))
                        return node;
                }
            }

            return null;
        }

        private TreeNode FindTreeNode(string strURL, TreeNodeCollection nodes, bool isTitle)
        {
            foreach (TreeNode node in nodes)
            {
                if (node.Tag != null && node.Tag is PageData)
                {
                    PageData data = (PageData)node.Tag;

                    if (isTitle == data.IsPageTitle)
                    {
                        if (isTitle == false)
                        {
                            if (strURL == data.PageURL + "#" + data.LinkName)
                                return node;
                        }
                        else
                        {
                            if (strURL == data.PageURL)
                                return node;
                        }
                    }
                }

                TreeNode child = FindTreeNode(strURL, node.Nodes, isTitle);

                if (child != null)
                    return child;
            }

            return null;
        }

        private void ReadFile(string strFilePath, TreeNodeCollection nodes, string strPageURL = null)
        {
            if (strPageURL == null)
                strPageURL = strFilePath;

            try
            {
                StreamReader reader = new StreamReader(strFilePath, WebSearch.PageEncoding);
                //StreamReader reader = new StreamReader(strFilePath, Encoding.UTF8);
                HtmlParser parser = new HtmlParser();
                int nLineCount = 0;

                while (!reader.EndOfStream)
                {
                    nLineCount++;
                    string strLine = reader.ReadLine().Trim();

                    if (!parser.ReadLine(strLine))
                    {
                        reader.Close();
                        return;
                    }
                }

                reader.Close();

                AddTreeItem(nodes, parser, strPageURL);
            }
            catch (Exception)
            {
            }
        }

        private void AddNode(TreeNodeCollection nodes, TreeNode node, PageData data)
        {
            int nNodeCount = nodes.Count;

            for (int i = 0; i < nNodeCount; i++)
            {
                TreeNode _node = nodes[i];
                TreeNode _node2 = i == nNodeCount - 1 ? null : nodes[i + 1];

                if (_node.Tag != null && _node.Tag is PageData)
                {
                    PageData _data = (PageData)_node.Tag;
                    PageData _data2 = _node2 != null && _node2.Tag != null && _node2.Tag is PageData ? (PageData)_node2.Tag : null;

                    if (data.OrderIndex < _data.OrderIndex)
                    {
                        nodes.Insert(i, node);
                        return;
                    }

                    if (_data2 != null)
                    {
                        if (_data.OrderIndex <= data.OrderIndex && data.OrderIndex <= _data2.OrderIndex)
                        {
                            nodes.Insert(i + 1, node);
                            return;
                        }
                    }
                    else
                    {
                        if (_data.OrderIndex <= data.OrderIndex)
                        {
                            nodes.Insert(i + 1, node);
                            return;
                        }
                    }
                }
            }

            nodes.Add(node);
        }

        private void AddTreeItem(TreeNodeCollection nodes, HtmlParser parser, string strPageURL)
        {
            PageData data = parser.Calc();

            if (data == null)
                return;

            data.PageURL = strPageURL;

            TreeNode node = new TreeNode(data.TreeItem);
            node.Tag = data;
            node.ImageIndex = node.SelectedImageIndex = NO_CHILD;
            //nodes.Add(node);
            AddNode(nodes, node, data);

            if (node.Parent != null)
            {
                node.Parent.ImageIndex = node.Parent.SelectedImageIndex = COLLAPSED;
            }

            AddTreeItem(data, strPageURL, node);
        }

        private void AddTreeItem(PageData data, string strPageURL, TreeNode node)
        {
            foreach (PageData child in data.ChildDatas)
            {
                child.PageURL = strPageURL;

                TreeNode childNode = new TreeNode(child.TreeItem);
                childNode.Tag = child;
                childNode.ImageIndex = childNode.SelectedImageIndex = NO_CHILD;
                AddNode(node.Nodes, childNode, child);

                node.ImageIndex = node.SelectedImageIndex = COLLAPSED;

                AddTreeItem(child, strPageURL, childNode);
            }
        }

        private void treeIndex_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (m_systemCall)
                return;

            if (treeIndex.SelectedNode == null)
                return;

            if (treeIndex.SelectedNode.Tag != null && treeIndex.SelectedNode.Tag is PageData)
            {
                PageData data = (PageData)treeIndex.SelectedNode.Tag;

                if (data.IsPageTitle)
                    webViewer.Navigate(data.PageURL);
                else
                    webViewer.Navigate(data.PageURL + "#" + data.LinkName);
            }
        }

        private void treeIndex_AfterExpand(object sender, TreeViewEventArgs e)
        {
            e.Node.ImageIndex = e.Node.SelectedImageIndex = EXPANDED;
        }

        private void treeIndex_AfterCollapse(object sender, TreeViewEventArgs e)
        {
            e.Node.ImageIndex = e.Node.SelectedImageIndex = COLLAPSED;
        }

        private void treeIndex_MouseMove(object sender, MouseEventArgs e)
        {
            Point pt = treeIndex.PointToClient(Cursor.Position);
            TreeNode node = GetMouseOverImageNode(pt, treeIndex.Nodes);

            if (node != null)
            {
                if (m_currentMouseOverNode != null)
                {
                    if (m_currentMouseOverNode != node)
                    {
                        m_currentMouseOverNode.ImageIndex = m_currentMouseOverNode.SelectedImageIndex = NotOverImageIndex(m_currentMouseOverNode);
                    }

                    node.ImageIndex = node.SelectedImageIndex = OverImageIndex(node);
                }
                else
                {
                    node.ImageIndex = node.SelectedImageIndex = OverImageIndex(node);
                }
            }
            else
            {
                if (m_currentMouseOverNode != null)
                {
                    m_currentMouseOverNode.ImageIndex = m_currentMouseOverNode.SelectedImageIndex = NotOverImageIndex(m_currentMouseOverNode);
                }
            }

            m_currentMouseOverNode = node;
        }

        private int OverImageIndex(TreeNode node)
        {
            if (node.IsExpanded)
                return EXPANDED_OVER;

            return COLLAPSED_OVER;
        }

        private int NotOverImageIndex(TreeNode node)
        {
            if (node.IsExpanded)
                return EXPANDED;

            return COLLAPSED;
        }

        private void treeIndex_MouseLeave(object sender, EventArgs e)
        {
            if (m_currentMouseOverNode != null)
            {
                m_currentMouseOverNode.ImageIndex = m_currentMouseOverNode.SelectedImageIndex = NotOverImageIndex(m_currentMouseOverNode);
                m_currentMouseOverNode = null;
            }
        }

        private TreeNode GetMouseOverImageNode(Point pt, TreeNodeCollection nodes)
        {
            foreach (TreeNode node in nodes)
            {
                if ((node.Parent != null && node.Parent.IsExpanded == false) || node.ImageIndex == NO_CHILD)
                    //if (node.IsVisible == false || node.ImageIndex == NO_CHILD)
                    continue;

                int nLeft = node.Bounds.Left - m_nLeftSpace - m_nImageSize;
                int nRight = nLeft + m_nImageSize;

                if (pt.X >= nLeft && pt.X <= nRight &&
                    pt.Y >= node.Bounds.Top && pt.Y <= node.Bounds.Bottom)
                {
                    return node;
                }

                TreeNode child = GetMouseOverImageNode(pt, node.Nodes);

                if (child != null)
                    return child;
            }

            return null;
        }

        private void treeIndex_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                TreeNode node = GetMouseOverImageNode(e.Location, treeIndex.Nodes);

                if (node != null)
                {
                    if (node.IsExpanded)
                        node.Collapse();
                    else
                        node.Expand();
                }
            }
        }

        private void AddSearchResult(TreeNode node)
        {
            int nItemCount = panelSearchResult.Controls.Count;

            LinkLabel label = new LinkLabel();
            panelSearchResult.Controls.Add(label);

            label.Location = new Point(10, 10 + 30 * nItemCount);
            label.Text = node.Text;
            label.Tag = node;

            label.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.SearchResultLinkClicked);
        }

        private void SearchNodes(TreeNodeCollection nodes, string strTag)
        {
            foreach (TreeNode node in nodes)
            {
                if (node.Tag != null && node.Tag is PageData)
                {
                    PageData data = (PageData)node.Tag;

                    foreach (string tag in data.Tags)
                    {
                        if (string.Compare(tag, strTag, true) == 0)
                        {
                            AddSearchResult(node);
                            break;
                        }
                    }
                }

                SearchNodes(node.Nodes, strTag);
            }
        }*/
        #endregion 일반 TreeView

        private void treeViewAdv1_Collapsed(object sender, TreeViewAdvEventArgs e)
        {

        }

        private void treeViewAdv1_Expanded(object sender, TreeViewAdvEventArgs e)
        {

        }

        private void tabControlHeader_DrawItem(object sender, DrawItemEventArgs e)
        {

        } 
    }

    [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
    internal class PageSetupDialogListener : NativeWindow
    {
        const int WM_COMMAND = 0x0111;
        const int IDOK = 1;
        const int IDCANCEL = 2;

        private FormMain m_frmParent = null;

        public PageSetupDialogListener(IntPtr handle, FormMain frmParent)
        {
            m_frmParent = frmParent;
            AssignHandle(handle);
        }

        [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
        protected override void WndProc(ref Message m)
        {
            // Listen for operating system messages

            switch (m.Msg)
            {
                case WM_COMMAND:
                    if (m.WParam.ToInt32() == IDOK)
                    {
                        ReleaseHandle();
                        m_frmParent.OnPrintPageSetupOK();
                    }
                    else if (m.WParam.ToInt32() == IDCANCEL)
                    {
                        ReleaseHandle();
                        m_frmParent.OnPrintPageSetupCancel();
                    }
                    break;
            }
            base.WndProc(ref m);
        }
    }
}
