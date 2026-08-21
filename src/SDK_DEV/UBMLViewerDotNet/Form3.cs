using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Core;
using System.Diagnostics;
using System.IO;


namespace UBMLViewerDotNet
{
    public partial class Form3 : Form
    {
        private Core.Engine mEngine = new Core.Engine();
        private ArrayList mViewList = new ArrayList();
        private BaseView mCurrent = null;
        private string szIconPath = "";
        private string szMediaPath = "";
        public Form3()
        {
            szMediaPath = Application.StartupPath + "\\Media";
            szIconPath = szMediaPath + "\\icons\\화재.ico";
            //this.panel1 = new System.Windows.Forms.PictureBox();
            //this.panel2 = new System.Windows.Forms.PictureBox();

            panel1 = new BaseView();
            panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            panel1.BackColor = System.Drawing.Color.Transparent;
            panel1.Location = new System.Drawing.Point(0, 0);
            panel1.Name = "panel1";
            panel1.Size = new System.Drawing.Size(961, 579);
            panel1.TabIndex = 17;
            Controls.Add(this.panel1);

            InitializeComponent();
            
            //DoubleBuffered = true;
            //panel1.BackColor = Color.Transparent;
           // panel2.BackColor = Color.Transparent;

            this.MouseWheel += new MouseEventHandler(OnMouseWheel);
        }

        public static string EnginPath()
        {
            string szMainPath = Path.GetDirectoryName(Application.ExecutablePath) + "\\";
            string szWorkPath = szMainPath;
            if (File.Exists(szWorkPath + "Core.dll"))
                return szWorkPath;

            szWorkPath = szMainPath + "common\\";
            if (File.Exists(szWorkPath + "Core.dll"))
                return szWorkPath;

            szWorkPath = szMainPath + "SOP\\";
            if (File.Exists(szWorkPath + "Core.dll"))
                return szWorkPath;

            return szMainPath;
        }
        private void Form1_Load(object sender, EventArgs e)
        { 
            mEngine.Init(EnginPath(), "DDDD");           
           
            mViewList.Add(panel1);
            mCurrent = (BaseView)panel1;
            
            try
            {
                ((BaseView)panel1).Popup = popupMenu;
                ((BaseView)panel1).InitBaseView();                
            }
            catch (System.Exception ex1)
            {
                Debug.WriteLine(ex1.StackTrace);
            }
            mCurrent.SetCheckPoistion(true);
    
        }

        private void Form3_FormClosing(object sender, FormClosingEventArgs e)
        {
        }

        private void panel1_Click(object sender, EventArgs e)
        {
            mCurrent = ((BaseView)panel1);
        }

        private void panel2_Click(object sender, EventArgs e)
        {
        }

        private void btnTop_Click(object sender, EventArgs e)
        {
            if (mCurrent != null)
                mCurrent.OnViewTop();
        }

        private void btnFront_Click(object sender, EventArgs e)
        {
            if (mCurrent != null)
                mCurrent.OnViewFront();
        }

        private void btnLeft_Click(object sender, EventArgs e)
        {
            if (mCurrent != null)
                mCurrent.OnViewLeft();
        }

        private void btnRight_Click(object sender, EventArgs e)
        {
            if (mCurrent != null)
                mCurrent.OnViewRight();
        }

        private void btnHome_Click(object sender, EventArgs e)
        {
            if (mCurrent != null)
                mCurrent.OnViewRear();
        }
        private void btnHome_Click_1(object sender, EventArgs e)
        {
            if (mCurrent != null)
                mCurrent.OnViewHome();
        }
        private void btnZoomIn_Click(object sender, EventArgs e)
        {

        }

        private void btnFit_Click(object sender, EventArgs e)
        {
            if (mCurrent != null)
                mCurrent.OnViewFit();
        }

        private void btnImportView1_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                UseWaitCursor = true;
                ((BaseView)panel1).OpenMesh(openFileDialog1.FileName);
                ((BaseView)panel1).UpdateWindow();
                UseWaitCursor = false;
            }
                        
        }

        private void btnImport2_Click(object sender, EventArgs e)
        {           
   
        }

        private void OnMouseWheel(object sender, MouseEventArgs e)
        {          
            if (mCurrent != null)
            {
                mCurrent.OnMouseWheel(e.X, e.Y, e.Delta);
            }
        }
        private void selectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string szName = mCurrent.OnSelect();
            Debug.WriteLine(szName);
        }


        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (mCurrent != null)
                mCurrent.AddPOI(szIconPath);
        }

        private void removePOIToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (mCurrent != null)
                mCurrent.RemovePOI();
        }

        
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            string szSelect = cmbPoiType.SelectedItem.ToString();
            if( szSelect != null && szSelect != "")
            {
                szIconPath = szMediaPath + "\\icons\\"+szSelect+".ico";
            }
        }

        private void btnAddPOI_Click(object sender, EventArgs e)
        {
            
        }

        private void popupMenu_Opening(object sender, CancelEventArgs e)
        {

        }

        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            Position3D pos = mCurrent.OnPosition();
            mCurrent.AddGroupName("한글 한글", pos.X, pos.Y, pos.Z);
        }
        private int m_nFireID = 1;
        private void toolStripMenuItem3_Click(object sender, EventArgs e)
        {
           // Position3D pos = mCurrent.OnPosition();
            //pos.Y += 3.0f;
            //mCurrent.AddFire(m_nFireID++, pos.X, pos.Y, pos.Z, "Fire"+m_nFireID.ToString());
        }


         
    }
}
