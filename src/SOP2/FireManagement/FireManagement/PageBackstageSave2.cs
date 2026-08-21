using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace FireManagement
{
    public partial class PageBackstageSave2 : Form
    {
        public PageBackstageSave2()
        {
            InitializeComponent();

            if (!FormMain2.Instance.IsPCMode)
            {
                btnToDB.Enabled = false;
                btnToDB.Visible = false;
                btnToFile.IsChecked = true;
            }
            else
            {
                btnToDB.Visible = true;
                 btnToDB.IsChecked = true;
            }
        }

        private void DocumentVersion()
        {
            int nGridViewHeight = dataGridVersion.Height / 4;

            dataGridVersion.ColumnCount = 2;

            string[] strValue = new string[] { "버전", "파일 생성일", "작성자", "설명" };
            for (int i = 0; i < 4; i++)
            {
                DataGridViewRow gridRow = new DataGridViewRow();
                gridRow.Height = nGridViewHeight;

                DataGridViewCell cell = null;


                cell = new DataGridViewTextBoxCell();
                cell.Value = strValue[i];
                gridRow.Cells.Add(cell);

                cell = new DataGridViewTextBoxCell();
                cell.Value = "";// "V_1.0";
                gridRow.Cells.Add(cell);

                dataGridVersion.Rows.Add(gridRow);
            }

            DataGridViewCellStyle cs = dataGridVersion.DefaultCellStyle.Clone();
            cs.BackColor = Color.Gray;
            cs.SelectionBackColor = Color.Gray;
            cs.Font = new Font("맑은 고딕", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            cs.ForeColor = System.Drawing.Color.FromArgb(1, 1, 1);
            dataGridVersion.Rows[0].Cells[0].Style = cs;
            
            dataGridVersion.ForeColor = System.Drawing.Color.FromArgb(1, 1, 1);
            dataGridVersion.Font = new Font("맑은 고딕", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));

            //dataGridVersion.Rows[1].Cells[1].Value = DateTime.Today.ToString();

        }

        private void SetInspectionMgr()
        {
            dataGridManagement.ColumnCount = 3;

            dataGridManagement.Columns[0].Name = "버전";
            dataGridManagement.Columns[1].Name = "수정일";
            dataGridManagement.Columns[2].Name = "담당자";

            dataGridManagement.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridManagement.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dataGridManagement.ColumnHeadersHeight = 50;

            dataGridManagement.ForeColor = System.Drawing.Color.FromArgb(1, 1, 1);
            dataGridManagement.Font = new Font("맑은 고딕", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));

            int nGridViewHeight = dataGridManagement.Height / 4;
            for (int i = 0; i < 4; i++)
            {
                DataGridViewRow gridRow = new DataGridViewRow();
                gridRow.Height = nGridViewHeight;

                dataGridManagement.Rows.Add(gridRow);
            }

            dataGridManagement.ForeColor = System.Drawing.Color.FromArgb(1, 1, 1);
            dataGridManagement.Font = new Font("맑은 고딕", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
        }

        private void PageBackstageSave2_Load(object sender, EventArgs e)
        {
            DocumentVersion();
            SetInspectionMgr();
        }

        public void SaveToFile(bool showResultMessage)
        {
            if (FormMain2.Instance.TagInputMode)
            {
                FileDialog dlg = new SaveFileDialog();
                dlg.Filter = "txt Files (*.txt)|*.txt| All Files (*.*)|*.*";

                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    FormEquipList leftBar = FormMain2.Instance.ViewControl.LeftBar;
                    //Dictionary<int, FireEquipment> dicEquipments = FormMain.Instance.ViewControl.LeftBar.EditedFireEquipmentsInTagInputMode;
                    string strErrorMessage = "", strResult = "";

                    if (WriteEquipmentIDsToFile(leftBar.FEShapes, leftBar.HDShapes, leftBar.FAShapes, dlg.FileName, ref strErrorMessage))
                        //if (WriteEquipmentIDsToFile(dicEquipments, dlg.FileName, ref strErrorMessage))
                        strResult = "설비 번호가 파일에 저장되었습니다.";
                    else
                        strResult = strErrorMessage;

                    if (showResultMessage)
                        MessageBox.Show(strResult);
                }
            }
            else
            {
                FileDialog dlg = new SaveFileDialog();
                dlg.Filter = "fmf Files (*.fmf)|*.fmf| All Files (*.*)|*.*";

                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    FormMain2.Instance.ViewControl.LeftBar.ApplyGridData();
                    string strResult = "";

                    if (FormMain2.Instance.FileManager.ExportData(dlg.FileName))
                        strResult = "파일이 생성되었습니다.";
                    else
                        strResult = "파일 생성이 실패하였습니다.";

                    if (showResultMessage)
                        MessageBox.Show(strResult);
                }
            }
        }

        public void SaveToDB(bool showResultMessage)
        {
            FormMain2.Instance.ViewControl.LeftBar.ApplyGridData();

            DXFManager dxfMgr = FormMain2.Instance.DXFManager;
            string strResult = "";

            if (dxfMgr.SaveToDB())
            {
                IOManager ioMgr = FormMain2.Instance.IOManager;
                ioMgr.ApplyEquipments(dxfMgr.Equipments, FormMain2.Instance.CurrentZone);
                ioMgr.ApplyEquipmentHistory(dxfMgr.EquipmentHistory);

                strResult = "데이터가 DB에 저장되었습니다.";
            }
            else
                strResult = "DB 저장이 실패하였습니다.";

            if (showResultMessage)
                MessageBox.Show(strResult);
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            //파일저장버튼클릭시
            if (btnToFile.IsChecked)
            {
                if (!FormMain2.Instance.IsPCMode && FormMain2.Instance.CurrentZone == null)
                {
                    MessageBox.Show("저장할 데이터가 없습니다.");
                    return;
                }

                SaveToFile(true);
            }

            //DB저장버튼클릭시
            else
            {
                SaveToDB(true);
            }
        }

        private bool WriteEquipmentIDToFile(FireEquipment equip, Dictionary<DXFViewer.Shape, DataGridViewRow> dicShapes, System.IO.StreamWriter writer, ref string strErrorMessage)
        {
            if (equip == null)
                return true;

            if (!CheckDuplicateEquipID(equip, dicShapes, ref strErrorMessage))
            {
                writer.Close();
                return false;
            }

            writer.WriteLine(string.Format("{0}, {1}, {2}", equip.ID, equip.EquipID, (int)equip.Type));
            writer.Flush();

            return true;
        }

        public bool WriteEquipmentIDsToFile(Dictionary<DXFViewer.Shape, DataGridViewRow> dicFEShapes, Dictionary<DXFViewer.Shape, DataGridViewRow> dicHDShapes, Dictionary<DXFViewer.Shape, DataGridViewRow> dicFAShapes, string strFilePath, ref string strErrorMessage)
        {
            try
            {
                System.IO.StreamWriter writer = new System.IO.StreamWriter(strFilePath, false, Encoding.UTF8);

                foreach (KeyValuePair<DXFViewer.Shape, DataGridViewRow> pair in dicFEShapes)
                {
                    if (!WriteEquipmentIDToFile((FireEquipment)pair.Value.Tag, dicFEShapes, writer, ref strErrorMessage))
                        return false;
                }

                foreach (KeyValuePair<DXFViewer.Shape, DataGridViewRow> pair in dicHDShapes)
                {
                    if (!WriteEquipmentIDToFile((FireEquipment)pair.Value.Tag, dicHDShapes, writer, ref strErrorMessage))
                        return false;
                }

                foreach (KeyValuePair<DXFViewer.Shape, DataGridViewRow> pair in dicFAShapes)
                {
                    if (!WriteEquipmentIDToFile((FireEquipment)pair.Value.Tag, dicFAShapes, writer, ref strErrorMessage))
                        return false;
                }

                writer.Close();
            }
            catch (Exception e)
            {
                strErrorMessage = e.Message;
                return false;
            }

            return true;
        }

        private bool WriteEquipmentIDsToFile(Dictionary<int, FireEquipment> dicEquipments, string strFilePath, ref string strErrorMessage)
        {
            try
            {
                System.IO.StreamWriter writer = new System.IO.StreamWriter(strFilePath, false, Encoding.UTF8);

                foreach (KeyValuePair<int, FireEquipment> pair in dicEquipments)
                {
                    FireEquipment equip = pair.Value;

                    if (!CheckDuplicateEquipID(equip, dicEquipments, ref strErrorMessage))
                    {
                        writer.Close();
                        return false;
                    }

                    writer.WriteLine(string.Format("{0}, {1}, {2}", equip.ID, equip.EquipID, (int)equip.Type));
                    writer.Flush();
                }

                writer.Close();
            }
            catch (Exception e)
            {
                strErrorMessage = e.Message;
                return false;
            }

            return true;
        }

        private bool CheckDuplicateEquipID(FireEquipment equip, Dictionary<DXFViewer.Shape, DataGridViewRow> dicShapes, ref string strErrorMessage)
        {
            foreach (KeyValuePair<DXFViewer.Shape, DataGridViewRow> pair in dicShapes)
            {
                if (pair.Value == null)
                    continue;

                FireEquipment _equip = (FireEquipment)pair.Value.Tag;

                if (_equip == equip)
                    continue;

                if (_equip.EquipID == equip.EquipID)
                {
                    if (_equip.Zone.DXFFilePath == equip.Zone.DXFFilePath)
                    {
                        strErrorMessage = string.Format("[{0}]에 같은 설비 ID [{1}](Type : {2})가 두번 이상 존재합니다.", equip.Zone.DXFFilePath, equip.EquipID, (int)equip.Type);
                        return false;
                    }
                    else
                    {
                        strErrorMessage = string.Format("[{0}]와 [{1}]에 같은 설비 ID [{2}](Type : {3})가 각각 존재합니다.", _equip.Zone.DXFFilePath, equip.Zone.DXFFilePath, equip.EquipID, (int)equip.Type);
                        return false;
                    }
                }
            }

            return true;
        }

        private bool CheckDuplicateEquipID(FireEquipment equip, Dictionary<int, FireEquipment> dicEquipments, ref string strErrorMessage)
        {
            foreach (KeyValuePair<int, FireEquipment> pair in dicEquipments)
            {
                if (pair.Value == equip)
                    continue;

                if (pair.Value.EquipID == equip.EquipID && pair.Value.Type == equip.Type)
                {
                    if (pair.Value.Zone.DXFFilePath == equip.Zone.DXFFilePath)
                    {
                        strErrorMessage = string.Format("[{0}]에 같은 설비 ID [{1}](Type : {2})가 두번 이상 존재합니다.", equip.Zone.DXFFilePath, equip.EquipID, (int)equip.Type);
                        return false;
                    }
                    else
                    {
                        strErrorMessage = string.Format("[{0}]와 [{1}]에 같은 설비 ID [{2}](Type : {3})가 각각 존재합니다.", pair.Value.Zone.DXFFilePath, equip.Zone.DXFFilePath, equip.EquipID, (int)equip.Type);
                        return false;
                    }
                }
            }

            return true;
        }

        private void btnToFile_Click(object sender, EventArgs e)
        {
            //if (FormMain2.Instance.IsPCMode)
            {
                btnToFile.IsChecked = true;
                btnToDB.IsChecked = false;
                //btnToDB.IsChecked = !btnToDB.IsChecked;
            }
            btnToFile.Refresh();
            btnToDB.Refresh();
        }

        private void btnToDB_Click(object sender, EventArgs e)
        {
            //if (FormMain2.Instance.IsPCMode)
            {
                btnToDB.IsChecked = true;
                btnToFile.IsChecked = false;
            }
            btnToFile.Refresh();
            btnToDB.Refresh();
        }
    }
}
