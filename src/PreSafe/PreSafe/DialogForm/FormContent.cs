using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Sections;

namespace PreSafe
{
    internal partial class FormContent : Form, ISectionListener
    {
        internal enum ShowOption
        {
            Text = 0,
            Expression = 1
        }

        public String SenarioTitle
        {
            get 
            {
                return mSenarioTitle.Text; 
            }
            set
            {
                mSenarioTitle.Text = value;

                mSectionPanel.ChangeStepName(this.Text, value);

                this.Text = value;

            }
        }

        private int m_nSenarioType = 1;
        public int SenarioType
        {
            get 
            {
                return m_nSenarioType; 
            }
            
            set 
            {                
                m_nSenarioType = value; 
                switch(m_nSenarioType)
                {
                    case 1:
                        mSectionPanel.DisasterType = "성범죄";
                        break;
                    case 2:
                        mSectionPanel.DisasterType = "노인";
                        break;
                    case 3:
                        mSectionPanel.DisasterType = "유아";
                        break;
                }
            }
        }

        private string m_szSenarioPath = "";
        public string SenarioPath
        {
            get { return m_szSenarioPath; }
            set { m_szSenarioPath = value; }
        }

        public PanelSectionEx SectionPanel
        {
            get { return mSectionPanel; }
        }

        private ShowOption m_ShowOption = ShowOption.Text;
        public ShowOption ContentOption
        {
            get { return m_ShowOption; }
            set
            {
                m_ShowOption = value;
                ChangeVisibleOption(m_ShowOption);
            }
        }

        
        private PointF[] m_arrDragDropOrigin = null;

        private Sections.Section.ComponentType m_sectionDragDropType = Sections.Section.ComponentType.NONE;
        
        public FormContent()
        {
            InitializeComponent();
            mSectionPanel.MouseMove += new System.Windows.Forms.MouseEventHandler(this.SectionPanel_MouseMove);
            mSectionPanel.SetListener(this);
            SetSectionColor();

            mSectionPanel.DisasterType = "성범죄";
            mSectionPanel.TeamName = "main";
            SenarioTitle = "새 시나리오";
            this.MouseWheel += new MouseEventHandler(FormContent_MouseWheel);
        }

        public void RefreshContent()
        {
            mSectionPanel.Refresh();
        }

        public bool InitSectionPanel()
        {
            if (CheckModify())
            {
                DialogResult result = UnE.Utility.UMessageBox.Show("변경된 사항이 있습니다. 현재 시나리오를 저장하시겠습니까?", "저장 확인", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);

                if( result == DialogResult.Yes)
                {
                    if(FormMain.Instance.SaveSenarioFile())
                    {                        
                        return true;
                    }                    
                }
                else if( result == DialogResult.No)
                {                    
                    return true;
                }

                return false;
            } 
            return true;
        }

        public void ClearData()
        {
            m_arrDragDropOrigin = null;
            m_sectionDragDropType = Section.ComponentType.NONE;

            mSectionPanel.ClearSelection();
            mSectionPanel.ClearData();            

            ClearModify();
            mSectionPanel.Refresh();
        }

        public void ClearModify()
        {
            mSectionPanel.IsModified = false;
        }
        
        public bool CheckModify()
        {
            return mSectionPanel.IsModified;
        }
        
        private void SetSectionColor()
        {
            EditBox.SetColor(true, Color.White);
            EditBox.SetColor(false, Color.FromArgb(60, 56, 71));

            Arrow.NormalPen.Color = Color.White;
            Arrow.TempLinePen.Color = Color.WhiteSmoke;
            Arrow.TriangleBrush.Color = Color.White;



            Arrow.TextFont = Properties.Settings.Default.ArrowFont;
            Arrow.TextBrush.Color = Color.WhiteSmoke;
            Sections.Shape.UseImage = false;
        }

        public void ClearSelectionComponent()
        {
            FormSelectComponet form = FormMain.Instance.ComponentForm;
            form.ClearSelection();
        }

        private void FormContent_Load(object sender, EventArgs e)
        {
        }

        private void FormContent_FormClosing(object sender, FormClosingEventArgs e)
        {
        }

        private void FormContent_Resize(object sender, EventArgs e)
        {
        }

        public void SetDragDropShape(PointF[] arrDragDrop, Sections.Section.ComponentType sectionType)
        {
            m_arrDragDropOrigin = arrDragDrop;
            m_sectionDragDropType = sectionType;

        }

        private void SectionPanel_MouseMove(object sender, MouseEventArgs e)
        {
            if (m_arrDragDropOrigin == null)
            {
                mSectionPanel.MoveDrawingArray(null, Sections.Section.ComponentType.NONE, 0, 0);
                return;
            }

            Point ptPanel = mSectionPanel.Location;
            Size sizePanel = mSectionPanel.Size;

            if (mSectionPanel.Visible == true)
            {
                if (e.X >= 0 && e.X <= sizePanel.Width && e.Y >= 0 && e.Y <= sizePanel.Height)
                    mSectionPanel.MoveDrawingArray(m_arrDragDropOrigin, m_sectionDragDropType, e.X, e.Y);
                else
                    mSectionPanel.MoveDrawingArray(null, m_sectionDragDropType, 0, 0);
            }
        }
        
        void FormContent_MouseWheel(object sender, MouseEventArgs e)
        {
            mWheelEndCheckTimer.Stop();
            mWheelEndCheckTimer.Enabled = false;

            Point ptTabBegin = Location;
            Rectangle rect = DisplayRectangle;

            int nPanelX = e.X - (ptTabBegin.X + rect.X);
            int nPanelY = e.Y - (ptTabBegin.Y + rect.Y);
           

            Point ptPanel = mSectionPanel.Location;
            Size sizePanel = mSectionPanel.Size;

            if (nPanelX >= ptPanel.X && nPanelX <= ptPanel.X + sizePanel.Width &&
                nPanelY >= ptPanel.Y && nPanelY <= ptPanel.Y + sizePanel.Height)
            {
                if (mSectionPanel.Visible == true)
                {
                    mSectionPanel.WheelMouse(nPanelX - ptPanel.X, nPanelY - ptPanel.Y, e.Delta);
                }
                
            }

            mWheelEndCheckTimer.Interval = 500;
            mWheelEndCheckTimer.Enabled = true;
            mWheelEndCheckTimer.Start();            
        }

        private void mWheelEndCheckTimer_Tick(object sender, EventArgs e)
        {
            mWheelEndCheckTimer.Stop();
            mWheelEndCheckTimer.Enabled = false;

            int x = Cursor.Position.X;
            int y = Cursor.Position.Y;
            Point ptClient = mSectionPanel.PointToClient(new Point(x, y));
            MouseEventArgs ex = new MouseEventArgs(MouseButtons.None, 0, ptClient.X, ptClient.Y, 0);

            SectionPanel_MouseMove(mSectionPanel, ex);

        }

        public void OnSelectedArrow(Sections.Arrow arSelected)
        {
            System.Diagnostics.Debug.WriteLine(arSelected);
        }

        public void OnSelectedSection(Sections.Section secSelected)
        {
            System.Diagnostics.Debug.WriteLine(secSelected);
            FormProperties form = FormMain.Instance.PropertiesForm;
            form.SetComponent(secSelected);
        }

        public void SetCurrentPanel(Sections.PanelSection panel)
        {

        }

        public void OnSelectedSectionList(ArrayList arSections)
        {

        }

        private void ChangeVisibleOption(ShowOption option)
        {
            if(option == ShowOption.Expression)
            {
                mSectionPanel.SetDisplayText(false);
            }
            else
            {
                mSectionPanel.SetDisplayText(true);
            }
        }
        
    }
}
