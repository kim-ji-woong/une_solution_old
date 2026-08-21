using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DidUIEditor;

namespace DidViewer.uCustomize
{
    public class uPanel : Panel
    {
        private Page m_page = new Page();
        public Page Page
        {
            get { return m_page; }
            set { m_page = value; }
        }
        
        public uPanel()
        {
            this.DoubleBuffered = true;
        }
    }
}
