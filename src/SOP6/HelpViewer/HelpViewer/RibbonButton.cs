using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace HelpViewer
{
    class RibbonButton : Button
    {
        public enum TextPosition
        {
            BOTTOM = 1,
            RIGHT,
            NONE
        }

        private Image m_imgNormal = null;
        private Image m_imgChecked = null;
        private Image m_imgDisabled = null;
        private Image m_imgMouseOverBkgnd = null;
        private Image m_imgCheckedBkgnd = null;
        private Image m_imgDisabledBkgnd = null;
        private Image m_imgMouseOver = null;
        private Image m_imgClicked = null;
        private Image m_imgMouseClickedBkgnd = null;

        public Image ClickedBackgroundImage
        {
            get { return m_imgMouseClickedBkgnd; }
            set { m_imgMouseClickedBkgnd = value; }
        }
        protected bool m_isChecked = false;

        protected System.Drawing.Font m_font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
        protected System.Drawing.Brush m_brush = new System.Drawing.SolidBrush(Color.White);
        protected static StringFormat m_textFormat = TextData.GetStringFormat();

        protected bool m_isLClicked = false;
        protected bool m_isMouseOver = false;

        protected IRibbonButtonOwner m_owner = null;

        private static int m_nOriginInitButtonWidth = 60;

        public static int OriginInitButtonWidth
        {
            get { return m_nOriginInitButtonWidth; }
            set { m_nOriginInitButtonWidth = value; }
        }

        protected bool m_bCheckButton = false;
        public bool CheckButton
        {
            get { return m_bCheckButton; }
            set { m_bCheckButton = value; }
        }


        private int m_nInitButtonWidth = -1;

        public int InitButtonWidth
        {
            get { return m_nInitButtonWidth; }
            set { m_nInitButtonWidth = value; }
        }

        protected Font m_Font = null;
        public new virtual System.Drawing.Font Font
        {
            get { return m_Font; }
            set
            {
                m_Font = value;
                UpdateTextRect();
            }
        }

        // 이미지가 그려질 위치와 크기를 User가 지정할 것인지 여부
        protected bool m_useCustomImageRect = false;
        public bool UseCustomImageRect
        {
            get { return m_useCustomImageRect; }
            set { m_useCustomImageRect = value; }
        }

        protected Rectangle m_rectCustomImage = new Rectangle(0, 0, 32, 32);
        public Rectangle CustomImageRect
        {
            get { return m_rectCustomImage; }
            set { m_rectCustomImage = value; }
        }

        protected TextPosition m_textPos = TextPosition.BOTTOM;
        public RibbonButton.TextPosition TextPos
        {
            get { return m_textPos; }
            set
            {
                m_textPos = value;
                if (m_textPos == TextPosition.RIGHT)
                    m_textFormat = TextData.GetStringFormat(TextData.TextPosition.RIGHT);
                if (m_textPos == TextPosition.BOTTOM)
                    m_textFormat = TextData.GetStringFormat(TextData.TextPosition.BOTTOM);
                UpdateTextRect();
            }
        }

        private void UpdateTextRect()
        {
            Font font = (m_Font == null ? m_font : m_Font);
            Graphics g = this.CreateGraphics();
            SizeF size = g.MeasureString(base.Text, font);

            this.Size = new Size(m_nInitButtonWidth, this.Size.Height);

            if ((int)size.Width + 3 > this.Size.Width)
            {
                this.Size = new Size((int)size.Width + 3, this.Size.Height);
            }
            if (m_textPos == TextPosition.BOTTOM)
            {
                m_rect = new Rectangle(0, this.Size.Height - (int)size.Height - 8, this.Size.Width, (int)size.Height);
            }
            else if (m_textPos == TextPosition.RIGHT)
            {
                int nAddSize = 10;
                if (m_imgNormal != null)
                {
                    nAddSize += m_imgNormal.Width;
                }
                int x = this.Size.Width / 5 + nAddSize;
                int y = (this.Size.Height - (int)size.Height) / 2;
                //m_rect = new Rectangle(x, y , this.Size.Width - x, (int)size.Height);

                int width = size.Width > (int)size.Width ? (int)size.Width + 1 : (int)size.Width;
                m_rect = new Rectangle(x, y, width, (int)size.Height);
            }
        }

        protected Brush m_Brush = null;
        public override Color ForeColor
        {
            get { return base.ForeColor; }
            set
            {
                base.ForeColor = value;
                m_Brush = new SolidBrush(value);
            }
        }

        protected System.Drawing.Rectangle m_rect = new Rectangle();
        public Rectangle TextRect
        {
            get { return m_rect; }
        }

        protected Point m_ptTextLocation = new Point();
        public System.Drawing.Point TextLocation
        {
            get { return m_ptTextLocation; }
            set
            {
                m_ptTextLocation = value;
                //SetTextLocation(m_ptTextLocation.X, m_ptTextLocation.Y);
            }
        }

        private bool m_bUseTextLocation = false;
        public bool UseTextLocation
        {
            get { return m_bUseTextLocation; }
            set { m_bUseTextLocation = value; }
        }


        public Image NormalImage
        {
            get { return m_imgNormal; }
            set
            {
                m_imgNormal = value;
                UpdateTextRect();
            }
        }

        public Image CheckedImage
        {
            get { return m_imgChecked; }
            set { m_imgChecked = value; }
        }

        public Image DisabledImage
        {
            get { return m_imgDisabled; }
            set { m_imgDisabled = value; }
        }

        public Image MouseOverBkgndImage
        {
            get { return m_imgMouseOverBkgnd; }
            set { m_imgMouseOverBkgnd = value; }
        }
        public Image MouseOverImage
        {
            get { return m_imgMouseOver; }
            set { m_imgMouseOver = value; }
        }

        public Image CheckedBkgndImage
        {
            get { return m_imgCheckedBkgnd; }
            set { m_imgCheckedBkgnd = value; }
        }

        public Image DisabledBkgndImage
        {
            get { return m_imgDisabledBkgnd; }
            set { m_imgDisabledBkgnd = value; }
        }

        public Image ClickedImage
        {
            get { return m_imgClicked; }
            set { m_imgClicked = value; }
        }

        public bool IsChecked
        {
            get { return m_isChecked; }
            set { m_isChecked = value; }
        }

        public IRibbonButtonOwner Owner
        {
            get { return m_owner; }
            set { m_owner = value; }
        }

        private System.Windows.Forms.ToolTip mToolTip = null;

        public string ToolTipText
        {
            get
            {
                if (mToolTip == null)
                    return "";
                return mToolTip.GetToolTip(this);
            }
            set
            {
                if (mToolTip == null)
                {
                    mToolTip = new System.Windows.Forms.ToolTip();
                }
                mToolTip.SetToolTip(this, value);
            }
        }

        public override string Text
        {
            get { return base.Text; }
            set
            {
                base.Text = value;
                UpdateTextRect();
                if (mToolTip == null)
                {
                    mToolTip = new System.Windows.Forms.ToolTip();
                }
                mToolTip.SetToolTip(this, value);

            }
        }

        protected int m_nID = -1;
        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }

        public RibbonButton()
        {
            m_nInitButtonWidth = m_nOriginInitButtonWidth;

            this.MouseUp += new MouseEventHandler(RibbonButton_MouseUp);
            this.MouseDown += new MouseEventHandler(RibbonButton_MouseDown);
            this.MouseEnter += new EventHandler(RibbonButton_MouseEnter);
            this.MouseLeave += new EventHandler(RibbonButton_MouseLeave);
        }

        public RibbonButton(int nInitButtonWidth)
        {
            m_nInitButtonWidth = nInitButtonWidth;

            this.MouseUp += new MouseEventHandler(RibbonButton_MouseUp);
            this.MouseDown += new MouseEventHandler(RibbonButton_MouseDown);
            this.MouseEnter += new EventHandler(RibbonButton_MouseEnter);
            this.MouseLeave += new EventHandler(RibbonButton_MouseLeave);
        }


        void RibbonButton_MouseLeave(object sender, EventArgs e)
        {
            m_isMouseOver = false;

            if (m_owner != null)
            {
                ToolStripStatusLabel label = m_owner.GetStatusLabel();
                if (label != null)
                {
                    label.Text = "";
                    label.ToolTipText = "";
                }

            }
        }

        void RibbonButton_MouseEnter(object sender, EventArgs e)
        {
            m_isMouseOver = true;

            if (m_owner != null)
            {
                ToolStripStatusLabel label = m_owner.GetStatusLabel();
                if (label != null)
                {
                    label.Text = (sender as RibbonButton).ToolTipText;
                    label.ToolTipText = label.Text;
                }

            }
        }

        void RibbonButton_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
                m_isLClicked = true;

            if (m_owner != null)
                m_owner.OnRibbonButtonMouseDown(sender, e);
        }

        void RibbonButton_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                m_isLClicked = false;
                this.Invalidate();
            }

            if (m_owner != null)
                m_owner.OnRibbonButtonMouseUp(sender, e);
        }

        protected override void OnPaint(PaintEventArgs pevent)
        {
            base.OnPaintBackground(pevent);

            Font font = (m_Font == null ? m_font : m_Font);
            Brush brush = (m_Brush == null ? m_brush : m_Brush);

            if (Enabled)
            {
                if (m_isChecked)
                {
                    if (m_imgCheckedBkgnd != null)
                    {
                        //((Bitmap)m_imgCheckedBkgnd).SetResolution(pevent.Graphics.DpiX, pevent.Graphics.DpiY);

                        pevent.Graphics.DrawImage(m_imgCheckedBkgnd, 0, 0, this.Size.Width, this.Size.Height);
                    }
                }
                else
                {
                    if (m_isMouseOver && !m_isLClicked)
                    {
                        if (m_imgMouseOverBkgnd != null)
                        {
                            //((Bitmap)m_imgMouseOverBkgnd).SetResolution(pevent.Graphics.DpiX, pevent.Graphics.DpiY);
                            pevent.Graphics.DrawImage(m_imgMouseOverBkgnd, 0, 0, this.Size.Width, this.Size.Height);
                        }
                    }
                    else if (m_isMouseOver && m_isLClicked)
                    {
                        if (m_imgMouseClickedBkgnd != null)
                        {
                            //((Bitmap)m_imgMouseClickedBkgnd).SetResolution(pevent.Graphics.DpiX, pevent.Graphics.DpiY);
                            pevent.Graphics.DrawImage(m_imgMouseClickedBkgnd, 0, 0, this.Size.Width, this.Size.Height);
                        }
                    }
                }

                //base.OnPaint(pevent);

                if (m_isChecked)
                {
                    if (m_imgChecked != null)
                    {
                        ((Bitmap)m_imgChecked).SetResolution(pevent.Graphics.DpiX, pevent.Graphics.DpiY);
                        DrawImage(m_imgChecked, pevent.Graphics);
                    }
                    else if (m_imgNormal != null)
                    {
                        ((Bitmap)m_imgNormal).SetResolution(pevent.Graphics.DpiX, pevent.Graphics.DpiY);
                        DrawImage(m_imgNormal, pevent.Graphics);
                    }
                }
                else
                {
                    if (m_isMouseOver == false && m_imgNormal != null)
                    {
                        ((Bitmap)m_imgNormal).SetResolution(pevent.Graphics.DpiX, pevent.Graphics.DpiY);
                        DrawImage(m_imgNormal, pevent.Graphics);
                    }
                }
                if (m_isMouseOver && !m_isLClicked)
                {
                    if (m_imgMouseOver != null)
                    {
                        ((Bitmap)m_imgMouseOver).SetResolution(pevent.Graphics.DpiX, pevent.Graphics.DpiY);
                        DrawImage(m_imgMouseOver, pevent.Graphics);
                    }
                    //else
                    //    DrawImage(m_imgMouseOver, pevent.Graphics);
                }
                else if (m_isMouseOver && m_isLClicked)
                {
                    if (m_imgClicked != null)
                    {
                        ((Bitmap)m_imgClicked).SetResolution(pevent.Graphics.DpiX, pevent.Graphics.DpiY);
                        DrawImage(m_imgClicked, pevent.Graphics);
                    }
                }
            }
            else
            {
                if (m_imgDisabledBkgnd != null)
                {

                    pevent.Graphics.DrawImage(m_imgDisabledBkgnd, 0, 0, this.Size.Width, this.Size.Height);
                }

                if (m_imgDisabled != null)
                {
                    ((Bitmap)m_imgDisabled).SetResolution(pevent.Graphics.DpiX, pevent.Graphics.DpiY);
                    DrawImage(m_imgDisabled, pevent.Graphics);
                }
                else if (m_imgNormal != null)
                {
                    ((Bitmap)m_imgNormal).SetResolution(pevent.Graphics.DpiX, pevent.Graphics.DpiY);
                    DrawImage(m_imgNormal, pevent.Graphics);
                }
            }

            if (Text.Length > 0)
            {
                if (UseTextLocation)
                {
                    Rectangle f = new Rectangle(m_ptTextLocation, m_rect.Size);
                    pevent.Graphics.DrawString(Text, font, brush, f, m_textFormat);
                }
                else
                {
                    StringFormat format = TextData.GetStringFormat();
                    pevent.Graphics.DrawString(Text, font, brush, m_rect, format);
                }

            }
        }

        protected virtual void DrawImage(Image img, Graphics g)
        {
            if (UseCustomImageRect)
            {
                g.DrawImage(img, m_rectCustomImage);
            }
            else
            {
                if (m_textPos == TextPosition.BOTTOM)
                {
                    int x = (this.Size.Width - img.Width) / 2;

                    int y = 5;

                    if (this.Text == "")
                        y = (this.Size.Height - img.Height) / 2;
                    g.DrawImage(img, x, y);
                }
                else if (m_textPos == TextPosition.RIGHT)
                {

                    int x = this.Size.Width / 5;
                    if (this.Text == "")
                        x = (this.Size.Width - img.Width) / 2;

                    int y = (this.Size.Height - img.Height) / 2;


                    g.DrawImage(img, x, y);
                }
            }
        }

        public void SetTextLocation(int x, int y)
        {
            m_rect.X = x;
            m_rect.Y = y;
        }
    }

    interface IRibbonButtonOwner
    {
        void OnRibbonButtonMouseDown(object sender, MouseEventArgs e);
        void OnRibbonButtonMouseUp(object sender, MouseEventArgs e);

        ToolStripStatusLabel GetStatusLabel();
    }

    class TextData
    {
        public enum TextPosition
        {
            BOTTOM = 1,
            RIGHT,
            NONE
        }
        private string m_strText = "";
        private System.Drawing.Font m_font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
        private System.Drawing.Brush m_brush = new System.Drawing.SolidBrush(Color.Black);
        private System.Drawing.Rectangle m_rect = new Rectangle();
        protected static StringFormat m_defTextFormat = GetStringFormat();
        private StringFormat m_textFormat;

        public TextData()
        {
            m_textFormat = m_defTextFormat;
        }

        public string Text
        {
            get { return m_strText; }
            set { m_strText = value; }
        }

        public System.Drawing.Font Font
        {
            get { return m_font; }
            set { m_font = value; }
        }

        public System.Drawing.Brush Brush
        {
            get { return m_brush; }
            set { m_brush = value; }
        }

        public System.Drawing.Rectangle Rectangle
        {
            get { return m_rect; }
            set { m_rect = value; }
        }

        public StringFormat TextFormat
        {
            get { return m_textFormat; }
            set { m_textFormat = value; }
        }

        public static StringFormat GetStringFormat(TextPosition pos = TextPosition.BOTTOM)
        {
            StringFormat format = new StringFormat();

            if (pos == TextPosition.BOTTOM)
            {
                // Set the LineAlignment and Alignment properties for 
                // both StringFormat objects to different values.
                format.LineAlignment = StringAlignment.Center;
                format.Alignment = StringAlignment.Center;
            }
            else if (pos == TextPosition.RIGHT)
            {
                format.LineAlignment = StringAlignment.Far;
                format.Alignment = StringAlignment.Near;
            }


            return format;
        }
    }

    // TabPage 버튼 안보이고, Border도 없음
    class TabControlBody : TabControl
    {
        private const int TCM_ADJUSTRECT = 0x1328;

        protected override void WndProc(ref Message m)
        {
            //Hide the tab headers at run-time
            if (m.Msg == TCM_ADJUSTRECT)
            {
                m.Result = (IntPtr)1;
                return;
            }

            base.WndProc(ref m);
        }
    }

    class TabControlHeader : TabControl
    {
        public static bool N_PositionMode;
        public static bool N_PlusButton;

        private Color m_clrNoSelectedTab = Color.FromArgb(62, 62, 62);
        private Color m_clrSelectedList = Color.FromArgb(239, 162, 54);
        private Color m_clrSelectedResult = Color.FromArgb(218, 83, 79);

        public TabControlHeader()
        {
            //DrawMode = TabDrawMode.OwnerDrawFixed;
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw | ControlStyles.UserPaint | ControlStyles.SupportsTransparentBackColor, true);
            DoubleBuffered = true;
            SizeMode = TabSizeMode.Fixed;
            ItemSize = new System.Drawing.Size(120, 30);
            N_PositionMode = false;
            N_PlusButton = false;
            this.DrawMode = TabDrawMode.OwnerDrawFixed;
            SetWindowTheme(this.Handle, "", "");
            //var tab = new TabPadding(this);
        }

        [System.Runtime.InteropServices.DllImportAttribute("uxtheme.dll")]
        private static extern int SetWindowTheme(IntPtr hWnd, string appname, string idlist);

        //All Properties
        [System.ComponentModel.Description("Desides if the Tab Control will display in vertical mode."), System.ComponentModel.Category("Design"), System.ComponentModel.Browsable(true), System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Always)]
        public bool VerticalMode { get { return N_PositionMode; } set { N_PositionMode = value; if (N_PositionMode == true) { SetToVerticalMode(); } if (N_PositionMode == false) { SetToHorrizontalMode(); } } }

        //Method for all of the properties
        private void SetToHorrizontalMode() { ItemSize = new System.Drawing.Size(120, 30); this.Alignment = TabAlignment.Top; }
        private void SetToVerticalMode() { ItemSize = new System.Drawing.Size(30, 120); Alignment = TabAlignment.Left; }


        protected override void CreateHandle()
        {
            base.CreateHandle();
            Alignment = TabAlignment.Bottom;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Bitmap B = new Bitmap(Width, Height);

            Graphics G = Graphics.FromImage(B);

            G.Clear(Color.Gainsboro);

            //Color NonSelected = Color.FromArgb(62, 62, 62);
            //Color Selected = Color.FromArgb(0, 172, 219);

            SolidBrush NOSelect = new SolidBrush(m_clrNoSelectedTab);
            SolidBrush ISSelect = new SolidBrush(m_clrSelectedList);

            for (int i = 0; i <= TabCount - 1; i++)
            {
                Rectangle TabRectangle = GetTabRect(i);

                if (i == SelectedIndex)
                {
                    if (i == 0)
                        ISSelect.Color = m_clrSelectedList;
                    else
                        ISSelect.Color = m_clrSelectedResult;

                    //Tab is selected
                    G.FillRectangle(ISSelect, TabRectangle);
                }
                else
                {
                    //Tab is not selected
                    G.FillRectangle(NOSelect, TabRectangle);
                }

                StringFormat sf = new StringFormat();

                sf.LineAlignment = StringAlignment.Center;
                sf.Alignment = StringAlignment.Center;

                if (i == SelectedIndex && i == 0)
                    G.DrawString(TabPages[i].Text, this.Font, Brushes.Black, TabRectangle, sf);
                else
                    G.DrawString(TabPages[i].Text, this.Font, Brushes.White, TabRectangle, sf);

                TabPages[i].BackColor = Color.FromArgb(62, 62, 62);
            }

            e.Graphics.DrawImage(B, 0, 0);
            G.Dispose();
            B.Dispose();
            base.OnPaint(e);
        }
    }
}
