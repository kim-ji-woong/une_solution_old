using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace SDMS
{
    public partial class FormStatus : Form
    {
        [DllImport("User32.dll", EntryPoint = "SetParent", ExactSpelling = false)]
        static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndParent);

        [DllImport("user32", CharSet = CharSet.Auto)]
        private static extern IntPtr SendMessage(HandleRef hWnd, int msg, int wParam, ref PARAFORMAT lParam);
        const int PFM_SPACEBEFORE = 0x00000040;
        const int PFM_SPACEAFTER  = 0x00000080;
        const int PFM_LINESPACING = 0x00000100;
        const int SCF_SELECTION = 1;
        const int EM_SETPARAFORMAT = 1095;

        public FormStatus(Form frmParent)
        {
            InitializeComponent();
            SetParent(this.Handle, frmParent.Handle);
        }

        private void setLineFormat(byte rule, int space)
        {
            PARAFORMAT fmt = new PARAFORMAT();
            fmt.cbSize = Marshal.SizeOf(fmt);
            fmt.dwMask = PFM_LINESPACING;
            fmt.dyLineSpacing = space;
            fmt.bLineSpacingRule = rule;
            richTextBox1.SelectAll();
            SendMessage(new HandleRef(richTextBox1, richTextBox1.Handle),
                         EM_SETPARAFORMAT,
                         SCF_SELECTION,
                         ref fmt
                       );
        }

        public void SetStatus(string strStatus)
        {
            setLineFormat(0, 0);

            richTextBox1.Font = new Font(richTextBox1.Font.Name, 24.0f, FontStyle.Bold);
            richTextBox1.SelectionAlignment = HorizontalAlignment.Center;
            richTextBox1.Text = strStatus;
        }

        public void SetStatus(string strPosition, string strStatus)
        {
            setLineFormat(4, 600);

            richTextBox1.Text = "";

            richTextBox1.SelectionFont = new Font(richTextBox1.Font.Name, 14.0f);
            richTextBox1.SelectionAlignment = HorizontalAlignment.Center;
            richTextBox1.AppendText(strPosition + "\r\n");

            richTextBox1.SelectionFont = new Font(richTextBox1.Font.Name, 24.0f, FontStyle.Bold);
            richTextBox1.AppendText(strStatus);
        }
    }

    [StructLayout( LayoutKind.Sequential )]
    public struct PARAFORMAT
    {
        public int cbSize;
        public uint dwMask;
        public short wNumbering;
        public short wReserved;
        public int dxStartIndent;
        public int dxRightIndent;
        public int dxOffset;
        public short wAlignment;
        public short cTabCount;
        [MarshalAs( UnmanagedType.ByValArray, SizeConst = 32 )]
        public int[] rgxTabs;
        // PARAFORMAT2 from here onwards
        public int dySpaceBefore;
        public int dySpaceAfter;
        public int dyLineSpacing;
        public short sStyle;
        public byte bLineSpacingRule;
        public byte bOutlineLevel;
        public short wShadingWeight;
        public short wShadingStyle;
        public short wNumberingStart;
        public short wNumberingStyle;
        public short wNumberingTab;
        public short wBorderSpace;
        public short wBorderWidth;
        public short wBorders;
    }
}
