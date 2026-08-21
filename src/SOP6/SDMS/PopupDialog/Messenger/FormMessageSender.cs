using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DBUtility2;
using System.Collections;
using UnE.GUI;

namespace SDMS.PopupDialog
{
    public partial class FormMessageSender : PopupFormBase
    {
        public const int SDMS_PUBLIC_MESSAGE_TYPE = 0;

        delegate Font Toggle_Style(Font font, bool isSelected);
        private bool m_closeForm = false;

        private string m_strOriginSenderName = "";

        public bool CloseForm
        {
            get { return m_closeForm; }
            set
            {
                m_closeForm = value;

                if (value)
                    this.Close();
            }
        }

        public FormMessageSender()
        {
            InitializeComponent();
             
            this.DoubleBuffered = true;
            SetButtonState(rtbBody.Font, rtbBody.SelectionColor);
            SetSenderName();

            InitCtrlSize(this);
        }  

        private void rtbBody_SelectionChanged(object sender, EventArgs e)
        {
            SetButtonState(rtbBody.SelectionFont, rtbBody.SelectionColor);
        }

        private void btnBold_Click(object sender, EventArgs e)
        {
            Toggle_Style func = ToggleBold;
            ToggleButtonClick(func, (ImageButton)sender);
        }

        private void btnItalic_Click(object sender, EventArgs e)
        {
            Toggle_Style func = ToggleItalic;
            ToggleButtonClick(func, (ImageButton)sender);
        }

        private void btnUnderline_Click(object sender, EventArgs e)
        {
            Toggle_Style func = ToggleUnderline;
            ToggleButtonClick(func, (ImageButton)sender);
        }

        private void btnStrikeout_Click(object sender, EventArgs e)
        {
            Toggle_Style func = ToggleStrikeout;
            ToggleButtonClick(func, (ImageButton)sender);
        }

        private void btnColor_Click(object sender, EventArgs e)
        {
            ColorDialog dlg = new ColorDialog();
            dlg.Color = rtbBody.SelectionColor;            

            if (dlg.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
            {
                btnColor.BackColor = dlg.Color;

                int nSelectionStart = rtbBody.SelectionStart;
                int nSelectionLength = rtbBody.SelectionLength;

                if (nSelectionLength == 0)
                    rtbBody.SelectionColor = dlg.Color;
                else if (nSelectionLength > 0)
                {
                    for (int i = 0; i < nSelectionLength; i++)
                    {
                        rtbBody.Select(nSelectionStart + i, 1);
                        rtbBody.SelectionColor = dlg.Color;
                    }
                }

                rtbBody.Select(nSelectionStart, nSelectionLength);
                rtbBody.Focus();
            }
        }

        private void btnFont_Click(object sender, EventArgs e)
        {
            FontDialog dlg = new FontDialog();
            dlg.Font = rtbBody.SelectionFont;

            if (dlg.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
            {
                int nSelectionStart = rtbBody.SelectionStart;
                int nSelectionLength = rtbBody.SelectionLength;

                if (nSelectionLength == 0)
                    rtbBody.SelectionFont = dlg.Font;
                else if (nSelectionLength > 0)
                {
                    for (int i = 0; i < nSelectionLength; i++)
                    {
                        rtbBody.Select(nSelectionStart + i, 1);
                        rtbBody.SelectionFont = dlg.Font;
                    }
                }

                rtbBody.Select(nSelectionStart, nSelectionLength);
                rtbBody.Focus();
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (SendMessage())
            {
                this.Hide();

                textBoxTitle.Text = "";
                rtbBody.Text = "";
                textBoxSenderName.Text = m_strOriginSenderName;
            }
        }
         
        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Hide();

            textBoxTitle.Text = "";
            rtbBody.Text = "";
            textBoxSenderName.Text = m_strOriginSenderName; 
        }

        private void SetButtonState(Font font, Color color)
        {
            if (font != null)
            {
                SetBold(font.Bold);
                SetItalic(font.Italic);
                SetUnderline(font.Underline);
                SetStrikeout(font.Strikeout);
            }

            btnColor.BackColor = color;
            int hash = color.GetHashCode();
        }

        private Image imgBoldDefault = global::SDMS.Properties.Resources.MessageSender_Bold;
        private Image imgBoldClick = global::SDMS.Properties.Resources.MessageSender_Bold_Click;

        private Image imgItalicDefault = global::SDMS.Properties.Resources.MessageSender_Italic;
        private Image imgItalicClick = global::SDMS.Properties.Resources.MessageSender_Italic_Click;

        private Image imgUnderlineDefault = global::SDMS.Properties.Resources.MessageSender_Underline;
        private Image imgUnderlineClick = global::SDMS.Properties.Resources.MessageSender_Underline_Click;

        private Image imgStrikoutDefault = global::SDMS.Properties.Resources.MessageSender_Strikout;
        private Image imgStrikoutClick = global::SDMS.Properties.Resources.MessageSender_Strikout_Click;

        private Image imgColorDefault = global::SDMS.Properties.Resources.MessageSender_Color;
        private Image imgColorClick = global::SDMS.Properties.Resources.MessageSender_Color_Click;

        private Image imgFontDefault = global::SDMS.Properties.Resources.MessageSender_Font;
        private Image imgFontClick = global::SDMS.Properties.Resources.MessageSender_Font_Click;

        private void SelectButton(bool isSelected, ImageButton btn)
        {
            if (isSelected)
            {
                if (btn == btnBold)
                    btn.ImageNormal = imgBoldClick;
                else if (btn == btnItalic)
                    btn.ImageNormal = imgItalicClick;
                else if (btn == btnUnderline)
                    btn.ImageNormal = imgUnderlineClick;
                else if (btn == btnStrikeout)
                    btn.ImageNormal = imgStrikoutClick;
                else if (btn == btnColor)
                    btn.ImageNormal = imgColorClick;
                else if (btn == btnFont)
                    btn.ImageNormal = imgFontClick;

                btn.BackColor = SystemColors.ControlDark;
            }
            else
            {
                if (btn == btnBold)
                    btn.ImageNormal = imgBoldDefault;
                else if (btn == btnItalic)
                    btn.ImageNormal = imgItalicDefault;
                else if (btn == btnUnderline)
                    btn.ImageNormal = imgUnderlineDefault;
                else if (btn == btnStrikeout)
                    btn.ImageNormal = imgStrikoutDefault;
                else if (btn == btnColor)
                    btn.ImageNormal = imgColorDefault;
                else if (btn == btnFont)
                    btn.ImageNormal = imgFontDefault;

                btn.BackColor = SystemColors.Control;
                //btn.UseVisualStyleBackColor = true;
            }
        }

        private void ToggleStyle(ref Font font, FontStyle style, bool isSelected)
        {
            FontStyle fontStyle = GetFontStyle(font);

            if (!isSelected)
                fontStyle = (fontStyle & (~style));
            else
                fontStyle |= style;

            font = new Font(font, fontStyle);
        }

        private FontStyle GetFontStyle(Font font)
        {
            FontStyle style = FontStyle.Regular;

            if (font.Bold)
                style |= FontStyle.Bold;

            if (font.Italic)
                style |= FontStyle.Italic;

            if (font.Underline)
                style |= FontStyle.Underline;

            if (font.Strikeout)
                style |= FontStyle.Strikeout;

            return style;
        }

        private Font ToggleBold(Font font, bool isSelected)
        {
            ToggleStyle(ref font, FontStyle.Bold, isSelected);
            SetBold(font.Bold);
            return font;
        }

        private Font ToggleItalic(Font font, bool isSelected)
        {
            ToggleStyle(ref font, FontStyle.Italic, isSelected);
            SetItalic(font.Italic);
            return font;
        }

        private Font ToggleUnderline(Font font, bool isSelected)
        {
            ToggleStyle(ref font, FontStyle.Underline, isSelected);
            SetUnderline(font.Underline);
            return font;
        }

        private Font ToggleStrikeout(Font font, bool isSelected)
        {
            ToggleStyle(ref font, FontStyle.Strikeout, isSelected);
            SetStrikeout(font.Strikeout);
            return font;
        }

        private void ToggleButtonClick(Toggle_Style func, ImageButton btn)
        {
            bool isSelected = btn.BackColor != SystemColors.Control;

            int nSelectionStart = rtbBody.SelectionStart;
            int nSelectionLength = rtbBody.SelectionLength;

            if (nSelectionLength == 0)
                rtbBody.SelectionFont = func(rtbBody.SelectionFont, !isSelected);
            else if (nSelectionLength > 0)
            {
                for (int i = 0; i < nSelectionLength; i++)
                {
                    rtbBody.Select(nSelectionStart + i, 1);
                    rtbBody.SelectionFont = func(rtbBody.SelectionFont, !isSelected);
                }
            }

            rtbBody.Select(nSelectionStart, nSelectionLength);
            rtbBody.Focus();
        }

        private void SetBold(bool isBold)
        {
            SelectButton(isBold, btnBold);
        }

        private void SetItalic(bool isItalic)
        {
            SelectButton(isItalic, btnItalic);
        }

        private void SetUnderline(bool isUnderline)
        {
            SelectButton(isUnderline, btnUnderline);
        }

        private void SetStrikeout(bool isStrikeout)
        {
            SelectButton(isStrikeout, btnStrikeout);
        }

        private void FormMessageSender_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!m_closeForm)
            {
                e.Cancel = true;
                btnCancel_Click(null, null);
                return;
            }
        }

        public static string RTFUniqueKey = "~!@#$%^&*()_+";
        private bool SendMessage()
        {
            if (rtbBody.Text.Trim().Length == 0)
            {
                rtbBody.Focus();
                MessageBox.Show("공지할 내용을 입력하세요.", "확인");
            }
            else
            {
                int nID = GetMaxID();

                if (nID < 0)
                {
                    MessageBox.Show("DB에 연결할 수 없습니다.\r\n네트웍 연결상태를 확인하세요.", "경고");
                    return false;
                }

                DateTime dtNow = DateTime.Now;
                string strTime = string.Format("{0}-{1:00}-{2:00} {3:00}:{4:00}:{5:00}",
                    dtNow.Year, dtNow.Month, dtNow.Day, dtNow.Hour, dtNow.Minute, dtNow.Second);

                // '\''은 DB 입력이 안되기 때문에 특수문자로 변환시킨다.
                string strTitle = textBoxTitle.Text.Replace("\'", RTFUniqueKey);
                string strSenderName = textBoxSenderName.Text.Trim().Replace("\'", RTFUniqueKey);
                string strText = rtbBody.Text.Replace("\'", RTFUniqueKey);
                //string strRTF = rtbBody.Rtf.Replace('\'', (char)8);
                string strRTF = rtbBody.Rtf.Replace("\'", RTFUniqueKey);
                
                string strSQL = "Insert into SDMSMessage (ID, SendTime, Title, Text, RichTextFormat, SOPGenUserID, SenderName, MessageType, SiteID) ";
                strSQL += string.Format("values ({0}, '{1}', '{2}', '{3}', '{4}', {5}, '{6}', {7}, {8})",
                    nID, strTime, strTitle, strText, strRTF, FormMain.Instance.SOPGenUserID,
                    strSenderName, SDMS_PUBLIC_MESSAGE_TYPE, UnE.SOP.ProxySOP.Instance.SiteID);

                if (FormMain.Instance.DBManager.GetResultData(strSQL) == null)
                {
                    MessageBox.Show("메시지 전달에 실패하였습니다.\r\n다시 시도해 주세요.");
                }
                else
                    return true;
            }

            return false;
        }

        private int GetMaxID()
        {
            string strSQL = "Select max(ID) from SDMSMessage";
            ArrayList arrResult = FormMain.Instance.DBManager.GetResultData(strSQL);

            if (arrResult == null)
                return -1;

            if (arrResult.Count == 0)
                return 1;

            VariousData<int> nID = WebDBManager.GetIntField(arrResult[0].ToString());

            if (nID == null)
                return 1;

            return nID.Data + 1;
        }

        private void SetSenderName()
        {
            string strSQL = string.Format("Select NickName from SOPGenUser where ID = {0} and SiteID = {1}",
                FormMain.Instance.SOPGenUserID, UnE.SOP.ProxySOP.Instance.SiteID);
            ArrayList arrResult = FormMain.Instance.DBManager.GetResultData(strSQL);

            if (arrResult == null || arrResult.Count == 0)
                return;

            string strNickName = WebDBManager.GetStringField(arrResult[0]);

            if (strNickName != null)
            {
                textBoxSenderName.Text = strNickName;
                m_strOriginSenderName = strNickName;
            }
        }

        private void btnShowReceiveForm_Click(object sender, EventArgs e)
        {
            FormMain.Instance.ShowSDMSReceiveForm();
        } 
    }
}
