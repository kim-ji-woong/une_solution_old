using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;

namespace SOPMonitoringSystem
{
    public partial class FormMissionStatus : Form
    {
        public enum ItemType { PREV_ITEM = 0, CURRENT_ITEM, NEXT_ITEM };

        private Sections.WorkFlow m_workFlowCurrent = null;
        private Sections.SectionState m_statePrev = null;
        private Sections.SectionState m_stateCurrent = null;
        private Sections.SectionState m_stateNext = null;

        private ArrayList m_arrPrev = new ArrayList();
        private ArrayList m_arrCurrent = new ArrayList();
        private ArrayList m_arrNext = new ArrayList();

        private Color m_colEvenLeft = Color.FromArgb(242, 242, 242);
        private Color m_colOddLeft = Color.FromArgb(232, 232, 232);
        private Color m_colEvenRight = Color.White;
        private Color m_colOddRight = Color.FromArgb(232, 232, 232);

        private Font m_fontGrid = null;
        //private float m_fFontHeight = 20.0f;

        private int m_nDefaultGridRowHeight = 64;

        /*private PointF[] m_arrPrevPolygon = new PointF[4];
        private PointF[] m_arrNextPolygon = new PointF[4];
        private PointF[] m_arrCurrentPolygon = new PointF[4];*/
        private ShadowControl m_shadowPrev = new ShadowControl();
        private ShadowControl m_shadowNext = new ShadowControl();
        private ShadowControl m_shadowCurrent = new ShadowControl();
        private int m_nShadowMoveX = 6;
        private int m_nShadowMoveY = 5;//

        public FormMissionStatus()
        {
            InitializeComponent();

            //FontFamily fontFamily = GetFontFamily("맑은고딕", "굴림체", "돋움체");
            //m_fontGrid = new Font(fontFamily, m_fFontHeight);
            m_fontGrid = labelTitle.Font;
        }

        // 우선순위 1, 2, 3
        private FontFamily GetFontFamily(string strFontName1, string strFontName2, string strFontName3)
        {
            FontFamily font2 = null;
            FontFamily font3 = null;

            FontFamily[] arrFamilies = FontFamily.Families;

            foreach (FontFamily family in arrFamilies)
            {
                if (family.Name == strFontName1)
                    return family;
                else if (family.Name == strFontName2)
                    font2 = family;
                else if (family.Name == strFontName3)
                    font3 = family;
            }

            if (font2 != null)
                return font2;
            else if (font3 != null)
                return font3;

            // Default FontFamily
            DataGridViewTextBoxCell cell = new DataGridViewTextBoxCell();
            return cell.Style.Font.FontFamily;
        }

        private void FormMissionStatus_Load(object sender, EventArgs e)
        {
            /*colPrevOption.CellTemplate.Value = null;
            colPrevOption.CellTemplate.Style.NullValue = null;
            colPrevOption.Icon = null;
            colPrevOption.Image = null;*/

            labelTitle.Parent = pictureBoxTitlebar;
            labelTitle.BackColor = Color.Transparent;
            labelTitle.Location = new Point(20, (labelTitle.Parent.Size.Height - labelTitle.Size.Height) / 2 - 3);

            pictureBoxTitle1Name.Parent = pictureBoxTitle1BG;
            pictureBoxTitle1Name.BackColor = Color.Transparent;

            pictureBoxLogo.Parent = pictureBoxTitle1BG;
            pictureBoxLogo.BackColor = Color.Transparent;

            m_shadowPrev.Parent = this;
            m_shadowNext.Parent = this;
            m_shadowCurrent.Parent = this;

            Reshape();
        }

        // 전체 화면
        public void ShowMaximize()
        {
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.WindowState = FormWindowState.Maximized;

            Reshape();
        }

        public void ShowNormal()
        {
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;
            this.WindowState = FormWindowState.Normal;

            Reshape();
        }

        private void Reshape()
        {
            pictureBoxTitle1BG.Location = new Point(0, 0);
            pictureBoxTitle1BG.Size = new Size(this.Size.Width, pictureBoxTitle1BG.Size.Height);
            pictureBoxTitle1Name.Location = new Point(100, 0);

            pictureBoxLogo.Location = new Point(pictureBoxTitle1BG.Size.Width - pictureBoxLogo.Size.Width, (pictureBoxTitle1BG.Size.Height - pictureBoxLogo.Size.Height) / 2);
            pictureBoxTitlebar.Location = new Point(0, pictureBoxTitle1BG.Location.Y + pictureBoxTitle1BG.Size.Height);
            pictureBoxTitlebar.Size = new Size(this.Size.Width + 20, pictureBoxTitlebar.Size.Height);

            int nSpaceX = 20, nSpaceY = 20;
            int nMiddleSpaceX = 20, nMiddleSpaceY = 20;
            int nGridTitleHeight = pictureBoxCurrentHeader.Size.Height;
            int nGridWidth = (this.Size.Width - nSpaceX * 2 - nMiddleSpaceX) / 2;
            int nGridHeight1 = this.Size.Height - pictureBoxTitlebar.Location.Y - pictureBoxTitlebar.Size.Height - nSpaceY * 2 - nGridTitleHeight;
            int nGridHeight2 = (nGridHeight1 + nGridTitleHeight - nMiddleSpaceY) / 2 - nGridTitleHeight;

            dataGridViewPrev.Location = new Point(nSpaceX, pictureBoxTitlebar.Location.Y + pictureBoxTitlebar.Size.Height + nSpaceY + nGridTitleHeight);
            dataGridViewPrev.Size = new Size(nGridWidth * 2 / 3, nGridHeight2);

            dataGridViewNext.Location = new Point(nSpaceX, dataGridViewPrev.Location.Y + dataGridViewPrev.Size.Height + nMiddleSpaceY + nGridTitleHeight);
            dataGridViewNext.Size = new Size(nGridWidth * 2 / 3, nGridHeight2);

            dataGridViewCurrent.Location = new Point(this.Size.Width - nSpaceX - nGridWidth * 4 / 3, dataGridViewPrev.Location.Y);
            dataGridViewCurrent.Size = new Size(nGridWidth * 4 / 3, nGridHeight1);

            ReshapeGridTitle(pictureBoxPrevHeader, pictureBoxPrevBody, pictureBoxPrevTail, dataGridViewPrev);
            ReshapeGridTitle(pictureBoxNextHeader, pictureBoxNextBody, pictureBoxNextTail, dataGridViewNext);
            ReshapeGridTitle(pictureBoxCurrentHeader, pictureBoxCurrentBody, pictureBoxCurrentTail, dataGridViewCurrent);

            ResizeGrid(dataGridViewPrev, true);
            ResizeGrid(dataGridViewNext, true);
            ResizeGrid(dataGridViewCurrent, false);

            ResizeShadowPolygon(dataGridViewPrev, pictureBoxPrevHeader, m_shadowPrev);
            ResizeShadowPolygon(dataGridViewNext, pictureBoxNextHeader, m_shadowNext);
            ResizeShadowPolygon(dataGridViewCurrent, pictureBoxCurrentHeader, m_shadowCurrent);
        }

        private void ResizeShadowPolygon(DataGridView grid, PictureBox pictureBox, ShadowControl shadow)
        {
            Point ptTL = new Point(pictureBox.Location.X + m_nShadowMoveX, pictureBox.Location.Y + m_nShadowMoveY);
            Size size = new Size(grid.Size.Width, pictureBox.Size.Height + grid.Size.Height);
            shadow.SetBoundary(ptTL, size);
        }

        private void ResizeGrid(DataGridView grid, bool isLeftGrid)
        {
            int nGridRowHeight = m_nDefaultGridRowHeight;

            int nCapacity = grid.Size.Height / nGridRowHeight;
            int nRowCount = grid.Rows.Count;

            if (nRowCount >= nCapacity)
                return;

            for (int i = nRowCount; i < nCapacity; i++)
            {
                AddNewLine(i, grid, isLeftGrid);
            }
        }

        private void AddNewLine(int nRowIndex, DataGridView grid, bool isLeftGrid, bool addBack = true)
        {
            int nGridRowHeight = m_nDefaultGridRowHeight;

            DataGridViewRow row = new DataGridViewRow();
            row.Height = nGridRowHeight;

            if (isLeftGrid)
            {
                if (nRowIndex % 2 == 0)
                    row.DefaultCellStyle.BackColor = m_colEvenLeft;
                else
                    row.DefaultCellStyle.BackColor = m_colOddLeft;
            }
            else
            {
                if (nRowIndex % 2 == 0)
                    row.DefaultCellStyle.BackColor = m_colEvenRight;
                else
                    row.DefaultCellStyle.BackColor = m_colOddRight;
            }

            DataGridViewImageCellBlank cell1 = new DataGridViewImageCellBlank();
            cell1.Value = Properties.Resources.none;
            row.Cells.Add(cell1);

            DataGridViewImageCellBlank cell2 = new DataGridViewImageCellBlank();
            cell2.Value = Properties.Resources.none;
            row.Cells.Add(cell2);

            DataGridViewTextBoxCell cell3 = new DataGridViewTextBoxCell();
            row.Cells.Add(cell3);
            cell3.ReadOnly = true;

            DataGridViewTextBoxCell cell4 = new DataGridViewTextBoxCell();
            row.Cells.Add(cell4);
            cell4.ReadOnly = true;

            if (!isLeftGrid)
            {
                DataGridViewTextBoxCell cell5 = new DataGridViewTextBoxCell();
                row.Cells.Add(cell5);
                cell5.ReadOnly = true;
            }

            if (addBack)
                grid.Rows.Add(row);
            else
                grid.Rows.Insert(0, row);
        }

        private void ReshapeGridTitle(PictureBox header, PictureBox body, PictureBox tail, DataGridView grid)
        {
            Point ptGrid = grid.Location;
            Size szGrid = grid.Size;

            header.Location = new Point(ptGrid.X, ptGrid.Y - header.Size.Height);
            tail.Location = new Point(ptGrid.X + szGrid.Width - tail.Size.Width, header.Location.Y);
            body.Location = new Point(header.Location.X + header.Size.Width, header.Location.Y);

            // 이미지 늘이기
            body.Size = new Size(tail.Location.X - body.Location.X + 20, header.Size.Height);
        }

        private void FormMissionStatus_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                if (this.FormBorderStyle != System.Windows.Forms.FormBorderStyle.Sizable)
                {
                    ShowNormal();
                }
            }
            else if (e.KeyCode == Keys.F2)
            {
                if (this.FormBorderStyle != System.Windows.Forms.FormBorderStyle.None)
                {
                    ShowMaximize();
                }
            }
        }

        // section이 null이면 해당 Grid를 초기화
        public void SetSectionContents(Sections.Section section, ItemType type)
        {
            if (type == ItemType.PREV_ITEM)
                SetSectionContents(dataGridViewPrev, m_arrPrev, colPrevOption, colPrevOption2, true, section);
            else if (type == ItemType.CURRENT_ITEM)
                SetSectionContents(dataGridViewCurrent, m_arrCurrent, colCurrentOption, colCurrentOption2, false, section);
            else if (type == ItemType.NEXT_ITEM)
                SetSectionContents(dataGridViewNext, m_arrNext, colNextOption, colNextOption2, true, section);
        }

        /*private ArrayList GetSectionItems(Sections.Section section)
        {
            ArrayList arrItems = new ArrayList();
            Sections.Section.ComponentType type = section.GetComponentType();

            if (type == Sections.Section.ComponentType.PROCESS)
            {
                Sections.SectionDataProcess data = (Sections.SectionDataProcess)section.Data;

                int nCheckedNotify1, nCheckedNotify2;
                Sections.WorkFlow.GetProcessCheckedNotify((Sections.SectionProcess)section, out nCheckedNotify1, out nCheckedNotify2);

                int nMissionCount = data.MissionItems.Count;

                for (int i=0;i<nMissionCount;i++)
                {
                    Sections.MissionItem missionItem = (Sections.MissionItem)data.MissionItems[i];
                    GridItem gridItem = new GridItem();

                    int nBitFlag = (1 << i);

                    gridItem.UseSMS = (nCheckedNotify1 & nBitFlag) == nBitFlag;
                    gridItem.UseBroadcast = (nCheckedNotify2 & nBitFlag) == nBitFlag;
                    gridItem.Mission = missionItem.Mission;
                    gridItem.ComponentType = type;

                    arrItems.Add(gridItem);
                }
            }
            else if (type == Sections.Section.ComponentType.INTERNAL)
            {
                int nCheckedNotify1;
                Sections.WorkFlow.GetInternalCheckedNotify((Sections.SectionInternal)section, out nCheckedNotify1);

                if ((nCheckedNotify1 & 1) == 1)
                {
                    GridItem gridItem = new GridItem();
                    gridItem.Mission = "팝업 메시지 사용";
                    gridItem.ComponentType = type;
                    arrItems.Add(gridItem);
                }

                if ((nCheckedNotify1 & 2) == 2)
                {
                    GridItem gridItem = new GridItem();
                    gridItem.Mission = "문자 메시지 사용";
                    gridItem.ComponentType = type;
                    arrItems.Add(gridItem);
                }

                if ((nCheckedNotify1 & 4) == 4)
                {
                    GridItem gridItem = new GridItem();
                    gridItem.Mission = "사내방송 사용";
                    gridItem.ComponentType = type;
                    arrItems.Add(gridItem);
                }
            }
            else if (type == Sections.Section.ComponentType.EXTERNAL)
            {
                int nCheckedNotify1, nCheckedNotify2;
                Sections.WorkFlow.GetExternalCheckedNotify((Sections.SectionExternal)section, out nCheckedNotify1, out nCheckedNotify2);
            }
        }*/

        private void SetSectionContents(DataGridView grid, ArrayList arrData, DataGridViewColumn colOption1, DataGridViewColumn colOption2, bool isLeftGrid, Sections.Section section)
        {
            if (section == null)
            {
                ClearGrid(grid, arrData, isLeftGrid);
                return;
            }

            int nCheckedNotify1 = 0, nCheckedNotify2 = 0;
            Sections.Section.ComponentType type = section.GetComponentType();

            if (type == Sections.Section.ComponentType.PROCESS)
                Sections.WorkFlow.GetProcessCheckedNotify((Sections.SectionProcess)section, out nCheckedNotify1, out nCheckedNotify2);
            else if (type == Sections.Section.ComponentType.INTERNAL)
                Sections.WorkFlow.GetInternalCheckedNotify((Sections.SectionInternal)section, out nCheckedNotify1);
            else if (type == Sections.Section.ComponentType.EXTERNAL)
                Sections.WorkFlow.GetExternalCheckedNotify((Sections.SectionExternal)section, out nCheckedNotify1, out nCheckedNotify2);
            else if (type == Sections.Section.ComponentType.TRANSMISSION)
                Sections.WorkFlow.GetTransmissionCheckedNotify((Sections.SectionTransmission)section, out nCheckedNotify1, out nCheckedNotify2);

            ComponentContents contents = new ComponentContents();
            PageBackstageHome.MakeComponentContentsData(contents, section.Title, DateTime.Now, "입력 대기", section, Sections.State.INPUT, nCheckedNotify1, nCheckedNotify2);

            SetContents(grid, arrData, colOption1, colOption2, isLeftGrid, contents);
        }

        // contents가 null이면 해당 Grid를 초기화
        public void SetContents(ComponentContents contents, ItemType type)
        {
            if (type == ItemType.PREV_ITEM)
                SetContents(dataGridViewPrev, m_arrPrev, colPrevOption, colPrevOption2, true, contents);
            else if (type == ItemType.CURRENT_ITEM)
                SetContents(dataGridViewCurrent, m_arrCurrent, colCurrentOption, colCurrentOption2, false, contents);
            else if (type == ItemType.NEXT_ITEM)
                SetContents(dataGridViewNext, m_arrNext, colNextOption, colNextOption2, true, contents);
        }

        private void SetDecisionContents(DataGridView grid, Sections.Section section, int nRowCount, bool isLeftGrid)
        {
            int nCellIndex = isLeftGrid ? 2 : 3;

            grid.Columns[0].Visible = false;
            grid.Columns[1].Visible = false;
            
            if (nCellIndex == 3)
                grid.Columns[2].Visible = false;

            if (nRowCount == 0)
                AddNewLine(0, grid, isLeftGrid);

            DataGridViewRow row = grid.Rows[0];

            DataGridViewCell cell1 = row.Cells[nCellIndex];
            DataGridViewCell cell2 = row.Cells[nCellIndex + 1];

            cell1.Value = "판단";
            cell2.Value = section.Title;

            cell1.Style.Font = m_fontGrid;
            cell2.Style.Font = m_fontGrid;

            if (isLeftGrid)
            {
                cell1.Style.ForeColor = Color.Gray;
                cell2.Style.ForeColor = Color.Gray;
            }

            // MultiLine Text Option
            cell1.Style.WrapMode = DataGridViewTriState.True;
            cell2.Style.WrapMode = DataGridViewTriState.True;

            Size szPrefer1 = cell1.PreferredSize;
            Size szPrefer2 = cell2.PreferredSize;
            int nBigHeight = szPrefer1.Height > szPrefer2.Height ? szPrefer1.Height : szPrefer2.Height;

            /*if (row.Height < nBigHeight)
                row.Height = nBigHeight;*/
            //row.Height = grid.Size.Height;

            grid.AutoResizeRow(0, DataGridViewAutoSizeRowMode.AllCellsExceptHeader);

            if (grid.Rows[0].Height < m_nDefaultGridRowHeight)
                grid.Rows[0].Height = m_nDefaultGridRowHeight;
        }

        private void SetContents(DataGridView grid, ArrayList arrData, DataGridViewColumn colOption1, DataGridViewColumn colOption2, bool isLeftGrid, ComponentContents contents)
        {
            ClearGrid(grid, arrData, isLeftGrid);
            if (contents == null)
                return;

            SelectFirstRow(grid);

            grid.Tag = contents;

            bool useBroadcast = contents.UseBroadcast;
            bool useSMS = contents.UseSMS;

            int nRowCount = grid.Rows.Count;
            int nItemCount = contents.ItemCount;
            bool check1, check2;
            string strItem, strMessenger, strTeamName;
            Sections.Section section = contents.Section;

            if (section != null && section.GetComponentType() == Sections.Section.ComponentType.DECISION)
            {
                SetDecisionContents(grid, section, nRowCount, isLeftGrid);
                return;
            }
            else
                grid.Columns[2].Visible = true;

            for (int i = 0; i < nItemCount; i++)
            {
                if (contents.GetItem(i, out check1, out check2, out strMessenger, out strTeamName, out strItem))
                {
                    bool isNewRow = i >= nRowCount;
                    DataGridViewRow row = !isNewRow ? grid.Rows[i] : new DataGridViewRow();

                    if (useSMS && useBroadcast)
                    {
                        if (colOption1 != null && colOption2 != null)
                        {
                            colOption1.Visible = true;
                            colOption2.Visible = true;
                        }

                        DataGridViewImageCell cell1 = !isNewRow ? (DataGridViewImageCell)row.Cells[0] : new DataGridViewImageCell();

                        if (isLeftGrid)
                            cell1.Value = check1 ? Properties.Resources.sms_done : Properties.Resources.sms_off;
                        else
                            cell1.Value = check1 ? Properties.Resources.sms_on : Properties.Resources.sms_off;

                        if (isNewRow)
                            row.Cells.Add(cell1);

                        DataGridViewImageCell cell2 = !isNewRow ? (DataGridViewImageCell)row.Cells[1] : new DataGridViewImageCell();

                        if (isLeftGrid)
                            cell2.Value = check2 ? Properties.Resources.broadcast_done : Properties.Resources.broadcast_off;
                        else
                            cell2.Value = check2 ? Properties.Resources.broadcast_on : Properties.Resources.broadcast_off;

                        if (isNewRow)
                            row.Cells.Add(cell2);
                    }
                    else if (useSMS)
                    {
                        if (colOption1 != null && colOption2 != null)
                        {
                            colOption1.Visible = true;
                            colOption2.Visible = false;
                        }

                        DataGridViewImageCell cell1 = !isNewRow ? (DataGridViewImageCell)row.Cells[0] : new DataGridViewImageCell();

                        if (isLeftGrid)
                            cell1.Value = check1 ? Properties.Resources.sms_done : Properties.Resources.sms_off;
                        else
                            cell1.Value = check1 ? Properties.Resources.sms_on : Properties.Resources.sms_off;
                        
                        if (isNewRow)
                        {
                            row.Cells.Add(cell1);
                            row.Cells.Add(new DataGridViewImageCell());
                        }
                    }
                    else if (useBroadcast)
                    {
                        if (colOption1 != null && colOption2 != null)
                        {
                            colOption1.Visible = false;
                            colOption2.Visible = true;
                        }

                        if (isNewRow)
                        {
                            row.Cells.Add(new DataGridViewImageCell());
                        }

                        DataGridViewImageCell cell2 = !isNewRow ? (DataGridViewImageCell)row.Cells[1] : new DataGridViewImageCell();

                        if (isLeftGrid)
                            cell2.Value = check2 ? Properties.Resources.broadcast_done : Properties.Resources.broadcast_off;
                        else
                            cell2.Value = check2 ? Properties.Resources.broadcast_on : Properties.Resources.broadcast_off;

                        if (isNewRow)
                            row.Cells.Add(cell2);
                    }
                    else
                    {
                        if (colOption1 != null && colOption2 != null)
                        {
                            colOption1.Visible = false;
                            colOption2.Visible = false;
                        }

                        if (isNewRow)
                        {
                            row.Cells.Add(new DataGridViewImageCell());
                            row.Cells.Add(new DataGridViewImageCell());
                        }
                    }

                    string strMission = SetTeamText(section, isNewRow, row, strItem, strTeamName, isLeftGrid);
                    int nMissionIndex = 3;

                    if (!isLeftGrid)
                    {
                        DataGridViewTextBoxCell cellMessenger = !isNewRow ? (DataGridViewTextBoxCell)row.Cells[3] : new DataGridViewTextBoxCell();
                        cellMessenger.Value = strMessenger;
                        cellMessenger.Style.Font = m_fontGrid;

                        // MultiLine Text Option
                        cellMessenger.Style.WrapMode = DataGridViewTriState.True;

                        nMissionIndex++;

                        if (isNewRow)
                            row.Cells.Add(cellMessenger);

                        cellMessenger.ReadOnly = true;
                    }

                    DataGridViewTextBoxCell cell = !isNewRow ? (DataGridViewTextBoxCell)row.Cells[nMissionIndex] : new DataGridViewTextBoxCell();
                    cell.Value = strItem;
                    cell.Style.Font = m_fontGrid;

                    if (isLeftGrid)
                        cell.Style.ForeColor = Color.Gray;

                    // MultiLine Text Option
                    cell.Style.WrapMode = DataGridViewTriState.True;
                    Size szPrefer = cell.PreferredSize;

                    /*if (row.Height < szPrefer.Height)
                        row.Height = szPrefer.Height;*/
                    //row.Height = grid.Size.Height;
                    if (isNewRow)
                    {
                        row.Cells.Add(cell);
                        grid.Rows.Add(row);
                    }

                    cell.ReadOnly = true;

                    arrData.Add(row);

                    grid.AutoResizeRow(row.Index, DataGridViewAutoSizeRowMode.AllCellsExceptHeader);

                    if (row.Height < m_nDefaultGridRowHeight)
                        row.Height = m_nDefaultGridRowHeight;
                }
            }
        }

        private string SetTeamText(Sections.Section section, bool isNewRow, DataGridViewRow row, string strItem, string strTeamName, bool isLeftGrid)
        {
            if (section == null)
            {
                if (isNewRow)
                    row.Cells.Add(new DataGridViewTextBoxCell());

                return strItem;
            }

            Sections.Section.ComponentType type = section.GetComponentType();
            string strTeamText = "", strSrc = "";

            if (type == Sections.Section.ComponentType.PROCESS)
            {
                Sections.SectionProcess process = (Sections.SectionProcess)section;
                strTeamText = process.TextDown + " → ";
                strSrc = process.TextDown;
            }

            // 문자열 앞뒤의 공백 문자들을 없앤다.
            strItem = strItem.TrimStart(new char[] { ' ', '\t', '\r', '\n' });
            strItem = strItem.TrimEnd(new char[] { ' ', '\t', '\r', '\n' });

            if (type == Sections.Section.ComponentType.PROCESS && strTeamName.Length > 0)
            {
                string strTrg = strTeamName;

                if (strTeamText.Length == 0)
                    strTeamText = "→ " + strTrg;
                else
                {
                    if (strSrc == strTrg)
                        strTeamText = strTrg;
                    else
                        strTeamText += strTrg;
                }
            }

            /*if (strItem.StartsWith("("))
            {
                int nIndex = strItem.IndexOf(')');

                if (nIndex >= 0)
                {
                    string strTrg = strItem.Substring(1, nIndex - 1);
                    strItem = strItem.Substring(nIndex + 1);
                    strItem = strItem.TrimStart(new char[] { ' ', '\t', '\r', '\n' });

                    if (strTeamText.Length == 0 && type == Sections.Section.ComponentType.PROCESS)
                        strTeamText = "→ " + strTrg;
                    else
                    {
                        if (strSrc == strTrg)
                            strTeamText = strTrg;
                        else
                            strTeamText += strTrg;
                    }
                }
            }*/

            DataGridViewTextBoxCell cell = !isNewRow ? (DataGridViewTextBoxCell)row.Cells[2] : new DataGridViewTextBoxCell();
            cell.Value = strTeamText;
            //cell.Style.Font = m_fontGrid;
            cell.Style.Font = new Font(m_fontGrid.FontFamily, 14.0f);

            if (isLeftGrid)
                cell.Style.ForeColor = Color.Gray;

            // MultiLine Text Option
            cell.Style.WrapMode = DataGridViewTriState.True;
            Size szPrefer = cell.PreferredSize;

            /*if (row.Height < szPrefer.Height)
                row.Height = szPrefer.Height;*/

            if (isNewRow)
                row.Cells.Add(cell);

            cell.ReadOnly = true;

            return strItem;
        }

        private void ClearGrid(DataGridView grid, ArrayList arrData, bool isLeftGrid)
        {
            grid.Tag = null;

            int nDataCount = arrData.Count;
            arrData.Clear();

            if (nDataCount > 0)
            {
                for (int i = 0; i < nDataCount; i++)
                {
                    if (grid.Rows.Count == 0)
                        break;

                    grid.Rows.RemoveAt(0);
                }

                if ((nDataCount % 2) == 1)
                {
                    AddNewLine(0, grid, isLeftGrid, false);
                    nDataCount--;
                }

                int nCurrentRowCount = grid.Rows.Count;

                for (int i = nCurrentRowCount; i < nCurrentRowCount + nDataCount; i++)
                {
                    AddNewLine(i, grid, isLeftGrid);
                }
            }
        }

        private void FormMissionStatus_DoubleClick(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Normal)
                ShowMaximize();
            else
                ShowNormal();
        }

        private void ChangePrev()
        {
            if (m_statePrev == null)
            {
                ClearGrid(dataGridViewPrev, m_arrPrev, true);
            }
            else
            {
                Sections.Section.ComponentType type = m_statePrev.Section.GetComponentType();

                if (type == Sections.Section.ComponentType.DECISION ||
                    type == Sections.Section.ComponentType.ENDPOINT ||
                    type == Sections.Section.ComponentType.TRANSSOP)
                {
                    bool isChanged = false;

                    if (colPrevOption.Visible || colPrevOption2.Visible || m_arrPrev.Count != 1)
                        isChanged = true;
                    else
                    {
                        GridItem item = (GridItem)m_arrPrev[0];

                        if (item.ComponentType != type || item.Mission != m_statePrev.Section.Title)
                            isChanged = true;
                    }

                    if (isChanged)
                    {
                        colPrevOption.Visible = colPrevOption2.Visible = false;
                        ClearGrid(dataGridViewPrev, m_arrPrev, true);

                        GridItem item = new GridItem();
                        item.Mission = m_statePrev.Section.Title;
                        item.ComponentType = type;
                        m_arrPrev.Add(item);

                        dataGridViewPrev.Rows[0].Cells[3].Value = m_statePrev.Section.Title;
                    }
                }
                else if (type == Sections.Section.ComponentType.INTERNAL)
                {
                    bool isChanged = false;
                    string strTitle = m_statePrev.Section.Title;
                    Sections.SectionDataInternal data = (Sections.SectionDataInternal)m_statePrev.Section.Data;

                    if (!colPrevOption.Visible || colPrevOption2.Visible || m_arrPrev.Count != 1)
                        isChanged = true;
                    else
                    {
                        GridItem item = new GridItem();

                        if (item.ComponentType != type || item.UseBroadcast != data.UseBroadcast ||
                            item.UseSMS != data.UseMobileApp || item.Mission != strTitle)
                            isChanged = false;
                    }

                    if (isChanged)
                    {
                        GridItem item = new GridItem();

                        item.UseSMS = data.UseMobileApp;
                        //item.UseBroadcast = data
                    }
                }
            }
        }

        public void SelectRows(ArrayList arrRowIndices, ComponentContents contents)
        {
            if (contents != null && dataGridViewCurrent.Tag == contents)
            {
                dataGridViewCurrent.ClearSelection();
                int nColumnCount = dataGridViewCurrent.Columns.Count;

                foreach (int nRowIndex in arrRowIndices)
                {
                    if (nRowIndex >= dataGridViewCurrent.Rows.Count)
                        continue;

                    // 보이지 않는 셀은 선택할 수 없다.
                    if (dataGridViewCurrent.Rows[nRowIndex].Cells[0].Visible)
                    {
                        // 자동 줄바꿈
                        dataGridViewCurrent.CurrentCell = dataGridViewCurrent.Rows[nRowIndex].Cells[0];
                    }

                    for (int i = 0; i < nColumnCount; i++)
                    {
                        // 보이지 않는 셀은 선택할 수 없다.
                        if (dataGridViewCurrent.Rows[nRowIndex].Cells[i].Visible)
                            dataGridViewCurrent.Rows[nRowIndex].Cells[i].Selected = true;
                    }
                }
            }
        }

        private void SelectFirstRow(DataGridView grid)
        {
            if (grid.Rows.Count == 0)
                return;

            int nColumnCount = grid.Columns.Count;
            grid.CurrentCell = grid.Rows[0].Cells[nColumnCount - 1];

            for (int i = 0; i < nColumnCount; i++)
            {
                grid.Rows[0].Cells[i].Selected = true;
            }
        }

        private void ChangeCurrent()
        {
        }

        private void ChangeNext()
        {
        }

        public Sections.SectionState PrevState
        {
            get { return m_statePrev; }
            set
            {
                m_statePrev = value;
                ChangePrev();
            }
        }

        public Sections.SectionState CurrentState
        {
            get { return m_stateCurrent; }
            set
            {
                m_stateCurrent = value;
                ChangeCurrent();
            }
        }

        public Sections.SectionState NextState
        {
            get { return m_stateNext; }
            set
            {
                m_stateNext = value;
                ChangeNext();
            }
        }

        public string Title
        {
            get { return labelTitle.Text; }
            set { labelTitle.Text = value; }
        }

        public Sections.WorkFlow CurrentWorkFlow
        {
            set { m_workFlowCurrent = value; }
        }

        public class DataGridViewImageCellBlank : DataGridViewImageCell
        {
            public DataGridViewImageCellBlank()
                : base()
            {
            }
            
            public DataGridViewImageCellBlank(bool valueIsIcon)
                : base(valueIsIcon)
            {
            }
            
            public override object DefaultNewRowValue
            {
                get
                {
                    return null; // RETURNS NULL, INSTEAD OF THE 'RED X'
                }
            }
        }

        public class GridItem
        {
            private bool m_useSMS = false;
            private bool m_useBroadcast = false;
            private string m_strTeamName = "";
            private string m_strMission = "";
            private Sections.Section.ComponentType m_componentType = Sections.Section.ComponentType.NONE;

            public bool UseSMS
            {
                get { return m_useSMS; }
                set { m_useSMS = value; }
            }

            public bool UseBroadcast
            {
                get { return m_useBroadcast; }
                set { m_useBroadcast = value; }
            }

            public string TeamName
            {
                get { return m_strTeamName; }
                set { m_strTeamName = value; }
            }

            public string Mission
            {
                get { return m_strMission; }
                set { m_strMission = value; }
            }

            public Sections.Section.ComponentType ComponentType
            {
                get { return m_componentType; }
                set { m_componentType = value; }
            }
        }
    }
}
