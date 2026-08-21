using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.OleDb;
using XtremePropertyGrid;
using XtremeDockingPane;

namespace section
{
    public partial class PageHome : Form
    {
        private FormMain m_MainForm = null;

        private FormDockingDataGrid m_DataGridForm = null;
        private FormDockingProperty m_Propertyform = null;
        public FormDockingProperty PropertyForm
        {
            get { return m_Propertyform; }
            set { m_Propertyform = value; }
        }

        private static PageHome m_home;
        public static PageHome Instance
        {
            get { return m_home; }
        }

        public ContextMenuStrip ContextMenu
        {
            get { return m_MainForm.MainContextMenuStrip; }
        }


        public PageHome(FormMain main)
        {
            m_home = this;
            m_MainForm = main;
            m_DataGridForm = new FormDockingDataGrid();
            m_Propertyform = new FormDockingProperty(this);

            InitializeComponent();

            CreateDockingPane();

            AddTabPage();
        }

        public void AddTabPage()
        {
            SectionTabPage tabPage1 = new SectionTabPage();
            tabPage1.Text = "새 드로잉";
            m_TabControl.Controls.Add(tabPage1);
        }

        private void CreateDockingPane()
        {

            Pane paneA = axDockingPane1.CreatePane(1, 200, 300, DockingDirection.DockLeftOf, null);
            paneA.Title = "PropertyGrid";
            paneA.Options = PaneOptions.PaneNoCloseable;

            Pane paneB = axDockingPane1.CreatePane(2, 500, 200, DockingDirection.DockBottomOf, null);
            paneB.Title = "DataGrid";
            paneB.Options = PaneOptions.PaneNoCloseable;

        }


        public void RedrawPanel()
        {
            TabPage page = m_TabControl.SelectedTab;
            if (page != null)
            {
                page.Invalidate();
            }
        }
               

        public void Add_Click(object sender, EventArgs e)
        {
            TabPage page = m_TabControl.SelectedTab;
            if (page != null)
            {
                ((SectionTabPage)page).AddSection(sender, e);
                Refresh();
            }
        }
         
        //저장
        public void SaveToDB()
        {
            SectionTabPage page = (SectionTabPage)m_TabControl.SelectedTab;
            if (page != null)
            {
                DBConn conn = new DBConn();
                conn.Open();

                //이전 데이터 삭제
                string sql = "delete * from Rectangle";
                conn.ExecuteSQL(sql);

                //새로운 데이터 집어넣음
                int r_num = 1;
                foreach (SectionTree n in page.SectionTreeList)
                {
                    int nParentID = -1;
                    if (n.Parent == null)
                        nParentID = -1;
                    else
                        nParentID = n.Parent.ID;

                    sql = "insert into Rectangle(r_num,p_num,r_content,r_width,r_height,r_x,r_y) values('" + r_num + "','" + nParentID + "', '"
                        + n.textBox1.Text + "','" + n.Rect.Width + "', '" + n.Rect.Height + "','" + n.Rect.X + "', '" + n.Rect.Y + "')";

                    n.ID = r_num;

                    conn.ExecuteSQL(sql);

                    ++r_num;
                }
                conn.Close();
                MessageBox.Show("Save OK");

                m_DataGridForm.UpdateData();
            }           
        }

        

        //불러오기
        public void LoadFromDB()
        {
            DBConn conn = new DBConn();
            conn.Open();
            string sql = "Select * from Rectangle";
            OleDbDataReader reader = conn.ExecuteReader(sql);

            SectionTabPage page = (SectionTabPage)m_TabControl.SelectedTab;
            if (page != null)
            {
                page.Controls.Clear();
                page.SectionTreeList.Clear();

                while (reader.Read())
                {
                    int rNum = int.Parse(reader[0].ToString());
                    int pNum = int.Parse(reader[1].ToString());
                    string rContent = reader[2].ToString();
                    int rWidth = int.Parse(reader[3].ToString());
                    int rHeight = int.Parse(reader[4].ToString());
                    int rx = int.Parse(reader[5].ToString());
                    int ry = int.Parse(reader[6].ToString());


                    SectionTree m_Load = new SectionTree(page);
                    m_Load.ID = rNum;
                    m_Load.SetLocation(rx, ry, rWidth, rHeight);

                    if (pNum == -1)
                        page.TreeRoot = m_Load;

                    m_Load.Parent = page.FindParent(pNum);

                    if (m_Load.Parent != null)
                        m_Load.Parent.AddChild(m_Load);

                    page.SectionTreeList.Add(m_Load);
                    m_Load.textBox1.Text = rContent;
                }
            }

           
            reader.Close();
            conn.Close();

            Refresh();

            MessageBox.Show("Load");
        }



        private void DockingPaneManager_ResizeEvent(object sender, EventArgs e)
        {
            int left, top, right, bottom;
            axDockingPane1.GetClientRect(out left, out top, out right, out bottom);
            m_TabControl.SetBounds(left, top, right - left, bottom - top);
        }

        private void DockingPane_AttachPaneEvent(object sender, AxXtremeDockingPane._DDockingPaneEvents_AttachPaneEvent e)
        {
            if (e.item.Id == 1)
                e.item.Handle = m_Propertyform.Handle.ToInt32();
            if (e.item.Id == 2)
                e.item.Handle = m_DataGridForm.Handle.ToInt32();
        }

        private void PageHome_Load(object sender, EventArgs e)
        {

        }

        private void PageHome_Resize(object sender, EventArgs e)
        {

        }
           
    

    }
}
