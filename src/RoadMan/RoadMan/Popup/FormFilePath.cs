using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RoadMan
{
	public partial class FormFilePath : Form
	{
		public FormFilePath()
		{
			InitializeComponent();
		}

		public string OriginalFilePath
		{
			get { return label1.Text; }
			set { label1.Text = value; }
		}

		public bool DXFType = true;
        private string m_strNewFilePath = "";

		public string NewFilePath
		{
            get { return m_strNewFilePath; }
			set { textBox1.Text = value; }
		}

		private void label1_Click(object sender, EventArgs e)
		{

		}

		private void button1_Click(object sender, EventArgs e)
		{
			if(DXFType == true)
			{
				openFileDialog1.Filter =
				"DXF files (*.dxf)|*.dxf|All files (*.*)|*.*";

				openFileDialog1.DefaultExt = "dxf";
				openFileDialog1.RestoreDirectory = true;
				openFileDialog1.Multiselect = false;
				openFileDialog1.FileName = "";
				if (openFileDialog1.ShowDialog() == DialogResult.OK)
				{
					string szFileName = openFileDialog1.FileName;
					textBox1.Text = szFileName;
				}
			}
			else
			{
				openFileDialog1.Filter =
				"Jpeg files (*.jpg)|*.jpg|Png files (*.png)|*.png|Bmp files (*.bmp)|*.bmp|All files (*.*)|*.*";

				openFileDialog1.DefaultExt = "png";
				openFileDialog1.RestoreDirectory = true;
				openFileDialog1.Multiselect = false;
				openFileDialog1.FileName = "";
				if(openFileDialog1.ShowDialog() == DialogResult.OK)
				{
					string szFileName = openFileDialog1.FileName;
					textBox1.Text = szFileName;
				}
			}			
		}

		private void button2_Click(object sender, EventArgs e)
		{
			if(textBox1.Text == "")
			{
				string szMsg = "변경파일을 반드시 지정해야 합니다.";
                UnE.Utility.UMessageBox.Show(this, szMsg, "파일 경로 오류", MessageBoxButtons.OK, MessageBoxIcon.Error);				
				return;
			}

            m_strNewFilePath = textBox1.Text;

			DialogResult = DialogResult.OK;
			this.Close();
		}
	}
}
