using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace FireManagement
{
    public partial class PageBackstageSave : Form
    {
        public PageBackstageSave()
        {
            InitializeComponent();

            if (!FormMain2.Instance.IsPCMode)
            {
                axBtnSaveToDB.Enabled = false;
                axBtnSaveToFile.Checked = true;
            }
            else
            {
                axBtnSaveToDB.Checked = true;
            }
        }

        private void axBtnSaveToFile_ClickEvent(object sender, EventArgs e)
        {
            if (FormMain2.Instance.IsPCMode)
            {
                axBtnSaveToFile.Checked = !axBtnSaveToFile.Checked;
                axBtnSaveToDB.Checked = !axBtnSaveToFile.Checked;
            }  
        }

        private void axBtnSaveToDB_ClickEvent(object sender, EventArgs e)
        {
            if (FormMain2.Instance.IsPCMode)
            {
                axBtnSaveToDB.Checked = !axBtnSaveToDB.Checked;
                axBtnSaveToFile.Checked = !axBtnSaveToDB.Checked;
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (axBtnSaveToFile.Checked)
            {
                if (FormMain2.Instance.TagInputMode)
                {
                    FileDialog dlg = new SaveFileDialog();
                    dlg.Filter = "txt Files (*.txt)|*.txt| All Files (*.*)|*.*";

                    if (dlg.ShowDialog() == DialogResult.OK)
                    {
                        FormEquipList leftBar = FormMain2.Instance.ViewControl.LeftBar;
                        //Dictionary<int, FireEquipment> dicEquipments = FormMain2.Instance.ViewControl.LeftBar.EditedFireEquipmentsInTagInputMode;
                        string strErrorMessage = "";

                        if (WriteEquipmentIDsToFile(leftBar.FEShapes, leftBar.HDShapes, leftBar.FAShapes, dlg.FileName, ref strErrorMessage))
                        //if (WriteEquipmentIDsToFile(dicEquipments, dlg.FileName, ref strErrorMessage))
                            MessageBox.Show("설비 번호가 파일에 저장되었습니다.");
                        else
                            MessageBox.Show(strErrorMessage);
                    }
                }
                else
                {
                    FileDialog dlg = new SaveFileDialog();
                    dlg.Filter = "fmf Files (*.fmf)|*.fmf| All Files (*.*)|*.*";

                    if (dlg.ShowDialog() == DialogResult.OK)
                    {
                        FormMain2.Instance.ViewControl.LeftBar.ApplyGridData();

                        if (FormMain2.Instance.FileManager.ExportData(dlg.FileName))
                            MessageBox.Show("파일이 생성되었습니다.");
                        else
                            MessageBox.Show("파일 생성이 실패하였습니다.");
                    }
                }
            }
            else
            {
                FormMain2.Instance.ViewControl.LeftBar.ApplyGridData();

                DXFManager dxfMgr = FormMain2.Instance.DXFManager;

                if (dxfMgr.SaveToDB())
                {
                    IOManager ioMgr = FormMain2.Instance.IOManager;
                    ioMgr.ApplyEquipments(dxfMgr.Equipments, FormMain2.Instance.CurrentZone);
                    ioMgr.ApplyEquipmentHistory(dxfMgr.EquipmentHistory);

                    MessageBox.Show("데이터가 DB에 저장되었습니다.");
                }
                else
                    MessageBox.Show("DB 저장이 실패하였습니다.");
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

        private bool WriteEquipmentIDsToFile(Dictionary<DXFViewer.Shape, DataGridViewRow> dicFEShapes, Dictionary<DXFViewer.Shape, DataGridViewRow> dicHDShapes, Dictionary<DXFViewer.Shape, DataGridViewRow> dicFAShapes, string strFilePath, ref string strErrorMessage)
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
    }
}
