using System.Drawing;
using System.Windows.Forms;

namespace UnE.Utility
{
    public class UMessageBoxRibbon
    {
        private static Color m_FrameColor = Color.FromArgb(60, 56, 71);
        public static Color FrameColor
        {
            get { return UMessageBoxRibbon.m_FrameColor; }
            set { UMessageBoxRibbon.m_FrameColor = value; }
        }

        private static Image m_CloseBtnImage = null;
        public static Image CloseButtonImage
        {
            get { return m_CloseBtnImage; }
            set { m_CloseBtnImage = value; }
        }

        private static Image m_CloseBtnOverImage = null;
        public static Image CloseButtonOverImage
        {
            get { return m_CloseBtnOverImage; }
            set { m_CloseBtnOverImage = value; }
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
            get { return UMessageBoxRibbon.m_btnForColor; }
            set { UMessageBoxRibbon.m_btnForColor = value; }
        }

        private static Color m_btnBackColor = Color.White;
        public static Color ButtonBackColor
        {
            get { return UMessageBoxRibbon.m_btnBackColor; }
            set { UMessageBoxRibbon.m_btnBackColor = value; }
        }

        private static Color m_TitleColor = Color.White;
        public static Color TitleColor
        {
            get { return UMessageBoxRibbon.m_TitleColor; }
            set { UMessageBoxRibbon.m_TitleColor = value; }
        }

        private static double m_WindowRateWidth = 1d;
        public static double WindowRateWidth
        {
            get { return m_WindowRateWidth; }
            set { m_WindowRateWidth = value; }
        }

        private static double m_WindowRateHeight = 1d;
        public static double WindowRateHeight
        {
            get { return m_WindowRateHeight; }
            set { m_WindowRateHeight = value; }
        }

        public static DialogResult Show(string text)
        {
            UMessageBoxFormRibbon form = new UMessageBoxFormRibbon();
            form.WindowRateWidth = WindowRateWidth;
            form.WindowRateHeight = WindowRateHeight;
            form.Text = text;
            form.Font = Font;
            form.BackColor = m_backColor;
            form.ForeColor = m_foreColor;
            form.DialogText(text);            
            form.SetIcon(MessageBoxIcon.Information);
            form.UpdateControlSize();
            form.SetButtons(MessageBoxButtons.OK);
            form.StartPosition = FormStartPosition.CenterParent;
            Size size = form.Size;
            UMessageBoxFrameRibbon form2 = new UMessageBoxFrameRibbon(form);
            form2.Size = new Size(size.Width + 10, size.Height + 15);
            form2.WindowRateWidth = WindowRateWidth;
            form2.WindowRateHeight = WindowRateHeight;
            form2.SetFrameColor(m_FrameColor);
            form2.SetTitleColor(m_TitleColor);
            form2.ShowInTaskbar = false;
            form2.CloseButtonImage = m_CloseBtnImage;
            form2.CloseButtonOverImage = m_CloseBtnOverImage;
            form2.StartPosition = FormStartPosition.CenterScreen;
            form2.Sizable = false;
            form2.UpdateControlSize();
            return form2.ShowDialog();
        }
       
        public static DialogResult Show(IWin32Window owner, string text)
        {
            UMessageBoxFormRibbon form = new UMessageBoxFormRibbon();
            form.WindowRateWidth = WindowRateWidth;
            form.WindowRateHeight = WindowRateHeight;
            form.Text = text;
            form.Font = Font;
            form.BackColor = m_backColor;
            form.ForeColor = m_foreColor;
            form.DialogText(text);
            form.SetIcon(MessageBoxIcon.Information);
            form.UpdateControlSize();
            form.SetButtons(MessageBoxButtons.OK);
            form.StartPosition = FormStartPosition.CenterParent;
            Size size = form.Size;
            UMessageBoxFrameRibbon form2 = new UMessageBoxFrameRibbon(form);
            form2.Size = new Size(size.Width + 10, size.Height + 15);
            form2.WindowRateWidth = WindowRateWidth;
            form2.WindowRateHeight = WindowRateHeight;
            form2.SetFrameColor(m_FrameColor);
            form2.SetTitleColor(m_TitleColor);
            form2.ShowInTaskbar = false;
            form2.CloseButtonImage = m_CloseBtnImage;
            form2.CloseButtonOverImage = m_CloseBtnOverImage;
            form2.StartPosition = FormStartPosition.CenterScreen;
            form2.Sizable = false;
            form2.UpdateControlSize();
            return form2.ShowDialog(owner);
        }
       
        public static DialogResult Show(string text, string caption)
        {
            UMessageBoxFormRibbon form = new UMessageBoxFormRibbon();
            form.WindowRateWidth = WindowRateWidth;
            form.WindowRateHeight = WindowRateHeight;
            form.Text = caption;
            form.Font = Font;
            form.BackColor = m_backColor;
            form.ForeColor = m_foreColor;
            form.DialogText(text);
            form.SetIcon(MessageBoxIcon.Information);
            form.UpdateControlSize();
            form.SetButtons(MessageBoxButtons.OK);
            form.StartPosition = FormStartPosition.CenterParent;
            Size size = form.Size;
            UMessageBoxFrameRibbon form2 = new UMessageBoxFrameRibbon(form);
            form2.Size = new Size(size.Width + 10, size.Height + 15);
            form2.WindowRateWidth = WindowRateWidth;
            form2.WindowRateHeight = WindowRateHeight;
            form2.SetFrameColor(m_FrameColor);
            form2.SetTitleColor(m_TitleColor);
            form2.ShowInTaskbar = false;
            form2.CloseButtonImage = m_CloseBtnImage;
            form2.CloseButtonOverImage = m_CloseBtnOverImage;
            form2.StartPosition = FormStartPosition.CenterScreen;
            form2.Sizable = false;
            form2.UpdateControlSize();
            return form2.ShowDialog();
        }
        
        public static DialogResult Show(IWin32Window owner, string text, string caption)
        {
            UMessageBoxFormRibbon form = new UMessageBoxFormRibbon();
            form.WindowRateWidth = WindowRateWidth;
            form.WindowRateHeight = WindowRateHeight;
            form.Text = caption;
            form.Font = Font;
            form.BackColor = m_backColor;
            form.ForeColor = m_foreColor;
            form.DialogText(text);
            form.SetIcon(MessageBoxIcon.Information);
            form.UpdateControlSize();
            form.SetButtons(MessageBoxButtons.OK);
            Size size = form.Size;
            UMessageBoxFrameRibbon form2 = new UMessageBoxFrameRibbon(form);
            form2.Size = new Size(size.Width + 10, size.Height + 15);
            form2.WindowRateWidth = WindowRateWidth;
            form2.WindowRateHeight = WindowRateHeight;
            form2.SetFrameColor(m_FrameColor);
            form2.SetTitleColor(m_TitleColor);
            form2.ShowInTaskbar = false;
            form2.CloseButtonImage = m_CloseBtnImage;
            form2.CloseButtonOverImage = m_CloseBtnOverImage;
            form2.StartPosition = FormStartPosition.CenterScreen;
            form2.Sizable = false;
            form2.UpdateControlSize();
            return form2.ShowDialog(owner);
        }
        
        public static DialogResult Show(string text, string caption, MessageBoxButtons buttons)
        {
            UMessageBoxFormRibbon form = new UMessageBoxFormRibbon();
            form.WindowRateWidth = WindowRateWidth;
            form.WindowRateHeight = WindowRateHeight;
            form.Text = caption;
            form.Font = Font;
            form.BackColor = m_backColor;
            form.ForeColor = m_foreColor;
            form.DialogText(text);
            form.UpdateControlSize();
            form.SetButtons(buttons);
            form.SetIcon(MessageBoxIcon.Information);
            Size size = form.Size;

            UMessageBoxFrameRibbon form2 = new UMessageBoxFrameRibbon(form);
            form2.Size = new Size(size.Width + 10, size.Height + 15);
            form2.WindowRateWidth = WindowRateWidth;
            form2.WindowRateHeight = WindowRateHeight;
            form2.SetFrameColor(m_FrameColor);
            form2.SetTitleColor(m_TitleColor);
            form2.ShowInTaskbar = false;
            form2.CloseButtonImage = m_CloseBtnImage;
            form2.CloseButtonOverImage = m_CloseBtnOverImage;
            form2.StartPosition = FormStartPosition.CenterScreen;
            form2.Sizable = false;
            form2.UpdateControlSize();
            return form2.ShowDialog();
        }
        
        public static DialogResult Show(IWin32Window owner, string text, string caption, MessageBoxButtons buttons)
        {
            UMessageBoxFormRibbon form = new UMessageBoxFormRibbon();
            form.WindowRateWidth = WindowRateWidth;
            form.WindowRateHeight = WindowRateHeight;
            form.Text = caption;
            form.Font = Font;
            form.BackColor = m_backColor;
            form.ForeColor = m_foreColor;
            form.DialogText(text);
            form.SetIcon(MessageBoxIcon.Information);
            form.UpdateControlSize();
            form.SetButtons(buttons);            
            Size size = form.Size;
            UMessageBoxFrameRibbon form2 = new UMessageBoxFrameRibbon(form);
            form2.Size = new Size(size.Width + 10, size.Height + 15);
            form2.WindowRateWidth = WindowRateWidth;
            form2.WindowRateHeight = WindowRateHeight;
            form2.SetFrameColor(m_FrameColor);
            form2.SetTitleColor(m_TitleColor);
            form2.ShowInTaskbar = false;
            form2.CloseButtonImage = m_CloseBtnImage;
            form2.CloseButtonOverImage = m_CloseBtnOverImage;
            form2.Sizable = false;
            form2.UpdateControlSize();
            form2.StartPosition = FormStartPosition.CenterScreen;
            return form2.ShowDialog(owner);
        }
       
        public static DialogResult Show(string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon)
        {
            UMessageBoxFormRibbon form = new UMessageBoxFormRibbon();
            form.WindowRateWidth = WindowRateWidth;
            form.WindowRateHeight = WindowRateHeight;
            form.Text = caption;
            form.Font = Font;
            form.BackColor = m_backColor;
            form.ForeColor = m_foreColor;
            form.DialogText(text);
            form.SetIcon(icon);
            form.UpdateControlSize();
            form.SetButtons(buttons);
            
            Size size = form.Size;
            UMessageBoxFrameRibbon form2 = new UMessageBoxFrameRibbon(form);
            form2.Size = new Size(size.Width + 10, size.Height + 15);
            form2.WindowRateWidth = WindowRateWidth;
            form2.WindowRateHeight = WindowRateHeight;
            form2.SetFrameColor(m_FrameColor);
            form2.SetTitleColor(m_TitleColor);
            form2.ShowInTaskbar = false;
            form2.CloseButtonImage = m_CloseBtnImage;
            form2.CloseButtonOverImage = m_CloseBtnOverImage;
            form2.Sizable = false;
            form2.UpdateControlSize();
            form2.StartPosition = FormStartPosition.CenterScreen;
            return form2.ShowDialog();
    
        }
        
        public static DialogResult Show(IWin32Window owner, string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon)
        {
            UMessageBoxFormRibbon form = new UMessageBoxFormRibbon();
            form.WindowRateWidth = WindowRateWidth;
            form.WindowRateHeight = WindowRateHeight;
            form.Text = caption;
            form.Font = Font;
            form.BackColor = m_backColor;
            form.ForeColor = m_foreColor;
            form.DialogText(text);
            form.SetIcon(icon);
            form.UpdateControlSize();
            form.SetButtons(buttons);
            
            
            Size size = form.Size;

            UMessageBoxFrameRibbon form2 = new UMessageBoxFrameRibbon(form);
            form2.Size = new Size(size.Width + 10, size.Height + 15);
            form2.WindowRateWidth = WindowRateWidth;
            form2.WindowRateHeight = WindowRateHeight;
            form2.SetFrameColor(m_FrameColor);
            form2.SetTitleColor(m_TitleColor);
            form2.ShowInTaskbar = false;
            form2.CloseButtonImage = m_CloseBtnImage;
            form2.CloseButtonOverImage = m_CloseBtnOverImage;
            form2.Sizable = false;
            form2.UpdateControlSize();
            form2.StartPosition = FormStartPosition.CenterScreen;
            return form2.ShowDialog(owner);
        }  
 
    }
}
