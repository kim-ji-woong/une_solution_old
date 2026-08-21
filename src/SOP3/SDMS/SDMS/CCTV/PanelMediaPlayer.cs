using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.DirectX.AudioVideoPlayback;

namespace SDMS
{
    public partial class PanelMediaPlayer : Panel
    {
        private Video m_video = null;
        private string m_strURL = "";

        public string URL
        {
            get { return m_strURL; }
            set
            {
                m_video = Video.FromFile(value);
                m_strURL = value;
            }
        }

        public PanelMediaPlayer(string strFile)
        {
            InitializeComponent();

            m_video = Video.FromFile(strFile);
            m_video.Owner = this;
            m_strURL = strFile;

            m_video.Size = this.Size;
        }

        public PanelMediaPlayer(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
        }

        public void Play()
        {           
            if (m_video != null)
                m_video.Play();
        }

        public void Stop()
        {
            if (m_video != null)
                m_video.Stop();
        }
    }
}
