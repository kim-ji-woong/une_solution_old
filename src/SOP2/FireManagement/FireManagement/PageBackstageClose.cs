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
    public partial class PageBackstageClose : Form
    {
        public PageBackstageClose()
        {
            InitializeComponent();
        }

        private void btnCanel_Click(object sender, EventArgs e)
        {
            FormMain2.Instance.SelectedPage(1);
            FormMain2.Instance.Refresh();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            FormFrame.Instance.Close();
        }

        private void btnSaveExit_Click(object sender, EventArgs e)
        {
            //일단 파일저장으로

            //파일저장버튼클릭시
            //if (axBtnSaveToFile.Checked)
            {
                if (FormMain2.Instance.TagInputMode)
                {
                    FileDialog dlg = new SaveFileDialog();
                    dlg.Filter = "txt Files (*.txt)|*.txt| All Files (*.*)|*.*";

                    if (dlg.ShowDialog() == DialogResult.OK)
                    {
                        FormEquipList leftBar = FormMain2.Instance.ViewControl.LeftBar;
                        //Dictionary<int, FireEquipment> dicEquipments = FormMain.Instance.ViewControl.LeftBar.EditedFireEquipmentsInTagInputMode;
                        string strErrorMessage = "";

                        if (FormMain2.Instance.PageSave.WriteEquipmentIDsToFile(leftBar.FEShapes, leftBar.HDShapes, leftBar.FAShapes, dlg.FileName, ref strErrorMessage))
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
                        {
                            MessageBox.Show("파일이 생성되었습니다.");
                            FormMain2.Instance.Close();
                            FormFrame.Instance.Close();
                            
                        }
                        else
                            MessageBox.Show("파일 생성이 실패하였습니다.");
                    }
                }
            }

            //DB저장버튼클릭시
            // else
            //{
            //    FormMain2.Instance.ViewControl.LeftBar.ApplyGridData();

            //    DXFManager dxfMgr = FormMain2.Instance.DXFManager;

            //    if (dxfMgr.SaveToDB())
            //    {
            //        IOManager ioMgr = FormMain2.Instance.IOManager;
            //        ioMgr.ApplyEquipments(dxfMgr.Equipments, FormMain2.Instance.CurrentZone);
            //        ioMgr.ApplyEquipmentHistory(dxfMgr.EquipmentHistory);

            //        MessageBox.Show("데이터가 DB에 저장되었습니다.");
            //    }
            //    else
            //        MessageBox.Show("DB 저장이 실패하였습니다.");
            //}
        }
    }
}
