using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace SOPGen
{
    public partial class FormProcess : Form
    {
        private FormMain m_Main = null;

        //private SectionTimeText m_sectionFirst = null;
        //private ArrayList m_arrSections = new ArrayList();
        private Dictionary<SOPData, ArrayList> m_dicSOPSections = new Dictionary<SOPData, ArrayList>();
        //private ArrayList m_arrTeamData = new ArrayList();
        private SectionTimeText m_sectionSelected = null;
        private bool m_clickedLButton = false;
        private Point m_ptClicked = new Point();
        private Point m_ptSelected = new Point();
        private Size m_sizeNormal;
        public Label m_label = null;
        private bool m_isSelect = false;
        private SOPData m_selectedSOP = null;
        private int m_nSelectedDepth = -1;
        
        enum SectionType { PROCESS_SECTION, GROUP_SECTION, INVALID_SECTION };

        //public ArrayList ArrTeamData
        //{
        //    get { return m_arrTeamData; }
        //    set { m_arrTeamData = value; }
        //}

        public FormProcess(FormMain main)
        {
            InitializeComponent();

            m_Main = main;

            Section temp = new Section(this);
            m_sizeNormal = temp.RectSize;
            temp.Hide();

            label4Scroll.Text = "";
        }

        SectionType GetSectionType(SectionTimeText section)
        {
            if (section == null)
                return SectionType.INVALID_SECTION;

            Section sectionParent = section.GetParentSection();

            if (sectionParent == null)
                return SectionType.PROCESS_SECTION;

            if (sectionParent.GetParentSection() == null)
                return SectionType.GROUP_SECTION;

            return SectionType.INVALID_SECTION;
        }

        public void AddProcess()
        {
            if (m_nSelectedDepth < 3)
            {
                if (m_sectionSelected == null)
                {
                    MessageBox.Show("프로세스를 생성할 SOP 단계가 지정되지 않았습니다.\r\n왼쪽의 SOP Tree에서 상황 및 단계를 생성하고 원하는 단계를 선택해 주세요.", "생성 오류");
                    return;
                }
            }

            if (m_selectedSOP == null)
                return;
            
            ArrayList arrSections;

            if (m_dicSOPSections.ContainsKey(m_selectedSOP))
                arrSections = m_dicSOPSections[m_selectedSOP];
            else
            {
                arrSections = new ArrayList();
                m_dicSOPSections[m_selectedSOP] = arrSections;
            }

            if (m_sectionSelected == null)
            {
                if (arrSections.Count != 0)
                    return;

                SectionTimeText sectionBegin = new SectionTimeText(this, /*m_Main.GetPaneLayer().Width + */60, 20);
                sectionBegin.GetTextBox().Text = "시작";

                //m_sizeNormal = sectionBegin.RectSize;

                SectionTimeText sectionEnd = new SectionTimeText(this);
                sectionEnd.GetTextBox().Text = "종료";

                Point pt = sectionBegin.Position;
                Size size = sectionBegin.RectSize;

                sectionEnd.Position = new Point(pt.X, pt.Y + size.Height + sectionEnd.RectSize.Height);

                arrSections.Add(sectionBegin);
                arrSections.Add(sectionEnd);
                sectionBegin.SetNext(sectionEnd);
            }
            else
            {
                if (GetSectionType(m_sectionSelected) != SectionType.PROCESS_SECTION)
                    return;

                Point pt = m_sectionSelected.Position;
                Size size = m_sectionSelected.RectSize;

                int nBeginHour, nBeginMinute, nProcessHour, nProcessMinute;
                m_sectionSelected.GetTime(true, out nBeginHour, out nBeginMinute);
                m_sectionSelected.GetTime(false, out nProcessHour, out nProcessMinute);

                int nMinute = (nBeginMinute + nProcessMinute) % 60;
                int nHour = nBeginHour + nProcessHour + (nBeginMinute + nProcessMinute) / 60;

                SectionTimeText newSection = new SectionTimeText(this);
                newSection.Position = new Point(pt.X, pt.Y + size.Height + newSection.RectSize.Height);
                newSection.SetTime(true, nHour, nMinute);

                m_sectionSelected.SetNext(newSection);

                int nIndex = arrSections.IndexOf(m_sectionSelected);
                arrSections.Insert(nIndex + 1, newSection);

                m_sectionSelected.Select(false, null);
                m_sectionSelected = null;

                newSection.Edit();
            }

            AutoAlign();

            Point ptScroll = GetRightPosition();
            label4Scroll.Location = ptScroll;

            Refresh();
            //Invalidate(true);
            //Update();
        }

        // Section의 가장 오른쪽 아래 모서리 Position
        protected Point GetRightPosition(Point ptBR = new Point(), ArrayList arrSections = null)
        {
            if (arrSections == null)
                arrSections = GetCurrentSections();

            if (arrSections == null)
                return ptBR;

            foreach (SectionTimeText section in arrSections)
            {
                int right = section.Position.X + section.RectSize.Width;
                int bottom = section.Position.Y + section.RectSize.Height;

                if (ptBR.X < right) ptBR.X = right;
                if (ptBR.Y < bottom) ptBR.Y = bottom;

                GetRightPosition(ptBR, section.GetChildSections());
            }

            return ptBR;
        }

        protected bool CheckDuplicateTeamSchedule(Section parentSection, TeamData data, out int nBeginHour, out int nBeginMinute)
        {
            nBeginHour = nBeginMinute = -1;
            if (data == null) return true;
            if (data.ID <= 0) return true;
            if (parentSection == null) return true;

            ArrayList arrChilds = parentSection.GetChildSections();
            FormTeamSchedule frm = null;

            int nHour, nMinute;
            
            foreach (SectionTimeText section in arrChilds)
            {
                if (section.Data == null) continue;

                if (section.Data.Type == data.Type &&
                    section.Data.ID == data.ID)
                {
                    if (frm == null)
                    {
                        frm = new FormTeamSchedule(data.Type == TeamData.DataType.TeamData);
                        frm.SetData(data);
                    }

                    section.GetTime(true, out nHour, out nMinute);
                    frm.AddBeginTime(string.Format("{0:00}:{1:00}", nHour, nMinute));
                }
            }

            if (frm == null) return true;

            if (frm.ShowDialog() == DialogResult.OK)
            {
                frm.GetBeginTime(out nBeginHour, out nBeginMinute);
                return true;
            }

            return false;
        }

        public void AddGroup(TeamData data)
        {
            if (m_sectionSelected == null)
                return;

            if (GetSectionType(m_sectionSelected) != SectionType.PROCESS_SECTION)
                return;

            int nBeginHour, nBeginMinute;
            if (!CheckDuplicateTeamSchedule(m_sectionSelected, data, out nBeginHour, out nBeginMinute))
                return;

            SectionTimeText newSection = new SectionTimeText(this);
            Section lastSection = m_sectionSelected.GetLastChild();

            if (lastSection == null)
            {
                Point pt = m_sectionSelected.Position;
                Size size = m_sectionSelected.RectSize;

                newSection.Position = new Point(pt.X + m_sectionSelected.RectSize.Width + newSection.RectSize.Width, pt.Y);
            }
            else
            {
                Point pt = lastSection.Position;
                Size size = lastSection.RectSize;

                newSection.Position = new Point(pt.X, pt.Y + size.Height + newSection.RectSize.Height);
            }

            //int nBeginHour, nBeginMinute;
            if (nBeginHour < 0 || nBeginMinute < 0)
                m_sectionSelected.GetTime(true, out nBeginHour, out nBeginMinute);

            newSection.SetTime(true, nBeginHour, nBeginMinute);

            m_sectionSelected.AddChild(newSection);

            m_sectionSelected.Select(false, null);
            m_sectionSelected = null;

            if (m_Main.GetPaneLayer().IsDragDrop)
            {
                //newSection.SetText(m_strValue);
                newSection.Data = data;
                newSection.SetText(data.Name);
            }
            else
                newSection.Edit();

            // Group Section과 연결된 임무 정보
            newSection.MissionData = new MemberofSection();
            newSection.MissionData.Member = newSection.GetTextBox().Text;

            AutoAlign();
            Refresh();
        }

        private void OnPaint(object sender, PaintEventArgs e)
        {
            ArrayList arrSections = GetCurrentSections();

            if (arrSections != null)
            {
                foreach (SectionTimeText section in arrSections)
                {
                    section.Draw(e.Graphics);
                }
            }
        }

        private void OnMouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                m_clickedLButton = true;
                m_ptClicked.X = e.X;
                m_ptClicked.Y = e.Y;

                if (m_sectionSelected != null && m_sectionSelected.GetChangeSizeOption() != EditBox.BoxPosition.NO_SELECT)
                {
                    m_sectionSelected.SetChangeSizeOriginPoint(e.X, e.Y);
                }
                else
                {
                    if (SelectSection(e.X, e.Y))
                        m_ptSelected = m_sectionSelected.Position;
                    else
                        m_Main.HideExpandedPane();
                }
            }
            else if (e.Button == MouseButtons.Right)
            {
                if (SelectSection(e.X, e.Y))
                {
                    m_ptSelected = m_sectionSelected.Position;

                    SectionType type = GetSectionType(m_sectionSelected);

                    if (type == SectionType.PROCESS_SECTION)
                    {
                        contextProcessMenu.Show(this, new Point(e.X, e.Y));
                    }
                    else if (type == SectionType.GROUP_SECTION)
                    {
                        contextGroupMenu.Show(this, new Point(e.X, e.Y));
                    }
                }
                else
                {
                    // 자동정렬 메뉴
                    //contextFormMenu.Show(this, new Point(e.X, e.Y));
                }
            }

        }

        private void OnMouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                m_clickedLButton = false;

                m_isSelect = SelectSection(e.X, e.Y);
                if (m_isSelect)
                    AddSection(e.X, e.Y);

                this.Controls.Remove(m_label);
                m_label = null;
                m_Main.GetPaneLayer().IsDragDrop = false;

                if (m_sectionSelected != null)
                    m_sectionSelected.DoubleSelect(false);
          
            }
        }

        public void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (m_clickedLButton)
            {
                if (m_sectionSelected != null)
                {
                    if (m_sectionSelected.GetChangeSizeOption() != EditBox.BoxPosition.NO_SELECT)
                    {
                        // Section 크기 변경 금지
                        //m_sectionSelected.ChangeSize(e.X, e.Y);
                    }
                    else
                    {
                        // Section 이동 금지
                        /*int xMove = e.X - m_ptClicked.X;
                        int yMove = e.Y - m_ptClicked.Y;

                        m_sectionSelected.Position = new Point(m_ptSelected.X + xMove, m_ptSelected.Y + yMove);
                        Refresh();*/
                        /*Rectangle rectOrigin = m_sectionSelected.InvalidateRectArea;

                        int nWidth = rectOrigin.Width;
                        int nHeight = rectOrigin.Height;
                        Rectangle rect = new Rectangle(rectOrigin.Left - nWidth, rectOrigin.Top - nHeight, nWidth * 3, nHeight * 3);

                        Invalidate();//rect);*/
                    }
                }
            }
            else
            {
                if (m_sectionSelected != null)
                {
                    m_sectionSelected.CheckMouse(e.X, e.Y);
                }
                else
                {
                    this.Cursor = Cursors.Arrow;
                }
            }

            if (m_label != null)
            {
                m_label.BringToFront();
                m_label.Location = new System.Drawing.Point(e.X, e.Y);
            }

        }

        private bool _SelectSection(int x, int y)
        {
            ArrayList arrSections = GetCurrentSections();
            if (arrSections == null) return false;

            foreach (SectionTimeText section in arrSections)
            //if (m_sectionFirst != null)
            {
                SectionTimeText secsionSelected = (SectionTimeText)section.Select(x, y);

                if (secsionSelected != null)
                {
                    if (m_sectionSelected != null)
                    {
                        if (m_sectionSelected != secsionSelected)
                        {
                            m_sectionSelected.Select(false, null);
                            Refresh();
                            //Invalidate(m_sectionSelected.InvalidateRectArea, true);
                            //Update();
                        }
                        else
                        {
                            // 선택된 상태에서 다시 선택되었음을 알린다.
                            // 텍스트 편집이나 기타 기능을 수행할 수 있다.
                            m_sectionSelected.DoubleSelect(true);
                        }
                    }
                    if (m_Main.GetPaneLayer().IsDragDrop)
                    {
                        if (secsionSelected.GetParentSection() == null)
                        {
                            secsionSelected.Select(true, null);//m_arrSection);
                            m_sectionSelected = secsionSelected;
                        }
                        else
                            return false;

                        //Refresh();
                        return true;
                    }
                    else
                    {
                        secsionSelected.Select(true, null);//m_arrSection);
                        m_sectionSelected = secsionSelected;
                        //Invalidate(secsionSelected.InvalidateRectArea, true);
                        //Update();
                        Refresh();
                        return true;
                    }
                }
            }

            if (m_sectionSelected != null)
            {
                m_sectionSelected.Select(false, null);//m_arrSection);
                Refresh();
                //Invalidate(m_sectionSelected.InvalidateRectArea, true);
                //Update();
                m_sectionSelected = null;
            }

            return false;
        }

        private bool SelectSection(int x, int y)
        {
            if (_SelectSection(x, y))
            {
                if(!m_Main.GetPaneLayer().IsDragDrop)
                {
                    if (m_sectionSelected.GetParentSection() != null)
                    {
                        m_sectionSelected.MissionData.Member = m_sectionSelected.GetTextBox().Text;
                        m_Main.GetMission().SetMissionData(m_sectionSelected.MissionData);

                        // 조직
                        m_Main.SelectGroup();
                    }
                    else
                    {
                        // Process
                        m_Main.SelectProcess();
                    }
                }
                return true;
            }

            return false;
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                if (m_sectionSelected != null)
                {
                    // 실행안됨
                }
            }
        }

        private void _DeleteSection(SectionTimeText section)
        {
            SectionTimeText sectionParent = (SectionTimeText)section.GetParentSection();

            if (sectionParent != null)
            {
                sectionParent.RemoveChild(section);
                section.Hide();
            }
            else
            {
                SectionTimeText next = (SectionTimeText)section.GetNext();
                SectionTimeText prev = (SectionTimeText)section.GetPrev();

                if (next != null)
                    next.SetPrev(prev);
                else if (prev != null)
                    prev.SetNext(next);

                section.RemoveAllChild();

                ArrayList arrSections = GetCurrentSections();
                if (arrSections != null)
                {
                    if (arrSections.Contains(section))
                        arrSections.Remove(section);
                }

                section.Hide();
            }

            Invalidate();
        }

        private bool DeleteSection(SectionTimeText section)
        {
            SectionType type = GetSectionType(section);

            if (type == SectionType.PROCESS_SECTION)
            {
                DialogResult result = MessageBox.Show("선택한 프로세스와 하위 조직을 모두 삭제하시겠습니까?", "프로세스 삭제", MessageBoxButtons.YesNo);

                if (result == DialogResult.Yes)
                {
                    _DeleteSection(section);
                    AutoAlign();
                    return true;
                }
            }
            else if (type == SectionType.GROUP_SECTION)
            {
                DialogResult result = MessageBox.Show("선택한 조직을 삭제하시겠습니까?", "프로세스 삭제", MessageBoxButtons.YesNo);

                if (result == DialogResult.Yes)
                {
                    _DeleteSection(section);
                    AutoAlign();
                    return true;
                }
            }

            return false;
        }

        public void OnMenuDeleteProcess(object sender, EventArgs e)
        {
            if (DeleteSection(m_sectionSelected))
                m_sectionSelected = null;
            /*if (m_sectionSelected != null)
            {
                SectionTimeText sectionParent = m_sectionSelected.GetParentSection();

                if (sectionParent != null)
                {
                    sectionParent.RemoveChild(m_sectionSelected);
                }
                else
                {
                    SectionTimeText next = m_sectionSelected.GetNext();
                    SectionTimeText prev = m_sectionSelected.GetPrev();

                    if (next != null)
                        next.SetPrev(prev);
                    else if (prev != null)
                        prev.SetNext(next);

                    //m_sectionSelected.SetPrev(null);
                    //m_sectionSelected.SetNext(null);

                    m_sectionSelected.RemoveAllChild();
                    m_arrSections.Remove(m_sectionSelected);
                    m_sectionSelected.Hide();
                }

                m_sectionSelected = null;
                Invalidate();
            }*/
        }

        private void OnMenuAddProcess(object sender, EventArgs e)
        {
            AddProcess();
        }

        public void OnMenuRenameProcess(object sender, EventArgs e)
        {
            if (m_sectionSelected != null)
            {
                m_sectionSelected.Edit();
            }
        }

        private void OnMenuAddGroup(object sender, EventArgs e)
        {
            AddGroup(null);
        }

        public void OnMenuRenameGroup(object sender, EventArgs e)
        {
            if (m_sectionSelected != null)
            {
                m_sectionSelected.Edit();
            }
        }

        public void OnMenuDeleteGroup(object sender, EventArgs e)
        {
            if (DeleteSection(m_sectionSelected))
                m_sectionSelected = null;
        }

        private void OnMenuAutoAlign(object sender, EventArgs e)
        {
            AutoAlign();
        }

        private ArrayList GetCurrentSections()
        {
            if (m_selectedSOP == null) return null;
            if (m_selectedSOP.Depth < 0) return null;

            if (m_dicSOPSections.ContainsKey(m_selectedSOP))
                return m_dicSOPSections[m_selectedSOP];

            return null;
        }

        public void AutoAlign()
        {
            ArrayList arrSections = GetCurrentSections();
            if (arrSections == null) return;

            if (arrSections.Count == 0)
                return;

            SectionTimeText sectionFirst = (SectionTimeText)arrSections[0];
            sectionFirst.Position = new Point(60, 20);
            Point pt = sectionFirst.Position;

            // 스크롤 영역 계산
            Point ptScroll = this.AutoScrollPosition;
            pt.X = pt.X - ptScroll.X;
            pt.Y = pt.Y - ptScroll.Y;
            /////////////////////////

            foreach (SectionTimeText section in arrSections)
            {
                section.Position = pt;
                //SectionTimeText.SetInterpolation(ptScroll.X, ptScroll.Y);
                int x = pt.X + section.GetDiffText(true) - section.GetTextBox().Left;
                int y = pt.Y + section.GetDiffText(false) - section.GetTextBox().Top;
                section.SetInterpolation(x == 0 ? ptScroll.X : x, y == 0 ? ptScroll.Y : y);

                Point ptChild = new Point(pt.X + section.RectSize.Width + m_sizeNormal.Width, pt.Y);

                ArrayList childList = section.GetChildSections();

                if (childList.Count > 0)
                {
                    foreach (SectionTimeText child in childList)
                    {
                        child.Position = ptChild;
                        ptChild.Y = ptChild.Y + child.RectSize.Height + m_sizeNormal.Height;
                    }

                    pt.Y = ptChild.Y;
                }
                else
                {
                    pt.Y = pt.Y + section.RectSize.Height + m_sizeNormal.Height;
                }
            }

            Refresh();
        }

        public void OnAddTime(SectionTimeText section, int nAddHour, int nAddMinute)
        {
            if (section == null)
                return;

            ArrayList arrSections = GetCurrentSections();
            if (arrSections == null) return;

            SectionTimeText parentSection = (SectionTimeText)section.GetParentSection();
            if (parentSection != null)
                return;

            // 프로세스
            bool find = false;

            foreach (SectionTimeText sec in arrSections)
            {
                if (section == sec)
                {
                    find = true;
                }
                else if (find)
                {
                    sec.AddTime(nAddHour, nAddMinute);
                }
            }
        }

        private void ShowListMenu_Click(object sender, EventArgs e)
        {
            FormTeam frm = FormTeam.Instance(true);

            if (frm.ShowDialog() == DialogResult.OK)
            {
                TeamData data = frm.GetSelectedTeamData();
                if (m_sectionSelected != null) m_sectionSelected.SetData(data);
            }
        }

        private void AddSection(int x, int y)
        {
            if (m_Main.GetPaneLayer().IsDragDrop)
            {
                DataGridView gridView = m_Main.GetPaneLayer().GetGridView();
                //m_strValue = m_Main.GetPaneLayer().GetSelectedValue();
                TeamData data = m_Main.GetPaneLayer().GetSelectedValue();
                
                //SelectSection(x, y);
                AddGroup(data);

            }
        }

        // 임무 할당
        //private void SettingJobMenu_Click(object sender, EventArgs e)
        //{
        //    // m_sectionSelected
        //    m_Main.GetMission().textMember.Text = sender.ToString();
        //}


        private SOPData FindSOPData(TreeNode node)
        {
            foreach (KeyValuePair<SOPData, ArrayList> sop in m_dicSOPSections)
            {
                if (sop.Key.Node == node)
                    return sop.Key;
            }

            return null;
        }

        private void ShowSections(ArrayList arrSections, bool isShow)
        {
            if (arrSections == null) return;

            foreach (SectionTimeText section in arrSections)
            {
                if (isShow) section.Show();
                else section.Hide();
            }
        }

        // Tree에서 선택된 SOP 항목
        public void OnSelectedSOP(int nDepth, string strSOPFullName, TreeNode node)
        {
            if (nDepth >= 3)
            {
                if (m_selectedSOP != null)
                {
                    if (m_selectedSOP.Node == node)
                    {
                        m_nSelectedDepth = nDepth;
                        return;
                    }

                    ResetNode(m_dicSOPSections, ref m_selectedSOP);                    
                    ShowSections(m_dicSOPSections[m_selectedSOP], false);
                }

                m_selectedSOP = FindSOPData(node);

                if (m_selectedSOP == null)
                {
                    m_selectedSOP = new SOPData(node);
                    m_dicSOPSections[m_selectedSOP] = new ArrayList();
                }
                else
                    ShowSections(m_dicSOPSections[m_selectedSOP], true);
            
                m_selectedSOP.Depth = nDepth;
                m_selectedSOP.FullName = strSOPFullName;

                m_sectionSelected = null;

                Refresh();
            }

            m_nSelectedDepth = nDepth;
        }

        public void OnChangedSOP(int nDepth, string strSOPFullName, TreeNode node)
        {
            SOPData data = FindSOPData(node);
            if (data == null) return;

            data.Depth = nDepth;
            data.FullName = strSOPFullName;
        }

        // Return값 : 현재 Section View의 노드를 지웠는가 여부
        private bool RemoveSOP(TreeNode node)
        {
            bool currentSectionRemoved = false;
            if (node == null) return currentSectionRemoved;

            foreach (TreeNode child in node.Nodes)
            {
                if (RemoveSOP(child))
                    currentSectionRemoved = true;
            }

            SOPData data = FindSOPData(node);
            if (data == null) return currentSectionRemoved;

            ArrayList arrSections = m_dicSOPSections[data];
            ShowSections(arrSections, false);

            m_dicSOPSections.Remove(data);

            if (m_selectedSOP == data)
            {
                m_selectedSOP = null;
                return true;
            }

            return currentSectionRemoved;
        }

        public void OnRemovedSOP(TreeNode node)
        {
            if (RemoveSOP(node))
                Refresh();
        }

        private string GetTreeNodeFullPath(TreeNode node)
        {
            if (node == null)
                return "";

            string strPath = node.Text;

            while (node.Parent != null)
            {
                node = node.Parent;
                strPath = node.Text + "/" + strPath;
            }

            return strPath;
        }

        private bool SectionValidCheck(SOPData data, ArrayList arrSections)
        {
            foreach (SectionTimeText section in arrSections)
            {
                string strSectionText = Utility.TrimString(section.GetTextBox().Text);
                
                if (strSectionText.Length == 0)
                {
                    string strSOPPath = GetTreeNodeFullPath(data.Node);
                    MessageBox.Show(string.Format("{0}의 프로세스 가운데 Text가 설정되지 않은 프로세스가 존재합니다.\r\nText를 설정해 주시기 바랍니다.", strSOPPath), "저장 오류");
                    return false;
                }

                ArrayList arrChilds = section.GetChildSections();

                foreach (SectionTimeText child in arrChilds)
                {
                    string strSectionText2 = Utility.TrimString(child.GetTextBox().Text);

                    if (strSectionText2.Length == 0)
                    {
                        string strSOPPath = GetTreeNodeFullPath(data.Node) + "/" + strSectionText;
                        MessageBox.Show(string.Format("{0}의 조직 가운데 Text가 설정되지 않은 조직이 존재합니다.\r\nText를 설정해 주시기 바랍니다.", strSOPPath), "저장 오류");
                        return false;
                    }
                }
            }

            return true;
        }

        private bool SectionValidCheck()
        {
            int nCount = m_dicSOPSections.Count();

            for (int i=0;i<nCount;i++)
            {
                KeyValuePair<SOPData, ArrayList> pair = m_dicSOPSections.ElementAt(i);
                
                if (!SectionValidCheck(pair.Key, pair.Value))
                    return false;
            }

            return true;
        }

        private bool SaveCheckTask(int nTaskID, MemberofSection.MissionofSection mission, int transaction)
        {
            string strSQL = "select Max(id) from CheckTask";

            //SqlDataReader reader;
            //m_Main.m_dbMgr.ReadDB(strSQL, transaction, out reader);
            ArrayList arrResult = m_Main.m_dbMgr.GetResultData(strSQL, 0);

            int nLastCheckTaskID = 0;

            for (int i = 0; i < arrResult.Count; i++)
            {
                nLastCheckTaskID = m_Main.m_dbMgr.GetIntField(arrResult[i].ToString(), 0);
            }

            ArrayList arrCheckItems = mission.CheckItems;

            foreach (MemberofSection.CheckofMission checkMission in arrCheckItems)
            {
                if (checkMission.SubCategory == null || checkMission.TaskName == null) continue;

                string strSubCategory = Utility.TrimString(checkMission.SubCategory);
                string strTaskName = Utility.TrimString(checkMission.TaskName);
                if (strSubCategory.Length == 0 || strTaskName.Length == 0) continue;

                string strDescription = checkMission.Description == null ? "" : checkMission.Description;
                string strCount = checkMission.Count == null ? "NULL" : checkMission.Count;
                if (strCount.Length == 0) strCount = "NULL";

                strSQL = string.Format("Insert into CheckTask (ID, TaskID, SubCategory, TaskName, Description, TargetCount, PositionList) values ({0}, {1}, '{2}', '{3}', '{4}', {5}, NULL)",
                    ++nLastCheckTaskID, nTaskID, strSubCategory, strTaskName, strDescription, strCount);
                //m_Main.m_dbMgr.Execute(strSQL, transaction);
                m_Main.m_dbMgr.GetResultData(strSQL, transaction);
            }

            return true;
        }
        
        private bool SaveTask(int nMissionInfoID, SectionTimeText section, int transaction)
        {
            if (section == null)
                return false;

            string strSQL = "select Max(id) from Task";

            //SqlDataReader reader;
            //m_Main.m_dbMgr.ReadDB(strSQL, transaction, out reader);
            ArrayList arrResult = m_Main.m_dbMgr.GetResultData(strSQL, 0);

            int nLastTaskID = 0;

            if (arrResult != null)
            {
                nLastTaskID = m_Main.m_dbMgr.GetIntField(arrResult[0].ToString(), 0);
            }

            MemberofSection data = section.MissionData;

            if (data != null)
            {
                foreach (MemberofSection.MissionofSection mission in data.Missions)
                {
                    strSQL = string.Format("Insert into Task (ID, MissionInfoID, TaskCategory, TaskName, Description) values ({0}, {1}, '{2}', '{3}', '{4}')",
                        ++nLastTaskID, nMissionInfoID, mission.Division, mission.TaskName, mission.Description);
                    //m_Main.m_dbMgr.Execute(strSQL, transaction);
                    m_Main.m_dbMgr.GetResultData(strSQL, transaction);

                    // 추후에 TaskReport(보고 대상) 저장토록 할것

                    if (!SaveCheckTask(nLastTaskID, mission, transaction))
                        return false;
                }
            }

            return true;
        }

        private bool SaveMission(int nStepMemberID, SectionTimeText section, int transaction)
        {
            if (section == null)
                return false;

            string strSQL = "select Max(id) from MissionInfo";

            //SqlDataReader reader;
            //m_Main.m_dbMgr.ReadDB(strSQL, transaction, out reader);
            ArrayList arrResult = m_Main.m_dbMgr.GetResultData(strSQL, 0);

            int nLastMissionID = 0;

            if (arrResult != null)
            {
                nLastMissionID = m_Main.m_dbMgr.GetIntField(arrResult[0].ToString(), 0);
            }

            MemberofSection data = section.MissionData;

            strSQL = string.Format("Insert into MissionInfo (ID, StepMemberID, CellPhoneNumber1, CellPhoneNumber2, CellPhoneNumber3, PhoneNumber1, PhoneNumber2, PhoneNumber3, MessangerID) values ({0}, {1}, '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}')",
                ++nLastMissionID, nStepMemberID, data.CellPhone1, data.CellPhone2, data.CellPhone3, data.Telephone1, data.Telephone2, data.Telephone3, data.MessengerID);
            //m_Main.m_dbMgr.Execute(strSQL, transaction);
            m_Main.m_dbMgr.GetResultData(strSQL, transaction);

            if (!SaveTask(nLastMissionID, section, transaction))
                return false;

            return true;
        }

        private bool SaveStepMembers(int nActionStepID, ArrayList arrSections, int nVersionID, int transaction)
        {
            string strSQL = "select Max(id) from StepMember";

            //SqlDataReader reader;
            //m_Main.m_dbMgr.ReadDB(strSQL, transaction, out reader);
            ArrayList arrResult = m_Main.m_dbMgr.GetResultData(strSQL, 0);

            int nLastStepMemberID = 0;

            if (arrResult != null)
            {
                nLastStepMemberID = m_Main.m_dbMgr.GetIntField(arrResult[0].ToString(), 0);
            }

            foreach (SectionTimeText section in arrSections)
            {
                string strSectionName = section.GetTextBox().Text;
                string strBeginTime = section.GetTimeString(true, true);
                string strProcessTime = section.GetTimeString(false, true);

                int nMemberID = section.Data.ID;
                int nMemberType = section.Data.Type == TeamData.DataType.MemberData ? 3 : 1;
                
                strSQL = string.Format("Insert into StepMember (ID, ActionStepID, MemberID, MemberType, BeginTime, ProcessTime, VersionID) values ({0}, {1}, {2}, {3}, '{4}', '{5}', {6})",
                    ++nLastStepMemberID, nActionStepID, nMemberID, nMemberType, strBeginTime, strProcessTime, nVersionID);
                //m_Main.m_dbMgr.Execute(strSQL, transaction);
                m_Main.m_dbMgr.GetResultData(strSQL, transaction);

                if (!SaveMission(nLastStepMemberID, section, transaction))
                    return false;
            }

            return true;
        }

        // Node 정보가 잘못 입력되어 있을 경우 Node를 바꾸어준다.
        private bool ResetNode(Dictionary<TreeNode, SubDisasterCategoryData> dicSubDisaster, SOPData data)
        {
            try
            {
                if (data.Node.Handle != null)
                    return true;
            }
            catch (System.NullReferenceException)
            {
                // Handle이 0인 경우
            }

            string strFullPath = data.FullName;
            strFullPath = strFullPath.Replace('/', '\\');

            foreach (KeyValuePair<TreeNode, SubDisasterCategoryData> pair in dicSubDisaster)
            {
                if (pair.Key.FullPath == strFullPath)
                {
                    data.Node = pair.Key;
                    return true;
                }
            }

            return false;
        }

        // SOPData 정보가 잘못 입력되어 있을 경우 SOPData를 바꾸어준다.
        private bool ResetNode(Dictionary<SOPData, ArrayList> dicSOPSections, ref SOPData data)
        {
            try
            {
                if (data.Node.Handle != null)
                    return true;
            }
            catch (System.NullReferenceException)
            {
                // Handle이 0인 경우
            }

            foreach (KeyValuePair<SOPData, ArrayList> pair in dicSOPSections)
            {
                if (pair.Key.FullName == data.FullName)
                {
                    data = pair.Key;
                    return true;
                }
            }

            return false;
        }

        private bool SaveActionSteps(Dictionary<TreeNode, SubDisasterCategoryData> dicSubDisaster, SOPData data, ArrayList arrSections, int nVersionID, int transaction)
        {
            // Node 정보가 잘못 입력되어 있을 경우 Node를 바꾸어준다.
            ResetNode(dicSubDisaster, data);

            if (!dicSubDisaster.ContainsKey(data.Node))
                return false;

            int nSubDisasterID = dicSubDisaster[data.Node].ID;

            string strSQL = "select Max(id) from ActionStep";

            //SqlDataReader reader;
            //m_Main.m_dbMgr.ReadDB(strSQL, transaction, out reader);
            ArrayList arrResult = m_Main.m_dbMgr.GetResultData(strSQL, 0);

            int nLastActionStepID = 0;

            if (arrResult != null)
            {
                nLastActionStepID = m_Main.m_dbMgr.GetIntField(arrResult[0].ToString(), 0);
            }

            foreach (SectionTimeText section in arrSections)
            {
                string strSectionName = section.GetTextBox().Text;
                string strBeginTime = section.GetTimeString(true, true);
                string strProcessTime = section.GetTimeString(false, true);

                strSQL = string.Format("Insert into ActionStep (ID, StepName, BeginTime, ProcessTime, SubDisasterID, VersionID) values ({0}, '{1}', '{2}', '{3}', {4}, {5})",
                    ++nLastActionStepID, strSectionName, strBeginTime, strProcessTime, nSubDisasterID, nVersionID);
                //m_Main.m_dbMgr.Execute(strSQL, transaction);
                m_Main.m_dbMgr.GetResultData(strSQL, transaction);

                if (!SaveStepMembers(nLastActionStepID, section.GetChildSections(), nVersionID, transaction))
                    return false;
            }

            return true;
        }

        public bool SaveSectionData(Dictionary<TreeNode, SubDisasterCategoryData> dicSubDisaster, int nVersionID, int transaction)
        {
            // 빈문자열의 Section이 존재하는지 검사한다.
            if (!SectionValidCheck())
                return false;

            int nCount = m_dicSOPSections.Count();

            for (int i = 0; i < nCount; i++)
            {
                KeyValuePair<SOPData, ArrayList> pair = m_dicSOPSections.ElementAt(i);
                if (!SaveActionSteps(dicSubDisaster, pair.Key, pair.Value, nVersionID, transaction))
                    return false;
                /*SOPData data = pair.Key;
                ArrayList arrSections = pair.Value;

                if (!SectionValidCheck(pair.Key, pair.Value))
                    return false;*/
            }

            return true;
        }

        private SectionTimeText AddProcessSection(ArrayList arrSections, string strSectionName, string strBeginTime, string strProcessTime)
        {
            SectionTimeText section = new SectionTimeText(this, /*m_Main.GetPaneLayer().Width + */60, 20);
            section.SetText(strSectionName);

            Size sz = section.RectSize;

            int nHour, nMinute;
            SectionTimeTextBox.TextToTime(strBeginTime, "", out nHour, out nMinute);
            section.SetTime(true, nHour, nMinute);

            SectionTimeTextBox.TextToTime(strProcessTime, "", out nHour, out nMinute);
            section.SetTime(false, nHour, nMinute);

            int nSectionCount = arrSections.Count;

            if (nSectionCount > 0)
            {
                SectionTimeText prev = (SectionTimeText)arrSections[nSectionCount - 1];
                prev.SetNext(section);
            }

            arrSections.Add(section);
            return section;
        }

        private TeamData MakeTeamData(StepMemberData data)
        {
            TeamData teamData;

            if (data.MemberType == 1)       // 상시 조직 팀
                FormTeam.Instance(true).FindItem(data.MemberID, false, out teamData);
            else if (data.MemberType == 2)  // 비상 조직 팀
                FormTeam.Instance(false).FindItem(data.MemberID, false, out teamData);
            else                            // 상시 조직 팀원(비상 조직 팀원은 정의되지 않았음)
                FormTeam.Instance(true).FindItem(data.MemberID, true, out teamData);

            return teamData;
        }

        private SectionTimeText AddGroupSection(SectionTimeText sectionParent, StepMemberData data)
        {
            TeamData teamData = MakeTeamData(data);
            if (teamData == null)
                return null;

            SectionTimeText section = new SectionTimeText(this, 60, 20);
            sectionParent.AddChild(section);

            section.SetText(teamData.Name);

            int nHour, nMinute;
            SectionTimeTextBox.TextToTime(data.BeginTime, "", out nHour, out nMinute);
            section.SetTime(true, nHour, nMinute);

            SectionTimeTextBox.TextToTime(data.ProcessTime, "", out nHour, out nMinute);
            section.SetTime(false, nHour, nMinute);

            section.Data = teamData;
            section.MissionData = new MemberofSection();
            section.MissionData.Member = section.GetTextBox().Text;

            return section;
        }

        private bool LoadTask(MemberofSection mission, ArrayList arrTasks)
        {
            // taskID, TaskData
            Dictionary<int, MemberofSection.MissionofSection> dicTask = new Dictionary<int, MemberofSection.MissionofSection>();
            string strCondition = "(";

            int nTaskCount = arrTasks.Count;
            if (nTaskCount == 0) return true;

            for (int i=0;i<nTaskCount;i++)
            {
                TaskData task = (TaskData)arrTasks[i];
                MemberofSection.MissionofSection missionTask = new MemberofSection.MissionofSection();

                missionTask.Division = task.TaskCategory;
                missionTask.TaskName = task.TaskName;
                missionTask.Report = "";

                mission.Missions.Add(missionTask);

                dicTask[task.ID] = missionTask;

                if (i == 0)
                    strCondition += task.ID.ToString();
                else
                    strCondition += "," + task.ID.ToString();

                if (i == nTaskCount - 1)
                    strCondition += ")";
            }

            string strSQL = "select * from CheckTask where TaskID in " + strCondition + " order by TaskID";

            //SqlDataReader reader;
            //m_Main.m_dbMgr.ReadDB(strSQL, null, out reader);
            ArrayList arrResult = m_Main.m_dbMgr.GetResultData(strSQL, 0);

            for (int i = 0; i < arrResult.Count - 6; i = i+7)
            {
                int nID = m_Main.m_dbMgr.GetIntField(arrResult[i].ToString(), 0);
                int nTaskID = m_Main.m_dbMgr.GetIntField(arrResult[i + 1].ToString(), 0);
                string strSubCategpry = m_Main.m_dbMgr.GetStringField(arrResult[i + 2].ToString(), "");
                string strTaskName = m_Main.m_dbMgr.GetStringField(arrResult[i + 3].ToString(), "");
                string strDescription = m_Main.m_dbMgr.GetStringField(arrResult[i + 4].ToString(), "");
                int nTargetCount = m_Main.m_dbMgr.GetIntField(arrResult[i + 5].ToString(), -1);
                string strPositionList = m_Main.m_dbMgr.GetStringField(arrResult[i + 6].ToString(), "");

                if (!dicTask.ContainsKey(nTaskID))
                    continue;

                MemberofSection.MissionofSection missionTask = dicTask[nTaskID];

                MemberofSection.CheckofMission checkTask = new MemberofSection.CheckofMission();

                checkTask.Category = missionTask.Division;
                checkTask.SubCategory = strSubCategpry;
                checkTask.TaskName = strTaskName;
                checkTask.Count = nTargetCount >= 0 ? nTargetCount.ToString() : "";

                missionTask.CheckItems.Add(checkTask);
            }

            return true;
        }

        private bool LoadMission(Dictionary<SectionTimeText, StepMemberData> dicStepMember)
        {
            foreach (KeyValuePair<SectionTimeText, StepMemberData> pair in dicStepMember)
            {
                SectionTimeText section = pair.Key;
                StepMemberData data = pair.Value;

                MemberofSection missionData = section.MissionData;
                if (missionData == null) continue;

                missionData.Member = section.GetTextBox().Text;

                string strSQL = "select * from MissionInfo where StepMemberID = " + data.ID.ToString();

                //SqlDataReader reader;
                //m_Main.m_dbMgr.ReadDB(strSQL, null, out reader);
                ArrayList arrResult = m_Main.m_dbMgr.GetResultData(strSQL, 0);

                int nMissionInfoID = 0;

                // StepMember와 MissionInfo는 1:1 관계

                if (arrResult != null)//(reader.Read())
                {
                    nMissionInfoID = m_Main.m_dbMgr.GetIntField(arrResult[0].ToString(), 0);

                    missionData.CellPhone1 = m_Main.m_dbMgr.GetStringField(arrResult[2].ToString(), "");
                    missionData.CellPhone2 = m_Main.m_dbMgr.GetStringField(arrResult[3].ToString(), "");
                    missionData.CellPhone3 = m_Main.m_dbMgr.GetStringField(arrResult[4].ToString(), "");
                    missionData.Telephone1 = m_Main.m_dbMgr.GetStringField(arrResult[5].ToString(), "");
                    missionData.Telephone2 = m_Main.m_dbMgr.GetStringField(arrResult[6].ToString(), "");
                    missionData.Telephone3 = m_Main.m_dbMgr.GetStringField(arrResult[7].ToString(), "");
                    missionData.MessengerID = m_Main.m_dbMgr.GetStringField(arrResult[8].ToString(), "");
                }
                else
                {
                    continue;
                }

                strSQL = "select * from Task where MissionInfoID = " + nMissionInfoID.ToString();
                //m_Main.m_dbMgr.ReadDB(strSQL, null, out reader);
                arrResult = m_Main.m_dbMgr.GetResultData(strSQL, 0);

                ArrayList arrTasks = new ArrayList();

                //while (reader.Read())
                for (int i = 0; i < arrResult.Count - 4; i=i+5) 
                {
                    int nID = m_Main.m_dbMgr.GetIntField(arrResult[i].ToString(), 0);
                    string strTaskCategory = m_Main.m_dbMgr.GetStringField(arrResult[i + 2].ToString(), "");
                    string strTaskName = m_Main.m_dbMgr.GetStringField(arrResult[i + 3].ToString(), "");
                    string strDescription = m_Main.m_dbMgr.GetStringField(arrResult[i + 4].ToString(), "");

                    TaskData task = new TaskData(nID, nMissionInfoID, strTaskCategory, strTaskName, strDescription);
                    arrTasks.Add(task);
                }

                if (!LoadTask(missionData, arrTasks))
                    return false;
            }

            return true;
        }

        private bool LoadStepMember(Dictionary<int, SectionTimeText> dicActionStep, int nVersionID)
        {
            string strSQL = "select * from StepMember where VersionID = " + nVersionID.ToString();

            //SqlDataReader reader;
            //m_Main.m_dbMgr.ReadDB(strSQL, null, out reader);
            ArrayList arrResult = m_Main.m_dbMgr.GetResultData(strSQL, 0);

            ArrayList arrStepMember = new ArrayList();

            for (int i = 0; i < arrResult.Count - 6; i = i + 7)
            {
                int nID = m_Main.m_dbMgr.GetIntField(arrResult[i].ToString(), 0);
                int nActionStepID = m_Main.m_dbMgr.GetIntField(arrResult[i + 1].ToString(), 0);
                int nMemberID = m_Main.m_dbMgr.GetIntField(arrResult[i + 2].ToString(), 0);
                int nMemberType = m_Main.m_dbMgr.GetIntField(arrResult[i + 3].ToString(), 0);
                string strBeginTime = m_Main.m_dbMgr.GetStringField(arrResult[i + 4].ToString(), "");
                string strProcessTime = m_Main.m_dbMgr.GetStringField(arrResult[i + 5].ToString(), "");

                StepMemberData data = new StepMemberData(nID, nActionStepID, nMemberID, nMemberType, strBeginTime, strProcessTime);
                arrStepMember.Add(data);
            }

            Dictionary<SectionTimeText, StepMemberData> dicStepMember = new Dictionary<SectionTimeText, StepMemberData>();

            foreach (StepMemberData data in arrStepMember)
            {
                if (!dicActionStep.ContainsKey(data.ActionStepID))
                    return false;

                SectionTimeText section = AddGroupSection(dicActionStep[data.ActionStepID], data);

                if (section == null)
                    return false;

                dicStepMember[section] = data;
            }

            if (!LoadMission(dicStepMember))
                return false;
            
            return true;
        }

        public void AfterLoadSOP()
        {
            m_selectedSOP = null;

            // 모든 Section들을 보이지 않게 한다.
            foreach (KeyValuePair<SOPData, ArrayList> sop in m_dicSOPSections)
            {
                if (sop.Value.Count > 0)
                {
                    m_selectedSOP = sop.Key;
                    AutoAlign();
                    ShowSections(sop.Value, false);
                }
            }

            // 선택된 Section들만 보이도록 한다.
            if (m_selectedSOP != null)
            {
                ShowSections(m_dicSOPSections[m_selectedSOP], true);
                m_Main.GetPaneLayer().SelectItem(m_selectedSOP.Node);
            }
        }

        private void SaveTempSOP(ArrayList arrTemp)
        {
            foreach (KeyValuePair<SOPData, ArrayList> pair in m_dicSOPSections)
            {
                arrTemp.Add(pair);
            }

            NewSOP();
        }

        private void RollbackSOP(ArrayList arrTemp)
        {
            m_dicSOPSections.Clear();

            foreach (KeyValuePair<SOPData, ArrayList> pair in arrTemp)
            {
                m_dicSOPSections.Add(pair.Key, pair.Value);
            }

            arrTemp.Clear();
        }

        public bool LoadActionStep(int nVersionID, Dictionary<TreeNode, SubDisasterCategoryData> dicSubDisaster, Dictionary<int, TreeNode> dicSubNode)
        {
            ArrayList arrTemp = new ArrayList();
            SaveTempSOP(arrTemp);

            string strSQL = "select * from ActionStep where VersionID = " + nVersionID.ToString();

            //SqlDataReader reader;
            //m_Main.m_dbMgr.ReadDB(strSQL, null, out reader);
            ArrayList arrResult = m_Main.m_dbMgr.GetResultData(strSQL, 0);

            Dictionary<int, SectionTimeText> dicActionStep = new Dictionary<int, SectionTimeText>();

            for (int i = 0; i < arrResult.Count - 5; i = i + 6)
            {
                int nID = m_Main.m_dbMgr.GetIntField(arrResult[i].ToString(), 0);
                string strActionStepName = m_Main.m_dbMgr.GetStringField(arrResult[i + 1].ToString(), "");
                string strBeginTime = m_Main.m_dbMgr.GetStringField(arrResult[i + 2].ToString(), "");
                string strProcessTime = m_Main.m_dbMgr.GetStringField(arrResult[i + 3].ToString(), "");
                int nSubDisasterID = m_Main.m_dbMgr.GetIntField(arrResult[i + 4].ToString(), 0);

                if (!dicSubNode.ContainsKey(nSubDisasterID))
                {
                    RollbackSOP(arrTemp);
                    return false;
                }

                TreeNode node = dicSubNode[nSubDisasterID];
                SOPData data = FindSOPData(node);
                ArrayList arrSections;

                if (data == null)
                {
                    string strFullPath;
                    int nDepth = m_Main.GetPaneLayer().GetNodeText(node, out strFullPath);

                    data = new SOPData(nDepth, strFullPath, node);

                    arrSections = new ArrayList();
                    m_dicSOPSections[data] = arrSections;
                }
                else
                    arrSections = m_dicSOPSections[data];

                SectionTimeText section = AddProcessSection(arrSections, strActionStepName, strBeginTime, strProcessTime);

                if (section == null)
                {
                    RollbackSOP(arrTemp);
                    return false;
                }

                dicActionStep[nID] = section;
            }

            if (!LoadStepMember(dicActionStep, nVersionID))
            {
                RollbackSOP(arrTemp);
                return false;
            }

            return true;
        }

        public void NewSOP()
        {
            m_selectedSOP = null;
            m_sectionSelected = null;
            m_nSelectedDepth = -1;

            foreach (KeyValuePair<SOPData, ArrayList> pair in m_dicSOPSections)
            {
                ArrayList arrSections = pair.Value;

                foreach (SectionTimeText section in arrSections)
                {
                    ArrayList arrChilds = section.GetChildSections();

                    foreach (SectionTimeText child in arrChilds)
                    {
                        child.Hide();
                    }

                    section.Hide();
                }
            }

            m_dicSOPSections.Clear();
        }
    }

    public class SOPData
    {
        protected int m_nDepth;
        protected string m_strFullName;
        protected string m_strName;
        protected TreeNode m_node;

        public SOPData(TreeNode node)
        {
            m_nDepth = -1;
            m_strName = m_strFullName = "";
            m_node = node;
        }

        public SOPData(int nDepth, string strFullName, TreeNode node)
        {
            Depth = nDepth;
            FullName = strFullName;
            m_node = node;
        }

        /*public override bool Equals(object obj)
        {
            if (obj.GetType().Name != "SOPData")
                return false;

            SOPData data = (SOPData)obj;

            if (m_nDepth != data.m_nDepth) return false;
            return m_strFullName == data.m_strFullName;
        }

        public static bool operator ==(SOPData data1, SOPData data2)
        {
            if (data1.m_nDepth != data2.m_nDepth) return false;
            return data1.m_strFullName == data2.m_strFullName;
        }

        public static bool operator !=(SOPData data1, SOPData data2)
        {
            return !(data1 == data2);
        }*/

        public int Depth
        {
            get { return m_nDepth; }
            set { m_nDepth = value; }
        }

        public string FullName
        {
            get { return m_strFullName; }
            set
            {
                int nIndex = value.LastIndexOf('/');

                if (nIndex >= 0)
                    m_strName = value.Substring(nIndex + 1);
                else
                    m_strName = value;

                m_strFullName = value;
            }
        }

        public string Name
        {
            get { return m_strName; }
        }

        public TreeNode Node
        {
            get { return m_node; }
            set { m_node = value; }
        }
    }

    class StepMemberData
    {
        private int m_nID = -1;
        private int m_nActionStepID = -1;
        private int m_nMemberID = -1;
        private int m_nMemberType = -1;
        private string m_strBeginTime = "";
        private string m_strProcessTime = "";

        public StepMemberData()
        {
        }

        public StepMemberData(int nID, int nActionStepID, int nMemberID, int nMemberType, string strBeginTime, string strProcessTime)
        {
            m_nID = nID;
            m_nActionStepID = nActionStepID;
            m_nMemberID = nMemberID;
            m_nMemberType = nMemberType;
            m_strBeginTime = strBeginTime;
            m_strProcessTime = strProcessTime;
        }

        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }

        public int ActionStepID
        {
            get { return m_nActionStepID; }
            set { m_nActionStepID = value; }
        }

        public int MemberID
        {
            get { return m_nMemberID; }
            set { m_nMemberID = value; }
        }

        public int MemberType
        {
            get { return m_nMemberType; }
            set { m_nMemberType = value; }
        }

        public string BeginTime
        {
            get { return m_strBeginTime; }
            set { m_strBeginTime = value; }
        }

        public string ProcessTime
        {
            get { return m_strProcessTime; }
            set { m_strProcessTime = value; }
        }
    }

    class TaskData
    {
        private int m_nID = -1;
        private int m_nMissionInfoID = -1;
        private string m_strTaskCategory = "";
        private string m_strTaskName = "";
        private string m_strDescription = "";

        public TaskData()
        {
        }

        public TaskData(int nID, int nMissionInfoID, string strTaskCategory, string strTaskName, string strDescription)
        {
            m_nID = nID;
            m_nMissionInfoID = nMissionInfoID;
            m_strTaskCategory = strTaskCategory;
            m_strTaskName = strTaskName;
            m_strDescription = strDescription;
        }

        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }

        public int MissionInfoID
        {
            get { return m_nMissionInfoID; }
            set { m_nMissionInfoID = value; }
        }

        public string TaskCategory
        {
            get { return m_strTaskCategory; }
            set { m_strTaskCategory = value; }
        }

        public string TaskName
        {
            get { return m_strTaskName; }
            set { m_strTaskName = value; }
        }

        public string Description
        {
            get { return m_strDescription; }
            set { m_strDescription = value; }
        }
    }
}
