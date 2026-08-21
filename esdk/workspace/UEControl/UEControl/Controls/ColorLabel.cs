using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UnE.Controls
{
    public class ColorLabel : Label
    {
        private Color m_colorNormal = Color.Black;
        private Color m_colorMouseOver = Color.Black;
        private Color m_colorClicked = Color.Black;

        public Color ColorNomal
        {
            get { return m_colorNormal; }
            set { m_colorNormal = value; }
        }
        public Color ColorMouseOver
        {
            get { return m_colorMouseOver; }
            set { m_colorMouseOver = value; }
        }
        public Color ColorClicked
        {
            get { return m_colorClicked; }
            set { m_colorClicked = value; }
        }

        public ColorLabel()
        {
            this.MouseDown += ColorLabel_MouseDown;
            this.MouseUp += ColorLabel_MouseUp;
            this.MouseHover += ColorLabel_MouseHover;
            this.MouseLeave += ColorLabel_MouseLeave;
        }

        private void ColorLabel_MouseHover(object sender, EventArgs e)
        {
            this.ForeColor = m_colorMouseOver;
        }

        private void ColorLabel_MouseLeave(object sender, EventArgs e)
        {
            this.ForeColor = m_colorNormal;
        }

        private void ColorLabel_MouseDown(object sender, MouseEventArgs e)
        {
            this.ForeColor = m_colorClicked;
        }

        private void ColorLabel_MouseUp(object sender, MouseEventArgs e)
        {
            this.ForeColor = m_colorMouseOver;
        }
    }
}
