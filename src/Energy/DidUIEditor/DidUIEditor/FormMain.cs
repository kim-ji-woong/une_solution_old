using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using DidUIEditor.Popups;
using DidUIEditor.uCustomize;
using UnE.GUI;

namespace DidUIEditor
{
    public enum Mode { Normal = 0, Emergency }
    public partial class FormMain : Form
    {        
        private Mode m_Mode = Mode.Normal;
        public Mode Mode
        {
            get { return m_Mode; }
            set { m_Mode = value; }
        }

        private int m_nCurrentPage = 0; // 0 = 첫번째 페이지
        private List<Page> m_haveNormalPages = new List<Page>();
        public List<Page> HaveNormalPages
        {
            get { return m_haveNormalPages; }
            set { m_haveNormalPages = value; }
        }

        private List<Page> m_haveEmergencyPages = new List<Page>();
        public List<Page> HaveEmergencyPages
        {
            get { return m_haveEmergencyPages; }
            set { m_haveEmergencyPages = value; }
        }

        private List<TabPage> m_tabPages = new List<TabPage>();

        private List<uPanel> m_selectedPn = new List<uPanel>();
        public List<uPanel> SelectedPn
        {
            get { return m_selectedPn; }
            set { m_selectedPn = value; }
        }

        private WebServerManager m_webMgr = null;
        public WebServerManager WebMgr
        {
            get { return m_webMgr; }
            set { m_webMgr = value; }
        }

        private string m_strLocalFilePath = Application.StartupPath + "\\Files";

        private static FormMain m_instance = null;
        public static FormMain Instance
        {
            get { return m_instance; }
        }

        private Image m_imgNoMovie = null;
        private Image m_imgYesMovie = null;

        public FormMain()
        {
            InitializeComponent();

            base.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            base.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            base.SetStyle(ControlStyles.ResizeRedraw, true);

                       
            m_instance = this;
            m_webMgr = new WebServerManager();
            m_webMgr.LocalFilePath = m_strLocalFilePath;

            //this.Size = new Size(1010, 730);

            m_imgNoMovie = global::DidUIEditor.Properties.Resources.no_movie;
            m_imgYesMovie = global::DidUIEditor.Properties.Resources.yes_movie;

            //tabControl1.Appearance = TabAppearance.FlatButtons;
            //tabControl1.ItemSize = new Size(0, 1);
            //tabControl1.SizeMode = TabSizeMode.Fixed;
        }

        public void SetDoubleBuffer(Panel panel, bool bEnabled)
        {
            Type dgvType1 = panel.GetType();
            System.Reflection.PropertyInfo pi1 = dgvType1.GetProperty("DoubleBuffered", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            pi1.SetValue(panel, bEnabled, null);
        }
        public void SetDoubleBuffer(TabControl tab, bool bEnabled)
        {
            Type dgvType1 = tab.GetType();
            System.Reflection.PropertyInfo pi1 = dgvType1.GetProperty("DoubleBuffered", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            pi1.SetValue(tab, bEnabled, null);
        }
        public void SetDoubleBuffer(TabPage tab, bool bEnabled)
        {
            Type dgvType1 = tab.GetType();
            System.Reflection.PropertyInfo pi1 = dgvType1.GetProperty("DoubleBuffered", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            pi1.SetValue(tab, bEnabled, null);
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            this.DoubleBuffered = true;
            SetDoubleBuffer(tabControl1, true);
            SetDoubleBuffer(pnUI, true);

            // 서버에서 xml 다운로드
            bool bXML = m_webMgr.Download("DID_UI.xml");
            // 서버에서 기본으로 제공하는 system page 다운로드            
            m_webMgr.Download("did_update.txt");

            if (bXML)
            {
                XMLManager xmlMgr = new XMLManager();
                xmlMgr.LoadXML();
            }

            LoadPage();

            txtPageTime.TextChanged += textBox1_TextChanged;
        }
        //private List<ImageButton> pageButtons = new List<ImageButton>();
        private void LoadPage()
        {
            m_tabPages.Clear();
            tabControl1.TabPages.Clear();
            List<Page> pages = null;

            if (m_Mode == Mode.Normal)
                pages = m_haveNormalPages;

            if (m_Mode == Mode.Emergency)
                pages = m_haveEmergencyPages;

            if (pages.Count == 0)
                return;

            //foreach (ImageButton item in pageButtons)
            //{
            //    pnMain.Controls.Remove(item);
            //}

            //pageButtons.Clear();

            foreach (Page item in pages)
            {
                TabPage tabPage = new TabPage();                
                tabPage.Text = item.Name;
                tabPage.Tag = item;
                SetDoubleBuffer(tabPage, true);

                tabControl1.TabPages.Add(tabPage);
                
                m_tabPages.Add(tabPage);

                //AddTabpageButton(tabPage);
            }

            if (tabControl1.TabPages.Count > 0 && m_tabPages.Count > 0)
            {
                tabControl1_SelectedIndexChanged(null, null);

                //TabPageBtn_Click(pageButtons[tabControl1.SelectedIndex], null);

                //pageButtons[tabControl1.SelectedIndex].ImageNormal = global::DidUIEditor.Properties.Resources.tabPage_white;
                //pageButtons[tabControl1.SelectedIndex].ImageClicked = global::DidUIEditor.Properties.Resources.tabPage_white;
                //pageButtons[tabControl1.SelectedIndex].ImageMouseOver = global::DidUIEditor.Properties.Resources.tabPage_white;
                //pageButtons[tabControl1.SelectedIndex].Refresh();
            }
        }

        private ImageButton AddTabpageButton(TabPage tabPage)
        {
            ImageButton imgBtn = new ImageButton();
            imgBtn.ImageNormal = global::DidUIEditor.Properties.Resources.tabPage_blue;
            imgBtn.ImageClicked = global::DidUIEditor.Properties.Resources.tabPage_white;
            imgBtn.ImageMouseOver = global::DidUIEditor.Properties.Resources.tabPage_white;
            imgBtn.BackColor = Color.Transparent;
            imgBtn.ButtonText = tabPage.Text;
            imgBtn.Tag = tabPage;
            imgBtn.TextColor = Color.FromArgb(0x1f, 0x34, 0x48);
            imgBtn.TextFont = new System.Drawing.Font("나눔바른고딕 ExtraBold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            imgBtn.Size = new Size(70, 20);
            imgBtn.Location = new Point(
                tabControl1.Location.X + ((tabControl1.TabPages.Count - 1) * imgBtn.Width) + ((tabControl1.TabPages.Count - 1) * 5),
                tabControl1.Location.Y - imgBtn.Height);
            imgBtn.Click += TabPageBtn_Click;
            imgBtn.Parent = tabControl1.Parent;

            //pageButtons.Add(imgBtn);

            return imgBtn;
        }

        private void CreateUIArea()
        {
            pnUI.Controls.Clear();

            if (m_Mode == Mode.Normal && m_haveNormalPages.Count == 0)
                return;

            if (m_Mode == Mode.Emergency && m_haveEmergencyPages.Count == 0)
                return;
                        
            Page page = (m_Mode == Mode.Normal) ? m_haveNormalPages[m_nCurrentPage] : m_haveEmergencyPages[m_nCurrentPage];
            txtPageTime.Text = page.PlaySeconds.ToString();
            //if (page.PageType == PageType.System)
            //{
                CreatePagePanel(pnUI, page);
            //}
            //else if (page.PageType == PageType.User)
            //{

            //}

            pnUI.Parent = tabControl1.SelectedTab;
        }

        private void CreatePagePanel(Panel pnParent, Page page)
        {
            uPanel pn = new uPanel();
            pn.BackColor = Color.White;
            pn.BackgroundImageLayout = ImageLayout.Stretch;
            pn.Name = page.Name;
            pn.Page = page;
            pn.Tag = page;

            if (page.strBackgroundIMG != null && page.strBackgroundIMG.Length > 0)
            {
                pn.BackgroundImageLayout = ImageLayout.Stretch;

                if (page.BackgroundIMG != null)
                {
                    pn.BackgroundImage = page.BackgroundIMG;                    
                }
                else
                {
                    string filePath = MakeMediaFilePath(page.strBackgroundIMG);
                    if (File.Exists(filePath))
                    {
                        using (FileStream fs = new FileStream(filePath, FileMode.Open))
                        {
                            Image img = Image.FromStream(fs);  //Image.FromFile(filePath);
                            pn.BackgroundImage = img;
                        }
                    }
                }
            }

            if (pnParent == pnUI)
            {
                pn.Size = pnParent.Size;
                pn.Location = new Point(0, 0);
                pn.SetNoEvent();
            }
            else
            {
                pn.Size = page.PageSize;
                pn.Location = page.PageLocation;
                pn.BackColor = Color.LightGray;
            }

            if (page.PageType == PageType.User)
            {
                MenuItem item1 = new MenuItem("배경화면 지정");
                item1.Click += (ss, ee) =>
                {
                    OpenFileDialog dlg = new OpenFileDialog();
                    dlg.Filter = "Image|*.png;*.jpg";
                    if (dlg.ShowDialog() == DialogResult.OK)
                    {
                        string filePath = MakeMediaFilePath(dlg.SafeFileName);
                        File.Copy(dlg.FileName, filePath, true);

                        page.strBackgroundIMG = dlg.SafeFileName;                        
                        using (FileStream fs = new FileStream(filePath, FileMode.Open))
                        {
                            Image img = Image.FromStream(fs);  //Image.FromFile(filePath);
                            page.BackgroundIMG = img;
                            pn.BackgroundImage = img;
                        }
                        pn.BackColor = Color.Transparent;
                    }
                };
                MenuItem item3 = new MenuItem("크기, 위치 지정");
                item3.Click += (ss, ee) =>
                {
                    PopupBack back = new PopupBack();
                    back.StartPosition = FormStartPosition.Manual;
                    back.Size = this.Size;
                    back.Location = this.Location;
                    back.Show();

                    FormPanelSetting frm = new FormPanelSetting(pn.Page.PageSize, pn.Page.PageLocation);
                    frm.StartPosition = FormStartPosition.CenterParent;
                    if (frm.ShowDialog() == DialogResult.Yes)
                    {
                        Size parentSize = pn.Parent.Size;
                        Point parentLocation = pn.Parent.Location;
                        if (frm.SetLocation.X > parentSize.Width || frm.SetLocation.Y > parentSize.Height)
                        {
                            MessageBox.Show("화면에서 벗어납니다.");
                            back.Close();
                            return;
                        }

                        pn.Page.PageLocation = pn.Location = frm.SetLocation;
                        pn.Page.PageSize = pn.Size = frm.SetSize;

                        MessageBox.Show("변경되었습니다.");
                    }
                    back.Close();
                };

                ContextMenu menu = new ContextMenu();
                menu.MenuItems.Add(item1);
                menu.MenuItems.Add(item3);
                pn.ContextMenu = menu;
            }

            foreach (Page child in page.ChildPages)
            {
                CreatePagePanel(pn, child);                
            }

            foreach (Media media in page.Medias)
            {
                uPanel pnMedia = CreateMediaPanel(media);
                pnMedia.Parent = pn;
            }

            pn.AddBtnSizable();
            pn.Parent = pnParent;
            SetDoubleBuffer(pn, true);
        }

        private uPanel CreateMediaPanel(Media media)
        {
            uPanel pnMedia = new uPanel();
            pnMedia.Page.PageType = PageType.None;
            pnMedia.Page.Medias.Add(media);
            pnMedia.Size = media.MediaSize;
            pnMedia.OrgSize = media.MediaSize;
            pnMedia.Location = media.MediaLocation;
            pnMedia.BackgroundImageLayout = ImageLayout.Stretch;
            pnMedia.BackColor = Color.Gray;

            string filePath = MakeMediaFilePath(media.File);

            if (media.MediaType == MediaType.Image)
            {                
                if (File.Exists(filePath))
                {
                    using (FileStream fs = new FileStream(filePath, FileMode.Open))
                    {                        
                        Image img = Image.FromStream(fs);  //Image.FromFile(filePath);
                        pnMedia.BackgroundImage = img;
                    }
                }
                else
                {
                    //pnMedia.BackgroundImage = global::DidUIEditor.Properties.Resources.noContent;
                }

                MenuItem item1 = new MenuItem("이미지 지정");
                item1.Click += (ss, ee) =>
                {
                    OpenFileDialog dlg = new OpenFileDialog();
                    dlg.Filter = "Image|*.png;*.jpg";
                    if (dlg.ShowDialog() == DialogResult.OK)
                    {
                        string filePath2 = MakeMediaFilePath(dlg.SafeFileName);
                        File.Copy(dlg.FileName, filePath2, true);

                        media.File = dlg.SafeFileName;
                        using (FileStream fs = new FileStream(dlg.FileName, FileMode.Open))
                        {
                            Image img = Image.FromStream(fs);  //Image.FromFile(filePath);
                            pnMedia.BackgroundImage = img;
                            pnMedia.BackColor = Color.Transparent;
                        }                        
                    }
                };

                MenuItem item3 = new MenuItem("크기, 위치 지정");
                item3.Click += (ss, ee) =>
                {
                    PopupBack back = new PopupBack();
                    back.StartPosition = FormStartPosition.Manual;
                    back.Size = this.Size;
                    back.Location = this.Location;
                    back.Show();

                    FormPanelSetting frm = new FormPanelSetting(media.MediaSize, media.MediaLocation);
                    frm.StartPosition = FormStartPosition.CenterParent;
                    if (frm.ShowDialog() == DialogResult.Yes)
                    {
                        Size parentSize = pnMedia.Parent.Size;
                        Point parentLocation = pnMedia.Parent.Location;
                        if (frm.SetLocation.X > parentSize.Width || frm.SetLocation.Y > parentSize.Height)
                        {
                            MessageBox.Show("화면에서 벗어납니다.");
                            back.Close();
                            return;
                        }

                        pnMedia.Page.Medias[0].MediaLocation = pnMedia.Location = frm.SetLocation;
                        pnMedia.Page.Medias[0].MediaSize = pnMedia.Size = frm.SetSize;

                        MessageBox.Show("변경되었습니다.");
                    }
                    back.Close();
                };

                MenuItem item4 = new MenuItem("삽입된 이미지 제거");
                item4.Click += (ss, ee) =>
                {                 
                    media.File = "";
                    pnMedia.BackgroundImage = null;
                    pnMedia.BackColor = Color.Gray;
                };

                ContextMenu menu = new ContextMenu();
                menu.MenuItems.Add(item1);
                menu.MenuItems.Add(item4);
                menu.MenuItems.Add(item3);
                pnMedia.ContextMenu = menu;
            }
            else if (media.MediaType == MediaType.Movie)
            {
                pnMedia.BackgroundImageLayout = ImageLayout.Stretch;
                if (File.Exists(filePath))
                {
                    pnMedia.BackgroundImage = m_imgYesMovie;
                    //string defaultMovieImg = MakeMediaFilePath("yes_movie.png");
                    //using (FileStream fs = new FileStream(defaultMovieImg, FileMode.Open))
                    //{
                    //    Image img = Image.FromStream(fs);  //Image.FromFile(filePath);
                    //    pnMedia.BackgroundImage = img;
                    //    pnMedia.BackgroundImageLayout = ImageLayout.Stretch;
                    //}
                }
                else
                {
                    pnMedia.BackgroundImage = m_imgNoMovie;                    
                    //string defaultMovieImg = MakeMediaFilePath("no_movie.png");
                    //using (FileStream fs = new FileStream(defaultMovieImg, FileMode.Open))
                    //{
                    //    Image img = Image.FromStream(fs);  //Image.FromFile(filePath);
                    //    pnMedia.BackgroundImage = img;
                    //    pnMedia.BackgroundImageLayout = ImageLayout.Stretch;
                    //}
                }

                MenuItem item1 = new MenuItem("동영상 지정");
                item1.Click += (ss, ee) =>
                {
                    OpenFileDialog dlg = new OpenFileDialog();
                    dlg.Filter = "Movie|*.mp4;*.avi;*.wmv";
                    if (dlg.ShowDialog() == DialogResult.OK)
                    {
                        this.Cursor = Cursors.WaitCursor;
                        
                        string filePath2 = MakeMediaFilePath(dlg.SafeFileName);
                        File.Copy(dlg.FileName, filePath2, true);

                        media.File = dlg.SafeFileName;

                        pnMedia.BackgroundImage = m_imgYesMovie;
                        //string defaultMovieImg = MakeMediaFilePath("yes_movie.png");
                        //using (FileStream fs = new FileStream(defaultMovieImg, FileMode.Open))
                        //{
                        //    Image img = Image.FromStream(fs);  //Image.FromFile(filePath);
                        //    pnMedia.BackgroundImage = img;
                        //    pnMedia.BackgroundImageLayout = ImageLayout.Stretch;                            
                        //}

                        this.Cursor = Cursors.Default;
                    }
                };

                //MenuItem item2 = new MenuItem("재생시간 지정");
                //item2.Click += (ss, ee) =>
                //{
                //    PopupBack back = new PopupBack();
                //    back.StartPosition = FormStartPosition.Manual;
                //    back.Size = this.Size;
                //    back.Location = this.Location;
                //    back.Show();

                //    FormMoviePropertySetting frm = new FormMoviePropertySetting(media);
                //    frm.StartPosition = FormStartPosition.CenterParent;
                //    frm.ShowDialog();
                //    back.Close();
                //};

                MenuItem item3 = new MenuItem("크기, 위치 지정");
                item3.Click += (ss, ee) =>
                {
                    PopupBack back = new PopupBack();
                    back.StartPosition = FormStartPosition.Manual;
                    back.Size = this.Size;
                    back.Location = this.Location;
                    back.Show();

                    FormPanelSetting frm = new FormPanelSetting(media.MediaSize, media.MediaLocation);
                    frm.StartPosition = FormStartPosition.CenterParent;
                    if (frm.ShowDialog() == DialogResult.Yes)
                    {
                        Size parentSize = pnMedia.Parent.Size;
                        Point parentLocation = pnMedia.Parent.Location;
                        if (frm.SetLocation.X > parentSize.Width || frm.SetLocation.Y > parentSize.Height)
                        {
                            MessageBox.Show("화면에서 벗어납니다.");
                            back.Close();
                            return;
                        }

                        pnMedia.Page.Medias[0].MediaLocation = pnMedia.Location = frm.SetLocation;
                        pnMedia.Page.Medias[0].MediaSize = pnMedia.Size = frm.SetSize;

                        MessageBox.Show("변경되었습니다.");
                    }
                    back.Close();
                };

                MenuItem item4 = new MenuItem("삽입된 동영상 제거");
                item4.Click += (ss, ee) =>
                {
                    media.File = "";
                    pnMedia.BackgroundImage = m_imgNoMovie;
                    pnMedia.BackColor = Color.Gray;
                };

                ContextMenu menu = new ContextMenu();
                menu.MenuItems.Add(item1);
                menu.MenuItems.Add(item4);
                //menu.MenuItems.Add(item2);
                menu.MenuItems.Add(item3);
                pnMedia.ContextMenu = menu;
            }

            pnMedia.AddBtnSizable();            
            SetDoubleBuffer(pnMedia, true);

            return pnMedia;
        }
        
        public void SetChangeSelectionPanel(uPanel selectPanel)
        {
            if (!m_pressCtrlKey)
            {
                if (m_selectedPn != null && m_selectedPn.Count > 0)
                {
                    foreach (uPanel item in m_selectedPn)
                    {
                        item.SetVisible(false);
                    }
                    m_selectedPn.Clear();
                }
            }

            m_selectedPn.Add(selectPanel);
            foreach (uPanel item in m_selectedPn)
            {
                item.SetVisible(true);
            }
        }

        private void TabPageBtn_Click(object sender, EventArgs e)
        {
            if (tabControl1.TabPages.Count == 0)
                return;

            ImageButton btn = sender as ImageButton;
            if (btn == null || btn.Tag == null || (btn.Tag is TabPage) == false)
                return;
            
            //pageButtons[tabControl1.SelectedIndex].ImageNormal = global::DidUIEditor.Properties.Resources.tabPage_blue;
            //pageButtons[tabControl1.SelectedIndex].ImageClicked = global::DidUIEditor.Properties.Resources.tabPage_white;
            //pageButtons[tabControl1.SelectedIndex].ImageMouseOver = global::DidUIEditor.Properties.Resources.tabPage_white;
            //pageButtons[tabControl1.SelectedIndex].Refresh();

            tabControl1.SelectedTab = (TabPage)btn.Tag;

            //for (int i = 0; i < pageButtons.Count; i++)
            //{
            //    if (btn == pageButtons[i])
            //    {
            //        m_nCurrentPage = i;
            //        break;
            //    }
            //}
            m_nCurrentPage = tabControl1.SelectedIndex;

            if (tabControl1.SelectedIndex < 0)
                return;

            //pageButtons[tabControl1.SelectedIndex].ImageNormal = global::DidUIEditor.Properties.Resources.tabPage_white;
            //pageButtons[tabControl1.SelectedIndex].ImageClicked = global::DidUIEditor.Properties.Resources.tabPage_white;
            //pageButtons[tabControl1.SelectedIndex].ImageMouseOver = global::DidUIEditor.Properties.Resources.tabPage_white;
            //pageButtons[tabControl1.SelectedIndex].Refresh();

            CreateUIArea();
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabControl1.TabPages.Count == 0)
                return;

            m_nCurrentPage = tabControl1.SelectedIndex;
            CreateUIArea();
        }

        private bool m_pressCtrlKey = false;
        private void FormMain_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.ControlKey)
            {
                m_pressCtrlKey = true;
            }
        }

        private void FormMain_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Delete && e.KeyCode != Keys.ControlKey)
                return;

            if (e.KeyCode == Keys.ControlKey)
            {
                m_pressCtrlKey = false;
                return;
            }

            if (m_selectedPn == null || m_selectedPn.Count == 0)
                return;

            Page page = null;
            if (m_Mode == Mode.Normal)
                page = m_haveNormalPages[tabControl1.SelectedIndex];
            else if (m_Mode == Mode.Emergency)
                page = m_haveEmergencyPages[tabControl1.SelectedIndex];

            if (page == null)
                return;

            foreach (uPanel item in m_selectedPn)
            {
                // Media만 지울 수 있음 (None = Media)
                if (item.Page.PageType != PageType.None)
                    return;

                if (DeletePage(page, item))
                    DeletePanel(pnUI, item); 
            }

            m_selectedPn.Clear();
        }

        private bool DeletePage(Page parentCtrl, uPanel selectedPn)
        {
            // Media
            if (selectedPn.Page.PageType == PageType.None)
            {
                foreach (Media media in parentCtrl.Medias)
                {
                    foreach (Media media2 in selectedPn.Page.Medias)
                    {
                        if (media == media2)
                        {
                            parentCtrl.Medias.Remove(media);
                            return true;
                        }
                    }
                }
            }

            foreach (Page page in parentCtrl.ChildPages)
            {
                if (page == selectedPn.Page)
                {
                    parentCtrl.ChildPages.Remove(page);
                    return true;
                }
                else
                {
                    bool find = DeletePage(page, selectedPn);
                    if (find)
                        return true;
                }

            }

            return false;
        }

        private bool DeletePanel(Control parentCtrl, uPanel selectedPn)
        {
            foreach (Control ctrl in parentCtrl.Controls)
            {
                if ((ctrl is uPanel) == false)
                    continue;

                if (ctrl == selectedPn)
                {
                    parentCtrl.Controls.Remove(ctrl);
                    //m_selectedPn = null;
                    return true;
                }
                else
                {
                    bool find = DeletePanel(ctrl, selectedPn);
                    if (find)
                        return true;
                }
                
            }

            return false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            PopupBack back = new PopupBack();
            back.StartPosition = FormStartPosition.Manual;
            back.Size = this.Size;
            back.Location = this.Location;
            back.Show();

            FormPageSetting frm = new FormPageSetting();
            frm.StartPosition = FormStartPosition.CenterParent;
            if (frm.ShowDialog() == DialogResult.Yes)
            {
                LoadPage();
            }
            back.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            PopupBack back = new PopupBack();
            back.StartPosition = FormStartPosition.Manual;
            back.Size = this.Size;
            back.Location = this.Location;
            back.Show();

            FormNewPage frm = new FormNewPage();
            frm.StartPosition = FormStartPosition.CenterParent;
            if (frm.ShowDialog() != DialogResult.Yes)
            {
                back.Close();
                return;
            }

            if (frm.ReturnPages == null || frm.ReturnPages.Count == 0)
            {
                back.Close();
                return;
            }

            if (tabControl1.SelectedIndex >= 0)
            {
                //pageButtons[tabControl1.SelectedIndex].ImageNormal = global::DidUIEditor.Properties.Resources.tabPage_blue;
                //pageButtons[tabControl1.SelectedIndex].ImageClicked = global::DidUIEditor.Properties.Resources.tabPage_white;
                //pageButtons[tabControl1.SelectedIndex].ImageMouseOver = global::DidUIEditor.Properties.Resources.tabPage_white;
                //pageButtons[tabControl1.SelectedIndex].Refresh(); 
            }

            ImageButton btn = null;

            foreach (Page item in frm.ReturnPages)
            {
                item.Name = (tabControl1.TabPages.Count + 1).ToString();

                if (m_Mode == Mode.Normal)
                    m_haveNormalPages.Add(item);
                else if (m_Mode == Mode.Emergency)
                    m_haveEmergencyPages.Add(item);

                TabPage tabPage = new TabPage();
                tabPage.Text = item.Name;
                tabPage.Tag = item;
                SetDoubleBuffer(tabPage, true);                

                tabControl1.TabPages.Add(tabPage);
                m_tabPages.Add(tabPage);

                tabControl1.SelectedTab = tabPage;

                //btn = AddTabpageButton(tabPage);
            }
            
            //TabPageBtn_Click(btn, null);
            
            //pageButtons[tabControl1.SelectedIndex].ImageNormal = global::DidUIEditor.Properties.Resources.tabPage_white;
            //pageButtons[tabControl1.SelectedIndex].ImageClicked = global::DidUIEditor.Properties.Resources.tabPage_white;
            //pageButtons[tabControl1.SelectedIndex].ImageMouseOver = global::DidUIEditor.Properties.Resources.tabPage_white;
            //pageButtons[tabControl1.SelectedIndex].Refresh();

            back.Close();
        }
        
        private void btnSave_Click(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;
            try
            {
                XMLManager xmlMgr = new XMLManager();
                bool suc = xmlMgr.Save();
                if (suc)
                {
                    m_webMgr.Upload(MakeMediaFilePath("DID_UI.xml"));
                    SetVersion();
                    this.Cursor = Cursors.Default;
                    MessageBox.Show("저장되었습니다.");
                    return;
                }
            }
            catch (Exception ex)
            {
                this.Cursor = Cursors.Default;
                MessageBox.Show(ex.Message);
            }
        }

        private void SetVersion()
        {
            string version = "";

            m_webMgr.Download("did_update.txt");

            string txtUpdate = MakeMediaFilePath("did_update.txt");
            if (!File.Exists(txtUpdate))
                File.Create(txtUpdate);

            using (StreamReader sr = new StreamReader(txtUpdate))
            {
                version = sr.ReadLine();
            }

            int nVersion = 0;
            int.TryParse(version, out nVersion);
            nVersion++;

            using (StreamWriter sw = new StreamWriter(txtUpdate, false))
            {
                sw.Write(nVersion);
            }

            m_webMgr.Upload(txtUpdate);
        }

        public string MakeMediaFilePath(string fileName)
        {
            if (!Directory.Exists(m_strLocalFilePath))
                Directory.CreateDirectory(m_strLocalFilePath);

            return m_strLocalFilePath + "\\" + fileName;
        }

        private void FormMain_FormClosed(object sender, FormClosedEventArgs e)
        {
            foreach (string item in Directory.GetFiles(m_strLocalFilePath))
            {
                File.Delete(item);
            }
        }

        private void btnLeftSort_Click(object sender, EventArgs e)
        {
            if (m_selectedPn == null || m_selectedPn.Count <= 1)
                return;

            uPanel standardPanel = m_selectedPn[0] as uPanel;
            int x = 0;
            if (standardPanel.Page.PageType == PageType.None)
                x = standardPanel.Page.Medias[0].MediaLocation.X;
            else
                x = standardPanel.Page.PageLocation.X;

            for (int i = 1; i < m_selectedPn.Count; i++)
            {
                int y = 0;
                if (standardPanel.Page.PageType == PageType.None)
                {
                    y = m_selectedPn[i].Page.Medias[0].MediaLocation.Y;
                    m_selectedPn[i].Page.Medias[0].MediaLocation = new Point(x, y);
                }
                else
                {
                    y = m_selectedPn[i].Page.PageLocation.Y;
                    m_selectedPn[i].Page.PageLocation = new Point(x, y);
                }
                
                Control fine = FinePanel(pnUI, m_selectedPn[i]);
                if (fine != null)
                    fine.Location = new Point(x, y);
            }
        }

        private void btnTopSort_Click(object sender, EventArgs e)
        {
            if (m_selectedPn == null || m_selectedPn.Count <= 1)
                return;

            uPanel standardPanel = m_selectedPn[0] as uPanel;
            int y = 0;
            if (standardPanel.Page.PageType == PageType.None)
                y = standardPanel.Page.Medias[0].MediaLocation.Y;
            else
                y = standardPanel.Page.PageLocation.Y;

            for (int i = 1; i < m_selectedPn.Count; i++)
            {
                int x = 0;
                if (standardPanel.Page.PageType == PageType.None)
                {
                    x = m_selectedPn[i].Page.Medias[0].MediaLocation.X;
                    m_selectedPn[i].Page.Medias[0].MediaLocation = new Point(x, y);
                }
                else
                {
                    x = m_selectedPn[i].Page.PageLocation.X;
                    m_selectedPn[i].Page.PageLocation = new Point(x, y);
                }

                Control fine = FinePanel(pnUI, m_selectedPn[i]);
                if (fine != null)
                    fine.Location = new Point(x, y);
            }
        }

        private Control FinePanel(Control parentCtrl, uPanel selectedPn)
        {
            Control returnPanel = null;

            foreach (Control ctrl in parentCtrl.Controls)
            {
                if ((ctrl is uPanel) == false)
                    continue;
                
                if (ctrl == selectedPn)
                {
                    returnPanel = (Control)ctrl;
                    break;
                }
                else
                {
                    returnPanel = FinePanel(ctrl, selectedPn);
                    if (returnPanel != null)
                        break;
                }
            }

            return returnPanel;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            PopupBack back = new PopupBack(); 
            back.Size = this.Size;
            //back.Location = new Point(0, 0);
            back.BringToFront();
            back.Show();
            //Form1 fm = new Form1();
            //fm.StartPosition = FormStartPosition.CenterParent;
            //if (fm.ShowDialog() == DialogResult.OK)
            //{

            //}
            back.Close();


        }

        private void btnAddMedia_Click(object sender, EventArgs e)
        {
            OpenFileDialog dia = new OpenFileDialog();
            dia.Multiselect = true;
            dia.Filter = "Media|*.png;*.jpg;*.mp4;*.avi;*.wmv";

            if (dia.ShowDialog() == DialogResult.OK)
            {
                for (int i = 0; i < dia.FileNames.Length; i++)
                {
                    string fileName = dia.FileNames[i];
                    string safeFileName = dia.SafeFileNames[i];

                    string filePath = MakeMediaFilePath(safeFileName);
                    File.Copy(fileName, filePath, true);

                    string extension = Path.GetExtension(fileName).ToLower();

                    Media media = new Media();
                    if (extension == ".png" || extension == ".jpg")
                        media.MediaType = MediaType.Image;
                    else if (extension == ".mp4" || extension == ".wmv")
                        media.MediaType = MediaType.Movie;

                    media.MediaSize = new Size(150, 150);
                    media.MediaLocation = new Point(0, 0);

                    if (dia.FileNames.Length == 1)
                    {
                        if (File.Exists(filePath))
                        {
                            if (media.MediaType == MediaType.Image)
                            {
                                using (FileStream fs = new FileStream(filePath, FileMode.Open))
                                {
                                    Image img = Image.FromStream(fs);
                                    media.MediaSize = new Size(img.Size.Width / 2, img.Size.Height / 2);
                                    media.MediaLocation = new Point((960 / 2) - (media.MediaSize.Width / 2), (540 / 2) - (media.MediaSize.Height / 2));
                                }
                            }
                            else
                            {
                                //FileStream fs = new FileStream(fileName, FileMode.Open);
                                //FileInfo fi = new FileInfo(fileName);
                                
                                //media.MediaSize = new Size();
                                //dia.
                            }
                        }
                    }

                    media.File = safeFileName;

                    uPanel pnMedia = CreateMediaPanel(media);
                    pnMedia.Parent = pnUI;
                    //pnMedia.AddBtnSizable();
                    pnMedia.BringToFront();

                    if (media.MediaType == MediaType.Movie)
                    {
                        pnMedia.BackgroundImage = m_imgYesMovie;
                    }

                    Page page = m_haveNormalPages[m_nCurrentPage];
                    page.Medias.Add(media);
                }
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void btnNormal_Click(object sender, EventArgs e)
        {
            if (m_Mode == Mode.Normal)
                return;

            btnNormal.ImageNormal = global::DidUIEditor.Properties.Resources.btnNormal_Click;
            btnEmergency.ImageNormal = global::DidUIEditor.Properties.Resources.btnEmergency_Default;

            btnEmergency.Refresh();
            
            m_Mode = Mode.Normal;

            LoadPage();
        }

        private void btnEmergency_Click(object sender, EventArgs e)
        {
            if (m_Mode == Mode.Emergency)
                return;

            m_Mode = Mode.Emergency;

            btnNormal.ImageNormal = global::DidUIEditor.Properties.Resources.btnNormal_Default;
            btnEmergency.ImageNormal = global::DidUIEditor.Properties.Resources.btnEmergency_Click;

            btnNormal.Refresh();

            LoadPage();
        }

        #region Form 이동
        private bool m_bLeftMouseDown = false;
        private Point m_ptMove = new Point();
        private bool m_isClicked = false;
        private Point m_ptOrigin = new Point();
        #endregion

        private void pnTop_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                m_bLeftMouseDown = true;
                m_ptMove = Control.MousePosition;
                m_ptOrigin = this.Location;
            }

            m_isClicked = true;
        }

        private void pnTop_MouseMove(object sender, MouseEventArgs e)
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

        private void pnTop_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
                m_bLeftMouseDown = false;

            m_isClicked = false;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (tabControl1.SelectedIndex < 0)
                return;

            Page page = null;
            if (m_Mode == Mode.Normal)
                page = m_haveNormalPages[tabControl1.SelectedIndex];
            else if (m_Mode == Mode.Emergency)
                page = m_haveEmergencyPages[tabControl1.SelectedIndex];

            int newSec = -1;
            if (!int.TryParse(txtPageTime.Text, out newSec))
            {
                txtPageTime.TextChanged -= textBox1_TextChanged;
                MessageBox.Show("숫자만 입력하세요.");
                txtPageTime.Text = page.PlaySeconds.ToString();
                txtPageTime.TextChanged += textBox1_TextChanged;
                return;
            }
            else
            {
                page.PlaySeconds = newSec;
            }
        }
    }
}
