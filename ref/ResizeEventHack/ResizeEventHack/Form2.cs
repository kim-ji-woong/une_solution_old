using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApplication5
{
	public partial class Form2 : Form
	{
		public Form2()
		{
			InitializeComponent();
		}

		private void Form2_SizeChanged(object sender, EventArgs e)
		{
			label1.Text = string.Format("Size : {0}, {1}", this.Width, this.Height);
		}

		private void button1_Click(object sender, EventArgs e)
		{
			this.Size = new Size(300, 400);
		}

		private void button2_Click(object sender, EventArgs e)
		{
			this.Size = new Size(400, 300);
		}
	}
}
