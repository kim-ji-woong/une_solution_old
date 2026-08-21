using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace UnECCTV
{
    public partial class FormCCTVList : Form
    {
        private string m_strPath = "resource.txt";

        public FormCCTVList()
        {
            InitializeComponent();
            ReadFile();
        }

        private void ReadFile()
        {
            if (File.Exists(m_strPath) == false)
                return;

            StreamReader reader = new StreamReader(m_strPath, Encoding.Default);

            while (reader.EndOfStream == false)
            {
                string strLine = reader.ReadLine().Trim();

                if (strLine.Length == 0)
                    continue;

                string[] tokens = strLine.Split('\t');

                int nID;

                if (int.TryParse(tokens[0].Trim(), out nID) == false)
                    continue;

                CCTV cctv = new CCTV();

                cctv.ID = nID;
                cctv.CameraName = tokens[1].Trim();

                if (tokens.Count() >= 3)
                    cctv.Channel1URL = tokens[2].Trim();

                if (tokens.Count() >= 4)
                    cctv.Channel2URL = tokens[3].Trim();

                if (tokens.Count() >= 5)
                    cctv.Channel3URL = tokens[4].Trim();

                int nRowIndex = gridCCTV.Rows.Add();
                DataGridViewRow row = gridCCTV.Rows[nRowIndex];

                row.Cells[0].Value = cctv.ID;
                row.Cells[1].Value = cctv.CameraName;

                if (tokens.Count() >= 3)
                    row.Cells[2].Value = cctv.Channel1URL.Length > 0;

                if (tokens.Count() >= 4)
                    row.Cells[3].Value = cctv.Channel2URL.Length > 0;

                if (tokens.Count() >= 5)
                    row.Cells[4].Value = cctv.Channel3URL.Length > 0;

                row.Tag = cctv;
            }

            reader.Close();
        }

        private void gridCCTV_CellMouseUp(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                if (e.RowIndex < 0)
                    return;

                DataGridViewRow row = gridCCTV.Rows[e.RowIndex];
                CCTV cctv = (CCTV)row.Tag;

                FormMain.Instance.SetCCTV(cctv);
            }
        }
    }
}
