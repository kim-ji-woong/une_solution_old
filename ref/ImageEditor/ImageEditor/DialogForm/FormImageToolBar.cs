using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using UnE.GUI;

namespace ImageEditor
{
    public partial class FormImageToolBar : Form, IRibbonButtonOwner
    {
        private ArrayList m_arRibbonButtons = new ArrayList();
        private enum ToggleMode { SelectArea = 0, SelectColor, Translate, StraightLine, Curve, Text, None };

        private int m_ToggleType = 0;
        public int ToggleType
        {
            get { return m_ToggleType; }
            set { m_ToggleType = value; }
        }

        public FormImageToolBar()
        {
            InitializeComponent();

            SetCommandID();

            //영역선택이 기본선택값
            rbSelectArea.IsChecked = true;
            m_ToggleType = 0;


            //rbTranslate.Enabled = false;
            rbZoomIn.Enabled = false;
            rbZoomOut.Enabled = false;
            rbTranslate.Enabled = false;
            rbRotate.Enabled = false;

            pictureBox3.Visible = false;
        }

        private void SetCommandID()
        {
            rbSelectArea.Owner = this;
            rbSelectColor.Owner = this;
            rbLineColor.Owner = this;
            rbZoomIn.Owner = this;
            rbZoomOut.Owner = this;
            rbTranslate.Owner = this;
            rbRotate.Owner = this;
            rbDrawStraightLine.Owner = this;
            rbDrawCurve.Owner = this;
            rbText.Owner = this;
            btnStrong.Owner = this;
            btnLean.Owner = this;
            btnUnderline.Owner = this;

            m_arRibbonButtons.Add(rbSelectArea);
            m_arRibbonButtons.Add(rbSelectColor);
            m_arRibbonButtons.Add(rbLineColor);
            m_arRibbonButtons.Add(rbZoomIn);
            m_arRibbonButtons.Add(rbZoomOut);
            m_arRibbonButtons.Add(rbTranslate);
            m_arRibbonButtons.Add(rbRotate);
            m_arRibbonButtons.Add(rbDrawStraightLine);
            m_arRibbonButtons.Add(rbDrawCurve);
            m_arRibbonButtons.Add(rbText);
            m_arRibbonButtons.Add(btnStrong);
            m_arRibbonButtons.Add(btnLean);
            m_arRibbonButtons.Add(btnUnderline);

            rbSelectArea.ID = ID.TOOLBAR_SELECT_AREA;
            rbSelectColor.ID = ID.TOOLBAR_SELECT_COLOR;
            rbLineColor.ID = ID.TOOLBAR_LINE_COLOR;
            rbZoomIn.ID = ID.TOOLBAR_ZOOMIN;
            rbZoomOut.ID = ID.TOOLBAR_ZOOMOUT;
            rbTranslate.ID = ID.TOOLBAR_TRANSLATE;
            rbRotate.ID = ID.TOOLBAR_ROTATE;
            rbDrawStraightLine.ID = ID.TOOLBAR_STRAIGHT_LINE;
            rbDrawCurve.ID = ID.TOOLBAR_CURVE;
            rbText.ID = ID.TOOLBAR_TEXT;
            btnStrong.ID = ID.TOOLBAR_STRONG;
            btnLean.ID = ID.TOOLBAR_LEAN;
            btnUnderline.ID = ID.TOOLBAR_UNDERLINE;
        }

        private void SetTooltip(Control ctrl, string strTooltipText)
        {
            ToolTip toolTip = new ToolTip();
            toolTip.SetToolTip(ctrl, strTooltipText);
        }

        private void SettingOption()
        {
            rbSelectArea.IsChecked = true;

        }

        public void OnRibbonButtonMouseDown(object sender, MouseEventArgs e)
        {
        }

        public void OnRibbonButtonMouseUp(object sender, MouseEventArgs e)
        {
            RibbonButton rbtn = (RibbonButton)sender;
            int nCmdID = rbtn.ID;
            //rbtn.IsChecked = !rbtn.IsChecked;
            if (rbtn.CheckButton)
            {
                bool bChecked = !rbtn.IsChecked;
                CheckedChanged(nCmdID, bChecked);
            }
            else
            {
                RunCommand(nCmdID);
            }
        }

        public ToolStripStatusLabel GetStatusLabel()
        {
            return null;
        }

        public void CheckedChanged(int nCommand, bool bChecked)
        {
            switch(nCommand)
            {
                case ID.TOOLBAR_SELECT_AREA:
                    break;
            }
        }

        private void CheckButtion(RibbonButton btn1, RibbonButton btn2)
        {

        }

        public void RunCommand(int nCommand)
        {
            switch (nCommand)
            {
                case ID.TOOLBAR_SELECT_AREA:

                    ButtonsCheckClear();

                    rbSelectArea.IsChecked = true;
                    rbSelectColor.IsChecked = false;

                    m_ToggleType = (int)ToggleMode.SelectArea;
                    FormMain.Instance.ContentForm.Cursor = Cursors.Default;
                    this.Cursor = Cursors.Default;
                    panel2.Visible = false;
                    panel3.Visible = false;
                    break;

                case ID.TOOLBAR_SELECT_COLOR:
 
                    ButtonsCheckClear();

                    rbSelectArea.IsChecked = false;
                    rbSelectColor.IsChecked = true;

                    m_ToggleType = (int)ToggleMode.SelectColor;
                    FormMain.Instance.ContentForm.Cursor = Cursors.Cross;
                    panel2.Visible = false;
                    panel3.Visible = false;
                    break;

                case ID.TOOLBAR_LINE_COLOR:
                    ColorDialog colorDialog = new ColorDialog();

                    ButtonsCheckClear();
                    colorDialog.AllowFullOpen = true;
                    colorDialog.ShowHelp = true;
                    colorDialog.AnyColor = true;
                    
                    if(colorDialog.ShowDialog() == DialogResult.OK)
                    {
                        FormMain.Instance.PropertiesForm.SetInfoGridColor(colorDialog.Color);
                    }
                    m_ToggleType = (int)ToggleMode.None;
                    FormMain.Instance.ContentForm.Cursor = Cursors.Default;
                    this.Cursor = Cursors.Default;
                    panel2.Visible = false;
                    panel3.Visible = false;
                    break;

                case ID.TOOLBAR_TRANSLATE:
                    ButtonsCheckClear();
                    rbTranslate.IsChecked = true;
                    m_ToggleType = (int)ToggleMode.Translate;
                    panel2.Visible = false;
                    panel3.Visible = false;
                    break;

                case ID.TOOLBAR_ROTATE:
                    ButtonsCheckClear();
                    FormMain.Instance.RotationForm = new FormRotation(1);
                    FormMain.Instance.RotationForm.StartPosition = FormStartPosition.CenterParent;
                    FormMain.Instance.RotationForm.Dock = DockStyle.Fill;
                    if (FormMain.Instance.RotationForm.ShowDialog(this) == DialogResult.OK)
                    {

                    }
                    m_ToggleType = (int)ToggleMode.None;
                    FormMain.Instance.ContentForm.Cursor = Cursors.Default;
                    this.Cursor = Cursors.Default;
                    panel2.Visible = false;
                    panel3.Visible = false;
                    break;

                case ID.TOOLBAR_ZOOMIN:
                    ButtonsCheckClear();
                    //FormMain.Instance.ContentForm.ZoomIn();
                    FormMain.Instance.ContentForm.SelectZoomIn();
                    m_ToggleType = (int)ToggleMode.None;
                    FormMain.Instance.ContentForm.Cursor = Cursors.Default;
                    this.Cursor = Cursors.Default;
                    panel2.Visible = false;
                    panel3.Visible = false;
                    break;

                case ID.TOOLBAR_ZOOMOUT:
                    ButtonsCheckClear();
                    //FormMain.Instance.ContentForm.ZoomOut();
                    FormMain.Instance.ContentForm.SelectZoomOut();
                    m_ToggleType = (int)ToggleMode.None;
                    FormMain.Instance.ContentForm.Cursor = Cursors.Default;
                    this.Cursor = Cursors.Default;
                    panel2.Visible = false;
                    panel3.Visible = false;
                    break;

                case ID.TOOLBAR_STRAIGHT_LINE:
                    ButtonsCheckClear();
                    rbDrawStraightLine.IsChecked = true;
                    m_ToggleType = (int)ToggleMode.StraightLine;
                    FormMain.Instance.ContentForm.Cursor = Cursors.Default;
                    this.Cursor = Cursors.Default;
                    panel2.Visible = false;
                    panel3.Visible = true;
                    break;

                case ID.TOOLBAR_CURVE:
                    ButtonsCheckClear();
                    rbDrawCurve.IsChecked = true;
                    m_ToggleType = (int)ToggleMode.Curve;
                    FormMain.Instance.ContentForm.Cursor = Cursors.Default;
                    this.Cursor = Cursors.Default;
                    panel2.Visible = false;
                    panel3.Visible = true;
                    break;

                case ID.TOOLBAR_TEXT:
                    ButtonsCheckClear();
                    rbText.IsChecked = true;
                    m_ToggleType = (int)ToggleMode.Text;
                    FormMain.Instance.ContentForm.Cursor = Cursors.Default;
                    this.Cursor = Cursors.Default;
                    panel2.Visible = true;
                    panel3.Visible = false;
                    break;
                case ID.TOOLBAR_STRONG:
                    if(btnStrong.IsChecked)
                    {
                        btnStrong.IsChecked = false;
                        FormMain.Instance.ContentForm.Bold = false;
                    }
                    else
                    {
                        btnStrong.IsChecked = true;
                        FormMain.Instance.ContentForm.Bold = true;
                    }
                    break;
                case ID.TOOLBAR_LEAN:
                    if (btnLean.IsChecked)
                    {
                        btnLean.IsChecked = false;
                        FormMain.Instance.ContentForm.Lean = false;
                    }
                    else
                    {
                        btnLean.IsChecked = true;
                        FormMain.Instance.ContentForm.Lean = true;
                    }
                    break;
                case ID.TOOLBAR_UNDERLINE:
                    if (btnUnderline.IsChecked)
                    {
                        btnUnderline.IsChecked = false;
                        FormMain.Instance.ContentForm.UnderLine = false;
                    }
                    else
                    {
                        btnUnderline.IsChecked = true;
                        FormMain.Instance.ContentForm.UnderLine = true;
                    }
                    break;
            }

            ButtonsRefresh();
        }

        public void ButtonsCheckClear()
        {
            foreach(RibbonButton btn in m_arRibbonButtons)
            {
                btn.IsChecked = false;
                btn.Refresh();
            }
        }

        public void ButtonsRefresh()
        {
            foreach(RibbonButton btn in m_arRibbonButtons)
            {
                btn.Refresh();
            }
        }

        public void ButtonChecked(int Command)
        {
            foreach (RibbonButton btn in m_arRibbonButtons)
            {
                if (btn.ID == Command)
                {
                    ButtonsCheckClear();
                    btn.IsChecked = true;
                    btn.Refresh();
                    RunCommand(btn.ID);
                }
            }

        }

        public void ButtonToggle()
        {
            if(rbSelectArea.IsChecked)
            {
                rbSelectArea.IsChecked = false;
                rbSelectColor.IsChecked = true;
            }
            else if (rbSelectColor.IsChecked)
            {
                rbSelectColor.IsChecked = false;
                rbSelectArea.IsChecked = true;
            }

            rbSelectColor.Refresh();
            rbSelectArea.Refresh();
        }

        private void FormImageToolBar_MouseUp(object sender, MouseEventArgs e)
        {
            if (ToggleType == (int)ToggleMode.SelectColor)
            {
                FormMain.Instance.ContentForm.Cursor = Cursors.Default;
                this.Cursor = Cursors.Default;
                ButtonToggle();
            }
        }

        private void panel1_MouseUp(object sender, MouseEventArgs e)
        {
            if (ToggleType == (int)ToggleMode.SelectColor)
            {
                FormMain.Instance.ContentForm.Cursor = Cursors.Default;
                this.Cursor = Cursors.Default;
                ButtonToggle();
            }
        }

        private void FormImageToolBar_Load(object sender, EventArgs e)
        {
            SetTooltip(rbSelectArea, "영역 선택");
            SetTooltip(rbSelectColor, "색상 선택");
            SetTooltip(rbLineColor, "선 색상");
            SetTooltip(rbZoomIn, "확대");
            SetTooltip(rbZoomOut, "축소");
            SetTooltip(rbTranslate, "이동");
            SetTooltip(rbRotate, "회전");
            SetTooltip(rbDrawStraightLine, "직선");
            SetTooltip(rbDrawCurve, "곡선");
            SetTooltip(rbText, "텍스트");
            SetTooltip(btnLean, "기울이기");
            SetTooltip(btnStrong, "굵게");
            SetTooltip(btnUnderline, "밑줄");

            panel2.Visible = false;
            panel3.Visible = false;
            panel3.Location = panel2.Location;


            InstalledFontCollection installedFontCollection = new InstalledFontCollection();
            //m_fontFamilies = installedFontCollection.Families;
            int count = installedFontCollection.Families.Length;
            for (int i = 0; i < count; ++i)
            {
                string familyName = installedFontCollection.Families[i].Name;
                cboTextFont.Items.Add(familyName);
            }

            string strFontName = "맑은 고딕";
            cboTextFont.SelectedIndex = CheckFontName(strFontName);

            cboLineThick.SelectedIndex = 1;
            cboTextSize.SelectedIndex = 1;
        }

        private int CheckFontName(string szText)
        {
            //bool bFind = false;
            InstalledFontCollection installedFontCollection = new InstalledFontCollection();
            for (int j = 0; j < installedFontCollection.Families.Length; ++j)
            {
                string familyName = installedFontCollection.Families[j].Name;
                if (familyName == szText)
                {
                    //bFind = true;
                    return j;
                }
            }
            cboTextFont.SelectedText = szText;
            return -1;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            rbZoomIn.Enabled = true;
            rbZoomOut.Enabled = true;
        }

        public void SelectArea()
        {
            rbZoomIn.Enabled = true;
            rbZoomOut.Enabled = true;
            rbTranslate.Enabled = true;
            rbRotate.Enabled = true;
        }

        public void SelectAreaClear()
        {
            rbZoomIn.Enabled = false;
            rbZoomOut.Enabled = false;
            rbTranslate.Enabled = false;
            rbRotate.Enabled = false;
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void panel2_VisibleChanged(object sender, EventArgs e)
        {
            int width = 878;

            if (panel2.Visible == true && panel3.Visible == false)
            {
                panel1.SetBounds(panel1.Location.X, panel1.Location.Y, width - panel3.Width, panel1.Size.Height);
                pictureBox3.Visible = true;
            }
            else if(panel2.Visible == false && panel3.Visible == true)
            {
                panel1.SetBounds(panel1.Location.X, panel1.Location.Y, width - panel2.Width, panel1.Size.Height);
                pictureBox3.Visible = true;
            }
            else if(panel2.Visible == false && panel3.Visible == false)
            {
                panel1.SetBounds(panel1.Location.X, panel1.Location.Y, width - panel3.Width - panel2.Width, panel1.Size.Height);
                pictureBox3.Visible = false;
            }
        }

        private void panel1_Resize(object sender, EventArgs e)
        {

        }

        private void panel3_VisibleChanged(object sender, EventArgs e)
        {
            int width = 878;

            if (panel2.Visible == true && panel3.Visible == false)
            {
                panel1.SetBounds(panel1.Location.X, panel1.Location.Y, width - panel3.Width, panel1.Size.Height);
                pictureBox3.Visible = true;
            }
            else if (panel2.Visible == false && panel3.Visible == true)
            {
                panel1.SetBounds(panel1.Location.X, panel1.Location.Y, width - panel2.Width, panel1.Size.Height);
                pictureBox3.Visible = true;
            }
            else if (panel2.Visible == false && panel3.Visible == false)
            {
                panel1.SetBounds(panel1.Location.X, panel1.Location.Y, width - panel3.Width - panel2.Width, panel1.Size.Height);
                pictureBox3.Visible = false;
            }
        }

        private int m_currentThick = 2;
        public int CurrentThick
        {
            get { return m_currentThick; }
            set { m_currentThick = value; }
        }

        private string m_strTextFont = "맑은 고딕";
        public string TextFont
        {
            get { return m_strTextFont; }
            set { m_strTextFont = value; }
        }
        private int m_nTextSize = 9;
        public int TextSize
        {
            get { return m_nTextSize; }
            set { m_nTextSize = value; }
        }
        private void cboLineThick_SelectedIndexChanged(object sender, EventArgs e)
        {
            m_currentThick = cboLineThick.SelectedIndex + 1;
        }

        private void cboTextFont_SelectedIndexChanged(object sender, EventArgs e)
        {
            m_strTextFont = cboTextFont.Items[(cboTextFont.SelectedIndex)].ToString();
        }

        private void cboTextSize_SelectedIndexChanged(object sender, EventArgs e)
        {
            m_nTextSize = Convert.ToInt32(cboTextSize.Items[(cboTextSize.SelectedIndex)].ToString());
        }

        private void rbTranslate_Click(object sender, EventArgs e)
        {

        }
    }
}
