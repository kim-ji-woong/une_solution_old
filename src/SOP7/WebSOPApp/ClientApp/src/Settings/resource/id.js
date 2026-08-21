import ProjectResource from "../../Root/resource/id";

export default class SettingsResource {
    static get ID() {
        return SettingsResource.id[ProjectResource.targetLanguage];
    }

    static id = {
        "ko": {
            menu:
            {
                monitoring3D: "3D 관제 시스템",
                dashboardSet: "대시보드",
                sopSet: "SOP",
                teamEditor: "조직관리"
            },

            monitoring3DMode:
            {
                normal: "일반",
                spread: "초기상황전파관리",
                detection: "센서감지관리",
            },

            shortcutKey:
            {
                sdms: "3D 관제시스템",
                history: "이력",
                sop: "SOP 실행",
                sopMgr: "SOP편집",
                teamEdit: "조직관리",
                dashBoard: "대시보드",
                settings: "설정",
                home: "홈버튼",
                rotation: "즉시회전",
            },

            excelMode:
            {
                building: "건물정보 업데이트",
                group: "건물그룹 정보 업데이트",
                facility: "설비정보 업데이트",
                regularTeam: "조직정보 업데이트"
            },

            facilityType:
            {
                Fire: "화재",
                PSM: "누출",
                ETC: "기타",
                SVMS: "지능형 영상",
            }

        },
        "en": {
            
        },

        
    }

    static reAlarm = {
        ReAlarm: "0",
        NoAlarmTerm: "1",
        NoAlarm: "2",
    }

    static timeUnit = {
        second: "0",
        minute: "1",
        hour: "2",
    }

    static eventInfoDisplayTerm = {
        day: "0",
        week: "1",
        month: "2",
    }

    static ExeSOPMode = {
        false: "0",
        exe: "1",
    }

    static facilityType = {
        Fire: 0,         // 화재
        PSM: 11,        // 누출
        ETC: 21,        // 기타
        SVMS: 900,      // 지능형 영상
    }

    static sopEndMode = {
        end: 0,         // 화재
        confirm: 1,        // 누출
        notEnd: 2,        // 기타
    }

    static messageType = {
        sms: 0,         // 문자
        email: 1,        // 이메일
    }

    static closeMode = {
        cancle: 0,         // 취소 및 닫기
        confirm: 1,        // 저장 및 확인
        afterReload: 2,    // 창이 닫힌 후 셋팅값 저장 
    }

    static moveDisplayAlarm = {
        currentDisplay: "0",    // 현재 화면 유지
        moveAlarm: "1",
        firstAlarm: "2",        // 첫번째 알람 화면으로 이동
        lastAlarm: "3",         // 마지막 알람 화면으로 이동
    }

    static usePoiFocus = {
        off: "false",
        on: "true",
    }

    static usePoiHighlight = {
        off: "false",
        on: "true",
    }

    static turnStart = {
        LastView: "1",
        StandardView: "2",
    }

    static useAlarmTurn = {
        off: "false",
        on: "true",
    }
}