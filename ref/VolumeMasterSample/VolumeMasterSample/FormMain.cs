using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using VolumeMaster;

namespace VolumeMasterSample
{
    public partial class FormMain : Form
    {
        public FormMain()
        {
            InitializeComponent();
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            trackBarVolume.SetRange(0, 100);

            bool isMute = Volume.IsMute();
            int nVolume = Volume.GetVolume();

            if (isMute)
                radioMute.Checked = true;
            else
                radioUseSound.Checked = true;

            trackBarVolume.Value = nVolume;
        }

        private void trackBarVolume_ValueChanged(object sender, EventArgs e)
        {
            label1.Text = "볼륨 : " + trackBarVolume.Value.ToString();
        }

        private void btnApply_Click(object sender, EventArgs e)
        {
            bool mute = radioMute.Checked;
            int nVolume = trackBarVolume.Value;
            Volume.ChangeVolume(nVolume, mute);
        }
    }
}
