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

namespace UnESample
{
    public partial class FormCCTVList : Form
    {
        private const int CHANNEL_INDEX = 0;
        private const int IP_INDEX = 1;
        private const int PORT_INDEX = 2;
        private const int BUILDING_INDEX = 3;
        private const int LOCATION_INDEX = 4;
        private const int TYPE_INDEX = 5;
        private const int ID_INDEX = 6;
        private const int PW_INDEX = 7;
        private const int INDOOR_INDEX = 8;

        public FormCCTVList()
        {
            InitializeComponent();
            InitData("./pnu_cctv.txt");
        }

        private void InitData(string strPath)
        {
            StreamReader reader = new StreamReader(strPath, Encoding.GetEncoding("ks_c_5601-1987"), true);
            bool isFirstLine = true;

            while (reader.EndOfStream == false)
            {
                string strLine = reader.ReadLine().Trim();

                if (strLine.Length == 0)
                    continue;

                if (isFirstLine)
                {
                    isFirstLine = false;
                    continue;
                }

                string[] tokens = strLine.Split('\t');

                if (tokens.Count() != 9)
                    continue;

                string strBuildingName = tokens[0].Trim();
                string strChannel = tokens[1].Trim();
                string strIP = tokens[2].Trim();
                string strPort = tokens[3].Trim();
                string strLocation = tokens[4].Trim();
                string strType = tokens[5].Trim();
                string strID = tokens[6].Trim();
                string strPW = tokens[7].Trim();
                string strIndoor = tokens[8].Trim();

                AddRow(strBuildingName, strChannel, strIP, strPort, strLocation, strType, strID, strPW, strIndoor);
            }

            reader.Close();
        }

        private void AddRow(string strBuildingName, string strChannel, string strIP, string strPort, string strLocation, string strType, string strID, string strPW, string strIndoor)
        {
            int nRowIndex = dataGridView1.Rows.Add();

            if (nRowIndex < 0)
                return;

            DataGridViewRow row = dataGridView1.Rows[nRowIndex];
            row.Cells[CHANNEL_INDEX].Value = strChannel;
            row.Cells[IP_INDEX].Value = strIP;
            row.Cells[PORT_INDEX].Value = strPort;
            row.Cells[BUILDING_INDEX].Value = strBuildingName;
            row.Cells[LOCATION_INDEX].Value = strLocation;
            row.Cells[TYPE_INDEX].Value = strType;
            row.Cells[ID_INDEX].Value = strID;
            row.Cells[PW_INDEX].Value = strPW;
            row.Cells[INDOOR_INDEX].Value = strIndoor;
        }

        private void dataGridView1_CellMouseUp(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                if (e.RowIndex < 0)
                    return;

                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];

                int nChannel = -1;
                ushort nPort = 0;

                string strChannel = row.Cells[CHANNEL_INDEX].Value.ToString();
                string strIP = row.Cells[IP_INDEX].Value.ToString();
                string strPort = row.Cells[PORT_INDEX].Value.ToString();
                string strID = row.Cells[ID_INDEX].Value.ToString();
                string strPW = row.Cells[PW_INDEX].Value.ToString();

                if (int.TryParse(strChannel, out nChannel) == false || ushort.TryParse(strPort, out nPort) == false)
                    return;

                FormMain.Instace.Connect(strIP, nPort, nChannel, strID, strPW);
            }
        }
    }
}
