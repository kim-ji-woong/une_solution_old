using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;

namespace UnE.SOP.Sections
{
     // Tab없애기
    public partial class SectionTabControl : UnE.Controls.TabControlEx
    //public partial class SectionTabControl : TabControl
    {
        private SectionTabPage tabPage1;
        private SectionTabPage tabPage2;
        private SectionTabPage tabPage3;
        private SectionTabPage tabPage4;

        // SOP의 표준 실행단계
        private static string[] StandardActionStepName = new string[] { "예방", "대비", "대응", "복구" };

        public static string[] StandardActionStepNames
        {
            get { return StandardActionStepName; }
        }

        // SOP 표준 실행단계
        public static void SetStandardActionStepNames(List<string> actionStepNames)
        {
            if (actionStepNames == null || actionStepNames.Count == 0)
                return;

            if (actionStepNames.Count != StandardActionStepName.Count())
                StandardActionStepName = new string[actionStepNames.Count];

            for (int i = 0; i < actionStepNames.Count; i++)
            {
                StandardActionStepName[i] = actionStepNames[i];
            }
        }

        // SOP 실행단계의 우선순위 : 대응, 예방, 대비, 복구 순
        public static int GetActionStepPriority(string strActionStepName)
        {
            int nCount = StandardActionStepName.Count();

            for (int i=0;i<nCount;i++)
            {
                if (strActionStepName == StandardActionStepName[i])
                {
                    if (i < 2)
                    {
                        if (nCount >= 3)
                            return i + 1;
                        else
                            return i;
                    }
                    else if (i == 2)
                        return 0;
                    else
                        return i;
                }
            }

            return nCount;
        }

        public SectionTabControl()
        {
            IntiTabPage();


            // Tab없애기
            this.UseCloseButton = false;
            //this.TabBackColor = Color.FromArgb(60, 56, 71);
            //this.SelectedTabColor = Color.FromArgb(60, 56, 71);
            //this.m_foreColor = Color.FromArgb(60, 56, 71);
            this.m_disableColor = Color.DarkGray;
            this.SelectedTabColor = System.Drawing.Color.DarkGray;
            this.ShowToolTips = true;
            this.Size = new System.Drawing.Size(718, 422);
            this.SizeMode = System.Windows.Forms.TabSizeMode.Fixed;
            this.TabBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(52)))), ((int)(((byte)(73)))), ((int)(((byte)(94)))));
            this.TabForeColor = System.Drawing.Color.White;
            this.TabIndex = 0;

          
            this.UseCloseButton = false;

            this.SelectedTab = null;
        }

        private void IntiTabPage()
        {
            this.SuspendLayout();
            // 
            // SectionTabControl
            // 

            if(!(System.ComponentModel.LicenseManager.UsageMode == LicenseUsageMode.Designtime))
            {
                tabPage1 = new SectionTabPage(this);
                tabPage2 = new SectionTabPage(this);
                tabPage3 = new SectionTabPage(this);
                tabPage4 = new SectionTabPage(this);

                this.Selected += new System.Windows.Forms.TabControlEventHandler(this.SectionTabControl_Selected);
                this.SizeChanged += new System.EventHandler(this.SectionTabControl_SizeChanged);
                this.Selecting += new System.Windows.Forms.TabControlCancelEventHandler(this.SectionTabControl_Selecting);
                this.Controls.Add(this.tabPage1);
                this.Controls.Add(this.tabPage2);
                this.Controls.Add(this.tabPage3);
                this.Controls.Add(this.tabPage4);
                this.Dock = System.Windows.Forms.DockStyle.Fill;
                this.Location = new System.Drawing.Point(0, 0);
                this.Name = "tabControl1";
                this.SelectedIndex = 0;

                // 
                // tabPage1
                // 
                this.tabPage1.Location = new System.Drawing.Point(4, 22);
                this.tabPage1.Name = "tabPage1";
                this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
                this.tabPage1.Size = new System.Drawing.Size(710, 396);
                this.tabPage1.TabIndex = 0;

                if (StandardActionStepName.Count() > 0)
                    this.tabPage1.Text = StandardActionStepName[0];
                else
                    this.tabPage1.Text = "예방";

                this.tabPage1.UseVisualStyleBackColor = true;
                // 
                // tabPage2
                // 
                this.tabPage2.Location = new System.Drawing.Point(4, 22);
                this.tabPage2.Name = "tabPage2";
                this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
                this.tabPage2.Size = new System.Drawing.Size(710, 396);
                this.tabPage2.TabIndex = 1;

                if (StandardActionStepName.Count() > 1)
                    this.tabPage2.Text = StandardActionStepName[1];
                else
                    this.tabPage2.Text = "대비";

                this.tabPage2.UseVisualStyleBackColor = true;
                // 
                // tabPage3
                // 
                this.tabPage3.Location = new System.Drawing.Point(4, 22);
                this.tabPage3.Name = "tabPage3";
                this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
                this.tabPage3.Size = new System.Drawing.Size(710, 396);
                this.tabPage3.TabIndex = 1;

                if (StandardActionStepName.Count() > 2)
                    this.tabPage3.Text = StandardActionStepName[2];
                else
                    this.tabPage3.Text = "대응";

                this.tabPage3.UseVisualStyleBackColor = true;
                // 
                // tabPage4
                // 
                this.tabPage4.Location = new System.Drawing.Point(4, 22);
                this.tabPage4.Name = "tabPage4";
                this.tabPage4.Size = new System.Drawing.Size(710, 396);
                this.tabPage4.TabIndex = 2;

                if (StandardActionStepName.Count() > 3)
                    this.tabPage4.Text = StandardActionStepName[3];
                else
                    this.tabPage4.Text = "복구";

                this.tabPage4.UseVisualStyleBackColor = true;

                ((Control)tabPage1).Enabled = false;
                ((Control)tabPage2).Enabled = false;
                ((Control)tabPage3).Enabled = false;
                ((Control)tabPage4).Enabled = false;
            }
           

            this.ResumeLayout(false);
        }

        private void SectionTabControl_Selecting(object sender, TabControlCancelEventArgs e)
        {
            if (e.TabPage == null)
            {
                e.Cancel = true;
                return;
            }
            if(this.Visible == false)
            {
                SectionTabPage page = (SectionTabPage)e.TabPage;
                if (page.ActionStepID <= 0)
                {
                    e.Cancel = true;
                    return;
                }
            }
            else
            {
                bool bEnabled = ((Control)e.TabPage).Enabled;
                if (bEnabled == false)
                {
                    e.Cancel = true;
                    return;
                }
            }           
        }

        private void SectionTabControl_Selected(object sender, TabControlEventArgs e)
        {

            if (this.SelectedTab != null)
            {
                SectionTabPage page = (SectionTabPage)this.SelectedTab;
                if( page != null)
                    page.ReSizePanel();
            }
        }

        private void SectionTabControl_SizeChanged(object sender, EventArgs e)
        {
            foreach (SectionTabPage page in Controls)
            {
                page.ReSizePanel();
            }           
           
        }

        public void ResizeTabContorl()
        {
            foreach (SectionTabPage page in Controls)
            {
                page.ReSizePanel();
            } 
        }

        public void RemoveAllTabPage()
        {
            TabPages.Clear();
        }

        public void InitTabPages()
        {
            RemoveAllTabPage();
            this.TabPages.Insert(0, tabPage1);
            this.TabPages.Insert(1, tabPage2);
            this.TabPages.Insert(2, tabPage3);
            this.TabPages.Insert(3, tabPage4);
        }

        public int GetValidTabPageCount()
        {
            int nCount = 0;
            foreach(SectionTabPage page in TabPages)
            {
                if( page.ActionStepID > 0)
                {
                    nCount++;
                }
            }
            return nCount;
        }

        public SectionTabPage GetFirstPage()
        {
            int nFirstPriority = -1;
            SectionTabPage firstPage = null;

            foreach (SectionTabPage page in TabPages)
            {
                if( page.ActionStepID > 0)
                {
                    int nPriority = SectionTabControl.GetActionStepPriority(page.Text);

                    if (firstPage == null || nFirstPriority > nPriority)
                    {
                        nFirstPriority = nPriority;
                        firstPage = page;
                    }
                }
            }

            return firstPage;
        }

        public void RemoveTabPage(SectionTabPage page)
        {
            if (page.Text == "예방")
            {
                if( this.TabPages.Contains(page))
                {
                    this.TabPages.Remove(page);
                    this.TabPages.Insert(0, tabPage1);
                }                
            }
            else if (page.Text == "대비")
            {
                if (this.TabPages.Contains(page))
                {
                    this.TabPages.Remove(page);
                    this.TabPages.Insert(1, tabPage2);
                } 
            }
            else if (page.Text == "대응")
            {
                if (this.TabPages.Contains(page))
                {
                    this.TabPages.Remove(page);
                    this.TabPages.Insert(2, tabPage3);
                } 
            }
            else if (page.Text == "복구")
            {
                if (this.TabPages.Contains(page))
                {
                    this.TabPages.Remove(page);
                    this.TabPages.Insert(3, tabPage4);
                } 
            }
        }

        protected new TabControl.TabPageCollection TabPages
        {
            get { return base.TabPages;  }
        }

        protected new Control.ControlCollection Controls
        {
            get { return base.Controls;  }
        }

        private TabPage FindPageIndex(string szName)
        {
            int nCount = 0;
            foreach(TabPage page in TabPages)
            {
                if( page.Text == szName)
                {
                    return page;
                }
                nCount++;
            }
            return null;
        }
        public void AddTabPage(SectionTabPage page)
        {
            TabPage delPage = FindPageIndex(page.Text);
            if (delPage != null)
            {
                if (page == delPage)
                    return;

                this.TabPages.Remove(delPage);
            }            

            if( page.Text == "예방")
            {
                this.TabPages.Insert(0, page);
            }
            else if( page.Text == "대비")
            {
                this.TabPages.Insert(1, page);
            }
            else if( page.Text == "대응")
            {              
                this.TabPages.Insert(2, page);
            }
            else if( page.Text == "복구")
            {
                this.TabPages.Insert(3, page);
            }            
        }
    }
}
