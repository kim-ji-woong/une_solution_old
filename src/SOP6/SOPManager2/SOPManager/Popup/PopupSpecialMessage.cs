using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SOPManager
{
    public partial class PopupSpecialMessage : Form
    {
        private class MessageItem
        {
            private int m_nID = -1;
            private string m_strText = "";

            public int ID
            {
                get { return m_nID; }
                set { m_nID = value; }
            }

            public string Text
            {
                get { return m_strText; }
                set { m_strText = value; }
            }

            public MessageItem()
            {
            }

            public MessageItem(int nID, string strText)
            {
                m_nID = nID;
                m_strText = strText;
            }

            public override string ToString()
            {
                return Text;
            }
        }

        private bool m_bLeftMouseDown = false;
        private Point m_ptMove = new Point();

        private Image m_btnOkNormalImg = null;
        public Image BtnOkNormalImg { set { btnOK.NormalImage = value; } }

        private Image m_btnOkClickedImg = null;
        public Image BtnOkClickedImg { set { btnOK.ClickedImage = value; } }

        private Image m_btnOkMouseOverImg = null;
        public Image BtnOkMouseOverImg { set { btnOK.MouseOverImage = value; } }

        private double m_WindowWidthRate = 1d;
        public double WindowWidthRate
        {
            get { return m_WindowWidthRate; }
            set { m_WindowWidthRate = value; }
        }
        
        private double m_WindowHeightRate = 1d;
        public double WindowHeightRate
        {
            get{ return m_WindowHeightRate; }
            set { m_WindowHeightRate = value; }
        }

        public PopupSpecialMessage()
        {
            InitializeComponent();

            comboBoxType.Items.Add(new MessageItem(0, "재난 발생 시각"));
            comboBoxType.Items.Add(new MessageItem(1, "재난 발생 장소"));
            comboBoxType.Items.Add(new MessageItem(2, "SOP 모드"));
            comboBoxType.Items.Add(new MessageItem(3, "유해화학물질"));
            comboBoxType.Items.Add(new MessageItem(4, "기후정보"));
            comboBoxType.Items.Add(new MessageItem(5, "지진"));
            comboBoxType.Items.Add(new MessageItem(6, "한글받침"));
            comboBoxType.Items.Add(new MessageItem(7, "알람"));

            btnOK.CustomImageRect = new Rectangle(0, 0, 69, 37);
            btnOK.InitButtonWidth = 69;
            btnOK.Size = new Size(69, 37);
            btnOK.UseCustomImageRect = true;
        }

        private void PopupSpecialMessage_Shown(object sender, EventArgs e)
        {
            if (comboBoxType.Items.Count > 0)
                comboBoxType.SelectedIndex = 0;       
        }

        public void UpdateControl()
        {
            UpdateWindowRate(this, WindowWidthRate, WindowHeightRate);
            UpdateWindowRate(label1, WindowWidthRate, WindowHeightRate);
            UpdateWindowRate(comboBoxType, WindowWidthRate, WindowHeightRate);
            UpdateWindowRate(panelHelp, WindowWidthRate, WindowHeightRate);
            UpdateWindowRate(btnOK, WindowWidthRate, WindowHeightRate);
        }

        public void UpdateWindowRate(Control ctl, double pWindowRateWidth, double pWindowRateHeight, String pFontFamily = "맑은 고딕")
        {
            if (ctl is UnE.GUI.RibbonButton || ctl.GetType().Name == "RibbonButton")
            {
                #region RibbonButton
                ((UnE.GUI.RibbonButton)ctl).CustomImageRect = new Rectangle(0, 0, (int)(ctl.Width * pWindowRateWidth), (int)(ctl.Height * pWindowRateHeight));
                ((UnE.GUI.RibbonButton)ctl).InitButtonWidth = ((UnE.GUI.RibbonButton)ctl).CustomImageRect.Width;
                ((UnE.GUI.RibbonButton)ctl).Size = new Size((int)(ctl.Width * pWindowRateWidth), (int)(ctl.Height * pWindowRateHeight));

                double fLabelFontSize = ctl.Font.Size * pWindowRateWidth;
                ctl.Font = new Font(pFontFamily, (float)fLabelFontSize, ctl.Font.Style);
             
                ((UnE.GUI.RibbonButton)ctl).TextLocation = new Point((int)(((UnE.GUI.RibbonButton)ctl).TextLocation.X * pWindowRateWidth), (int)(((UnE.GUI.RibbonButton)ctl).TextLocation.Y * pWindowRateHeight));
                #endregion
            }
            else if (ctl is ComboBox || ctl.GetType().Name == "ComboBox")
            {
                #region ComboBox
                ComboBox cbo = (ComboBox)ctl;
                float fLabelFontSize = (int)(cbo.Font.Size * pWindowRateWidth);
                cbo.Font = new Font(pFontFamily, fLabelFontSize, ctl.Font.Style);
                cbo.Size = new Size((int)(cbo.Size.Width * pWindowRateWidth), (int)(cbo.Size.Height * pWindowRateHeight));
                #endregion
            }

            else if (ctl is Label || ctl.GetType().Name == "Label")
            {
                #region Label
                double fLabelFontSize = ctl.Font.Size * pWindowRateWidth;
                ctl.Font = new Font(pFontFamily, (float)fLabelFontSize, ctl.Font.Style);

                if (((Label)ctl).AutoSize == false)
                {
                    ctl.Size = new Size((int)(ctl.Width * pWindowRateWidth), (int)(ctl.Height * pWindowRateHeight));
                }
                #endregion
            }
            else if (ctl is Panel || ctl.GetType().Name == "Panel")
            {
                ctl.Size = new Size((int)(ctl.Width * pWindowRateWidth), (int)(ctl.Height * pWindowRateHeight));
            }
            else if (ctl is Form || ctl.GetType().Name == "Form")
            {
                ctl.Size = new System.Drawing.Size((int)(ctl.Size.Width * pWindowRateWidth), (int)(ctl.Size.Height * pWindowRateHeight));
            }
            else
            {
                return;
            }

            ctl.Location = new Point((int)(ctl.Location.X * pWindowRateWidth), (int)(ctl.Location.Y * pWindowRateHeight));
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Escape)
            {
                this.Close();
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            Hide();
        }

        private void TitleBar_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                m_bLeftMouseDown = true;
                m_ptMove = this.PointToScreen(new Point(e.X, e.Y));
            }
        }

        private void TitleBar_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (m_bLeftMouseDown == true)
                {
                    Point pt = this.PointToScreen(new Point(e.X, e.Y));
                    int dx = pt.X - m_ptMove.X;
                    int dy = pt.Y - m_ptMove.Y;
                    if (!(dx == 0 && dy == 0))
                    {
                        Point ptCur = this.Location;
                        this.Location = new Point(ptCur.X + dx, ptCur.Y + dy);
                        m_ptMove.X += dx;
                        m_ptMove.Y += dy;
                    }
                }
            }
        }

        private void TitleBar_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
                m_bLeftMouseDown = false;
        }

        private void comboBoxType_SelectedIndexChanged(object sender, EventArgs e)
        {
            int nCount = panelHelp.Controls.Count;

            for (int i=0;i<nCount;i++)
            {
                panelHelp.Controls.RemoveAt(0);
            }

            Form frm = null;

            if (comboBoxType.SelectedIndex >= 0)
            {
                MessageItem item = (MessageItem)comboBoxType.Items[comboBoxType.SelectedIndex];
                int nIndex = item.ID;
                //int nIndex = comboBoxType.SelectedIndex;

                if (nIndex == 0)
                {
                    frm = new SOPManager.Popup.SpecialMessagePanels.FormSpecialMessageHelpTime();
                    ((SOPManager.Popup.SpecialMessagePanels.FormSpecialMessageHelpTime)frm).WindowWidthRate = WindowWidthRate;
                    ((SOPManager.Popup.SpecialMessagePanels.FormSpecialMessageHelpTime)frm).WindowHeightRate = WindowHeightRate;
                    ((SOPManager.Popup.SpecialMessagePanels.FormSpecialMessageHelpTime)frm).UpdateControl();
                }
                else if (nIndex == 1)
                {
                    frm = new SOPManager.Popup.SpecialMessagePanels.FormSpecialMessageHelpLocation();
                    ((SOPManager.Popup.SpecialMessagePanels.FormSpecialMessageHelpLocation)frm).WindowWidthRate = WindowWidthRate;
                    ((SOPManager.Popup.SpecialMessagePanels.FormSpecialMessageHelpLocation)frm).WindowHeightRate = WindowHeightRate;
                    ((SOPManager.Popup.SpecialMessagePanels.FormSpecialMessageHelpLocation)frm).UpdateControl();
                }
                else if (nIndex == 2)
                {
                    frm = new SOPManager.Popup.SpecialMessagePanels.FormSpecialMessageHelpSOPMode();
                    ((SOPManager.Popup.SpecialMessagePanels.FormSpecialMessageHelpSOPMode)frm).WindowWidthRate = WindowWidthRate;
                    ((SOPManager.Popup.SpecialMessagePanels.FormSpecialMessageHelpSOPMode)frm).WindowHeightRate = WindowHeightRate;
                    ((SOPManager.Popup.SpecialMessagePanels.FormSpecialMessageHelpSOPMode)frm).UpdateControl();
                }
                else if (nIndex == 3)
                {
                    frm = new SOPManager.Popup.SpecialMessagePanels.FormSpecialMessageHelpPSM();
                    ((SOPManager.Popup.SpecialMessagePanels.FormSpecialMessageHelpPSM)frm).WindowWidthRate = WindowWidthRate;
                    ((SOPManager.Popup.SpecialMessagePanels.FormSpecialMessageHelpPSM)frm).WindowHeightRate = WindowHeightRate;
                    ((SOPManager.Popup.SpecialMessagePanels.FormSpecialMessageHelpPSM)frm).UpdateControl();
                }
                else if (nIndex == 4)
                {
                    frm = new SOPManager.Popup.SpecialMessagePanels.FormSpecialMessageHelpClimate();
                    ((SOPManager.Popup.SpecialMessagePanels.FormSpecialMessageHelpClimate)frm).WindowWidthRate = WindowWidthRate;
                    ((SOPManager.Popup.SpecialMessagePanels.FormSpecialMessageHelpClimate)frm).WindowHeightRate = WindowHeightRate;
                    ((SOPManager.Popup.SpecialMessagePanels.FormSpecialMessageHelpClimate)frm).UpdateControl();
                }
                else if (nIndex == 5)
                {
                    frm = new SOPManager.Popup.SpecialMessagePanels.FormSpecialMessageHelpEarthquake();
                    ((SOPManager.Popup.SpecialMessagePanels.FormSpecialMessageHelpEarthquake)frm).WindowWidthRate = WindowWidthRate;
                    ((SOPManager.Popup.SpecialMessagePanels.FormSpecialMessageHelpEarthquake)frm).WindowHeightRate = WindowHeightRate;
                    ((SOPManager.Popup.SpecialMessagePanels.FormSpecialMessageHelpEarthquake)frm).UpdateControl();
                }
                else if (nIndex == 6)
                {
                    frm = new SOPManager.Popup.SpecialMessagePanels.FormSpecialMessageHelpHangul();
                    ((SOPManager.Popup.SpecialMessagePanels.FormSpecialMessageHelpHangul)frm).WindowWidthRate = WindowWidthRate;
                    ((SOPManager.Popup.SpecialMessagePanels.FormSpecialMessageHelpHangul)frm).WindowHeightRate = WindowHeightRate;
                    ((SOPManager.Popup.SpecialMessagePanels.FormSpecialMessageHelpHangul)frm).UpdateControl();
                }
                else if (nIndex == 7)
                {
                    frm = new SOPManager.Popup.SpecialMessagePanels.FormSpecialMessageHelpAlarm();
                    ((SOPManager.Popup.SpecialMessagePanels.FormSpecialMessageHelpAlarm)frm).WindowWidthRate = WindowWidthRate;
                    ((SOPManager.Popup.SpecialMessagePanels.FormSpecialMessageHelpAlarm)frm).WindowHeightRate = WindowHeightRate;
                    ((SOPManager.Popup.SpecialMessagePanels.FormSpecialMessageHelpAlarm)frm).UpdateControl();
                }
            }

            if (frm != null)
            {
                frm.TopLevel = false;
                frm.Parent = panelHelp;
                frm.Location = new Point(0, 0);
                frm.Dock = DockStyle.Fill;
                panelHelp.Controls.Add(frm);

                frm.Show();
            }
        }

        public static List<SOPParameter> GetSystemParameters(string strCagtegoryName, string strSubCategoryName)
        {
            List<SOPParameter> parameters = new List<SOPParameter>();

            SOPManager.Popup.SpecialMessagePanels.FormSpecialMessageHelpTime.GetParameters(parameters);
            SOPManager.Popup.SpecialMessagePanels.FormSpecialMessageHelpLocation.GetParameters(parameters, strCagtegoryName, strSubCategoryName);
            SOPManager.Popup.SpecialMessagePanels.FormSpecialMessageHelpSOPMode.GetParameters(parameters);
            SOPManager.Popup.SpecialMessagePanels.FormSpecialMessageHelpPSM.GetParameters(parameters, strCagtegoryName, strSubCategoryName);
            SOPManager.Popup.SpecialMessagePanels.FormSpecialMessageHelpClimate.GetParameters(parameters, strCagtegoryName, strSubCategoryName);
            SOPManager.Popup.SpecialMessagePanels.FormSpecialMessageHelpEarthquake.GetParameters(parameters, strCagtegoryName, strSubCategoryName);
            SOPManager.Popup.SpecialMessagePanels.FormSpecialMessageHelpHangul.GetParameters(parameters);
            SOPManager.Popup.SpecialMessagePanels.FormSpecialMessageHelpAlarm.GetParameters(parameters);

            return parameters;
        }


    }

    public class SOPParameter
    {
        //public enum VariableType { UNKNOWN, INTEGER, DOUBLE, STRING, BOOLEAN };

        private string m_strVariableName = "";
        private Sections.SectionDataDecision.VariableType m_type = Sections.SectionDataDecision.VariableType.UNKNOWN;
        private string m_strDescription = "";
        private int m_nNo = -1;

        public int No
        {
            get { return m_nNo; }
            set { m_nNo = value; }
        }

        public string VariableName
        {
            get { return m_strVariableName; }
            set { m_strVariableName = value; }
        }

        public Sections.SectionDataDecision.VariableType Type
        {
            get { return m_type; }
            set { m_type = value; }
        }

        public string Description
        {
            get { return m_strDescription; }
            set { m_strDescription = value; }
        }

        public SOPParameter Clone()
        {
            SOPParameter param = new SOPParameter();

            param.VariableName = this.VariableName;
            param.Type = this.Type;
            param.Description = this.Description;
            param.No = this.No;

            return param;
        }
    }
}
