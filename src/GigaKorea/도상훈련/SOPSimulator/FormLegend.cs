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
            pictureBoxSkip.Size = pictureBoxRunning.Size = pictureBoxWait.Size = pictureBoxProcessed.Size = pictureBoxComplete.Size = pictureBoxNotProcessed.Size = pictureBoxCurrent.Size =  new Size(20, 7);
            pictureBoxProcessed.Image = Properties.Resources.arrow1;
            pictureBoxProcessed.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBoxProcessed.BackColor = Color.Transparent;
            //pictureBoxNotProcessed.Location = new Point(5, 3);
            pictureBoxNotProcessed.Image = Properties.Resources.arrow2;
            pictureBoxNotProcessed.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBoxNotProcessed.BackColor = Color.Transparent;
            /*pictureBoxProcessed.Location = new Point(5, 12);
            pictureBoxWait.Location = new Point(5, 21);
            pictureBoxSkip.Location = new Point(5, 30);
            pictureBoxRunning.Location = new Point(5, 39);
            pictureBoxComplete.Location = new Point(5, 48);
            pictureBoxCurrent.Location = new Point(5, 57);*/
            pictureBoxWait.BorderStyle = pictureBoxSkip.BorderStyle = pictureBoxRunning.BorderStyle = pictureBoxComplete.BorderStyle = pictureBoxCurrent.BorderStyle = BorderStyle.FixedSingle;

            labelSkip.Font = labelRunning.Font = labelWait.Font = labelNotProcessed.Font = labelComplete.Font = labelProcessed.Font = labelCurrent.Font = font;
            /*labelNotProcessed.Location = new Point(27, 4);
            labelProcessed.Location = new Point(27, 13);
            labelWait.Location = new Point(27, 22);
            labelSkip.Location = new Point(27, 31);
            labelRunning.Location = new Point(27, 40);
            labelComplete.Location = new Point(27, 49);
            labelCurrent.Location = new Point(27, 58);*/
        }

        public void ChangeBackColor(int num, Color color) // 0 : 대기 업무 / 1: 실행중 업무 / 2: 완료된 업무 / 3: 건너뛴 업무 / 4: 실행하지 않은 프로세스
        {
            //Color colors = Color.FromArgb((int)UInt32.Parse(color));
            switch (num)
            {
                case 0:
                    pictureBoxCurrent.BackColor = color;
                    break;
                case 1:
                    pictureBoxRunning.BackColor = color;
                    break;
                case 2:
                    pictureBoxComplete.BackColor = color;
                    break;
                case 3:
                    pictureBoxSkip.BackColor = color;
                    break;
                case 4:
                    pictureBoxWait.BackColor = color;
                    break;
            }
        }
    }
}
