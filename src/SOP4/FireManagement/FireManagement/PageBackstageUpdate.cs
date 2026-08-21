using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;

namespace FireManagement
{
    public partial class PageBackstageUpdate : Form
    {
        public PageBackstageUpdate()
        {
            InitializeComponent();
        }

        private string GetFolderName(string strFilePath)
        {
            int nIndex = strFilePath.LastIndexOf('\\');
            return strFilePath.Substring(0, nIndex + 1);
        }

        private void btnSearchFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "fmf Files (*.fmf)|*.fmf| All Files (*.*)|*.*";
            //dlg.Multiselect = true;

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                EventManager.Instance.ProcessEvent(Event.PREV_OPEN_FMF);

                FormMain2 frmMain = FormMain2.Instance;
                frmMain.CurrentZone = null;

                string strFolderName = GetFolderName(dlg.FileName);

                foreach (string strFileName in dlg.SafeFileNames)
                {
                    string strFilePath = strFolderName + strFileName;
                    bool isPCMode = FormMain2.Instance.IsPCMode;

                    if (frmMain.FileManager.ImportData(strFilePath, ref isPCMode))
                    {
                    }
                    else
                    {
                        if (!FormMain2.Instance.IsPCMode && !isPCMode)
                        {
                            MessageBox.Show("Tablet에서는 PC에서 생성한 파일만 불러올 수 있습니다.");
                            return;
                        }

                        MessageBox.Show(strFilePath + "\r\n업데이트 실패");
                        return;
                    }
                }

                frmMain.CurrentZone = null;

                // 현재 설정되어있는 BuildingGroup으로 다시 지정
                BuildingGroup currentBuildingGroup = FormMain2.Instance.FormFileLoad.GetCurrentBuildingGroup();

                foreach (KeyValuePair<BuildingGroup, ArrayList> pair in FormMain2.Instance.IOManager.AllBuildingGroups)
                {
                    if (pair.Key.ID == currentBuildingGroup.ID)
                    {
                        currentBuildingGroup = pair.Key;
                        break;
                    }
                }
                ////////////////////////////////////////////////

                FormMain2.Instance.FormFileLoad.SetBuildingGroup(currentBuildingGroup);

                MessageBox.Show("업데이트 성공");
                //MessageBox.Show("FMF 갱신 완료");
                //FormMain.Instance.FileManager.ImportData(dlg.FileName);

                EventManager.Instance.ProcessEvent(Event.POST_OPEN_FMF, dlg.FileName);
            }
        }

        private void btnFileUpdate_Click(object sender, EventArgs e)
        {

        }


    }
}
