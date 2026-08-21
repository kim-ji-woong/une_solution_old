using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using UnE.Utility;

namespace PreSafe
{
    internal class SOPChecker : IDisposable
    {
        public void Dispose()
        {

        }

        public bool CheckSOP(  Sections.PanelSectionEx panel)
        {
            int nStart = 0, nEnd = 0;
            
            string szStepName = panel.StepName;
            string szHeader = string.Format("[{0}] - ", szStepName);
            foreach (Sections.Section section in panel.Sections)
            {
                Sections.Section.ComponentType type = section.GetComponentType();

                if(type == Sections.Section.ComponentType.PROCESS)
                {
                    if (!CheckProcess((Sections.SectionProcess)section))
                        return false;
                }
                else if (type == Sections.Section.ComponentType.ENDPOINT)
                {
                    PrepareCheckEndPoint((Sections.SectionEndPoint)section, ref nStart, ref nEnd);
                }
            }

            if (nStart == 0)
            {
                UMessageBox.Show(szHeader + "[시작] 태그가 없습니다.\r\n확인후 저장하십시오.", "저장오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            else if (nStart > 1)
            {
                UMessageBox.Show(szHeader + string.Format("[시작] 태그가 {0}개 존재합니다.\r\n[시작] 태그는 반드시 하나만 존재하여야 합니다.\r\n확인후 저장하십시오.", nStart), "저장오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            if (nEnd == 0)
            {
                UMessageBox.Show(szHeader + "[종료] 태그가 없습니다.\r\n확인후 저장하십시오.", "저장오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            return true;
        }

        private void PrepareCheckEndPoint(Sections.SectionEndPoint section, ref int nStart, ref int nEnd)
        {
            Sections.SectionDataEndPoint data = (Sections.SectionDataEndPoint)section.Data;

            if (data.IsBegin)
                nStart++;
            else
                nEnd++;
        }

        private bool CheckProcess(Sections.SectionProcess section)
        {
            Sections.SectionDataProcess data = (Sections.SectionDataProcess)section.Data;

            //if (data.TeamList.Count < 0)
            //{
                //MessageBox.Show("임무를 수행할 대상이 지정되지 않은 [프로세스] 태그가 존재합니다.\r\n확인후 저장하십시오.");
               // ZoomNSelectSection(section);
               // return false;
           // }

            return true;
        }

    }
}
