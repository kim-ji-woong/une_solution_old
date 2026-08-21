using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.DirectX.AudioVideoPlayback;
using System.Diagnostics;


namespace WindowsFormsApplication3
{
	public partial class Form1 : Form
	{
		public Form1()
		{
			InitializeComponent();
		}

		private void Form1_Load(object sender, EventArgs e)
		{
			try
			{
				Video v = Video.FromFile("cctv_video01.wmv");
				v.Owner = panel1;
				v.Play();
			}
			catch (System.Exception ex)
			{
				Debug.WriteLine(ex);
			}
		
		}


	}
}
