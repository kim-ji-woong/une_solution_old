using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

namespace WindowsFormsApplication4
{
	public partial class FormMain : Form
	{
		public FormMain()
		{
			InitializeComponent();

			pictureBox1.BackColor = Color.Red;
			pictureBox2.BackColor = Color.Black;
			mOverlayPane.LineColor = Color.Red;
			mOverlayPane.TextColor = Color.Black;
			mOverlayPane.DrawMode = 1;
			mOverlayPane.LineThick = 1.0f;

			mOverlayPane.Paint += new System.Windows.Forms.PaintEventHandler(mOverlayPane.OnPaint);
			mOverlayPane.MouseDown += new System.Windows.Forms.MouseEventHandler(mOverlayPane.OnMouseDown);
			mOverlayPane.MouseEnter += new System.EventHandler(mOverlayPane.OnMouseEnter);
			mOverlayPane.MouseLeave += new System.EventHandler(mOverlayPane.OnMouseLeave);
			mOverlayPane.MouseHover += new System.EventHandler(mOverlayPane.OnMouseHover);
			mOverlayPane.MouseMove += new System.Windows.Forms.MouseEventHandler(mOverlayPane.OnMouseMove);
			mOverlayPane.MouseUp += new System.Windows.Forms.MouseEventHandler(mOverlayPane.OnMouseUp);
			MouseWheel += new System.Windows.Forms.MouseEventHandler(mOverlayPane.OnMouseWheel);
		}

		private void button4_Click(object sender, EventArgs e)
		{
			
		}

		private void pictureBox1_Click(object sender, EventArgs e)
		{
			colorDialog1.Color = pictureBox1.BackColor;
			colorDialog1.AllowFullOpen = true;
			if (colorDialog1.ShowDialog() == DialogResult.OK)
			{
				pictureBox1.BackColor = colorDialog1.Color;
				mOverlayPane.LineColor = colorDialog1.Color;
			}
		}

		private void pictureBox2_Click(object sender, EventArgs e)
		{
			colorDialog1.Color = pictureBox2.BackColor;
			colorDialog1.AllowFullOpen = true;
			if (colorDialog1.ShowDialog() == DialogResult.OK)
			{
				pictureBox2.BackColor = colorDialog1.Color;
				mOverlayPane.TextColor = colorDialog1.Color;
			}
		}

		private void button1_Click(object sender, EventArgs e)
		{
			mOverlayPane.DrawMode = 1;			
		}

		private void button2_Click(object sender, EventArgs e)
		{
			mOverlayPane.DrawMode = 2;			
		}

		private void button3_Click(object sender, EventArgs e)
		{
			mOverlayPane.DrawMode = 3;			
		}

		private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
		{
			string szText = comboBox1.SelectedItem.ToString();
			if( szText == null || szText == "")
				return;

			szText = szText.Replace("px", "");

			float fLineThick = 1.0f;
			if (!float.TryParse(szText, out fLineThick))
			{				
				fLineThick = 1.0f;
			}

			mOverlayPane.LineThick = fLineThick;
			mOverlayPane.Focus();
		}

		private void button5_Click(object sender, EventArgs e)
		{
			mOverlayPane.Clear();
			mOverlayPane.Invalidate();
		}

		private void button4_Click_1(object sender, EventArgs e)
		{
			mOverlayPane.DrawMode = 6;
		}

		private void button7_Click(object sender, EventArgs e)
		{
			if(mOverlayPane.SelectObject != null)
			{
				mOverlayPane.Remove(mOverlayPane.SelectObject);
				mOverlayPane.SelectObject = null;
				mOverlayPane.Invalidate();
			}
		}

		private void button6_Click(object sender, EventArgs e)
		{
			mOverlayPane.DrawMode = 4;
		}

		private void button8_Click(object sender, EventArgs e)
		{
			mOverlayPane.DrawMode = 5;
		}

		
	}
}
