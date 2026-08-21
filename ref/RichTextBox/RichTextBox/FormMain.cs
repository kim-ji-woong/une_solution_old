using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DBUtility;

namespace RichTextBox
{
    public partial class FormMain : Form
    {
        delegate Font Toggle_Style(Font font, bool isSelected);

        public FormMain()
        {
            InitializeComponent();

            SetButtonState(rtbBody.Font, rtbBody.SelectionColor);
        }

        private void rtbBody_SelectionChanged(object sender, EventArgs e)
        {
            SetButtonState(rtbBody.SelectionFont, rtbBody.SelectionColor);
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
            System.Diagnostics.Trace.WriteLine(hash);
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

        private void SelectButton(bool isSelected, Button btn)
        {
            if (isSelected)
                btn.BackColor = SystemColors.ControlDark;
            else
            {
                btn.BackColor = SystemColors.Control;
                btn.UseVisualStyleBackColor = true;
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

        private void ToggleButtonClick(Toggle_Style func, Button btn)
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

        private void btnBold_Click(object sender, EventArgs e)
        {
            Toggle_Style func = ToggleBold;
            ToggleButtonClick(func, (Button)sender);
        }

        private void btnItalic_Click(object sender, EventArgs e)
        {
            Toggle_Style func = ToggleItalic;
            ToggleButtonClick(func, (Button)sender);
        }

        private void btnUnderline_Click(object sender, EventArgs e)
        {
            Toggle_Style func = ToggleUnderline;
            ToggleButtonClick(func, (Button)sender);
        }

        private void btnStrikeout_Click(object sender, EventArgs e)
        {
            Toggle_Style func = ToggleStrikeout;
            ToggleButtonClick(func, (Button)sender);
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
                //rtbBody.SelectionFont = dlg.Font;
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
            this.Close();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.Filter = "RTF Files (*.rtf)|*.rtf";
            string defaultName = "제목없음";
            dlg.FileName = defaultName;

            if (dlg.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
            {
                rtbBody.SaveFile(dlg.FileName, RichTextBoxStreamType.RichText);
            }
        }

        private void btnLoadFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "RTF Files (*.rtf)|*.rtf";

            if (dlg.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
            {
                rtbBody.LoadFile(dlg.FileName);
            }
        }
    }
}
