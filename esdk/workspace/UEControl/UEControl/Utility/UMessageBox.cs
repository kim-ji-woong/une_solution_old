using System.Drawing;
using System.Windows.Forms;

namespace UnE.Utility
{   
    public class UMessageBox
    {
        private static Color m_FrameColor = Color.FromArgb(60, 56, 71);
        public static Color FrameColor
        {
            get { return UMessageBox.m_FrameColor; }
            set { UMessageBox.m_FrameColor = value; }
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
            get { return UMessageBox.m_btnForColor; }
            set { UMessageBox.m_btnForColor = value; }
        }

        private static Color m_btnBackColor = Color.White;
        public static Color ButtonBackColor
        {
            get { return UMessageBox.m_btnBackColor; }
            set { UMessageBox.m_btnBackColor = value; }
        }

        public static DialogResult Show(string text)
        {
            UMessageBoxForm form = new UMessageBoxForm();
            form.Text = text;
            form.Font = Font;
            form.BackColor = m_backColor;
            form.ForeColor = m_foreColor;
            form.DialogText(text);
            form.SetButtons(MessageBoxButtons.OK);
            form.SetIcon(MessageBoxIcon.Information);
            form.StartPosition = FormStartPosition.CenterParent;
            Size size = form.Size;
            UMessageBoxFrame form2 = new UMessageBoxFrame(form);
            form2.Size = new Size(size.Width + 10, size.Height + 15);
            form2.SetFrameColor(m_FrameColor);
            form2.ShowInTaskbar = false;
            form2.CloseButtonImage = m_CloseBtnImage;
            form2.StartPosition = FormStartPosition.CenterParent;
            form2.Sizable = false;
            return form2.ShowDialog();
        }
       
        public static DialogResult Show(IWin32Window owner, string text)
        {
            UMessageBoxForm form = new UMessageBoxForm();
            form.Text = text;
            form.Font = Font;
            form.BackColor = m_backColor;
            form.ForeColor = m_foreColor;
            form.DialogText(text);
            form.SetButtons(MessageBoxButtons.OK);
            form.SetIcon(MessageBoxIcon.Information);
            form.StartPosition = FormStartPosition.CenterParent;
            Size size = form.Size;
            UMessageBoxFrame form2 = new UMessageBoxFrame(form);
            form2.Size = new Size(size.Width + 10, size.Height + 15);
            form2.SetFrameColor(m_FrameColor);
            form2.ShowInTaskbar = false;
            form2.CloseButtonImage = m_CloseBtnImage;
            form2.StartPosition = FormStartPosition.CenterParent;
            form2.Sizable = false;
            return form2.ShowDialog(owner);
        }
       
        public static DialogResult Show(string text, string caption)
        {
            UMessageBoxForm form = new UMessageBoxForm();
            form.Text = caption;
            form.Font = Font;
            form.BackColor = m_backColor;
            form.ForeColor = m_foreColor;
            form.DialogText(text);
            form.SetButtons(MessageBoxButtons.OK);
            form.SetIcon(MessageBoxIcon.Information);
            form.StartPosition = FormStartPosition.CenterParent;
            Size size = form.Size;
            UMessageBoxFrame form2 = new UMessageBoxFrame(form);
            form2.Size = new Size(size.Width + 10, size.Height + 15);
            form2.SetFrameColor(m_FrameColor);
            form2.ShowInTaskbar = false;
            form2.CloseButtonImage = m_CloseBtnImage;
            form2.StartPosition = FormStartPosition.CenterParent;
            form2.Sizable = false;
            return form2.ShowDialog();
        }
        
        public static DialogResult Show(IWin32Window owner, string text, string caption)
        {
            UMessageBoxForm form = new UMessageBoxForm();
            form.Text = caption;
            form.Font = Font;
            form.BackColor = m_backColor;
            form.ForeColor = m_foreColor;
            form.DialogText(text);
            form.SetButtons(MessageBoxButtons.OK);
            form.SetIcon(MessageBoxIcon.Information);
            Size size = form.Size;
            UMessageBoxFrame form2 = new UMessageBoxFrame(form);
            form2.Size = new Size(size.Width + 10, size.Height + 15);
            form2.SetFrameColor(m_FrameColor);
            form2.ShowInTaskbar = false;
            form2.CloseButtonImage = m_CloseBtnImage;
            form2.StartPosition = FormStartPosition.CenterParent;
            form2.Sizable = false;
            return form2.ShowDialog(owner);
        }
        
        public static DialogResult Show(string text, string caption, MessageBoxButtons buttons)
        {
            UMessageBoxForm form = new UMessageBoxForm();
            form.Text = caption;
            form.Font = Font;
            form.BackColor = m_backColor;
            form.ForeColor = m_foreColor;
            form.DialogText(text);
            form.SetButtons(buttons);
            form.SetIcon(MessageBoxIcon.Information);
            Size size = form.Size;

            UMessageBoxFrame form2 = new UMessageBoxFrame(form);
            form2.Size = new Size(size.Width + 10, size.Height + 15);
            form2.SetFrameColor(m_FrameColor);
            form2.ShowInTaskbar = false;
            form2.CloseButtonImage = m_CloseBtnImage;
            form2.StartPosition = FormStartPosition.CenterParent;
            form2.Sizable = false;
            return form2.ShowDialog();
        }
        
        public static DialogResult Show(IWin32Window owner, string text, string caption, MessageBoxButtons buttons)
        {
            UMessageBoxForm form = new UMessageBoxForm();
            form.Text = caption;
            form.Font = Font;
            form.BackColor = m_backColor;
            form.ForeColor = m_foreColor;
            form.DialogText(text);
            form.SetButtons(buttons);
            form.SetIcon(MessageBoxIcon.Information);
            form.StartPosition = FormStartPosition.CenterParent;
            Size size = form.Size;
            UMessageBoxFrame form2 = new UMessageBoxFrame(form);
            form2.Size = new Size(size.Width + 10, size.Height + 15);
            form2.SetFrameColor(m_FrameColor);
            form2.ShowInTaskbar = false;
            form2.CloseButtonImage = m_CloseBtnImage;
            form2.Sizable = false;
            return form2.ShowDialog(owner);
        }
       
        public static DialogResult Show(string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon)
        {
            UMessageBoxForm form = new UMessageBoxForm();
            form.Text = caption;
            form.Font = Font;
            form.BackColor = m_backColor;
            form.ForeColor = m_foreColor;
            form.DialogText(text);
            form.SetButtons(buttons);
            form.SetIcon(icon);
            form.StartPosition = FormStartPosition.CenterParent;
            Size size = form.Size;
            UMessageBoxFrame form2 = new UMessageBoxFrame(form);
            form2.Size = new Size(size.Width + 10, size.Height + 15);
            form2.SetFrameColor(m_FrameColor);
            form2.ShowInTaskbar = false;
            form2.CloseButtonImage = m_CloseBtnImage;
            form2.Sizable = false;
            return form2.ShowDialog();
    
        }
        
        public static DialogResult Show(IWin32Window owner, string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon)
        {
            UMessageBoxForm form = new UMessageBoxForm();
            form.Text = caption;
            form.Font = Font;
            form.BackColor = m_backColor;
            form.ForeColor = m_foreColor;
            form.DialogText(text);
            form.SetButtons(buttons);
            form.SetIcon(icon);
            form.StartPosition = FormStartPosition.CenterParent;
            
            Size size = form.Size;

            UMessageBoxFrame form2 = new UMessageBoxFrame(form);
            form2.Size = new Size(size.Width + 10, size.Height + 15);
            form2.SetFrameColor(m_FrameColor);
            form2.ShowInTaskbar = false;
            form2.CloseButtonImage = m_CloseBtnImage;
            form2.Sizable = false;
            return form2.ShowDialog(owner);
        }  
 
    }
}
