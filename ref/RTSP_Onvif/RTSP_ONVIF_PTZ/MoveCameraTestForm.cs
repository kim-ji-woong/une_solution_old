using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RTSP_ONVIF
{
    public partial class MoveCameraTestForm : Form
    {
        ONVIF_PTZ_Manager manager = ONVIF_PTZ_Manager.Instance;
        public MoveCameraTestForm()
        {
            InitializeComponent();            
        }

        //Up btn
        private void button1_Click(object sender, EventArgs e)
        {
            manager.Move(2, PTZDirectionCode.UP);
        }
        //right
        private void button3_Click(object sender, EventArgs e)
        {
            
            manager.Move(2, PTZDirectionCode.RIGHT);
        }
        //left
        private void button4_Click(object sender, EventArgs e)
        {
            manager.Move(2, PTZDirectionCode.LEFT);
        }

        //downBtn
        private void button2_Click(object sender, EventArgs e)
        {
            manager.Move(2, PTZDirectionCode.DOWN);
        }

        //zoom in
        private void button2_Click_1(object sender, EventArgs e)
        {
            manager.Move(2, PTZDirectionCode.ZOOMIN);
        }

        //zoom out
        private void button5_Click(object sender, EventArgs e)
        {
            manager.Move(2, PTZDirectionCode.ZOOMOUT);
        }

        private void button3_MouseDown(object sender, MouseEventArgs e)
        {
            manager.Move(2, PTZDirectionCode.RIGHT);
        }

        private void button3_MouseUp(object sender, MouseEventArgs e)
        {
            //manager.Stop(2, PTZDirectionCode.RIGHT);
        }
    }
}
