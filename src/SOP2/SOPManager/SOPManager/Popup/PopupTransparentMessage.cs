using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SOPManager
{
    public partial class PopupTransparentMessage : Form
    {
        public override string Text
        {
            get
            {
                if (labelMessage == null)
                    return base.Text;

                return labelMessage.Text;
            }
            set { labelMessage.Text = value; }
        }

        public string Text2
        {
            get { return labelMessage2.Text; }
            set { labelMessage2.Text = value; }
        }

        private int m_nInitPos1 = 0, m_nInitPos2 = 0;

        private bool m_useMultiLine = false;
        public bool UseMultiLine
        {
            get { return m_useMultiLine; }
            set
            {
                if (m_useMultiLine != value)
                {
                    m_useMultiLine = value;
                    UseMultiline(m_useMultiLine);
                }
            }
        }

        public PopupTransparentMessage()
        {
            InitializeComponent();

            m_nInitPos1 = labelMessage.Location.Y;
            m_nInitPos2 = labelMessage2.Location.Y;

            labelMessage.Text = labelMessage2.Text = "";
            UseMultiline(false);
        }

        private void UseMultiline(bool multiLine)
        {
            if (multiLine)
            {
                labelMessage.Location = new Point(labelMessage.Location.X, m_nInitPos1);
                labelMessage2.Location = new Point(labelMessage2.Location.X, m_nInitPos2);
                labelMessage2.Visible = true;
            }
            else
            {
                labelMessage.Location = new Point(labelMessage.Location.X, (panelWhiteBoard.Size.Height - labelMessage.Size.Height) / 2);
                labelMessage2.Visible = false;
            }
        }

        public new void Show()
        {
            base.Show();
            Refresh();
        }
    }
}
