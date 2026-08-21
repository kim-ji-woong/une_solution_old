using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace libSplash
{
    public class Animator
    {
        private Timer m_timer = null;
        private List<Image> m_images = null;
        private int m_nImageCount = 0;
        private int m_nImageIndex = -1;
        private Control m_parentCtrl = null;
        private bool m_isRunning = false;

        public bool IsRunning
        {
            get { return m_isRunning; }
        }

        public Animator(List<Image> images, int fps, Control ctrl)
        {
            m_parentCtrl = ctrl;
            m_images = images;

            if (m_images != null)
            {
                m_nImageCount = m_images.Count;
                m_nImageIndex = 0;

                m_timer = new Timer();
                m_timer.Interval = 1000 / fps;
                m_timer.Tick += new System.EventHandler(OnTimer);
            }
        }

        public void Run()
        {
            if (m_timer != null)
            {
                m_timer.Start();
                m_isRunning = true;
            }
        }

        public void Stop()
        {
            if (m_timer != null)
            {
                m_timer.Stop();
                m_isRunning = false;
            }
        }

        private void OnTimer(object sender, EventArgs e)
        {
            m_parentCtrl.Invalidate();
        }

        public void Draw(Graphics g)
        {
            if (m_timer == null)
                return;

            g.DrawImage(m_images[m_nImageIndex++], 0, 0);

            if (m_nImageIndex >= m_nImageCount)
                m_nImageIndex = 0;
        }

        public void Draw(Graphics g, int x, int y, ref Color color)
        {
            if (m_timer == null)
                return;

            color = ((Bitmap)m_images[m_nImageIndex]).GetPixel(x, y);
            g.DrawImage(m_images[m_nImageIndex++], 0, 0);

            if (m_nImageIndex >= m_nImageCount)
                m_nImageIndex = 0;
        }
    }
}
