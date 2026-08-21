using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnE.GUI;
using System.Drawing;

namespace FireManagement
{
    class TextPictureBoxEx : TextPictureBox
    {
        public TextPictureBoxEx() : base()
        {
            m_brushText = new SolidBrush(Color.Black);
            TEXT_FONT = new Font("맑은고딕", 11, FontStyle.Bold);
        }
    }
}
