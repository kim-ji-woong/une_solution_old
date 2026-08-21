using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DidUIEditor.Popups
{
    /// <summary>
    /// 동영상의 재생시간 설정
    /// </summary>
    public partial class FormMoviePropertySetting : Form
    {
        private Media m_media = null;

        public FormMoviePropertySetting(Media media)
        {
            InitializeComponent();

            m_media = media;

            numRunning.Minimum = 0;
            numBegin.Minimum = 0;

            MediaLoad();
        }

        private void MediaLoad()
        {
            numRunning.Value = m_media.RunningSeconds;
            numBegin.Value = m_media.BeginSeconds;
        }
        
        private void btnSave_Click(object sender, EventArgs e)
        {
            m_media.BeginSeconds = (int)numBegin.Value;
            m_media.RunningSeconds = (int)numRunning.Value;

            this.DialogResult = DialogResult.Yes;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.No;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.No;
        }
    }
}
