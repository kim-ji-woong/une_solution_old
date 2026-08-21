using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using Sections;
using UnE.SOP.Sections;
using UnE.SOP.Workstate;

namespace SOPMonitoringSystem
{
    public class PreviewComponentContainer  : Panel
    {
        private ArrayList m_arComponentContes = new ArrayList();
        private SectionTabPage m_ParentPage = null;
        private Label label1;
        private int m_nActionStepID = -1;
        private ArrayList m_arSections = null;
        
        public PreviewComponentContainer(SectionTabPage page) : base()
        {
            this.DoubleBuffered = true;
            
            InitializeComponent();

            m_ParentPage = page;

            m_nActionStepID = m_ParentPage.ActionStepID;           
        }

        public FormPreviewContainer GetParent()
        {
            return (FormPreviewContainer)this.Parent;
        }
        
        public void InitSectionCentent()
        {
            ArrayList arList = SOPScenarioManager.Instance.GetAllPanels(m_nActionStepID);
            m_arSections = SOPScenarioManager.Instance.GetAllPanelSections(arList);
            m_arSections.Reverse();
            foreach(Section section in m_arSections)
            {

                if (section.GetComponentType() == Section.ComponentType.ANNOTATION)
                    continue;
                if (section.GetComponentType() == Section.ComponentType.ENDPOINT)
                    continue;
                int nComponentID = section.Data.ID;

                string szName = section.Data.Title;
                string strStatus = "미리보기";
                PreviewComponentContents cotent = MakeComponentContents(nComponentID, szName, strStatus, section, State.NORMAL, 0,0);
            }
        }

        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(0, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(100, 23);
            this.label1.TabIndex = 0;
            this.label1.Text = "label1";
            this.ResumeLayout(false);
        }

        private ArrayList GetComponentContentsList()
        {
            return (ArrayList)m_arComponentContes.Clone();
        }

        private PreviewComponentContents MakeComponentContents(int nComponentID, string strTask, string strStatus, Sections.Section section, State sectionState, int nCheckNotify1, int nCheckNotify2)
        {
            ArrayList arrContents = GetComponentContentsList();
            int nContentsCount = arrContents == null ? 0 : arrContents.Count;

            PreviewComponentContents frmContents = new PreviewComponentContents();

            frmContents.Location = new Point(0, 0);
            frmContents.Anchor = ((System.Windows.Forms.AnchorStyles)(AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right));
            frmContents.TopLevel = false;
            frmContents.Parent = this;
            frmContents.ComponentID = nComponentID;   
            frmContents.Dock = DockStyle.Top;
           
            MakeComponentContentsData(frmContents, strTask, strStatus, section, sectionState, nCheckNotify1, nCheckNotify2);

            AddComponentContents(frmContents);

            ///frmContents.State = sectionState;
            frmContents.TogleCollapse();
            frmContents.Show();
            frmContents.Select();

            return frmContents;
        }

        public static void MakeComponentContentsData(PreviewComponentContents frmContents, string strTask, string strStatus, Sections.Section section, State sectionState, int nCheckNotify1, int nCheckNotify2)
        {
            
            int nSection = section.Data.SectionNumber;
            frmContents.Tag = nSection;
            frmContents.SetTitle(nSection .ToString() + ". "+ strTask, strStatus);
            frmContents.AddGridData(section, strStatus, nCheckNotify1, nCheckNotify2);
        }

        public void AddComponentContents(PreviewComponentContents contents)
        {
            ArrayList arrContents = GetComponentContentsList();

            if (arrContents.Count == 0)
            {
                m_arComponentContes.Add(contents);

                m_ParentPage.PanelPreviewComponentContents.Controls.Add(contents);
                m_ParentPage.PanelPreviewComponentContents.Controls.SetChildIndex(contents, 0);
                contents.SendToBack();
            }
            else
            {
                InsertComponentContents(contents);
                return;
            }
        }

        public void InsertComponentContents(PreviewComponentContents contents)
        {
            ArrayList arrContents = GetComponentContentsList();
           

            int nSection = (int)contents.Tag;
            int nCount = 0;
            foreach (PreviewComponentContents comp in arrContents)
            { 
                int nNum = (int)comp.Tag;
                if( nNum < nSection)
                {
                    break;
                }
                nCount++;
            }
            m_arComponentContes.Insert(nCount, contents);
            //m_arComponentContes.Add();

            m_ParentPage.PanelPreviewComponentContents.Controls.Add(contents);
            m_ParentPage.PanelPreviewComponentContents.Controls.SetChildIndex(contents, nCount);
        }

        public void ClearComponentContents()
        {
            m_arComponentContes.Clear();
            m_ParentPage.PanelPreviewComponentContents.Controls.Clear();
        }
        

        public void CollapseAll()
        {
            ArrayList arList = (ArrayList)m_arComponentContes.Clone();
            foreach(PreviewComponentContents content in arList)
            {
                content.Collase();
            }
        }

        public void ExpandAll()
        {
            ArrayList arList = (ArrayList)m_arComponentContes.Clone();
            foreach (PreviewComponentContents content in arList)
            {
                content.Expand();
            }
        }

        public void SelectSection(Section section)
        {
            ArrayList arList = (ArrayList)m_arComponentContes.Clone();
            foreach (PreviewComponentContents content in arList)
            {
                if(content.Section == section)
                {
                    content.ExternalSelectIon();
                    this.AutoScrollPosition = content.Location;
                }
                else
                {
                    content.UnSelectSection();
                }
            }
        }

        protected override System.Drawing.Point ScrollToControl(System.Windows.Forms.Control activeControl)
        {
            // Returning the current location prevents the panel from
            // scrolling to the active control when the panel loses and regains focus
            return this.DisplayRectangle.Location;
        }
    }
}
