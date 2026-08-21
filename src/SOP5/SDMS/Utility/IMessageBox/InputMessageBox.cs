using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.Collections;

namespace SDMS.IMessageBox
{
    public class InputMessageBox
    {
        private static Color m_FrameColor = Color.FromArgb(60, 56, 71);
        public static Color FrameColor
        {
            get { return InputMessageBox.m_FrameColor; }
            set { InputMessageBox.m_FrameColor = value; }
        }

        private static Image m_CloseBtnImage = null;
        public static Image CloseButtonImage
        {
            get { return m_CloseBtnImage; }
            set { m_CloseBtnImage = value; }
        }


        private static Font m_DialogFont = new Font("맑은 고딕", 10.0f, FontStyle.Regular);
        public static Font Font
        {
            get { return m_DialogFont; }
            set { m_DialogFont = value; }
        }

        private static Color m_foreColor = Color.Black;
        public static Color ForeColor
        {
            get { return m_foreColor; }
            set { m_foreColor = value; }
        }

        private static Color m_backColor = Color.White;
        public static Color BackColor
        {
            get { return m_backColor; }
            set { m_backColor = value; }
        }

        private static Color m_btnForColor = Color.Black;
        public static Color ButtonForeColor
        {
            get { return InputMessageBox.m_btnForColor; }
            set { InputMessageBox.m_btnForColor = value; }
        }

        private static Color m_btnBackColor = Color.White;
        public static Color ButtonBackColor
        {
            get { return InputMessageBox.m_btnBackColor; }
            set { InputMessageBox.m_btnBackColor = value; }
        }

        private static List<string> m_descriptionList = new List<string>();
        private static int m_nLastReadDescriptionTextID = -1;

        private static void ReadDescriptionText(InputMessageBoxForm form)
        {
            string strSQL = "Select ID, Description from SensorReactionHistoryDescriptionText where ID > " + m_nLastReadDescriptionTextID.ToString();
            ArrayList arrResult = FormMain.Instance.DBManager.GetResultData(strSQL, 0);

            if (arrResult == null)
                return;

            int nResultCount = arrResult.Count;

            for (int i=0;i<nResultCount-1;i+=2)
            {
                DBUtility.VariousData<int> id = DBUtility.WebDBManager.GetIntField(arrResult[i].ToString());
                string strDescription = DBUtility.WebDBManager.GetStringField(arrResult[i + 1]);

                if (id == null || strDescription == null)
                    continue;

                if (m_nLastReadDescriptionTextID < id.Data)
                    m_nLastReadDescriptionTextID = id.Data;

                m_descriptionList.Add(strDescription);
            }

            form.SetAutoCompleteSource(m_descriptionList);
        }

        public static DialogResult Show(string text, string strDescriptionInfo, ref string strDescriptionText)
        {
            InputMessageBoxForm form = new InputMessageBoxForm();
            form.Text = text;
            form.Font = Font;
            form.BackColor = m_backColor;
            form.ForeColor = m_foreColor;
            form.DescriptionInfo = strDescriptionInfo;
            form.DialogText(text);
            form.SetButtons(MessageBoxButtons.OK);
            form.SetIcon(MessageBoxIcon.Information);
            form.StartPosition = FormStartPosition.CenterScreen;
            Size size = form.Size;

            ReadDescriptionText(form);

            InputMessageBoxFrame form2 = new InputMessageBoxFrame(form);
            form2.Size = new Size(size.Width + 10, size.Height + 15);
            form2.SetFrameColor(m_FrameColor);
            form2.ShowInTaskbar = false;
            form2.CloseButtonImage = m_CloseBtnImage;
            form2.StartPosition = FormStartPosition.CenterParent;
            form2.Sizable = false;
            DialogResult result = form2.ShowDialog();

            strDescriptionText = form.DescriptionText;
            return result;
        }

        public static DialogResult Show(IWin32Window owner, string text, string strDescriptionInfo, ref string strDescriptionText)
        {
            InputMessageBoxForm form = new InputMessageBoxForm();
            form.Text = text;
            form.Font = Font;
            form.BackColor = m_backColor;
            form.ForeColor = m_foreColor;
            form.DescriptionInfo = strDescriptionInfo;
            form.DialogText(text);
            form.SetButtons(MessageBoxButtons.OK);
            form.SetIcon(MessageBoxIcon.Information);
            form.StartPosition = FormStartPosition.CenterParent;
            Size size = form.Size;

            ReadDescriptionText(form);

            InputMessageBoxFrame form2 = new InputMessageBoxFrame(form);
            form2.Size = new Size(size.Width + 10, size.Height + 15);
            form2.SetFrameColor(m_FrameColor);
            form2.ShowInTaskbar = false;
            form2.CloseButtonImage = m_CloseBtnImage;
            form2.StartPosition = FormStartPosition.CenterParent;
            form2.Sizable = false;
            DialogResult result = form2.ShowDialog(owner); 

            strDescriptionText = form.DescriptionText;
            return result;
        }

        public static DialogResult Show(string text, string caption, string strDescriptionInfo, ref string strDescriptionText)
        {
            InputMessageBoxForm form = new InputMessageBoxForm();
            form.Text = caption;
            form.Font = Font;
            form.BackColor = m_backColor;
            form.ForeColor = m_foreColor;
            form.DescriptionInfo = strDescriptionInfo;
            form.DialogText(text);
            form.SetButtons(MessageBoxButtons.OK);
            form.SetIcon(MessageBoxIcon.Information);
            form.StartPosition = FormStartPosition.CenterParent;
            Size size = form.Size;

            ReadDescriptionText(form);

            InputMessageBoxFrame form2 = new InputMessageBoxFrame(form);
            form2.Size = new Size(size.Width + 10, size.Height + 15);
            form2.SetFrameColor(m_FrameColor);
            form2.ShowInTaskbar = false;
            form2.CloseButtonImage = m_CloseBtnImage;
            form2.StartPosition = FormStartPosition.CenterParent;
            form2.Sizable = false;
            DialogResult result = form2.ShowDialog();

            strDescriptionText = form.DescriptionText;
            return result;
        }

        public static DialogResult Show(IWin32Window owner, string text, string caption, string strDescriptionInfo, ref string strDescriptionText)
        {
            InputMessageBoxForm form = new InputMessageBoxForm();
            form.Text = caption;
            form.Font = Font;
            form.BackColor = m_backColor;
            form.ForeColor = m_foreColor;
            form.DescriptionInfo = strDescriptionInfo;
            form.DialogText(text);
            form.SetButtons(MessageBoxButtons.OK);
            form.SetIcon(MessageBoxIcon.Information);
            Size size = form.Size;

            ReadDescriptionText(form);

            InputMessageBoxFrame form2 = new InputMessageBoxFrame(form);
            form2.Size = new Size(size.Width + 10, size.Height + 15);
            form2.SetFrameColor(m_FrameColor);
            form2.ShowInTaskbar = false;
            form2.CloseButtonImage = m_CloseBtnImage;
            form2.StartPosition = FormStartPosition.CenterParent;
            form2.Sizable = false;
            DialogResult result = form2.ShowDialog(owner);

            strDescriptionText = form.DescriptionText;
            return result;
        }

        public static DialogResult Show(string text, string caption, MessageBoxButtons buttons, string strDescriptionInfo, ref string strDescriptionText)
        {
            InputMessageBoxForm form = new InputMessageBoxForm();
            form.Text = caption;
            form.Font = Font;
            form.BackColor = m_backColor;
            form.ForeColor = m_foreColor;
            form.DescriptionInfo = strDescriptionInfo;
            form.DialogText(text);
            form.SetButtons(buttons);
            form.SetIcon(MessageBoxIcon.Information);
            form.TopMost = true;
            Size size = form.Size;

            ReadDescriptionText(form);

            InputMessageBoxFrame form2 = new InputMessageBoxFrame(form);
            form2.Size = new Size(size.Width + 10, size.Height + 15);
            form2.SetFrameColor(m_FrameColor);
            form2.ShowInTaskbar = false;
            form2.CloseButtonImage = m_CloseBtnImage;
            form2.StartPosition = FormStartPosition.CenterScreen;
            form2.Sizable = false;
            form2.TopMost = true;
            DialogResult result = form2.ShowDialog();

            strDescriptionText = form.DescriptionText;
            return result;
        }

        public static DialogResult Show(IWin32Window owner, string text, string caption, MessageBoxButtons buttons, string strDescriptionInfo, ref string strDescriptionText)
        {
            InputMessageBoxForm form = new InputMessageBoxForm();
            form.Text = caption;
            form.Font = Font;
            form.BackColor = m_backColor;
            form.ForeColor = m_foreColor;
            form.DescriptionInfo = strDescriptionInfo;
            form.DialogText(text);
            form.SetButtons(buttons);
            form.SetIcon(MessageBoxIcon.Information);
            form.StartPosition = FormStartPosition.CenterParent;
            Size size = form.Size;

            ReadDescriptionText(form);

            InputMessageBoxFrame form2 = new InputMessageBoxFrame(form);
            form2.Size = new Size(size.Width + 10, size.Height + 15);
            form2.SetFrameColor(m_FrameColor);
            form2.ShowInTaskbar = false;
            form2.CloseButtonImage = m_CloseBtnImage;
            form2.Sizable = false;
            DialogResult result = form2.ShowDialog(owner);

            strDescriptionText = form.DescriptionText;
            return result;
        }

        public static DialogResult Show(string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon, string strDescriptionInfo, ref string strDescriptionText)
        {
            InputMessageBoxForm form = new InputMessageBoxForm();
            form.Text = caption;
            form.Font = Font;
            form.BackColor = m_backColor;
            form.ForeColor = m_foreColor;
            form.DescriptionInfo = strDescriptionInfo;
            form.DialogText(text);
            form.SetButtons(buttons);
            form.SetIcon(icon);
            form.StartPosition = FormStartPosition.CenterParent;
            Size size = form.Size;

            ReadDescriptionText(form);

            InputMessageBoxFrame form2 = new InputMessageBoxFrame(form);
            form2.Size = new Size(size.Width + 10, size.Height + 15);
            form2.SetFrameColor(m_FrameColor);
            form2.ShowInTaskbar = false;
            form2.CloseButtonImage = m_CloseBtnImage;
            form2.Sizable = false;
            DialogResult result = form2.ShowDialog();

            strDescriptionText = form.DescriptionText;
            return result;

        }

        public static DialogResult Show(IWin32Window owner, string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon, string strDescriptionInfo, ref string strDescriptionText)
        {
            InputMessageBoxForm form = new InputMessageBoxForm();
            form.Text = caption;
            form.Font = Font;
            form.BackColor = m_backColor;
            form.ForeColor = m_foreColor;
            form.DescriptionInfo = strDescriptionInfo;
            form.DialogText(text);
            form.SetButtons(buttons);
            form.SetIcon(icon);
            form.StartPosition = FormStartPosition.CenterParent;
            Size size = form.Size;

            ReadDescriptionText(form);

            InputMessageBoxFrame form2 = new InputMessageBoxFrame(form);
            form2.Size = new Size(size.Width + 10, size.Height + 15);
            form2.SetFrameColor(m_FrameColor);
            form2.ShowInTaskbar = false;
            form2.CloseButtonImage = m_CloseBtnImage;
            form2.Sizable = false;
            DialogResult result = form2.ShowDialog(owner);

            strDescriptionText = form.DescriptionText;
            return result;
        }
    }
}
