
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace CCTVSingleViewer
{
    // TabPage 버튼 안보이고, Border도 없음
    public class TabControlBody : TabControl
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

    public class TabControlHeader : TabControl
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
            ItemSize = new System.Drawing.Size(80, 30);
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
