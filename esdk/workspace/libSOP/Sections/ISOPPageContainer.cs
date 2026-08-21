using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;

using Sections;
using DBUtility;

namespace UnE.SOP.Sections
{
    public interface ISOPPageContainer
    {
        /// <summary>
        /// 현재 구동중인 ActionStep의 HistoryID리스트
        /// </summary>
        ArrayList ArrLoadHistory
        {
            get;            
        }

        /// <summary>
        /// 열려진 모든 TabPage를 ArrayList에 담아온다.
        /// </summary>
        /// <returns></returns>
        ArrayList GetTabPage();

        /// <summary>
        /// 해당 ActionStep과 관련된 모든 PanelSection을 받는다.
        /// </summary>
        /// <param name="nActionStepID"></param>
        /// <returns></returns>
        ArrayList GetAllPanels(int nActionStepID);

        /// <summary>
        /// ActionStepPanel에 포함된 모든 Section을 가져온다.
        /// </summary>
        /// <param name="arPanels"></param>
        /// <returns></returns>
        ArrayList GetAllPanelSections(ArrayList arPanels);

        bool LoadActionStepPanel(WebDBManager dbMgr, string strActionstepIDs, ArrayList arrActionStepHistoryID, ArrayList arrActionStepID, ArrayList arrActionStepBeginTime, ArrayList arrActionStepDetectTime, ArrayList arrDisaster, ArrayList arrSensorZoneHistories, bool isRealMode, Dictionary<int, DisasterInfo> dicDisaster, Dictionary<DisasterInfo, string> dicDisasterFullPath, bool isRegular, bool isNormal);

        /// <summary>
        /// TabContorl에서 해당 Tabpage가 있으면 제거
        /// </summary>
        /// <param name="page"></param>
        /// <param name="bRemoveOnly">true인경우 TabPage만 제거</param>
        void RemoveTabPage(SectionTabPage page, bool bRemoveOnly);

        /// <summary>
        /// TabPage와 연관된 SOPScenario를 제거하고 TabPage를 제거
        /// </summary>
        /// <param name="page">대상 TabPage</param>
        /// <returns></returns>
        bool RemoveTabPage(SectionTabPage page);

        /// <summary>
        /// 주어진 Section의 ArrayList에서 특정 ComponentID의 Section을 찾는다.
        /// </summary>
        /// <param name="nComponentID"></param>
        /// <param name="nComponentType"></param>
        /// <param name="arrSections"></param>
        /// <returns></returns>
        Section FindSection(int nComponentID, int nComponentType, ArrayList arrSections);

        /// <summary>
        /// ActionStepHistoryID를 설정하는 함수
        /// ActionStepHistory는 비동기로 DB에 저장되므로 UI에 ActionStepHistoryID를 저장하는 해야 함.
        /// </summary>
        /// <param name="nActionStepID"></param>
        /// <param name="isRealMode"></param>
        /// <param name="nActionStepHistoryID"></param>
        void SetActionStepHistoryID(int nActionStepID, bool isRealMode, int nActionStepHistoryID);

        /// <summary>
        /// ActionStepHistory를 닫을때 호출하는 함수 
        /// 시점에 따라 ActionStepHistoryID가 없을 수 있으므로 ActionStepID와 RealMode로 찾는다.
        /// 탭이 종료 되는 경우 처리는 별도로 추가
        /// </summary>
        /// <param name="nActionStepID"></param>
        /// <param name="isRealMode"></param>
        /// <param name="bCloseTab"></param>
        void RemoveActionStepHistory(int nActionStepID, bool isRealMode, bool bCloseTab = false);

        /// <summary>
        /// 새롭게 추가된 ActionStepHistory가 있는경우 해당 ID를 넘겨주는 함수
        /// </summary>
        /// <param name="nActionStepHistoryID"></param>
        void NewActionStepHistory(int nActionStepHistoryID);

        /// <summary>
        /// Page컨테이너에서 해당정보의 SOPScenario가 존재하는지 여부
        /// </summary>
        /// <param name="nRealActionStepID"></param>
        /// <param name="bReal"></param>
        /// <returns></returns>
        bool ExistSOPScenario(int nRealActionStepID, bool bReal);
    }
}
