using System;
using UnE.Geometry;
using System.Collections.Generic;

namespace Common.IDAL
{
    using Model.Option;
    using Model.History;
    using Model;

    public interface ICreate
    {
        // Option
        Options CreateOption(Options.OptionTarget eTargetName, string strPropertyName, string strPropertyValue, int nSiteID, string strDescription = null);

        // History
        /// <summary>
        /// 
        /// </summary>
        /// <param name="nActionStepID"></param>
        /// <param name="dtBegin"></param>
        /// <param name="realMode">실제상황인가?</param>
        /// <param name="dtEnd"></param>
        /// <param name="dtLastAccessed"></param>
        /// <param name="dtDetectEnd"></param>
        /// <param name="dtDetect"></param>
        /// <param name="strPosition"></param>
        /// <param name="nLastAccesseduserID"></param>
        /// <param name="nStartOption">SOP시작 옵션 : 0:None 1:SMS 2:Broadcast 4:Reserve1 8:Reserve2</param>
        /// <param name="strDisasterOption"></param>
        /// <param name="nSensorZoneHistoryID"></param>
        /// <param name="strDescription"></param>
        /// <returns></returns>
        ActionStepHistory CreateActionStepHistory(int nActionStepID, DateTime dtBegin, bool? realMode = null, DateTime? dtEnd = null, DateTime? dtLastAccessed = null, DateTime? dtDetectEnd = null, DateTime? dtDetect = null, string strPosition = null, int? nLastAccesseduserID = null, int? nStartOption = null, string strDisasterOption = null, int? nSensorZoneHistoryID = null, string strDescription = null);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="nActionStepHistoryID"></param>
        /// <param name="nComponentID"></param>
        /// <param name="nComponentType">PROCESS(0), DECISION(1), ANNOTATION(2), ENDPOINT(3), LINK(4), TRANSSOP(5), INTERNAL(6), EXTERNAL(7), NONE(8)</param>
        /// <param name="dtTime"></param>
        /// <param name="nStatus">
        /// 하위 2바이트(실행상태) : 대기상태(1), 실행중(2), 완료(3), 입력대기상태(4), 건너뜀 상태(5)
        /// 상위 2바이트(실행방향, bit flag 조합) : 위쪽(1), 오른쪽(2), 아래쪽(4), 왼쪽(8)
        /// </param>
        /// <param name="strTask"></param>
        /// <param name="nCompleteCount"></param>
        /// <param name="showBoard"></param>
        /// <param name="nAccessedUserID"></param>
        /// <param name="nCheckedNotify1"></param>
        /// <param name="nCheckedNotify2"></param>
        /// <param name="nCheckedRun"></param>
        /// <param name="nCheckedComplete"></param>
        /// <param name="strDescription"></param>
        /// <returns></returns>
        ComponentHistory CreateComponentHistory(int nActionStepHistoryID, int nComponentID, int nComponentType, DateTime dtTime, int nStatus, string strTask = null, int? nCompleteCount = null, bool? showBoard = null, int? nAccessedUserID = null, int? nCheckedNotify1 = null, int? nCheckedNotify2 = null, int? nCheckedRun = null, int? nCheckedComplete = null, string strDescription = null);
        ComponentHistoryDetail CreateComponentHistoryDetail(int nComponentHistoryID, int nDataIndex, int? nData = null, float? fData = null, string strData = null, DateTime? dtTime = null);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="nActionStepHistoryID"></param>
        /// <param name="nActionStepID"></param>
        /// <param name="nUseCloseNoInput">입력이 없을때 SOP 자동 종료 사용여부</param>
        /// <param name="nUseCloseSensorReset">센서 리셋 신호시 SOP자동 종료 사용여부</param>
        /// <param name="nUseCloseSensorResetWaitTime">센서 신호시 몇분뒤 자동 종료 사용여부</param>
        /// <param name="nInputWaitTime">입력 대기시간 (초)</param>
        /// <param name="nSensorResetWaitTime">센서 리셋 후 대기 시간 (초)</param>
        /// <param name="dtBegin">SOP 시작 시간</param>
        /// <param name="nSensorZoneID"></param>
        /// <param name="nSensorZoneHistoryID"></param>
        /// <param name="strDescription"></param>
        /// <returns></returns>
        ActionStepAutoClose CreateActionStepAutoClose(int nActionStepHistoryID, int? nActionStepID = null, int? nUseCloseNoInput = null, int? nUseCloseSensorReset = null, int? nUseCloseSensorResetWaitTime = null, int? nInputWaitTime = null, int? nSensorResetWaitTime = null, DateTime? dtBegin = null, int? nSensorZoneID = null, int? nSensorZoneHistoryID = null, string strDescription = null);
        Shelter CreateShelter(string strShelterName, int nShelterType, int nShelterIDType, int? nShelterID, List<Polygon> boundary, int nSiteID, string strDescription);
        Site CreateSite(string strSiteName, int nTeamID);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="nUserID"></param>
        /// <param name="nTargetType">0:POI, 1:공간정보이름, 2:가벽, 3:SOP편집, 4:현황정보, 5:사용자권한부여</param>
        /// <param name="nActionType">0:추가, 1:수정, 2:삭제, 3:업로드, 4:다운로드</param>
        /// <param name="strHistoryContent"></param>
        /// <returns></returns>
        bool CreateUserHistory(int nUserID, int nTargetType, int nActionType, string strHistoryContent);

        string GetErrorMessage();
    }
}
