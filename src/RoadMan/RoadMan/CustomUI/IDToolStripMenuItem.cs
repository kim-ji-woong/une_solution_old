using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace UnE.Utility
{
    internal class IDToolStripMenuItem : ToolStripMenuItem
    {
        private int m_nCommandID = -1;
        public int CommandID
        {
            get { return m_nCommandID; }
            set { m_nCommandID = value; }
        }

    }

    public interface IMenuCommandOwner
    {
        void RunCommand(int nCommandID);
        void CheckedChanged(int nCommandID, bool bChecked);

        ToolStripStatusLabel GetStatusLabel();
    }

    
}
