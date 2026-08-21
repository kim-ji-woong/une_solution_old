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

namespace CCTVLocation
{
    public partial class FormCCTVList : Form
    {
        private string m_strDataFilePath = "";

        public string DataFilePath
        {
            get { return m_strDataFilePath; }
            set { m_strDataFilePath = value; }
        }

        public FormCCTVList(string strPath)
        {
            InitializeComponent();
            m_strDataFilePath = strPath;

            foreach (DataGridViewColumn column in dataGridView1.Columns)
            {
                column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Hide();
            FormMain.Instance.OnHideCCTVList();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            SaveDataFile(m_strDataFilePath);
        }

        // Return 값 : dxf file Path
        //             없으면 null 리턴
        public string Show(out List<CCTV> cctvList)
        {
            cctvList = null;

            string strDXFFilePath = null;
            dataGridView1.Rows.Clear();

            if (m_strDataFilePath.Length == 0)
                return strDXFFilePath;

            cctvList = new List<CCTV>();

            bool isFirst = true;
            StreamReader reader = new StreamReader(m_strDataFilePath, Encoding.UTF8);

            while (!reader.EndOfStream)
            {
                string strLine = reader.ReadLine().Trim();

                if (strLine.Length == 0)
                    continue;

                if (isFirst)
                {
                    isFirst = false;

                    if (!strLine.Contains('\t'))
                    {
                        strDXFFilePath = strLine;
                        continue;
                    }
                }

                List<string> datas = ReadLine(strLine);

                if (datas == null)
                    continue;

                DataGridViewRow row = MakeNewRow();
                row.Cells[0].Value = row.Index + 1;

                foreach (DataGridViewCell cell in row.Cells)
                {
                    cell.ReadOnly = true;
                }

                row.Cells[6].ReadOnly = false;

                CCTV cctv = new CCTV();
                row.Tag = cctv;
                cctv.Row = row;
                cctvList.Add(cctv);

                int nDataCount = datas.Count;

                for (int i = 0; i < nDataCount; i++)
                {
                    if (datas[i].Length > 0)
                        row.Cells[i + 1].Value = datas[i];
                    /*}
                    else
                    {
                        row.Cells[i + 1].Value = datas[i];
                    }*/

                    if (row.Cells[i + 1].Value != null)
                    {
                        if (i == 1)
                            cctv.CCTVName = row.Cells[i + 1].Value.ToString();
                        else if (i == 5)
                        {
                            int nIndex = colLocationOption.Items.IndexOf(row.Cells[i + 1].Value.ToString());

                            if (nIndex >= 0)
                                cctv.Option = (CCTV.LocationOption)nIndex;
                        }
                        else if (i == 6)
                        {
                            double x;

                            if (double.TryParse(row.Cells[i + 1].Value.ToString(), out x))
                            {
                                if (cctv.Position == null)
                                    cctv.Position = new UnE.Geometry.Vertex2D(x, 0.0);
                                else
                                    cctv.Position.SetVertex(x, cctv.Position.y);
                            }
                        }
                        else if (i == 7)
                        {
                            double y;

                            if (double.TryParse(row.Cells[i + 1].Value.ToString(), out y))
                            {
                                if (cctv.Position == null)
                                    cctv.Position = new UnE.Geometry.Vertex2D(0.0, y);
                                else
                                    cctv.Position.SetVertex(cctv.Position.x, y);
                            }
                        }
                    }
                }
            }

            reader.Close();

            base.Show();
            return strDXFFilePath;
        }

        private DataGridViewRow MakeNewRow()
        {
            if (dataGridView1.AllowUserToAddRows)
            {
                DataGridViewRow row = (DataGridViewRow)dataGridView1.Rows[dataGridView1.Rows.Count - 1].Clone();
                dataGridView1.Rows.Add(row);

                return dataGridView1.Rows[dataGridView1.Rows.Count - 2];
            }
            else
            {
                dataGridView1.AllowUserToAddRows = true;

                DataGridViewRow row = (DataGridViewRow)dataGridView1.Rows[dataGridView1.Rows.Count - 1].Clone();
                dataGridView1.Rows.Add(row);

                dataGridView1.AllowUserToAddRows = false;
            }

            return dataGridView1.Rows[dataGridView1.Rows.Count - 1];
        }

        private List<string> ReadLine(string strLine)
        {
            List<string> datas = new List<string>();

            int len = strLine.Length;
            int nBeginIndex = 0;

            for (int i=nBeginIndex;i<len;i++)
            {
                char ch = strLine.ElementAt(i);

                if (ch == '\t')
                {
                    if (nBeginIndex == i)
                        datas.Add("");
                    else
                    {
                        string strData = strLine.Substring(nBeginIndex, i - nBeginIndex);
                        datas.Add(strData);
                    }

                    nBeginIndex = i + 1;
                }
            }

            if (nBeginIndex < len - 1)
            {
                string strData = strLine.Substring(nBeginIndex);
                datas.Add(strData);
            }

            return datas;
        }

        private void FormCCTVList_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!FormMain.Instance.CloseApplication)
            {
                e.Cancel = true;
                this.Hide();
                FormMain.Instance.OnHideCCTVList();
            }
        }

        public bool SaveDataFile(string strDataFilePath)
        {
            if (strDataFilePath.Length == 0)
                return false;

            try
            {
                StreamWriter writer = new StreamWriter(strDataFilePath, false, Encoding.UTF8);

                if (FormMain.Instance.DXFFilePath.Length > 0)
                    writer.WriteLine(FormMain.Instance.DXFFilePath);
            
                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    string strLine = "";

                    for (int i = 1; i <= 5; i++)
                    {
                        if (strLine.Length == 0)
                            strLine = row.Cells[i].Value.ToString();
                        else
                            strLine += "\t" + row.Cells[1].Value.ToString();
                    }

                    for (int i = 6; i <= 8; i++)
                    {
                        strLine += "\t";

                        if (row.Cells[i].Value != null)
                            strLine += row.Cells[i].Value.ToString();
                    }

                    writer.WriteLine(strLine);
                }

                writer.Close();
                return true;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }

            return false;
        }

        public CCTV GetCurrentCCTV()
        {
            if (dataGridView1.SelectedCells.Count == 0)
                return null;

            int nRowIndex = dataGridView1.SelectedCells[0].RowIndex;

            if (nRowIndex < 0)
                return null;

            DataGridViewRow row = dataGridView1.Rows[nRowIndex];

            if (row.IsNewRow)
                return null;

            CCTV cctv = (CCTV)row.Tag;
            return cctv;
        }

        private void dataGridView1_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                CCTV cctv = GetCurrentCCTV();

                if (cctv != null)
                    FormMain.Instance.SelectCCTV(cctv);
            }
        }

        private void btnShowAll_Click(object sender, EventArgs e)
        {
            FormMain.Instance.ShowAll();
        }
    }

    public class CCTV
    {
        public enum LocationOption { Indoor = 0, Outdoor, Unknown };

        private string m_strCCTVName = null;
        private LocationOption m_option = LocationOption.Unknown;
        private UnE.Geometry.Vertex2D m_vPos = null;
        private DataGridViewRow m_row = null;

        public string CCTVName
        {
            get { return m_strCCTVName; }
            set { m_strCCTVName = value; }
        }

        public LocationOption Option
        {
            get { return m_option; }
            set { m_option = value; }
        }

        public UnE.Geometry.Vertex2D Position
        {
            get { return m_vPos; }
            set
            {
                m_vPos = value;

                if (m_row != null)
                {
                    if (m_vPos == null)
                    {
                        m_row.Cells[7].Value = null;
                        m_row.Cells[8].Value = null;
                    }
                    else
                    {
                        m_row.Cells[7].Value = m_vPos.x;
                        m_row.Cells[8].Value = m_vPos.y;
                    }
                }
            }
        }

        public DataGridViewRow Row
        {
            get { return m_row; }
            set { m_row = value; }
        }
    }
}
