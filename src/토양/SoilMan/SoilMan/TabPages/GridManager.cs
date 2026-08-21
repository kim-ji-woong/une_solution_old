using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;

namespace SoilMan.TabPages
{
    public class GridManager
    {
        protected string m_strSectionName = "";
        protected UnE.Controls.MergedDataGridView m_grid = null;

        public string SectionName
        {
            get { return m_strSectionName; }
        }

        public GridManager(string strSectionName, UnE.Controls.MergedDataGridView grid)
        {
            m_strSectionName = strSectionName;
            m_grid = grid;
        }

        public virtual void ReadConfig(string strPath)
        {
            if (!File.Exists(strPath))
                return;

            // EUC-KR : 51949
            Encoding encEUC_KR = System.Text.Encoding.GetEncoding(51949);
            StreamReader reader = new StreamReader(strPath, encEUC_KR);

            bool sectionBegin = false;

            while (!reader.EndOfStream)
            {
                string strLine = reader.ReadLine().Trim();

                if (!sectionBegin)
                {
                    if (strLine == m_strSectionName)
                        sectionBegin = true;
                }
                else
                {
                    if (strLine.Length == 0)
                        break;

                    if (strLine.StartsWith("["))
                        break;

                    List<string> datas = ParseLine(strLine);
                    AddGrid(datas);
                }
            }

            reader.Close();
        }

        protected List<string> ParseLine(string strLine)
        {
            List<string> tokens = new List<string>();
            int nBeginIndex = 0;

            while (true)
            {
                int nIndex = strLine.IndexOf('\t', nBeginIndex);

                if (nIndex < 0)
                {
                    if (nBeginIndex == strLine.Length)
                        tokens.Add("");
                    else
                    {
                        string str = strLine.Substring(nBeginIndex, strLine.Length - nBeginIndex);
                        tokens.Add(str);
                    }

                    break;
                }

                if (nBeginIndex == nIndex)
                    tokens.Add("");
                else
                {
                    string str = strLine.Substring(nBeginIndex, nIndex - nBeginIndex);
                    tokens.Add(str);
                }

                nBeginIndex = nIndex + 1;
            }

            return tokens;
        }

        protected virtual void AddGrid(List<string> datas)
        {
            DataGridViewRow row = new DataGridViewRow();

            int nColumnCount = m_grid.Columns.Count;
            int nDataCount = datas.Count;

            for (int i = -1; i < nDataCount; i++)
            {
                if (i >= nColumnCount)
                    break;

                DataGridViewTextBoxCell cell = new DataGridViewTextBoxCell();

                if (i < 0)
                {
                    // Index 부여
                    cell.Value = m_grid.Rows.Count + 1;
                }
                else
                {
                    string strData = datas[i];

                    if (strData.Length > 0)
                        cell.Value = strData;
                }

                row.Cells.Add(cell);
            }

            int nCellCount = row.Cells.Count;

            for (int i = nCellCount; i < nColumnCount; i++)
            {
                DataGridViewTextBoxCell cell = new DataGridViewTextBoxCell();
                row.Cells.Add(cell);
            }

            m_grid.Rows.Add(row);
            ResetAmount(m_grid.Rows.Count - 1);
        }

        public virtual void ResetAmount(int nRowIndex)
        {
        }
    }
}
