using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;


using UnE.SOP.Workstate;

namespace SOPMonitoringSystem
{

    public class PictureBoxDB : PictureBox
    {
        public PictureBoxDB() : base()
        {
            EnableDoubleBuffering();
        }

        public void EnableDoubleBuffering()
        {
            // Set the value of the double-buffering style bits to true.
            this.SetStyle(ControlStyles.DoubleBuffer, true);
            this.UpdateStyles();
        }
    }

    public partial class FormMissionStatus : Form
    {
        public enum ItemType { PREV_ITEM = 0, CURRENT_ITEM, NEXT_ITEM };

        private WorkFlow m_workFlowCurrent = null;
        private SectionState m_statePrev = null;
        private SectionState m_stateCurrent = null;
        private SectionState m_stateNext = null;

        private ArrayList m_arrPrev = new ArrayList();
        private ArrayList m_arrCurrent = new ArrayList();
        private ArrayList m_arrNext = new ArrayList();

        private Color m_colEvenLeft = Color.FromArgb(242, 242, 242);
        private Color m_colOddLeft = Color.FromArgb(232, 232, 232);
        private Color m_colEvenRight = Color.White;
        private Color m_colOddRight = Color.FromArgb(232, 232, 232);

        private Font m_fontGrid = null;
        //private float m_fFontHeight = 20.0f;

        private int m_COLUMN_INDEX_TITLE = 0;
        private int m_COLUMN_INDEX_CONTENT = 1;
        private int m_COLUMN_INDEX_SMS = 2;
        private int m_COLUMN_INDEX_COMPLETE = 3;

        private string m_strHeaderTitle_Process = "실행자";
        private string m_strHeaderContent_Process = "임무내용";
        private string m_strHeaderContent_Internal = "전파내용";
        private string m_strHeaderContent_Decision = "판단";
        private string m_strHeaderSMS_Process = "문자";
        private string m_strHeaderSMS_Internal = "실행";

        private int m_nDefaultGridRowHeight = 64;

        /*private PointF[] m_arrPrevPolygon = new PointF[4];
        private PointF[] m_arrNextPolygon = new PointF[4];
        private PointF[] m_arrCurrentPolygon = new PointF[4];*/
        private ShadowControl m_shadowPrev = new ShadowControl();
        private ShadowControl m_shadowNext = new ShadowControl();
        private ShadowControl m_shadowCurrent = new ShadowControl();
        private int m_nShadowMoveX = 6;
        private int m_nShadowMoveY = 5;//

        //private Dictionary<DataGridViewCell, TextBox> m_dicCellTextBox = new Dictionary<DataGridViewCell, TextBox>();
        public void EnableDoubleBuffering()
        {
            // Set the value of the double-buffering style bits to true.
            this.SetStyle(ControlStyles.DoubleBuffer, true);
            this.UpdateStyles();
        }

        public FormMissionStatus()
        {
            InitializeComponent();

            this.DoubleBuffered = true;
            // 타이틀 공백을 초기화
            Title = "";

            //FontFamily fontFamily = GetFontFamily("맑은고딕", "굴림체", "돋움체");
            //m_fontGrid = new Font(fontFamily, m_fFontHeight);
            m_fontGrid = labelTitle.Font;

            btnClose.Click += btnClose_Click;

            this.StyleChanged += FormMissionStatus_StyleChanged;
            this.Resize += FormMissionStatus_Resize;
            this.ResizeEnd += FormMissionStatus_ResizeEnd;

            FormSOP.SetDoubleBuffer(dataGridViewPrev, true);
            FormSOP.SetDoubleBuffer(dataGridViewCurrent, true);
            FormSOP.SetDoubleBuffer(dataGridViewNext, true);

            FormSOP.SetDoubleBuffer(panelCurrentBody, true);
            FormSOP.SetDoubleBuffer(panelPrevBody, true);
            FormSOP.SetDoubleBuffer(panelNextBody, true);
                        
#if SAFE_KOREA_YH_2017
            this.TopMost = true;
#endif

            //dataGridViewPrev.Resize += dataGridView_Resize;
            //dataGridViewCurrent.Resize += dataGridView_Resize;
            //dataGridViewNext.Resize += dataGridView_Resize;

            //dataGridViewPrev.CellValueChanged += dataGridView_CellValueChanged;
            //dataGridViewCurrent.CellValueChanged += dataGridView_CellValueChanged;
            //dataGridViewNext.CellValueChanged += dataGridView_CellValueChanged;
        }

        //private void dataGridView_Resize(object sender, EventArgs e)
        //{
        //    DataGridView grid = sender as DataGridView;

        //    for (int nRow = 0; nRow < grid.Rows.Count; nRow++)
        //    {
        //        DataGridViewCell cell = grid.Rows[nRow].Cells[m_COLUMN_INDEX_CONTENT];

        //        if (m_dicCellTextBox.ContainsKey(cell))
        //        {
        //            TextBox txt = m_dicCellTextBox[cell];

        //            System.Drawing.Rectangle rect = grid.GetCellDisplayRectangle(m_COLUMN_INDEX_CONTENT, nRow, false);

        //            txt.Location = new Point(rect.X, rect.Y);
        //            txt.Size = new Size(rect.Width - 2, rect.Height - 2);
        //        }
        //    }
        //}

        //private void dataGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        //{
        //    if (e.RowIndex < 0 || e.ColumnIndex < 0)
        //        return;

        //    if (e.ColumnIndex != m_COLUMN_INDEX_CONTENT)
        //        return;

        //    DataGridView grid = sender as DataGridView;

        //    if (e.ColumnIndex == m_COLUMN_INDEX_CONTENT)
        //    {
        //        DataGridViewCell cell = grid.Rows[e.RowIndex].Cells[e.ColumnIndex];

        //        if (m_dicCellTextBox.ContainsKey(cell))
        //        {
        //            m_dicCellTextBox[cell].Text = cell.Value.ToString();
        //        }
        //    }
        //}

        private void FormMissionStatus_Resize(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Maximized
                && FormBorderStyle == System.Windows.Forms.FormBorderStyle.Sizable)
            {
                ShowMaximize();
                return;
            }
        }

        private void FormMissionStatus_ResizeEnd(object sender, EventArgs e)
        {
            Reshape();
        }

        private void FormMissionStatus_StyleChanged(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Maximized
                && FormBorderStyle == System.Windows.Forms.FormBorderStyle.Sizable)
            {
                this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            }
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
            // 창의 크기에 따라 컨트롤의 위치와 크기가 배정되므로 Maximize 모드로 전환전에 미리 창의 위치와 크기를 지정해줌.
            Screen currScreen = Screen.FromControl(this);
            if (currScreen != null)
            {
                this.Location = new Point(currScreen.Bounds.X, currScreen.Bounds.Y);
                this.Size = new Size(currScreen.Bounds.Width, currScreen.Bounds.Height);
            }

            this.WindowState = FormWindowState.Maximized;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;

            btnClose.Visible = true;

            Reshape();
        }

        public void ShowNormal()
        {
            this.WindowState = FormWindowState.Normal;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;

            btnClose.Visible = false;

            Reshape();
        }

        private void Reshape()
        {
            btnClose.Location = new Point(this.ClientSize.Width - btnClose.Width, 0);

            pictureBoxTitle1BG.Location = new Point(0, 0);
            pictureBoxTitle1BG.Size = new Size(this.ClientSize.Width, pictureBoxTitle1BG.Size.Height);
            pictureBoxTitle1Name.Location = new Point(100, 0);

            if (UnE.SOP.ProxySOP.Instance.SiteID == 3)
            {
                this.pictureBoxLogo.Image = global::SOPMonitoringSystem.Properties.Resources.KDHC_logo;
                this.pictureBoxLogo.Size = new Size(232, 43);
            }

            pictureBoxLogo.Location = new Point(pictureBoxTitle1BG.Size.Width - pictureBoxLogo.Size.Width - btnClose.Width, (pictureBoxTitle1BG.Size.Height - pictureBoxLogo.Size.Height) / 2);
            pictureBoxTitlebar.Location = new Point(0, pictureBoxTitle1BG.Location.Y + pictureBoxTitle1BG.Size.Height);
            pictureBoxTitlebar.Size = new Size(this.ClientSize.Width + 20, pictureBoxTitlebar.Size.Height);

            #region 상중하 그리드 나눔 (사용)
            int nSpaceX = 10, nSpaceY = 10;
            int nMiddleSpaceY = 10;
            int nGridTitleHeight = pictureBoxCurrentHeader.Size.Height;
            int nGridWidth = (this.ClientSize.Width - nSpaceX * 2);
            int nGridHeight = this.ClientSize.Height - pictureBoxTitlebar.Location.Y - pictureBoxTitlebar.Size.Height - nSpaceY - nGridTitleHeight * 3;

            dataGridViewPrev.Location = new Point(nSpaceX, pictureBoxTitlebar.Location.Y + pictureBoxTitlebar.Size.Height + nSpaceY + nGridTitleHeight);
            dataGridViewPrev.Size = new Size(nGridWidth, nGridHeight / 4);

            dataGridViewCurrent.Location = new Point(nSpaceX, dataGridViewPrev.Location.Y + dataGridViewPrev.Size.Height + nMiddleSpaceY + nGridTitleHeight);
            dataGridViewCurrent.Size = new Size(nGridWidth, nGridHeight / 2 - nMiddleSpaceY * 3);

            dataGridViewNext.Location = new Point(nSpaceX, dataGridViewCurrent.Location.Y + dataGridViewCurrent.Size.Height + nMiddleSpaceY + nGridTitleHeight);
            dataGridViewNext.Size = new Size(nGridWidth, nGridHeight / 4);
            #endregion

            #region 좌우 그리드 나눔 (미사용)
            //int nSpaceX = 20, nSpaceY = 20;
            //int nMiddleSpaceX = 20, nMiddleSpaceY = 20;
            //int nGridTitleHeight = pictureBoxCurrentHeader.Size.Height;
            //int nGridWidth = (this.ClientSize.Width - nSpaceX * 2 - nMiddleSpaceX) / 2;
            //int nGridHeight1 = this.ClientSize.Height - pictureBoxTitlebar.Location.Y - pictureBoxTitlebar.Size.Height - nSpaceY * 2 - nGridTitleHeight;
            //int nGridHeight2 = (nGridHeight1 + nGridTitleHeight - nMiddleSpaceY) / 2 - nGridTitleHeight;

            //dataGridViewPrev.Location = new Point(nSpaceX, pictureBoxTitlebar.Location.Y + pictureBoxTitlebar.Size.Height + nSpaceY + nGridTitleHeight);
            //dataGridViewPrev.Size = new Size(nGridWidth * 2 / 3, nGridHeight2);

            //dataGridViewNext.Location = new Point(nSpaceX, dataGridViewPrev.Location.Y + dataGridViewPrev.Size.Height + nMiddleSpaceY + nGridTitleHeight);
            //dataGridViewNext.Size = new Size(nGridWidth * 2 / 3, nGridHeight2);

            //dataGridViewCurrent.Location = new Point(this.ClientSize.Width - nSpaceX - nGridWidth * 4 / 3, dataGridViewPrev.Location.Y);
            //dataGridViewCurrent.Size = new Size(nGridWidth * 4 / 3, nGridHeight1);
            #endregion 좌우 그리드 나눔

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



            DataGridViewTextBoxCell cell1 = new DataGridViewTextBoxCell();
            row.Cells.Add(cell1);
            cell1.ReadOnly = true;

            DataGridViewTextBoxCell cell2 = new DataGridViewTextBoxCell();
            row.Cells.Add(cell2);
            cell2.ReadOnly = true;

            if (isLeftGrid == false)
            {
                DataGridViewImageCellBlank cell3 = new DataGridViewImageCellBlank();
                cell3.Value = Properties.Resources.none;
                row.Cells.Add(cell3);

                DataGridViewImageCellBlank cell4 = new DataGridViewImageCellBlank();
                cell4.Value = Properties.Resources.none;
                row.Cells.Add(cell4);
            }


            if (addBack)
                grid.Rows.Add(row);
            else
                grid.Rows.Insert(0, row);



            //TextBox txt = new TextBox()
            //{
            //    Multiline = true,
            //    ScrollBars = ScrollBars.Vertical,
            //    ReadOnly = true
            //};

            //if (m_dicCellTextBox.ContainsKey(cell2))
            //{
            //    m_dicCellTextBox[cell2].Dispose();
            //    m_dicCellTextBox[cell2] = txt;
            //}
            //else
            //{
            //    m_dicCellTextBox.Add(cell2, txt);
            //}

            //grid.Controls.Add(txt);

            //System.Drawing.Rectangle rect = grid.GetCellDisplayRectangle(cell2.ColumnIndex, cell2.RowIndex, false);

            //txt.Location = new Point(rect.X, rect.Y);
            //txt.Size = new Size(rect.Width - 2, rect.Height - 2);
            //txt.Show();
        }



        private void ReshapeGridTitle(PictureBox header, PictureBox body, PictureBox tail, DataGridView grid)
        {
            string strDefaultTitleText = "미션 Title";
            string strDefaultTargetText = "수신자";

            Point ptGrid = grid.Location;
            Size szGrid = grid.Size;

            header.Location = new Point(ptGrid.X, ptGrid.Y - header.Size.Height);
            tail.Location = new Point(ptGrid.X + szGrid.Width - tail.Size.Width, header.Location.Y);
            body.Location = new Point(header.Location.X + header.Size.Width, header.Location.Y);

            // 이미지 늘이기
            body.Size = new Size(tail.Location.X - body.Location.X + 20, header.Size.Height);

            if (body == pictureBoxCurrentBody)
            {
                panelCurrentBody.Location = body.Location;
                panelCurrentBody.Size = body.Size;

                body.Visible = false;

                labelTarget.Text = (String.Equals(labelTarget.Text, strDefaultTargetText) ? "" : labelTarget.Text);
                labelCurrentTitle.Text = (String.Equals(labelCurrentTitle.Text, strDefaultTitleText) ? "" : labelCurrentTitle.Text).Replace(Environment.NewLine, " ");
            }
            else if (body == pictureBoxNextBody)
            {
                panelNextBody.Location = body.Location;
                panelNextBody.Size = body.Size;

                body.Visible = false;

                labelNextTitle.Text = (String.Equals(labelNextTitle.Text, strDefaultTitleText) ? "" : labelNextTitle.Text).Replace(Environment.NewLine, " ");
            }
            else if (body == pictureBoxPrevBody)
            {
                panelPrevBody.Location = body.Location;
                panelPrevBody.Size = body.Size;

                body.Visible = false;

                labelPrevTitle.Text = (String.Equals(labelPrevTitle.Text, strDefaultTitleText) ? "" : labelPrevTitle.Text).Replace(Environment.NewLine, " ");
            }
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
                SetSectionContents(dataGridViewPrev, m_arrPrev, true, section);
            else if (type == ItemType.CURRENT_ITEM)
            {

                SetSectionContents(dataGridViewCurrent, m_arrCurrent, false, section);

                if (section != null)
                 System.Diagnostics.Trace.WriteLine("Mission Secction Current : " + section.Data.Title + " ID : " + section.Data.ComponentID);
            }
            else if (type == ItemType.NEXT_ITEM)
                SetSectionContents(dataGridViewNext, m_arrNext, true, section);
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

        private void SetSectionContents(DataGridView grid, ArrayList arrData, bool isLeftGrid, Sections.Section section)
        {
            if (section == null)
            {
                ClearGrid(grid, arrData, isLeftGrid);
                return;
            }

            int nCheckedNotify1 = 0, nCheckedNotify2 = 0;
            Sections.Section.ComponentType type = section.GetComponentType();

            if (type == Sections.Section.ComponentType.PROCESS)
                WorkFlow.GetProcessCheckedNotify((Sections.SectionProcess)section, out nCheckedNotify1, out nCheckedNotify2);
            else if (type == Sections.Section.ComponentType.INTERNAL)
                WorkFlow.GetInternalCheckedNotify((Sections.SectionInternal)section, out nCheckedNotify1);
            else if (type == Sections.Section.ComponentType.EXTERNAL)
                WorkFlow.GetExternalCheckedNotify((Sections.SectionExternal)section, out nCheckedNotify1, out nCheckedNotify2);
            else if (type == Sections.Section.ComponentType.TRANSMISSION)
                WorkFlow.GetTransmissionCheckedNotify((Sections.SectionTransmission)section, out nCheckedNotify1, out nCheckedNotify2);

            // FormMission으로 데이터 복사를 위한 개체
            ComponentContents contents = new ComponentContents();

            string strTitle = section.Title;

            if (section.Data.SectionNumber > 0)
                strTitle = section.Data.SectionNumber.ToString() + ". " + strTitle;

            PageBackstageSOP.MakeComponentContentsData(contents, strTitle, DateTime.Now, "입력 대기", section, State.INPUT, nCheckedNotify1, nCheckedNotify2);

            SetContents(grid, arrData, isLeftGrid, contents);
        }

        // contents가 null이면 해당 Grid를 초기화
        public void SetContents(ComponentContents contents, ItemType type)
        {
            if (type == ItemType.PREV_ITEM)
                SetContents(dataGridViewPrev, m_arrPrev, true, contents);
            else if (type == ItemType.CURRENT_ITEM)
                SetContents(dataGridViewCurrent, m_arrCurrent, false, contents);
            else if (type == ItemType.NEXT_ITEM)
                SetContents(dataGridViewNext, m_arrNext, true, contents);
        }

        private void SetDecisionContents(DataGridView grid, Sections.Section section, int nRowCount, bool isLeftGrid, ArrayList arrData)
        {
            grid.Columns[m_COLUMN_INDEX_CONTENT].HeaderText = m_strHeaderContent_Decision;
            grid.Columns[m_COLUMN_INDEX_TITLE].Visible = false;

            if (isLeftGrid == false)
            {
                grid.Columns[m_COLUMN_INDEX_SMS].Visible = false;
            }

            if (nRowCount == 0)
                AddNewLine(0, grid, isLeftGrid);

            DataGridViewRow row = grid.Rows[0];

            DataGridViewCell cell1 = row.Cells[m_COLUMN_INDEX_TITLE];
            DataGridViewCell cell2 = row.Cells[m_COLUMN_INDEX_CONTENT];

            cell1.Value = "판단";
            cell2.Value = section.Title;

            cell1.Style.Font = m_fontGrid;
            cell2.Style.Font = m_fontGrid;

            cell1.Style.ForeColor = Color.Gray;
            cell2.Style.ForeColor = Color.Gray;

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

            arrData.Add(row);

            grid.ClearSelection();
        }

        private void SetInternalContents(DataGridView grid, Sections.Section section, int nRowCount, bool isLeftGrid, ArrayList arrData)
        {
            grid.Columns[m_COLUMN_INDEX_CONTENT].HeaderText = m_strHeaderContent_Internal;
            grid.Columns[m_COLUMN_INDEX_TITLE].Visible = false;

            if (isLeftGrid == false)
            {
                grid.Columns[m_COLUMN_INDEX_SMS].Visible = true;
                grid.Columns[m_COLUMN_INDEX_SMS].HeaderText = m_strHeaderSMS_Internal;
            }

            if (nRowCount == 0)
                AddNewLine(0, grid, isLeftGrid);

            DataGridViewRow row = grid.Rows[0];

            DataGridViewCell cell1 = row.Cells[m_COLUMN_INDEX_TITLE];
            DataGridViewCell cell2 = row.Cells[m_COLUMN_INDEX_CONTENT];

            string strSMS = "문자전파", strBroadcast = "방송전파";
            if (((Sections.SectionDataInternal)(section.Data)).UseBroadcast)
                cell1.Value = strBroadcast;
            else if (((Sections.SectionDataInternal)(section.Data)).UseMobileApp)
                cell1.Value = strSMS;

            cell2.Value = ((Sections.SectionDataInternal)(section.Data)).BroadcastMessage;

            cell1.Style.Font = m_fontGrid;
            cell2.Style.Font = m_fontGrid;

            cell1.Style.ForeColor = Color.Gray;
            cell2.Style.ForeColor = Color.Gray;

            // MultiLine Text Option
            cell1.Style.WrapMode = DataGridViewTriState.True;
            cell2.Style.WrapMode = DataGridViewTriState.True;


            if (isLeftGrid == false)
            {
                string strCommanderName, strCommanderName2, strCommanderPhoneNumber;
                Sections.SectionCommander commander = ComponentContents.GetCommanderInfo(section, out strCommanderName, out strCommanderName2, out strCommanderPhoneNumber);
                labelTarget.Text = String.Format("( 발신자 : {0} )", strCommanderName2);

                //if (((Sections.SectionDataInternal)(section.Data)).UseMobileApp)
                //{
                //    string strCommanderName, strCommanderName2, strCommanderPhoneNumber, strReceiverNames;
                //    bool onlyTeamLeaderReceiver;
                //    ArrayList arrTeamList;

                //    Sections.SectionCommander commander = ComponentContents.GetCommanderInfo(section, out strCommanderName, out strCommanderName2, out strCommanderPhoneNumber);
                //    ArrayList arrReceiverPhoneNumbers = ComponentContents.GetReceiverInfo(section, out strReceiverNames, out arrTeamList, out onlyTeamLeaderReceiver);

                //    if (/* String.IsNullOrWhiteSpace(labelTarget.Text) == true && */String.IsNullOrWhiteSpace(strReceiverNames) == false)
                //    {
                //        labelTarget.Text = String.Format("( 발신자 : {0}      수신자 : {1} )", strCommanderName2, strReceiverNames);
                //    }
                //    else
                //    {
                //        labelTarget.Text = String.Format("( 발신자 : {0}      수신자 미지정 )", strCommanderName2);
                //    }
                //}
                //else if (((Sections.SectionDataInternal)(section.Data)).UseBroadcast)
                //{
                //    string strCommanderName, strCommanderName2, strCommanderPhoneNumber;

                //    Sections.SectionCommander commander = ComponentContents.GetCommanderInfo(section, out strCommanderName, out strCommanderName2, out strCommanderPhoneNumber);

                //    labelTarget.Text = String.Format("( 발신자 : {0} )", strCommanderName2);
                //}

                bool isBroadcast, isExcute, isComplete;
                ComponentContents contents = grid.Tag as ComponentContents;

                if (contents.GetItem(out isBroadcast, out isExcute, out isComplete))
                {
                    DataGridViewCell cell3 = row.Cells[m_COLUMN_INDEX_SMS];
                    DataGridViewCell cell4 = row.Cells[m_COLUMN_INDEX_COMPLETE];

                    cell3.Value = (isExcute ? (isBroadcast ? Properties.Resources.broadcast_on : Properties.Resources.sms_send) : Properties.Resources.none);
                    cell4.Value = (isComplete ? Properties.Resources.checkbox_complate_64 : Properties.Resources.none);
                }
            }

            Size szPrefer1 = cell1.PreferredSize;
            Size szPrefer2 = cell2.PreferredSize;
            int nBigHeight = szPrefer1.Height > szPrefer2.Height ? szPrefer1.Height : szPrefer2.Height;

            /*if (row.Height < nBigHeight)
                row.Height = nBigHeight;*/
            //row.Height = grid.Size.Height;

            grid.AutoResizeRow(0, DataGridViewAutoSizeRowMode.AllCellsExceptHeader);

            if (grid.Rows[0].Height < m_nDefaultGridRowHeight)
                grid.Rows[0].Height = m_nDefaultGridRowHeight;

            arrData.Add(row);

            grid.ClearSelection();
        }

        private void SetProcessContents(DataGridView grid, Sections.Section section, int nRowCount, bool isLeftGrid, ArrayList arrData)
        {
            grid.Columns[m_COLUMN_INDEX_CONTENT].HeaderText = m_strHeaderContent_Process;
            grid.Columns[m_COLUMN_INDEX_TITLE].HeaderText = m_strHeaderTitle_Process;
            grid.Columns[m_COLUMN_INDEX_TITLE].Visible = true;

            if (isLeftGrid == false)
            {
                grid.Columns[m_COLUMN_INDEX_SMS].Visible = true;
                grid.Columns[m_COLUMN_INDEX_SMS].HeaderText = m_strHeaderSMS_Process;
            }

            ComponentContents contents = grid.Tag as ComponentContents;

            bool isSendSMS, isComplete;
            string strSender, strItem, strReceivers, strPerformer;
            int nItemCount = contents.ItemCount;

            for (int i = 0; i < nItemCount; i++)
            {
                if (contents.GetItem(i, out isSendSMS, out isComplete, out strSender, out strItem, out strReceivers, out strPerformer))
                {
                    if (i >= nRowCount)
                    {
                        AddNewLine(0, grid, isLeftGrid);
                    }

                    DataGridViewRow row = grid.Rows[i];

                    DataGridViewTextBoxCell cellPerformer = (DataGridViewTextBoxCell)row.Cells[m_COLUMN_INDEX_TITLE];
                    DataGridViewTextBoxCell cellContents = (DataGridViewTextBoxCell)row.Cells[m_COLUMN_INDEX_CONTENT];

                    if (isLeftGrid == false)
                    {
                        if (String.IsNullOrWhiteSpace(strReceivers) == false &&
                            (String.IsNullOrWhiteSpace(labelTarget.Text) == true || String.Equals(strReceivers, labelTarget) == false))
                        {
                            labelTarget.Text = String.Format("( 수신자 : {0} )", strReceivers);
                        }
                        else
                        {
                            labelTarget.Text = "( 수신자 미지정 )";
                        }

                        DataGridViewImageCell cellUseSMS = (DataGridViewImageCell)row.Cells[m_COLUMN_INDEX_SMS];
                        DataGridViewImageCell cellComplete = (DataGridViewImageCell)row.Cells[m_COLUMN_INDEX_COMPLETE];

                        if (isSendSMS == true)
                        {
                            cellUseSMS.Value = Properties.Resources.sms_send;
                        }
                        else
                        {
                            cellUseSMS.Value = Properties.Resources.none;
                        }

                        if (isComplete == true)
                        {
                            cellComplete.Value = Properties.Resources.checkbox_complate_64;
                        }
                        else
                        {
                            cellComplete.Value = Properties.Resources.none;
                        }
                    }

                    
                    cellPerformer.Style.Font = m_fontGrid;
                    cellContents.Style.Font = m_fontGrid;

                    cellPerformer.Style.ForeColor = Color.Gray;
                    cellContents.Style.ForeColor = Color.Gray;

                    // MultiLine Text Option
                    cellPerformer.Style.WrapMode = DataGridViewTriState.True;
                    cellContents.Style.WrapMode = DataGridViewTriState.True;


                    string strMission = SetTeamText(section, row, strItem, strReceivers, isLeftGrid);


                    //cellSender.Value = strSender;
                    cellPerformer.Value = strPerformer;
                    cellContents.Value = strItem;

                    grid.AutoResizeRow(row.Index, DataGridViewAutoSizeRowMode.AllCellsExceptHeader);

                    if (row.Height < m_nDefaultGridRowHeight)
                        row.Height = m_nDefaultGridRowHeight;


                    arrData.Add(row);

                }
            }

            grid.ClearSelection();

            //if (m_currContents == null) return;

            if (contents != null && m_currContents != null)
            {
                if (String.Equals(contents.Title, m_currContents.Title))
                {
                    SelectRows(m_arrCurrRowIndices, contents);
                }
            }

            #region description

            //ComponentContents contents = grid.Tag as ComponentContents;

            //bool useBroadcast = contents.UseBroadcast;
            //bool useSMS = contents.UseSMS;
            //bool check1, check2;
            //string strItem, strMessenger, strTeamName;

            //for (int i = 0; i < nItemCount; i++)
            //{
            //    if (contents.GetItem(i, out check1, out check2, out strMessenger, out strTeamName, out strItem))
            //    {
            //        bool isNewRow = i >= nRowCount;
            //        DataGridViewRow row = !isNewRow ? grid.Rows[i] : new DataGridViewRow();

            //        if (useSMS && useBroadcast)
            //        {
            //            //if (colOption1 != null && colOption2 != null)
            //            //{
            //            //    colOption1.Visible = true;
            //            //    colOption2.Visible = true;
            //            //}

            //            DataGridViewImageCell cell1 = !isNewRow ? (DataGridViewImageCell)row.Cells[0] : new DataGridViewImageCell();

            //            if (isLeftGrid)
            //                cell1.Value = check1 ? Properties.Resources.sms_done : Properties.Resources.sms_off;
            //            else
            //                cell1.Value = check1 ? Properties.Resources.sms_on : Properties.Resources.sms_off;

            //            if (isNewRow)
            //                row.Cells.Add(cell1);

            //            DataGridViewImageCell cell2 = !isNewRow ? (DataGridViewImageCell)row.Cells[1] : new DataGridViewImageCell();

            //            if (isLeftGrid)
            //                cell2.Value = check2 ? Properties.Resources.broadcast_done : Properties.Resources.broadcast_off;
            //            else
            //                cell2.Value = check2 ? Properties.Resources.broadcast_on : Properties.Resources.broadcast_off;

            //            if (isNewRow)
            //                row.Cells.Add(cell2);
            //        }
            //        else if (useSMS)
            //        {
            //            //if (colOption1 != null && colOption2 != null)
            //            //{
            //            //    colOption1.Visible = true;
            //            //    colOption2.Visible = false;
            //            //}

            //            DataGridViewImageCell cell1 = !isNewRow ? (DataGridViewImageCell)row.Cells[0] : new DataGridViewImageCell();

            //            if (isLeftGrid)
            //                cell1.Value = check1 ? Properties.Resources.sms_done : Properties.Resources.sms_off;
            //            else
            //                cell1.Value = check1 ? Properties.Resources.sms_on : Properties.Resources.sms_off;

            //            if (isNewRow)
            //            {
            //                row.Cells.Add(cell1);
            //                row.Cells.Add(new DataGridViewImageCell());
            //            }
            //        }
            //        else if (useBroadcast)
            //        {
            //            //if (colOption1 != null && colOption2 != null)
            //            //{
            //            //    colOption1.Visible = false;
            //            //    colOption2.Visible = true;
            //            //}

            //            if (isNewRow)
            //            {
            //                row.Cells.Add(new DataGridViewImageCell());
            //            }

            //            DataGridViewImageCell cell2 = !isNewRow ? (DataGridViewImageCell)row.Cells[1] : new DataGridViewImageCell();

            //            if (isLeftGrid)
            //                cell2.Value = check2 ? Properties.Resources.broadcast_done : Properties.Resources.broadcast_off;
            //            else
            //                cell2.Value = check2 ? Properties.Resources.broadcast_on : Properties.Resources.broadcast_off;

            //            if (isNewRow)
            //                row.Cells.Add(cell2);
            //        }
            //        else
            //        {
            //            //if (colOption1 != null && colOption2 != null)
            //            //{
            //            //    colOption1.Visible = false;
            //            //    colOption2.Visible = false;
            //            //}

            //            if (isNewRow)
            //            {
            //                row.Cells.Add(new DataGridViewImageCell());
            //                row.Cells.Add(new DataGridViewImageCell());
            //            }
            //        }

            //        string strMission = SetTeamText(section, isNewRow, row, strItem, strTeamName, isLeftGrid);
            //        int nMissionIndex = 3;

            //        if (!isLeftGrid)
            //        {
            //            DataGridViewTextBoxCell cellMessenger = !isNewRow ? (DataGridViewTextBoxCell)row.Cells[3] : new DataGridViewTextBoxCell();
            //            cellMessenger.Value = strMessenger;
            //            cellMessenger.Style.Font = m_fontGrid;

            //            // MultiLine Text Option
            //            cellMessenger.Style.WrapMode = DataGridViewTriState.True;

            //            nMissionIndex++;

            //            if (isNewRow)
            //                row.Cells.Add(cellMessenger);

            //            cellMessenger.ReadOnly = true;
            //        }

            //        DataGridViewTextBoxCell cell = !isNewRow ? (DataGridViewTextBoxCell)row.Cells[nMissionIndex] : new DataGridViewTextBoxCell();
            //        cell.Value = strItem;
            //        cell.Style.Font = m_fontGrid;

            //        if (isLeftGrid)
            //            cell.Style.ForeColor = Color.Gray;

            //        // MultiLine Text Option
            //        cell.Style.WrapMode = DataGridViewTriState.True;
            //        Size szPrefer = cell.PreferredSize;

            //        /*if (row.Height < szPrefer.Height)
            //            row.Height = szPrefer.Height;*/
            //        //row.Height = grid.Size.Height;
            //        if (isNewRow)
            //        {
            //            row.Cells.Add(cell);
            //            grid.Rows.Add(row);
            //        }

            //        cell.ReadOnly = true;

            //        arrData.Add(row);

            //        grid.AutoResizeRow(row.Index, DataGridViewAutoSizeRowMode.AllCellsExceptHeader);

            //        if (row.Height < m_nDefaultGridRowHeight)
            //            row.Height = m_nDefaultGridRowHeight;
            //    }
            //}
            #endregion
        }

        private void SetContents(DataGridView grid, ArrayList arrData, bool isLeftGrid, ComponentContents contents)
        {
            ClearGrid(grid, arrData, isLeftGrid);

            if (contents == null)
            {
                if (grid == dataGridViewCurrent)
                {
                    labelTarget.Text = "";
                    labelCurrentTitle.Text = "";
                }
                else if (grid == dataGridViewNext)
                {
                    labelNextTitle.Text = "";
                }
                else if (grid == dataGridViewPrev)
                {
                    labelPrevTitle.Text = "";
                }

                return;
            }

            SelectFirstRow(grid);

            grid.Tag = contents;

            int nRowCount = grid.Rows.Count;
            Sections.Section section = contents.Section;


            if (grid == dataGridViewCurrent)
            {
                labelCurrentTitle.Text = String.Format("☞ {0}. {1}", section.Data.SectionNumber, section.Title.Replace(Environment.NewLine, " "));
            }
            else if (grid == dataGridViewNext)
            {
                labelNextTitle.Text = String.Format("☞ {0}. {1}", section.Data.SectionNumber, section.Title.Replace(Environment.NewLine, " "));
            }
            else if (grid == dataGridViewPrev)
            {
                labelPrevTitle.Text = String.Format("☞ {0}. {1}", section.Data.SectionNumber, section.Title.Replace(Environment.NewLine, " "));
            }

            if (section != null)
                System.Diagnostics.Trace.WriteLine("Mission Secction Current : " + section.Data.Title + " ID : " + section.Data.ComponentID);

            switch (section.GetComponentType())
            {
                case Sections.Section.ComponentType.PROCESS:
                    SetProcessContents(grid, section, nRowCount, isLeftGrid, arrData);
                    break;
                case Sections.Section.ComponentType.INTERNAL:
                    SetInternalContents(grid, section, nRowCount, isLeftGrid, arrData);
                    break;
                case Sections.Section.ComponentType.DECISION:
                    SetDecisionContents(grid, section, nRowCount, isLeftGrid, arrData);
                    break;
                default:
                    break;
            }

        }

        private string SetTeamText(Sections.Section section, DataGridViewRow row, string strItem, string strTeamName, bool isLeftGrid)
        {
            if (section == null)
            {
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
            strItem = strItem.Replace(",", ", ");

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

            DataGridViewTextBoxCell cell = (DataGridViewTextBoxCell)row.Cells[isLeftGrid ? 0 : 1];
            cell.Value = strTeamText;
            cell.Style.Font = m_fontGrid;
            //cell.Style.Font = new Font(m_fontGrid.FontFamily, 14.0f);

            if (isLeftGrid)
                cell.Style.ForeColor = Color.Gray;

            // MultiLine Text Option
            cell.Style.WrapMode = DataGridViewTriState.True;
            Size szPrefer = cell.PreferredSize;
            
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

                    //if (m_dicCellTextBox.ContainsKey(grid.Rows[0].Cells[1]))
                    //{
                    //    m_dicCellTextBox[grid.Rows[0].Cells[1]].Dispose();
                    //    m_dicCellTextBox.Remove(grid.Rows[0].Cells[1]);
                    //}

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

                    if (/*colPrevOption.Visible || colPrevOption2.Visible || */m_arrPrev.Count != 1)
                        isChanged = true;
                    else
                    {
                        GridItem item = (GridItem)m_arrPrev[0];

                        if (item.ComponentType != type || item.Mission != m_statePrev.Section.Title)
                            isChanged = true;
                    }

                    if (isChanged)
                    {
                        //colPrevOption.Visible = colPrevOption2.Visible = false;
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

                    if (/*!colPrevOption.Visible || colPrevOption2.Visible || */m_arrPrev.Count != 1)
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

        private ArrayList m_arrCurrRowIndices = null;
        private ComponentContents m_currContents = null;

        public void SelectRows(ArrayList arrRowIndices, ComponentContents contents)
        {
            m_arrCurrRowIndices = arrRowIndices;
            m_currContents = contents;

            if (contents != null && dataGridViewCurrent.Tag == contents)
            {
                dataGridViewCurrent.ClearSelection();
                bool bCellSelected = false;

                foreach (int nRowIndex in arrRowIndices)
                {
                    if (nRowIndex >= dataGridViewCurrent.Rows.Count)
                        continue;

                    if (bCellSelected == false)
                    {
                        // 자동 줄바꿈
                        for (int i = 0; i < dataGridViewCurrent.Columns.Count; i++)
                        {
                            // 보이지 않는 셀은 선택할 수 없다.
                            if (dataGridViewCurrent.Rows[nRowIndex].Cells[i].Visible)
                            {
                                dataGridViewCurrent.CurrentCell = dataGridViewCurrent.Rows[nRowIndex].Cells[i];
                                bCellSelected = true;
                                break;
                            }
                        }
                    }

                    dataGridViewCurrent.Rows[nRowIndex].Selected = true;

                    //// 보이지 않는 셀은 선택할 수 없다.
                    //if (dataGridViewCurrent.Rows[nRowIndex].Cells[0].Visible)
                    //{
                    //    // 자동 줄바꿈
                    //    dataGridViewCurrent.CurrentCell = dataGridViewCurrent.Rows[nRowIndex].Cells[0];
                    //}

                    //for (int i = 0; i < nColumnCount; i++)
                    //{
                    //    // 보이지 않는 셀은 선택할 수 없다.
                    //    if (dataGridViewCurrent.Rows[nRowIndex].Cells[i].Visible)
                    //        dataGridViewCurrent.Rows[nRowIndex].Cells[i].Selected = true;
                    //}
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

        public SectionState PrevState
        {
            get { return m_statePrev; }
            set
            {
                m_statePrev = value;
                ChangePrev();
            }
        }

        public SectionState CurrentState
        {
            get { return m_stateCurrent; }
            set
            {
                m_stateCurrent = value;
                ChangeCurrent();
            }
        }

        public SectionState NextState
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
            set { labelTitle.Text = value.Replace(@"\", " / "); }
        }

        public WorkFlow CurrentWorkFlow
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

        private void tsMenuInitialize_Click(object sender, EventArgs e)
        {
            SetSectionContents(null, ItemType.PREV_ITEM);
            SetSectionContents(null, ItemType.CURRENT_ITEM);
            SetSectionContents(null, ItemType.NEXT_ITEM);
            Title = "";
            labelTarget.Text = "";
            labelCurrentTitle.Text = "";
            labelNextTitle.Text = "";
            labelPrevTitle.Text = "";
        }

        private void OnMouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                cmsMain.Show((Control)sender, e.Location);
            }
        }

        private void FormMissionStatus_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!FormSOP.Instance.CloseThread)
            {
                e.Cancel = true;
                this.Hide();
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

    }
}
