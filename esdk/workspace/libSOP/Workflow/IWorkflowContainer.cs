using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;

using Sections;
using UnE.SOP.Tree;
using UnE.SOP.History;
using UnE.SOP.Log;
using UnE.SOP.Workstate;

namespace UnE.SOP.Workstate
{
    public interface IWorkflowContainer
    {    
        //////////////////////////////////////////////////////////////////////////
        // Section
        //////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Section Component의 완료 처리
        /// </summary>
        /// <param name="section">완료되는 Section</param>
        /// <param name="paenl">Section을 포함하는 Panel</param>
        /// <returns>정상 처리인경우 true</returns>
        bool CompleteSection(Section section, PanelSection paenl);
        
        /// <summary>
        /// 대상 Section Component에 Focus 처리
        /// </summary>
        /// <param name="section">Focus를 가지는 Section</param>
        /// <returns>정상동작인경우 true</returns>
        bool FocusSection(Section section);
        
        /// <summary>
        /// 현재 실행중인 Section인지 여부
        /// </summary>
        /// <param name="section"></param>
        /// <returns>true이면 실행중</returns>
        bool IsWorkingMode(Section section);

        //////////////////////////////////////////////////////////////////////////
        // Workflow
        //////////////////////////////////////////////////////////////////////////

        void StopWorkflow(DateTime dtStop, bool noDBWrite, int nActionStepID, bool isRealMode);

        void StopWorkflow(DateTime dtStop, bool noDBWrite = false);

        void SetCurrentWorkflow(WorkFlow work);


        //////////////////////////////////////////////////////////////////////////
        // ActionStep
        //////////////////////////////////////////////////////////////////////////
        

        /// <summary>
        /// 현재 수행중인 ActionStep인지 여부
        /// </summary>
        /// <param name="nActionStepID">Action Step의 ID</param>
        /// <param name="bReal">true이면 실제모드, false이면 훈련모드</param>
        /// <returns>true이면 실행중</returns>
        bool IsWorkingMode(int nActionStepID, bool bReal);

        /// <summary>
        /// 현재 실행중인  
        /// </summary>
        /// <param name="bReal"></param>
        /// <returns></returns>
        int ReadCurrentActionStep(ref bool bReal);

        void SetCurrentActionStep(int nActionStepID, bool bReal);

        //void AddScenario(string strPath, int nActionStepID, bool bReal, bool bRegular, bool bNormal, int nActionStepHistoryID);
        
        void ClearProcess();


        //////////////////////////////////////////////////////////////////////////
        // SOP Scenario
        //////////////////////////////////////////////////////////////////////////
        
        /// <summary>
        /// 현재 실행중인 SOP 시나리오
        /// </summary>
        /// <returns>현재 실행중인 SOPScenario, 없는 경우 null</returns>
        SOPScenario GetCurrentSOPScenario();
        
        /// <summary>
        /// 현재 실행중인 모든 SOP 시나리오
        /// </summary>
        /// <returns>모든 SOPScenario가 있는 ArrayList</returns>
        ArrayList GetAllSenario();

        /// <summary>
        /// 새로운 Scenario가 로드될때 자동으로 호출
        /// </summary>
        /// <param name="sc">새로 추가되는 시나리오</param>
        void OnLoadScenario(SOPScenario sc);


        void SelectedScenario(int nActionStepID, bool isRealMode);



        void OnWorkflowChanged(object sender, WorkFlowEventArgs args);

        void RunWorkflowWithEvent();
        void PostChangeSectionState(Section section, State state);

        // 섹션이 실행될때 마다 호출되는 함수
        void TouchSection(Section section);
    }
}
