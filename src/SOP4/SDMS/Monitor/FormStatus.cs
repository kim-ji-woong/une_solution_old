using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using UnE.Win32;

namespace SDMS
{
	public partial class FormStatus : Form
	{
	

		//[DllImport("user32", CharSet = CharSet.Auto)]
		//public static extern IntPtr SendMessage(HandleRef hWnd, int msg, int wParam, ref PARAFORMAT lParam);

		//public const int PFM_SPACEBEFORE = 0x00000040;
        //public const int PFM_SPACEAFTER = 0x00000080;
        //public const int PFM_LINESPACING = 0x00000100;
        //public const int SCF_SELECTION = 1;
        //public const int EM_SETPARAFORMAT = 1095;

		public FormStatus(Form frmParent)
		{
			InitializeComponent();
			UnE.Win32.NativeMethods.SetParent(this.Handle, frmParent.Handle);
		}

		private void setLineFormat(byte rule, int space)
		{
            UnE.Win32.PARAFORMAT fmt = new UnE.Win32.PARAFORMAT();
			fmt.cbSize = Marshal.SizeOf(fmt);
            fmt.dwMask = UnE.Win32.NativeMethods.PFM_LINESPACING;
			fmt.dyLineSpacing = space;
			fmt.bLineSpacingRule = rule;
			richTextBox1.SelectAll();
            UnE.Win32.NativeMethods.SendMessage(new HandleRef(richTextBox1, richTextBox1.Handle),
                         UnE.Win32.NativeMethods.EM_SETPARAFORMAT,
                         UnE.Win32.NativeMethods.SCF_SELECTION,
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

	
}