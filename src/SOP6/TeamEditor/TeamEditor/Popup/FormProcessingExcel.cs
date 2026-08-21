using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TeamEditor.Popup
{
    public partial class FormProcessingExcel : Form
    {
        private int m_nMaxRowCount = 0;
        private int m_nCurrentRowCount = 0;
        private int m_nMaxDotCount = 15;
        private int m_nCurrentDotCount = 1;

        public int RowCount
        {
            get { return m_nMaxRowCount; }
            set { m_nMaxRowCount = value; }
        }

        public FormProcessingExcel(int nRowCount)
        {
            InitializeComponent();

            m_nMaxRowCount = nRowCount;
            labelProcessing.Text = "";
        }

        public void SetRowCount(int nRowCount)
        {
            m_nCurrentRowCount = nRowCount;

            if (m_nMaxRowCount == 0)
                return;

            int nDotCount = m_nCurrentDotCount;
            string strText = string.Format("{0} / {1} ({2}%) ", m_nCurrentRowCount, m_nMaxRowCount, m_nCurrentRowCount * 100 / m_nMaxRowCount);

            for (int i = 0; i < nDotCount; i++)
                strText += ".";

            labelProcessing.Text = strText;
            m_nCurrentDotCount++;

            if (m_nCurrentDotCount >= m_nMaxDotCount)
                m_nCurrentDotCount = 1;

            this.Refresh();
        }

        public void WaitUntilBeginning()
        {
            int nDotCount = m_nCurrentDotCount;
            string strText = "";

            for (int i = 0; i < nDotCount; i++)
                strText += ".";

            labelProcessing.Text = strText;
            m_nCurrentDotCount++;

            if (m_nCurrentDotCount >= m_nMaxDotCount)
                m_nCurrentDotCount = 1;

            this.Refresh();
        }

        private void FormProcessingExcel_Load(object sender, EventArgs e)
        {
            Point ptParent = FormFrame.Instance.Location;
            Size sizeParent = FormFrame.Instance.Size;

            this.Location = new Point(ptParent.X + sizeParent.Width / 2 - this.Size.Width / 2, ptParent.Y + sizeParent.Height / 2 - this.Size.Height / 2);
        }
    }
}
