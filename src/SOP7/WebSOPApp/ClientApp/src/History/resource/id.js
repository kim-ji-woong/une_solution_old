import ProjectResource from "../../Root/resource/id";

export default class HistoryResource {
    static get ID() {
        return HistoryResource.id[ProjectResource.targetLanguage];
    }

    static id = {
        "ko": {
            projectName: "이력",

            menu:
            {
                userHistory: "데이터 수정 이력",
                sensorDetectHistory: "센서 감지 이력",
                sensorDetectAnalysis: "센서 감지 분석",
                sopHistory: "SOP 이력",
                spreadHistory: "상황전파 이력",
                detailHistory: "상세보기"
            }
        }
    }
}