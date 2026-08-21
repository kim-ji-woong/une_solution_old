using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Sections;
using UnE.SOP.Tree;
using UnE.SOP.History;
using UnE.SOP.Log;
using UnE.SOP.Data;
using UnE.SOP.Workstate;


namespace UnE.SOP.Data
{
    public interface ISOPDataContainer
    {
        VersionInfo GetActionStepVersionInfo(int nActionStepID);

        void SetActionStepHistoryID(int nActionStepID, bool isRealMode, int nActionStepHistoryID);

        /// <summary>
        /// [FullPath<->재난] 구조로 이루어진 SOP Data Dictionary를 얻어오는 함수
        /// FullPath : Category/SubCategory/Disaster
        /// </summary>
        /// <param name="isRegistered">
        /// 등록된 버전인지 여부
        /// </param>
        /// <param name="isNormal">
        /// isNormal이 true이면 평일 주간 모드
        ///            false이면 휴일 또는 야간 모드
        /// </param>
        Dictionary<string, DisasterInfo> GetSOPDictionary(bool isRegistered, bool isNormal);

        /// <summary>
        /// SOP의 [Disaster ID<->Version] 구조로 이루어진 SOP Data Dictionary를 얻어오는 함수
        /// </summary>
        /// <param name="isRegistered">
        /// 등록된 버전인지 여부
        /// </param>
        /// <param name="isNormal">
        /// isNormal이 true이면 평일 주간 모드
        ///            false이면 휴일 또는 야간 모드
        /// </param>
        Dictionary<int, VersionInfo> GetVersionDictionary(bool isRegular, bool isNormal);

        /// <summary>
        /// nActionStepID에 해당하는 ActionStep을 찾은후에 해당 ActionStep을 소유한 DisasterInfo 객체를 리턴한다.
        /// </summary>
        /// <param name="nActionStepID"></param>
        /// <returns></returns>
        bool LoadDisasterActionStep(int nActionStepID);
        DisasterInfo GetDisaster(int nDisasterID, out bool isNormal, out bool isRegular);
        ActionStepInfo GetActionStepInfo(int nActionStepID);
        string GetDisasterFullPath(DisasterInfo disaster);

        void NewActionStepHistory(int nID);
    }
}
