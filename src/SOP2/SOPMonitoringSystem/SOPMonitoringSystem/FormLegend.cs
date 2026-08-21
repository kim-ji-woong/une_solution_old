using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SOPMonitoringSystem
{
    public partial class FormLegend : Form
    {
        public FormLegend()
        {
            InitializeComponent();
            Init();
            //ChangeBackColor(2, "4294957567");
        }

        private void Init()
        {
            Font font = new Font("Tahoma", 7, FontStyle.Bold);
            pictureBox1.Size = pictureBox2.Size = pictureBox3.Size = pictureBox4.Size = pictureBox5.Size = pictureBox6.Size = pictureBox7.Size =  new Size(20, 7);
            pictureBox4.Image = Properties.Resources.arrow1;
            pictureBox4.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox4.BackColor = Color.Transparent;
            pictureBox6.Location = new Point(5, 3);
            pictureBox6.Image = Properties.Resources.arrow2;
            pictureBox6.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox6.BackColor = Color.Transparent;
            pictureBox4.Location = new Point(5, 12);
            pictureBox3.Location = new Point(5, 21);
            pictureBox1.Location = new Point(5, 30);
            pictureBox2.Location = new Point(5, 39);
            pictureBox5.Location = new Point(5, 48);
            pictureBox7.Location = new Point(5, 57);
            pictureBox3.BorderStyle = pictureBox1.BorderStyle = pictureBox2.BorderStyle = pictureBox5.BorderStyle = pictureBox7.BorderStyle = BorderStyle.FixedSingle;

            label1.Font = label2.Font = label3.Font = label4.Font = label5.Font = label6.Font = label7.Font = font;
            label4.Location = new Point(27, 4);
            label6.Location = new Point(27, 13);
            label3.Location = new Point(27, 22);
            label1.Location = new Point(27, 31);
            label2.Location = new Point(27, 40);
            label5.Location = new Point(27, 49);
            label7.Location = new Point(27, 58);
        }

        public void ChangeBackColor(int num, Color color) // 0 : 대기 업무 / 1: 실행중 업무 / 2: 완료된 업무 / 3: 건너뛴 업무 / 4: 실행하지 않은 프로세스
        {
            //Color colors = Color.FromArgb((int)UInt32.Parse(color));
            switch (num)
            {
                case 0:
                    pictureBox7.BackColor = color;
                    break;
                case 1:
                    pictureBox2.BackColor = color;
                    break;
                case 2:
                    pictureBox5.BackColor = color;
                    break;
                case 3:
                    pictureBox1.BackColor = color;
                    break;
                case 4:
                    pictureBox3.BackColor = color;
                    break;
            }
        }
    }
}
