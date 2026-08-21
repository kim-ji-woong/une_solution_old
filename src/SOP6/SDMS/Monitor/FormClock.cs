using System;
using System.Globalization;
using System.Windows.Forms;

namespace SDMS
{
	public partial class FormClock : Form
	{
		[System.Runtime.InteropServices.DllImport("User32.dll", EntryPoint = "SetParent", ExactSpelling = false)]
		private static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndParent);

		private int m_nYear = 0;
		private int m_nMonth = 0;
		private int m_nDay = 0;

		public FormClock(Form frmParent)
		{
			InitializeComponent();
			SetParent(this.Handle, frmParent.Handle);

			SetDate(DateTime.Now);
			timer1.Start();
		}

		private void SetDate(DateTime dt)
		{
			m_nYear = dt.Year;
			m_nMonth = dt.Month;
			m_nDay = dt.Day;

			//string[] strMonth = new string[] { "JAN", "FEB", "MAR", "APR", "MAY", "JUN", "JUL", "AUG", "SEP", "OCT", "NOV", "DEC" };
			string[] strMonth = new string[] { "1월", "2월", "3월", "4월", "5월", "6월", "7월", "8월", "9월", "10월", "11월", "12월" };

			string strToday;
			string /*strMonth = "",*/ strDay = "";

			string szText = dt.ToString(new CultureInfo("ko-KR"));
			string[] tt = szText.Split(' ');
			szText = tt[0];
			string strWeek = dt.DayOfWeek.ToString();
			int nMonth = int.Parse(dt.ToString("MM"));

			strDay = dt.ToString("dd");

			string strTemp = strWeek.Remove(3, strWeek.Length - 3).ToUpper();

			//textBox1.ForeColor = Color.Goldenrod;

			strToday = strTemp + " " + strMonth[nMonth - 1] + " " + strDay;
			textBox1.Text = strToday;
			textBox1.Text = szText;
		}

		private void OnTimer(object sender, EventArgs e)
		{
			DateTime dtNow = DateTime.Now;
			clockControl.DigitText = dtNow.ToString("HH:mm:ss");
			ResetDate(dtNow);
		}

		public void ResetDate(DateTime dt)
		{
			if (dt.Year == m_nYear && dt.Month == m_nMonth && dt.Day == m_nDay)
				return;

			SetDate(dt);
		}
	}
}