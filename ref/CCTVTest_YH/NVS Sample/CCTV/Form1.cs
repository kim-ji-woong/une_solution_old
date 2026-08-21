using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CCTV
{
    public partial class Form1 : Form
    {
        private const int CTL_AUX_ON = 0x4001;
        private const int CTL_AUX_OFF = 0x4002;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //axNVSViewerCtrl1.EnableAudio = false;
            //axNVSViewerCtrl1.EnableInternalCodec = true;
            //axNVSViewerCtrl1.CameraName = "#4-2 CWP";
            //axNVSViewerCtrl1.IPAddress = "172.20.130.240";
            //axNVSViewerCtrl1.CameraName = "1호기 보일러 5층AB3";
            axNVSViewerCtrl1.CameraName = "1호기 보일러 5.5층CD2";
            axNVSViewerCtrl1.IPAddress = "172.20.130.82";
            //axNVSViewerCtrl1.Port = 554;
            
            
            //axNVSViewerCtrl1.CameraModel = "WV-SC385";
            //axNVSViewerCtrl1.CaptionData = 255;
            bool selectChannel = axNVSViewerCtrl1.SelectChannel(4);
            bool success = axNVSViewerCtrl1.Preview();
            axNVSViewerCtrl1.ViewNum = 0;

            //axNVSViewerCtrl1.AboutBox();

            System.Diagnostics.Trace.WriteLine("Camera is " + success.ToString());

            //axNVSViewerCtrl1.PutCamCtl(CTL_AUX_ON, 1, 1);

            //axNVSViewerCtrl1.AboutBox();
        }
    }
}
