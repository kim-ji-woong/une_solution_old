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

namespace UnE.SOP.Log
{
    public interface ISOPLogContainer
    {
        /// <summary>
        /// 특정 시점에 발생한 이벤트에 대하여 SOP 로그창에 데이터를 기록할 수 있도록 데이터를 넘겨준다.
        /// 로그창 UI에서는 건네받은 각 변수값들을 사용하여 적절히 데이터를 가공하여 로그를 표기할 수 있다.
        /// 이 함수는 쓰레드에서 호출될수도 있는데 그럴 경우 callByThread가 true 값을 가진다.
        /// </summary>
        /// <param name="nActionStepHistoryID">
        /// 발생한 이벤트와 관련된 ActionStep History ID
        /// </param>
        /// <param name="nComponentHistoryID">
        /// 발생한 이벤트가 Section에 관련된 것이라면 이 값은 해당 Section과 연관된 ComponentHistory ID 값을 가진다.
        /// Section이 아닌 전체 ActionStep에 대한 이벤트라면 이 값은 0보다 작게 된다.
        /// </param>
        /// <param name="data"></param>
        /// <param name="nActionStepID"></param>
        /// <param name="isRealMode"></param>
        /// <param name="nComponentID">
        /// 발생한 이벤트가 Section에 관련된 것이라면 이 값은 해당 Section의 Component ID 값을 가진다.
        /// Section이 아닌 전체 ActionStep에 대한 이벤트라면 이 값은 0보다 작게 된다.
        /// </param>
        /// <param name="strStepMemberName">
        /// SOP 아래의 [단계]명
        /// </param>
        /// <param name="strTeamList">
        /// 발생한 이벤트가 Section에 관련된 것이고 해당 Component가 Process일 경우
        /// Process에 연결된 전체 팀이름들을 쉼표(,)로 구분하여 넘겨준다.
        /// 그렇지 않을 경우 "-"와 같은 값을 가진다.
        /// </param>
        /// <param name="strComponentType">
        /// 발생한 이벤트가 Section에 관련된 것이라면 해당 Section의 Component Type을 알려준다.
        /// Type이 SectionEndPoint일 경우 [시작/종료] 여부를 구분하여 표기한다.
        /// </param>
        /// <param name="strTask">
        /// 발생 이벤트 내용
        /// </param>
        /// <param name="strStatus"></param>
        /// <param name="nCompleteCount">
        /// 하나의 Component가 여러번 실행되었을 경우 총 실행된 회수
        /// </param>
        /// <param name="callByThread">
        /// 이 함수가 Thread에 의하여 호출되었는지 여부
        /// </param>
        void AddLog(int nActionStepHistoryID, int nComponentHistoryID, HistorySectionData data, int nActionStepID, bool isRealMode, int nComponentID, string strStepMemberName, string strTeamList, string strComponentType, string strTask, string strStatus, int nCompleteCount = -1, bool callByThread = false);

        /// <summary>
        /// AddLog(...) 함수 호출시 저장될 Log를 DataLogGridViewRow에 담을 경우 사용하는 함수
        /// DataLogGridViewRow를 사용하지 않을 경우 구현할 필요 없음.
        /// Grid의 각 행에 데이터를 담아서 리턴해준다.
        /// </summary>
        /// <param name="section">
        /// 발생한 이벤트와 관련된 Section 객체
        /// </param>
        /// <param name="noDBWrite">
        /// 이 함수는 로그를 Grid에 저장함과 동시에 DB에도 값을 쓰게 되는데, noDBWrite가 true일 경우 DB에는 쓰지 않는다.
        /// </param>
        /// <param name="nActionStepHistoryID">
        /// 발생한 이벤트와 관련된 ActionStep History ID
        /// </param>
        /// <param name="nComponentHistoryID">
        /// 발생한 이벤트가 Section에 관련된 것이라면 이 값은 해당 Section과 연관된 ComponentHistory ID 값을 가진다.
        /// Section이 아닌 전체 ActionStep에 대한 이벤트라면 이 값은 0보다 작게 된다.
        /// </param>
        /// <param name="nActionStepID">
        /// 연관된 ActionStep의 ID
        /// </param>
        /// <param name="isRealMode">
        /// SOP가 실제모드로 동작하였는가?
        /// </param>
        /// <param name="nComponentID">
        /// 발생한 이벤트가 Section에 관련된 것이라면 이 값은 해당 Section의 Component ID 값을 가진다.
        /// Section이 아닌 전체 ActionStep에 대한 이벤트라면 이 값은 0보다 작게 된다.
        /// </param>
        /// <param name="componentType">
        /// 발생한 이벤트가 Section에 관련된 것이라면 해당 Section의 ComponentType을 알려준다.
        /// </param>
        /// <param name="time">
        /// 이벤트 발생 시간
        /// </param>
        /// <param name="strStepMemberName">
        /// SOP 아래의 [단계]명
        /// </param>
        /// <param name="strTeamList">
        /// 발생한 이벤트가 Section에 관련된 것이고 해당 Component가 Process일 경우
        /// Process에 연결된 전체 팀이름들을 쉼표(,)로 구분하여 넘겨준다.
        /// 그렇지 않을 경우 "-"와 같은 값을 가진다.
        /// </param>
        /// <param name="strComponentType">
        /// componentType에 관한 문자열 값
        /// </param>
        /// <param name="strTask">
        /// 임무 내용
        /// </param>
        /// <param name="strStatus">
        /// Component의(또는 SOP의) 현재 상황
        /// </param>
        /// <param name="nCompleteCount">
        /// 하나의 Component가 여러번 실행되었을 경우 총 실행된 회수
        /// </param>
        /// <param name="callByThread">
        /// 이 함수가 Thread에 의하여 호출되었는지 여부
        /// </param>
        /// <param name="showBoard">
        /// [상황판] 프로그램에 이 이벤트를 기록할 것인지 여부
        /// </param>
        DataLogGridViewRow AddLogData(Section section, bool noDBWrite, int nActionStepHistoryID, int nComponentHistoryID, int nActionStepID, bool isRealMode, int nComponentID, Section.ComponentType componentType, DateTime time, string strStepMemberName, string strTeamList, string strComponentType, string strTask, string strStatus, int nCompleteCount = -1, bool callByThread = false, bool showBoard = false);

        /// <summary>
        /// SOP가 종료 또는 취소되었을때 각 Component들의 상태값을 기록하기 위한 함수
        /// 모든 Component들의 상태를 기록하진 못하고 Process Section에 대해서만 기록한다.
        /// 만들어진 로그 기록은 ActionStepDetailLog 객체에 담겨서 리턴된다.
        /// 
        /// 2018-01-02 추가사항(skkim)
        /// SOP자동종료를 위해 SensorZoneHistoryID를 Detail로그에 포함시킴. 필요하지 않더라도 모든 상세로그에 SensorZoneHistoryID를 추가해야함
        /// 
        /// </summary>
        /// <param name="nActionStepID">
        /// 연관된 ActionStep의 ID
        /// </param>
        /// <param name="isRealMode">
        /// SOP가 실제모드로 동작하였는가?
        /// </param>
        /// <param name="nHistoryID">
        /// ActionStepHistory ID
        /// </param>
        /// <param name="dtBegin">
        /// SOP 시작 시간
        /// </param>
        /// <param name="arrProcess">
        /// SectionProcess들의 ID List(long), 상위 4바이트(Component Type, Section.ComponentType), 하위 4바이트(Component ID)
        /// </param>
        
        // edit by skkim 2018-01-02 . add SensroZoneHistoryID
        //ActionStepDetailLog MakeActionStepLog(int nActionStepID, bool isRealMode, int nHistoryID, DateTime dtBegin, ArrayList arrProcess);        
        ActionStepDetailLog MakeActionStepLog(int nActionStepID, bool isRealMode, int nHistoryID,int nSensorZoneHistory, DateTime dtBegin, ArrayList arrProcess);

        /// <summary>
        /// MakeActionStepLog(...) 함수를 통하여 만들어진 ActionStepDetailLog 객체들을 얻어오는 함수
        /// Key 값은 nActionStepID와 isRealMode이다.
        /// </summary>
        ActionStepDetailLog GetActionStepDetailLog(int nActionStepID, bool isRealMode);

        /// <summary>
        /// MakeActionStepLog(...) 함수를 통하여 만들어진 ActionStepDetailLog 값을 갱신하는 함수.
        /// 마지막 상태값을 Complete으로 바꾼다.
        /// </summary>
        void CompleteActionStepDetailLog(int nActionStepID, bool isRealMode, DateTime dtEnd);

        /// <summary>
        /// MakeActionStepLog(...) 함수를 통하여 만들어진 ActionStepDetailLog 값을 갱신하는 함수.
        /// 마지막 상태값을 Cancel로 바꾼다.
        /// </summary>
        void CancelActionStepDetailLog(int nActionStepID, bool isRealMode, DateTime dtCancel);

        /// <summary>
        /// SOP 동작후 각 Component들의 실행 상태는 DB에 기록하게 되는데, 제어권을 가지지 못한 
        /// SOP Simulator는 Thread나 Timer를 통하여 실시간으로 Component들의 상태를 감시한다.
        /// 이때, 특정 시간에 발생한 Component들의 실행 이력을 AddSOPSectionLog(...) 함수 호출로 기록하게 된다.
        /// </summary>
        /// <param name="nActionStepID">
        /// 연관된 ActionStep의 ID
        /// </param>
        /// <param name="arrComponentHistoryID">
        /// 각 Component들의 ComponentHistoryID
        /// 이 ID에 해당하는 Section들은 arrSections에 담겨있다.
        /// </param>
        /// <param name="arrSections">
        /// 각 Component들의 Section 객체
        /// 각 Section 객체의 ComponentHistoryID는 arrComponentHistoryID에 담겨있다.
        /// </param>
        /// <param name="arrStatus">
        /// 각 Component들의 상태 정보
        /// </param>
        /// <param name="arrProcessDirections">
        /// 각 Component들의 진행 방향
        /// 진행 방향은 Component가 완료 상태로 바뀔때 생기는 것이며 연결된 화살표 가운데 어느 방향으로 진행되는지를 표시한다.
        /// 하나의 Component는 여러번 실행될 수 있으므로 Process Direction은 단일값이 아니라 Bit Flag로 배열값을 가진다.
        /// Process Direction : 1(Top), 2(Right), 4(Bottom), 8(Left)
        /// </param>
        /// <param name="arrTask">
        /// 각 Component들의 임무 정보
        /// </param>
        /// <param name="arrTime">
        /// 각 Component들의 실행 시간
        /// </param>
        /// <param name="arrDescription">
        /// 각 Component들의 실행 내용에 대한 Description
        /// </param>
        /// <param name="arrShowBoard">
        /// 각 Component들의 실행 이력을 상황판 프로그램에 표시할 것인지 여부
        /// </param>
        /// <param name="arrCheckedNotify1">
        /// 각 Component들의 상세 실행 내역에 대한 첫번째 데이터(4Byte 정수) - Bit Flag
        /// 각 실행 내역에 대한 내용은 Component Type에 따라 다름
        /// [내부상황전파]
        /// 1 : Popup Message 실행 여부(1이면 실행)
        /// 2 : 문자 메시지 발송 여부(2이면 발송)
        /// 4 : 방송 실행 여부(4면 방송 실행)
        /// [외부상황전파]
        /// 8 : 문자 메시지 발송 여부(8이면 발송)
        /// 16 : Fax 발송 여부(16이면 발송)
        /// [통합 상황전파]
        /// 1 : 내부, Popup Message 실행 여부(1이면 실행)
        /// 2 : 내부, 문자 메시지 발송 여부(2이면 발송)
        /// 4 : 내부, 방송 실행 여부(4면 방송 실행)
        /// 8 : 외부, 문자 메시지 발송 여부(8이면 발송)
        /// 16 : 외부, Fax 발송 여부(16이면 발송)
        /// [프로세스]
        /// 1 : 첫번째 임무의 문자메시지 발송 여부
        /// 2 : 두번째 임무의 문자메시지 발송 여부
        /// ...
        /// 32 : 32번째 임무의 문자메시지 발송 여부
        /// 32번째 임무까지 기록 가능함
        /// </param>
        /// <param name="arrCheckedNotify2">
        /// 각 Component들의 상세 실행 내역에 대한 두번째 데이터(4Byte 정수) - Bit Flag
        /// 각 실행 내역에 대한 내용은 Component Type에 따라 다름
        /// [프로세스]
        /// 1 : 첫번째 임무의 방송 실행 여부
        /// 2 : 두번째 임무의 방송 실행 여부
        /// ...
        /// 32 : 32번째 임무의 방송 실행 여부
        /// 32번째 임무까지 기록 가능함
        /// </param>
        /// <param name="isRealMode">
        /// SOP가 실제모드로 실행중인지 여부
        /// </param>
        /// <param name="dicDetailDatas">
        /// 특정 ComponentHistory에 대한 상세내용
        /// Key : arrComponentHistoryID에 대한 배열 Index
        /// </param>
        /// <param name="workFlow">
        /// 실행중인 ActionStep에 대한 WorkFlow 객체
        /// </param>
        void AddSOPSectionLog(int nActionStepID, ArrayList arrComponentHistoryID, ArrayList arrSections, ArrayList arrStatus, ArrayList arrProcessDirections, ArrayList arrTask, ArrayList arrTime, ArrayList arrDescription, ArrayList arrShowBoard, ArrayList arrCheckedNotify1, ArrayList arrCheckedNotify2, ArrayList arrCheckedRun, ArrayList arrCheckedComplete, ArrayList arrAccessedUserID, bool isRealMode, Dictionary<int, List<HistorySectionData.DetailData>> dicDetailDatas, WorkFlow workFlow);
        //void AddSOPSectionLog(int nActionStepID, Sections.Section section, int nComponentHistoryID, bool isRealMode, int nStatus, int nProcessDirections, ArrayList arrSections, string strTask, DateTime time, string strDescription, bool showBoard, int nCheckedNotify1, int nCheckedNotify2, Sections.WorkFlow workFlow)
    }
}
