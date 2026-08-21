using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DivisysCCTVInfo
{
    public partial class FormEventViewer : Form, IEventOwner
    {
        public FormEventViewer()
        {
            InitializeComponent();
        }

        public void AddEvent(int nEventCode, string strData)
        {
            AddEvent(false, nEventCode, strData);
        }

        public void AddEvent(bool all, int nEventCode, string strData)
        {
            string strAll = textBoxAllEvent.Text;

            if (strAll.Length == 0)
                strAll = string.Format("{0} : {1}", nEventCode, strData);
            else
                strAll += string.Format("\r\n{0} : {1}", nEventCode, strData);

            textBoxAllEvent.Text = strAll;

            if (all == false)
            {
                string strHost = textBoxHostEvent.Text;

                if (strHost.Length == 0)
                    strHost = string.Format("{0} : {1}", nEventCode, strData);
                else
                    strHost += string.Format("\r\n{0} : {1}", nEventCode, strData);

                textBoxHostEvent.Text = strHost;
            }
        }

        private void TextBoxEvent_TextChanged(object sender, EventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            textBox.SelectionStart = textBox.Text.Length;
            textBox.ScrollToCaret();
        }
    }
}
